using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200007F RID: 127
public class CritterTemplate : ScriptableObject
{
	// Token: 0x1700003A RID: 58
	// (get) Token: 0x06000318 RID: 792 RVA: 0x00012240 File Offset: 0x00010440
	private string HapticsBlurb
	{
		get
		{
			float num = this.grabbedStruggleHaptics.GetPeakMagnitude() * this.grabbedStruggleHapticsStrength;
			float num2 = this.grabbedStruggleHaptics.GetRMSMagnitude() * this.grabbedStruggleHapticsStrength;
			return string.Format("Peak Strength: {0:0.##} Mean Strength: {1:0.##}", num, num2);
		}
	}

	// Token: 0x06000319 RID: 793 RVA: 0x0001228C File Offset: 0x0001048C
	private void SetMaxStrength(float maxStrength = 1f)
	{
		float peakMagnitude = this.grabbedStruggleHaptics.GetPeakMagnitude();
		Debug.Log(string.Format("Clip {0} max strength: {1}", this.grabbedStruggleHaptics, peakMagnitude));
		if (peakMagnitude > 0f)
		{
			this.grabbedStruggleHapticsStrength = maxStrength / peakMagnitude;
		}
	}

	// Token: 0x0600031A RID: 794 RVA: 0x000122D4 File Offset: 0x000104D4
	private void SetMeanStrength(float meanStrength = 1f)
	{
		float rmsmagnitude = this.grabbedStruggleHaptics.GetRMSMagnitude();
		Debug.Log(string.Format("Clip {0} mean strength: {1}", this.grabbedStruggleHaptics, rmsmagnitude));
		if (meanStrength > 0f)
		{
			this.grabbedStruggleHapticsStrength = meanStrength / rmsmagnitude;
		}
	}

	// Token: 0x0600031B RID: 795 RVA: 0x00012319 File Offset: 0x00010519
	private void OnValidate()
	{
		this.modifiedValues.Clear();
		this.RegisterModifiedBehaviour();
		this.RegisterModifiedVisual();
	}

	// Token: 0x0600031C RID: 796 RVA: 0x00012332 File Offset: 0x00010532
	private void OnEnable()
	{
		this.OnValidate();
	}

