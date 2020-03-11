// <copyright file="IAutoMLServiceParamater.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Contract
{
    public interface IAutoMLServiceParamater: ILocalAutoMLTrainParameters, IRemoteAutoMLTrainParameters
    {
    }
}
