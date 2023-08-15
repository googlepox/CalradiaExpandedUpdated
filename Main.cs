// Decompiled with JetBrains decompiler
// Type: CalradiaExpanded.Main
// Assembly: CalradiaExpanded, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E3EA9ABE-2234-47FC-9D86-DD09A05E7905
// Assembly location: C:\Steam\steamapps\common\Mount & Blade II Bannerlord\Modules\CalradiaExpanded\bin\Win64_Shipping_Client\CalradiaExpanded.dll

using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CalradiaExpanded
{
  public class Main : MBSubModuleBase
  {
        Harmony harmony;
        protected override void OnSubModuleLoad()
        {
            harmony = new Harmony("CE");
            harmony.PatchAll();
            base.OnSubModuleLoad();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
          if (!(game.GameType is Campaign))
            return;
          CampaignGameStarter campaignGameStarter = (CampaignGameStarter) gameStarter;
          campaignGameStarter.AddBehavior(new TradeBoundBehavior());

        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            var speedModel = AccessTools.Method(typeof(DefaultPartySpeedCalculatingModel), "CalculateFinalSpeed");
            harmony.Patch(speedModel, finalizer: new HarmonyMethod(AccessTools.Method(typeof(CEKPartySpeedCalculatingModel), "SpeedFinalizer")));
        }
        }
}
