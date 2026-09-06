using System;
using UnityEngine;

// Token: 0x02000DC2 RID: 3522
public abstract class ObservableBehavior : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
{
	// Token: 0x17000841 RID: 2113
	// (get) Token: 0x06005681 RID: 22145 RVA: 0x001C4084 File Offset: 0x001C2284
	// (set) Token: 0x06005682 RID: 22146 RVA: 0x001C408C File Offset: 0x001C228C
	public ObservableBehaviorRule ObservableBehaviorRule
	{
		get
		{
			return this.observableBehaviorRule;
		}
		set
		{
			this.observableBehaviorRule = value;
			this.firstFrame = true;
		}
	}

	// Token: 0x17000842 RID: 2114
	// (get) Token: 0x06005683 RID: 22147 RVA: 0x001C409C File Offset: 0x001C229C
	public float Distance
	{
		get
		{
			return this.dist;
		}
	}

	// Token: 0x06005684 RID: 22148 RVA: 0x001C40A4 File Offset: 0x001C22A4
	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.UnityOnEnable();
	}

	// Token: 0x06005685 RID: 22149 RVA: 0x001C40B3 File Offset: 0x001C22B3
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		if (this.observable)
		{
			this.observable = false;
			this.OnLostObservable();
		}
		this.UnityOnDisable();
	}

	// Token: 0x06005686 RID: 22150 RVA: 0x001C40D8 File Offset: 0x001C22D8
	private void OnDestroy()
	{
		if (this.observable)
		{
			this.observable = false;
			this.OnLostObservable();
		}
	}

	// Token: 0x06005687 RID: 22151 RVA: 0x001C40F0 File Offset: 0x001C22F0
	void IGorillaSliceableSimple.SliceUpdate()
	{
		bool flag = this.observableVolume != null && this.observableVolume.LocalRigPresent;
		if (this.observableVolume == null && this.observableBehaviorRule != null)
		{
			Transform transform = Camera.main.transform;
			this.dist = Vector3.Distance(transform.position, base.transform.position);
			float num;
			if (this.observableBehaviorRule.InverseObservable)
			{
				num = Vector3.Dot((base.transform.position - transform.position).normalized, base.transform.forward);
			}
			else
			{
				num = Vector3.Dot((transform.position - base.transform.position).normalized, transform.transform.forward);
			}
			flag = this.observableBehaviorRule.ObservableDistanceRange.x <= this.dist && this.dist <= this.observableBehaviorRule.ObservableDistanceRange.y && this.observableBehaviorRule.ObservableDotRange.x <= num && num <= this.observableBehaviorRule.ObservableDotRange.y;
		}
		if ((this.firstFrame && flag) || (this.observable != flag && flag))
		{
			this.OnBecameObservable();
		}
		else if ((this.firstFrame && !flag && this.triggerLostObservableIfSpawnedUnobservable) || (this.observable != flag && !flag))
		{
			this.OnLostObservable();
		}
		this.firstFrame = false;
		this.observable = flag;
		if (this.observable)
		{
			this.ObservableSliceUpdate();
		}
	}

	// Token: 0x06005688 RID: 22152 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void UnityOnEnable()
	{
	}

	// Token: 0x06005689 RID: 22153 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void UnityOnDisable()
	{
	}

	// Token: 0x0600568A RID: 22154
	protected abstract void OnLostObservable();

	// Token: 0x0600568B RID: 22155
	protected abstract void OnBecameObservable();

	// Token: 0x0600568C RID: 22156
	protected abstract void ObservableSliceUpdate();

	// Token: 0x0600568D RID: 22157 RVA: 0x001C4290 File Offset: 0x001C2490
	public bool BuildValidationCheck()
	{
		if (this.observableVolume == null && this.observableBehaviorRule == null)
		{
			Debug.LogError("observableVolume & observableBehaviorRule can't both be null!");
			return false;
		}
		if (this.observableVolume != null && this.observableBehaviorRule != null)
		{
			Debug.LogWarning("observableVolume will override the observableBehaviorRule");
		}
		return true;
	}

	// Token: 0x0600568E RID: 22158 RVA: 0x001C42EC File Offset: 0x001C24EC
	public void OnDrawGizmosSelected()
	{
		if (this.observableBehaviorRule != null)
		{
			if (this.observableBehaviorRule.ObservableDistanceRange.x > 0f)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(base.transform.position, this.observableBehaviorRule.ObservableDistanceRange.x);
			}
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(base.transform.position, this.observableBehaviorRule.ObservableDistanceRange.y);
		}
	}

	// Token: 0x0400678C RID: 26508
	private bool firstFrame = true;

	// Token: 0x0400678D RID: 26509
	protected bool observable;

	// Token: 0x0400678E RID: 26510
	[SerializeField]
	private ObservableBehaviorRule observableBehaviorRule;

	// Token: 0x0400678F RID: 26511
	[SerializeField]
	private RigEventVolume observableVolume;

	// Token: 0x04006790 RID: 26512
	private float dist;

	// Token: 0x04006791 RID: 26513
	[SerializeField]
	private bool triggerLostObservableIfSpawnedUnobservable;
}
