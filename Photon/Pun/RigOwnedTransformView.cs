using System;
using GorillaExtensions;
using UnityEngine;

namespace Photon.Pun
{
	// Token: 0x02000F36 RID: 3894
	[HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
	public class RigOwnedTransformView : MonoBehaviourPun, IPunObservable
	{
		// Token: 0x17000940 RID: 2368
		// (get) Token: 0x06005F88 RID: 24456 RVA: 0x001E51EE File Offset: 0x001E33EE
		// (set) Token: 0x06005F89 RID: 24457 RVA: 0x001E51F6 File Offset: 0x001E33F6
		public bool IsMine { get; private set; }

		// Token: 0x06005F8A RID: 24458 RVA: 0x001E51FF File Offset: 0x001E33FF
		public void SetIsMine(bool isMine)
		{
			this.IsMine = isMine;
		}

		// Token: 0x06005F8B RID: 24459 RVA: 0x001E5208 File Offset: 0x001E3408
		public void Awake()
		{
			this.m_StoredPosition = base.transform.localPosition;
			this.m_NetworkPosition = Vector3.zero;
			this.m_networkScale = Vector3.one;
			this.m_NetworkRotation = Quaternion.identity;
		}

		// Token: 0x06005F8C RID: 24460 RVA: 0x001E523C File Offset: 0x001E343C
		private void Reset()
		{
			this.m_UseLocal = true;
		}

		// Token: 0x06005F8D RID: 24461 RVA: 0x001E5245 File Offset: 0x001E3445
		private void OnEnable()
		{
			this.m_firstTake = true;
		}

		// Token: 0x06005F8E RID: 24462 RVA: 0x001E5250 File Offset: 0x001E3450
		public void Update()
		{
			Transform transform = base.transform;
			if (!this.IsMine && this.IsValid(this.m_NetworkPosition) && this.IsValid(this.m_NetworkRotation))
			{
				if (this.m_UseLocal)
				{
					transform.localPosition = Vector3.MoveTowards(transform.localPosition, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
					transform.localRotation = Quaternion.RotateTowards(transform.localRotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
					return;
				}
				transform.position = Vector3.MoveTowards(transform.position, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
			}
		}

		// Token: 0x06005F8F RID: 24463 RVA: 0x001E5344 File Offset: 0x001E3544
		private bool IsValid(Vector3 v)
		{
			return !float.IsNaN(v.x) && !float.IsNaN(v.y) && !float.IsNaN(v.z) && !float.IsInfinity(v.x) && !float.IsInfinity(v.y) && !float.IsInfinity(v.z);
		}

		// Token: 0x06005F90 RID: 24464 RVA: 0x001E53A4 File Offset: 0x001E35A4
		private bool IsValid(Quaternion q)
		{
			return !float.IsNaN(q.x) && !float.IsNaN(q.y) && !float.IsNaN(q.z) && !float.IsNaN(q.w) && !float.IsInfinity(q.x) && !float.IsInfinity(q.y) && !float.IsInfinity(q.z) && !float.IsInfinity(q.w);
		}

		// Token: 0x06005F91 RID: 24465 RVA: 0x001E541C File Offset: 0x001E361C
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != info.photonView.Owner)
			{
				return;
			}
			try
			{
				Transform transform = base.transform;
				if (stream.IsWriting)
				{
					if (this.m_SynchronizePosition)
					{
						if (this.m_UseLocal)
						{
							this.m_Direction = transform.localPosition - this.m_StoredPosition;
							this.m_StoredPosition = transform.localPosition;
							stream.SendNext(transform.localPosition);
							stream.SendNext(this.m_Direction);
						}
						else
						{
							this.m_Direction = transform.position - this.m_StoredPosition;
							this.m_StoredPosition = transform.position;
							stream.SendNext(transform.position);
							stream.SendNext(this.m_Direction);
						}
					}
					if (this.m_SynchronizeRotation)
					{
						if (this.m_UseLocal)
						{
							stream.SendNext(transform.localRotation);
						}
						else
						{
							stream.SendNext(transform.rotation);
						}
					}
					if (this.m_SynchronizeScale)
					{
						stream.SendNext(transform.localScale);
					}
				}
				else
				{
					if (this.m_SynchronizePosition)
					{
						Vector3 vector = (Vector3)stream.ReceiveNext();
						(ref this.m_NetworkPosition).SetValueSafe(in vector);
						vector = (Vector3)stream.ReceiveNext();
						(ref this.m_Direction).SetValueSafe(in vector);
						if (this.m_firstTake)
						{
							if (this.m_UseLocal)
							{
								transform.localPosition = this.m_NetworkPosition;
							}
							else
							{
								transform.position = this.m_NetworkPosition;
							}
							this.m_Distance = 0f;
						}
						else
						{
							float num = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
							this.m_NetworkPosition += this.m_Direction * num;
							if (this.m_UseLocal)
							{
								this.m_Distance = Vector3.Distance(transform.localPosition, this.m_NetworkPosition);
							}
							else
							{
								this.m_Distance = Vector3.Distance(transform.position, this.m_NetworkPosition);
							}
						}
					}
					if (this.m_SynchronizeRotation)
					{
						Quaternion quaternion = (Quaternion)stream.ReceiveNext();
						(ref this.m_NetworkRotation).SetValueSafe(in quaternion);
						if (this.m_firstTake)
						{
							this.m_Angle = 0f;
							if (this.m_UseLocal)
							{
								transform.localRotation = this.m_NetworkRotation;
							}
							else
							{
								transform.rotation = this.m_NetworkRotation;
							}
						}
						else if (this.m_UseLocal)
						{
							this.m_Angle = Quaternion.Angle(transform.localRotation, this.m_NetworkRotation);
						}
						else
						{
							this.m_Angle = Quaternion.Angle(transform.rotation, this.m_NetworkRotation);
						}
					}
					if (this.m_SynchronizeScale)
					{
						Vector3 vector = (Vector3)stream.ReceiveNext();
						(ref this.m_networkScale).SetValueSafe(in vector);
						transform.localScale = this.m_networkScale;
					}
					if (this.m_firstTake)
					{
						this.m_firstTake = false;
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06005F92 RID: 24466 RVA: 0x001E5245 File Offset: 0x001E3445
		public void GTAddition_DoTeleport()
		{
			this.m_firstTake = true;
		}

		// Token: 0x04006DF2 RID: 28146
		private float m_Distance;

		// Token: 0x04006DF3 RID: 28147
		private float m_Angle;

		// Token: 0x04006DF4 RID: 28148
		private Vector3 m_Direction;

		// Token: 0x04006DF5 RID: 28149
		private Vector3 m_NetworkPosition;

		// Token: 0x04006DF6 RID: 28150
		private Vector3 m_StoredPosition;

		// Token: 0x04006DF7 RID: 28151
		private Vector3 m_networkScale;

		// Token: 0x04006DF8 RID: 28152
		private Quaternion m_NetworkRotation;

		// Token: 0x04006DF9 RID: 28153
		public bool m_SynchronizePosition = true;

		// Token: 0x04006DFA RID: 28154
		public bool m_SynchronizeRotation = true;

		// Token: 0x04006DFB RID: 28155
		public bool m_SynchronizeScale;

		// Token: 0x04006DFC RID: 28156
		[Tooltip("Indicates if localPosition and localRotation should be used. Scale ignores this setting, and always uses localScale to avoid issues with lossyScale.")]
		public bool m_UseLocal;

		// Token: 0x04006DFD RID: 28157
		private bool m_firstTake;
	}
}
