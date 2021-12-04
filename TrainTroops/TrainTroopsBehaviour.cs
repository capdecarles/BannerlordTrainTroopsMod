using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using System.Collections.Generic;
using System.Linq;

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

                int leaderLeadership = Hero.MainHero.GetSkillValue(DefaultSkills.Leadership);

                int totalXPEarned = 0;
                Dictionary<string, int> troopsReadyToUpgrade = new Dictionary<string, int>();
                for (int i = 0; i < party.MemberRoster.Count; i++)
                {
                    //IMPORTANT: bear in mind we only get a COPY. So after any changes to the troop, info will be inconsistent.
                    TroopRosterElement troop = party.MemberRoster.GetElementCopyAtIndex(i);
                    //Only gain XP if character LVL is lower than the leader's LVL AND the troop can be upgraded
                    if (troop.Character.Level < Hero.MainHero.Level && !troop.Character.UpgradeTargets.IsEmpty())
                    {
                        int lvlDifference = Hero.MainHero.Level - troop.Character.Level;

                        //Get the least xp this troop needs to lvl up (seems it could have different troops to level up to and need different xp for each one)
                        int minXPForUpgrade = troop.Character.GetUpgradeXpCost(party.Party, 0);
                        int targetIndex = 1;
                        while (targetIndex < troop.Character.UpgradeTargets.Length) {
                            minXPForUpgrade = System.Math.Min(minXPForUpgrade, troop.Character.GetUpgradeXpCost(party.Party, targetIndex));
                            targetIndex++;
                        }


                        int trainableTroopCount = troop.Number - troop.Xp / minXPForUpgrade;

                        //Perform the math
                        int xpEarned = (leaderLeadership * troopXPMultiplier + lvlDifference * levelDifferenceMultiplier) * trainableTroopCount;
                        party.Party.MemberRoster.AddXpToTroopAtIndex(xpEarned, i);
                        int troopsReadyToUpgradeCount = (troop.Xp + xpEarned) / minXPForUpgrade;
                        //Report troops ready to upgrade
                        if (troopsReadyToUpgradeCount != 0)
                        {
                            //Will update the party button notification so that the red icon is shown (?)
                            //PlayerUpdateTracker.Current.GetPartyNotificationText();
                            //PlayerUpdateTracker.Current.UpdatePartyNotification();

                            //TODO: get the troop name, for now it only gets it in english
                            string troopName = troop.Character.ToString();
                            //Count how many troops of each type are ready to upgrade


                            //If a troop with the same name has already been counted, add it
                            if (troopsReadyToUpgrade.ContainsKey(troopName))
                            {
                                troopsReadyToUpgrade[troopName] += troopsReadyToUpgradeCount;
                            } 
                            //Else add it anew
                            else
                            {
                                troopsReadyToUpgrade.Add(troopName, troopsReadyToUpgradeCount);
                            }
                        }

                        totalXPEarned += xpEarned;
                    }
                }

                InformationManager.DisplayMessage(new InformationMessage("Total training XP for the day: " + totalXPEarned + "." + getTroopsReadyToUpgradeMessage(troopsReadyToUpgrade)));
                
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

        public override void SyncData(IDataStore dataStore)
        {
        }

    }

}
