using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003D8 RID: 984
[ExecuteAlways]
public class GTShaderVolume : MonoBehaviour
{
	// Token: 0x0600176D RID: 5997 RVA: 0x000872A4 File Offset: 0x000854A4
	private void OnEnable()
	{
		if (GTShaderVolume.gVolumes.Count > 16)
		{
			return;
		}
		if (!GTShaderVolume.gVolumes.Contains(this))
		{
			GTShaderVolume.gVolumes.Add(this);
		}
	}

	// Token: 0x0600176E RID: 5998 RVA: 0x000872CD File Offset: 0x000854CD
	private void OnDisable()
	{
		GTShaderVolume.gVolumes.Remove(this);
	}

	// Token: 0x0600176F RID: 5999 RVA: 0x000872DC File Offset: 0x000854DC
	public static void SyncVolumeData()
	{
		m4x4 m4x = default(m4x4);
		int count = GTShaderVolume.gVolumes.Count;
		for (int i = 0; i < 16; i++)
		{
			if (i >= count)
			{
				MatrixUtils.Clear(ref GTShaderVolume.ShaderData[i]);
			}
			else
			{
				GTShaderVolume gtshaderVolume = GTShaderVolume.gVolumes[i];
				if (!gtshaderVolume)
				{
					MatrixUtils.Clear(ref GTShaderVolume.ShaderData[i]);
				}
				else
				{
					Transform transform = gtshaderVolume.transform;
					Vector4 vector = transform.position;
					Vector4 vector2 = transform.rotation.ToVector();
					Vector4 vector3 = transform.localScale;
					m4x.SetRow0(ref vector);
					m4x.SetRow1(ref vector2);
					m4x.SetRow2(ref vector3);
					m4x.Push(ref GTShaderVolume.ShaderData[i]);
				}
			}
		}
		Shader.SetGlobalInteger(GTShaderVolume._GT_ShaderVolumesActive, count);
		Shader.SetGlobalMatrixArray(GTShaderVolume._GT_ShaderVolumes, GTShaderVolume.ShaderData);
	}

	// Token: 0x040022AF RID: 8879
	public const int MAX_VOLUMES = 16;

	// Token: 0x040022B0 RID: 8880
	private static Matrix4x4[] ShaderData = new Matrix4x4[16];

	// Token: 0x040022B1 RID: 8881
	[Space]
	private static List<GTShaderVolume> gVolumes = new List<GTShaderVolume>(16);

	// Token: 0x040022B2 RID: 8882
	private static ShaderHashId _GT_ShaderVolumes = "_GT_ShaderVolumes";

	// Token: 0x040022B3 RID: 8883
	private static ShaderHashId _GT_ShaderVolumesActive = "_GT_ShaderVolumesActive";
}
