using System;
using System.Collections.Generic;
using Photon.Pun;

// Token: 0x02000050 RID: 80
public class CrittersActorSpawnerPoint : CrittersActor
{
	// Token: 0x14000004 RID: 4
	// (add) Token: 0x06000193 RID: 403 RVA: 0x0000A048 File Offset: 0x00008248
	// (remove) Token: 0x06000194 RID: 404 RVA: 0x0000A080 File Offset: 0x00008280
	public event Action<CrittersActor> OnSpawnChanged;

	// Token: 0x06000195 RID: 405 RVA: 0x0000A0B5 File Offset: 0x000082B5
	public override void Initialize()
	{
		base.Initialize();
		base.UpdateImpulses(false, false);
	}

	// Token: 0x06000196 RID: 406 RVA: 0x0000A0C5 File Offset: 0x000082C5
	public override void OnDisable()
	{
		base.OnDisable();
		this.spawnedActorID = -1;
		this.spawnedActor = null;
	}

	// Token: 0x06000197 RID: 407 RVA: 0x0000A0DC File Offset: 0x000082DC
	public void SetSpawnedActor(CrittersActor actor)
	{
		if (this.spawnedActor == actor)
		{
			return;
		}
		this.spawnedActor = actor;
		if (this.spawnedActor != null)
		{
			this.spawnedActorID = this.spawnedActor.actorId;
		}
		else
		{
			this.spawnedActorID = -1;
		}
		Action<CrittersActor> onSpawnChanged = this.OnSpawnChanged;
		if (onSpawnChanged != null)
		{
			onSpawnChanged(this.spawnedActor);
		}
		this.updatedSinceLastFrame = true;
	}

	// Token: 0x06000198 RID: 408 RVA: 0x0000A148 File Offset: 0x00008348
	private void UpdateSpawnedActor(int newSpawnedActorID)
	{
		if (this.spawnedActorID == newSpawnedActorID)
		{
			return;
		}
		if (newSpawnedActorID == -1)
		{
			this.spawnedActorID = newSpawnedActorID;
			this.spawnedActor = null;
		}
		else
		{
			CrittersActor crittersActor;
			if (!CrittersManager.instance.actorById.TryGetValue(newSpawnedActorID, out crittersActor))
			{
				return;
			}
			this.spawnedActorID = newSpawnedActorID;
			this.spawnedActor = crittersActor;
		}
		Action<CrittersActor> onSpawnChanged = this.OnSpawnChanged;
		if (onSpawnChanged == null)
		{
			return;
		}
		onSpawnChanged(this.spawnedActor);
	}

	// Token: 0x06000199 RID: 409 RVA: 0x0000A1AE File Offset: 0x000083AE
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(this.spawnedActorID);
	}

	// Token: 0x0600019A RID: 410 RVA: 0x0000A1C8 File Offset: 0x000083C8
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		if (!base.UpdateSpecificActor(stream))
		{
			return false;
		}
		int num;
		if (!CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num))
		{
			return false;
		}
		if (num < -1 || num >= CrittersManager.instance.universalActorId)
		{
			return false;
		}
		this.UpdateSpawnedActor(num);
		return true;
	}

	// Token: 0x0600019B RID: 411 RVA: 0x0000A20E File Offset: 0x0000840E
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(this.spawnedActorID);
		return this.TotalActorDataLength();
	}

	// Token: 0x0600019C RID: 412 RVA: 0x0000A230 File Offset: 0x00008430
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 1;
	}

	// Token: 0x0600019D RID: 413 RVA: 0x0000A23C File Offset: 0x0000843C
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		int num;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex], out num))
		{
			return this.TotalActorDataLength();
		}
		if (num >= -1 && num < CrittersManager.instance.universalActorId)
		{
			return this.TotalActorDataLength();
		}
		this.UpdateSpawnedActor(num);
		return this.TotalActorDataLength();
	}

	// Token: 0x040001B3 RID: 435
	private CrittersActor spawnedActor;

	// Token: 0x040001B4 RID: 436
	private int spawnedActorID = -1;
}
