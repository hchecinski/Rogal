using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLNET;
using RogalTutorial.Core;
using RogalTutorial.Systems;
using RogueSharp.Random;

namespace RogalTutorial
{
    /// <summary>
    /// Główna klasa gry
    /// </summary>
    class Game
    {
        #region
        // Wielkość okna gry
        private static readonly int _screenWidth = 100;
        private static readonly int _screenHeight = 70;
        private static RLRootConsole _rootConsole;

        // The map console takes up most of the screen and is where the map will be drawn
        private static readonly int _mapWidth = 80;
        private static readonly int _mapHeight = 48;
        private static RLConsole _mapConsole;

        // Below the map console is the message console which displays attack rolls and other information
        private static readonly int _messageWidth = 80;
        private static readonly int _messageHeight = 11;
        private static RLConsole _messageConsole;

        // The stat console is to the right of the map and display player and monster stats
        private static readonly int _statWidth = 20;
        private static readonly int _statHeight = 70;
        private static RLConsole _statConsole;

        // Above the map is the inventory console which shows the players equipment, abilities, and items
        private static readonly int _inventoryWidth = 80;
        private static readonly int _inventoryHeight = 11;
        private static RLConsole _inventoryConsole;

        private static bool _renderRequired = true;
        public static Player Player { get; set; }
        public static DungeonMap DungeonMap { get; private set; }
        public static CommandSystem CommandSystem { get; private set; }
        public static MessageLog MessageLog { get; private set; }
        public static SchedulingSystem SchedulingSystem { get; private set; }

        // We can use this instance of IRandom throughout our game when generating random number
        public static IRandom Random { get; private set; }
        #endregion

        public static void Main()
        {
            #region ########## Ustawienie ziarna dla wielkości losowej ##########
            int seed = (int)DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);
            #endregion

            // Stworzenie harmonogramu aby wiedzieć kto kiedy wykonuje swoją turę
            SchedulingSystem = new SchedulingSystem();

            // Ustawienia konsol
            _rootConsole = new RLRootConsole("terminal8x8.png", _screenWidth, _screenHeight, 8, 8, 1f, $"RougeSharp V3 Tutorial");

            _mapConsole = new RLConsole(_mapWidth, _mapHeight);
            _messageConsole = new RLConsole(_messageWidth, _messageHeight);
            _statConsole = new RLConsole(_statWidth, _statHeight);
            _inventoryConsole = new RLConsole(_inventoryWidth, _inventoryHeight);
            
            // Tworzenie mapy
            MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7);
            DungeonMap = mapGenerator.CreateMap();
            DungeonMap.UpdatePlayerFieldOfView();

            CommandSystem = new CommandSystem();

            #region ########## Ustawienie konsol ##########
            // Set up a handler for RLNET's Update event
            _rootConsole.Update += OnRootConsoleUpdate;

            // Set up a handler for RLNET's Render event
            _rootConsole.Render += OnRootConsoleRender;

            MessageLog = new MessageLog();
            MessageLog.Add("The rogue arrives on level 1");
            MessageLog.Add($"Level created with seed '{seed}'");

            // Set background color and text for each console so that we can verify they are in the correct positions
            _messageConsole.SetBackColor(0, 0, _messageWidth, _messageHeight, Swatch.DbDeepWater);

            //_statConsole.SetBackColor(0, 0, _statWidth, _statHeight, Swatch.DbOldStone);
            //_statConsole.Print(1, 1, "Stats", Colors.TextHeading);

            _inventoryConsole.SetBackColor(0, 0, _inventoryWidth, _inventoryHeight, Swatch.DbWood);
            _inventoryConsole.Print(1, 1, "Inventory", Colors.TextHeading);
            #endregion
            
            _rootConsole.Run();
        }

        /// <summary>
        /// Akcja przyjmuje input od gracza oraz zarządza turami
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            bool didPlayerAct = false;
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
            if (CommandSystem.IsPlayerTurn)
            {
                if (keyPress != null)
                {
                    #region ########## Klawisze poruszania się ##########
                    if (keyPress.Key == RLKey.Up || keyPress.Key == RLKey.Keypad8)
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
                    }
                    else if (keyPress.Key == RLKey.Down || keyPress.Key == RLKey.Keypad2)
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
                    }
                    else if (keyPress.Key == RLKey.Left || keyPress.Key == RLKey.Keypad4)
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
                    }
                    else if (keyPress.Key == RLKey.Right || keyPress.Key == RLKey.Keypad6)
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
                    }
                    else if (keyPress.Key == RLKey.Keypad7)
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.UpLeft);
                    }
                    else if (keyPress.Key == RLKey.Keypad9)
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.UpRight);
                    }
                    else if (keyPress.Key == RLKey.Keypad1)
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.DownLeft);
                    }
                    else if (keyPress.Key == RLKey.Keypad3)
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.DownRight);
                    }

                    #endregion

                    else if (keyPress.Key == RLKey.Escape)
                    {
                        _rootConsole.Close();
                    }
                    else if (keyPress.Key == RLKey.C)
                    {
                        didPlayerAct = CommandSystem.CloseDoor(Player, DungeonMap, _rootConsole);
                    }
                }

                if (didPlayerAct)
                {
                    _renderRequired = true;
                    CommandSystem.EndPlayerTurn();
                }
            }
            else
            {
                CommandSystem.ActivateMonsters();
                _renderRequired = true;
            }
        }
        
        /// <summary>
        /// Akcja renderująca
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            // Rysuj jeśli skończyła się tura potworków albo gracza
            if (_renderRequired)
            {
                _mapConsole.Clear();
                _statConsole.Clear();
                _messageConsole.Clear();

                DungeonMap.Draw(_mapConsole, _statConsole);
                Player.Draw(_mapConsole, DungeonMap);
                MessageLog.Draw(_messageConsole);
                Player.DrawStats(_statConsole);

                // Ułóż konsole w oknie gry
                RLConsole.Blit(_mapConsole, 0, 0, _mapWidth, _mapHeight, _rootConsole, 0, _inventoryHeight);
                RLConsole.Blit(_messageConsole, 0, 0, _messageWidth, _messageHeight, _rootConsole, 0, _screenHeight - _messageHeight);
                RLConsole.Blit(_statConsole, 0, 0, _statWidth, _statHeight, _rootConsole, _mapWidth, 0);
                RLConsole.Blit(_inventoryConsole, 0, 0, _inventoryWidth, _inventoryHeight, _rootConsole, 0, 0);

                // Tell RLNET to draw the console that we set
                _rootConsole.Draw();

                _renderRequired = false;
            }
        }
    }
}
