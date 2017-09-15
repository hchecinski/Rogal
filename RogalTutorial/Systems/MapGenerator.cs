using RogalTutorial.Core;
using RogalTutorial.Monsters;
using RogueSharp;
using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogalTutorial.Systems
{
    /// <summary>
    /// Klasa generująca losową mapę
    /// </summary>
    public class MapGenerator
    {
        /// <summary>
        /// Szerokość mapy
        /// </summary>
        private readonly int _width;

        /// <summary>
        /// Wysokość mapy
        /// </summary>
        private readonly int _height;

        /// <summary>
        /// Maksymalna ilość pokoi na mapie
        /// </summary>
        private readonly int _maxRooms;

        /// <summary>
        /// Maksymalna wielkość pokoju
        /// </summary>
        private readonly int _roomMaxSize;

        /// <summary>
        /// Minimalna wielkość pokoju
        /// </summary>
        private readonly int _roomMinSize;

        /// <summary>
        /// Mapa
        /// </summary>
        private readonly DungeonMap _map;

        public MapGenerator(int width, int height, int maxRooms, int roomMaxSize, int roomMinSize)
        {
            _width = width;
            _height = height;
            _maxRooms = maxRooms;
            _roomMaxSize = roomMaxSize;
            _roomMinSize = roomMinSize;
            _map = new DungeonMap();
        }

        /// <summary>
        /// Ustawia gracza na mapie
        /// </summary>
        private void PlacePlayer()
        {
            Player player = Game.Player;
            if (player == null) player = new Player();

            //Narysuj gracza w centrum pierwszego stworzonego pokoju
            player.X = _map.Rooms[0].Center.X;
            player.Y = _map.Rooms[0].Center.Y;

            _map.AddPlayer(player);
        }

        /// <summary>
        /// Tworzy mapę
        /// </summary>
        /// <returns></returns>
        public DungeonMap CreateMap()
        {
            //Wypełnia mapę obiektami komórek
            _map.Initialize(_width, _height);

            //Tworzy kwadraty które będzie można zmienić w pokoje
            for (int r = _maxRooms; r > 0; r--)
            {
                // Determine the size and position of the room randomly
                int roomWidth = Game.Random.Next(_roomMinSize, _roomMaxSize);
                int roomHeight = Game.Random.Next(_roomMinSize, _roomMaxSize);
                int roomXPosition = Game.Random.Next(0, _width - roomWidth - 1);
                int roomYPosition = Game.Random.Next(0, _height - roomHeight - 1);

                // Wszystkie pokoje reprezentowane są jako prostokąty
                var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);

                // Jeśli się nie przecina to dodaj pokój do listy pokojów w mapie
                if (!_map.Rooms.Any(room => newRoom.Intersects(room))) _map.Rooms.Add(newRoom);
            }

            // Zamienia stworzone kwadraty w pokoje
            foreach (Rectangle room in _map.Rooms) CreateRoom(room);

            // Iteruję się po stworzonych pokojach
            for (int r = 1; r < _map.Rooms.Count; r++)
            {
                // Znajduje srodek pokoju porzedniego i następnego
                int previousRoomCenterX = _map.Rooms[r - 1].Center.X;
                int previousRoomCenterY = _map.Rooms[r - 1].Center.Y;
                int currentRoomCenterX = _map.Rooms[r].Center.X;
                int currentRoomCenterY = _map.Rooms[r].Center.Y;

                // Daj 50% na stworzenie korytarza w kształcie L
                if (Game.Random.Next(1, 2) == 1)
                {
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, previousRoomCenterY);
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, currentRoomCenterX);
                }
                else
                {
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, previousRoomCenterX);
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, currentRoomCenterY);
                }
            }

            foreach (Rectangle room in _map.Rooms) CreateDoor(room);

            //Ustaw aktorów
            PlacePlayer();
            PlaceMonsters();

            return _map;
        }

        /// <summary>
        /// Ustaw podłogę w pokojach
        /// </summary>
        /// <param name="room"></param>
        private void CreateRoom(Rectangle room)
        {
            for (int x = room.Left + 1; x < room.Right; x++)
                for (int y = room.Top + 1; y < room.Bottom; y++)
                    _map.SetCellProperties(x, y, true, true, false);
        }

        /// <summary>
        /// Zbuduj tunel poziomy
        /// </summary>
        /// <param name="xStart"></param>
        /// <param name="xEnd"></param>
        /// <param name="yPosition"></param>
        private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
        {
            for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
                _map.SetCellProperties(x, yPosition, true, true);
        }

        /// <summary>
        /// Zbuduj tunel pionowy
        /// </summary>
        /// <param name="yStart"></param>
        /// <param name="yEnd"></param>
        /// <param name="xPosition"></param>
        private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
                _map.SetCellProperties(xPosition, y, true, true);
        }

        /// <summary>
        /// Metoda ustawia potwora
        /// </summary>
        private void PlaceMonsters()
        {
            foreach (var room in _map.Rooms)
            {
                //Każde pomieszczenie ma 60% szans na stworzenie potworków
                if (Dice.Roll("1D10") < 7)
                {
                    // Stwórz mierzy 1 -> 4 potworki
                    var numberOfMonsters = Dice.Roll("1D4");
                    for (int i = 0; i < numberOfMonsters; i++)
                    {
                        // Znajdz możliwe miejsce do poruszania się w pokoju
                        Point randomRoomLocation = _map.GetRandomWalkableLocationInRoom(room);
                        if (randomRoomLocation != null)
                        {
                            var monster = Kobold.Create(1);
                            monster.X = randomRoomLocation.X;
                            monster.Y = randomRoomLocation.Y;
                            _map.AddMonster(monster);
                        }
                    }
                }
            }
        }

        private void CreateDoor(Rectangle room)
        {
            // Pobranie krawędzi pokoju
            int xMin = room.Left;
            int xMax = room.Right;
            int yMin = room.Top;
            int yMax = room.Bottom;

            // W granicach pokoju włóż opowiednie komórki
            List<Cell> borderCells = _map.GetCellsAlongLine(xMin, yMin, xMax, yMin).ToList();
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMax, yMin, xMax, yMax));

            foreach (Cell cell in borderCells)
            {
                if (IsPotentialDoor(cell))
                {
                    _map.SetCellProperties(cell.X, cell.Y, false, true);
                    _map.Doors.Add(new Door { X = cell.X, Y = cell.Y, IsOpen = false });
                }
            }
        }

        private bool IsPotentialDoor( Cell cell)
        {
            if (!cell.IsWalkable) return false;

            Cell right = _map.GetCell(cell.X + 1, cell.Y);
            Cell left = _map.GetCell(cell.X - 1, cell.Y);
            Cell top = _map.GetCell(cell.X, cell.Y - 1);
            Cell bottom = _map.GetCell(cell.X, cell.Y + 1);

            // Sprawdzamy czy nie ma tutaj drzwi
            if ( _map.GetDoor(cell.X, cell.Y) != null ||
                _map.GetDoor(right.X, right.Y) != null ||
                _map.GetDoor(left.X, left.Y) != null ||
                _map.GetDoor(top.X, top.Y) != null ||
                _map.GetDoor(bottom.X, bottom.Y) != null ) return false;

            // Dobre miejse na wstawienie drzwi z lewej lub prawej strony
            if (right.IsWalkable && left.IsWalkable && !top.IsWalkable && !bottom.IsWalkable) return true;

            // Dobre miejse na wstawienie drzwi u góry lub dole
            if (!right.IsWalkable && !left.IsWalkable && top.IsWalkable && bottom.IsWalkable) return true;

            return false;
        }
    }
}
