﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace App.Metrics.Serialization.Interfaces
{
    public interface IHealthStatusSerializer
    {
        T Deserialize<T>(string json);

        string Serialize<T>(T value);
    }
}