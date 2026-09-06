using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x0200127C RID: 4732
	[Serializable]
	public struct CosmeticAttachInfo
	{
		// Token: 0x17000BA0 RID: 2976
		// (get) Token: 0x0600776A RID: 30570 RVA: 0x0026B2F4 File Offset: 0x002694F4
		public static CosmeticAttachInfo Identity
		{
			get
			{
				return new CosmeticAttachInfo
				{
					selectSide = ECosmeticSelectSide.Both,
					parentBone = GTHardCodedBones.EBone.None,
					offset = XformOffset.Identity
				};
			}
		}

		// Token: 0x0600776B RID: 30571 RVA: 0x0026B330 File Offset: 0x00269530
		public CosmeticAttachInfo(ECosmeticSelectSide selectSide, GTHardCodedBones.EBone parentBone, XformOffset offset)
		{
			this.selectSide = selectSide;
			this.parentBone = parentBone;
			this.offset = offset;
		}

		// Token: 0x0400870B RID: 34571
		[Tooltip("(Not used for holdables) Determines if the cosmetic part be shown depending on the hand that is used to press the in-game wardrobe \"EQUIP\" button.\n- Both: Show no matter what hand is used.\n- Left: Only show if the left hand selected.\n- Right: Only show if the right hand selected.\n")]
		public StringEnum<ECosmeticSelectSide> selectSide;

		// Token: 0x0400870C RID: 34572
		public GTHardCodedBones.SturdyEBone parentBone;

		// Token: 0x0400870D RID: 34573
		public XformOffset offset;
	}
}
