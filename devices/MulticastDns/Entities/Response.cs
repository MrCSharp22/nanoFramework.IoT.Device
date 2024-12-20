﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Iot.Device.MulticastDns.Enum;

namespace Iot.Device.MulticastDns.Entities
{
    /// <summary>
    /// A Multicast DNS Response Message.
    /// </summary>
    public class Response : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response" /> class.
        /// </summary>
        public Response() : base(DnsHeaderFlags.Response)
        {
        }

        /// <summary>
        /// Adds an answer to the Response message.
        /// </summary>
        /// <param name="resource">The answer resource to add.</param>
        public void AddAnswer(Resource resource)
        {
            _answers.Add(resource);
        }
    }
}
