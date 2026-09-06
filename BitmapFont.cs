using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020004EB RID: 1259
public class BitmapFont : ScriptableObject
{
	// Token: 0x06001EA3 RID: 7843 RVA: 0x000A3AE0 File Offset: 0x000A1CE0
	private void OnEnable()
	{
		this._charToSymbol = this.symbols.ToDictionary((BitmapFont.SymbolData s) => s.character, (BitmapFont.SymbolData s) => s);
	}

	// Token: 0x06001EA4 RID: 7844 RVA: 0x000A3B3C File Offset: 0x000A1D3C
	public void RenderToTexture(Texture2D target, string text)
	{
		if (text == null)
		{
			text = string.Empty;
		}
		int num = target.width * target.height;
		if (this._empty.Length != num)
		{
			this._empty = new Color[num];
			for (int i = 0; i < this._empty.Length; i++)
			{
				this._empty[i] = Color.black;
			}
		}
		target.SetPixels(this._empty);
		int length = text.Length;
		int num2 = 1;
		int width = this.fontImage.width;
		int height = this.fontImage.height;
		for (int j = 0; j < length; j++)
		{
			char c = text[j];
			BitmapFont.SymbolData symbolData = this._charToSymbol[c];
			int width2 = symbolData.width;
			int height2 = symbolData.height;
			int x = symbolData.x;
			int y = symbolData.y;
			Graphics.CopyTexture(this.fontImage, 0, 0, x, height - (y + height2), width2, height2, target, 0, 0, num2, 2 + symbolData.yoffset);
			num2 += width2 + 1;
		}
		target.Apply(false);
	}

	// Token: 0x040028DA RID: 10458
	public Texture2D fontImage;

	// Token: 0x040028DB RID: 10459
	public TextAsset fontJson;

	// Token: 0x040028DC RID: 10460
	public int symbolPixelsPerUnit = 1;

	// Token: 0x040028DD RID: 10461
	public string characterMap;

	// Token: 0x040028DE RID: 10462
	[Space]
	public BitmapFont.SymbolData[] symbols = new BitmapFont.SymbolData[0];

	// Token: 0x040028DF RID: 10463
	private Dictionary<char, BitmapFont.SymbolData> _charToSymbol;

	// Token: 0x040028E0 RID: 10464
	private Color[] _empty = new Color[0];

	// Token: 0x020004EC RID: 1260
	[Serializable]
	public struct SymbolData
	{
		// Token: 0x040028E1 RID: 10465
		public char character;

		// Token: 0x040028E2 RID: 10466
		[Space]
		public int id;

		// Token: 0x040028E3 RID: 10467
		public int width;

		// Token: 0x040028E4 RID: 10468
		public int height;

		// Token: 0x040028E5 RID: 10469
		public int x;

		// Token: 0x040028E6 RID: 10470
		public int y;

		// Token: 0x040028E7 RID: 10471
		public int xadvance;

		// Token: 0x040028E8 RID: 10472
		public int yoffset;
	}
}
