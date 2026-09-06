using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F4B RID: 3915
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WhackAMoleLevelSetting", order = 1)]
	public class WhackAMoleLevelSO : ScriptableObject
	{
		// Token: 0x06006032 RID: 24626 RVA: 0x001E856C File Offset: 0x001E676C
		public int GetMinScore(bool isCoop)
		{
			if (!isCoop)
			{
				return this.minScore;
			}
			return this.minScore * 2;
		}

		// Token: 0x04006ED0 RID: 28368
		public int levelNumber;

		// Token: 0x04006ED1 RID: 28369
		public float levelDuration;

		// Token: 0x04006ED2 RID: 28370
		[Tooltip("For how long do the moles stay visible?")]
		public float showMoleDuration;

		// Token: 0x04006ED3 RID: 28371
		[Tooltip("How fast we pick a random new mole?")]
		public float pickNextMoleTime;

		// Token: 0x04006ED4 RID: 28372
		[Tooltip("Minimum score to get in order to be able to proceed to the next level")]
		[SerializeField]
		private int minScore;

		// Token: 0x04006ED5 RID: 28373
		[Tooltip("Chance of each mole being a hazard mole at the start, and end, of the level.")]
		public Vector2 hazardMoleChance = new Vector2(0f, 0.5f);

		// Token: 0x04006ED6 RID: 28374
		[Tooltip("Minimum number of moles selected as level progresses.")]
		public Vector2 minimumMoleCount = new Vector2(1f, 2f);

		// Token: 0x04006ED7 RID: 28375
		[Tooltip("Minimum number of moles selected as level progresses.")]
		public Vector2 maximumMoleCount = new Vector2(1.5f, 3f);
	}
}
