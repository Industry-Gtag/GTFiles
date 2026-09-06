using System;
using UnityEngine;

// Token: 0x02000A02 RID: 2562
public class TappableDent : Tappable
{
	// Token: 0x060041C9 RID: 16841 RVA: 0x0015E9E0 File Offset: 0x0015CBE0
	private void Start()
	{
		if (this.parent == null)
		{
			this.parent = base.gameObject;
		}
		this.offsetPerTap = base.transform.parent.InverseTransformVector(base.transform.TransformVector(this.finalLocalOffset / (float)this.numTapsToDestroy));
		this.scaleOffsetPerTap = (this.finalLocalScale - base.transform.localScale) / (float)this.numTapsToDestroy;
	}

	// Token: 0x060041CA RID: 16842 RVA: 0x0015EA64 File Offset: 0x0015CC64
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		if (this.numTapsSoFar > this.numTapsToDestroy)
		{
			return;
		}
		this.numTapsSoFar++;
		if (this.numTapsSoFar >= this.numTapsToDestroy)
		{
			this.parent.SetActive(false);
			return;
		}
		base.transform.localPosition += this.offsetPerTap;
		base.transform.localScale += this.scaleOffsetPerTap;
	}

	// Token: 0x060041CB RID: 16843 RVA: 0x0015EAE4 File Offset: 0x0015CCE4
	public void ChangeNumTapsToDestroy(int i)
	{
		float num = (float)this.numTapsSoFar / (float)this.numTapsToDestroy;
		this.numTapsToDestroy = i;
		this.numTapsSoFar = Mathf.RoundToInt((float)this.numTapsToDestroy * num);
	}

	// Token: 0x04005271 RID: 21105
	[SerializeField]
	private int numTapsToDestroy = 3;

	// Token: 0x04005272 RID: 21106
	[SerializeField]
	private Vector3 finalLocalOffset;

	// Token: 0x04005273 RID: 21107
	[SerializeField]
	private Vector3 finalLocalScale;

	// Token: 0x04005274 RID: 21108
	[SerializeField]
	private GameObject parent;

	// Token: 0x04005275 RID: 21109
	private int numTapsSoFar;

	// Token: 0x04005276 RID: 21110
	private Vector3 offsetPerTap;

	// Token: 0x04005277 RID: 21111
	private Vector3 scaleOffsetPerTap;
}
