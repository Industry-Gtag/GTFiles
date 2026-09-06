using System;
using UnityEngine;

// Token: 0x02000DC3 RID: 3523
[CreateAssetMenu(fileName = "ObservableBehaviorRule", menuName = "Utilities/ObservableBehaviorRule")]
public class ObservableBehaviorRule : ScriptableObject
{
	// Token: 0x17000843 RID: 2115
	// (get) Token: 0x06005690 RID: 22160 RVA: 0x001C4381 File Offset: 0x001C2581
	public Vector2 ObservableDistanceRange
	{
		get
		{
			return this.observableDistanceRange;
		}
	}

	// Token: 0x17000844 RID: 2116
	// (get) Token: 0x06005691 RID: 22161 RVA: 0x001C4389 File Offset: 0x001C2589
	public Vector2 ObservableDotRange
	{
		get
		{
			return this.observableDotRange;
		}
	}

	// Token: 0x17000845 RID: 2117
	// (get) Token: 0x06005692 RID: 22162 RVA: 0x001C4391 File Offset: 0x001C2591
	public bool InverseObservable
	{
		get
		{
			return this.inverseObservable;
		}
	}

	// Token: 0x04006792 RID: 26514
	[SerializeField]
	private Vector2 observableDistanceRange = new Vector2(0f, 15f);

	// Token: 0x04006793 RID: 26515
	[SerializeField]
	private Vector2 observableDotRange = new Vector2(-1f, 0f);

	// Token: 0x04006794 RID: 26516
	[SerializeField]
	private bool inverseObservable;
}
