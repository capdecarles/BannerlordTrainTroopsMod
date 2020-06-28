using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace TrainTroops
{
    class MobilePartyDailyTickBehaviour : CampaignBehaviorBase
    {

        //The higher this is, the more impact leadership will have on training.
        private const int troopXPMultiplier = 3;
        //The higher this is, the more impact level difference will have on training.
        private const int levelDifferenceMultiplier = 10;

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new System.Action<MobileParty>(this.addXp));
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
                    //IMPORTANT: bear in mind we only get a COPY. So after any changes to the troop, info will be inconsistent.
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
                        int troopsReadyToUpgradeCount = party.MemberRoster.GetElementCopyAtIndex(i).NumberReadyToUpgrade;
                        //Report troops ready to upgrade
                        if (troopsReadyToUpgradeCount != 0)
                        {
                            //Will update the party button notification so that the red icon is shown (?)
                            //PlayerUpdateTracker.Current.GetPartyNotificationText();
                            //PlayerUpdateTracker.Current.UpdatePartyNotification();
                            
                            //TODO: get the localized troop name, for now it only gets it in english
                            string troopName = LocalizedTextManager.GetTranslatedText(LocalizedTextManager.DefaultEnglishLanguageId, troop.Character.Name.GetID());
                            //Count how many troops of each type are ready to upgrade
                            troopsReadyToUpgrade.Add(troopName, troopsReadyToUpgradeCount);
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

        public override void SyncData(IDataStore dataStore)
        {
        }

    }

}
