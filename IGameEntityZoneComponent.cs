using System;
using System.IO;
using UnityEngine;

// Token: 0x020006CC RID: 1740
public interface IGameEntityZoneComponent
{
	// Token: 0x06002B81 RID: 11137
	void OnZoneCreate();

	// Token: 0x06002B82 RID: 11138
	void OnZoneInit();

	// Token: 0x06002B83 RID: 11139
	void OnZoneClear(ZoneClearReason reason);

	// Token: 0x06002B84 RID: 11140
	void OnCreateGameEntity(GameEntity entity);

	// Token: 0x06002B85 RID: 11141
	void SerializeZoneData(BinaryWriter writer);

	// Token: 0x06002B86 RID: 11142
	void DeserializeZoneData(BinaryReader reader);

	// Token: 0x06002B87 RID: 11143
	void SerializeZoneEntityData(BinaryWriter writer, GameEntity entity);

	// Token: 0x06002B88 RID: 11144
	void DeserializeZoneEntityData(BinaryReader reader, GameEntity entity);

	// Token: 0x06002B89 RID: 11145
	void SerializeZonePlayerData(BinaryWriter writer, int actorNumber);

	// Token: 0x06002B8A RID: 11146
	void DeserializeZonePlayerData(BinaryReader reader, int actorNumber);

	// Token: 0x06002B8B RID: 11147
	bool IsZoneReady();

	// Token: 0x06002B8C RID: 11148
	bool ShouldClearZone();

	// Token: 0x06002B8D RID: 11149
	long ProcessMigratedGameEntityCreateData(GameEntity entity, long createData);

	// Token: 0x06002B8E RID: 11150
	bool ValidateMigratedGameEntity(int netId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int actorNr);

	// Token: 0x06002B8F RID: 11151
	bool ValidateCreateMultipleItems(int zoneId, byte[] compressedStateData, int EntityCount);

	// Token: 0x06002B90 RID: 11152
	bool ValidateCreateItem(int nedId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int createdByEntityNetId);

	// Token: 0x06002B91 RID: 11153
	bool ValidateCreateItemBatchSize(int size);
}
