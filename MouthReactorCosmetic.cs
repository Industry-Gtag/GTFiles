using System;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000951 RID: 2385
public class MouthReactorCosmetic : MonoBehaviour, ITickSystemTick
{
	// Token: 0x06003EA7 RID: 16039 RVA: 0x00152078 File Offset: 0x00150278
	private void ResetReactorTransform()
	{
		if (this.reactorTransform == null)
		{
			this.reactorTransform = base.transform;
		}
	}

	// Token: 0x06003EA8 RID: 16040 RVA: 0x00152094 File Offset: 0x00150294
	private void ResetRadius()
	{
		this.reactorRadius = 0.1666667f;
	}

	// Token: 0x170005C2 RID: 1474
	// (get) Token: 0x06003EA9 RID: 16041 RVA: 0x001520A1 File Offset: 0x001502A1
	private bool IsRadiusChanged
	{
		get
		{
			return this.reactorRadius != 0.1666667f;
		}
	}

	// Token: 0x06003EAA RID: 16042 RVA: 0x001520B3 File Offset: 0x001502B3
	private void ResetOffset()
	{
		this.mouthOffset = MouthReactorCosmetic.DEFAULT_OFFSET;
	}

	// Token: 0x170005C3 RID: 1475
	// (get) Token: 0x06003EAB RID: 16043 RVA: 0x001520C0 File Offset: 0x001502C0
	private bool IsOffsetChanged
	{
		get
		{
			return this.mouthOffset != MouthReactorCosmetic.DEFAULT_OFFSET;
		}
	}

	// Token: 0x06003EAC RID: 16044 RVA: 0x001520D2 File Offset: 0x001502D2
	private void OnEnable()
	{
		if (this.myRig == null)
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06003EAD RID: 16045 RVA: 0x0001A29F File Offset: 0x0001849F
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x170005C4 RID: 1476
	// (get) Token: 0x06003EAE RID: 16046 RVA: 0x001520EE File Offset: 0x001502EE
	// (set) Token: 0x06003EAF RID: 16047 RVA: 0x001520F6 File Offset: 0x001502F6
	public bool TickRunning { get; set; }

	// Token: 0x06003EB0 RID: 16048 RVA: 0x00152100 File Offset: 0x00150300
	public void Tick()
	{
		Vector3 vector = this.myRig.head.rigTarget.TransformPoint(this.mouthOffset);
		float sqrMagnitude = (this.reactorTransform.TransformPoint(this.reactorOffset) - vector).sqrMagnitude;
		if (sqrMagnitude < this.reactorRadius * this.reactorRadius)
		{
			if ((!this.mustExitBeforeRefire || !this.wasInside) && Time.time - this.lastInsideTime >= this.eventRefireDelay)
			{
				UnityEvent unityEvent = this.onInsideMouth;
				if (unityEvent != null)
				{
					unityEvent.Invoke();
				}
				this.lastInsideTime = Time.time;
			}
			this.wasInside = true;
		}
		else
		{
			this.wasInside = false;
		}
		if (this.continuousProperties.Count > 0)
		{
			this.continuousProperties.ApplyAll(Mathf.Min(0f, Mathf.Sqrt(sqrMagnitude) - this.reactorRadius));
		}
	}

	// Token: 0x04004F13 RID: 20243
	private static readonly Vector3 DEFAULT_OFFSET = new Vector3(0f, 0.0208f, 0.171f);

	// Token: 0x04004F14 RID: 20244
	private const float DEFAULT_RADIUS = 0.1666667f;

	// Token: 0x04004F15 RID: 20245
	[Tooltip("The transform to check against the mouth's position. Defaults to the transform this script is attached to.")]
	public Transform reactorTransform;

	// Token: 0x04004F16 RID: 20246
	[Tooltip("Offset the relative position of the reactor transform.")]
	public Vector3 reactorOffset = Vector3.zero;

	// Token: 0x04004F17 RID: 20247
	[Tooltip("How close the reactor needs to be to the mouth to trigger the event.")]
	public float reactorRadius = 0.1666667f;

	// Token: 0x04004F18 RID: 20248
	[Tooltip("The continuous value is the distance to the mouth. When inside the mouth radius, the value will always be 0.")]
	public ContinuousPropertyArray continuousProperties;

	// Token: 0x04004F19 RID: 20249
	[Tooltip("After the event fires, it must wait this many seconds before it fires again.")]
	public float eventRefireDelay = 0.6f;

	// Token: 0x04004F1A RID: 20250
	[Tooltip("After the event fires, prevent firing again until the reactor transform is moved outside the mouth and then back in.")]
	public bool mustExitBeforeRefire = true;

	// Token: 0x04004F1B RID: 20251
	public UnityEvent onInsideMouth;

	// Token: 0x04004F1C RID: 20252
	public Vector3 mouthOffset = MouthReactorCosmetic.DEFAULT_OFFSET;

	// Token: 0x04004F1D RID: 20253
	private VRRig myRig;

	// Token: 0x04004F1E RID: 20254
	private float lastInsideTime;

	// Token: 0x04004F1F RID: 20255
	private bool wasInside;
}
