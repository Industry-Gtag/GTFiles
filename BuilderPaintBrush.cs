using System;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000618 RID: 1560
public class BuilderPaintBrush : HoldableObject
{
	// Token: 0x06002700 RID: 9984 RVA: 0x000CE49C File Offset: 0x000CC69C
	private void Awake()
	{
		this.pieceLayers |= 1 << LayerMask.NameToLayer("Gorilla Object");
		this.pieceLayers |= 1 << LayerMask.NameToLayer("BuilderProp");
		this.pieceLayers |= 1 << LayerMask.NameToLayer("Prop");
		this.paintDistance = Vector3.SqrMagnitude(this.paintVolumeHalfExtents);
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06002701 RID: 9985 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void DropItemCleanup()
	{
	}

	// Token: 0x06002702 RID: 9986 RVA: 0x000CE538 File Offset: 0x000CC738
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		this.holdingHand = grabbingHand;
		this.handVelocity = grabbingHand.GetComponent<GorillaVelocityTracker>();
		if (this.handVelocity == null)
		{
			Debug.Log("No Velocity Estimator");
		}
		this.inLeftHand = grabbingHand == EquipmentInteractor.instance.leftHand;
		BodyDockPositions myBodyDockPositions = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions;
		this.rb.isKinematic = true;
		this.rb.useGravity = false;
		if (this.inLeftHand)
		{
			base.transform.SetParent(myBodyDockPositions.leftHandTransform, true);
		}
		else
		{
			base.transform.SetParent(myBodyDockPositions.rightHandTransform, true);
		}
		base.transform.localScale = Vector3.one;
		EquipmentInteractor.instance.UpdateHandEquipment(this, this.inLeftHand);
		GorillaTagger.Instance.StartVibration(this.inLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		this.brushState = BuilderPaintBrush.PaintBrushState.Held;
	}

	// Token: 0x06002703 RID: 9987 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06002704 RID: 9988 RVA: 0x000CE638 File Offset: 0x000CC838
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (base.OnRelease(zoneReleased, releasingHand))
		{
			this.holdingHand = null;
			EquipmentInteractor.instance.UpdateHandEquipment(null, this.inLeftHand);
			this.inLeftHand = false;
			this.handVelocity = null;
			this.ClearHoveredPiece();
			base.transform.parent = null;
			base.transform.localScale = Vector3.one;
			this.rb.isKinematic = false;
			this.rb.linearVelocity = Vector3.zero;
			this.rb.angularVelocity = Vector3.zero;
			this.rb.useGravity = true;
			return true;
		}
		return false;
	}

	// Token: 0x06002705 RID: 9989 RVA: 0x000CE6D7 File Offset: 0x000CC8D7
	private void LateUpdate()
	{
		if (this.brushState == BuilderPaintBrush.PaintBrushState.Inactive)
		{
			return;
		}
		if (this.holdingHand == null || this.materialType == -1)
		{
			this.brushState = BuilderPaintBrush.PaintBrushState.Inactive;
			return;
		}
		this.FindPieceToPaint();
	}

