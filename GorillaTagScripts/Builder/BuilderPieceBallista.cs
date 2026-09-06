using System;
using System.Collections;
using System.Collections.Generic;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001031 RID: 4145
	public class BuilderPieceBallista : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x0600672C RID: 26412 RVA: 0x002124DC File Offset: 0x002106DC
		private void Awake()
		{
			this.animator.SetFloat(this.pitchParamHash, this.pitch);
			this.appliedAnimatorPitch = this.pitch;
			this.launchDirection = this.launchEnd.position - this.launchStart.position;
			this.launchRampDistance = this.launchDirection.magnitude;
			this.launchDirection /= this.launchRampDistance;
			this.playerPullInRate = Mathf.Exp(this.playerMagnetismStrength);
			if (this.handTrigger != null)
			{
				this.handTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnHandTriggerPressed));
			}
			this.hasLaunchParticles = this.launchParticles != null;
		}

		// Token: 0x0600672D RID: 26413 RVA: 0x002125A2 File Offset: 0x002107A2
		private void OnDestroy()
		{
			if (this.handTrigger != null)
			{
				this.handTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnHandTriggerPressed));
			}
		}

		// Token: 0x0600672E RID: 26414 RVA: 0x002125CE File Offset: 0x002107CE
		private void OnHandTriggerPressed()
		{
			if (this.autoLaunch)
			{
				return;
			}
			if (this.ballistaState == BuilderPieceBallista.BallistaState.PlayerInTrigger)
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 4);
			}
		}

		// Token: 0x0600672F RID: 26415 RVA: 0x00212604 File Offset: 0x00210804
		private void UpdateStateMaster()
		{
			if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
			switch (this.ballistaState)
			{
			case BuilderPieceBallista.BallistaState.Idle:
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				return;
			case BuilderPieceBallista.BallistaState.Loading:
				if (currentAnimatorStateInfo.shortNameHash == this.loadStateHash && (double)Time.time > this.loadCompleteTime)
				{
					if (this.playerInTrigger && this.playerRigInTrigger != null && (this.launchBigMonkes || (double)this.playerRigInTrigger.scaleFactor < 0.99))
					{
						this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
						return;
					}
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					this.ballistaState = BuilderPieceBallista.BallistaState.WaitingForTrigger;
					return;
				}
				break;
			case BuilderPieceBallista.BallistaState.WaitingForTrigger:
				if (!this.playerInTrigger || this.playerRigInTrigger == null || (!this.launchBigMonkes && this.playerRigInTrigger.scaleFactor >= 0.99f))
				{
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					return;
				}
				if (this.playerInTrigger)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			case BuilderPieceBallista.BallistaState.PlayerInTrigger:
				if (!this.playerInTrigger || this.playerRigInTrigger == null || (!this.launchBigMonkes && this.playerRigInTrigger.scaleFactor >= 0.99f))
				{
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 2, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				if (this.autoLaunch && (double)Time.time > this.enteredTriggerTime + (double)this.autoLaunchDelay)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			case BuilderPieceBallista.BallistaState.PrepareForLaunch:
			case BuilderPieceBallista.BallistaState.PrepareForLaunchLocal:
			{
				if (!this.playerInTrigger || this.playerRigInTrigger == null || (!this.launchBigMonkes && this.playerRigInTrigger.scaleFactor >= 0.99f))
				{
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					this.ResetFlags();
					this.myPiece.functionalPieceState = 0;
					this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
					return;
				}
				Vector3 playerBodyCenterPosition = this.GetPlayerBodyCenterPosition(this.playerRigInTrigger.transform, this.playerRigInTrigger.scaleFactor);
				Vector3 vector = Vector3.Dot(playerBodyCenterPosition - this.launchStart.position, this.launchDirection) * this.launchDirection + this.launchStart.position;
				Vector3 vector2 = playerBodyCenterPosition - vector;
				if (Vector3.Lerp(Vector3.zero, vector2, Mathf.Exp(-this.playerPullInRate * Time.deltaTime)).sqrMagnitude < this.playerReadyToFireDist * this.myPiece.GetScale() * this.playerReadyToFireDist * this.myPiece.GetScale())
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 6, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			}
			case BuilderPieceBallista.BallistaState.Launching:
			case BuilderPieceBallista.BallistaState.LaunchingLocal:
				if (currentAnimatorStateInfo.shortNameHash == this.idleStateHash)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06006730 RID: 26416 RVA: 0x00212A13 File Offset: 0x00210C13
		private void ResetFlags()
		{
			this.playerLaunched = false;
			this.loadCompleteTime = double.MaxValue;
		}

		// Token: 0x06006731 RID: 26417 RVA: 0x00212A2C File Offset: 0x00210C2C
		private void UpdatePlayerPosition()
		{
			if (this.ballistaState != BuilderPieceBallista.BallistaState.PrepareForLaunchLocal && this.ballistaState != BuilderPieceBallista.BallistaState.LaunchingLocal)
			{
				return;
			}
			float deltaTime = Time.deltaTime;
			GTPlayer instance = GTPlayer.Instance;
			Vector3 playerBodyCenterPosition = this.GetPlayerBodyCenterPosition(instance.headCollider.transform, instance.scale);
			Vector3 vector = playerBodyCenterPosition - this.launchStart.position;
			BuilderPieceBallista.BallistaState ballistaState = this.ballistaState;
			if (ballistaState == BuilderPieceBallista.BallistaState.PrepareForLaunchLocal)
			{
				Vector3 vector2 = Vector3.Dot(vector, this.launchDirection) * this.launchDirection + this.launchStart.position;
				Vector3 vector3 = playerBodyCenterPosition - vector2;
				Vector3 vector4 = Vector3.Lerp(Vector3.zero, vector3, Mathf.Exp(-this.playerPullInRate * deltaTime));
				instance.transform.position = instance.transform.position + (vector4 - vector3);
				instance.SetPlayerVelocity(Vector3.zero);
				instance.SetMaximumSlipThisFrame();
				return;
			}
			if (ballistaState != BuilderPieceBallista.BallistaState.LaunchingLocal)
			{
				return;
			}
			if (!this.playerLaunched)
			{
				float num = Vector3.Dot(this.launchBone.position - this.launchStart.position, this.launchDirection) / this.launchRampDistance;
				float num2 = Vector3.Dot(vector, this.launchDirection) / this.launchRampDistance;
				float num3 = 0.25f * this.myPiece.GetScale() / this.launchRampDistance;
				float num4 = Mathf.Max(num + num3, num2);
				float num5 = num4 * this.launchRampDistance;
				Vector3 vector5 = this.launchDirection * num5 + this.launchStart.position;
				instance.transform.position + (vector5 - playerBodyCenterPosition);
				instance.transform.position = instance.transform.position + (vector5 - playerBodyCenterPosition);
				instance.SetPlayerVelocity(Vector3.zero);
				instance.SetMaximumSlipThisFrame();
				if (num4 >= 1f)
				{
					this.playerLaunched = true;
					this.launchedTime = (double)Time.time;
					instance.SetPlayerVelocity(this.launchSpeed * this.myPiece.GetScale() * this.launchDirection);
					instance.SetMaximumSlipThisFrame();
					return;
				}
			}
			else if ((double)Time.time < this.launchedTime + (double)this.slipOverrideDuration)
			{
				instance.SetMaximumSlipThisFrame();
			}
		}

		// Token: 0x06006732 RID: 26418 RVA: 0x00212C64 File Offset: 0x00210E64
		private Vector3 GetPlayerBodyCenterPosition(Transform headTransform, float playerScale)
		{
			return headTransform.position + Quaternion.Euler(0f, headTransform.rotation.eulerAngles.y, 0f) * new Vector3(0f, 0f, this.playerBodyOffsetFromHead.z * playerScale) + Vector3.down * (this.playerBodyOffsetFromHead.y * playerScale);
		}

		// Token: 0x06006733 RID: 26419 RVA: 0x00212CDC File Offset: 0x00210EDC
		private void OnTriggerEnter(Collider other)
		{
			if (this.playerRigInTrigger != null)
			{
				return;
			}
			if (other.GetComponent<CapsuleCollider>() == null)
			{
				return;
			}
			if (other.attachedRigidbody == null)
			{
				return;
			}
			VRRig vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (vrrig == null)
			{
				if (!(GTPlayer.Instance.bodyCollider == other))
				{
					return;
				}
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (!this.launchBigMonkes && (double)vrrig.scaleFactor > 0.99)
			{
				return;
			}
			this.playerRigInTrigger = vrrig;
			this.playerInTrigger = true;
		}

		// Token: 0x06006734 RID: 26420 RVA: 0x00212D7C File Offset: 0x00210F7C
		private void OnTriggerExit(Collider other)
		{
			if (this.playerRigInTrigger == null || !this.playerInTrigger)
			{
				return;
			}
			if (other.GetComponent<CapsuleCollider>() == null)
			{
				return;
			}
			if (other.attachedRigidbody == null)
			{
				return;
			}
			VRRig vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (vrrig == null)
			{
				if (!(GTPlayer.Instance.bodyCollider == other))
				{
					return;
				}
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (this.playerRigInTrigger.Equals(vrrig))
			{
				this.playerInTrigger = false;
				this.playerRigInTrigger = null;
			}
		}

		// Token: 0x06006735 RID: 26421 RVA: 0x00212E14 File Offset: 0x00211014
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			if (!this.myPiece.GetTable().isTableMutable)
			{
				this.launchBigMonkes = true;
			}
			this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
			this.playerInTrigger = false;
			this.playerRigInTrigger = null;
			this.playerLaunched = false;
		}

		// Token: 0x06006736 RID: 26422 RVA: 0x00212E4B File Offset: 0x0021104B
		public void OnPieceDestroy()
		{
			this.myPiece.functionalPieceState = 0;
			this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
		}

		// Token: 0x06006737 RID: 26423 RVA: 0x00212E60 File Offset: 0x00211060
		public void OnPiecePlacementDeserialized()
		{
			this.launchDirection = this.launchEnd.position - this.launchStart.position;
			this.launchRampDistance = this.launchDirection.magnitude;
			this.launchDirection /= this.launchRampDistance;
		}

		// Token: 0x06006738 RID: 26424 RVA: 0x00212EB8 File Offset: 0x002110B8
		public void OnPieceActivate()
		{
			foreach (Collider collider in this.triggers)
			{
				collider.enabled = true;
			}
			this.animator.SetFloat(this.pitchParamHash, this.pitch);
			this.appliedAnimatorPitch = this.pitch;
			this.launchDirection = this.launchEnd.position - this.launchStart.position;
			this.launchRampDistance = this.launchDirection.magnitude;
			this.launchDirection /= this.launchRampDistance;
			this.myPiece.GetTable().RegisterFunctionalPiece(this);
		}

		// Token: 0x06006739 RID: 26425 RVA: 0x00212F88 File Offset: 0x00211188
		public void OnPieceDeactivate()
		{
			foreach (Collider collider in this.triggers)
			{
				collider.enabled = false;
			}
			if (this.hasLaunchParticles)
			{
				this.launchParticles.Stop();
				this.launchParticles.Clear();
			}
			this.myPiece.functionalPieceState = 0;
			this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
			this.playerInTrigger = false;
			this.playerRigInTrigger = null;
			this.ResetFlags();
			this.myPiece.GetTable().UnregisterFunctionalPiece(this);
		}

		// Token: 0x0600673A RID: 26426 RVA: 0x00213030 File Offset: 0x00211230
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
			if ((BuilderPieceBallista.BallistaState)newState == this.ballistaState)
			{
				return;
			}
			if (newState == 4)
			{
				if (this.ballistaState == BuilderPieceBallista.BallistaState.PlayerInTrigger && this.playerInTrigger && this.playerRigInTrigger != null)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, this.playerRigInTrigger.Creator.GetPlayerRef(), timeStamp);
					return;
				}
			}
			else
			{
				Debug.LogWarning("BuilderPiece Ballista unexpected state request for " + newState.ToString());
			}
		}

		// Token: 0x0600673B RID: 26427 RVA: 0x002130D0 File Offset: 0x002112D0
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			BuilderPieceBallista.BallistaState ballistaState = (BuilderPieceBallista.BallistaState)newState;
			if (ballistaState == this.ballistaState)
			{
				return;
			}
			switch (newState)
			{
			case 0:
				this.ResetFlags();
				goto IL_02C2;
			case 1:
				this.ResetFlags();
				foreach (Collider collider in this.disableWhileLaunching)
				{
					collider.enabled = true;
				}
				if (this.ballistaState == BuilderPieceBallista.BallistaState.Launching || this.ballistaState == BuilderPieceBallista.BallistaState.LaunchingLocal)
				{
					this.loadCompleteTime = (double)(Time.time + this.reloadDelay);
					if (this.loadSFX != null)
					{
						this.loadSFX.Play();
					}
				}
				else
				{
					this.loadCompleteTime = (double)(Time.time + this.loadTime);
				}
				this.animator.SetTrigger(this.loadTriggerHash);
				goto IL_02C2;
			case 2:
			case 5:
				goto IL_02C2;
			case 3:
				this.enteredTriggerTime = (double)Time.time;
				if (this.autoLaunch && this.cockSFX != null)
				{
					this.cockSFX.Play();
					goto IL_02C2;
				}
				goto IL_02C2;
			case 4:
			{
				this.playerLaunched = false;
				if (!this.autoLaunch && this.cockSFX != null)
				{
					this.cockSFX.Play();
				}
				if (!instigator.IsLocal)
				{
					goto IL_02C2;
				}
				GTPlayer instance = GTPlayer.Instance;
				if (Vector3.Distance(this.GetPlayerBodyCenterPosition(instance.headCollider.transform, instance.scale), this.launchStart.position) > this.prepareForLaunchDistance * this.myPiece.GetScale() || (!this.launchBigMonkes && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor >= 0.99))
				{
					goto IL_02C2;
				}
				ballistaState = BuilderPieceBallista.BallistaState.PrepareForLaunchLocal;
				using (List<Collider>.Enumerator enumerator = this.disableWhileLaunching.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Collider collider2 = enumerator.Current;
						collider2.enabled = false;
					}
					goto IL_02C2;
				}
				break;
			}
			case 6:
				break;
			default:
				goto IL_02C2;
			}
			this.playerLaunched = false;
			this.animator.SetTrigger(this.fireTriggerHash);
			if (this.launchSFX != null)
			{
				this.launchSFX.Play();
			}
			if (this.hasLaunchParticles)
			{
				this.launchParticles.Play();
			}
			if (this.debugDrawTrajectoryOnLaunch)
			{
				base.StartCoroutine(this.DebugDrawTrajectory(8f));
			}
			if (instigator.IsLocal && this.ballistaState == BuilderPieceBallista.BallistaState.PrepareForLaunchLocal)
			{
				ballistaState = BuilderPieceBallista.BallistaState.LaunchingLocal;
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 4f);
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 4f);
			}
			IL_02C2:
			this.ballistaState = ballistaState;
		}

		// Token: 0x0600673C RID: 26428 RVA: 0x002133C4 File Offset: 0x002115C4
		public bool IsStateValid(byte state)
		{
			return state < 8;
		}

		// Token: 0x0600673D RID: 26429 RVA: 0x002133CA File Offset: 0x002115CA
		public void FunctionalPieceUpdate()
		{
			if (this.myPiece == null || this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.UpdateStateMaster();
			}
			this.UpdatePlayerPosition();
		}

		// Token: 0x0600673E RID: 26430 RVA: 0x00213400 File Offset: 0x00211600
		private void UpdatePredictionLine()
		{
			float num = 0.033333335f;
			Vector3 vector = this.launchEnd.position;
			Vector3 vector2 = (this.launchEnd.position - this.launchStart.position).normalized * this.launchSpeed;
			for (int i = 0; i < 240; i++)
			{
				this.predictionLinePoints[i] = vector;
				vector += vector2 * num;
				vector2 += Vector3.down * 9.8f * num;
			}
		}

		// Token: 0x0600673F RID: 26431 RVA: 0x0021349A File Offset: 0x0021169A
		private IEnumerator DebugDrawTrajectory(float duration)
		{
			this.UpdatePredictionLine();
			float startTime = Time.time;
			while (Time.time < startTime + duration)
			{
				DebugUtil.DrawLine(this.launchStart.position, this.launchEnd.position, Color.yellow, true);
				DebugUtil.DrawLines(this.predictionLinePoints, Color.yellow, true);
				yield return null;
			}
			yield break;
		}

		// Token: 0x04007610 RID: 30224
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04007611 RID: 30225
		[SerializeField]
		private List<Collider> triggers;

		// Token: 0x04007612 RID: 30226
		[SerializeField]
		private List<Collider> disableWhileLaunching;

		// Token: 0x04007613 RID: 30227
		[Tooltip("Trigger to start the launch if not autoLaunch")]
		[SerializeField]
		private BuilderSmallHandTrigger handTrigger;

		// Token: 0x04007614 RID: 30228
		[Tooltip("Should the player launch without a hand trigger press")]
		[SerializeField]
		private bool autoLaunch;

		// Token: 0x04007615 RID: 30229
		[SerializeField]
		private float autoLaunchDelay = 0.75f;

		// Token: 0x04007616 RID: 30230
		private double enteredTriggerTime;

		// Token: 0x04007617 RID: 30231
		public Animator animator;

		// Token: 0x04007618 RID: 30232
		public Transform launchStart;

		// Token: 0x04007619 RID: 30233
		public Transform launchEnd;

		// Token: 0x0400761A RID: 30234
		public Transform launchBone;

		// Token: 0x0400761B RID: 30235
		[SerializeField]
		private SoundBankPlayer loadSFX;

		// Token: 0x0400761C RID: 30236
		[SerializeField]
		private SoundBankPlayer launchSFX;

		// Token: 0x0400761D RID: 30237
		[SerializeField]
		private SoundBankPlayer cockSFX;

		// Token: 0x0400761E RID: 30238
		[SerializeField]
		private ParticleSystem launchParticles;

		// Token: 0x0400761F RID: 30239
		private bool hasLaunchParticles;

		// Token: 0x04007620 RID: 30240
		public float reloadDelay = 1f;

		// Token: 0x04007621 RID: 30241
		public float loadTime = 1.933f;

		// Token: 0x04007622 RID: 30242
		public float slipOverrideDuration = 0.1f;

		// Token: 0x04007623 RID: 30243
		private double launchedTime;

		// Token: 0x04007624 RID: 30244
		public float playerMagnetismStrength = 3f;

		// Token: 0x04007625 RID: 30245
		[Tooltip("Speed will be scaled by piece scale")]
		public float launchSpeed = 20f;

		// Token: 0x04007626 RID: 30246
		[Range(0f, 1f)]
		public float pitch;

		// Token: 0x04007627 RID: 30247
		private bool debugDrawTrajectoryOnLaunch;

		// Token: 0x04007628 RID: 30248
		private int loadTriggerHash = Animator.StringToHash("Load");

		// Token: 0x04007629 RID: 30249
		private int fireTriggerHash = Animator.StringToHash("Fire");

		// Token: 0x0400762A RID: 30250
		private int pitchParamHash = Animator.StringToHash("Pitch");

		// Token: 0x0400762B RID: 30251
		private int idleStateHash = Animator.StringToHash("Idle");

		// Token: 0x0400762C RID: 30252
		private int loadStateHash = Animator.StringToHash("Load");

		// Token: 0x0400762D RID: 30253
		private int fireStateHash = Animator.StringToHash("Fire");

		// Token: 0x0400762E RID: 30254
		private bool playerInTrigger;

		// Token: 0x0400762F RID: 30255
		private VRRig playerRigInTrigger;

		// Token: 0x04007630 RID: 30256
		private bool playerLaunched;

		// Token: 0x04007631 RID: 30257
		private float playerReadyToFireDist = 1.6667f;

		// Token: 0x04007632 RID: 30258
		private float prepareForLaunchDistance = 2.5f;

		// Token: 0x04007633 RID: 30259
		private Vector3 launchDirection;

		// Token: 0x04007634 RID: 30260
		private float launchRampDistance;

		// Token: 0x04007635 RID: 30261
		private float playerPullInRate;

		// Token: 0x04007636 RID: 30262
		private float appliedAnimatorPitch;

		// Token: 0x04007637 RID: 30263
		private bool launchBigMonkes;

		// Token: 0x04007638 RID: 30264
		private Vector3 playerBodyOffsetFromHead = new Vector3(0f, -0.4f, -0.15f);

		// Token: 0x04007639 RID: 30265
		private double loadCompleteTime;

		// Token: 0x0400763A RID: 30266
		private BuilderPieceBallista.BallistaState ballistaState;

		// Token: 0x0400763B RID: 30267
		private const int predictionLineSamples = 240;

		// Token: 0x0400763C RID: 30268
		private Vector3[] predictionLinePoints = new Vector3[240];

		// Token: 0x02001032 RID: 4146
		private enum BallistaState
		{
			// Token: 0x0400763E RID: 30270
			Idle,
			// Token: 0x0400763F RID: 30271
			Loading,
			// Token: 0x04007640 RID: 30272
			WaitingForTrigger,
			// Token: 0x04007641 RID: 30273
			PlayerInTrigger,
			// Token: 0x04007642 RID: 30274
			PrepareForLaunch,
			// Token: 0x04007643 RID: 30275
			PrepareForLaunchLocal,
			// Token: 0x04007644 RID: 30276
			Launching,
			// Token: 0x04007645 RID: 30277
			LaunchingLocal,
			// Token: 0x04007646 RID: 30278
			Count
		}
	}
}
