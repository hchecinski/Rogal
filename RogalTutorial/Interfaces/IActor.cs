using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogalTutorial.Interfaces
{
    public interface IActor
    {
        /// <summary>
        /// Jak nazywa się aktor
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Jak dużo widzi
        /// </summary>
        int Awareness { get; set; }
    }
}
