using RLNET;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogalTutorial.Interfaces
{
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
        /// Mapa jest ważna ponieważ na różnych mapach może być różny zasięg widzenia.
        /// </summary>
        /// <param name="console">która konsola</param>
        /// <param name="map">która mapa</param>
        void Draw(RLConsole console, IMap map);
    }
}
