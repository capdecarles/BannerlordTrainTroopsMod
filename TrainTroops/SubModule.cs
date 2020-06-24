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
			base.OnGameStart(game, gameStarterObject);

			if (gameStarterObject != null)
			{
				(gameStarterObject as CampaignGameStarter).AddBehavior(new MobilePartyDailyTickBehaviour());
				gameStarterObject.AddModel(new TweakedCombatXpModel());
                Hero.MainHero.AddSkillXp(DefaultSkills.Athletics, 1999);
            }
		}

		private class TweakedCombatXpModel : DefaultCombatXpModel
		{

            public override void GetXpFromHit(CharacterObject attackerTroop, CharacterObject attackedTroop, int damage, bool isFatal, MissionTypeEnum missionType, out int xpAmount)
            {
                base.GetXpFromHit(attackerTroop, attackedTroop, damage, isFatal, missionType, out xpAmount);
                xpAmount *= 15;
            }

            public override float GetXpMultiplierFromShotDifficulty(float shotDifficulty)
            {
                return base.GetXpMultiplierFromShotDifficulty(shotDifficulty * 50);
            }
        }


	}
}

