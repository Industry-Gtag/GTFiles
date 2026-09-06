using System;
using UnityEngine;

namespace Voxels
{
	// Token: 0x020013A8 RID: 5032
	public class TextureArrayUtil : MonoBehaviour
	{
		// Token: 0x17000C3F RID: 3135
		// (get) Token: 0x06007D77 RID: 32119 RVA: 0x00291F3B File Offset: 0x0029013B
		private bool UnreadableTextureFound
		{
			get
			{
				return !this.TexturesReadable;
			}
		}

		// Token: 0x17000C40 RID: 3136
		// (get) Token: 0x06007D78 RID: 32120 RVA: 0x00291F48 File Offset: 0x00290148
		private bool TexturesReadable
		{
			get
			{
				foreach (TextureEntry textureEntry in this.textureEntries)
				{
					if (textureEntry.Diffuse == null || textureEntry.Normal == null)
					{
						return false;
					}
					if (!textureEntry.Diffuse.isReadable || !textureEntry.Normal.isReadable)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x06007D79 RID: 32121 RVA: 0x00291FB0 File Offset: 0x002901B0
		public static Texture2DArray CreateTextureArray(Texture2D[] textures)
		{
			if (textures == null || textures.Length == 0)
			{
				return null;
			}
			int width = textures[0].width;
			int height = textures[0].height;
			bool flag = textures[0].mipmapCount > 1;
			int num = textures.Length;
			Texture2DArray texture2DArray = new Texture2DArray(width, height, num, textures[0].format, flag);
			for (int i = 0; i < num; i++)
			{
				Graphics.CopyTexture(textures[i], 0, texture2DArray, i);
			}
			texture2DArray.Apply(false, true);
			return texture2DArray;
		}

		// Token: 0x04009087 RID: 36999
		public TextureEntry[] textureEntries;

		// Token: 0x04009088 RID: 37000
		public Texture2DArray diffuseArray;

		// Token: 0x04009089 RID: 37001
		public Texture2DArray normalArray;

		// Token: 0x0400908A RID: 37002
		public Material material;

		// Token: 0x0400908B RID: 37003
		public bool linearNormalMaps = true;

		// Token: 0x0400908C RID: 37004
		public string diffuseName = "_Diffuse";

		// Token: 0x0400908D RID: 37005
		public string normalName = "_Normal";
	}
}
