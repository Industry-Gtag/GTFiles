using System;
using UnityEngine;

// Token: 0x0200059F RID: 1439
[RequireComponent(typeof(Animator))]
public class RandomAnimationSelector : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002479 RID: 9337 RVA: 0x000C3E00 File Offset: 0x000C2000
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animationTrigger = Animator.StringToHash(this.animationTriggerName);
		this.animationSelect = Animator.StringToHash(this.animationSelectName);
	}

	// Token: 0x0600247A RID: 9338 RVA: 0x000C3E30 File Offset: 0x000C2030
	public void OnEnable()
	{
		if (this.animator != null)
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			this.lastSliceUpdateTime = Time.time;
		}
	}

	// Token: 0x0600247B RID: 9339 RVA: 0x00019269 File Offset: 0x00017469
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600247C RID: 9340 RVA: 0x000C3E54 File Offset: 0x000C2054
	public void SliceUpdate()
	{
		float num = Time.time - this.lastSliceUpdateTime;
		this.lastSliceUpdateTime = Time.time;
		float num2 = 1f - Mathf.Exp(-this.animationChancePerSecond * num);
		if (Random.value < num2)
		{
			float num3 = Time.time - (float)((int)Time.time);
			this.animator.SetFloat(this.animationSelect, num3);
			this.animator.SetTrigger(this.animationTrigger);
		}
	}

	// Token: 0x04002FD6 RID: 12246
	[SerializeField]
	private string animationTriggerName;

	// Token: 0x04002FD7 RID: 12247
	private int animationTrigger;

	// Token: 0x04002FD8 RID: 12248
	[SerializeField]
	private string animationSelectName;

	// Token: 0x04002FD9 RID: 12249
	private int animationSelect;

	// Token: 0x04002FDA RID: 12250
	[Range(0f, 1f)]
	[SerializeField]
	private float animationChancePerSecond = 0.33f;

	// Token: 0x04002FDB RID: 12251
	private Animator animator;

	// Token: 0x04002FDC RID: 12252
	private float lastSliceUpdateTime;
}
