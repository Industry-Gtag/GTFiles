using System;

namespace GorillaTagScripts.GhostReactor
{
	// Token: 0x02001018 RID: 4120
	public static class GREnemyTypeExtensions
	{
		// Token: 0x06006698 RID: 26264 RVA: 0x0020F31C File Offset: 0x0020D51C
		public static GREnemyType GetEnemyType(this GameEntity entity)
		{
			if (entity == null)
			{
				return GREnemyType.None;
			}
			GREnemy component = entity.GetComponent<GREnemy>();
			if (component == null)
			{
				return GREnemyType.None;
			}
			if (component.enemyType == GREnemyType.MoonBoss_Phase1 || component.enemyType == GREnemyType.MoonBoss_Phase2)
			{
				return GREnemyType.MoonBoss;
			}
			return component.enemyType;
		}

		// Token: 0x06006699 RID: 26265 RVA: 0x0020F364 File Offset: 0x0020D564
		public static string Pluralize(this GREnemyType t)
		{
			string text;
			if (t == GREnemyType.MoonBoss)
			{
				text = "Meteor Monsters";
			}
			else
			{
				text = string.Format("{0}s", t);
			}
			return text;
		}
	}
}
