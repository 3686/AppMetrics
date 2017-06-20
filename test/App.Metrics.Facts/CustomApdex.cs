﻿// <copyright file="CustomApdex.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Apdex;
using App.Metrics.Core.Apdex;

namespace App.Metrics.Facts
{
    public class CustomApdex : IApdex
    {
        public long CurrentTime() { return 0; }

        public long EndRecording() { return 0; }

        public ApdexContext NewContext() { return new ApdexContext(new CustomApdex()); }

        public void Reset() { }

        public long StartRecording() { return 0; }

        public void Track(long duration) { }

        public void Track(Action action) { }

        public T Track<T>(Func<T> action) { return action(); }
    }
}