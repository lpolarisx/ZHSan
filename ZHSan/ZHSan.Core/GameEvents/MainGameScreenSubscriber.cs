using GameManager;

namespace GameEvents;

public class MainGameScreenSubscriber
{
    public MainGameScreenSubscriber()
    {
        var eventManager = EventManager.Instance;

        // 建筑事件
        eventManager.Subscribe<AppointMayorEvent>(OnAppointMayor);
        eventManager.Subscribe<ZhaoXianEvent>(OnZhaoXian);
        eventManager.Subscribe<SelectPrinceEvent>(OnSelectPrince);
        eventManager.Subscribe<PopulationEscapeEvent>(OnPopulationEscape);
        eventManager.Subscribe<PopulationEnterEvent>(OnPopulationEnter);
        eventManager.Subscribe<RecentlyAttackedEvent>(OnRecentlyAttacked);
        eventManager.Subscribe<ReleaseCaptiveEvent>(OnReleaseCaptive);
        eventManager.Subscribe<DisasterOccurredEvent>(OnDisasterOccurred);
        eventManager.Subscribe<FacilityCompletedEvent>(OnFacilityCompleted);
        eventManager.Subscribe<RewardPersonEvent>(OnRewardPerson);
        eventManager.Subscribe<HirePersonEvent>(OnHirePerson);

        // 俘虏事件
        eventManager.Subscribe<PlayerReleaseEvent>(OnPlayerRelease);
        eventManager.Subscribe<ReleaseEvent>(OnRelease);
        eventManager.Subscribe<SelfReleaseEvent>(OnSelfRelease);
        eventManager.Subscribe<EscapeEvent>(OnEscape);

        // 势力事件
        eventManager.Subscribe<AfterCatchLeaderEvent>(OnAfterCatchLeader);
        eventManager.Subscribe<FactionDestoryEvent>(OnFactionDestory);
        eventManager.Subscribe<UpgradeTechniqueEvent>(OnUpgraeTechnique);
        eventManager.Subscribe<TechniqueFinishedEvent>(OnTechniqueFinished);
        eventManager.Subscribe<InitiativeChangeCapitalEvent>(OnInitiativeChangeCapital);
        eventManager.Subscribe<ForcedChangeCapitalEvent>(OnForcedChangeCapital);
        eventManager.Subscribe<GetControlEvent>(OnGetControl);

        // 人物事件
        eventManager.Subscribe<JailBreakSuccessedEvent>(OnJailBreakSuccessed);
        eventManager.Subscribe<JailBreakFailedEvent>(OnJailBreakFailed);
        eventManager.Subscribe<ConvinceSuccessedEvent>(OnConvinceSuccessed);
        eventManager.Subscribe<ConvinceFailedEvent>(OnConvinceFailed);
        eventManager.Subscribe<InformationAcquisitionSuccessedEvent>(OnInformationAcquisitionSuccessed);
        eventManager.Subscribe<InformationAcquisitionFailedEvent>(OnInformationAcquisitionFailed);
        eventManager.Subscribe<SpyingSuccessedEvent>(OnSpyingSuccessed);
        eventManager.Subscribe<SypingFailedEvent>(OnSypingFailed);
        eventManager.Subscribe<DestroySuccessedEvent>(OnDestroySuccessed);
        eventManager.Subscribe<DestroyFailedEvent>(OnDestroyFailed);
        eventManager.Subscribe<InstigateSuccessedEvent>(OnInstigateSuccessed);
        eventManager.Subscribe<InstigateFailedEvent>(OnInstigateFailed);
        eventManager.Subscribe<GossipSuccessedEvent>(OnGossipSuccessed);
        eventManager.Subscribe<GossipFailedEvent>(OnGossipFailed);
        eventManager.Subscribe<SearchFinishedEvent>(OnSearchFinished);
        eventManager.Subscribe<SpierFoundEvent>(OnSpierFound);
        eventManager.Subscribe<TreasureFoundEvent>(OnTreasureFound);
        eventManager.Subscribe<ShowMessageEvent>(OnShowMessage);
        eventManager.Subscribe<DeathEvent>(OnDeath);
        eventManager.Subscribe<LeaveEvent>(OnLeave);
        eventManager.Subscribe<BeKilledEvent>(OnBeKilled);
        eventManager.Subscribe<DeathChangeLeaderEvent>(OnDeathChangeLeader);
        eventManager.Subscribe<DeathChangeFactionEvent>(OnDeathChangeFaction);
        eventManager.Subscribe<StudyTitleFinishedEvent>(OnStudyTitleFinished);
        eventManager.Subscribe<StudySkillFinishedEvent>(OnStudySkillFinished);
        eventManager.Subscribe<StudyStuntFinishedEvent>(OnStudyStuntFinished);
        eventManager.Subscribe<AwardedTreasureEvent>(OnAwardedTreasure);
        eventManager.Subscribe<ConfiscatedTreasureEvent>(OnConfiscatedTreasure);
        eventManager.Subscribe<CapturedByArchitectureEvent>(OnCapturedByArchitecture);
        eventManager.Subscribe<CreateBrotherEvent>(OnCreateBrother);
        eventManager.Subscribe<CreateSisterEvent>(OnCreateSister);
        eventManager.Subscribe<CreateSpouseEvent>(OnCreateSpouse);

        // 部队事件
        eventManager.Subscribe<TroopCreateEvent>(OnTroopCreate);
        eventManager.Subscribe<EndPathEvent>(OnEndPath);
        eventManager.Subscribe<PathNotFoundEvent>(OnPathNotFound);
        eventManager.Subscribe<NormalAttackEvent>(OnNormalAttack);
        eventManager.Subscribe<CombatMethodAttackEvent>(OnCombatMethodAttack);
        eventManager.Subscribe<CastStratagemEvent>(OnCastStratagem);
        eventManager.Subscribe<CriticalStrikeEvent>(OnCriticalStrike);
        eventManager.Subscribe<ReceiveCriticalStrikeEvent>(OnReceiveCriticalStrike);
        eventManager.Subscribe<WaylayEvent>(OnWaylay);
        eventManager.Subscribe<ReceiveWaylayEvent>(OnReceiveWaylay);
        eventManager.Subscribe<SurroundEvent>(OnSurround);
        eventManager.Subscribe<SetCombatMethodEvent>(OnSetCombatMethod);
        eventManager.Subscribe<SetStratagemEvent>(OnSetStratagem);
        eventManager.Subscribe<StratagemSuccessedEvent>(OnStratagemSuccessed);
        eventManager.Subscribe<ChaosEvent>(OnChaos);
        eventManager.Subscribe<RumourEvent>(OnRumour);
        eventManager.Subscribe<AttractEvent>(OnAttract);
        eventManager.Subscribe<RecoverFromChaosEvent>(OnRecoverFromChaos);
        eventManager.Subscribe<CastDeepChaosEvent>(OnCastDeepChaos);
        eventManager.Subscribe<ResistStratagemEvent>(OnResistStratagem);
        eventManager.Subscribe<AmbushEvent>(OnAmbush);
        eventManager.Subscribe<StopAmbushEvent>(OnStopAmbush);
        eventManager.Subscribe<DiscoverAmbushEvent>(OnDiscoverAmbush);
        eventManager.Subscribe<RoutEvent>(OnRout);
        eventManager.Subscribe<RoutedEvent>(OnRouted);
        eventManager.Subscribe<BreakWallEvent>(OnBreakWall);
        eventManager.Subscribe<SpreadBurntEvent>(OnSpreadBurnt);
        eventManager.Subscribe<OccupyArchitectureEvent>(OnOccupyArchitecture);
        eventManager.Subscribe<AntiAttackEvent>(OnAntiAttack);
        eventManager.Subscribe<AntiArrowAttackEvent>(OnAntiArrowAttack);
        eventManager.Subscribe<LevyGrainEvent>(OnLevyGrain);
        eventManager.Subscribe<CutRoutewayEvent>(OnCutRouteway);
        eventManager.Subscribe<CutRoutewayResultEvent>(OnCutRoutewayResult);
        eventManager.Subscribe<GetNewCaptiveEvent>(OnGetNewCaptive);
        eventManager.Subscribe<ReleaseTroopCaptiveEvent>(OnReleaseTroopCaptive);
        eventManager.Subscribe<PersonChallengeEvent>(OnPersonChallenge);
        eventManager.Subscribe<PersonControversyEvent>(OnPersonControversy);
        eventManager.Subscribe<OutburstEvent>(OnOutburst);
        eventManager.Subscribe<ApplyStuntEvent>(OnApplyStunt);
        eventManager.Subscribe<TransportArrivedEvent>(OnTransportArrived);
        
        eventManager.Subscribe<ApplyTroopEvent>(OnApplyTroop);
    }

