using RLNET;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogalTutorial.Interfaces
{
    /// <summary>
    /// Interace służący do rysowania na konsoli
    /// </summary>
    public interface IDrawable
    {
        RLColor Color { get; set; }
        char Symbol { get; set; }

        /// <summary>
        /// wspł X
        /// </summary>
        int X { get; set; }

        /// <summary>
        /// wspł Y
        /// </summary>
        int Y { get; set; }

        /// <summary>
        /// Metoda rysująca 
        /// </summary>
        /// <param name="console">która konsola</param>
        /// <param name="map">która mapa</param>
        void Draw(RLConsole console, IMap map);
    }
}
