# Краткое описание скриптов проекта

Описание включает собственные скрипты из `Assets/Scripts` и редакторские расширения из `Assets/Editor`. Сторонние пакеты, `Library` и импортированные ассеты не описывались.

## Assets/Scripts

### Animators

- `Character/AnimatorSwitcher.cs` - переключает наборы анимаций персонажа.
- `Character/PlayerAnimator.cs` - управляет параметрами аниматора игрока.
- `Character/StepAnimator.cs`, `StepDirection.cs` - рассчитывают шаги и направление шага.
- `Character/WeaponAnimatorEntry.cs`, `WeaponType.cs` - описывают привязку типа оружия к анимациям.
- `Cursor/CursorAnimator.cs` - анимирует игровой курсор.
- `Effects/DamageShakeAnimator.cs` - тряска при получении урона.
- `Effects/PickupAnimator.cs` - анимация подбора предмета.
- `Effects/PooledParticleEffect.cs` - pooled-эффект частиц.
- `Feedbacks/Feedback.cs`, `FeedbackGroup.cs` - базовая система визуальных feedback-эффектов.
- `Feedbacks/ColorEmissionFeedback.cs`, `ColorFlashFeedback.cs`, `ColorFlashImageFeedback.cs`, `ShakeFeedback.cs` - конкретные feedback-эффекты цвета, emission, UI-картинки и shake.
- `Feedbacks/HealthFeedbackTrigger.cs` - запускает feedback при изменении здоровья.
- `Highlighting/HighlighterBase.cs`, `HighlightAnimator.cs`, `HighlightLayerSwitcher.cs`, `InventorySlotHighlight.cs` - подсветка объектов и слотов инвентаря.

### Characters

- `AimRotationSolver.cs` - расчет поворота прицеливания.
- `CharacterDied.cs` - обработка смерти персонажа.
- `CharacterEffects.cs` - запуск эффектов персонажа.
- `Attack/AttackData.cs`, `Attacker.cs`, `AttackShapeBase.cs`, `SphereForwardAttackShape.cs` - данные атаки, исполнитель атаки и форма попадания.

#### Characters/Boss

- `AI/BossExcavatorBrain.cs` и partial-файлы `Attack`, `AttackQueue`, `AttackRules`, `AttackSelection`, `Stall`, `State` - логика выбора состояний и атак босса-экскаватора.
- `Attacks/BossExcavatorBucketAttack.cs`, `BucketAttack.Combat.cs` - атака ковшом и ее боевое применение.
- `Attacks/BossExcavatorChargeAttack.cs`, `ChargeAttack.Combat.cs` - рывок босса и урон во время рывка.
- `Attacks/BossExcavatorScrapTrailAttack.cs` - атака с созданием следа из мусора.
- `Attacks/BossExcavatorSweepAttack.cs` - sweeping-атака босса.
- `Attacks/BossExcavatorThrowAttack.cs` - бросок снаряда/мусора.
- `Components/BossExcavatorAim.cs` - прицеливание босса.
- `Components/BossExcavatorArm.cs` - управление рукой/ковшом.
- `Components/BossExcavatorMove.cs` и partial-файлы `Agent`, `Collision`, `Gizmo`, `Motion`, `Navigation`, `Points`, `Runtime`, `Targeting` - движение, навигация, цели, столкновения и debug-отрисовка босса.
- `Core/BossExcavator.cs` и partial-файлы `Facade`, `Gizmo`, `Targeting` - главный фасад босса, доступ к подсистемам и debug.
- `Core/BossExcavatorConfig.cs`, `BossExcavatorConfig.Properties.cs`, `BossExcavatorMotionProfile.cs` - настройки фаз, движения и атак босса.
- `Core/BossExcavatorStateMachine.cs`, `BossExcavatorState.cs`, `BossExcavatorPhase.cs` - состояние и фазы босса.
- `Core/BossExcavatorAttack.cs`, `BossExcavatorArmPose.cs`, `BossExcavatorAxis.cs`, `BossExcavatorTargetPoint.cs` - enum-описания атак, поз, осей и точек цели.
- `Core/BossExcavatorDebugRuntime.cs` - runtime-debug босса.
- `PhaseThree/BossExcavatorPhaseThreeController.cs`, `BossExcavatorPhaseThreeMinion.cs`, `BossRoomEnemySpawnPoint.cs` - третья фаза босса и спавн помощников.
- `Scrap/BossScrapCubeProjectile.cs`, `BossScrapCubeSpawner.cs`, `BossScrapTrailBlock.cs`, `BossScrapTrailBlockSpawner.cs` - снаряды и блоки мусора босса.

