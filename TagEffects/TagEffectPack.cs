using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x0200117A RID: 4474
	[CreateAssetMenu(fileName = "New Tag Effect Pack", menuName = "Tag Effect Pack")]
	public class TagEffectPack : ScriptableObject
	{
		// Token: 0x04008028 RID: 32808
		public GameObject thirdPerson;

		// Token: 0x04008029 RID: 32809
		public bool thirdPersonParentEffect = true;

		// Token: 0x0400802A RID: 32810
		public GameObject firstPerson;

		// Token: 0x0400802B RID: 32811
		public bool firstPersonParentEffect = true;

		// Token: 0x0400802C RID: 32812
		public GameObject highFive;

		// Token: 0x0400802D RID: 32813
		public bool highFiveParentEffect;

		// Token: 0x0400802E RID: 32814
		public GameObject fistBump;

		// Token: 0x0400802F RID: 32815
		public bool fistBumpParentEffect;

		// Token: 0x04008030 RID: 32816
		public bool shouldFaceTagger;
	}
}
