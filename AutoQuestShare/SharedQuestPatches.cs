using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace AutoQuestShare
{
    [HarmonyPatch(typeof(NetPackageSharedQuest), "ProcessPackage")]
    public static class AutoAcceptSharedQuestPatch
    {
        public static void Postfix(string ___questID, int ___sharedWithEntityID, int ___sharedByEntityID, int ___questGiverID)
        {
            if (GameManager.IsDedicatedServer)
            {
                return;
            }

            var player = GameManager.Instance.World.GetEntity(___sharedWithEntityID) as EntityPlayerLocal;

            if (player == null)
                return;

            var questCode = Quest.CalculateQuestCode(___questID, ___sharedByEntityID, ___questGiverID);
            var questJournal = player.QuestJournal;

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


            if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
            {
                SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageSharedQuest>().Setup(quest.QuestCode, sharedQuestEntry.SharedByPlayerID, player.entityId, true), _attachedToEntityId: sharedQuestEntry.SharedByPlayerID);
            }
            else
            {
                SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageSharedQuest>().Setup(quest.QuestCode, sharedQuestEntry.SharedByPlayerID, player.entityId, true));
            }
        }
    }

    [HarmonyPatch(typeof(XUiC_QuestOfferWindow), "btnAccept_OnPress")]
    public static class AutoInviteSharedQuestOnAcceptPatch
    {
        public static void Postfix(XUiC_QuestOfferWindow __instance)
        {
            if (GameManager.IsDedicatedServer)
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

    [HarmonyPatch(typeof(Party), "ServerHandleAcceptInvite")]
    public static class AutoInviteSharedQuestOnPartyJoinServerPatch
    {   
        public static void Postfix(EntityPlayer invitedBy, EntityPlayer invitedEntity)
        {
            if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer || GameManager.IsDedicatedServer)
            {
                return;
            }

            var entityPlayer = QuestUtils.LocalPlayer;

            if (entityPlayer == null)
                return;

            if (invitedBy != entityPlayer && invitedEntity != entityPlayer)
            {
                return;
            }

            QuestUtils.shareAllQuests(entityPlayer);
        }
    }

    [HarmonyPatch(typeof(NetPackagePartyData), "ProcessPackage")]
    public static class AutoInviteSharedQuestOnPartyJoinClientPatch
    {
        public static void Postfix(int ___PartyID, NetPackagePartyData.PartyActions ___partyAction)
        {
            if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer || ___partyAction != NetPackagePartyData.PartyActions.AcceptInvite)
            {
                return;
            }
            var party = PartyManager.Current?.GetParty(___PartyID);

            var entityPlayer = QuestUtils.LocalPlayer;

            if (party == null || entityPlayer == null || entityPlayer.Party != party)
                return;

            QuestUtils.shareAllQuests(entityPlayer);
        }
    }
}