#### Characters/Enemy

- `Core/Enemy.cs` - корневой компонент врага.
- `Core/EnemyState.cs`, `IEnemyBrain.cs`, `IEnemyAlert.cs` - состояние и контракты AI/alert.
- `Core/EnemyMove.cs`, `EnemyRotator.cs`, `EnemyPatrolPicker.cs`, `EnemyAimCollider.cs` - движение, поворот, патруль и aim-коллайдер.
- `Core/EnemyAnimation.cs`, `EnemyAlertPulse.cs`, `EnemyDebugView.cs`, `CurrencyDropOnDeath.cs` - анимация, сигнал тревоги, debug и дроп валюты.
- `Drone/EnemyDroneBrain.cs`, `EnemyDroneMove.cs`, `EnemyDroneCrash.cs` - AI, движение и падение дрона.
- `Melee/EnemyMeleeBrain.cs` и partial-файлы `Combat`, `Gizmo`, `Idle`, `State`, `Utility` - AI ближнего врага.
- `Melee/EnemySuicideAttack.cs` - самоубийственная атака.
- `Room/EnemyRoomAlert.cs`, `EnemyRoomLock.cs` - тревога и блокировка комнаты врагами.
- `Steering/EnemySteering.cs` и partial-файлы `Combat`, `Nav`, `Point`, `Probe` - steering-навигация врагов.

#### Characters/Fire

- `BulletFireExecutor.cs`, `RocketFireExecutor.cs`, `ShotgunFireExecutor.cs`, `FireExecutor.cs` - исполнители стрельбы для разных типов оружия.
- `Ammo/Core/Ammo.cs` и partial-файлы `Collision`, `Motion` - базовая логика снаряда, движение и столкновения.
- `Ammo/Core/AmmoReturner.cs`, `IAmmoSpawner.cs` - возврат ammo в пул и контракт спавнера.
- `Ammo/Effect/AmmoEffect.cs`, `AmmoParticleSystem.cs`, `AmmoRendererDisabler.cs`, `AmmoTrailRenderer.cs` - эффекты жизненного цикла снаряда.
- `Ammo/Lifecycle/AmmoLifeListener.cs`, `AmmoEnemyAlert.cs`, `AmmoLifePath.cs` - подписчики жизненного цикла ammo, alert и путь полета.
- `Ammo/Projectiles/Bullet.cs`, `Rocket.cs` - конкретные projectile-типы.
- `FireExecutrer/FireExecutorPresenter.cs`, `FireShotContext.cs` - presentation-слой выстрела и контекст выстрела.
- `FireExecutrer/Calculator/FireDamageCalculator.cs`, `FireModifierState.cs`, `FireRateProvider.cs`, `IDamageCalculator.cs`, `IFireRateProvider.cs` - расчет урона, скорострельности и модификаторов.
- `FireExecutrer/Strategy/BulletShotStrategy.cs`, `RocketShotStrategy.cs`, `ShotgunBurstShotStrategy.cs`, `IShotStrategy.cs` - стратегии выполнения выстрелов.

#### Characters/Turret

- `Turret.cs` - основная логика турели.
- `IdleRotator.cs`, `TargetRotator.cs`, `TargetVision.cs` - idle-поворот, наведение и поиск цели.
- `TurretHeadCrash.cs` - поведение головы турели при разрушении.

### Developer

- `DeveloperCheatSave.cs` - dev-инструмент для cheat/save-сценариев.

### Environment

- `DamageableObject.cs` - разрушаемый объект окружения.
- `FadableObstacle.cs`, `IFadable.cs` - препятствие, которое может становиться прозрачным.
- `Harvestable.cs` - объект, который можно собрать/разрушить ради ресурса.
- `Light/StaticRotation.cs` - фиксирует/задает статичный поворот.

### Interactables

- `Interactable.cs` - базовый интерактивный объект.
- `DoorInteractable.cs` - взаимодействие с дверью.
- `FractureFx.cs` - эффект разрушения.
- `PickupOnDeath.cs` - создание pickup после смерти/разрушения.

### Level

