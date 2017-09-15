using RLNET;
using RogalTutorial.Behaviors;
using RogalTutorial.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogalTutorial.Core
{
    /// <summary>
    /// Bazowa klasa dla potworków
    /// </summary>
    public class Monster : Actor
    {
        public int? TurnsAlerted { get; set; }

        public virtual void PerformAction(CommandSystem commandSystem)
        {
            var behavior = new StandardMoveAndAttack();
            behavior.Act(this, commandSystem);
        }

        /// <summary>
        /// Rysuj potworka na konsoli ze statystykami
        /// </summary>
        /// <param name="statConsole"></param>
        /// <param name="position"></param>
        public void DrawStats(RLConsole statConsole, int position)
        {
            // Określa wspórzędną Y na konsoli
            int yPosition = 13 + (position * 2);

            // Rysuje symbol potworka
            statConsole.Print(1, yPosition, Symbol.ToString(), Color);

            // Szerokość paska ze zdrowiem potworka
            int width = Convert.ToInt32(((double)Health / (double)MaxHealth) * 16.0);
            int remainingWidth = 16 - width;

            // Rysuje kolor paska oraz tła;
            statConsole.SetBackColor(3, yPosition, width, 1, Swatch.Primary);
            statConsole.SetBackColor(3 + width, yPosition, remainingWidth, 1, Swatch.PrimaryDarkest);

            // Ustawia nazwę potworka na pasku
            statConsole.Print(2, yPosition, $": {Name}", Swatch.DbLight);
        }
    }
}
