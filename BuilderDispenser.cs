using System;
using System.Collections;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200062D RID: 1581
public class BuilderDispenser : MonoBehaviour
{
	// Token: 0x06002761 RID: 10081 RVA: 0x000D042C File Offset: 0x000CE62C
	private void Awake()
	{
		this.nullPiece = new BuilderPieceSet.PieceInfo
		{
			piecePrefab = null,
			overrideSetMaterial = false
		};
	}

	// Token: 0x06002762 RID: 10082 RVA: 0x000D0458 File Offset: 0x000CE658
	public void UpdateDispenser()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!this.hasPiece && Time.timeAsDouble > this.nextSpawnTime && this.pieceToSpawn.piecePrefab != null)
		{
			this.TrySpawnPiece();
			this.nextSpawnTime = Time.timeAsDouble + (double)this.spawnRetryDelay;
			return;
		}
		if (this.hasPiece && (this.spawnedPieceInstance == null || (this.spawnedPieceInstance.state != BuilderPiece.State.OnShelf && this.spawnedPieceInstance.state != BuilderPiece.State.Displayed)))
		{
			base.StopAllCoroutines();
			if (this.spawnedPieceInstance != null)
			{
				this.spawnedPieceInstance.shelfOwner = -1;
			}
			this.nextSpawnTime = Time.timeAsDouble + (double)this.OnGrabSpawnDelay;
			this.spawnedPieceInstance = null;
			this.hasPiece = false;
		}
	}

	// Token: 0x06002763 RID: 10083 RVA: 0x000D0524 File Offset: 0x000CE724
	public bool DoesPieceMatchSpawnInfo(BuilderPiece piece)
	{
		if (piece == null || this.pieceToSpawn.piecePrefab == null)
		{
			return false;
		}
		if (piece.pieceType != this.pieceToSpawn.piecePrefab.name.GetStaticHash())
		{
			return false;
		}
		if (!(piece.materialOptions != null))
		{
			return true;
		}
		int num = piece.materialType;
		int num2;
		Material material;
		int num3;
		piece.materialOptions.GetDefaultMaterial(out num2, out material, out num3);
		if (this.pieceToSpawn.overrideSetMaterial)
		{
			for (int i = 0; i < this.pieceToSpawn.pieceMaterialTypes.Length; i++)
			{
				string text = this.pieceToSpawn.pieceMaterialTypes[i];
				if (!string.IsNullOrEmpty(text))
				{
					int hashCode = text.GetHashCode();
					if (hashCode == num)
					{
						return true;
					}
					if (hashCode == num2 && num == -1)
					{
						return true;
					}
				}
				else if (num == -1 || num == num2)
				{
					return true;
				}
			}
		}
		else if (num == this.materialType || (this.materialType == num2 && num == -1) || (num == num2 && this.materialType == -1))
		{
			return true;
		}
		return false;
	}

	// Token: 0x06002764 RID: 10084 RVA: 0x000D0628 File Offset: 0x000CE828
	public void ShelfPieceCreated(BuilderPiece piece, bool playAnimation)
	{
		if (this.DoesPieceMatchSpawnInfo(piece))
		{
			if (this.hasPiece && this.spawnedPieceInstance != null)
			{
				this.spawnedPieceInstance.shelfOwner = -1;
			}
			this.spawnedPieceInstance = piece;
			this.hasPiece = true;
			this.spawnCount++;
			this.spawnCount = Mathf.Max(0, this.spawnCount);
			if (this.table.GetTableState() == BuilderTable.TableState.Ready && playAnimation)
			{
				base.StartCoroutine(this.PlayAnimation());
				if (this.playFX)
				{
					ObjectPools.instance.Instantiate(this.dispenserFX, this.spawnTransform.position, this.spawnTransform.rotation, true);
					return;
				}
				this.playFX = true;
				return;
			}
			else
			{
				Vector3 desiredShelfOffset = this.pieceToSpawn.piecePrefab.desiredShelfOffset;
				Vector3 vector = this.displayTransform.position + this.displayTransform.rotation * desiredShelfOffset;
				Quaternion quaternion = this.displayTransform.rotation * Quaternion.Euler(this.pieceToSpawn.piecePrefab.desiredShelfRotationOffset);
				this.spawnedPieceInstance.transform.SetPositionAndRotation(vector, quaternion);
				this.spawnedPieceInstance.SetState(BuilderPiece.State.OnShelf, false);
				this.playFX = true;
			}
		}
	}

	// Token: 0x06002765 RID: 10085 RVA: 0x000D0768 File Offset: 0x000CE968
	private IEnumerator PlayAnimation()
	{
		this.spawnedPieceInstance.SetState(BuilderPiece.State.Displayed, false);
		this.animateParent.Rewind();
		this.spawnedPieceInstance.transform.SetParent(this.animateParent.transform);
		this.spawnedPieceInstance.transform.SetLocalPositionAndRotation(this.pieceToSpawn.piecePrefab.desiredShelfOffset, Quaternion.Euler(this.pieceToSpawn.piecePrefab.desiredShelfRotationOffset));
		this.animateParent.Play();
		yield return new WaitForSeconds(this.animateParent.clip.length);
		if (this.spawnedPieceInstance != null && this.spawnedPieceInstance.state == BuilderPiece.State.Displayed)
		{
			this.spawnedPieceInstance.transform.SetParent(null);
			Vector3 desiredShelfOffset = this.pieceToSpawn.piecePrefab.desiredShelfOffset;
			Vector3 vector = this.displayTransform.position + this.displayTransform.rotation * desiredShelfOffset;
			Quaternion quaternion = this.displayTransform.rotation * Quaternion.Euler(this.pieceToSpawn.piecePrefab.desiredShelfRotationOffset);
			this.spawnedPieceInstance.transform.SetPositionAndRotation(vector, quaternion);
			this.spawnedPieceInstance.SetState(BuilderPiece.State.OnShelf, false);
		}
		yield break;
	}

	// Token: 0x06002766 RID: 10086 RVA: 0x000D0778 File Offset: 0x000CE978
	public void ShelfPieceRecycled(BuilderPiece piece)
	{
		if (piece != null && this.spawnedPieceInstance != null && piece.Equals(this.spawnedPieceInstance))
		{
			piece.shelfOwner = -1;
			this.spawnedPieceInstance = null;
			this.hasPiece = false;
			this.nextSpawnTime = Time.timeAsDouble + (double)this.OnGrabSpawnDelay;
		}
	}

	// Token: 0x06002767 RID: 10087 RVA: 0x000D07D4 File Offset: 0x000CE9D4
	public void AssignPieceType(BuilderPieceSet.PieceInfo piece, int inMaterialType)
	{
		this.playFX = false;
		this.pieceToSpawn = piece;
		this.materialType = inMaterialType;
		this.nextSpawnTime = Time.timeAsDouble + (double)this.OnGrabSpawnDelay;
		this.currentAnimation = this.dispenseDefaultAnimation;
		this.animateParent.clip = this.currentAnimation;
		this.spawnCount = 0;
	}

	// Token: 0x06002768 RID: 10088 RVA: 0x000D0830 File Offset: 0x000CEA30
	private void TrySpawnPiece()
	{
		if (this.spawnedPieceInstance != null && this.spawnedPieceInstance.state == BuilderPiece.State.OnShelf)
		{
			return;
		}
		if (this.pieceToSpawn.piecePrefab == null)
		{
			return;
		}
		if (this.table.HasEnoughResources(this.pieceToSpawn.piecePrefab))
		{
			Vector3 desiredShelfOffset = this.pieceToSpawn.piecePrefab.desiredShelfOffset;
			Vector3 vector = this.spawnTransform.position + this.spawnTransform.rotation * desiredShelfOffset;
			Quaternion quaternion = this.spawnTransform.rotation * Quaternion.Euler(this.pieceToSpawn.piecePrefab.desiredShelfRotationOffset);
			int num = this.materialType;
			if (this.pieceToSpawn.overrideSetMaterial && this.pieceToSpawn.pieceMaterialTypes.Length != 0)
			{
				int num2 = this.spawnCount % this.pieceToSpawn.pieceMaterialTypes.Length;
				string text = this.pieceToSpawn.pieceMaterialTypes[num2];
				if (string.IsNullOrEmpty(text))
				{
					num = -1;
				}
				else
				{
					num = text.GetHashCode();
				}
			}
			this.table.RequestCreateDispenserShelfPiece(this.pieceToSpawn.piecePrefab.name.GetStaticHash(), vector, quaternion, num, this.shelfID);
		}
	}

	// Token: 0x06002769 RID: 10089 RVA: 0x000D0968 File Offset: 0x000CEB68
	public void ParentPieceToShelf(Transform shelfTransform)
	{
		if (this.spawnedPieceInstance != null)
		{
			if (this.spawnedPieceInstance.state != BuilderPiece.State.OnShelf && this.spawnedPieceInstance.state != BuilderPiece.State.Displayed)
			{
				base.StopAllCoroutines();
				if (this.spawnedPieceInstance != null)
				{
					this.spawnedPieceInstance.shelfOwner = -1;
				}
				this.nextSpawnTime = Time.timeAsDouble + (double)this.OnGrabSpawnDelay;
				this.spawnedPieceInstance = null;
				this.hasPiece = false;
				return;
			}
			this.spawnedPieceInstance.SetState(BuilderPiece.State.Displayed, false);
			this.spawnedPieceInstance.transform.SetParent(shelfTransform);
		}
	}

	// Token: 0x0600276A RID: 10090 RVA: 0x000D0A00 File Offset: 0x000CEC00
	public void ClearDispenser()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.pieceToSpawn = this.nullPiece;
		this.hasPiece = false;
		if (this.spawnedPieceInstance != null)
		{
			if (this.spawnedPieceInstance.state != BuilderPiece.State.OnShelf && this.spawnedPieceInstance.state != BuilderPiece.State.Displayed)
			{
				this.spawnedPieceInstance.shelfOwner = -1;
				this.nextSpawnTime = Time.timeAsDouble + (double)this.OnGrabSpawnDelay;
				this.spawnedPieceInstance = null;
				return;
			}
			this.table.RequestRecyclePiece(this.spawnedPieceInstance, false, -1);
		}
	}

	// Token: 0x0600276B RID: 10091 RVA: 0x000D0A8C File Offset: 0x000CEC8C
	public void OnClearTable()
	{
		this.playFX = false;
		this.nextSpawnTime = 0.0;
		this.hasPiece = false;
		this.spawnedPieceInstance = null;
	}

	// Token: 0x04003302 RID: 13058
	public Transform displayTransform;

	// Token: 0x04003303 RID: 13059
	public Transform spawnTransform;

	// Token: 0x04003304 RID: 13060
	public Animation animateParent;

	// Token: 0x04003305 RID: 13061
	public AnimationClip dispenseDefaultAnimation;

	// Token: 0x04003306 RID: 13062
	public GameObject dispenserFX;

	// Token: 0x04003307 RID: 13063
	private AnimationClip currentAnimation;

	// Token: 0x04003308 RID: 13064
	[HideInInspector]
	public BuilderTable table;

	// Token: 0x04003309 RID: 13065
	[HideInInspector]
	public int shelfID;

	// Token: 0x0400330A RID: 13066
	private BuilderPieceSet.PieceInfo pieceToSpawn;

	// Token: 0x0400330B RID: 13067
	private BuilderPiece spawnedPieceInstance;

	// Token: 0x0400330C RID: 13068
	private int materialType = -1;

	// Token: 0x0400330D RID: 13069
	private BuilderPieceSet.PieceInfo nullPiece;

	// Token: 0x0400330E RID: 13070
	private int spawnCount;

	// Token: 0x0400330F RID: 13071
	private double nextSpawnTime;

	// Token: 0x04003310 RID: 13072
	private bool hasPiece;

	// Token: 0x04003311 RID: 13073
	private float OnGrabSpawnDelay = 0.5f;

	// Token: 0x04003312 RID: 13074
	private float spawnRetryDelay = 2f;

	// Token: 0x04003313 RID: 13075
	private bool playFX;
}
