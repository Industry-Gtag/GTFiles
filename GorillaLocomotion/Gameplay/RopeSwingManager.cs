using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020011AE RID: 4526
	public class RopeSwingManager : NetworkSceneObject
	{
		// Token: 0x17000B30 RID: 2864
		// (get) Token: 0x0600722B RID: 29227 RVA: 0x0025293F File Offset: 0x00250B3F
		// (set) Token: 0x0600722C RID: 29228 RVA: 0x00252946 File Offset: 0x00250B46
		public static RopeSwingManager instance { get; private set; }

		// Token: 0x0600722D RID: 29229 RVA: 0x00252950 File Offset: 0x00250B50
		private void Awake()
		{
			if (RopeSwingManager.instance != null && RopeSwingManager.instance != this)
			{
				GTDev.LogWarning<string>("Instance of RopeSwingManager already exists. Destroying.", null);
				global::UnityEngine.Object.Destroy(this);
				return;
			}
			if (RopeSwingManager.instance == null)
			{
				RopeSwingManager.instance = this;
			}
		}

		// Token: 0x0600722E RID: 29230 RVA: 0x0025299C File Offset: 0x00250B9C
		private void RegisterInstance(GorillaRopeSwing t)
		{
			this.ropes.Add(t.ropeId, t);
		}

		// Token: 0x0600722F RID: 29231 RVA: 0x002529B0 File Offset: 0x00250BB0
		private void UnregisterInstance(GorillaRopeSwing t)
		{
			this.ropes.Remove(t.ropeId);
		}

		// Token: 0x06007230 RID: 29232 RVA: 0x002529C4 File Offset: 0x00250BC4
		public static void Register(GorillaRopeSwing t)
		{
			RopeSwingManager.instance.RegisterInstance(t);
		}

		// Token: 0x06007231 RID: 29233 RVA: 0x002529D1 File Offset: 0x00250BD1
		public static void Unregister(GorillaRopeSwing t)
		{
			RopeSwingManager.instance.UnregisterInstance(t);
		}

		// Token: 0x06007232 RID: 29234 RVA: 0x002529E0 File Offset: 0x00250BE0
		public void SendSetVelocity_RPC(int ropeId, int boneIndex, Vector3 velocity, bool wholeRope)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				this.photonView.RPC("SetVelocity", RpcTarget.All, new object[] { ropeId, boneIndex, velocity, wholeRope });
				return;
			}
			this.SetVelocityShared(ropeId, boneIndex, velocity, wholeRope, default(PhotonMessageInfoWrapped));
		}

		// Token: 0x06007233 RID: 29235 RVA: 0x00252A4A File Offset: 0x00250C4A
		public bool TryGetRope(int ropeId, out GorillaRopeSwing result)
		{
			return this.ropes.TryGetValue(ropeId, out result);
		}

		// Token: 0x06007234 RID: 29236 RVA: 0x00252A5C File Offset: 0x00250C5C
		[PunRPC]
		public void SetVelocity(int ropeId, int boneIndex, Vector3 velocity, bool wholeRope, PhotonMessageInfo info)
		{
			PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
			this.SetVelocityShared(ropeId, boneIndex, velocity, wholeRope, photonMessageInfoWrapped);
			Utils.Log("Receiving RPC for ropes");
		}

		// Token: 0x06007235 RID: 29237 RVA: 0x00252A88 File Offset: 0x00250C88
		[Rpc]
		public unsafe static void RPC_SetVelocity(NetworkRunner runner, int ropeId, int boneIndex, Vector3 velocity, bool wholeRope, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					int num = 8;
					num += 4;
					num += 4;
					num += 12;
					num += 4;
					if (SimulationMessage.CanAllocateUserPayload(num))
					{
						if (runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaLocomotion.Gameplay.RopeSwingManager::RPC_SetVelocity(Fusion.NetworkRunner,System.Int32,System.Int32,UnityEngine.Vector3,System.Boolean,Fusion.RpcInfo)"));
							int num2 = 8;
							*(int*)(ptr2 + num2) = ropeId;
							num2 += 4;
							*(int*)(ptr2 + num2) = boneIndex;
							num2 += 4;
							*(Vector3*)(ptr2 + num2) = velocity;
							num2 += 12;
							ReadWriteUtilsForWeaver.WriteBoolean((int*)(ptr2 + num2), wholeRope);
							num2 += 4;
							ptr->Offset = num2 * 8;
							ptr->SetStatic();
							runner.SendRpc(ptr);
						}
						info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_0010;
					}
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaLocomotion.Gameplay.RopeSwingManager::RPC_SetVelocity(Fusion.NetworkRunner,System.Int32,System.Int32,UnityEngine.Vector3,System.Boolean,Fusion.RpcInfo)", num);
				}
				return;
			}
			IL_0010:
			PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
			RopeSwingManager.instance.SetVelocityShared(ropeId, boneIndex, velocity, wholeRope, photonMessageInfoWrapped);
		}

		// Token: 0x06007236 RID: 29238 RVA: 0x00252C20 File Offset: 0x00250E20
		private void SetVelocityShared(int ropeId, int boneIndex, Vector3 velocity, bool wholeRope, PhotonMessageInfoWrapped info)
		{
			if (info.Sender != null)
			{
				MonkeAgent.IncrementRPCCall(info, "SetVelocityShared");
			}
			GorillaRopeSwing gorillaRopeSwing;
			if (this.TryGetRope(ropeId, out gorillaRopeSwing) && gorillaRopeSwing != null)
			{
				gorillaRopeSwing.SetVelocity(boneIndex, velocity, wholeRope, info);
			}
		}

		// Token: 0x06007238 RID: 29240 RVA: 0x00252C78 File Offset: 0x00250E78
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaLocomotion.Gameplay.RopeSwingManager::RPC_SetVelocity(Fusion.NetworkRunner,System.Int32,System.Int32,UnityEngine.Vector3,System.Boolean,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_SetVelocity@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			int num2 = *(int*)(ptr + num);
			num += 4;
			int num3 = num2;
			int num4 = *(int*)(ptr + num);
			num += 4;
			int num5 = num4;
			Vector3 vector = *(Vector3*)(ptr + num);
			num += 12;
			Vector3 vector2 = vector;
			bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(ptr + num));
			num += 4;
			bool flag2 = flag;
			RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			RopeSwingManager.RPC_SetVelocity(runner, num3, num5, vector2, flag2, rpcInfo);
		}

		// Token: 0x040082FB RID: 33531
		private Dictionary<int, GorillaRopeSwing> ropes = new Dictionary<int, GorillaRopeSwing>();
	}
}
