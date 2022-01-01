using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainTroops.Settings
{
    class DefaultTrainTroopsSettings : ISettingsProvider
    {
        public int TroopXPMultiplier { get; set; } = 3;
        public int LevelDifferenceMultiplier { get; set; } = 10;
    }
}
