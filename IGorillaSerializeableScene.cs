using System;

// Token: 0x0200084D RID: 2125
internal interface IGorillaSerializeableScene : IGorillaSerializeable
{
	// Token: 0x060036B9 RID: 14009
	void OnSceneLinking(GorillaSerializerScene serializer);

	// Token: 0x060036BA RID: 14010
	void OnNetworkObjectDisable();

	// Token: 0x060036BB RID: 14011
	void OnNetworkObjectEnable();
}
