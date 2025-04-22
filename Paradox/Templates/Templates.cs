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

        public static string StatsTemplate()
        {
            return @"## Character Stats
These are your base stats, influenced by any items you're carrying. They will affect the outcome of your actions.

### Strength (STR): 8
> Affects your physical power, ability to lift objects, and close-quarters combat.

### Agility (AGI): 28
> Determines your speed, reflexes, and ability to dodge attacks.

### Dexterity (DEX): 45
> Impacts your precision, your skill with ranged weapons, and ability to handle delicate tasks.

### Hit Points (HP): 25 / 25
> Your current health. Lose all HP, and your journey ends...

> Note: These stats are modified by equipped items. Currently, only your base stats are shown.

> Do you wish to review your inventory or take another action?";
        }
    }
}
