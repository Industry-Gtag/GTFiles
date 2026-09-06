using System;
using GorillaExtensions;
using UnityEngine;

namespace Photon.Pun
{
	// Token: 0x02000F35 RID: 3893
	[RequireComponent(typeof(Rigidbody))]
	public class RigOwnedRigidbodyView : MonoBehaviourPun, IPunObservable
	{
		// Token: 0x1700093F RID: 2367
		// (get) Token: 0x06005F81 RID: 24449 RVA: 0x001E4E98 File Offset: 0x001E3098
		// (set) Token: 0x06005F82 RID: 24450 RVA: 0x001E4EA0 File Offset: 0x001E30A0
		public bool IsMine { get; private set; }

		// Token: 0x06005F83 RID: 24451 RVA: 0x001E4EA9 File Offset: 0x001E30A9
		public void SetIsMine(bool isMine)
		{
			this.IsMine = isMine;
		}

		// Token: 0x06005F84 RID: 24452 RVA: 0x001E4EB2 File Offset: 0x001E30B2
		public void Awake()
		{
			this.m_Body = base.GetComponent<Rigidbody>();
			this.m_NetworkPosition = default(Vector3);
			this.m_NetworkRotation = default(Quaternion);
		}

		// Token: 0x06005F85 RID: 24453 RVA: 0x001E4ED8 File Offset: 0x001E30D8
		public void FixedUpdate()
		{
			if (!this.IsMine)
			{
				this.m_Body.position = Vector3.MoveTowards(this.m_Body.position, this.m_NetworkPosition, this.m_Distance * (1f / (float)PhotonNetwork.SerializationRate));
				this.m_Body.rotation = Quaternion.RotateTowards(this.m_Body.rotation, this.m_NetworkRotation, this.m_Angle * (1f / (float)PhotonNetwork.SerializationRate));
			}
		}

		// Token: 0x06005F86 RID: 24454 RVA: 0x001E4F58 File Offset: 0x001E3158
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != info.photonView.Owner)
			{
				return;
			}
			try
			{
				if (stream.IsWriting)
				{
					stream.SendNext(this.m_Body.position);
					stream.SendNext(this.m_Body.rotation);
					if (this.m_SynchronizeVelocity)
					{
						stream.SendNext(this.m_Body.linearVelocity);
					}
					if (this.m_SynchronizeAngularVelocity)
					{
						stream.SendNext(this.m_Body.angularVelocity);
					}
					stream.SendNext(this.m_Body.IsSleeping());
				}
				else
				{
					Vector3 vector = (Vector3)stream.ReceiveNext();
					(ref this.m_NetworkPosition).SetValueSafe(in vector);
					Quaternion quaternion = (Quaternion)stream.ReceiveNext();
					(ref this.m_NetworkRotation).SetValueSafe(in quaternion);
					if (this.m_TeleportEnabled && Vector3.Distance(this.m_Body.position, this.m_NetworkPosition) > this.m_TeleportIfDistanceGreaterThan)
					{
						this.m_Body.position = this.m_NetworkPosition;
					}
					if (this.m_SynchronizeVelocity || this.m_SynchronizeAngularVelocity)
					{
						float num = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
						if (this.m_SynchronizeVelocity)
						{
							Vector3 vector2 = (Vector3)stream.ReceiveNext();
							float num2 = 10000f;
							if (!(in vector2).IsValid(in num2))
							{
								vector2 = Vector3.zero;
							}
							if (!this.m_Body.isKinematic)
							{
								this.m_Body.linearVelocity = vector2;
							}
							this.m_NetworkPosition += this.m_Body.linearVelocity * num;
							this.m_Distance = Vector3.Distance(this.m_Body.position, this.m_NetworkPosition);
						}
						if (this.m_SynchronizeAngularVelocity)
						{
							Vector3 vector3 = (Vector3)stream.ReceiveNext();
							float num2 = 10000f;
							if (!(in vector3).IsValid(in num2))
							{
								vector3 = Vector3.zero;
							}
							this.m_Body.angularVelocity = vector3;
							this.m_NetworkRotation = Quaternion.Euler(this.m_Body.angularVelocity * num) * this.m_NetworkRotation;
							this.m_Angle = Quaternion.Angle(this.m_Body.rotation, this.m_NetworkRotation);
						}
					}
					if ((bool)stream.ReceiveNext())
					{
						this.m_Body.Sleep();
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x04006DE8 RID: 28136
		private float m_Distance;

		// Token: 0x04006DE9 RID: 28137
		private float m_Angle;

		// Token: 0x04006DEA RID: 28138
		private Rigidbody m_Body;

		// Token: 0x04006DEB RID: 28139
		private Vector3 m_NetworkPosition;

		// Token: 0x04006DEC RID: 28140
		private Quaternion m_NetworkRotation;

		// Token: 0x04006DED RID: 28141
		public bool m_SynchronizeVelocity = true;

		// Token: 0x04006DEE RID: 28142
		public bool m_SynchronizeAngularVelocity;

		// Token: 0x04006DEF RID: 28143
		public bool m_TeleportEnabled;

		// Token: 0x04006DF0 RID: 28144
		public float m_TeleportIfDistanceGreaterThan = 3f;
	}
}
