// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace App.Metrics.Serialization
{
    public interface IMetricDataSerializer
    {
        T Deserialize<T>(string json);

        string Serialize<T>(T value);
    }
}