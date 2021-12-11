using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crapascii
{
    class Program
    {
        private static readonly Keybind[] keybinds = new[]
        {
            /* Movement */
            new Keybind(() => Move(0, 1), ConsoleKey.UpArrow, 0),
            new Keybind(() => Move(0, -1), ConsoleKey.DownArrow, 0),
            new Keybind(() => Move(-1, 0), ConsoleKey.LeftArrow, 0),
            new Keybind(() => Move(1, 0), ConsoleKey.RightArrow, 0),

            /* Accelerated Movement */
            new Keybind(() => Move(0, 5), ConsoleKey.PageUp, 0),
            new Keybind(() => Move(0, -5), ConsoleKey.PageDown, 0),
            new Keybind(() => Move(-5, 0), ConsoleKey.Home, 0),
            new Keybind(() => Move(5, 0), ConsoleKey.End, 0),

            /* Drawing */
            new Keybind(ToggleTouchdown, ConsoleKey.Tab, ConsoleModifiers.Control),
            new Keybind(ToggleAdvance, ConsoleKey.D1, ConsoleModifiers.Control),
            new Keybind(Clear, ConsoleKey.K, ConsoleModifiers.Control),
            new Keybind(PickChar, ConsoleKey.Spacebar, ConsoleModifiers.Control),

            /* File management */
            new Keybind(Save, ConsoleKey.S, ConsoleModifiers.Control),
            new Keybind(Open, ConsoleKey.O, ConsoleModifiers.Control),

            /* Misc */
            new Keybind(Display.Redraw, ConsoleKey.F, ConsoleModifiers.Control),
            new Keybind(ShowHelp, ConsoleKey.H, ConsoleModifiers.Control)
        };

        private static void ShowHelp()
        {
            Display.ClearConsole();

            Display.WriteConsole("--=[ Help ]=--\n", out int ox, out int oy);
            Display.WriteConsole("Move: arrow keys\n");
            Display.WriteConsole("Move fast: home/end/pgup/pgdn\n");
            Display.WriteConsole("Save: ctrl+s\n");
            Display.WriteConsole("Open: ctrl+o\n");
            Display.WriteConsole("Toggle auto-advance: ctrl+1\n");
            Display.WriteConsole("Toggle touchdown   : ctrl+tab\n");
            Display.WriteConsole("Redraw display     : ctrl+f\n");
            Display.WriteConsole("Clear everything   : ctrl+k\n");
            Display.WriteConsole("Select character   : ctrl+space\n");
            Display.WriteConsole("Help: ctrl + h\n");
            Display.WriteConsole("\nPress any key to close\n");

            Display.SetCursorPosition(ox, oy);
            Display.Pause();
            Display.Redraw();
        }

        public static bool touchdown;
        public static bool advance;
        public static char lastChar = ' ';
        public static int lastXmove;
        public static int lastYmove;

        static void Main(string[] args)
        {
            Display.Title($"crapascii ({Console.WindowWidth},{Console.WindowHeight})");
            Display.Init();

            while (true)
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);

                bool unboundKey = true;
                for (int i = 0; i < keybinds.Length && unboundKey; i++) unboundKey = !keybinds[i].Execute(cki);

                if (unboundKey && !Display.IsRetarded(cki.KeyChar))
                {
                    lastChar = cki.KeyChar;
                    Display.SetChar(cki.KeyChar);

                    if (advance)
                    {
                        Display.MoveCursor(lastXmove, lastYmove);
                    }
                }
            }
        }

        static void ToggleTouchdown()
        {
            touchdown = !touchdown;
            Display.Title($"touchdown {(touchdown ? "on" : "off")}");
        }

        static void ToggleAdvance()
        {
            if (lastXmove == 0 && lastYmove == 0) lastXmove = 1;
            advance = !advance;

            Display.Title($"advance {(advance ? "on" : "off")} ({ToDirArrow(lastXmove, lastYmove)})");
        }

        static void Move(int x, int y)
        {
            lastXmove = x;
            lastYmove = y;


            if (advance)
            {
                Display.Title($"advance {(advance ? "on" : "off")} ({ToDirArrow(lastXmove, lastYmove)})");
            }
            else
            {
                Display.MoveCursor(x, y);
            }

            if (touchdown)
            {
                Display.SetChar(lastChar);
            }
        }

        static void Save()
        {
            Display.ClearConsole();

            string file = Display.Input("Save to file (leave blank to cancel): ", 0, 0);

            if (file.Trim().Length == 0)
            {
                Display.Redraw();
                return;
            }

            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.ReadWrite))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                for (int y = 0; y < Display.buffer.Length; y++)
                {
                    sw.WriteLine(new string(Display.buffer[y]).TrimEnd());
                }

                sw.Flush();
                sw.Close();
            }

            Display.Redraw();
        }

        private static void Open()
        {
            Display.ClearConsole();

            string file = Display.Input("Open file (leave blank to cancel): ", 0, 0);
            Display.ClearConsole();

            if (file.Trim().Length == 0)
            {
                Display.Redraw();
                return;
            }

            string[] lines = File.ReadAllLines(file);

            int y = 0;
            foreach (string l in lines)
            {
                Display.SetCursorPosition(0, y);

                foreach (char c in l)
                {
                    Display.PutChar(c);
                }
                y++;
            }
        }

        private static void Clear()
        {
            Display.ClearConsole();
            Display.WriteConsole("Are you sure you want to clear the drawing? (y/n): ");

            if (Display.GetYesNo())
            {
                Display.Init();
            }

            Display.SetCursorPosition(0, 0);
            Display.Redraw();
        }

        private static void PickChar()
        {
            lastChar = Display.buffer[Console.CursorTop][Console.CursorLeft];
            Display.Title($"character is now '{lastChar}'");
        }

        private static string ToDirArrow(int x, int y)
        {
            string r = "";

            if (x > 0) r += "►";
            if (x < 0) r += "◄";
            if (y > 0) r += "▲";
            if (y < 0) r += "▼";

            return r;
        }
    }
}
