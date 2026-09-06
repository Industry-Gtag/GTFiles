using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using GorillaExtensions;
using GorillaGameModes;
using GT_CustomMapSupportRuntime;
using Photon.Pun;
using Photon.Realtime;
using Unity.Collections;
using UnityEngine;

// Token: 0x02000C9E RID: 3230
public class LuauVm : MonoBehaviourPunCallbacks, IOnEventCallback
{
	// Token: 0x06004FBD RID: 20413 RVA: 0x001A79E4 File Offset: 0x001A5BE4
	private void LateUpdate()
	{
		foreach (LuauScriptRunner luauScriptRunner in LuauScriptRunner.ScriptRunners)
		{
			if (!luauScriptRunner.Tick(Time.deltaTime))
			{
				LuauHud.Instance.LuauLog(luauScriptRunner.ScriptName + " errored out");
				LuauScriptRunner.ScriptRunners.Remove(luauScriptRunner);
				break;
			}
		}
	}

	// Token: 0x06004FBE RID: 20414 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Start()
	{
	}

	// Token: 0x06004FBF RID: 20415 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Awake()
	{
	}

	// Token: 0x06004FC0 RID: 20416 RVA: 0x001A7A64 File Offset: 0x001A5C64
	public void OnEvent(EventData eventData)
	{
		if (eventData.Code != 180)
		{
			return;
		}
		if (Utils.PlayerInRoom(eventData.Sender))
		{
			object[] array = eventData.CustomData as object[];
			if (array != null && array.Length <= 20 && array.Length >= 1)
			{
				float num = 0f;
				LuauVm.callTimers.TryGetValue(eventData.Sender, out num);
				if (num < Time.time - 1f)
				{
					num = Time.time - 1f;
				}
				num += 1f / LuauVm.callCount;
				LuauVm.callTimers[eventData.Sender] = num;
				if (num > Time.time)
				{
					return;
				}
				string text = array[0] as string;
				if (text == null || text.Length > 30)
				{
					return;
				}
				for (int i = 1; i < array.Length; i++)
				{
					object obj = array[i];
					if (obj != null && !(obj is double) && !(obj is bool) && !(obj is Vector3) && !(obj is Quaternion) && !(obj is Player))
					{
						return;
					}
				}
				object[] array2 = new object[]
				{
					NetworkSystem.Instance.GetPlayer(eventData.Sender),
					array
				};
				LuauVm.eventQueue.Enqueue(array2);
				if (LuauVm.eventQueue.Count > 500)
				{
					LuauVm.eventQueue.Dequeue();
				}
				return;
			}
		}
	}

