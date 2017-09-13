using RLNET;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogalTutorial.Core
{
    /// <summary>
    /// Klasa która jest narzędziem do rysowania map.
    /// </summary>
    public class DungeonMap : Map
    {
        /// <summary>
        /// Metoda rysuje mapę
        /// </summary>
        /// <param name="mapConsole"></param>
        public void Draw(RLConsole mapConsole)
        {
            mapConsole.Clear();
            foreach(Cell cell in GetAllCells())
            {
                SetConsoleSymbolForCell(mapConsole, cell);
            }
        }

        /// <summary>
        /// Metoda przedstawia w jaki sposób rysuje każdą komurkę mapy
        /// </summary>
        /// <param name="console"></param>
        /// <param name="cell"></param>
        private void SetConsoleSymbolForCell(RLConsole console, Cell cell)
        {
            // Jeśli komurka nie jest odwiedzona to nie rysuj jej
            if (!cell.IsExplored)
            {
                return;
            }

            // Jeśli komurka jest w zasięgu wzroku to rysują ją na jasny kolor
            if (IsInFov(cell.X, cell.Y))
            {
                // Choose the symbol to draw based on if the cell is walkable or not
                // '.' for floor and '#' for walls
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.');
                }
                else
                {
                    console.Set(cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#');
                }
            }
            // When a cell is outside of the field of view draw it with darker colors
            else
            {
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, '.');
                }
                else
                {
                    console.Set(cell.X, cell.Y, Colors.Wall, Colors.WallBackground, '#');
                }
            }
        }

        public void UpdatePlayerFieldOfView()
        {
            Player player = Game.Player;

            ComputeFov(player.X, player.Y, player.Awareness, true);
            foreach(Cell cell in GetAllCells())
            {
                if(IsInFov(cell.X, cell.Y))
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }
        }
    }
}
