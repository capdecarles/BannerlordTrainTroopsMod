using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Localization;
using SandBox.GauntletUI.Map;

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
                Dictionary<string, int> troopsReadyToUpgrade = new Dictionary<string, int>();
                for (int i = 0; i < party.MemberRoster.Count; i++)
                {
                    TroopRosterElement troop = party.MemberRoster.GetElementCopyAtIndex(i);
                    //Only gain XP if character LVL is lower than the leader's LVL
                    if (troop.Character.Level < Hero.MainHero.Level)
                    {
                        int leaderLeadership = Hero.MainHero.GetSkillValue(DefaultSkills.Leadership);
                        int lvlDifference = Hero.MainHero.Level - troop.Character.Level;
                        int trainableTroopCount = troop.Number - troop.NumberReadyToUpgrade;

                        //Gain XP: leadership skill * 3 + level difference * 10
                        Logger("Troop: " + LocalizedTextManager.GetTranslatedText(LocalizedTextManager.DefaultEnglishLanguageId, troop.Character.Name.GetID()) + " Leadership: " + leaderLeadership + " xpEarnedByLevel: " + leaderLeadership * troopXPMultiplier + " xpByLevelDifference: " + lvlDifference * levelDifferenceMultiplier + " trainable troop count: " + trainableTroopCount);
                        
                        //Perform the math
                        int xpEarned = (leaderLeadership * troopXPMultiplier + lvlDifference * levelDifferenceMultiplier) * trainableTroopCount;
                        party.Party.MemberRoster.AddXpToTroopAtIndex(xpEarned, i);
                        //Report troops ready to upgrade
                        if (troop.NumberReadyToUpgrade != 0)
                        {
                            //TODO: get the localized troop name, for now it only gets it in english
                            string troopName = LocalizedTextManager.GetTranslatedText(LocalizedTextManager.DefaultEnglishLanguageId, troop.Character.Name.GetID());
                            //Count how many troops of each type are ready to upgrade
                            troopsReadyToUpgrade.Add(troopName, troop.NumberReadyToUpgrade);
                        }

                        totalXPEarned += xpEarned;
                    }
                }

                InformationManager.DisplayMessage(new InformationMessage("Total training XP for the day: " + totalXPEarned + getTroopsReadyToUpgradeMessage(troopsReadyToUpgrade)));
            }

        }

        private static string getTroopsReadyToUpgradeMessage(Dictionary<string, int> troopsReadyToUpgrade)
        {
            string troopsReadyToUpgradeMessage = "";
            if (troopsReadyToUpgrade.Count != 0)
            {
                troopsReadyToUpgradeMessage += " Troops ready to upgrade: ";
                for (int i = 0; i < troopsReadyToUpgrade.Count; i++)
                {
                    troopsReadyToUpgradeMessage += troopsReadyToUpgrade.Keys.ElementAt(i) + ": " + troopsReadyToUpgrade[troopsReadyToUpgrade.Keys.ElementAt(i)];
                    if (i != troopsReadyToUpgrade.Count - 1)
                    {
                        troopsReadyToUpgradeMessage += ", ";
                    }
                }
                troopsReadyToUpgradeMessage += ".";
            }
            return troopsReadyToUpgradeMessage;
        }

        public static void Logger(string lines)
        {
            //Write the string to a file.append mode is enabled so that the log
            //lines get appended to  test.txt than wiping content and writing the log

            using (System.IO.StreamWriter file = new System.IO.StreamWriter("./trainTroops.log", true))
            {
                file.WriteLine(lines);
            }
        }

        static void Main(string[] args)
        {
            Dictionary<string, int> troopsToUpgrade = new Dictionary<string, int>();
            troopsToUpgrade.Add("Footman", 3);
            troopsToUpgrade.Add("Cavalry", 1);
            // Display the number of command line arguments.
            Logger(getTroopsReadyToUpgradeMessage(troopsToUpgrade));
            System.Console.WriteLine(getTroopsReadyToUpgradeMessage(troopsToUpgrade));
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
