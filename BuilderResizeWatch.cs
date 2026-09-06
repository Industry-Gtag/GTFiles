using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000656 RID: 1622
public class BuilderResizeWatch : MonoBehaviour
{
	// Token: 0x1700040D RID: 1037
	// (get) Token: 0x06002884 RID: 10372 RVA: 0x000DAE48 File Offset: 0x000D9048
	public int SizeLayerMaskGrow
	{
		get
		{
			int num = 0;
			if (this.growSettings.affectLayerA)
			{
				num |= 1;
			}
			if (this.growSettings.affectLayerB)
			{
				num |= 2;
			}
			if (this.growSettings.affectLayerC)
			{
				num |= 4;
			}
			if (this.growSettings.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x1700040E RID: 1038
	// (get) Token: 0x06002885 RID: 10373 RVA: 0x000DAE9C File Offset: 0x000D909C
	public int SizeLayerMaskShrink
	{
		get
		{
			int num = 0;
			if (this.shrinkSettings.affectLayerA)
			{
				num |= 1;
			}
			if (this.shrinkSettings.affectLayerB)
			{
				num |= 2;
			}
			if (this.shrinkSettings.affectLayerC)
			{
				num |= 4;
			}
			if (this.shrinkSettings.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x06002886 RID: 10374 RVA: 0x000DAEF0 File Offset: 0x000D90F0
	private void Start()
	{
		if (this.enlargeButton != null)
		{
			this.enlargeButton.onPressButton.AddListener(new UnityAction(this.OnEnlargeButtonPressed));
		}
		if (this.shrinkButton != null)
		{
			this.shrinkButton.onPressButton.AddListener(new UnityAction(this.OnShrinkButtonPressed));
		}
		this.ownerRig = base.GetComponentInParent<VRRig>();
		this.enableDist = GTPlayer.Instance.bodyCollider.height;
		this.enableDistSq = this.enableDist * this.enableDist;
	}

	// Token: 0x06002887 RID: 10375 RVA: 0x000DAF88 File Offset: 0x000D9188
	private void OnDestroy()
	{
		if (this.enlargeButton != null)
		{
			this.enlargeButton.onPressButton.RemoveListener(new UnityAction(this.OnEnlargeButtonPressed));
		}
		if (this.shrinkButton != null)
		{
			this.shrinkButton.onPressButton.RemoveListener(new UnityAction(this.OnShrinkButtonPressed));
		}
	}

	// Token: 0x06002888 RID: 10376 RVA: 0x000DAFEC File Offset: 0x000D91EC
	private void OnEnlargeButtonPressed()
	{
		if (this.sizeManager == null)
		{
			if (this.ownerRig == null)
			{
				Debug.LogWarning("Builder resize watch has no owner rig");
				return;
			}
			this.sizeManager = this.ownerRig.sizeManager;
		}
		if (this.sizeManager != null && this.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMaskGrow && !this.updateCollision)
		{
			this.DisableCollisionWithPieces();
			this.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMaskGrow;
			if (this.fxForLayerChange != null)
			{
				ObjectPools.instance.Instantiate(this.fxForLayerChange, this.ownerRig.transform.position, true);
			}
			this.timeToCheckCollision = (double)(Time.time + this.growDelay);
			this.updateCollision = true;
		}
	}

	// Token: 0x06002889 RID: 10377 RVA: 0x000DB0BC File Offset: 0x000D92BC
	private void DisableCollisionWithPieces()
	{
		BuilderTable builderTable;
		if (!BuilderTable.TryGetBuilderTableForZone(this.ownerRig.zoneEntity.currentZone, out builderTable))
		{
			return;
		}
		int num = Physics.OverlapSphereNonAlloc(GTPlayer.Instance.headCollider.transform.position, 1f, this.tempDisableColliders, builderTable.allPiecesMask);
		for (int i = 0; i < num; i++)
		{
			BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(this.tempDisableColliders[i]);
			if (builderPieceFromCollider != null && builderPieceFromCollider.state == BuilderPiece.State.AttachedAndPlaced && !builderPieceFromCollider.isBuiltIntoTable && !this.collisionDisabledPieces.Contains(builderPieceFromCollider))
			{
				foreach (Collider collider in builderPieceFromCollider.colliders)
				{
					collider.enabled = false;
				}
				foreach (Collider collider2 in builderPieceFromCollider.placedOnlyColliders)
				{
					collider2.enabled = false;
				}
				this.collisionDisabledPieces.Add(builderPieceFromCollider);
			}
		}
	}

	// Token: 0x0600288A RID: 10378 RVA: 0x000DB1F8 File Offset: 0x000D93F8
	private void EnableCollisionWithPieces()
	{
		for (int i = this.collisionDisabledPieces.Count - 1; i >= 0; i--)
		{
			BuilderPiece builderPiece = this.collisionDisabledPieces[i];
			if (builderPiece == null)
			{
				this.collisionDisabledPieces.RemoveAt(i);
			}
			else if (Vector3.SqrMagnitude(GTPlayer.Instance.bodyCollider.transform.position - builderPiece.transform.position) >= this.enableDistSq)
			{
				this.EnableCollisionWithPiece(builderPiece);
				this.collisionDisabledPieces.RemoveAt(i);
			}
		}
	}

	// Token: 0x0600288B RID: 10379 RVA: 0x000DB288 File Offset: 0x000D9488
	private void EnableCollisionWithPiece(BuilderPiece piece)
	{
		foreach (Collider collider in piece.colliders)
		{
			collider.enabled = piece.state != BuilderPiece.State.None && piece.state != BuilderPiece.State.Displayed;
		}
		foreach (Collider collider2 in piece.placedOnlyColliders)
		{
			collider2.enabled = piece.state == BuilderPiece.State.AttachedAndPlaced;
		}
	}

	// Token: 0x0600288C RID: 10380 RVA: 0x000DB338 File Offset: 0x000D9538
	private void Update()
	{
		if (this.updateCollision && (double)Time.time >= this.timeToCheckCollision)
		{
			this.EnableCollisionWithPieces();
			if (this.collisionDisabledPieces.Count <= 0)
			{
				this.updateCollision = false;
			}
		}
	}

	// Token: 0x0600288D RID: 10381 RVA: 0x000DB36C File Offset: 0x000D956C
	private void OnShrinkButtonPressed()
	{
		if (this.sizeManager == null)
		{
			if (this.ownerRig == null)
			{
				Debug.LogWarning("Builder resize watch has no owner rig");
			}
			this.sizeManager = this.ownerRig.sizeManager;
		}
		if (this.sizeManager != null && this.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMaskShrink)
		{
			this.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMaskShrink;
		}
	}

	// Token: 0x040034B7 RID: 13495
	[SerializeField]
	private HeldButton enlargeButton;

	// Token: 0x040034B8 RID: 13496
	[SerializeField]
	private HeldButton shrinkButton;

	// Token: 0x040034B9 RID: 13497
	[SerializeField]
	private GameObject fxForLayerChange;

	// Token: 0x040034BA RID: 13498
	private VRRig ownerRig;

	// Token: 0x040034BB RID: 13499
	private SizeManager sizeManager;

	// Token: 0x040034BC RID: 13500
	[HideInInspector]
	public Collider[] tempDisableColliders = new Collider[128];

	// Token: 0x040034BD RID: 13501
	[HideInInspector]
	public List<BuilderPiece> collisionDisabledPieces = new List<BuilderPiece>();

	// Token: 0x040034BE RID: 13502
	private float enableDist = 1f;

	// Token: 0x040034BF RID: 13503
	private float enableDistSq = 1f;

	// Token: 0x040034C0 RID: 13504
	private bool updateCollision;

	// Token: 0x040034C1 RID: 13505
	private float growDelay = 1f;

	// Token: 0x040034C2 RID: 13506
	private double timeToCheckCollision;

	// Token: 0x040034C3 RID: 13507
	public BuilderResizeWatch.BuilderSizeChangeSettings growSettings;

	// Token: 0x040034C4 RID: 13508
	public BuilderResizeWatch.BuilderSizeChangeSettings shrinkSettings;

	// Token: 0x02000657 RID: 1623
	[Serializable]
	public struct BuilderSizeChangeSettings
	{
		// Token: 0x040034C5 RID: 13509
		public bool affectLayerA;

		// Token: 0x040034C6 RID: 13510
		public bool affectLayerB;

		// Token: 0x040034C7 RID: 13511
		public bool affectLayerC;

		// Token: 0x040034C8 RID: 13512
		public bool affectLayerD;
	}
}
