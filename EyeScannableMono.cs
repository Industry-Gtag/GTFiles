using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x020000B5 RID: 181
public class EyeScannableMono : MonoBehaviour, IEyeScannable
{
	// Token: 0x1400000D RID: 13
	// (add) Token: 0x06000458 RID: 1112 RVA: 0x00019398 File Offset: 0x00017598
	// (remove) Token: 0x06000459 RID: 1113 RVA: 0x000193D0 File Offset: 0x000175D0
	public event Action OnDataChange;

	// Token: 0x1700004D RID: 77
	// (get) Token: 0x0600045A RID: 1114 RVA: 0x00019405 File Offset: 0x00017605
	int IEyeScannable.scannableId
	{
		get
		{
			return base.GetInstanceID();
		}
	}

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x0600045B RID: 1115 RVA: 0x0001940D File Offset: 0x0001760D
	Vector3 IEyeScannable.Position
	{
		get
		{
			return base.transform.position - this._initialPosition + this._bounds.center;
		}
	}

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x0600045C RID: 1116 RVA: 0x00019435 File Offset: 0x00017635
	Bounds IEyeScannable.Bounds
	{
		get
		{
			return this._bounds;
		}
	}

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x0600045D RID: 1117 RVA: 0x0001943D File Offset: 0x0001763D
	IList<KeyValueStringPair> IEyeScannable.Entries
	{
		get
		{
			return this.data.Entries;
		}
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x0001944A File Offset: 0x0001764A
	private void Awake()
	{
		this.RecalculateBounds();
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x00019452 File Offset: 0x00017652
	public void OnEnable()
	{
		this.RecalculateBoundsLater();
		EyeScannerMono.Register(this);
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x000149A1 File Offset: 0x00012BA1
	public void OnDisable()
	{
		EyeScannerMono.Unregister(this);
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x00019460 File Offset: 0x00017660
	private async void RecalculateBoundsLater()
	{
		await Task.Delay(100);
		this.RecalculateBounds();
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x00019498 File Offset: 0x00017698
	private void RecalculateBounds()
	{
		this._initialPosition = base.transform.position;
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		this._bounds = default(Bounds);
		if (componentsInChildren.Length == 0)
		{
			this._bounds.center = base.transform.position;
			this._bounds.Expand(1f);
			return;
		}
		this._bounds = componentsInChildren[0].bounds;
		for (int i = 1; i < componentsInChildren.Length; i++)
		{
			this._bounds.Encapsulate(componentsInChildren[i].bounds);
		}
	}

	// Token: 0x040004C2 RID: 1218
	[SerializeField]
	private KeyValuePairSet data;

	// Token: 0x040004C3 RID: 1219
	private Bounds _bounds;

	// Token: 0x040004C4 RID: 1220
	private Vector3 _initialPosition;
}
