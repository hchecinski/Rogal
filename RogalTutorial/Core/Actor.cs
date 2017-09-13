using RogalTutorial.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLNET;
using RogueSharp;

namespace RogalTutorial.Core
{
    public class Actor : IActor, IDrawable
    {
        public string Name { get; set; }
        public int Awareness { get; set; }

        public RLColor Color { get; set; }
        public char Symbol { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public void Draw(RLConsole console, IMap map)
        {
            //Nie rysuj postaci jeśli nie została odkryta
            if (!map.GetCell(X, Y).IsExplored)
            {
                return;
            }

            //Rysuj tylko aktorów którzy są widzalni przez bohatera
            if (map.IsInFov(X, Y))
            {
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
            else
            {
                console.Set(X, Y, Colors.Floor, Colors.FloorBackground, '.');
            }
        }
    }
}
