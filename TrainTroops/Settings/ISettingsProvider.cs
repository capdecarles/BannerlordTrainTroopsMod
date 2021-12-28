using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainTroops.Settings
{
    interface ISettingsProvider
    {
        int TroopXPMultiplier { get; set; }

        int LevelDifferenceMultiplier { get; set; }
    }
}
