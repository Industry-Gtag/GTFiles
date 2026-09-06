using System;
using UnityEngine;

// Token: 0x02000AFA RID: 2810
public static class GradientHelper
{
	// Token: 0x060047ED RID: 18413 RVA: 0x0018438C File Offset: 0x0018258C
	public static Gradient FromColor(Color color)
	{
		float a = color.a;
		Color color2 = color;
		color2.a = 1f;
		return new Gradient
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(color2, 1f)
			},
			alphaKeys = new GradientAlphaKey[]
			{
				new GradientAlphaKey(a, 1f)
			}
		};
	}
}
