using HarmonyLib;
using UnityEngine;

namespace AutoAcceptPartyInvite
{

    [HarmonyPatch(typeof(EntityPlayer), "AddPartyInvite")]
    public static class AutoAcceptInvitePatches
    {
        public static void Postfix(EntityPlayer __instance)
        {
            if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
            {
                return;
            }

            if (!(__instance is EntityPlayerLocal))
            {
                return;
            }

            var entityPlayer = (EntityPlayerLocal)__instance;

            // Player is already in a party
            if (entityPlayer.IsInParty())
                return;

            var friendsList = entityPlayer.persistentPlayerData?.ACL;
            var playerList = GameManager.Instance?.GetPersistentPlayerList();

            if (friendsList == null || playerList == null)
            {
                return;
            }

            EntityPlayer inviter = null;

            foreach (var i in entityPlayer.partyInvites)
            {
                // Don't join a full party
                if (i.IsInParty() && i.Party.MemberList.Count >= 8)
                {
                    continue;
                }

                var invData = playerList.GetPlayerDataFromEntityID(i.entityId);

                if (invData == null || !friendsList.Contains(invData.UserIdentifier))
                {
                    continue;
                }

                inviter = i;
                break;
            }

            if (inviter == null)
                return;

            SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePartyActions>().Setup(NetPackagePartyActions.PartyActions.AcceptInvite, inviter.entityId, entityPlayer.entityId));
        }
    }

}


