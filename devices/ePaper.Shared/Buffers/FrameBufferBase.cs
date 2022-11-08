﻿using System;

using Iot.Device.ePaper.Shared.Primitives;

namespace Iot.Device.ePaper.Shared.Buffers
{
    /// <summary>
    /// Base implementation for a frame buffer class.
    /// </summary>
    public abstract class FrameBufferBase : IFrameBuffer
    {
        private int currentFramePage;
        private int currentFramePageLowerBufferBound;
        private int currentFramePageUpperBufferBound;

        /// <inheritdoc/>>
        public int Height { get; }

        /// <inheritdoc/>>
        public int Width { get; }

        /// <inheritdoc/>>
        public virtual int BitDepth
            => this.ColorFormat switch
            {
                ColorFormat.Color1BitPerPixel => 1,
                ColorFormat.Color2BitPerPixel => 2,

                _ => throw new NotImplementedException()
            };

        /// <inheritdoc/>>
        public virtual byte[] Buffer { get; }

        /// <inheritdoc/>>
        public virtual int BufferByteCount
            => (this.Width * this.Height * this.BitDepth) / 8;

        /// <inheritdoc/>
        public abstract ColorFormat ColorFormat { get; }

        /// <inheritdoc/>
        public virtual Point StartPoint { get; set; }

        /// <inheritdoc/>
        public virtual int CurrentFramePage
        {
            get => this.currentFramePage;
            set
            {
                this.currentFramePage = value;
                this.currentFramePageLowerBufferBound = this.currentFramePage * this.BufferByteCount;
                this.currentFramePageUpperBufferBound = (this.currentFramePage + 1) * this.BufferByteCount;
            }
        }

        /// <inheritdoc/>
        public byte this[int index]
        {
            get => this.Buffer[index];
            set => this.Buffer[index] = value;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FrameBufferBase"/> class.
        /// </summary>
        /// <param name="height">The height of the frame to manage.</param>
        /// <param name="width">The width of the frame to manage.</param>
        protected FrameBufferBase(int height, int width)
        {
            this.Height = height;
            this.Width = width;
            this.Buffer = new byte[this.BufferByteCount];
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FrameBufferBase"/> class by copying the specified buffer.
        /// </summary>
        /// <param name="height">The height of the frame to manage.</param>
        /// <param name="width">The width of the frame to manage.</param>
        /// <param name="buffer">The starting frame buffer.</param>
        protected FrameBufferBase(int height, int width, byte[] buffer)
        {
            this.Height = height;
            this.Width = width;
            this.Buffer = buffer;

            if (buffer.Length != this.BufferByteCount)
                throw new ArgumentException("Length mismatch between the provided buffer and the specified width and height.");
        }

        /// <inheritdoc/>>
        public void Clear()
        {
            Array.Clear(this.Buffer, 0, this.Buffer.Length);
        }

        /// <inheritdoc/>>
        public abstract void Clear(Color color);

        /// <inheritdoc/>>
        public void WriteBuffer(IFrameBuffer buffer)
            => this.WriteBuffer(buffer, Point.Default);

        /// <inheritdoc/>>
        public virtual void WriteBuffer(IFrameBuffer buffer, Point start)
            => this.WriteBufferSlow(buffer, start);

        /// <inheritdoc/>>
        public void Fill(Color color)
            => this.Fill(Point.Default, this.Width, this.Height, color);

        /// <inheritdoc/>>
        public abstract void Fill(Point start, int width, int height, Color color);

        /// <inheritdoc/>>
        public abstract Color GetPixel(Point point);

        /// <inheritdoc/>>
        public abstract void SetPixel(Point point, Color pixelColor);

        /// <inheritdoc/>>
        public virtual bool IsPointWithinFrameBuffer(Point point)
        {
            return point.X >= this.StartPoint.X && point.X < (this.StartPoint.X + this.Width)
                && point.Y >= this.StartPoint.Y && point.Y < (this.StartPoint.Y + this.Height);
        }

        /// <summary>
        /// Gets the index of the byte within the <see cref="Buffer"/> array which contains the specified point.
        /// </summary>
        /// <param name="point">The point to get its index within <see cref="Buffer"/>.</param>
        /// <returns>The index within the <see cref="Buffer"/> for the byte that contains the specified pixe location.</returns>
        protected int GetFrameBufferIndexForPoint(Point point)
            => this.GetFrameBufferIndexForPoint(point.X, point.Y);

        /// <summary>
        /// Gets the index of the byt within the <see cref="Buffer"/> array which contains the specified point.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <returns>The index within the <see cref="Buffer"/> for the byte that contains the specified pixe location.</returns>
        protected int GetFrameBufferIndexForPoint(int x, int y)
            => ((x + (y * this.Width)) / 8) - this.currentFramePageLowerBufferBound;

        /// <summary>
        /// Gets a byte mask pattern for the specified point.
        /// </summary>
        /// <param name="point">The point within the frame buffer.</param>
        /// <returns>A byte mask pattern with the pixel bit flipped to 1.</returns>
        protected byte GetPointByteMask(Point point)
            => (byte)(128 >> (point.X & 7));

        /// <summary>
        /// Copies the specified <see cref="IFrameBuffer"/> to this instance by iterating every pixel.
        /// This can be a very slow operation but useful for when copying frames with incompatible bit depth.
        /// </summary>
        /// <param name="buffer">The buffer to copy from.</param>
        /// <param name="start">The starting point to copy from and write to.</param>
        protected virtual void WriteBufferSlow(IFrameBuffer buffer, Point start)
        {
            for (var x = start.X; x < buffer.Width; x++)
            {
                for (var y = start.Y; y < buffer.Height; y++)
                {
                    var currentPoint = new Point(x, y);
                    this.SetPixel(currentPoint, buffer.GetPixel(currentPoint));
                }
            }
        }
    }
}
