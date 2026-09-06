using System;
using UnityEngine;

// Token: 0x020004EE RID: 1262
public class BitmapFontText : MonoBehaviour
{
	// Token: 0x06001EAA RID: 7850 RVA: 0x000A3C8B File Offset: 0x000A1E8B
	private void Awake()
	{
		this.Init();
		this.Render();
	}

	// Token: 0x06001EAB RID: 7851 RVA: 0x000A3C99 File Offset: 0x000A1E99
	public void Render()
	{
		this.font.RenderToTexture(this.texture, this.uppercaseOnly ? this.text.ToUpperInvariant() : this.text);
	}

	// Token: 0x06001EAC RID: 7852 RVA: 0x000A3CC8 File Offset: 0x000A1EC8
	public void Init()
	{
		this.texture = new Texture2D(this.textArea.x, this.textArea.y, this.font.fontImage.format, false);
		this.texture.filterMode = FilterMode.Point;
		this.material = new Material(this.renderer.sharedMaterial);
		this.material.mainTexture = this.texture;
		this.renderer.sharedMaterial = this.material;
	}

	// Token: 0x040028EC RID: 10476
	public string text;

	// Token: 0x040028ED RID: 10477
	public bool uppercaseOnly;

	// Token: 0x040028EE RID: 10478
	public Vector2Int textArea;

	// Token: 0x040028EF RID: 10479
	[Space]
	public Renderer renderer;

	// Token: 0x040028F0 RID: 10480
	public Texture2D texture;

	// Token: 0x040028F1 RID: 10481
	public Material material;

	// Token: 0x040028F2 RID: 10482
	public BitmapFont font;
}
