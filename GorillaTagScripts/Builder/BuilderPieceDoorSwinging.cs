using System;
using BoingKit;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001036 RID: 4150
	public class BuilderPieceDoorSwinging : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x0600675C RID: 26460 RVA: 0x00214140 File Offset: 0x00212340
		private void Awake()
		{
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
			{
				builderSmallMonkeTrigger.onTriggerFirstEntered += this.OnHoldTriggerEntered;
				builderSmallMonkeTrigger.onTriggerLastExited += this.OnHoldTriggerExited;
			}
			this.frontTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnFrontTriggerEntered));
			this.backTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnBackTriggerEntered));
		}

		// Token: 0x0600675D RID: 26461 RVA: 0x002141C0 File Offset: 0x002123C0
		private void OnDestroy()
		{
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
			{
				builderSmallMonkeTrigger.onTriggerFirstEntered -= this.OnHoldTriggerEntered;
				builderSmallMonkeTrigger.onTriggerLastExited -= this.OnHoldTriggerExited;
			}
			this.frontTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnFrontTriggerEntered));
			this.backTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnBackTriggerEntered));
		}

		// Token: 0x0600675E RID: 26462 RVA: 0x00214240 File Offset: 0x00212440
		private void OnFrontTriggerEntered()
		{
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				if (NetworkSystem.Instance.IsMasterClient)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 7, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 7);
			}
		}

		// Token: 0x0600675F RID: 26463 RVA: 0x002142B4 File Offset: 0x002124B4
		private void OnBackTriggerEntered()
		{
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				if (NetworkSystem.Instance.IsMasterClient)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 3);
			}
		}

		// Token: 0x06006760 RID: 26464 RVA: 0x00214328 File Offset: 0x00212528
		private void OnHoldTriggerEntered()
		{
			this.peopleInHoldOpenVolume = true;
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			BuilderPieceDoorSwinging.SwingingDoorState swingingDoorState = this.currentState;
			if (swingingDoorState != BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				if (swingingDoorState == BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut)
				{
					this.openSound.Play();
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				if (swingingDoorState != BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn)
				{
					return;
				}
				this.openSound.Play();
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 8, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x06006761 RID: 26465 RVA: 0x002143D8 File Offset: 0x002125D8
		private void OnHoldTriggerExited()
		{
			this.peopleInHoldOpenVolume = false;
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
			{
				builderSmallMonkeTrigger.ValidateOverlappingColliders();
				if (builderSmallMonkeTrigger.overlapCount > 0)
				{
					this.peopleInHoldOpenVolume = true;
					break;
				}
			}
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn && !this.peopleInHoldOpenVolume)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 5, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				return;
			}
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut && !this.peopleInHoldOpenVolume)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x06006762 RID: 26466 RVA: 0x002144AC File Offset: 0x002126AC
		private void SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState value)
		{
			bool flag = this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed;
			bool flag2 = value == BuilderPieceDoorSwinging.SwingingDoorState.Closed;
			this.currentState = value;
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				this.frontTrigger.enabled = true;
				this.backTrigger.enabled = true;
			}
			else
			{
				this.frontTrigger.enabled = false;
				this.backTrigger.enabled = false;
			}
			if (flag != flag2)
			{
				if (flag2)
				{
					this.myPiece.GetTable().UnregisterFunctionalPiece(this);
					return;
				}
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
		}

		// Token: 0x06006763 RID: 26467 RVA: 0x00214534 File Offset: 0x00212734
		private void UpdateDoorStateMaster()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn:
				if (Mathf.Abs(this.doorSpring.Value) < 1f && Mathf.Abs(this.doorSpring.Velocity) < this.doorClosedVelocityMag)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.Closed);
					return;
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenIn:
				if (Time.time - this.tLastOpened > this.timeUntilDoorCloses)
				{
					this.peopleInHoldOpenVolume = false;
					foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
					{
						builderSmallMonkeTrigger.ValidateOverlappingColliders();
						if (builderSmallMonkeTrigger.overlapCount > 0)
						{
							this.peopleInHoldOpenVolume = true;
							break;
						}
					}
					if (this.peopleInHoldOpenVolume)
					{
						BuilderPieceDoorSwinging.SwingingDoorState swingingDoorState = ((this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.OpenIn) ? BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn : BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut);
						this.checkHoldTriggersTime = (double)(Time.time + this.checkHoldTriggersDelay);
						this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)swingingDoorState, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
						return;
					}
					BuilderPieceDoorSwinging.SwingingDoorState swingingDoorState2 = ((this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.OpenIn) ? BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn : BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut);
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)swingingDoorState2, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut:
				if (Mathf.Abs(this.doorSpring.Value) > 89f)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.OpenOut);
					return;
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn:
				if ((double)Time.time > this.checkHoldTriggersTime)
				{
					foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger2 in this.doorHoldTriggers)
					{
						builderSmallMonkeTrigger2.ValidateOverlappingColliders();
						if (builderSmallMonkeTrigger2.overlapCount > 0)
						{
							this.peopleInHoldOpenVolume = true;
							break;
						}
					}
					if (this.peopleInHoldOpenVolume)
					{
						this.checkHoldTriggersTime = (double)(Time.time + this.checkHoldTriggersDelay);
						return;
					}
					BuilderPieceDoorSwinging.SwingingDoorState swingingDoorState3 = ((this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn) ? BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn : BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut);
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)swingingDoorState3, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn:
				if (Mathf.Abs(this.doorSpring.Value) > 89f)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.OpenIn);
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06006764 RID: 26468 RVA: 0x0021477C File Offset: 0x0021297C
		private void UpdateDoorState()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn:
				if (Mathf.Abs(this.doorSpring.Value) < 1f && Mathf.Abs(this.doorSpring.Velocity) < this.doorClosedVelocityMag)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.Closed);
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenIn:
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut:
				if (Mathf.Abs(this.doorSpring.Value) > 89f)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.OpenOut);
					return;
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn:
				if (Mathf.Abs(this.doorSpring.Value) > 89f)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.OpenIn);
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06006765 RID: 26469 RVA: 0x0021482C File Offset: 0x00212A2C
		private void CloseDoor()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut:
				this.closeSound.Play();
				this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut);
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn:
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenIn:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn:
				this.closeSound.Play();
				this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn);
				return;
			default:
				return;
			}
		}

		// Token: 0x06006766 RID: 26470 RVA: 0x0021488A File Offset: 0x00212A8A
		private void OpenDoor(bool openIn)
		{
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				this.tLastOpened = Time.time;
				this.openSound.Play();
				this.SetDoorState(openIn ? BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn : BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut);
			}
		}

		// Token: 0x06006767 RID: 26471 RVA: 0x002148B8 File Offset: 0x00212AB8
		private void UpdateDoorAnimation()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut:
				this.doorSpring.TrackDampingRatio(-90f, 3.1415927f * this.doorOpenSpeed, 1f, Time.deltaTime);
				this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
				if (this.isDoubleDoor && this.doorTransformB != null)
				{
					this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
					return;
				}
				return;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenIn:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn:
				this.doorSpring.TrackDampingRatio(90f, 3.1415927f * this.doorOpenSpeed, 1f, Time.deltaTime);
				this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
				if (this.isDoubleDoor && this.doorTransformB != null)
				{
					this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
					return;
				}
				return;
			}
			this.doorSpring.TrackDampingRatio(0f, 3.1415927f * this.doorCloseSpeed, this.dampingRatio, Time.deltaTime);
			this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
			if (this.isDoubleDoor && this.doorTransformB != null)
			{
				this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
			}
		}

		// Token: 0x06006768 RID: 26472 RVA: 0x00214AA8 File Offset: 0x00212CA8
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.tLastOpened = 0f;
			this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.Closed);
			this.doorSpring.Reset();
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
		}

		// Token: 0x06006769 RID: 26473 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPieceDestroy()
		{
		}

		// Token: 0x0600676A RID: 26474 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x0600676B RID: 26475 RVA: 0x00214AF0 File Offset: 0x00212CF0
		public void OnPieceActivate()
		{
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
		}

		// Token: 0x0600676C RID: 26476 RVA: 0x00214B1C File Offset: 0x00212D1C
		public void OnPieceDeactivate()
		{
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			this.myPiece.functionalPieceState = 0;
			this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.Closed);
			this.doorSpring.Reset();
			this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
			if (this.isDoubleDoor && this.doorTransformB != null)
			{
				this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
			}
		}

		// Token: 0x0600676D RID: 26477 RVA: 0x00214BC8 File Offset: 0x00212DC8
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			switch (newState)
			{
			case 1:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.OpenOut || this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut)
				{
					this.CloseDoor();
				}
				break;
			case 3:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
				{
					this.OpenDoor(false);
				}
				break;
			case 4:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut)
				{
					this.openSound.Play();
				}
				break;
			case 5:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.OpenIn || this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn)
				{
					this.CloseDoor();
				}
				break;
			case 7:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
				{
					this.OpenDoor(true);
				}
				break;
			case 8:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn)
				{
					this.openSound.Play();
				}
				break;
			}
			this.SetDoorState((BuilderPieceDoorSwinging.SwingingDoorState)newState);
		}

		// Token: 0x0600676E RID: 26478 RVA: 0x00214C98 File Offset: 0x00212E98
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (this.IsStateValid(newState) && instigator != null && (newState == 7 || newState == 3) && this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x0600676F RID: 26479 RVA: 0x00214CF6 File Offset: 0x00212EF6
		public bool IsStateValid(byte state)
		{
			return state <= 8;
		}

		// Token: 0x06006770 RID: 26480 RVA: 0x00214D00 File Offset: 0x00212F00
		public void FunctionalPieceUpdate()
		{
			if (this.myPiece != null && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced)
			{
				if (!NetworkSystem.Instance.InRoom && this.currentState != BuilderPieceDoorSwinging.SwingingDoorState.Closed)
				{
					this.CloseDoor();
				}
				else if (NetworkSystem.Instance.IsMasterClient)
				{
					this.UpdateDoorStateMaster();
				}
				else
				{
					this.UpdateDoorState();
				}
				this.UpdateDoorAnimation();
			}
		}

		// Token: 0x0400766A RID: 30314
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x0400766B RID: 30315
		[SerializeField]
		private Vector3 rotateAxis = Vector3.up;

		// Token: 0x0400766C RID: 30316
		[SerializeField]
		private Transform doorTransform;

		// Token: 0x0400766D RID: 30317
		[SerializeField]
		private Collider[] triggerVolumes;

		// Token: 0x0400766E RID: 30318
		[SerializeField]
		private BuilderSmallMonkeTrigger[] doorHoldTriggers;

		// Token: 0x0400766F RID: 30319
		[SerializeField]
		private BuilderSmallHandTrigger frontTrigger;

		// Token: 0x04007670 RID: 30320
		[SerializeField]
		private BuilderSmallHandTrigger backTrigger;

		// Token: 0x04007671 RID: 30321
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04007672 RID: 30322
		[SerializeField]
		private SoundBankPlayer openSound;

		// Token: 0x04007673 RID: 30323
		[SerializeField]
		private SoundBankPlayer closeSound;

		// Token: 0x04007674 RID: 30324
		[SerializeField]
		private float doorOpenSpeed = 1f;

		// Token: 0x04007675 RID: 30325
		[SerializeField]
		private float doorCloseSpeed = 1f;

		// Token: 0x04007676 RID: 30326
		[SerializeField]
		[Range(1.5f, 10f)]
		private float timeUntilDoorCloses = 3f;

		// Token: 0x04007677 RID: 30327
		[SerializeField]
		private float doorClosedVelocityMag = 30f;

		// Token: 0x04007678 RID: 30328
		[SerializeField]
		private float dampingRatio = 0.5f;

		// Token: 0x04007679 RID: 30329
		[Header("Double Door Settings")]
		[SerializeField]
		private bool isDoubleDoor;

		// Token: 0x0400767A RID: 30330
		[SerializeField]
		private Vector3 rotateAxisB = Vector3.down;

		// Token: 0x0400767B RID: 30331
		[SerializeField]
		private Transform doorTransformB;

		// Token: 0x0400767C RID: 30332
		private BuilderPieceDoorSwinging.SwingingDoorState currentState;

		// Token: 0x0400767D RID: 30333
		private float tLastOpened;

		// Token: 0x0400767E RID: 30334
		private FloatSpring doorSpring;

		// Token: 0x0400767F RID: 30335
		private bool peopleInHoldOpenVolume;

		// Token: 0x04007680 RID: 30336
		private double checkHoldTriggersTime;

		// Token: 0x04007681 RID: 30337
		private float checkHoldTriggersDelay = 3f;

		// Token: 0x04007682 RID: 30338
		private int pushDirection = 1;

		// Token: 0x02001037 RID: 4151
		private enum SwingingDoorState
		{
			// Token: 0x04007684 RID: 30340
			Closed,
			// Token: 0x04007685 RID: 30341
			ClosingOut,
			// Token: 0x04007686 RID: 30342
			OpenOut,
			// Token: 0x04007687 RID: 30343
			OpeningOut,
			// Token: 0x04007688 RID: 30344
			HeldOpenOut,
			// Token: 0x04007689 RID: 30345
			ClosingIn,
			// Token: 0x0400768A RID: 30346
			OpenIn,
			// Token: 0x0400768B RID: 30347
			OpeningIn,
			// Token: 0x0400768C RID: 30348
			HeldOpenIn
		}
	}
}
