using System.Device.Gpio;
using System.Device.Spi;
using System.Diagnostics;

using Iot.Device.ePaper.Drivers;
using Iot.Device.ePaperGraphics;

namespace SSD1681Sample
{
    public class Program
    {
        public static void Main()
        {
            // Setup SPI connection with the display
            var spiConnectionSettings = new SpiConnectionSettings(1, 22)
            {
                ClockFrequency = 20 * 1000 * 1000,
                Mode = SpiMode.Mode0,
                ChipSelectLineActiveState = false,
                Configuration = SpiBusConfiguration.HalfDuplex,
                DataFlow = DataFlow.MsbFirst
            };

            using var spiDevice = new SpiDevice(spiConnectionSettings);


            // Setup required GPIO Pins
            using var gpioController = new GpioController();

            using var resetPin = gpioController.OpenPin(15, PinMode.Output);
            using var dataCommandPin = gpioController.OpenPin(5, PinMode.Output);
            using var busyPin = gpioController.OpenPin(4, PinMode.Input);
            using var ledPin = gpioController.OpenPin(2, PinMode.Output);


            // Create an instance of the display driver
            using var display = new Ssd1681(resetPin, busyPin, dataCommandPin,
                spiDevice, width: 200, height: 200, enableFramePaging: true);

            using var gfx = new Graphics(display);


            // Power on the display and initialize it
            display.PowerOn();
            display.Initialize();

            // clear the display
            display.Clear(triggerPageRefresh: true);

            // draw a frame using paging
            display.BeginFrameDraw();
            do
            {
                for (var y = 0; y < display.Height; y++)
                {
                    display.DrawPixel(100, y, inverted: true);
                }

                gfx.DrawLine(0, 0, 200, 200, Color.Black);

                gfx.DrawCircle(25, 25, 100, Color.Black);

                gfx.DrawCircle(150, 50, 10, Color.Black);

                gfx.DrawText("Hello World", new Font8x12(), 0, 0);

                gfx.DrawText("Hello World", new Font8x12(), 25, 24);

                gfx.DrawText("Hello World", new Font8x12(), 35, 50);

                gfx.DrawText("Hello World", new Font8x12(), 80, 140);

                gfx.DrawText("@MrCSharp", new Font8x12(), 40, 180);

                gfx.DrawRectangle(25, 95, 100, 100, Color.Black, true);

            } while (display.NextFramePage());
            display.EndFrameDraw();

            //display.DirectDrawBuffer(0, 0, qrCode);
            display.PerformFullRefresh();

            // Put the display to sleep to reduce power consumption
            display.PowerDown(Ssd1681.SleepMode.DeepSleepModeTwo);
        }