    #region 建筑事件

    private void OnAppointMayor(AppointMayorEvent e)
    {
        Session.MainGame.mainGameScreen.Appointmayor(e.leader, e.person);
    }

    private void OnZhaoXian(ZhaoXianEvent e)
    {
        Session.MainGame.mainGameScreen.Zhaoxian(e.leader, e.person);
    }

    private void OnSelectPrince(SelectPrinceEvent e)
    {
        Session.MainGame.mainGameScreen.Selectprince(e.leader, e.person);
    }

    private void OnPopulationEscape(PopulationEscapeEvent e)
    {
        Session.MainGame.mainGameScreen.ArchitecturePopulationEscape(e.architecture, e.quantity);
    }

    private void OnPopulationEnter(PopulationEnterEvent e)
    {
        Session.MainGame.mainGameScreen.ArchitecturePopulationEnter(e.architecture, e.quantity);
    }

    private void OnRecentlyAttacked(RecentlyAttackedEvent e)
    {
        Session.MainGame.mainGameScreen.ArchitectureBeginRecentlyAttacked(e.architecture);
    }

    private void OnReleaseCaptive(ReleaseCaptiveEvent e)
    {
        Session.MainGame.mainGameScreen.ArchitectureReleaseCaptiveAfterOccupied(e.architecture, e.persons);
    }

