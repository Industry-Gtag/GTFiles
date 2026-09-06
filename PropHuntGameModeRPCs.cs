using System;

// Token: 0x02000279 RID: 633
internal class PropHuntGameModeRPCs : RPCNetworkBase
{
	// Token: 0x0600111F RID: 4383 RVA: 0x0005B9C1 File Offset: 0x00059BC1
	public override void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler)
	{
		this.propHuntManager = (GorillaPropHuntGameManager)target;
		this.serializer = (GameModeSerializer)netHandler;
	}

	// Token: 0x04001458 RID: 5208
	private GameModeSerializer serializer;

	// Token: 0x04001459 RID: 5209
	private GorillaPropHuntGameManager propHuntManager;
}
