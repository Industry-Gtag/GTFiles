using System;
using UnityEngine;

// Token: 0x02000391 RID: 913
public struct TexFormatInfo
{
	// Token: 0x06001616 RID: 5654 RVA: 0x00081028 File Offset: 0x0007F228
	public TexFormatInfo(Texture2D tex2d)
	{
		this.width = tex2d.width;
		this.height = tex2d.height;
		this.format = tex2d.format;
		this.filterMode = tex2d.filterMode;
		this.isLinearColor = !tex2d.isDataSRGB;
		this.mipmapCount = tex2d.mipmapCount;
		this.isValid = true;
	}

	// Token: 0x06001617 RID: 5655 RVA: 0x00081088 File Offset: 0x0007F288
	public override string ToString()
	{
		return string.Concat(new string[]
		{
			"TexFormatInfo(isValid: ",
			this.isValid.ToString(),
			", width: ",
			this.width.ToString(),
			", height: ",
			this.height.ToString(),
			", format: ",
			this.format.ToString(),
			", filterMode: ",
			this.filterMode.ToString(),
			", isLinearColor: ",
			this.isLinearColor.ToString(),
			", mipmapCount: ",
			this.mipmapCount.ToString(),
			")"
		});
	}

	// Token: 0x04002068 RID: 8296
	public bool isValid;

	// Token: 0x04002069 RID: 8297
	public int width;

	// Token: 0x0400206A RID: 8298
	public int height;

	// Token: 0x0400206B RID: 8299
	public TextureFormat format;

	// Token: 0x0400206C RID: 8300
	public FilterMode filterMode;

	// Token: 0x0400206D RID: 8301
	public int mipmapCount;

	// Token: 0x0400206E RID: 8302
	public bool isLinearColor;
}
