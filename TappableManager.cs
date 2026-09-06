using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000932 RID: 2354
public class TappableManager : NetworkSceneObject
{
	// Token: 0x06003DAC RID: 15788 RVA: 0x0014E550 File Offset: 0x0014C750
	private void Awake()
	{
		if (TappableManager.gManager != null && TappableManager.gManager != this)
		{
			GTDev.LogWarning<string>("Instance of TappableManager already exists. Destroying.", null);
			global::UnityEngine.Object.Destroy(this);
			return;
		}
		if (TappableManager.gManager == null)
		{
			TappableManager.gManager = this;
		}
		if (TappableManager.gRegistry.Count == 0)
		{
			return;
		}
		Tappable[] array = TappableManager.gRegistry.ToArray<Tappable>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] == null))
			{
				this.RegisterInstance(array[i]);
			}
		}
		TappableManager.gRegistry.Clear();
	}

	// Token: 0x06003DAD RID: 15789 RVA: 0x0014E5E0 File Offset: 0x0014C7E0
	private void RegisterInstance(Tappable t)
	{
		if (t == null)
		{
			GTDev.LogError<string>("Tappable is null.", null);
			return;
		}
		t.manager = this;
		if (this.idSet.Add(t.tappableId))
		{
			this.tappables.Add(t);
		}
	}

	// Token: 0x06003DAE RID: 15790 RVA: 0x0014E61D File Offset: 0x0014C81D
	private void UnregisterInstance(Tappable t)
	{
		if (t == null)
		{
			return;
		}
		if (!this.idSet.Remove(t.tappableId))
		{
			return;
		}
		this.tappables.Remove(t);
		t.manager = null;
	}

	// Token: 0x06003DAF RID: 15791 RVA: 0x0014E651 File Offset: 0x0014C851
	public static void Register(Tappable t)
	{
		if (TappableManager.gManager != null)
		{
			TappableManager.gManager.RegisterInstance(t);
			return;
		}
		TappableManager.gRegistry.Add(t);
	}

	// Token: 0x06003DB0 RID: 15792 RVA: 0x0014E678 File Offset: 0x0014C878
	public static void Unregister(Tappable t)
	{
		if (TappableManager.gManager != null)
		{
			TappableManager.gManager.UnregisterInstance(t);
			return;
		}
		TappableManager.gRegistry.Remove(t);
	}

	// Token: 0x06003DB1 RID: 15793 RVA: 0x0014E6A0 File Offset: 0x0014C8A0
	[Conditional("QATESTING")]
	public void DebugTestTap()
	{
		if (this.tappables.Count > 0)
		{
			int num = Random.Range(0, this.tappables.Count);
			Debug.Log("Send TestTap to tappable index: " + num.ToString() + "/" + this.tappables.Count.ToString());
			this.tappables[num].OnTap(10f);
			return;
		}
		Debug.Log("TappableManager: tappables array is empty.");
	}

	// Token: 0x06003DB2 RID: 15794 RVA: 0x0014E71C File Offset: 0x0014C91C
	[PunRPC]
	public void SendOnTapRPC(int key, float tapStrength, PhotonMessageInfo info)
	{
		this.SendOnTapShared(key, tapStrength, new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06003DB3 RID: 15795 RVA: 0x0014E72C File Offset: 0x0014C92C
	[Rpc]
	public unsafe static void RPC_SendOnTap(NetworkRunner runner, int key, float tapStrength, RpcInfo info = default(RpcInfo))
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
				if (SimulationMessage.CanAllocateUserPayload(num))
				{
					if (runner.HasAnyActiveConnections())
					{
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
						*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void TappableManager::RPC_SendOnTap(Fusion.NetworkRunner,System.Int32,System.Single,Fusion.RpcInfo)"));
						int num2 = 8;
						*(int*)(ptr2 + num2) = key;
						num2 += 4;
						*(float*)(ptr2 + num2) = tapStrength;
						num2 += 4;
						ptr->Offset = num2 * 8;
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
					info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
					goto IL_0010;
				}
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void TappableManager::RPC_SendOnTap(Fusion.NetworkRunner,System.Int32,System.Single,Fusion.RpcInfo)", num);
			}
			return;
		}
		IL_0010:
		TappableManager.gManager.SendOnTapShared(key, tapStrength, new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06003DB4 RID: 15796 RVA: 0x0014E868 File Offset: 0x0014CA68
	private void SendOnTapShared(int key, float tapStrength, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "SendOnTapShared");
		if (key == 0 || !float.IsFinite(tapStrength))
		{
			return;
		}
		tapStrength = Mathf.Clamp(tapStrength, 0f, 1f);
		for (int i = 0; i < this.tappables.Count; i++)
		{
			Tappable tappable = this.tappables[i];
			if (tappable.tappableId == key && !tappable.IsLocalOnly)
			{
				tappable.OnTapLocal(tapStrength, Time.time, info);
			}
		}
	}

	// Token: 0x06003DB5 RID: 15797 RVA: 0x0014E8DF File Offset: 0x0014CADF
	[PunRPC]
	public void SendOnGrabRPC(int key, PhotonMessageInfo info)
	{
		this.SendOnGrabShared(key, new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06003DB6 RID: 15798 RVA: 0x0014E8F0 File Offset: 0x0014CAF0
	[Rpc]
	public unsafe static void RPC_SendOnGrab(NetworkRunner runner, int key, RpcInfo info = default(RpcInfo))
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
				if (SimulationMessage.CanAllocateUserPayload(num))
				{
					if (runner.HasAnyActiveConnections())
					{
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
						*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void TappableManager::RPC_SendOnGrab(Fusion.NetworkRunner,System.Int32,Fusion.RpcInfo)"));
						int num2 = 8;
						*(int*)(ptr2 + num2) = key;
						num2 += 4;
						ptr->Offset = num2 * 8;
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
					info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
					goto IL_0010;
				}
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void TappableManager::RPC_SendOnGrab(Fusion.NetworkRunner,System.Int32,Fusion.RpcInfo)", num);
			}
			return;
		}
		IL_0010:
		TappableManager.gManager.SendOnGrabShared(key, new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06003DB7 RID: 15799 RVA: 0x0014EA08 File Offset: 0x0014CC08
	private void SendOnGrabShared(int key, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "SendOnGrabShared");
		if (key == 0)
		{
			return;
		}
		for (int i = 0; i < this.tappables.Count; i++)
		{
			Tappable tappable = this.tappables[i];
			if (tappable.tappableId == key && !tappable.IsLocalOnly)
			{
				tappable.OnGrabLocal(Time.time, info);
			}
		}
	}

	// Token: 0x06003DB8 RID: 15800 RVA: 0x0014EA64 File Offset: 0x0014CC64
	[PunRPC]
	public void SendOnReleaseRPC(int key, PhotonMessageInfo info)
	{
		this.SendOnReleaseShared(key, new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06003DB9 RID: 15801 RVA: 0x0014EA74 File Offset: 0x0014CC74
	[Rpc]
	public unsafe static void RPC_SendOnRelease(NetworkRunner runner, int key, RpcInfo info = default(RpcInfo))
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
				if (SimulationMessage.CanAllocateUserPayload(num))
				{
					if (runner.HasAnyActiveConnections())
					{
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
						*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void TappableManager::RPC_SendOnRelease(Fusion.NetworkRunner,System.Int32,Fusion.RpcInfo)"));
						int num2 = 8;
						*(int*)(ptr2 + num2) = key;
						num2 += 4;
						ptr->Offset = num2 * 8;
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
					info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
					goto IL_0010;
				}
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void TappableManager::RPC_SendOnRelease(Fusion.NetworkRunner,System.Int32,Fusion.RpcInfo)", num);
			}
			return;
		}
		IL_0010:
		TappableManager.gManager.SendOnReleaseShared(key, new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06003DBA RID: 15802 RVA: 0x0014EB8C File Offset: 0x0014CD8C
	public void SendOnReleaseShared(int key, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "SendOnReleaseShared");
		if (key == 0)
		{
			return;
		}
		for (int i = 0; i < this.tappables.Count; i++)
		{
			Tappable tappable = this.tappables[i];
			if (tappable.tappableId == key)
			{
				tappable.OnReleaseLocal(Time.time, info);
			}
		}
	}

	// Token: 0x06003DBD RID: 15805 RVA: 0x0014EC0C File Offset: 0x0014CE0C
	[NetworkRpcStaticWeavedInvoker("System.Void TappableManager::RPC_SendOnTap(Fusion.NetworkRunner,System.Int32,System.Single,Fusion.RpcInfo)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_SendOnTap@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		int num2 = *(int*)(ptr + num);
		num += 4;
		int num3 = num2;
		float num4 = *(float*)(ptr + num);
		num += 4;
		float num5 = num4;
		RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
		NetworkBehaviourUtils.InvokeRpc = true;
		TappableManager.RPC_SendOnTap(runner, num3, num5, rpcInfo);
	}

	// Token: 0x06003DBE RID: 15806 RVA: 0x0014EC88 File Offset: 0x0014CE88
	[NetworkRpcStaticWeavedInvoker("System.Void TappableManager::RPC_SendOnGrab(Fusion.NetworkRunner,System.Int32,Fusion.RpcInfo)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_SendOnGrab@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		int num2 = *(int*)(ptr + num);
		num += 4;
		int num3 = num2;
		RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
		NetworkBehaviourUtils.InvokeRpc = true;
		TappableManager.RPC_SendOnGrab(runner, num3, rpcInfo);
	}

	// Token: 0x06003DBF RID: 15807 RVA: 0x0014ECE8 File Offset: 0x0014CEE8
	[NetworkRpcStaticWeavedInvoker("System.Void TappableManager::RPC_SendOnRelease(Fusion.NetworkRunner,System.Int32,Fusion.RpcInfo)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_SendOnRelease@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		int num2 = *(int*)(ptr + num);
		num += 4;
		int num3 = num2;
		RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
		NetworkBehaviourUtils.InvokeRpc = true;
		TappableManager.RPC_SendOnRelease(runner, num3, rpcInfo);
	}

	// Token: 0x04004E68 RID: 20072
	private static TappableManager gManager;

	// Token: 0x04004E69 RID: 20073
	[SerializeField]
	private List<Tappable> tappables = new List<Tappable>();

	// Token: 0x04004E6A RID: 20074
	private HashSet<int> idSet = new HashSet<int>();

	// Token: 0x04004E6B RID: 20075
	private static HashSet<Tappable> gRegistry = new HashSet<Tappable>();
}
