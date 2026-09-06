using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200067E RID: 1662
public abstract class CosmeticCritter : MonoBehaviour
{
	// Token: 0x17000430 RID: 1072
	// (get) Token: 0x06002992 RID: 10642 RVA: 0x000E10CF File Offset: 0x000DF2CF
	// (set) Token: 0x06002993 RID: 10643 RVA: 0x000E10D7 File Offset: 0x000DF2D7
	public int Seed { get; protected set; }

	// Token: 0x17000431 RID: 1073
	// (get) Token: 0x06002994 RID: 10644 RVA: 0x000E10E0 File Offset: 0x000DF2E0
	// (set) Token: 0x06002995 RID: 10645 RVA: 0x000E10E8 File Offset: 0x000DF2E8
	public CosmeticCritterSpawner Spawner { get; protected set; }

	// Token: 0x17000432 RID: 1074
	// (get) Token: 0x06002996 RID: 10646 RVA: 0x000E10F1 File Offset: 0x000DF2F1
	// (set) Token: 0x06002997 RID: 10647 RVA: 0x000E10F9 File Offset: 0x000DF2F9
	public Type CachedType { get; private set; }

	// Token: 0x06002998 RID: 10648 RVA: 0x000E1102 File Offset: 0x000DF302
	public int GetGlobalMaxCritters()
	{
		return this.globalMaxCritters;
	}

	// Token: 0x06002999 RID: 10649 RVA: 0x000E110A File Offset: 0x000DF30A
	public void SetSeedSpawnerTypeAndTime(int seed, CosmeticCritterSpawner spawner, Type type, double time)
	{
		this.Seed = seed;
		this.Spawner = spawner;
		this.CachedType = type;
		this.startTime = time;
	}

	// Token: 0x0600299A RID: 10650 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void OnSpawn()
	{
	}

	// Token: 0x0600299B RID: 10651 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void OnDespawn()
	{
	}

	// Token: 0x0600299C RID: 10652 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void SetRandomVariables()
	{
	}

	// Token: 0x0600299D RID: 10653
	public abstract void Tick();

	// Token: 0x0600299E RID: 10654 RVA: 0x000E1129 File Offset: 0x000DF329
	protected double GetAliveTime()
	{
		if (!PhotonNetwork.InRoom)
		{
			return Time.timeAsDouble - this.startTime;
		}
		return PhotonNetwork.Time - this.startTime;
	}

	// Token: 0x0600299F RID: 10655 RVA: 0x000E114B File Offset: 0x000DF34B
	public virtual bool Expired()
	{
		return this.GetAliveTime() > (double)this.lifetime || this.GetAliveTime() < 0.0;
	}

	// Token: 0x04003633 RID: 13875
	[Tooltip("After this many seconds the critter will forcibly despawn.")]
	[SerializeField]
	protected float lifetime;

	// Token: 0x04003634 RID: 13876
	[Tooltip("The maximum number of this kind of critter that can be in the room at any given time.")]
	[SerializeField]
	private int globalMaxCritters;

	// Token: 0x04003638 RID: 13880
	protected double startTime;
}
