using System;
using System.Collections.Generic;
using WorldOfTheThreeKingdoms.GameScreens;

namespace GameEvents;

public class MainGameScreenSubscriber : IDisposable
{
    private MainGameScreen screen;

    private readonly List<IDisposable> subscriptions = new();

    private bool disposed;

    public MainGameScreenSubscriber(MainGameScreen screen)
    {
        this.screen = screen;

        var eventManager = EventManager.Instance;

        // 建筑事件
        Subscribe<AppointMayorEvent>(OnAppointMayor);
        Subscribe<ZhaoXianEvent>(OnZhaoXian);
        Subscribe<SelectPrinceEvent>(OnSelectPrince);
        Subscribe<PopulationEscapeEvent>(OnPopulationEscape);
        Subscribe<PopulationEnterEvent>(OnPopulationEnter);
        Subscribe<RecentlyAttackedEvent>(OnRecentlyAttacked);
        Subscribe<ReleaseCaptiveEvent>(OnReleaseCaptive);
        Subscribe<DisasterOccurredEvent>(OnDisasterOccurred);
        Subscribe<FacilityCompletedEvent>(OnFacilityCompleted);
        Subscribe<RewardPersonEvent>(OnRewardPerson);
        Subscribe<HirePersonEvent>(OnHirePerson);
        Subscribe<ArchitectureEvent>(OnApplyArchitectureEvent);

        // 俘虏事件
        Subscribe<PlayerReleaseEvent>(OnPlayerRelease);
        Subscribe<ReleaseEvent>(OnRelease);
        Subscribe<SelfReleaseEvent>(OnSelfRelease);
        Subscribe<EscapeEvent>(OnEscape);

        // 势力事件
        Subscribe<AfterCatchLeaderEvent>(OnAfterCatchLeader);
        Subscribe<FactionDestoryEvent>(OnFactionDestory);
        Subscribe<UpgradeTechniqueEvent>(OnUpgraeTechnique);
        Subscribe<TechniqueFinishedEvent>(OnTechniqueFinished);
        Subscribe<InitiativeChangeCapitalEvent>(OnInitiativeChangeCapital);
        Subscribe<ForcedChangeCapitalEvent>(OnForcedChangeCapital);
        Subscribe<GetControlEvent>(OnGetControl);

        // 人物事件
        Subscribe<JailBreakSuccessedEvent>(OnJailBreakSuccessed);
        Subscribe<JailBreakFailedEvent>(OnJailBreakFailed);
        Subscribe<ConvinceSuccessedEvent>(OnConvinceSuccessed);
        Subscribe<ConvinceFailedEvent>(OnConvinceFailed);
        Subscribe<InformationAcquisitionSuccessedEvent>(OnInformationAcquisitionSuccessed);
        Subscribe<InformationAcquisitionFailedEvent>(OnInformationAcquisitionFailed);
        Subscribe<SpyingSuccessedEvent>(OnSpyingSuccessed);
        Subscribe<SypingFailedEvent>(OnSypingFailed);
        Subscribe<DestroySuccessedEvent>(OnDestroySuccessed);
        Subscribe<DestroyFailedEvent>(OnDestroyFailed);
        Subscribe<InstigateSuccessedEvent>(OnInstigateSuccessed);
        Subscribe<InstigateFailedEvent>(OnInstigateFailed);
        Subscribe<GossipSuccessedEvent>(OnGossipSuccessed);
        Subscribe<GossipFailedEvent>(OnGossipFailed);
        Subscribe<SearchFinishedEvent>(OnSearchFinished);
        Subscribe<SpierFoundEvent>(OnSpierFound);
        Subscribe<TreasureFoundEvent>(OnTreasureFound);
        Subscribe<ShowMessageEvent>(OnShowMessage);
        Subscribe<DeathEvent>(OnDeath);
        Subscribe<LeaveEvent>(OnLeave);
        Subscribe<BeKilledEvent>(OnBeKilled);
        Subscribe<DeathChangeLeaderEvent>(OnDeathChangeLeader);
        Subscribe<DeathChangeFactionEvent>(OnDeathChangeFaction);
        Subscribe<StudyTitleFinishedEvent>(OnStudyTitleFinished);
        Subscribe<StudySkillFinishedEvent>(OnStudySkillFinished);
        Subscribe<StudyStuntFinishedEvent>(OnStudyStuntFinished);
        Subscribe<AwardedTreasureEvent>(OnAwardedTreasure);
        Subscribe<ConfiscatedTreasureEvent>(OnConfiscatedTreasure);
        Subscribe<CapturedByArchitectureEvent>(OnCapturedByArchitecture);
        Subscribe<CreateBrotherEvent>(OnCreateBrother);
        Subscribe<CreateSisterEvent>(OnCreateSister);
        Subscribe<CreateSpouseEvent>(OnCreateSpouse);

        // 部队事件
        Subscribe<TroopCreateEvent>(OnTroopCreate);
        Subscribe<EndPathEvent>(OnEndPath);
        Subscribe<PathNotFoundEvent>(OnPathNotFound);
        Subscribe<NormalAttackEvent>(OnNormalAttack);
        Subscribe<CombatMethodAttackEvent>(OnCombatMethodAttack);
        Subscribe<CastStratagemEvent>(OnCastStratagem);
        Subscribe<CriticalStrikeEvent>(OnCriticalStrike);
        Subscribe<ReceiveCriticalStrikeEvent>(OnReceiveCriticalStrike);
        Subscribe<WaylayEvent>(OnWaylay);
        Subscribe<ReceiveWaylayEvent>(OnReceiveWaylay);
        Subscribe<SurroundEvent>(OnSurround);
        Subscribe<SetCombatMethodEvent>(OnSetCombatMethod);
        Subscribe<SetStratagemEvent>(OnSetStratagem);
        Subscribe<StratagemSuccessedEvent>(OnStratagemSuccessed);
        Subscribe<ChaosEvent>(OnChaos);
        Subscribe<RumourEvent>(OnRumour);
        Subscribe<AttractEvent>(OnAttract);
        Subscribe<RecoverFromChaosEvent>(OnRecoverFromChaos);
        Subscribe<CastDeepChaosEvent>(OnCastDeepChaos);
        Subscribe<ResistStratagemEvent>(OnResistStratagem);
        Subscribe<AmbushEvent>(OnAmbush);
        Subscribe<StopAmbushEvent>(OnStopAmbush);
        Subscribe<DiscoverAmbushEvent>(OnDiscoverAmbush);
        Subscribe<RoutEvent>(OnRout);
        Subscribe<RoutedEvent>(OnRouted);
        Subscribe<BreakWallEvent>(OnBreakWall);
        Subscribe<SpreadBurntEvent>(OnSpreadBurnt);
        Subscribe<OccupyArchitectureEvent>(OnOccupyArchitecture);
        Subscribe<AntiAttackEvent>(OnAntiAttack);
        Subscribe<AntiArrowAttackEvent>(OnAntiArrowAttack);
        Subscribe<LevyGrainEvent>(OnLevyGrain);
        Subscribe<CutRoutewayEvent>(OnCutRouteway);
        Subscribe<CutRoutewayResultEvent>(OnCutRoutewayResult);
        Subscribe<GetNewCaptiveEvent>(OnGetNewCaptive);
        Subscribe<ReleaseTroopCaptiveEvent>(OnReleaseTroopCaptive);
        Subscribe<PersonChallengeEvent>(OnPersonChallenge);
        Subscribe<PersonControversyEvent>(OnPersonControversy);
        Subscribe<OutburstEvent>(OnOutburst);
        Subscribe<ApplyStuntEvent>(OnApplyStunt);
        Subscribe<TransportArrivedEvent>(OnTransportArrived);

        Subscribe<ApplyTroopEvent>(OnApplyTroop);
    }

