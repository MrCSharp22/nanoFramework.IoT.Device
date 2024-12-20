﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Iot.Device.MulticastDns.Enum;
using Iot.Device.MulticastDns.Package;

namespace Iot.Device.MulticastDns.Entities
{
    /// <summary>
    /// Represents a PTR Record Resource (DNS Resource Type 12).
    /// </summary>
    public class PtrRecord : TargetResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PtrRecord" /> class.
        /// </summary>
        /// <param name="domain">The domain this Record is about.</param>
        /// <param name="targetDomain">The targetDomain which points to the domain.</param>
        /// <param name="ttl">The TTL of this resource.</param>
        public PtrRecord(string domain, string targetDomain, int ttl = 2000) : base(domain, DnsResourceType.PTR, ttl)
            => Target = targetDomain;

        internal PtrRecord(PacketParser packet, string domain, int ttl) : base(domain, DnsResourceType.PTR, ttl)
            => Target = packet.ReadDomain();
    }
}