- `Core/Generation/LevelGenerator.cs` - главный генератор уровня.
- `Core/Generation/LevelGenerationContext.cs`, `LevelGeneratorTypes.cs`, `LevelGeneratorUtility.cs`, `LevelDoorAlignmentUtility.cs`, `LevelPlanBuilder.cs` - контекст, типы, утилиты, выравнивание дверей и план уровня.
- `Core/Corridors/LevelCorridorBoundsBuilder.cs`, `LevelCorridorExecutor.cs` - расчет границ и создание коридоров.
- `Core/Rooms/LevelRoomBoundsCalculator.cs`, `LevelRoomFinalizer.cs`, `LevelRoomPlacer.cs`, `LevelRoomShellInstantiator.cs`, `StartRoomCenterer.cs` - размещение, создание shell и финализация комнат.
- `Core/Runtime/LevelRoomStreamer.cs`, `LevelRuntimeNavMesh.cs` - runtime-стриминг комнат и NavMesh.
- `Corridors/LevelCorridorBuilder.cs` - построение коридора.
- `Profiles/LevelGenerationProfile.cs`, `LevelRoomPrefabLibrary.cs`, `LevelSequenceProfile.cs`, `WeightedRoomGeneratorPrefab.cs` - профили генерации, библиотеки комнат и weighted-префабы.

### Pickups

- `Core/BasePickup.cs`, `BaseAnimatedPickup.cs` - базовые pickup-компоненты.
- `Core/PickupReturner.cs`, `ScaleCompensator.cs` - возврат pickup в пул и компенсация масштаба.
- `Idle/PickupIdle.cs`, `PickupIdleBehaviour.cs`, `PickupIdleMotion.cs`, `PickupIdleParticle.cs` - idle-анимация pickup.
- `Items/BerryPickup.cs`, `CurrencyPickup.cs`, `ItemPickup.cs` - pickup ягод, валюты и предметов.

### Player

- `BerryWallet.cs`, `CurrencyWallet.cs` - кошельки игрока для ягод и валюты.
- `Animation/AttackEndedStateBehaviour.cs`, `PlayerAnimationEvents.cs` - события анимаций игрока.
- `Combat/Attacks/PlayerMeleeAttack.cs`, `PlayerRangedFire.cs` - ближняя и дальняя атака игрока.
- `Combat/Core/PlayerCombat.cs`, `PlayerCombatCore.cs`, `PlayerBattleState.cs`, `PlayerBattleTimer.cs`, `PlayerActiveWeaponType.cs` - состояние боя, таймеры и активное оружие.
- `Input/PlayerInputActions.cs` - сгенерированные input actions.
- `Interact/CarryAttachment.cs`, `CharacterInteractor.cs`, `PickupTrigger.cs`, `PlayerInteraction.cs` - перенос объектов, взаимодействие и pickup-триггеры.
- `Inventory/Core/Inventory.cs`, `InventorySlot.cs`, `InventoryDropper.cs`, `PlayerInventory.cs` - инвентарь, слоты и выброс предметов.
- `Inventory/UI/InventoryView.cs`, `InventorySlotView.cs` - UI инвентаря.
- `Modifier/PlayerModifier.cs`, `PlayerModifierApplier.cs`, `PlayerModifierContext.cs`, `PlayerModifierStack.cs`, `PlayerModifierStat.cs`, `PlayerMultiplierModifier.cs`, `PlayerHealthRegenUnlock.cs` - система модификаторов игрока.
- `Movement/CameraMover.cs`, `CharacterJumper.cs`, `CharacterMover.cs`, `CharacterRotator.cs`, `PlayerMovementGate.cs` - базовое движение, камера, прыжок, поворот и блокировка движения.
- `Movement/PlayerMovement/PlayerMovement.cs`, `PlayerMoveApplier.cs`, `PlayerJumpAction.cs`, `PlayerSprint.cs`, `PlayerRotationByState.cs`, `PlayerMoveStopDelay.cs`, `PlayerAttackMovementBlend.cs` - движение игрока, спринт, прыжок, задержка остановки и blend с атакой.
- `Root/Player.cs`, `PlayerDie.cs`, `PlayerVictory.cs`, `PlayerRoundStats.cs`, `PlayerRoundStatsSnapshot.cs`, `LineSightFader.cs` - корень игрока, смерть, победа, статистика раунда и скрытие препятствий по линии зрения.
- `UI/Cursor/CursorManager.cs`, `CursorInputHandler.cs`, `CursorRadialIndicator.cs`, `FireCooldownCursorIndicator.cs` - управление курсором и индикаторы кулдауна.
- `UI/Hud/BerryWalletView.cs`, `CurrencyWalletView.cs`, `RemainingEnemyIndicatorView.cs`, `RemainingEnemyOverlay.cs` - HUD кошельков и оставшихся врагов.
- `UI/Pause/PlayerPause.cs` - пауза игрока.
- `Weapon/WeaponHolder.cs`, `WeaponGrid.cs`, `InventoryWeaponBinder.cs`, `HeldMode.cs` - удержание оружия, сетка оружия и связь с инвентарем.
- `Weapon/Modificator/WeaponModifier.cs`, `WeaponModifierApplier.cs`, `WeaponModifierContext.cs`, `WeaponModifierStack.cs`, `WeaponTypeFilteredModifier.cs` - система модификаторов оружия.

