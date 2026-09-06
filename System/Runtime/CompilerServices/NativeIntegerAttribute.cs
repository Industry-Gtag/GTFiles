using System;
using Microsoft.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
	// Token: 0x0200000B RID: 11
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = false)]
	internal sealed class NativeIntegerAttribute : Attribute
	{
		// Token: 0x06000023 RID: 35 RVA: 0x000025A8 File Offset: 0x000007A8
		public NativeIntegerAttribute()
		{
			this.TransformFlags = new bool[] { true };
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000025C0 File Offset: 0x000007C0
		public NativeIntegerAttribute(bool[] A_1)
		{
			this.TransformFlags = A_1;
		}

		// Token: 0x0400000C RID: 12
		public readonly bool[] TransformFlags;
	}
}
