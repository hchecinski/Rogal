using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogalTutorial.Systems
{
    /// <summary>
    /// Klasa dla logów
    /// </summary>
    public class MessageLog
    {
        /// <summary>
        /// Maksymalna ilość logów
        /// </summary>
        private static readonly int _maxLines = 9;

        /// <summary>
        /// kolejka
        /// </summary>
        private readonly Queue<string> _lines;

        public MessageLog()
        {
            _lines = new Queue<string>();
        }

        /// <summary>
        /// Dodaj wiadomość do kolejki
        /// </summary>
        /// <param name="message"></param>
        public void Add(string message)
        {
            _lines.Enqueue(message);//Dodaje obiekt do końca kolejki

            // Jeśli lini jest za dużo to usuń pierwszy wiersz
            if (_lines.Count > _maxLines)
            {
                _lines.Dequeue();
            }
        }

        /// <summary>
        /// Rysuj linie logów w konsoli
        /// </summary>
        /// <param name="console"></param>
        public void Draw(RLConsole console)
        {
            console.Clear();
            string[] lines = _lines.ToArray();
            for (int i = 0; i < lines.Length; i++)
            {
                console.Print(1, i + 1, lines[i], RLColor.White);
            }
        }
    }
}
