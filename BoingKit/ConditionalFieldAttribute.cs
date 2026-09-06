using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200145B RID: 5211
	[AttributeUsage(AttributeTargets.Field)]
	public class ConditionalFieldAttribute : PropertyAttribute
	{
		// Token: 0x17000C9F RID: 3231
		// (get) Token: 0x0600835C RID: 33628 RVA: 0x002B02BF File Offset: 0x002AE4BF
		public bool ShowRange
		{
			get
			{
				return this.Min != this.Max;
			}
		}

		// Token: 0x0600835D RID: 33629 RVA: 0x002B02D4 File Offset: 0x002AE4D4
		public ConditionalFieldAttribute(string propertyToCheck = null, object compareValue = null, object compareValue2 = null, object compareValue3 = null, object compareValue4 = null, object compareValue5 = null, object compareValue6 = null)
		{
			this.PropertyToCheck = propertyToCheck;
			this.CompareValue = compareValue;
			this.CompareValue2 = compareValue2;
			this.CompareValue3 = compareValue3;
			this.CompareValue4 = compareValue4;
			this.CompareValue5 = compareValue5;
			this.CompareValue6 = compareValue6;
			this.Label = "";
			this.Tooltip = "";
			this.Min = 0f;
			this.Max = 0f;
		}

		// Token: 0x0400945C RID: 37980
		public string PropertyToCheck;

		// Token: 0x0400945D RID: 37981
		public object CompareValue;

		// Token: 0x0400945E RID: 37982
		public object CompareValue2;

		// Token: 0x0400945F RID: 37983
		public object CompareValue3;

		// Token: 0x04009460 RID: 37984
		public object CompareValue4;

		// Token: 0x04009461 RID: 37985
		public object CompareValue5;

		// Token: 0x04009462 RID: 37986
		public object CompareValue6;

		// Token: 0x04009463 RID: 37987
		public string Label;

		// Token: 0x04009464 RID: 37988
		public string Tooltip;

		// Token: 0x04009465 RID: 37989
		public float Min;

		// Token: 0x04009466 RID: 37990
		public float Max;
	}
}
