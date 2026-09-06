using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000A03 RID: 2563
[DisallowMultipleComponent]
public class TappableGuardianIdol : Tappable
{
	// Token: 0x17000634 RID: 1588
	// (get) Token: 0x060041CD RID: 16845 RVA: 0x0015EB2B File Offset: 0x0015CD2B
	// (set) Token: 0x060041CE RID: 16846 RVA: 0x0015EB33 File Offset: 0x0015CD33
	public bool isChangingPositions { get; private set; }

	// Token: 0x060041CF RID: 16847 RVA: 0x0015EB3C File Offset: 0x0015CD3C
	protected override void OnEnable()
	{
		base.OnEnable();
		this._colliderBaseRadius = this.tapCollision.radius;
	}

	// Token: 0x060041D0 RID: 16848 RVA: 0x0015EB55 File Offset: 0x0015CD55
	protected override void OnDisable()
	{
		base.OnDisable();
		this.isChangingPositions = false;
		this._activationState = -1;
		this.isActivationReady = true;
		this.tapCollision.radius = this._colliderBaseRadius;
	}

	// Token: 0x060041D1 RID: 16849 RVA: 0x0015EB83 File Offset: 0x0015CD83
	public void OnZoneActiveStateChanged(bool zoneActive)
	{
		this._zoneIsActive = zoneActive;
		this.idolVisualRoot.SetActive(this._zoneIsActive);
	}

	// Token: 0x060041D2 RID: 16850 RVA: 0x0015EBA0 File Offset: 0x0015CDA0
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		if (info.Sender.IsLocal)
		{
			this.zoneManager.SetScaleCenterPoint(base.transform);
		}
		if (!this.isChangingPositions)
		{
			if (!this.zoneManager.IsZoneValid())
			{
				return;
			}
			RigContainer rigContainer;
			if (PhotonNetwork.LocalPlayer.IsMasterClient && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
			{
				if (Vector3.Magnitude(rigContainer.Rig.transform.position - base.transform.position) > this.requiredTapDistance + Mathf.Epsilon)
				{
					return;
				}
				this.zoneManager.IdolWasTapped(info.Sender);
			}
			if (!this.zoneManager.IsPlayerGuardian(info.Sender))
			{
				this.tapFX.Play();
			}
		}
	}

	// Token: 0x060041D3 RID: 16851 RVA: 0x0015EC68 File Offset: 0x0015CE68
	public void SetPosition(Vector3 position)
	{
		base.transform.position = position + new Vector3(0f, this.activeHeight, 0f);
		this.UpdateStageActivatedObjects();
		this._audio.GTPlayOneShot(this._activateSound, this._audio.volume);
		base.StartCoroutine(this.<SetPosition>g__Unshrink|49_0());
	}

	// Token: 0x060041D4 RID: 16852 RVA: 0x0015ECCA File Offset: 0x0015CECA
	public void MovePositions(Vector3 finalPosition)
	{
		if (this.isChangingPositions)
		{
			return;
		}
		this.transitionPos = finalPosition + this.fallStartOffset;
		this.finalPos = finalPosition;
		base.StartCoroutine(this.TransitionToNextIdol());
	}

	// Token: 0x060041D5 RID: 16853 RVA: 0x0015ECFC File Offset: 0x0015CEFC
	public void UpdateActivationProgress(float rawProgress, bool progressing)
	{
		this.isActivationReady = !progressing;
		if (rawProgress <= 0f && !progressing)
		{
			if (this._activationState >= 0)
			{
				if (this._activationRoutine != null)
				{
					base.StopCoroutine(this._activationRoutine);
					this._activationRoutine = null;
				}
				this.idolMeshRoot.transform.localScale = Vector3.one;
			}
			this._activationState = -1;
			this.UpdateStageActivatedObjects();
			this._audio.GTStop();
			return;
		}
		int num = (int)rawProgress;
		progressing &= this._activationStageSounds.Length > num;
		if (this._activationState == num || !progressing)
		{
			return;
		}
		if (this._activationRoutine != null)
		{
			base.StopCoroutine(this._activationRoutine);
		}
		this._activationRoutine = base.StartCoroutine(this.ShowActivationEffect());
		this._activationState = num;
		this.UpdateStageActivatedObjects();
		TappableGuardianIdol.IdolActivationSound idolActivationSound = this._activationStageSounds[num];
		this._audio.GTPlayOneShot(idolActivationSound.activation, this._audio.volume);
		this._audio.clip = idolActivationSound.loop;
		this._audio.loop = true;
		this._audio.GTPlay();
	}

