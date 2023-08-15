// Decompiled with JetBrains decompiler
// Type: CalradiaExpanded.TradeBoundBehavior
// Assembly: CalradiaExpanded, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E3EA9ABE-2234-47FC-9D86-DD09A05E7905
// Assembly location: C:\Steam\steamapps\common\Mount & Blade II Bannerlord\Modules\CalradiaExpanded\bin\Win64_Shipping_Client\CalradiaExpanded.dll

using Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Settlements;

namespace CalradiaExpanded
{
  public class TradeBoundBehavior : CampaignBehaviorBase
  {
    public const float TradeBoundDistanceLimit = 150f;

    public override void RegisterEvents()
    {
      CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener( this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
      CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
      CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
      CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.WarDeclared));
      CampaignEvents.MakePeace.AddNonSerializedListener(this, new Action<IFaction, IFaction, MakePeaceAction.MakePeaceDetail>(this.OnMakePeace));
      CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreatedPartialFollowUp));
    }

    public override void SyncData(IDataStore dataStore)
    {
    }

    private void ClanChangedKingdom(
      Clan clan,
      Kingdom oldKingdom,
      Kingdom newKingdom,
      ChangeKingdomAction.ChangeKingdomActionDetail detail,
      bool showNotification = true)
    {
      this.UpdateTradeBounds();
    }

    private void OnGameLoaded(CampaignGameStarter obj) => this.UpdateTradeBounds();

    private void OnMakePeace(IFaction faction1, IFaction faction2, MakePeaceAction.MakePeaceDetail peaceDetail) => this.UpdateTradeBounds();

    private void WarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail warDetail) => this.UpdateTradeBounds();

    private void OnSettlementOwnerChanged(
      Settlement settlement,
      bool openToClaim,
      Hero newOwner,
      Hero oldOwner,
      Hero capturerHero,
      ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
    {
      this.UpdateTradeBounds();
    }

    public void OnNewGameCreated(CampaignGameStarter campaignGameStarter) => this.UpdateTradeBounds();

    private void OnNewGameCreatedPartialFollowUp(CampaignGameStarter starter, int i) => this.UpdateTradeBounds();

    private void UpdateTradeBounds()
    {
      foreach (Town allCastle in (IEnumerable<Town>) Town.AllCastles)
      {
        foreach (Village village in allCastle.Villages)
          this.TryToAssignTradeBoundForVillage(village);
      }
    }

    private void TryToAssignTradeBoundForVillage(Village village)
    {
      if (village.TradeBound != null)
        return;
      Settlement nearestSettlement1 = SettlementHelper.FindNearestSettlement((Func<Settlement, bool>) (x => x.IsTown && x.Town.MapFaction == ((SettlementComponent) village).Settlement.MapFaction), (IMapPoint) ((SettlementComponent) village).Settlement);
      PropertyInfo property = village.GetType().GetProperty("TradeBound", BindingFlags.Instance | BindingFlags.Public);
      if (nearestSettlement1 != null)
      {
        if ((double) Campaign.Current.Models.MapDistanceModel.GetDistance(nearestSettlement1, ((SettlementComponent) village).Settlement) <= 150.0)
        {
          property.SetValue((object) village, (object) nearestSettlement1);
          return;
        }
        Settlement nearestSettlement2 = SettlementHelper.FindNearestSettlement((Func<Settlement, bool>) (x => x.IsTown && x.Town.MapFaction != ((SettlementComponent) village).Settlement.MapFaction && !x.Town.MapFaction.IsAtWarWith(((SettlementComponent) village).Settlement.MapFaction) && (double) Campaign.Current.Models.MapDistanceModel.GetDistance(x, ((SettlementComponent) village).Settlement) <= 150.0), (IMapPoint) ((SettlementComponent) village).Settlement);
        if (nearestSettlement2 != null)
        {
          property.SetValue((object) village, (object) nearestSettlement2);
          return;
        }
      }
      if (village.TradeBound != null)
        return;
      property.SetValue((object) village, (object) nearestSettlement1);
    }
  }
}
