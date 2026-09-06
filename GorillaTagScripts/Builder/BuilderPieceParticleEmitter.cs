using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001039 RID: 4153
	public class BuilderPieceParticleEmitter : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x06006778 RID: 26488 RVA: 0x00214E68 File Offset: 0x00213068
		private void OnZoneChanged()
		{
			this.inBuilderZone = ZoneManagement.instance.IsZoneActive(this.myPiece.GetTable().tableZone);
			if (this.inBuilderZone && this.isPieceActive)
			{
				this.StartParticles();
				return;
			}
			if (!this.inBuilderZone)
			{
				this.StopParticles();
			}
		}

		// Token: 0x06006779 RID: 26489 RVA: 0x00214EBC File Offset: 0x002130BC
		private void StopParticles()
		{
			foreach (ParticleSystem particleSystem in this.particles)
			{
				if (particleSystem.isPlaying)
				{
					particleSystem.Stop();
					particleSystem.Clear();
				}
			}
		}

		// Token: 0x0600677A RID: 26490 RVA: 0x00214F1C File Offset: 0x0021311C
		private void StartParticles()
		{
			foreach (ParticleSystem particleSystem in this.particles)
			{
				if (!particleSystem.isPlaying)
				{
					particleSystem.Play();
				}
			}
		}

		// Token: 0x0600677B RID: 26491 RVA: 0x00214F78 File Offset: 0x00213178
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.StopParticles();
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
			this.OnZoneChanged();
		}

		// Token: 0x0600677C RID: 26492 RVA: 0x00214FAC File Offset: 0x002131AC
		public void OnPieceDestroy()
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}

		// Token: 0x0600677D RID: 26493 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x0600677E RID: 26494 RVA: 0x00214FD4 File Offset: 0x002131D4
		public void OnPieceActivate()
		{
			this.isPieceActive = true;
			if (this.inBuilderZone)
			{
				this.StartParticles();
			}
		}

		// Token: 0x0600677F RID: 26495 RVA: 0x00214FEB File Offset: 0x002131EB
		public void OnPieceDeactivate()
		{
			this.isPieceActive = false;
			this.StopParticles();
		}

		// Token: 0x0400768E RID: 30350
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x0400768F RID: 30351
		[SerializeField]
		private List<ParticleSystem> particles;

		// Token: 0x04007690 RID: 30352
		private bool inBuilderZone;

		// Token: 0x04007691 RID: 30353
		private bool isPieceActive;
	}
}
