using Iot.Device.Ssd13xx;
using System;

namespace nanoFramework.MagicBit
{
    /// <summary>
    /// Console class to display text on screens
    /// </summary>
    public static class Console
    {
        /// <summary>
        /// The font to use in the screen
        /// </summary>
        public static IFont Font { get; set; }

        /// <summary>
        /// Gets the height of the screen in rows
        /// </summary>
        public static int WindowHeight { get => Screen.Height / Font.Height; }

        /// <summary>
        /// Gets the width of the screen in columns. Note this is based on the max character size.
        /// If you are using a non fix font, you will most likely be able to writ"e more characters.
        /// </summary>
        public static int WindowWidth { get => Screen.Width / Font.Width; }

        /// <summary>
        /// Gets or sets the column position.
        /// </summary>
        public static int CursorLeft { get; set; } = 0;

        /// <summary>
        /// Gets or sets the row position.
        /// </summary>
        public static int CursorTop { get; set; } = 0;

        /// <summary>
        /// Clears the screen.
        /// </summary>
        public static void Clear()
        {
            Screen.Clear();
            CursorLeft = 0;
            CursorTop = 0;
        }

        /// <summary>
        /// Writes a text on the screen at the cursor position.
        /// Cursor position will automatically increase.
        /// </summary>
        /// <remarks>No new line character will be recognized, use WriteLine instead.</remarks>
        /// <param name="text">The text to display.</param>
        public static void Write(string text)
        {
            ushort width = (ushort)(Screen.Width - CursorLeft * Font.Width);
            if (text.Length <= width / Font.Width)
            {
                Screen.Write((ushort)(CursorLeft * Font.Width), (ushort)(CursorTop * Font.Height), text);
                CursorLeft += text.Length;
            }
            else
            {
                string newTxt = text.Substring(0, width / Font.Width);
                Screen.Write((ushort)(CursorLeft * Font.Width), (ushort)(CursorTop * Font.Height), newTxt);
                CursorTop++;
                newTxt = text.Substring(width / Font.Width);
                Write(newTxt);
            }

            CursorTop = CursorTop > WindowHeight ? WindowHeight : CursorTop;
            Screen.Display();
        }

        /// <summary>
        /// Writes a text on the screen at the cursor position and goes to the next line.
        /// </summary>
        /// <param name="text">The text to display.</param>
        public static void WriteLine(string text)
        {
            Write(text);
            CursorLeft = 0;
            CursorTop++;
            CursorTop = CursorTop > WindowHeight ? WindowHeight : CursorTop;
        }
    }
}
