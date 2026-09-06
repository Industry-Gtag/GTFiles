using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class LiquidMain : MonoBehaviour
{
	// Token: 0x06000080 RID: 128 RVA: 0x000040D4 File Offset: 0x000022D4
	private void ResetEffector(GameObject obj)
	{
		obj.transform.position = new Vector3(Random.Range(-0.3f, 0.3f), -100f, Random.Range(-0.3f, 0.3f)) * LiquidMain.kPlaneMeshCellSize * (float)LiquidMain.kPlaneMeshResolution;
	}

	// Token: 0x06000081 RID: 129 RVA: 0x0000412C File Offset: 0x0000232C
	public void Start()
	{
		this.m_planeMesh = new Mesh();
		this.m_planeMesh.vertices = new Vector3[]
		{
			new Vector3(-0.5f, 0f, -0.5f) * LiquidMain.kPlaneMeshCellSize,
			new Vector3(-0.5f, 0f, 0.5f) * LiquidMain.kPlaneMeshCellSize,
			new Vector3(0.5f, 0f, 0.5f) * LiquidMain.kPlaneMeshCellSize,
			new Vector3(0.5f, 0f, -0.5f) * LiquidMain.kPlaneMeshCellSize
		};
		this.m_planeMesh.normals = new Vector3[]
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0f)
		};
		this.m_planeMesh.SetIndices(new int[] { 0, 1, 2, 0, 2, 3 }, MeshTopology.Triangles, 0);
		LiquidMain.kNumPlaneCells = LiquidMain.kPlaneMeshResolution * LiquidMain.kPlaneMeshResolution;
		this.m_aaInstancedPlaneCellMatrix = new Matrix4x4[(LiquidMain.kNumPlaneCells + LiquidMain.kNumInstancedPlaneCellPerDrawCall - 1) / LiquidMain.kNumInstancedPlaneCellPerDrawCall][];
		for (int i = 0; i < this.m_aaInstancedPlaneCellMatrix.Length; i++)
		{
			this.m_aaInstancedPlaneCellMatrix[i] = new Matrix4x4[LiquidMain.kNumInstancedPlaneCellPerDrawCall];
		}
		Vector3 vector = new Vector3(-0.5f, 0f, -0.5f) * LiquidMain.kPlaneMeshCellSize * (float)LiquidMain.kPlaneMeshResolution;
		for (int j = 0; j < LiquidMain.kPlaneMeshResolution; j++)
		{
			for (int k = 0; k < LiquidMain.kPlaneMeshResolution; k++)
			{
				int num = j * LiquidMain.kPlaneMeshResolution + k;
				Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3((float)k, 0f, (float)j) * LiquidMain.kPlaneMeshCellSize + vector, Quaternion.identity, Vector3.one);
				this.m_aaInstancedPlaneCellMatrix[num / LiquidMain.kNumInstancedPlaneCellPerDrawCall][num % LiquidMain.kNumInstancedPlaneCellPerDrawCall] = matrix4x;
			}
		}
		this.m_aMovingEffector = new GameObject[LiquidMain.kNumMovingEffectors];
		this.m_aMovingEffectorPhase = new float[LiquidMain.kNumMovingEffectors];
		BoingEffector[] array = new BoingEffector[LiquidMain.kNumMovingEffectors];
		for (int l = 0; l < LiquidMain.kNumMovingEffectors; l++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.Effector);
			this.m_aMovingEffector[l] = gameObject;
			this.ResetEffector(gameObject);
			this.m_aMovingEffectorPhase[l] = -MathUtil.HalfPi + (float)l / (float)LiquidMain.kNumMovingEffectors * MathUtil.Pi;
			array[l] = gameObject.GetComponent<BoingEffector>();
		}
		this.ReactorField.Effectors = array;
	}

	// Token: 0x06000082 RID: 130 RVA: 0x00004418 File Offset: 0x00002618
	public void Update()
	{
		this.ReactorField.UpdateShaderConstants(this.PlaneMaterial, 1f, 1f);
		int num = LiquidMain.kNumPlaneCells;
		for (int i = 0; i < this.m_aaInstancedPlaneCellMatrix.Length; i++)
		{
			Matrix4x4[] array = this.m_aaInstancedPlaneCellMatrix[i];
			Graphics.DrawMeshInstanced(this.m_planeMesh, 0, this.PlaneMaterial, array, Mathf.Min(num, LiquidMain.kNumInstancedPlaneCellPerDrawCall));
			num -= LiquidMain.kNumInstancedPlaneCellPerDrawCall;
		}
		for (int j = 0; j < LiquidMain.kNumMovingEffectors; j++)
		{
			GameObject gameObject = this.m_aMovingEffector[j];
			float num2 = this.m_aMovingEffectorPhase[j];
			num2 += MathUtil.TwoPi * LiquidMain.kMovingEffectorPhaseSpeed * Time.deltaTime;
			float num3 = num2;
			num2 = Mathf.Repeat(num2 + MathUtil.HalfPi, MathUtil.Pi) - MathUtil.HalfPi;
			this.m_aMovingEffectorPhase[j] = num2;
			if (num2 < num3 - 0.01f)
			{
				this.ResetEffector(gameObject);
			}
			Vector3 position = gameObject.transform.position;
			position.y = Mathf.Tan(Mathf.Clamp(num2, -MathUtil.HalfPi + 0.2f, MathUtil.HalfPi - 0.2f)) + 3.5f;
			gameObject.transform.position = position;
		}
	}

	// Token: 0x04000077 RID: 119
	public Material PlaneMaterial;

	// Token: 0x04000078 RID: 120
	public BoingReactorField ReactorField;

	// Token: 0x04000079 RID: 121
	public GameObject Effector;

	// Token: 0x0400007A RID: 122
	private static readonly float kPlaneMeshCellSize = 0.25f;

	// Token: 0x0400007B RID: 123
	private static readonly int kNumInstancedPlaneCellPerDrawCall = 1000;

	// Token: 0x0400007C RID: 124
	private static readonly int kNumMovingEffectors = 5;

	// Token: 0x0400007D RID: 125
	private static readonly float kMovingEffectorPhaseSpeed = 0.5f;

	// Token: 0x0400007E RID: 126
	private static int kNumPlaneCells;

	// Token: 0x0400007F RID: 127
	private static readonly int kPlaneMeshResolution = 64;

	// Token: 0x04000080 RID: 128
	private Mesh m_planeMesh;

	// Token: 0x04000081 RID: 129
	private Matrix4x4[][] m_aaInstancedPlaneCellMatrix;

	// Token: 0x04000082 RID: 130
	private GameObject[] m_aMovingEffector;

	// Token: 0x04000083 RID: 131
	private float[] m_aMovingEffectorPhase;
}