    private void Subscribe<T>(Action<T> handler)
    {
        var subscription = EventManager.Instance.Subscribe(handler);
        subscriptions.Add(subscription);
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        foreach (var subscription in subscriptions)
        {
            subscription.Dispose();
        }

        subscriptions.Clear();
    }

    #region 建筑事件

    private void OnAppointMayor(AppointMayorEvent e)
    {
        screen.Appointmayor(e.leader, e.person);
    }

    private void OnZhaoXian(ZhaoXianEvent e)
    {
        screen.Zhaoxian(e.leader, e.person);
    }

    private void OnSelectPrince(SelectPrinceEvent e)
    {
        screen.Selectprince(e.leader, e.person);
    }

    private void OnPopulationEscape(PopulationEscapeEvent e)
    {
        screen.ArchitecturePopulationEscape(e.architecture, e.quantity);
    }

    private void OnPopulationEnter(PopulationEnterEvent e)
    {
        screen.ArchitecturePopulationEnter(e.architecture, e.quantity);
    }

    private void OnRecentlyAttacked(RecentlyAttackedEvent e)
    {
        screen.ArchitectureBeginRecentlyAttacked(e.architecture);
    }

    private void OnReleaseCaptive(ReleaseCaptiveEvent e)
    {
        screen.ArchitectureReleaseCaptiveAfterOccupied(e.architecture, e.persons);
    }

