using System;
using TMPro;
using UnityEngine;

// Token: 0x020003E6 RID: 998
public class HowManyMonkeDisplay : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060017B8 RID: 6072 RVA: 0x00088468 File Offset: 0x00086668
	public void OnEnable()
	{
		this.currValue = (this.nextValue = HowManyMonke.ThisMany);
		this.text.text = this.currValue.ToString("N0");
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060017B9 RID: 6073 RVA: 0x000884AB File Offset: 0x000866AB
	public void OnDisable()
	{
		HowManyMonke.OnCheck = (Action<int>)Delegate.Remove(HowManyMonke.OnCheck, new Action<int>(this.HowManyMonke_OnCheck));
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060017BA RID: 6074 RVA: 0x000884D5 File Offset: 0x000866D5
	private void OnDestroy()
	{
		HowManyMonke.OnCheck = (Action<int>)Delegate.Remove(HowManyMonke.OnCheck, new Action<int>(this.HowManyMonke_OnCheck));
	}

	// Token: 0x060017BB RID: 6075 RVA: 0x000884F7 File Offset: 0x000866F7
	private void HowManyMonke_OnCheck(int thisMany)
	{
		this.currValue = this.nextValue;
		this.nextValue = thisMany;
		this.checkTime = Time.time;
	}

	// Token: 0x060017BC RID: 6076 RVA: 0x00088518 File Offset: 0x00086718
	public void SliceUpdate()
	{
		float num = Mathf.Lerp((float)this.currValue, (float)this.nextValue, (Time.time - this.checkTime) / HowManyMonke.RecheckDelay);
		this.text.text = num.ToString("N0");
		this.particleSystem.emission.rateOverTime = this.particleSystemRateToCount.Evaluate(num);
		float sqrMagnitude = (VRRig.LocalRig.transform.position - base.transform.position).sqrMagnitude;
		if (this.observable && sqrMagnitude > this.observableDistance)
		{
			this.observable = false;
			HowManyMonke.OnCheck = (Action<int>)Delegate.Remove(HowManyMonke.OnCheck, new Action<int>(this.HowManyMonke_OnCheck));
			if (this.observableActive)
			{
				this.observableActive.SetActive(this.observable);
				return;
			}
		}
		else if (!this.observable && sqrMagnitude < this.observableDistance)
		{
			this.observable = true;
			HowManyMonke.OnCheck = (Action<int>)Delegate.Combine(HowManyMonke.OnCheck, new Action<int>(this.HowManyMonke_OnCheck));
			if (this.observableActive)
			{
				this.observableActive.SetActive(this.observable);
			}
		}
	}

	// Token: 0x040022FD RID: 8957
	[SerializeField]
	private TMP_Text text;

	// Token: 0x040022FE RID: 8958
	[SerializeField]
	private float observableDistance = 100f;

	// Token: 0x040022FF RID: 8959
	[SerializeField]
	private GameObject observableActive;

	// Token: 0x04002300 RID: 8960
	[SerializeField]
	private ParticleSystem particleSystem;

	// Token: 0x04002301 RID: 8961
	[SerializeField]
	private AnimationCurve particleSystemRateToCount;

	// Token: 0x04002302 RID: 8962
	private bool observable;

	// Token: 0x04002303 RID: 8963
	private int currValue;

	// Token: 0x04002304 RID: 8964
	private int nextValue;

	// Token: 0x04002305 RID: 8965
	private float checkTime;
}