    private void OnDisasterOccurred(DisasterOccurredEvent e)
    {
        Session.MainGame.mainGameScreen.Architecturefashengzainan(e.architecture, e.disasterId);
    }

    private void OnFacilityCompleted(FacilityCompletedEvent e)
    {
        Session.MainGame.mainGameScreen.ArchitectureFacilityCompleted(e.architecture, e.facility);
    }

    private void OnRewardPerson(RewardPersonEvent e)
    {
        Session.MainGame.mainGameScreen.ArchitectureRewardPersons(e.architecture, e.persons);
    }

    private void OnHirePerson(HirePersonEvent e)
    {
        Session.MainGame.mainGameScreen.ArchitectureHirePerson(e.persons);
    }

    #endregion

    #region 俘虏事件

    private void OnPlayerRelease(PlayerReleaseEvent e)
    {
        Session.MainGame.mainGameScreen.CaptivePlayerRelease(e.from, e.to, e.captive);
    }

    private void OnRelease(ReleaseEvent e)
    {
        Session.MainGame.mainGameScreen.CaptiveRelease(e.success, e.from, e.to, e.person);
    }

    private void OnSelfRelease(SelfReleaseEvent e)
    {
        Session.MainGame.mainGameScreen.SelfCaptiveRelease(e.captive);
    }

    private void OnEscape(EscapeEvent e)
    {
        //captive.Scenario.GameScreen.CaptiveEscape(captive);
    }

    #endregion

    #region 势力事件

    private void OnAfterCatchLeader(AfterCatchLeaderEvent e)
    {
        Session.MainGame.mainGameScreen.FactionAfterCatchLeader(e.leader, e.faction);
    }

    private void OnFactionDestory(FactionDestoryEvent e)
    {
        Session.MainGame.mainGameScreen.FactionDestroy(e.faction);
    }

    private void OnUpgraeTechnique(UpgradeTechniqueEvent e)
    {
        Session.MainGame.mainGameScreen.FactionUpgradeTechnique(e.faction, e.technique, e.architecture);
    }

    private void OnTechniqueFinished(TechniqueFinishedEvent e)
    {
        Session.MainGame.mainGameScreen.FactionTechniqueFinished(e.faction, e.technique);
    }

    private void OnInitiativeChangeCapital(InitiativeChangeCapitalEvent e)
    {
        Session.MainGame.mainGameScreen.FactionInitialtiveChangeCapital(e.faction, e.oldCapital, e.newCapital);
    }

    private void OnForcedChangeCapital(ForcedChangeCapitalEvent e)
    {
        Session.MainGame.mainGameScreen.FactionForcedChangeCapital(e.faction, e.oldCapital, e.newCapital);
    }

    private void OnGetControl(GetControlEvent e)
    {
        Session.MainGame.mainGameScreen.FactionGetControl(e.faction);
    }

    #endregion

    #region 人物事件

    private void OnJailBreakSuccessed(JailBreakSuccessedEvent e)
    {
        Session.MainGame.mainGameScreen.PersonJailBreak(e.person, e.captive);
    }

