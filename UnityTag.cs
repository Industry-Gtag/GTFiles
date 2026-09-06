using System;

// Token: 0x020003B0 RID: 944
public enum UnityTag
{
	// Token: 0x0400213D RID: 8509
	Invalid = -1,
	// Token: 0x0400213E RID: 8510
	Untagged,
	// Token: 0x0400213F RID: 8511
	Respawn,
	// Token: 0x04002140 RID: 8512
	Finish,
	// Token: 0x04002141 RID: 8513
	EditorOnly,
	// Token: 0x04002142 RID: 8514
	MainCamera,
	// Token: 0x04002143 RID: 8515
	Player,
	// Token: 0x04002144 RID: 8516
	GameController,
	// Token: 0x04002145 RID: 8517
	SceneChanger,
	// Token: 0x04002146 RID: 8518
	PlayerOffset,
	// Token: 0x04002147 RID: 8519
	GorillaTagManager,
	// Token: 0x04002148 RID: 8520
	GorillaTagCollider,
	// Token: 0x04002149 RID: 8521
	GorillaPlayer,
	// Token: 0x0400214A RID: 8522
	GorillaObject,
	// Token: 0x0400214B RID: 8523
	GorillaGameManager,
	// Token: 0x0400214C RID: 8524
	GorillaCosmetic,
	// Token: 0x0400214D RID: 8525
	projectile,
	// Token: 0x0400214E RID: 8526
	FxTemporaire,
	// Token: 0x0400214F RID: 8527
	SlingshotProjectile,
	// Token: 0x04002150 RID: 8528
	SlingshotProjectileTrail,
	// Token: 0x04002151 RID: 8529
	SlingshotProjectilePlayerImpactFX,
	// Token: 0x04002152 RID: 8530
	SlingshotProjectileSurfaceImpactFX,
	// Token: 0x04002153 RID: 8531
	BalloonPopFX,
	// Token: 0x04002154 RID: 8532
	WorldShareableItem,
	// Token: 0x04002155 RID: 8533
	HornsSlingshotProjectile,
	// Token: 0x04002156 RID: 8534
	HornsSlingshotProjectileTrail,
	// Token: 0x04002157 RID: 8535
	HornsSlingshotProjectilePlayerImpactFX,
	// Token: 0x04002158 RID: 8536
	HornsSlingshotProjectileSurfaceImpactFX,
	// Token: 0x04002159 RID: 8537
	FryingPan,
	// Token: 0x0400215A RID: 8538
	LeafPileImpactFX,
	// Token: 0x0400215B RID: 8539
	BalloonPopFx,
	// Token: 0x0400215C RID: 8540
	CloudSlingshotProjectile,
	// Token: 0x0400215D RID: 8541
	CloudSlingshotProjectileTrail,
	// Token: 0x0400215E RID: 8542
	CloudSlingshotProjectilePlayerImpactFX,
	// Token: 0x0400215F RID: 8543
	CloudSlingshotProjectileSurfaceImpactFX,
	// Token: 0x04002160 RID: 8544
	SnowballProjectile,
	// Token: 0x04002161 RID: 8545
	SnowballProjectileImpactFX,
	// Token: 0x04002162 RID: 8546
	CupidBowProjectile,
	// Token: 0x04002163 RID: 8547
	CupidBowProjectileTrail,
	// Token: 0x04002164 RID: 8548
	CupidBowProjectileSurfaceImpactFX,
	// Token: 0x04002165 RID: 8549
	NoCrazyCheck,
	// Token: 0x04002166 RID: 8550
	IceSlingshotProjectile,
	// Token: 0x04002167 RID: 8551
	IceSlingshotProjectileSurfaceImpactFX,
	// Token: 0x04002168 RID: 8552
	IceSlingshotProjectileTrail,
	// Token: 0x04002169 RID: 8553
	ElfBowProjectile,
	// Token: 0x0400216A RID: 8554
	ElfBowProjectileSurfaceImpactFX,
	// Token: 0x0400216B RID: 8555
	ElfBowProjectileTrail,
	// Token: 0x0400216C RID: 8556
	RenderIfSmall,
	// Token: 0x0400216D RID: 8557
	DeleteOnNonBetaBuild,
	// Token: 0x0400216E RID: 8558
	DeleteOnNonDebugBuild,
	// Token: 0x0400216F RID: 8559
	FlagColoringCauldon,
	// Token: 0x04002170 RID: 8560
	WaterRippleEffect,
	// Token: 0x04002171 RID: 8561
	WaterSplashEffect,
	// Token: 0x04002172 RID: 8562
	FireworkMortarProjectile,
	// Token: 0x04002173 RID: 8563
	FireworkMortarProjectileImpactFX,
	// Token: 0x04002174 RID: 8564
	WaterBalloonProjectile,
	// Token: 0x04002175 RID: 8565
	WaterBalloonProjectileImpactFX,
	// Token: 0x04002176 RID: 8566
	PlayerHeadTrigger,
	// Token: 0x04002177 RID: 8567
	WizardStaff,
	// Token: 0x04002178 RID: 8568
	LurkerGhost,
	// Token: 0x04002179 RID: 8569
	HauntedObject,
	// Token: 0x0400217A RID: 8570
	WanderingGhost,
	// Token: 0x0400217B RID: 8571
	LavaSurfaceRock,
	// Token: 0x0400217C RID: 8572
	LavaRockProjectile,
	// Token: 0x0400217D RID: 8573
	LavaRockProjectileImpactFX,
	// Token: 0x0400217E RID: 8574
	MoltenSlingshotProjectile,
	// Token: 0x0400217F RID: 8575
	MoltenSlingshotProjectileTrail,
	// Token: 0x04002180 RID: 8576
	MoltenSlingshotProjectileSurfaceImpactFX,
	// Token: 0x04002181 RID: 8577
	MoltenSlingshotProjectilePlayerImpactFX,
	// Token: 0x04002182 RID: 8578
	SpiderBowProjectile,
	// Token: 0x04002183 RID: 8579
	SpiderBowProjectileTrail,
	// Token: 0x04002184 RID: 8580
	SpiderBowProjectileSurfaceImpactFX,
	// Token: 0x04002185 RID: 8581
	SpiderBowProjectilePlayerImpactFX,
	// Token: 0x04002186 RID: 8582
	ZoneRoot,
	// Token: 0x04002187 RID: 8583
	DontProcessMaterials,
	// Token: 0x04002188 RID: 8584
	OrnamentProjectileSurfaceImpactFX,
	// Token: 0x04002189 RID: 8585
	BucketGiftCane,
	// Token: 0x0400218A RID: 8586
	BucketGiftCoal,
	// Token: 0x0400218B RID: 8587
	BucketGiftRoll,
	// Token: 0x0400218C RID: 8588
	BucketGiftRound,
	// Token: 0x0400218D RID: 8589
	BucketGiftSquare,
	// Token: 0x0400218E RID: 8590
	OrnamentProjectile,
	// Token: 0x0400218F RID: 8591
	OrnamentShatterFX,
	// Token: 0x04002190 RID: 8592
	ScienceCandyProjectile,
	// Token: 0x04002191 RID: 8593
	ScienceCandyImpactFX,
	// Token: 0x04002192 RID: 8594
	PaperAirplaneProjectile,
	// Token: 0x04002193 RID: 8595
	DevilBowProjectile,
	// Token: 0x04002194 RID: 8596
	DevilBowProjectileTrail,
	// Token: 0x04002195 RID: 8597
	DevilBowProjectileSurfaceImpactFX,
	// Token: 0x04002196 RID: 8598
	DevilBowProjectilePlayerImpactFX,
	// Token: 0x04002197 RID: 8599
	FireFX,
	// Token: 0x04002198 RID: 8600
	FishFood,
	// Token: 0x04002199 RID: 8601
	FishFoodImpactFX,
	// Token: 0x0400219A RID: 8602
	LeafNinjaStarProjectile,
	// Token: 0x0400219B RID: 8603
	LeafNinjaStarProjectileC1,
	// Token: 0x0400219C RID: 8604
	LeafNinjaStarProjectileC2,
	// Token: 0x0400219D RID: 8605
	SamuraiBowProjectile,
	// Token: 0x0400219E RID: 8606
	SamuraiBowProjectileTrail,
	// Token: 0x0400219F RID: 8607
	SamuraiBowProjectileSurfaceImpactFX,
	// Token: 0x040021A0 RID: 8608
	SamuraiBowProjectilePlayerImpactFX,
	// Token: 0x040021A1 RID: 8609
	DragonSlingProjectile,
	// Token: 0x040021A2 RID: 8610
	DragonSlingProjectileTrail,
	// Token: 0x040021A3 RID: 8611
	DragonSlingProjectileSurfaceImpactFX,
	// Token: 0x040021A4 RID: 8612
	DragonSlingProjectilePlayerImpactFX,
	// Token: 0x040021A5 RID: 8613
	FireballProjectile,
	// Token: 0x040021A6 RID: 8614
	StealthHandTapFX,
	// Token: 0x040021A7 RID: 8615
	EnvPieceTree01,
	// Token: 0x040021A8 RID: 8616
	FxSnapPiecePlaced,
	// Token: 0x040021A9 RID: 8617
	FxSnapPieceDisconnected,
	// Token: 0x040021AA RID: 8618
	FxSnapPieceGrabbed,
	// Token: 0x040021AB RID: 8619
	FxSnapPieceLocationLock,
	// Token: 0x040021AC RID: 8620
	CyberNinjaStarProjectile,
	// Token: 0x040021AD RID: 8621
	RoomLight,
	// Token: 0x040021AE RID: 8622
	SamplesInfoPanel,
	// Token: 0x040021AF RID: 8623
	GorillaHandLeft,
	// Token: 0x040021B0 RID: 8624
	GorillaHandRight,
	// Token: 0x040021B1 RID: 8625
	GorillaHandSocket,
	// Token: 0x040021B2 RID: 8626
	PlayingCardProjectile,
	// Token: 0x040021B3 RID: 8627
	RottenPumpkinProjectile,
	// Token: 0x040021B4 RID: 8628
	FxSnapPieceRecycle,
	// Token: 0x040021B5 RID: 8629
	FxSnapPieceDispenser,
	// Token: 0x040021B6 RID: 8630
	AppleProjectile,
	// Token: 0x040021B7 RID: 8631
	AppleProjectileSurfaceImpactFX,
	// Token: 0x040021B8 RID: 8632
	RecyclerForceVolumeFX,
	// Token: 0x040021B9 RID: 8633
	FxSnapPieceTooHeavy,
	// Token: 0x040021BA RID: 8634
	FxBuilderPrivatePlotClaimed,
	// Token: 0x040021BB RID: 8635
	TrickTreatCandy,
	// Token: 0x040021BC RID: 8636
	TrickTreatEyeball,
	// Token: 0x040021BD RID: 8637
	TrickTreatBat,
	// Token: 0x040021BE RID: 8638
	TrickTreatBomb,
	// Token: 0x040021BF RID: 8639
	TrickTreatSurfaceImpact,
	// Token: 0x040021C0 RID: 8640
	TrickTreatBatImpact,
	// Token: 0x040021C1 RID: 8641
	TrickTreatBombImpact,
	// Token: 0x040021C2 RID: 8642
	GuardianSlapFX,
	// Token: 0x040021C3 RID: 8643
	GuardianSlamFX,
	// Token: 0x040021C4 RID: 8644
	GuardianIdolLandedFX,
	// Token: 0x040021C5 RID: 8645
	GuardianIdolFallFX,
	// Token: 0x040021C6 RID: 8646
	GuardianIdolTappedFX,
	// Token: 0x040021C7 RID: 8647
	VotingRockProjectile,
	// Token: 0x040021C8 RID: 8648
	LeafPileImpactFXMedium,
	// Token: 0x040021C9 RID: 8649
	LeafPileImpactFXSmall,
	// Token: 0x040021CA RID: 8650
	WoodenSword,
	// Token: 0x040021CB RID: 8651
	WoodenShield,
	// Token: 0x040021CC RID: 8652
	FxBuilderShrink,
	// Token: 0x040021CD RID: 8653
	FxBuilderGrow,
	// Token: 0x040021CE RID: 8654
	FxSnapPieceWreathJump,
	// Token: 0x040021CF RID: 8655
	ElfLauncherElf,
	// Token: 0x040021D0 RID: 8656
	RubberBandCar,
	// Token: 0x040021D1 RID: 8657
	SnowPileImpactFX,
	// Token: 0x040021D2 RID: 8658
	FirecrackersProjectile,
	// Token: 0x040021D3 RID: 8659
	PaperAirplaneSquareProjectile,
	// Token: 0x040021D4 RID: 8660
	SmokeBombProjectile,
	// Token: 0x040021D5 RID: 8661
	ThrowableHeartProjectile,
	// Token: 0x040021D6 RID: 8662
	SunFlowers,
	// Token: 0x040021D7 RID: 8663
	RobotCannonProjectile,
	// Token: 0x040021D8 RID: 8664
	RobotCannonProjectileImpact,
	// Token: 0x040021D9 RID: 8665
	SmokeBombExplosionEffect,
	// Token: 0x040021DA RID: 8666
	FireCrackerExplosionEffect,
	// Token: 0x040021DB RID: 8667
	GorillaMouth
}
