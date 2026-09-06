using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;

// Token: 0x0200064A RID: 1610
[CreateAssetMenu(fileName = "BuilderPieceSet01", menuName = "Gorilla Tag/Builder/PieceSet", order = 0)]
public class BuilderPieceSet : ScriptableObject
{
	// Token: 0x1700040C RID: 1036
	// (get) Token: 0x0600285B RID: 10331 RVA: 0x000D8945 File Offset: 0x000D6B45
	public string SetName
	{
		get
		{
			return this.setName;
		}
	}

	// Token: 0x0600285C RID: 10332 RVA: 0x000D894D File Offset: 0x000D6B4D
	public int GetIntIdentifier()
	{
		return this.playfabID.GetStaticHash();
	}

	// Token: 0x0600285D RID: 10333 RVA: 0x000D895C File Offset: 0x000D6B5C
	public DateTime GetScheduleDateTime()
	{
		if (this.isScheduled)
		{
			try
			{
				return DateTime.Parse(this.scheduledDate, CultureInfo.InvariantCulture);
			}
			catch
			{
				return DateTime.MinValue;
			}
		}
		return DateTime.MinValue;
	}

	// Token: 0x04003436 RID: 13366
	[Tooltip("Display Name - Fallback for Localization")]
	public string setName;

	// Token: 0x04003437 RID: 13367
	public GameObject displayModel;

	// Token: 0x04003438 RID: 13368
	[Tooltip("If this should error if no localization is found")]
	public bool isLocalized;

	// Token: 0x04003439 RID: 13369
	[Tooltip("Localized Display Name")]
	public LocalizedString setLocName;

	// Token: 0x0400343A RID: 13370
	[FormerlySerializedAs("uniqueId")]
	[Tooltip("If purchaseable, this should be a valid playfabID starting with LD\nIf a starter set, this just needs to be a unique string from the other set IDs")]
	public string playfabID;

	// Token: 0x0400343B RID: 13371
	[Tooltip("(Optional) Default Material ID applied to all prefabs with BuilderMaterialOptions")]
	public string materialId;

	// Token: 0x0400343C RID: 13372
	[Tooltip("(Optional) If this set is not available on launch day use scheduling")]
	public bool isScheduled;

	// Token: 0x0400343D RID: 13373
	public string scheduledDate = "1/1/0001 00:00:00";

	// Token: 0x0400343E RID: 13374
	[Tooltip("A group of pieces on the same shelf")]
	public List<BuilderPieceSet.BuilderPieceSubset> subsets;

	// Token: 0x0200064B RID: 1611
	public enum BuilderPieceCategory
	{
		// Token: 0x04003440 RID: 13376
		FLAT,
		// Token: 0x04003441 RID: 13377
		TALL,
		// Token: 0x04003442 RID: 13378
		HALF_HEIGHT,
		// Token: 0x04003443 RID: 13379
		BEAM,
		// Token: 0x04003444 RID: 13380
		SLOPE,
		// Token: 0x04003445 RID: 13381
		OVERSIZED,
		// Token: 0x04003446 RID: 13382
		SPECIAL_DISPLAY,
		// Token: 0x04003447 RID: 13383
		FUNCTIONAL = 18,
		// Token: 0x04003448 RID: 13384
		DECORATIVE,
		// Token: 0x04003449 RID: 13385
		MISC
	}

	// Token: 0x0200064C RID: 1612
	[Serializable]
	public class BuilderPieceSubset
	{
		// Token: 0x0600285F RID: 10335 RVA: 0x000D89B7 File Offset: 0x000D6BB7
		public string GetShelfButtonName()
		{
			return this.shelfButtonName;
		}

		// Token: 0x0400344A RID: 13386
		[Tooltip("(Optional) Text to put on the shelf button if not the set name")]
		public string shelfButtonName;

		// Token: 0x0400344B RID: 13387
		public LocalizedString localizedShelfButtonName;

		// Token: 0x0400344C RID: 13388
		public BuilderPieceSet.BuilderPieceCategory pieceCategory;

		// Token: 0x0400344D RID: 13389
		public List<BuilderPieceSet.PieceInfo> pieceInfos;
	}

	// Token: 0x0200064D RID: 1613
	[Serializable]
	public struct PieceInfo
	{
		// Token: 0x0400344E RID: 13390
		public BuilderPiece piecePrefab;

		// Token: 0x0400344F RID: 13391
		[Tooltip("(Optional) should this piece use a materialID other than the set's materialID")]
		public bool overrideSetMaterial;

		// Token: 0x04003450 RID: 13392
		[Tooltip("material type string should match an entry in this prefab's BuilderMaterialOptions\nIf multiple are in the list the piece will cycle through materials when spawned\nTo have each variant on the shelf create a new pieceInfo for each color")]
		public string[] pieceMaterialTypes;
	}

	// Token: 0x0200064E RID: 1614
	public class BuilderDisplayGroup
	{
		// Token: 0x06002861 RID: 10337 RVA: 0x000D89BF File Offset: 0x000D6BBF
		public BuilderDisplayGroup()
		{
			this.displayName = string.Empty;
			this.pieceSubsets = new List<BuilderPieceSet.BuilderPieceSubset>();
			this.defaultMaterial = string.Empty;
			this.setID = -1;
			this.uniqueGroupID = string.Empty;
		}

		// Token: 0x06002862 RID: 10338 RVA: 0x000D89FA File Offset: 0x000D6BFA
		public BuilderDisplayGroup(string groupName, string material, int inSetID, string groupID)
		{
			this.displayName = groupName;
			this.pieceSubsets = new List<BuilderPieceSet.BuilderPieceSubset>();
			this.defaultMaterial = material;
			this.setID = inSetID;
			this.uniqueGroupID = groupID;
		}

		// Token: 0x06002863 RID: 10339 RVA: 0x000D8A2A File Offset: 0x000D6C2A
		public int GetDisplayGroupIdentifier()
		{
			return this.uniqueGroupID.GetStaticHash();
		}

		// Token: 0x04003451 RID: 13393
		public string displayName;

		// Token: 0x04003452 RID: 13394
		public List<BuilderPieceSet.BuilderPieceSubset> pieceSubsets;

		// Token: 0x04003453 RID: 13395
		public string defaultMaterial;

		// Token: 0x04003454 RID: 13396
		public int setID;

		// Token: 0x04003455 RID: 13397
		public string uniqueGroupID;
	}
}
