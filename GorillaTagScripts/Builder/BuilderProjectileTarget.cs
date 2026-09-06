using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001044 RID: 4164
	public class BuilderProjectileTarget : MonoBehaviour, IBuilderPieceFunctional
	{
		// Token: 0x060067D1 RID: 26577 RVA: 0x00216644 File Offset: 0x00214844
		private void Awake()
		{
			this.hitNotifier.OnProjectileHit += this.OnProjectileHit;
			foreach (Collider collider in this.colliders)
			{
				collider.contactOffset = 0.0001f;
			}
		}

		// Token: 0x060067D2 RID: 26578 RVA: 0x002166B0 File Offset: 0x002148B0
		private void OnDestroy()
		{
			this.hitNotifier.OnProjectileHit -= this.OnProjectileHit;
		}

		// Token: 0x060067D3 RID: 26579 RVA: 0x002166C9 File Offset: 0x002148C9
		private void OnDisable()
		{
			this.hitCount = 0;
			if (this.scoreText != null)
			{
				this.scoreText.text = this.hitCount.ToString("D2");
			}
		}

		// Token: 0x060067D4 RID: 26580 RVA: 0x002166FC File Offset: 0x002148FC
		private void OnProjectileHit(SlingshotProjectile projectile, Collision collision)
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
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 11);
			}
		}

		// Token: 0x060067D5 RID: 26581 RVA: 0x0021676A File Offset: 0x0021496A
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
			if (newState == 11)
			{
				return;
			}
			this.lastHitTime = (double)Time.time;
			this.hitCount = Mathf.Clamp((int)newState, 0, 10);
			this.PlayHitEffects();
		}

		// Token: 0x060067D6 RID: 26582 RVA: 0x002167A4 File Offset: 0x002149A4
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (instigator == null)
			{
				return;
			}
			if (newState != 11)
			{
				return;
			}
			this.hitCount++;
			this.hitCount %= 11;
			this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)this.hitCount, instigator.GetPlayerRef(), timeStamp);
		}

		// Token: 0x060067D7 RID: 26583 RVA: 0x0021681D File Offset: 0x00214A1D
		public bool IsStateValid(byte state)
		{
			return state <= 11;
		}

		// Token: 0x060067D8 RID: 26584 RVA: 0x00216828 File Offset: 0x00214A28
		private void PlayHitEffects()
		{
			if (this.hitSoundbank != null)
			{
				this.hitSoundbank.Play();
			}
			if (this.hitAnimation != null && this.hitAnimation.clip != null)
			{
				this.hitAnimation.Play();
			}
			if (this.scoreText != null)
			{
				this.scoreText.text = this.hitCount.ToString("D2");
			}
		}

		// Token: 0x060067D9 RID: 26585 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void FunctionalPieceUpdate()
		{
		}

		// Token: 0x060067DA RID: 26586 RVA: 0x002168A4 File Offset: 0x00214AA4
		public float GetInteractionDistace()
		{
			return 20f;
		}

		// Token: 0x040076D8 RID: 30424
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x040076D9 RID: 30425
		[SerializeField]
		private SlingshotProjectileHitNotifier hitNotifier;

		// Token: 0x040076DA RID: 30426
		[SerializeField]
		protected float hitCooldown = 2f;

		// Token: 0x040076DB RID: 30427
		[Tooltip("Optional Sounds to play on hit")]
		[SerializeField]
		protected SoundBankPlayer hitSoundbank;

		// Token: 0x040076DC RID: 30428
		[Tooltip("Optional Sounds to play on hit")]
		[SerializeField]
		protected Animation hitAnimation;

		// Token: 0x040076DD RID: 30429
		[SerializeField]
		protected List<Collider> colliders;

		// Token: 0x040076DE RID: 30430
		[SerializeField]
		private TMP_Text scoreText;

		// Token: 0x040076DF RID: 30431
		private double lastHitTime;

		// Token: 0x040076E0 RID: 30432
		private int hitCount;

		// Token: 0x040076E1 RID: 30433
		private const byte MAX_SCORE = 10;

		// Token: 0x040076E2 RID: 30434
		private const byte HIT = 11;
	}
}