	// Token: 0x0600031D RID: 797 RVA: 0x0001233C File Offset: 0x0001053C
	private void RegisterModifiedBehaviour()
	{
		if (this.maxJumpVel != 0f)
		{
			this.modifiedValues.Add("maxJumpVel", this.maxJumpVel);
		}
		if (this.jumpCooldown != 0f)
		{
			this.modifiedValues.Add("jumpCooldown", this.jumpCooldown);
		}
		if (this.scaredJumpCooldown != 0f)
		{
			this.modifiedValues.Add("scaredJumpCooldown", this.scaredJumpCooldown);
		}
		if (this.jumpVariabilityTime != 0f)
		{
			this.modifiedValues.Add("jumpVariabilityTime", this.jumpVariabilityTime);
		}
		if (this.visionConeAngle != 0f)
		{
			this.modifiedValues.Add("visionConeAngle", this.visionConeAngle);
		}
		if (this.sensoryRange != 0f)
		{
			this.modifiedValues.Add("sensoryRange", this.sensoryRange);
		}
		if (this.maxHunger != 0f)
		{
			this.modifiedValues.Add("maxHunger", this.maxHunger);
		}
		if (this.hungryThreshold != 0f)
		{
			this.modifiedValues.Add("hungryThreshold", this.hungryThreshold);
		}
		if (this.satiatedThreshold != 0f)
		{
			this.modifiedValues.Add("satiatedThreshold", this.satiatedThreshold);
		}
		if (this.hungerLostPerSecond != 0f)
		{
			this.modifiedValues.Add("hungerLostPerSecond", this.hungerLostPerSecond);
		}
		if (this.hungerGainedPerSecond != 0f)
		{
			this.modifiedValues.Add("hungerGainedPerSecond", this.hungerGainedPerSecond);
		}
		if (this.maxFear != 0f)
		{
			this.modifiedValues.Add("maxFear", this.maxFear);
		}
		if (this.scaredThreshold != 0f)
		{
			this.modifiedValues.Add("scaredThreshold", this.scaredThreshold);
		}
		if (this.calmThreshold != 0f)
		{
			this.modifiedValues.Add("calmThreshold", this.calmThreshold);
		}
		if (this.fearLostPerSecond != 0f)
		{
			this.modifiedValues.Add("fearLostPerSecond", this.fearLostPerSecond);
		}
		if (this.maxAttraction != 0f)
		{
			this.modifiedValues.Add("maxAttraction", this.maxAttraction);
		}
		if (this.attractedThreshold != 0f)
		{
			this.modifiedValues.Add("attractedThreshold", this.attractedThreshold);
		}
		if (this.unattractedThreshold != 0f)
		{
			this.modifiedValues.Add("unattractedThreshold", this.unattractedThreshold);
		}
		if (this.attractionLostPerSecond != 0f)
		{
			this.modifiedValues.Add("attractionLostPerSecond", this.attractionLostPerSecond);
		}
		if (this.maxSleepiness != 0f)
		{
			this.modifiedValues.Add("maxSleepiness", this.maxSleepiness);
		}
		if (this.tiredThreshold != 0f)
		{
			this.modifiedValues.Add("tiredThreshold", this.tiredThreshold);
		}
		if (this.awakeThreshold != 0f)
		{
			this.modifiedValues.Add("awakeThreshold", this.awakeThreshold);
		}
		if (this.sleepinessGainedPerSecond != 0f)
		{
			this.modifiedValues.Add("sleepinessGainedPerSecond", this.sleepinessGainedPerSecond);
		}
		if (this.sleepinessLostPerSecond != 0f)
		{
			this.modifiedValues.Add("sleepinessLostPerSecond", this.sleepinessLostPerSecond);
		}
		if (this.maxStruggle != 0f)
		{
			this.modifiedValues.Add("maxStruggle", this.maxStruggle);
		}
		if (this.escapeThreshold != 0f)
		{
			this.modifiedValues.Add("escapeThreshold", this.escapeThreshold);
		}
		if (this.catchableThreshold != 0f)
		{
			this.modifiedValues.Add("catchableThreshold", this.catchableThreshold);
		}
		if (this.struggleGainedPerSecond != 0f)
		{
			this.modifiedValues.Add("struggleGainedPerSecond", this.struggleGainedPerSecond);
		}
		if (this.struggleLostPerSecond != 0f)
		{
			this.modifiedValues.Add("struggleLostPerSecond", this.struggleLostPerSecond);
		}
		if (this.afraidOfList != null)
		{
			this.modifiedValues.Add("afraidOfList", this.afraidOfList);
		}
		if (this.attractedToList != null)
		{
			this.modifiedValues.Add("attractedToList", this.attractedToList);
		}
		if (this.lifeTime != 0f)
		{
			this.modifiedValues.Add("lifeTime", this.lifeTime);
		}
	}

