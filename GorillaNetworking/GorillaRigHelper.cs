using System;

namespace GorillaNetworking
{
	// Token: 0x020010D3 RID: 4307
	[Serializable]
	internal struct GorillaRigHelper : IComparable
	{
		// Token: 0x06006BA5 RID: 27557 RVA: 0x0022B17A File Offset: 0x0022937A
		public int CompareTo(object obj)
		{
			return this.sqrDistance.CompareTo(((GorillaRigHelper)obj).sqrDistance);
		}

		// Token: 0x04007B17 RID: 31511
		public VRRig rig;

		// Token: 0x04007B18 RID: 31512
		public CosmeticsThrottler.RigDrawState state;

		// Token: 0x04007B19 RID: 31513
		public float sqrDistance;

		// Token: 0x04007B1A RID: 31514
		public float prevSqrDistance;
	}
}
