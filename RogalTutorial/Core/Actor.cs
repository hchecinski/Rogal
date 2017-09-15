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
    /// <summary>
    /// Bazowa klasa do tworzenie aktorów w grze
    /// </summary>
    public class Actor : IActor, IDrawable, IScheduleable
    {
        /// <summary>
        /// Obrazenia zadawane przez aktora
        /// </summary>
        private int _attack;

        /// <summary>
        /// Szansa na udany atak przez aktora
        /// </summary>
        private int _attackChance;

        /// <summary>
        /// Zasięg widzenia przez aktora
        /// </summary>
        private int _awareness;

        /// <summary>
        /// Obrona aktora
        /// </summary>
        private int _defense;

        /// <summary>
        /// Szanse na udaną obronę przez aktora
        /// </summary>
        private int _defenseChance;

        /// <summary>
        /// Posiadana ilość złota przez aktora
        /// </summary>
        private int _gold;

        /// <summary>
        /// Aktualny stan zdrowia aktora
        /// </summary>
        private int _health;

        /// <summary>
        /// Maksymalny stan zdrowia aktora
        /// </summary>
        private int _maxHealth;

        /// <summary>
        /// Nazwa aktora
        /// </summary>
        private string _name;

        /// <summary>
        /// Szybkość aktora
        /// </summary>
        private int _speed;

        /// <summary>
        /// Kolor wyświetlanego symbolu aktora
        /// </summary>
        private RLColor _color;

        /// <summary>
        /// Symbol aktora
        /// </summary>
        private char _symbol;

        /// <summary>
        /// Położenie w konsoli symbolu aktora
        /// </summary>
        private int _x;

        /// <summary>
        /// Położenie w konsoli symbolu aktora
        /// </summary>
        private int _y;

        #region ########## Właściwości ##########
        public int Attack
        {
            get
            {
                return _attack;
            }
            set
            {
                _attack = value;
            }
        }

        public int AttackChance
        {
            get
            {
                return _attackChance;
            }
            set
            {
                _attackChance = value;
            }
        }

        public int Awareness
        {
            get
            {
                return _awareness;
            }
            set
            {
                _awareness = value;
            }
        }

        public RLColor Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
            }
        }

        public int Defense
        {
            get
            {
                return _defense;
            }
            set
            {
                _defense = value;
            }
        }

        public int DefenseChance
        {
            get
            {
                return _defenseChance;
            }
            set
            {
                _defenseChance = value;
            }
        }

        public int Gold
        {
            get
            {
                return _gold;
            }
            set
            {
                _gold = value;
            }
        }

        public int Health
        {
            get
            {
                return _health;
            }
            set
            {
                _health = value;
            }
        }

        public int MaxHealth
        {
            get
            {
                return _maxHealth;
            }
            set
            {
                _maxHealth = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public int Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = value;
            }
        }

        public char Symbol
        {
            get
            {
                return _symbol;
            }

            set
            {
                _symbol = value;
            }
        }

        public int X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
            }
        }

        public int Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
            }
        }

        public int Time
        {
            get
            {
                return Speed;
            }
        }
        #endregion

        /// <summary>
        /// Rysowanie aktora na mapie
        /// </summary>
        /// <param name="console"></param>
        /// <param name="map"></param>
        public void Draw(RLConsole console, IMap map)
        {
            //Nie rysuj postaci jeśli nie została odkryta
            if (!map.GetCell(X, Y).IsExplored)
                return;

            //Rysuj tylko aktorów którzy są widzalni przez bohatera
            if (map.IsInFov(X, Y))
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            else
                console.Set(X, Y, Colors.Floor, Colors.FloorBackground, '.');
        }
    }
}
