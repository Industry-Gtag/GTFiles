using System;

// Token: 0x020006BE RID: 1726
public interface IGameEntityComponent
{
	// Token: 0x06002B18 RID: 11032
	void OnEntityInit();

	// Token: 0x06002B19 RID: 11033
	void OnEntityDestroy();

	// Token: 0x06002B1A RID: 11034
	void OnEntityStateChange(long prevState, long newState);
}
