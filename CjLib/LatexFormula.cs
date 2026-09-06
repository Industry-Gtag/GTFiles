using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200140D RID: 5133
	[ExecuteInEditMode]
	public class LatexFormula : MonoBehaviour
	{
		// Token: 0x04009251 RID: 37457
		public static readonly string BaseUrl = "http://tex.s2cms.ru/svg/f(x) ";

		// Token: 0x04009252 RID: 37458
		private int m_hash = LatexFormula.BaseUrl.GetHashCode();

		// Token: 0x04009253 RID: 37459
		[SerializeField]
		private string m_formula = "";

		// Token: 0x04009254 RID: 37460
		private Texture m_texture;
	}
}
