using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crapascii
{
    public class Keybind
    {
        public ConsoleKey key;
        public ConsoleModifiers mod;

        public Action handler;

        public Keybind(Action handler, ConsoleKey key, ConsoleModifiers mod)
        {
            this.key = key;
            this.mod = mod;
            this.handler = handler;
        }

        public bool Match(ConsoleKeyInfo cki)
        {
            return cki.Key == key && cki.Modifiers == mod;
        }

        public bool Execute(ConsoleKeyInfo cki)
        {
            if (Match(cki) && handler != null)
            {
                handler();
                return true;
            }

            return false;
        }

        //public static void Bind(ConsoleModifiers mod, ConsoleKey key, Action onFire)
        //{

        //}
    }
}
