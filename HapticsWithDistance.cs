using System;
using UnityEngine;

// Token: 0x02000593 RID: 1427
[RequireComponent(typeof(SphereCollider))]
public class HapticsWithDistance : MonoBehaviour, ITickSystemTick
{
	// Token: 0x06002422 RID: 9250 RVA: 0x000C2537 File Offset: 0x000C0737
	private bool OnWrongLayer()
	{
		return base.gameObject.layer != 18;
	}

	// Token: 0x06002423 RID: 9251 RVA: 0x000C254B File Offset: 0x000C074B
	public void SetVibrationMult(float mult)
	{
		this.vibrationMult = mult;
	}

	// Token: 0x06002424 RID: 9252 RVA: 0x000C2554 File Offset: 0x000C0754
	public void FingerFlexVibrationMult(bool dummy, float mult)
	{
		this.SetVibrationMult(mult);
	}

	// Token: 0x06002425 RID: 9253 RVA: 0x000C255D File Offset: 0x000C075D
	private void Awake()
	{
		this.inverseColliderRadius = 1f / base.GetComponent<SphereCollider>().radius;
	}

	// Token: 0x06002426 RID: 9254 RVA: 0x000C2578 File Offset: 0x000C0778
	private void OnTriggerEnter(Collider other)
	{
		GorillaGrabber gorillaGrabber;
		if (other.TryGetComponent<GorillaGrabber>(out gorillaGrabber) && gorillaGrabber.enabled)
		{
			if (gorillaGrabber.IsLeftHand)
			{
				this.leftOfflineHand = gorillaGrabber.transform;
				TickSystem<object>.AddTickCallback(this);
				return;
			}
			if (gorillaGrabber.IsRightHand)
			{
				this.rightOfflineHand = gorillaGrabber.transform;
				TickSystem<object>.AddTickCallback(this);
			}
		}
	}

	// Token: 0x06002427 RID: 9255 RVA: 0x000C25CC File Offset: 0x000C07CC
	private void OnTriggerExit(Collider other)
	{
		if (this.leftOfflineHand == other.transform)
		{
			this.leftOfflineHand = null;
			if (!this.rightOfflineHand)
			{
				TickSystem<object>.RemoveTickCallback(this);
				return;
			}
		}
		else if (this.rightOfflineHand == other.transform)
		{
			this.rightOfflineHand = null;
			if (!this.leftOfflineHand)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}
	}

	// Token: 0x06002428 RID: 9256 RVA: 0x000C2634 File Offset: 0x000C0834
	private void OnDisable()
	{
		this.leftOfflineHand = null;
		this.rightOfflineHand = null;
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x170003CD RID: 973
	// (get) Token: 0x06002429 RID: 9257 RVA: 0x000C264A File Offset: 0x000C084A
	// (set) Token: 0x0600242A RID: 9258 RVA: 0x000C2652 File Offset: 0x000C0852
	public bool TickRunning { get; set; }

	// Token: 0x0600242B RID: 9259 RVA: 0x000C265C File Offset: 0x000C085C
	public void Tick()
	{
		Vector3 position = base.transform.position;
		if (this.leftOfflineHand)
		{
			GorillaTagger.Instance.StartVibration(true, this.vibrationMult * this.vibrationIntensityByDistance.Evaluate(Vector3.Distance(this.leftOfflineHand.position, position) * this.inverseColliderRadius), Time.deltaTime);
		}
		if (this.rightOfflineHand)
		{
			GorillaTagger.Instance.StartVibration(false, this.vibrationMult * this.vibrationIntensityByDistance.Evaluate(Vector3.Distance(this.rightOfflineHand.position, position) * this.inverseColliderRadius), Time.deltaTime);
		}
	}

	// Token: 0x04002F67 RID: 12135
	[SerializeField]
	[Tooltip("X is the normalized distance and should start at 0 and end at 1. Y is the vibration amplitude and can be anywhere from 0-1.")]
	private AnimationCurve vibrationIntensityByDistance;

	// Token: 0x04002F68 RID: 12136
	private float inverseColliderRadius;

	// Token: 0x04002F69 RID: 12137
	private float vibrationMult = 1f;

	// Token: 0x04002F6A RID: 12138
	private Transform leftOfflineHand;

	// Token: 0x04002F6B RID: 12139
	private Transform rightOfflineHand;
}
