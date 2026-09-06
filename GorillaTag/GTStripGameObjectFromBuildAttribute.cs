using System;

namespace GorillaTag
{
	// Token: 0x020011E2 RID: 4578
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class GTStripGameObjectFromBuildAttribute : Attribute
	{
		// Token: 0x17000B43 RID: 2883
		// (get) Token: 0x0600745E RID: 29790 RVA: 0x0025D74B File Offset: 0x0025B94B
		public string Condition { get; }

		// Token: 0x0600745F RID: 29791 RVA: 0x0025D753 File Offset: 0x0025B953
		public GTStripGameObjectFromBuildAttribute(string condition = "")
		{
			this.Condition = condition;
		}
	}
}
