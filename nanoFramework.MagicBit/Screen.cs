using Iot.Device.Ssd13xx;
using System;
using System.Device.I2c;

namespace nanoFramework.MagicBit
{
    /// <summary>
    /// MagicBit Screen
    /// </summary>
    public class Screen
    {
        /// <summary>
        /// The primitive driver for the screen to allow advance scenarios
        /// </summary>
        public static Ssd1306 Device { get; internal set; }

        /// <summary>
        /// Creates the screen
        /// </summary>
        public Screen()
        {
            // If we have a device, it's been already setup
            if (Device != null)
            {
                return;
            }

            Device = new Ssd1306(I2cDevice.Create(new I2cConnectionSettings(1, Ssd1306.DefaultI2cAddress)), Ssd13xx.DisplayResolution.OLED128x64);
            Device.ClearScreen();
            Device.Font = new BasicFont();
            Console.Font = Device.Font;
        }

        /// <summary>
        /// Clears the screen.
        /// </summary>
        public static void Clear() => Device.ClearScreen();

        /// <summary>
        /// Displays the screen elements.
        /// </summary>
        public static void Display() => Device.Display();

        /// <summary>
        /// Writes a text on the screen.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="size">The size of the text.</param>
        /// <param name="center">True for center alignment.</param>
        public static void Write(int x, int y, string text, byte size = 1, bool center = false) => Device.DrawString(x, y, text, size, center);

        /// <summary>
        /// Draw a bitmpa on the screen.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">The width, must be 8 or a multiple of 8.</param>
        /// <param name="art">The bitmap where a bit represents a pixel.</param>
        public static void DrawBitmap(int x, int y, int width, byte[] art)
        {
            if (width % 8 != 0)
            {
                throw new ArgumentException(nameof(width));
            }

            if ((art.Length * 8 / width) % 8 != 0)
            {
                throw new ArgumentException(nameof(art));
            }

            for (int yy = 0; yy < (art.Length - width / 8); yy++)
            {
                for (int xx = 0; xx < 8 + width / 8; xx++)
                {
                    Device.DrawPixel(x + xx, y + yy, (art[yy] & (1 << xx)) == (1 << xx));
                }
            }
        }

        /// <summary>
        /// Gets the screen width.
        /// </summary>
        public static int Width { get => Device.Width; }

        /// <summary>
        /// Get the screen height.
        /// </summary>
        public static int Height { get => Device.Height; }
    }
}
