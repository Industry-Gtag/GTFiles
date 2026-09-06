using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001045 RID: 4165
	public class BuilderReplicatedTriggerEnter : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x060067DC RID: 26588 RVA: 0x002168C0 File Offset: 0x00214AC0
		private void Awake()
		{
			this.colliders.Clear();
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

		// Token: 0x060067DD RID: 26589 RVA: 0x0021696C File Offset: 0x00214B6C
		private void OnDestroy()
		{
			BuilderSmallHandTrigger[] array = this.handTriggers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].TriggeredEvent.RemoveListener(new UnityAction(this.OnHandTriggerEntered));
			}
			BuilderSmallMonkeTrigger[] array2 = this.bodyTriggers;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].onPlayerEnteredTrigger -= this.OnBodyTriggerEntered;
			}
		}

		// Token: 0x060067DE RID: 26590 RVA: 0x002169D0 File Offset: 0x00214BD0
		private void PlayTriggerEffects(NetPlayer target)
		{
			UnityEvent onTriggered = this.OnTriggered;
			if (onTriggered != null)
			{
				onTriggered.Invoke();
			}
			if (this.animationOnTrigger != null && this.animationOnTrigger.clip != null)
			{
				this.animationOnTrigger.Rewind();
				this.animationOnTrigger.Play();
			}
			if (this.activateSoundBank != null)
			{
				this.activateSoundBank.Play();
			}
			if (target.IsLocal)
			{
				VRRig rig = VRRigCache.Instance.localRig.Rig;
				if (rig != null)
				{
					float num = 1.5f * rig.scaleFactor;
					if ((rig.transform.position - base.transform.position).sqrMagnitude > num * num)
					{
						return;
					}
					GTPlayer.Instance.SetMaximumSlipThisFrame();
					GTPlayer.Instance.ApplyKnockback(this.knockbackDirection.forward, this.knockbackVelocity * rig.scaleFactor, false);
					GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
					GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
				}
			}
		}

		// Token: 0x060067DF RID: 26591 RVA: 0x00216B09 File Offset: 0x00214D09
		private void OnHandTriggerEntered()
		{
			if (this.CanTrigger())
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 1);
			}
		}

		// Token: 0x060067E0 RID: 26592 RVA: 0x00216B34 File Offset: 0x00214D34
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
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, player.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x060067E1 RID: 26593 RVA: 0x00216B97 File Offset: 0x00214D97
		private bool CanTrigger()
		{
			return this.isPieceActive && this.currentState == BuilderReplicatedTriggerEnter.FunctionalState.Idle && Time.time > this.lastTriggerTime + this.triggerCooldown;
		}

		// Token: 0x060067E2 RID: 26594 RVA: 0x00216BBF File Offset: 0x00214DBF
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.currentState = BuilderReplicatedTriggerEnter.FunctionalState.Idle;
		}

		// Token: 0x060067E3 RID: 26595 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPieceDestroy()
		{
		}

		// Token: 0x060067E4 RID: 26596 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x060067E5 RID: 26597 RVA: 0x00216BC8 File Offset: 0x00214DC8
		public void OnPieceActivate()
		{
			this.isPieceActive = true;
			foreach (Collider collider in this.colliders)
			{
				collider.enabled = true;
			}
		}

		// Token: 0x060067E6 RID: 26598 RVA: 0x00216C20 File Offset: 0x00214E20
		public void OnPieceDeactivate()
		{
			this.isPieceActive = false;
			if (this.currentState == BuilderReplicatedTriggerEnter.FunctionalState.TriggerEntered)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
			foreach (Collider collider in this.colliders)
			{
				collider.enabled = false;
			}
		}

		// Token: 0x060067E7 RID: 26599 RVA: 0x00216CB4 File Offset: 0x00214EB4
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (newState == 1 && this.currentState != BuilderReplicatedTriggerEnter.FunctionalState.TriggerEntered)
			{
				this.lastTriggerTime = Time.time;
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
				this.PlayTriggerEffects(instigator);
			}
			this.currentState = (BuilderReplicatedTriggerEnter.FunctionalState)newState;
		}

		// Token: 0x060067E8 RID: 26600 RVA: 0x00216D04 File Offset: 0x00214F04
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
			if (newState == 1 && this.CanTrigger())
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x060067E9 RID: 26601 RVA: 0x00215173 File Offset: 0x00213373
		public bool IsStateValid(byte state)
		{
			return state <= 1;
		}

		// Token: 0x060067EA RID: 26602 RVA: 0x00216D60 File Offset: 0x00214F60
		public void FunctionalPieceUpdate()
		{
			if (this.lastTriggerTime + this.triggerCooldown < Time.time)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x040076E3 RID: 30435
		[SerializeField]
		protected BuilderPiece myPiece;

		// Token: 0x040076E4 RID: 30436
		[Tooltip("How long in seconds to wait between trigger events")]
		[SerializeField]
		protected float triggerCooldown = 0.5f;

		// Token: 0x040076E5 RID: 30437
		[SerializeField]
		private BuilderSmallHandTrigger[] handTriggers;

		// Token: 0x040076E6 RID: 30438
		[SerializeField]
		private BuilderSmallMonkeTrigger[] bodyTriggers;

		// Token: 0x040076E7 RID: 30439
		[Tooltip("Optional Animation to play when triggered")]
		[SerializeField]
		private Animation animationOnTrigger;

		// Token: 0x040076E8 RID: 30440
		[Tooltip("Optional Sound to play when triggered")]
		[SerializeField]
		private SoundBankPlayer activateSoundBank;

		// Token: 0x040076E9 RID: 30441
		[Tooltip("Knockback the triggering player?")]
		[SerializeField]
		private bool knockbackOnTriggerEnter;

		// Token: 0x040076EA RID: 30442
		[SerializeField]
		private float knockbackVelocity;

		// Token: 0x040076EB RID: 30443
		[Tooltip("uses Forward of the transform provided")]
		[SerializeField]
		private Transform knockbackDirection;

		// Token: 0x040076EC RID: 30444
		private List<Collider> colliders = new List<Collider>(5);

		// Token: 0x040076ED RID: 30445
		private bool isPieceActive;

		// Token: 0x040076EE RID: 30446
		private float lastTriggerTime;

		// Token: 0x040076EF RID: 30447
		private BuilderReplicatedTriggerEnter.FunctionalState currentState;

		// Token: 0x040076F0 RID: 30448
		public UnityEvent OnTriggered;

		// Token: 0x02001046 RID: 4166
		private enum FunctionalState
		{
			// Token: 0x040076F2 RID: 30450
			Idle,
			// Token: 0x040076F3 RID: 30451
			TriggerEntered
		}
	}
}
