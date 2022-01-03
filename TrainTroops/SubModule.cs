using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace TrainTroops
{
    public class SubModule : MBSubModuleBase
    {
		//TODO: add behaviour on game load to make save-compatible?

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{

			if (!(game.GameType is Campaign))
				return;

			if (gameStarterObject is CampaignGameStarter)
				(gameStarterObject as CampaignGameStarter).AddBehavior(new MobilePartyDailyTickBehaviour());

		}

	}
}

