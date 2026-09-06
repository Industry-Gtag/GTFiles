using System;
using UnityEngine;

// Token: 0x02000045 RID: 69
[Serializable]
public class CritterConfiguration
{
	// Token: 0x0600011C RID: 284 RVA: 0x00006CC4 File Offset: 0x00004EC4
	public CritterConfiguration()
	{
		this.animalType = CritterConfiguration.AnimalType.UNKNOWN;
	}

	// Token: 0x0600011D RID: 285 RVA: 0x00006CF0 File Offset: 0x00004EF0
	public int GetIndex()
	{
		return CrittersManager.instance.creatureIndex.critterTypes.IndexOf(this);
	}

	// Token: 0x0600011E RID: 286 RVA: 0x00006D09 File Offset: 0x00004F09
	private bool RegionMatches(CrittersRegion region)
	{
		return !region || (region.Biome & this.biome) > (CrittersBiome)0;
	}

	// Token: 0x0600011F RID: 287 RVA: 0x00006D25 File Offset: 0x00004F25
	private bool SpawnCriteriaMatches()
	{
		return !this.spawnCriteria || this.spawnCriteria.CanSpawn();
	}

	// Token: 0x06000120 RID: 288 RVA: 0x00006D41 File Offset: 0x00004F41
	public bool CanSpawn()
	{
		return this.SpawnCriteriaMatches();
	}

	// Token: 0x06000121 RID: 289 RVA: 0x00006D49 File Offset: 0x00004F49
	public bool CanSpawn(CrittersRegion region)
	{
		return this.RegionMatches(region) && this.SpawnCriteriaMatches();
	}

	// Token: 0x06000122 RID: 290 RVA: 0x00006D5C File Offset: 0x00004F5C
	public bool DateConditionsMet(DateTime utcDate)
	{
		return !this.dateLimit || this.dateLimit.MatchesDate(utcDate);
	}

	// Token: 0x06000123 RID: 291 RVA: 0x00006D79 File Offset: 0x00004F79
	public bool ShouldDespawn()
	{
		return !this.SpawnCriteriaMatches();
	}

	// Token: 0x06000124 RID: 292 RVA: 0x00006D84 File Offset: 0x00004F84
	public void ApplyToCreature(CrittersPawn crittersPawn)
	{
		this.behaviour.ApplyToCritter(crittersPawn);
		if (CrittersManager.instance.LocalAuthority())
		{
			this.ApplyVisualsTo(crittersPawn, true);
			return;
		}
		this.ApplyVisualsTo(crittersPawn, false);
	}

	// Token: 0x06000125 RID: 293 RVA: 0x00006DB1 File Offset: 0x00004FB1
	private void ApplyVisualsTo(CrittersPawn critter, bool generateAppearance = true)
	{
		this.ApplyVisualsTo(critter.visuals, generateAppearance);
	}

	// Token: 0x06000126 RID: 294 RVA: 0x00006DC0 File Offset: 0x00004FC0
	public void ApplyVisualsTo(CritterVisuals visuals, bool generateAppearance = true)
	{
		visuals.critterType = this.GetIndex();
		visuals.ApplyMesh(CritterIndex.GetMesh(this.animalType));
		visuals.ApplyMaterial(this.critterMat);
		if (generateAppearance)
		{
			visuals.SetAppearance(this.GenerateAppearance());
		}
	}

	// Token: 0x06000127 RID: 295 RVA: 0x00006DFC File Offset: 0x00004FFC
	public CritterAppearance GenerateAppearance()
	{
		string text = "";
		if (Random.value <= this.behaviour.GetTemplateValue<float>("hatChance"))
		{
			GameObject[] templateValue = this.behaviour.GetTemplateValue<GameObject[]>("hats");
			if (!templateValue.IsNullOrEmpty<GameObject>())
			{
				text = templateValue[Random.Range(0, templateValue.Length)].name;
			}
		}
		float templateValue2 = this.behaviour.GetTemplateValue<float>("minSize");
		float templateValue3 = this.behaviour.GetTemplateValue<float>("maxSize");
		float num = Random.Range(templateValue2, templateValue3);
		return new CritterAppearance(text, num);
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00006E84 File Offset: 0x00005084
	public override string ToString()
	{
		return string.Format("{0} B:{1} C:{2}", this.critterName, this.behaviour, this.spawnCriteria);
	}

	// Token: 0x04000120 RID: 288
	[Tooltip("Basic internal description of critter.  Could be role, purpose, player experience, etc.")]
	public string internalDescription;

	// Token: 0x04000121 RID: 289
	public string critterName = "UNNAMED CRITTER";

	// Token: 0x04000122 RID: 290
	public CritterConfiguration.AnimalType animalType;

	// Token: 0x04000123 RID: 291
	public CritterTemplate behaviour;

	// Token: 0x04000124 RID: 292
	public CritterSpawnCriteria spawnCriteria;

	// Token: 0x04000125 RID: 293
	public RealWorldDateTimeWindow dateLimit;

	// Token: 0x04000126 RID: 294
	public CrittersBiome biome = CrittersBiome.Any;

	// Token: 0x04000127 RID: 295
	public float spawnWeight = 1f;

	// Token: 0x04000128 RID: 296
	public Material critterMat;

	// Token: 0x02000046 RID: 70
	public enum AnimalType
	{
		// Token: 0x0400012A RID: 298
		Raccoon,
		// Token: 0x0400012B RID: 299
		Cat,
		// Token: 0x0400012C RID: 300
		Bird,
		// Token: 0x0400012D RID: 301
		Goblin,
		// Token: 0x0400012E RID: 302
		Egg,
		// Token: 0x0400012F RID: 303
		UNKNOWN = -1
	}
}
