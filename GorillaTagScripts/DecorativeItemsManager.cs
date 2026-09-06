using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace GorillaTagScripts
{
	// Token: 0x02000F81 RID: 3969
	[NetworkBehaviourWeaved(1)]
	public class DecorativeItemsManager : NetworkComponent
	{
		// Token: 0x17000966 RID: 2406
		// (get) Token: 0x06006267 RID: 25191 RVA: 0x001FA8CC File Offset: 0x001F8ACC
		public static DecorativeItemsManager Instance
		{
			get
			{
				return DecorativeItemsManager._instance;
			}
		}

		// Token: 0x06006268 RID: 25192 RVA: 0x001FA8D4 File Offset: 0x001F8AD4
		protected override void Awake()
		{
			base.Awake();
			if (DecorativeItemsManager._instance != null && DecorativeItemsManager._instance != this)
			{
				global::UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				DecorativeItemsManager._instance = this;
			}
			this.currentIndex = -1;
			this.shouldRunUpdate = true;
			this.zone = base.GetComponent<ZoneBasedObject>();
			foreach (DecorativeItem decorativeItem in this.decorativeItemsContainer.GetComponentsInChildren<DecorativeItem>(false))
			{
				if (decorativeItem)
				{
					this.itemsList.Add(decorativeItem);
					DecorativeItem decorativeItem2 = decorativeItem;
					decorativeItem2.respawnItem = (UnityAction<DecorativeItem>)Delegate.Combine(decorativeItem2.respawnItem, new UnityAction<DecorativeItem>(this.OnRequestToRespawn));
				}
			}
			foreach (AttachPoint attachPoint in this.respawnableHooksContainer.GetComponentsInChildren<AttachPoint>(false))
			{
				if (attachPoint)
				{
					this.respawnableHooks.Add(attachPoint);
				}
			}
			this.allHooks.AddRange(this.respawnableHooks);
			foreach (GameObject gameObject in this.nonRespawnableHooksContainer)
			{
				foreach (AttachPoint attachPoint2 in gameObject.GetComponentsInChildren<AttachPoint>(false))
				{
					if (attachPoint2)
					{
						this.allHooks.Add(attachPoint2);
					}
				}
			}
		}

		// Token: 0x06006269 RID: 25193 RVA: 0x001FAA3C File Offset: 0x001F8C3C
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (DecorativeItem decorativeItem in this.itemsList)
			{
				decorativeItem.respawnItem = (UnityAction<DecorativeItem>)Delegate.Remove(decorativeItem.respawnItem, new UnityAction<DecorativeItem>(this.OnRequestToRespawn));
			}
			this.itemsList.Clear();
			this.respawnableHooks.Clear();
			if (DecorativeItemsManager._instance == this)
			{
				DecorativeItemsManager._instance = null;
			}
		}

		// Token: 0x0600626A RID: 25194 RVA: 0x001FAAD8 File Offset: 0x001F8CD8
		private void Update()
		{
			if (!PhotonNetwork.InRoom)
			{
				return;
			}
			if (this.wasInZone != this.zone.IsLocalPlayerInZone())
			{
				this.shouldRunUpdate = true;
			}
			if (!this.shouldRunUpdate)
			{
				return;
			}
			if (base.IsMine)
			{
				if (this.wasInZone != this.zone.IsLocalPlayerInZone())
				{
					foreach (AttachPoint attachPoint in this.allHooks)
					{
						attachPoint.SetIsHook(false);
					}
					for (int i = 0; i < this.itemsList.Count; i++)
					{
						this.itemsList[i].itemState = TransferrableObject.ItemStates.State2;
						this.SpawnItem(i);
					}
					this.shouldRunUpdate = false;
				}
				this.wasInZone = this.zone.IsLocalPlayerInZone();
				this.SpawnItem(this.UpdateListPerFrame());
			}
		}

		// Token: 0x0600626B RID: 25195 RVA: 0x001FABC8 File Offset: 0x001F8DC8
		private void SpawnItem(int index)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			if (index < 0 || index >= this.itemsList.Count)
			{
				return;
			}
			if (this.respawnableHooks == null)
			{
				return;
			}
			if (this.itemsList == null)
			{
				return;
			}
			if (this.itemsList.Count > this.respawnableHooks.Count)
			{
				Debug.LogError("Trying to snap more decorative items than allowed! Some items will be left un-hooked!");
				return;
			}
			Transform transform = this.RandomSpawn();
			if (transform == null)
			{
				return;
			}
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			DecorativeItem decorativeItem = this.itemsList[index];
			decorativeItem.WorldShareableRequestOwnership();
			decorativeItem.Respawn(position, rotation);
			base.SendRPC("RespawnItemRPC", RpcTarget.Others, new object[] { index, position, rotation });
		}

		// Token: 0x0600626C RID: 25196 RVA: 0x001FAC8F File Offset: 0x001F8E8F
		[PunRPC]
		private void RespawnItemRPC(int index, Vector3 _transformPos, Quaternion _transformRot, PhotonMessageInfo info)
		{
			this.RespawnItemShared(index, _transformPos, _transformRot, info);
		}

		// Token: 0x0600626D RID: 25197 RVA: 0x001FACA4 File Offset: 0x001F8EA4
		[Rpc]
		private unsafe void RPC_RespawnItem(int index, Vector3 _transformPos, Quaternion _transformRot, RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != SimulationStages.Resimulate)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) == 0)
					{
						NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTagScripts.DecorativeItemsManager::RPC_RespawnItem(System.Int32,UnityEngine.Vector3,UnityEngine.Quaternion,Fusion.RpcInfo)", base.Object, 7);
					}
					else
					{
						int num = 8;
						num += 4;
						num += 12;
						num += 16;
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.DecorativeItemsManager::RPC_RespawnItem(System.Int32,UnityEngine.Vector3,UnityEngine.Quaternion,Fusion.RpcInfo)", num);
						}
						else
						{
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
								int num2 = 8;
								*(int*)(ptr2 + num2) = index;
								num2 += 4;
								*(Vector3*)(ptr2 + num2) = _transformPos;
								num2 += 12;
								*(Quaternion*)(ptr2 + num2) = _transformRot;
								num2 += 16;
								ptr->Offset = num2 * 8;
								base.Runner.SendRpc(ptr);
							}
							if ((localAuthorityMask & 7) != 0)
							{
								info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
								goto IL_0012;
							}
						}
					}
				}
				return;
			}
			this.InvokeRpc = false;
			IL_0012:
			this.RespawnItemShared(index, _transformPos, _transformRot, info);
		}

		// Token: 0x0600626E RID: 25198 RVA: 0x001FAE64 File Offset: 0x001F9064
		protected void RespawnItemShared(int index, Vector3 _transformPos, Quaternion _transformRot, PhotonMessageInfoWrapped info)
		{
			if (index >= 0 && index <= this.itemsList.Count - 1)
			{
				float num = 10000f;
				if ((in _transformPos).IsValid(in num) && (in _transformRot).IsValid() && info.Sender == NetworkSystem.Instance.MasterClient)
				{
					MonkeAgent.IncrementRPCCall(info, "RespawnItemRPC");
					this.itemsList[index].Respawn(_transformPos, _transformRot);
					return;
				}
			}
		}

		// Token: 0x0600626F RID: 25199 RVA: 0x001FAED4 File Offset: 0x001F90D4
		private Transform RandomSpawn()
		{
			this.lastIndex = this.currentIndex;
			bool flag = false;
			bool flag2 = this.zone.IsLocalPlayerInZone();
			int num = Random.Range(0, this.respawnableHooks.Count);
			while (!flag)
			{
				num = Random.Range(0, this.respawnableHooks.Count);
				if (!this.respawnableHooks[num].inForest == flag2)
				{
					flag = true;
				}
			}
			if (!this.respawnableHooks[num].IsHooked())
			{
				this.currentIndex = num;
			}
			else
			{
				this.currentIndex = -1;
			}
			if (this.currentIndex != this.lastIndex && this.currentIndex > -1)
			{
				return this.respawnableHooks[this.currentIndex].attachPoint;
			}
			this.currentIndex = -1;
			return null;
		}

		// Token: 0x06006270 RID: 25200 RVA: 0x001FAF96 File Offset: 0x001F9196
		private int UpdateListPerFrame()
		{
			this.arrayIndex++;
			if (this.arrayIndex >= this.itemsList.Count || this.arrayIndex < 0)
			{
				this.shouldRunUpdate = false;
				return -1;
			}
			return this.arrayIndex;
		}

		// Token: 0x06006271 RID: 25201 RVA: 0x001FAFD4 File Offset: 0x001F91D4
		private void OnRequestToRespawn(DecorativeItem item)
		{
			if (base.IsMine)
			{
				if (item == null)
				{
					return;
				}
				int num = this.itemsList.IndexOf(item);
				this.SpawnItem(num);
			}
		}

		// Token: 0x06006272 RID: 25202 RVA: 0x001FB008 File Offset: 0x001F9208
		public AttachPoint getCurrentAttachPointByPosition(Vector3 _attachPoint)
		{
			foreach (AttachPoint attachPoint in this.allHooks)
			{
				if (attachPoint.attachPoint.position == _attachPoint)
				{
					return attachPoint;
				}
			}
			return null;
		}

		// Token: 0x17000967 RID: 2407
		// (get) Token: 0x06006273 RID: 25203 RVA: 0x001FB070 File Offset: 0x001F9270
		// (set) Token: 0x06006274 RID: 25204 RVA: 0x001FB096 File Offset: 0x001F9296
		[Networked]
		[NetworkedWeaved(0, 1)]
		public unsafe int Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing DecorativeItemsManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return this.Ptr[0];
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing DecorativeItemsManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				this.Ptr[0] = value;
			}
		}

		// Token: 0x06006275 RID: 25205 RVA: 0x001FB0BD File Offset: 0x001F92BD
		public override void WriteDataFusion()
		{
			this.Data = this.currentIndex;
		}

		// Token: 0x06006276 RID: 25206 RVA: 0x001FB0CB File Offset: 0x001F92CB
		public override void ReadDataFusion()
		{
			this.currentIndex = this.Data;
		}

		// Token: 0x06006277 RID: 25207 RVA: 0x001FB0D9 File Offset: 0x001F92D9
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			stream.SendNext(this.currentIndex);
		}

		// Token: 0x06006278 RID: 25208 RVA: 0x001FB0FA File Offset: 0x001F92FA
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			this.currentIndex = (int)stream.ReceiveNext();
		}

		// Token: 0x0600627A RID: 25210 RVA: 0x001FB156 File Offset: 0x001F9356
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x0600627B RID: 25211 RVA: 0x001FB16E File Offset: 0x001F936E
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x0600627C RID: 25212 RVA: 0x001FB184 File Offset: 0x001F9384
		[NetworkRpcWeavedInvoker(1, 7, 7)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_RespawnItem@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			int num2 = *(int*)(ptr + num);
			num += 4;
			int num3 = num2;
			Vector3 vector = *(Vector3*)(ptr + num);
			num += 12;
			Vector3 vector2 = vector;
			Quaternion quaternion = *(Quaternion*)(ptr + num);
			num += 16;
			Quaternion quaternion2 = quaternion;
			RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
			behaviour.InvokeRpc = true;
			((DecorativeItemsManager)behaviour).RPC_RespawnItem(num3, vector2, quaternion2, rpcInfo);
		}

		// Token: 0x04007131 RID: 28977
		public GameObject decorativeItemsContainer;

		// Token: 0x04007132 RID: 28978
		public GameObject respawnableHooksContainer;

		// Token: 0x04007133 RID: 28979
		public List<GameObject> nonRespawnableHooksContainer = new List<GameObject>();

		// Token: 0x04007134 RID: 28980
		private readonly List<DecorativeItem> itemsList = new List<DecorativeItem>();

		// Token: 0x04007135 RID: 28981
		private readonly List<AttachPoint> respawnableHooks = new List<AttachPoint>();

		// Token: 0x04007136 RID: 28982
		private readonly List<AttachPoint> allHooks = new List<AttachPoint>();

		// Token: 0x04007137 RID: 28983
		private int lastIndex;

		// Token: 0x04007138 RID: 28984
		private int currentIndex;

		// Token: 0x04007139 RID: 28985
		private int arrayIndex = -1;

		// Token: 0x0400713A RID: 28986
		private bool shouldRunUpdate;

		// Token: 0x0400713B RID: 28987
		private ZoneBasedObject zone;

		// Token: 0x0400713C RID: 28988
		private bool wasInZone;

		// Token: 0x0400713D RID: 28989
		[OnEnterPlay_SetNull]
		private static DecorativeItemsManager _instance;

		// Token: 0x0400713E RID: 28990
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 1)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private int _Data;
	}
}
