using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainTroops.Settings;

namespace TrainTroops
{
    class TrainTroopsSettings
    {
        ISettingsProvider _provider;
        public int TroopXPMultiplier { get => _provider.TroopXPMultiplier; set => _provider.TroopXPMultiplier = value; }
        public int LevelDifferenceMultiplier { get => _provider.LevelDifferenceMultiplier; set => _provider.LevelDifferenceMultiplier = value; }

        static TrainTroopsSettings _instance;
        public static TrainTroopsSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TrainTroopsSettings();

                return _instance;
            }
        }

        public TrainTroopsSettings()
        {
            ISettingsProvider mcm = null;
            // MCM as a soft dependency
            try
            {
                mcm = MCMTrainTroopsSettings.Instance;
            }
            catch { }
            if (mcm != null)
                _provider = mcm;
            else
                _provider = new MCMTrainTroopsSettings();
        }
    }
}
