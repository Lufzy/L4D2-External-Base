using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Memorys.Memorys;

namespace L4D2Cheat.L4D2
{
    class Globals
    {
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static int 
            Client = -1, 
            Engine = -1, 
            Server = -1, 
            MaterialSystem = -1;

        public static void Start()
        {
            if(Process.GetProcessesByName("left4dead2").Length != 0) // if "l4d2.exe" running
            {
                Process proc = Process.GetProcessesByName("left4dead2").FirstOrDefault();
                Initialize(proc.Id);
                Globals.Client = new Module(proc, "client.dll").Address;
                Globals.Engine = new Module(proc, "engine.dll").Address;
                Globals.Server = new Module(proc, "server.dll").Address;
                Globals.MaterialSystem = new Module(proc, "materialsystem.dll").Address;

                Cheat.Start();
            }
        }

        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        public static bool IsKeyPushedDown(System.Windows.Forms.Keys vKey)
        {
            return 0 != (GetAsyncKeyState(vKey) & 0x8000);
        }
    }
}
