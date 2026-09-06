using System;
using System.Linq;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200130F RID: 4879
	public class ContinuousPropertyModeSO : ScriptableObject
	{
		// Token: 0x17000BF8 RID: 3064
		// (get) Token: 0x06007A71 RID: 31345 RVA: 0x0027F332 File Offset: 0x0027D532
		private string GetTestDescription
		{
			get
			{
				if (this.castData.Length == 0)
				{
					return "";
				}
				return "Sample Description: " + this.GetDescriptionForCast(this.castData[0].target);
			}
		}

		// Token: 0x06007A72 RID: 31346 RVA: 0x0027F364 File Offset: 0x0027D564
		public bool IsCastValid(ContinuousProperty.Cast cast)
		{
			for (int i = 0; i < this.castData.Length; i++)
			{
				if (ContinuousProperty.CastMatches(this.castData[i].target, cast))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007A73 RID: 31347 RVA: 0x0027F3A0 File Offset: 0x0027D5A0
		public ContinuousProperty.Cast GetClosestCast(ContinuousProperty.Cast cast)
		{
			for (int i = 0; i < this.castData.Length; i++)
			{
				if (ContinuousProperty.CastMatches(this.castData[i].target, cast))
				{
					return this.castData[i].target;
				}
			}
			return ContinuousProperty.Cast.Null;
		}

		// Token: 0x06007A74 RID: 31348 RVA: 0x0027F3EC File Offset: 0x0027D5EC
		public ContinuousProperty.DataFlags GetFlagsForCast(ContinuousProperty.Cast cast)
		{
			for (int i = 0; i < this.castData.Length; i++)
			{
				if (this.castData[i].target == cast)
				{
					return this.castData[i].additionalFlags | this.flags;
				}
			}
			return this.flags;
		}

		// Token: 0x06007A75 RID: 31349 RVA: 0x0027F440 File Offset: 0x0027D640
		public ContinuousProperty.DataFlags GetFlagsForClosestCast(ContinuousProperty.Cast cast)
		{
			for (int i = 0; i < this.castData.Length; i++)
			{
				if (ContinuousProperty.CastMatches(this.castData[i].target, cast))
				{
					return this.castData[i].additionalFlags | this.flags;
				}
			}
			return this.flags;
		}

		// Token: 0x06007A76 RID: 31350 RVA: 0x0027F498 File Offset: 0x0027D698
		public string GetDescriptionForCast(ContinuousProperty.Cast cast)
		{
			for (int i = 0; i < this.castData.Length; i++)
			{
				if (ContinuousProperty.CastMatches(this.castData[i].target, cast) || this.castData.Length == 1)
				{
					if (!this.replaceDescription.IsNullOrEmpty())
					{
						return this.replaceDescription;
					}
					switch (this.descriptionStyle)
					{
					case ContinuousPropertyModeSO.DescriptionStyle.Continuous:
						return string.Concat(new string[]
						{
							"sets the ",
							this.castData[i].whatItSets,
							" on the ",
							this.castData[i].target.ToString(),
							" using the height of the curve at the provided time.",
							(" " + this.afterSentence).TrimEnd()
						});
					case ContinuousPropertyModeSO.DescriptionStyle.SingleThreshold:
						return this.castData[i].whatItSets + " the " + this.type.ToString() + " when entering the 'true' part of the range.";
					case ContinuousPropertyModeSO.DescriptionStyle.DualThreshold:
					{
						string[] array = this.castData[i].whatItSets.Split('|', StringSplitOptions.None);
						if (array.Length != 2)
						{
							return string.Format("Error! '{0}'s '{1}.{2}' does not have two string separated by '|'.", base.name, this.castData[i].target, "whatItSets");
						}
						return string.Concat(new string[]
						{
							array[0],
							" the ",
							this.castData[i].target.ToString(),
							" when entering the 'true' part of the range, ",
							array[1],
							" the ",
							this.castData[i].target.ToString(),
							" when entering the 'false' part of the range."
						});
					}
					}
				}
			}
			return "Invalid target\n\n" + this.ListValidCasts();
		}

		// Token: 0x06007A77 RID: 31351 RVA: 0x0027F68A File Offset: 0x0027D88A
		public string ListValidCasts()
		{
			return "Valid targets: " + string.Join<ContinuousProperty.Cast>(", ", this.castData.Select((ContinuousPropertyModeSO.CastData x) => x.target));
		}

		// Token: 0x04008C01 RID: 35841
		public ContinuousProperty.Type type;

		// Token: 0x04008C02 RID: 35842
		public ContinuousProperty.DataFlags flags;

		// Token: 0x04008C03 RID: 35843
		public ContinuousPropertyModeSO.CastData[] castData;

		// Token: 0x04008C04 RID: 35844
		[Space]
		public ContinuousPropertyModeSO.DescriptionStyle descriptionStyle;

		// Token: 0x04008C05 RID: 35845
		[TextArea]
		public string afterSentence;

		// Token: 0x04008C06 RID: 35846
		[TextArea]
		public string replaceDescription;

		// Token: 0x02001310 RID: 4880
		[Serializable]
		public struct CastData
		{
			// Token: 0x04008C07 RID: 35847
			public ContinuousProperty.Cast target;

			// Token: 0x04008C08 RID: 35848
			public ContinuousProperty.DataFlags additionalFlags;

			// Token: 0x04008C09 RID: 35849
			public string whatItSets;
		}

		// Token: 0x02001311 RID: 4881
		public enum DescriptionStyle
		{
			// Token: 0x04008C0B RID: 35851
			Continuous,
			// Token: 0x04008C0C RID: 35852
			SingleThreshold,
			// Token: 0x04008C0D RID: 35853
			DualThreshold
		}
	}
}
