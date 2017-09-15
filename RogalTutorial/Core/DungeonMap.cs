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
    /// Klasa narzędziowa do rysowania oraz obsługi map.
    /// </summary>
    public class DungeonMap : Map
    {
        #region ########## Pola ##########
        /// <summary>
        /// Lista pokoi na mapie
        /// </summary>
        public List<Rectangle> Rooms;

        /// <summary>
        /// Lista potworów na mapie
        /// </summary>
        private readonly List<Monster> _monsters;

        public List<Door> Doors;
        #endregion

        /// <summary>
        /// Konstruktor który inicjalizuje pola
        /// </summary>
        public DungeonMap()
        {
            Rooms = new List<Rectangle>();
            _monsters = new List<Monster>();
            Doors = new List<Door>();
        }

        /// <summary>
        /// Metoda tworzy gracza na mapie 
        /// </summary>
        /// <param name="player">Obiekt gracza</param>
        public void AddPlayer(Player player)
        {
            Game.Player = player;
            SetIsWalkable(player.X, player.Y, false); // Ustawienie pola Zajętego przez gracza na pole możliwe do poruszania się
            UpdatePlayerFieldOfView(); // Narysowanie obszaru widzenia gracza
            Game.SchedulingSystem.Add(player); // Zapisanie gracza do systemu poruszania się
        }

        /// <summary>
        /// Metoda rysująca mapę
        /// </summary>
        /// <param name="mapConsole">Konsola na której będzie rysowana mapa</param>
        /// <param name="statConsole">Konsola opisująca obiekty na mapie które widzi gracz</param>
        public void Draw(RLConsole mapConsole, RLConsole statConsole)
        {
            mapConsole.Clear();

            // Pobierz każdą komurkę mapy oraz narysuj symbol przydzielony do niej
            foreach (Cell cell in GetAllCells()) 
                SetConsoleSymbolForCell(mapConsole, cell);

            // Liczy rzędy w konoli statystyk aby wypisać w niej potworki
            int i = 0; 

            // Pętla leci po potworach w liście potworów 
            foreach (Monster monster in _monsters)
                // Jeśli potwór jest w obszarze widzenia gracza to narysuj go na mapie oraz podaj jego statystyki
                if (IsInFov(monster.X, monster.Y)) 
                {
                    // Pass in the index to DrawStats and increment it afterwards
                    monster.Draw(mapConsole, this);
                    monster.DrawStats(statConsole, i);
                    i++;
                }

            // Rysujemy drzwi na mapie
            foreach(Door door in Doors)
                door.Draw(mapConsole, this);
        }

        /// <summary>
        /// Metoda przedstawia w jaki sposób rysuje każdą komórkę mapy
        /// </summary>
        /// <param name="console">Konsola mapy</param>
        /// <param name="cell">Komórka w mapie</param>
        private void SetConsoleSymbolForCell(RLConsole console, Cell cell)
        {
            // Sprawdza czy komórka została już odkryta przez gracza
            if (!cell.IsExplored)
                return;

            // Jeśli komórka jest w zasięgu wzroku to rysują ją na jasny kolor
            if (IsInFov(cell.X, cell.Y))
            {
                // Jeśli po komurce możesz chodzić rysują jako podłogą '.', a jeśli nie to jako ścianę '#'
                if (cell.IsWalkable)
                    console.Set(cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.');
                else
                    console.Set(cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#');
            }
            // Jeśli komórka nie jest w obszarze widoku gracza to rysują ją w innych kolorach
            else
            {
                if (cell.IsWalkable)
                    console.Set(cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, '.');
                else
                {
                    var monster = GetMonsterAt(cell.X, cell.Y);
                    if(monster != null)
                        console.Set(cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, '.');
                    else
                        console.Set(cell.X, cell.Y, Colors.Wall, Colors.WallBackground, '#');
                }
            }
        }

        /// <summary>
        /// Aktualizuje obszar widoku dla gracza
        /// </summary>
        public void UpdatePlayerFieldOfView()
        {
            Player player = Game.Player;

            // Funkcja oblicza pole widoku dla gracza
            ComputeFov(player.X, player.Y, player.Awareness, true);

            // Pobiera każdą komórkę z mapy i ustawia jej właściwości
            foreach(Cell cell in GetAllCells())
                if(IsInFov(cell.X, cell.Y))
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
        }

        /// <summary>
        /// Zwraca wartość true, jeśli jest w stanie umieścić aktor na komórce lub fałsz w przeciwnym razie
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool SetActorPosition(Actor actor, int x, int y)
        {
            // Dopuszcza się umieszczenie aktorów, jeśli można wejść na komórkę
            if (GetCell(x, y).IsWalkable)
            {
                // Komórka, na której aktor była poprzednio, ustaw na możliwą do zajęcia
                SetIsWalkable(actor.X, actor.Y, true);
                // Aktualizuj wspórzędne aktora
                actor.X = x;
                actor.Y = y;
                // Tylko jeden aktor może byc na komórce
                SetIsWalkable(actor.X, actor.Y, false);

                // Próbuje otworzyć drzwi
                OpenDoor(actor, x, y);

                // Aktualizuj obszar widoku gracza dla nowej pozycji
                if (actor is Player)
                    UpdatePlayerFieldOfView();

                return true;
            }
            return false;
        }

        /// <summary>
        /// Metoda pomocnicza. Ustawia możliwośc wejścia na komórkę o podanych współrzędnych
        /// </summary>
        /// <param name="x">Wspórzędna x komórki</param>
        /// <param name="y">Wspórzędna y komórki</param>
        /// <param name="isWalkable">Ustaw czy można wejść na komórkę o podanych współrzędnych</param>
        public void SetIsWalkable(int x, int y, bool isWalkable)
        {
            Cell cell = GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
        }

        /// <summary>
        /// Poszukaj losowej lokacji w pokoju na którą można wejść
        /// </summary>
        /// <param name="room">Prostokąt który reprezentuje pokój</param>
        /// <returns>
        /// Jeśli nie znaleziono lokacji zwróć null, w innym przypadku zwróć obiekt Point
        /// </returns>
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
            
            return null;
        }

        /// <summary>
        /// Iteruj przez każdą komórkę w pokoju i zwróć prawdę, jeśli są one walkable
        /// </summary>
        /// <param name="room">Prostokąt który reprezentuje pokój</param>
        /// <returns></returns>
        public bool DoesRoomHaveWalkableSpace(Rectangle room)
        {
            for (int x = 1; x <= room.Width - 2; x++)
                for (int y = 1; y <= room.Height - 2; y++)
                    if (IsWalkable(x + room.X, y + room.Y))
                        return true;
            return false;
        }

        #region ########## Zarządzanie Potworkami na mapie ##########
        /// <summary>
        /// Dodaj potwora do listy potworów na aktualnej mapie
        /// </summary>
        /// <param name="monster"></param>
        public void AddMonster(Monster monster)
        {
            _monsters.Add(monster);
            // Po dodaniu potwora do mapy ustaw komórkę na której się znajduje
            // na niemożliwą do zajęcia przez innego aktora
            SetIsWalkable(monster.X, monster.Y, false);
            Game.SchedulingSystem.Add(monster);
        }

        /// <summary>
        /// Usuń potworka z mapy jeśli go już zabiłeś
        /// </summary>
        /// <param name="monster"></param>
        public void RemoveMonster(Monster monster)
        {
            _monsters.Remove(monster);
            // After removing the monster from the map, make sure the cell is walkable again
            SetIsWalkable(monster.X, monster.Y, true);
            Game.SchedulingSystem.Remove(monster);
        }

        /// <summary>
        /// Pobierz potworka znajdującego się w lokacji o wspł. x i y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Monster GetMonsterAt(int x, int y)
        {
            return _monsters.FirstOrDefault(m => m.X == x && m.Y == y);
        }
        #endregion

        #region ########## Obsługa drzwi ##########
        public Door GetDoor(int x, int y)
        {
            return Doors.SingleOrDefault(d => d.X == x && d.Y == y);
        }

        public void OpenDoor( Actor actor, int x, int y)
        {
            Door door = GetDoor(x, y);
            if(door != null && !door.IsOpen)
            {
                door.IsOpen = true;
                var cell = GetCell(x, y);
                SetCellProperties(x, y, true, cell.IsWalkable, cell.IsExplored);
            }
        }

        public void CloseDoor(Actor actor, int x, int y)
        {
            Door door = GetDoor(x, y);
            if (door != null && door.IsOpen)
            {
                door.IsOpen = false;
                var cell = GetCell(x, y);
                SetCellProperties(x, y, false, cell.IsWalkable, cell.IsExplored);
            }
        }
        #endregion
    }
}
