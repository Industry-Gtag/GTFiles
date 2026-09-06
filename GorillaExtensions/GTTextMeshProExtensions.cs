using System;
using Cysharp.Text;
using TMPro;

namespace GorillaExtensions
{
	// Token: 0x020011C4 RID: 4548
	public static class GTTextMeshProExtensions
	{
		// Token: 0x060072AC RID: 29356 RVA: 0x00255B0C File Offset: 0x00253D0C
		public static void SetTextToZString(this TMP_Text textMono, Utf16ValueStringBuilder zStringBuilder)
		{
			ArraySegment<char> arraySegment = zStringBuilder.AsArraySegment();
			textMono.SetCharArray(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
		}
	}
}
