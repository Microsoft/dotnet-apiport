﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace ApiPortVS
{
    public class TargetPlatformVersion
    {
        public TargetPlatformVersion()
        {
        }

        public string PlatformName { get; set; }

        public bool IsSelected { get; set; }

        public Version Version { get; set; }


        public override string ToString()
        {
            if (Version == null)
            {
                return PlatformName;
            }
            else
            {
                return String.Format("{0}, Version={1}", PlatformName, Version);
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as TargetPlatformVersion;

            if (other == null)
            {
                return false;
            }

            return String.Equals(PlatformName, other.PlatformName, StringComparison.OrdinalIgnoreCase)
                && Version == other.Version;
        }

        public override int GetHashCode()
        {
            const int HashMultipler = 31;

            unchecked
            {
                int hash = 17;

                hash = hash * HashMultipler + PlatformName.GetHashCode();
                hash = hash * HashMultipler + Version.GetHashCode();

                return hash;
            }
        }
    }
}
