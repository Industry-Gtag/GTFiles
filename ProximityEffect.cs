using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002F4 RID: 756
public class ProximityEffect : MonoBehaviour, ITickSystemTick
{
	// Token: 0x0600133E RID: 4926 RVA: 0x000660C5 File Offset: 0x000642C5
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.enableVisualization = false;
		if (this.visualizer)
		{
			Object.Destroy(this.visualizer);
		}
	}

	// Token: 0x0600133F RID: 4927 RVA: 0x000660F2 File Offset: 0x000642F2
	public void AddReceiver(IProximityEffectReceiver receiver)
	{
		if (this.receivers == null)
		{
			this.receivers = new List<IProximityEffectReceiver> { receiver };
			return;
		}
		if (!this.receivers.Contains(receiver))
		{
			this.receivers.Add(receiver);
		}
	}

	// Token: 0x06001340 RID: 4928 RVA: 0x00066129 File Offset: 0x00064329
	public void RemoveReceiver(IProximityEffectReceiver receiver)
	{
		this.receivers.Remove(receiver);
	}

	// Token: 0x06001341 RID: 4929 RVA: 0x00066138 File Offset: 0x00064338
	private void StartCalculating()
	{
		this.centerTransform.position = (this.leftTransform.position + this.rightTransform.position) / 2f;
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06001342 RID: 4930 RVA: 0x00066170 File Offset: 0x00064370
	private void StopCalculating()
	{
		ProximityEffect.ProximityEvent[] array = this.events;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ResetAllEvents();
		}
		ContinuousPropertyArray continuousPropertyArray = this.continuousProperties;
		if (continuousPropertyArray != null)
		{
			continuousPropertyArray.ApplyAll(0f);
		}
		UnityEvent<float> unityEvent = this.onScoreCalculated;
		if (unityEvent != null)
		{
			unityEvent.Invoke(0f);
		}
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06001343 RID: 4931 RVA: 0x000661CC File Offset: 0x000643CC
	private void OnEnable()
	{
		if (this.triggersToActivate == 0)
		{
			this.StartCalculating();
		}
	}

	// Token: 0x06001344 RID: 4932 RVA: 0x000661DC File Offset: 0x000643DC
	private void OnDisable()
	{
		if (this.triggersToActivate == 0)
		{
			this.StopCalculating();
		}
	}

	// Token: 0x06001345 RID: 4933 RVA: 0x000661EC File Offset: 0x000643EC
	public void AddTrigger()
	{
		if (this.numTriggers < this.triggersToActivate)
		{
			this.numTriggers++;
			if (this.numTriggers == this.triggersToActivate)
			{
				this.StartCalculating();
			}
		}
	}

	// Token: 0x06001346 RID: 4934 RVA: 0x0006621E File Offset: 0x0006441E
	public void RemoveTrigger()
	{
		if (this.numTriggers > 0)
		{
			if (this.numTriggers == this.triggersToActivate)
			{
				this.StopCalculating();
			}
			this.numTriggers--;
		}
	}

	// Token: 0x06001347 RID: 4935 RVA: 0x0006624C File Offset: 0x0006444C
	private void CalculateProximityScores()
	{
		float num;
		float num2;
		float num3;
		Vector3 vector;
		this.CalculateProximityScores(true, out num, out num2, out num3, out vector);
	}

	// Token: 0x06001348 RID: 4936 RVA: 0x00066268 File Offset: 0x00064468
	private void CalculateProximityScores(out float distance, out float alignment, out float parallel, out Vector3 midpoint)
	{
		this.CalculateProximityScores(false, out distance, out alignment, out parallel, out midpoint);
	}

	// Token: 0x06001349 RID: 4937 RVA: 0x00066278 File Offset: 0x00064478
	private void CalculateProximityScores(bool drawGizmos, out float distance, out float alignment, out float parallel, out Vector3 midpoint)
	{
		float num = ((this.rig != null) ? this.rig.scaleFactor : 1f);
		Vector3 position = this.leftTransform.position;
		Vector3 position2 = this.rightTransform.position;
		Vector3 forward = this.leftTransform.forward;
		Vector3 forward2 = this.rightTransform.forward;
		Vector3 vector = (position2 - position) / num;
		float magnitude = vector.magnitude;
		Vector3 vector2 = vector / magnitude;
		distance = this.scoreCurves.distanceModifierCurve.Evaluate(magnitude);
		alignment = this.scoreCurves.alignmentModifierCurve.Evaluate(-Vector3.Dot(forward, forward2));
		parallel = this.scoreCurves.parallelModifierCurve.Evaluate((Vector3.Dot(forward, vector2) + Vector3.Dot(forward2, -vector2)) / 2f);
		midpoint = position + 0.5f * vector;
	}

	// Token: 0x0600134A RID: 4938 RVA: 0x00066370 File Offset: 0x00064570
	private void MoveTransform(Transform target, float score, Vector3 midpoint)
	{
		Vector3 vector;
		Quaternion quaternion;
		target.GetPositionAndRotation(out vector, out quaternion);
		Vector3 vector2 = Vector3.Lerp(vector, midpoint, ProximityEffect.<MoveTransform>g__ExpT|40_0(this.positionCTLerpSpeed));
		if (this.rotateCT)
		{
			Vector3 vector3 = (vector2 - vector) / Time.deltaTime;
			if (vector3 != Vector3.zero)
			{
				Quaternion quaternion2 = Quaternion.LookRotation(vector3);
				Quaternion quaternion3 = Quaternion.LookRotation(vector2 - this.rig.syncPos);
				Quaternion quaternion4 = Quaternion.Slerp(quaternion, Quaternion.Slerp(quaternion3, quaternion2, vector3.magnitude), ProximityEffect.<MoveTransform>g__ExpT|40_0(this.rotationCTLerpSpeed));
				target.SetPositionAndRotation(vector2, quaternion4);
			}
		}
		else
		{
			target.position = vector2;
		}
		if (this.scaleCT)
		{
			target.localScale = Vector3.Lerp(target.localScale, score * this.scaleCTMult * Vector3.one, ProximityEffect.<MoveTransform>g__ExpT|40_0(this.scaleCTLerpSpeed));
		}
	}

	// Token: 0x170001E8 RID: 488
	// (get) Token: 0x0600134B RID: 4939 RVA: 0x0006644C File Offset: 0x0006464C
	// (set) Token: 0x0600134C RID: 4940 RVA: 0x00066454 File Offset: 0x00064654
	public bool TickRunning { get; set; }

	// Token: 0x0600134D RID: 4941 RVA: 0x00066460 File Offset: 0x00064660
	public void Tick()
	{
		float num;
		float num2;
		float num3;
		Vector3 vector;
		this.CalculateProximityScores(out num, out num2, out num3, out vector);
		if (this.receivers != null)
		{
			for (int i = 0; i < this.receivers.Count; i++)
			{
				this.receivers[i].OnProximityCalculated(num, num2, num3);
			}
		}
		float num4 = num * num2 * num3;
		ContinuousPropertyArray continuousPropertyArray = this.continuousProperties;
		if (continuousPropertyArray != null)
		{
			continuousPropertyArray.ApplyAll(num4);
		}
		UnityEvent<float> unityEvent = this.onScoreCalculated;
		if (unityEvent != null)
		{
			unityEvent.Invoke(num4);
		}
		if (this.centerTransform != null)
		{
			this.MoveTransform(this.centerTransform, num4, vector);
		}
		this.anyAboveThreshold = false;
		foreach (ProximityEffect.ProximityEvent proximityEvent in this.events)
		{
			this.anyAboveThreshold = proximityEvent.Evaluate(num4) || this.anyAboveThreshold;
		}
	}

	// Token: 0x0600134F RID: 4943 RVA: 0x000665C5 File Offset: 0x000647C5
	[CompilerGenerated]
	internal static float <MoveTransform>g__ExpT|40_0(float speed)
	{
		return 1f - Mathf.Exp(-speed * Time.deltaTime);
	}

	// Token: 0x04001772 RID: 6002
	[SerializeField]
	private Transform leftTransform;

	// Token: 0x04001773 RID: 6003
	[SerializeField]
	private Transform rightTransform;

	// Token: 0x04001774 RID: 6004
	[SerializeField]
	[Tooltip("How many times AddTrigger() needs to be called before the events are allowed to be invoked. Used for pausing events until certain actions are performed (like squeezing the triggers of both controllers).")]
	private int triggersToActivate;

	// Token: 0x04001775 RID: 6005
	[Space]
	[SerializeField]
	[Tooltip("The transform that moves to follow the midpoint of the left and right transforms.")]
	private Transform centerTransform;

	// Token: 0x04001776 RID: 6006
	private const string SHOW_CONDITION = "@centerTransform != null";

	// Token: 0x04001777 RID: 6007
	[SerializeField]
	private float positionCTLerpSpeed = 10f;

	// Token: 0x04001778 RID: 6008
	[SerializeField]
	private bool rotateCT;

	// Token: 0x04001779 RID: 6009
	private const string SHOW_ROTATE_CONDITION = "@centerTransform != null && rotateCT";

	// Token: 0x0400177A RID: 6010
	[SerializeField]
	private float rotationCTLerpSpeed = 10f;

	// Token: 0x0400177B RID: 6011
	[SerializeField]
	private bool scaleCT;

	// Token: 0x0400177C RID: 6012
	private const string SHOW_SCALE_CONDITION = "@centerTransform != null && scaleCT";

	// Token: 0x0400177D RID: 6013
	[SerializeField]
	private float scaleCTLerpSpeed = 10f;

	// Token: 0x0400177E RID: 6014
	[SerializeField]
	private float scaleCTMult = 1f;

	// Token: 0x0400177F RID: 6015
	[Space]
	[SerializeField]
	[Tooltip("The curves that get evaluated to determine the alignment score. They get multiplied together, so their Y values should all range from 0-1. The result is compared against the thresholds of the ProximityEvents.")]
	private ProximityEffectScoreCurvesSO scoreCurves;

	// Token: 0x04001780 RID: 6016
	[Space]
	[SerializeField]
	private ContinuousPropertyArray continuousProperties;

	// Token: 0x04001781 RID: 6017
	[SerializeField]
	private UnityEvent<float> onScoreCalculated;

	// Token: 0x04001782 RID: 6018
	[SerializeField]
	private ProximityEffect.ProximityEvent[] events;

	// Token: 0x04001783 RID: 6019
	[Header("Editor Only")]
	[SerializeField]
	private Vector3 defaultLeftHandLocalPosition = new Vector3(-0.0568f, 0.04311f, 0.00249f);

	// Token: 0x04001784 RID: 6020
	[SerializeField]
	private Vector3 defaultLeftHandLocalEuler = new Vector3(173.176f, 80.201f, 3.615f);

	// Token: 0x04001785 RID: 6021
	[Header("Visualization is currently NOT WORKING IN PLAY MODE due to tick optimization")]
	[SerializeField]
	private bool enableVisualization = true;

	// Token: 0x04001786 RID: 6022
	[SerializeField]
	private Material visualizationMaterial;

	// Token: 0x04001787 RID: 6023
	[SerializeField]
	[Range(0f, 1f)]
	private float visualizationLineThickness = 0.01f;

	// Token: 0x04001788 RID: 6024
	[SerializeField]
	[HideInInspector]
	private LineRenderer visualizer;

	// Token: 0x04001789 RID: 6025
	private List<IProximityEffectReceiver> receivers;

	// Token: 0x0400178A RID: 6026
	private VRRig rig;

	// Token: 0x0400178B RID: 6027
	private bool anyAboveThreshold;

	// Token: 0x0400178C RID: 6028
	private int numTriggers;

	// Token: 0x020002F5 RID: 757
	[Serializable]
	private class ProximityEvent
	{
		// Token: 0x06001350 RID: 4944 RVA: 0x000665DC File Offset: 0x000647DC
		public bool Evaluate(float score)
		{
			if (score >= this.highThreshold)
			{
				if (!this.wasAboveThreshold && Time.time - this.lastThresholdTime >= this.highThresholdBufferTime)
				{
					UnityEvent unityEvent = this.onThresholdHigh;
					if (unityEvent != null)
					{
						unityEvent.Invoke();
					}
					this.wasAboveThreshold = true;
					this.wasBelowThreshold = false;
				}
				if (this.wasAboveThreshold)
				{
					this.lastThresholdTime = Time.time;
				}
				return true;
			}
			if (score < this.lowThreshold)
			{
				if (!this.wasBelowThreshold && Time.time - this.lastThresholdTime >= this.lowThresholdBufferTime)
				{
					UnityEvent unityEvent2 = this.onThresholdLow;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke();
					}
					this.wasAboveThreshold = false;
					this.wasBelowThreshold = true;
				}
				if (this.wasBelowThreshold)
				{
					this.lastThresholdTime = Time.time;
				}
			}
			return false;
		}

		// Token: 0x06001351 RID: 4945 RVA: 0x0006669A File Offset: 0x0006489A
		public void ResetAllEvents()
		{
			this.wasAboveThreshold = false;
			this.wasBelowThreshold = true;
		}

		// Token: 0x0400178E RID: 6030
		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("High-threshold events will only fire if the alignment score is above this value.")]
		private float highThreshold = 0.5f;

		// Token: 0x0400178F RID: 6031
		[SerializeField]
		[Tooltip("Wait this many seconds before activating the high-threshold events.")]
		private float highThresholdBufferTime;

		// Token: 0x04001790 RID: 6032
		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Low-threshold events will only fire if the alignment score is below this value.")]
		private float lowThreshold = 0.3f;

		// Token: 0x04001791 RID: 6033
		[SerializeField]
		[Tooltip("Wait this many seconds before activating the low-threshold events.")]
		private float lowThresholdBufferTime;

		// Token: 0x04001792 RID: 6034
		public UnityEvent onThresholdHigh;

		// Token: 0x04001793 RID: 6035
		public UnityEvent onThresholdLow;

		// Token: 0x04001794 RID: 6036
		private bool wasAboveThreshold;

		// Token: 0x04001795 RID: 6037
		private bool wasBelowThreshold = true;

		// Token: 0x04001796 RID: 6038
		private float lastThresholdTime = -100f;
	}
}
