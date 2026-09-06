using System;
using System.Collections.Generic;

// Token: 0x020000FE RID: 254
public interface IPrefabRequirements
{
	// Token: 0x17000066 RID: 102
	// (get) Token: 0x060005F4 RID: 1524
	IEnumerable<GameEntity> RequiredPrefabs { get; }
}
