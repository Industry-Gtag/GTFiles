using System;
using System.Collections.Generic;
using System.Linq;
using BoingKit;
using UnityEngine;

// Token: 0x02000023 RID: 35
public class BushFieldReactorFieldMain : MonoBehaviour
{
	// Token: 0x06000089 RID: 137 RVA: 0x00004694 File Offset: 0x00002894
	public void Start()
	{
		Random.InitState(0);
		if (this.Bush.GetComponent<BoingReactorFieldGPUSampler>() == null)
		{
			for (int i = 0; i < this.NumBushes; i++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.Bush);
				float num = Random.Range(this.BushScaleRange.x, this.BushScaleRange.y);
				gameObject.transform.position = new Vector3(Random.Range(-0.5f * this.FieldBounds.x, 0.5f * this.FieldBounds.x), 0.2f * num, Random.Range(-0.5f * this.FieldBounds.y, 0.5f * this.FieldBounds.y));
				gameObject.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
				gameObject.transform.localScale = num * Vector3.one;
				BoingReactorFieldCPUSampler component = gameObject.GetComponent<BoingReactorFieldCPUSampler>();
				if (component != null)
				{
					component.ReactorField = this.ReactorField;
				}
				BoingReactorFieldGPUSampler component2 = gameObject.GetComponent<BoingReactorFieldGPUSampler>();
				if (component2 != null)
				{
					component2.ReactorField = this.ReactorField;
				}
			}
		}
		else
		{
			this.m_aaInstancedBushMatrix = new Matrix4x4[(this.NumBushes + BushFieldReactorFieldMain.kNumInstancedBushesPerDrawCall - 1) / BushFieldReactorFieldMain.kNumInstancedBushesPerDrawCall][];
			for (int j = 0; j < this.m_aaInstancedBushMatrix.Length; j++)
			{
				this.m_aaInstancedBushMatrix[j] = new Matrix4x4[BushFieldReactorFieldMain.kNumInstancedBushesPerDrawCall];
			}
			for (int k = 0; k < this.NumBushes; k++)
			{
				float num2 = Random.Range(this.BushScaleRange.x, this.BushScaleRange.y);
				Vector3 vector = new Vector3(Random.Range(-0.5f * this.FieldBounds.x, 0.5f * this.FieldBounds.x), 0.2f * num2, Random.Range(-0.5f * this.FieldBounds.y, 0.5f * this.FieldBounds.y));
				Quaternion quaternion = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
				this.m_aaInstancedBushMatrix[k / BushFieldReactorFieldMain.kNumInstancedBushesPerDrawCall][k % BushFieldReactorFieldMain.kNumInstancedBushesPerDrawCall].SetTRS(vector, quaternion, num2 * Vector3.one);
			}
		}
		for (int l = 0; l < this.NumBlossoms; l++)
		{
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.Blossom);
			float num3 = Random.Range(this.BlossomScaleRange.x, this.BlossomScaleRange.y);
			gameObject2.transform.position = new Vector3(Random.Range(-0.5f * this.FieldBounds.x, 0.5f * this.FieldBounds.y), 0.2f * num3, Random.Range(-0.5f * this.FieldBounds.y, 0.5f * this.FieldBounds.y));
			gameObject2.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			gameObject2.transform.localScale = num3 * Vector3.one;
			gameObject2.GetComponent<BoingReactorFieldCPUSampler>().ReactorField = this.ReactorField;
		}
		this.m_aSphere = new List<BoingEffector>(this.NumSpheresPerCircle * this.NumCircles);
		for (int m = 0; m < this.NumCircles; m++)
		{
			for (int n = 0; n < this.NumSpheresPerCircle; n++)
			{
				GameObject gameObject3 = Object.Instantiate<GameObject>(this.Sphere);
				this.m_aSphere.Add(gameObject3.GetComponent<BoingEffector>());
			}
		}
		BoingReactorField component3 = this.ReactorField.GetComponent<BoingReactorField>();
		component3.Effectors = ((component3.Effectors != null) ? component3.Effectors.Concat(this.m_aSphere.ToArray()).ToArray<BoingEffector>() : this.m_aSphere.ToArray());
		this.m_basePhase = 0f;
	}

	// Token: 0x0600008A RID: 138 RVA: 0x00004AB4 File Offset: 0x00002CB4
	public void Update()
	{
		int num = 0;
		for (int i = 0; i < this.NumCircles; i++)
		{
			float num2 = this.MaxCircleRadius / (float)(i + 1);
			for (int j = 0; j < this.NumSpheresPerCircle; j++)
			{
				float num3 = this.m_basePhase + (float)j / (float)this.NumSpheresPerCircle * 2f * 3.1415927f;
				num3 *= ((i % 2 == 0) ? 1f : (-1f));
				this.m_aSphere[num].transform.position = new Vector3(num2 * Mathf.Cos(num3), 0.2f, num2 * Mathf.Sin(num3));
				num++;
			}
		}
		this.m_basePhase -= this.CircleSpeed / this.MaxCircleRadius * Time.deltaTime;
		if (this.m_aaInstancedBushMatrix != null)
		{
			Mesh sharedMesh = this.Bush.GetComponent<MeshFilter>().sharedMesh;
			Material sharedMaterial = this.Bush.GetComponent<MeshRenderer>().sharedMaterial;
			if (this.m_bushMaterialProps == null)
			{
				this.m_bushMaterialProps = new MaterialPropertyBlock();
			}
			if (this.ReactorField.UpdateShaderConstants(this.m_bushMaterialProps, 1f, 1f))
			{
				foreach (Matrix4x4[] array in this.m_aaInstancedBushMatrix)
				{
					Graphics.DrawMeshInstanced(sharedMesh, 0, sharedMaterial, array, array.Length, this.m_bushMaterialProps);
				}
			}
		}
	}

	// Token: 0x04000088 RID: 136
	public GameObject Bush;

	// Token: 0x04000089 RID: 137
	public GameObject Blossom;

	// Token: 0x0400008A RID: 138
	public GameObject Sphere;

	// Token: 0x0400008B RID: 139
	public BoingReactorField ReactorField;

	// Token: 0x0400008C RID: 140
	public int NumBushes;

	// Token: 0x0400008D RID: 141
	public Vector2 BushScaleRange;

	// Token: 0x0400008E RID: 142
	public int NumBlossoms;

	// Token: 0x0400008F RID: 143
	public Vector2 BlossomScaleRange;

	// Token: 0x04000090 RID: 144
	public Vector2 FieldBounds;

	// Token: 0x04000091 RID: 145
	public int NumSpheresPerCircle;

	// Token: 0x04000092 RID: 146
	public int NumCircles;

	// Token: 0x04000093 RID: 147
	public float MaxCircleRadius;

	// Token: 0x04000094 RID: 148
	public float CircleSpeed;

	// Token: 0x04000095 RID: 149
	private List<BoingEffector> m_aSphere;

	// Token: 0x04000096 RID: 150
	private float m_basePhase;

	// Token: 0x04000097 RID: 151
	private static readonly int kNumInstancedBushesPerDrawCall = 1000;

	// Token: 0x04000098 RID: 152
	private Matrix4x4[][] m_aaInstancedBushMatrix;

	// Token: 0x04000099 RID: 153
	private MaterialPropertyBlock m_bushMaterialProps;
}
