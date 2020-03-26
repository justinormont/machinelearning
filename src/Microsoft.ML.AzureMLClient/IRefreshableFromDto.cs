// <copyright file="IRefreshableFromDto.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Azure.MachineLearning.Services
{
    public interface IRefreshableFromDto<T>
    {
        DateTime LastRefreshFromDto { get; }

        void RefreshFromDto(T dto);
    }
}
