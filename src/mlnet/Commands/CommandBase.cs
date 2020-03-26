// <copyright file="CommandBase.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.ML.CLI.Commands
{


    internal abstract class CommandBase
    {
        public abstract string CommandName { get; }

        public abstract string Description { get; }

        public virtual System.CommandLine.Command ToCommand(ICommandHandler commandHandler = null)
        {
            // Get all properties that has CommandArgument Attributes
            var propertyInfos = this.GetType().GetProperties();
            var commandArguments = propertyInfos.Where(x =>
            {
                return System.Attribute.GetCustomAttribute(x, typeof(CommandArgumentAttribute)) != null;
            });

            var command = new System.CommandLine.Command(this.CommandName, this.Description);
            command.Handler = commandHandler;
            command.TreatUnmatchedTokensAsErrors = true;

            // Add Group Validator
            this.AddRequiredValidator(command, propertyInfos);

            var getCreateArgumentMethod = typeof(CommandBase).GetMethod(nameof(CommandBase.CreateArgument), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var param in commandArguments)
            {
                var getGeneraicCreateArgumentMethod = getCreateArgumentMethod.MakeGenericMethod(param.PropertyType);
                var paramArgument = System.Attribute.GetCustomAttribute(param, typeof(CommandArgumentAttribute)) as CommandArgumentAttribute;

                // Add Required Validator
                if (paramArgument.Required)
                {
                    command.AddValidator((sym) =>
                    {
                        if (!sym.Children.Contains(paramArgument.Alias[0]))
                        {
                            return $"Option required: {paramArgument.Alias[0]}";
                        }

                        return null;
                    });
                }

                var paramType = param.PropertyType;
                dynamic argument = getGeneraicCreateArgumentMethod.Invoke(this, new object[] { paramArgument });

                var option = new Option(paramArgument.Alias.ToArray(), paramArgument.HelperText);
                option.Argument = argument;

                command.AddOption(option);
            }

            // Add Validator
            command.AddValidator(this.Validator);

            return command;
        }

        protected virtual string Validator(CommandResult result)
        {
            return null;
        }

        private Argument<T> CreateArgument<T>(CommandArgumentAttribute attr)
        {
            var argument = new Argument<T>();

            if (typeof(IEnumerable<int>).IsAssignableFrom(typeof(T)))
            {
                argument = new Argument<IEnumerable<int>>(result =>
                {
                    return result.Tokens.Select(t => int.Parse(t.Value)).ToList();
                }) as Argument<T>;
            }
            else if (typeof(IEnumerable<string>).IsAssignableFrom(typeof(T)))
            {
                argument = new Argument<IEnumerable<string>>(result =>
                {
                    return result.Tokens.Select(t => t.Value).ToList();
                }) as Argument<T>;
            }
            else if (typeof(T) == typeof(FileInfo))
            {
                argument.Arity = ArgumentArity.ExactlyOne;
                argument.AddValidator(symbol =>
                                      symbol.Tokens
                                            .Select(t => t.Value)
                                            .Where(filePath => !File.Exists(filePath))
                                            .Select((filePath) => $"File or Directory does not exist: {filePath}")
                                            .FirstOrDefault());
            }
            else
            {
                argument.Arity = ArgumentArity.ExactlyOne;
            }

            if (attr.DefaultValue != null)
            {
                argument.SetDefaultValue(attr.DefaultValue);
            }

            return argument;
        }

        private void AddRequiredValidator(System.CommandLine.Command command, PropertyInfo[] propertyInfos)
        {
            Func<PropertyInfo, bool> isRequiredArgument = x => System.Attribute.GetCustomAttribute(x, typeof(GroupAttribute)) != null;
            Func<PropertyInfo, GroupAttribute> getRequiredArgument = x => (System.Attribute.GetCustomAttribute(x, typeof(GroupAttribute)) as GroupAttribute);
            Func<PropertyInfo, CommandArgumentAttribute> getCommandArgument = x => (System.Attribute.GetCustomAttribute(x, typeof(CommandArgumentAttribute)) as CommandArgumentAttribute);

            var groupPropertyInfos = propertyInfos
                    .Where(x => isRequiredArgument(x))
                    .GroupBy(x => getRequiredArgument(x).ID); // group by RequiredArgument.ID

            foreach (var groupPropertyInfo in groupPropertyInfos)
            {
                var requiredArgList = groupPropertyInfo.Select(x => getRequiredArgument(x));
                var commandArgList = groupPropertyInfo.Select(x => getCommandArgument(x));

                // TODO
                // Check if required is the same in the same group
                var required = requiredArgList.First().Required;
                command.AddValidator((sym) =>
                {
                    var aliasList = commandArgList.Select(x => x.Alias[0]);
                    var aliasContainedCount = aliasList
                                             .Select(x => sym.Children.Contains(x))
                                             .Where(x => x)
                                             .Count();
                    if ( aliasContainedCount > 1)
                    {
                        return $"The following options are mutually exclusive please provide only one :{string.Join(',', aliasList)}";
                    }

                    if (required && aliasContainedCount != 1)
                    {
                        return $"One of the options required: {string.Join(',', aliasList)}";
                    }
                    else
                    {
                        return null;
                    }
                });
            }
        }
    }
}
