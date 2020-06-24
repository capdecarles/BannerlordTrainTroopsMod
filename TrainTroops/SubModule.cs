using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using System;
using TaleWorlds.CampaignSystem;

namespace TrainTroops
{
    public class SubModule : MBSubModuleBase
    {

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			base.OnGameStart(game, gameStarterObject);

			if (gameStarterObject != null)
			{
				(gameStarterObject as CampaignGameStarter).AddBehavior(new MobilePartyDailyTickBehaviour());
				System.Console.WriteLine("Train troops behaviour added ;)");
			}
		}

	}
}

