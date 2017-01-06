﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using App.Metrics.Core.Interfaces;

namespace App.Metrics
{
    public struct ApdexContext : IDisposable
    {
        private readonly long _start;
        private IApdex _apdex;

        public ApdexContext(IApdex apdex)
        {
            _start = apdex.StartRecording();
            _apdex = apdex;
        }

        /// <summary>
        ///     Provides the currently elapsed time from when the instance has been created
        /// </summary>
        public TimeSpan Elapsed
        {
            get
            {
                if (_apdex == null)
                {
                    return TimeSpan.Zero;
                }
                var milliseconds = TimeUnit.Nanoseconds.Convert(TimeUnit.Milliseconds, _apdex.CurrentTime() - _start);
                return TimeSpan.FromMilliseconds(milliseconds);
            }
        }

        public void Dispose()
        {
            if (_apdex == null) return;

            var end = _apdex.EndRecording();

            var duration = end - _start;

            _apdex.Track(duration);

            _apdex = null;
        }
    }
}