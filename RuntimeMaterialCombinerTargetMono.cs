using System;
using UnityEngine;

// Token: 0x02000389 RID: 905
public class RuntimeMaterialCombinerTargetMono : MonoBehaviour
{
	// Token: 0x06001606 RID: 5638 RVA: 0x000763F3 File Offset: 0x000745F3
	protected void Awake()
	{
		throw new NotImplementedException("// TODO: get the material combiner manager to fingerprint and combine these materials.");
	}

	// Token: 0x04001B8E RID: 7054
	[HideInInspector]
	public GTSerializableDict<string, string>[] m_matSlot_to_texProp_to_texGuid;
}
