﻿using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.ObjectSystem;

namespace CalradiaExpanded
{
    [HarmonyPatch(typeof(DefaultMapDistanceModel), "GetDistance", new[] { typeof(MobileParty), typeof(Settlement) })]
    internal class PartyToLiberartisPatch
    {
        public static bool Prefix(MobileParty fromParty, Settlement toSettlement, ref float __result)
        {
            if (toSettlement?.StringId == "town_TT1")
            {
                if (fromParty.CurrentNavigationFace.IsValid())
                {
                    Settlement zeonica = MBObjectManager.Instance.GetObject<Settlement>("town_EW2"), sanala = MBObjectManager.Instance.GetObject<Settlement>("town_A6");
                    float zeonicaDistance = Campaign.Current.Models.MapDistanceModel.GetDistance(fromParty, zeonica), sanalaDistance = Campaign.Current.Models.MapDistanceModel.GetDistance(fromParty, sanala);

                    __result = (sanalaDistance + zeonicaDistance) / 2f;
                }

                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(DefaultMapDistanceModel), "GetDistance", new[] { typeof(Settlement), typeof(Settlement) })]
    internal class SettlementLiberartisPatch
    {
        public static void Postfix(Settlement fromSettlement, Settlement toSettlement, ref float __result)
        {
            if (fromSettlement != null && fromSettlement.StringId == "town_TT1")
            {
                Settlement zeonica = MBObjectManager.Instance.GetObject<Settlement>("town_EW2");
                float zeonicaDistance = Campaign.Current.Models.MapDistanceModel.GetDistance(zeonica, toSettlement);
                Settlement sanala = MBObjectManager.Instance.GetObject<Settlement>("town_A6");
                float sanalaDistance = Campaign.Current.Models.MapDistanceModel.GetDistance(sanala, toSettlement);
                float avgDistance = (sanalaDistance + zeonicaDistance) / 2f;
                __result = avgDistance;
            }
            else if (toSettlement != null && toSettlement.StringId == "town_TT1")
            {
                Settlement zeonica = MBObjectManager.Instance.GetObject<Settlement>("town_EW2");
                float zeonicaDistance = Campaign.Current.Models.MapDistanceModel.GetDistance(fromSettlement, zeonica);
                Settlement sanala = MBObjectManager.Instance.GetObject<Settlement>("town_A6");
                float sanalaDistance = Campaign.Current.Models.MapDistanceModel.GetDistance(fromSettlement, sanala);
                float avgDistance = (sanalaDistance + zeonicaDistance) / 2f;
                __result = avgDistance;
            }
        }
    }
}
