using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020011E5 RID: 4581
	[Serializable]
	public struct BoneOffset
	{
		// Token: 0x17000B49 RID: 2889
		// (get) Token: 0x06007474 RID: 29812 RVA: 0x0025E068 File Offset: 0x0025C268
		public Vector3 pos
		{
			get
			{
				return this.offset.pos;
			}
		}

		// Token: 0x17000B4A RID: 2890
		// (get) Token: 0x06007475 RID: 29813 RVA: 0x0025E075 File Offset: 0x0025C275
		public Quaternion rot
		{
			get
			{
				return this.offset.rot;
			}
		}

		// Token: 0x17000B4B RID: 2891
		// (get) Token: 0x06007476 RID: 29814 RVA: 0x0025E082 File Offset: 0x0025C282
		public Vector3 scale
		{
			get
			{
				return this.offset.scale;
			}
		}

		// Token: 0x06007477 RID: 29815 RVA: 0x0025E08F File Offset: 0x0025C28F
		public BoneOffset(GTHardCodedBones.EBone bone)
		{
			this.bone = bone;
			this.offset = XformOffset.Identity;
		}

		// Token: 0x06007478 RID: 29816 RVA: 0x0025E0A8 File Offset: 0x0025C2A8
		public BoneOffset(GTHardCodedBones.EBone bone, XformOffset offset)
		{
			this.bone = bone;
			this.offset = offset;
		}

		// Token: 0x06007479 RID: 29817 RVA: 0x0025E0BD File Offset: 0x0025C2BD
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Quaternion rot)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rot);
		}

		// Token: 0x0600747A RID: 29818 RVA: 0x0025E0D8 File Offset: 0x0025C2D8
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Vector3 rotAngles)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rotAngles);
		}

		// Token: 0x0600747B RID: 29819 RVA: 0x0025E0F3 File Offset: 0x0025C2F3
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Quaternion rot, Vector3 scale)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rot, scale);
		}

		// Token: 0x0600747C RID: 29820 RVA: 0x0025E110 File Offset: 0x0025C310
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Vector3 rotAngles, Vector3 scale)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rotAngles, scale);
		}

		// Token: 0x04008456 RID: 33878
		public GTHardCodedBones.SturdyEBone bone;

		// Token: 0x04008457 RID: 33879
		public XformOffset offset;

		// Token: 0x04008458 RID: 33880
		public static readonly BoneOffset Identity = new BoneOffset
		{
			bone = GTHardCodedBones.EBone.None,
			offset = XformOffset.Identity
		};
	}
}
