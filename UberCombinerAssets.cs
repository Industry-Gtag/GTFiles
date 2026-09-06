using System;
using UnityEngine;

// Token: 0x02000E6F RID: 3695
public class UberCombinerAssets : ScriptableObject
{
	// Token: 0x17000897 RID: 2199
	// (get) Token: 0x06005A2B RID: 23083 RVA: 0x001D3DFF File Offset: 0x001D1FFF
	public static UberCombinerAssets Instance
	{
		get
		{
			UberCombinerAssets.gInstance == null;
			return UberCombinerAssets.gInstance;
		}
	}

	// Token: 0x06005A2C RID: 23084 RVA: 0x001D3E12 File Offset: 0x001D2012
	private void OnEnable()
	{
		this.Setup();
	}

	// Token: 0x06005A2D RID: 23085 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Setup()
	{
	}

	// Token: 0x06005A2E RID: 23086 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void ClearMaterialAssets()
	{
	}

	// Token: 0x06005A2F RID: 23087 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void ClearPrefabAssets()
	{
	}

	// Token: 0x04006A86 RID: 27270
	[SerializeField]
	private Object _rootFolder;

	// Token: 0x04006A87 RID: 27271
	[SerializeField]
	private Object _resourcesFolder;

	// Token: 0x04006A88 RID: 27272
	[SerializeField]
	private Object _materialsFolder;

	// Token: 0x04006A89 RID: 27273
	[SerializeField]
	private Object _prefabsFolder;

	// Token: 0x04006A8A RID: 27274
	[Space]
	public Object MeshBakerDefaultCustomizer;

	// Token: 0x04006A8B RID: 27275
	public Material ReferenceUberMaterial;

	// Token: 0x04006A8C RID: 27276
	public Shader TextureArrayCapableShader;

	// Token: 0x04006A8D RID: 27277
	[Space]
	public string RootFolderPath;

	// Token: 0x04006A8E RID: 27278
	public string ResourcesFolderPath;

	// Token: 0x04006A8F RID: 27279
	public string MaterialsFolderPath;

	// Token: 0x04006A90 RID: 27280
	public string PrefabsFolderPath;

	// Token: 0x04006A91 RID: 27281
	private static UberCombinerAssets gInstance;
}
