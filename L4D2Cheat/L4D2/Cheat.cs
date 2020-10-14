using L4D2Cheat.L4D2.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Memorys.Memorys;

namespace L4D2Cheat.L4D2
{
    class Cheat
    {
        private static bool Started = false;
        public static Thread cheatThread = new Thread(new ThreadStart(CheatThread));

        public static void CheatThread()
        {
            while(Globals.Client != 0)
            {
                bool OnFocus = Read<int>(Globals.Engine + Offsets.OnFocusGame) == 257 ? true : false;
                bool InConsole = Read<bool>(Globals.Client + Offsets.OnConsole);
                if (OnFocus && !InConsole) // for running in Game and not GameConsole
                {
                    if (Settings.blAutoBunny) // Example feature
                    {
                        bool OnAir = Read<int>(Globals.Client + Offsets.OnAir) == 0 ? false : true;
                        if (Globals.IsKeyPushedDown(Keys.Space)&& !OnAir)
                        {
                            Write<int>(Globals.Client + Offsets.dwForceJump, 6);
                        }
                    }
                }

                Thread.Sleep(10); // for reduce cpu using
            }
        }

        public static void Start()
        {
            if(!Started)
            {
                cheatThread.Start();
                Started = false;
            }
        }
    }
}