	// Token: 0x060041D6 RID: 16854 RVA: 0x0015EE13 File Offset: 0x0015D013
	public void StartLookingAround()
	{
		if (this._lookRoutine != null)
		{
			base.StopCoroutine(this._lookRoutine);
		}
		this._lookRoutine = base.StartCoroutine(this.DoLookingAround());
	}

	// Token: 0x060041D7 RID: 16855 RVA: 0x0015EE3B File Offset: 0x0015D03B
	public void StopLookingAround()
	{
		if (this._lookRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this._lookRoutine);
		this._lookRoot.localRotation = Quaternion.identity;
		this._lookRoutine = null;
	}

	// Token: 0x060041D8 RID: 16856 RVA: 0x0015EE69 File Offset: 0x0015D069
	private IEnumerator DoLookingAround()
	{
		TappableGuardianIdol.<>c__DisplayClass54_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.nextLookTime = Time.time;
		CS$<>8__locals1._lookDirection = this._lookRoot.rotation;
		yield return null;
		for (;;)
		{
			if (Time.time >= CS$<>8__locals1.nextLookTime)
			{
				this.<DoLookingAround>g__PickLookTarget|54_0(ref CS$<>8__locals1);
			}
			this._lookRoot.rotation = Quaternion.Slerp(this._lookRoot.rotation, CS$<>8__locals1._lookDirection, Time.deltaTime * Mathf.Max(1f, (float)this._activationState * this._baseLookRate));
			yield return null;
		}
		yield break;
	}

	// Token: 0x060041D9 RID: 16857 RVA: 0x0015EE78 File Offset: 0x0015D078
	private void UpdateStageActivatedObjects()
	{
		foreach (TappableGuardianIdol.StageActivatedObject stageActivatedObject in this._stageActivatedObjects)
		{
			stageActivatedObject.UpdateActiveState(this._activationState);
		}
	}

