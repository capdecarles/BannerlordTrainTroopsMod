using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Settings.Base.Global;

namespace TrainTroops.Settings
{
    public class MCMTrainTroopsSettings : AttributeGlobalSettings<MCMTrainTroopsSettings>, ISettingsProvider
    {
        public override string Id => nameof(MCMTrainTroopsSettings);

        public override string DisplayName => "Train Troops Settings";

        public override string FolderName => nameof(MCMTrainTroopsSettings);

        public override string FormatType => "json2";

        [SettingPropertyInteger("Troop XP Multiplier", 1, 10, HintText = "(Default 3) The higher this is, the more impact leadership will have on training.", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Global Settings")]
        public int TroopXPMultiplier { get; set; } = 3;

        [SettingPropertyInteger("Level Difference Multiplier", 2, 20, HintText = "(Default 10) The higher this is, the more impact level difference will have on training.", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Global Settings")] 
        public int LevelDifferenceMultiplier { get; set; } = 10;
    }
}