    private void OnJailBreakFailed(JailBreakFailedEvent e)
    {
        Session.MainGame.mainGameScreen.PersonJailBreakFailed(e.person, e.architecture);
    }

    private void OnConvinceSuccessed(ConvinceSuccessedEvent e)
    {
        Session.MainGame.mainGameScreen.PersonConvinceSuccess(e.source, e.target, e.faction);
    }

    private void OnConvinceFailed(ConvinceFailedEvent e)
    {
        Session.MainGame.mainGameScreen.PersonConvinceFailed(e.source, e.target);
    }

    private void OnInformationAcquisitionSuccessed(InformationAcquisitionSuccessedEvent e)
    {
        Session.MainGame.mainGameScreen.PersonInformationObtained(e.person, e.information);
    }

    private void OnInformationAcquisitionFailed(InformationAcquisitionFailedEvent e)
    {
        Session.MainGame.mainGameScreen.qingbaoshibai(e.person);
    }

    private void OnSpyingSuccessed(SpyingSuccessedEvent e)
    {
        // Session.MainGame.mainGameScreen.PersonSpySuccess(e.person, e.architecture);
    }

    private void OnSypingFailed(SypingFailedEvent e)
    {
        // Session.MainGame.mainGameScreen.PersonSpyFailed(e.person, e.architecture);
    }

    private void OnDestroySuccessed(DestroySuccessedEvent e)
    {
        Session.MainGame.mainGameScreen.PersonDestroySuccess(e.person, e.architecture, e.down);
    }

    private void OnDestroyFailed(DestroyFailedEvent e)
    {
        Session.MainGame.mainGameScreen.PersonDestroyFailed(e.person, e.architecture);
    }

    private void OnInstigateSuccessed(InstigateSuccessedEvent e)
    {
        Session.MainGame.mainGameScreen.PersonInstigateSuccess(e.person, e.architecture, e.down);
    }

    private void OnInstigateFailed(InstigateFailedEvent e)
    {
        Session.MainGame.mainGameScreen.PersonInstigateFailed(e.person, e.architecture);
    }

    private void OnGossipSuccessed(GossipSuccessedEvent e)
    {
        Session.MainGame.mainGameScreen.PersonGossipSuccess(e.person, e.architecture);
    }

    private void OnGossipFailed(GossipFailedEvent e)
    {
        Session.MainGame.mainGameScreen.PersonGossipFailed(e.person, e.architecture);
    }

    private void OnSearchFinished(SearchFinishedEvent e)
    {
        Session.MainGame.mainGameScreen.PersonSearchFinished(e.person, e.architecture, e.result);
    }

    private void OnSpierFound(SpierFoundEvent e)
    {
        // Session.MainGame.mainGameScreen.PersonSpyFound(e.person, e.spier);
    }

    private void OnTreasureFound(TreasureFoundEvent e)
    {
        Session.MainGame.mainGameScreen.PersonTreasureFound(e.person, e.treasure);
    }

    private void OnShowMessage(ShowMessageEvent e)
    {
        // Session.MainGame.mainGameScreen.PersonShowMessage(e.person, e.message);
    }

    private void OnDeath(DeathEvent e)
    {
        Session.MainGame.mainGameScreen.PersonDeath(e.person, e.killer, e.architecture, e.troop);
    }

    private void OnLeave(LeaveEvent e)
    {
        Session.MainGame.mainGameScreen.PersonLeave(e.person, e.architecture);
    }

    private void OnBeKilled(BeKilledEvent e)
    {
        Session.MainGame.mainGameScreen.PersonBeKilled(e.person, e.architecture);
    }

    private void OnDeathChangeLeader(DeathChangeLeaderEvent e)
    {
        Session.MainGame.mainGameScreen.PersonChangeLeader(e.faction, e.leader, e.changeName, e.oldName);
    }

    private void OnDeathChangeFaction(DeathChangeFactionEvent e)
    {
        Session.MainGame.mainGameScreen.PersonDeathChangeFaction(e.dead, e.leader, e.oldName);
    }

    private void OnStudyTitleFinished(StudyTitleFinishedEvent e)
    {
        Session.MainGame.mainGameScreen.PersonStudyTitleFinished(e.person, e.title, e.success);
    }

    private void OnStudySkillFinished(StudySkillFinishedEvent e)
    {
        Session.MainGame.mainGameScreen.PersonStudySkillFinished(e.person, e.skillString, e.success);
    }

