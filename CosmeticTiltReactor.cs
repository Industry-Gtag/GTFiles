using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000687 RID: 1671
public class CosmeticTiltReactor : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060029DF RID: 10719 RVA: 0x000E1D14 File Offset: 0x000DFF14
	private void Awake()
	{
		this.referenceDirection.Normalize();
		if (!this.useTransform && this.referenceDirection == Vector3.zero)
		{
			GTDev.LogError<string>("CosmeticTiltReactor " + base.gameObject.name + " referenceDirection cannot be 0 vector", null);
		}
		if (this.useTransform && this.referenceTransform == null)
		{
			GTDev.LogError<string>("CosmeticTiltReactor " + base.gameObject.name + " referenceTransform cannot be null", null);
		}
		this.hasContinuousProperties = this.continuousProperties != null && this.continuousProperties.Count > 0;
		this.calculateDot = this.hasContinuousProperties;
		using (List<CosmeticTiltReactor.TiltEvent>.Enumerator enumerator = this.events.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.comparisonMethod == CosmeticTiltReactor.TiltEvent.ComparisonMethod.DotProduct)
				{
					this.calculateDot = true;
				}
				else
				{
					this.calculateAngle = true;
				}
				if (this.calculateDot && this.calculateAngle)
				{
					break;
				}
			}
		}
		this._rig = base.GetComponentInParent<VRRig>();
		this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
		if (this._rig == null && base.gameObject.GetComponentInParent<GTPlayer>() != null)
		{
			this._rig = GorillaTagger.Instance.offlineVRRig;
		}
		if (this._rig == null && !this.syncForAllPlayers)
		{
			GTDev.LogError<string>("CosmeticTiltReactor on " + base.gameObject.name + " set to not syncForAllPlayers and has no VR Rig parent. Events will not fire", null);
		}
		else if (this._rig != null)
		{
			this.isLocallyOwned = this._rig.isLocal;
		}
		if (this.parentTransferable == null && this.onlyWhileHeld)
		{
			GTDev.LogError<string>("CosmeticTiltReactor on " + base.gameObject.name + " set to OnlyWhileHeld but has no TransferrableObject parent. Events will not fire", null);
		}
	}

	// Token: 0x060029E0 RID: 10720 RVA: 0x000E1F04 File Offset: 0x000E0104
	public void OnEnable()
	{
		if (!this.syncForAllPlayers && !this.isLocallyOwned)
		{
			return;
		}
		if (this.useTransform && this.referenceTransform == null)
		{
			return;
		}
		Vector3 vector = (this.useTransform ? this.referenceTransform.up : this.referenceDirection);
		if (this.calculateAngle)
		{
			this.angle = Vector3.Angle(base.transform.up, vector);
		}
		if (this.calculateDot)
		{
			this.dotProduct = Vector3.Dot(base.transform.up, vector);
		}
		this.ResetEvents();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060029E1 RID: 10721 RVA: 0x000E1FA0 File Offset: 0x000E01A0
	public void OnDisable()
	{
		if (!this.syncForAllPlayers && !this.isLocallyOwned)
		{
			return;
		}
		if (this.useTransform && this.referenceTransform == null)
		{
			return;
		}
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060029E2 RID: 10722 RVA: 0x000E1FD4 File Offset: 0x000E01D4
	public void SliceUpdate()
	{
		if (this.onlyWhileHeld)
		{
			bool flag = this.parentTransferable != null && this.parentTransferable.InHand();
			if (!flag && this.wasInHand)
			{
				this.ResetEvents();
			}
			this.wasInHand = flag;
			if (!flag)
			{
				return;
			}
		}
		Vector3 vector = (this.useTransform ? this.referenceTransform.up : this.referenceDirection);
		if (this.calculateAngle)
		{
			this.angle = Vector3.Angle(base.transform.up, vector);
		}
		if (this.calculateDot)
		{
			this.dotProduct = Vector3.Dot(base.transform.up, vector);
		}
		this.FireEvents();
		if (this.hasContinuousProperties)
		{
			this.continuousProperties.ApplyAll(this.dotProduct);
		}
	}

	// Token: 0x060029E3 RID: 10723 RVA: 0x000E209C File Offset: 0x000E029C
	private void ResetEvents()
	{
		if (this.events == null || this.events.Count <= 0)
		{
			return;
		}
		foreach (CosmeticTiltReactor.TiltEvent tiltEvent in this.events)
		{
			switch (tiltEvent.tiltEventType)
			{
			case CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThreshold:
				tiltEvent.wasGreater = true;
				break;
			case CosmeticTiltReactor.TiltEvent.TiltEventType.GreaterThanThreshold:
				tiltEvent.wasGreater = false;
				break;
			case CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThresholdForDuration:
				tiltEvent.wasGreater = true;
				tiltEvent.hasFired = false;
				break;
			case CosmeticTiltReactor.TiltEvent.TiltEventType.GreaterThanThresholdForDuration:
				tiltEvent.wasGreater = false;
				tiltEvent.hasFired = false;
				break;
			}
			tiltEvent.thresholdCrossTime = double.MinValue;
		}
	}

	// Token: 0x060029E4 RID: 10724 RVA: 0x000E2160 File Offset: 0x000E0360
	private void FireEvents()
	{
		if (this.events == null || this.events.Count <= 0)
		{
			return;
		}
		foreach (CosmeticTiltReactor.TiltEvent tiltEvent in this.events)
		{
			bool flag = ((tiltEvent.comparisonMethod == CosmeticTiltReactor.TiltEvent.ComparisonMethod.Angle) ? (this.angle > tiltEvent.angleThreshold) : (this.dotProduct > tiltEvent.dotThreshold));
			CosmeticTiltReactor.TiltEvent.TiltEventType tiltEventType = tiltEvent.tiltEventType;
			if (tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThreshold || tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.GreaterThanThreshold)
			{
				if (flag != tiltEvent.wasGreater)
				{
					if (tiltEvent.tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.GreaterThanThreshold && flag)
					{
						if (tiltEvent.thresholdCrossTime + (double)tiltEvent.retriggerDelay <= Time.timeAsDouble)
						{
							tiltEvent.thresholdCrossTime = Time.timeAsDouble;
							tiltEvent.wasGreater = true;
							UnityEvent onTiltEvent = tiltEvent.OnTiltEvent;
							if (onTiltEvent != null)
							{
								onTiltEvent.Invoke();
							}
						}
					}
					else if (tiltEvent.tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThreshold && !flag)
					{
						if (tiltEvent.thresholdCrossTime + (double)tiltEvent.retriggerDelay <= Time.timeAsDouble)
						{
							tiltEvent.thresholdCrossTime = Time.timeAsDouble;
							tiltEvent.wasGreater = false;
							UnityEvent onTiltEvent2 = tiltEvent.OnTiltEvent;
							if (onTiltEvent2 != null)
							{
								onTiltEvent2.Invoke();
							}
						}
					}
					else
					{
						tiltEvent.wasGreater = flag;
					}
				}
			}
			else
			{
				if (tiltEvent.tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.GreaterThanThresholdForDuration)
				{
					if (flag)
					{
						if (!tiltEvent.wasGreater)
						{
							tiltEvent.thresholdCrossTime = Time.timeAsDouble;
						}
						else if (!tiltEvent.hasFired && tiltEvent.thresholdCrossTime + (double)tiltEvent.duration <= Time.timeAsDouble)
						{
							UnityEvent onTiltEvent3 = tiltEvent.OnTiltEvent;
							if (onTiltEvent3 != null)
							{
								onTiltEvent3.Invoke();
							}
							tiltEvent.hasFired = true;
						}
					}
					else
					{
						tiltEvent.hasFired = false;
					}
				}
				if (tiltEvent.tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThresholdForDuration)
				{
					if (!flag)
					{
						if (tiltEvent.wasGreater)
						{
							tiltEvent.thresholdCrossTime = Time.timeAsDouble;
						}
						else if (!tiltEvent.hasFired && tiltEvent.thresholdCrossTime + (double)tiltEvent.duration <= Time.timeAsDouble)
						{
							UnityEvent onTiltEvent4 = tiltEvent.OnTiltEvent;
							if (onTiltEvent4 != null)
							{
								onTiltEvent4.Invoke();
							}
							tiltEvent.hasFired = true;
						}
					}
					else
					{
						tiltEvent.hasFired = false;
					}
				}
				tiltEvent.wasGreater = flag;
			}
		}
	}

	// Token: 0x04003658 RID: 13912
	[SerializeField]
	private bool useTransform;

	// Token: 0x04003659 RID: 13913
	[Tooltip("Direction to which this transform's y is compared in world space")]
	[SerializeField]
	private Vector3 referenceDirection = Vector3.up;

	// Token: 0x0400365A RID: 13914
	[Tooltip("compare referenceTransform's y to this transform's y")]
	[SerializeField]
	private Transform referenceTransform;

	// Token: 0x0400365B RID: 13915
	[SerializeField]
	private List<CosmeticTiltReactor.TiltEvent> events;

	// Token: 0x0400365C RID: 13916
	[Tooltip("input for continuous properties is the dot product of this transform's y and the reference direction")]
	[SerializeField]
	private ContinuousPropertyArray continuousProperties;

	// Token: 0x0400365D RID: 13917
	[Tooltip("Should this script be run for all clients or just the owner")]
	[SerializeField]
	private bool syncForAllPlayers = true;

	// Token: 0x0400365E RID: 13918
	[Tooltip("option to run only if this transferrable object is in the hand")]
	[SerializeField]
	private bool onlyWhileHeld;

	// Token: 0x0400365F RID: 13919
	private VRRig _rig;

	// Token: 0x04003660 RID: 13920
	private TransferrableObject parentTransferable;

	// Token: 0x04003661 RID: 13921
	private bool isLocallyOwned;

	// Token: 0x04003662 RID: 13922
	private bool hasContinuousProperties;

	// Token: 0x04003663 RID: 13923
	private float angle;

	// Token: 0x04003664 RID: 13924
	private float dotProduct;

	// Token: 0x04003665 RID: 13925
	private bool calculateAngle;

	// Token: 0x04003666 RID: 13926
	private bool calculateDot;

	// Token: 0x04003667 RID: 13927
	private bool wasInHand;

	// Token: 0x02000688 RID: 1672
	[Serializable]
	public class TiltEvent
	{
		// Token: 0x060029E6 RID: 10726 RVA: 0x000E23A8 File Offset: 0x000E05A8
		public TiltEvent()
		{
			this.tiltEventType = CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThreshold;
			this.comparisonMethod = CosmeticTiltReactor.TiltEvent.ComparisonMethod.DotProduct;
			this.angleThreshold = 15f;
			this.retriggerDelay = 0f;
			this.duration = 0.5f;
		}

		// Token: 0x04003668 RID: 13928
		public CosmeticTiltReactor.TiltEvent.ComparisonMethod comparisonMethod;

		// Token: 0x04003669 RID: 13929
		public CosmeticTiltReactor.TiltEvent.TiltEventType tiltEventType;

		// Token: 0x0400366A RID: 13930
		[Range(0f, 180f)]
		[Tooltip("Angle in degrees from the reference direction")]
		public float angleThreshold;

		// Token: 0x0400366B RID: 13931
		[Range(-1f, 1f)]
		[Tooltip("Dot product compared to the reference direction")]
		public float dotThreshold;

		// Token: 0x0400366C RID: 13932
		[Tooltip("Minimum time between events firing")]
		public float retriggerDelay;

		// Token: 0x0400366D RID: 13933
		[Tooltip("Amount of time the angle or dot product should be less/greater than the threshold before firing an event")]
		public float duration;

		// Token: 0x0400366E RID: 13934
		public UnityEvent OnTiltEvent;

		// Token: 0x0400366F RID: 13935
		[NonSerialized]
		public bool wasGreater;

		// Token: 0x04003670 RID: 13936
		[NonSerialized]
		public bool hasFired;

		// Token: 0x04003671 RID: 13937
		[NonSerialized]
		public double thresholdCrossTime = double.MinValue;

		// Token: 0x02000689 RID: 1673
		public enum ComparisonMethod
		{
			// Token: 0x04003673 RID: 13939
			DotProduct,
			// Token: 0x04003674 RID: 13940
			Angle
		}

		// Token: 0x0200068A RID: 1674
		public enum TiltEventType
		{
			// Token: 0x04003676 RID: 13942
			LessThanThreshold,
			// Token: 0x04003677 RID: 13943
			GreaterThanThreshold,
			// Token: 0x04003678 RID: 13944
			LessThanThresholdForDuration,
			// Token: 0x04003679 RID: 13945
			GreaterThanThresholdForDuration
		}
	}
}
