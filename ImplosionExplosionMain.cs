using System;
using BoingKit;
using UnityEngine;

// Token: 0x0200001E RID: 30
public class ImplosionExplosionMain : MonoBehaviour
{
	// Token: 0x06000073 RID: 115 RVA: 0x00003D40 File Offset: 0x00001F40
	public void Start()
	{
		this.m_aaInstancedDiamondMatrix = new Matrix4x4[(this.NumDiamonds + ImplosionExplosionMain.kNumInstancedBushesPerDrawCall - 1) / ImplosionExplosionMain.kNumInstancedBushesPerDrawCall][];
		for (int i = 0; i < this.m_aaInstancedDiamondMatrix.Length; i++)
		{
			this.m_aaInstancedDiamondMatrix[i] = new Matrix4x4[ImplosionExplosionMain.kNumInstancedBushesPerDrawCall];
		}
		for (int j = 0; j < this.NumDiamonds; j++)
		{
			float num = Random.Range(0.1f, 0.4f);
			Vector3 vector = new Vector3(Random.Range(-3.5f, 3.5f), Random.Range(0.5f, 7f), Random.Range(-3.5f, 3.5f));
			Quaternion quaternion = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
			this.m_aaInstancedDiamondMatrix[j / ImplosionExplosionMain.kNumInstancedBushesPerDrawCall][j % ImplosionExplosionMain.kNumInstancedBushesPerDrawCall].SetTRS(vector, quaternion, num * Vector3.one);
		}
	}

	// Token: 0x06000074 RID: 116 RVA: 0x00003E50 File Offset: 0x00002050
	public void Update()
	{
		Mesh sharedMesh = this.Diamond.GetComponent<MeshFilter>().sharedMesh;
		Material sharedMaterial = this.Diamond.GetComponent<MeshRenderer>().sharedMaterial;
		if (this.m_diamondMaterialProps == null)
		{
			this.m_diamondMaterialProps = new MaterialPropertyBlock();
		}
		if (this.ReactorField.UpdateShaderConstants(this.m_diamondMaterialProps, 1f, 1f))
		{
			foreach (Matrix4x4[] array in this.m_aaInstancedDiamondMatrix)
			{
				Graphics.DrawMeshInstanced(sharedMesh, 0, sharedMaterial, array, array.Length, this.m_diamondMaterialProps);
			}
		}
	}

	// Token: 0x04000068 RID: 104
	public BoingReactorField ReactorField;

	// Token: 0x04000069 RID: 105
	public GameObject Diamond;

	// Token: 0x0400006A RID: 106
	public int NumDiamonds;

	// Token: 0x0400006B RID: 107
	private static readonly int kNumInstancedBushesPerDrawCall = 1000;

	// Token: 0x0400006C RID: 108
	private Matrix4x4[][] m_aaInstancedDiamondMatrix;

	// Token: 0x0400006D RID: 109
	private MaterialPropertyBlock m_diamondMaterialProps;
}