    private void OnStudyStuntFinished(StudyStuntFinishedEvent e)
    {
        Session.MainGame.mainGameScreen.PersonStudyStuntFinished(e.person, e.stunt, e.success);
    }

    private void OnAwardedTreasure(AwardedTreasureEvent e)
    {
        Session.MainGame.mainGameScreen.PersonBeAwardedTreasure(e.person, e.treasure);
    }

    private void OnConfiscatedTreasure(ConfiscatedTreasureEvent e)
    {
        Session.MainGame.mainGameScreen.PersonBeConfiscatedTreasure(e.person, e.treasure);
    }

    private void OnCapturedByArchitecture(CapturedByArchitectureEvent e)
    {
        Session.MainGame.mainGameScreen.PersonCapturedByArchitecture(e.person, e.architecture);
    }

    private void OnCreateBrother(CreateBrotherEvent e)
    {
        Session.MainGame.mainGameScreen.CreateBrother(e.p1, e.p2);
    }

    private void OnCreateSister(CreateSisterEvent e)
    {
        Session.MainGame.mainGameScreen.CreateSister(e.p1, e.p2);
    }

    private void OnCreateSpouse(CreateSpouseEvent e)
    {
        Session.MainGame.mainGameScreen.CreateSpouse(e.p1, e.p2);
    }

    #endregion

    #region  部队事件

    private void OnTroopCreate(TroopCreateEvent e)
    {
        Session.MainGame.mainGameScreen.TroopCreate(e.troop);
    }

    private void OnEndPath(EndPathEvent e)
    {
        Session.MainGame.mainGameScreen.TroopEndPath(e.troop);
    }

    private void OnPathNotFound(PathNotFoundEvent e)
    {
        Session.MainGame.mainGameScreen.TroopPathNotFound(e.troop);
    }

    private void OnNormalAttack(NormalAttackEvent e)
    {
        Session.MainGame.mainGameScreen.TroopNormalAttack(e.attacker, e.defender);
    }

    private void OnCombatMethodAttack(CombatMethodAttackEvent e)
    {
        Session.MainGame.mainGameScreen.TroopCombatMethodAttack(e.attacker, e.defender, e.combatMethod);
    }

    private void OnCastStratagem(CastStratagemEvent e)
    {
        Session.MainGame.mainGameScreen.TroopCastStratagem(e.attacker, e.defender, e.stratagem);
    }

    private void OnCriticalStrike(CriticalStrikeEvent e)
    {
        Session.MainGame.mainGameScreen.TroopCriticalStrike(e.attacker, e.defender);
    }

    private void OnReceiveCriticalStrike(ReceiveCriticalStrikeEvent e)
    {
        Session.MainGame.mainGameScreen.TroopReceiveCriticalStrike(e.attacker, e.defender);
    }

    private void OnWaylay(WaylayEvent e)
    {
        Session.MainGame.mainGameScreen.TroopWaylay(e.attacker, e.defender);
    }

    private void OnReceiveWaylay(ReceiveWaylayEvent e)
    {
        Session.MainGame.mainGameScreen.TroopReceiveWaylay(e.attacker, e.defender);
    }

    private void OnSurround(SurroundEvent e)
    {
        Session.MainGame.mainGameScreen.TroopSurround(e.attacker, e.defender);
    }

    private void OnSetCombatMethod(SetCombatMethodEvent e)
    {
        Session.MainGame.mainGameScreen.TroopSetCombatMethod(e.troop, e.combatMethod);
    }

    private void OnSetStratagem(SetStratagemEvent e)
    {
        Session.MainGame.mainGameScreen.TroopSetStratagem(e.troop, e.stratagem);
    }

    private void OnStratagemSuccessed(StratagemSuccessedEvent e)
    {
        Session.MainGame.mainGameScreen.TroopStratagemSuccess(e.attacker, e.defender, e.stratagem, e.isHarmful);
    }

    private void OnChaos(ChaosEvent e)
    {
        Session.MainGame.mainGameScreen.TroopChaos(e.troop, e.deepChaos);
    }

    private void OnRumour(RumourEvent e)
    {
        Session.MainGame.mainGameScreen.TroopRumour(e.troop);
    }

    private void OnAttract(AttractEvent e)
    {
        Session.MainGame.mainGameScreen.TroopAttract(e.attacker, e.defender);
    }

