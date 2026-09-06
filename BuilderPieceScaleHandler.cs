using System;
using System.Collections.Generic;
using GorillaTagScripts.Builder;
using UnityEngine;

// Token: 0x0200061E RID: 1566
public class BuilderPieceScaleHandler : MonoBehaviour, IBuilderPieceComponent
{
	// Token: 0x06002715 RID: 10005 RVA: 0x000CEDF4 File Offset: 0x000CCFF4
	public void OnPieceCreate(int pieceType, int pieceId)
	{
		foreach (BuilderScaleAudioRadius builderScaleAudioRadius in this.audioScalers)
		{
			builderScaleAudioRadius.SetScale(this.myPiece.GetScale());
		}
		foreach (BuilderScaleParticles builderScaleParticles in this.particleScalers)
		{
			builderScaleParticles.SetScale(this.myPiece.GetScale());
		}
	}

	// Token: 0x06002716 RID: 10006 RVA: 0x000CEE9C File Offset: 0x000CD09C
	public void OnPieceDestroy()
	{
		foreach (BuilderScaleAudioRadius builderScaleAudioRadius in this.audioScalers)
		{
			builderScaleAudioRadius.RevertScale();
		}
		foreach (BuilderScaleParticles builderScaleParticles in this.particleScalers)
		{
			builderScaleParticles.RevertScale();
		}
	}

	// Token: 0x06002717 RID: 10007 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPiecePlacementDeserialized()
	{
	}

	// Token: 0x06002718 RID: 10008 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPieceActivate()
	{
	}

	// Token: 0x06002719 RID: 10009 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPieceDeactivate()
	{
	}

	// Token: 0x0400329F RID: 12959
	[SerializeField]
	private BuilderPiece myPiece;

	// Token: 0x040032A0 RID: 12960
	[SerializeField]
	private List<BuilderScaleAudioRadius> audioScalers = new List<BuilderScaleAudioRadius>();

	// Token: 0x040032A1 RID: 12961
	[SerializeField]
	private List<BuilderScaleParticles> particleScalers = new List<BuilderScaleParticles>();
}
