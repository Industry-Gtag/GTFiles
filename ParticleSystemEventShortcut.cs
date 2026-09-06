using System;
using UnityEngine;

// Token: 0x0200059C RID: 1436
[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemEventShortcut : MonoBehaviour
{
	// Token: 0x06002469 RID: 9321 RVA: 0x000C393C File Offset: 0x000C1B3C
	private void InitIfNeeded()
	{
		if (!this.initialized)
		{
			this.initialized = true;
			this.ps = base.GetComponent<ParticleSystem>();
			this.shape = this.ps.shape;
			this.poolExists = ObjectPools.instance.DoesPoolExist(base.gameObject);
		}
	}

	// Token: 0x0600246A RID: 9322 RVA: 0x000C398B File Offset: 0x000C1B8B
	public void StopAndClear()
	{
		this.InitIfNeeded();
		this.ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	// Token: 0x0600246B RID: 9323 RVA: 0x000C39A0 File Offset: 0x000C1BA0
	public void ClearAndPlay()
	{
		this.InitIfNeeded();
		this.ps.Clear();
		this.ps.Play();
	}

	// Token: 0x0600246C RID: 9324 RVA: 0x000C39BE File Offset: 0x000C1BBE
	public void PlayFromMesh(MeshRenderer mesh)
	{
		this.InitIfNeeded();
		this.shape.shapeType = ParticleSystemShapeType.MeshRenderer;
		this.shape.meshRenderer = mesh;
		this.ps.Play();
	}

	// Token: 0x0600246D RID: 9325 RVA: 0x000C39EA File Offset: 0x000C1BEA
	public void PlayFromSkin(SkinnedMeshRenderer skin)
	{
		this.InitIfNeeded();
		this.shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
		this.shape.skinnedMeshRenderer = skin;
		this.ps.Play();
	}

	// Token: 0x0600246E RID: 9326 RVA: 0x000C3A16 File Offset: 0x000C1C16
	public void ReturnToPool()
	{
		this.InitIfNeeded();
		if (this.poolExists)
		{
			ObjectPools.instance.Destroy(base.gameObject);
		}
	}

	// Token: 0x0600246F RID: 9327 RVA: 0x000C3A36 File Offset: 0x000C1C36
	private void OnParticleSystemStopped()
	{
		this.ReturnToPool();
	}

	// Token: 0x04002FC2 RID: 12226
	private bool initialized;

	// Token: 0x04002FC3 RID: 12227
	private ParticleSystem ps;

	// Token: 0x04002FC4 RID: 12228
	private ParticleSystem.ShapeModule shape;

	// Token: 0x04002FC5 RID: 12229
	private bool poolExists;
}
