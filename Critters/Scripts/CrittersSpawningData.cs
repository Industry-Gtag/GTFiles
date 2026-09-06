using System;
using System.Collections.Generic;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x020013EE RID: 5102
	public class CrittersSpawningData : MonoBehaviour
	{
		// Token: 0x060080BA RID: 32954 RVA: 0x0029DA8C File Offset: 0x0029BC8C
		public void InitializeSpawnCollection()
		{
			for (int i = 0; i < this.SpawnParametersList.Count; i++)
			{
				for (int j = 0; j < this.SpawnParametersList[i].ChancesToSpawn; j++)
				{
					this.templateCollection.Add(i);
				}
			}
		}

		// Token: 0x060080BB RID: 32955 RVA: 0x0029DAD8 File Offset: 0x0029BCD8
		public int GetRandomTemplate()
		{
			int num = Random.Range(0, this.templateCollection.Count - 1);
			return this.templateCollection[num];
		}

		// Token: 0x040091C0 RID: 37312
		public List<CrittersSpawningData.CreatureSpawnParameters> SpawnParametersList;

		// Token: 0x040091C1 RID: 37313
		private List<int> templateCollection = new List<int>();

		// Token: 0x020013EF RID: 5103
		[Serializable]
		public class CreatureSpawnParameters
		{
			// Token: 0x040091C2 RID: 37314
			public CritterTemplate Template;

			// Token: 0x040091C3 RID: 37315
			public int ChancesToSpawn;

			// Token: 0x040091C4 RID: 37316
			[HideInInspector]
			[NonSerialized]
			public int StartingIndex;
		}
	}
}
