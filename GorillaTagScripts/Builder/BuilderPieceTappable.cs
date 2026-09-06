using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x0200103A RID: 4154
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(GorillaSurfaceOverride))]
	public class BuilderPieceTappable : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional, IBuilderTappable
	{
		// Token: 0x06006781 RID: 26497 RVA: 0x00214FFA File Offset: 0x002131FA
		public virtual bool CanTap()
		{
			return this.isPieceActive && Time.time > this.lastTapTime + this.tapCooldown;
		}

		// Token: 0x06006782 RID: 26498 RVA: 0x0021501A File Offset: 0x0021321A
		public void OnTapLocal(float tapStrength)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			if (!this.CanTap())
			{
				return;
			}
			this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 1);
		}

		// Token: 0x06006783 RID: 26499 RVA: 0x00215053 File Offset: 0x00213253
		public virtual void OnTapReplicated()
		{
			UnityEvent onTapped = this.OnTapped;
			if (onTapped == null)
			{
				return;
			}
			onTapped.Invoke();
		}

		// Token: 0x06006784 RID: 26500 RVA: 0x00215065 File Offset: 0x00213265
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.currentState = BuilderPieceTappable.FunctionalState.Idle;
		}

		// Token: 0x06006785 RID: 26501 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06006786 RID: 26502 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06006787 RID: 26503 RVA: 0x0021506E File Offset: 0x0021326E
		public void OnPieceActivate()
		{
			this.isPieceActive = true;
		}

		// Token: 0x06006788 RID: 26504 RVA: 0x00215078 File Offset: 0x00213278
		public void OnPieceDeactivate()
		{
			this.isPieceActive = false;
			if (this.currentState == BuilderPieceTappable.FunctionalState.Tap)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x06006789 RID: 26505 RVA: 0x002150C8 File Offset: 0x002132C8
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (newState == 1 && this.currentState != BuilderPieceTappable.FunctionalState.Tap)
			{
				this.lastTapTime = Time.time;
				this.OnTapReplicated();
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
			this.currentState = (BuilderPieceTappable.FunctionalState)newState;
		}

		// Token: 0x0600678A RID: 26506 RVA: 0x00215118 File Offset: 0x00213318
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.IsStateValid(newState) || instigator == null)
			{
				return;
			}
			if (newState == 1 && this.CanTap())
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x0600678B RID: 26507 RVA: 0x00215173 File Offset: 0x00213373
		public bool IsStateValid(byte state)
		{
			return state <= 1;
		}

		// Token: 0x0600678C RID: 26508 RVA: 0x0021517C File Offset: 0x0021337C
		public void FunctionalPieceUpdate()
		{
			if (this.lastTapTime + this.tapCooldown < Time.time)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x04007692 RID: 30354
		[SerializeField]
		protected BuilderPiece myPiece;

		// Token: 0x04007693 RID: 30355
		[SerializeField]
		protected float tapCooldown = 0.5f;

		// Token: 0x04007694 RID: 30356
		private bool isPieceActive;

		// Token: 0x04007695 RID: 30357
		private float lastTapTime;

		// Token: 0x04007696 RID: 30358
		private BuilderPieceTappable.FunctionalState currentState;

		// Token: 0x04007697 RID: 30359
		[Tooltip("Called on all clients when this collider is tapped by anyone")]
		[SerializeField]
		protected UnityEvent OnTapped;

		// Token: 0x0200103B RID: 4155
		private enum FunctionalState
		{
			// Token: 0x04007699 RID: 30361
			Idle,
			// Token: 0x0400769A RID: 30362
			Tap
		}
	}
}