	// Token: 0x06004FC1 RID: 20417 RVA: 0x001A7BAC File Offset: 0x001A5DAC
	public unsafe static int SendEvent(lua_State* L, object[] args, bool useTable = true)
	{
		try
		{
			NetPlayer netPlayer = null;
			if (args[0] is NetPlayer)
			{
				netPlayer = (NetPlayer)args[0];
				args = (object[])args[1];
			}
			if (GorillaGameManager.instance.GameType() != GameModeType.Custom)
			{
				return -1;
			}
			Luau.lua_getfield(L, -10002, "onEvent");
			if (Luau.lua_type(L, -1) != 7)
			{
				Luau.lua_pop(L, 1);
				return 0;
			}
			string text = args[0] as string;
			if (text == null)
			{
				Luau.lua_pop(L, 1);
				return 0;
			}
			if (string.IsNullOrEmpty(text))
			{
				Luau.lua_pop(L, 1);
				return 0;
			}
			if (text.Length > 30)
			{
				Luau.lua_pop(L, 1);
				return 0;
			}
			Luau.lua_pushstring(L, (string)args[0]);
			if (useTable)
			{
				Luau.lua_createtable(L, args.Length, 0);
			}
			int i = 1;
			while (i < args.Length)
			{
				object obj = args[i];
				if (obj.IsType<double>())
				{
					if (double.IsFinite((double)obj))
					{
						Luau.lua_pushnumber(L, (double)obj);
						goto IL_03A7;
					}
				}
				else
				{
					if (obj.IsType<bool>())
					{
						Luau.lua_pushboolean(L, ((bool)obj) ? 1 : 0);
						goto IL_03A7;
					}
					if (obj.IsType<Vector3>())
					{
						Vector3 vector = (Vector3)obj;
						vector.ClampMagnitudeSafe(10000000f);
						*Luau.lua_class_push<Vector3>(L, "Vec3") = vector;
						goto IL_03A7;
					}
					if (obj.IsType<Quaternion>())
					{
						Quaternion quaternion = (Quaternion)obj;
						if (float.IsFinite(quaternion.x) && float.IsFinite(quaternion.y) && float.IsFinite(quaternion.z) && float.IsFinite(quaternion.w))
						{
							*Luau.lua_class_push<Quaternion>(L, "Quat") = quaternion;
							goto IL_03A7;
						}
					}
					else if (obj.IsType<Player>())
					{
						int actorNumber = ((Player)obj).ActorNumber;
						IntPtr intPtr;
						if (Bindings.LuauPlayerList.TryGetValue(actorNumber, out intPtr))
						{
							Luau.lua_class_push(L, "Player", intPtr);
							goto IL_03A7;
						}
						NetPlayer netPlayer2 = (NetPlayer)obj;
						if (netPlayer2 == null)
						{
							Luau.lua_pushnil(L);
							goto IL_03A7;
						}
						Bindings.LuauPlayer* ptr = Luau.lua_class_push<Bindings.LuauPlayer>(L);
						ptr->PlayerID = netPlayer2.ActorNumber;
						ptr->ScaleMultiplier = 1f;
						ptr->PlayerName = netPlayer2.SanitizedNickName;
						ptr->PlayerMaterial = 0;
						ptr->IsMasterClient = netPlayer2.IsMasterClient;
						RigContainer rigContainer;
						VRRigCache.Instance.TryGetVrrig(netPlayer2, out rigContainer);
						VRRig rig = rigContainer.Rig;
						Bindings.LuauVRRigList[netPlayer2.ActorNumber] = rig;
						Bindings.PlayerFunctions.UpdatePlayer(L, rig, ptr);
						Bindings.LuauPlayerList[netPlayer2.ActorNumber] = (IntPtr)((void*)ptr);
						goto IL_03A7;
					}
					else
					{
						if (!obj.IsType<Bindings.LuauAIAgent>())
						{
							Luau.lua_pushnil(L);
							goto IL_03A7;
						}
						int entityID = ((Bindings.LuauAIAgent)obj).EntityID;
						IntPtr intPtr2;
						if (Bindings.LuauAIAgentList.TryGetValue(entityID, out intPtr2))
						{
							Luau.lua_class_push(L, "AIAgent", intPtr2);
							goto IL_03A7;
						}
						bool flag = false;
						if (Bindings.LuauAIAgentList.Count + Bindings.LuauGrabbablesList.Count == Constants.aiAgentLimit)
						{
							Debug.Log("[LuauVM::OnEvent] Custom Map AI Agent limit has already been reached!");
						}
						else
						{
							GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
							if (entityManager.IsNotNull())
							{
								GameEntityId entityIdFromNetId = entityManager.GetEntityIdFromNetId(entityID);
								GameEntity gameEntity = entityManager.GetGameEntity(entityIdFromNetId);
								if (gameEntity.IsNotNull() && gameEntity.gameObject.IsNotNull() && gameEntity.gameObject.GetComponent<GameAgent>() != null)
								{
									Bindings.LuauAIAgent* ptr2 = Luau.lua_class_push<Bindings.LuauAIAgent>(L);
									Bindings.AIAgentFunctions.UpdateEntity(gameEntity, ptr2);
									Bindings.LuauAIAgentList[entityID] = (IntPtr)((void*)ptr2);
									flag = true;
								}
							}
						}
						if (!flag)
						{
							Luau.lua_pushnil(L);
							goto IL_03A7;
						}
						goto IL_03A7;
					}
				}
				IL_03B3:
				i++;
				continue;
				IL_03A7:
				if (useTable)
				{
					Luau.lua_rawseti(L, -2, i);
					goto IL_03B3;
				}
				goto IL_03B3;
			}
			if (netPlayer != null)
			{
				int actorNumber2 = netPlayer.ActorNumber;
				IntPtr intPtr3;
				if (Bindings.LuauPlayerList.TryGetValue(actorNumber2, out intPtr3))
				{
					Luau.lua_class_push(L, "Player", intPtr3);
				}
				else
				{
					NetPlayer netPlayer3 = netPlayer;
					if (netPlayer3 == null)
					{
						Luau.lua_pushnil(L);
					}
					else
					{
						Bindings.LuauPlayer* ptr3 = Luau.lua_class_push<Bindings.LuauPlayer>(L);
						ptr3->PlayerID = netPlayer3.ActorNumber;
						ptr3->ScaleMultiplier = 1f;
						ptr3->PlayerName = netPlayer3.SanitizedNickName;
						ptr3->PlayerMaterial = 0;
						ptr3->IsMasterClient = netPlayer3.IsMasterClient;
						RigContainer rigContainer2;
						VRRigCache.Instance.TryGetVrrig(netPlayer3, out rigContainer2);
						VRRig rig2 = rigContainer2.Rig;
						Bindings.LuauVRRigList[netPlayer3.ActorNumber] = rig2;
						Bindings.PlayerFunctions.UpdatePlayer(L, rig2, ptr3);
						Bindings.LuauPlayerList[netPlayer3.ActorNumber] = (IntPtr)((void*)ptr3);
					}
				}
				return Luau.lua_pcall(L, 3, 0, 0);
			}
			return Luau.lua_pcall(L, 2, 0, 0);
		}
		catch (Exception)
		{
		}
		return 0;
	}

