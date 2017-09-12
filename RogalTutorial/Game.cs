using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLNET;

namespace RogalTutorial
{
    class Game
    {
        private static RLRootConsole rootConsole;
        private static readonly int SCREEN_WIDTH = 100;
        private static readonly int SCREEN_HIGHT = 70;

        static void Main(string[] args)
        {
            rootConsole = new RLRootConsole("terminal8x8_gs_ro.png", SCREEN_WIDTH, SCREEN_HIGHT, 8, 8, 1, "Rogal tutorial");
            rootConsole.Render += rootConsole_Render;
            rootConsole.Run();

        }

        private static void rootConsole_Render(object sender, UpdateEventArgs e)
        {
            rootConsole.Clear();
            rootConsole.Print(1, 1, "Witaj Świecie", RLColor.White);
            rootConsole.Draw();
        }
    }
}
