using System;
using UnityEngine;

// Token: 0x020000BE RID: 190
public class MarkOneMitts : HandTapBehaviour, ITickSystemTick, IProximityEffectReceiver
{
	// Token: 0x060004AB RID: 1195 RVA: 0x0001A240 File Offset: 0x00018440
	private void Awake()
	{
		this.leftMitt.Init();
		this.rightMitt.Init();
		this.rig = base.GetComponentInParent<VRRig>();
		this.vibrateController = this.vibrateController && this.rig.isOfflineVRRig;
		this.proximityEffect.AddReceiver(this);
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x0001A297 File Offset: 0x00018497
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x0001A29F File Offset: 0x0001849F
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x0001A2A8 File Offset: 0x000184A8
	public void OnProximityCalculated(float distance, float alignment, float parallel)
	{
		float num = distance * alignment * parallel;
		if (num > 0.1f)
		{
			float num2 = this.proximitySpeedCurve.Evaluate(num);
			float num3 = this.proximitySpreadCurve.Evaluate(num);
			ParticleSystem.MinMaxCurve minMaxCurve = new ParticleSystem.MinMaxCurve(-num3, num3);
			this.StartFlame(this.leftMitt, num, num2, minMaxCurve);
			this.StartFlame(this.rightMitt, num, num2, minMaxCurve);
			if (this.vibrateController && this.vibrationStrengthMult > 0f)
			{
				GorillaTagger.Instance.StartVibration(true, this.vibrationStrengthMult * 0.5f * num, Time.deltaTime);
				GorillaTagger.Instance.StartVibration(false, this.vibrationStrengthMult * 0.5f * num, Time.deltaTime);
			}
			this.SetInterferenceAudio(true);
			float num4 = 1f - Mathf.Exp(-this.proximityAudioReactionSpeed * Time.deltaTime);
			this.proximityAudioSource.pitch = Mathf.Lerp(this.proximityAudioSource.pitch, this.proximityAudioPitch.Evaluate(num), num4);
			this.proximityAudioSource.volume = Mathf.Lerp(this.proximityAudioSource.volume, this.proximityAudioVolume.Evaluate(num), num4);
			return;
		}
		if (this.leftMitt.thermalSource.enabled || this.rightMitt.thermalSource.enabled)
		{
			this.leftMitt.flame.Stop();
			this.leftMitt.thermalSource.enabled = false;
			this.rightMitt.flame.Stop();
			this.rightMitt.thermalSource.enabled = false;
			this.SetInterferenceAudio(false);
		}
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x0001A43C File Offset: 0x0001863C
	private void StartFlame(MarkOneMitts.Mitt mitt, float scale, float speed, ParticleSystem.MinMaxCurve xy)
	{
		if (!mitt.thermalSource.enabled)
		{
			mitt.flame.Play();
			mitt.thermalSource.enabled = true;
		}
		mitt.flameTransform.localScale = this.flameScale * scale * Vector3.one;
		mitt.flameMain.startSpeed = speed;
		mitt.flameForce.x = xy;
		mitt.flameForce.y = xy;
		mitt.thermalSource.celsius = this.heatMultiplier * scale;
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x0001A4C8 File Offset: 0x000186C8
	private void RunTimer(MarkOneMitts.Mitt mitt, bool isLeftHand)
	{
		if (mitt.timer <= 0f)
		{
			return;
		}
		mitt.timer -= Time.deltaTime;
		if (mitt.timer <= 0f)
		{
			mitt.timer = 0f;
			mitt.flame.Stop();
			mitt.thermalSource.enabled = false;
			if (this.leftMitt.timer <= 0f && this.rightMitt.timer <= 0f)
			{
				this.proximityEffect.enabled = true;
				return;
			}
		}
		else
		{
			float num = mitt.lastTapStrength * mitt.timer;
			mitt.flameTransform.localScale = this.flameScale * num * Vector3.one;
			mitt.thermalSource.celsius = this.heatMultiplier * num;
			if (this.vibrateController)
			{
				GorillaTagger.Instance.StartVibration(isLeftHand, this.vibrationStrengthMult * num, 0.1f);
			}
		}
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x0001A5B3 File Offset: 0x000187B3
	private void TryPlayProximityStartStopAudio(AudioClip clip, float volume)
	{
		if (this.proximityStartStopAudioSource.isPlaying)
		{
			return;
		}
		this.proximityStartStopAudioSource.clip = clip;
		this.proximityStartStopAudioSource.volume = volume;
		this.proximityStartStopAudioSource.Play();
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x0001A5E8 File Offset: 0x000187E8
	private void SetInterferenceAudio(bool active)
	{
		if (this.proximityAudioSource.isPlaying == active)
		{
			return;
		}
		if (active)
		{
			this.TryPlayProximityStartStopAudio(this.proximityStartAudioClip, this.proximityStartAudioVolume);
			this.proximityAudioSource.Play();
			return;
		}
		this.TryPlayProximityStartStopAudio(this.proximityStopAudioClip, this.proximityStopAudioVolume);
		this.proximityAudioSource.Stop();
	}

	// Token: 0x1700005B RID: 91
	// (get) Token: 0x060004B3 RID: 1203 RVA: 0x0001A642 File Offset: 0x00018842
	// (set) Token: 0x060004B4 RID: 1204 RVA: 0x0001A64A File Offset: 0x0001884A
	public bool TickRunning { get; set; }

	// Token: 0x060004B5 RID: 1205 RVA: 0x0001A654 File Offset: 0x00018854
	public void Tick()
	{
		if (this.leftMitt.timer <= 0f && this.rightMitt.timer <= 0f)
		{
			TickSystem<object>.RemoveTickCallback(this);
			return;
		}
		this.RunTimer(this.leftMitt, true);
		this.RunTimer(this.rightMitt, false);
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x0001A6A8 File Offset: 0x000188A8
	internal override void OnTap(HandEffectContext handContext)
	{
		float num = this.handSpeedToEffectStrength.Evaluate(handContext.Speed);
		if (num >= this.minEffectStrength)
		{
			TickSystem<object>.AddTickCallback(this);
			MarkOneMitts.Mitt mitt = (handContext.isLeftHand ? this.leftMitt : this.rightMitt);
			mitt.lastTapStrength = num;
			mitt.timer = this.flameTime;
			mitt.bursts[0].count = num * 10f;
			mitt.bursts[1].count = num * 5f;
			mitt.burst.emission.SetBursts(mitt.bursts);
			mitt.burstTransform.localScale = num * Vector3.one;
			this.StartFlame(mitt, num * this.flameScale * this.flameTime, this.flameSpeed, this.emptyParticleCurve);
			mitt.burst.Play();
			Keyframe[] keys = this.handSpeedToEffectStrength.keys;
			float value = keys[keys.Length - 1].value;
			handContext.soundPitch = Mathf.Clamp(value / num, 1f, 3f);
			this.proximityEffect.enabled = false;
			return;
		}
		handContext.soundFX = null;
	}

	// Token: 0x04000514 RID: 1300
	[SerializeField]
	private MarkOneMitts.Mitt leftMitt;

	// Token: 0x04000515 RID: 1301
	[SerializeField]
	private MarkOneMitts.Mitt rightMitt;

	// Token: 0x04000516 RID: 1302
	[SerializeField]
	private ProximityEffect proximityEffect;

	// Token: 0x04000517 RID: 1303
	[SerializeField]
	private AnimationCurve handSpeedToEffectStrength;

	// Token: 0x04000518 RID: 1304
	[SerializeField]
	private float minEffectStrength = 0.5f;

	// Token: 0x04000519 RID: 1305
	[SerializeField]
	private float flameScale = 3f;

	// Token: 0x0400051A RID: 1306
	[SerializeField]
	private float flameTime = 0.5f;

	// Token: 0x0400051B RID: 1307
	[SerializeField]
	private float flameSpeed = 5f;

	// Token: 0x0400051C RID: 1308
	[SerializeField]
	private float heatMultiplier = 100f;

	// Token: 0x0400051D RID: 1309
	[SerializeField]
	private AnimationCurve proximitySpeedCurve;

	// Token: 0x0400051E RID: 1310
	[SerializeField]
	private AnimationCurve proximitySpreadCurve;

	// Token: 0x0400051F RID: 1311
	[Space]
	[SerializeField]
	private bool vibrateController;

	// Token: 0x04000520 RID: 1312
	[SerializeField]
	private float vibrationStrengthMult = 1f;

	// Token: 0x04000521 RID: 1313
	[Space]
	[SerializeField]
	private AudioSource proximityAudioSource;

	// Token: 0x04000522 RID: 1314
	[SerializeField]
	private AnimationCurve proximityAudioPitch;

	// Token: 0x04000523 RID: 1315
	[SerializeField]
	private AnimationCurve proximityAudioVolume;

	// Token: 0x04000524 RID: 1316
	[SerializeField]
	private float proximityAudioReactionSpeed = 0.2f;

	// Token: 0x04000525 RID: 1317
	[Space]
	[SerializeField]
	private AudioSource proximityStartStopAudioSource;

	// Token: 0x04000526 RID: 1318
	[SerializeField]
	private AudioClip proximityStartAudioClip;

	// Token: 0x04000527 RID: 1319
	[SerializeField]
	private float proximityStartAudioVolume = 0.5f;

	// Token: 0x04000528 RID: 1320
	[SerializeField]
	private AudioClip proximityStopAudioClip;

	// Token: 0x04000529 RID: 1321
	[SerializeField]
	private float proximityStopAudioVolume = 0.5f;

	// Token: 0x0400052A RID: 1322
	private VRRig rig;

	// Token: 0x0400052B RID: 1323
	private ParticleSystem.MinMaxCurve emptyParticleCurve = new ParticleSystem.MinMaxCurve(0f);

	// Token: 0x020000BF RID: 191
	[Serializable]
	private class Mitt
	{
		// Token: 0x060004B8 RID: 1208 RVA: 0x0001A868 File Offset: 0x00018A68
		public void Init()
		{
			this.bursts = new ParticleSystem.Burst[2];
			this.burst.emission.GetBursts(this.bursts);
			this.burstTransform = this.burst.transform;
			this.flameTransform = this.flame.transform;
			this.flameMain = this.flame.main;
			this.flameForce = this.flame.forceOverLifetime;
		}

		// Token: 0x0400052D RID: 1325
		public ParticleSystem burst;

		// Token: 0x0400052E RID: 1326
		public ParticleSystem flame;

		// Token: 0x0400052F RID: 1327
		public ThermalSourceVolume thermalSource;

		// Token: 0x04000530 RID: 1328
		[NonSerialized]
		public float lastTapStrength;

		// Token: 0x04000531 RID: 1329
		[NonSerialized]
		public ParticleSystem.Burst[] bursts;

		// Token: 0x04000532 RID: 1330
		[NonSerialized]
		public Transform burstTransform;

		// Token: 0x04000533 RID: 1331
		[NonSerialized]
		public Transform flameTransform;

		// Token: 0x04000534 RID: 1332
		[NonSerialized]
		public float timer;

		// Token: 0x04000535 RID: 1333
		[NonSerialized]
		public ParticleSystem.MainModule flameMain;

		// Token: 0x04000536 RID: 1334
		[NonSerialized]
		public ParticleSystem.ForceOverLifetimeModule flameForce;
	}
}
