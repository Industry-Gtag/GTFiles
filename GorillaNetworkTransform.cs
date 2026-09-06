using System;
using System.Runtime.InteropServices;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000846 RID: 2118
[NetworkBehaviourWeaved(15)]
internal class GorillaNetworkTransform : NetworkComponent, ITickSystemTick
{
	// Token: 0x170004C6 RID: 1222
	// (get) Token: 0x06003671 RID: 13937 RVA: 0x0012CE6C File Offset: 0x0012B06C
	public bool RespectOwnership
	{
		get
		{
			return this.respectOwnership;
		}
	}

	// Token: 0x170004C7 RID: 1223
	// (get) Token: 0x06003672 RID: 13938 RVA: 0x0012CE74 File Offset: 0x0012B074
	// (set) Token: 0x06003673 RID: 13939 RVA: 0x0012CE7C File Offset: 0x0012B07C
	public bool TickRunning { get; set; }

	// Token: 0x170004C8 RID: 1224
	// (get) Token: 0x06003674 RID: 13940 RVA: 0x0012CE85 File Offset: 0x0012B085
	// (set) Token: 0x06003675 RID: 13941 RVA: 0x0012CEAF File Offset: 0x0012B0AF
	[Networked]
	[NetworkedWeaved(0, 15)]
	private unsafe GorillaNetworkTransform.NetTransformData data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GorillaNetworkTransform.data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(GorillaNetworkTransform.NetTransformData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GorillaNetworkTransform.data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(GorillaNetworkTransform.NetTransformData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06003676 RID: 13942 RVA: 0x0012CEDC File Offset: 0x0012B0DC
	public new void Awake()
	{
		this.m_StoredPosition = base.transform.localPosition;
		this.m_NetworkPosition = Vector3.zero;
		this.m_NetworkScale = Vector3.zero;
		this.m_NetworkRotation = Quaternion.identity;
		this.maxDistanceSquare = this.maxDistance * this.maxDistance;
	}

	// Token: 0x06003677 RID: 13943 RVA: 0x0012CF30 File Offset: 0x0012B130
	private new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		this.m_firstTake = true;
		if (this.clampToSpawn)
		{
			this.clampOriginPoint = (this.m_UseLocal ? base.transform.localPosition : base.transform.position);
		}
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06003678 RID: 13944 RVA: 0x0012CF7E File Offset: 0x0012B17E
	private new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06003679 RID: 13945 RVA: 0x0012CF8C File Offset: 0x0012B18C
	public void Tick()
	{
		if (!base.IsLocallyOwned)
		{
			if (this.m_UseLocal)
			{
				base.transform.SetLocalPositionAndRotation(Vector3.MoveTowards(base.transform.localPosition, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)NetworkSystem.Instance.TickRate), Quaternion.RotateTowards(base.transform.localRotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)NetworkSystem.Instance.TickRate));
				return;
			}
			base.transform.SetPositionAndRotation(Vector3.MoveTowards(base.transform.position, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)NetworkSystem.Instance.TickRate), Quaternion.RotateTowards(base.transform.rotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)NetworkSystem.Instance.TickRate));
		}
	}

	// Token: 0x0600367A RID: 13946 RVA: 0x0012D07C File Offset: 0x0012B27C
	public override void WriteDataFusion()
	{
		GorillaNetworkTransform.NetTransformData netTransformData = this.SharedWrite();
		double num = NetworkSystem.Instance.SimTick / 1000.0;
		netTransformData.SentTime = num;
		this.data = netTransformData;
	}

	// Token: 0x0600367B RID: 13947 RVA: 0x0012D0B6 File Offset: 0x0012B2B6
	public override void ReadDataFusion()
	{
		this.SharedRead(this.data);
	}

	// Token: 0x0600367C RID: 13948 RVA: 0x0012D0C4 File Offset: 0x0012B2C4
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (this.respectOwnership && player != base.Owner)
		{
			return;
		}
		GorillaNetworkTransform.NetTransformData netTransformData = this.SharedWrite();
		if (this.m_SynchronizePosition)
		{
			stream.SendNext(netTransformData.position);
			stream.SendNext(netTransformData.velocity);
		}
		if (this.m_SynchronizeRotation)
		{
			stream.SendNext(netTransformData.rotation);
		}
		if (this.m_SynchronizeScale)
		{
			stream.SendNext(netTransformData.scale);
		}
	}

	// Token: 0x0600367D RID: 13949 RVA: 0x0012D158 File Offset: 0x0012B358
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (this.respectOwnership && player != base.Owner)
		{
			return;
		}
		GorillaNetworkTransform.NetTransformData netTransformData = default(GorillaNetworkTransform.NetTransformData);
		if (this.m_SynchronizePosition)
		{
			netTransformData.position = (Vector3)stream.ReceiveNext();
			netTransformData.velocity = (Vector3)stream.ReceiveNext();
		}
		if (this.m_SynchronizeRotation)
		{
			netTransformData.rotation = (Quaternion)stream.ReceiveNext();
		}
		if (this.m_SynchronizeScale)
		{
			netTransformData.scale = (Vector3)stream.ReceiveNext();
		}
		netTransformData.SentTime = (double)((float)info.SentServerTime);
		this.SharedRead(netTransformData);
	}

	// Token: 0x0600367E RID: 13950 RVA: 0x0012D208 File Offset: 0x0012B408
	private void SharedRead(GorillaNetworkTransform.NetTransformData data)
	{
		if (this.m_SynchronizePosition)
		{
			(ref this.m_NetworkPosition).SetValueSafe(in data.position);
			(ref this.m_Velocity).SetValueSafe(in data.velocity);
			if (this.clampDistanceFromSpawn && Vector3.SqrMagnitude(this.clampOriginPoint - this.m_NetworkPosition) > this.maxDistanceSquare)
			{
				this.m_NetworkPosition = this.clampOriginPoint + this.m_Velocity.normalized * this.maxDistance;
				this.m_Velocity = Vector3.zero;
			}
			if (this.m_firstTake)
			{
				if (this.m_UseLocal)
				{
					base.transform.localPosition = this.m_NetworkPosition;
				}
				else
				{
					base.transform.position = this.m_NetworkPosition;
				}
				this.m_Distance = 0f;
			}
			else
			{
				float num = Mathf.Abs((float)(NetworkSystem.Instance.SimTime - data.SentTime));
				this.m_NetworkPosition += this.m_Velocity * num;
				if (this.m_UseLocal)
				{
					this.m_Distance = Vector3.Distance(base.transform.localPosition, this.m_NetworkPosition);
				}
				else
				{
					this.m_Distance = Vector3.Distance(base.transform.position, this.m_NetworkPosition);
				}
			}
		}
		if (this.m_SynchronizeRotation)
		{
			(ref this.m_NetworkRotation).SetValueSafe(in data.rotation);
			if (this.m_firstTake)
			{
				this.m_Angle = 0f;
				if (this.m_UseLocal)
				{
					base.transform.localRotation = this.m_NetworkRotation;
				}
				else
				{
					base.transform.rotation = this.m_NetworkRotation;
				}
			}
			else if (this.m_UseLocal)
			{
				this.m_Angle = Quaternion.Angle(base.transform.localRotation, this.m_NetworkRotation);
			}
			else
			{
				this.m_Angle = Quaternion.Angle(base.transform.rotation, this.m_NetworkRotation);
			}
		}
		if (this.m_SynchronizeScale)
		{
			(ref this.m_NetworkScale).SetValueSafe(in data.scale);
			base.transform.localScale = this.m_NetworkScale;
		}
		if (this.m_firstTake)
		{
			this.m_firstTake = false;
		}
	}

	// Token: 0x0600367F RID: 13951 RVA: 0x0012D430 File Offset: 0x0012B630
	private GorillaNetworkTransform.NetTransformData SharedWrite()
	{
		GorillaNetworkTransform.NetTransformData netTransformData = default(GorillaNetworkTransform.NetTransformData);
		if (this.m_SynchronizePosition)
		{
			if (this.m_UseLocal)
			{
				this.m_Velocity = base.transform.localPosition - this.m_StoredPosition;
				this.m_StoredPosition = base.transform.localPosition;
				netTransformData.position = base.transform.localPosition;
				netTransformData.velocity = this.m_Velocity;
			}
			else
			{
				this.m_Velocity = base.transform.position - this.m_StoredPosition;
				this.m_StoredPosition = base.transform.position;
				netTransformData.position = base.transform.position;
				netTransformData.velocity = this.m_Velocity;
			}
		}
		if (this.m_SynchronizeRotation)
		{
			if (this.m_UseLocal)
			{
				netTransformData.rotation = base.transform.localRotation;
			}
			else
			{
				netTransformData.rotation = base.transform.rotation;
			}
		}
		if (this.m_SynchronizeScale)
		{
			netTransformData.scale = base.transform.localScale;
		}
		return netTransformData;
	}

	// Token: 0x06003680 RID: 13952 RVA: 0x0012D543 File Offset: 0x0012B743
	public void GTAddition_DoTeleport()
	{
		this.m_firstTake = true;
	}

	// Token: 0x06003682 RID: 13954 RVA: 0x0012D57B File Offset: 0x0012B77B
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.data = this._data;
	}

	// Token: 0x06003683 RID: 13955 RVA: 0x0012D593 File Offset: 0x0012B793
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._data = this.data;
	}

	// Token: 0x04004724 RID: 18212
	[Tooltip("Indicates if localPosition and localRotation should be used. Scale ignores this setting, and always uses localScale to avoid issues with lossyScale.")]
	public bool m_UseLocal;

	// Token: 0x04004725 RID: 18213
	[SerializeField]
	private bool respectOwnership;

	// Token: 0x04004726 RID: 18214
	[SerializeField]
	private bool clampDistanceFromSpawn = true;

	// Token: 0x04004727 RID: 18215
	[SerializeField]
	private float maxDistance = 100f;

	// Token: 0x04004728 RID: 18216
	private float maxDistanceSquare;

	// Token: 0x04004729 RID: 18217
	[SerializeField]
	private bool clampToSpawn = true;

	// Token: 0x0400472A RID: 18218
	[Tooltip("Use this if clampToSpawn is false, to set the center point to check the synced position against")]
	[SerializeField]
	private Vector3 clampOriginPoint;

	// Token: 0x0400472B RID: 18219
	public bool m_SynchronizePosition = true;

	// Token: 0x0400472C RID: 18220
	public bool m_SynchronizeRotation = true;

	// Token: 0x0400472D RID: 18221
	public bool m_SynchronizeScale;

	// Token: 0x0400472E RID: 18222
	private float m_Distance;

	// Token: 0x0400472F RID: 18223
	private float m_Angle;

	// Token: 0x04004730 RID: 18224
	private Vector3 m_Velocity;

	// Token: 0x04004731 RID: 18225
	private Vector3 m_NetworkPosition;

	// Token: 0x04004732 RID: 18226
	private Vector3 m_StoredPosition;

	// Token: 0x04004733 RID: 18227
	private Vector3 m_NetworkScale;

	// Token: 0x04004734 RID: 18228
	private Quaternion m_NetworkRotation;

	// Token: 0x04004735 RID: 18229
	private bool m_firstTake;

	// Token: 0x04004737 RID: 18231
	[WeaverGenerated]
	[DefaultForProperty("data", 0, 15)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private GorillaNetworkTransform.NetTransformData _data;

	// Token: 0x02000847 RID: 2119
	[NetworkStructWeaved(15)]
	[StructLayout(LayoutKind.Explicit, Size = 60)]
	private struct NetTransformData : INetworkStruct
	{
		// Token: 0x04004738 RID: 18232
		[FieldOffset(0)]
		public Vector3 position;

		// Token: 0x04004739 RID: 18233
		[FieldOffset(12)]
		public Vector3 velocity;

		// Token: 0x0400473A RID: 18234
		[FieldOffset(24)]
		public Quaternion rotation;

		// Token: 0x0400473B RID: 18235
		[FieldOffset(40)]
		public Vector3 scale;

		// Token: 0x0400473C RID: 18236
		[FieldOffset(52)]
		public double SentTime;
	}
}
