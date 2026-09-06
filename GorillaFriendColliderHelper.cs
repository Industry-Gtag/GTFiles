using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000CCA RID: 3274
public class GorillaFriendColliderHelper : MonoBehaviour
{
	// Token: 0x06005132 RID: 20786 RVA: 0x001AEC50 File Offset: 0x001ACE50
	public void Awake()
	{
		GorillaFriendColliderHelper.Instance = this;
	}

	// Token: 0x06005133 RID: 20787 RVA: 0x001AEC58 File Offset: 0x001ACE58
	public GorillaFriendCollider FindFriendCollider(string search)
	{
		for (int i = 0; i < this.MappedFriendColliders.Length; i++)
		{
			if (this.MappedFriendColliders[i].ColliderName.Equals(search, StringComparison.OrdinalIgnoreCase))
			{
				return this.MappedFriendColliders[i].Collider;
			}
		}
		return null;
	}

	// Token: 0x06005134 RID: 20788 RVA: 0x001AECA8 File Offset: 0x001ACEA8
	public GorillaNetworkJoinTrigger FindJoinCollider(string search)
	{
		for (int i = 0; i < this.MappedFriendColliders.Length; i++)
		{
			if (this.MappedFriendColliders[i].ColliderName.Equals(search, StringComparison.OrdinalIgnoreCase))
			{
				return this.MappedFriendColliders[i].JoinTrigger;
			}
		}
		return null;
	}

	// Token: 0x04006334 RID: 25396
	public static GorillaFriendColliderHelper Instance;

	// Token: 0x04006335 RID: 25397
	public GorillaFriendColliderHelper.FriendColliderPair[] MappedFriendColliders;

	// Token: 0x02000CCB RID: 3275
	[Serializable]
	public struct FriendColliderPair
	{
		// Token: 0x04006336 RID: 25398
		public string ColliderName;

		// Token: 0x04006337 RID: 25399
		public GorillaFriendCollider Collider;

		// Token: 0x04006338 RID: 25400
		public GorillaNetworkJoinTrigger JoinTrigger;
	}
}
