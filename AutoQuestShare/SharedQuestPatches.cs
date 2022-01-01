using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace AutoQuestShare
{
    [HarmonyPatch(typeof(GameManager), "QuestShareClient")]
    public static class AutoAcceptSharedQuestPatch
    {
        public static void Postfix(string _questID, EntityPlayerLocal _player, int _SharedByEntityID, int _questGiverID)
        {
            if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
            {
                return;
            }

            if (_player == null)
                return;

            var questCode = Quest.CalculateQuestCode(_questID, _SharedByEntityID, _questGiverID);
            var questJournal = _player.QuestJournal;

            SharedQuestEntry sharedQuestEntry = null;

            foreach (var q in questJournal.sharedQuestEntries)
            {
                if (q.Quest.QuestCode == questCode)
                {
                    sharedQuestEntry = q;
                    break;
                }
            }

            if (sharedQuestEntry == null)
                return;

            var quest = sharedQuestEntry.Quest;

            quest.RemoveMapObject();

            questJournal.AddQuest(quest);
            questJournal.RemoveSharedQuestEntry(sharedQuestEntry);

            quest.AddSharedLocation(sharedQuestEntry.Position, sharedQuestEntry.Size);
            quest.SetPositionData(Quest.PositionDataTypes.QuestGiver, sharedQuestEntry.ReturnPos);
            quest.Position = sharedQuestEntry.Position;

            SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageSharedQuest>().Setup(quest.QuestCode, sharedQuestEntry.SharedByPlayerID, _player.entityId, true));
        }
    }

    [HarmonyPatch(typeof(XUiC_QuestOfferWindow), "btnAccept_OnPress")]
    public static class AutoInviteSharedQuestOnAcceptPatch
    {
        public static void Postfix(XUiC_QuestOfferWindow __instance)
        {
            if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
            {
                return;
            }

            if (__instance.QuestGiverID == -1)
                return;

            var entityPlayer = __instance.xui.playerUI.entityPlayer;
            var quest = __instance.Quest;

            quest.QuestGiverID = __instance.QuestGiverID;

            QuestUtils.shareQuest(entityPlayer, quest);
        }
    }


    [HarmonyPatch(typeof(NetPackagePartyData), "ProcessPackage")]
    public static class AutoInviteSharedQuestOnPartyJoinPatch
    {
        public static void Postfix(int ___PartyID, NetPackagePartyData.PartyActions ___partyAction)
        {
            if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
            {
                return;
            }

            if (___partyAction != NetPackagePartyData.PartyActions.AcceptInvite)
                return;

            var localPlayers = GameManager.Instance?.World?.GetLocalPlayers();

            if (localPlayers == null || localPlayers.Count == 0)
                return;

            var entityPlayer = localPlayers[0];

            var party = PartyManager.Current?.GetParty(___PartyID);

            if (party == null || entityPlayer.Party != party)
                return;

            // Share all quests
            foreach (var quest in entityPlayer.QuestJournal.quests)
            {
                if (!quest.IsShareable)
                    continue;

                QuestUtils.shareQuest(entityPlayer, quest);
            }
        }
    }
}