    private void OnDisasterOccurred(DisasterOccurredEvent e)
    {
        screen.Architecturefashengzainan(e.architecture, e.disasterId);
    }

    private void OnFacilityCompleted(FacilityCompletedEvent e)
    {
        screen.ArchitectureFacilityCompleted(e.architecture, e.facility);
    }

    private void OnRewardPerson(RewardPersonEvent e)
    {
        screen.ArchitectureRewardPersons(e.architecture, e.persons);
    }

    private void OnHirePerson(HirePersonEvent e)
    {
        screen.ArchitectureHirePerson(e.persons);
    }

    private void OnApplyArchitectureEvent(ArchitectureEvent e)
    {
        e.screen.ApplyEvent(e.archEvent, e.architecture, e.screen);
    }

    #endregion

    #region 俘虏事件

    private void OnPlayerRelease(PlayerReleaseEvent e)
    {
        screen.CaptivePlayerRelease(e.from, e.to, e.captive);
    }

    private void OnRelease(ReleaseEvent e)
    {
        screen.CaptiveRelease(e.success, e.from, e.to, e.person);
    }

    private void OnSelfRelease(SelfReleaseEvent e)
    {
        screen.SelfCaptiveRelease(e.captive);
    }

    private void OnEscape(EscapeEvent e)
    {
        //captive.Scenario.GameScreen.CaptiveEscape(captive);
    }

    #endregion

    #region 势力事件

    private void OnAfterCatchLeader(AfterCatchLeaderEvent e)
    {
        screen.FactionAfterCatchLeader(e.leader, e.faction);
    }

    private void OnFactionDestory(FactionDestoryEvent e)
    {
        screen.FactionDestroy(e.faction);
    }

    private void OnUpgraeTechnique(UpgradeTechniqueEvent e)
    {
        screen.FactionUpgradeTechnique(e.faction, e.technique, e.architecture);
    }

    private void OnTechniqueFinished(TechniqueFinishedEvent e)
    {
        screen.FactionTechniqueFinished(e.faction, e.technique);
    }

    private void OnInitiativeChangeCapital(InitiativeChangeCapitalEvent e)
    {
        screen.FactionInitialtiveChangeCapital(e.faction, e.oldCapital, e.newCapital);
    }

    private void OnForcedChangeCapital(ForcedChangeCapitalEvent e)
    {
        screen.FactionForcedChangeCapital(e.faction, e.oldCapital, e.newCapital);
    }

    private void OnGetControl(GetControlEvent e)
    {
        screen.FactionGetControl(e.faction);
    }

    #endregion

    #region 人物事件

    private void OnJailBreakSuccessed(JailBreakSuccessedEvent e)
    {
        screen.PersonJailBreak(e.person, e.captive);
    }

    private void OnJailBreakFailed(JailBreakFailedEvent e)
    {
        screen.PersonJailBreakFailed(e.person, e.architecture);
    }

    private void OnConvinceSuccessed(ConvinceSuccessedEvent e)
    {
        screen.PersonConvinceSuccess(e.source, e.target, e.faction);
    }

    private void OnConvinceFailed(ConvinceFailedEvent e)
    {
        screen.PersonConvinceFailed(e.source, e.target);
    }

    private void OnInformationAcquisitionSuccessed(InformationAcquisitionSuccessedEvent e)
    {
        screen.PersonInformationObtained(e.person, e.information);
    }

    private void OnInformationAcquisitionFailed(InformationAcquisitionFailedEvent e)
    {
        screen.qingbaoshibai(e.person);
    }

    private void OnSpyingSuccessed(SpyingSuccessedEvent e)
    {
        // screen.PersonSpySuccess(e.person, e.architecture);
    }

    private void OnSypingFailed(SypingFailedEvent e)
    {
        // screen.PersonSpyFailed(e.person, e.architecture);
    }

    private void OnDestroySuccessed(DestroySuccessedEvent e)
    {
        screen.PersonDestroySuccess(e.person, e.architecture, e.down);
    }

    private void OnDestroyFailed(DestroyFailedEvent e)
    {
        screen.PersonDestroyFailed(e.person, e.architecture);
    }

    private void OnInstigateSuccessed(InstigateSuccessedEvent e)
    {
        screen.PersonInstigateSuccess(e.person, e.architecture, e.down);
    }

    private void OnInstigateFailed(InstigateFailedEvent e)
    {
        screen.PersonInstigateFailed(e.person, e.architecture);
    }