	// Token: 0x0600031E RID: 798 RVA: 0x00012838 File Offset: 0x00010A38
	private void RegisterModifiedVisual()
	{
		if (this.hatChance != 0f)
		{
			this.modifiedValues.Add("hatChance", this.hatChance);
		}
		if (this.hats != null && this.hats.Length != 0)
		{
			this.modifiedValues.Add("hats", this.hats);
		}
		if (this.minSize != 0f)
		{
			this.modifiedValues.Add("minSize", this.minSize);
		}
		if (this.maxSize != 0f)
		{
			this.modifiedValues.Add("maxSize", this.maxSize);
		}
		if (this.eatingStartFX != null)
		{
			this.modifiedValues.Add("eatingStartFX", this.eatingStartFX);
		}
		if (this.eatingOngoingFX != null)
		{
			this.modifiedValues.Add("eatingOngoingFX", this.eatingOngoingFX);
		}
		if (CrittersAnim.IsModified(this.eatingAnim))
		{
			this.modifiedValues.Add("eatingAnim", this.eatingAnim);
		}
		if (this.fearStartFX != null)
		{
			this.modifiedValues.Add("fearStartFX", this.fearStartFX);
		}
		if (this.fearOngoingFX != null)
		{
			this.modifiedValues.Add("fearOngoingFX", this.fearOngoingFX);
		}
		if (CrittersAnim.IsModified(this.fearAnim))
		{
			this.modifiedValues.Add("fearAnim", this.fearAnim);
		}
		if (this.attractionStartFX != null)
		{
			this.modifiedValues.Add("attractionStartFX", this.attractionStartFX);
		}
		if (this.attractionOngoingFX != null)
		{
			this.modifiedValues.Add("attractionOngoingFX", this.attractionOngoingFX);
		}
		if (CrittersAnim.IsModified(this.attractionAnim))
		{
			this.modifiedValues.Add("attractionAnim", this.attractionAnim);
		}
		if (this.sleepStartFX != null)
		{
			this.modifiedValues.Add("sleepStartFX", this.sleepStartFX);
		}
		if (this.sleepOngoingFX != null)
		{
			this.modifiedValues.Add("sleepOngoingFX", this.sleepOngoingFX);
		}
		if (CrittersAnim.IsModified(this.sleepAnim))
		{
			this.modifiedValues.Add("sleepAnim", this.sleepAnim);
		}
		if (this.grabbedStartFX != null)
		{
			this.modifiedValues.Add("grabbedStartFX", this.grabbedStartFX);
		}
		if (this.grabbedOngoingFX != null)
		{
			this.modifiedValues.Add("grabbedOngoingFX", this.grabbedOngoingFX);
		}
		if (this.grabbedStopFX != null)
		{
			this.modifiedValues.Add("grabbedStopFX", this.grabbedStopFX);
		}
		if (CrittersAnim.IsModified(this.grabbedAnim))
		{
			this.modifiedValues.Add("grabbedAnim", this.grabbedAnim);
		}
		if (this.hungryStartFX != null)
		{
			this.modifiedValues.Add("hungryStartFX", this.hungryStartFX);
		}
		if (this.hungryOngoingFX != null)
		{
			this.modifiedValues.Add("hungryOngoingFX", this.hungryOngoingFX);
		}
		if (CrittersAnim.IsModified(this.hungryAnim))
		{
			this.modifiedValues.Add("hungryAnim", this.hungryAnim);
		}
		if (this.despawningStartFX != null)
		{
			this.modifiedValues.Add("despawningStartFX", this.despawningStartFX);
		}
		if (this.despawningOngoingFX != null)
		{
			this.modifiedValues.Add("despawningOngoingFX", this.despawningOngoingFX);
		}
		if (CrittersAnim.IsModified(this.despawningAnim))
		{
			this.modifiedValues.Add("despawningAnim", this.despawningAnim);
		}
		if (this.spawningStartFX != null)
		{
			this.modifiedValues.Add("spawningStartFX", this.spawningStartFX);
		}
		if (this.spawningOngoingFX != null)
		{
			this.modifiedValues.Add("spawningOngoingFX", this.spawningOngoingFX);
		}
		if (CrittersAnim.IsModified(this.spawningAnim))
		{
			this.modifiedValues.Add("spawningAnim", this.spawningAnim);
		}
		if (this.capturedStartFX != null)
		{
			this.modifiedValues.Add("capturedStartFX", this.capturedStartFX);
		}
		if (this.capturedOngoingFX != null)
		{
			this.modifiedValues.Add("capturedOngoingFX", this.capturedOngoingFX);
		}
		if (CrittersAnim.IsModified(this.capturedAnim))
		{
			this.modifiedValues.Add("capturedAnim", this.capturedAnim);
		}
		if (this.stunnedStartFX != null)
		{
			this.modifiedValues.Add("stunnedStartFX", this.stunnedStartFX);
		}
		if (this.stunnedOngoingFX != null)
		{
			this.modifiedValues.Add("stunnedOngoingFX", this.stunnedOngoingFX);
		}
		if (CrittersAnim.IsModified(this.stunnedAnim))
		{
			this.modifiedValues.Add("stunnedAnim", this.stunnedAnim);
		}
		if (this.grabbedStruggleHaptics != null)
		{
			this.modifiedValues.Add("grabbedStruggleHaptics", this.grabbedStruggleHaptics);
		}
		if (this.grabbedStruggleHapticsStrength != 0f)
		{
			this.modifiedValues.Add("grabbedStruggleHapticsStrength", this.grabbedStruggleHapticsStrength);
		}
	}

