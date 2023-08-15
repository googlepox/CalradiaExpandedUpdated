// Decompiled with JetBrains decompiler
// Type: CalradiaExpanded.NavalPatch
// Assembly: CalradiaExpanded, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E3EA9ABE-2234-47FC-9D86-DD09A05E7905
// Assembly location: C:\Steam\steamapps\common\Mount & Blade II Bannerlord\Modules\CalradiaExpanded\bin\Win64_Shipping_Client\CalradiaExpanded.dll

using HarmonyLib;
using SandBox;
using SandBox.View.Map;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace CalradiaExpanded
{
  public class NavalPatch
  {
    [HarmonyPatch(typeof (MapScene), "GetNavigationMeshIndexOfTerrainType")]
    public class GetNavigationMeshIndexOfTerrainTypePatch
    {
      public static void Postfix(TerrainType terrainType, int __result)
      {
        if (terrainType != TerrainType.Swamp)
          __result = 9;
      }
    }

    [HarmonyPatch(typeof (MapScene), "GetFaceTerrainType")]
    public class GetFaceTerrainTypePatch
    {
      public static void Postfix(
        PathFaceRecord navMeshFace,
        MapScene __instance,
        ref TerrainType __result)
      {
        if (__instance.Scene.GetIdOfNavMeshFace(((PathFaceRecord)navMeshFace).FaceIndex) == 9)
            __result = TerrainType.Swamp;
      }
    }

    [HarmonyPatch(typeof (MobileParty), "TickForMovingMobileParty")]
    public class OnAiTickInternalPatch
    {
      public static MethodInfo SetVisualsMethodInfo = AccessTools.Method(typeof (PartyVisual), "set_MountAgentVisuals", (Type[]) null, (Type[]) null);
      public static MethodInfo GetBannerOfCharacterMethodInfo = AccessTools.Method(typeof (PartyVisual), "GetBannerOfCharacter", (Type[]) null, (Type[]) null);

      private static void SetVisualsNull(PartyVisual instance) => SetVisualsMethodInfo.Invoke((object) instance, new object[1]);

      private static MetaMesh GetBannerOfCharacter(Banner banner, string bannerMeshName) => (MetaMesh) NavalPatch.OnAiTickInternalPatch.GetBannerOfCharacterMethodInfo.Invoke((object) null, new object[2]
      {
        (object) banner,
        (object) bannerMeshName
      });

        public static void Postfix(MobileParty __instance)
        {
            try
            {
                if (((PartyVisual)__instance.Party.Visuals).HumanAgentVisuals != null && Campaign.Current.MapSceneWrapper.GetFaceTerrainType(__instance.CurrentNavigationFace) == TerrainType.Swamp && ((PartyVisual)__instance.Party.Visuals).HumanAgentVisuals.GetEntity().GetVisibilityExcludeParents())
                {
                    if (((PartyVisual)__instance.Party.Visuals).StrategicEntity.ChildCount == 0)
                    {
                        GameEntity empty = GameEntity.CreateEmpty(((PartyVisual)MobileParty.MainParty.Party.Visuals).StrategicEntity.Scene, true);
                        if (__instance.IsCaravan)
                        {
                            MatrixFrame identity = MatrixFrame.Identity;
                            ((Mat3)identity.rotation).ApplyScaleLocal(1f);
                            ((MatrixFrame)identity).Rotate(1.4f, Vec3.Up);
                            empty.AddMultiMesh(MetaMesh.GetCopy("mi_ship_2", true, false), true);
                            identity.Rotate(1.4f, Vec3.Up);
                            identity.Scale(new Vec3(1f, 1f, 1f));
                            empty.SetFrame(ref identity);
                        }
                        else
                        {
                            MatrixFrame identity = MatrixFrame.Identity;
                            ((Mat3)identity.rotation).ApplyScaleLocal(0.08f);
                            if (__instance.Army != null && __instance.Army.LeaderParty != __instance)
                                ((Mat3)identity.rotation).ApplyScaleLocal(0.8f);
                            empty.AddMultiMesh(MetaMesh.GetCopy("ship_a", true, false), true);
                            identity.Scale(new Vec3((float)0.08, (float)0.08, (float)0.08));
                            empty.SetFrame(ref identity);
                        }
                        if (empty.ChildCount == 0)
                        {
                            empty.AddChild(GameEntity.CreateEmpty(((PartyVisual)MobileParty.MainParty.Party.Visuals).StrategicEntity.Scene, true), false);
                            if (__instance.LeaderHero != null && __instance.LeaderHero != null && __instance.LeaderHero.ClanBanner != null)
                            {
                                MetaMesh bannerOfCharacter = GetBannerOfCharacter(new Banner(__instance.LeaderHero.ClanBanner.Serialize()), "campaign_flag");
                                empty.GetChild(0).AddMultiMesh(bannerOfCharacter, true);
                                MatrixFrame identity = MatrixFrame.Identity;
                                ((Mat3)identity.rotation).ApplyScaleLocal(4f);
                                ((MatrixFrame)identity).Advance(-0.21f);
                                ((MatrixFrame)identity).Strafe(0.03f);
                                empty.GetChild(0).SetFrame(ref identity);
                            }
                        }
                        ((PartyVisual)__instance.Party.Visuals).StrategicEntity.AddChild(empty, false);
                    }
                    if (((PartyVisual)__instance.Party.Visuals).HumanAgentVisuals.GetEntity().GetVisibilityExcludeParents())
                    {
                        ((PartyVisual)__instance.Party.Visuals).HumanAgentVisuals.GetEntity().SetVisibilityExcludeParents(false);
                        if (((PartyVisual)__instance.Party.Visuals).MountAgentVisuals != null)
                        {
                            ((PartyVisual)__instance.Party.Visuals).MountAgentVisuals.Reset();
                            NavalPatch.OnAiTickInternalPatch.SetVisualsNull((PartyVisual)__instance.Party.Visuals);
                        }
                        if (((PartyVisual)__instance.Party.Visuals).CaravanMountAgentVisuals != null)
                            ((PartyVisual)__instance.Party.Visuals).CaravanMountAgentVisuals.GetEntity().SetVisibilityExcludeParents(false);
                    }
                }
                if (Campaign.Current.MapSceneWrapper.GetFaceTerrainType(__instance.CurrentNavigationFace) != TerrainType.Swamp && ((PartyVisual)__instance.Party.Visuals).StrategicEntity.ChildCount > 0)
                    __instance.Party.Visuals.SetMapIconAsDirty();
            }
            catch (Exception ex)
            {
                MBDebug.ShowWarning(string.Format("Error while trying to show party as ship. Party: [{0}]\nException: [{1}]", (object)__instance.Name, (object)ex.Message));
            }
        }
    }
  }
}
