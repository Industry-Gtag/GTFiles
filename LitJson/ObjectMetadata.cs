using System;
using System.Collections.Generic;

namespace LitJson
{
	// Token: 0x02000EEF RID: 3823
	internal struct ObjectMetadata
	{
		// Token: 0x17000910 RID: 2320
		// (get) Token: 0x06005D72 RID: 23922 RVA: 0x001DDAE8 File Offset: 0x001DBCE8
		// (set) Token: 0x06005D73 RID: 23923 RVA: 0x001DDB09 File Offset: 0x001DBD09
		public Type ElementType
		{
			get
			{
				if (this.element_type == null)
				{
					return typeof(JsonData);
				}
				return this.element_type;
			}
			set
			{
				this.element_type = value;
			}
		}

		// Token: 0x17000911 RID: 2321
		// (get) Token: 0x06005D74 RID: 23924 RVA: 0x001DDB12 File Offset: 0x001DBD12
		// (set) Token: 0x06005D75 RID: 23925 RVA: 0x001DDB1A File Offset: 0x001DBD1A
		public bool IsDictionary
		{
			get
			{
				return this.is_dictionary;
			}
			set
			{
				this.is_dictionary = value;
			}
		}

		// Token: 0x17000912 RID: 2322
		// (get) Token: 0x06005D76 RID: 23926 RVA: 0x001DDB23 File Offset: 0x001DBD23
		// (set) Token: 0x06005D77 RID: 23927 RVA: 0x001DDB2B File Offset: 0x001DBD2B
		public IDictionary<string, PropertyMetadata> Properties
		{
			get
			{
				return this.properties;
			}
			set
			{
				this.properties = value;
			}
		}

		// Token: 0x04006CC9 RID: 27849
		private Type element_type;

		// Token: 0x04006CCA RID: 27850
		private bool is_dictionary;

		// Token: 0x04006CCB RID: 27851
		private IDictionary<string, PropertyMetadata> properties;
	}
}
