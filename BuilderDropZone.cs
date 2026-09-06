using System;
using System.Collections;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000630 RID: 1584
public class BuilderDropZone : MonoBehaviour
{
	// Token: 0x06002786 RID: 10118 RVA: 0x000D1462 File Offset: 0x000CF662
	private void Awake()
	{
		this.repelDirectionWorld = base.transform.TransformDirection(this.repelDirectionLocal.normalized);
	}

	// Token: 0x06002787 RID: 10119 RVA: 0x000D1480 File Offset: 0x000CF680
	private void OnTriggerEnter(Collider other)
	{
		if (!this.onEnter)
		{
			return;
		}
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		BuilderPieceCollider component = other.GetComponent<BuilderPieceCollider>();
		if (component != null)
		{
			BuilderPiece piece = component.piece;
			if (this.table != null && this.table.builderNetworking != null)
			{
				if (piece == null)
				{
					return;
				}
				if (this.dropType == BuilderDropZone.DropType.Recycle)
				{
					bool flag = piece.state != BuilderPiece.State.Displayed && piece.state != BuilderPiece.State.OnShelf && piece.state > BuilderPiece.State.AttachedAndPlaced;
					if (!piece.isBuiltIntoTable && flag)
					{
						this.table.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, true, -1);
						return;
					}
				}
				else
				{
					this.table.builderNetworking.PieceEnteredDropZone(piece, this.dropType, this.dropZoneID);
				}
			}
		}
	}

	// Token: 0x06002788 RID: 10120 RVA: 0x000D156A File Offset: 0x000CF76A
	public Vector3 GetRepelDirectionWorld()
	{
		return this.repelDirectionWorld;
	}

	// Token: 0x06002789 RID: 10121 RVA: 0x000D1574 File Offset: 0x000CF774
	public void PlayEffect()
	{
		if (this.vfxRoot != null && !this.playingEffect)
		{
			this.vfxRoot.SetActive(true);
			this.playingEffect = true;
			if (this.sfxPrefab != null)
			{
				ObjectPools.instance.Instantiate(this.sfxPrefab, base.transform.position, base.transform.rotation, true);
			}
			base.StartCoroutine(this.DelayedStopEffect());
		}
	}

	// Token: 0x0600278A RID: 10122 RVA: 0x000D15ED File Offset: 0x000CF7ED
	private IEnumerator DelayedStopEffect()
	{
		yield return new WaitForSeconds(this.effectDuration);
		this.vfxRoot.SetActive(false);
		this.playingEffect = false;
		yield break;
	}

	// Token: 0x0600278B RID: 10123 RVA: 0x000D15FC File Offset: 0x000CF7FC
	private void OnTriggerExit(Collider other)
	{
		if (this.onEnter)
		{
			return;
		}
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		BuilderPieceCollider component = other.GetComponent<BuilderPieceCollider>();
		if (component != null)
		{
			BuilderPiece piece = component.piece;
			if (this.table != null && this.table.builderNetworking != null)
			{
				if (piece == null)
				{
					return;
				}
				if (this.dropType == BuilderDropZone.DropType.Recycle)
				{
					bool flag = piece.state != BuilderPiece.State.Displayed && piece.state != BuilderPiece.State.OnShelf && piece.state > BuilderPiece.State.AttachedAndPlaced;
					if (!piece.isBuiltIntoTable && flag)
					{
						this.table.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, true, -1);
						return;
					}
				}
				else
				{
					this.table.builderNetworking.PieceEnteredDropZone(piece, this.dropType, this.dropZoneID);
				}
			}
		}
	}

	// Token: 0x0400332D RID: 13101
	[SerializeField]
	private BuilderDropZone.DropType dropType;

	// Token: 0x0400332E RID: 13102
	[SerializeField]
	private bool onEnter = true;

	// Token: 0x0400332F RID: 13103
	[SerializeField]
	private GameObject vfxRoot;

	// Token: 0x04003330 RID: 13104
	[SerializeField]
	private GameObject sfxPrefab;

	// Token: 0x04003331 RID: 13105
	public float effectDuration = 1f;

	// Token: 0x04003332 RID: 13106
	private bool playingEffect;

	// Token: 0x04003333 RID: 13107
	public bool overrideDirection;

	// Token: 0x04003334 RID: 13108
	[SerializeField]
	private Vector3 repelDirectionLocal = Vector3.up;

	// Token: 0x04003335 RID: 13109
	private Vector3 repelDirectionWorld = Vector3.up;

	// Token: 0x04003336 RID: 13110
	[HideInInspector]
	public int dropZoneID = -1;

	// Token: 0x04003337 RID: 13111
	internal BuilderTable table;

	// Token: 0x02000631 RID: 1585
	public enum DropType
	{
		// Token: 0x04003339 RID: 13113
		Invalid = -1,
		// Token: 0x0400333A RID: 13114
		Repel,
		// Token: 0x0400333B RID: 13115
		ReturnToShelf,
		// Token: 0x0400333C RID: 13116
		BreakApart,
		// Token: 0x0400333D RID: 13117
		Recycle
	}
}