	// Token: 0x060041DA RID: 16858 RVA: 0x0015EEAF File Offset: 0x0015D0AF
	private IEnumerator ShowActivationEffect()
	{
		float bulgeDuration = 1f;
		float lerpVal = 0f;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime / bulgeDuration;
			float num = Mathf.Lerp(1f, this.bulgeScale, this.bulgeCurve.Evaluate(lerpVal));
			this.idolMeshRoot.transform.localScale = Vector3.one * num;
			this.tapCollision.radius = this._colliderBaseRadius * num;
			yield return null;
		}
		this._activationRoutine = null;
		yield break;
	}

	// Token: 0x060041DB RID: 16859 RVA: 0x0015EEBE File Offset: 0x0015D0BE
	private IEnumerator TransitionToNextIdol()
	{
		this.isChangingPositions = true;
		this._audio.GTStop();
		if (this.knockbackOnTrigger)
		{
			this.zoneManager.TriggerIdolKnockback();
		}
		if (this.explodeFX)
		{
			ObjectPools.instance.Instantiate(this.explodeFX, base.transform.position, true);
		}
		this.UpdateActivationProgress(-1f, false);
		this.idolMeshRoot.SetActive(false);
		this.tapCollision.enabled = false;
		base.transform.position = this.transitionPos;
		yield return new WaitForSeconds(this.floatDuration);
		this.idolMeshRoot.SetActive(true);
		this.tapCollision.enabled = true;
		if (this.startFallFX)
		{
			ObjectPools.instance.Instantiate(this.startFallFX, this.transitionPos, true);
		}
		this._audio.GTPlayOneShot(this._descentSound, 1f);
		this.trailFX.Play();
		float fall = 0f;
		Vector3 startPos = this.transitionPos;
		Vector3 destinationPos = this.finalPos;
		while (fall < this.fallDuration)
		{
			fall += Time.deltaTime;
			base.transform.position = Vector3.Lerp(startPos, destinationPos, fall / this.fallDuration);
			yield return null;
		}
		base.transform.position = destinationPos;
		this.trailFX.Stop();
		if (this.landedFX)
		{
			ObjectPools.instance.Instantiate(this.landedFX, destinationPos, true);
		}
		if (this.knockbackOnLand)
		{
			this.zoneManager.TriggerIdolKnockback();
		}
		yield return new WaitForSeconds(this.inactiveDuration);
		this._audio.GTPlayOneShot(this._activateSound, this._audio.volume);
		float activateLerp = 0f;
		startPos = this.finalPos;
		destinationPos = this.finalPos + new Vector3(0f, this.activeHeight, 0f);
		AnimationCurve animCurve = AnimationCurves.EaseInOutQuad;
		while (activateLerp < 1f)
		{
			activateLerp = Mathf.Clamp01(activateLerp + Time.deltaTime / this.activationDuration);
			base.transform.position = Vector3.Lerp(startPos, destinationPos, animCurve.Evaluate(activateLerp));
			yield return null;
		}
		if (this.activatedFX)
		{
			ObjectPools.instance.Instantiate(this.activatedFX, base.transform.position, true);
		}
		if (this.knockbackOnActivate)
		{
			this.zoneManager.TriggerIdolKnockback();
		}
		this.isChangingPositions = false;
		yield break;
	}

	// Token: 0x060041DC RID: 16860 RVA: 0x0015EECD File Offset: 0x0015D0CD
	private float EaseInOut(float input)
	{
		if (input >= 0.5f)
		{
			return 1f - Mathf.Pow(-2f * input + 2f, 3f) / 2f;
		}
		return 4f * input * input * input;
	}

	// Token: 0x060041DE RID: 16862 RVA: 0x0015F004 File Offset: 0x0015D204
	[CompilerGenerated]
	private IEnumerator <SetPosition>g__Unshrink|49_0()
	{
		float lerpVal = 0f;
		float growDuration = 0.5f;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime / growDuration;
			float num = Mathf.Lerp(0f, 1f, AnimationCurves.EaseOutQuad.Evaluate(lerpVal));
			this.idolMeshRoot.transform.localScale = Vector3.one * num;
			this.tapCollision.radius = this._colliderBaseRadius * num;
			yield return null;
		}
		yield break;
	}

	// Token: 0x060041DF RID: 16863 RVA: 0x0015F014 File Offset: 0x0015D214
	[CompilerGenerated]
	private void <DoLookingAround>g__PickLookTarget|54_0(ref TappableGuardianIdol.<>c__DisplayClass54_0 A_1)
	{
		Transform transform = this.<DoLookingAround>g__GetClosestPlayerPosition|54_2(ref A_1);
		A_1._lookDirection = (transform ? Quaternion.LookRotation(transform.position - this._lookRoot.position) : Quaternion.Euler((float)Random.Range(-15, 15), this._lookRoot.rotation.eulerAngles.y + (float)Random.Range(-45, 45), 0f));
		this.<DoLookingAround>g__SetLookTime|54_1(ref A_1);
	}

	// Token: 0x060041E0 RID: 16864 RVA: 0x0015F092 File Offset: 0x0015D292
	[CompilerGenerated]
	private void <DoLookingAround>g__SetLookTime|54_1(ref TappableGuardianIdol.<>c__DisplayClass54_0 A_1)
	{
		A_1.nextLookTime = Time.time + this._lookInterval / (float)this._activationState * 0.5f + Random.value;
	}

	// Token: 0x060041E1 RID: 16865 RVA: 0x0015F0BC File Offset: 0x0015D2BC
	[CompilerGenerated]
	private Transform <DoLookingAround>g__GetClosestPlayerPosition|54_2(ref TappableGuardianIdol.<>c__DisplayClass54_0 A_1)
	{
		if (Random.value < this._randomLookChance)
		{
			return null;
		}
		Vector3 position = base.transform.position;
		float num = float.MaxValue;
		Transform transform = null;
		foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
		{
			if (!rigContainer.IsNull())
			{
				bool flag = rigContainer.Creator == this.zoneManager.CurrentGuardian;
				float num2 = Vector3.SqrMagnitude(rigContainer.transform.position - position) * (float)(flag ? 100 : 1);
				if (num2 < num)
				{
					num = num2;
					transform = rigContainer.transform;
				}
			}
		}
		return transform;
	}

	// Token: 0x04005278 RID: 21112
	[SerializeField]
	private GorillaGuardianZoneManager zoneManager;

	// Token: 0x04005279 RID: 21113
	[SerializeField]
	private float floatDuration = 2f;

	// Token: 0x0400527A RID: 21114
	[SerializeField]
	private float fallDuration = 1.5f;

	// Token: 0x0400527B RID: 21115
	[SerializeField]
	private float inactiveDuration = 2f;

	// Token: 0x0400527C RID: 21116
	[SerializeField]
	private float activationDuration = 1f;

	// Token: 0x0400527D RID: 21117
	[SerializeField]
	private float activeHeight = 1f;

	// Token: 0x0400527E RID: 21118
	[SerializeField]
	private bool knockbackOnTrigger;

	// Token: 0x0400527F RID: 21119
	[SerializeField]
	private bool knockbackOnLand = true;

	// Token: 0x04005280 RID: 21120
	[SerializeField]
	private bool knockbackOnActivate;

	// Token: 0x04005281 RID: 21121
	[SerializeField]
	private Vector3 fallStartOffset = new Vector3(3f, 20f, 3f);

	// Token: 0x04005282 RID: 21122
	[SerializeField]
	private ParticleSystem trailFX;

	// Token: 0x04005283 RID: 21123
	[SerializeField]
	private ParticleSystem tapFX;

	// Token: 0x04005284 RID: 21124
	[SerializeField]
	private GameObject explodeFX;

	// Token: 0x04005285 RID: 21125
	[SerializeField]
	private GameObject startFallFX;

	// Token: 0x04005286 RID: 21126
	[SerializeField]
	private GameObject landedFX;

	// Token: 0x04005287 RID: 21127
	[SerializeField]
	private GameObject activatedFX;

	// Token: 0x04005288 RID: 21128
	[SerializeField]
	private SphereCollider tapCollision;

	// Token: 0x04005289 RID: 21129
	[SerializeField]
	private GameObject idolVisualRoot;

	// Token: 0x0400528A RID: 21130
	[SerializeField]
	private GameObject idolMeshRoot;

	// Token: 0x0400528B RID: 21131
	[SerializeField]
	private AnimationCurve bulgeCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.5f, 1f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x0400528C RID: 21132
	[SerializeField]
	private float bulgeScale = 1.1f;

	// Token: 0x0400528D RID: 21133
	[SerializeField]
	private AudioSource _audio;

	// Token: 0x0400528E RID: 21134
	[SerializeField]
	private AudioClip[] _descentSound;

	// Token: 0x0400528F RID: 21135
	[SerializeField]
	private AudioClip[] _activateSound;

	// Token: 0x04005290 RID: 21136
	[SerializeField]
	private TappableGuardianIdol.IdolActivationSound[] _activationStageSounds;

	// Token: 0x04005291 RID: 21137
	[SerializeField]
	private TappableGuardianIdol.StageActivatedObject[] _stageActivatedObjects;

	// Token: 0x04005292 RID: 21138
	[Header("Look Around")]
	[SerializeField]
	private Transform _lookRoot;

	// Token: 0x04005293 RID: 21139
	[SerializeField]
	private float _lookInterval = 10f;

	// Token: 0x04005294 RID: 21140
	[SerializeField]
	private float _baseLookRate = 1f;

	// Token: 0x04005295 RID: 21141
	[SerializeField]
	private float _randomLookChance = 0.25f;

	// Token: 0x04005296 RID: 21142
	private Coroutine _lookRoutine;

	// Token: 0x04005298 RID: 21144
	private Vector3 transitionPos;

	// Token: 0x04005299 RID: 21145
	private Vector3 finalPos;

	// Token: 0x0400529A RID: 21146
	private int _activationState;

	// Token: 0x0400529B RID: 21147
	private Coroutine _activationRoutine;

	// Token: 0x0400529C RID: 21148
	private float _colliderBaseRadius;

	// Token: 0x0400529D RID: 21149
	private bool _zoneIsActive = true;

	// Token: 0x0400529E RID: 21150
	public bool isActivationReady;

	// Token: 0x0400529F RID: 21151
	private float requiredTapDistance = 3f;

	// Token: 0x02000A04 RID: 2564
	[Serializable]
	public struct IdolActivationSound
	{
		// Token: 0x040052A0 RID: 21152
		public AudioClip activation;

		// Token: 0x040052A1 RID: 21153
		public AudioClip loop;
	}

	// Token: 0x02000A05 RID: 2565
	[Serializable]
	public struct StageActivatedObject
	{
		// Token: 0x060041E2 RID: 16866 RVA: 0x0015F17C File Offset: 0x0015D37C
		public void UpdateActiveState(int stage)
		{
			bool flag = stage >= this.min && stage <= this.max;
			GameObject[] array = this.objects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(flag);
			}
		}

		// Token: 0x040052A2 RID: 21154
		public GameObject[] objects;

		// Token: 0x040052A3 RID: 21155
		public int min;

		// Token: 0x040052A4 RID: 21156
		public int max;
	}
}
