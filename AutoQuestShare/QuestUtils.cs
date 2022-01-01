using UnityEngine;

namespace AutoQuestShare
{
    class QuestUtils
    {
        public static void shareQuest(EntityPlayerLocal entityPlayer, Quest quest)
        {
            var party = entityPlayer.Party;

            if (party == null)
                return;

            if (!quest.IsShareable)
            {
                return;
            }

            quest.SetupQuestCode();

            var pos = Vector3.zero;
            quest.GetPositionData(out pos, Quest.PositionDataTypes.QuestGiver);

            int num = 0;

            foreach (var member in party.MemberList)
            {
                if (member == entityPlayer || quest.HasSharedWith(member))
                    continue;

                GameManager.Instance.QuestShareServer(quest.ID, quest.GetPOIName(), quest.GetLocation(), quest.GetLocationSize(), pos, entityPlayer.entityId, member.entityId, quest.QuestGiverID);
                num++;
            }

            if (num == 0)
                GameManager.ShowTooltip(entityPlayer, Localization.Get("ttQuestShareNoPartyInRange"));
            else
                GameManager.ShowTooltip(entityPlayer, string.Format(Localization.Get("ttQuestShareWithParty"), quest.QuestClass.Name));
        }
    }
}
