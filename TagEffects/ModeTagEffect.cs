using System;
using System.Collections.Generic;
using GorillaGameModes;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02001181 RID: 4481
	[Serializable]
	public class ModeTagEffect
	{
		// Token: 0x17000ACC RID: 2764
		// (get) Token: 0x06007052 RID: 28754 RVA: 0x002437BE File Offset: 0x002419BE
		public HashSet<GameModeType> Modes
		{
			get
			{
				if (this.modesHash == null)
				{
					this.modesHash = new HashSet<GameModeType>(this.modes);
				}
				return this.modesHash;
			}
		}

		// Token: 0x0400804D RID: 32845
		[SerializeField]
		private GameModeType[] modes;

		// Token: 0x0400804E RID: 32846
		private HashSet<GameModeType> modesHash;

		// Token: 0x0400804F RID: 32847
		public TagEffectPack tagEffect;

		// Token: 0x04008050 RID: 32848
		public bool blockTagOverride;

		// Token: 0x04008051 RID: 32849
		public bool blockFistBumpOverride;

		// Token: 0x04008052 RID: 32850
		public bool blockHiveFiveOverride;
	}
}