	// Token: 0x0600031F RID: 799 RVA: 0x00012D82 File Offset: 0x00010F82
	public bool IsValueModified(string valueName)
	{
		return this.modifiedValues.ContainsKey(valueName);
	}

	// Token: 0x06000320 RID: 800 RVA: 0x00012D90 File Offset: 0x00010F90
	public T GetParentValue<T>(string valueName)
	{
		if (this.parent != null)
		{
			return this.parent.GetTemplateValue<T>(valueName);
		}
		return default(T);
	}

	// Token: 0x06000321 RID: 801 RVA: 0x00012DC4 File Offset: 0x00010FC4
	public T GetTemplateValue<T>(string valueName)
	{
		object obj;
		if (this.modifiedValues.TryGetValue(valueName, out obj))
		{
			return (T)((object)obj);
		}
		if (this.parent != null)
		{
			return this.parent.GetTemplateValue<T>(valueName);
		}
		return default(T);
	}

	// Token: 0x06000322 RID: 802 RVA: 0x00012E0C File Offset: 0x0001100C
	public void ApplyToCritter(CrittersPawn critter)
	{
		this.ApplyBehaviour(critter);
		this.ApplyBehaviourFX(critter);
	}

	// Token: 0x06000323 RID: 803 RVA: 0x00012E1C File Offset: 0x0001101C
	private void ApplyBehaviour(CrittersPawn critter)
	{
		critter.maxJumpVel = this.GetTemplateValue<float>("maxJumpVel");
		critter.jumpCooldown = this.GetTemplateValue<float>("jumpCooldown");
		critter.scaredJumpCooldown = this.GetTemplateValue<float>("scaredJumpCooldown");
		critter.jumpVariabilityTime = this.GetTemplateValue<float>("jumpVariabilityTime");
		critter.visionConeAngle = this.GetTemplateValue<float>("visionConeAngle");
		critter.sensoryRange = this.GetTemplateValue<float>("sensoryRange");
		critter.maxHunger = this.GetTemplateValue<float>("maxHunger");
		critter.hungryThreshold = this.GetTemplateValue<float>("hungryThreshold");
		critter.satiatedThreshold = this.GetTemplateValue<float>("satiatedThreshold");
		critter.hungerLostPerSecond = this.GetTemplateValue<float>("hungerLostPerSecond");
		critter.hungerGainedPerSecond = this.GetTemplateValue<float>("hungerGainedPerSecond");
		critter.maxFear = this.GetTemplateValue<float>("maxFear");
		critter.scaredThreshold = this.GetTemplateValue<float>("scaredThreshold");
		critter.calmThreshold = this.GetTemplateValue<float>("calmThreshold");
		critter.fearLostPerSecond = this.GetTemplateValue<float>("fearLostPerSecond");
		critter.maxAttraction = this.GetTemplateValue<float>("maxAttraction");
		critter.attractedThreshold = this.GetTemplateValue<float>("attractedThreshold");
		critter.unattractedThreshold = this.GetTemplateValue<float>("unattractedThreshold");
		critter.attractionLostPerSecond = this.GetTemplateValue<float>("attractionLostPerSecond");
		critter.maxSleepiness = this.GetTemplateValue<float>("maxSleepiness");
		critter.tiredThreshold = this.GetTemplateValue<float>("tiredThreshold");
		critter.awakeThreshold = this.GetTemplateValue<float>("awakeThreshold");
		critter.sleepinessGainedPerSecond = this.GetTemplateValue<float>("sleepinessGainedPerSecond");
		critter.sleepinessLostPerSecond = this.GetTemplateValue<float>("sleepinessLostPerSecond");
		critter.maxStruggle = this.GetTemplateValue<float>("maxStruggle");
		critter.escapeThreshold = this.GetTemplateValue<float>("escapeThreshold");
		critter.catchableThreshold = this.GetTemplateValue<float>("catchableThreshold");
		critter.struggleGainedPerSecond = this.GetTemplateValue<float>("struggleGainedPerSecond");
		critter.struggleLostPerSecond = this.GetTemplateValue<float>("struggleLostPerSecond");
		critter.lifeTime = (double)this.GetTemplateValue<float>("lifeTime");
		critter.attractedToList = this.GetTemplateValue<List<crittersAttractorStruct>>("attractedToList");
		critter.afraidOfList = this.GetTemplateValue<List<crittersAttractorStruct>>("afraidOfList");
	}

