using System;
using GorillaExtensions;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F5A RID: 3930
	public class BuilderItemReliableState : MonoBehaviour, IPunObservable
	{
		// Token: 0x060060B0 RID: 24752 RVA: 0x001EAAFC File Offset: 0x001E8CFC
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(this.rightHandAttachPos);
				stream.SendNext(this.rightHandAttachRot);
				stream.SendNext(this.leftHandAttachPos);
				stream.SendNext(this.leftHandAttachRot);
				return;
			}
			this.rightHandAttachPos = (Vector3)stream.ReceiveNext();
			this.rightHandAttachRot = (Quaternion)stream.ReceiveNext();
			this.leftHandAttachPos = (Vector3)stream.ReceiveNext();
			this.leftHandAttachRot = (Quaternion)stream.ReceiveNext();
			float num = 10000f;
			if (!(in this.rightHandAttachPos).IsValid(in num))
			{
				this.rightHandAttachPos = Vector3.zero;
			}
			if (!(in this.rightHandAttachRot).IsValid())
			{
				this.rightHandAttachRot = quaternion.identity;
			}
			num = 10000f;
			if (!(in this.leftHandAttachPos).IsValid(in num))
			{
				this.leftHandAttachPos = Vector3.zero;
			}
			if (!(in this.leftHandAttachRot).IsValid())
			{
				this.leftHandAttachRot = quaternion.identity;
			}
			this.dirty = true;
		}

		// Token: 0x04006F4F RID: 28495
		public Vector3 rightHandAttachPos = Vector3.zero;

		// Token: 0x04006F50 RID: 28496
		public Quaternion rightHandAttachRot = Quaternion.identity;

		// Token: 0x04006F51 RID: 28497
		public Vector3 leftHandAttachPos = Vector3.zero;

		// Token: 0x04006F52 RID: 28498
		public Quaternion leftHandAttachRot = Quaternion.identity;

		// Token: 0x04006F53 RID: 28499
		public bool dirty;
	}
}