### Room

- `Build/RoomFloorOccupancy.cs`, `RoomInteriorBlockFiller.cs`, `RoomShellBuilder.cs` - занятость пола, заполнение интерьера и построение shell комнаты.
- `Config/EnemySpawnConfig.cs`, `EnemySpawnHeight.cs`, `EnemySpawnPicker.cs`, `Enums.cs`, `NookPrefabConfig.cs`, `WeightedPrefab.cs`, `WeightedPrefabPicker.cs` - конфиги спавна, высоты, enum-ы и weighted-выбор префабов.
- `Core/RoomGenerator.cs`, `RoomCombatLock.cs`, `RoomEnterTrigger.cs`, `RoomRuntimeState.cs` - генератор комнаты, боевой lock, вход и runtime-состояние.
- `Doors/RoomDoorGate.cs`, `RoomDoorMarker.cs`, `RoomDoorPlan.cs` - двери комнаты, маркеры и план дверей.
- `Planning/RoomDoorPlanner.cs`, `RoomPassagePlanner.cs` - планирование дверей и проходов.
- `Spawn/RoomContentSpawner.cs`, `RoomNookSpawner.cs` - спавн содержимого комнаты и nook-объектов.
- `Utils/Chunk/ChunkRootContext.cs`, `ChunkRootInstaller.cs`, `ChunkVariantRootComposer.cs`, `ChunkVariantSwitcherBase.cs` - базовая инфраструктура chunk-root и переключения вариантов.
- `Utils/Interior/RoomInteriorBlockRangeCollector.cs`, `RoomInteriorVoxelClusterer.cs`, `RoomInteriorClusterMeshCombiner.cs`, `RoomInteriorChunkCombiner.cs`, `RoomInteriorCombinedMeshOwner.cs` - сбор диапазонов, кластеризация voxel-блоков и объединение мешей.
- `Utils/Interior/RoomInteriorHiddenBlockCuller.cs`, `RoomInteriorSourceDisabler.cs` - скрытие внутренних блоков и отключение исходников.
- `Utils/Interior/RoomInteriorChunkDynamicOnDamage.cs`, `RoomInteriorChunkDynamicOnDamageInstaller.cs` - динамическое поведение interior-chunk после урона.
- `Utils/Interior/RoomInteriorChunkRootCompositionProfile.cs`, `RoomInteriorChunkVariantRootComposer.cs`, `RoomInteriorChunkVariantSwitcher.cs`, `RoomInteriorChunkVariantSwitcherInstaller.cs` - профили и переключение вариантов interior-chunk.

### ScriptableObject / ScriptableObjects

- `FadableSettings.cs` - настройки прозрачности препятствий.
- `FireRateWeaponModifier.cs`, `FireRateAddToMultiplierWeaponModifier.cs` - модификаторы скорострельности оружия.
- `Item/Item.cs`, `ItemEffect.cs`, `FoodEffect.cs`, `ItemAudioProfile.cs` - данные предмета, эффекты еды и аудио-профиль.
- `Modifier/CriticalHitModifier.cs`, `DamageMultiplierModifier.cs`, `DamageMultiplierFilteredModifier.cs`, `ExplosionRadiusMultiplierModifier.cs`, `PelletBonusModifier.cs`, `ProjectileSpeedMultiplierModifier.cs`, `SpreadMultiplierModifier.cs` - ScriptableObject-модификаторы оружия.
- `ScriptableObjects/RoomNoiseProfile.cs`, `RoomTypeProfile.cs` - профили шума и типа комнаты.

### Shop

