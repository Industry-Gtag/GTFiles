using System;
using System.Diagnostics;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020011E3 RID: 4579
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class VectorLabelTextAttribute : PropertyAttribute
	{
		// Token: 0x06007460 RID: 29792 RVA: 0x0025D762 File Offset: 0x0025B962
		public VectorLabelTextAttribute(params string[] labels)
			: this(-1, labels)
		{
		}

		// Token: 0x06007461 RID: 29793 RVA: 0x00013383 File Offset: 0x00011583
		public VectorLabelTextAttribute(int width, params string[] labels)
		{
		}
	}
}
