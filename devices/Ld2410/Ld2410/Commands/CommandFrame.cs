﻿using Ld2410.Extensions;

using System;

namespace Ld2410.Commands
{
    internal abstract class CommandFrame
    {
        internal static byte[] Header = new byte[4] { 0xFD, 0xFC, 0xFB, 0xFA };
        internal static byte[] End = new byte[4] { 0x04, 0x03, 0x02, 0x01 };

        protected const ushort DataLengthSegmentSize = 2;
        protected const ushort CommandWordLength = 2;

        internal CommandWord Command { get; set; }

        protected byte[] Value { get; set; }

        protected CommandFrame(CommandWord command)
        {
            this.Command = command;
            this.Value = new byte[0];
        }

        /// <summary>
        /// Serializes this frame to its binary format as per the LD2410 specs and writes it to a stream.
        /// </summary>
        internal virtual byte[] Serialize()
        {
            /*
             * The structure of the command frame in the stream is:
             * 
             * FRAME HEADER | FRAME DATA LENGTH | FRAME DATA (COMMAND + VALUE) | FRAME END
             * 
             * All in little-endian.
             * 
             */

            /*
             * The full frame length is: 4 (FRAME HEADER) + 2 (Data Length Segment) + 2 (COMMAND WORD) + X (COMMAND VALUE) + 4 (FRAME END)
             */
            var serializedFrame = new byte[
                Header.Length +
                DataLengthSegmentSize +
                CommandWordLength +
                Value.Length +
                End.Length];

            var serializedFrameCurrentPosition = 0;

            // FRAME HEADER
            Array.Copy(
                sourceArray: Header,
                sourceIndex: 0,
                destinationArray: serializedFrame,
                destinationIndex: 0,
                length: Header.Length);

            serializedFrameCurrentPosition += Header.Length;

            /*
             * FRAME DATA LENGTH = Command Word Length + Command Value Length.
             * 
             * Command Word Length is always 2 Bytes.
             * 
             * The Length value has to be 2 bytes long so we cast to C# Short.
             * 
             */
            var frameDataLength = BitConverter.GetBytes((short)(CommandWordLength + this.Value.Length));
            Array.Copy(
                sourceArray: frameDataLength,
                sourceIndex: 0,
                destinationArray: serializedFrame,
                destinationIndex: serializedFrameCurrentPosition,
                length: frameDataLength.Length
                );

            serializedFrameCurrentPosition += frameDataLength.Length;

            // FRAME DATA: COMMAND
            var commandBytes = ((ushort)Command).ToLittleEndianBytes();
            Array.Copy(
                sourceArray: commandBytes,
                sourceIndex: 0,
                destinationArray: serializedFrame,
                destinationIndex: serializedFrameCurrentPosition,
                length: commandBytes.Length
                );

            serializedFrameCurrentPosition += commandBytes.Length;

            // FRAME DATA: VALUE
            Array.Copy(
                sourceArray: Value,
                sourceIndex: 0,
                destinationArray: serializedFrame,
                destinationIndex: serializedFrameCurrentPosition,
                length: Value.Length
                );

            serializedFrameCurrentPosition += Value.Length;

            // FRAME END
            Array.Copy(
                sourceArray: End,
                sourceIndex: 0,
                destinationArray: serializedFrame,
                destinationIndex: serializedFrameCurrentPosition,
                length: End.Length
                );

            return serializedFrame;
        }
    }
}
