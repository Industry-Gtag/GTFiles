using System;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x0200103D RID: 4157
	public class BuilderPieceToggle : MonoBehaviour, IBuilderPieceFunctional, IBuilderPieceComponent, IBuilderTappable
	{
		// Token: 0x0600679D RID: 26525 RVA: 0x00215674 File Offset: 0x00213874
		private void Awake()
		{
			this.colliders.Clear();
			if (this.toggleType == BuilderPieceToggle.ToggleType.OnTriggerEnter)
			{
				foreach (BuilderSmallHandTrigger builderSmallHandTrigger in this.handTriggers)
				{
					builderSmallHandTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnHandTriggerEntered));
					Collider component = builderSmallHandTrigger.GetComponent<Collider>();
					if (component != null)
					{
						this.colliders.Add(component);
					}
				}
				foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.bodyTriggers)
				{
					builderSmallMonkeTrigger.onPlayerEnteredTrigger += this.OnBodyTriggerEntered;
					Collider component2 = builderSmallMonkeTrigger.GetComponent<Collider>();
					if (component2 != null)
					{
						this.colliders.Add(component2);
					}
				}
			}
		}

		// Token: 0x0600679E RID: 26526 RVA: 0x0021572C File Offset: 0x0021392C
		private void OnDestroy()
		{
			foreach (BuilderSmallHandTrigger builderSmallHandTrigger in this.handTriggers)
			{
				if (!(builderSmallHandTrigger == null))
				{
					builderSmallHandTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnHandTriggerEntered));
				}
			}
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.bodyTriggers)
			{
				if (!(builderSmallMonkeTrigger == null))
				{
					builderSmallMonkeTrigger.onPlayerEnteredTrigger -= this.OnBodyTriggerEntered;
				}
			}
		}

		// Token: 0x0600679F RID: 26527 RVA: 0x002157AC File Offset: 0x002139AC
		private bool CanTap()
		{
			return (!this.onlySmallMonkeTaps || !this.myPiece.GetTable().isTableMutable || (double)VRRigCache.Instance.localRig.Rig.scaleFactor <= 0.99) && this.toggleType == BuilderPieceToggle.ToggleType.OnTap && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced;
		}

		// Token: 0x060067A0 RID: 26528 RVA: 0x0021580D File Offset: 0x00213A0D
		public void OnTapLocal(float tapStrength)
		{
			if (!this.CanTap())
			{
				Debug.Log("BuilderPieceToggle Can't Tap");
				return;
			}
			Debug.Log("Tap Local");
			this.ToggleStateRequest();
		}

		// Token: 0x060067A1 RID: 26529 RVA: 0x00215832 File Offset: 0x00213A32
		private bool CanTrigger()
		{
			return this.toggleType == BuilderPieceToggle.ToggleType.OnTriggerEnter && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced;
		}

		// Token: 0x060067A2 RID: 26530 RVA: 0x0021584D File Offset: 0x00213A4D
		private void OnHandTriggerEntered()
		{
			if (this.CanTrigger())
			{
				this.ToggleStateRequest();
				return;
			}
			Debug.Log("BuilderPieceToggle Can't Trigger");
		}

		// Token: 0x060067A3 RID: 26531 RVA: 0x00215868 File Offset: 0x00213A68
		private void OnBodyTriggerEntered(int playerNumber)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(playerNumber);
			if (player == null)
			{
				return;
			}
			if (this.CanTrigger())
			{
				this.ToggleStateMaster(player.GetPlayerRef());
				return;
			}
			Debug.Log("BuilderPieceToggle Can't Trigger");
		}

		// Token: 0x060067A4 RID: 26532 RVA: 0x002158B4 File Offset: 0x00213AB4
		private void ToggleStateRequest()
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			BuilderPieceToggle.ToggleStates toggleStates = ((this.toggleState == BuilderPieceToggle.ToggleStates.Off) ? BuilderPieceToggle.ToggleStates.On : BuilderPieceToggle.ToggleStates.Off);
			Debug.Log("BuilderPieceToggle" + string.Format(" Requesting state {0}", toggleStates));
			this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, (byte)toggleStates);
		}

		// Token: 0x060067A5 RID: 26533 RVA: 0x0021591C File Offset: 0x00213B1C
		private void ToggleStateMaster(Player instigator)
		{
			BuilderPieceToggle.ToggleStates toggleStates = ((this.toggleState == BuilderPieceToggle.ToggleStates.Off) ? BuilderPieceToggle.ToggleStates.On : BuilderPieceToggle.ToggleStates.Off);
			Debug.Log("BuilderPieceToggle" + string.Format(" Set Master state {0}", toggleStates));
			this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)toggleStates, instigator, NetworkSystem.Instance.ServerTimestamp);
		}

		// Token: 0x060067A6 RID: 26534 RVA: 0x00215984 File Offset: 0x00213B84
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				Debug.Log("BuilderPieceToggle State Invalid");
				return;
			}
			Debug.Log("BuilderPieceToggle" + string.Format(" State Changed {0}", newState));
			if ((BuilderPieceToggle.ToggleStates)newState != this.toggleState)
			{
				if (newState == 1)
				{
					Debug.Log("BuilderPieceToggle Toggled On");
					UnityEvent toggledOn = this.ToggledOn;
					if (toggledOn != null)
					{
						toggledOn.Invoke();
					}
				}
				else
				{
					Debug.Log("BuilderPieceToggle Toggled Off");
					this.ToggledOff.Invoke();
				}
			}
			this.toggleState = (BuilderPieceToggle.ToggleStates)newState;
		}

		// Token: 0x060067A7 RID: 26535 RVA: 0x00215A0C File Offset: 0x00213C0C
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.IsStateValid(newState) || instigator == null)
			{
				Debug.Log("BuilderPieceToggle State Invalid or Player Null");
				return;
			}
			Debug.Log("BuilderPieceToggle" + string.Format(" State Request {0}", newState));
			if (newState != (byte)this.toggleState)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
				return;
			}
			Debug.Log("BuilderPieceToggle Same State");
		}

		// Token: 0x060067A8 RID: 26536 RVA: 0x00215A99 File Offset: 0x00213C99
		public bool IsStateValid(byte state)
		{
			Debug.Log(string.Format("Is State Valid? {0}", state));
			return state <= 1;
		}

		// Token: 0x060067A9 RID: 26537 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void FunctionalPieceUpdate()
		{
		}

		// Token: 0x060067AA RID: 26538 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x060067AB RID: 26539 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPieceDestroy()
		{
		}

		// Token: 0x060067AC RID: 26540 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x060067AD RID: 26541 RVA: 0x00215AB8 File Offset: 0x00213CB8
		public void OnPieceActivate()
		{
			foreach (Collider collider in this.colliders)
			{
				collider.enabled = true;
			}
		}

		// Token: 0x060067AE RID: 26542 RVA: 0x00215B0C File Offset: 0x00213D0C
		public void OnPieceDeactivate()
		{
			this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			foreach (Collider collider in this.colliders)
			{
				collider.enabled = false;
			}
		}

		// Token: 0x040076A6 RID: 30374
		[SerializeField]
		protected BuilderPiece myPiece;

		// Token: 0x040076A7 RID: 30375
		[SerializeField]
		private BuilderPieceToggle.ToggleType toggleType;

		// Token: 0x040076A8 RID: 30376
		public bool onlySmallMonkeTaps;

		// Token: 0x040076A9 RID: 30377
		[SerializeField]
		private BuilderSmallHandTrigger[] handTriggers;

		// Token: 0x040076AA RID: 30378
		[SerializeField]
		private BuilderSmallMonkeTrigger[] bodyTriggers;

		// Token: 0x040076AB RID: 30379
		[SerializeField]
		protected UnityEvent ToggledOn;

		// Token: 0x040076AC RID: 30380
		[SerializeField]
		protected UnityEvent ToggledOff;

		// Token: 0x040076AD RID: 30381
		private List<Collider> colliders = new List<Collider>(5);

		// Token: 0x040076AE RID: 30382
		private BuilderPieceToggle.ToggleStates toggleState;

		// Token: 0x0200103E RID: 4158
		[Serializable]
		private enum ToggleType
		{
			// Token: 0x040076B0 RID: 30384
			OnTap,
			// Token: 0x040076B1 RID: 30385
			OnTriggerEnter
		}

		// Token: 0x0200103F RID: 4159
		private enum ToggleStates
		{
			// Token: 0x040076B3 RID: 30387
			Off,
			// Token: 0x040076B4 RID: 30388
			On
		}
	}
}
