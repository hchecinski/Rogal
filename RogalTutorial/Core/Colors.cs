using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogalTutorial.Core
{
    /// <summary>
    /// Klasa która łączy zdefiniowane kolory z obiektami w grze
    /// </summary>
    public class Colors
    {
        /// <summary>
        /// Kolor gracza
        /// </summary>
        public static RLColor Player = Swatch.DbLight;

        /// <summary>
        /// Kolor tła dla podłogi
        /// </summary>
        public static RLColor FloorBackground = RLColor.Black;

        /// <summary>
        /// Kolor podłogi
        /// </summary>
        public static RLColor Floor = Swatch.AlternateDarkest;

        /// <summary>
        /// Kolor tła podłogi w obszarze widzenia gracza
        /// </summary>
        public static RLColor FloorBackgroundFov = Swatch.DbDark;

        /// <summary>
        /// Kolor podłogi w obszarze widzenia gracza
        /// </summary>
        public static RLColor FloorFov = Swatch.Alternate;

        /// <summary>
        /// Kolor tła muru
        /// </summary>
        public static RLColor WallBackground = Swatch.SecondaryDarkest;

        /// <summary>
        /// Kolor muru
        /// </summary>
        public static RLColor Wall = Swatch.Secondary;

        /// <summary>
        /// Kolor tła muru w obszarze widzenia gracza
        /// </summary>
        public static RLColor WallBackgroundFov = Swatch.SecondaryDarker;

        /// <summary>
        /// Kolor muru w obszarze widzenia gracza
        /// </summary>
        public static RLColor WallFov = Swatch.SecondaryLighter;
        
        /// <summary>
        /// Kolor nagłówków
        /// </summary>
        public static RLColor TextHeading = RLColor.White;

        /// <summary>
        /// Kolor tekstu
        /// </summary>
        public static RLColor Text = Swatch.DbLight;

        /// <summary>
        /// Kolor złota
        /// </summary>
        public static RLColor Gold = Swatch.DbSun;

        /// <summary>
        /// Kolor dla kobolda
        /// </summary>
        public static RLColor KoboldColor = Swatch.DbBrightWood;

        public static RLColor DoorBackground = Swatch.ComplimentDarkest;
        public static RLColor Door = Swatch.ComplimentLighter;
        public static RLColor DoorBackgroundFov = Swatch.ComplimentDarker;
        public static RLColor DoorFov = Swatch.ComplimentLightest;

    }
}
