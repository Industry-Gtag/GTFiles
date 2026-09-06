using System;
using UnityEngine;

// Token: 0x0200020F RID: 527
public class WingsWearable : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06000DEA RID: 3562 RVA: 0x0004C5F8 File Offset: 0x0004A7F8
	private void Awake()
	{
		if (this.animator == null)
		{
			GTDev.LogError<string>("WingsWearable on " + base.gameObject.name + " missing animator", null);
			return;
		}
		this.xform = this.animator.transform;
	}

	// Token: 0x06000DEB RID: 3563 RVA: 0x0004C645 File Offset: 0x0004A845
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		this.oldPos = this.xform.localPosition;
		this.lastSliceTime = Time.unscaledTime;
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x00019269 File Offset: 0x00017469
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x0004C66C File Offset: 0x0004A86C
	public void SliceUpdate()
	{
		Vector3 position = this.xform.position;
		float unscaledTime = Time.unscaledTime;
		float num = Mathf.Max(unscaledTime - this.lastSliceTime, Mathf.Epsilon);
		float num2 = (position - this.oldPos).magnitude / num;
		float num3 = this.flapSpeedCurve.Evaluate(Mathf.Abs(num2));
		this.animator.SetFloat(this.flapSpeedParamID, num3);
		this.oldPos = position;
		this.lastSliceTime = unscaledTime;
	}

	// Token: 0x04001096 RID: 4246
	[Tooltip("This animator must have a parameter called 'FlapSpeed'")]
	public Animator animator;

	// Token: 0x04001097 RID: 4247
	[Tooltip("X axis is move speed, Y axis is flap speed")]
	public AnimationCurve flapSpeedCurve;

	// Token: 0x04001098 RID: 4248
	private Transform xform;

	// Token: 0x04001099 RID: 4249
	private Vector3 oldPos;

	// Token: 0x0400109A RID: 4250
	private float lastSliceTime;

	// Token: 0x0400109B RID: 4251
	private readonly int flapSpeedParamID = Animator.StringToHash("FlapSpeed");
}
