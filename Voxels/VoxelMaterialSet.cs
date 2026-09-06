using System;
using System.Linq;
using Pooling;
using UnityEngine;

namespace Voxels
{
	// Token: 0x020013C5 RID: 5061
	[CreateAssetMenu(fileName = "VoxelMaterialSet", menuName = "Voxels/VoxelMaterialSet")]
	public class VoxelMaterialSet : ScriptableObject
	{
		// Token: 0x17000C54 RID: 3156
		// (get) Token: 0x06007E99 RID: 32409 RVA: 0x00297CAF File Offset: 0x00295EAF
		// (set) Token: 0x06007E9A RID: 32410 RVA: 0x00297CB7 File Offset: 0x00295EB7
		private Texture2DArray TextureArray { get; set; }

		// Token: 0x17000C55 RID: 3157
		// (get) Token: 0x06007E9B RID: 32411 RVA: 0x00297CC0 File Offset: 0x00295EC0
		// (set) Token: 0x06007E9C RID: 32412 RVA: 0x00297CCE File Offset: 0x00295ECE
		public Material Material
		{
			get
			{
				this.Init();
				return this._material;
			}
			set
			{
				this._material = value;
			}
		}

		// Token: 0x06007E9D RID: 32413 RVA: 0x00297CD8 File Offset: 0x00295ED8
		private void Init()
		{
			if (this._initialized)
			{
				return;
			}
			this.TextureArray = TextureArrayUtil.CreateTextureArray(this.Materials.Select((VoxelMaterial m) => m.texture).ToArray<Texture2D>());
			if (this.TextureArray)
			{
				this.TextureArray.name = "VoxelMaterialTextureArray";
			}
			this._material = new Material(Shader.Find("Shader Graphs/TriplanarTextureArrayGraph_Unlit"));
			this._material.name = "VoxelMaterial";
			this._material.SetFloat("_Tile", this.tile);
			this._material.SetFloat("_BacklightPower", this.backlightPower);
			this._material.SetTexture("_Diffuse", this.TextureArray);
			this._initialized = true;
		}

		// Token: 0x06007E9E RID: 32414 RVA: 0x00297DB4 File Offset: 0x00295FB4
		public int GetHardness(byte material)
		{
			if ((int)material >= this.Materials.Length)
			{
				return 1;
			}
			return this.Materials[(int)material].hardness;
		}

		// Token: 0x06007E9F RID: 32415 RVA: 0x00297DE4 File Offset: 0x00295FE4
		public void PlayDigFX(Vector3 position, Vector3 normal, int[] amounts)
		{
			int frameCount = Time.frameCount;
			if (this._lastFrame != frameCount)
			{
				this._callCount = 0;
			}
			if (this._callCount >= 5)
			{
				return;
			}
			for (int i = 0; i < this.Materials.Length; i++)
			{
				int num = amounts[i];
				if (num > 0)
				{
					this._lastFrame = frameCount;
					this._callCount++;
					((num >= 20) ? this.Materials[i].digBigFX : this.Materials[i].digFX).Get(position, Quaternion.LookRotation(normal), null);
				}
			}
		}

		// Token: 0x0400911F RID: 37151
		public VoxelMaterial[] Materials;

		// Token: 0x04009120 RID: 37152
		[SerializeField]
		private float tile = 0.2f;

		// Token: 0x04009121 RID: 37153
		[SerializeField]
		private float backlightPower = 4f;

		// Token: 0x04009123 RID: 37155
		private bool _initialized;

		// Token: 0x04009124 RID: 37156
		private Material _material;

		// Token: 0x04009125 RID: 37157
		private int _lastFrame;

		// Token: 0x04009126 RID: 37158
		private int _callCount;
	}
}
