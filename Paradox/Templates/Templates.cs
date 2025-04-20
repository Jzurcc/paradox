using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox.Templates
{
    class Templates
    {
        public static string ItemsTemplate()
        {
            return @"## Item Name
---------------------------------------
*Came from the fucking depths of 
the deepest level of hell. You will 
make all horses act in heat*
---------------------------------------
STR: +12      AGI: +10      DEX: +25
---------------------------------------

## Item Name
---------------------------------------
*Came from the fucking depths of 
the deepest level of hell. You will 
make all horses act in heat*
---------------------------------------
STR: +12      AGI: +10      DEX: +25
---------------------------------------";
        }
    }
}
