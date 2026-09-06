using System;
using System.Collections.Generic;
using System.IO;
using GorillaTagScripts.GhostReactor;

// Token: 0x020007F5 RID: 2037
public class GRShiftStat
{
	// Token: 0x170004BA RID: 1210
	// (get) Token: 0x06003404 RID: 13316 RVA: 0x0011DFF8 File Offset: 0x0011C1F8
	public IReadOnlyDictionary<GREnemyType, int> EnemyKills
	{
		get
		{
			return this.enemyKills;
		}
	}

	// Token: 0x06003405 RID: 13317 RVA: 0x0011E000 File Offset: 0x0011C200
	public void Serialize(BinaryWriter writer)
	{
		writer.Write(this.GetShiftStat(GRShiftStatType.EnemyDeaths));
		writer.Write(this.GetShiftStat(GRShiftStatType.PlayerDeaths));
		writer.Write(this.GetShiftStat(GRShiftStatType.CoresCollected));
		writer.Write(this.GetShiftStat(GRShiftStatType.SentientCoresCollected));
		writer.Write(this.enemyKills.Count);
		foreach (KeyValuePair<GREnemyType, int> keyValuePair in this.enemyKills)
		{
			writer.Write((int)keyValuePair.Key);
			writer.Write(keyValuePair.Value);
		}
	}

	// Token: 0x06003406 RID: 13318 RVA: 0x0011E0AC File Offset: 0x0011C2AC
	public void Deserialize(BinaryReader reader)
	{
		this.shiftStats[GRShiftStatType.EnemyDeaths] = reader.ReadInt32();
		this.shiftStats[GRShiftStatType.PlayerDeaths] = reader.ReadInt32();
		this.shiftStats[GRShiftStatType.CoresCollected] = reader.ReadInt32();
		this.shiftStats[GRShiftStatType.SentientCoresCollected] = reader.ReadInt32();
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			GREnemyType grenemyType = (GREnemyType)reader.ReadInt32();
			this.enemyKills[grenemyType] = reader.ReadInt32();
		}
	}

	// Token: 0x06003407 RID: 13319 RVA: 0x0011E12D File Offset: 0x0011C32D
	public void SetShiftStat(GRShiftStatType stat, int newValue)
	{
		this.shiftStats[stat] = newValue;
		GhostReactor.instance.shiftManager.RefreshDepthDisplay();
	}

	// Token: 0x06003408 RID: 13320 RVA: 0x0011E14C File Offset: 0x0011C34C
	public void IncrementShiftStat(GRShiftStatType stat)
	{
		if (this.shiftStats.ContainsKey(stat))
		{
			Dictionary<GRShiftStatType, int> dictionary = this.shiftStats;
			int num = dictionary[stat];
			dictionary[stat] = num + 1;
			return;
		}
		this.shiftStats[stat] = 1;
		GhostReactor.instance.shiftManager.RefreshDepthDisplay();
	}

	// Token: 0x06003409 RID: 13321 RVA: 0x0011E1A0 File Offset: 0x0011C3A0
	public void IncrementEnemyKills(GREnemyType type)
	{
		if (type == GREnemyType.None)
		{
			return;
		}
		if (!this.enemyKills.TryAdd(type, 1))
		{
			Dictionary<GREnemyType, int> dictionary = this.enemyKills;
			int num = dictionary[type];
			dictionary[type] = num + 1;
		}
		GhostReactor.instance.shiftManager.RefreshDepthDisplay();
	}

	// Token: 0x0600340A RID: 13322 RVA: 0x0011E1E8 File Offset: 0x0011C3E8
	public void ResetShiftStats()
	{
		this.shiftStats[GRShiftStatType.EnemyDeaths] = 0;
		this.shiftStats[GRShiftStatType.PlayerDeaths] = 0;
		this.shiftStats[GRShiftStatType.CoresCollected] = 0;
		this.shiftStats[GRShiftStatType.SentientCoresCollected] = 0;
		this.enemyKills.Clear();
		GhostReactor.instance.shiftManager.RefreshDepthDisplay();
	}

	// Token: 0x0600340B RID: 13323 RVA: 0x0011E243 File Offset: 0x0011C443
	public int GetShiftStat(GRShiftStatType stat)
	{
		if (this.shiftStats.ContainsKey(stat))
		{
			return this.shiftStats[stat];
		}
		return 0;
	}

	// Token: 0x040043D1 RID: 17361
	public Dictionary<GRShiftStatType, int> shiftStats = new Dictionary<GRShiftStatType, int>();

	// Token: 0x040043D2 RID: 17362
	private Dictionary<GREnemyType, int> enemyKills = new Dictionary<GREnemyType, int>();
}