        private static byte[] qrCode = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfc, 0x00, 0x00, 0x01, 0xff, 0xe0, 0x01, 0xff, 0xe0, 0x00, 0x07, 0xf0, 0x3f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x01, 0xff, 0xe0, 0x01, 0xff, 0xe0, 0x00, 0x07, 0xf0, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x01, 0xff, 0xe0, 0x01, 0xff, 0xe0, 0x00, 0x07, 0xf0, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x01, 0xff, 0xe0, 0x01, 0xff, 0xe0, 0x00, 0x07, 0xf0, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x01, 0xff, 0xe0, 0x01, 0xff, 0xe0, 0x00, 0x07, 0xf0, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x01, 0xff, 0xe0, 0x01, 0xff, 0xe0, 0x00, 0x07, 0xf0, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x00, 0x00, 0x7e, 0x07, 0xe0, 0x00, 0x00, 0x1f, 0xfe, 0x07, 0xf0, 0x7f, 0x00, 0x00, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x00, 0x00, 0x7e, 0x07, 0xe0, 0x00, 0x00, 0x1f, 0xfe, 0x07, 0xf0, 0x7f, 0x00, 0x00, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x00, 0x00, 0x7e, 0x07, 0xe0, 0x00, 0x00, 0x1f, 0xfe, 0x07, 0xf0, 0x7f, 0x00, 0x00, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x00, 0x00, 0x7e, 0x07, 0xe0, 0x00, 0x00, 0x1f, 0xfe, 0x07, 0xf0, 0x7f, 0x00, 0x00, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x00, 0x00, 0x7e, 0x07, 0xe0, 0x00, 0x00, 0x1f, 0xfe, 0x07, 0xf0, 0x7f, 0x00, 0x00, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x00, 0x00, 0x7e, 0x07, 0xe0, 0x00, 0x00, 0x1f, 0xfe, 0x07, 0xf0, 0x7f, 0x00, 0x00, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x07, 0xe0, 0x7e, 0x07, 0xf0, 0x01, 0xfc, 0x00, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x01, 0xfc, 0x00, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x01, 0xfc, 0x00, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x01, 0xfc, 0x00, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x01, 0xfc, 0x00, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x01, 0xfc, 0x00, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x07, 0xe0, 0x00, 0x07, 0xff, 0x80, 0x00, 0x00, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x07, 0xe0, 0x00, 0x07, 0xff, 0x80, 0x00, 0x00, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x07, 0xe0, 0x00, 0x07, 0xff, 0x80, 0x00, 0x00, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x07, 0xe0, 0x00, 0x07, 0xff, 0x80, 0x00, 0x00, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x07, 0xe0, 0x00, 0x07, 0xff, 0x80, 0x00, 0x00, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x07, 0xe0, 0x00, 0x07, 0xff, 0x80, 0x00, 0x00, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x03, 0xe0, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xff, 0xf8, 0x1f, 0xff, 0xf8, 0x1f, 0xfe, 0x07, 0xf0, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xff, 0xf8, 0x1f, 0xff, 0xf8, 0x1f, 0xfe, 0x07, 0xf0, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xff, 0xf8, 0x1f, 0xff, 0xf8, 0x1f, 0xfe, 0x07, 0xf0, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xff, 0xf8, 0x1f, 0xff, 0xf8, 0x1f, 0xfe, 0x07, 0xf0, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xff, 0xf8, 0x1f, 0xff, 0xf8, 0x1f, 0xfe, 0x07, 0xf0, 0x7f, 0x03, 0xff, 0xff, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x03, 0xe0, 0x7f, 0x00, 0x00, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xe0, 0x7e, 0x00, 0x00, 0x7f, 0x00, 0x00, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xe0, 0x7e, 0x00, 0x00, 0x7f, 0x00, 0x00, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xe0, 0x7e, 0x00, 0x00, 0x7f, 0x00, 0x00, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xe0, 0x7e, 0x00, 0x00, 0x7f, 0x00, 0x00, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xe0, 0x7e, 0x00, 0x00, 0x7f, 0x00, 0x00, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xe0, 0x7e, 0x03, 0xe0, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xf0, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xf0, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xf0, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xf0, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xf0, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x07, 0xff, 0xff, 0xff, 0xff, 0xfc, 0x07, 0xe0, 0xfe, 0x0f, 0xf0, 0x7f, 0x07, 0xe0, 0x7f, 0x07, 0xe0, 0x3f, 0xff, 0xff, 0xff, 0xff, 0xe0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0xff, 0x81, 0xf8, 0x1f, 0x81, 0xf8, 0x00, 0x7f, 0xfc, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0xff, 0x81, 0xf8, 0x1f, 0x81, 0xf8, 0x00, 0x7f, 0xfc, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0xff, 0x81, 0xf8, 0x1f, 0x81, 0xf8, 0x00, 0x7f, 0xfc, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0xff, 0x81, 0xf8, 0x1f, 0x81, 0xf8, 0x00, 0x7f, 0xfc, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0xff, 0x81, 0xf8, 0x1f, 0x81, 0xf8, 0x00, 0x7f, 0xfc, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0xff, 0x81, 0xf8, 0x1f, 0x81, 0xf8, 0x00, 0x7f, 0xfc, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x00, 0x07, 0xe0, 0x7f, 0xf8, 0x1f, 0x80, 0x07, 0xf0, 0x7f, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x00, 0x07, 0xe0, 0x7f, 0xf8, 0x1f, 0x80, 0x07, 0xf0, 0x7f, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x00, 0x07, 0xe0, 0x7f, 0xf8, 0x1f, 0x80, 0x07, 0xf0, 0x7f, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x00, 0x07, 0xe0, 0x7f, 0xf8, 0x1f, 0x80, 0x07, 0xf0, 0x7f, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x00, 0x07, 0xe0, 0x7f, 0xf8, 0x1f, 0x80, 0x07, 0xf0, 0x7f, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x00, 0x07, 0xe0, 0x7f, 0xfc, 0x1f, 0xc0, 0x07, 0xf0, 0x7f, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1f, 0x81, 0xf8, 0x00, 0x01, 0xff, 0xe0, 0x7f, 0xff, 0xf0, 0x7f, 0xff, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1f, 0x81, 0xf8, 0x00, 0x01, 0xff, 0xe0, 0x7f, 0xff, 0xf0, 0x7f, 0xff, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1f, 0x81, 0xf8, 0x00, 0x01, 0xff, 0xe0, 0x7f, 0xff, 0xf0, 0x7f, 0xff, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1f, 0x81, 0xf8, 0x00, 0x01, 0xff, 0xe0, 0x7f, 0xff, 0xf0, 0x7f, 0xff, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1f, 0x81, 0xf8, 0x00, 0x01, 0xff, 0xe0, 0x7f, 0xff, 0xf0, 0x7f, 0xff, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3f, 0x81, 0xf8, 0x00, 0x01, 0xff, 0xe0, 0x7f, 0xff, 0xf0, 0x7f, 0xff, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0x03, 0xf8, 0x00, 0xfe, 0x07, 0xff, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7e, 0x07, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x03, 0xf8, 0x00, 0xfe, 0x07, 0xff, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7e, 0x07, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x03, 0xf8, 0x00, 0xfe, 0x07, 0xff, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7e, 0x07, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x03, 0xf8, 0x00, 0xfe, 0x07, 0xff, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7e, 0x07, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x03, 0xf8, 0x00, 0xfe, 0x07, 0xff, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7e, 0x07, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xe0, 0x03, 0xf8, 0x00, 0xfe, 0x0f, 0xff, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7e, 0x07, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xfc, 0x0f, 0xff, 0x81, 0xff, 0xff, 0x80, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x00, 0x07, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xfc, 0x0f, 0xff, 0x81, 0xff, 0xff, 0x80, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x00, 0x07, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xfc, 0x0f, 0xff, 0x81, 0xff, 0xff, 0x80, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x00, 0x07, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xfc, 0x0f, 0xff, 0x81, 0xff, 0xff, 0x80, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x00, 0x07, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xfc, 0x0f, 0xff, 0x81, 0xff, 0xff, 0x80, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x00, 0x07, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xfc, 0x0f, 0xff, 0x83, 0xff, 0xff, 0x80, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x00, 0x07, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xff, 0xfe, 0x07, 0xe0, 0x7f, 0xff, 0xe0, 0x00, 0x07, 0xff, 0x81, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xff, 0xfe, 0x07, 0xe0, 0x7f, 0xff, 0xe0, 0x00, 0x07, 0xff, 0x81, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xff, 0xfe, 0x07, 0xe0, 0x7f, 0xff, 0xe0, 0x00, 0x07, 0xff, 0x81, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xff, 0xfe, 0x07, 0xe0, 0x7f, 0xff, 0xe0, 0x00, 0x07, 0xff, 0x81, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xff, 0xfe, 0x07, 0xe0, 0x7f, 0xff, 0xe0, 0x00, 0x07, 0xff, 0x81, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xff, 0xfe, 0x07, 0xe0, 0x7f, 0xff, 0xe0, 0x00, 0x07, 0xff, 0x81, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3f, 0xff, 0xff, 0xe0, 0x03, 0xf8, 0x00, 0x7f, 0xf8, 0x1f, 0xff, 0xf8, 0x00, 0x00, 0x07, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x3f, 0xff, 0xff, 0xe0, 0x01, 0xf8, 0x00, 0x7f, 0xf8, 0x1f, 0xff, 0xf8, 0x00, 0x00, 0x07, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x3f, 0xff, 0xff, 0xe0, 0x01, 0xf8, 0x00, 0x7f, 0xf8, 0x1f, 0xff, 0xf8, 0x00, 0x00, 0x07, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x3f, 0xff, 0xff, 0xe0, 0x01, 0xf8, 0x00, 0x7f, 0xf8, 0x1f, 0xff, 0xf8, 0x00, 0x00, 0x07, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x3f, 0xff, 0xff, 0xe0, 0x01, 0xf8, 0x00, 0x7f, 0xf8, 0x1f, 0xff, 0xf8, 0x00, 0x00, 0x07, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x3f, 0xff, 0xff, 0xe0, 0x01, 0xf8, 0x00, 0x7f, 0xf8, 0x1f, 0xff, 0xf8, 0x00, 0x00, 0x07, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0x03, 0xf8, 0x3f, 0xff, 0xff, 0xff, 0xff, 0xf8, 0x1f, 0xff, 0xff, 0xff, 0xfe, 0x07, 0xff, 0xc0, 0x07, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x03, 0xf8, 0x3f, 0xff, 0xff, 0xff, 0xff, 0xf8, 0x1f, 0xff, 0xff, 0xff, 0xfe, 0x07, 0xff, 0xc0, 0x03, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x03, 0xf8, 0x3f, 0xff, 0xff, 0xff, 0xff, 0xf8, 0x1f, 0xff, 0xff, 0xff, 0xfe, 0x07, 0xff, 0xc0, 0x03, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x03, 0xf8, 0x3f, 0xff, 0xff, 0xff, 0xff, 0xf8, 0x1f, 0xff, 0xff, 0xff, 0xfe, 0x07, 0xff, 0xc0, 0x03, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x03, 0xf8, 0x3f, 0xff, 0xff, 0xff, 0xff, 0xf8, 0x1f, 0xff, 0xff, 0xff, 0xfe, 0x07, 0xff, 0xc0, 0x03, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x03, 0xf8, 0x3f, 0xff, 0xff, 0xff, 0xff, 0xf8, 0x1f, 0xff, 0xff, 0xff, 0xfe, 0x07, 0xff, 0xc0, 0x03, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3f, 0x03, 0xf8, 0x3f, 0x80, 0x00, 0x1f, 0x80, 0x07, 0xff, 0x81, 0xf8, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0xfc, 0x00, 0x00, 0x00, 0x00, 0x3f, 0x03, 0xf8, 0x3f, 0x80, 0x00, 0x1f, 0x80, 0x07, 0xff, 0x81, 0xf8, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0xfc, 0x00, 0x00, 0x00, 0x00, 0x3f, 0x03, 0xf8, 0x3f, 0x80, 0x00, 0x1f, 0x80, 0x07, 0xff, 0x81, 0xf8, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0xfc, 0x00, 0x00, 0x00, 0x00, 0x3f, 0x03, 0xf8, 0x3f, 0x80, 0x00, 0x1f, 0x80, 0x07, 0xff, 0x81, 0xf8, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0xfc, 0x00, 0x00, 0x00, 0x00, 0x3f, 0x03, 0xf8, 0x3f, 0x80, 0x00, 0x1f, 0x80, 0x07, 0xff, 0x81, 0xf8, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0xfc, 0x00, 0x00, 0x00, 0x00, 0x3f, 0x03, 0xf8, 0x3f, 0x80, 0x00, 0x1f, 0x80, 0x07, 0xff, 0x81, 0xf8, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xe0, 0xff, 0xff, 0xe0, 0x00, 0x07, 0xe0, 0x7f, 0xf8, 0x1f, 0xc1, 0xfc, 0x1f, 0xc1, 0xfc, 0x1f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xe0, 0xff, 0xff, 0xe0, 0x00, 0x07, 0xe0, 0x7f, 0xf8, 0x1f, 0x81, 0xfc, 0x1f, 0xc0, 0xfc, 0x0f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xe0, 0xff, 0xff, 0xe0, 0x00, 0x07, 0xe0, 0x7f, 0xf8, 0x1f, 0x81, 0xfc, 0x1f, 0xc0, 0xfc, 0x0f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xe0, 0xff, 0xff, 0xe0, 0x00, 0x07, 0xe0, 0x7f, 0xf8, 0x1f, 0x81, 0xfc, 0x1f, 0xc0, 0xfc, 0x0f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xe0, 0xff, 0xff, 0xe0, 0x00, 0x07, 0xe0, 0x7f, 0xf8, 0x1f, 0x81, 0xfc, 0x1f, 0xc0, 0xfc, 0x0f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xe0, 0xff, 0xff, 0xe0, 0x00, 0x07, 0xe0, 0x7f, 0xf8, 0x1f, 0x81, 0xfc, 0x1f, 0xc0, 0xfc, 0x0f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xff, 0x80, 0x00, 0x00, 0x01, 0xf8, 0x00, 0x01, 0xff, 0xf0, 0x01, 0xfc, 0x1f, 0xff, 0xff, 0xf0, 0x7f, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xff, 0x80, 0x00, 0x00, 0x01, 0xf8, 0x00, 0x01, 0xff, 0xe0, 0x01, 0xfc, 0x1f, 0xff, 0xff, 0xf0, 0x3f, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xff, 0x80, 0x00, 0x00, 0x01, 0xf8, 0x00, 0x01, 0xff, 0xe0, 0x01, 0xfc, 0x1f, 0xff, 0xff, 0xf0, 0x3f, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xff, 0x80, 0x00, 0x00, 0x01, 0xf8, 0x00, 0x01, 0xff, 0xe0, 0x01, 0xfc, 0x1f, 0xff, 0xff, 0xf0, 0x3f, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xff, 0x80, 0x00, 0x00, 0x01, 0xf8, 0x00, 0x01, 0xff, 0xe0, 0x01, 0xfc, 0x1f, 0xff, 0xff, 0xf0, 0x3f, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x0f, 0xff, 0x80, 0x00, 0x00, 0x01, 0xf8, 0x00, 0x01, 0xff, 0xe0, 0x01, 0xfc, 0x1f, 0xff, 0xff, 0xf0, 0x3f, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xe0, 0x00, 0x0f, 0xff, 0xfe, 0x00, 0x00, 0x01, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x7f, 0x07, 0xf0, 0x7f, 0xff, 0xf0, 0x3f, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x0f, 0xff, 0xfe, 0x00, 0x00, 0x01, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x7e, 0x07, 0xf0, 0x7f, 0xff, 0xf0, 0x3f, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x0f, 0xff, 0xfe, 0x00, 0x00, 0x01, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x7e, 0x07, 0xf0, 0x7f, 0xff, 0xf0, 0x3f, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x0f, 0xff, 0xfe, 0x00, 0x00, 0x01, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x7e, 0x07, 0xf0, 0x7f, 0xff, 0xf0, 0x3f, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x0f, 0xff, 0xfe, 0x00, 0x00, 0x01, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x7e, 0x07, 0xf0, 0x7f, 0xff, 0xf0, 0x3f, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x0f, 0xff, 0xfe, 0x00, 0x00, 0x01, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x7e, 0x07, 0xf0, 0x7f, 0xff, 0xf0, 0x3f, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x03, 0xf8, 0x00, 0x00, 0x07, 0xff, 0xfe, 0x00, 0x00, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xff, 0xff, 0x00, 0x00, 0x01, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x03, 0xf8, 0x00, 0x00, 0x07, 0xff, 0xfe, 0x00, 0x00, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xff, 0xff, 0x00, 0x00, 0x00, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x03, 0xf8, 0x00, 0x00, 0x07, 0xff, 0xfe, 0x00, 0x00, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xff, 0xff, 0x00, 0x00, 0x00, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x03, 0xf8, 0x00, 0x00, 0x07, 0xff, 0xfe, 0x00, 0x00, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xff, 0xff, 0x00, 0x00, 0x00, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x03, 0xf8, 0x00, 0x00, 0x07, 0xff, 0xfe, 0x00, 0x00, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xff, 0xff, 0x00, 0x00, 0x00, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x03, 0xf8, 0x00, 0x00, 0x07, 0xff, 0xfe, 0x00, 0x00, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xff, 0xff, 0x00, 0x00, 0x00, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xf8, 0x00, 0x7c, 0x07, 0xff, 0xff, 0xf8, 0x00, 0x7e, 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x3f, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xf8, 0x00, 0xfe, 0x00, 0x00, 0x7f, 0xf8, 0x00, 0x00, 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x3f, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xf8, 0x00, 0xfe, 0x00, 0x00, 0x7f, 0xf8, 0x00, 0x00, 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x3f, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xf8, 0x00, 0xfe, 0x00, 0x00, 0x7f, 0xf8, 0x00, 0x00, 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x3f, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xf8, 0x00, 0xfe, 0x00, 0x00, 0x7f, 0xf8, 0x00, 0x00, 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x3f, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xf8, 0x00, 0xfe, 0x00, 0x00, 0x7f, 0xf8, 0x00, 0x00, 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x3f, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0x7f, 0xf0, 0x00, 0x7c, 0x07, 0xff, 0xff, 0xf8, 0x1f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0xff, 0x81, 0xf8, 0x1f, 0xff, 0xf8, 0x00, 0x00, 0x07, 0xf0, 0x00, 0x03, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0xff, 0x81, 0xf8, 0x1f, 0xff, 0xf8, 0x00, 0x00, 0x07, 0xf0, 0x00, 0x03, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0xff, 0x81, 0xf8, 0x1f, 0xff, 0xf8, 0x00, 0x00, 0x07, 0xf0, 0x00, 0x03, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0xff, 0x81, 0xf8, 0x1f, 0xff, 0xf8, 0x00, 0x00, 0x07, 0xf0, 0x00, 0x03, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0xff, 0x81, 0xf8, 0x1f, 0xff, 0xf8, 0x00, 0x00, 0x07, 0xf0, 0x00, 0x03, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x07, 0xff, 0xff, 0xff, 0xff, 0xfc, 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x3e, 0x03, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x7f, 0x03, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x7f, 0x03, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x7f, 0x03, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x7f, 0x03, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x7f, 0x03, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x3e, 0x03, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xff, 0x81, 0xf8, 0x00, 0x7f, 0xff, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xff, 0x81, 0xf8, 0x00, 0x7f, 0xff, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xff, 0x81, 0xf8, 0x00, 0x7f, 0xff, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xff, 0x81, 0xf8, 0x00, 0x7f, 0xff, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xff, 0x81, 0xf8, 0x00, 0x7f, 0xff, 0xf0, 0x00, 0x03, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xff, 0xc1, 0xf8, 0x00, 0x7f, 0xff, 0xf0, 0x00, 0x07, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0x80, 0x00, 0x00, 0x7f, 0xf8, 0x1f, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xf0, 0x3f, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0x80, 0x00, 0x00, 0x7f, 0xf8, 0x1f, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xf0, 0x3f, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0x80, 0x00, 0x00, 0x7f, 0xf8, 0x1f, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xf0, 0x3f, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0x80, 0x00, 0x00, 0x7f, 0xf8, 0x1f, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xf0, 0x3f, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0x80, 0x00, 0x00, 0x7f, 0xf8, 0x1f, 0xfe, 0x07, 0xff, 0xff, 0xff, 0xf0, 0x3f, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0x80, 0x00, 0x00, 0x7f, 0xfc, 0x1f, 0xff, 0x07, 0xff, 0xff, 0xff, 0xf0, 0x7f, 0x03, 0xf0, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xe0, 0x01, 0xff, 0xff, 0x81, 0xff, 0xe0, 0x01, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xe0, 0x01, 0xff, 0xff, 0x81, 0xff, 0xe0, 0x01, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xe0, 0x01, 0xff, 0xff, 0x81, 0xff, 0xe0, 0x01, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xe0, 0x01, 0xff, 0xff, 0x81, 0xff, 0xe0, 0x01, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xe0, 0x01, 0xff, 0xff, 0x81, 0xff, 0xe0, 0x01, 0xff, 0xf0, 0x00, 0x00, 0x0f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xe0, 0x01, 0xff, 0xff, 0x81, 0xff, 0xf0, 0x01, 0xff, 0xf0, 0x00, 0x00, 0x1f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x00, 0x1f, 0xfe, 0x00, 0x1f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x00, 0x1f, 0xfe, 0x00, 0x1f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x00, 0x1f, 0xfe, 0x00, 0x1f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x00, 0x1f, 0xfe, 0x00, 0x1f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x00, 0x1f, 0xfe, 0x00, 0x1f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0xff, 0xff, 0xc0, 0xfe, 0x07, 0xff, 0xfe, 0x00, 0x1f, 0xfe, 0x00, 0x1f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x00, 0x1f, 0x80, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xff, 0xc0, 0xfc, 0x0f, 0xc0, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x00, 0x1f, 0x80, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xff, 0xc0, 0xfc, 0x0f, 0xc0, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x00, 0x1f, 0x80, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xff, 0xc0, 0xfc, 0x0f, 0xc0, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x00, 0x1f, 0x80, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xff, 0xc0, 0xfc, 0x0f, 0xc0, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x00, 0x1f, 0x80, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x7e, 0x07, 0xff, 0xc0, 0xfc, 0x0f, 0xc0, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xc0, 0x00, 0x00, 0x00, 0xfe, 0x00, 0x1f, 0x80, 0x07, 0xe0, 0x7e, 0x07, 0xe0, 0x7f, 0x07, 0xff, 0xc1, 0xfc, 0x0f, 0xc0, 0xfc, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x07, 0xe0, 0x01, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x7f, 0xfc, 0x00, 0x7f, 0xff, 0xf0, 0x3f, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x07, 0xe0, 0x01, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x7f, 0xfc, 0x00, 0x7f, 0xff, 0xf0, 0x3f, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x07, 0xe0, 0x01, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x7f, 0xfc, 0x00, 0x7f, 0xff, 0xf0, 0x3f, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x07, 0xe0, 0x01, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x7f, 0xfc, 0x00, 0x7f, 0xff, 0xf0, 0x3f, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x07, 0xe0, 0x01, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x7f, 0xfc, 0x00, 0x7f, 0xff, 0xf0, 0x3f, 0x00, 0x00, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xfc, 0x07, 0xe0, 0x01, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x7f, 0xfc, 0x00, 0x7f, 0xff, 0xf0, 0x3f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
    }
}