- `VendingMachine/Data/IShopOffer.cs`, `ModifierOffer.cs`, `ModifierOfferPool.cs`, `ModifierOfferRarity.cs`, `PlayerModifierOffer.cs`, `PlayerModifierPool.cs` - данные офферов магазина и пулов модификаторов.
- `VendingMachine/Runtime/ModifierVendingMachine.cs`, `ModifierVendingMachinePurchase.cs`, `PlayerModifierPurchase.cs`, `PlayerModifierShop.cs` - runtime-логика торговых автоматов и покупок.
- `VendingMachine/UI/Cards/ModifierOfferCardView.cs`, `ModifierOfferCardAnimator.cs` - карточка оффера и hover-анимация.
- `VendingMachine/UI/Menu/ModifierVendingMachineMenuView.cs`, `ModifierVendingMachineMenuAnimator.cs`, `ModifierVendingMachineMenuOpener.cs`, `PlayerModifierMenuView.cs`, `PlayerModifierMenuOpener.cs` - меню магазина и открытие UI.

### Spawners

- `Core/Spawner.cs`, `SpawnerGeneric.cs`, `ServiceLocator.cs`, `InitialSpawnMode.cs` - базовая система пулов/спавнеров и режим начального спавна.
- `Effects/DamagePopupSpawner.cs`, `ParticleEffectSpawner.cs` - спавн popup-урона и particle-эффектов.
- `Pickups/AmmoSpawner.cs`, `PickupSpawner.cs`, `PickupSpawnPoint.cs` - спавн ammo, pickup и точка спавна.

### Stats

- `Stat.cs` - базовая характеристика.
- `Health.cs` - здоровье.
- `Stamina.cs` - выносливость.

### UI

- `Effects/DamagePopup.cs`, `DamagePopupOnHealth.cs`, `LookAtMainCameraAssigner.cs` - popup урона и ориентация UI к камере.
- `Health/BossHealthOverlay.cs`, `BossSegmentedHealthIndicator.cs`, `HealthSmoothSliderIndicator.cs`, `HealthTextIndicator.cs`, `StaminaSmoothSliderIndicator.cs`, `StatIndicatorBase.cs` - индикаторы здоровья, стамины и босса.
- `Menu/Core/BaseMenuView.cs`, `BlurOverlay.cs`, `ButtonMenu.cs`, `DungeonSchematicView.cs`, `MenuButtonScale.cs`, `SceneLoadingScreen.cs` - базовые элементы меню, blur, кнопки, схема данжа и экран загрузки.
- `Menu/Exit/ExitButton.cs`, `ExitMenuView.cs`, `VictoryRoundStatsView.cs` - выход и экран статистики победы.
- `Menu/Main/MainMenuSceneController.cs`, `MainMenuCursorFollower.cs`, `MainMenuScrapBackgroundGenerator.cs`, `MainMenuSettingsPanelView.cs` - главное меню, курсор, фон и настройки.
- `Menu/Pause/PauseController.cs`, `PauseMenuView.cs`, `PauseMenuNavigation.cs`, `PauseMenuButtonListener.cs`, `PauseCameraFov.cs`, `PauseSlideDirection.cs` - пауза, навигация и визуальное состояние камеры.
- `Menu/Settings/SettingsData.cs`, `SettingsSave.cs`, `SettingsPresenter.cs`, `SettingsMenuView.cs`, `SettingsPanelView.cs`, `SettingsSlideDirection.cs` - данные, сохранение, presenter и UI настроек.

### Utils

- `FramesPerSecondDisplay.cs` - вывод FPS.
- `Texter.cs` - утилита для текста.
- `TMPWarmup.cs` - прогрев TextMeshPro.
- `Colorer/IColorer.cs`, `ColorerRenderer.cs`, `ColorerGraphic.cs`, `ColorerEmissionRenderer.cs` - смена цвета renderer/graphic/emission.
- `TimeScale/TimeScaleBase.cs`, `TImeScale.cs`, `TimeScaleSettings.cs` - управление масштабом времени.

## Assets/Editor

- `EnemyDebugViewEditor.cs` - custom editor для debug-вида врага.
- `EnemySpawnHeightDrawer.cs` - drawer для высоты спавна врагов.
- `LevelGeneratorEditor.cs` - editor-инструменты генератора уровня.
- `PrefabConfigDrawer.cs` - drawer для `NookPrefabConfig`.
- `RoomGeneratorEditor.cs` - editor-инструменты генератора комнаты.
- `RoomNoiseProfileEditor.cs` - custom editor профиля шума комнаты.
- `RoomTypeProfileEditor.cs` - custom editor профиля типа комнаты.
- `TmpFontMaterialBuildSanitizer.cs` - подготовка TMP font material перед билдом.
- `WeightedPrefabDrawer.cs` - drawer для weighted-префабов.
