using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200066C RID: 1644
public class BuilderShelf : MonoBehaviour
{
	// Token: 0x06002903 RID: 10499 RVA: 0x000DE8F8 File Offset: 0x000DCAF8
	public void Init()
	{
		this.shelfSlot = 0;
		this.buildPieceSpawnIndex = 0;
		this.spawnCount = 0;
		this.count = 0;
		this.spawnCosts = new List<BuilderResources>(this.buildPieceSpawns.Count);
		for (int i = 0; i < this.buildPieceSpawns.Count; i++)
		{
			this.count += this.buildPieceSpawns[i].count;
			BuilderPiece component = this.buildPieceSpawns[i].buildPiecePrefab.GetComponent<BuilderPiece>();
			this.spawnCosts.Add(component.cost);
		}
	}

	// Token: 0x06002904 RID: 10500 RVA: 0x000DE993 File Offset: 0x000DCB93
	public bool HasOpenSlot()
	{
		return this.shelfSlot < this.count;
	}

	// Token: 0x06002905 RID: 10501 RVA: 0x000DE9A4 File Offset: 0x000DCBA4
	public void BuildNextPiece(BuilderTable table)
	{
		if (!this.HasOpenSlot())
		{
			return;
		}
		BuilderShelf.BuildPieceSpawn buildPieceSpawn = this.buildPieceSpawns[this.buildPieceSpawnIndex];
		BuilderResources builderResources = this.spawnCosts[this.buildPieceSpawnIndex];
		while (!table.HasEnoughUnreservedResources(builderResources) && this.buildPieceSpawnIndex < this.buildPieceSpawns.Count - 1)
		{
			int num = buildPieceSpawn.count - this.spawnCount;
			this.shelfSlot += num;
			this.spawnCount = 0;
			this.buildPieceSpawnIndex++;
			buildPieceSpawn = this.buildPieceSpawns[this.buildPieceSpawnIndex];
			builderResources = this.spawnCosts[this.buildPieceSpawnIndex];
		}
		if (!table.HasEnoughUnreservedResources(builderResources))
		{
			int num2 = buildPieceSpawn.count - this.spawnCount;
			this.shelfSlot += num2;
			this.spawnCount = 0;
			return;
		}
		int staticHash = buildPieceSpawn.buildPiecePrefab.name.GetStaticHash();
		int num3 = (string.IsNullOrEmpty(buildPieceSpawn.materialID) ? (-1) : buildPieceSpawn.materialID.GetHashCode());
		Vector3 vector;
		Quaternion quaternion;
		this.GetSpawnLocation(this.shelfSlot, buildPieceSpawn, out vector, out quaternion);
		int num4 = table.CreatePieceId();
		table.CreatePiece(staticHash, num4, vector, quaternion, num3, BuilderPiece.State.OnShelf, PhotonNetwork.LocalPlayer);
		this.spawnCount++;
		this.shelfSlot++;
		if (this.spawnCount >= buildPieceSpawn.count)
		{
			this.buildPieceSpawnIndex++;
			this.spawnCount = 0;
		}
	}

	// Token: 0x06002906 RID: 10502 RVA: 0x000DEB20 File Offset: 0x000DCD20
	public void InitCount()
	{
		this.count = 0;
		for (int i = 0; i < this.buildPieceSpawns.Count; i++)
		{
			this.count += this.buildPieceSpawns[i].count;
		}
	}

	// Token: 0x06002907 RID: 10503 RVA: 0x000DEB68 File Offset: 0x000DCD68
	public void BuildItems(BuilderTable table)
	{
		int num = 0;
		this.InitCount();
		for (int i = 0; i < this.buildPieceSpawns.Count; i++)
		{
			BuilderShelf.BuildPieceSpawn buildPieceSpawn = this.buildPieceSpawns[i];
			if (buildPieceSpawn != null && buildPieceSpawn.count != 0)
			{
				int staticHash = buildPieceSpawn.buildPiecePrefab.name.GetStaticHash();
				int num2 = (string.IsNullOrEmpty(buildPieceSpawn.materialID) ? (-1) : buildPieceSpawn.materialID.GetHashCode());
				int num3 = 0;
				while (num3 < buildPieceSpawn.count && num < this.count)
				{
					Vector3 vector;
					Quaternion quaternion;
					this.GetSpawnLocation(num, buildPieceSpawn, out vector, out quaternion);
					int num4 = table.CreatePieceId();
					table.CreatePiece(staticHash, num4, vector, quaternion, num2, BuilderPiece.State.OnShelf, PhotonNetwork.LocalPlayer);
					num++;
					num3++;
				}
			}
		}
	}

	// Token: 0x06002908 RID: 10504 RVA: 0x000DEC34 File Offset: 0x000DCE34
	public void GetSpawnLocation(int slot, BuilderShelf.BuildPieceSpawn spawn, out Vector3 spawnPosition, out Quaternion spawnRotation)
	{
		if (this.center == null)
		{
			this.center = base.transform;
		}
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		BuilderPiece component = spawn.buildPiecePrefab.GetComponent<BuilderPiece>();
		if (component != null)
		{
			vector = component.desiredShelfOffset;
			vector2 = component.desiredShelfRotationOffset;
		}
		spawnRotation = this.center.rotation * Quaternion.Euler(vector2);
		float num = (float)slot * this.separation - (float)(this.count - 1) * this.separation / 2f;
		spawnPosition = this.center.position + this.center.rotation * (spawn.localAxis * num + vector);
	}

	// Token: 0x04003569 RID: 13673
	private int count;

	// Token: 0x0400356A RID: 13674
	public float separation;

	// Token: 0x0400356B RID: 13675
	public Transform center;

	// Token: 0x0400356C RID: 13676
	public List<BuilderShelf.BuildPieceSpawn> buildPieceSpawns;

	// Token: 0x0400356D RID: 13677
	private List<BuilderResources> spawnCosts;

	// Token: 0x0400356E RID: 13678
	private int shelfSlot;

	// Token: 0x0400356F RID: 13679
	private int buildPieceSpawnIndex;

	// Token: 0x04003570 RID: 13680
	private int spawnCount;

	// Token: 0x0200066D RID: 1645
	[Serializable]
	public class BuildPieceSpawn
	{
		// Token: 0x04003571 RID: 13681
		public GameObject buildPiecePrefab;

		// Token: 0x04003572 RID: 13682
		public string materialID;

		// Token: 0x04003573 RID: 13683
		public int count = 1;

		// Token: 0x04003574 RID: 13684
		public Vector3 localAxis = Vector3.right;

		// Token: 0x04003575 RID: 13685
		[Tooltip("Optional Editor Visual")]
		public Mesh previewMesh;
	}
}
