﻿using RLNET;
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
        public List<Rectangle> Rooms;
        private readonly List<Monster> _monsters;

        public DungeonMap()
        {
            // Initialize the list of rooms when we create a new DungeonMap
            Rooms = new List<Rectangle>();
            _monsters = new List<Monster>();
        }

        public void AddPlayer(Player player)
        {
            Game.Player = player;
            SetIsWalkable(player.X, player.Y, false);
            UpdatePlayerFieldOfView();
        }
        /// <summary>
        /// Metoda rysuje mapę
        /// </summary>
        /// <param name="mapConsole"></param>
        public void Draw(RLConsole mapConsole, RLConsole statConsole)
        {
            mapConsole.Clear();
            foreach(Cell cell in GetAllCells())
            {
                SetConsoleSymbolForCell(mapConsole, cell);
            }

            // Keep an index so we know which position to draw monster stats at
            int i = 0;

            // Iterate through each monster on the map and draw it after drawing the Cells
            foreach (Monster monster in _monsters)
            {
                // When the monster is in the field-of-view also draw their stats
                if (IsInFov(monster.X, monster.Y))
                {
                    monster.Draw(mapConsole, this);

                    // Pass in the index to DrawStats and increment it afterwards
                    monster.DrawStats(statConsole, i);
                    i++;
                }
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

        // Returns true when able to place the Actor on the cell or false otherwise
        public bool SetActorPosition(Actor actor, int x, int y)
        {
            // Only allow actor placement if the cell is walkable
            if (GetCell(x, y).IsWalkable)
            {
                // The cell the actor was previously on is now walkable
                SetIsWalkable(actor.X, actor.Y, true);
                // Update the actor's position
                actor.X = x;
                actor.Y = y;
                // The new cell the actor is on is now not walkable
                SetIsWalkable(actor.X, actor.Y, false);
                // Don't forget to update the field of view if we just repositioned the player
                if (actor is Player)
                {
                    UpdatePlayerFieldOfView();
                }
                return true;
            }
            return false;
        }

        // A helper method for setting the IsWalkable property on a Cell
        public void SetIsWalkable(int x, int y, bool isWalkable)
        {
            Cell cell = GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
        }

        public void AddMonster(Monster monster)
        {
            _monsters.Add(monster);
            // After adding the monster to the map make sure to make the cell not walkable
            SetIsWalkable(monster.X, monster.Y, false);
        }

        // Look for a random location in the room that is walkable.
        public Point GetRandomWalkableLocationInRoom(Rectangle room)
        {
            if (DoesRoomHaveWalkableSpace(room))
            {
                for (int i = 0; i < 100; i++)
                {
                    int x = Game.Random.Next(1, room.Width - 2) + room.X;
                    int y = Game.Random.Next(1, room.Height - 2) + room.Y;
                    if (IsWalkable(x, y))
                    {
                        return new Point(x, y);
                    }
                }
            }

            // If we didn't find a walkable location in the room return null
            return null;
        }

        // Iterate through each Cell in the room and return true if any are walkable
        public bool DoesRoomHaveWalkableSpace(Rectangle room)
        {
            for (int x = 1; x <= room.Width - 2; x++)
            {
                for (int y = 1; y <= room.Height - 2; y++)
                {
                    if (IsWalkable(x + room.X, y + room.Y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
