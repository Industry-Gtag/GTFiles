using System;
using UnityEngine;

// Token: 0x02000927 RID: 2343
public class SizeChangerTrigger : MonoBehaviour, IBuilderPieceComponent
{
	// Token: 0x14000071 RID: 113
	// (add) Token: 0x06003D61 RID: 15713 RVA: 0x0014D6E0 File Offset: 0x0014B8E0
	// (remove) Token: 0x06003D62 RID: 15714 RVA: 0x0014D718 File Offset: 0x0014B918
	public event SizeChangerTrigger.SizeChangerTriggerEvent OnEnter;

	// Token: 0x14000072 RID: 114
	// (add) Token: 0x06003D63 RID: 15715 RVA: 0x0014D750 File Offset: 0x0014B950
	// (remove) Token: 0x06003D64 RID: 15716 RVA: 0x0014D788 File Offset: 0x0014B988
	public event SizeChangerTrigger.SizeChangerTriggerEvent OnExit;

	// Token: 0x06003D65 RID: 15717 RVA: 0x0014D7BD File Offset: 0x0014B9BD
	private void Awake()
	{
		this.myCollider = base.GetComponent<Collider>();
	}

	// Token: 0x06003D66 RID: 15718 RVA: 0x0014D7CB File Offset: 0x0014B9CB
	public void OnTriggerEnter(Collider other)
	{
		if (this.OnEnter != null)
		{
			this.OnEnter(other);
		}
	}

	// Token: 0x06003D67 RID: 15719 RVA: 0x0014D7E1 File Offset: 0x0014B9E1
	public void OnTriggerExit(Collider other)
	{
		if (this.OnExit != null)
		{
			this.OnExit(other);
		}
	}

	// Token: 0x06003D68 RID: 15720 RVA: 0x0014D7F7 File Offset: 0x0014B9F7
	public Vector3 ClosestPoint(Vector3 position)
	{
		return this.myCollider.ClosestPoint(position);
	}

	// Token: 0x06003D69 RID: 15721 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPieceCreate(int pieceType, int pieceId)
	{
	}

	// Token: 0x06003D6A RID: 15722 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPieceDestroy()
	{
	}

	// Token: 0x06003D6B RID: 15723 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPiecePlacementDeserialized()
	{
	}

	// Token: 0x06003D6C RID: 15724 RVA: 0x0014D805 File Offset: 0x0014BA05
	public void OnPieceActivate()
	{
		Debug.LogError("Size Trigger Pieces no longer work, need reimplementation");
	}

	// Token: 0x06003D6D RID: 15725 RVA: 0x0014D805 File Offset: 0x0014BA05
	public void OnPieceDeactivate()
	{
		Debug.LogError("Size Trigger Pieces no longer work, need reimplementation");
	}

	// Token: 0x04004E2E RID: 20014
	private Collider myCollider;

	// Token: 0x04004E31 RID: 20017
	public bool builderEnterTrigger;

	// Token: 0x04004E32 RID: 20018
	public bool builderExitOnEnterTrigger;

	// Token: 0x02000928 RID: 2344
	// (Invoke) Token: 0x06003D70 RID: 15728
	public delegate void SizeChangerTriggerEvent(Collider other);
}