	// Token: 0x06004FC2 RID: 20418 RVA: 0x001A80A0 File Offset: 0x001A62A0
	public static void ProcessEvents()
	{
		while (LuauVm.eventQueue.Count > 0)
		{
			object[] array = LuauVm.eventQueue.Dequeue();
			foreach (LuauScriptRunner luauScriptRunner in LuauScriptRunner.ScriptRunners)
			{
				if (luauScriptRunner.ShouldTick)
				{
					int num = LuauVm.SendEvent(luauScriptRunner.L, array, true);
					luauScriptRunner.ShouldTick = !LuauScriptRunner.ErrorCheck(luauScriptRunner.L, num);
				}
			}
		}
		while (LuauVm.localEventQueue.Count > 0)
		{
			object[] array2 = LuauVm.localEventQueue.Dequeue();
			foreach (LuauScriptRunner luauScriptRunner2 in LuauScriptRunner.ScriptRunners)
			{
				if (luauScriptRunner2.ShouldTick)
				{
					int num2 = LuauVm.SendEvent(luauScriptRunner2.L, array2, false);
					luauScriptRunner2.ShouldTick = !LuauScriptRunner.ErrorCheck(luauScriptRunner2.L, num2);
				}
			}
		}
		while (LuauVm.touchEventsQueue.Count > 0)
		{
			GameObject gameObject = LuauVm.touchEventsQueue.Dequeue();
			foreach (LuauScriptRunner luauScriptRunner3 in LuauScriptRunner.ScriptRunners)
			{
				int num3;
				if (luauScriptRunner3.ShouldTick && Bindings.LuauTriggerCallbacks.TryGetValue(gameObject, out num3))
				{
					Luau.lua_getref(luauScriptRunner3.L, num3);
					if (Luau.lua_type(luauScriptRunner3.L, -1) == 7)
					{
						int num4 = Luau.lua_pcall(luauScriptRunner3.L, 0, 0, 0);
						luauScriptRunner3.ShouldTick = !LuauScriptRunner.ErrorCheck(luauScriptRunner3.L, num4);
					}
				}
			}
		}
	}

	// Token: 0x06004FC3 RID: 20419 RVA: 0x001A8274 File Offset: 0x001A6474
	protected override void Finalize()
	{
		try
		{
			foreach (GCHandle gchandle in LuauVm.Handles)
			{
				gchandle.Free();
			}
			if (BurstClassInfo.ClassList.InfoFields.Data.IsCreated)
			{
				foreach (KVPair<int, BurstClassInfo.ClassInfo> kvpair in BurstClassInfo.ClassList.InfoFields.Data)
				{
					if (kvpair.Value.FieldList.IsCreated)
					{
						kvpair.Value.FieldList.Dispose();
					}
				}
				BurstClassInfo.ClassList.InfoFields.Data.Dispose();
			}
		}
		catch (ObjectDisposedException ex)
		{
			Debug.Log(ex);
		}
		finally
		{
			base.Finalize();
		}
	}

	// Token: 0x040061F1 RID: 25073
	public static List<object> ClassBuilders = new List<object>();

	// Token: 0x040061F2 RID: 25074
	public static List<GCHandle> Handles = new List<GCHandle>();

	// Token: 0x040061F3 RID: 25075
	private static Dictionary<int, float> callTimers = new Dictionary<int, float>();

	// Token: 0x040061F4 RID: 25076
	private static float callCount = 25f;

	// Token: 0x040061F5 RID: 25077
	public static Queue<object[]> eventQueue = new Queue<object[]>();

	// Token: 0x040061F6 RID: 25078
	public static Queue<object[]> localEventQueue = new Queue<object[]>();

	// Token: 0x040061F7 RID: 25079
	public static Queue<GameObject> touchEventsQueue = new Queue<GameObject>();
}
