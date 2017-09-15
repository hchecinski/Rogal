﻿using RogalTutorial.Core;
using RogalTutorial.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogalTutorial.Interfaces
{
    /// <summary>
    /// Interace dla zachowań potworków
    /// </summary>
    public interface IBehavior
    {
        bool Act( Monster monster, CommandSystem commandSystem );
    }
}
