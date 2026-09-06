using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GorillaTag
{
	// Token: 0x020011E6 RID: 4582
	[Serializable]
	public class GTAssetRef<TObject> : AssetReferenceT<TObject> where TObject : Object
	{
		// Token: 0x0600747E RID: 29822 RVA: 0x0025E164 File Offset: 0x0025C364
		public GTAssetRef(string guid)
			: base(guid)
		{
		}
	}
}
