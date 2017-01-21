﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.ComponentModel;

namespace App.Metrics.Abstractions
{
    /// <summary>
    ///     Marker interface to cleanup editor visible members on metrics.
    /// </summary>
    /// <remarks>Created by Daniel Cazzulino http://www.clariusconsulting.net/blogs/kzu/archive/2008/03/10/58301.aspx</remarks>
    public interface IHideObjectMembers
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);

        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetType();

        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();
    }
}