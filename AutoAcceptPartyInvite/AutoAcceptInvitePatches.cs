using HarmonyLib;
using UnityEngine;

namespace AutoAcceptPartyInvite
{

    [HarmonyPatch(typeof(EntityPlayer), "AddPartyInvite")]
    public static class AutoAcceptInvitePatches
    {
        public static void Postfix(EntityPlayer __instance, int playerEntityID)
        {
            if (GameManager.IsDedicatedServer)
            {
                return;
            }

            var localPlayer = __instance as EntityPlayerLocal; // received invite
            var remotePlayer = GameManager.Instance.World.GetEntity(playerEntityID) as EntityPlayer; // sent invite

            if (localPlayer == null || remotePlayer == null)
                return;

            // Player is already in a party
            if (localPlayer.IsInParty())
                return;

            var friendsList = localPlayer.persistentPlayerData?.ACL;
            var remotePlayerData = GameManager.Instance?.GetPersistentPlayerList()?.GetPlayerDataFromEntityID(playerEntityID);

            if (friendsList == null || remotePlayerData == null)
            {
                return;
            }

            if (!friendsList.Contains(remotePlayerData.UserIdentifier))
                return;
            
            if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
            {
                Party.ServerHandleAcceptInvite(remotePlayer, localPlayer);
            }
            else
            {
                SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePartyActions>().Setup(NetPackagePartyActions.PartyActions.AcceptInvite, inviter.entityId, localPlayer.entityId));
            }

            
        }
    }

}


