using System;
using System.IO;

// Token: 0x020006C0 RID: 1728
public interface IGameEntitySerialize
{
	// Token: 0x06002B1C RID: 11036
	void OnGameEntitySerialize(BinaryWriter writer);

	// Token: 0x06002B1D RID: 11037
	void OnGameEntityDeserialize(BinaryReader reader);
}
