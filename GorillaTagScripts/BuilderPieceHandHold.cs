using System;
using System.Collections.Generic;
using GorillaLocomotion.Gameplay;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F4E RID: 3918
	[RequireComponent(typeof(Collider))]
	public class BuilderPieceHandHold : MonoBehaviour, IGorillaGrabable, IBuilderPieceComponent, ITickSystemTick
	{
		// Token: 0x06006043 RID: 24643 RVA: 0x001E8755 File Offset: 0x001E6955
		private void Initialize()
		{
			if (this.initialized)
			{
				return;
			}
			this.myCollider = base.GetComponent<Collider>();
			this.initialized = true;
		}

		// Token: 0x06006044 RID: 24644 RVA: 0x001E8773 File Offset: 0x001E6973
		public bool IsHandHoldMoving()
		{
			return this.myPiece.IsPieceMoving();
		}

		// Token: 0x06006045 RID: 24645 RVA: 0x001E8780 File Offset: 0x001E6980
		public bool MomentaryGrabOnly()
		{
			return this.forceMomentary;
		}

		// Token: 0x06006046 RID: 24646 RVA: 0x001E8788 File Offset: 0x001E6988
		public virtual bool CanBeGrabbed(GorillaGrabber grabber)
		{
			return this.myPiece.state == BuilderPiece.State.AttachedAndPlaced && (!this.myPiece.GetTable().isTableMutable || grabber.Player.scale < 0.5f);
		}

		// Token: 0x06006047 RID: 24647 RVA: 0x001E87C0 File Offset: 0x001E69C0
		public void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
		{
			this.Initialize();
			grabbedTransform = base.transform;
			Vector3 position = grabber.transform.position;
			localGrabbedPosition = base.transform.InverseTransformPoint(position);
			this.activeGrabbers.Add(grabber);
			this.isGrabbed = true;
			Vector3 vector;
			grabber.Player.AddHandHold(base.transform, localGrabbedPosition, grabber, grabber.IsRightHand, false, out vector);
		}

		// Token: 0x06006048 RID: 24648 RVA: 0x001E882D File Offset: 0x001E6A2D
		public void OnGrabReleased(GorillaGrabber grabber)
		{
			this.Initialize();
			this.activeGrabbers.Remove(grabber);
			this.isGrabbed = this.activeGrabbers.Count < 1;
			grabber.Player.RemoveHandHold(grabber, grabber.IsRightHand);
		}

		// Token: 0x1700095C RID: 2396
		// (get) Token: 0x06006049 RID: 24649 RVA: 0x001E8868 File Offset: 0x001E6A68
		// (set) Token: 0x0600604A RID: 24650 RVA: 0x001E8870 File Offset: 0x001E6A70
		public bool TickRunning { get; set; }

		// Token: 0x0600604B RID: 24651 RVA: 0x001E887C File Offset: 0x001E6A7C
		public void Tick()
		{
			if (!this.isGrabbed)
			{
				return;
			}
			foreach (GorillaGrabber gorillaGrabber in this.activeGrabbers)
			{
				if (gorillaGrabber != null && gorillaGrabber.Player.scale > 0.5f)
				{
					this.OnGrabReleased(gorillaGrabber);
				}
			}
		}

		// Token: 0x0600604C RID: 24652 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x0600604D RID: 24653 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPieceDestroy()
		{
		}

		// Token: 0x0600604E RID: 24654 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x0600604F RID: 24655 RVA: 0x001E88F4 File Offset: 0x001E6AF4
		public void OnPieceActivate()
		{
			if (!this.TickRunning && this.myPiece.GetTable().isTableMutable)
			{
				TickSystem<object>.AddCallbackTarget(this);
			}
		}

		// Token: 0x06006050 RID: 24656 RVA: 0x001E8918 File Offset: 0x001E6B18
		public void OnPieceDeactivate()
		{
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveCallbackTarget(this);
			}
			foreach (GorillaGrabber gorillaGrabber in this.activeGrabbers)
			{
				this.OnGrabReleased(gorillaGrabber);
			}
		}

		// Token: 0x06006052 RID: 24658 RVA: 0x00014B5B File Offset: 0x00012D5B
		string IGorillaGrabable.get_name()
		{
			return base.name;
		}

		// Token: 0x04006EDF RID: 28383
		private bool initialized;

		// Token: 0x04006EE0 RID: 28384
		private Collider myCollider;

		// Token: 0x04006EE1 RID: 28385
		[SerializeField]
		private bool forceMomentary = true;

		// Token: 0x04006EE2 RID: 28386
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04006EE3 RID: 28387
		private List<GorillaGrabber> activeGrabbers = new List<GorillaGrabber>(2);

		// Token: 0x04006EE4 RID: 28388
		private bool isGrabbed;
	}
}
