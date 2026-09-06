using System;
using UnityEngine;

// Token: 0x02000340 RID: 832
public class GTShaderGlobals : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x17000208 RID: 520
	// (get) Token: 0x0600146B RID: 5227 RVA: 0x0006DE48 File Offset: 0x0006C048
	public static Vector3 WorldSpaceCameraPos
	{
		get
		{
			return GTShaderGlobals.gMainCameraWorldPos;
		}
	}

	// Token: 0x17000209 RID: 521
	// (get) Token: 0x0600146C RID: 5228 RVA: 0x0006DE4F File Offset: 0x0006C04F
	public static float Time
	{
		get
		{
			return GTShaderGlobals.gTime;
		}
	}

	// Token: 0x1700020A RID: 522
	// (get) Token: 0x0600146D RID: 5229 RVA: 0x0006DE56 File Offset: 0x0006C056
	public static int Frame
	{
		get
		{
			return GTShaderGlobals.gIFrame;
		}
	}

	// Token: 0x0600146E RID: 5230 RVA: 0x0006DE5D File Offset: 0x0006C05D
	private void Awake()
	{
		GTShaderGlobals.gMainCamera = Camera.main;
		if (GTShaderGlobals.gMainCamera)
		{
			GTShaderGlobals.gMainCameraXform = GTShaderGlobals.gMainCamera.transform;
			GTShaderGlobals.gMainCameraWorldPos = GTShaderGlobals.gMainCameraXform.position;
		}
		this.SliceUpdate();
	}

	// Token: 0x0600146F RID: 5231 RVA: 0x0006DE99 File Offset: 0x0006C099
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Initialize()
	{
		GTShaderGlobals.InitBlueNoiseTex();
	}

	// Token: 0x06001470 RID: 5232 RVA: 0x00019260 File Offset: 0x00017460
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001471 RID: 5233 RVA: 0x00019269 File Offset: 0x00017469
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001472 RID: 5234 RVA: 0x0006DEA0 File Offset: 0x0006C0A0
	public void SliceUpdate()
	{
		GTShaderGlobals.UpdateTime();
		GTShaderGlobals.UpdateFrame();
		GTShaderGlobals.UpdateCamera();
	}

	// Token: 0x06001473 RID: 5235 RVA: 0x0006DEB1 File Offset: 0x0006C0B1
	private static void UpdateFrame()
	{
		GTShaderGlobals.gIFrame = global::UnityEngine.Time.frameCount;
		Shader.SetGlobalInteger(GTShaderGlobals._GT_iFrame, GTShaderGlobals.gIFrame);
	}

	// Token: 0x06001474 RID: 5236 RVA: 0x0006DED1 File Offset: 0x0006C0D1
	private static void UpdateCamera()
	{
		if (!GTShaderGlobals.gMainCameraXform)
		{
			return;
		}
		GTShaderGlobals.gMainCameraWorldPos = GTShaderGlobals.gMainCameraXform.position;
		Shader.SetGlobalVector(GTShaderGlobals._GT_WorldSpaceCameraPos, GTShaderGlobals.gMainCameraWorldPos);
	}

	// Token: 0x06001475 RID: 5237 RVA: 0x0006DF08 File Offset: 0x0006C108
	private static void UpdateTime()
	{
		GTShaderGlobals.gTime = (float)(DateTime.UtcNow - GTShaderGlobals.gStartTime).TotalSeconds;
		Shader.SetGlobalFloat(GTShaderGlobals._GT_Time, GTShaderGlobals.gTime);
	}

	// Token: 0x06001476 RID: 5238 RVA: 0x0006DF46 File Offset: 0x0006C146
	private static void UpdatePawns()
	{
		GTShaderGlobals.gActivePawns = GorillaPawn.ActiveCount;
		GorillaPawn.SyncPawnData();
		Shader.SetGlobalMatrixArray(GTShaderGlobals._GT_PawnData, GTShaderGlobals.gPawnData);
		Shader.SetGlobalInteger(GTShaderGlobals._GT_PawnActiveCount, GTShaderGlobals.gActivePawns);
	}

	// Token: 0x06001477 RID: 5239 RVA: 0x0006DF80 File Offset: 0x0006C180
	private static void InitBlueNoiseTex()
	{
		GTShaderGlobals.gBlueNoiseTex = Resources.Load<Texture2D>("Graphics/Textures/noise_blue_rgba_128");
		GTShaderGlobals.gBlueNoiseTexWH = GTShaderGlobals.gBlueNoiseTex.GetTexelSize();
		Shader.SetGlobalTexture(GTShaderGlobals._GT_BlueNoiseTex, GTShaderGlobals.gBlueNoiseTex);
		Shader.SetGlobalVector(GTShaderGlobals._GT_BlueNoiseTex_WH, GTShaderGlobals.gBlueNoiseTexWH);
	}

	// Token: 0x04001941 RID: 6465
	private static Camera gMainCamera;

	// Token: 0x04001942 RID: 6466
	private static Transform gMainCameraXform;

	// Token: 0x04001943 RID: 6467
	private static Vector3 gMainCameraWorldPos;

	// Token: 0x04001944 RID: 6468
	[Space]
	private static int gIFrame;

	// Token: 0x04001945 RID: 6469
	private static float gTime;

	// Token: 0x04001946 RID: 6470
	[Space]
	private static Texture2D gBlueNoiseTex;

	// Token: 0x04001947 RID: 6471
	private static Vector4 gBlueNoiseTexWH;

	// Token: 0x04001948 RID: 6472
	[Space]
	private static int gActivePawns;

	// Token: 0x04001949 RID: 6473
	[Space]
	private static DateTime gStartTime = DateTime.Today.AddDays(-1.0).ToUniversalTime();

	// Token: 0x0400194A RID: 6474
	private static Matrix4x4[] gPawnData = GorillaPawn.ShaderData;

	// Token: 0x0400194B RID: 6475
	private static ShaderHashId _GT_WorldSpaceCameraPos = "_GT_WorldSpaceCameraPos";

	// Token: 0x0400194C RID: 6476
	private static ShaderHashId _GT_BlueNoiseTex = "_GT_BlueNoiseTex";

	// Token: 0x0400194D RID: 6477
	private static ShaderHashId _GT_BlueNoiseTex_WH = "_GT_BlueNoiseTex_WH";

	// Token: 0x0400194E RID: 6478
	private static ShaderHashId _GT_iFrame = "_GT_iFrame";

	// Token: 0x0400194F RID: 6479
	private static ShaderHashId _GT_Time = "_GT_Time";

	// Token: 0x04001950 RID: 6480
	private static ShaderHashId _GT_PawnData = "_GT_PawnData";

	// Token: 0x04001951 RID: 6481
	private static ShaderHashId _GT_PawnActiveCount = "_GT_PawnActiveCount";
}
