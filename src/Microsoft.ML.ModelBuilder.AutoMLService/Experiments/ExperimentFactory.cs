// <copyright file="ExperimentFactory.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Experiments
{
    internal static class ExperimentFactory
    {
        public static IExperiment CreateExperiment(MLContext context, IAutoMLServiceParamater settings)
        {
            IExperiment experiment;
            switch (settings.Scenario)
            {
                case AutoMLSharedServiceConstants.BinaryClassification:
                    experiment = new BinaryClassificationExperiment(context, settings as ILocalAutoMLTrainParameters);
                    break;
                case AutoMLSharedServiceConstants.MulticlassClassification:
                    experiment = new MultiClassificationExperiment(context, settings as ILocalAutoMLTrainParameters);
                    break;
                case AutoMLSharedServiceConstants.Regression:
                    experiment = new RegressionExperiment(context, settings as ILocalAutoMLTrainParameters);
                    break;
                case AutoMLSharedServiceConstants.Recommendation:
                    experiment = new RecommendationExperiment(context, settings as ILocalAutoMLTrainParameters);
                    break;
                case AutoMLSharedServiceConstants.ImageClassification:
                    if (settings.IsAzureTraining)
                    {
                        experiment = new AzureImageClassificationExperiment(context, settings as IRemoteAutoMLTrainParameters, AutoMLServiceLogger.Instance);
                    }
                    else
                    {
                        experiment = new MultiClassificationExperiment(context, settings as ILocalAutoMLTrainParameters);
                    }

                    break;
                default:
                    throw new Exception("Not Implemented");
            }

            return experiment;
        }
    }
}
