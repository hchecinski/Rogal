using RogalTutorial.Core;
using RogalTutorial.Interfaces;
using RogalTutorial.Systems;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogalTutorial.Behaviors
{
    /// <summary>
    /// Klasa przechowująca podstawowy ruch i aktak
    /// </summary>
    public class StandardMoveAndAttack : IBehavior
    {
        public bool Act(Monster monster, CommandSystem commandSystem)
        {
            DungeonMap dungeonMap = Game.DungeonMap;
            Player player = Game.Player;
            FieldOfView monsterFov = new FieldOfView(dungeonMap);

            // Jeśli potwór nie został ostrzeżony, oblicz pole widzenia
            // Użyj wartości Awareness potwora na odległość FoV
            // Jeśli gracz znajduje się w FoV potwora, to zmień mu status
            // Dodaj wiadomość do MessageLog w odniesieniu do zmiany stanu
            if (!monster.TurnsAlerted.HasValue)
            {
                monsterFov.ComputeFov(monster.X, monster.Y, monster.Awareness, true);
                if (monsterFov.IsInFov(player.X, player.Y))
                {
                    Game.MessageLog.Add($"{monster.Name} rusza do walki z {player.Name}");
                    monster.TurnsAlerted = 1;
                }
            }

            if (monster.TurnsAlerted.HasValue)
            {
                // Przed ruchem potworka ustaw ich pola jako możliwe do poruszania się
                dungeonMap.SetIsWalkable(monster.X, monster.Y, true);
                dungeonMap.SetIsWalkable(player.X, player.Y, true);

                PathFinder pathFinder = new PathFinder(dungeonMap);
                Path path = null;

                try
                {
                    // Spróbuj się ruszyć w kierunku gracza
                    path = pathFinder.ShortestPath(
                    dungeonMap.GetCell(monster.X, monster.Y),
                    dungeonMap.GetCell(player.X, player.Y));
                }
                catch (PathNotFoundException)
                {
                    // Potworek widzi gracza ale ścieżka do niego jest zablokowana
                    // Może to wystąpić np jeśli między graczem a potworkiem są inne potworki
                    Game.MessageLog.Add($"{monster.Name} czeka na ture");
                }

                // Cofnij ustawienie
                dungeonMap.SetIsWalkable(monster.X, monster.Y, false);
                dungeonMap.SetIsWalkable(player.X, player.Y, false);

                // Powiedz klasie commandSystem że potworek się rusza
                if (path != null)
                {
                    try
                    {
                        // TODO: This should be path.StepForward() but there is a bug in RogueSharp V3
                        // The bug is that a Path returned from a PathFinder does not include the source Cell
                        // Wykonaj pierwszy krok
                        commandSystem.MoveMonster(monster, path.Steps.First());
                    }
                    catch (NoMoreStepsException)
                    {
                        Game.MessageLog.Add($"{monster.Name} growls in frustration");
                    }
                }

                monster.TurnsAlerted++;

                // Lose alerted status every 15 turns. 
                // As long as the player is still in FoV the monster will stay alert
                // Otherwise the monster will quit chasing the player.
                if (monster.TurnsAlerted > 15)
                {
                    monster.TurnsAlerted = null;
                }
            }
            return true;
        }
    }
}