    private void OnRecoverFromChaos(RecoverFromChaosEvent e)
    {
        Session.MainGame.mainGameScreen.TroopRecoverFromChaos(e.troop);
    }

    private void OnCastDeepChaos(CastDeepChaosEvent e)
    {
        Session.MainGame.mainGameScreen.TroopCastDeepChaos(e.attacker, e.defender);
    }

    private void OnResistStratagem(ResistStratagemEvent e)
    {
        Session.MainGame.mainGameScreen.TroopResistStratagem(e.attacker, e.defender, e.stratagem, e.isHarmful);
    }

    private void OnAmbush(AmbushEvent e)
    {
        Session.MainGame.mainGameScreen.TroopAmbush(e.troop);
    }

    private void OnStopAmbush(StopAmbushEvent e)
    {
        Session.MainGame.mainGameScreen.TroopStopAmbush(e.troop);
    }

    private void OnDiscoverAmbush(DiscoverAmbushEvent e)
    {
        Session.MainGame.mainGameScreen.TroopDiscoverAmbush(e.attacker, e.defender);
    }

    private void OnRout(RoutEvent e)
    {
        Session.MainGame.mainGameScreen.TroopRout(e.attacker, e.defender);
    }

    private void OnRouted(RoutedEvent e)
    {
        Session.MainGame.mainGameScreen.TroopRouted(e.attacker, e.defender);
    }

    private void OnBreakWall(BreakWallEvent e)
    {
        Session.MainGame.mainGameScreen.TroopBreakWall(e.troop, e.architecture);
    }

    private void OnSpreadBurnt(SpreadBurntEvent e)
    {
        Session.MainGame.mainGameScreen.TroopGetSpreadBurnt(e.troop);
    }

    private void OnOccupyArchitecture(OccupyArchitectureEvent e)
    {
        Session.MainGame.mainGameScreen.TroopOccupyArchitecture(e.troop, e.architecture);
    }

    private void OnAntiAttack(AntiAttackEvent e)
    {
        Session.MainGame.mainGameScreen.TroopAntiAttack(e.attacker, e.defender);
    }

    private void OnAntiArrowAttack(AntiArrowAttackEvent e)
    {
        Session.MainGame.mainGameScreen.TroopAntiArrowAttack(e.attacker, e.defender);
    }

    private void OnLevyGrain(LevyGrainEvent e)
    {
        Session.MainGame.mainGameScreen.TroopLevyFieldFood(e.troop, e.grain);
    }

    private void OnCutRouteway(CutRoutewayEvent e)
    {
        Session.MainGame.mainGameScreen.TroopStartCutRouteway(e.troop, e.days);
    }

    private void OnCutRoutewayResult(CutRoutewayResultEvent e)
    {
        Session.MainGame.mainGameScreen.TroopEndCutRouteway(e.troop, e.success);
    }

    private void OnGetNewCaptive(GetNewCaptiveEvent e)
    {
        Session.MainGame.mainGameScreen.TroopGetNewCaptive(e.troop, e.persons);
    }

    private void OnReleaseTroopCaptive(ReleaseTroopCaptiveEvent e)
    {
        Session.MainGame.mainGameScreen.TroopReleaseCaptive(e.troop, e.persons);
    }

    private void OnPersonChallenge(PersonChallengeEvent e)
    {
        Session.MainGame.mainGameScreen.TroopPersonChallenge(e.win, e.attackingTroop, e.attacker, e.defenseTroop, e.defender);
    }

    private void OnPersonControversy(PersonControversyEvent e)
    {
        Session.MainGame.mainGameScreen.TroopPersonControversy(e.win, e.attackingTroop, e.attacker, e.defenseTroop, e.defender);
    }

    private void OnOutburst(OutburstEvent e)
    {
        Session.MainGame.mainGameScreen.TroopOutburst(e.troop, e.kind);
    }

    private void OnApplyStunt(ApplyStuntEvent e)
    {
        Session.MainGame.mainGameScreen.TroopApplyStunt(e.troop, e.stunt);
    }

    private void OnTransportArrived(TransportArrivedEvent e)
    {
        Session.MainGame.mainGameScreen.AskWhenTransportArrived(e.troop, e.architecture);
    }

    private void OnApplyTroop(ApplyTroopEvent e)
    {
        Session.MainGame.mainGameScreen.TroopApplyTroopEvent(e.troopEvent, e.troop);
    }

    #endregion


}