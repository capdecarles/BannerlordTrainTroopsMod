using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Localization;

namespace TrainTroops
{
    class MobilePartyDailyTickBehaviour : CampaignBehaviorBase
    {

        private int troopXPMultiplier = 3;
        private int levelDifferenceMultiplier = 10;
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new System.Action<MobileParty>(this.addXp));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void addXp(MobileParty party)
        {
            //Only train troops in main hero party to alleviate CPU load
            if (party.IsActive && !party.IsLeaderless && party.IsMainParty && party.LeaderHero != null && party.LeaderHero == Hero.MainHero)
            {

                int totalXPEarned = 0;
                int troopsReadyToUpgradeCount = 0;
                Dictionary<string, int> troopsReadyToUpgrade = new Dictionary<string, int>();
                for (int i = 0; i < party.MemberRoster.Count; i++)
                {
                    CharacterObject troop = party.MemberRoster.GetCharacterAtIndex(i);
                    //Only gain XP if character LVL is lower than the leader's LVL
                    //if (troop.Level < Hero.MainHero.Level)
                    //{
                    int leaderLeadership = Hero.MainHero.GetSkillValue(DefaultSkills.Leadership);
                    int lvlDifference = Hero.MainHero.Level - troop.Level;
                    //Gain XP: leadership skill * 3 + level difference * 10
                    int xpEarned = leaderLeadership * troopXPMultiplier + lvlDifference * levelDifferenceMultiplier;
                    party.Party.MemberRoster.AddXpToTroopAtIndex(xpEarned, i);
                    //Is troop now ready to upgrade?
                    if (troop.GetXpValue() > troop.UpgradeXpCost)
                    {
                        troopsReadyToUpgradeCount++;
                        //TODO: get the localized troop name, for now it only gets it in english
                        string troopName = LocalizedTextManager.GetTranslatedText(LocalizedTextManager.DefaultEnglishLanguageId, troop.Name.GetID());
                        //Count how many troops of each type are ready to upgrade
                        if (troopsReadyToUpgrade.ContainsKey(troopName))
                        {
                            troopsReadyToUpgrade[troopName]++;
                        }
                        else
                        {
                            troopsReadyToUpgrade.Add(troopName, 0);
                        }
                    }

                    totalXPEarned += xpEarned;
                }
                //}
                string troopsReadyToUpgradeMessage = "";
                if (troopsReadyToUpgrade.Count != 0)
                {
                    troopsReadyToUpgradeMessage += " (";
                    for (int i = 0; i < troopsReadyToUpgrade.Count; i++)
                    {
                        troopsReadyToUpgradeMessage += troopsReadyToUpgrade.Keys.ElementAt(i) + ": " + troopsReadyToUpgrade[troopsReadyToUpgrade.Keys.ElementAt(i)];
                        if (i != troopsReadyToUpgrade.Count - 1)
                        {
                            troopsReadyToUpgradeMessage += ", ";
                        }
                    }
                    troopsReadyToUpgradeMessage += ")";
                }
                InformationManager.DisplayMessage(new InformationMessage("Total training XP for the day: " + totalXPEarned + troopsReadyToUpgrade));
            }

        }


    }
}

/*
        private void addXp(MobileParty party)
        {
            if (!party.IsActive)
                return;

            bool hasCombatTips = party.HasPerk(DefaultPerks.Leadership.CombatTips);
            bool hasRaiseTheMeek = party.HasPerk(DefaultPerks.Leadership.RaiseTheMeek);

            if (!hasCombatTips && !hasRaiseTheMeek)
                return;

            for (int i = 0; i < party.MemberRoster.Count; i++)
            {
                TroopRosterElement troopElement = party.MemberRoster.GetElementCopyAtIndex(i);
                int troopMultiplier = troopElement.Number;
                int totalTroopXp = 0;

                // If we scaleByReadyToUpgrade, ignore the amount of units ready to upgrade
                if (ConfigLoader.Instance.Config.ScaleByReadyToUpgrade)
                {
                    troopMultiplier -= troopElement.NumberReadyToUpgrade;
                }

                if (hasCombatTips)
                {
                    // Add combatTips xp
                    totalTroopXp += ConfigLoader.Instance.Config.CombatTipsXpAmount * troopMultiplier;

                    // Remove the default added xp
                    totalTroopXp -= Campaign.Current.Models.PartyTrainingModel.GetPerkExperiencesForTroops(DefaultPerks.Leadership.CombatTips);
                }

                if (hasRaiseTheMeek && troopElement.Character.Tier < 4)
                {
                    // Add raiseTheMeek xp
                    totalTroopXp += ConfigLoader.Instance.Config.RaiseTheMeekXpAmount * troopMultiplier;

                    // Remove the default added xp only if we haven't removed it for CombatTips, native doesn't support both
                    // even if its technically possible as party can have multiple leaders. It only applies CombatTips instead.
                    if (!hasCombatTips)
                    {
                        totalTroopXp -= Campaign.Current.Models.PartyTrainingModel.GetPerkExperiencesForTroops(DefaultPerks.Leadership.RaiseTheMeek);
                    }
                }

                // Add xp to the troop
                party.Party.MemberRoster.AddXpToTroopAtIndex(totalTroopXp, i);
            } 
        }*/
