using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001042 RID: 4162
	public class BuilderProjectileLauncher : MonoBehaviour, IBuilderPieceFunctional, IBuilderPieceComponent
	{
		// Token: 0x060067C4 RID: 26564 RVA: 0x002162BC File Offset: 0x002144BC
		private void LaunchProjectile(int timeStamp)
		{
			if (Time.time > this.lastFireTime + this.fireCooldown)
			{
				this.lastFireTime = Time.time;
				int num = PoolUtils.GameObjHashCode(this.projectilePrefab);
				try
				{
					GameObject gameObject = ObjectPools.instance.Instantiate(num, true);
					this.projectileScale = this.myPiece.GetScale();
					gameObject.transform.localScale = Vector3.one * this.projectileScale;
					BuilderProjectile component = gameObject.GetComponent<BuilderProjectile>();
					int num2 = HashCode.Combine<int, int>(this.myPiece.pieceId, timeStamp);
					if (this.allProjectiles.ContainsKey(num2))
					{
						this.allProjectiles.Remove(num2);
					}
					this.allProjectiles.Add(num2, component);
					SlingshotProjectile.AOEKnockbackConfig aoeknockbackConfig = new SlingshotProjectile.AOEKnockbackConfig
					{
						aeoOuterRadius = this.knockbackConfig.aeoOuterRadius * this.projectileScale,
						aeoInnerRadius = this.knockbackConfig.aeoInnerRadius * this.projectileScale,
						applyAOEKnockback = this.knockbackConfig.applyAOEKnockback,
						impactVelocityThreshold = this.knockbackConfig.impactVelocityThreshold * this.projectileScale,
						knockbackVelocity = this.knockbackConfig.knockbackVelocity * this.projectileScale,
						playerProximityEffect = this.knockbackConfig.playerProximityEffect
					};
					component.aoeKnockbackConfig = new SlingshotProjectile.AOEKnockbackConfig?(aoeknockbackConfig);
					component.gravityMultiplier = this.gravityMultiplier;
					component.Launch(this.launchPosition.position, this.launchVelocity * this.projectileScale * this.launchPosition.up, this, num2, this.projectileScale, timeStamp);
					if (this.launchSound != null && this.launchSound.clip != null)
					{
						this.launchSound.Play();
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					throw;
				}
			}
		}

		// Token: 0x060067C5 RID: 26565 RVA: 0x002164A4 File Offset: 0x002146A4
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if ((BuilderProjectileLauncher.FunctionalState)newState == this.currentState)
			{
				return;
			}
			this.currentState = (BuilderProjectileLauncher.FunctionalState)newState;
			if (newState == 1)
			{
				this.LaunchProjectile(timeStamp);
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x060067C6 RID: 26566 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
		}

		// Token: 0x060067C7 RID: 26567 RVA: 0x00215173 File Offset: 0x00213373
		public bool IsStateValid(byte state)
		{
			return state <= 1;
		}

		// Token: 0x060067C8 RID: 26568 RVA: 0x002164F8 File Offset: 0x002146F8
		public void FunctionalPieceUpdate()
		{
			for (int i = this.launchedProjectiles.Count - 1; i >= 0; i--)
			{
				this.launchedProjectiles[i].UpdateProjectile();
			}
			if (PhotonNetwork.IsMasterClient && this.lastFireTime + this.fireCooldown < Time.time)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x060067C9 RID: 26569 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x060067CA RID: 26570 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPieceDestroy()
		{
		}

		// Token: 0x060067CB RID: 26571 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x060067CC RID: 26572 RVA: 0x00216579 File Offset: 0x00214779
		public void OnPieceActivate()
		{
			this.myPiece.GetTable().RegisterFunctionalPiece(this);
		}

		// Token: 0x060067CD RID: 26573 RVA: 0x0021658C File Offset: 0x0021478C
		public void OnPieceDeactivate()
		{
			this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			for (int i = this.launchedProjectiles.Count - 1; i >= 0; i--)
			{
				this.launchedProjectiles[i].Deactivate();
			}
		}

		// Token: 0x060067CE RID: 26574 RVA: 0x002165D3 File Offset: 0x002147D3
		public void RegisterProjectile(BuilderProjectile projectile)
		{
			this.launchedProjectiles.Add(projectile);
		}

		// Token: 0x060067CF RID: 26575 RVA: 0x002165E1 File Offset: 0x002147E1
		public void UnRegisterProjectile(BuilderProjectile projectile)
		{
			this.launchedProjectiles.Remove(projectile);
			this.allProjectiles.Remove(projectile.projectileId);
		}

		// Token: 0x040076C8 RID: 30408
		private List<BuilderProjectile> launchedProjectiles = new List<BuilderProjectile>();

		// Token: 0x040076C9 RID: 30409
		[SerializeField]
		protected BuilderPiece myPiece;

		// Token: 0x040076CA RID: 30410
		[SerializeField]
		protected float fireCooldown = 2f;

		// Token: 0x040076CB RID: 30411
		[Tooltip("launch in Y direction")]
		[SerializeField]
		private Transform launchPosition;

		// Token: 0x040076CC RID: 30412
		[SerializeField]
		private float launchVelocity;

		// Token: 0x040076CD RID: 30413
		[SerializeField]
		private AudioSource launchSound;

		// Token: 0x040076CE RID: 30414
		[SerializeField]
		protected GameObject projectilePrefab;

		// Token: 0x040076CF RID: 30415
		protected float projectileScale = 0.06f;

		// Token: 0x040076D0 RID: 30416
		[SerializeField]
		protected float gravityMultiplier = 1f;

		// Token: 0x040076D1 RID: 30417
		public SlingshotProjectile.AOEKnockbackConfig knockbackConfig;

		// Token: 0x040076D2 RID: 30418
		private float lastFireTime;

		// Token: 0x040076D3 RID: 30419
		private BuilderProjectileLauncher.FunctionalState currentState;

		// Token: 0x040076D4 RID: 30420
		private Dictionary<int, BuilderProjectile> allProjectiles = new Dictionary<int, BuilderProjectile>();

		// Token: 0x02001043 RID: 4163
		private enum FunctionalState
		{
			// Token: 0x040076D6 RID: 30422
			Idle,
			// Token: 0x040076D7 RID: 30423
			Fire
		}
	}
}
