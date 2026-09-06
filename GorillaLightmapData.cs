using System;
using UnityEngine;

// Token: 0x02000892 RID: 2194
public class GorillaLightmapData : MonoBehaviour
{
	// Token: 0x0600393F RID: 14655 RVA: 0x0013832C File Offset: 0x0013652C
	public void Awake()
	{
		this.lights = new Color[this.lightTextures.Length][];
		this.dirs = new Color[this.dirTextures.Length][];
		for (int i = 0; i < this.dirTextures.Length; i++)
		{
			float value = Random.value;
			Debug.Log(value.ToString() + " before load " + Time.realtimeSinceStartup.ToString());
			this.dirs[i] = this.dirTextures[i].GetPixels();
			this.lights[i] = this.lightTextures[i].GetPixels();
			Debug.Log(value.ToString() + " after load " + Time.realtimeSinceStartup.ToString());
		}
	}

	// Token: 0x04004953 RID: 18771
	[SerializeField]
	public Texture2D[] dirTextures;

	// Token: 0x04004954 RID: 18772
	[SerializeField]
	public Texture2D[] lightTextures;

	// Token: 0x04004955 RID: 18773
	public Color[][] lights;

	// Token: 0x04004956 RID: 18774
	public Color[][] dirs;

	// Token: 0x04004957 RID: 18775
	public bool done;
}
