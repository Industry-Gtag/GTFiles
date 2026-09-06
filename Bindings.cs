using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000C1D RID: 3101
[BurstCompile]
public static class Bindings
{
	// Token: 0x06004DA5 RID: 19877 RVA: 0x0019E81C File Offset: 0x0019CA1C
	public unsafe static void GameObjectBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.LuauGameObject>("GameObject").AddField("position", "Position").AddField("rotation", "Rotation").AddField("scale", "Scale")
			.AddStaticFunction("findGameObject", new lua_CFunction(Bindings.GameObjectFunctions.FindGameObject))
			.AddFunction("setCollision", new lua_CFunction(Bindings.GameObjectFunctions.SetCollision))
			.AddFunction("setVisibility", new lua_CFunction(Bindings.GameObjectFunctions.SetVisibility))
			.AddFunction("setActive", new lua_CFunction(Bindings.GameObjectFunctions.SetActive))
			.AddFunction("setText", new lua_CFunction(Bindings.GameObjectFunctions.SetText))
			.AddFunction("onTouched", new lua_CFunction(Bindings.GameObjectFunctions.OnTouched))
			.AddFunction("setVelocity", new lua_CFunction(Bindings.GameObjectFunctions.SetVelocity))
			.AddFunction("getVelocity", new lua_CFunction(Bindings.GameObjectFunctions.GetVelocity))
			.AddFunction("setColor", new lua_CFunction(Bindings.GameObjectFunctions.SetColor))
			.AddFunction("findChild", new lua_CFunction(Bindings.GameObjectFunctions.FindChildGameObject))
			.AddFunction("clone", new lua_CFunction(Bindings.GameObjectFunctions.CloneGameObject))
			.AddFunction("destroy", new lua_CFunction(Bindings.GameObjectFunctions.DestroyGameObject))
			.AddFunction("findComponent", new lua_CFunction(Bindings.GameObjectFunctions.FindComponent))
			.AddFunction("equals", new lua_CFunction(Bindings.GameObjectFunctions.Equals))
			.Build(L, true));
	}

	// Token: 0x06004DA6 RID: 19878 RVA: 0x0019E9A8 File Offset: 0x0019CBA8
	public unsafe static void GorillaLocomotionSettingsBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.GorillaLocomotionSettings>("PSettings").AddField("velocityLimit", null).AddField("slideVelocityLimit", null).AddField("maxJumpSpeed", null)
			.AddField("jumpMultiplier", null)
			.Build(L, false));
		Bindings.LocomotionSettings = Luau.lua_class_push<Bindings.GorillaLocomotionSettings>(L);
		Bindings.LocomotionSettings->velocityLimit = GTPlayer.Instance.velocityLimit;
		Bindings.LocomotionSettings->slideVelocityLimit = GTPlayer.Instance.slideVelocityLimit;
		Bindings.LocomotionSettings->maxJumpSpeed = 6.5f;
		Bindings.LocomotionSettings->jumpMultiplier = 1.1f;
		Luau.lua_setglobal(L, "PlayerSettings");
	}

	// Token: 0x06004DA7 RID: 19879 RVA: 0x0019EA5C File Offset: 0x0019CC5C
	public unsafe static void PlayerInputBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.PlayerInput>("PInput").AddField("leftXAxis", null).AddField("rightXAxis", null).AddField("leftYAxis", null)
			.AddField("rightYAxis", null)
			.AddField("leftTrigger", null)
			.AddField("rightTrigger", null)
			.AddField("leftGrip", null)
			.AddField("rightGrip", null)
			.AddField("leftPrimaryButton", null)
			.AddField("rightPrimaryButton", null)
			.AddField("leftSecondaryButton", null)
			.AddField("rightSecondaryButton", null)
			.Build(L, false));
		Bindings.LocalPlayerInput = Luau.lua_class_push<Bindings.PlayerInput>(L);
		Bindings.UpdateInputs();
		Luau.lua_setglobal(L, "PlayerInput");
	}

	// Token: 0x06004DA8 RID: 19880 RVA: 0x0019EB24 File Offset: 0x0019CD24
	public unsafe static void UpdateInputs()
	{
		if (Bindings.LocalPlayerInput != null)
		{
			Bindings.LocalPlayerInput->leftPrimaryButton = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
			Bindings.LocalPlayerInput->rightPrimaryButton = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
			Bindings.LocalPlayerInput->leftSecondaryButton = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
			Bindings.LocalPlayerInput->rightSecondaryButton = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
			Bindings.LocalPlayerInput->leftGrip = ControllerInputPoller.GripFloat(XRNode.LeftHand);
			Bindings.LocalPlayerInput->rightGrip = ControllerInputPoller.GripFloat(XRNode.RightHand);
			Bindings.LocalPlayerInput->leftTrigger = ControllerInputPoller.TriggerFloat(XRNode.LeftHand);
			Bindings.LocalPlayerInput->rightTrigger = ControllerInputPoller.TriggerFloat(XRNode.RightHand);
			Vector2 vector = ControllerInputPoller.Primary2DAxis(XRNode.LeftHand);
			Vector2 vector2 = ControllerInputPoller.Primary2DAxis(XRNode.RightHand);
			Bindings.LocalPlayerInput->leftXAxis = vector.x;
			Bindings.LocalPlayerInput->leftYAxis = vector.y;
			Bindings.LocalPlayerInput->rightXAxis = vector2.x;
			Bindings.LocalPlayerInput->rightYAxis = vector2.y;
		}
	}

	// Token: 0x06004DA9 RID: 19881 RVA: 0x0019EC0C File Offset: 0x0019CE0C
	public unsafe static void Vec3Builder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Vector3>("Vec3").AddField("x", null).AddField("y", null).AddField("z", null)
			.AddStaticFunction("new", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.New)))
			.AddFunction("__add", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Add)))
			.AddFunction("__sub", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Sub)))
			.AddFunction("__mul", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Mul)))
			.AddFunction("__div", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Div)))
			.AddFunction("__unm", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Unm)))
			.AddFunction("__eq", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Eq)))
			.AddFunction("__tostring", new lua_CFunction(Bindings.Vec3Functions.ToString))
			.AddFunction("toString", new lua_CFunction(Bindings.Vec3Functions.ToString))
			.AddFunction("dot", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Dot)))
			.AddFunction("cross", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Cross)))
			.AddFunction("projectOnTo", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Project)))
			.AddFunction("length", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Length)))
			.AddFunction("normalize", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Normalize)))
			.AddFunction("getSafeNormal", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.SafeNormal)))
			.AddStaticFunction("rotate", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Rotate)))
			.AddFunction("rotate", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Rotate)))
			.AddStaticFunction("distance", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Distance)))
			.AddFunction("distance", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Distance)))
			.AddStaticFunction("lerp", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Lerp)))
			.AddFunction("lerp", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Lerp)))
			.AddProperty("zeroVector", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.ZeroVector)))
			.AddProperty("oneVector", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.OneVector)))
			.AddStaticFunction("nearlyEqual", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.NearlyEqual)))
			.Build(L, true));
	}

	// Token: 0x06004DAA RID: 19882 RVA: 0x0019EED4 File Offset: 0x0019D0D4
	public unsafe static void QuatBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Quaternion>("Quat").AddField("x", null).AddField("y", null).AddField("z", null)
			.AddField("w", null)
			.AddStaticFunction("new", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.New)))
			.AddFunction("__mul", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.Mul)))
			.AddFunction("__eq", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.Eq)))
			.AddFunction("__tostring", new lua_CFunction(Bindings.QuatFunctions.ToString))
			.AddFunction("toString", new lua_CFunction(Bindings.QuatFunctions.ToString))
			.AddStaticFunction("fromEuler", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.FromEuler)))
			.AddStaticFunction("fromDirection", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.FromDirection)))
			.AddFunction("getUpVector", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.GetUpVector)))
			.AddFunction("euler", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.Euler)))
			.Build(L, true));
	}

	// Token: 0x06004DAB RID: 19883 RVA: 0x0019F014 File Offset: 0x0019D214
	public unsafe static void PlayerBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.LuauPlayer>("Player").AddField("playerID", "PlayerID").AddField("playerName", "PlayerName").AddField("playerMaterial", "PlayerMaterial")
			.AddField("isMasterClient", "IsMasterClient")
			.AddField("bodyPosition", "BodyPosition")
			.AddField("velocity", "Velocity")
			.AddField("isPCVR", "IsPCVR")
			.AddField("leftHandPosition", "LeftHandPosition")
			.AddField("rightHandPosition", "RightHandPosition")
			.AddField("headRotation", "HeadRotation")
			.AddField("leftHandRotation", "LeftHandRotation")
			.AddField("rightHandRotation", "RightHandRotation")
			.AddField("isInVStump", "IsInVStump")
			.AddField("isEntityAuthority", "IsEntityAuthority")
			.AddField("scaleMultiplier", "ScaleMultiplier")
			.AddStaticFunction("getPlayerByID", new lua_CFunction(Bindings.PlayerFunctions.GetPlayerByID))
			.Build(L, true));
	}

	// Token: 0x06004DAC RID: 19884 RVA: 0x0019F134 File Offset: 0x0019D334
	public unsafe static void AIAgentBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.LuauAIAgent>("AIAgent").AddField("entityID", "EntityID").AddField("agentPosition", "EntityPosition").AddField("agentRotation", "EntityRotation")
			.AddFunction("__tostring", new lua_CFunction(Bindings.AIAgentFunctions.ToString))
			.AddFunction("toString", new lua_CFunction(Bindings.AIAgentFunctions.ToString))
			.AddFunction("setDestination", new lua_CFunction(Bindings.AIAgentFunctions.SetDestination))
			.AddFunction("destroyAgent", new lua_CFunction(Bindings.AIAgentFunctions.DestroyEntity))
			.AddFunction("playAgentAnimation", new lua_CFunction(Bindings.AIAgentFunctions.PlayAgentAnimation))
			.AddFunction("getTargetPlayer", new lua_CFunction(Bindings.AIAgentFunctions.GetTarget))
			.AddFunction("setTargetPlayer", new lua_CFunction(Bindings.AIAgentFunctions.SetTarget))
			.AddStaticFunction("findPrePlacedAIAgentByID", new lua_CFunction(Bindings.AIAgentFunctions.FindPrePlacedAIAgentByID))
			.AddStaticFunction("getAIAgentByEntityID", new lua_CFunction(Bindings.AIAgentFunctions.GetAIAgentByEntityID))
			.AddStaticFunction("spawnAIAgent", new lua_CFunction(Bindings.AIAgentFunctions.SpawnAIAgent))
			.Build(L, true));
	}

	// Token: 0x06004DAD RID: 19885 RVA: 0x0019F268 File Offset: 0x0019D468
	public unsafe static void GrabbableEntityBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.LuauGrabbableEntity>("GrabbableEntity").AddField("entityID", "EntityID").AddField("entityPosition", "EntityPosition").AddField("entityRotation", "EntityRotation")
			.AddFunction("__tostring", new lua_CFunction(Bindings.GrabbableEntityFunctions.ToString))
			.AddFunction("toString", new lua_CFunction(Bindings.GrabbableEntityFunctions.ToString))
			.AddFunction("destroyGrabbable", new lua_CFunction(Bindings.GrabbableEntityFunctions.DestroyEntity))
			.AddStaticFunction("findPrePlacedGrabbableEntityByID", new lua_CFunction(Bindings.GrabbableEntityFunctions.FindPrePlacedGrabbableEntityByID))
			.AddStaticFunction("getGrabbableEntityByEntityID", new lua_CFunction(Bindings.GrabbableEntityFunctions.GetGrabbableEntityByEntityID))
			.AddStaticFunction("getHoldingActorNumberByEntityID", new lua_CFunction(Bindings.GrabbableEntityFunctions.GetHoldingActorNumberByEntityID))
			.AddStaticFunction("getHoldingActorNumberByLuauID", new lua_CFunction(Bindings.GrabbableEntityFunctions.GetHoldingActorNumberByLuauID))
			.AddStaticFunction("spawnGrabbableEntity", new lua_CFunction(Bindings.GrabbableEntityFunctions.SpawnGrabbableEntity))
			.Build(L, true));
	}

	// Token: 0x06004DAE RID: 19886 RVA: 0x0019F370 File Offset: 0x0019D570
	public unsafe static void InventoryItemBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.MInventoryItem>("InventoryItem").AddField("name", "Name").AddField("quantity", "Quantity").AddField("inGameId", "InGameId")
			.AddField("displayName", "DisplayName")
			.AddField("displayDescription", "DisplayDescription")
			.AddField("id", "ID")
			.Build(L, true));
	}

	// Token: 0x06004DAF RID: 19887 RVA: 0x0019F3F4 File Offset: 0x0019D5F4
	public unsafe static void OnlineErrorBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.MOnlineError>("OnlineError").AddField("name", "Name").AddField("message", "Message").AddField("errorCode", "ErrorCode")
			.AddField("httpCode", "HttpCode")
			.Build(L, true));
	}

	// Token: 0x06004DB0 RID: 19888 RVA: 0x0019F45C File Offset: 0x0019D65C
	public unsafe static void OnlineFunctionsBuilder(lua_State* L)
	{
		Luau.lua_createtable(L, 0, 4);
		Luau.lua_pushcfunction(L, new lua_CFunction(Bindings.OnlineFunctions.GetLocalPlayerInventory), "getLocalPlayerInventory");
		Luau.lua_setfield(L, -2, "getLocalPlayerInventory");
		Luau.lua_pushcfunction(L, new lua_CFunction(Bindings.OnlineFunctions.GetInventoryForPlayer), "getInventoryForPlayer");
		Luau.lua_setfield(L, -2, "getInventoryForPlayer");
		Luau.lua_pushcfunction(L, new lua_CFunction(Bindings.OnlineFunctions.LoadStorefront), "getStore");
		Luau.lua_setfield(L, -2, "getStore");
		Luau.lua_pushcfunction(L, new lua_CFunction(Bindings.OnlineFunctions.TryPurchase), "tryPurchase");
		Luau.lua_setfield(L, -2, "tryPurchase");
		Luau.lua_pushcfunction(L, new lua_CFunction(Bindings.OnlineFunctions.TryConsume), "tryConsume");
		Luau.lua_setfield(L, -2, "tryConsume");
		Luau.lua_setglobal(L, "OnlineFunctions");
	}

	// Token: 0x06004DB1 RID: 19889 RVA: 0x0019F530 File Offset: 0x0019D730
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int LuaStartVibration(lua_State* L)
	{
		bool flag = Luau.lua_toboolean(L, 1) == 1;
		float num = (float)Luau.luaL_checknumber(L, 2);
		float num2 = (float)Luau.luaL_checknumber(L, 3);
		GorillaTagger.Instance.StartVibration(flag, num, num2);
		return 0;
	}

	// Token: 0x06004DB2 RID: 19890 RVA: 0x0019F568 File Offset: 0x0019D768
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int LuaPlaySound(lua_State* L)
	{
		int num = (int)Luau.luaL_checknumber(L, 1);
		Vector3 vector = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
		float num2 = (float)Luau.luaL_checknumber(L, 3);
		if (num < 0 || num >= VRRig.LocalRig.clipToPlay.Length)
		{
			return 0;
		}
		AudioSource.PlayClipAtPoint(VRRig.LocalRig.clipToPlay[num], vector, num2);
		return 0;
	}

	// Token: 0x06004DB3 RID: 19891 RVA: 0x0019F5C8 File Offset: 0x0019D7C8
	public unsafe static void RoomStateBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.LuauRoomState>("RState").AddField("isQuest", "IsQuest").AddField("fps", "FPS").AddField("isPrivate", "IsPrivate")
			.AddField("code", "RoomCode")
			.Build(L, false));
		Bindings.RoomState = Luau.lua_class_push<Bindings.LuauRoomState>(L);
		Bindings.UpdateRoomState();
		Bindings.RoomState->IsQuest = false;
		Bindings.RoomState->IsPrivate = !PhotonNetwork.CurrentRoom.IsVisible;
		Bindings.RoomState->RoomCode = PhotonNetwork.CurrentRoom.Name;
		Luau.lua_setglobal(L, "Room");
	}

	// Token: 0x06004DB4 RID: 19892 RVA: 0x0019F683 File Offset: 0x0019D883
	public unsafe static void UpdateRoomState()
	{
		Bindings.RoomState->FPS = 1f / Time.smoothDeltaTime;
	}

	// Token: 0x040060F8 RID: 24824
	public static Dictionary<GameObject, IntPtr> LuauGameObjectList = new Dictionary<GameObject, IntPtr>();

	// Token: 0x040060F9 RID: 24825
	public static List<KeyValuePair<GameObject, IntPtr>> LuauGameObjectDepthList = new List<KeyValuePair<GameObject, IntPtr>>();

	// Token: 0x040060FA RID: 24826
	public static Dictionary<IntPtr, GameObject> LuauGameObjectListReverse = new Dictionary<IntPtr, GameObject>();

	// Token: 0x040060FB RID: 24827
	public static Dictionary<GameObject, Bindings.LuauGameObjectInitialState> LuauGameObjectStates = new Dictionary<GameObject, Bindings.LuauGameObjectInitialState>();

	// Token: 0x040060FC RID: 24828
	public static Dictionary<GameObject, int> LuauTriggerCallbacks = new Dictionary<GameObject, int>();

	// Token: 0x040060FD RID: 24829
	public static Dictionary<int, IntPtr> LuauPlayerList = new Dictionary<int, IntPtr>();

	// Token: 0x040060FE RID: 24830
	public static Dictionary<int, VRRig> LuauVRRigList = new Dictionary<int, VRRig>();

	// Token: 0x040060FF RID: 24831
	public unsafe static Bindings.GorillaLocomotionSettings* LocomotionSettings;

	// Token: 0x04006100 RID: 24832
	public unsafe static Bindings.PlayerInput* LocalPlayerInput;

	// Token: 0x04006101 RID: 24833
	public unsafe static Bindings.LuauRoomState* RoomState;

	// Token: 0x04006102 RID: 24834
	public static Dictionary<int, IntPtr> LuauAIAgentList = new Dictionary<int, IntPtr>();

	// Token: 0x04006103 RID: 24835
	public static Dictionary<int, IntPtr> LuauGrabbablesList = new Dictionary<int, IntPtr>();

	// Token: 0x02000C1E RID: 3102
	public static class LuaEmit
	{
		// Token: 0x06004DB6 RID: 19894 RVA: 0x0019F704 File Offset: 0x0019D904
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Emit(lua_State* L)
		{
			if (Bindings.LuaEmit.callTime < Time.time - 1f)
			{
				Bindings.LuaEmit.callTime = Time.time - 1f;
			}
			Bindings.LuaEmit.callTime += 1f / Bindings.LuaEmit.callCount;
			if (Bindings.LuaEmit.callTime > Time.time)
			{
				LuauHud.Instance.LuauLog("Emit rate limit reached, event not sent");
				return 0;
			}
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions
			{
				Receivers = ReceiverGroup.Others
			};
			if (Luau.lua_type(L, 2) != 6)
			{
				Luau.luaL_errorL(L, "Argument 2 must be a table", Array.Empty<string>());
				return 0;
			}
			Luau.lua_pushnil(L);
			int num = 0;
			List<object> list = new List<object>();
			list.Add(Marshal.PtrToStringAnsi((IntPtr)((void*)Luau.luaL_checkstring(L, 1))));
			while (Luau.lua_next(L, 2) != 0 && num++ < 10)
			{
				Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, -1);
				if (lua_Types <= Luau.lua_Types.LUA_TNUMBER)
				{
					if (lua_Types == Luau.lua_Types.LUA_TBOOLEAN)
					{
						list.Add(Luau.lua_toboolean(L, -1) == 1);
						Luau.lua_pop(L, 1);
						continue;
					}
					if (lua_Types == Luau.lua_Types.LUA_TNUMBER)
					{
						list.Add(Luau.luaL_checknumber(L, -1));
						Luau.lua_pop(L, 1);
						continue;
					}
				}
				else if (lua_Types == Luau.lua_Types.LUA_TTABLE || lua_Types == Luau.lua_Types.LUA_TUSERDATA)
				{
					Luau.luaL_getmetafield(L, -1, "metahash");
					BurstClassInfo.ClassInfo classInfo;
					if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
					{
						FixedString64Bytes fixedString64Bytes = "\"Internal Class Info Error No Metatable Found\"";
						Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString64Bytes>(ref fixedString64Bytes) + 2));
						return 0;
					}
					Luau.lua_pop(L, 1);
					FixedString32Bytes fixedString32Bytes = "Vec3";
					if ((in classInfo.Name) == (in fixedString32Bytes))
					{
						list.Add(*Luau.lua_class_get<Vector3>(L, -1));
						Luau.lua_pop(L, 1);
						continue;
					}
					fixedString32Bytes = "Quat";
					if ((in classInfo.Name) == (in fixedString32Bytes))
					{
						list.Add(*Luau.lua_class_get<Quaternion>(L, -1));
						Luau.lua_pop(L, 1);
						continue;
					}
					fixedString32Bytes = "Player";
					if ((in classInfo.Name) == (in fixedString32Bytes))
					{
						int playerID = Luau.lua_class_get<Bindings.LuauPlayer>(L, -1)->PlayerID;
						NetPlayer netPlayer = null;
						foreach (NetPlayer netPlayer2 in RoomSystem.PlayersInRoom)
						{
							if (netPlayer2.ActorNumber == playerID)
							{
								netPlayer = netPlayer2;
							}
						}
						if (netPlayer == null)
						{
							list.Add(null);
						}
						else
						{
							list.Add(netPlayer.GetPlayerRef());
						}
						Luau.lua_pop(L, 1);
						continue;
					}
					FixedString32Bytes fixedString32Bytes2 = "\"Unknown Type in table\"";
					Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
					continue;
				}
				FixedString32Bytes fixedString32Bytes3 = "\"Unknown Type in table\"";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2));
				return 0;
			}
			if (PhotonNetwork.InRoom)
			{
				PhotonNetwork.RaiseEvent(180, list.ToArray(), raiseEventOptions, SendOptions.SendReliable);
			}
			return 0;
		}

		// Token: 0x04006104 RID: 24836
		private static float callTime = 0f;

		// Token: 0x04006105 RID: 24837
		private static float callCount = 20f;
	}

	// Token: 0x02000C1F RID: 3103
	[BurstCompile]
	public struct LuauGameObject
	{
		// Token: 0x04006106 RID: 24838
		public Vector3 Position;

		// Token: 0x04006107 RID: 24839
		public Quaternion Rotation;

		// Token: 0x04006108 RID: 24840
		public Vector3 Scale;
	}

	// Token: 0x02000C20 RID: 3104
	[BurstCompile]
	public struct LuauGameObjectInitialState
	{
		// Token: 0x04006109 RID: 24841
		public Vector3 Position;

		// Token: 0x0400610A RID: 24842
		public Quaternion Rotation;

		// Token: 0x0400610B RID: 24843
		public Vector3 Scale;

		// Token: 0x0400610C RID: 24844
		public bool Visible;

		// Token: 0x0400610D RID: 24845
		public bool Collidable;

		// Token: 0x0400610E RID: 24846
		public bool Created;
	}

	// Token: 0x02000C21 RID: 3105
	[BurstCompile]
	public static class GameObjectFunctions
	{
		// Token: 0x06004DB8 RID: 19896 RVA: 0x0019FA10 File Offset: 0x0019DC10
		public static int GetDepth(GameObject gameObject)
		{
			int num = 0;
			Transform transform = gameObject.transform;
			while (transform.parent != null)
			{
				num++;
				transform = transform.parent;
			}
			return num;
		}

		// Token: 0x06004DB9 RID: 19897 RVA: 0x0019FA42 File Offset: 0x0019DC42
		public static void UpdateDepthList()
		{
			Bindings.LuauGameObjectDepthList.Clear();
			Bindings.LuauGameObjectDepthList = Bindings.LuauGameObjectList.OrderByDescending((KeyValuePair<GameObject, IntPtr> kv) => Bindings.GameObjectFunctions.GetDepth(kv.Key)).ToList<KeyValuePair<GameObject, IntPtr>>();
		}

		// Token: 0x06004DBA RID: 19898 RVA: 0x0019FA84 File Offset: 0x0019DC84
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int New(lua_State* L)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			Bindings.LuauGameObject* ptr = Luau.lua_class_push<Bindings.LuauGameObject>(L);
			ptr->Position = gameObject.transform.position;
			ptr->Rotation = gameObject.transform.rotation;
			ptr->Scale = gameObject.transform.localScale;
			Bindings.LuauGameObjectList.TryAdd(gameObject, (IntPtr)((void*)ptr));
			Bindings.LuauGameObjectListReverse.TryAdd((IntPtr)((void*)ptr), gameObject);
			return 1;
		}

		// Token: 0x06004DBB RID: 19899 RVA: 0x0019FAF8 File Offset: 0x0019DCF8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindGameObject(lua_State* L)
		{
			GameObject gameObject = GameObject.Find(new string((sbyte*)Luau.luaL_checkstring(L, 1)));
			if (!(gameObject != null))
			{
				return 0;
			}
			if (!CustomMapLoader.IsCustomScene(gameObject.scene.name))
			{
				return 0;
			}
			IntPtr intPtr;
			if (Bindings.LuauGameObjectList.TryGetValue(gameObject, out intPtr))
			{
				Luau.lua_class_push(L, "GameObject", intPtr);
			}
			else
			{
				Bindings.LuauGameObject* ptr = Luau.lua_class_push<Bindings.LuauGameObject>(L);
				ptr->Position = gameObject.transform.position;
				ptr->Rotation = gameObject.transform.rotation;
				ptr->Scale = gameObject.transform.localScale;
				Bindings.LuauGameObjectInitialState luauGameObjectInitialState = default(Bindings.LuauGameObjectInitialState);
				luauGameObjectInitialState.Position = gameObject.transform.localPosition;
				luauGameObjectInitialState.Rotation = gameObject.transform.localRotation;
				luauGameObjectInitialState.Scale = gameObject.transform.localScale;
				luauGameObjectInitialState.Visible = true;
				luauGameObjectInitialState.Collidable = true;
				luauGameObjectInitialState.Created = false;
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				Collider component2 = gameObject.GetComponent<Collider>();
				if (component2.IsNotNull())
				{
					luauGameObjectInitialState.Collidable = component2.enabled;
				}
				if (component.IsNotNull())
				{
					luauGameObjectInitialState.Visible = component.enabled;
				}
				Bindings.LuauGameObjectList.TryAdd(gameObject, (IntPtr)((void*)ptr));
				Bindings.LuauGameObjectListReverse.TryAdd((IntPtr)((void*)ptr), gameObject);
				Bindings.LuauGameObjectStates.TryAdd(gameObject, luauGameObjectInitialState);
				Bindings.GameObjectFunctions.UpdateDepthList();
			}
			return 1;
		}

		// Token: 0x06004DBC RID: 19900 RVA: 0x0019FC68 File Offset: 0x0019DE68
		public static Transform FindChild(Transform parent, string name)
		{
			foreach (object obj in parent)
			{
				Transform transform = (Transform)obj;
				if (transform.name == name)
				{
					return transform;
				}
				Transform transform2 = Bindings.GameObjectFunctions.FindChild(transform, name);
				if (transform2 != null)
				{
					return transform2;
				}
			}
			return null;
		}

		// Token: 0x06004DBD RID: 19901 RVA: 0x0019FCE4 File Offset: 0x0019DEE4
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindChildGameObject(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), out gameObject))
			{
				string text = new string((sbyte*)Luau.luaL_checkstring(L, 2));
				Transform transform = Bindings.GameObjectFunctions.FindChild(gameObject.transform, text);
				GameObject gameObject2 = ((transform != null) ? transform.gameObject : null);
				if (gameObject2.IsNotNull())
				{
					IntPtr intPtr;
					if (Bindings.LuauGameObjectList.TryGetValue(gameObject2, out intPtr))
					{
						Luau.lua_class_push(L, "GameObject", intPtr);
					}
					else
					{
						Bindings.LuauGameObject* ptr2 = Luau.lua_class_push<Bindings.LuauGameObject>(L);
						ptr2->Position = gameObject2.transform.position;
						ptr2->Rotation = gameObject2.transform.rotation;
						ptr2->Scale = gameObject2.transform.localScale;
						Bindings.LuauGameObjectInitialState luauGameObjectInitialState = default(Bindings.LuauGameObjectInitialState);
						luauGameObjectInitialState.Position = gameObject2.transform.localPosition;
						luauGameObjectInitialState.Rotation = gameObject2.transform.localRotation;
						luauGameObjectInitialState.Scale = gameObject2.transform.localScale;
						luauGameObjectInitialState.Visible = true;
						luauGameObjectInitialState.Collidable = true;
						luauGameObjectInitialState.Created = false;
						MeshRenderer component = gameObject2.GetComponent<MeshRenderer>();
						Collider component2 = gameObject2.GetComponent<Collider>();
						if (component2.IsNotNull())
						{
							luauGameObjectInitialState.Collidable = component2.enabled;
						}
						if (component.IsNotNull())
						{
							luauGameObjectInitialState.Visible = component.enabled;
						}
						Bindings.LuauGameObjectList.TryAdd(gameObject2, (IntPtr)((void*)ptr2));
						Bindings.LuauGameObjectListReverse.TryAdd((IntPtr)((void*)ptr2), gameObject2);
						Bindings.LuauGameObjectStates.TryAdd(gameObject2, luauGameObjectInitialState);
						Bindings.GameObjectFunctions.UpdateDepthList();
					}
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x06004DBE RID: 19902 RVA: 0x0019FE80 File Offset: 0x0019E080
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindComponent(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), out gameObject))
			{
				if (gameObject == null)
				{
					return 0;
				}
				string text = new string((sbyte*)Luau.luaL_checkstring(L, 2));
				if (text == "ParticleSystem")
				{
					ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
					if (component == null)
					{
						return 0;
					}
					Bindings.Components.LuauParticleSystemBindings.LuauParticleSystem* ptr2 = Luau.lua_class_push<Bindings.Components.LuauParticleSystemBindings.LuauParticleSystem>(L);
					Bindings.Components.ComponentList.TryAdd((IntPtr)((void*)ptr2), component);
					return 1;
				}
				else if (text == "AudioSource")
				{
					AudioSource component2 = gameObject.GetComponent<AudioSource>();
					if (component2 == null)
					{
						return 0;
					}
					Bindings.Components.LuauAudioSourceBindings.LuauAudioSource* ptr3 = Luau.lua_class_push<Bindings.Components.LuauAudioSourceBindings.LuauAudioSource>(L);
					Bindings.Components.ComponentList.TryAdd((IntPtr)((void*)ptr3), component2);
					return 1;
				}
				else if (text == "Light")
				{
					Light component3 = gameObject.GetComponent<Light>();
					if (component3 == null)
					{
						return 0;
					}
					Bindings.Components.LuauLightBindings.LuauLight* ptr4 = Luau.lua_class_push<Bindings.Components.LuauLightBindings.LuauLight>(L);
					Bindings.Components.ComponentList.TryAdd((IntPtr)((void*)ptr4), component3);
					return 1;
				}
				else if (text == "Animator")
				{
					Animator component4 = gameObject.GetComponent<Animator>();
					if (component4 == null)
					{
						return 0;
					}
					Bindings.Components.LuauAnimatorBindings.LuauAnimator* ptr5 = Luau.lua_class_push<Bindings.Components.LuauAnimatorBindings.LuauAnimator>(L);
					Bindings.Components.ComponentList.TryAdd((IntPtr)((void*)ptr5), component4);
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x06004DBF RID: 19903 RVA: 0x0019FFC8 File Offset: 0x0019E1C8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int CloneGameObject(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), out gameObject))
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, gameObject.transform.parent, false);
				Bindings.LuauGameObject* ptr2 = Luau.lua_class_push<Bindings.LuauGameObject>(L);
				ptr2->Position = gameObject2.transform.position;
				ptr2->Rotation = gameObject2.transform.rotation;
				ptr2->Scale = gameObject2.transform.localScale;
				Bindings.LuauGameObjectInitialState luauGameObjectInitialState = default(Bindings.LuauGameObjectInitialState);
				luauGameObjectInitialState.Position = gameObject2.transform.localPosition;
				luauGameObjectInitialState.Rotation = gameObject2.transform.localRotation;
				luauGameObjectInitialState.Scale = gameObject2.transform.localScale;
				luauGameObjectInitialState.Visible = true;
				luauGameObjectInitialState.Collidable = true;
				luauGameObjectInitialState.Created = true;
				MeshRenderer component = gameObject2.GetComponent<MeshRenderer>();
				Collider component2 = gameObject2.GetComponent<Collider>();
				if (component2.IsNotNull())
				{
					luauGameObjectInitialState.Collidable = component2.enabled;
				}
				if (component.IsNotNull())
				{
					luauGameObjectInitialState.Visible = component.enabled;
				}
				Bindings.LuauGameObjectList.TryAdd(gameObject2, (IntPtr)((void*)ptr2));
				Bindings.LuauGameObjectListReverse.TryAdd((IntPtr)((void*)ptr2), gameObject2);
				Bindings.LuauGameObjectStates.TryAdd(gameObject2, luauGameObjectInitialState);
				Bindings.GameObjectFunctions.UpdateDepthList();
				return 1;
			}
			return 0;
		}

		// Token: 0x06004DC0 RID: 19904 RVA: 0x001A011C File Offset: 0x0019E31C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DestroyGameObject(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			Bindings.LuauGameObjectInitialState luauGameObjectInitialState;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), out gameObject) && Bindings.LuauGameObjectStates.TryGetValue(gameObject, out luauGameObjectInitialState))
			{
				if (!luauGameObjectInitialState.Created)
				{
					Luau.luaL_errorL(L, "Cannot destroy a non-instantiated GameObject.", Array.Empty<string>());
					return 0;
				}
				Queue<GameObject> queue = new Queue<GameObject>();
				queue.Enqueue(gameObject);
				while (queue.Count != 0)
				{
					GameObject gameObject2 = queue.Dequeue();
					IntPtr intPtr;
					if (Bindings.LuauGameObjectList.TryGetValue(gameObject2, out intPtr))
					{
						Bindings.LuauGameObjectList.Remove(gameObject2);
						Bindings.LuauGameObjectListReverse.Remove(intPtr);
						Bindings.LuauGameObjectStates.Remove(gameObject2);
						foreach (object obj in gameObject2.transform)
						{
							Transform transform = (Transform)obj;
							queue.Enqueue(transform.gameObject);
						}
					}
				}
				Bindings.GameObjectFunctions.UpdateDepthList();
				gameObject.Destroy();
			}
			return 0;
		}

		// Token: 0x06004DC1 RID: 19905 RVA: 0x001A0248 File Offset: 0x0019E448
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetCollision(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), out gameObject))
			{
				Collider component = gameObject.GetComponent<Collider>();
				if (component.IsNotNull())
				{
					component.enabled = Luau.lua_toboolean(L, 2) == 1;
				}
			}
			return 0;
		}

		// Token: 0x06004DC2 RID: 19906 RVA: 0x001A029C File Offset: 0x0019E49C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetVisibility(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), out gameObject))
			{
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				if (component.IsNotNull())
				{
					component.enabled = Luau.lua_toboolean(L, 2) == 1;
				}
			}
			return 0;
		}

		// Token: 0x06004DC3 RID: 19907 RVA: 0x001A02F0 File Offset: 0x0019E4F0
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetActive(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), out gameObject))
			{
				gameObject.SetActive(Luau.lua_toboolean(L, 2) == 1);
			}
			return 0;
		}

		// Token: 0x06004DC4 RID: 19908 RVA: 0x001A0334 File Offset: 0x0019E534
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetText(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), out gameObject))
			{
				string text = new string(Luau.lua_tostring(L, 2));
				TextMeshPro component = gameObject.GetComponent<TextMeshPro>();
				if (component.IsNotNull())
				{
					component.text = text;
				}
				else
				{
					TextMesh component2 = gameObject.GetComponent<TextMesh>();
					if (component2.IsNotNull())
					{
						component2.text = text;
					}
				}
			}
			return 0;
		}

		// Token: 0x06004DC5 RID: 19909 RVA: 0x001A03A8 File Offset: 0x0019E5A8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int OnTouched(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), out gameObject))
			{
				int num;
				if (Bindings.LuauTriggerCallbacks.TryGetValue(gameObject, out num))
				{
					Luau.lua_unref(L, num);
					Bindings.LuauTriggerCallbacks.Remove(gameObject);
				}
				if (Luau.lua_type(L, 2) == 7)
				{
					int num2 = Luau.lua_ref(L, 2);
					Bindings.LuauTriggerCallbacks.TryAdd(gameObject, num2);
				}
				else
				{
					FixedString32Bytes fixedString32Bytes = "Callback must be a function";
					Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
				}
			}
			return 0;
		}

		// Token: 0x06004DC6 RID: 19910 RVA: 0x001A043C File Offset: 0x0019E63C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetVelocity(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), out gameObject))
			{
				Vector3 vector = *Luau.lua_class_get<Vector3>(L, 2);
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				if (component.IsNotNull())
				{
					component.linearVelocity = vector;
				}
			}
			return 0;
		}

		// Token: 0x06004DC7 RID: 19911 RVA: 0x001A0494 File Offset: 0x0019E694
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetVelocity(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), out gameObject))
			{
				if (gameObject.IsNull())
				{
					return 0;
				}
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				Vector3* ptr2 = Luau.lua_class_push<Vector3>(L, "Vec3");
				if (component.IsNotNull())
				{
					*ptr2 = component.linearVelocity;
				}
				else
				{
					*ptr2 = Vector3.zero;
				}
			}
			return 1;
		}

		// Token: 0x06004DC8 RID: 19912 RVA: 0x001A050C File Offset: 0x0019E70C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetColor(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 2);
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), out gameObject))
			{
				Color color = new Color(Mathf.Clamp01(vector.x / 255f), Mathf.Clamp01(vector.y / 255f), Mathf.Clamp01(vector.z / 255f), 1f);
				TextMeshPro component = gameObject.GetComponent<TextMeshPro>();
				if (component != null)
				{
					component.color = color;
					return 0;
				}
				TextMesh component2 = gameObject.GetComponent<TextMesh>();
				if (component2 != null)
				{
					component2.color = color;
					return 0;
				}
				Renderer component3 = gameObject.GetComponent<Renderer>();
				if (component3 != null)
				{
					component3.material.color = color;
				}
			}
			return 0;
		}

		// Token: 0x06004DC9 RID: 19913 RVA: 0x001A05E8 File Offset: 0x0019E7E8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Equals(lua_State* L)
		{
			Bindings.LuauGameObject* ptr = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject gameObject;
			if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr), out gameObject))
			{
				Bindings.LuauGameObject* ptr2 = Luau.lua_class_get<Bindings.LuauGameObject>(L, 2, "GameObject");
				GameObject gameObject2;
				if (Bindings.LuauGameObjectListReverse.TryGetValue((IntPtr)((void*)ptr2), out gameObject2) && gameObject == gameObject2)
				{
					Luau.lua_pushboolean(L, 1);
					return 1;
				}
			}
			Luau.lua_pushboolean(L, 0);
			return 1;
		}
	}

	// Token: 0x02000C23 RID: 3107
	[BurstCompile]
	public struct LuauPlayer
	{
		// Token: 0x04006111 RID: 24849
		public int PlayerID;

		// Token: 0x04006112 RID: 24850
		public FixedString32Bytes PlayerName;

		// Token: 0x04006113 RID: 24851
		public int PlayerMaterial;

		// Token: 0x04006114 RID: 24852
		[MarshalAs(UnmanagedType.U1)]
		public bool IsMasterClient;

		// Token: 0x04006115 RID: 24853
		public Vector3 BodyPosition;

		// Token: 0x04006116 RID: 24854
		public float ScaleMultiplier;

		// Token: 0x04006117 RID: 24855
		public Vector3 Velocity;

		// Token: 0x04006118 RID: 24856
		[MarshalAs(UnmanagedType.U1)]
		public bool IsPCVR;

		// Token: 0x04006119 RID: 24857
		public Vector3 LeftHandPosition;

		// Token: 0x0400611A RID: 24858
		public Vector3 RightHandPosition;

		// Token: 0x0400611B RID: 24859
		[MarshalAs(UnmanagedType.U1)]
		public bool IsEntityAuthority;

		// Token: 0x0400611C RID: 24860
		public Quaternion HeadRotation;

		// Token: 0x0400611D RID: 24861
		public Quaternion LeftHandRotation;

		// Token: 0x0400611E RID: 24862
		public Quaternion RightHandRotation;

		// Token: 0x0400611F RID: 24863
		[MarshalAs(UnmanagedType.U1)]
		public bool IsInVStump;
	}

	// Token: 0x02000C24 RID: 3108
	[BurstCompile]
	public static class PlayerFunctions
	{
		// Token: 0x06004DCD RID: 19917 RVA: 0x001A0678 File Offset: 0x0019E878
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetPlayerByID(lua_State* L)
		{
			int num = (int)Luau.luaL_checknumber(L, 1);
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				if (netPlayer.ActorNumber == num)
				{
					IntPtr intPtr;
					if (Bindings.LuauPlayerList.TryGetValue(netPlayer.ActorNumber, out intPtr))
					{
						Luau.lua_class_push(L, "Player", intPtr);
					}
					else
					{
						Bindings.LuauPlayer* ptr = Luau.lua_class_push<Bindings.LuauPlayer>(L);
						ptr->PlayerID = netPlayer.ActorNumber;
						ptr->ScaleMultiplier = 1f;
						ptr->PlayerMaterial = 0;
						ptr->IsMasterClient = netPlayer.IsMasterClient;
						Bindings.LuauPlayerList[netPlayer.ActorNumber] = (IntPtr)((void*)ptr);
						GorillaGameManager instance = GorillaGameManager.instance;
						VRRig vrrig = ((instance != null) ? instance.FindPlayerVRRig(netPlayer) : null);
						if (vrrig != null)
						{
							ptr->PlayerName = vrrig.playerNameVisible;
							Bindings.LuauVRRigList[netPlayer.ActorNumber] = vrrig;
							Bindings.PlayerFunctions.UpdatePlayer(L, vrrig, ptr);
							Bindings.LuauPlayerList[netPlayer.ActorNumber] = (IntPtr)((void*)ptr);
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x06004DCE RID: 19918 RVA: 0x001A07C8 File Offset: 0x0019E9C8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static void UpdatePlayer(lua_State* L, VRRig p, Bindings.LuauPlayer* data)
		{
			if (!string.IsNullOrWhiteSpace(p.playerNameVisible))
			{
				FixedString32Bytes fixedString32Bytes = p.playerNameVisible;
				if ((in data->PlayerName) != (in fixedString32Bytes))
				{
					data->PlayerName = p.playerNameVisible;
				}
			}
			data->BodyPosition = p.transform.position;
			data->Velocity = p.LatestVelocity();
			data->LeftHandPosition = p.leftHandTransform.position;
			data->RightHandPosition = p.rightHandTransform.position;
			data->HeadRotation = p.head.rigTarget.rotation;
			data->LeftHandRotation = p.leftHandTransform.rotation;
			data->RightHandRotation = p.rightHandTransform.rotation;
			if (p.isLocal)
			{
				data->IsInVStump = CustomMapManager.IsLocalPlayerInVirtualStump();
			}
			else if (p.creator != null)
			{
				data->IsInVStump = CustomMapManager.IsRemotePlayerInVirtualStump(p.creator.UserId);
			}
			else
			{
				data->IsInVStump = false;
			}
			data->IsEntityAuthority = CustomMapsGameManager.instance.IsNotNull() && CustomMapsGameManager.instance.gameEntityManager.IsNotNull() && CustomMapsGameManager.instance.gameEntityManager.IsZoneAuthority();
		}
	}

	// Token: 0x02000C25 RID: 3109
	[BurstCompile]
	public struct LuauAIAgent
	{
		// Token: 0x04006120 RID: 24864
		public int EntityID;

		// Token: 0x04006121 RID: 24865
		public Vector3 EntityPosition;

		// Token: 0x04006122 RID: 24866
		public Quaternion EntityRotation;
	}

	// Token: 0x02000C26 RID: 3110
	[BurstCompile]
	public struct LuauGrabbableEntity
	{
		// Token: 0x04006123 RID: 24867
		public int EntityID;

		// Token: 0x04006124 RID: 24868
		public Vector3 EntityPosition;

		// Token: 0x04006125 RID: 24869
		public Quaternion EntityRotation;
	}

	// Token: 0x02000C27 RID: 3111
	[BurstCompile]
	public static class GrabbableEntityFunctions
	{
		// Token: 0x06004DCF RID: 19919 RVA: 0x001A08F8 File Offset: 0x0019EAF8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ToString(lua_State* L)
		{
			string text = "NULL";
			Bindings.LuauGrabbableEntity* ptr = Luau.lua_class_get<Bindings.LuauGrabbableEntity>(L, 1);
			if (ptr != null)
			{
				text = string.Concat(new string[]
				{
					"ID: ",
					ptr->EntityID.ToString(),
					" | Pos: ",
					ptr->EntityPosition.ToString(),
					" | Rot: ",
					ptr->EntityRotation.ToString()
				});
			}
			Luau.lua_pushstring(L, text);
			return 1;
		}

		// Token: 0x06004DD0 RID: 19920 RVA: 0x001A097C File Offset: 0x0019EB7C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetGrabbableEntityByEntityID(lua_State* L)
		{
			int num = (int)Luau.luaL_checknumber(L, 1);
			Debug.Log(string.Format("[LuauBindings::GetGrabbableEntityByEntityID] ID: {0}", num));
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNotNull())
			{
				GameEntityId entityIdFromNetId = gameEntityManager.GetEntityIdFromNetId(num);
				GameEntity gameEntity = gameEntityManager.GetGameEntity(entityIdFromNetId);
				if (gameEntity.IsNotNull())
				{
					if (gameEntity.gameObject.IsNull())
					{
						return 0;
					}
					Debug.Log("[LuauBindings::GetGrabbableEntityByEntityID] Found agent: " + gameEntity.gameObject.name);
					IntPtr intPtr;
					if (Bindings.LuauGrabbablesList.TryGetValue(num, out intPtr))
					{
						Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntity, (Bindings.LuauGrabbableEntity*)(void*)intPtr);
						Luau.lua_class_push(L, "GrabbableEntity", intPtr);
					}
					else
					{
						Bindings.LuauGrabbableEntity* ptr = Luau.lua_class_push<Bindings.LuauGrabbableEntity>(L);
						Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntity, ptr);
						Bindings.LuauGrabbablesList[num] = (IntPtr)((void*)ptr);
					}
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x06004DD1 RID: 19921 RVA: 0x001A0A54 File Offset: 0x0019EC54
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetHoldingActorNumberByLuauID(lua_State* L)
		{
			short num = (short)Luau.luaL_checknumber(L, 1);
			Debug.Log(string.Format("[LuauBindings::GetHoldingActorNumberByLuauID] ID: {0}", num));
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNull())
			{
				return 0;
			}
			List<GameEntity> gameEntities = gameEntityManager.GetGameEntities();
			for (int i = 0; i < gameEntities.Count; i++)
			{
				if (!gameEntities[i].IsNull() && !gameEntities[i].gameObject.IsNull())
				{
					CustomMapsGrabbablesController component = gameEntities[i].gameObject.GetComponent<CustomMapsGrabbablesController>();
					if (!component.IsNull())
					{
						Debug.Log("[LuauBindings::GetHoldingActorNumberByLuauID] checking GrabbableController on " + string.Format("{0}, id: {1}", component.gameObject.name, component.luaAgentID));
						if (component.luaAgentID == num)
						{
							Luau.lua_pushnumber(L, (double)component.GetGrabbingActor());
							return 1;
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x06004DD2 RID: 19922 RVA: 0x001A0B3C File Offset: 0x0019ED3C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetHoldingActorNumberByEntityID(lua_State* L)
		{
			int num = (int)Luau.luaL_checknumber(L, 1);
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNull())
			{
				return 0;
			}
			GameEntityId entityIdFromNetId = gameEntityManager.GetEntityIdFromNetId(num);
			GameEntity gameEntity = gameEntityManager.GetGameEntity(entityIdFromNetId);
			if (gameEntity.IsNull() || gameEntity.gameObject.IsNull())
			{
				return 0;
			}
			CustomMapsGrabbablesController component = gameEntity.gameObject.GetComponent<CustomMapsGrabbablesController>();
			if (component.IsNull())
			{
				return 0;
			}
			Luau.lua_pushnumber(L, (double)component.GetGrabbingActor());
			return 1;
		}

		// Token: 0x06004DD3 RID: 19923 RVA: 0x001A0BB8 File Offset: 0x0019EDB8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindPrePlacedGrabbableEntityByID(lua_State* L)
		{
			short num = (short)Luau.luaL_checknumber(L, 1);
			Debug.Log(string.Format("[LuauBindings::FindPrePlacedGrabbableEntityByID] ID: {0}", num));
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNotNull())
			{
				List<GameEntity> gameEntities = gameEntityManager.GetGameEntities();
				for (int i = 0; i < gameEntities.Count; i++)
				{
					if (!gameEntities[i].IsNull() && !gameEntities[i].gameObject.IsNull())
					{
						CustomMapsGrabbablesController component = gameEntities[i].gameObject.GetComponent<CustomMapsGrabbablesController>();
						if (!component.IsNull())
						{
							Debug.Log("[LuauBindings::FindPrePlacedGrabbableEntityByID] checking GrabbableController on " + string.Format("{0}, id: {1}", component.gameObject.name, component.luaAgentID));
							if (component.luaAgentID == num)
							{
								IntPtr intPtr;
								if (Bindings.LuauGrabbablesList.TryGetValue(gameEntities[i].GetNetId(), out intPtr))
								{
									Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntities[i], (Bindings.LuauGrabbableEntity*)(void*)intPtr);
									Luau.lua_class_push(L, "GrabbableEntity", intPtr);
								}
								else
								{
									Bindings.LuauGrabbableEntity* ptr = Luau.lua_class_push<Bindings.LuauGrabbableEntity>(L);
									Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntities[i], ptr);
									Bindings.LuauGrabbablesList[gameEntities[i].GetNetId()] = (IntPtr)((void*)ptr);
								}
								return 1;
							}
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x06004DD4 RID: 19924 RVA: 0x001A0D10 File Offset: 0x0019EF10
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SpawnGrabbableEntity(lua_State* L)
		{
			Debug.Log("[LuauBindings::SpawnGrabbableEntity]");
			CustomMapsGameManager instance = CustomMapsGameManager.instance;
			GameEntityManager gameEntityManager = (instance.IsNotNull() ? instance.gameEntityManager : null);
			if (gameEntityManager.IsNull())
			{
				LuauHud.Instance.LuauLog("SpawnGrabbableEntity failed. EntityManager is null.");
				return 0;
			}
			if (!gameEntityManager.IsZoneAuthority())
			{
				LuauHud.Instance.LuauLog("SpawnGrabbableEntity failed. Local Player doesn't have Entity Authority.");
				return 0;
			}
			if (Bindings.LuauAIAgentList.Count + Bindings.LuauGrabbablesList.Count == Constants.aiAgentLimit)
			{
				LuauHud.Instance.LuauLog(string.Format("SpawnGrabbableEntity failed, EntityLimit of {0}", Constants.aiAgentLimit) + " has already been reached.");
				return 0;
			}
			int num = (int)Luau.luaL_checknumber(L, 1);
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 3, "Quat");
			GameEntityId gameEntityId = instance.SpawnGrabbableAtLocation(num, vector, quaternion);
			Debug.Log("[LuauBindings::SpawnGrabbableEntity] spawnedGrabbable");
			if (!gameEntityId.IsValid())
			{
				LuauHud.Instance.LuauLog("SpawnGrabbableEntity failed to create entity.");
				return 0;
			}
			Debug.Log("[LuauBindings::SpawnGrabbableEntity] spawnedGrabbable ID valid");
			GameEntity gameEntity = gameEntityManager.GetGameEntity(gameEntityId);
			IntPtr intPtr;
			if (Bindings.LuauGrabbablesList.TryGetValue(gameEntity.GetNetId(), out intPtr))
			{
				Debug.Log("[LuauBindings::SpawnGrabbableEntity] fround grabbable");
				Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntity, (Bindings.LuauGrabbableEntity*)(void*)intPtr);
				Luau.lua_class_push(L, "GrabbableEntity", intPtr);
				return 1;
			}
			Debug.Log("[LuauBindings::SpawnGrabbableEntity] grabbable not found");
			Luau.lua_getglobal(L, "GrabbableEntities");
			Bindings.LuauGrabbableEntity* ptr = Luau.lua_class_push<Bindings.LuauGrabbableEntity>(L);
			Bindings.GrabbableEntityFunctions.UpdateEntity(gameEntity, ptr);
			Bindings.LuauGrabbablesList[gameEntity.GetNetId()] = (IntPtr)((void*)ptr);
			Debug.Log("[LuauBindings::SpawnGrabbableEntity] created new grabbable");
			Luau.lua_rawseti(L, -2, Bindings.LuauGrabbablesList.Count);
			Luau.lua_pop(L, 1);
			Debug.Log("[LuauBindings::SpawnGrabbableEntity] pushing new grabbable");
			Luau.lua_class_push(L, "GrabbableEntity", (IntPtr)((void*)ptr));
			return 1;
		}

		// Token: 0x06004DD5 RID: 19925 RVA: 0x001A0EFA File Offset: 0x0019F0FA
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static void UpdateEntity(GameEntity entity, Bindings.LuauGrabbableEntity* luaAgent)
		{
			luaAgent->EntityID = entity.GetNetId();
			luaAgent->EntityPosition = entity.transform.position;
			luaAgent->EntityRotation = entity.transform.rotation;
		}

		// Token: 0x06004DD6 RID: 19926 RVA: 0x001A0F2C File Offset: 0x0019F12C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DestroyEntity(lua_State* L)
		{
			Bindings.LuauGrabbableEntity* ptr = Luau.lua_class_get<Bindings.LuauGrabbableEntity>(L, 1);
			if (ptr != null)
			{
				GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
				if (entityManager.IsNotNull())
				{
					GameEntityId entityIdFromNetId = entityManager.GetEntityIdFromNetId(ptr->EntityID);
					entityManager.RequestDestroyItem(entityIdFromNetId);
				}
			}
			return 0;
		}
	}

	// Token: 0x02000C28 RID: 3112
	[BurstCompile]
	public static class AIAgentFunctions
	{
		// Token: 0x06004DD7 RID: 19927 RVA: 0x001A0F6C File Offset: 0x0019F16C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ToString(lua_State* L)
		{
			string text = "NULL";
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			if (ptr != null)
			{
				text = string.Concat(new string[]
				{
					"ID: ",
					ptr->EntityID.ToString(),
					" | Pos: ",
					ptr->EntityPosition.ToString(),
					" | Rot: ",
					ptr->EntityRotation.ToString()
				});
			}
			Luau.lua_pushstring(L, text);
			return 1;
		}

		// Token: 0x06004DD8 RID: 19928 RVA: 0x001A0FF0 File Offset: 0x0019F1F0
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetAIAgentByEntityID(lua_State* L)
		{
			int num = (int)Luau.luaL_checknumber(L, 1);
			Debug.Log(string.Format("[LuauBindings::GetAIAgentByEntityID] ID: {0}", num));
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNotNull())
			{
				GameEntityId entityIdFromNetId = gameEntityManager.GetEntityIdFromNetId(num);
				GameEntity gameEntity = gameEntityManager.GetGameEntity(entityIdFromNetId);
				if (gameEntity.IsNotNull())
				{
					if (gameEntity.gameObject.IsNull())
					{
						return 0;
					}
					if (gameEntity.gameObject.GetComponent<GameAgent>().IsNotNull())
					{
						Debug.Log("[LuauBindings::GetAIAgentByEntityID] Found agent: " + gameEntity.gameObject.name);
						IntPtr intPtr;
						if (Bindings.LuauAIAgentList.TryGetValue(num, out intPtr))
						{
							Bindings.AIAgentFunctions.UpdateEntity(gameEntity, (Bindings.LuauAIAgent*)(void*)intPtr);
							Luau.lua_class_push(L, "AIAgent", intPtr);
						}
						else
						{
							Bindings.LuauAIAgent* ptr = Luau.lua_class_push<Bindings.LuauAIAgent>(L);
							Bindings.AIAgentFunctions.UpdateEntity(gameEntity, ptr);
							Bindings.LuauAIAgentList[num] = (IntPtr)((void*)ptr);
						}
					}
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x06004DD9 RID: 19929 RVA: 0x001A10DC File Offset: 0x0019F2DC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindPrePlacedAIAgentByID(lua_State* L)
		{
			short num = (short)Luau.luaL_checknumber(L, 1);
			GameAgentManager gameAgentManager = CustomMapsGameManager.instance.gameAgentManager;
			if (gameAgentManager.IsNotNull())
			{
				List<GameAgent> agents = gameAgentManager.GetAgents();
				for (int i = 0; i < agents.Count; i++)
				{
					if (!agents[i].gameObject.IsNull())
					{
						CustomMapsAIBehaviourController component = agents[i].gameObject.GetComponent<CustomMapsAIBehaviourController>();
						if (!component.IsNull() && component.luaAgentID == num)
						{
							IntPtr intPtr;
							if (Bindings.LuauAIAgentList.TryGetValue(agents[i].entity.GetNetId(), out intPtr))
							{
								Bindings.AIAgentFunctions.UpdateEntity(agents[i].entity, (Bindings.LuauAIAgent*)(void*)intPtr);
								Luau.lua_class_push(L, "AIAgent", intPtr);
							}
							else
							{
								Bindings.LuauAIAgent* ptr = Luau.lua_class_push<Bindings.LuauAIAgent>(L);
								Bindings.AIAgentFunctions.UpdateEntity(agents[i].entity, ptr);
								Bindings.LuauAIAgentList[agents[i].entity.GetNetId()] = (IntPtr)((void*)ptr);
							}
							return 1;
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x06004DDA RID: 19930 RVA: 0x001A11F4 File Offset: 0x0019F3F4
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SpawnAIAgent(lua_State* L)
		{
			CustomMapsGameManager instance = CustomMapsGameManager.instance;
			GameEntityManager gameEntityManager = (instance.IsNotNull() ? instance.gameEntityManager : null);
			if (gameEntityManager.IsNull())
			{
				LuauHud.Instance.LuauLog("SpawnAIAgent failed. EntityManager is null.");
				return 0;
			}
			if (!gameEntityManager.IsZoneAuthority())
			{
				LuauHud.Instance.LuauLog("SpawnAIAgent failed. Local Player doesn't have Entity Authority.");
				return 0;
			}
			if (Bindings.LuauAIAgentList.Count + Bindings.LuauGrabbablesList.Count == Constants.aiAgentLimit)
			{
				LuauHud.Instance.LuauLog(string.Format("SpawnAIAgent failed, AIAgentLimit of {0}", Constants.aiAgentLimit) + " has already been reached.");
				return 0;
			}
			int num = (int)Luau.luaL_checknumber(L, 1);
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 3, "Quat");
			GameEntityId gameEntityId = instance.SpawnEnemyAtLocation(num, vector, quaternion);
			if (gameEntityId.IsValid())
			{
				GameEntity gameEntity = gameEntityManager.GetGameEntity(gameEntityId);
				if ((gameEntity.IsNotNull() ? gameEntity.gameObject.GetComponent<GameAgent>() : null).IsNotNull())
				{
					IntPtr intPtr;
					if (Bindings.LuauAIAgentList.TryGetValue(gameEntity.GetNetId(), out intPtr))
					{
						Bindings.AIAgentFunctions.UpdateEntity(gameEntity, (Bindings.LuauAIAgent*)(void*)intPtr);
						Luau.lua_class_push(L, "AIAgent", intPtr);
						return 1;
					}
					Luau.lua_getglobal(L, "AIAgents");
					Bindings.LuauAIAgent* ptr = Luau.lua_class_push<Bindings.LuauAIAgent>(L);
					Bindings.AIAgentFunctions.UpdateEntity(gameEntity, ptr);
					Bindings.LuauAIAgentList[gameEntity.GetNetId()] = (IntPtr)((void*)ptr);
					Luau.lua_rawseti(L, -2, Bindings.LuauAIAgentList.Count);
					Luau.lua_pop(L, 1);
					Luau.lua_class_push(L, "AIAgent", (IntPtr)((void*)ptr));
					return 1;
				}
			}
			LuauHud.Instance.LuauLog("SpawnAIAgent failed to create entity.");
			return 0;
		}

		// Token: 0x06004DDB RID: 19931 RVA: 0x001A13BC File Offset: 0x0019F5BC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetDestination(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			Vector3* ptr2 = Luau.lua_class_get<Vector3>(L, 2);
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNotNull())
			{
				CustomMapsAIBehaviourController component = gameEntityManager.GetGameEntity(gameEntityManager.GetEntityIdFromNetId(ptr->EntityID)).gameObject.GetComponent<CustomMapsAIBehaviourController>();
				if (component.IsNotNull())
				{
					component.RequestDestination(*ptr2);
				}
			}
			return 0;
		}

		// Token: 0x06004DDC RID: 19932 RVA: 0x001A1420 File Offset: 0x0019F620
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int PlayAgentAnimation(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			string text = Marshal.PtrToStringAnsi((IntPtr)((void*)Luau.luaL_checkstring(L, 2)));
			if (ptr != null)
			{
				GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
				if (entityManager.IsNotNull())
				{
					CustomMapsAIBehaviourController behaviorControllerForEntity = CustomMapsGameManager.GetBehaviorControllerForEntity(entityManager.GetEntityIdFromNetId(ptr->EntityID));
					if (behaviorControllerForEntity.IsNotNull())
					{
						behaviorControllerForEntity.PlayAnimation(text, 0f);
					}
				}
			}
			return 0;
		}

		// Token: 0x06004DDD RID: 19933 RVA: 0x001A1484 File Offset: 0x0019F684
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetTarget(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			if (ptr == null)
			{
				return 0;
			}
			GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
			if (entityManager.IsNull() || !entityManager.IsAuthority())
			{
				return 0;
			}
			int num = (int)Luau.luaL_checknumber(L, 2);
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(num, out rigContainer))
			{
				num = -1;
			}
			CustomMapsAIBehaviourController behaviorControllerForEntity = CustomMapsGameManager.GetBehaviorControllerForEntity(entityManager.GetEntityIdFromNetId(ptr->EntityID));
			if (behaviorControllerForEntity.IsNull())
			{
				return 0;
			}
			if (num == -1)
			{
				behaviorControllerForEntity.ClearTarget();
			}
			else
			{
				GRPlayer component = rigContainer.Rig.GetComponent<GRPlayer>();
				behaviorControllerForEntity.SetTarget(component);
			}
			return 0;
		}

		// Token: 0x06004DDE RID: 19934 RVA: 0x001A1514 File Offset: 0x0019F714
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetTarget(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			if (ptr != null)
			{
				GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
				if (entityManager.IsNotNull() && entityManager.IsAuthority())
				{
					CustomMapsAIBehaviourController behaviorControllerForEntity = CustomMapsGameManager.GetBehaviorControllerForEntity(entityManager.GetEntityIdFromNetId(ptr->EntityID));
					if (behaviorControllerForEntity.IsNotNull() && behaviorControllerForEntity.TargetPlayer.IsNotNull() && behaviorControllerForEntity.TargetPlayer.MyRig.IsNotNull() && !behaviorControllerForEntity.TargetPlayer.MyRig.OwningNetPlayer.IsNull)
					{
						Luau.lua_pushnumber(L, (double)behaviorControllerForEntity.TargetPlayer.MyRig.OwningNetPlayer.ActorNumber);
						return 1;
					}
				}
			}
			Luau.lua_pushnumber(L, -1.0);
			return 1;
		}

		// Token: 0x06004DDF RID: 19935 RVA: 0x001A15C5 File Offset: 0x0019F7C5
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static void UpdateEntity(GameEntity entity, Bindings.LuauAIAgent* luaAgent)
		{
			luaAgent->EntityID = entity.GetNetId();
			luaAgent->EntityPosition = entity.transform.position;
			luaAgent->EntityRotation = entity.transform.rotation;
		}

		// Token: 0x06004DE0 RID: 19936 RVA: 0x001A15F8 File Offset: 0x0019F7F8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DestroyEntity(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			if (ptr != null)
			{
				GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
				if (entityManager.IsNotNull())
				{
					GameEntityId entityIdFromNetId = entityManager.GetEntityIdFromNetId(ptr->EntityID);
					entityManager.RequestDestroyItem(entityIdFromNetId);
				}
			}
			return 0;
		}
	}

	// Token: 0x02000C29 RID: 3113
	[BurstCompile]
	public static class Vec3Functions
	{
		// Token: 0x06004DE1 RID: 19937 RVA: 0x001A1635 File Offset: 0x0019F835
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int New(lua_State* L)
		{
			return Bindings.Vec3Functions.New_00004DE1$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DE2 RID: 19938 RVA: 0x001A163D File Offset: 0x0019F83D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Add(lua_State* L)
		{
			return Bindings.Vec3Functions.Add_00004DE2$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DE3 RID: 19939 RVA: 0x001A1645 File Offset: 0x0019F845
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Sub(lua_State* L)
		{
			return Bindings.Vec3Functions.Sub_00004DE3$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DE4 RID: 19940 RVA: 0x001A164D File Offset: 0x0019F84D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Mul(lua_State* L)
		{
			return Bindings.Vec3Functions.Mul_00004DE4$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DE5 RID: 19941 RVA: 0x001A1655 File Offset: 0x0019F855
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Div(lua_State* L)
		{
			return Bindings.Vec3Functions.Div_00004DE5$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DE6 RID: 19942 RVA: 0x001A165D File Offset: 0x0019F85D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Unm(lua_State* L)
		{
			return Bindings.Vec3Functions.Unm_00004DE6$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DE7 RID: 19943 RVA: 0x001A1665 File Offset: 0x0019F865
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Eq(lua_State* L)
		{
			return Bindings.Vec3Functions.Eq_00004DE7$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DE8 RID: 19944 RVA: 0x001A1670 File Offset: 0x0019F870
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ToString(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Luau.lua_pushstring(L, vector.ToString());
			return 1;
		}

		// Token: 0x06004DE9 RID: 19945 RVA: 0x001A16A9 File Offset: 0x0019F8A9
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Dot(lua_State* L)
		{
			return Bindings.Vec3Functions.Dot_00004DE9$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DEA RID: 19946 RVA: 0x001A16B1 File Offset: 0x0019F8B1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Cross(lua_State* L)
		{
			return Bindings.Vec3Functions.Cross_00004DEA$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DEB RID: 19947 RVA: 0x001A16B9 File Offset: 0x0019F8B9
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Project(lua_State* L)
		{
			return Bindings.Vec3Functions.Project_00004DEB$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DEC RID: 19948 RVA: 0x001A16C1 File Offset: 0x0019F8C1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Length(lua_State* L)
		{
			return Bindings.Vec3Functions.Length_00004DEC$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DED RID: 19949 RVA: 0x001A16C9 File Offset: 0x0019F8C9
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Normalize(lua_State* L)
		{
			return Bindings.Vec3Functions.Normalize_00004DED$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DEE RID: 19950 RVA: 0x001A16D1 File Offset: 0x0019F8D1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SafeNormal(lua_State* L)
		{
			return Bindings.Vec3Functions.SafeNormal_00004DEE$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DEF RID: 19951 RVA: 0x001A16D9 File Offset: 0x0019F8D9
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Distance(lua_State* L)
		{
			return Bindings.Vec3Functions.Distance_00004DEF$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DF0 RID: 19952 RVA: 0x001A16E1 File Offset: 0x0019F8E1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Lerp(lua_State* L)
		{
			return Bindings.Vec3Functions.Lerp_00004DF0$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DF1 RID: 19953 RVA: 0x001A16E9 File Offset: 0x0019F8E9
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Rotate(lua_State* L)
		{
			return Bindings.Vec3Functions.Rotate_00004DF1$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DF2 RID: 19954 RVA: 0x001A16F1 File Offset: 0x0019F8F1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ZeroVector(lua_State* L)
		{
			return Bindings.Vec3Functions.ZeroVector_00004DF2$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DF3 RID: 19955 RVA: 0x001A16F9 File Offset: 0x0019F8F9
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int OneVector(lua_State* L)
		{
			return Bindings.Vec3Functions.OneVector_00004DF3$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DF4 RID: 19956 RVA: 0x001A1701 File Offset: 0x0019F901
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int NearlyEqual(lua_State* L)
		{
			return Bindings.Vec3Functions.NearlyEqual_00004DF4$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004DF5 RID: 19957 RVA: 0x001A170C File Offset: 0x0019F90C
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int New$BurstManaged(lua_State* L)
		{
			Vector3* ptr = Luau.lua_class_push<Vector3>(L, "Vec3");
			ptr->x = (float)Luau.luaL_optnumber(L, 1, 0.0);
			ptr->y = (float)Luau.luaL_optnumber(L, 2, 0.0);
			ptr->z = (float)Luau.luaL_optnumber(L, 3, 0.0);
			return 1;
		}

		// Token: 0x06004DF6 RID: 19958 RVA: 0x001A1770 File Offset: 0x0019F970
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Add$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = vector + vector2;
			return 1;
		}

		// Token: 0x06004DF7 RID: 19959 RVA: 0x001A17C8 File Offset: 0x0019F9C8
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Sub$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = vector - vector2;
			return 1;
		}

		// Token: 0x06004DF8 RID: 19960 RVA: 0x001A1820 File Offset: 0x0019FA20
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Mul$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			float num = (float)Luau.luaL_checknumber(L, 2);
			*Luau.lua_class_push<Vector3>(L, "Vec3") = vector * num;
			return 1;
		}

		// Token: 0x06004DF9 RID: 19961 RVA: 0x001A186C File Offset: 0x0019FA6C
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Div$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			float num = (float)Luau.luaL_checknumber(L, 2);
			*Luau.lua_class_push<Vector3>(L, "Vec3") = vector / num;
			return 1;
		}

		// Token: 0x06004DFA RID: 19962 RVA: 0x001A18B8 File Offset: 0x0019FAB8
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Unm$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = -vector;
			return 1;
		}

		// Token: 0x06004DFB RID: 19963 RVA: 0x001A18F8 File Offset: 0x0019FAF8
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Eq$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			int num = ((vector == vector2) ? 1 : 0);
			Luau.lua_pushnumber(L, (double)num);
			return 1;
		}

		// Token: 0x06004DFC RID: 19964 RVA: 0x001A1948 File Offset: 0x0019FB48
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Dot$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			double num = (double)Vector3.Dot(vector, vector2);
			Luau.lua_pushnumber(L, num);
			return 1;
		}

		// Token: 0x06004DFD RID: 19965 RVA: 0x001A1994 File Offset: 0x0019FB94
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Cross$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Vector3.Cross(vector, vector2);
			return 1;
		}

		// Token: 0x06004DFE RID: 19966 RVA: 0x001A19EC File Offset: 0x0019FBEC
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Project$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Vector3.Project(vector, vector2);
			return 1;
		}

		// Token: 0x06004DFF RID: 19967 RVA: 0x001A1A44 File Offset: 0x0019FC44
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Length$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Luau.lua_pushnumber(L, (double)Vector3.Magnitude(vector));
			return 1;
		}

		// Token: 0x06004E00 RID: 19968 RVA: 0x001A1A76 File Offset: 0x0019FC76
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Normalize$BurstManaged(lua_State* L)
		{
			Luau.lua_class_get<Vector3>(L, 1, "Vec3")->Normalize();
			return 0;
		}

		// Token: 0x06004E01 RID: 19969 RVA: 0x001A1A90 File Offset: 0x0019FC90
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int SafeNormal$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = vector.normalized;
			return 1;
		}

		// Token: 0x06004E02 RID: 19970 RVA: 0x001A1AD4 File Offset: 0x0019FCD4
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Distance$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			Luau.lua_pushnumber(L, (double)Vector3.Distance(vector, vector2));
			return 1;
		}

		// Token: 0x06004E03 RID: 19971 RVA: 0x001A1B20 File Offset: 0x0019FD20
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Lerp$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			double num = Luau.luaL_checknumber(L, 3);
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Vector3.Lerp(vector, vector2, (float)num);
			return 1;
		}

		// Token: 0x06004E04 RID: 19972 RVA: 0x001A1B84 File Offset: 0x0019FD84
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Rotate$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 2, "Quat");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = quaternion * vector;
			return 1;
		}

		// Token: 0x06004E05 RID: 19973 RVA: 0x001A1BDC File Offset: 0x0019FDDC
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int ZeroVector$BurstManaged(lua_State* L)
		{
			Vector3* ptr = Luau.lua_class_push<Vector3>(L, "Vec3");
			ptr->x = 0f;
			ptr->y = 0f;
			ptr->z = 0f;
			return 1;
		}

		// Token: 0x06004E06 RID: 19974 RVA: 0x001A1C0F File Offset: 0x0019FE0F
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int OneVector$BurstManaged(lua_State* L)
		{
			Vector3* ptr = Luau.lua_class_push<Vector3>(L, "Vec3");
			ptr->x = 1f;
			ptr->y = 1f;
			ptr->z = 1f;
			return 1;
		}

		// Token: 0x06004E07 RID: 19975 RVA: 0x001A1C44 File Offset: 0x0019FE44
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int NearlyEqual$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			float num = (float)Luau.luaL_optnumber(L, 3, 0.0001);
			bool flag = Math.Abs(vector.x - vector2.x) <= num;
			if (flag && Math.Abs(vector.y - vector2.y) > num)
			{
				flag = false;
			}
			if (flag && Math.Abs(vector.z - vector2.z) > num)
			{
				flag = false;
			}
			Luau.lua_pushboolean(L, flag ? 1 : 0);
			return 1;
		}

		// Token: 0x02000C2A RID: 3114
		// (Invoke) Token: 0x06004E09 RID: 19977
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int New_00004DE1$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C2B RID: 3115
		internal static class New_00004DE1$BurstDirectCall
		{
			// Token: 0x06004E0C RID: 19980 RVA: 0x001A1CEC File Offset: 0x0019FEEC
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.New_00004DE1$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.New_00004DE1$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.New_00004DE1$PostfixBurstDelegate>(new Bindings.Vec3Functions.New_00004DE1$PostfixBurstDelegate(Bindings.Vec3Functions.New)).Value;
				}
				A_0 = Bindings.Vec3Functions.New_00004DE1$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E0D RID: 19981 RVA: 0x001A1D2C File Offset: 0x0019FF2C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.New_00004DE1$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E0E RID: 19982 RVA: 0x001A1D44 File Offset: 0x0019FF44
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.New_00004DE1$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.New$BurstManaged(L);
			}

			// Token: 0x04006126 RID: 24870
			private static IntPtr Pointer;
		}

		// Token: 0x02000C2C RID: 3116
		// (Invoke) Token: 0x06004E10 RID: 19984
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Add_00004DE2$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C2D RID: 3117
		internal static class Add_00004DE2$BurstDirectCall
		{
			// Token: 0x06004E13 RID: 19987 RVA: 0x001A1D78 File Offset: 0x0019FF78
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Add_00004DE2$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Add_00004DE2$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Add_00004DE2$PostfixBurstDelegate>(new Bindings.Vec3Functions.Add_00004DE2$PostfixBurstDelegate(Bindings.Vec3Functions.Add)).Value;
				}
				A_0 = Bindings.Vec3Functions.Add_00004DE2$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E14 RID: 19988 RVA: 0x001A1DB8 File Offset: 0x0019FFB8
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Add_00004DE2$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E15 RID: 19989 RVA: 0x001A1DD0 File Offset: 0x0019FFD0
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Add_00004DE2$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Add$BurstManaged(L);
			}

			// Token: 0x04006127 RID: 24871
			private static IntPtr Pointer;
		}

		// Token: 0x02000C2E RID: 3118
		// (Invoke) Token: 0x06004E17 RID: 19991
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Sub_00004DE3$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C2F RID: 3119
		internal static class Sub_00004DE3$BurstDirectCall
		{
			// Token: 0x06004E1A RID: 19994 RVA: 0x001A1E04 File Offset: 0x001A0004
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Sub_00004DE3$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Sub_00004DE3$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Sub_00004DE3$PostfixBurstDelegate>(new Bindings.Vec3Functions.Sub_00004DE3$PostfixBurstDelegate(Bindings.Vec3Functions.Sub)).Value;
				}
				A_0 = Bindings.Vec3Functions.Sub_00004DE3$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E1B RID: 19995 RVA: 0x001A1E44 File Offset: 0x001A0044
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Sub_00004DE3$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E1C RID: 19996 RVA: 0x001A1E5C File Offset: 0x001A005C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Sub_00004DE3$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Sub$BurstManaged(L);
			}

			// Token: 0x04006128 RID: 24872
			private static IntPtr Pointer;
		}

		// Token: 0x02000C30 RID: 3120
		// (Invoke) Token: 0x06004E1E RID: 19998
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Mul_00004DE4$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C31 RID: 3121
		internal static class Mul_00004DE4$BurstDirectCall
		{
			// Token: 0x06004E21 RID: 20001 RVA: 0x001A1E90 File Offset: 0x001A0090
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Mul_00004DE4$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Mul_00004DE4$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Mul_00004DE4$PostfixBurstDelegate>(new Bindings.Vec3Functions.Mul_00004DE4$PostfixBurstDelegate(Bindings.Vec3Functions.Mul)).Value;
				}
				A_0 = Bindings.Vec3Functions.Mul_00004DE4$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E22 RID: 20002 RVA: 0x001A1ED0 File Offset: 0x001A00D0
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Mul_00004DE4$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E23 RID: 20003 RVA: 0x001A1EE8 File Offset: 0x001A00E8
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Mul_00004DE4$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Mul$BurstManaged(L);
			}

			// Token: 0x04006129 RID: 24873
			private static IntPtr Pointer;
		}

		// Token: 0x02000C32 RID: 3122
		// (Invoke) Token: 0x06004E25 RID: 20005
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Div_00004DE5$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C33 RID: 3123
		internal static class Div_00004DE5$BurstDirectCall
		{
			// Token: 0x06004E28 RID: 20008 RVA: 0x001A1F1C File Offset: 0x001A011C
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Div_00004DE5$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Div_00004DE5$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Div_00004DE5$PostfixBurstDelegate>(new Bindings.Vec3Functions.Div_00004DE5$PostfixBurstDelegate(Bindings.Vec3Functions.Div)).Value;
				}
				A_0 = Bindings.Vec3Functions.Div_00004DE5$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E29 RID: 20009 RVA: 0x001A1F5C File Offset: 0x001A015C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Div_00004DE5$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E2A RID: 20010 RVA: 0x001A1F74 File Offset: 0x001A0174
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Div_00004DE5$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Div$BurstManaged(L);
			}

			// Token: 0x0400612A RID: 24874
			private static IntPtr Pointer;
		}

		// Token: 0x02000C34 RID: 3124
		// (Invoke) Token: 0x06004E2C RID: 20012
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Unm_00004DE6$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C35 RID: 3125
		internal static class Unm_00004DE6$BurstDirectCall
		{
			// Token: 0x06004E2F RID: 20015 RVA: 0x001A1FA8 File Offset: 0x001A01A8
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Unm_00004DE6$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Unm_00004DE6$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Unm_00004DE6$PostfixBurstDelegate>(new Bindings.Vec3Functions.Unm_00004DE6$PostfixBurstDelegate(Bindings.Vec3Functions.Unm)).Value;
				}
				A_0 = Bindings.Vec3Functions.Unm_00004DE6$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E30 RID: 20016 RVA: 0x001A1FE8 File Offset: 0x001A01E8
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Unm_00004DE6$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E31 RID: 20017 RVA: 0x001A2000 File Offset: 0x001A0200
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Unm_00004DE6$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Unm$BurstManaged(L);
			}

			// Token: 0x0400612B RID: 24875
			private static IntPtr Pointer;
		}

		// Token: 0x02000C36 RID: 3126
		// (Invoke) Token: 0x06004E33 RID: 20019
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Eq_00004DE7$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C37 RID: 3127
		internal static class Eq_00004DE7$BurstDirectCall
		{
			// Token: 0x06004E36 RID: 20022 RVA: 0x001A2034 File Offset: 0x001A0234
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Eq_00004DE7$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Eq_00004DE7$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Eq_00004DE7$PostfixBurstDelegate>(new Bindings.Vec3Functions.Eq_00004DE7$PostfixBurstDelegate(Bindings.Vec3Functions.Eq)).Value;
				}
				A_0 = Bindings.Vec3Functions.Eq_00004DE7$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E37 RID: 20023 RVA: 0x001A2074 File Offset: 0x001A0274
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Eq_00004DE7$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E38 RID: 20024 RVA: 0x001A208C File Offset: 0x001A028C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Eq_00004DE7$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Eq$BurstManaged(L);
			}

			// Token: 0x0400612C RID: 24876
			private static IntPtr Pointer;
		}

		// Token: 0x02000C38 RID: 3128
		// (Invoke) Token: 0x06004E3A RID: 20026
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Dot_00004DE9$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C39 RID: 3129
		internal static class Dot_00004DE9$BurstDirectCall
		{
			// Token: 0x06004E3D RID: 20029 RVA: 0x001A20C0 File Offset: 0x001A02C0
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Dot_00004DE9$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Dot_00004DE9$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Dot_00004DE9$PostfixBurstDelegate>(new Bindings.Vec3Functions.Dot_00004DE9$PostfixBurstDelegate(Bindings.Vec3Functions.Dot)).Value;
				}
				A_0 = Bindings.Vec3Functions.Dot_00004DE9$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E3E RID: 20030 RVA: 0x001A2100 File Offset: 0x001A0300
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Dot_00004DE9$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E3F RID: 20031 RVA: 0x001A2118 File Offset: 0x001A0318
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Dot_00004DE9$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Dot$BurstManaged(L);
			}

			// Token: 0x0400612D RID: 24877
			private static IntPtr Pointer;
		}

		// Token: 0x02000C3A RID: 3130
		// (Invoke) Token: 0x06004E41 RID: 20033
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Cross_00004DEA$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C3B RID: 3131
		internal static class Cross_00004DEA$BurstDirectCall
		{
			// Token: 0x06004E44 RID: 20036 RVA: 0x001A214C File Offset: 0x001A034C
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Cross_00004DEA$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Cross_00004DEA$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Cross_00004DEA$PostfixBurstDelegate>(new Bindings.Vec3Functions.Cross_00004DEA$PostfixBurstDelegate(Bindings.Vec3Functions.Cross)).Value;
				}
				A_0 = Bindings.Vec3Functions.Cross_00004DEA$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E45 RID: 20037 RVA: 0x001A218C File Offset: 0x001A038C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Cross_00004DEA$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E46 RID: 20038 RVA: 0x001A21A4 File Offset: 0x001A03A4
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Cross_00004DEA$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Cross$BurstManaged(L);
			}

			// Token: 0x0400612E RID: 24878
			private static IntPtr Pointer;
		}

		// Token: 0x02000C3C RID: 3132
		// (Invoke) Token: 0x06004E48 RID: 20040
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Project_00004DEB$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C3D RID: 3133
		internal static class Project_00004DEB$BurstDirectCall
		{
			// Token: 0x06004E4B RID: 20043 RVA: 0x001A21D8 File Offset: 0x001A03D8
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Project_00004DEB$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Project_00004DEB$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Project_00004DEB$PostfixBurstDelegate>(new Bindings.Vec3Functions.Project_00004DEB$PostfixBurstDelegate(Bindings.Vec3Functions.Project)).Value;
				}
				A_0 = Bindings.Vec3Functions.Project_00004DEB$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E4C RID: 20044 RVA: 0x001A2218 File Offset: 0x001A0418
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Project_00004DEB$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E4D RID: 20045 RVA: 0x001A2230 File Offset: 0x001A0430
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Project_00004DEB$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Project$BurstManaged(L);
			}

			// Token: 0x0400612F RID: 24879
			private static IntPtr Pointer;
		}

		// Token: 0x02000C3E RID: 3134
		// (Invoke) Token: 0x06004E4F RID: 20047
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Length_00004DEC$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C3F RID: 3135
		internal static class Length_00004DEC$BurstDirectCall
		{
			// Token: 0x06004E52 RID: 20050 RVA: 0x001A2264 File Offset: 0x001A0464
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Length_00004DEC$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Length_00004DEC$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Length_00004DEC$PostfixBurstDelegate>(new Bindings.Vec3Functions.Length_00004DEC$PostfixBurstDelegate(Bindings.Vec3Functions.Length)).Value;
				}
				A_0 = Bindings.Vec3Functions.Length_00004DEC$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E53 RID: 20051 RVA: 0x001A22A4 File Offset: 0x001A04A4
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Length_00004DEC$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E54 RID: 20052 RVA: 0x001A22BC File Offset: 0x001A04BC
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Length_00004DEC$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Length$BurstManaged(L);
			}

			// Token: 0x04006130 RID: 24880
			private static IntPtr Pointer;
		}

		// Token: 0x02000C40 RID: 3136
		// (Invoke) Token: 0x06004E56 RID: 20054
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Normalize_00004DED$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C41 RID: 3137
		internal static class Normalize_00004DED$BurstDirectCall
		{
			// Token: 0x06004E59 RID: 20057 RVA: 0x001A22F0 File Offset: 0x001A04F0
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Normalize_00004DED$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Normalize_00004DED$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Normalize_00004DED$PostfixBurstDelegate>(new Bindings.Vec3Functions.Normalize_00004DED$PostfixBurstDelegate(Bindings.Vec3Functions.Normalize)).Value;
				}
				A_0 = Bindings.Vec3Functions.Normalize_00004DED$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E5A RID: 20058 RVA: 0x001A2330 File Offset: 0x001A0530
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Normalize_00004DED$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E5B RID: 20059 RVA: 0x001A2348 File Offset: 0x001A0548
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Normalize_00004DED$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Normalize$BurstManaged(L);
			}

			// Token: 0x04006131 RID: 24881
			private static IntPtr Pointer;
		}

		// Token: 0x02000C42 RID: 3138
		// (Invoke) Token: 0x06004E5D RID: 20061
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int SafeNormal_00004DEE$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C43 RID: 3139
		internal static class SafeNormal_00004DEE$BurstDirectCall
		{
			// Token: 0x06004E60 RID: 20064 RVA: 0x001A237C File Offset: 0x001A057C
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.SafeNormal_00004DEE$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.SafeNormal_00004DEE$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.SafeNormal_00004DEE$PostfixBurstDelegate>(new Bindings.Vec3Functions.SafeNormal_00004DEE$PostfixBurstDelegate(Bindings.Vec3Functions.SafeNormal)).Value;
				}
				A_0 = Bindings.Vec3Functions.SafeNormal_00004DEE$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E61 RID: 20065 RVA: 0x001A23BC File Offset: 0x001A05BC
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.SafeNormal_00004DEE$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E62 RID: 20066 RVA: 0x001A23D4 File Offset: 0x001A05D4
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.SafeNormal_00004DEE$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.SafeNormal$BurstManaged(L);
			}

			// Token: 0x04006132 RID: 24882
			private static IntPtr Pointer;
		}

		// Token: 0x02000C44 RID: 3140
		// (Invoke) Token: 0x06004E64 RID: 20068
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Distance_00004DEF$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C45 RID: 3141
		internal static class Distance_00004DEF$BurstDirectCall
		{
			// Token: 0x06004E67 RID: 20071 RVA: 0x001A2408 File Offset: 0x001A0608
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Distance_00004DEF$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Distance_00004DEF$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Distance_00004DEF$PostfixBurstDelegate>(new Bindings.Vec3Functions.Distance_00004DEF$PostfixBurstDelegate(Bindings.Vec3Functions.Distance)).Value;
				}
				A_0 = Bindings.Vec3Functions.Distance_00004DEF$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E68 RID: 20072 RVA: 0x001A2448 File Offset: 0x001A0648
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Distance_00004DEF$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E69 RID: 20073 RVA: 0x001A2460 File Offset: 0x001A0660
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Distance_00004DEF$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Distance$BurstManaged(L);
			}

			// Token: 0x04006133 RID: 24883
			private static IntPtr Pointer;
		}

		// Token: 0x02000C46 RID: 3142
		// (Invoke) Token: 0x06004E6B RID: 20075
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Lerp_00004DF0$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C47 RID: 3143
		internal static class Lerp_00004DF0$BurstDirectCall
		{
			// Token: 0x06004E6E RID: 20078 RVA: 0x001A2494 File Offset: 0x001A0694
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Lerp_00004DF0$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Lerp_00004DF0$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Lerp_00004DF0$PostfixBurstDelegate>(new Bindings.Vec3Functions.Lerp_00004DF0$PostfixBurstDelegate(Bindings.Vec3Functions.Lerp)).Value;
				}
				A_0 = Bindings.Vec3Functions.Lerp_00004DF0$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E6F RID: 20079 RVA: 0x001A24D4 File Offset: 0x001A06D4
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Lerp_00004DF0$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E70 RID: 20080 RVA: 0x001A24EC File Offset: 0x001A06EC
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Lerp_00004DF0$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Lerp$BurstManaged(L);
			}

			// Token: 0x04006134 RID: 24884
			private static IntPtr Pointer;
		}

		// Token: 0x02000C48 RID: 3144
		// (Invoke) Token: 0x06004E72 RID: 20082
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Rotate_00004DF1$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C49 RID: 3145
		internal static class Rotate_00004DF1$BurstDirectCall
		{
			// Token: 0x06004E75 RID: 20085 RVA: 0x001A2520 File Offset: 0x001A0720
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Rotate_00004DF1$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Rotate_00004DF1$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.Rotate_00004DF1$PostfixBurstDelegate>(new Bindings.Vec3Functions.Rotate_00004DF1$PostfixBurstDelegate(Bindings.Vec3Functions.Rotate)).Value;
				}
				A_0 = Bindings.Vec3Functions.Rotate_00004DF1$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E76 RID: 20086 RVA: 0x001A2560 File Offset: 0x001A0760
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Rotate_00004DF1$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E77 RID: 20087 RVA: 0x001A2578 File Offset: 0x001A0778
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Rotate_00004DF1$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Rotate$BurstManaged(L);
			}

			// Token: 0x04006135 RID: 24885
			private static IntPtr Pointer;
		}

		// Token: 0x02000C4A RID: 3146
		// (Invoke) Token: 0x06004E79 RID: 20089
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int ZeroVector_00004DF2$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C4B RID: 3147
		internal static class ZeroVector_00004DF2$BurstDirectCall
		{
			// Token: 0x06004E7C RID: 20092 RVA: 0x001A25AC File Offset: 0x001A07AC
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.ZeroVector_00004DF2$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.ZeroVector_00004DF2$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.ZeroVector_00004DF2$PostfixBurstDelegate>(new Bindings.Vec3Functions.ZeroVector_00004DF2$PostfixBurstDelegate(Bindings.Vec3Functions.ZeroVector)).Value;
				}
				A_0 = Bindings.Vec3Functions.ZeroVector_00004DF2$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E7D RID: 20093 RVA: 0x001A25EC File Offset: 0x001A07EC
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.ZeroVector_00004DF2$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E7E RID: 20094 RVA: 0x001A2604 File Offset: 0x001A0804
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.ZeroVector_00004DF2$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.ZeroVector$BurstManaged(L);
			}

			// Token: 0x04006136 RID: 24886
			private static IntPtr Pointer;
		}

		// Token: 0x02000C4C RID: 3148
		// (Invoke) Token: 0x06004E80 RID: 20096
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int OneVector_00004DF3$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C4D RID: 3149
		internal static class OneVector_00004DF3$BurstDirectCall
		{
			// Token: 0x06004E83 RID: 20099 RVA: 0x001A2638 File Offset: 0x001A0838
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.OneVector_00004DF3$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.OneVector_00004DF3$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.OneVector_00004DF3$PostfixBurstDelegate>(new Bindings.Vec3Functions.OneVector_00004DF3$PostfixBurstDelegate(Bindings.Vec3Functions.OneVector)).Value;
				}
				A_0 = Bindings.Vec3Functions.OneVector_00004DF3$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E84 RID: 20100 RVA: 0x001A2678 File Offset: 0x001A0878
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.OneVector_00004DF3$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E85 RID: 20101 RVA: 0x001A2690 File Offset: 0x001A0890
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.OneVector_00004DF3$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.OneVector$BurstManaged(L);
			}

			// Token: 0x04006137 RID: 24887
			private static IntPtr Pointer;
		}

		// Token: 0x02000C4E RID: 3150
		// (Invoke) Token: 0x06004E87 RID: 20103
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int NearlyEqual_00004DF4$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C4F RID: 3151
		internal static class NearlyEqual_00004DF4$BurstDirectCall
		{
			// Token: 0x06004E8A RID: 20106 RVA: 0x001A26C4 File Offset: 0x001A08C4
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.NearlyEqual_00004DF4$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.NearlyEqual_00004DF4$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.Vec3Functions.NearlyEqual_00004DF4$PostfixBurstDelegate>(new Bindings.Vec3Functions.NearlyEqual_00004DF4$PostfixBurstDelegate(Bindings.Vec3Functions.NearlyEqual)).Value;
				}
				A_0 = Bindings.Vec3Functions.NearlyEqual_00004DF4$BurstDirectCall.Pointer;
			}

			// Token: 0x06004E8B RID: 20107 RVA: 0x001A2704 File Offset: 0x001A0904
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.NearlyEqual_00004DF4$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004E8C RID: 20108 RVA: 0x001A271C File Offset: 0x001A091C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.NearlyEqual_00004DF4$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.NearlyEqual$BurstManaged(L);
			}

			// Token: 0x04006138 RID: 24888
			private static IntPtr Pointer;
		}
	}

	// Token: 0x02000C50 RID: 3152
	[BurstCompile]
	public static class QuatFunctions
	{
		// Token: 0x06004E8D RID: 20109 RVA: 0x001A274D File Offset: 0x001A094D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int New(lua_State* L)
		{
			return Bindings.QuatFunctions.New_00004DF5$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004E8E RID: 20110 RVA: 0x001A2755 File Offset: 0x001A0955
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Mul(lua_State* L)
		{
			return Bindings.QuatFunctions.Mul_00004DF6$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004E8F RID: 20111 RVA: 0x001A275D File Offset: 0x001A095D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Eq(lua_State* L)
		{
			return Bindings.QuatFunctions.Eq_00004DF7$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004E90 RID: 20112 RVA: 0x001A2768 File Offset: 0x001A0968
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ToString(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			Luau.lua_pushstring(L, quaternion.ToString());
			return 1;
		}

		// Token: 0x06004E91 RID: 20113 RVA: 0x001A27A1 File Offset: 0x001A09A1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FromEuler(lua_State* L)
		{
			return Bindings.QuatFunctions.FromEuler_00004DF9$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004E92 RID: 20114 RVA: 0x001A27A9 File Offset: 0x001A09A9
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FromDirection(lua_State* L)
		{
			return Bindings.QuatFunctions.FromDirection_00004DFA$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004E93 RID: 20115 RVA: 0x001A27B1 File Offset: 0x001A09B1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetUpVector(lua_State* L)
		{
			return Bindings.QuatFunctions.GetUpVector_00004DFB$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004E94 RID: 20116 RVA: 0x001A27B9 File Offset: 0x001A09B9
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Euler(lua_State* L)
		{
			return Bindings.QuatFunctions.Euler_00004DFC$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06004E95 RID: 20117 RVA: 0x001A27C4 File Offset: 0x001A09C4
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int New$BurstManaged(lua_State* L)
		{
			Quaternion* ptr = Luau.lua_class_push<Quaternion>(L, "Quat");
			ptr->x = (float)Luau.luaL_optnumber(L, 1, 0.0);
			ptr->y = (float)Luau.luaL_optnumber(L, 2, 0.0);
			ptr->z = (float)Luau.luaL_optnumber(L, 3, 0.0);
			ptr->w = (float)Luau.luaL_optnumber(L, 4, 0.0);
			return 1;
		}

		// Token: 0x06004E96 RID: 20118 RVA: 0x001A2840 File Offset: 0x001A0A40
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Mul$BurstManaged(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			Quaternion quaternion2 = *Luau.lua_class_get<Quaternion>(L, 2, "Quat");
			*Luau.lua_class_push<Quaternion>(L, "Quat") = quaternion * quaternion2;
			return 1;
		}

		// Token: 0x06004E97 RID: 20119 RVA: 0x001A2898 File Offset: 0x001A0A98
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Eq$BurstManaged(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			Quaternion quaternion2 = *Luau.lua_class_get<Quaternion>(L, 2, "Quat");
			int num = ((quaternion == quaternion2) ? 1 : 0);
			Luau.lua_pushnumber(L, (double)num);
			return 1;
		}

		// Token: 0x06004E98 RID: 20120 RVA: 0x001A28E8 File Offset: 0x001A0AE8
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int FromEuler$BurstManaged(lua_State* L)
		{
			float num = (float)Luau.luaL_optnumber(L, 1, 0.0);
			float num2 = (float)Luau.luaL_optnumber(L, 2, 0.0);
			float num3 = (float)Luau.luaL_optnumber(L, 3, 0.0);
			Luau.lua_class_push<Quaternion>(L, "Quat")->eulerAngles = new Vector3(num, num2, num3);
			return 1;
		}

		// Token: 0x06004E99 RID: 20121 RVA: 0x001A294C File Offset: 0x001A0B4C
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int FromDirection$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Luau.lua_class_push<Quaternion>(L, "Quat")->SetLookRotation(vector);
			return 1;
		}

		// Token: 0x06004E9A RID: 20122 RVA: 0x001A2988 File Offset: 0x001A0B88
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int GetUpVector$BurstManaged(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = quaternion * Vector3.up;
			return 1;
		}

		// Token: 0x06004E9B RID: 20123 RVA: 0x001A29D0 File Offset: 0x001A0BD0
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Euler$BurstManaged(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = quaternion.eulerAngles;
			return 1;
		}

		// Token: 0x02000C51 RID: 3153
		// (Invoke) Token: 0x06004E9D RID: 20125
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int New_00004DF5$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C52 RID: 3154
		internal static class New_00004DF5$BurstDirectCall
		{
			// Token: 0x06004EA0 RID: 20128 RVA: 0x001A2A14 File Offset: 0x001A0C14
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.New_00004DF5$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.New_00004DF5$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.New_00004DF5$PostfixBurstDelegate>(new Bindings.QuatFunctions.New_00004DF5$PostfixBurstDelegate(Bindings.QuatFunctions.New)).Value;
				}
				A_0 = Bindings.QuatFunctions.New_00004DF5$BurstDirectCall.Pointer;
			}

			// Token: 0x06004EA1 RID: 20129 RVA: 0x001A2A54 File Offset: 0x001A0C54
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.QuatFunctions.New_00004DF5$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004EA2 RID: 20130 RVA: 0x001A2A6C File Offset: 0x001A0C6C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.New_00004DF5$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.New$BurstManaged(L);
			}

			// Token: 0x04006139 RID: 24889
			private static IntPtr Pointer;
		}

		// Token: 0x02000C53 RID: 3155
		// (Invoke) Token: 0x06004EA4 RID: 20132
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Mul_00004DF6$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C54 RID: 3156
		internal static class Mul_00004DF6$BurstDirectCall
		{
			// Token: 0x06004EA7 RID: 20135 RVA: 0x001A2AA0 File Offset: 0x001A0CA0
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.Mul_00004DF6$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.Mul_00004DF6$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.Mul_00004DF6$PostfixBurstDelegate>(new Bindings.QuatFunctions.Mul_00004DF6$PostfixBurstDelegate(Bindings.QuatFunctions.Mul)).Value;
				}
				A_0 = Bindings.QuatFunctions.Mul_00004DF6$BurstDirectCall.Pointer;
			}

			// Token: 0x06004EA8 RID: 20136 RVA: 0x001A2AE0 File Offset: 0x001A0CE0
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.QuatFunctions.Mul_00004DF6$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004EA9 RID: 20137 RVA: 0x001A2AF8 File Offset: 0x001A0CF8
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.Mul_00004DF6$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.Mul$BurstManaged(L);
			}

			// Token: 0x0400613A RID: 24890
			private static IntPtr Pointer;
		}

		// Token: 0x02000C55 RID: 3157
		// (Invoke) Token: 0x06004EAB RID: 20139
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Eq_00004DF7$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C56 RID: 3158
		internal static class Eq_00004DF7$BurstDirectCall
		{
			// Token: 0x06004EAE RID: 20142 RVA: 0x001A2B2C File Offset: 0x001A0D2C
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.Eq_00004DF7$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.Eq_00004DF7$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.Eq_00004DF7$PostfixBurstDelegate>(new Bindings.QuatFunctions.Eq_00004DF7$PostfixBurstDelegate(Bindings.QuatFunctions.Eq)).Value;
				}
				A_0 = Bindings.QuatFunctions.Eq_00004DF7$BurstDirectCall.Pointer;
			}

			// Token: 0x06004EAF RID: 20143 RVA: 0x001A2B6C File Offset: 0x001A0D6C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.QuatFunctions.Eq_00004DF7$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004EB0 RID: 20144 RVA: 0x001A2B84 File Offset: 0x001A0D84
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.Eq_00004DF7$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.Eq$BurstManaged(L);
			}

			// Token: 0x0400613B RID: 24891
			private static IntPtr Pointer;
		}

		// Token: 0x02000C57 RID: 3159
		// (Invoke) Token: 0x06004EB2 RID: 20146
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int FromEuler_00004DF9$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C58 RID: 3160
		internal static class FromEuler_00004DF9$BurstDirectCall
		{
			// Token: 0x06004EB5 RID: 20149 RVA: 0x001A2BB8 File Offset: 0x001A0DB8
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.FromEuler_00004DF9$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.FromEuler_00004DF9$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.FromEuler_00004DF9$PostfixBurstDelegate>(new Bindings.QuatFunctions.FromEuler_00004DF9$PostfixBurstDelegate(Bindings.QuatFunctions.FromEuler)).Value;
				}
				A_0 = Bindings.QuatFunctions.FromEuler_00004DF9$BurstDirectCall.Pointer;
			}

			// Token: 0x06004EB6 RID: 20150 RVA: 0x001A2BF8 File Offset: 0x001A0DF8
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.QuatFunctions.FromEuler_00004DF9$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004EB7 RID: 20151 RVA: 0x001A2C10 File Offset: 0x001A0E10
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.FromEuler_00004DF9$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.FromEuler$BurstManaged(L);
			}

			// Token: 0x0400613C RID: 24892
			private static IntPtr Pointer;
		}

		// Token: 0x02000C59 RID: 3161
		// (Invoke) Token: 0x06004EB9 RID: 20153
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int FromDirection_00004DFA$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C5A RID: 3162
		internal static class FromDirection_00004DFA$BurstDirectCall
		{
			// Token: 0x06004EBC RID: 20156 RVA: 0x001A2C44 File Offset: 0x001A0E44
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.FromDirection_00004DFA$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.FromDirection_00004DFA$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.FromDirection_00004DFA$PostfixBurstDelegate>(new Bindings.QuatFunctions.FromDirection_00004DFA$PostfixBurstDelegate(Bindings.QuatFunctions.FromDirection)).Value;
				}
				A_0 = Bindings.QuatFunctions.FromDirection_00004DFA$BurstDirectCall.Pointer;
			}

			// Token: 0x06004EBD RID: 20157 RVA: 0x001A2C84 File Offset: 0x001A0E84
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.QuatFunctions.FromDirection_00004DFA$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004EBE RID: 20158 RVA: 0x001A2C9C File Offset: 0x001A0E9C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.FromDirection_00004DFA$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.FromDirection$BurstManaged(L);
			}

			// Token: 0x0400613D RID: 24893
			private static IntPtr Pointer;
		}

		// Token: 0x02000C5B RID: 3163
		// (Invoke) Token: 0x06004EC0 RID: 20160
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int GetUpVector_00004DFB$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C5C RID: 3164
		internal static class GetUpVector_00004DFB$BurstDirectCall
		{
			// Token: 0x06004EC3 RID: 20163 RVA: 0x001A2CD0 File Offset: 0x001A0ED0
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.GetUpVector_00004DFB$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.GetUpVector_00004DFB$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.GetUpVector_00004DFB$PostfixBurstDelegate>(new Bindings.QuatFunctions.GetUpVector_00004DFB$PostfixBurstDelegate(Bindings.QuatFunctions.GetUpVector)).Value;
				}
				A_0 = Bindings.QuatFunctions.GetUpVector_00004DFB$BurstDirectCall.Pointer;
			}

			// Token: 0x06004EC4 RID: 20164 RVA: 0x001A2D10 File Offset: 0x001A0F10
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.QuatFunctions.GetUpVector_00004DFB$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004EC5 RID: 20165 RVA: 0x001A2D28 File Offset: 0x001A0F28
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.GetUpVector_00004DFB$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.GetUpVector$BurstManaged(L);
			}

			// Token: 0x0400613E RID: 24894
			private static IntPtr Pointer;
		}

		// Token: 0x02000C5D RID: 3165
		// (Invoke) Token: 0x06004EC7 RID: 20167
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Euler_00004DFC$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000C5E RID: 3166
		internal static class Euler_00004DFC$BurstDirectCall
		{
			// Token: 0x06004ECA RID: 20170 RVA: 0x001A2D5C File Offset: 0x001A0F5C
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.Euler_00004DFC$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.Euler_00004DFC$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<Bindings.QuatFunctions.Euler_00004DFC$PostfixBurstDelegate>(new Bindings.QuatFunctions.Euler_00004DFC$PostfixBurstDelegate(Bindings.QuatFunctions.Euler)).Value;
				}
				A_0 = Bindings.QuatFunctions.Euler_00004DFC$BurstDirectCall.Pointer;
			}

			// Token: 0x06004ECB RID: 20171 RVA: 0x001A2D9C File Offset: 0x001A0F9C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.QuatFunctions.Euler_00004DFC$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06004ECC RID: 20172 RVA: 0x001A2DB4 File Offset: 0x001A0FB4
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.Euler_00004DFC$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.Euler$BurstManaged(L);
			}

			// Token: 0x0400613F RID: 24895
			private static IntPtr Pointer;
		}
	}

	// Token: 0x02000C5F RID: 3167
	public struct GorillaLocomotionSettings
	{
		// Token: 0x04006140 RID: 24896
		public float velocityLimit;

		// Token: 0x04006141 RID: 24897
		public float slideVelocityLimit;

		// Token: 0x04006142 RID: 24898
		public float maxJumpSpeed;

		// Token: 0x04006143 RID: 24899
		public float jumpMultiplier;
	}

	// Token: 0x02000C60 RID: 3168
	[BurstCompile]
	public struct PlayerInput
	{
		// Token: 0x04006144 RID: 24900
		public float leftXAxis;

		// Token: 0x04006145 RID: 24901
		[MarshalAs(UnmanagedType.U1)]
		public bool leftPrimaryButton;

		// Token: 0x04006146 RID: 24902
		public float rightXAxis;

		// Token: 0x04006147 RID: 24903
		[MarshalAs(UnmanagedType.U1)]
		public bool rightPrimaryButton;

		// Token: 0x04006148 RID: 24904
		public float leftYAxis;

		// Token: 0x04006149 RID: 24905
		[MarshalAs(UnmanagedType.U1)]
		public bool leftSecondaryButton;

		// Token: 0x0400614A RID: 24906
		public float rightYAxis;

		// Token: 0x0400614B RID: 24907
		[MarshalAs(UnmanagedType.U1)]
		public bool rightSecondaryButton;

		// Token: 0x0400614C RID: 24908
		public float leftTrigger;

		// Token: 0x0400614D RID: 24909
		public float rightTrigger;

		// Token: 0x0400614E RID: 24910
		public float leftGrip;

		// Token: 0x0400614F RID: 24911
		public float rightGrip;
	}

	// Token: 0x02000C61 RID: 3169
	public static class JSON
	{
		// Token: 0x06004ECD RID: 20173 RVA: 0x001A2DE8 File Offset: 0x001A0FE8
		public unsafe static Dictionary<object, object> ConsumeTable(lua_State* L, int tableIndex)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			Luau.lua_pushnil(L);
			if (tableIndex < 0)
			{
				tableIndex--;
			}
			while (Luau.lua_next(L, tableIndex) != 0)
			{
				Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, -1);
				Luau.lua_Types lua_Types2 = (Luau.lua_Types)Luau.lua_type(L, -2);
				object obj;
				if (lua_Types2 == Luau.lua_Types.LUA_TSTRING)
				{
					obj = new string(Luau.lua_tostring(L, -2));
				}
				else
				{
					if (lua_Types2 != Luau.lua_Types.LUA_TNUMBER)
					{
						FixedString64Bytes fixedString64Bytes = "Invalid key in table, key must be a string or a number";
						Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString64Bytes>(ref fixedString64Bytes) + 2));
						return null;
					}
					obj = Luau.lua_tonumber(L, -2);
				}
				switch (lua_Types)
				{
				case Luau.lua_Types.LUA_TBOOLEAN:
					dictionary.Add(obj, Luau.lua_toboolean(L, -1) == 1);
					Luau.lua_pop(L, 1);
					continue;
				case Luau.lua_Types.LUA_TNUMBER:
					dictionary.Add(obj, Luau.luaL_checknumber(L, -1));
					Luau.lua_pop(L, 1);
					continue;
				case Luau.lua_Types.LUA_TSTRING:
					dictionary.Add(obj, new string(Luau.lua_tostring(L, -1)));
					Luau.lua_pop(L, 1);
					continue;
				case Luau.lua_Types.LUA_TTABLE:
				case Luau.lua_Types.LUA_TUSERDATA:
					if (Luau.luaL_getmetafield(L, -1, "metahash") == 1)
					{
						BurstClassInfo.ClassInfo classInfo;
						if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
						{
							FixedString64Bytes fixedString64Bytes2 = "\"Internal Class Info Error No Metatable Found\"";
							Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString64Bytes>(ref fixedString64Bytes2) + 2));
							return null;
						}
						Luau.lua_pop(L, 1);
						FixedString32Bytes fixedString32Bytes = "Vec3";
						if ((in classInfo.Name) == (in fixedString32Bytes))
						{
							dictionary.Add(obj, *Luau.lua_class_get<Vector3>(L, -1));
							Luau.lua_pop(L, 1);
							continue;
						}
						fixedString32Bytes = "Quat";
						if ((in classInfo.Name) == (in fixedString32Bytes))
						{
							dictionary.Add(obj, *Luau.lua_class_get<Quaternion>(L, -1));
							Luau.lua_pop(L, 1);
							continue;
						}
						FixedString32Bytes fixedString32Bytes2 = "Invalid type in table";
						Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
						return null;
					}
					else
					{
						object obj2 = Bindings.JSON.ConsumeTable(L, -1);
						Luau.lua_pop(L, 1);
						if (obj2 != null)
						{
							dictionary.Add(obj, obj2);
							continue;
						}
						return null;
					}
					break;
				}
				FixedString32Bytes fixedString32Bytes3 = "Unknown type in table";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2));
				return null;
			}
			return dictionary;
		}

		// Token: 0x06004ECE RID: 20174 RVA: 0x001A3030 File Offset: 0x001A1230
		private static int ParseStrictInt(string input)
		{
			if (string.IsNullOrEmpty(input) || input != input.Trim())
			{
				return -1;
			}
			int num;
			if (!int.TryParse(input, out num))
			{
				return -1;
			}
			return num;
		}

		// Token: 0x06004ECF RID: 20175 RVA: 0x001A3064 File Offset: 0x001A1264
		private static bool CompareKeys(JObject obj, HashSet<string> set)
		{
			HashSet<string> hashSet = new HashSet<string>(from p in obj.Properties()
				select p.Name);
			return set.SetEquals(hashSet);
		}

		// Token: 0x06004ED0 RID: 20176 RVA: 0x001A30A8 File Offset: 0x001A12A8
		private unsafe static bool TryPushValue(lua_State* L, JToken value)
		{
			JObject jobject = value as JObject;
			if (jobject != null)
			{
				if (Bindings.JSON.CompareKeys(jobject, new HashSet<string> { "x", "y", "z" }))
				{
					float num = jobject["x"].ToObject<float>();
					float num2 = jobject["y"].ToObject<float>();
					float num3 = jobject["z"].ToObject<float>();
					Vector3 vector = new Vector3(num, num2, num3);
					*Luau.lua_class_push<Vector3>(L) = vector;
				}
				else if (Bindings.JSON.CompareKeys(jobject, new HashSet<string> { "x", "y", "z", "w" }))
				{
					float num4 = jobject["x"].ToObject<float>();
					float num5 = jobject["y"].ToObject<float>();
					float num6 = jobject["z"].ToObject<float>();
					float num7 = jobject["w"].ToObject<float>();
					Quaternion quaternion = new Quaternion(num4, num5, num6, num7);
					*Luau.lua_class_push<Quaternion>(L) = quaternion;
				}
				else
				{
					Bindings.JSON.PushTable(L, jobject);
				}
				return true;
			}
			JArray jarray = value as JArray;
			if (jarray != null)
			{
				Luau.lua_createtable(L, jarray.Count, 0);
				int num8 = 0;
				foreach (JToken jtoken in jarray)
				{
					if (jtoken != null && Bindings.JSON.TryPushValue(L, jtoken))
					{
						Luau.lua_rawseti(L, -2, ++num8);
					}
				}
				return true;
			}
			if (value is JValue)
			{
				JTokenType type = value.Type;
				if (type == JTokenType.Integer || type == JTokenType.Float)
				{
					Luau.lua_pushnumber(L, value.ToObject<double>());
					return true;
				}
				if (type == JTokenType.Boolean)
				{
					Luau.lua_pushboolean(L, value.ToObject<bool>() ? 1 : 0);
					return true;
				}
				if (type == JTokenType.String)
				{
					Luau.lua_pushstring(L, value.ToString());
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004ED1 RID: 20177 RVA: 0x001A32BC File Offset: 0x001A14BC
		public unsafe static bool PushTable(lua_State* L, JObject table)
		{
			Luau.lua_createtable(L, 0, 0);
			foreach (KeyValuePair<string, JToken> keyValuePair in table)
			{
				if (keyValuePair.Key != null && keyValuePair.Value != null)
				{
					int num = Bindings.JSON.ParseStrictInt(keyValuePair.Key);
					if (num == -1)
					{
						Luau.lua_pushstring(L, keyValuePair.Key);
					}
					if (!Bindings.JSON.TryPushValue(L, keyValuePair.Value))
					{
						if (num == -1)
						{
							Luau.lua_pop(L, 1);
						}
					}
					else if (num == -1)
					{
						Luau.lua_rawset(L, -3);
					}
					else
					{
						Luau.lua_rawseti(L, -2, num);
					}
				}
			}
			return true;
		}

		// Token: 0x06004ED2 RID: 20178 RVA: 0x001A336C File Offset: 0x001A156C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DataSave(lua_State* L)
		{
			int num;
			try
			{
				string text = Bindings.DataSaveUtils.ConvertForSave(Bindings.JSON.ConsumeTable(L, 1)).ToString(Formatting.Indented, Array.Empty<JsonConverter>());
				if (text.Length > 10000)
				{
					Luau.luaL_errorL(L, "Save exceeds 10000 bytes", Array.Empty<string>());
					num = 0;
				}
				else
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(Path.Join(Bindings.JSON.ModIODirectory, "saves", CustomMapLoader.LoadedMapModId.ToString()));
					if (!directoryInfo.Exists)
					{
						directoryInfo.Create();
					}
					File.WriteAllText(Path.Join(directoryInfo.FullName, "luau.json"), text);
					num = 0;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("DataSave failed: {0}", ex));
				Luau.luaL_errorL(L, "DataSave failed", Array.Empty<string>());
				num = 0;
			}
			return num;
		}

		// Token: 0x06004ED3 RID: 20179 RVA: 0x001A3458 File Offset: 0x001A1658
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DataLoad(lua_State* L)
		{
			int num;
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(Path.Join(Bindings.JSON.ModIODirectory, "saves", CustomMapLoader.LoadedMapModId.ToString()));
				if (!directoryInfo.Exists)
				{
					Luau.lua_createtable(L, 0, 0);
					num = 1;
				}
				else
				{
					FileInfo[] files = directoryInfo.GetFiles("luau.json");
					if (files.Length == 0)
					{
						Luau.lua_createtable(L, 0, 0);
						num = 1;
					}
					else
					{
						JObject jobject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(files[0].FullName));
						if (Bindings.JSON.PushTable(L, jobject))
						{
							num = 1;
						}
						else
						{
							num = 0;
						}
					}
				}
			}
			catch
			{
				Luau.luaL_errorL(L, "Error while loading data", Array.Empty<string>());
				num = 0;
			}
			return num;
		}

		// Token: 0x04006150 RID: 24912
		private static string ModIODirectory = ((Application.platform == RuntimePlatform.WindowsPlayer) ? Path.Join(Application.persistentDataPath, "luauDataSaves") : Path.Join(Path.Join(Application.persistentDataPath, "mod.io", "06657"), "data"));
	}

	// Token: 0x02000C63 RID: 3171
	[BurstCompile]
	public struct LuauRoomState
	{
		// Token: 0x04006153 RID: 24915
		[MarshalAs(UnmanagedType.U1)]
		public bool IsQuest;

		// Token: 0x04006154 RID: 24916
		public float FPS;

		// Token: 0x04006155 RID: 24917
		[MarshalAs(UnmanagedType.U1)]
		public bool IsPrivate;

		// Token: 0x04006156 RID: 24918
		public FixedString32Bytes RoomCode;
	}

	// Token: 0x02000C64 RID: 3172
	public static class PlayerUtils
	{
		// Token: 0x06004ED8 RID: 20184 RVA: 0x001A359C File Offset: 0x001A179C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int TeleportPlayer(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			bool flag = Luau.lua_toboolean(L, 2) == 1;
			if (GTPlayer.hasInstance)
			{
				GTPlayer instance = GTPlayer.Instance;
				Vector3 position = instance.transform.position;
				Vector3 vector2 = instance.mainCamera.transform.position - position;
				Vector3 vector3 = vector - vector2;
				instance.TeleportTo(vector3, instance.transform.rotation, flag, false);
			}
			return 0;
		}

		// Token: 0x06004ED9 RID: 20185 RVA: 0x001A361C File Offset: 0x001A181C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetVelocity(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1);
			if (GTPlayer.hasInstance)
			{
				GTPlayer.Instance.SetVelocity(vector);
			}
			return 0;
		}
	}

	// Token: 0x02000C65 RID: 3173
	public static class DataSaveUtils
	{
		// Token: 0x06004EDA RID: 20186 RVA: 0x001A364C File Offset: 0x001A184C
		public static JToken ConvertForSave(object value)
		{
			if (value == null)
			{
				return JValue.CreateNull();
			}
			if (value is Vector3)
			{
				return Bindings.DataSaveUtils.SerializeVector3((Vector3)value);
			}
			if (value is Quaternion)
			{
				return Bindings.DataSaveUtils.SerializeQuaternion((Quaternion)value);
			}
			if (value is Dictionary<object, object>)
			{
				Dictionary<object, object> dictionary = (Dictionary<object, object>)value;
				JObject jobject = new JObject();
				foreach (KeyValuePair<object, object> keyValuePair in dictionary)
				{
					if (keyValuePair.Key != null)
					{
						jobject[keyValuePair.Key.ToString()] = Bindings.DataSaveUtils.ConvertForSave(keyValuePair.Value);
					}
				}
				return jobject;
			}
			return JToken.FromObject(value);
		}

		// Token: 0x06004EDB RID: 20187 RVA: 0x001A3708 File Offset: 0x001A1908
		private static JObject SerializeVector3(Vector3 value)
		{
			JObject jobject = new JObject();
			jobject["x"] = value.x;
			jobject["y"] = value.y;
			jobject["z"] = value.z;
			return jobject;
		}

		// Token: 0x06004EDC RID: 20188 RVA: 0x001A375C File Offset: 0x001A195C
		private static JObject SerializeQuaternion(Quaternion value)
		{
			JObject jobject = new JObject();
			jobject["x"] = value.x;
			jobject["y"] = value.y;
			jobject["z"] = value.z;
			jobject["w"] = value.w;
			return jobject;
		}
	}

	// Token: 0x02000C66 RID: 3174
	public static class RayCastUtils
	{
		// Token: 0x06004EDD RID: 20189 RVA: 0x001A37C8 File Offset: 0x001A19C8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int RayCast(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			if (!Physics.Raycast(vector, vector2, out Bindings.RayCastUtils.rayHit))
			{
				return 0;
			}
			Luau.lua_createtable(L, 0, 0);
			Luau.lua_pushstring(L, "distance");
			Luau.lua_pushnumber(L, (double)Bindings.RayCastUtils.rayHit.distance);
			Luau.lua_rawset(L, -3);
			Luau.lua_pushstring(L, "point");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Bindings.RayCastUtils.rayHit.point;
			Luau.lua_rawset(L, -3);
			Luau.lua_pushstring(L, "normal");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Bindings.RayCastUtils.rayHit.normal;
			Luau.lua_rawset(L, -3);
			Luau.lua_pushstring(L, "object");
			IntPtr intPtr;
			if (Bindings.LuauGameObjectList.TryGetValue(Bindings.RayCastUtils.rayHit.transform.gameObject, out intPtr))
			{
				Luau.lua_class_push(L, "GameObject", intPtr);
			}
			else
			{
				Luau.lua_pushnil(L);
			}
			Luau.lua_rawset(L, -3);
			Luau.lua_pushstring(L, "player");
			Collider collider = Bindings.RayCastUtils.rayHit.collider;
			VRRig vrrig = ((collider != null) ? collider.GetComponentInParent<VRRig>() : null);
			if (vrrig != null)
			{
				NetPlayer creator = vrrig.creator;
				if (creator != null)
				{
					IntPtr intPtr2;
					if (Bindings.LuauPlayerList.TryGetValue(creator.ActorNumber, out intPtr2))
					{
						Luau.lua_class_push(L, "Player", intPtr2);
					}
					else
					{
						Luau.lua_pushnil(L);
					}
				}
				else
				{
					Luau.lua_pushnil(L);
				}
			}
			else
			{
				Luau.lua_pushnil(L);
			}
			Luau.lua_rawset(L, -3);
			return 1;
		}

		// Token: 0x04006157 RID: 24919
		public static RaycastHit rayHit;
	}

	// Token: 0x02000C67 RID: 3175
	public static class Components
	{
		// Token: 0x06004EDE RID: 20190 RVA: 0x001A396B File Offset: 0x001A1B6B
		public unsafe static void Build(lua_State* L)
		{
			Bindings.Components.LuauParticleSystemBindings.Builder(L);
			Bindings.Components.LuauAudioSourceBindings.Builder(L);
			Bindings.Components.LuauLightBindings.Builder(L);
			Bindings.Components.LuauAnimatorBindings.Builder(L);
		}

		// Token: 0x04006158 RID: 24920
		public static Dictionary<IntPtr, object> ComponentList = new Dictionary<IntPtr, object>();

		// Token: 0x02000C68 RID: 3176
		public static class LuauParticleSystemBindings
		{
			// Token: 0x06004EE0 RID: 20192 RVA: 0x001A3994 File Offset: 0x001A1B94
			public unsafe static void Builder(lua_State* L)
			{
				LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.Components.LuauParticleSystemBindings.LuauParticleSystem>("ParticleSystem").AddFunction("play", new lua_CFunction(Bindings.Components.LuauParticleSystemBindings.play)).AddFunction("stop", new lua_CFunction(Bindings.Components.LuauParticleSystemBindings.stop)).AddFunction("clear", new lua_CFunction(Bindings.Components.LuauParticleSystemBindings.clear))
					.Build(L, false));
			}

			// Token: 0x06004EE1 RID: 20193 RVA: 0x001A3A00 File Offset: 0x001A1C00
			public unsafe static ParticleSystem GetParticleSystem(lua_State* L)
			{
				Bindings.Components.LuauParticleSystemBindings.LuauParticleSystem* ptr = Luau.lua_class_get<Bindings.Components.LuauParticleSystemBindings.LuauParticleSystem>(L, 1);
				object obj;
				if (Bindings.Components.ComponentList.TryGetValue((IntPtr)((void*)ptr), out obj))
				{
					ParticleSystem particleSystem = obj as ParticleSystem;
					if (particleSystem != null)
					{
						return particleSystem;
					}
				}
				return null;
			}

			// Token: 0x06004EE2 RID: 20194 RVA: 0x001A3A38 File Offset: 0x001A1C38
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int play(lua_State* L)
			{
				ParticleSystem particleSystem = Bindings.Components.LuauParticleSystemBindings.GetParticleSystem(L);
				if (particleSystem != null)
				{
					particleSystem.Play();
				}
				return 0;
			}

			// Token: 0x06004EE3 RID: 20195 RVA: 0x001A3A5C File Offset: 0x001A1C5C
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int stop(lua_State* L)
			{
				ParticleSystem particleSystem = Bindings.Components.LuauParticleSystemBindings.GetParticleSystem(L);
				if (particleSystem != null)
				{
					particleSystem.Stop();
				}
				return 0;
			}

			// Token: 0x06004EE4 RID: 20196 RVA: 0x001A3A80 File Offset: 0x001A1C80
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int clear(lua_State* L)
			{
				ParticleSystem particleSystem = Bindings.Components.LuauParticleSystemBindings.GetParticleSystem(L);
				if (particleSystem != null)
				{
					particleSystem.Clear();
				}
				return 0;
			}

			// Token: 0x02000C69 RID: 3177
			public struct LuauParticleSystem
			{
				// Token: 0x04006159 RID: 24921
				public int x;
			}
		}

		// Token: 0x02000C6A RID: 3178
		public static class LuauAudioSourceBindings
		{
			// Token: 0x06004EE5 RID: 20197 RVA: 0x001A3AA4 File Offset: 0x001A1CA4
			public unsafe static void Builder(lua_State* L)
			{
				LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.Components.LuauAudioSourceBindings.LuauAudioSource>("AudioSource").AddFunction("play", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.play)).AddFunction("setVolume", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.setVolume)).AddFunction("setLoop", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.setLoop))
					.AddFunction("setPitch", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.setPitch))
					.AddFunction("setMinDistance", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.setMinDistance))
					.AddFunction("setMaxDistance", new lua_CFunction(Bindings.Components.LuauAudioSourceBindings.setMaxDistance))
					.Build(L, false));
			}

			// Token: 0x06004EE6 RID: 20198 RVA: 0x001A3B54 File Offset: 0x001A1D54
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static AudioSource GetAudioSource(lua_State* L)
			{
				Bindings.Components.LuauAudioSourceBindings.LuauAudioSource* ptr = Luau.lua_class_get<Bindings.Components.LuauAudioSourceBindings.LuauAudioSource>(L, 1);
				object obj;
				if (Bindings.Components.ComponentList.TryGetValue((IntPtr)((void*)ptr), out obj))
				{
					AudioSource audioSource = obj as AudioSource;
					if (audioSource != null)
					{
						return audioSource;
					}
				}
				return null;
			}

			// Token: 0x06004EE7 RID: 20199 RVA: 0x001A3B8C File Offset: 0x001A1D8C
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int play(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				if (audioSource != null)
				{
					audioSource.Play();
				}
				return 0;
			}

			// Token: 0x06004EE8 RID: 20200 RVA: 0x001A3BB0 File Offset: 0x001A1DB0
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setVolume(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (audioSource != null)
				{
					audioSource.volume = (float)num;
				}
				return 0;
			}

			// Token: 0x06004EE9 RID: 20201 RVA: 0x001A3BE0 File Offset: 0x001A1DE0
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setLoop(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				bool flag = Luau.lua_toboolean(L, 2) == 1;
				if (audioSource != null)
				{
					audioSource.loop = flag;
				}
				return 0;
			}

			// Token: 0x06004EEA RID: 20202 RVA: 0x001A3C10 File Offset: 0x001A1E10
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setPitch(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (audioSource != null)
				{
					audioSource.pitch = (float)num;
				}
				return 0;
			}

			// Token: 0x06004EEB RID: 20203 RVA: 0x001A3C40 File Offset: 0x001A1E40
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setMinDistance(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (audioSource != null)
				{
					audioSource.minDistance = (float)num;
				}
				return 0;
			}

			// Token: 0x06004EEC RID: 20204 RVA: 0x001A3C70 File Offset: 0x001A1E70
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setMaxDistance(lua_State* L)
			{
				AudioSource audioSource = Bindings.Components.LuauAudioSourceBindings.GetAudioSource(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (audioSource != null)
				{
					audioSource.maxDistance = (float)num;
				}
				return 0;
			}

			// Token: 0x02000C6B RID: 3179
			public struct LuauAudioSource
			{
				// Token: 0x0400615A RID: 24922
				public int x;
			}
		}

		// Token: 0x02000C6C RID: 3180
		public static class LuauLightBindings
		{
			// Token: 0x06004EED RID: 20205 RVA: 0x001A3CA0 File Offset: 0x001A1EA0
			public unsafe static void Builder(lua_State* L)
			{
				LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.Components.LuauLightBindings.LuauLight>("Light").AddFunction("setColor", new lua_CFunction(Bindings.Components.LuauLightBindings.setColor)).AddFunction("setIntensity", new lua_CFunction(Bindings.Components.LuauLightBindings.setIntensity)).AddFunction("setRange", new lua_CFunction(Bindings.Components.LuauLightBindings.setRange))
					.Build(L, false));
			}

			// Token: 0x06004EEE RID: 20206 RVA: 0x001A3D0C File Offset: 0x001A1F0C
			public unsafe static Light GetLight(lua_State* L)
			{
				Bindings.Components.LuauLightBindings.LuauLight* ptr = Luau.lua_class_get<Bindings.Components.LuauLightBindings.LuauLight>(L, 1);
				object obj;
				if (Bindings.Components.ComponentList.TryGetValue((IntPtr)((void*)ptr), out obj))
				{
					Light light = obj as Light;
					if (light != null)
					{
						return light;
					}
				}
				return null;
			}

			// Token: 0x06004EEF RID: 20207 RVA: 0x001A3D44 File Offset: 0x001A1F44
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setColor(lua_State* L)
			{
				Light light = Bindings.Components.LuauLightBindings.GetLight(L);
				Vector3 vector = *Luau.lua_class_get<Vector3>(L, 2);
				if (light != null)
				{
					light.color = new Color(vector.x, vector.y, vector.z);
				}
				return 0;
			}

			// Token: 0x06004EF0 RID: 20208 RVA: 0x001A3D8C File Offset: 0x001A1F8C
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setIntensity(lua_State* L)
			{
				Light light = Bindings.Components.LuauLightBindings.GetLight(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (light != null)
				{
					light.intensity = (float)num;
				}
				return 0;
			}

			// Token: 0x06004EF1 RID: 20209 RVA: 0x001A3DBC File Offset: 0x001A1FBC
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setRange(lua_State* L)
			{
				Light light = Bindings.Components.LuauLightBindings.GetLight(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (light != null)
				{
					light.range = (float)num;
				}
				return 0;
			}

			// Token: 0x02000C6D RID: 3181
			public struct LuauLight
			{
				// Token: 0x0400615B RID: 24923
				public int x;
			}
		}

		// Token: 0x02000C6E RID: 3182
		public static class LuauAnimatorBindings
		{
			// Token: 0x06004EF2 RID: 20210 RVA: 0x001A3DEC File Offset: 0x001A1FEC
			public unsafe static void Builder(lua_State* L)
			{
				LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.Components.LuauAnimatorBindings.LuauAnimator>("Animator").AddFunction("setSpeed", new lua_CFunction(Bindings.Components.LuauAnimatorBindings.setSpeed)).AddFunction("startPlayback", new lua_CFunction(Bindings.Components.LuauAnimatorBindings.startPlayback)).AddFunction("stopPlayback", new lua_CFunction(Bindings.Components.LuauAnimatorBindings.stopPlayback))
					.AddFunction("reset", new lua_CFunction(Bindings.Components.LuauAnimatorBindings.reset))
					.Build(L, false));
			}

			// Token: 0x06004EF3 RID: 20211 RVA: 0x001A3E70 File Offset: 0x001A2070
			public unsafe static Animator GetAnimator(lua_State* L)
			{
				Bindings.Components.LuauAnimatorBindings.LuauAnimator* ptr = Luau.lua_class_get<Bindings.Components.LuauAnimatorBindings.LuauAnimator>(L, 1);
				object obj;
				if (Bindings.Components.ComponentList.TryGetValue((IntPtr)((void*)ptr), out obj))
				{
					Animator animator = obj as Animator;
					if (animator != null)
					{
						return animator;
					}
				}
				return null;
			}

			// Token: 0x06004EF4 RID: 20212 RVA: 0x001A3EA8 File Offset: 0x001A20A8
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int setSpeed(lua_State* L)
			{
				Animator animator = Bindings.Components.LuauAnimatorBindings.GetAnimator(L);
				double num = Luau.luaL_checknumber(L, 2);
				if (animator != null)
				{
					animator.speed = (float)num;
				}
				return 0;
			}

			// Token: 0x06004EF5 RID: 20213 RVA: 0x001A3ED8 File Offset: 0x001A20D8
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int startPlayback(lua_State* L)
			{
				Animator animator = Bindings.Components.LuauAnimatorBindings.GetAnimator(L);
				if (animator != null)
				{
					animator.StartPlayback();
				}
				return 0;
			}

			// Token: 0x06004EF6 RID: 20214 RVA: 0x001A3EFC File Offset: 0x001A20FC
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int stopPlayback(lua_State* L)
			{
				Animator animator = Bindings.Components.LuauAnimatorBindings.GetAnimator(L);
				if (animator != null)
				{
					animator.StopPlayback();
				}
				return 0;
			}

			// Token: 0x06004EF7 RID: 20215 RVA: 0x001A3F20 File Offset: 0x001A2120
			[MonoPInvokeCallback(typeof(lua_CFunction))]
			public unsafe static int reset(lua_State* L)
			{
				Animator animator = Bindings.Components.LuauAnimatorBindings.GetAnimator(L);
				if (animator != null)
				{
					animator.ResetToEntryState();
				}
				return 0;
			}

			// Token: 0x02000C6F RID: 3183
			public struct LuauAnimator
			{
				// Token: 0x0400615C RID: 24924
				public int x;
			}
		}
	}

	// Token: 0x02000C70 RID: 3184
	[BurstCompile]
	public struct MInventoryItem
	{
		// Token: 0x0400615D RID: 24925
		public FixedString512Bytes Name;

		// Token: 0x0400615E RID: 24926
		public int Quantity;

		// Token: 0x0400615F RID: 24927
		public FixedString512Bytes InGameId;

		// Token: 0x04006160 RID: 24928
		public FixedString512Bytes DisplayName;

		// Token: 0x04006161 RID: 24929
		public FixedString512Bytes DisplayDescription;

		// Token: 0x04006162 RID: 24930
		public FixedString512Bytes ID;
	}

	// Token: 0x02000C71 RID: 3185
	[BurstCompile]
	public struct MOnlineError
	{
		// Token: 0x04006163 RID: 24931
		public FixedString512Bytes Name;

		// Token: 0x04006164 RID: 24932
		public FixedString512Bytes Message;

		// Token: 0x04006165 RID: 24933
		public FixedString64Bytes ErrorCode;

		// Token: 0x04006166 RID: 24934
		public int HttpCode;
	}

	// Token: 0x02000C72 RID: 3186
	[BurstCompile]
	public static class OnlineFunctions
	{
		// Token: 0x06004EF8 RID: 20216 RVA: 0x001A3F44 File Offset: 0x001A2144
		private unsafe static void DispatchOnlineError(lua_State* L, int errorCallbackRID, LuauScriptRunner runner, string name, string message, string errorCode, int httpCode)
		{
			Luau.lua_getref(L, errorCallbackRID);
			if (Luau.lua_type(L, -1) == 7)
			{
				Bindings.MOnlineError* ptr = Luau.lua_class_push<Bindings.MOnlineError>(L);
				(ref ptr->Name).CopyFromTruncated(name);
				(ref ptr->Message).CopyFromTruncated(message);
				(ref ptr->ErrorCode).CopyFromTruncated(errorCode);
				ptr->HttpCode = httpCode;
				int num = Luau.lua_pcall(L, 1, 0, 0);
				if (num != 0)
				{
					if (runner != null)
					{
						if (LuauScriptRunner.ErrorCheck(L, num))
						{
							runner.ShouldTick = false;
							return;
						}
					}
					else
					{
						sbyte* ptr2 = Luau.lua_tostring(L, -1);
						LuauHud.Instance.LuauLog(new string(ptr2));
						Luau.lua_pop(L, 1);
					}
				}
			}
			else
			{
				Luau.lua_pop(L, 1);
			}
			Luau.lua_unref(L, errorCallbackRID);
		}

		// Token: 0x06004EF9 RID: 20217 RVA: 0x001A3FE8 File Offset: 0x001A21E8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetLocalPlayerInventory(lua_State* L)
		{
			if (Luau.lua_type(L, 1) != 7)
			{
				Luau.luaL_errorL(L, "getInventory expects a callback function", Array.Empty<string>());
				return 0;
			}
			if (Luau.lua_type(L, 2) != 7)
			{
				Luau.luaL_errorL(L, "getInventory expects an error callback function", Array.Empty<string>());
				return 0;
			}
			bool flag = Bindings.OnlineFunctions.RateLimitBackendFunction(delegate
			{
				int callbackRID = Luau.lua_ref(L, 1);
				int errorCallbackRID = Luau.lua_ref(L, 2);
				bool userInventory = MothershipClientApiUnity.GetUserInventory(delegate(MothershipGetInventoryResponse Response)
				{
					Bindings.OnlineFunctions.ClearBackendFunctionInFlight("GetInventory");
					LuauScriptRunner luauScriptRunner = Bindings.OnlineFunctions.FindAliveScriptRunner(L);
					if (luauScriptRunner == null)
					{
						return;
					}
					Bindings.OnlineFunctions.CachedInventory = new Dictionary<string, Bindings.OnlineFunctions.CachedInventoryItem>();
					Luau.lua_getref(L, callbackRID);
					if (Luau.lua_type(L, -1) == 7)
					{
						foreach (KeyValuePair<string, MothershipPlayerInventorySummary> keyValuePair in Response.Results)
						{
							foreach (MothershipInventoryItemSummary mothershipInventoryItemSummary in keyValuePair.Value.entitlements)
							{
								if (Bindings.OnlineFunctions.CachedInventory.ContainsKey(mothershipInventoryItemSummary.entitlement_id))
								{
									Bindings.OnlineFunctions.CachedInventory[mothershipInventoryItemSummary.entitlement_id].Quantity += mothershipInventoryItemSummary.quantity;
								}
								else
								{
									Bindings.OnlineFunctions.CachedInventory.Add(mothershipInventoryItemSummary.entitlement_id, new Bindings.OnlineFunctions.CachedInventoryItem
									{
										DisplayDescription = mothershipInventoryItemSummary.display_description,
										DisplayName = mothershipInventoryItemSummary.display_name,
										ID = mothershipInventoryItemSummary.entitlement_id,
										InGameId = mothershipInventoryItemSummary.in_game_id,
										Name = mothershipInventoryItemSummary.name,
										Quantity = mothershipInventoryItemSummary.quantity
									});
								}
							}
						}
						Luau.lua_createtable(L, 0, 0);
						int num = 0;
						foreach (KeyValuePair<string, Bindings.OnlineFunctions.CachedInventoryItem> keyValuePair2 in Bindings.OnlineFunctions.CachedInventory)
						{
							if (keyValuePair2.Value.InGameId.Contains(CustomMapLoader.LoadedMapModId.ToString()) || keyValuePair2.Value.InGameId.Equals("geodes", StringComparison.OrdinalIgnoreCase))
							{
								Bindings.MInventoryItem* ptr = Luau.lua_class_push<Bindings.MInventoryItem>(L);
								(ref ptr->InGameId).CopyFromTruncated(keyValuePair2.Value.InGameId);
								ptr->Quantity = keyValuePair2.Value.Quantity;
								(ref ptr->Name).CopyFromTruncated(keyValuePair2.Value.Name);
								(ref ptr->DisplayDescription).CopyFromTruncated(keyValuePair2.Value.DisplayDescription);
								(ref ptr->DisplayName).CopyFromTruncated(keyValuePair2.Value.DisplayName);
								(ref ptr->ID).CopyFromTruncated(keyValuePair2.Value.ID);
								Luau.lua_rawseti(L, -2, ++num);
							}
						}
						int num2 = Luau.lua_pcall(L, 1, 0, 0);
						if (LuauScriptRunner.ErrorCheck(L, num2))
						{
							luauScriptRunner.ShouldTick = false;
							return;
						}
					}
					else
					{
						Luau.lua_pop(L, 1);
					}
					Luau.lua_unref(L, callbackRID);
					Luau.lua_unref(L, errorCallbackRID);
				}, delegate(MothershipError Error, int code)
				{
					Bindings.OnlineFunctions.ClearBackendFunctionInFlight("GetInventory");
					LuauScriptRunner luauScriptRunner2 = Bindings.OnlineFunctions.FindAliveScriptRunner(L);
					if (luauScriptRunner2 == null)
					{
						return;
					}
					Luau.lua_unref(L, callbackRID);
					Luau.lua_getref(L, errorCallbackRID);
					if (Luau.lua_type(L, -1) == 7)
					{
						Bindings.MOnlineError* ptr2 = Luau.lua_class_push<Bindings.MOnlineError>(L);
						(ref ptr2->Name).CopyFromTruncated(Error.Name);
						(ref ptr2->Message).CopyFromTruncated(Error.Message);
						(ref ptr2->ErrorCode).CopyFromTruncated(Error.MothershipErrorCode);
						ptr2->HttpCode = code;
						int num3 = Luau.lua_pcall(L, 1, 0, 0);
						if (LuauScriptRunner.ErrorCheck(L, num3))
						{
							luauScriptRunner2.ShouldTick = false;
							return;
						}
					}
					else
					{
						Luau.lua_pop(L, 1);
					}
					Luau.lua_unref(L, errorCallbackRID);
				});
				if (!userInventory)
				{
					Luau.lua_unref(L, callbackRID);
					Luau.lua_unref(L, errorCallbackRID);
				}
				return userInventory;
			}, "GetInventory");
			Luau.lua_pushboolean(L, flag ? 1 : 0);
			return 1;
		}

		// Token: 0x06004EFA RID: 20218 RVA: 0x001A4078 File Offset: 0x001A2278
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetInventoryForPlayer(lua_State* L)
		{
			Debug.Log("In get inventory for player");
			if (Luau.lua_type(L, 1) != 3)
			{
				Luau.luaL_errorL(L, "getInventoryForPlayer expects a number as the first argument", Array.Empty<string>());
				return 0;
			}
			if (Luau.lua_type(L, 2) != 7)
			{
				Luau.luaL_errorL(L, "getInventoryForPlayer expects a callback function", Array.Empty<string>());
				return 0;
			}
			if (Luau.lua_type(L, 3) != 7)
			{
				Luau.luaL_errorL(L, "getInventoryForPlayer expects an error callback function", Array.Empty<string>());
				return 0;
			}
			Debug.Log("Past get inventory for player param validation");
			bool flag = Bindings.OnlineFunctions.RateLimitBackendFunction(delegate
			{
				int num = (int)Luau.luaL_checknumber(L, 1);
				int callbackRID = Luau.lua_ref(L, 2);
				int errorCallbackRID = Luau.lua_ref(L, 3);
				string playerMothershipId = NetworkSystem.Instance.GetPlayerMothershipId(num);
				Debug.Log("Target Mothership ID " + playerMothershipId);
				if (!playerMothershipId.IsNullOrEmpty())
				{
					bool userInventory = MothershipClientApiUnity.GetUserInventory(playerMothershipId, delegate(MothershipGetMergedInventoryResponse Response)
					{
						Debug.Log("In get user inventory success callback");
						Bindings.OnlineFunctions.ClearBackendFunctionInFlight("GetInventoryForUser");
						LuauScriptRunner luauScriptRunner2 = Bindings.OnlineFunctions.FindAliveScriptRunner(L);
						if (luauScriptRunner2 == null)
						{
							return;
						}
						Luau.lua_getref(L, callbackRID);
						if (Luau.lua_type(L, -1) == 7)
						{
							Luau.lua_createtable(L, 0, 0);
							int num3 = 0;
							foreach (MothershipInventoryItemSummary mothershipInventoryItemSummary in Response.Results)
							{
								if (mothershipInventoryItemSummary.in_game_id.Contains(CustomMapLoader.LoadedMapModId.ToString()) || mothershipInventoryItemSummary.in_game_id.Equals("geodes", StringComparison.OrdinalIgnoreCase))
								{
									Bindings.MInventoryItem* ptr2 = Luau.lua_class_push<Bindings.MInventoryItem>(L);
									(ref ptr2->InGameId).CopyFromTruncated(mothershipInventoryItemSummary.in_game_id);
									ptr2->Quantity = mothershipInventoryItemSummary.quantity;
									(ref ptr2->Name).CopyFromTruncated(mothershipInventoryItemSummary.name);
									(ref ptr2->DisplayDescription).CopyFromTruncated(mothershipInventoryItemSummary.display_description);
									(ref ptr2->DisplayName).CopyFromTruncated(mothershipInventoryItemSummary.display_name);
									(ref ptr2->ID).CopyFromTruncated(mothershipInventoryItemSummary.entitlement_id);
									Luau.lua_rawseti(L, -2, ++num3);
								}
							}
							Debug.Log("In get user inventory success callback, calling back lua now");
							int num4 = Luau.lua_pcall(L, 1, 0, 0);
							if (LuauScriptRunner.ErrorCheck(L, num4))
							{
								luauScriptRunner2.ShouldTick = false;
								return;
							}
						}
						else
						{
							Luau.lua_pop(L, 1);
						}
						Luau.lua_unref(L, callbackRID);
						Luau.lua_unref(L, errorCallbackRID);
					}, delegate(MothershipError Error, int code)
					{
						Bindings.OnlineFunctions.ClearBackendFunctionInFlight("GetInventoryForUser");
						LuauScriptRunner luauScriptRunner3 = Bindings.OnlineFunctions.FindAliveScriptRunner(L);
						if (luauScriptRunner3 == null)
						{
							return;
						}
						Luau.lua_unref(L, callbackRID);
						Luau.lua_getref(L, errorCallbackRID);
						if (Luau.lua_type(L, -1) == 7)
						{
							Bindings.MOnlineError* ptr3 = Luau.lua_class_push<Bindings.MOnlineError>(L);
							(ref ptr3->Name).CopyFromTruncated(Error.Name);
							(ref ptr3->Message).CopyFromTruncated(Error.Message);
							(ref ptr3->ErrorCode).CopyFromTruncated(Error.MothershipErrorCode);
							ptr3->HttpCode = code;
							int num5 = Luau.lua_pcall(L, 1, 0, 0);
							if (LuauScriptRunner.ErrorCheck(L, num5))
							{
								luauScriptRunner3.ShouldTick = false;
								return;
							}
						}
						else
						{
							Luau.lua_pop(L, 1);
						}
						Luau.lua_unref(L, errorCallbackRID);
					});
					if (!userInventory)
					{
						Luau.lua_unref(L, callbackRID);
						Luau.lua_unref(L, errorCallbackRID);
					}
					return userInventory;
				}
				Bindings.OnlineFunctions.ClearBackendFunctionInFlight("GetInventoryForUser");
				LuauScriptRunner luauScriptRunner = Bindings.OnlineFunctions.FindAliveScriptRunner(L);
				Luau.lua_unref(L, callbackRID);
				if (luauScriptRunner == null)
				{
					Luau.lua_unref(L, errorCallbackRID);
					return false;
				}
				Luau.lua_getref(L, errorCallbackRID);
				if (Luau.lua_type(L, -1) == 7)
				{
					Bindings.MOnlineError* ptr = Luau.lua_class_push<Bindings.MOnlineError>(L);
					(ref ptr->Name).CopyFromTruncated("No Mothership Player found for that actor ID");
					(ref ptr->Message).CopyFromTruncated("Is that actor ID in this room?");
					(ref ptr->ErrorCode).CopyFromTruncated("99999");
					ptr->HttpCode = 0;
					int num2 = Luau.lua_pcall(L, 1, 0, 0);
					if (LuauScriptRunner.ErrorCheck(L, num2))
					{
						luauScriptRunner.ShouldTick = false;
						return false;
					}
				}
				Luau.lua_unref(L, errorCallbackRID);
				return false;
			}, "GetInventoryForUser");
			Luau.lua_pushboolean(L, flag ? 1 : 0);
			return 1;
		}

		// Token: 0x06004EFB RID: 20219 RVA: 0x001A4144 File Offset: 0x001A2344
		private unsafe static LuauScriptRunner FindAliveScriptRunner(lua_State* L)
		{
			foreach (LuauScriptRunner luauScriptRunner in LuauScriptRunner.ScriptRunners)
			{
				if (luauScriptRunner.L == L)
				{
					return luauScriptRunner.ShouldTick ? luauScriptRunner : null;
				}
			}
			return null;
		}

		// Token: 0x06004EFC RID: 20220 RVA: 0x001A41AC File Offset: 0x001A23AC
		public static void ResetOnlineFunctionsState()
		{
			Bindings.OnlineFunctions.VStumpMothershipOfferDisplayId = "";
			Bindings.OnlineFunctions.CachedStore = null;
			Bindings.OnlineFunctions.CachedInventory = null;
			Bindings.OnlineFunctions._consentInFlight = false;
		}

		// Token: 0x06004EFD RID: 20221 RVA: 0x001A41CC File Offset: 0x001A23CC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int LoadStorefront(lua_State* L)
		{
			if (Luau.lua_type(L, 1) != 7)
			{
				Luau.luaL_errorL(L, "getStore expects a callback function", Array.Empty<string>());
				return 0;
			}
			if (Luau.lua_type(L, 2) != 7)
			{
				Luau.luaL_errorL(L, "getStore expects an error callback function", Array.Empty<string>());
				return 0;
			}
			bool flag = Bindings.OnlineFunctions.RateLimitBackendFunction(delegate
			{
				int callbackRID = Luau.lua_ref(L, 1);
				int errorCallbackRID = Luau.lua_ref(L, 2);
				bool storefront = MothershipClientApiUnity.GetStorefront(new string[0], delegate(MothershipGetStorefrontResponse Response)
				{
					Bindings.OnlineFunctions.ClearBackendFunctionInFlight("GetStore");
					LuauScriptRunner luauScriptRunner = Bindings.OnlineFunctions.FindAliveScriptRunner(L);
					if (luauScriptRunner == null)
					{
						return;
					}
					Bindings.OnlineFunctions.CachedStore = new Dictionary<string, Bindings.OnlineFunctions.CachedOffer>();
					Luau.lua_getref(L, callbackRID);
					if (Luau.lua_type(L, -1) == 7)
					{
						JObject jobject = new JObject();
						foreach (KeyValuePair<string, MothershipBoundOfferDisplay> keyValuePair in Response.Results)
						{
							JArray jarray = new JArray();
							if (!keyValuePair.Key.Equals("vstump", StringComparison.OrdinalIgnoreCase))
							{
								Debug.Log("Skipping offer display " + keyValuePair.Key + " because it wasn't vstump");
							}
							else
							{
								if (Bindings.OnlineFunctions.VStumpMothershipOfferDisplayId.IsNullOrEmpty())
								{
									Bindings.OnlineFunctions.VStumpMothershipOfferDisplayId = keyValuePair.Value.offer_display_id;
								}
								foreach (MothershipNormalizedOffer mothershipNormalizedOffer in keyValuePair.Value.offers)
								{
									Bindings.OnlineFunctions.CachedOffer cachedOffer = new Bindings.OnlineFunctions.CachedOffer
									{
										OfferId = mothershipNormalizedOffer.OfferId,
										DisplayIndex = mothershipNormalizedOffer.DisplayIndex,
										PurchaseAllowed = mothershipNormalizedOffer.PurchaseAllowed,
										DisplayName = mothershipNormalizedOffer.DisplayName,
										DisplayDescription = mothershipNormalizedOffer.DisplayDescription
									};
									foreach (KeyValuePair<string, MothershipEntitlementDeltaSummary> keyValuePair2 in mothershipNormalizedOffer.PersonalCredits)
									{
										cachedOffer.Credits.Add(new Bindings.OnlineFunctions.CachedOfferDelta
										{
											Name = keyValuePair2.Value.name,
											Change = keyValuePair2.Value.change,
											DisplayDescription = keyValuePair2.Value.display_description,
											DisplayName = keyValuePair2.Value.display_name
										});
									}
									foreach (KeyValuePair<string, MothershipEntitlementDeltaSummary> keyValuePair3 in mothershipNormalizedOffer.PersonalDebits)
									{
										cachedOffer.Debits.Add(new Bindings.OnlineFunctions.CachedOfferDelta
										{
											Name = keyValuePair3.Value.name,
											Change = keyValuePair3.Value.change,
											DisplayDescription = keyValuePair3.Value.display_description,
											DisplayName = keyValuePair3.Value.display_name
										});
									}
									Bindings.OnlineFunctions.CachedStore[mothershipNormalizedOffer.OfferId] = cachedOffer;
									if (mothershipNormalizedOffer.OfferName.Contains(CustomMapLoader.LoadedMapModId.ToString()))
									{
										JObject jobject2 = new JObject
										{
											{ "name", mothershipNormalizedOffer.OfferName },
											{ "displayIndex", mothershipNormalizedOffer.DisplayIndex },
											{ "purchasable", mothershipNormalizedOffer.PurchaseAllowed },
											{
												"displayId",
												keyValuePair.Value.offer_display_id
											},
											{ "id", mothershipNormalizedOffer.OfferId },
											{ "displayName", mothershipNormalizedOffer.DisplayName },
											{ "displayDescription", mothershipNormalizedOffer.DisplayDescription }
										};
										JObject jobject3 = new JObject();
										foreach (KeyValuePair<string, MothershipEntitlementDeltaSummary> keyValuePair4 in mothershipNormalizedOffer.PersonalCredits)
										{
											jobject3.Add(keyValuePair4.Key, new JObject
											{
												{
													"inGameId",
													keyValuePair4.Value.in_game_id
												},
												{
													"change",
													keyValuePair4.Value.change
												},
												{
													"displayName",
													keyValuePair4.Value.display_name
												},
												{
													"displayDescription",
													keyValuePair4.Value.display_description
												}
											});
										}
										jobject2.Add("credits", jobject3);
										JObject jobject4 = new JObject();
										foreach (KeyValuePair<string, MothershipEntitlementDeltaSummary> keyValuePair5 in mothershipNormalizedOffer.PersonalDebits)
										{
											jobject4.Add(keyValuePair5.Key, new JObject
											{
												{
													"inGameId",
													keyValuePair5.Value.in_game_id
												},
												{
													"change",
													keyValuePair5.Value.change
												},
												{
													"displayName",
													keyValuePair5.Value.display_name
												},
												{
													"displayDescription",
													keyValuePair5.Value.display_description
												}
											});
										}
										jobject2.Add("debits", jobject4);
										jarray.Add(jobject2);
									}
								}
								jobject.Add(keyValuePair.Key, jarray);
							}
						}
						Bindings.JSON.PushTable(L, jobject);
						int num = Luau.lua_pcall(L, 1, 0, 0);
						if (LuauScriptRunner.ErrorCheck(L, num))
						{
							luauScriptRunner.ShouldTick = false;
							return;
						}
					}
					else
					{
						Luau.lua_pop(L, 1);
					}
					Luau.lua_unref(L, callbackRID);
					Luau.lua_unref(L, errorCallbackRID);
				}, delegate(MothershipError Error, int StatusCode)
				{
					Bindings.OnlineFunctions.ClearBackendFunctionInFlight("GetStore");
					LuauScriptRunner luauScriptRunner2 = Bindings.OnlineFunctions.FindAliveScriptRunner(L);
					if (luauScriptRunner2 == null)
					{
						return;
					}
					Luau.lua_unref(L, callbackRID);
					Luau.lua_getref(L, errorCallbackRID);
					if (Luau.lua_type(L, -1) == 7)
					{
						Bindings.MOnlineError* ptr = Luau.lua_class_push<Bindings.MOnlineError>(L);
						(ref ptr->Name).CopyFromTruncated(Error.Name);
						(ref ptr->Message).CopyFromTruncated(Error.Message);
						(ref ptr->ErrorCode).CopyFromTruncated(Error.MothershipErrorCode);
						ptr->HttpCode = StatusCode;
						int num2 = Luau.lua_pcall(L, 1, 0, 0);
						if (LuauScriptRunner.ErrorCheck(L, num2))
						{
							luauScriptRunner2.ShouldTick = false;
							return;
						}
					}
					else
					{
						Luau.lua_pop(L, 1);
					}
					Luau.lua_unref(L, errorCallbackRID);
				});
				if (!storefront)
				{
					Luau.lua_unref(L, callbackRID);
					Luau.lua_unref(L, errorCallbackRID);
				}
				return storefront;
			}, "GetStore");
			Luau.lua_pushboolean(L, flag ? 1 : 0);
			return 1;
		}

		// Token: 0x06004EFE RID: 20222 RVA: 0x001A425C File Offset: 0x001A245C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int TryPurchase(lua_State* L)
		{
			if (Luau.lua_type(L, 1) != 5)
			{
				Luau.luaL_errorL(L, "tryPurchase expects a string offer id first parameter", Array.Empty<string>());
				return 0;
			}
			if (Luau.lua_type(L, 2) != 7)
			{
				Luau.luaL_errorL(L, "tryPurchase expects a callback function second parameter", Array.Empty<string>());
				return 0;
			}
			if (Luau.lua_type(L, 3) != 7)
			{
				Luau.luaL_errorL(L, "tryPurchase expects an error callback function third parameter", Array.Empty<string>());
				return 0;
			}
			string offerId = Marshal.PtrToStringAnsi((IntPtr)((void*)Luau.luaL_checkstring(L, 1)));
			int callbackRID = Luau.lua_ref(L, 2);
			int errorCallbackRID = Luau.lua_ref(L, 3);
			if (Bindings.OnlineFunctions.VStumpMothershipOfferDisplayId.IsNullOrEmpty() || Bindings.OnlineFunctions.CachedStore == null)
			{
				Debug.LogError("Need to fetch store before purchase");
				Luau.lua_unref(L, callbackRID);
				Bindings.OnlineFunctions.DispatchOnlineError(L, errorCallbackRID, null, "StoreFetchRequired", "You must fetch the store before attempting a purchase", "0", 0);
				return 0;
			}
			if (!Bindings.OnlineFunctions.CachedStore.ContainsKey(offerId))
			{
				Debug.LogError("Tried to purchase an offer not in the store cache");
				Luau.lua_unref(L, callbackRID);
				Bindings.OnlineFunctions.DispatchOnlineError(L, errorCallbackRID, null, "StoreOfferMismatch", "Tried to purchase an offer not in the store cache, try re-fetching the store and look at the script.", "0", 0);
				return 0;
			}
			if (Bindings.OnlineFunctions._consentInFlight)
			{
				Debug.LogError("Must close other consent ui before invoking a new consent UI");
				Luau.lua_unref(L, callbackRID);
				Bindings.OnlineFunctions.DispatchOnlineError(L, errorCallbackRID, null, "OtherOperationInProgress", "Only 1 consent operation is allowed at a time. Make sure that you're not spamming", "0", 0);
				return 0;
			}
			Bindings.OnlineFunctions._consentInFlight = true;
			Bindings.OnlineFunctions.CachedOffer offerToPurchase = Bindings.OnlineFunctions.CachedStore[offerId];
			List<ConsentScreen.ConsentCost> list = new List<ConsentScreen.ConsentCost>(offerToPurchase.Debits.Count);
			foreach (Bindings.OnlineFunctions.CachedOfferDelta cachedOfferDelta in offerToPurchase.Debits)
			{
				int num = 0;
				bool flag = false;
				if (Bindings.OnlineFunctions.CachedInventory != null)
				{
					foreach (Bindings.OnlineFunctions.CachedInventoryItem cachedInventoryItem in Bindings.OnlineFunctions.CachedInventory.Values)
					{
						if (cachedInventoryItem.Name == cachedOfferDelta.Name)
						{
							num = cachedInventoryItem.Quantity;
							flag = true;
							break;
						}
					}
				}
				list.Add(new ConsentScreen.ConsentCost
				{
					DisplayName = cachedOfferDelta.DisplayName,
					Amount = Mathf.Abs(cachedOfferDelta.Change),
					CurrentBalance = num,
					HasBalance = flag
				});
			}
			ConsentScreen.StartConsentFlow(offerToPurchase.DisplayName, list, delegate(bool Consented, Action<string> AsyncWorkComplete)
			{
				if (Consented)
				{
					MothershipClientApiUnity.PurchaseOffer(Bindings.OnlineFunctions.VStumpMothershipOfferDisplayId, offerId, offerToPurchase.DisplayIndex, delegate(MothershipPurchaseOfferResponse Response)
					{
						Bindings.OnlineFunctions._consentInFlight = false;
						Bindings.OnlineFunctions.CachedInventory = null;
						AsyncWorkComplete("Purchase Completed!");
						LuauScriptRunner luauScriptRunner2 = Bindings.OnlineFunctions.FindAliveScriptRunner(L);
						if (luauScriptRunner2 == null)
						{
							return;
						}
						Luau.lua_unref(L, errorCallbackRID);
						Luau.lua_getref(L, callbackRID);
						if (Luau.lua_type(L, -1) == 7)
						{
							Luau.lua_createtable(L, 0, 0);
							int num2 = 0;
							foreach (KeyValuePair<string, MothershipEntitlementUpdate> keyValuePair in Response.Changes)
							{
								Bindings.MInventoryItem* ptr = Luau.lua_class_push<Bindings.MInventoryItem>(L);
								(ref ptr->InGameId).CopyFromTruncated(keyValuePair.Value.in_game_id);
								ptr->Quantity = keyValuePair.Value.quantity;
								(ref ptr->Name).CopyFromTruncated(keyValuePair.Value.name);
								(ref ptr->DisplayDescription).CopyFromTruncated(keyValuePair.Value.display_description);
								(ref ptr->DisplayName).CopyFromTruncated(keyValuePair.Value.display_name);
								(ref ptr->ID).CopyFromTruncated(keyValuePair.Value.entitlement_id);
								Luau.lua_rawseti(L, -2, ++num2);
							}
							int num3 = Luau.lua_pcall(L, 1, 0, 0);
							if (LuauScriptRunner.ErrorCheck(L, num3))
							{
								luauScriptRunner2.ShouldTick = false;
								return;
							}
						}
						else
						{
							Luau.lua_pop(L, 1);
						}
						Luau.lua_unref(L, callbackRID);
					}, delegate(MothershipError Error, int StatusCode)
					{
						Bindings.OnlineFunctions._consentInFlight = false;
						Debug.LogError(string.Concat(new string[] { Error.Name, ": ", Error.Message, " ", Error.MothershipErrorCode }));
						AsyncWorkComplete("Purchase Failed: " + Error.MothershipErrorCode + ": " + Error.Message);
						LuauScriptRunner luauScriptRunner3 = Bindings.OnlineFunctions.FindAliveScriptRunner(L);
						if (luauScriptRunner3 == null)
						{
							return;
						}
						Luau.lua_unref(L, callbackRID);
						Bindings.OnlineFunctions.DispatchOnlineError(L, errorCallbackRID, luauScriptRunner3, Error.Name, Error.Message, Error.MothershipErrorCode, StatusCode);
					});
					return;
				}
				Bindings.OnlineFunctions._consentInFlight = false;
				AsyncWorkComplete("");
				Debug.LogError("Consent Denied");
				LuauScriptRunner luauScriptRunner = Bindings.OnlineFunctions.FindAliveScriptRunner(L);
				if (luauScriptRunner == null)
				{
					return;
				}
				Luau.lua_unref(L, callbackRID);
				Bindings.OnlineFunctions.DispatchOnlineError(L, errorCallbackRID, luauScriptRunner, "ConsentDenied", "The player did not consent to this purchase. They can re-open the UI and try again.", "0", 0);
			});
			return 0;
		}

		// Token: 0x06004EFF RID: 20223 RVA: 0x001A455C File Offset: 0x001A275C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int TryConsume(lua_State* L)
		{
			if (Luau.lua_type(L, 1) != 5)
			{
				Luau.luaL_errorL(L, "tryConsume expects a string entitlement id first parameter", Array.Empty<string>());
				return 0;
			}
			if (Luau.lua_type(L, 2) != 7)
			{
				Luau.luaL_errorL(L, "tryConsume expects a callback function second parameter", Array.Empty<string>());
				return 0;
			}
			if (Luau.lua_type(L, 3) != 7)
			{
				Luau.luaL_errorL(L, "tryConsume expects an error callback function third parameter", Array.Empty<string>());
				return 0;
			}
			string entitlementId = Marshal.PtrToStringAnsi((IntPtr)((void*)Luau.luaL_checkstring(L, 1)));
			int callbackRID = Luau.lua_ref(L, 2);
			int errorCallbackRID = Luau.lua_ref(L, 3);
			if (Bindings.OnlineFunctions.CachedInventory == null || !Bindings.OnlineFunctions.CachedInventory.ContainsKey(entitlementId) || Bindings.OnlineFunctions.CachedInventory[entitlementId].Quantity < 1)
			{
				Debug.LogError("Need to fetch inventory before consumption, and this player must have this entitlement in their inventory");
				Luau.lua_unref(L, callbackRID);
				Bindings.OnlineFunctions.DispatchOnlineError(L, errorCallbackRID, null, "InventoryFetchRequired", "Need to fetch inventory before consumption, and this player must have this entitlement in their inventory", "0", 0);
				return 0;
			}
			MothershipClientApiUnity.ConsumeConsumable(entitlementId, delegate(MothershipConsumeConsumableResponse Response)
			{
				LuauScriptRunner luauScriptRunner = Bindings.OnlineFunctions.FindAliveScriptRunner(L);
				if (luauScriptRunner == null)
				{
					return;
				}
				Luau.lua_unref(L, errorCallbackRID);
				Luau.lua_getref(L, callbackRID);
				if (Luau.lua_type(L, -1) == 7)
				{
					Bindings.MInventoryItem* ptr = Luau.lua_class_push<Bindings.MInventoryItem>(L);
					(ref ptr->InGameId).CopyFromTruncated(Response.Entitlement.inGameId);
					ptr->Quantity = Response.NewQuantity;
					(ref ptr->Name).CopyFromTruncated(Response.Entitlement.name);
					(ref ptr->DisplayDescription).CopyFromTruncated(Response.Entitlement.display_description);
					(ref ptr->DisplayName).CopyFromTruncated(Response.Entitlement.display_name);
					(ref ptr->ID).CopyFromTruncated(Response.Entitlement.entitlementId);
					if (Bindings.OnlineFunctions.CachedInventory != null && Bindings.OnlineFunctions.CachedInventory.ContainsKey(entitlementId))
					{
						Bindings.OnlineFunctions.CachedInventory[entitlementId].Quantity--;
					}
					int num = Luau.lua_pcall(L, 1, 0, 0);
					if (LuauScriptRunner.ErrorCheck(L, num))
					{
						luauScriptRunner.ShouldTick = false;
						return;
					}
				}
				else
				{
					Luau.lua_pop(L, 1);
				}
				Luau.lua_unref(L, callbackRID);
			}, delegate(MothershipError Error, int StatusCode)
			{
				Debug.LogError(string.Concat(new string[] { Error.Name, ": ", Error.Message, " ", Error.MothershipErrorCode }));
				LuauScriptRunner luauScriptRunner2 = Bindings.OnlineFunctions.FindAliveScriptRunner(L);
				if (luauScriptRunner2 == null)
				{
					return;
				}
				Luau.lua_unref(L, callbackRID);
				Bindings.OnlineFunctions.DispatchOnlineError(L, errorCallbackRID, luauScriptRunner2, Error.Name, Error.Message, Error.MothershipErrorCode, StatusCode);
			});
			return 0;
		}

		// Token: 0x06004F00 RID: 20224 RVA: 0x001A46C0 File Offset: 0x001A28C0
		private static bool RateLimitBackendFunction(Func<bool> Action, string Key)
		{
			if (!CustomMapLoader.LoadedMapModId.IsValid())
			{
				Debug.LogError("Tried to call an online function with an invalid map id");
				return false;
			}
			if (CustomMapLoader.LoadedMapModId._id == 9999999999L)
			{
				Debug.LogError("Tried to call an online function with a map id that indicates this map is sideloaded or otherwise local");
				return false;
			}
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			Bindings.OnlineFunctions.BackendCallState backendCallState;
			if (Bindings.OnlineFunctions.BackendCallStates.TryGetValue(Key, out backendCallState))
			{
				if (backendCallState.InFlight && realtimeSinceStartup - backendCallState.LastCallTime < 30f)
				{
					return false;
				}
				if (realtimeSinceStartup - backendCallState.LastCallTime < 5f)
				{
					return false;
				}
			}
			bool flag = Action();
			if (flag)
			{
				Bindings.OnlineFunctions.BackendCallStates[Key] = new Bindings.OnlineFunctions.BackendCallState
				{
					InFlight = flag,
					LastCallTime = realtimeSinceStartup
				};
			}
			return flag;
		}

		// Token: 0x06004F01 RID: 20225 RVA: 0x001A4778 File Offset: 0x001A2978
		private static void ClearBackendFunctionInFlight(string Key)
		{
			Bindings.OnlineFunctions.BackendCallState backendCallState;
			if (Bindings.OnlineFunctions.BackendCallStates.TryGetValue(Key, out backendCallState))
			{
				backendCallState.InFlight = false;
				Bindings.OnlineFunctions.BackendCallStates[Key] = backendCallState;
			}
		}

		// Token: 0x04006167 RID: 24935
		private static Dictionary<string, Bindings.OnlineFunctions.CachedInventoryItem> CachedInventory = null;

		// Token: 0x04006168 RID: 24936
		private static string VStumpMothershipOfferDisplayId = "";

		// Token: 0x04006169 RID: 24937
		private static Dictionary<string, Bindings.OnlineFunctions.CachedOffer> CachedStore = null;

		// Token: 0x0400616A RID: 24938
		private static bool _consentInFlight = false;

		// Token: 0x0400616B RID: 24939
		private const float BackendFunctionCooldownSeconds = 5f;

		// Token: 0x0400616C RID: 24940
		private const float BackendFunctionInFlightTimeoutSeconds = 30f;

		// Token: 0x0400616D RID: 24941
		private static readonly Dictionary<string, Bindings.OnlineFunctions.BackendCallState> BackendCallStates = new Dictionary<string, Bindings.OnlineFunctions.BackendCallState>();

		// Token: 0x02000C73 RID: 3187
		private class CachedInventoryItem
		{
			// Token: 0x0400616E RID: 24942
			public string Name;

			// Token: 0x0400616F RID: 24943
			public int Quantity;

			// Token: 0x04006170 RID: 24944
			public string InGameId;

			// Token: 0x04006171 RID: 24945
			public string DisplayName;

			// Token: 0x04006172 RID: 24946
			public string DisplayDescription;

			// Token: 0x04006173 RID: 24947
			public string ID;
		}

		// Token: 0x02000C74 RID: 3188
		private struct CachedOfferDelta
		{
			// Token: 0x04006174 RID: 24948
			public string Name;

			// Token: 0x04006175 RID: 24949
			public int Change;

			// Token: 0x04006176 RID: 24950
			public string DisplayName;

			// Token: 0x04006177 RID: 24951
			public string DisplayDescription;
		}

		// Token: 0x02000C75 RID: 3189
		private class CachedOffer
		{
			// Token: 0x04006178 RID: 24952
			public string OfferId;

			// Token: 0x04006179 RID: 24953
			public int DisplayIndex;

			// Token: 0x0400617A RID: 24954
			public bool PurchaseAllowed;

			// Token: 0x0400617B RID: 24955
			public string DisplayName;

			// Token: 0x0400617C RID: 24956
			public string DisplayDescription;

			// Token: 0x0400617D RID: 24957
			public List<Bindings.OnlineFunctions.CachedOfferDelta> Credits = new List<Bindings.OnlineFunctions.CachedOfferDelta>();

			// Token: 0x0400617E RID: 24958
			public List<Bindings.OnlineFunctions.CachedOfferDelta> Debits = new List<Bindings.OnlineFunctions.CachedOfferDelta>();
		}

		// Token: 0x02000C76 RID: 3190
		private struct BackendCallState
		{
			// Token: 0x0400617F RID: 24959
			public bool InFlight;

			// Token: 0x04006180 RID: 24960
			public float LastCallTime;
		}
	}
}
