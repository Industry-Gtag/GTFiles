using System;
using GorillaExtensions;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F80 RID: 3968
	public class DecorativeItemReliableState : MonoBehaviour, IPunObservable
	{
		// Token: 0x06006265 RID: 25189 RVA: 0x001FA7A8 File Offset: 0x001F89A8
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(this.isSnapped);
				stream.SendNext(this.snapPosition);
				stream.SendNext(this.respawnPosition);
				stream.SendNext(this.respawnRotation);
				return;
			}
			this.isSnapped = (bool)stream.ReceiveNext();
			this.snapPosition = (Vector3)stream.ReceiveNext();
			this.respawnPosition = (Vector3)stream.ReceiveNext();
			this.respawnRotation = (Quaternion)stream.ReceiveNext();
			float num = 10000f;
			if (!(in this.snapPosition).IsValid(in num))
			{
				this.snapPosition = Vector3.zero;
			}
			num = 10000f;
			if (!(in this.respawnPosition).IsValid(in num))
			{
				this.respawnPosition = Vector3.zero;
			}
			if (!(in this.respawnRotation).IsValid())
			{
				this.respawnRotation = quaternion.identity;
			}
		}

		// Token: 0x0400712D RID: 28973
		public bool isSnapped;

		// Token: 0x0400712E RID: 28974
		public Vector3 snapPosition = Vector3.zero;

		// Token: 0x0400712F RID: 28975
		public Vector3 respawnPosition = Vector3.zero;

		// Token: 0x04007130 RID: 28976
		public Quaternion respawnRotation = Quaternion.identity;
	}
}
