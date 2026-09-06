using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

// Token: 0x02000C81 RID: 3201
[BurstCompile]
public static class BurstClassInfo
{
	// Token: 0x06004F1D RID: 20253 RVA: 0x001A5EC4 File Offset: 0x001A40C4
	public unsafe static void NewClass<[IsUnmanaged] T>(string className, Dictionary<int, FieldInfo> fieldList, Dictionary<int, lua_CFunction> functionList, Dictionary<int, FunctionPointer<lua_CFunction>> functionPtrList) where T : struct, ValueType
	{
		if (!BurstClassInfo.ClassList.InfoFields.Data.IsCreated)
		{
			*BurstClassInfo.ClassList.InfoFields.Data = new NativeHashMap<int, BurstClassInfo.ClassInfo>(20, Allocator.Persistent);
		}
		BurstClassInfo.ClassList.MetatableNames<T>.Name = className;
		ReflectionMetaNames.ReflectedNames.TryAdd(typeof(T), className);
		BurstClassInfo.ClassInfo classInfo = default(BurstClassInfo.ClassInfo);
		classInfo.NameHash = LuaHashing.ByteHash(className);
		if (className.Length > 30)
		{
			throw new Exception("Name to long");
		}
		classInfo.Name = className;
		classInfo.Size = sizeof(T);
		classInfo.FieldList = new NativeHashMap<int, BurstClassInfo.BurstFieldInfo>(fieldList.Count, Allocator.Persistent);
		foreach (KeyValuePair<int, FieldInfo> keyValuePair in fieldList)
		{
			BurstClassInfo.BurstFieldInfo burstFieldInfo = default(BurstClassInfo.BurstFieldInfo);
			burstFieldInfo.NameHash = keyValuePair.Key;
			burstFieldInfo.Name = keyValuePair.Value.Name;
			burstFieldInfo.Offset = (int)Marshal.OffsetOf<T>(keyValuePair.Value.Name);
			Type fieldType = keyValuePair.Value.FieldType;
			if (fieldType == typeof(float))
			{
				burstFieldInfo.FieldType = BurstClassInfo.EFieldTypes.Float;
			}
			else if (fieldType == typeof(int))
			{
				burstFieldInfo.FieldType = BurstClassInfo.EFieldTypes.Int;
			}
			else if (fieldType == typeof(double))
			{
				burstFieldInfo.FieldType = BurstClassInfo.EFieldTypes.Double;
			}
			else if (fieldType == typeof(bool))
			{
				burstFieldInfo.FieldType = BurstClassInfo.EFieldTypes.Bool;
			}
			else if (fieldType == typeof(FixedString32Bytes) || fieldType == typeof(FixedString64Bytes) || fieldType == typeof(FixedString128Bytes) || fieldType == typeof(FixedString512Bytes) || fieldType == typeof(FixedString4096Bytes))
			{
				burstFieldInfo.FieldType = BurstClassInfo.EFieldTypes.String;
			}
			else if (!fieldType.IsPrimitive)
			{
				burstFieldInfo.FieldType = BurstClassInfo.EFieldTypes.LightUserData;
				ReflectionMetaNames.ReflectedNames.TryGetValue(fieldType, out burstFieldInfo.MetatableName);
			}
			burstFieldInfo.Size = Marshal.SizeOf(fieldType);
			classInfo.FieldList.TryAdd(keyValuePair.Key, burstFieldInfo);
		}
		classInfo.FunctionList = new NativeHashMap<int, IntPtr>(functionList.Count + functionPtrList.Count, Allocator.Persistent);
		foreach (KeyValuePair<int, lua_CFunction> keyValuePair2 in functionList)
		{
			classInfo.FunctionList.TryAdd(keyValuePair2.Key, Marshal.GetFunctionPointerForDelegate<lua_CFunction>(keyValuePair2.Value));
		}
		foreach (KeyValuePair<int, FunctionPointer<lua_CFunction>> keyValuePair3 in functionPtrList)
		{
			classInfo.FunctionList.TryAdd(keyValuePair3.Key, keyValuePair3.Value.Value);
		}
		BurstClassInfo.ClassList.InfoFields.Data.Add(classInfo.NameHash, classInfo);
		Debug.Log(string.Format("[NewClass] Registered {0}, MetaHash={1}, Count={2}, Contains={3}", new object[]
		{
			className,
			classInfo.NameHash,
			BurstClassInfo.ClassList.InfoFields.Data.Count,
			BurstClassInfo.ClassList.InfoFields.Data.ContainsKey(classInfo.NameHash)
		}));
	}

