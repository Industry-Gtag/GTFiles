using System;

// Token: 0x02000121 RID: 289
public enum SIUpgradeType
{
	// Token: 0x04000924 RID: 2340
	InvalidNode = -2,
	// Token: 0x04000925 RID: 2341
	Initialize,
	// Token: 0x04000926 RID: 2342
	Thruster_Unlock,
	// Token: 0x04000927 RID: 2343
	Thruster_Jet,
	// Token: 0x04000928 RID: 2344
	Thruster_Prop,
	// Token: 0x04000929 RID: 2345
	Thruster_Jet_Duration,
	// Token: 0x0400092A RID: 2346
	Thruster_Jet_Accel,
	// Token: 0x0400092B RID: 2347
	Thruster_Prop_Duration,
	// Token: 0x0400092C RID: 2348
	Thruster_Prop_Speed,
	// Token: 0x0400092D RID: 2349
	Thruster_Jet_Tag,
	// Token: 0x0400092E RID: 2350
	Thruster_Prop_Knockback,
	// Token: 0x0400092F RID: 2351
	Thruster_Fuel_Grounding,
	// Token: 0x04000930 RID: 2352
	Thruster_Throttle_Control,
	// Token: 0x04000931 RID: 2353
	Stilt_Unlock = 100,
	// Token: 0x04000932 RID: 2354
	Stilt_Tag_Tip,
	// Token: 0x04000933 RID: 2355
	Stilt_Retractable,
	// Token: 0x04000934 RID: 2356
	Stilt_Adjustable_Length,
	// Token: 0x04000935 RID: 2357
	Stilt_Retract_Speed,
	// Token: 0x04000936 RID: 2358
	Stilt_Max_Length,
	// Token: 0x04000937 RID: 2359
	Stilt_Stun_Tip,
	// Token: 0x04000938 RID: 2360
	Stilt_Muscle_Fusion,
	// Token: 0x04000939 RID: 2361
	Stilt_Short,
	// Token: 0x0400093A RID: 2362
	Stilt_Long,
	// Token: 0x0400093B RID: 2363
	Stilt_Motorized,
	// Token: 0x0400093C RID: 2364
	Stilt_Motorized_Triple,
	// Token: 0x0400093D RID: 2365
	Stilt_Turkey_Coma,
	// Token: 0x0400093E RID: 2366
	Grenade_Concussion_Unlock = 200,
	// Token: 0x0400093F RID: 2367
	Grenade_Antigravity_Unlock,
	// Token: 0x04000940 RID: 2368
	Grenade_Concussion_Stun,
	// Token: 0x04000941 RID: 2369
	Grenade_Concussion_Radius,
	// Token: 0x04000942 RID: 2370
	Grenade_Antigravity_Persists,
	// Token: 0x04000943 RID: 2371
	Grenade_Antigravity_Cooldown,
	// Token: 0x04000944 RID: 2372
	Grenade_Concussion_Self_Boost,
	// Token: 0x04000945 RID: 2373
	Grenade_Concussion_Overcharge,
	// Token: 0x04000946 RID: 2374
	Grenade_Antigravity_Pro_Gravity,
	// Token: 0x04000947 RID: 2375
	Grenade_Concussion_Impact_Accelerant,
	// Token: 0x04000948 RID: 2376
	Grenade_Antigravity_Gravity_Bomb,
	// Token: 0x04000949 RID: 2377
	Grenade_Antigravity_Black_Hole,
	// Token: 0x0400094A RID: 2378
	Grenade_Holster_Unlock,
	// Token: 0x0400094B RID: 2379
	Grenade_Stun_Unlock,
	// Token: 0x0400094C RID: 2380
	Grenade_Puller_Unlock,
	// Token: 0x0400094D RID: 2381
	Grenade_Disrupter_Unlock,
	// Token: 0x0400094E RID: 2382
	Dash_Yoyo_Unlock = 301,
	// Token: 0x0400094F RID: 2383
	Dash_Yoyo_Range = 304,
	// Token: 0x04000950 RID: 2384
	Dash_Yoyo_Speed,
	// Token: 0x04000951 RID: 2385
	Dash_Unused_306,
	// Token: 0x04000952 RID: 2386
	Dash_Unused_307,
	// Token: 0x04000953 RID: 2387
	Dash_Yoyo_Cooldown,
	// Token: 0x04000954 RID: 2388
	Dash_Yoyo_Dynamic,
	// Token: 0x04000955 RID: 2389
	Dash_Unused_310,
	// Token: 0x04000956 RID: 2390
	Dash_Yoyo_Stun,
	// Token: 0x04000957 RID: 2391
	Dash_Yoyo_Tag,
	// Token: 0x04000958 RID: 2392
	Dash_Unused_313,
	// Token: 0x04000959 RID: 2393
	Dash_Unused_314,
	// Token: 0x0400095A RID: 2394
	Platform_Unlock = 400,
	// Token: 0x0400095B RID: 2395
	Platform_Cooldown,
	// Token: 0x0400095C RID: 2396
	Platform_Duration,
	// Token: 0x0400095D RID: 2397
	Platform_Capacity,
	// Token: 0x0400095E RID: 2398
	Platform_SpeedBoost,
	// Token: 0x0400095F RID: 2399
	Tapteleport_Unlock = 500,
	// Token: 0x04000960 RID: 2400
	Tapteleport_Zone,
	// Token: 0x04000961 RID: 2401
	Tapteleport_Stealth,
	// Token: 0x04000962 RID: 2402
	Tapteleport_Portal_Selection,
	// Token: 0x04000963 RID: 2403
	Tapteleport_Keep_Velocity,
	// Token: 0x04000964 RID: 2404
	Tapteleport_Infinite_Use,
	// Token: 0x04000965 RID: 2405
	Tentacle_Unlock = 600,
	// Token: 0x04000966 RID: 2406
	Tentacle_Power_Claw,
	// Token: 0x04000967 RID: 2407
	Tentacle_Charge_Rate,
	// Token: 0x04000968 RID: 2408
	Tentacle_Efficiency,
	// Token: 0x04000969 RID: 2409
	Tentacle_Crawler,
	// Token: 0x0400096A RID: 2410
	Tentacle_Strider,
	// Token: 0x0400096B RID: 2411
	AirControl_AirJuke_Unlock = 700,
	// Token: 0x0400096C RID: 2412
	AirControl_AirJuke_Speed,
	// Token: 0x0400096D RID: 2413
	AirControl_AirGrab_Unlock,
	// Token: 0x0400096E RID: 2414
	AirControl_AirGrab_Speed,
	// Token: 0x0400096F RID: 2415
	AirControl_AirGrab_HoldTime,
	// Token: 0x04000970 RID: 2416
	AirControl_Zipline_Unlock,
	// Token: 0x04000971 RID: 2417
	AirControl_Zipline_Speed,
	// Token: 0x04000972 RID: 2418
	Prototype_SlipMitt = 800,
	// Token: 0x04000973 RID: 2419
	Prototype_Wing,
	// Token: 0x04000974 RID: 2420
	Prototype_802,
	// Token: 0x04000975 RID: 2421
	Prototype_803,
	// Token: 0x04000976 RID: 2422
	Prototype_804,
	// Token: 0x04000977 RID: 2423
	Prototype_805,
	// Token: 0x04000978 RID: 2424
	Blaster_Standard_Unlock = 1000,
	// Token: 0x04000979 RID: 2425
	Blaster_Charge_Unlock,
	// Token: 0x0400097A RID: 2426
	Blaster_Lobber_Unlock,
	// Token: 0x0400097B RID: 2427
	Blaster_PumpDart_Unlock,
	// Token: 0x0400097C RID: 2428
	Blaster_MegaCharge_Unlock,
	// Token: 0x0400097D RID: 2429
	Blaster_LongBlaster_Unlock
}