    private void OnGossipSuccessed(GossipSuccessedEvent e)
    {
        screen.PersonGossipSuccess(e.person, e.architecture);
    }

    private void OnGossipFailed(GossipFailedEvent e)
    {
        screen.PersonGossipFailed(e.person, e.architecture);
    }

    private void OnSearchFinished(SearchFinishedEvent e)
    {
        screen.PersonSearchFinished(e.person, e.architecture, e.result);
    }

    private void OnSpierFound(SpierFoundEvent e)
    {
        // screen.PersonSpyFound(e.person, e.spier);
    }

    private void OnTreasureFound(TreasureFoundEvent e)
    {
        screen.PersonTreasureFound(e.person, e.treasure);
    }

    private void OnShowMessage(ShowMessageEvent e)
    {
        // screen.PersonShowMessage(e.person, e.message);
    }

    private void OnDeath(DeathEvent e)
    {
        screen.PersonDeath(e.person, e.killer, e.architecture, e.troop);
    }

    private void OnLeave(LeaveEvent e)
    {
        screen.PersonLeave(e.person, e.architecture);
    }

    private void OnBeKilled(BeKilledEvent e)
    {
        screen.PersonBeKilled(e.person, e.architecture);
    }

    private void OnDeathChangeLeader(DeathChangeLeaderEvent e)
    {
        screen.PersonChangeLeader(e.faction, e.leader, e.changeName, e.oldName);
    }

    private void OnDeathChangeFaction(DeathChangeFactionEvent e)
    {
        screen.PersonDeathChangeFaction(e.dead, e.leader, e.oldName);
    }

    private void OnStudyTitleFinished(StudyTitleFinishedEvent e)
    {
        screen.PersonStudyTitleFinished(e.person, e.title, e.success);
    }

    private void OnStudySkillFinished(StudySkillFinishedEvent e)
    {
        screen.PersonStudySkillFinished(e.person, e.skillString, e.success);
    }

    private void OnStudyStuntFinished(StudyStuntFinishedEvent e)
    {
        screen.PersonStudyStuntFinished(e.person, e.stunt, e.success);
    }

    private void OnAwardedTreasure(AwardedTreasureEvent e)
    {
        screen.PersonBeAwardedTreasure(e.person, e.treasure);
    }

    private void OnConfiscatedTreasure(ConfiscatedTreasureEvent e)
    {
        screen.PersonBeConfiscatedTreasure(e.person, e.treasure);
    }

    private void OnCapturedByArchitecture(CapturedByArchitectureEvent e)
    {
        screen.PersonCapturedByArchitecture(e.person, e.architecture);
    }

    private void OnCreateBrother(CreateBrotherEvent e)
    {
        screen.CreateBrother(e.p1, e.p2);
    }

    private void OnCreateSister(CreateSisterEvent e)
    {
        screen.CreateSister(e.p1, e.p2);
    }

    private void OnCreateSpouse(CreateSpouseEvent e)
    {
        screen.CreateSpouse(e.p1, e.p2);
    }

    #endregion

    #region  部队事件

    private void OnTroopCreate(TroopCreateEvent e)
    {
        screen.TroopCreate(e.troop);
    }

    private void OnEndPath(EndPathEvent e)
    {
        screen.TroopEndPath(e.troop);
    }

    private void OnPathNotFound(PathNotFoundEvent e)
    {
        screen.TroopPathNotFound(e.troop);
    }

    private void OnNormalAttack(NormalAttackEvent e)
    {
        screen.TroopNormalAttack(e.attacker, e.defender);
    }

    private void OnCombatMethodAttack(CombatMethodAttackEvent e)
    {
        screen.TroopCombatMethodAttack(e.attacker, e.defender, e.combatMethod);
    }

    private void OnCastStratagem(CastStratagemEvent e)
    {
        screen.TroopCastStratagem(e.attacker, e.defender, e.stratagem);
    }

    private void OnCriticalStrike(CriticalStrikeEvent e)
    {
        screen.TroopCriticalStrike(e.attacker, e.defender);
    }

    private void OnReceiveCriticalStrike(ReceiveCriticalStrikeEvent e)
    {
        screen.TroopReceiveCriticalStrike(e.attacker, e.defender);
    }

    private void OnWaylay(WaylayEvent e)
    {
        screen.TroopWaylay(e.attacker, e.defender);
    }

    private void OnReceiveWaylay(ReceiveWaylayEvent e)
    {
        screen.TroopReceiveWaylay(e.attacker, e.defender);
    }

