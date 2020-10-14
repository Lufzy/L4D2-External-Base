using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L4D2Cheat.L4D2.SDK
{
    class Offsets // https://hastebin.com/axulolumix.csharp // 14.10.2020
    {
        // engine.dll
        public static int sv_cheats = 0x6768A0; // def 0
        public static int host_timescale = 0x69AA74; // def 1.0f
        public static int OnFocusGame = 0x437A9C; // 256 - 257
        public static int r_drawentites = 0x607F80;  // def 1
        public static int r_drawleaf = 0x607320;  // def -1
        public static int currentMapArea = 0x68CEB0; // string
        public static int currentMapBSP = 0x68D6B0; // string

        public static int engine_lastCommand = 0x67861A;  // string
        public static int last_requestEvent = 0x67E4B8; // string
        public static int last_responseEvent = 0x68070C; // string

        // client.dll
        public static int sv_infinite_ammo = 0x703A30; // 0, 1, 2
        public static int nb_stop = 0x7B0800; // 0 -1
        public static int fog_enable = 0x785230; // 0 - 1
        public static int cl_viewmodelfovsurvivor = 0x7A22F4; // 51
        public static int viewmodel_fov = 0x782120; // def 50
        public static int cl_drawhud = 0x732578; // 0 - 1
        public static int r_drawrenderboxes = 0x7073B0; // 1 off, 2 hitbox, 3 hitboxPlus
        public static int r_drawothermodels = 0x7050E8; // 1 off 2 
        public static int cl_drawshadowtexture = 0x782D50; // 0 - 1 (for ESP)
        public static int OnConsole = 0x6BA408; // 0 - 1
        public static int OnConsole_copy = 0x78F700; // 0 - 1
        public static int cl_showpos = 0x781948; // 0 - 1
        public static int cl_showfps = 0x781900; // 0 - 1

        public static int crosshair = 0x737A00; // 0 - 1
        public static int cl_crosshair_red = 0x7ABE28; // 0 - 255
        public static int cl_crosshair_green = 0x7ABE70; // 0 - 255
        public static int cl_crosshair_blue = 0x7ABEB8;// 0 - 255
        public static int cl_crosshair_dynamic = 0x7ABF90; // 0 - 1
        public static int cl_crosshair_thickness = 0x7ABF48;

        public static int viewmodel_offset_z = 0x704070;

        // +event = 5, -event = 4, +event after -event = 6
        public static int dwForceJump = 0x739918;
        public static int dwForceAttack = 0x7399F0;
        public static int dwForceAttack2 = 0x7399D8;
        public static int dwForceDuck = 0x7398D0;
        public static int dwForceReload = 0x7398B8; // im not sure for this offset

        public static int OnAir = 0x6C5E80; // ground = 0, air > 1

        public static int dwHealthBase = 0x0071A1C4;
        public static int dwHealthOffset = 0x9A0;

        // server.dll
        public static int god = 0x83E438; // 1 - 0
        public static int buddha = 0x7E87A8; // 1- 0
        public static int server_lastCommand = 0x816659; // string 

        // materialsystem.dll
        public static int mat_fullbright = 0xFE3F0; // 1 - 0
    }
}
