using System;
using UnityEngine;

// Token: 0x0200028E RID: 654
[RequireComponent(typeof(ParticleSystem))]
public class ParticleVertexSnapper : MonoBehaviour
{
	// Token: 0x0600119B RID: 4507 RVA: 0x0005EA08 File Offset: 0x0005CC08
	private void Start()
	{
		this.particleSystemComponent = base.GetComponent<ParticleSystem>();
		this.bakedMesh = new Mesh();
		if (this.targetSkinnedMesh == null)
		{
			Debug.LogError("Please assign a Target Skinned Mesh Renderer!", this);
		}
	}

	// Token: 0x0600119C RID: 4508 RVA: 0x0005EA3C File Offset: 0x0005CC3C
	private void LateUpdate()
	{
		if (this.targetSkinnedMesh == null || this.particleSystemComponent == null)
		{
			return;
		}
		this.targetSkinnedMesh.BakeMesh(this.bakedMesh);
		this.vertexPositions = this.bakedMesh.vertices;
		if (this.vertexPositions.Length == 0)
		{
			return;
		}
		int maxParticles = this.particleSystemComponent.main.maxParticles;
		if (this.particles == null || this.particles.Length < maxParticles)
		{
			this.particles = new ParticleSystem.Particle[maxParticles];
		}
		int num = this.particleSystemComponent.GetParticles(this.particles);
		for (int i = 0; i < num; i++)
		{
			int num2 = (int)((ulong)this.particles[i].randomSeed % (ulong)((long)this.vertexPositions.Length));
			Vector3 vector = this.targetSkinnedMesh.transform.TransformPoint(this.vertexPositions[num2]);
			if (this.particleSystemComponent.main.simulationSpace == ParticleSystemSimulationSpace.Local)
			{
				this.particles[i].position = base.transform.InverseTransformPoint(vector);
			}
			else
			{
				this.particles[i].position = vector;
			}
		}
		this.particleSystemComponent.SetParticles(this.particles, num);
	}

	// Token: 0x0600119D RID: 4509 RVA: 0x0005EB81 File Offset: 0x0005CD81
	private void OnDestroy()
	{
		if (this.bakedMesh != null)
		{
			Object.Destroy(this.bakedMesh);
		}
	}

	// Token: 0x040014FA RID: 5370
	[SerializeField]
	private SkinnedMeshRenderer targetSkinnedMesh;

	// Token: 0x040014FB RID: 5371
	private ParticleSystem particleSystemComponent;

	// Token: 0x040014FC RID: 5372
	private ParticleSystem.Particle[] particles;

	// Token: 0x040014FD RID: 5373
	private Mesh bakedMesh;

	// Token: 0x040014FE RID: 5374
	private Vector3[] vertexPositions;
}
