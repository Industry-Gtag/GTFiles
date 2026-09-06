using System;
using System.Collections.Generic;

namespace GorillaNetworking
{
	// Token: 0x02001103 RID: 4355
	[Serializable]
	internal class FeatureFlagData
	{
		// Token: 0x04007D65 RID: 32101
		public string name;

		// Token: 0x04007D66 RID: 32102
		public int value;

		// Token: 0x04007D67 RID: 32103
		public string valueType;

		// Token: 0x04007D68 RID: 32104
		public List<string> alwaysOnForUsers;
	}
}
