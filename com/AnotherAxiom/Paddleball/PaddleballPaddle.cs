using System;
using UnityEngine;

namespace com.AnotherAxiom.Paddleball
{
	// Token: 0x02001174 RID: 4468
	public class PaddleballPaddle : MonoBehaviour
	{
		// Token: 0x17000AAF RID: 2735
		// (get) Token: 0x06007005 RID: 28677 RVA: 0x00242622 File Offset: 0x00240822
		public bool Right
		{
			get
			{
				return this.right;
			}
		}

		// Token: 0x04008013 RID: 32787
		[SerializeField]
		private bool right;
	}
}
