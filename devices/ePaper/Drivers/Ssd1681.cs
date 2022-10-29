﻿using System;
using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;

using Iot.Device.ePaperGraphics;

namespace Iot.Device.ePaper.Drivers
{
    public sealed class Ssd1681 : IColoredEPaperDisplay
    {
        private const byte Black = 0x00;
        private const byte White = 0xff;
        private const int PagesPerFrame = 5;

        private readonly GpioPin resetPin;
        private readonly GpioPin busyPin;
        private readonly GpioPin dataCommandPin;
        private readonly SpiDevice spiDevice;

        private byte[] blackAndWhiteFrameBuffer;
        private byte[] redFrameBuffer;
        private int currentFrameBufferPage;
        private int currentFrameBufferPageLowerBound;
        private int currentFrameBufferPageUpperBound;
        private int currentFrameBufferStartXPosition;
        private int currentFrameBufferStartYPosition;
        private bool disposed;

        /// <summary>
        /// Gets the width of the display.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the height of the display.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the current power state of the display panel.
        /// </summary>
        public PowerState PowerState { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="Ssd1681"/> class.
        /// </summary>
        /// <param name="resetPin">The reset GPIO pin.</param>
        /// <param name="busyPin">The busy GPIO pin.</param>
        /// <param name="dataCommandPin">The data/command GPIO pin.</param>
        /// <param name="spiDevice">The SPI Device used to communicate with the display.</param>
        /// <param name="width">The width of the display.</param>
        /// <param name="height">The height of the display.</param>
        /// <param name="enableFramePaging">Page the frame buffer and all operations to use less memory.</param>
        /// <remarks>
        /// For a 200x200 SSD1681 display, a full Frame requires about 5KB of RAM ((200 * 200) / 8). SSD1681 has 2 RAMs for B/W and Red pixel.
        /// This means to have a full frame in memory, you need about 10KB of RAM. If you can't guarantee 10KB to be available to the driver
        /// then enable paging by setting <paramref name="enableFramePaging"/> to true. A page uses about 2KB (1KB for B/W and 1KB for Red).
        /// </remarks>
        public Ssd1681(GpioPin resetPin, GpioPin busyPin, GpioPin dataCommandPin, 
            SpiDevice spiDevice, int width, int height, bool enableFramePaging = true)
        {
            if (width <= 0 || width > 200)
                throw new ArgumentOutOfRangeException(nameof(width), "Display width can't be less than 0 or greater than 200");

            if (height <= 0 || height > 200)
                throw new ArgumentOutOfRangeException(nameof(height), "Display height can't be less than 0 or greater than 200");

            this.resetPin = resetPin;
            this.busyPin = busyPin;
            this.dataCommandPin = dataCommandPin;
            this.spiDevice = spiDevice;

            var bufferSize = (width * height) / 8;
            var pageSize = bufferSize / PagesPerFrame;

            this.blackAndWhiteFrameBuffer = this.CreateBuffer(enableFramePaging ? pageSize : bufferSize, White);
            this.redFrameBuffer = this.CreateBuffer(enableFramePaging ? pageSize : bufferSize, Black); // defaulting red buffer to 0x00

            this.currentFrameBufferPage = 0;
            this.CalculateFrameBufferPageBounds();

            this.Width = width;
            this.Height = height;

            this.PowerState = PowerState.Unknown;
        }

        /// <summary>
        /// Performs the required steps to "power on" the display.
        /// </summary>
        public void PowerOn()
        {
            this.HardwareReset();
            this.SoftwareReset();

            this.PowerState = PowerState.PoweredOn;
        }

        /// <summary>
        /// Puts the display to sleep using the specified <see cref="SleepMode"/>.
        /// </summary>
        /// <param name="sleepMode">The sleep mode to use when powering down the display.</param>
        public void PowerDown(SleepMode sleepMode = SleepMode.Normal)
        {
            this.SendCommand((byte)Command.DeepSleepMode);
            this.SendData((byte)sleepMode);

            this.PowerState = PowerState.PoweredOff;
        }

        /// <summary>
        /// Perform the required initialization steps to set up the display.
        /// </summary>
        public void Initialize()
        {
            if (this.PowerState != PowerState.PoweredOn)
                throw new InvalidOperationException("Unable to initialize the display until it has been powered on. Call PowerOn() first.");

            // set gate lines and scanning sequence
            this.SendCommand((byte)Command.DriverOutputControl);
            this.SendData((byte)(this.Height - 1), 0x00, 0x00); // refer to spec for description

            // Set data entry sequence
            this.SendCommand((byte)Command.DataEntryModeSetting);
            this.SendData(0x03); // Y Increment, X Increment with RAM address counter auto updated in the X direction.

            // Set RAM X start / end position
            this.SendCommand((byte)Command.SetRAMAddressXStartEndPosition);
            this.SendData(/* Start at 0*/ 0x00, 
                          /* End */ (byte)(this.Width / 8 - 1)); // end at width bits converted to bytes (starts @ 0)

            // Set RAM Y start / end positon
            this.SendCommand((byte)Command.SetRAMAddressYStartEndPosition);
            this.SendData(/* Start at 0 */ 0x00, 0x00, /* End */ (byte)(this.Height - 1), 0x00);

            // Set Panel Border
            this.SendCommand((byte)Command.BorderWaveformControl);
            this.SendData(0xc0); // was 0x05

            // Set Temperature sensor
            this.SendCommand((byte)Command.TempSensorControlSelection);
            this.SendData(0x80); // use internal temp sensor

            // Do a full refresh of the display
            this.PerformFullRefresh();
        }


        /// <summary>
        /// Perform full panel refresh sequence.
        /// </summary>
        public void PerformFullRefresh()
        {
            this.SendCommand((byte)Command.BoosterSoftStartControl);
            this.SendData(0x8b, 0x9c, 0x96, 0x0f);

            this.SendCommand((byte)Command.DisplayUpdateControl2);
            this.SendData((byte)RefreshMode.FullRefresh); // Display Mode 1 (Full Refresh)

            this.SendCommand((byte)Command.MasterActivation);
            this.WaitReady();
        }

        /// <summary>
        /// Perform partial panel refresh sequence.
        /// </summary>
        public void PerformPartialRefresh()
        {
            this.SendCommand((byte)Command.BoosterSoftStartControl);
            this.SendData(0x8b, 0x9c, 0x96, 0x0f);

            this.SendCommand((byte)Command.DisplayUpdateControl2);
            this.SendData((byte)RefreshMode.PartialRefresh); // Display Mode 2 (Partial Refresh)

            this.SendCommand((byte)Command.MasterActivation);
            this.WaitReady();
        }



        /// <summary>
        /// Sets the 'cursor' position within the display's RAM.
        /// </summary>
        /// <param name="x">X location within the display's RAM.</param>
        /// <param name="y">Y location within the display's RAM.</param>
        public void SetPosition(int x, int y)
        {
            this.EnforceBounds(ref x, ref y);

            this.SendCommand((byte)Command.SetRAMAddressCounterX);
            this.SendData((byte)(x / 8)); // each row can have up to 200 points each represented by a single bit.

            this.SendCommand((byte)Command.SetRAMAddressCounterY);
            this.SendData((byte)y);
        }

        /// <summary>
        /// Clears the display.
        /// </summary>
        public void Clear()
        {
            //this.SetFrameBufferPage(page: 0);

            //if (!refreshDisplay)
            //    return;

            //var currentPositionY = 0;
            //for (var i = 0; i < PagesPerFrame; i++)
            //{
            //    this.DrawBuffer(this.blackAndWhiteFrameBuffer, startXPos: 0, startYPos: currentPositionY);
            //}

            //this.SetPosition(x: 0, y: 0);
        }

        /// <summary>
        /// Draws a single pixel to the appropriate frame buffer based on the specified color.
        /// </summary>
        /// <param name="x">The X Position</param>
        /// <param name="y">The Y Position</param>
        /// <param name="color">Pixel color. See the remarks for how a buffer is selected based on the color value.</param>
        /// <remarks>
        /// The SSD1681 comes with 2 RAMs: a Black and White RAM and a Red RAM.
        /// Writing to the B/W RAM draws B/W pixels on the panel. While writing to the Red RAM draws red pixels on the panel (if the panel supports red).
        /// However, the SSD1681 doesn't support specifying the color level (no grayscaling), therefore the way the buffer is selected 
        /// is by performing a simple binary check: 
        /// if R >= 128 && G == 0 && B == 0 then write a red pixel to the Red Buffer/RAM
        /// if R == 0 && G == 0 && B == 0 then write a black pixel to B/W Buffer/RAM
        /// else, assume white pixel and write to B/W Buffer/RAM.
        /// </remarks>
        public void DrawPixel(int x, int y, Color color)
        {
            // ignore out of bounds draw attempts
            if (x < 0 || x >= this.Width || y < 0 || y >= this.Height)
            {
                return;
            }

            var frameByteIndex = this.GetFrameBufferIndex(x, y);
            var pageByteIndex = frameByteIndex - this.currentFrameBufferPageLowerBound;

            // if the specified point falls in the current page, update the buffer
            if (this.currentFrameBufferPageLowerBound < frameByteIndex 
                && frameByteIndex < this.currentFrameBufferPageUpperBound)
            {
                /*
                 * Lookup Table for colors on SSD1681
                 * 
                 *  LUT for Red, Black, and White ePaper display
                 * 
                 * |                |               |                    |
                 * | Data Red RAM   | Data B/W RAM  | Result Pixel Color |
                 * |----------------|---------------|--------------------|
                 * |        0       |       0       |       Black        |
                 * |        0       |       1       |       White        |
                 * |        1       |       0       |       Red          |
                 * |        1       |       1       |       Red          |
                 * 
                 * 
                 *  LUT for Black and White ePaper display with SSD1681
                 * |                |               |                    |
                 * | Data Red RAM   | Data B/W RAM  | Result Pixel Color |
                 * |----------------|---------------|--------------------|
                 * |        0       |       0       |       Black        |
                 * |        0       |       1       |       White        |
                 * |        1       |       0       |       Black        |
                 * |        1       |       1       |       White        |
                 */

                if (color.R >= 128 && color.G == 0 && color.B == 0) // is a colored pixel
                {
                    // according to LUT, no need to change the pixel value in B/W buffer.
                    // red frame buffer starts with 0x00. ORing it to set red pixel to 1.
                    this.redFrameBuffer[pageByteIndex] |= (byte)(128 >> (x & 7));
                }
                else if (color.R == 0 && color.G == 0 && color.B == 0) // black pixel
                {
                    this.blackAndWhiteFrameBuffer[pageByteIndex] &= (byte)~(128 >> (x & 7));
                }
                else // assume white if R, G, and B > 0
                {
                    this.blackAndWhiteFrameBuffer[pageByteIndex] |= (byte)(128 >> (x & 7));
                }
            }
        }

        /// <summary>
        /// Draws the specified buffer directly to the Black/White RAM on the display.
        /// Call <see cref="PerformFullRefresh"/> after to update the display.
        /// </summary>
        /// <param name="buffer">The buffer array to draw.</param>
        /// <param name="startXPos">X start position.</param>
        /// <param name="startYPos">Y start position.</param>
        public void DrawBuffer(byte[] buffer, int startXPos, int startYPos)
        {
            this.SetPosition(startXPos, startYPos);

            this.SendCommand((byte)Command.WriteBackWhiteRAM);
            this.SendData(buffer);

            this.SetFrameBufferPage(page: 0);
        }

        /// <summary>
        /// Draws the specified buffer directly to the Red RAM on the display.
        /// Call <see cref="PerformFullRefresh"/> after to update the display.
        /// </summary>
        /// <param name="buffer">The buffer array to draw.</param>
        /// <param name="startXPos">X start position.</param>
        /// <param name="startYPos">Y start position.</param>
        public void DrawColorBuffer(byte[] buffer, int startXPos, int startYPos)
        {
            this.SetPosition(startXPos, startYPos);

            this.SendCommand((byte)Command.WriteRedRAM);
            this.SendData(buffer);

            this.SetFrameBufferPage(page: 0);
        }

        /// <summary>
        /// Begins a frame draw operation with frame paging support.
        /// <code>
        /// SSD1681.BeginFrameDraw();
        /// do {
        ///     // Drawing calls
        /// } while (SSD1681.NextFramePage());
        /// SSD1681.EndFrameDraw();
        /// </code>
        /// </summary>
        public void BeginFrameDraw()
        {
            // make sure we start from the first page with clear buffers
            this.SetFrameBufferPage(page: 0);
        }

        /// <summary>
        /// Moves the current buffers to the next frame page and returns true if successful.
        /// </summary>
        /// <returns>True if the next frame page is available and the internal buffers have moved to it, otherwise; false.</returns>
        public bool NextFramePage()
        {
            if (this.currentFrameBufferPage < PagesPerFrame - 1)
            {
                this.WriteInternalBuffersToDevice();

                this.currentFrameBufferPage++;
                this.SetFrameBufferPage(this.currentFrameBufferPage);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Ends the frame draw and flushes the current page to the device.
        /// </summary>
        public void EndFrameDraw()
        {
            this.WriteInternalBuffersToDevice();

            this.PerformFullRefresh();
            this.SetFrameBufferPage(page: 0);
        }


        /// <summary>
        /// Sends a command to the <see cref="SpiDevice"/>.
        /// </summary>
        /// <param name="command">The command's byte(s) to send.</param>
        public void SendCommand(params byte[] command)
        {
            // make sure we are setting data/command pin to low (command mode)
            this.dataCommandPin.Write(PinValue.Low);

            foreach (var b in command)
            {
                // write the command byte
                spiDevice.WriteByte(b);
            }
        }

        /// <summary>
        /// Sends data to the <see cref="SpiDevice"/>.
        /// </summary>
        /// <param name="data">The data byte(s) to send.</param>
        public void SendData(params byte[] data)
        {
            // set the data/command pin to high to indicate to the display we will be sending data
            this.dataCommandPin.Write(PinValue.High);

            foreach (var @byte in data)
            {
                this.spiDevice.WriteByte(@byte);
            }

            // go back to low (command mode)
            this.dataCommandPin.Write(PinValue.Low);
        }

        /// <summary>
        /// Waits for the busy pin on the display to read <see cref="PinValue.Low"/> (which indicates the display is ready) before unblocking execution.
        /// </summary>
        public void WaitReady()
        {
            while (this.busyPin.Read() == PinValue.High)
            {
                this.WaitMs(5);
            }
        }



        private void HardwareReset()
        {
            // specs say to wait 10ms after supplying voltage to the display
            this.WaitMs(10);

            this.resetPin.Write(PinValue.Low);
            this.WaitMs(200);

            this.resetPin.Write(PinValue.High);
            this.WaitMs(200);
        }

        private void SoftwareReset()
        {
            this.SendCommand((byte)(byte)Command.SoftwareReset);

            this.WaitReady();
            this.WaitMs(10);
        }

        private void WaitMs(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        private void EnforceBounds(ref int x, ref int y)
        {
            x = x < 0 || x > this.Width - 1 ? 0 : x;
            y = y < 0 || y > this.Height - 1 ? 0 : y;
        }

        private int GetFrameBufferIndex(int x, int y)
        {
            // x specifies the column
            return (x + (y * this.Width)) / 8;
        }

        private int GetXPositionFromFrameBufferIndex(int index)
        {
            return ((index * 8) - 8) % this.Width;
        }

        private int GetYPositionFromFrameBufferIndex(int index)
        {
            return ((index * 8) - 8) / this.Height;
        }

        private byte[] CreateBuffer(int size, byte defaultValue)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException();

            var buffer = new byte[size];

            if (defaultValue != default)
            {
                this.ClearBuffer(ref buffer, defaultValue);
            }

            return buffer;
        }

        private void ClearFrameBuffers()
        {
            this.ClearBuffer(ref this.blackAndWhiteFrameBuffer, White);
            this.ClearBuffer(ref this.redFrameBuffer, Black);
        }

        private void ClearBuffer(ref byte[] buffer, byte defaultValue)
        {
            for(var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = defaultValue;
            }
        }

        private void SetFrameBufferPage(int page)
        {
            if (page < 0 || page >= PagesPerFrame)
                page = 0;

            this.ClearFrameBuffers();

            this.currentFrameBufferPage = page;
            this.CalculateFrameBufferPageBounds();
        }

        private void CalculateFrameBufferPageBounds()
        {
            this.currentFrameBufferPageLowerBound = this.currentFrameBufferPage * this.blackAndWhiteFrameBuffer.Length;
            this.currentFrameBufferPageUpperBound = (this.currentFrameBufferPage + 1) * this.blackAndWhiteFrameBuffer.Length;
            this.currentFrameBufferStartXPosition = this.GetXPositionFromFrameBufferIndex(this.currentFrameBufferPageLowerBound);
            this.currentFrameBufferStartYPosition = this.GetYPositionFromFrameBufferIndex(this.currentFrameBufferPageLowerBound);
        }

        private void WriteInternalBuffersToDevice()
        {
            this.DrawBuffer(this.blackAndWhiteFrameBuffer,
                                this.currentFrameBufferStartXPosition,
                                this.currentFrameBufferStartYPosition);

            this.DrawBuffer(this.redFrameBuffer,
                this.currentFrameBufferStartXPosition,
                this.currentFrameBufferStartYPosition);

            this.PerformFullRefresh();
        }


        /// <summary>
        /// Commands supported by SSD1681.
        /// </summary>
        public enum Command : byte
        {
            DriverOutputControl = 0x01,
            GateDrivingVoltage = 0x03,
            SourceDrivingVoltageControl = 0x04,
            InitialCodeSettingOtpProgram = 0x08,
            WriteRegisterForInitialCodeSetting = 0x09,
            ReadRegisterForInitialCodeSetting = 0x0a,
            BoosterSoftStartControl = 0x0c,
            DeepSleepMode = 0x10,
            DataEntryModeSetting = 0x11,
            SoftwareReset = 0x12,
            HvReadyDetection = 0x14,
            VciDetection = 0x15,
            TempSensorControlSelection = 0x18,
            TempSensorControlWriteRegister = 0x1a,
            TempSensorControlReadRegister = 0x1b,
            ExternalTempSensorControlWrite = 0x1c,
            MasterActivation = 0x20,
            DisplayUpdateControl1 = 0x21,
            DisplayUpdateControl2 = 0x22,
            WriteBackWhiteRAM = 0x24,
            WriteRedRAM = 0x26,
            ReadRAM = 0x27,
            VCOMSense = 0x28,
            VCOMSenseDuration = 0x29,
            ProgramVCOMOTP = 0x2a,
            WriteRegisterControlVCOM = 0x2b,
            WriteVCOMRegister = 0x2c,
            OTPRegisterReadDisplayOption = 0x2d,
            UserIdRead = 0x2e,
            StatusBitRead = 0x2f,
            ProgramWSOTP = 0x30,
            LoadWSOTP = 0x31,
            WriteLUTRegister = 0x32,
            CrcCalculation = 0x34,
            CrcStatusRead = 0x35,
            ProgramOTPSelection = 0x36,
            WriteRegisterForDisplayOption = 0x37,
            WriteRegisterForUserId = 0x38,
            OTPProgramMode = 0x39,
            BorderWaveformControl = 0x3c,
            EndOption = 0x3f,
            ReadRAMOption = 0x41,
            SetRAMAddressXStartEndPosition = 0x44,
            SetRAMAddressYStartEndPosition = 0x45,
            AutoWriteRAMForRegularPatternRed = 0x46,
            AutoWriteRAMForRegularPatternBlackWhite = 0x47,
            SetRAMAddressCounterX = 0x4e,
            SetRAMAddressCounterY = 0x4f,
            NOP = 0x7f
        }

        /// <summary>
        /// SSD1681 Supported Sleep Modes.
        /// </summary>
        public enum SleepMode : byte
        {
            /// <summary>
            /// Normal Sleep Mode.
            /// In this mode: 
            /// - DC/DC Off 
            /// - No Clock 
            /// - No Output load 
            /// - MCU Interface Access: ON
            /// - RAM Data Access: ON
            /// </summary>
            Normal = 0x00,

            /// <summary>
            /// Deep Sleep Mode 1.
            /// In this mode: 
            /// - DC/DC Off 
            /// - No Clock 
            /// - No Output load 
            /// - MCU Interface Access: OFF
            /// - RAM Data Access: ON (RAM contents retained)
            /// </summary>
            DeepSleepModeOne = 0x01,

            /// <summary>
            /// Deep Sleep Mode 2.
            /// In this mode: 
            /// - DC/DC Off 
            /// - No Clock 
            /// - No Output load 
            /// - MCU Interface Access: OFF
            /// - RAM Data Access: OFF (RAM contents NOT retained)
            /// </summary>
            DeepSleepModeTwo = 0x11,
        }

        /// <summary>
        /// SSD1861 Supported Refresh Modes
        /// </summary>
        public enum RefreshMode : byte
        {
            /// <summary>
            /// Causes the display to perform a full refresh of the panel (Display Mode 1).
            /// </summary>
            FullRefresh = 0xf7,

            /// <summary>
            /// Causes the display to perform a partial refresh of the panel (Display Mode 2).
            /// </summary>
            PartialRefresh = 0xff,
        }

        /// <summary>
        /// SSD1681 RAM
        /// </summary>
        public enum Ram : byte
        {
            /// <summary>
            /// Specifies the black & white RAM area.
            /// </summary>
            BlackWhite = 0x00,

            /// <summary>
            /// Specifies the Colored RAM area (Red for SSD1681).
            /// </summary>
            Color = 0x01,
        }

        #region IDisposable

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this.resetPin.Dispose();
                    this.busyPin.Dispose();
                    this.dataCommandPin.Dispose();
                    this.spiDevice.Dispose();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }

        #endregion
    }
}
