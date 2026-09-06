using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001049 RID: 4169
	public class BuilderShootingGallery : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x060067FA RID: 26618 RVA: 0x002175CC File Offset: 0x002157CC
		private void Awake()
		{
			foreach (Collider collider in this.colliders)
			{
				collider.contactOffset = 0.0001f;
			}
			this.wheelHitNotifier.OnProjectileHit += this.OnWheelHit;
			this.cowboyHitNotifier.OnProjectileHit += this.OnCowboyHit;
		}

		// Token: 0x060067FB RID: 26619 RVA: 0x00217650 File Offset: 0x00215850
		private void OnDestroy()
		{
			this.wheelHitNotifier.OnProjectileHit -= this.OnWheelHit;
			this.cowboyHitNotifier.OnProjectileHit -= this.OnCowboyHit;
		}

		// Token: 0x060067FC RID: 26620 RVA: 0x00217680 File Offset: 0x00215880
		private void OnWheelHit(SlingshotProjectile projectile, Collision collision)
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (projectile.projectileOwner == null || projectile.projectileOwner != NetworkSystem.Instance.LocalPlayer)
			{
				return;
			}
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 1);
			}
		}

		// Token: 0x060067FD RID: 26621 RVA: 0x002176F0 File Offset: 0x002158F0
		private void OnCowboyHit(SlingshotProjectile projectile, Collision collision)
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (projectile.projectileOwner == null || projectile.projectileOwner != NetworkSystem.Instance.LocalPlayer)
			{
				return;
			}
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 2);
			}
		}

		// Token: 0x060067FE RID: 26622 RVA: 0x00217760 File Offset: 0x00215960
		private void CowboyHitEffects()
		{
			if (this.cowboyHitSound != null)
			{
				this.cowboyHitSound.Play();
			}
			if (this.cowboyHitAnimation != null && this.cowboyHitAnimation.clip != null)
			{
				this.cowboyHitAnimation.Play();
			}
		}

		// Token: 0x060067FF RID: 26623 RVA: 0x002177B4 File Offset: 0x002159B4
		private void WheelHitEffects()
		{
			if (this.wheelHitSound != null)
			{
				this.wheelHitSound.Play();
			}
			if (this.wheelHitAnimation != null && this.wheelHitAnimation.clip != null)
			{
				this.wheelHitAnimation.Play();
			}
		}

		// Token: 0x06006800 RID: 26624 RVA: 0x00217808 File Offset: 0x00215A08
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.currentState = BuilderShootingGallery.FunctionalState.Idle;
			this.cowboyInitLocalPos = this.cowboyTransform.transform.localPosition;
			this.cowboyInitLocalRotation = this.cowboyTransform.transform.localRotation;
			this.wheelInitLocalRot = this.wheelTransform.transform.localRotation;
			this.distance = Vector3.Distance(this.cowboyStart.position, this.cowboyEnd.position);
			this.cowboyCycleDuration = this.distance / (this.cowboyVelocity * this.myPiece.GetScale());
			this.wheelCycleDuration = 1f / this.wheelVelocity;
		}

		// Token: 0x06006801 RID: 26625 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06006802 RID: 26626 RVA: 0x002178B0 File Offset: 0x00215AB0
		public void OnPiecePlacementDeserialized()
		{
			if (!this.activated && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced)
			{
				this.myPiece.GetTable().RegisterFunctionalPieceFixedUpdate(this);
				this.activated = true;
			}
		}

		// Token: 0x06006803 RID: 26627 RVA: 0x002178E0 File Offset: 0x00215AE0
		public void OnPieceActivate()
		{
			this.cowboyTransform.SetLocalPositionAndRotation(this.cowboyInitLocalPos, this.cowboyInitLocalRotation);
			this.wheelTransform.SetLocalPositionAndRotation(this.wheelTransform.localPosition, this.wheelInitLocalRot);
			if (!this.activated)
			{
				this.myPiece.GetTable().RegisterFunctionalPieceFixedUpdate(this);
				this.activated = true;
			}
		}

		// Token: 0x06006804 RID: 26628 RVA: 0x00217940 File Offset: 0x00215B40
		public void OnPieceDeactivate()
		{
			if (this.currentState != BuilderShootingGallery.FunctionalState.Idle)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
			if (this.activated)
			{
				this.myPiece.GetTable().UnregisterFunctionalPieceFixedUpdate(this);
				this.activated = false;
			}
			this.cowboyTransform.SetLocalPositionAndRotation(this.cowboyInitLocalPos, this.cowboyInitLocalRotation);
			this.wheelTransform.SetLocalPositionAndRotation(this.wheelTransform.localPosition, this.wheelInitLocalRot);
		}

		// Token: 0x06006805 RID: 26629 RVA: 0x002179DC File Offset: 0x00215BDC
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (instigator == null)
			{
				return;
			}
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (newState == 1 && this.currentState == BuilderShootingGallery.FunctionalState.Idle)
			{
				this.lastHitTime = (double)Time.time;
				this.WheelHitEffects();
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
			else if (newState == 2 && this.currentState == BuilderShootingGallery.FunctionalState.Idle)
			{
				this.lastHitTime = (double)Time.time;
				this.CowboyHitEffects();
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
			this.currentState = (BuilderShootingGallery.FunctionalState)newState;
		}

		// Token: 0x06006806 RID: 26630 RVA: 0x00217A60 File Offset: 0x00215C60
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
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x06006807 RID: 26631 RVA: 0x00217AC5 File Offset: 0x00215CC5
		public bool IsStateValid(byte state)
		{
			return state <= 2;
		}

		// Token: 0x06006808 RID: 26632 RVA: 0x00217AD0 File Offset: 0x00215CD0
		public void FunctionalPieceUpdate()
		{
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x06006809 RID: 26633 RVA: 0x00217B24 File Offset: 0x00215D24
		public void FunctionalPieceFixedUpdate()
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			this.currT = this.CowboyCycleCompletionPercent();
			this.currForward = this.IsEvenCycle();
			float num = (this.currForward ? this.currT : (1f - this.currT));
			float num2 = this.WheelCycleCompletionPercent();
			float num3 = this.cowboyCurve.Evaluate(num);
			this.cowboyTransform.localPosition = Vector3.Lerp(this.cowboyStart.localPosition, this.cowboyEnd.localPosition, num3);
			Quaternion quaternion = Quaternion.AngleAxis(num2 * 360f, Vector3.right);
			this.wheelTransform.localRotation = quaternion;
		}

		// Token: 0x0600680A RID: 26634 RVA: 0x00217BCB File Offset: 0x00215DCB
		private long NetworkTimeMs()
		{
			if (PhotonNetwork.InRoom)
			{
				return (long)((ulong)(PhotonNetwork.ServerTimestamp + int.MinValue));
			}
			return (long)(Time.time * 1000f);
		}

		// Token: 0x0600680B RID: 26635 RVA: 0x00217BED File Offset: 0x00215DED
		private long CowboyCycleLengthMs()
		{
			return (long)(this.cowboyCycleDuration * 1000f);
		}

		// Token: 0x0600680C RID: 26636 RVA: 0x00217BFC File Offset: 0x00215DFC
		private long WheelCycleLengthMs()
		{
			return (long)(this.wheelCycleDuration * 1000f);
		}

		// Token: 0x0600680D RID: 26637 RVA: 0x00217C0C File Offset: 0x00215E0C
		public double CowboyPlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.CowboyCycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x0600680E RID: 26638 RVA: 0x00217C38 File Offset: 0x00215E38
		public double WheelPlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.WheelCycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x0600680F RID: 26639 RVA: 0x00217C63 File Offset: 0x00215E63
		public int CowboyCycleCount()
		{
			return (int)(this.NetworkTimeMs() / this.CowboyCycleLengthMs());
		}

		// Token: 0x06006810 RID: 26640 RVA: 0x00217C73 File Offset: 0x00215E73
		public float CowboyCycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.CowboyPlatformTime() / (double)this.cowboyCycleDuration), 0f, 1f);
		}

		// Token: 0x06006811 RID: 26641 RVA: 0x00217C93 File Offset: 0x00215E93
		public float WheelCycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.WheelPlatformTime() / (double)this.wheelCycleDuration), 0f, 1f);
		}

		// Token: 0x06006812 RID: 26642 RVA: 0x00217CB3 File Offset: 0x00215EB3
		public bool IsEvenCycle()
		{
			return this.CowboyCycleCount() % 2 == 0;
		}

		// Token: 0x04007719 RID: 30489
		public BuilderPiece myPiece;

		// Token: 0x0400771A RID: 30490
		[SerializeField]
		private Transform wheelTransform;

		// Token: 0x0400771B RID: 30491
		[SerializeField]
		private Transform cowboyTransform;

		// Token: 0x0400771C RID: 30492
		[SerializeField]
		private SlingshotProjectileHitNotifier wheelHitNotifier;

		// Token: 0x0400771D RID: 30493
		[SerializeField]
		private SlingshotProjectileHitNotifier cowboyHitNotifier;

		// Token: 0x0400771E RID: 30494
		[SerializeField]
		protected List<Collider> colliders;

		// Token: 0x0400771F RID: 30495
		[SerializeField]
		protected SoundBankPlayer wheelHitSound;

		// Token: 0x04007720 RID: 30496
		[SerializeField]
		protected Animation wheelHitAnimation;

		// Token: 0x04007721 RID: 30497
		[SerializeField]
		protected SoundBankPlayer cowboyHitSound;

		// Token: 0x04007722 RID: 30498
		[SerializeField]
		private Animation cowboyHitAnimation;

		// Token: 0x04007723 RID: 30499
		[SerializeField]
		private float hitCooldown = 1f;

		// Token: 0x04007724 RID: 30500
		private double lastHitTime;

		// Token: 0x04007725 RID: 30501
		private BuilderShootingGallery.FunctionalState currentState;

		// Token: 0x04007726 RID: 30502
		private bool activated;

		// Token: 0x04007727 RID: 30503
		[SerializeField]
		private float cowboyVelocity;

		// Token: 0x04007728 RID: 30504
		[SerializeField]
		private Transform cowboyStart;

		// Token: 0x04007729 RID: 30505
		[SerializeField]
		private Transform cowboyEnd;

		// Token: 0x0400772A RID: 30506
		[SerializeField]
		private AnimationCurve cowboyCurve;

		// Token: 0x0400772B RID: 30507
		[SerializeField]
		private float wheelVelocity;

		// Token: 0x0400772C RID: 30508
		private Quaternion cowboyInitLocalRotation = Quaternion.identity;

		// Token: 0x0400772D RID: 30509
		private Vector3 cowboyInitLocalPos = Vector3.zero;

		// Token: 0x0400772E RID: 30510
		private Quaternion wheelInitLocalRot = Quaternion.identity;

		// Token: 0x0400772F RID: 30511
		private float cowboyCycleDuration;

		// Token: 0x04007730 RID: 30512
		private float wheelCycleDuration;

		// Token: 0x04007731 RID: 30513
		private float distance;

		// Token: 0x04007732 RID: 30514
		private float currT;

		// Token: 0x04007733 RID: 30515
		private bool currForward;

		// Token: 0x04007734 RID: 30516
		private float dtSinceServerUpdate;

		// Token: 0x04007735 RID: 30517
		private int lastServerTimeStamp;

		// Token: 0x04007736 RID: 30518
		private float rotateStartAmt;

		// Token: 0x04007737 RID: 30519
		private float rotateAmt;

		// Token: 0x0200104A RID: 4170
		private enum FunctionalState
		{
			// Token: 0x04007739 RID: 30521
			Idle,
			// Token: 0x0400773A RID: 30522
			HitWheel,
			// Token: 0x0400773B RID: 30523
			HitCowboy
		}
	}
}