	// Token: 0x06002706 RID: 9990 RVA: 0x000CE708 File Offset: 0x000CC908
	private void FindPieceToPaint()
	{
		switch (this.brushState)
		{
		case BuilderPaintBrush.PaintBrushState.Held:
		{
			if (this.materialType == -1)
			{
				return;
			}
			Array.Clear(this.hitColliders, 0, this.hitColliders.Length);
			int num = Physics.OverlapBoxNonAlloc(this.brushSurface.transform.position - this.brushSurface.up * this.paintVolumeHalfExtents.y, this.paintVolumeHalfExtents, this.hitColliders, this.brushSurface.transform.rotation, this.pieceLayers, QueryTriggerInteraction.Ignore);
			BuilderPieceCollider builderPieceCollider = null;
			Collider collider = null;
			float num2 = float.MaxValue;
			for (int i = 0; i < num; i++)
			{
				BuilderPieceCollider component = this.hitColliders[i].GetComponent<BuilderPieceCollider>();
				if (component != null && component.piece.materialType != this.materialType && component.piece.materialType != -1)
				{
					float sqrMagnitude = (this.brushSurface.transform.position - component.transform.position).sqrMagnitude;
					if (sqrMagnitude < num2 && component.piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, component.piece.transform.position))
					{
						num2 = sqrMagnitude;
						builderPieceCollider = component;
						collider = this.hitColliders[i];
					}
				}
			}
			if (builderPieceCollider != null)
			{
				this.ClearHoveredPiece();
				this.hoveredPiece = builderPieceCollider.piece;
				this.hoveredPieceCollider = collider;
				this.hoveredPiece.PaintingTint(true);
				GorillaTagger.Instance.StartVibration(this.inLeftHand, GorillaTagger.Instance.tapHapticStrength / 4f, GorillaTagger.Instance.tapHapticDuration);
				this.positionDelta = 0f;
				this.lastPosition = this.brushSurface.transform.position;
				this.brushState = BuilderPaintBrush.PaintBrushState.Hover;
				return;
			}
			break;
		}
		case BuilderPaintBrush.PaintBrushState.Hover:
		{
			if (this.hoveredPiece == null || this.hoveredPieceCollider == null)
			{
				this.ClearHoveredPiece();
				return;
			}
			float sqrMagnitude2 = this.handVelocity.GetLatestVelocity(false).sqrMagnitude;
			float sqrMagnitude3 = this.handVelocity.GetAverageVelocity(false, 0.15f, false).sqrMagnitude;
			if (this.handVelocity != null && (sqrMagnitude2 > this.maxPaintVelocitySqrMag || sqrMagnitude3 > this.maxPaintVelocitySqrMag))
			{
				this.ClearHoveredPiece();
				return;
			}
			Vector3 vector = this.brushSurface.position - this.brushSurface.up * this.paintVolumeHalfExtents.y;
			Vector3 vector2 = this.hoveredPieceCollider.ClosestPointOnBounds(vector);
			if (Vector3.SqrMagnitude(vector - vector2) > this.paintDistance)
			{
				this.ClearHoveredPiece();
				return;
			}
			GorillaTagger.Instance.StartVibration(this.inLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, Time.deltaTime);
			float num3 = Vector3.Distance(this.lastPosition, this.brushSurface.position);
			if (num3 < this.minimumWiggleFrameDistance)
			{
				this.lastPosition = this.brushSurface.position;
				return;
			}
			this.positionDelta += Math.Min(num3, this.maximumWiggleFrameDistance);
			this.lastPosition = this.brushSurface.position;
			if (this.positionDelta >= this.wiggleDistanceRequirement)
			{
				this.positionDelta = 0f;
				this.audioSource.clip = this.paintSound;
				this.audioSource.GTPlay();
				this.PaintPiece();
				this.brushState = BuilderPaintBrush.PaintBrushState.JustPainted;
				return;
			}
			break;
		}
		case BuilderPaintBrush.PaintBrushState.JustPainted:
			if (this.paintTimeElapsed > this.paintDelay)
			{
				this.paintTimeElapsed = 0f;
				this.brushState = BuilderPaintBrush.PaintBrushState.Held;
				return;
			}
			this.paintTimeElapsed += Time.deltaTime;
			break;
		default:
			return;
		}
	}

	// Token: 0x06002707 RID: 9991 RVA: 0x000CEAD0 File Offset: 0x000CCCD0
	private void PaintPiece()
	{
		this.hoveredPiece.GetTable().RequestPaintPiece(this.hoveredPiece.pieceId, this.materialType);
		this.hoveredPiece.PaintingTint(false);
		this.hoveredPiece = null;
		this.hoveredPieceCollider = null;
		this.paintTimeElapsed = 0f;
		GorillaTagger.Instance.StartVibration(this.inLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
	}

	// Token: 0x06002708 RID: 9992 RVA: 0x000CEB50 File Offset: 0x000CCD50
	private void ClearHoveredPiece()
	{
		if (this.hoveredPiece != null)
		{
			this.hoveredPiece.PaintingTint(false);
		}
		this.hoveredPiece = null;
		this.hoveredPieceCollider = null;
		this.positionDelta = 0f;
		this.brushState = ((this.holdingHand == null || this.materialType == -1) ? BuilderPaintBrush.PaintBrushState.Inactive : BuilderPaintBrush.PaintBrushState.Held);
	}

	// Token: 0x06002709 RID: 9993 RVA: 0x000CEBB4 File Offset: 0x000CCDB4
	public void SetBrushMaterial(int inMaterialType)
	{
		this.materialType = inMaterialType;
		this.audioSource.clip = this.paintSound;
		this.audioSource.GTPlay();
		if (this.holdingHand != null)
		{
			GorillaTagger.Instance.StartVibration(this.inLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
		if (this.materialType == -1)
		{
			this.ClearHoveredPiece();
		}
		else if (this.brushState == BuilderPaintBrush.PaintBrushState.Inactive && this.holdingHand != null)
		{
			this.brushState = BuilderPaintBrush.PaintBrushState.Held;
		}
		if (this.paintBrushMaterialOptions != null && this.brushRenderer != null)
		{
			Material material;
			int num;
			this.paintBrushMaterialOptions.GetMaterialFromType(this.materialType, out material, out num);
			if (material != null)
			{
				this.brushRenderer.material = material;
			}
		}
	}

	// Token: 0x04003273 RID: 12915
	[SerializeField]
	private Transform brushSurface;

	// Token: 0x04003274 RID: 12916
	[SerializeField]
	private Vector3 paintVolumeHalfExtents;

	// Token: 0x04003275 RID: 12917
	[SerializeField]
	private BuilderMaterialOptions paintBrushMaterialOptions;

	// Token: 0x04003276 RID: 12918
	[SerializeField]
	private MeshRenderer brushRenderer;

	// Token: 0x04003277 RID: 12919
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003278 RID: 12920
	[SerializeField]
	private AudioClip paintSound;

	// Token: 0x04003279 RID: 12921
	[SerializeField]
	private AudioClip brushStrokeSound;

	// Token: 0x0400327A RID: 12922
	private GameObject holdingHand;

	// Token: 0x0400327B RID: 12923
	private bool inLeftHand;

	// Token: 0x0400327C RID: 12924
	private GorillaVelocityTracker handVelocity;

	// Token: 0x0400327D RID: 12925
	private BuilderPiece hoveredPiece;

	// Token: 0x0400327E RID: 12926
	private Collider hoveredPieceCollider;

	// Token: 0x0400327F RID: 12927
	private Collider[] hitColliders = new Collider[16];

	// Token: 0x04003280 RID: 12928
	private LayerMask pieceLayers = 0;

	// Token: 0x04003281 RID: 12929
	private Vector3 lastPosition = Vector3.zero;

	// Token: 0x04003282 RID: 12930
	private float positionDelta;

	// Token: 0x04003283 RID: 12931
	private float wiggleDistanceRequirement = 0.08f;

	// Token: 0x04003284 RID: 12932
	private float minimumWiggleFrameDistance = 0.005f;

	// Token: 0x04003285 RID: 12933
	private float maximumWiggleFrameDistance = 0.04f;

	// Token: 0x04003286 RID: 12934
	private float maxPaintVelocitySqrMag = 0.5f;

	// Token: 0x04003287 RID: 12935
	private float paintDelay = 0.2f;

	// Token: 0x04003288 RID: 12936
	private float paintTimeElapsed = -1f;

	// Token: 0x04003289 RID: 12937
	private float paintDistance;

	// Token: 0x0400328A RID: 12938
	private int materialType = -1;

	// Token: 0x0400328B RID: 12939
	private BuilderPaintBrush.PaintBrushState brushState;

	// Token: 0x0400328C RID: 12940
	private Rigidbody rb;

	// Token: 0x02000619 RID: 1561
	public enum PaintBrushState
	{
		// Token: 0x0400328E RID: 12942
		Inactive,
		// Token: 0x0400328F RID: 12943
		HeldRemote,
		// Token: 0x04003290 RID: 12944
		Held,
		// Token: 0x04003291 RID: 12945
		Hover,
		// Token: 0x04003292 RID: 12946
		JustPainted
	}
}