	// Token: 0x06000324 RID: 804 RVA: 0x0001304C File Offset: 0x0001124C
	private void ApplyBehaviourFX(CrittersPawn critter)
	{
		critter.StartStateFX.Clear();
		critter.OngoingStateFX.Clear();
		critter.stateAnim.Clear();
		critter.StartStateFX.Add(CrittersPawn.CreatureState.Eating, this.GetTemplateValue<GameObject>("eatingStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.Eating, this.GetTemplateValue<GameObject>("eatingOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.Eating, this.GetTemplateValue<CrittersAnim>("eatingAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.Running, this.GetTemplateValue<GameObject>("fearStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.Running, this.GetTemplateValue<GameObject>("fearOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.Running, this.GetTemplateValue<CrittersAnim>("fearAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.AttractedTo, this.GetTemplateValue<GameObject>("attractionStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.AttractedTo, this.GetTemplateValue<GameObject>("attractionOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.AttractedTo, this.GetTemplateValue<CrittersAnim>("attractionAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.Sleeping, this.GetTemplateValue<GameObject>("sleepStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.Sleeping, this.GetTemplateValue<GameObject>("sleepOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.Sleeping, this.GetTemplateValue<CrittersAnim>("sleepAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.Grabbed, this.GetTemplateValue<GameObject>("grabbedStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.Grabbed, this.GetTemplateValue<GameObject>("grabbedOngoingFX"));
		critter.OnReleasedFX = this.GetTemplateValue<GameObject>("grabbedStopFX");
		critter.stateAnim.Add(CrittersPawn.CreatureState.Grabbed, this.GetTemplateValue<CrittersAnim>("grabbedAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.SeekingFood, this.GetTemplateValue<GameObject>("hungryStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.SeekingFood, this.GetTemplateValue<GameObject>("hungryOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.SeekingFood, this.GetTemplateValue<CrittersAnim>("hungryAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.Despawning, this.GetTemplateValue<GameObject>("despawningStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.Despawning, this.GetTemplateValue<GameObject>("despawningOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.Despawning, this.GetTemplateValue<CrittersAnim>("despawningAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.Spawning, this.GetTemplateValue<GameObject>("spawningStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.Spawning, this.GetTemplateValue<GameObject>("spawningOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.Spawning, this.GetTemplateValue<CrittersAnim>("spawningAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.Captured, this.GetTemplateValue<GameObject>("capturedStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.Captured, this.GetTemplateValue<GameObject>("capturedOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.Captured, this.GetTemplateValue<CrittersAnim>("capturedAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.Stunned, this.GetTemplateValue<GameObject>("stunnedStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.Stunned, this.GetTemplateValue<GameObject>("stunnedOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.Stunned, this.GetTemplateValue<CrittersAnim>("stunnedAnim"));
		critter.grabbedHaptics = this.GetTemplateValue<AudioClip>("grabbedStruggleHaptics");
		critter.grabbedHapticsStrength = this.GetTemplateValue<float>("grabbedStruggleHapticsStrength");
	}

	// Token: 0x0400037A RID: 890
	public CritterTemplate parent;

	// Token: 0x0400037B RID: 891
	[Space]
	[Header("Description")]
	public string temperament = "UNKNOWN";

	// Token: 0x0400037C RID: 892
	[Space]
	[Header("Behaviour")]
	[CritterTemplateParameter]
	public float maxJumpVel;

	// Token: 0x0400037D RID: 893
	[CritterTemplateParameter]
	public float jumpCooldown;

	// Token: 0x0400037E RID: 894
	[CritterTemplateParameter]
	public float scaredJumpCooldown;

	// Token: 0x0400037F RID: 895
	[CritterTemplateParameter]
	public float jumpVariabilityTime;

	// Token: 0x04000380 RID: 896
	[Space]
	[CritterTemplateParameter]
	public float visionConeAngle;

	// Token: 0x04000381 RID: 897
	[FormerlySerializedAs("visionConeHeight")]
	[CritterTemplateParameter]
	public float sensoryRange;

	// Token: 0x04000382 RID: 898
	[Space]
	[CritterTemplateParameter]
	public float maxHunger;

	// Token: 0x04000383 RID: 899
	[CritterTemplateParameter]
	public float hungryThreshold;

	// Token: 0x04000384 RID: 900
	[CritterTemplateParameter]
	public float satiatedThreshold;

	// Token: 0x04000385 RID: 901
	[CritterTemplateParameter]
	public float hungerLostPerSecond;

	// Token: 0x04000386 RID: 902
	[CritterTemplateParameter]
	public float hungerGainedPerSecond;

	// Token: 0x04000387 RID: 903
	[Space]
	[CritterTemplateParameter]
	public float maxFear;

	// Token: 0x04000388 RID: 904
	[CritterTemplateParameter]
	public float scaredThreshold;

	// Token: 0x04000389 RID: 905
	[CritterTemplateParameter]
	public float calmThreshold;

	// Token: 0x0400038A RID: 906
	[CritterTemplateParameter]
	public float fearLostPerSecond;

	// Token: 0x0400038B RID: 907
	[Space]
	[CritterTemplateParameter]
	public float maxAttraction;

	// Token: 0x0400038C RID: 908
	[CritterTemplateParameter]
	public float attractedThreshold;

	// Token: 0x0400038D RID: 909
	[CritterTemplateParameter]
	public float unattractedThreshold;

	// Token: 0x0400038E RID: 910
	[CritterTemplateParameter]
	public float attractionLostPerSecond;

	// Token: 0x0400038F RID: 911
	[Space]
	[CritterTemplateParameter]
	public float maxSleepiness;

	// Token: 0x04000390 RID: 912
	[CritterTemplateParameter]
	public float tiredThreshold;

	// Token: 0x04000391 RID: 913
	[CritterTemplateParameter]
	public float awakeThreshold;

	// Token: 0x04000392 RID: 914
	[CritterTemplateParameter]
	public float sleepinessGainedPerSecond;

	// Token: 0x04000393 RID: 915
	[CritterTemplateParameter]
	public float sleepinessLostPerSecond;

	// Token: 0x04000394 RID: 916
	[Space]
	[CritterTemplateParameter]
	public float struggleGainedPerSecond;

	// Token: 0x04000395 RID: 917
	[CritterTemplateParameter]
	public float maxStruggle;

	// Token: 0x04000396 RID: 918
	[CritterTemplateParameter]
	public float escapeThreshold;

	// Token: 0x04000397 RID: 919
	[CritterTemplateParameter]
	public float catchableThreshold;

	// Token: 0x04000398 RID: 920
	[CritterTemplateParameter]
	public float struggleLostPerSecond;

	// Token: 0x04000399 RID: 921
	[Space]
	[CritterTemplateParameter]
	public float lifeTime;

	// Token: 0x0400039A RID: 922
	[Space]
	public List<crittersAttractorStruct> attractedToList;

	// Token: 0x0400039B RID: 923
	public List<crittersAttractorStruct> afraidOfList;

	// Token: 0x0400039C RID: 924
	[Space]
	[Header("Visual")]
	[CritterTemplateParameter]
	public float minSize;

	// Token: 0x0400039D RID: 925
	[CritterTemplateParameter]
	public float maxSize;

	// Token: 0x0400039E RID: 926
	[CritterTemplateParameter]
	public float hatChance;

	// Token: 0x0400039F RID: 927
	public GameObject[] hats;

	// Token: 0x040003A0 RID: 928
	[Space]
	[Header("Behaviour FX")]
	public GameObject eatingStartFX;

	// Token: 0x040003A1 RID: 929
	public GameObject eatingOngoingFX;

	// Token: 0x040003A2 RID: 930
	public CrittersAnim eatingAnim;

	// Token: 0x040003A3 RID: 931
	public GameObject fearStartFX;

	// Token: 0x040003A4 RID: 932
	public GameObject fearOngoingFX;

	// Token: 0x040003A5 RID: 933
	public CrittersAnim fearAnim;

	// Token: 0x040003A6 RID: 934
	public GameObject attractionStartFX;

	// Token: 0x040003A7 RID: 935
	public GameObject attractionOngoingFX;

	// Token: 0x040003A8 RID: 936
	public CrittersAnim attractionAnim;

	// Token: 0x040003A9 RID: 937
	public GameObject sleepStartFX;

	// Token: 0x040003AA RID: 938
	public GameObject sleepOngoingFX;

	// Token: 0x040003AB RID: 939
	public CrittersAnim sleepAnim;

	// Token: 0x040003AC RID: 940
	public GameObject grabbedStartFX;

	// Token: 0x040003AD RID: 941
	public GameObject grabbedOngoingFX;

	// Token: 0x040003AE RID: 942
	public GameObject grabbedStopFX;

	// Token: 0x040003AF RID: 943
	public CrittersAnim grabbedAnim;

	// Token: 0x040003B0 RID: 944
	public GameObject hungryStartFX;

	// Token: 0x040003B1 RID: 945
	public GameObject hungryOngoingFX;

	// Token: 0x040003B2 RID: 946
	public CrittersAnim hungryAnim;

	// Token: 0x040003B3 RID: 947
	public GameObject spawningStartFX;

	// Token: 0x040003B4 RID: 948
	public GameObject spawningOngoingFX;

	// Token: 0x040003B5 RID: 949
	public CrittersAnim spawningAnim;

	// Token: 0x040003B6 RID: 950
	public GameObject despawningStartFX;

	// Token: 0x040003B7 RID: 951
	public GameObject despawningOngoingFX;

	// Token: 0x040003B8 RID: 952
	public CrittersAnim despawningAnim;

	// Token: 0x040003B9 RID: 953
	public GameObject capturedStartFX;

	// Token: 0x040003BA RID: 954
	public GameObject capturedOngoingFX;

	// Token: 0x040003BB RID: 955
	public CrittersAnim capturedAnim;

	// Token: 0x040003BC RID: 956
	public GameObject stunnedStartFX;

	// Token: 0x040003BD RID: 957
	public GameObject stunnedOngoingFX;

	// Token: 0x040003BE RID: 958
	public CrittersAnim stunnedAnim;

	// Token: 0x040003BF RID: 959
	public AudioClip grabbedStruggleHaptics;

	// Token: 0x040003C0 RID: 960
	public float grabbedStruggleHapticsStrength;

	// Token: 0x040003C1 RID: 961
	private Dictionary<string, object> modifiedValues = new Dictionary<string, object>();
}
