using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200120B RID: 4619
	[CreateAssetMenu(fileName = "New String List", menuName = "String List")]
	public class StringList : ScriptableObject
	{
		// Token: 0x17000B56 RID: 2902
		// (get) Token: 0x0600750E RID: 29966 RVA: 0x002602BE File Offset: 0x0025E4BE
		public string[] Strings
		{
			get
			{
				return this.strings;
			}
		}

		// Token: 0x040084C4 RID: 33988
		[SerializeField]
		private string[] strings;
	}
}