	// Token: 0x06004F1E RID: 20254 RVA: 0x001A62B4 File Offset: 0x001A44B4
	[BurstCompile]
	[MonoPInvokeCallback(typeof(BurstClassInfo.Index_00004E4E$PostfixBurstDelegate))]
	public unsafe static int Index(lua_State* L)
	{
		return BurstClassInfo.Index_00004E4E$BurstDirectCall.Invoke(L);
	}

	// Token: 0x06004F1F RID: 20255 RVA: 0x001A62BC File Offset: 0x001A44BC
	[BurstCompile]
	[MonoPInvokeCallback(typeof(BurstClassInfo.NewIndex_00004E4F$PostfixBurstDelegate))]
	public unsafe static int NewIndex(lua_State* L)
	{
		return BurstClassInfo.NewIndex_00004E4F$BurstDirectCall.Invoke(L);
	}

	// Token: 0x06004F20 RID: 20256 RVA: 0x001A62C4 File Offset: 0x001A44C4
	[BurstCompile]
	[MonoPInvokeCallback(typeof(BurstClassInfo.NameCall_00004E50$PostfixBurstDelegate))]
	public unsafe static int NameCall(lua_State* L)
	{
		return BurstClassInfo.NameCall_00004E50$BurstDirectCall.Invoke(L);
	}

