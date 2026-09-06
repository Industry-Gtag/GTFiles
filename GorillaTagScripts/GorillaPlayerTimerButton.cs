using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000F94 RID: 3988
	public class GorillaPlayerTimerButton : MonoBehaviour
	{
		// Token: 0x06006331 RID: 25393 RVA: 0x001FED4C File Offset: 0x001FCF4C
		private void Awake()
		{
			this.materialProps = new MaterialPropertyBlock();
		}

		// Token: 0x06006332 RID: 25394 RVA: 0x001FED59 File Offset: 0x001FCF59
		private void Start()
		{
			this.TryInit();
		}

		// Token: 0x06006333 RID: 25395 RVA: 0x001FED59 File Offset: 0x001FCF59
		private void OnEnable()
		{
			this.TryInit();
		}

		// Token: 0x06006334 RID: 25396 RVA: 0x001FED64 File Offset: 0x001FCF64
		private void TryInit()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (PlayerTimerManager.instance == null)
			{
				return;
			}
			PlayerTimerManager.instance.OnTimerStopped.AddListener(new UnityAction<int, int>(this.OnTimerStopped));
			PlayerTimerManager.instance.OnLocalTimerStarted.AddListener(new UnityAction(this.OnLocalTimerStarted));
			if (this.isBothStartAndStop)
			{
				this.isStartButton = !PlayerTimerManager.instance.IsLocalTimerStarted();
			}
			this.isInitialized = true;
		}

		// Token: 0x06006335 RID: 25397 RVA: 0x001FEDE0 File Offset: 0x001FCFE0
		private void OnDisable()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.OnTimerStopped.RemoveListener(new UnityAction<int, int>(this.OnTimerStopped));
				PlayerTimerManager.instance.OnLocalTimerStarted.RemoveListener(new UnityAction(this.OnLocalTimerStarted));
			}
			this.isInitialized = false;
		}

		// Token: 0x06006336 RID: 25398 RVA: 0x001FEE37 File Offset: 0x001FD037
		private void OnLocalTimerStarted()
		{
			if (this.isBothStartAndStop)
			{
				this.isStartButton = false;
			}
		}

		// Token: 0x06006337 RID: 25399 RVA: 0x001FEE48 File Offset: 0x001FD048
		private void OnTimerStopped(int actorNum, int timeDelta)
		{
			if (this.isBothStartAndStop && actorNum == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.isStartButton = true;
			}
		}

		// Token: 0x06006338 RID: 25400 RVA: 0x001FEE6C File Offset: 0x001FD06C
		private void OnTriggerEnter(Collider other)
		{
			if (!base.enabled)
			{
				return;
			}
			GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent == null)
			{
				return;
			}
			if (Time.time < this.lastTriggeredTime + this.debounceTime)
			{
				return;
			}
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			this.mesh.GetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, this.pressColor);
			this.mesh.SetPropertyBlock(this.materialProps);
			PlayerTimerManager.instance.RequestTimerToggle(this.isStartButton);
			this.lastTriggeredTime = Time.time;
		}

		// Token: 0x06006339 RID: 25401 RVA: 0x001FEF2C File Offset: 0x001FD12C
		private void OnTriggerExit(Collider other)
		{
			if (!base.enabled)
			{
				return;
			}
			if (other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
			{
				return;
			}
			this.mesh.GetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, this.notPressedColor);
			this.mesh.SetPropertyBlock(this.materialProps);
		}

		// Token: 0x040071E1 RID: 29153
		private float lastTriggeredTime;

		// Token: 0x040071E2 RID: 29154
		[SerializeField]
		private bool isStartButton;

		// Token: 0x040071E3 RID: 29155
		[SerializeField]
		private bool isBothStartAndStop;

		// Token: 0x040071E4 RID: 29156
		[SerializeField]
		private float debounceTime = 0.5f;

		// Token: 0x040071E5 RID: 29157
		[SerializeField]
		private MeshRenderer mesh;

		// Token: 0x040071E6 RID: 29158
		[SerializeField]
		private Color pressColor;

		// Token: 0x040071E7 RID: 29159
		[SerializeField]
		private Color notPressedColor;

		// Token: 0x040071E8 RID: 29160
		private MaterialPropertyBlock materialProps;

		// Token: 0x040071E9 RID: 29161
		private bool isInitialized;
	}
}
