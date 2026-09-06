using System;
using UnityEngine.Events;

// Token: 0x020000AF RID: 175
public class GenericObservable : ObservableBehavior
{
	// Token: 0x06000435 RID: 1077 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void ObservableSliceUpdate()
	{
	}

	// Token: 0x06000436 RID: 1078 RVA: 0x00018CEE File Offset: 0x00016EEE
	protected override void OnBecameObservable()
	{
		UnityEvent onObservable = this.OnObservable;
		if (onObservable == null)
		{
			return;
		}
		onObservable.Invoke();
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x00018D00 File Offset: 0x00016F00
	protected override void OnLostObservable()
	{
		UnityEvent onUnobservable = this.OnUnobservable;
		if (onUnobservable == null)
		{
			return;
		}
		onUnobservable.Invoke();
	}

	// Token: 0x0400049A RID: 1178
	public UnityEvent OnObservable;

	// Token: 0x0400049B RID: 1179
	public UnityEvent OnUnobservable;
}
