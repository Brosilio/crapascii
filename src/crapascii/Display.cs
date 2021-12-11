using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crapascii
{
    public static class Display
    {
        public static char[][] buffer;

        private static int width;
        private static int height;

        /* wasted like 20 minutes trying to figure out why shit wasnt Redraw()ing properly
         * turns out it was these retarded characters screwing with it */
        private static readonly char[] retarded = new[] { '\n', '\b', '\r', '\0' };

        public static void Title(string title)
        {
            Console.Title = title;
        }

        public static void Init()
        {
            Init(Console.WindowWidth, Console.WindowHeight);
        }

        public static void Init(int width, int height)
        {
            Display.width = width;
            Display.height = height;

            Console.SetBufferSize(width, height);

            char[][] newBuffer = new char[height][];

            for (int y = 0; y < height; y++)
            {
                newBuffer[y] = new string(' ', width).ToCharArray();
            }

            buffer = newBuffer;
        }

        public static void Redraw()
        {
            int ox = Console.CursorLeft;
            int oy = Console.CursorTop;

            Console.Clear();

            for (int y = 0; y < height; y++)
            {
                Console.SetCursorPosition(0, y);
                if (y == height - 1)
                {
                    Console.Write(buffer[y], 0, width - 1);
                }
                else
                {
                    Console.Write(buffer[y], 0, width);
                }
            }

            SetCursorPosition(ox, oy);
        }

        /// <summary>
        /// Move the console cursor relative to where it is now
        /// </summary>
        /// <param name="x">x movement</param>
        /// <param name="y">y movement</param>
        public static void MoveCursor(int x, int y)
        {
            SetCursorPosition(Console.CursorLeft + x, Console.CursorTop - y);
        }

        /// <summary>
        /// Safely sets the cursor to an absolute position. Will clamp to window/buffer size
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        public static void SetCursorPosition(int x, int y)
        {
            if (x < 0) x = 0;
            if (x >= Console.BufferWidth) x = Console.BufferWidth - 1;

            if (y < 0) y = 0;
            if (y >= Console.BufferHeight) y = Console.BufferHeight - 1;

            Console.SetCursorPosition(x, y);
        }

        /// <summary>
        /// Write a character to the display and then move the cursor to where it was.
        /// </summary>
        /// <param name="c">the character</param>
        public static void SetChar(char c)
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;

            if (!IsRetarded(c))
            {
                buffer[y][x] = c;
                Console.Write(c);
            }

            Console.SetCursorPosition(x, y);
        }

        public static void PutChar(char c)
        {
            //if (!IsRetarded(c))
            {
                int x = Console.CursorLeft;
                int y = Console.CursorTop;

                buffer[y][x] = c;
                Console.Write(c);
            }
        }

        public static string Input(string prompt, int x, int y)
        {
            int ox = Console.CursorLeft;
            int oy = Console.CursorTop;

            SetCursorPosition(x, y);
            Console.Write(prompt);

            string input = Console.ReadLine();

            SetCursorPosition(ox, oy);

            return input;
        }

        public static bool GetYesNo()
        {
            char k;

            do
            {
                k = Console.ReadKey(true).KeyChar;
            } while (k != 'y' && k != 'n');

            return k == 'y';
        }

        public static void ClearConsole()
        {
            int ox = Console.CursorLeft;
            int oy = Console.CursorTop;

            Console.Clear();

            SetCursorPosition(ox, oy);
        }

        public static void WriteConsole(string s, out int ox, out int oy)
        {
            ox = Console.CursorLeft;
            oy = Console.CursorTop;

            Console.Write(s);
        }

        public static void WriteConsole(string s)
        {
            WriteConsole(s, out _, out _);
        }

        public static bool IsRetarded(char c)
        {
            return retarded.Contains(c);
        }

        public static void Pause()
        {
            Console.ReadKey(true);
        }
    }
}
