using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000E7B RID: 3707
public static class GTUberShaderUtils
{
	// Token: 0x06005A32 RID: 23090 RVA: 0x001D3E52 File Offset: 0x001D2052
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SetStencilComparison(this Material m, GTShaderStencilCompare cmp)
	{
		m.SetFloat(GTUberShaderUtils._StencilComparison, (float)cmp);
	}

	// Token: 0x06005A33 RID: 23091 RVA: 0x001D3E66 File Offset: 0x001D2066
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SetStencilPassFrontOp(this Material m, GTShaderStencilOp op)
	{
		m.SetFloat(GTUberShaderUtils._StencilPassFront, (float)op);
	}

	// Token: 0x06005A34 RID: 23092 RVA: 0x001D3E7A File Offset: 0x001D207A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SetStencilReferenceValue(this Material m, int value)
	{
		m.SetFloat(GTUberShaderUtils._StencilReference, (float)value);
	}

	// Token: 0x06005A35 RID: 23093 RVA: 0x001D3E90 File Offset: 0x001D2090
	public static void SetVisibleToXRay(this Material m, bool visible, bool saveToDisk = false)
	{
		GTShaderStencilCompare gtshaderStencilCompare = (visible ? GTShaderStencilCompare.Equal : GTShaderStencilCompare.NotEqual);
		GTShaderStencilOp gtshaderStencilOp = (visible ? GTShaderStencilOp.Replace : GTShaderStencilOp.Keep);
		m.SetStencilComparison(gtshaderStencilCompare);
		m.SetStencilPassFrontOp(gtshaderStencilOp);
		m.SetStencilReferenceValue(7);
	}

	// Token: 0x06005A36 RID: 23094 RVA: 0x001D3EC4 File Offset: 0x001D20C4
	public static void SetRevealsXRay(this Material m, bool reveals, bool changeQueue = true, bool saveToDisk = false)
	{
		m.SetFloat(GTUberShaderUtils._ZWrite, (float)(reveals ? 0 : 1));
		m.SetFloat(GTUberShaderUtils._ColorMask_, (float)(reveals ? 0 : 14));
		m.SetStencilComparison(GTShaderStencilCompare.Disabled);
		m.SetStencilPassFrontOp(reveals ? GTShaderStencilOp.Replace : GTShaderStencilOp.Keep);
		m.SetStencilReferenceValue(reveals ? 7 : 0);
		if (changeQueue)
		{
			int renderQueue = m.renderQueue;
			m.renderQueue = renderQueue + (reveals ? (-1) : 1);
		}
	}

	// Token: 0x06005A37 RID: 23095 RVA: 0x001D3F3C File Offset: 0x001D213C
	public static int GetNearestRenderQueue(this Material m, out RenderQueue queue)
	{
		int renderQueue = m.renderQueue;
		int num = -1;
		int num2 = int.MaxValue;
		for (int i = 0; i < GTUberShaderUtils.kRenderQueueInts.Length; i++)
		{
			int num3 = GTUberShaderUtils.kRenderQueueInts[i];
			int num4 = Math.Abs(num3 - renderQueue);
			if (num2 > num4)
			{
				num = num3;
				num2 = num4;
			}
		}
		queue = (RenderQueue)num;
		return num;
	}

	// Token: 0x06005A38 RID: 23096 RVA: 0x001D3F8D File Offset: 0x001D218D
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitOnLoad()
	{
		GTUberShaderUtils.kUberShader = Shader.Find("GorillaTag/UberShader");
	}

	// Token: 0x04006ADE RID: 27358
	private static Shader kUberShader;

	// Token: 0x04006ADF RID: 27359
	private static readonly ShaderHashId _StencilComparison = "_StencilComparison";

	// Token: 0x04006AE0 RID: 27360
	private static readonly ShaderHashId _StencilPassFront = "_StencilPassFront";

	// Token: 0x04006AE1 RID: 27361
	private static readonly ShaderHashId _StencilReference = "_StencilReference";

	// Token: 0x04006AE2 RID: 27362
	private static readonly ShaderHashId _ColorMask_ = "_ColorMask_";

	// Token: 0x04006AE3 RID: 27363
	private static readonly ShaderHashId _ManualZWrite = "_ManualZWrite";

	// Token: 0x04006AE4 RID: 27364
	private static readonly ShaderHashId _ZWrite = "_ZWrite";

	// Token: 0x04006AE5 RID: 27365
	private static readonly int[] kRenderQueueInts = new int[] { 1000, 2000, 2450, 2500, 3000, 4000 };
}
