using System;
using UnityEngine;

// Token: 0x020005AD RID: 1453
public class SyncToPlayerColor : MonoBehaviour
{
	// Token: 0x060024D0 RID: 9424 RVA: 0x000C5EC5 File Offset: 0x000C40C5
	protected virtual void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this._colorFunc = new Action<Color>(this.UpdateColor);
	}

	// Token: 0x060024D1 RID: 9425 RVA: 0x000C5EE6 File Offset: 0x000C40E6
	protected virtual void Start()
	{
		this.UpdateColor(this.rig.playerColor);
		this.rig.OnColorInitialized(this._colorFunc);
	}

	// Token: 0x060024D2 RID: 9426 RVA: 0x000C5F0A File Offset: 0x000C410A
	protected virtual void OnEnable()
	{
		this.rig.OnColorChanged += this._colorFunc;
	}

	// Token: 0x060024D3 RID: 9427 RVA: 0x000C5F1D File Offset: 0x000C411D
	protected virtual void OnDisable()
	{
		this.rig.OnColorChanged -= this._colorFunc;
	}

	// Token: 0x060024D4 RID: 9428 RVA: 0x000C5F30 File Offset: 0x000C4130
	public virtual void UpdateColor(Color color)
	{
		if (!this.target)
		{
			return;
		}
		if (this.colorPropertiesToSync == null)
		{
			return;
		}
		for (int i = 0; i < this.colorPropertiesToSync.Length; i++)
		{
			ShaderHashId shaderHashId = this.colorPropertiesToSync[i];
			this.target.SetColor(shaderHashId, color);
		}
	}

	// Token: 0x0400305B RID: 12379
	public VRRig rig;

	// Token: 0x0400305C RID: 12380
	public Material target;

	// Token: 0x0400305D RID: 12381
	public ShaderHashId[] colorPropertiesToSync = new ShaderHashId[] { "_BaseColor" };

	// Token: 0x0400305E RID: 12382
	private Action<Color> _colorFunc;
}
