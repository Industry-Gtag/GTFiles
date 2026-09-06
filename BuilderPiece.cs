using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000642 RID: 1602
public class BuilderPiece : MonoBehaviour
{
	// Token: 0x060027D1 RID: 10193 RVA: 0x000D2E98 File Offset: 0x000D1098
	private void Awake()
	{
		if (this.fXInfo == null)
		{
			Debug.LogErrorFormat("BuilderPiece {0} is missing Effect Info", new object[] { base.gameObject.name });
		}
		this.materialType = -1;
		this.pieceType = -1;
		this.pieceId = -1;
		this.pieceDataIndex = -1;
		this.state = BuilderPiece.State.None;
		this.isStatic = true;
		this.parentPiece = null;
		this.firstChildPiece = null;
		this.nextSiblingPiece = null;
		this.attachIndex = -1;
		this.parentAttachIndex = -1;
		this.parentHeld = null;
		this.heldByPlayerActorNumber = -1;
		this.placedOnlyColliders = new List<Collider>(4);
		List<Collider> list = new List<Collider>(4);
		foreach (GameObject gameObject in this.onlyWhenPlaced)
		{
			list.Clear();
			gameObject.GetComponentsInChildren<Collider>(list);
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].isTrigger)
				{
					BuilderPieceCollider builderPieceCollider = list[i].GetComponent<BuilderPieceCollider>();
					if (builderPieceCollider == null)
					{
						builderPieceCollider = list[i].AddComponent<BuilderPieceCollider>();
					}
					builderPieceCollider.piece = this;
					this.placedOnlyColliders.Add(list[i]);
				}
			}
		}
		this.SetActive(this.onlyWhenPlaced, false);
		this.SetActive(this.onlyWhenNotPlaced, true);
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		for (int j = this.colliders.Count - 1; j >= 0; j--)
		{
			if (this.colliders[j].isTrigger)
			{
				this.colliders.RemoveAt(j);
			}
			else
			{
				BuilderPieceCollider builderPieceCollider2 = this.colliders[j].GetComponent<BuilderPieceCollider>();
				if (builderPieceCollider2 == null)
				{
					builderPieceCollider2 = this.colliders[j].AddComponent<BuilderPieceCollider>();
				}
				builderPieceCollider2.piece = this;
			}
		}
		this.gridPlanes = new List<BuilderAttachGridPlane>(8);
		base.GetComponentsInChildren<BuilderAttachGridPlane>(this.gridPlanes);
		this.pieceComponents = new List<IBuilderPieceComponent>(1);
		base.GetComponentsInChildren<IBuilderPieceComponent>(true, this.pieceComponents);
		this.pieceComponentsActive = false;
		this.functionalPieceComponent = base.GetComponentInChildren<IBuilderPieceFunctional>(true);
		this.SetCollidersEnabled<Collider>(this.colliders, false);
		this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
		this.preventSnapUntilMoved = 0;
		this.preventSnapUntilMovedFromPos = Vector3.zero;
		this.renderingIndirect = new List<MeshRenderer>(4);
		this.renderingDirect = new List<MeshRenderer>(4);
		this.FindActiveRenderers();
		this.paintingCount = 0;
		this.potentialGrabCount = 0;
		this.potentialGrabChildCount = 0;
		this.isPrivatePlot = this.plotComponent != null;
		this.privatePlotIndex = -1;
		this.ClearCollisionHistory();
	}

	// Token: 0x060027D2 RID: 10194 RVA: 0x000D3154 File Offset: 0x000D1354
	public void SetTable(BuilderTable table)
	{
		this.tableOwner = table;
	}

	// Token: 0x060027D3 RID: 10195 RVA: 0x000D315D File Offset: 0x000D135D
	public BuilderTable GetTable()
	{
		return this.tableOwner;
	}

	// Token: 0x060027D4 RID: 10196 RVA: 0x000D3168 File Offset: 0x000D1368
	public void OnReturnToPool()
	{
		this.tableOwner.builderRenderer.RemovePiece(this);
		for (int i = 0; i < this.pieceComponents.Count; i++)
		{
			this.pieceComponents[i].OnPieceDestroy();
		}
		this.functionalPieceState = 0;
		this.state = BuilderPiece.State.None;
		this.isStatic = true;
		this.materialType = -1;
		this.pieceType = -1;
		this.pieceId = -1;
		this.pieceDataIndex = -1;
		this.parentPiece = null;
		this.firstChildPiece = null;
		this.nextSiblingPiece = null;
		this.attachIndex = -1;
		this.parentAttachIndex = -1;
		this.overrideSavedPiece = false;
		this.savedMaterialType = -1;
		this.savedPieceType = -1;
		this.shelfOwner = -1;
		this.parentHeld = null;
		this.heldByPlayerActorNumber = -1;
		this.activatedTimeStamp = 0;
		this.forcedFrozen = false;
		this.SetActive(this.onlyWhenPlaced, false);
		this.SetActive(this.onlyWhenNotPlaced, true);
		this.SetCollidersEnabled<Collider>(this.colliders, false);
		this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
		this.preventSnapUntilMoved = 0;
		this.preventSnapUntilMovedFromPos = Vector3.zero;
		base.transform.localScale = Vector3.one;
		if (this.isArmShelf)
		{
			if (this.armShelf != null)
			{
				this.armShelf.piece = null;
			}
			this.armShelf = null;
		}
		for (int j = 0; j < this.gridPlanes.Count; j++)
		{
			this.gridPlanes[j].OnReturnToPool(this.tableOwner.builderPool);
		}
	}

	// Token: 0x060027D5 RID: 10197 RVA: 0x000D32EA File Offset: 0x000D14EA
	public void OnCreatedByPool()
	{
		this.materialSwapTargets = new List<MeshRenderer>(4);
		base.GetComponentsInChildren<MeshRenderer>(this.areMeshesToggledOnPlace, this.materialSwapTargets);
		this.surfaceOverrides = new List<GorillaSurfaceOverride>(4);
		base.GetComponentsInChildren<GorillaSurfaceOverride>(this.areMeshesToggledOnPlace, this.surfaceOverrides);
	}

	// Token: 0x060027D6 RID: 10198 RVA: 0x000D3328 File Offset: 0x000D1528
	public void SetupPiece(float gridSize)
	{
		for (int i = 0; i < this.gridPlanes.Count; i++)
		{
			this.gridPlanes[i].Setup(this, i, gridSize);
		}
	}

	// Token: 0x060027D7 RID: 10199 RVA: 0x000D3360 File Offset: 0x000D1560
	public void SetMaterial(int inMaterialType, bool force = false)
	{
		if (this.materialOptions == null || this.materialSwapTargets == null || this.materialSwapTargets.Count < 1)
		{
			return;
		}
		if (this.materialType == inMaterialType && !force)
		{
			return;
		}
		this.materialType = inMaterialType;
		Material material = null;
		int num = -1;
		if (inMaterialType == -1)
		{
			this.materialOptions.GetDefaultMaterial(out this.materialType, out material, out num);
		}
		else
		{
			this.materialOptions.GetMaterialFromType(this.materialType, out material, out num);
			if (material == null)
			{
				this.materialOptions.GetDefaultMaterial(out this.materialType, out material, out num);
			}
		}
		if (material == null)
		{
			Debug.LogErrorFormat("Piece {0} has no material matching Type {1}", new object[]
			{
				this.GetPieceId(),
				inMaterialType
			});
			return;
		}
		foreach (MeshRenderer meshRenderer in this.materialSwapTargets)
		{
			if (!(meshRenderer == null) && meshRenderer.enabled)
			{
				meshRenderer.material = material;
			}
		}
		if (this.surfaceOverrides != null && num != -1)
		{
			foreach (GorillaSurfaceOverride gorillaSurfaceOverride in this.surfaceOverrides)
			{
				gorillaSurfaceOverride.overrideIndex = num;
			}
		}
		if (this.renderingIndirect.Count > 0)
		{
			this.tableOwner.builderRenderer.ChangePieceIndirectMaterial(this, this.materialSwapTargets, material);
		}
	}

	// Token: 0x060027D8 RID: 10200 RVA: 0x000D34F4 File Offset: 0x000D16F4
	public int GetPieceId()
	{
		return this.pieceId;
	}

	// Token: 0x060027D9 RID: 10201 RVA: 0x000D34FC File Offset: 0x000D16FC
	public int GetParentPieceId()
	{
		if (!(this.parentPiece == null))
		{
			return this.parentPiece.pieceId;
		}
		return -1;
	}

	// Token: 0x060027DA RID: 10202 RVA: 0x000D3519 File Offset: 0x000D1719
	public int GetAttachIndex()
	{
		return this.attachIndex;
	}

	// Token: 0x060027DB RID: 10203 RVA: 0x000D3521 File Offset: 0x000D1721
	public int GetParentAttachIndex()
	{
		return this.parentAttachIndex;
	}

	// Token: 0x060027DC RID: 10204 RVA: 0x000D352C File Offset: 0x000D172C
	private void SetPieceActive(List<IBuilderPieceComponent> components, bool active)
	{
		if (components == null || active == this.pieceComponentsActive)
		{
			return;
		}
		this.pieceComponentsActive = active;
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				if (active)
				{
					components[i].OnPieceActivate();
				}
				else
				{
					components[i].OnPieceDeactivate();
				}
			}
		}
	}

	// Token: 0x060027DD RID: 10205 RVA: 0x000D3584 File Offset: 0x000D1784
	private void SetBehavioursEnabled<T>(List<T> components, bool enabled) where T : Behaviour
	{
		if (components == null)
		{
			return;
		}
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				components[i].enabled = enabled;
			}
		}
	}

	// Token: 0x060027DE RID: 10206 RVA: 0x000D35CC File Offset: 0x000D17CC
	public void UpdateCollidersEnabled(bool _enabled)
	{
		this.SetCollidersEnabled<Collider>(this.colliders, _enabled);
	}

	// Token: 0x060027DF RID: 10207 RVA: 0x000D35DC File Offset: 0x000D17DC
	private void SetCollidersEnabled<T>(List<T> components, bool enabled) where T : Collider
	{
		if (components == null)
		{
			return;
		}
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				components[i].enabled = enabled;
			}
		}
	}

	// Token: 0x060027E0 RID: 10208 RVA: 0x000D3624 File Offset: 0x000D1824
	public void SetColliderLayers<T>(List<T> components, int layer) where T : Collider
	{
		this.currentColliderLayer = layer;
		if (components == null)
		{
			return;
		}
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				components[i].gameObject.layer = layer;
			}
		}
	}

	// Token: 0x060027E1 RID: 10209 RVA: 0x000D3678 File Offset: 0x000D1878
	private void SetActive(List<GameObject> gameObjects, bool active)
	{
		if (gameObjects == null)
		{
			return;
		}
		for (int i = 0; i < gameObjects.Count; i++)
		{
			if (gameObjects[i] != null)
			{
				gameObjects[i].SetActive(active);
			}
		}
	}

	// Token: 0x060027E2 RID: 10210 RVA: 0x000D36B6 File Offset: 0x000D18B6
	public void SetFunctionalPieceState(byte fState, NetPlayer instigator, int timeStamp)
	{
		if (this.functionalPieceComponent == null || !this.functionalPieceComponent.IsStateValid(fState))
		{
			fState = 0;
		}
		this.functionalPieceState = fState;
		IBuilderPieceFunctional builderPieceFunctional = this.functionalPieceComponent;
		if (builderPieceFunctional == null)
		{
			return;
		}
		builderPieceFunctional.OnStateChanged(fState, instigator, timeStamp);
	}

	// Token: 0x060027E3 RID: 10211 RVA: 0x000D36EB File Offset: 0x000D18EB
	public void SetScale(float scale)
	{
		if (this.scaleRoot != null)
		{
			this.scaleRoot.localScale = Vector3.one * scale;
		}
		this.pieceScale = scale;
	}

	// Token: 0x060027E4 RID: 10212 RVA: 0x000D3718 File Offset: 0x000D1918
	public float GetScale()
	{
		return this.pieceScale;
	}

	// Token: 0x060027E5 RID: 10213 RVA: 0x000D3720 File Offset: 0x000D1920
	public void PaintingTint(bool enable)
	{
		if (enable)
		{
			this.paintingCount++;
			if (this.paintingCount == 1)
			{
				this.RefreshTint();
				return;
			}
		}
		else
		{
			this.paintingCount--;
			if (this.paintingCount == 0)
			{
				this.RefreshTint();
			}
		}
	}

	// Token: 0x060027E6 RID: 10214 RVA: 0x000D3760 File Offset: 0x000D1960
	public void PotentialGrab(bool enable)
	{
		if (enable)
		{
			this.potentialGrabCount++;
			if (this.potentialGrabCount == 1 && this.potentialGrabChildCount == 0)
			{
				this.RefreshTint();
				return;
			}
		}
		else
		{
			this.potentialGrabCount--;
			if (this.potentialGrabCount == 0 && this.potentialGrabChildCount == 0)
			{
				this.RefreshTint();
			}
		}
	}

	// Token: 0x060027E7 RID: 10215 RVA: 0x000D37BC File Offset: 0x000D19BC
	public static void PotentialGrabChildren(BuilderPiece piece, bool enable)
	{
		BuilderPiece builderPiece = piece.firstChildPiece;
		while (builderPiece != null)
		{
			if (enable)
			{
				builderPiece.potentialGrabChildCount++;
				if (builderPiece.potentialGrabChildCount == 1 && builderPiece.potentialGrabCount == 0)
				{
					builderPiece.RefreshTint();
				}
			}
			else
			{
				builderPiece.potentialGrabChildCount--;
				if (builderPiece.potentialGrabChildCount == 0 && builderPiece.potentialGrabCount == 0)
				{
					builderPiece.RefreshTint();
				}
			}
			BuilderPiece.PotentialGrabChildren(builderPiece, enable);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x060027E8 RID: 10216 RVA: 0x000D3838 File Offset: 0x000D1A38
	private void RefreshTint()
	{
		if (this.potentialGrabCount > 0 || this.potentialGrabChildCount > 0)
		{
			this.SetTint(this.tableOwner.potentialGrabTint);
			return;
		}
		if (this.paintingCount > 0)
		{
			this.SetTint(this.tableOwner.paintingTint);
			return;
		}
		switch (this.state)
		{
		case BuilderPiece.State.AttachedToDropped:
		case BuilderPiece.State.Dropped:
			this.SetTint(this.tableOwner.droppedTint);
			return;
		case BuilderPiece.State.Grabbed:
		case BuilderPiece.State.GrabbedLocal:
		case BuilderPiece.State.AttachedToArm:
			this.SetTint(this.tableOwner.grabbedTint);
			return;
		case BuilderPiece.State.OnShelf:
		case BuilderPiece.State.OnConveyor:
			this.SetTint(this.tableOwner.shelfTint);
			return;
		}
		this.SetTint(this.tableOwner.defaultTint);
	}

	// Token: 0x060027E9 RID: 10217 RVA: 0x000D38FC File Offset: 0x000D1AFC
	private void SetTint(float tint)
	{
		if (tint == this.tint)
		{
			return;
		}
		this.tint = tint;
		this.tableOwner.builderRenderer.SetPieceTint(this, tint);
	}

	// Token: 0x060027EA RID: 10218 RVA: 0x000D3924 File Offset: 0x000D1B24
	public void SetParentPiece(int newAttachIndex, BuilderPiece newParentPiece, int newParentAttachIndex)
	{
		if (this.parentHeld != null)
		{
			Debug.LogErrorFormat(newParentPiece.gameObject, "Cannot attach to piece {0} while already held", new object[] { (newParentPiece == null) ? null : newParentPiece.gameObject.name });
			return;
		}
		BuilderPiece.RemovePieceFromParent(this);
		this.attachIndex = newAttachIndex;
		this.parentPiece = newParentPiece;
		this.parentAttachIndex = newParentAttachIndex;
		this.AddPieceToParent(this);
		Transform transform = null;
		if (newParentPiece != null)
		{
			if (newParentAttachIndex >= 0)
			{
				transform = newParentPiece.gridPlanes[newParentAttachIndex].transform;
			}
			else
			{
				transform = newParentPiece.transform;
			}
		}
		base.transform.SetParent(transform, true);
		this.requestedParentPiece = null;
		this.tableOwner.UpdatePieceData(this);
	}

	// Token: 0x060027EB RID: 10219 RVA: 0x000D39DC File Offset: 0x000D1BDC
	public void ClearParentPiece(bool ignoreSnaps = false)
	{
		if (this.parentPiece == null)
		{
			if (!ignoreSnaps)
			{
				BuilderPiece.RemoveOverlapsWithDifferentPieceRoot(this, this, this.tableOwner.builderPool);
			}
			return;
		}
		BuilderPiece builderPiece = this.parentPiece;
		BuilderPiece.RemovePieceFromParent(this);
		this.attachIndex = -1;
		this.parentPiece = null;
		this.parentAttachIndex = -1;
		base.transform.SetParent(null, true);
		this.requestedParentPiece = null;
		this.tableOwner.UpdatePieceData(this);
		if (!ignoreSnaps)
		{
			BuilderPiece.RemoveOverlapsWithDifferentPieceRoot(this, this.GetRootPiece(), this.tableOwner.builderPool);
		}
	}

	// Token: 0x060027EC RID: 10220 RVA: 0x000D3A6C File Offset: 0x000D1C6C
	public static void RemoveOverlapsWithDifferentPieceRoot(BuilderPiece piece, BuilderPiece root, BuilderPool pool)
	{
		for (int i = 0; i < piece.gridPlanes.Count; i++)
		{
			piece.gridPlanes[i].RemoveSnapsWithDifferentRoot(root, pool);
		}
		BuilderPiece builderPiece = piece.firstChildPiece;
		while (builderPiece != null)
		{
			BuilderPiece.RemoveOverlapsWithDifferentPieceRoot(builderPiece, root, pool);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x060027ED RID: 10221 RVA: 0x000D3AC4 File Offset: 0x000D1CC4
	private void AddPieceToParent(BuilderPiece piece)
	{
		BuilderPiece builderPiece = piece.parentPiece;
		if (builderPiece == null)
		{
			return;
		}
		this.nextSiblingPiece = builderPiece.firstChildPiece;
		builderPiece.firstChildPiece = piece;
		if (piece.parentAttachIndex >= 0 && piece.parentAttachIndex < builderPiece.gridPlanes.Count)
		{
			builderPiece.gridPlanes[piece.parentAttachIndex].ChangeChildPieceCount(1 + piece.GetChildCount());
		}
	}

	// Token: 0x060027EE RID: 10222 RVA: 0x000D3B30 File Offset: 0x000D1D30
	private static void RemovePieceFromParent(BuilderPiece piece)
	{
		BuilderPiece builderPiece = piece.parentPiece;
		if (builderPiece == null)
		{
			return;
		}
		BuilderPiece builderPiece2 = builderPiece.firstChildPiece;
		if (builderPiece2 == null)
		{
			Debug.LogErrorFormat("Parent {0} of piece {1} doesn't have any children", new object[] { builderPiece.name, piece.name });
		}
		bool flag = false;
		if (builderPiece2 == piece)
		{
			builderPiece.firstChildPiece = builderPiece2.nextSiblingPiece;
			flag = true;
		}
		else
		{
			while (builderPiece2 != null)
			{
				if (builderPiece2.nextSiblingPiece == piece)
				{
					builderPiece2.nextSiblingPiece = piece.nextSiblingPiece;
					piece.nextSiblingPiece = null;
					flag = true;
					break;
				}
				builderPiece2 = builderPiece2.nextSiblingPiece;
			}
		}
		if (!flag)
		{
			Debug.LogErrorFormat("Parent {0} of piece {1} doesn't have the piece a child", new object[] { builderPiece.name, piece.name });
			return;
		}
		if (piece.parentAttachIndex >= 0 && piece.parentAttachIndex < builderPiece.gridPlanes.Count)
		{
			builderPiece.gridPlanes[piece.parentAttachIndex].ChangeChildPieceCount(-1 * (1 + piece.GetChildCount()));
		}
	}

	// Token: 0x060027EF RID: 10223 RVA: 0x000D3C34 File Offset: 0x000D1E34
	public void SetParentHeld(Transform parentHeld, int heldByPlayerActorNumber, bool heldInLeftHand)
	{
		if (this.parentPiece != null)
		{
			Debug.LogErrorFormat(this.parentPiece.gameObject, "Cannot hold while already attached to piece {0}", new object[] { this.parentPiece.gameObject.name });
			return;
		}
		this.heldByPlayerActorNumber = heldByPlayerActorNumber;
		this.parentHeld = parentHeld;
		this.heldInLeftHand = heldInLeftHand;
		base.transform.SetParent(parentHeld);
		this.tableOwner.UpdatePieceData(this);
		if (heldByPlayerActorNumber != -1)
		{
			this.OnGrabbedAsRoot();
			return;
		}
		this.OnReleasedAsRoot();
	}

	// Token: 0x060027F0 RID: 10224 RVA: 0x000D3CBC File Offset: 0x000D1EBC
	public void ClearParentHeld()
	{
		if (this.parentHeld == null)
		{
			return;
		}
		if (this.isArmShelf && this.armShelf != null)
		{
			this.armShelf.piece = null;
			this.armShelf = null;
		}
		this.heldByPlayerActorNumber = -1;
		this.parentHeld = null;
		this.heldInLeftHand = false;
		base.transform.SetParent(this.parentHeld);
		this.tableOwner.UpdatePieceData(this);
		this.OnReleasedAsRoot();
	}

	// Token: 0x060027F1 RID: 10225 RVA: 0x000D3D39 File Offset: 0x000D1F39
	public bool IsHeldLocal()
	{
		return this.heldByPlayerActorNumber != -1 && this.heldByPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x060027F2 RID: 10226 RVA: 0x000D3D58 File Offset: 0x000D1F58
	public bool IsHeldBy(int actorNumber)
	{
		return actorNumber != -1 && this.heldByPlayerActorNumber == actorNumber;
	}

	// Token: 0x060027F3 RID: 10227 RVA: 0x000D3D69 File Offset: 0x000D1F69
	public bool IsHeldInLeftHand()
	{
		return this.heldInLeftHand;
	}

	// Token: 0x060027F4 RID: 10228 RVA: 0x000D3D71 File Offset: 0x000D1F71
	public static bool IsDroppedState(BuilderPiece.State state)
	{
		return state == BuilderPiece.State.Dropped || state == BuilderPiece.State.AttachedToDropped || state == BuilderPiece.State.OnShelf || state == BuilderPiece.State.OnConveyor;
	}

	// Token: 0x060027F5 RID: 10229 RVA: 0x000D3D88 File Offset: 0x000D1F88
	public void SetActivateTimeStamp(int timeStamp)
	{
		this.activatedTimeStamp = timeStamp;
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			builderPiece.SetActivateTimeStamp(timeStamp);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x060027F6 RID: 10230 RVA: 0x000D3DBC File Offset: 0x000D1FBC
	public void SetState(BuilderPiece.State newState, bool force = false)
	{
		if (newState == this.state && !force)
		{
			if (newState == BuilderPiece.State.Grabbed)
			{
				int expectedGrabCollisionLayer = this.GetExpectedGrabCollisionLayer();
				if (this.currentColliderLayer != expectedGrabCollisionLayer)
				{
					this.SetColliderLayers<Collider>(this.colliders, expectedGrabCollisionLayer);
					this.SetChildrenCollisionLayer(expectedGrabCollisionLayer);
				}
			}
			return;
		}
		if (newState == BuilderPiece.State.Dropped && this.state != BuilderPiece.State.Dropped)
		{
			this.tableOwner.AddPieceToDropList(this);
		}
		else if (this.state == BuilderPiece.State.Dropped && newState != BuilderPiece.State.Dropped)
		{
			this.tableOwner.RemovePieceFromDropList(this);
		}
		BuilderPiece.State state = this.state;
		this.state = newState;
		if (this.pieceDataIndex >= 0)
		{
			this.tableOwner.UpdatePieceData(this);
		}
		switch (this.state)
		{
		case BuilderPiece.State.None:
			this.SetCollidersEnabled<Collider>(this.colliders, false);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, false);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.None, force);
			this.tableOwner.builderRenderer.RemovePiece(this);
			this.isStatic = true;
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.AttachedAndPlaced:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, true);
			this.SetActive(this.onlyWhenPlaced, true);
			this.SetActive(this.onlyWhenNotPlaced, false);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.placedLayer);
			this.SetChildrenState(BuilderPiece.State.AttachedAndPlaced, force);
			this.SetStatic(false, force || this.areMeshesToggledOnPlace);
			this.SetPieceActive(this.pieceComponents, true);
			this.RefreshTint();
			return;
		case BuilderPiece.State.AttachedToDropped:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.AttachedToDropped, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.Grabbed:
		{
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			int expectedGrabCollisionLayer2 = this.GetExpectedGrabCollisionLayer();
			this.SetColliderLayers<Collider>(this.colliders, expectedGrabCollisionLayer2);
			this.SetChildrenState(BuilderPiece.State.Grabbed, force);
			this.SetStatic(false, force || (this.areMeshesToggledOnPlace && state == BuilderPiece.State.AttachedAndPlaced));
			this.SetPieceActive(this.pieceComponents, false);
			this.SetActivateTimeStamp(0);
			this.RefreshTint();
			this.forcedFrozen = false;
			return;
		}
		case BuilderPiece.State.Dropped:
			this.ClearCollisionHistory();
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(false, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.AttachedToDropped, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.OnShelf:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.OnShelf, force);
			this.SetStatic(true, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.Displayed:
			this.SetCollidersEnabled<Collider>(this.colliders, false);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetChildrenState(BuilderPiece.State.Displayed, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.GrabbedLocal:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.heldLayerLocal);
			this.SetChildrenState(BuilderPiece.State.GrabbedLocal, force);
			this.SetStatic(false, force || (this.areMeshesToggledOnPlace && state == BuilderPiece.State.AttachedAndPlaced));
			this.SetPieceActive(this.pieceComponents, false);
			this.SetActivateTimeStamp(0);
			this.RefreshTint();
			this.forcedFrozen = false;
			return;
		case BuilderPiece.State.OnConveyor:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.OnConveyor, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.AttachedToArm:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.heldLayerLocal);
			this.SetChildrenState(BuilderPiece.State.AttachedToArm, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		default:
			return;
		}
	}

	// Token: 0x060027F7 RID: 10231 RVA: 0x000D4350 File Offset: 0x000D2550
	public void OnGrabbedAsRoot()
	{
		if (this.isArmShelf)
		{
			return;
		}
		if (this.heldByPlayerActorNumber != NetworkSystem.Instance.LocalPlayer.ActorNumber && !this.listeningToHandLinks)
		{
			TakeMyHand_HandLink.OnHandLinkChanged = (Action)Delegate.Combine(TakeMyHand_HandLink.OnHandLinkChanged, new Action(this.UpdateGrabbedPieceCollisionLayer));
			this.listeningToHandLinks = true;
		}
	}

	// Token: 0x060027F8 RID: 10232 RVA: 0x000D43AC File Offset: 0x000D25AC
	public void OnReleasedAsRoot()
	{
		if (this.isArmShelf)
		{
			return;
		}
		if (this.listeningToHandLinks)
		{
			TakeMyHand_HandLink.OnHandLinkChanged = (Action)Delegate.Remove(TakeMyHand_HandLink.OnHandLinkChanged, new Action(this.UpdateGrabbedPieceCollisionLayer));
			this.listeningToHandLinks = false;
		}
	}

	// Token: 0x060027F9 RID: 10233 RVA: 0x000D43E8 File Offset: 0x000D25E8
	public void SetKinematic(bool kinematic, bool destroyImmediate = true)
	{
		if (kinematic && this.rigidBody != null)
		{
			if (destroyImmediate)
			{
				Object.DestroyImmediate(this.rigidBody);
				this.rigidBody = null;
			}
			else
			{
				Object.Destroy(this.rigidBody);
				this.rigidBody = null;
			}
		}
		else if (!kinematic && this.rigidBody == null)
		{
			this.rigidBody = base.gameObject.GetComponent<Rigidbody>();
			if (this.rigidBody != null)
			{
				Debug.LogErrorFormat("We should never already have a rigid body here {0} {1}", new object[] { this.pieceId, this.pieceType });
			}
			if (this.rigidBody == null)
			{
				this.rigidBody = base.gameObject.AddComponent<Rigidbody>();
			}
			if (this.rigidBody != null)
			{
				this.rigidBody.isKinematic = kinematic;
			}
		}
		if (this.rigidBody != null)
		{
			this.rigidBody.mass = 1f;
		}
	}

	// Token: 0x060027FA RID: 10234 RVA: 0x000D44F0 File Offset: 0x000D26F0
	public void ClearCollisionHistory()
	{
		if (this.collisionEnterHistory == null)
		{
			this.collisionEnterHistory = new float[this.collisionEnterLimit];
		}
		for (int i = 0; i < this.collisionEnterLimit; i++)
		{
			this.collisionEnterHistory[i] = float.MinValue;
		}
		this.collidersEntered.Clear();
		this.oldCollisionTimeIndex = 0;
		this.forcedFrozen = false;
	}

	// Token: 0x060027FB RID: 10235 RVA: 0x000D4550 File Offset: 0x000D2750
	private void OnCollisionEnter(Collision other)
	{
		if (this.state != BuilderPiece.State.Dropped || this.forcedFrozen)
		{
			return;
		}
		BuilderPieceCollider component = other.collider.GetComponent<BuilderPieceCollider>();
		if (component != null)
		{
			BuilderPiece piece = component.piece;
			if ((piece.state == BuilderPiece.State.AttachedAndPlaced || piece.forcedFrozen) && !this.collidersEntered.Add(other.collider.GetInstanceID()))
			{
				if (this.collisionEnterHistory[this.oldCollisionTimeIndex] > Time.time)
				{
					this.tableOwner.FreezeDroppedPiece(this);
					return;
				}
				this.collisionEnterHistory[this.oldCollisionTimeIndex] = Time.time + this.collisionEnterCooldown;
				int num = this.oldCollisionTimeIndex + 1;
				this.oldCollisionTimeIndex = num;
				this.oldCollisionTimeIndex = num % this.collisionEnterLimit;
			}
		}
	}

	// Token: 0x060027FC RID: 10236 RVA: 0x000D4610 File Offset: 0x000D2810
	public int GetExpectedGrabCollisionLayer()
	{
		if (this.heldByPlayerActorNumber != -1)
		{
			if (!GorillaTagger.Instance.offlineVRRig.IsInHandHoldChainWithOtherPlayer(this.heldByPlayerActorNumber))
			{
				return BuilderTable.heldLayer;
			}
			return BuilderTable.heldLayerLocal;
		}
		else
		{
			if (this.parentPiece != null)
			{
				return this.parentPiece.currentColliderLayer;
			}
			return BuilderTable.heldLayer;
		}
	}

	// Token: 0x060027FD RID: 10237 RVA: 0x000D4668 File Offset: 0x000D2868
	public void UpdateGrabbedPieceCollisionLayer()
	{
		int expectedGrabCollisionLayer = this.GetExpectedGrabCollisionLayer();
		if (this.currentColliderLayer != expectedGrabCollisionLayer)
		{
			this.SetColliderLayers<Collider>(this.colliders, expectedGrabCollisionLayer);
			this.SetChildrenCollisionLayer(expectedGrabCollisionLayer);
		}
	}

	// Token: 0x060027FE RID: 10238 RVA: 0x000D469C File Offset: 0x000D289C
	private void SetChildrenCollisionLayer(int layer)
	{
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			builderPiece.SetColliderLayers<Collider>(builderPiece.colliders, layer);
			builderPiece.SetChildrenCollisionLayer(layer);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x060027FF RID: 10239 RVA: 0x000D46D8 File Offset: 0x000D28D8
	public void SetStatic(bool isStatic, bool force = false)
	{
		isStatic = true;
		if (this.isStatic == isStatic && !force)
		{
			return;
		}
		this.SetDirectRenderersVisible(true);
		this.tableOwner.builderRenderer.RemovePiece(this);
		this.isStatic = isStatic;
		if (this.areMeshesToggledOnPlace)
		{
			this.FindActiveRenderers();
		}
		this.tableOwner.builderRenderer.AddPiece(this);
		this.SetDirectRenderersVisible(this.tableOwner.IsInBuilderZone());
	}

	// Token: 0x06002800 RID: 10240 RVA: 0x000D4744 File Offset: 0x000D2944
	private void FindActiveRenderers()
	{
		if (this.renderingDirect.Count > 0)
		{
			foreach (MeshRenderer meshRenderer in this.renderingDirect)
			{
				meshRenderer.enabled = true;
			}
		}
		this.renderingDirect.Clear();
		BuilderPiece.tempRenderers.Clear();
		base.GetComponentsInChildren<MeshRenderer>(false, BuilderPiece.tempRenderers);
		foreach (MeshRenderer meshRenderer2 in BuilderPiece.tempRenderers)
		{
			if (meshRenderer2.enabled)
			{
				this.renderingDirect.Add(meshRenderer2);
			}
		}
	}

	// Token: 0x06002801 RID: 10241 RVA: 0x000D4814 File Offset: 0x000D2A14
	public void SetDirectRenderersVisible(bool visible)
	{
		if (this.renderingDirect != null && this.renderingDirect.Count > 0)
		{
			foreach (MeshRenderer meshRenderer in this.renderingDirect)
			{
				meshRenderer.enabled = visible;
			}
		}
	}

	// Token: 0x06002802 RID: 10242 RVA: 0x000D487C File Offset: 0x000D2A7C
	private void SetChildrenState(BuilderPiece.State newState, bool force)
	{
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			builderPiece.SetState(newState, force);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x06002803 RID: 10243 RVA: 0x000D48AC File Offset: 0x000D2AAC
	public void OnCreate()
	{
		for (int i = 0; i < this.pieceComponents.Count; i++)
		{
			this.pieceComponents[i].OnPieceCreate(this.pieceType, this.pieceId);
		}
	}

	// Token: 0x06002804 RID: 10244 RVA: 0x000D48EC File Offset: 0x000D2AEC
	public void OnPlacementDeserialized()
	{
		for (int i = 0; i < this.pieceComponents.Count; i++)
		{
			this.pieceComponents[i].OnPiecePlacementDeserialized();
		}
	}

	// Token: 0x06002805 RID: 10245 RVA: 0x000D4920 File Offset: 0x000D2B20
	public void PlayPlacementFx()
	{
		this.PlayFX(this.fXInfo.placeVFX);
	}

	// Token: 0x06002806 RID: 10246 RVA: 0x000D4933 File Offset: 0x000D2B33
	public void PlayDisconnectFx()
	{
		this.PlayFX(this.fXInfo.disconnectVFX);
	}

	// Token: 0x06002807 RID: 10247 RVA: 0x000D4946 File Offset: 0x000D2B46
	public void PlayGrabbedFx()
	{
		this.PlayFX(this.fXInfo.grabbedVFX);
	}

	// Token: 0x06002808 RID: 10248 RVA: 0x000D4959 File Offset: 0x000D2B59
	public void PlayTooHeavyFx()
	{
		this.PlayFX(this.fXInfo.tooHeavyVFX);
	}

	// Token: 0x06002809 RID: 10249 RVA: 0x000D496C File Offset: 0x000D2B6C
	public void PlayLocationLockFx()
	{
		this.PlayFX(this.fXInfo.locationLockVFX);
	}

	// Token: 0x0600280A RID: 10250 RVA: 0x000D497F File Offset: 0x000D2B7F
	public void PlayRecycleFx()
	{
		this.PlayFX(this.fXInfo.recycleVFX);
	}

	// Token: 0x0600280B RID: 10251 RVA: 0x000D4992 File Offset: 0x000D2B92
	private void PlayFX(GameObject fx)
	{
		ObjectPools.instance.Instantiate(fx, base.transform.position, true);
	}

	// Token: 0x0600280C RID: 10252 RVA: 0x000D49AC File Offset: 0x000D2BAC
	public static BuilderPiece GetBuilderPieceFromCollider(Collider collider)
	{
		if (collider == null)
		{
			return null;
		}
		BuilderPieceCollider component = collider.GetComponent<BuilderPieceCollider>();
		if (!(component == null))
		{
			return component.piece;
		}
		return null;
	}

	// Token: 0x0600280D RID: 10253 RVA: 0x000D49DC File Offset: 0x000D2BDC
	public static BuilderPiece GetBuilderPieceFromTransform(Transform transform)
	{
		while (transform != null)
		{
			BuilderPiece component = transform.GetComponent<BuilderPiece>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x0600280E RID: 10254 RVA: 0x000D4A10 File Offset: 0x000D2C10
	public static void MakePieceRoot(BuilderPiece piece)
	{
		if (piece == null)
		{
			return;
		}
		if (piece.parentPiece == null || piece.parentPiece.isBuiltIntoTable)
		{
			return;
		}
		BuilderPiece.MakePieceRoot(piece.parentPiece);
		int num = piece.parentAttachIndex;
		int num2 = piece.attachIndex;
		BuilderPiece builderPiece = piece.parentPiece;
		bool flag = true;
		piece.ClearParentPiece(flag);
		builderPiece.SetParentPiece(num, piece, num2);
	}

	// Token: 0x0600280F RID: 10255 RVA: 0x000D4A74 File Offset: 0x000D2C74
	public BuilderPiece GetRootPiece()
	{
		BuilderPiece builderPiece = this;
		while (builderPiece.parentPiece != null && !builderPiece.parentPiece.isBuiltIntoTable)
		{
			builderPiece = builderPiece.parentPiece;
		}
		return builderPiece;
	}

	// Token: 0x06002810 RID: 10256 RVA: 0x000D4AA8 File Offset: 0x000D2CA8
	public bool IsPrivatePlot()
	{
		return this.isPrivatePlot;
	}

	// Token: 0x06002811 RID: 10257 RVA: 0x000D4AB0 File Offset: 0x000D2CB0
	public bool TryGetPlotComponent(out BuilderPiecePrivatePlot plot)
	{
		plot = this.plotComponent;
		return this.isPrivatePlot;
	}

	// Token: 0x06002812 RID: 10258 RVA: 0x000D4AC8 File Offset: 0x000D2CC8
	public static bool CanPlayerAttachPieceToPiece(int playerActorNumber, BuilderPiece attachingPiece, BuilderPiece attachToPiece)
	{
		if (attachToPiece.state != BuilderPiece.State.AttachedAndPlaced && !attachToPiece.IsPrivatePlot() && attachToPiece.state != BuilderPiece.State.AttachedToArm)
		{
			return true;
		}
		BuilderPiece attachedBuiltInPiece = attachToPiece.GetAttachedBuiltInPiece();
		if (attachedBuiltInPiece == null || (!attachedBuiltInPiece.isPrivatePlot && !attachedBuiltInPiece.isArmShelf))
		{
			return true;
		}
		if (attachedBuiltInPiece.isArmShelf)
		{
			return attachedBuiltInPiece.heldByPlayerActorNumber == playerActorNumber && attachedBuiltInPiece.armShelf != null && attachedBuiltInPiece.armShelf.CanAttachToArmPiece();
		}
		BuilderPiecePrivatePlot builderPiecePrivatePlot;
		return !attachedBuiltInPiece.TryGetPlotComponent(out builderPiecePrivatePlot) || (builderPiecePrivatePlot.CanPlayerAttachToPlot(playerActorNumber) && builderPiecePrivatePlot.IsChainUnderCapacity(attachingPiece));
	}

	// Token: 0x06002813 RID: 10259 RVA: 0x000D4B60 File Offset: 0x000D2D60
	public bool CanPlayerGrabPiece(int actorNumber, Vector3 worldPosition)
	{
		if (this.state != BuilderPiece.State.AttachedAndPlaced && !this.isPrivatePlot)
		{
			return true;
		}
		BuilderPiece attachedBuiltInPiece = this.GetAttachedBuiltInPiece();
		BuilderPiecePrivatePlot builderPiecePrivatePlot;
		return attachedBuiltInPiece == null || !attachedBuiltInPiece.isPrivatePlot || !attachedBuiltInPiece.TryGetPlotComponent(out builderPiecePrivatePlot) || builderPiecePrivatePlot.CanPlayerGrabFromPlot(actorNumber, worldPosition) || this.tableOwner.IsLocationWithinSharedBuildArea(worldPosition);
	}

	// Token: 0x06002814 RID: 10260 RVA: 0x000D4BC0 File Offset: 0x000D2DC0
	public bool IsPieceMoving()
	{
		if (this.state != BuilderPiece.State.AttachedAndPlaced)
		{
			return false;
		}
		if (this.attachPlayerToPiece)
		{
			return true;
		}
		if (this.attachIndex < 0 || this.attachIndex >= this.gridPlanes.Count)
		{
			return false;
		}
		if (this.gridPlanes[this.attachIndex].IsAttachedToMovingGrid())
		{
			return true;
		}
		using (List<BuilderAttachGridPlane>.Enumerator enumerator = this.gridPlanes.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.isMoving)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002815 RID: 10261 RVA: 0x000D4C68 File Offset: 0x000D2E68
	public BuilderPiece GetAttachedBuiltInPiece()
	{
		if (this.isBuiltIntoTable)
		{
			return this;
		}
		if (this.state != BuilderPiece.State.AttachedAndPlaced)
		{
			return null;
		}
		BuilderPiece rootPiece = this.GetRootPiece();
		if (rootPiece.parentPiece != null)
		{
			rootPiece = rootPiece.parentPiece;
		}
		if (rootPiece.isBuiltIntoTable)
		{
			return rootPiece;
		}
		return null;
	}

	// Token: 0x06002816 RID: 10262 RVA: 0x000D4CB0 File Offset: 0x000D2EB0
	public int GetChainCostAndCount(int[] costArray)
	{
		for (int i = 0; i < costArray.Length; i++)
		{
			costArray[i] = 0;
		}
		foreach (BuilderResourceQuantity builderResourceQuantity in this.cost.quantities)
		{
			if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
			{
				costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
			}
		}
		return 1 + this.GetChildCountAndCost(costArray);
	}

	// Token: 0x06002817 RID: 10263 RVA: 0x000D4D44 File Offset: 0x000D2F44
	public int GetChildCountAndCost(int[] costArray)
	{
		int num = 0;
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			num++;
			foreach (BuilderResourceQuantity builderResourceQuantity in builderPiece.cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
				}
			}
			num += builderPiece.GetChildCountAndCost(costArray);
			builderPiece = builderPiece.nextSiblingPiece;
		}
		return num;
	}

	// Token: 0x06002818 RID: 10264 RVA: 0x000D4DE8 File Offset: 0x000D2FE8
	public int GetChildCount()
	{
		int num = 0;
		foreach (BuilderAttachGridPlane builderAttachGridPlane in this.gridPlanes)
		{
			num += builderAttachGridPlane.GetChildCount();
		}
		return num;
	}

	// Token: 0x06002819 RID: 10265 RVA: 0x000D4E40 File Offset: 0x000D3040
	public void GetChainCost(int[] costArray)
	{
		for (int i = 0; i < costArray.Length; i++)
		{
			costArray[i] = 0;
		}
		foreach (BuilderResourceQuantity builderResourceQuantity in this.cost.quantities)
		{
			if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
			{
				costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
			}
		}
		this.AddChildCost(costArray);
	}

	// Token: 0x0600281A RID: 10266 RVA: 0x000D4ED4 File Offset: 0x000D30D4
	public void AddChildCost(int[] costArray)
	{
		int num = 0;
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			num++;
			foreach (BuilderResourceQuantity builderResourceQuantity in builderPiece.cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
				}
			}
			builderPiece.AddChildCost(costArray);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x0600281B RID: 10267 RVA: 0x000D4F74 File Offset: 0x000D3174
	public void BumpTwistToPositionRotation(byte twist, sbyte xOffset, sbyte zOffset, int potentialAttachIndex, BuilderAttachGridPlane potentialParentGridPlane, out Vector3 localPosition, out Quaternion localRotation, out Vector3 worldPosition, out Quaternion worldRotation)
	{
		float gridSize = this.tableOwner.gridSize;
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[potentialAttachIndex];
		bool flag = (long)(twist % 2) == 1L;
		Transform center = potentialParentGridPlane.center;
		Vector3 position = center.position;
		Quaternion rotation = center.rotation;
		float num = (flag ? builderAttachGridPlane.lengthOffset : builderAttachGridPlane.widthOffset);
		float num2 = (flag ? builderAttachGridPlane.widthOffset : builderAttachGridPlane.lengthOffset);
		float num3 = num - potentialParentGridPlane.widthOffset;
		float num4 = num2 - potentialParentGridPlane.lengthOffset;
		Quaternion quaternion = Quaternion.Euler(0f, (float)twist * 90f, 0f);
		Quaternion quaternion2 = rotation * quaternion;
		float num5 = (float)xOffset * gridSize + num3;
		float num6 = (float)zOffset * gridSize + num4;
		Vector3 vector = new Vector3(num5, 0f, num6);
		Vector3 vector2 = position + rotation * vector;
		Transform center2 = builderAttachGridPlane.center;
		Quaternion quaternion3 = quaternion2 * Quaternion.Inverse(center2.localRotation);
		Vector3 vector3 = base.transform.InverseTransformPoint(center2.position);
		Vector3 vector4 = vector2 - quaternion3 * vector3;
		localPosition = potentialParentGridPlane.transform.InverseTransformPoint(vector4);
		localRotation = quaternion * Quaternion.Inverse(center2.localRotation);
		worldPosition = vector4;
		worldRotation = quaternion3;
	}

	// Token: 0x0600281C RID: 10268 RVA: 0x000D50C8 File Offset: 0x000D32C8
	public Quaternion TwistToLocalRotation(byte twist, int potentialAttachIndex)
	{
		float num = 90f * (float)twist;
		Quaternion quaternion = Quaternion.Euler(0f, num, 0f);
		if (potentialAttachIndex < 0 || potentialAttachIndex >= this.gridPlanes.Count)
		{
			return quaternion;
		}
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[potentialAttachIndex];
		Transform transform = ((builderAttachGridPlane.center != null) ? builderAttachGridPlane.center : builderAttachGridPlane.transform);
		return quaternion * Quaternion.Inverse(transform.localRotation);
	}

	// Token: 0x0600281D RID: 10269 RVA: 0x000D5140 File Offset: 0x000D3340
	public int GetPiecePlacement()
	{
		byte pieceTwist = this.GetPieceTwist();
		sbyte b;
		sbyte b2;
		this.GetPieceBumpOffset(pieceTwist, out b, out b2);
		return BuilderTable.PackPiecePlacement(pieceTwist, b, b2);
	}

	// Token: 0x0600281E RID: 10270 RVA: 0x000D5168 File Offset: 0x000D3368
	public byte GetPieceTwist()
	{
		if (this.attachIndex == -1)
		{
			return 0;
		}
		Quaternion localRotation = base.transform.localRotation;
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[this.attachIndex];
		Quaternion quaternion = localRotation * builderAttachGridPlane.transform.localRotation;
		float num = 0.866f;
		Vector3 vector = quaternion * Vector3.forward;
		float num2 = Vector3.Dot(vector, Vector3.forward);
		float num3 = Vector3.Dot(vector, Vector3.right);
		bool flag = Mathf.Abs(num2) > num;
		bool flag2 = Mathf.Abs(num3) > num;
		if (!flag && !flag2)
		{
			return 0;
		}
		uint num4;
		if (flag)
		{
			num4 = ((num2 > 0f) ? 0U : 2U);
		}
		else
		{
			num4 = ((num3 > 0f) ? 1U : 3U);
		}
		return (byte)num4;
	}

	// Token: 0x0600281F RID: 10271 RVA: 0x000D521C File Offset: 0x000D341C
	public void GetPieceBumpOffset(byte twist, out sbyte xOffset, out sbyte zOffset)
	{
		if (this.attachIndex == -1 || this.parentPiece == null)
		{
			xOffset = 0;
			zOffset = 0;
			return;
		}
		float gridSize = this.tableOwner.gridSize;
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[this.attachIndex];
		BuilderAttachGridPlane builderAttachGridPlane2 = this.parentPiece.gridPlanes[this.parentAttachIndex];
		bool flag = (long)(twist % 2) == 1L;
		float num = (flag ? builderAttachGridPlane.lengthOffset : builderAttachGridPlane.widthOffset);
		float num2 = (flag ? builderAttachGridPlane.widthOffset : builderAttachGridPlane.lengthOffset);
		float num3 = num - builderAttachGridPlane2.widthOffset;
		float num4 = num2 - builderAttachGridPlane2.lengthOffset;
		Vector3 position = builderAttachGridPlane.center.position;
		Vector3 position2 = builderAttachGridPlane2.center.position;
		Vector3 vector = Quaternion.Inverse(builderAttachGridPlane2.center.rotation) * (position - position2);
		xOffset = (sbyte)Mathf.RoundToInt((vector.x - num3) / gridSize);
		zOffset = (sbyte)Mathf.RoundToInt((vector.z - num4) / gridSize);
	}

	// Token: 0x04003383 RID: 13187
	public const int INVALID = -1;

	// Token: 0x04003384 RID: 13188
	public const float LIGHT_MASS = 1f;

	// Token: 0x04003385 RID: 13189
	public const float HEAVY_MASS = 10000f;

	// Token: 0x04003386 RID: 13190
	[Tooltip("Name for debug text")]
	public string displayName;

	// Token: 0x04003387 RID: 13191
	[Tooltip("(Optional) scriptable object containing material swaps")]
	public BuilderMaterialOptions materialOptions;

	// Token: 0x04003388 RID: 13192
	[Tooltip("Builder Resources used by this object\nbuilderRscBasic for simple meshes\nbuilderRscDecorative for detailed meshes\nbuilderRscFunctional for extra scripts or effects")]
	public BuilderResources cost;

	// Token: 0x04003389 RID: 13193
	[Tooltip("Spawn Offset")]
	public Vector3 desiredShelfOffset = Vector3.zero;

	// Token: 0x0400338A RID: 13194
	[Tooltip("Spawn Offset")]
	public Vector3 desiredShelfRotationOffset = Vector3.zero;

	// Token: 0x0400338B RID: 13195
	[FormerlySerializedAs("vFXInfo")]
	[Tooltip("sounds for block actions. everything uses BuilderPieceEffectInfo_Default")]
	[SerializeField]
	private BuilderPieceEffectInfo fXInfo;

	// Token: 0x0400338C RID: 13196
	private List<MeshRenderer> materialSwapTargets;

	// Token: 0x0400338D RID: 13197
	private List<GorillaSurfaceOverride> surfaceOverrides;

	// Token: 0x0400338E RID: 13198
	[Tooltip("parent object of everything scaled with the piece")]
	public Transform scaleRoot;

	// Token: 0x0400338F RID: 13199
	[Tooltip("Is the block part of the room / immovable (used for the base terrain)")]
	public bool isBuiltIntoTable;

	// Token: 0x04003390 RID: 13200
	public bool isArmShelf;

	// Token: 0x04003391 RID: 13201
	[HideInInspector]
	public BuilderArmShelf armShelf;

	// Token: 0x04003392 RID: 13202
	[Tooltip("Used to prevent log warnings from materials incompatible with the builder renderer\nAnything that needs text/transparency/or particles uses the normal rendering pipeline")]
	public bool suppressMaterialWarnings;

	// Token: 0x04003393 RID: 13203
	[Tooltip("Only used by private plots")]
	private bool isPrivatePlot;

	// Token: 0x04003394 RID: 13204
	[HideInInspector]
	public int privatePlotIndex;

	// Token: 0x04003395 RID: 13205
	[Tooltip("Only used by private plots")]
	public BuilderPiecePrivatePlot plotComponent;

	// Token: 0x04003396 RID: 13206
	[Tooltip("Add piece movement to player movement when touched")]
	public bool attachPlayerToPiece;

	// Token: 0x04003397 RID: 13207
	public int pieceType;

	// Token: 0x04003398 RID: 13208
	public int pieceId;

	// Token: 0x04003399 RID: 13209
	public int pieceDataIndex;

	// Token: 0x0400339A RID: 13210
	public int materialType = -1;

	// Token: 0x0400339B RID: 13211
	public int heldByPlayerActorNumber;

	// Token: 0x0400339C RID: 13212
	public bool heldInLeftHand;

	// Token: 0x0400339D RID: 13213
	public Transform parentHeld;

	// Token: 0x0400339E RID: 13214
	[HideInInspector]
	public BuilderPiece parentPiece;

	// Token: 0x0400339F RID: 13215
	[HideInInspector]
	public BuilderPiece firstChildPiece;

	// Token: 0x040033A0 RID: 13216
	[HideInInspector]
	public BuilderPiece nextSiblingPiece;

	// Token: 0x040033A1 RID: 13217
	[HideInInspector]
	public int attachIndex;

	// Token: 0x040033A2 RID: 13218
	[HideInInspector]
	public int parentAttachIndex;

	// Token: 0x040033A3 RID: 13219
	public int shelfOwner = -1;

	// Token: 0x040033A4 RID: 13220
	[HideInInspector]
	public List<BuilderAttachGridPlane> gridPlanes;

	// Token: 0x040033A5 RID: 13221
	[HideInInspector]
	public List<Collider> colliders;

	// Token: 0x040033A6 RID: 13222
	public List<Collider> placedOnlyColliders;

	// Token: 0x040033A7 RID: 13223
	private int currentColliderLayer = BuilderTable.droppedLayer;

	// Token: 0x040033A8 RID: 13224
	[Tooltip("Components enabled when the block is snapped to the build table")]
	public List<Behaviour> onlyWhenPlacedBehaviours;

	// Token: 0x040033A9 RID: 13225
	[Tooltip("Game objects enabled when the block is snapped to the build table\nAny concave collision should be here")]
	public List<GameObject> onlyWhenPlaced;

	// Token: 0x040033AA RID: 13226
	[Tooltip("Game objects enabled when the block is not snapped to the build table\n Convex collision should be here if there is concave collision when placed")]
	public List<GameObject> onlyWhenNotPlaced;

	// Token: 0x040033AB RID: 13227
	public List<IBuilderPieceComponent> pieceComponents;

	// Token: 0x040033AC RID: 13228
	public IBuilderPieceFunctional functionalPieceComponent;

	// Token: 0x040033AD RID: 13229
	public byte functionalPieceState;

	// Token: 0x040033AE RID: 13230
	public List<IBuilderPieceFunctional> pieceFunctionComponents;

	// Token: 0x040033AF RID: 13231
	private bool pieceComponentsActive;

	// Token: 0x040033B0 RID: 13232
	[Tooltip("Check if any renderers are in the onlyWhenPlaced or onlyWhenNotPlaced lists")]
	public bool areMeshesToggledOnPlace;

	// Token: 0x040033B1 RID: 13233
	[NonSerialized]
	public Rigidbody rigidBody;

	// Token: 0x040033B2 RID: 13234
	[NonSerialized]
	public int activatedTimeStamp;

	// Token: 0x040033B3 RID: 13235
	[HideInInspector]
	public int preventSnapUntilMoved;

	// Token: 0x040033B4 RID: 13236
	[HideInInspector]
	public Vector3 preventSnapUntilMovedFromPos;

	// Token: 0x040033B5 RID: 13237
	[HideInInspector]
	public BuilderPiece requestedParentPiece;

	// Token: 0x040033B6 RID: 13238
	private BuilderTable tableOwner;

	// Token: 0x040033B7 RID: 13239
	public PieceFallbackInfo fallbackInfo;

	// Token: 0x040033B8 RID: 13240
	[NonSerialized]
	public bool overrideSavedPiece;

	// Token: 0x040033B9 RID: 13241
	[NonSerialized]
	public int savedPieceType = -1;

	// Token: 0x040033BA RID: 13242
	[NonSerialized]
	public int savedMaterialType = -1;

	// Token: 0x040033BB RID: 13243
	private float pieceScale;

	// Token: 0x040033BC RID: 13244
	private float[] collisionEnterHistory;

	// Token: 0x040033BD RID: 13245
	private int collisionEnterLimit = 10;

	// Token: 0x040033BE RID: 13246
	private float collisionEnterCooldown = 2f;

	// Token: 0x040033BF RID: 13247
	private int oldCollisionTimeIndex;

	// Token: 0x040033C0 RID: 13248
	[HideInInspector]
	public BuilderPiece.State state;

	// Token: 0x040033C1 RID: 13249
	[HideInInspector]
	public bool isStatic;

	// Token: 0x040033C2 RID: 13250
	[NonSerialized]
	private bool listeningToHandLinks;

	// Token: 0x040033C3 RID: 13251
	[HideInInspector]
	public List<MeshRenderer> renderingDirect;

	// Token: 0x040033C4 RID: 13252
	[HideInInspector]
	public List<MeshRenderer> renderingIndirect;

	// Token: 0x040033C5 RID: 13253
	[HideInInspector]
	public List<int> renderingIndirectTransformIndex;

	// Token: 0x040033C6 RID: 13254
	[HideInInspector]
	public float tint;

	// Token: 0x040033C7 RID: 13255
	private int paintingCount;

	// Token: 0x040033C8 RID: 13256
	private int potentialGrabCount;

	// Token: 0x040033C9 RID: 13257
	private int potentialGrabChildCount;

	// Token: 0x040033CA RID: 13258
	internal bool forcedFrozen;

	// Token: 0x040033CB RID: 13259
	private HashSet<int> collidersEntered = new HashSet<int>(128);

	// Token: 0x040033CC RID: 13260
	private static List<MeshRenderer> tempRenderers = new List<MeshRenderer>(48);

	// Token: 0x02000643 RID: 1603
	public enum State
	{
		// Token: 0x040033CE RID: 13262
		None = -1,
		// Token: 0x040033CF RID: 13263
		AttachedAndPlaced,
		// Token: 0x040033D0 RID: 13264
		AttachedToDropped,
		// Token: 0x040033D1 RID: 13265
		Grabbed,
		// Token: 0x040033D2 RID: 13266
		Dropped,
		// Token: 0x040033D3 RID: 13267
		OnShelf,
		// Token: 0x040033D4 RID: 13268
		Displayed,
		// Token: 0x040033D5 RID: 13269
		GrabbedLocal,
		// Token: 0x040033D6 RID: 13270
		OnConveyor,
		// Token: 0x040033D7 RID: 13271
		AttachedToArm
	}
}
