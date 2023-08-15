// Decompiled with JetBrains decompiler
// Type: CalradiaExpanded.CEKPartySpeedCalculatingModel
// Assembly: CalradiaExpanded, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E3EA9ABE-2234-47FC-9D86-DD09A05E7905
// Assembly location: C:\Steam\steamapps\common\Mount & Blade II Bannerlord\Modules\CalradiaExpanded\bin\Win64_Shipping_Client\CalradiaExpanded.dll

using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;
using HarmonyLib;

namespace CalradiaExpanded
{

    //[HarmonyPatch(typeof(DefaultPartySpeedCalculatingModel), "CalculateFinalSpeed")]
    internal class CEKPartySpeedCalculatingModel
    {
        public static void SpeedFinalizer(ExplainedNumber __result, MobileParty mobileParty)
        {
            if (Campaign.Current.MapSceneWrapper.GetFaceTerrainType(mobileParty.CurrentNavigationFace) == TaleWorlds.Core.TerrainType.Swamp)
            {
                __result.AddFactor(-0.4f, new TextObject("at Sea"));
            }
        }
    }
}