	// Token: 0x06004F22 RID: 20258 RVA: 0x001A62E0 File Offset: 0x001A44E0
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static int Index$BurstManaged(lua_State* L)
	{
		FixedString32Bytes k_metatableLookup = BurstClassInfo._k_metatableLookup;
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref k_metatableLookup) + 2;
		Luau.luaL_getmetafield(L, 1, ptr);
		BurstClassInfo.ClassInfo classInfo;
		if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
		{
			Debug.LogError(string.Format("[LuauBindings::Index] Internal Class Info Error: MetaHash={0}", (int)Luau.luaL_checknumber(L, -1)));
			FixedString32Bytes fixedString32Bytes = "\"Internal Class Info Error\"";
			Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
			return 0;
		}
		Luau.lua_pop(L, 1);
		byte* ptr2 = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref classInfo.Name) + 2;
		IntPtr intPtr = IntPtr.Zero;
		Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, 1);
		if (lua_Types == Luau.lua_Types.LUA_TUSERDATA)
		{
			intPtr = (IntPtr)Luau.luaL_checkudata(L, 1, ptr2);
		}
		else
		{
			if (lua_Types != Luau.lua_Types.LUA_TTABLE)
			{
				FixedString32Bytes fixedString32Bytes2 = "\"Unknown type for __index\"";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
				return 0;
			}
			intPtr = Luau.lua_light_ptr(L, 1);
		}
		int num = Luau.lua_objlen(L, 2);
		int num2 = LuaHashing.ByteHash(Luau.luaL_checkstring(L, 2), num);
		BurstClassInfo.BurstFieldInfo burstFieldInfo;
		if (classInfo.FieldList.TryGetValue(num2, out burstFieldInfo))
		{
			IntPtr intPtr2 = intPtr + burstFieldInfo.Offset;
			switch (burstFieldInfo.FieldType)
			{
			case BurstClassInfo.EFieldTypes.Float:
				Luau.lua_pushnumber(L, (double)(*(float*)(void*)intPtr2));
				return 1;
			case BurstClassInfo.EFieldTypes.Int:
				Luau.lua_pushnumber(L, (double)(*(int*)(void*)intPtr2));
				return 1;
			case BurstClassInfo.EFieldTypes.Double:
				Luau.lua_pushnumber(L, *(double*)(void*)intPtr2);
				return 1;
			case BurstClassInfo.EFieldTypes.Bool:
				Luau.lua_pushboolean(L, (*(byte*)(void*)intPtr2 != 0) ? 1 : 0);
				return 1;
			case BurstClassInfo.EFieldTypes.String:
				Luau.lua_pushstring(L, (byte*)(void*)intPtr2 + 2);
				return 1;
			case BurstClassInfo.EFieldTypes.LightUserData:
				Luau.lua_class_push(L, burstFieldInfo.MetatableName, intPtr2);
				return 1;
			}
		}
		IntPtr intPtr3;
		if (classInfo.FunctionList.TryGetValue(num2, out intPtr3))
		{
			FunctionPointer<lua_CFunction> functionPointer = new FunctionPointer<lua_CFunction>(intPtr3);
			FixedString32Bytes fixedString32Bytes3 = "";
			Luau.lua_pushcclosurek(L, functionPointer, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2, 0, null);
			return 1;
		}
		FixedString32Bytes fixedString32Bytes4 = "\"Unknown Type?\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes4) + 2));
		return 0;
	}

	// Token: 0x06004F23 RID: 20259 RVA: 0x001A64F0 File Offset: 0x001A46F0
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static int NewIndex$BurstManaged(lua_State* L)
	{
		FixedString32Bytes k_metatableLookup = BurstClassInfo._k_metatableLookup;
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref k_metatableLookup) + 2;
		Luau.luaL_getmetafield(L, 1, ptr);
		int num = (int)Luau.luaL_checknumber(L, -1);
		BurstClassInfo.ClassInfo classInfo;
		if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue(num, out classInfo))
		{
			Debug.LogError(string.Format("[LuauBindings::NewIndex] Internal Class Info Error: MetaHash={0}", num));
			FixedString32Bytes fixedString32Bytes = "\"Internal Class Info Error\"";
			Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
			return 0;
		}
		Luau.lua_pop(L, 1);
		byte* ptr2 = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref classInfo.Name) + 2;
		IntPtr intPtr = IntPtr.Zero;
		Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, 1);
		if (lua_Types == Luau.lua_Types.LUA_TUSERDATA)
		{
			intPtr = (IntPtr)Luau.luaL_checkudata(L, 1, ptr2);
		}
		else
		{
			if (lua_Types != Luau.lua_Types.LUA_TTABLE)
			{
				FixedString32Bytes fixedString32Bytes2 = "\"Unknown type for __newindex\"";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
				return 0;
			}
			intPtr = Luau.lua_light_ptr(L, 1);
		}
		int num2 = Luau.lua_objlen(L, 2);
		int num3 = LuaHashing.ByteHash(Luau.luaL_checkstring(L, 2), num2);
		BurstClassInfo.BurstFieldInfo burstFieldInfo;
		if (classInfo.FieldList.TryGetValue(num3, out burstFieldInfo))
		{
			IntPtr intPtr2 = intPtr + burstFieldInfo.Offset;
			switch (burstFieldInfo.FieldType)
			{
			case BurstClassInfo.EFieldTypes.Float:
				*(float*)(void*)intPtr2 = (float)Luau.luaL_checknumber(L, 3);
				return 0;
			case BurstClassInfo.EFieldTypes.Int:
				*(int*)(void*)intPtr2 = (int)Luau.luaL_checknumber(L, 3);
				return 0;
			case BurstClassInfo.EFieldTypes.Double:
				*(double*)(void*)intPtr2 = Luau.luaL_checknumber(L, 3);
				return 0;
			case BurstClassInfo.EFieldTypes.Bool:
				*(byte*)(void*)intPtr2 = ((Luau.lua_toboolean(L, 3) != 0) ? 1 : 0);
				return 0;
			case BurstClassInfo.EFieldTypes.LightUserData:
				Buffer.MemoryCopy((void*)((IntPtr)((void*)Luau.lua_class_get(L, 3, burstFieldInfo.MetatableName))), (void*)intPtr2, (long)burstFieldInfo.Size, (long)burstFieldInfo.Size);
				return 0;
			}
		}
		FixedString32Bytes fixedString32Bytes3 = "\"Unknown Type\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2));
		return 0;
	}

	// Token: 0x06004F24 RID: 20260 RVA: 0x001A66D8 File Offset: 0x001A48D8
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static int NameCall$BurstManaged(lua_State* L)
	{
		FixedString32Bytes k_metatableLookup = BurstClassInfo._k_metatableLookup;
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref k_metatableLookup) + 2;
		Luau.luaL_getmetafield(L, 1, ptr);
		int num = (int)Luau.luaL_checknumber(L, -1);
		BurstClassInfo.ClassInfo classInfo;
		if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue(num, out classInfo))
		{
			Debug.LogError(string.Format("[LuauBindings::NameCall] Internal Class Info Error: MetaHash={0}, Count={1}, IsCreated={2}, Contains={3}", new object[]
			{
				num,
				BurstClassInfo.ClassList.InfoFields.Data.Count,
				BurstClassInfo.ClassList.InfoFields.Data.IsCreated,
				BurstClassInfo.ClassList.InfoFields.Data.ContainsKey(num)
			}));
			FixedString32Bytes fixedString32Bytes = "\"Internal Class Info Error\"";
			Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
			return 0;
		}
		Luau.lua_pop(L, 1);
		int num2 = LuaHashing.ByteHash(Luau.lua_namecallatom(L, null));
		IntPtr intPtr;
		if (classInfo.FunctionList.TryGetValue(num2, out intPtr))
		{
			FunctionPointer<lua_CFunction> functionPointer = new FunctionPointer<lua_CFunction>(intPtr);
			return functionPointer.Invoke(L);
		}
		FixedString32Bytes fixedString32Bytes2 = "\"Function not found in function list\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
		return 0;
	}

	// Token: 0x04006199 RID: 24985
	private static readonly FixedString32Bytes _k_metatableLookup = "metahash";

	// Token: 0x02000C82 RID: 3202
	public enum EFieldTypes
	{
		// Token: 0x0400619B RID: 24987
		Float,
		// Token: 0x0400619C RID: 24988
		Int,
		// Token: 0x0400619D RID: 24989
		Double,
		// Token: 0x0400619E RID: 24990
		Bool,
		// Token: 0x0400619F RID: 24991
		String,
		// Token: 0x040061A0 RID: 24992
		LightUserData
	}

	// Token: 0x02000C83 RID: 3203
	[BurstCompile]
	public struct BurstFieldInfo
	{
		// Token: 0x040061A1 RID: 24993
		public int NameHash;

		// Token: 0x040061A2 RID: 24994
		public FixedString32Bytes Name;

		// Token: 0x040061A3 RID: 24995
		public FixedString32Bytes MetatableName;

		// Token: 0x040061A4 RID: 24996
		public int Offset;

		// Token: 0x040061A5 RID: 24997
		public BurstClassInfo.EFieldTypes FieldType;

		// Token: 0x040061A6 RID: 24998
		public int Size;
	}

	// Token: 0x02000C84 RID: 3204
	[BurstCompile]
	public struct ClassInfo
	{
		// Token: 0x040061A7 RID: 24999
		public int NameHash;

		// Token: 0x040061A8 RID: 25000
		public int Size;

		// Token: 0x040061A9 RID: 25001
		public FixedString32Bytes Name;

		// Token: 0x040061AA RID: 25002
		public NativeHashMap<int, BurstClassInfo.BurstFieldInfo> FieldList;

		// Token: 0x040061AB RID: 25003
		public NativeHashMap<int, IntPtr> FunctionList;
	}

	// Token: 0x02000C85 RID: 3205
	public abstract class ClassList
	{
		// Token: 0x040061AC RID: 25004
		public static readonly SharedStatic<NativeHashMap<int, BurstClassInfo.ClassInfo>> InfoFields = SharedStatic<NativeHashMap<int, BurstClassInfo.ClassInfo>>.GetOrCreateUnsafe(0U, -7258312696341931442L, -7445903157129162016L);

		// Token: 0x02000C86 RID: 3206
		private class FieldKey
		{
		}

		// Token: 0x02000C87 RID: 3207
		public static class MetatableNames<T>
		{
			// Token: 0x040061AD RID: 25005
			public static FixedString32Bytes Name;
		}
	}

	// Token: 0x02000C88 RID: 3208
	// (Invoke) Token: 0x06004F29 RID: 20265
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal unsafe delegate int Index_00004E4E$PostfixBurstDelegate(lua_State* L);

	// Token: 0x02000C89 RID: 3209
	internal static class Index_00004E4E$BurstDirectCall
	{
		// Token: 0x06004F2C RID: 20268 RVA: 0x001A681C File Offset: 0x001A4A1C
		[BurstDiscard]
		private static void GetFunctionPointerDiscard(ref IntPtr A_0)
		{
			if (BurstClassInfo.Index_00004E4E$BurstDirectCall.Pointer == 0)
			{
				BurstClassInfo.Index_00004E4E$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstClassInfo.Index_00004E4E$PostfixBurstDelegate>(new BurstClassInfo.Index_00004E4E$PostfixBurstDelegate(BurstClassInfo.Index)).Value;
			}
			A_0 = BurstClassInfo.Index_00004E4E$BurstDirectCall.Pointer;
		}

		// Token: 0x06004F2D RID: 20269 RVA: 0x001A685C File Offset: 0x001A4A5C
		private static IntPtr GetFunctionPointer()
		{
			IntPtr intPtr = (IntPtr)0;
			BurstClassInfo.Index_00004E4E$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
			return intPtr;
		}

		// Token: 0x06004F2E RID: 20270 RVA: 0x001A6874 File Offset: 0x001A4A74
		public unsafe static int Invoke(lua_State* L)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = BurstClassInfo.Index_00004E4E$BurstDirectCall.GetFunctionPointer();
				if (functionPointer != 0)
				{
					return calli(System.Int32(lua_State*), L, functionPointer);
				}
			}
			return BurstClassInfo.Index$BurstManaged(L);
		}

		// Token: 0x040061AE RID: 25006
		private static IntPtr Pointer;
	}

	// Token: 0x02000C8A RID: 3210
	// (Invoke) Token: 0x06004F30 RID: 20272
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal unsafe delegate int NewIndex_00004E4F$PostfixBurstDelegate(lua_State* L);

	// Token: 0x02000C8B RID: 3211
	internal static class NewIndex_00004E4F$BurstDirectCall
	{
		// Token: 0x06004F33 RID: 20275 RVA: 0x001A68A8 File Offset: 0x001A4AA8
		[BurstDiscard]
		private static void GetFunctionPointerDiscard(ref IntPtr A_0)
		{
			if (BurstClassInfo.NewIndex_00004E4F$BurstDirectCall.Pointer == 0)
			{
				BurstClassInfo.NewIndex_00004E4F$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstClassInfo.NewIndex_00004E4F$PostfixBurstDelegate>(new BurstClassInfo.NewIndex_00004E4F$PostfixBurstDelegate(BurstClassInfo.NewIndex)).Value;
			}
			A_0 = BurstClassInfo.NewIndex_00004E4F$BurstDirectCall.Pointer;
		}

		// Token: 0x06004F34 RID: 20276 RVA: 0x001A68E8 File Offset: 0x001A4AE8
		private static IntPtr GetFunctionPointer()
		{
			IntPtr intPtr = (IntPtr)0;
			BurstClassInfo.NewIndex_00004E4F$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
			return intPtr;
		}

		// Token: 0x06004F35 RID: 20277 RVA: 0x001A6900 File Offset: 0x001A4B00
		public unsafe static int Invoke(lua_State* L)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = BurstClassInfo.NewIndex_00004E4F$BurstDirectCall.GetFunctionPointer();
				if (functionPointer != 0)
				{
					return calli(System.Int32(lua_State*), L, functionPointer);
				}
			}
			return BurstClassInfo.NewIndex$BurstManaged(L);
		}

		// Token: 0x040061AF RID: 25007
		private static IntPtr Pointer;
	}

	// Token: 0x02000C8C RID: 3212
	// (Invoke) Token: 0x06004F37 RID: 20279
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal unsafe delegate int NameCall_00004E50$PostfixBurstDelegate(lua_State* L);

	// Token: 0x02000C8D RID: 3213
	internal static class NameCall_00004E50$BurstDirectCall
	{
		// Token: 0x06004F3A RID: 20282 RVA: 0x001A6934 File Offset: 0x001A4B34
		[BurstDiscard]
		private static void GetFunctionPointerDiscard(ref IntPtr A_0)
		{
			if (BurstClassInfo.NameCall_00004E50$BurstDirectCall.Pointer == 0)
			{
				BurstClassInfo.NameCall_00004E50$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstClassInfo.NameCall_00004E50$PostfixBurstDelegate>(new BurstClassInfo.NameCall_00004E50$PostfixBurstDelegate(BurstClassInfo.NameCall)).Value;
			}
			A_0 = BurstClassInfo.NameCall_00004E50$BurstDirectCall.Pointer;
		}

		// Token: 0x06004F3B RID: 20283 RVA: 0x001A6974 File Offset: 0x001A4B74
		private static IntPtr GetFunctionPointer()
		{
			IntPtr intPtr = (IntPtr)0;
			BurstClassInfo.NameCall_00004E50$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
			return intPtr;
		}

		// Token: 0x06004F3C RID: 20284 RVA: 0x001A698C File Offset: 0x001A4B8C
		public unsafe static int Invoke(lua_State* L)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = BurstClassInfo.NameCall_00004E50$BurstDirectCall.GetFunctionPointer();
				if (functionPointer != 0)
				{
					return calli(System.Int32(lua_State*), L, functionPointer);
				}
			}
			return BurstClassInfo.NameCall$BurstManaged(L);
		}

		// Token: 0x040061B0 RID: 25008
		private static IntPtr Pointer;
	}
}
