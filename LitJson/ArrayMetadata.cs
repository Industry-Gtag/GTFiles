using System;

namespace LitJson
{
	// Token: 0x02000EEE RID: 3822
	internal struct ArrayMetadata
	{
		// Token: 0x1700090D RID: 2317
		// (get) Token: 0x06005D6C RID: 23916 RVA: 0x001DDA9C File Offset: 0x001DBC9C
		// (set) Token: 0x06005D6D RID: 23917 RVA: 0x001DDABD File Offset: 0x001DBCBD
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

		// Token: 0x1700090E RID: 2318
		// (get) Token: 0x06005D6E RID: 23918 RVA: 0x001DDAC6 File Offset: 0x001DBCC6
		// (set) Token: 0x06005D6F RID: 23919 RVA: 0x001DDACE File Offset: 0x001DBCCE
		public bool IsArray
		{
			get
			{
				return this.is_array;
			}
			set
			{
				this.is_array = value;
			}
		}

		// Token: 0x1700090F RID: 2319
		// (get) Token: 0x06005D70 RID: 23920 RVA: 0x001DDAD7 File Offset: 0x001DBCD7
		// (set) Token: 0x06005D71 RID: 23921 RVA: 0x001DDADF File Offset: 0x001DBCDF
		public bool IsList
		{
			get
			{
				return this.is_list;
			}
			set
			{
				this.is_list = value;
			}
		}

		// Token: 0x04006CC6 RID: 27846
		private Type element_type;

		// Token: 0x04006CC7 RID: 27847
		private bool is_array;

		// Token: 0x04006CC8 RID: 27848
		private bool is_list;
	}
}
