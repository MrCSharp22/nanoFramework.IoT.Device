// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// File Automatically Generated by Bitmap2Font

using System;

namespace Iot.Device.Ssd13xx
{
    /// <summary>
    /// Sinclair8x8 font.
    /// </summary>
    public class Sinclair8x8 : IFont
    {
        private static readonly byte[][] _fontTable =
        {
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new byte[] { 0x10, 0x10, 0x10, 0x10, 0x10, 0x00, 0x10, 0x00 },
            new byte[] { 0x28, 0x28, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new byte[] { 0x00, 0x24, 0x7E, 0x24, 0x24, 0x7E, 0x24, 0x00 },
            new byte[] { 0x08, 0x3E, 0x0A, 0x3E, 0x28, 0x3E, 0x08, 0x00 },
            new byte[] { 0x00, 0x46, 0x26, 0x10, 0x08, 0x64, 0x62, 0x00 },
            new byte[] { 0x00, 0x08, 0x14, 0x08, 0x54, 0x22, 0x5C, 0x00 },
            new byte[] { 0x00, 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new byte[] { 0x00, 0x10, 0x08, 0x08, 0x08, 0x08, 0x10, 0x00 },
            new byte[] { 0x00, 0x08, 0x10, 0x10, 0x10, 0x10, 0x08, 0x00 },
            new byte[] { 0x00, 0x00, 0x14, 0x08, 0x3E, 0x08, 0x14, 0x00 },
            new byte[] { 0x00, 0x00, 0x08, 0x08, 0x3E, 0x08, 0x08, 0x00 },
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x10, 0x08 },
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x3E, 0x00, 0x00, 0x00 },
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x18, 0x18, 0x00 },
            new byte[] { 0x00, 0x00, 0x20, 0x10, 0x08, 0x04, 0x02, 0x00 },
            new byte[] { 0x00, 0x1E, 0x31, 0x29, 0x25, 0x23, 0x1E, 0x00 },
            new byte[] { 0x00, 0x06, 0x05, 0x04, 0x04, 0x04, 0x1F, 0x00 },
            new byte[] { 0x00, 0x1E, 0x21, 0x20, 0x1E, 0x01, 0x3F, 0x00 },
            new byte[] { 0x00, 0x1E, 0x21, 0x18, 0x20, 0x21, 0x1E, 0x00 },
            new byte[] { 0x00, 0x08, 0x0C, 0x0A, 0x09, 0x3F, 0x08, 0x00 },
            new byte[] { 0x00, 0x3F, 0x01, 0x1F, 0x20, 0x21, 0x1E, 0x00 },
            new byte[] { 0x00, 0x1E, 0x01, 0x1F, 0x21, 0x21, 0x1E, 0x00 },
            new byte[] { 0x00, 0x3F, 0x20, 0x10, 0x08, 0x04, 0x04, 0x00 },
            new byte[] { 0x00, 0x1E, 0x21, 0x1E, 0x21, 0x21, 0x1E, 0x00 },
            new byte[] { 0x00, 0x1E, 0x21, 0x21, 0x3E, 0x20, 0x1E, 0x00 },
            new byte[] { 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x08, 0x00 },
            new byte[] { 0x00, 0x00, 0x08, 0x00, 0x00, 0x08, 0x08, 0x04 },
            new byte[] { 0x00, 0x00, 0x10, 0x08, 0x04, 0x08, 0x10, 0x00 },
            new byte[] { 0x00, 0x00, 0x00, 0x3E, 0x00, 0x3E, 0x00, 0x00 },
            new byte[] { 0x00, 0x00, 0x04, 0x08, 0x10, 0x08, 0x04, 0x00 },
            new byte[] { 0x00, 0x3C, 0x42, 0x20, 0x10, 0x00, 0x10, 0x00 },
            new byte[] { 0x00, 0x3C, 0x52, 0x6A, 0x7A, 0x02, 0x3C, 0x00 },
            new byte[] { 0x00, 0x1E, 0x21, 0x21, 0x3F, 0x21, 0x21, 0x00 },
            new byte[] { 0x00, 0x1F, 0x21, 0x1F, 0x21, 0x21, 0x1F, 0x00 },
            new byte[] { 0x00, 0x1E, 0x21, 0x01, 0x01, 0x21, 0x1E, 0x00 },
            new byte[] { 0x00, 0x0F, 0x11, 0x21, 0x21, 0x11, 0x0F, 0x00 },
            new byte[] { 0x00, 0x3F, 0x01, 0x1F, 0x01, 0x01, 0x3F, 0x00 },
            new byte[] { 0x00, 0x3F, 0x01, 0x1F, 0x01, 0x01, 0x01, 0x00 },
            new byte[] { 0x00, 0x1E, 0x21, 0x01, 0x39, 0x21, 0x1E, 0x00 },
            new byte[] { 0x00, 0x21, 0x21, 0x3F, 0x21, 0x21, 0x21, 0x00 },
            new byte[] { 0x00, 0x3E, 0x08, 0x08, 0x08, 0x08, 0x3E, 0x00 },
            new byte[] { 0x00, 0x20, 0x20, 0x20, 0x21, 0x21, 0x1E, 0x00 },
            new byte[] { 0x00, 0x11, 0x09, 0x07, 0x09, 0x11, 0x21, 0x00 },
            new byte[] { 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x3F, 0x00 },
            new byte[] { 0x00, 0x21, 0x33, 0x2D, 0x21, 0x21, 0x21, 0x00 },
            new byte[] { 0x00, 0x21, 0x23, 0x25, 0x29, 0x31, 0x21, 0x00 },
            new byte[] { 0x00, 0x1E, 0x21, 0x21, 0x21, 0x21, 0x1E, 0x00 },
            new byte[] { 0x00, 0x1F, 0x21, 0x21, 0x1F, 0x01, 0x01, 0x00 },
            new byte[] { 0x00, 0x1E, 0x21, 0x21, 0x25, 0x29, 0x1E, 0x00 },
            new byte[] { 0x00, 0x1F, 0x21, 0x21, 0x1F, 0x11, 0x21, 0x00 },
            new byte[] { 0x00, 0x1E, 0x01, 0x1E, 0x20, 0x21, 0x1E, 0x00 },
            new byte[] { 0x00, 0x7F, 0x08, 0x08, 0x08, 0x08, 0x08, 0x00 },
            new byte[] { 0x00, 0x21, 0x21, 0x21, 0x21, 0x21, 0x1E, 0x00 },
            new byte[] { 0x00, 0x21, 0x21, 0x21, 0x21, 0x12, 0x0C, 0x00 },
            new byte[] { 0x00, 0x21, 0x21, 0x21, 0x21, 0x2D, 0x12, 0x00 },
            new byte[] { 0x00, 0x21, 0x12, 0x0C, 0x0C, 0x12, 0x21, 0x00 },
            new byte[] { 0x00, 0x41, 0x22, 0x14, 0x08, 0x08, 0x08, 0x00 },
            new byte[] { 0x00, 0x3F, 0x10, 0x08, 0x04, 0x02, 0x3F, 0x00 },
            new byte[] { 0x00, 0x1C, 0x04, 0x04, 0x04, 0x04, 0x1C, 0x00 },
            new byte[] { 0x00, 0x00, 0x02, 0x04, 0x08, 0x10, 0x20, 0x00 },
            new byte[] { 0x00, 0x1C, 0x10, 0x10, 0x10, 0x10, 0x1C, 0x00 },
            new byte[] { 0x00, 0x08, 0x1C, 0x2A, 0x08, 0x08, 0x08, 0x00 },
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7F },
            new byte[] { 0x3C, 0x42, 0x99, 0x85, 0x85, 0x99, 0x42, 0x3C },
            new byte[] { 0x00, 0x00, 0x1C, 0x20, 0x3C, 0x22, 0x3C, 0x00 },
            new byte[] { 0x00, 0x02, 0x02, 0x1E, 0x22, 0x22, 0x1E, 0x00 },
            new byte[] { 0x00, 0x00, 0x38, 0x04, 0x04, 0x04, 0x38, 0x00 },
            new byte[] { 0x00, 0x20, 0x20, 0x3C, 0x22, 0x22, 0x3C, 0x00 },
            new byte[] { 0x00, 0x00, 0x1C, 0x22, 0x1E, 0x02, 0x3C, 0x00 },
            new byte[] { 0x00, 0x30, 0x08, 0x18, 0x08, 0x08, 0x08, 0x00 },
            new byte[] { 0x00, 0x00, 0x7C, 0x42, 0x42, 0x7C, 0x40, 0x3C },
            new byte[] { 0x00, 0x02, 0x02, 0x1E, 0x22, 0x22, 0x22, 0x00 },
            new byte[] { 0x00, 0x10, 0x00, 0x18, 0x10, 0x10, 0x38, 0x00 },
            new byte[] { 0x00, 0x20, 0x00, 0x20, 0x20, 0x20, 0x24, 0x18 },
            new byte[] { 0x00, 0x02, 0x0A, 0x06, 0x06, 0x0A, 0x12, 0x00 },
            new byte[] { 0x00, 0x08, 0x08, 0x08, 0x08, 0x08, 0x30, 0x00 },
            new byte[] { 0x00, 0x00, 0x16, 0x2A, 0x2A, 0x2A, 0x2A, 0x00 },
            new byte[] { 0x00, 0x00, 0x1E, 0x22, 0x22, 0x22, 0x22, 0x00 },
            new byte[] { 0x00, 0x00, 0x1C, 0x22, 0x22, 0x22, 0x1C, 0x00 },
            new byte[] { 0x00, 0x00, 0x1E, 0x22, 0x22, 0x1E, 0x02, 0x02 },
            new byte[] { 0x00, 0x00, 0x3C, 0x22, 0x22, 0x3C, 0x20, 0x60 },
            new byte[] { 0x00, 0x00, 0x38, 0x04, 0x04, 0x04, 0x04, 0x00 },
            new byte[] { 0x00, 0x00, 0x1C, 0x02, 0x1C, 0x20, 0x1E, 0x00 },
            new byte[] { 0x00, 0x08, 0x1C, 0x08, 0x08, 0x08, 0x30, 0x00 },
            new byte[] { 0x00, 0x00, 0x22, 0x22, 0x22, 0x22, 0x1C, 0x00 },
            new byte[] { 0x00, 0x00, 0x22, 0x22, 0x14, 0x14, 0x08, 0x00 },
            new byte[] { 0x00, 0x00, 0x22, 0x2A, 0x2A, 0x2A, 0x14, 0x00 },
            new byte[] { 0x00, 0x00, 0x22, 0x14, 0x08, 0x14, 0x22, 0x00 },
            new byte[] { 0x00, 0x00, 0x22, 0x22, 0x22, 0x3C, 0x20, 0x1C },
            new byte[] { 0x00, 0x00, 0x3E, 0x10, 0x08, 0x04, 0x3E, 0x00 },
            new byte[] { 0x00, 0x38, 0x08, 0x06, 0x08, 0x08, 0x38, 0x00 },
            new byte[] { 0x00, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x00 },
            new byte[] { 0x00, 0x0E, 0x08, 0x30, 0x08, 0x08, 0x0E, 0x00 },
            new byte[] { 0x00, 0x28, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new byte[] { 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
        };

        /// <inheritdoc/>
        public override byte Width { get => 8; }

        /// <inheritdoc/>
        public override byte Height { get => 8; }

        /// <inheritdoc/>
        public override byte[] this[char character]
        {
            get
            {
                var index = (byte)character;
                if ((index < 32) || (index > 127))
                {
                    return _fontTable[32];
                }
                else
                {
                    return _fontTable[index - 32];
                }
            }
        }
    }
}