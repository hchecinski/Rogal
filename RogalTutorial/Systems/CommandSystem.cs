using RLNET;
using RogalTutorial.Core;
using RogalTutorial.Interfaces;
using RogueSharp;
using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogalTutorial.Systems
{
    public class CommandSystem
    {
        public bool IsPlayerTurn { get; set; }

        /// <summary>
        /// Ustaw koniec tury gracza
        /// </summary>
        public void EndPlayerTurn()
        {
            IsPlayerTurn = false;
        }

        /// <summary>
        /// Aktywuje tury potworką dopuki nie będzie tury gracza
        /// </summary>
        public void ActivateMonsters()
        {
            IScheduleable scheduleable = Game.SchedulingSystem.Get();

            // Jeśli pobrany obiekt to gracz, aktywuj jego turę
            // w innym przypadku wykonaj turę potworka
            if (scheduleable is Player)
            {
                IsPlayerTurn = true;
                Game.SchedulingSystem.Add(Game.Player);
            }
            else
            {
                Monster monster = scheduleable as Monster;

                if (monster != null)
                {
                    monster.PerformAction(this);
                    Game.SchedulingSystem.Add(monster);
                }

                ActivateMonsters();
            }
        }

        /// <summary>
        /// Ruch potworka na wskazaną lokację.
        /// Jeśli jest zajęta przez gracza to wykonaj atak.
        /// </summary>
        /// <param name="monster"></param>
        /// <param name="cell"></param>
        public void MoveMonster(Monster monster, Cell cell)
        {
            if (!Game.DungeonMap.SetActorPosition(monster, cell.X, cell.Y))
                if (Game.Player.X == cell.X && Game.Player.Y == cell.Y)
                    Attack(monster, Game.Player);
        }

        /// <summary>
        /// Aktualizuj pozycję gracza
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool MovePlayer(Direction direction)
        {
            int x = Game.Player.X;
            int y = Game.Player.Y;

            switch (direction)
            {
                case Direction.Up:
                    y = Game.Player.Y - 1;
                    break;
                case Direction.Down:
                    y = Game.Player.Y + 1;
                    break;
                case Direction.Left:
                    x = Game.Player.X - 1;
                    break;
                case Direction.Right:
                    x = Game.Player.X + 1;
                    break;
                case Direction.DownLeft:
                    x = Game.Player.X - 1;
                    y = Game.Player.Y + 1;
                    break;
                case Direction.DownRight:
                    x = Game.Player.X + 1;
                    y = Game.Player.Y + 1;
                    break;
                case Direction.Center:
                    x = Game.Player.X;
                    y = Game.Player.Y;
                    break;
                case Direction.UpLeft:
                    x = Game.Player.X - 1;
                    y = Game.Player.Y - 1;
                    break;
                case Direction.UpRight:
                    x = Game.Player.X + 1;
                    y = Game.Player.Y - 1;
                    break;
                default:
                        return false;
            }

            //Jeśli pole jest puste to przesuń gracza
            if (Game.DungeonMap.SetActorPosition(Game.Player, x, y))
                return true;

            Monster monster = Game.DungeonMap.GetMonsterAt(x, y);

            //Jeśli na lokacji znajduję sie potworek to go zaatakuj
            if (monster != null)
            {
                Attack(Game.Player, monster);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Akcja ataku
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        public void Attack(Actor attacker, Actor defender)
        {
            StringBuilder attackMessage = new StringBuilder();
            StringBuilder defenseMessage = new StringBuilder();

            int hits = ResolveAttack(attacker, defender, attackMessage);

            int blocks = ResolveDefense(defender, hits, attackMessage, defenseMessage);

            Game.MessageLog.Add(attackMessage.ToString());
            if (!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
                Game.MessageLog.Add(defenseMessage.ToString());

            int damage = hits - blocks;

            ResolveDamage(defender, damage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <param name="attackMessage"></param>
        /// <returns></returns>
        private int ResolveAttack(Actor attacker, Actor defender, StringBuilder attackMessage)
        {
            int hits = 0;

            attackMessage.AppendFormat("{0} atakuje {1} i rzuca kością: ", attacker.Name, defender.Name);

            // Atak to rzut kością 100ścienną
            DiceExpression attackDice = new DiceExpression().Dice(attacker.Attack, 100);
            DiceResult attackResult = attackDice.Roll();

            // Look at the face value of each single die that was rolled
            foreach (TermResult termResult in attackResult.Results)
            {
                attackMessage.Append(termResult.Value + ", ");
                // Compare the value to 100 minus the attack chance and add a hit if it's greater
                if (termResult.Value >= 100 - attacker.AttackChance)
                {
                    hits++;
                }
            }

            return hits;
        }

        /// <summary>
        /// Akcja bloku
        /// </summary>
        /// <param name="defender"></param>
        /// <param name="hits"></param>
        /// <param name="attackMessage"></param>
        /// <param name="defenseMessage"></param>
        /// <returns></returns>
        private int ResolveDefense(Actor defender, int hits, StringBuilder attackMessage, StringBuilder defenseMessage)
        {
            int blocks = 0;

            if (hits > 0)
            {
                attackMessage.AppendFormat("scoring {0} hits.", hits);
                defenseMessage.AppendFormat("  {0} defends and rolls: ", defender.Name);

                // Roll a number of 100-sided dice equal to the Defense value of the defendering actor
                DiceExpression defenseDice = new DiceExpression().Dice(defender.Defense, 100);
                DiceResult defenseRoll = defenseDice.Roll();

                // Look at the face value of each single die that was rolled
                foreach (TermResult termResult in defenseRoll.Results)
                {
                    defenseMessage.Append(termResult.Value + ", ");
                    // Compare the value to 100 minus the defense chance and add a block if it's greater
                    if (termResult.Value >= 100 - defender.DefenseChance)
                    {
                        blocks++;
                    }
                }
                defenseMessage.AppendFormat("resulting in {0} blocks.", blocks);
            }
            else
            {
                attackMessage.Append("i chybia zupełnie.");
            }

            return blocks;
        }

        /// <summary>
        /// Akcja zadająca obrażenia
        /// </summary>
        /// <param name="defender"></param>
        /// <param name="damage"></param>
        private static void ResolveDamage(Actor defender, int damage)
        {
            if (damage > 0)
            {
                defender.Health = defender.Health - damage;

                Game.MessageLog.Add($"  {defender.Name} was hit for {damage} damage");

                if (defender.Health <= 0)
                {
                    ResolveDeath(defender);
                }
            }
            else
            {
                Game.MessageLog.Add($"  {defender.Name} blocked all damage");
            }
        }

        private static void ResolveDeath(Actor defender)
        {
            if (defender is Player)
            {
                Game.MessageLog.Add($"  {defender.Name} was killed, GAME OVER MAN!");
            }
            else if (defender is Monster)
            {
                Game.DungeonMap.RemoveMonster((Monster)defender);

                Game.MessageLog.Add($"  {defender.Name} died and dropped {defender.Gold} gold");
            }
        }

        public bool CloseDoor(Actor actor, DungeonMap map, RLRootConsole consola)
        {
            List<Door> Doors = new List<Door>();

            Door right = map.GetDoor(actor.X + 1, actor.Y);
            if(right != null) Doors.Add(right);

            Door left = map.GetDoor(actor.X - 1, actor.Y);
            if (left != null) Doors.Add(left);

            Door top = map.GetDoor(actor.X, actor.Y - 1);
            if (top != null) Doors.Add(top);

            Door bottom = map.GetDoor(actor.X, actor.Y + 1);
            if (bottom != null) Doors.Add(bottom);

            Door topRight = map.GetDoor(actor.X + 1, actor.Y - 1);
            if (topRight != null) Doors.Add(topRight);

            Door topLeft = map.GetDoor(actor.X - 1, actor.Y - 1);
            if (topLeft != null) Doors.Add(topLeft);

            Door bottomLeft = map.GetDoor(actor.X - 1, actor.Y + 1);
            if (bottomLeft != null) Doors.Add(bottomLeft);

            Door bottomRigth = map.GetDoor(actor.X + 1, actor.Y + 1);
            if (bottomRigth != null) Doors.Add(bottomRigth);

            if (Doors.Count == 0) return false;
            if (Doors.Count == 1)
            {
                Door door = Doors.FirstOrDefault(d => d.IsOpen);
                if(door != null)
                    map.CloseDoor(actor, door.X, door.Y);

                map.UpdatePlayerFieldOfView();
                return true;
            }
            if (Doors.Count > 1)
            {
                var result = false;
                RLKeyPress keyPress = consola.Keyboard.GetKeyPress();
                if (keyPress != null)
                {
                    switch (keyPress.Key)
                    {
                        case RLKey.Keypad8:
                            if(top != null)
                            {
                                map.CloseDoor(actor, top.X, top.Y);
                                result = true;
                            }
                            break;
                        case RLKey.Keypad2:
                            if (bottom != null)
                            {
                                map.CloseDoor(actor, bottom.X, bottom.Y);
                                result = true;
                            }
                            break;
                        case RLKey.Keypad4:
                            if (left != null)
                            {
                                map.CloseDoor(actor, left.X, left.Y);
                                result = true;
                            }
                            break;
                        case RLKey.Keypad6:
                            if (right != null)
                            {
                                map.CloseDoor(actor, right.X, right.Y);
                                result = true;
                            }
                            break;
                        case RLKey.Keypad7:
                            if (topLeft != null)
                            {
                                map.CloseDoor(actor, topLeft.X, topLeft.Y);
                                result = true;
                            }
                            break;
                        case RLKey.Keypad9:
                            if (topRight != null)
                            {
                                map.CloseDoor(actor, topRight.X, topRight.Y);
                                result = true;
                            }
                            break;
                        case RLKey.Keypad1:
                            if (bottomLeft != null)
                            {
                                map.CloseDoor(actor, bottomLeft.X, bottomLeft.Y);
                                result = true;
                            }
                            break;
                        case RLKey.Keypad3:
                            if (bottomRigth != null)
                            {
                                map.CloseDoor(actor, bottomRigth.X, bottomRigth.Y);
                                result = true;
                            }
                            break;
                    }
                }
                map.UpdatePlayerFieldOfView();
                return result;
            }

            return false;
        }
    }
}