    private void OnSurround(SurroundEvent e)
    {
        screen.TroopSurround(e.attacker, e.defender);
    }

    private void OnSetCombatMethod(SetCombatMethodEvent e)
    {
        screen.TroopSetCombatMethod(e.troop, e.combatMethod);
    }

    private void OnSetStratagem(SetStratagemEvent e)
    {
        screen.TroopSetStratagem(e.troop, e.stratagem);
    }

    private void OnStratagemSuccessed(StratagemSuccessedEvent e)
    {
        screen.TroopStratagemSuccess(e.attacker, e.defender, e.stratagem, e.isHarmful);
    }

    private void OnChaos(ChaosEvent e)
    {
        screen.TroopChaos(e.troop, e.deepChaos);
    }

    private void OnRumour(RumourEvent e)
    {
        screen.TroopRumour(e.troop);
    }

    private void OnAttract(AttractEvent e)
    {
        screen.TroopAttract(e.attacker, e.defender);
    }

    private void OnRecoverFromChaos(RecoverFromChaosEvent e)
    {
        screen.TroopRecoverFromChaos(e.troop);
    }

    private void OnCastDeepChaos(CastDeepChaosEvent e)
    {
        screen.TroopCastDeepChaos(e.attacker, e.defender);
    }

    private void OnResistStratagem(ResistStratagemEvent e)
    {
        screen.TroopResistStratagem(e.attacker, e.defender, e.stratagem, e.isHarmful);
    }

    private void OnAmbush(AmbushEvent e)
    {
        screen.TroopAmbush(e.troop);
    }

    private void OnStopAmbush(StopAmbushEvent e)
    {
        screen.TroopStopAmbush(e.troop);
    }

    private void OnDiscoverAmbush(DiscoverAmbushEvent e)
    {
        screen.TroopDiscoverAmbush(e.attacker, e.defender);
    }

    private void OnRout(RoutEvent e)
    {
        screen.TroopRout(e.attacker, e.defender);
    }

    private void OnRouted(RoutedEvent e)
    {
        screen.TroopRouted(e.attacker, e.defender);
    }

    private void OnBreakWall(BreakWallEvent e)
    {
        screen.TroopBreakWall(e.troop, e.architecture);
    }

    private void OnSpreadBurnt(SpreadBurntEvent e)
    {
        screen.TroopGetSpreadBurnt(e.troop);
    }

    private void OnOccupyArchitecture(OccupyArchitectureEvent e)
    {
        screen.TroopOccupyArchitecture(e.troop, e.architecture);
    }

    private void OnAntiAttack(AntiAttackEvent e)
    {
        screen.TroopAntiAttack(e.attacker, e.defender);
    }

    private void OnAntiArrowAttack(AntiArrowAttackEvent e)
    {
        screen.TroopAntiArrowAttack(e.attacker, e.defender);
    }

    private void OnLevyGrain(LevyGrainEvent e)
    {
        screen.TroopLevyFieldFood(e.troop, e.grain);
    }

    private void OnCutRouteway(CutRoutewayEvent e)
    {
        screen.TroopStartCutRouteway(e.troop, e.days);
    }

    private void OnCutRoutewayResult(CutRoutewayResultEvent e)
    {
        screen.TroopEndCutRouteway(e.troop, e.success);
    }

    private void OnGetNewCaptive(GetNewCaptiveEvent e)
    {
        screen.TroopGetNewCaptive(e.troop, e.persons);
    }

    private void OnReleaseTroopCaptive(ReleaseTroopCaptiveEvent e)
    {
        screen.TroopReleaseCaptive(e.troop, e.persons);
    }

    private void OnPersonChallenge(PersonChallengeEvent e)
    {
        screen.TroopPersonChallenge(e.win, e.attackingTroop, e.attacker, e.defenseTroop, e.defender);
    }

    private void OnPersonControversy(PersonControversyEvent e)
    {
        screen.TroopPersonControversy(e.win, e.attackingTroop, e.attacker, e.defenseTroop, e.defender);
    }

    private void OnOutburst(OutburstEvent e)
    {
        screen.TroopOutburst(e.troop, e.kind);
    }

    private void OnApplyStunt(ApplyStuntEvent e)
    {
        screen.TroopApplyStunt(e.troop, e.stunt);
    }

    private void OnTransportArrived(TransportArrivedEvent e)
    {
        screen.AskWhenTransportArrived(e.troop, e.architecture);
    }

    private void OnApplyTroop(ApplyTroopEvent e)
    {
        screen.TroopApplyTroopEvent(e.troopEvent, e.troop);
    }
    #endregion
}