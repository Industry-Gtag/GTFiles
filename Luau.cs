using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x02000C94 RID: 3220
public class Luau
{
	// Token: 0x06004F52 RID: 20306
	[DllImport("luau")]
	public unsafe static extern lua_State* luaL_newstate();

	// Token: 0x06004F53 RID: 20307
	[DllImport("luau")]
	public unsafe static extern void luaL_openlibs(lua_State* L);

	// Token: 0x06004F54 RID: 20308
	[DllImport("luau")]
	public unsafe static extern sbyte* luau_compile([MarshalAs(UnmanagedType.LPStr)] string source, [NativeInteger] UIntPtr size, lua_CompileOptions* options, [NativeInteger] UIntPtr* outsize);

	// Token: 0x06004F55 RID: 20309
	[DllImport("luau")]
	public unsafe static extern int luau_load(lua_State* L, [MarshalAs(UnmanagedType.LPStr)] string chunkname, sbyte* data, [NativeInteger] UIntPtr size, int env);

	// Token: 0x06004F56 RID: 20310
	[DllImport("luau")]
	public unsafe static extern void lua_pushvalue(lua_State* L, int idx);

	// Token: 0x06004F57 RID: 20311
	[DllImport("luau")]
	public unsafe static extern void lua_pushcclosurek(lua_State* L, lua_CFunction fn, [MarshalAs(UnmanagedType.LPStr)] string debugname, int nup, lua_Continuation cont);

	// Token: 0x06004F58 RID: 20312
	[DllImport("luau")]
	public unsafe static extern void lua_pushcclosurek(lua_State* L, FunctionPointer<lua_CFunction> fn, [MarshalAs(UnmanagedType.LPStr)] string debugname, int nup, lua_Continuation cont);

	// Token: 0x06004F59 RID: 20313
	[DllImport("luau")]
	public unsafe static extern void lua_pushcclosurek(lua_State* L, FunctionPointer<lua_CFunction> fn, byte* debugname, int nup, int* cont);

	// Token: 0x06004F5A RID: 20314 RVA: 0x001A6F58 File Offset: 0x001A5158
	public unsafe static void lua_pushcfunction(lua_State* L, FunctionPointer<lua_CFunction> fn, [MarshalAs(UnmanagedType.LPStr)] string debugname)
	{
		Luau.lua_pushcclosurek(L, fn, debugname, 0, null);
	}

	// Token: 0x06004F5B RID: 20315 RVA: 0x001A6F64 File Offset: 0x001A5164
	public unsafe static void lua_pushcfunction(lua_State* L, lua_CFunction fn, [MarshalAs(UnmanagedType.LPStr)] string debugname)
	{
		Luau.lua_pushcclosurek(L, fn, debugname, 0, null);
	}

	// Token: 0x06004F5C RID: 20316
	[DllImport("luau")]
	public unsafe static extern void lua_settop(lua_State* L, int idx);

	// Token: 0x06004F5D RID: 20317
	[DllImport("luau")]
	public unsafe static extern int lua_gettop(lua_State* L);

	// Token: 0x06004F5E RID: 20318
	[DllImport("luau")]
	public unsafe static extern sbyte* lua_tolstring(lua_State* L, int idx, int* len);

	// Token: 0x06004F5F RID: 20319
	[DllImport("luau")]
	public unsafe static extern int lua_resume(lua_State* L, lua_State* from, int nargs);

	// Token: 0x06004F60 RID: 20320
	[DllImport("luau")]
	public unsafe static extern void lua_setfield(lua_State* L, int index, [MarshalAs(UnmanagedType.LPStr)] string k);

	// Token: 0x06004F61 RID: 20321
	[DllImport("luau")]
	public unsafe static extern void lua_setfield(lua_State* L, int index, byte* k);

	// Token: 0x06004F62 RID: 20322 RVA: 0x001A6F70 File Offset: 0x001A5170
	public unsafe static void lua_setglobal(lua_State* L, string s)
	{
		Luau.lua_setfield(L, -10002, s);
	}

	// Token: 0x06004F63 RID: 20323 RVA: 0x001A6F80 File Offset: 0x001A5180
	public unsafe static void lua_register(lua_State* L, lua_CFunction f, string n)
	{
		lua_Continuation lua_Continuation = null;
		Luau.lua_pushcclosurek(L, f, n, 0, lua_Continuation);
		Luau.lua_setglobal(L, n);
	}

	// Token: 0x06004F64 RID: 20324 RVA: 0x001A6FA0 File Offset: 0x001A51A0
	public unsafe static void lua_pop(lua_State* L, int n)
	{
		Luau.lua_settop(L, -n - 1);
	}

	// Token: 0x06004F65 RID: 20325 RVA: 0x001A6FAC File Offset: 0x001A51AC
	public unsafe static sbyte* lua_tostring(lua_State* L, int idx)
	{
		return Luau.lua_tolstring(L, idx, null);
	}

	// Token: 0x06004F66 RID: 20326
	[DllImport("luau")]
	public unsafe static extern int lua_isstring(lua_State* L, int index);

	// Token: 0x06004F67 RID: 20327
	[DllImport("luau")]
	public unsafe static extern int lua_type(lua_State* L, int index);

	// Token: 0x06004F68 RID: 20328
	[DllImport("luau")]
	public unsafe static extern int lua_pushstring(lua_State* L, [MarshalAs(UnmanagedType.LPStr)] string s);

	// Token: 0x06004F69 RID: 20329
	[DllImport("luau")]
	public unsafe static extern int lua_pushstring(lua_State* L, byte* s);

	// Token: 0x06004F6A RID: 20330
	[DllImport("luau")]
	public unsafe static extern int lua_error(lua_State* L);

	// Token: 0x06004F6B RID: 20331
	[DllImport("luau")]
	public unsafe static extern void luaL_errorL(lua_State* L, [MarshalAs(UnmanagedType.LPStr)] string fmt, [MarshalAs(UnmanagedType.LPStr)] params string[] a);

	// Token: 0x06004F6C RID: 20332
	[DllImport("luau")]
	public unsafe static extern void luaL_errorL(lua_State* L, sbyte* fmt);

	// Token: 0x06004F6D RID: 20333
	[DllImport("luau")]
	public unsafe static extern int lua_toboolean(lua_State* L, int index);

	// Token: 0x06004F6E RID: 20334
	[DllImport("luau")]
	public unsafe static extern byte* lua_debugtrace(lua_State* L);

	// Token: 0x06004F6F RID: 20335
	[DllImport("luau")]
	public unsafe static extern void lua_close(lua_State* L);

	// Token: 0x06004F70 RID: 20336
	[DllImport("luau")]
	public unsafe static extern int lua_ref(lua_State* L, int idx);

	// Token: 0x06004F71 RID: 20337
	[DllImport("luau")]
	public unsafe static extern void lua_unref(lua_State* L, int rid);

	// Token: 0x06004F72 RID: 20338 RVA: 0x001A6FB7 File Offset: 0x001A51B7
	public unsafe static void lua_getref(lua_State* L, int rid)
	{
		Luau.lua_rawgeti(L, -10000, rid);
	}

	// Token: 0x06004F73 RID: 20339
	[DllImport("luau")]
	public unsafe static extern void* lua_touserdatatagged(lua_State* L, int idx, int tag);

	// Token: 0x06004F74 RID: 20340
	[DllImport("luau")]
	public unsafe static extern void* lua_touserdata(lua_State* L, int index);

	// Token: 0x06004F75 RID: 20341
	[DllImport("luau")]
	public unsafe static extern void* lua_newuserdatatagged(lua_State* L, int sz, int tag);

	// Token: 0x06004F76 RID: 20342
	[DllImport("luau")]
	public unsafe static extern void lua_getuserdatametatable(lua_State* L, int tag);

	// Token: 0x06004F77 RID: 20343
	[DllImport("luau")]
	public unsafe static extern void lua_setuserdatametatable(lua_State* L, int tag, int idx);

	// Token: 0x06004F78 RID: 20344
	[DllImport("luau")]
	public unsafe static extern int lua_setmetatable(lua_State* L, int objindex);

	// Token: 0x06004F79 RID: 20345
	[DllImport("luau")]
	public unsafe static extern int luaL_newmetatable(lua_State* L, [MarshalAs(UnmanagedType.LPStr)] string tname);

	// Token: 0x06004F7A RID: 20346
	[DllImport("luau")]
	public unsafe static extern int lua_getfield(lua_State* L, int idx, [MarshalAs(UnmanagedType.LPStr)] string k);

	// Token: 0x06004F7B RID: 20347
	[DllImport("luau")]
	public unsafe static extern int lua_getfield(lua_State* L, int idx, byte* k);

	// Token: 0x06004F7C RID: 20348
	[DllImport("luau")]
	public unsafe static extern int luaL_getmetafield(lua_State* L, int idx, byte* k);

	// Token: 0x06004F7D RID: 20349
	[DllImport("luau")]
	public unsafe static extern int luaL_getmetafield(lua_State* L, int idx, [MarshalAs(UnmanagedType.LPStr)] string k);

	// Token: 0x06004F7E RID: 20350 RVA: 0x001A6FC5 File Offset: 0x001A51C5
	public unsafe static void luaL_getmetatable(lua_State* L, string n)
	{
		Luau.lua_getfield(L, -10000, n);
	}

	// Token: 0x06004F7F RID: 20351 RVA: 0x001A6FD4 File Offset: 0x001A51D4
	public unsafe static void luaL_getmetatable(lua_State* L, byte* n)
	{
		Luau.lua_getfield(L, -10000, n);
	}

	// Token: 0x06004F80 RID: 20352 RVA: 0x001A6FE3 File Offset: 0x001A51E3
	public unsafe static void lua_getglobal(lua_State* L, string n)
	{
		Luau.lua_getfield(L, -10002, n);
	}

	// Token: 0x06004F81 RID: 20353
	[DllImport("luau")]
	public unsafe static extern int lua_getmetatable(lua_State* L, int objindex);

	// Token: 0x06004F82 RID: 20354
	[DllImport("luau")]
	public unsafe static extern byte* lua_namecallatom(lua_State* L, int* atom);

	// Token: 0x06004F83 RID: 20355
	[DllImport("luau")]
	public unsafe static extern byte* luaL_checklstring(lua_State* L, int numArg, int* l);

	// Token: 0x06004F84 RID: 20356 RVA: 0x001A6FF2 File Offset: 0x001A51F2
	public unsafe static byte* luaL_checkstring(lua_State* L, int n)
	{
		return Luau.luaL_checklstring(L, n, null);
	}

	// Token: 0x06004F85 RID: 20357
	[DllImport("luau")]
	public unsafe static extern void lua_pushnumber(lua_State* L, double n);

	// Token: 0x06004F86 RID: 20358
	[DllImport("luau")]
	public unsafe static extern double luaL_checknumber(lua_State* L, int numArg);

	// Token: 0x06004F87 RID: 20359
	[DllImport("luau")]
	public unsafe static extern void lua_setreadonly(lua_State* L, int idx, int enabled);

	// Token: 0x06004F88 RID: 20360
	[DllImport("luau")]
	public unsafe static extern double lua_tonumberx(lua_State* L, int index, int* isnum);

	// Token: 0x06004F89 RID: 20361
	[DllImport("luau")]
	public unsafe static extern int lua_gc(lua_State* L, int what, int data);

	// Token: 0x06004F8A RID: 20362
	[DllImport("luau")]
	public unsafe static extern void lua_call(lua_State* L, int nargs, int nresults);

	// Token: 0x06004F8B RID: 20363
	[DllImport("luau")]
	public unsafe static extern int lua_pcall(lua_State* L, int nargs, int nresults, int fn);

	// Token: 0x06004F8C RID: 20364
	[DllImport("luau")]
	public unsafe static extern int lua_status(lua_State* L);

	// Token: 0x06004F8D RID: 20365
	[DllImport("luau")]
	public unsafe static extern void* luaL_checkudata(lua_State* L, int arg, [MarshalAs(UnmanagedType.LPStr)] string tname);

	// Token: 0x06004F8E RID: 20366
	[DllImport("luau")]
	public unsafe static extern void* luaL_checkudata(lua_State* L, int arg, byte* tname);

	// Token: 0x06004F8F RID: 20367
	[DllImport("luau")]
	public unsafe static extern int lua_objlen(lua_State* L, int index);

	// Token: 0x06004F90 RID: 20368
	[DllImport("luau")]
	public unsafe static extern double luaL_optnumber(lua_State* L, int narg, double d);

	// Token: 0x06004F91 RID: 20369
	[DllImport("luau")]
	public unsafe static extern void lua_createtable(lua_State* L, int narr, int nrec);

	// Token: 0x06004F92 RID: 20370
	[DllImport("luau")]
	public unsafe static extern void lua_pushlightuserdatatagged(lua_State* L, void* p, int tag);

	// Token: 0x06004F93 RID: 20371
	[DllImport("luau")]
	public unsafe static extern void lua_pushnil(lua_State* L);

	// Token: 0x06004F94 RID: 20372
	[DllImport("luau")]
	public unsafe static extern int lua_next(lua_State* L, int index);

	// Token: 0x06004F95 RID: 20373
	[DllImport("luau")]
	public unsafe static extern void lua_rawseti(lua_State* L, int idx, int n);

	// Token: 0x06004F96 RID: 20374
	[DllImport("luau")]
	public unsafe static extern void lua_rawgeti(lua_State* L, int index, int n);

	// Token: 0x06004F97 RID: 20375
	[DllImport("luau")]
	public unsafe static extern void lua_rawget(lua_State* L, int index);

	// Token: 0x06004F98 RID: 20376
	[DllImport("luau")]
	public unsafe static extern void lua_rawset(lua_State* L, int index);

	// Token: 0x06004F99 RID: 20377
	[DllImport("luau")]
	public unsafe static extern void lua_remove(lua_State* L, int index);

	// Token: 0x06004F9A RID: 20378
	[DllImport("luau")]
	public unsafe static extern void lua_pushboolean(lua_State* L, int b);

	// Token: 0x06004F9B RID: 20379
	[DllImport("luau")]
	public unsafe static extern int lua_rawequal(lua_State* L, int a, int b);

	// Token: 0x06004F9C RID: 20380 RVA: 0x001A6FFD File Offset: 0x001A51FD
	public unsafe static void* lua_newuserdata(lua_State* L, int size)
	{
		return Luau.lua_newuserdatatagged(L, size, 0);
	}

	// Token: 0x06004F9D RID: 20381 RVA: 0x001A7007 File Offset: 0x001A5207
	public unsafe static double lua_tonumber(lua_State* L, int index)
	{
		return Luau.lua_tonumberx(L, index, null);
	}

	// Token: 0x06004F9E RID: 20382 RVA: 0x001A7014 File Offset: 0x001A5214
	public unsafe static T* lua_class_push<[IsUnmanaged] T>(lua_State* L) where T : struct, ValueType
	{
		T* ptr = (T*)Luau.lua_newuserdata(L, sizeof(T) + 4);
		FixedString32Bytes name = BurstClassInfo.ClassList.MetatableNames<T>.Name;
		Luau.luaL_getmetatable(L, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2);
		Luau.lua_setmetatable(L, -2);
		return ptr;
	}

	// Token: 0x06004F9F RID: 20383 RVA: 0x001A7050 File Offset: 0x001A5250
	public unsafe static T* lua_class_push<[IsUnmanaged] T>(lua_State* L, FixedString32Bytes name) where T : struct, ValueType
	{
		T* ptr = (T*)Luau.lua_newuserdata(L, sizeof(T) + 4);
		Luau.luaL_getmetatable(L, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2);
		Luau.lua_setmetatable(L, -2);
		return ptr;
	}

	// Token: 0x06004FA0 RID: 20384 RVA: 0x001A7088 File Offset: 0x001A5288
	public unsafe static void lua_class_push(lua_State* L, FixedString32Bytes name, IntPtr ptr)
	{
		FixedString32Bytes fixedString32Bytes = "__ptr";
		Luau.lua_createtable(L, 0, 0);
		Luau.lua_pushlightuserdatatagged(L, (void*)ptr, 0);
		Luau.lua_setfield(L, -2, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
		Luau.luaL_getmetatable(L, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2);
		Luau.lua_setmetatable(L, -2);
	}

	// Token: 0x06004FA1 RID: 20385 RVA: 0x001A70E0 File Offset: 0x001A52E0
	public unsafe static T* lua_class_get<[IsUnmanaged] T>(lua_State* L, int idx) where T : struct, ValueType
	{
		int num = Luau.lua_type(L, idx);
		FixedString32Bytes name = BurstClassInfo.ClassList.MetatableNames<T>.Name;
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2;
		if (num == 8)
		{
			T* ptr2 = (T*)Luau.luaL_checkudata(L, idx, ptr);
			if (ptr2 != null)
			{
				return ptr2;
			}
		}
		if (num == 6)
		{
			Luau.lua_getmetatable(L, idx);
			Luau.luaL_getmetatable(L, ptr);
			bool flag = Luau.lua_rawequal(L, -1, -2) == 1;
			Luau.lua_pop(L, 2);
			if (flag)
			{
				Luau.lua_getfield(L, idx, "__ptr");
				if (Luau.lua_type(L, -1) == 2)
				{
					T* ptr3 = (T*)Luau.lua_touserdata(L, -1);
					Luau.lua_pop(L, 1);
					if (ptr3 != null)
					{
						return ptr3;
					}
				}
				Luau.lua_pop(L, 1);
			}
		}
		FixedString32Bytes fixedString32Bytes = "\"Invalid Type\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
		return null;
	}

	// Token: 0x06004FA2 RID: 20386 RVA: 0x001A7198 File Offset: 0x001A5398
	public unsafe static T* lua_class_get<[IsUnmanaged] T>(lua_State* L, int idx, FixedString32Bytes name) where T : struct, ValueType
	{
		int num = Luau.lua_type(L, idx);
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2;
		if (num == 8)
		{
			T* ptr2 = (T*)Luau.luaL_checkudata(L, idx, ptr);
			if (ptr2 != null)
			{
				return ptr2;
			}
		}
		if (num == 6)
		{
			Luau.lua_getmetatable(L, idx);
			Luau.luaL_getmetatable(L, ptr);
			bool flag = Luau.lua_rawequal(L, -1, -2) == 1;
			Luau.lua_pop(L, 1);
			if (flag)
			{
				FixedString32Bytes fixedString32Bytes = "__ptr";
				Luau.lua_getfield(L, idx, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
				if (Luau.lua_type(L, -1) == 2)
				{
					T* ptr3 = (T*)Luau.lua_touserdata(L, -1);
					Luau.lua_pop(L, 1);
					if (ptr3 != null)
					{
						return ptr3;
					}
				}
				Luau.lua_pop(L, 1);
			}
		}
		FixedString32Bytes fixedString32Bytes2 = "\"Invalid Type\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
		return null;
	}

	// Token: 0x06004FA3 RID: 20387 RVA: 0x001A7258 File Offset: 0x001A5458
	public unsafe static byte* lua_class_get(lua_State* L, int idx, FixedString32Bytes name)
	{
		int num = Luau.lua_type(L, idx);
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2;
		if (num == 8)
		{
			byte* ptr2 = (byte*)Luau.luaL_checkudata(L, idx, ptr);
			if (ptr2 != null)
			{
				return ptr2;
			}
		}
		if (num == 6)
		{
			Luau.lua_getmetatable(L, idx);
			Luau.luaL_getmetatable(L, ptr);
			bool flag = Luau.lua_rawequal(L, -1, -2) == 1;
			Luau.lua_pop(L, 1);
			if (flag)
			{
				FixedString32Bytes fixedString32Bytes = "__ptr";
				Luau.lua_getfield(L, idx, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
				if (Luau.lua_type(L, -1) == 2)
				{
					byte* ptr3 = (byte*)Luau.lua_touserdata(L, -1);
					Luau.lua_pop(L, 1);
					if (ptr3 != null)
					{
						return ptr3;
					}
				}
				Luau.lua_pop(L, 1);
			}
		}
		FixedString32Bytes fixedString32Bytes2 = "\"Invalid Type\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
		return null;
	}

	// Token: 0x06004FA4 RID: 20388 RVA: 0x001A7318 File Offset: 0x001A5518
	public unsafe static IntPtr lua_light_ptr(lua_State* L, int idx)
	{
		FixedString32Bytes fixedString32Bytes = "__ptr";
		Luau.lua_getfield(L, idx, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
		if (Luau.lua_type(L, -1) == 2)
		{
			void* ptr = Luau.lua_touserdata(L, -1);
			Luau.lua_pop(L, 1);
			if (ptr != null)
			{
				return (IntPtr)ptr;
			}
		}
		return IntPtr.Zero;
	}

	// Token: 0x06004FA5 RID: 20389 RVA: 0x001A736B File Offset: 0x001A556B
	public unsafe static bool lua_class_check<[IsUnmanaged] T>(lua_State* L, int idx) where T : struct, ValueType
	{
		return Luau.lua_objlen(L, idx) == sizeof(T);
	}

	// Token: 0x06004FA6 RID: 20390 RVA: 0x001A737C File Offset: 0x001A557C
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int lua_print(lua_State* L)
	{
		string text = "";
		int num = Luau.lua_gettop(L);
		for (int i = 1; i <= num; i++)
		{
			int num2 = Luau.lua_type(L, i);
			if (num2 == 5 || num2 == 3)
			{
				sbyte* ptr = Luau.lua_tostring(L, i);
				text += Marshal.PtrToStringAnsi((IntPtr)((void*)ptr));
			}
			else
			{
				if (num2 != 1)
				{
					Luau.luaL_errorL(L, "Invalid String", Array.Empty<string>());
					return 0;
				}
				int num3 = Luau.lua_toboolean(L, i);
				text += ((num3 == 1) ? "true" : "false");
			}
		}
		LuauHud.Instance.LuauLog(text);
		return 0;
	}

	// Token: 0x040061BC RID: 25020
	public const int LUA_GLOBALSINDEX = -10002;

	// Token: 0x040061BD RID: 25021
	public const int LUA_REGISTRYINDEX = -10000;

	// Token: 0x02000C95 RID: 3221
	public enum lua_Types
	{
		// Token: 0x040061BF RID: 25023
		LUA_TNIL,
		// Token: 0x040061C0 RID: 25024
		LUA_TBOOLEAN,
		// Token: 0x040061C1 RID: 25025
		LUA_TLIGHTUSERDATA,
		// Token: 0x040061C2 RID: 25026
		LUA_TNUMBER,
		// Token: 0x040061C3 RID: 25027
		LUA_TVECTOR,
		// Token: 0x040061C4 RID: 25028
		LUA_TSTRING,
		// Token: 0x040061C5 RID: 25029
		LUA_TTABLE,
		// Token: 0x040061C6 RID: 25030
		LUA_TFUNCTION,
		// Token: 0x040061C7 RID: 25031
		LUA_TUSERDATA,
		// Token: 0x040061C8 RID: 25032
		LUA_TTHREAD,
		// Token: 0x040061C9 RID: 25033
		LUA_TBUFFER,
		// Token: 0x040061CA RID: 25034
		LUA_TPROTO,
		// Token: 0x040061CB RID: 25035
		LUA_TUPVAL,
		// Token: 0x040061CC RID: 25036
		LUA_TDEADKEY,
		// Token: 0x040061CD RID: 25037
		LUA_T_COUNT = 11
	}

	// Token: 0x02000C96 RID: 3222
	public enum lua_Status
	{
		// Token: 0x040061CF RID: 25039
		LUA_OK,
		// Token: 0x040061D0 RID: 25040
		LUA_YIELD,
		// Token: 0x040061D1 RID: 25041
		LUA_ERRRUN,
		// Token: 0x040061D2 RID: 25042
		LUA_ERRSYNTAX,
		// Token: 0x040061D3 RID: 25043
		LUA_ERRMEM,
		// Token: 0x040061D4 RID: 25044
		LUA_ERRERR,
		// Token: 0x040061D5 RID: 25045
		LUA_BREAK
	}

	// Token: 0x02000C97 RID: 3223
	public enum gc_status
	{
		// Token: 0x040061D7 RID: 25047
		LUA_GCSTOP,
		// Token: 0x040061D8 RID: 25048
		LUA_GCRESTART,
		// Token: 0x040061D9 RID: 25049
		LUA_GCCOLLECT,
		// Token: 0x040061DA RID: 25050
		LUA_GCCOUNT,
		// Token: 0x040061DB RID: 25051
		LUA_GCISRUNNING,
		// Token: 0x040061DC RID: 25052
		LUA_GCSTEP,
		// Token: 0x040061DD RID: 25053
		LUA_GCSETGOAL,
		// Token: 0x040061DE RID: 25054
		LUA_GCSETSTEPMUL,
		// Token: 0x040061DF RID: 25055
		LUA_GCSETSTEPSIZE
	}

	// Token: 0x02000C98 RID: 3224
	public static class lua_TypeID
	{
		// Token: 0x06004FA8 RID: 20392 RVA: 0x001A7418 File Offset: 0x001A5618
		public static string get(Type t)
		{
			string text;
			if (Luau.lua_TypeID.names.TryGetValue(t, out text))
			{
				return text;
			}
			return "";
		}

		// Token: 0x06004FA9 RID: 20393 RVA: 0x001A743B File Offset: 0x001A563B
		public static void push(Type t, string name)
		{
			Luau.lua_TypeID.names.TryAdd(t, name);
		}

		// Token: 0x040061E0 RID: 25056
		private static Dictionary<Type, string> names = new Dictionary<Type, string>();
	}

	// Token: 0x02000C99 RID: 3225
	public static class lua_ClassFields<T>
	{
		// Token: 0x06004FAB RID: 20395 RVA: 0x001A7458 File Offset: 0x001A5658
		public static FieldInfo Get(string name)
		{
			Dictionary<int, FieldInfo> dictionary;
			FieldInfo fieldInfo;
			if (Luau.lua_ClassFields<T>.classDictionarys.TryGetValue(typeof(T).GetHashCode(), out dictionary) && dictionary.TryGetValue(name.GetHashCode(), out fieldInfo))
			{
				return fieldInfo;
			}
			return null;
		}

		// Token: 0x06004FAC RID: 20396 RVA: 0x001A7498 File Offset: 0x001A5698
		public static void Add(string name, FieldInfo field)
		{
			Dictionary<int, FieldInfo> dictionary;
			if (Luau.lua_ClassFields<T>.classDictionarys.TryGetValue(typeof(T).GetHashCode(), out dictionary))
			{
				dictionary.TryAdd(name.GetHashCode(), field);
				return;
			}
			Dictionary<int, FieldInfo> dictionary2 = new Dictionary<int, FieldInfo>();
			dictionary2.TryAdd(name.GetHashCode(), field);
			Luau.lua_ClassFields<T>.classDictionarys.TryAdd(typeof(T).GetHashCode(), dictionary2);
		}

		// Token: 0x040061E1 RID: 25057
		private static Dictionary<int, Dictionary<int, FieldInfo>> classDictionarys = new Dictionary<int, Dictionary<int, FieldInfo>>();
	}

	// Token: 0x02000C9A RID: 3226
	public static class lua_ClassProperties<T>
	{
		// Token: 0x06004FAE RID: 20398 RVA: 0x001A750C File Offset: 0x001A570C
		public static lua_CFunction Get(string name)
		{
			Dictionary<string, lua_CFunction> dictionary;
			lua_CFunction lua_CFunction;
			if (Luau.lua_ClassProperties<T>.classProperties.TryGetValue(typeof(T), out dictionary) && dictionary.TryGetValue(name, out lua_CFunction))
			{
				return lua_CFunction;
			}
			return null;
		}

		// Token: 0x06004FAF RID: 20399 RVA: 0x001A7540 File Offset: 0x001A5740
		public static void Add(string name, lua_CFunction field)
		{
			Dictionary<string, lua_CFunction> dictionary;
			if (Luau.lua_ClassProperties<T>.classProperties.TryGetValue(typeof(T), out dictionary))
			{
				dictionary.TryAdd(name, field);
				return;
			}
			Dictionary<string, lua_CFunction> dictionary2 = new Dictionary<string, lua_CFunction>();
			dictionary2.TryAdd(name, field);
			Luau.lua_ClassProperties<T>.classProperties.TryAdd(typeof(T), dictionary2);
		}

		// Token: 0x040061E2 RID: 25058
		private static Dictionary<Type, Dictionary<string, lua_CFunction>> classProperties = new Dictionary<Type, Dictionary<string, lua_CFunction>>();
	}

	// Token: 0x02000C9B RID: 3227
	public static class lua_ClassFunctions<T>
	{
		// Token: 0x06004FB1 RID: 20401 RVA: 0x001A75A0 File Offset: 0x001A57A0
		public static lua_CFunction Get(string name)
		{
			Dictionary<string, lua_CFunction> dictionary;
			lua_CFunction lua_CFunction;
			if (Luau.lua_ClassFunctions<T>.classProperties.TryGetValue(typeof(T), out dictionary) && dictionary.TryGetValue(name, out lua_CFunction))
			{
				return lua_CFunction;
			}
			return null;
		}

		// Token: 0x06004FB2 RID: 20402 RVA: 0x001A75D4 File Offset: 0x001A57D4
		public static void Add(string name, lua_CFunction field)
		{
			Dictionary<string, lua_CFunction> dictionary;
			if (Luau.lua_ClassFunctions<T>.classProperties.TryGetValue(typeof(T), out dictionary))
			{
				dictionary.TryAdd(name, field);
				return;
			}
			Dictionary<string, lua_CFunction> dictionary2 = new Dictionary<string, lua_CFunction>();
			dictionary2.TryAdd(name, field);
			Luau.lua_ClassFunctions<T>.classProperties.TryAdd(typeof(T), dictionary2);
		}

		// Token: 0x040061E3 RID: 25059
		private static Dictionary<Type, Dictionary<string, lua_CFunction>> classProperties = new Dictionary<Type, Dictionary<string, lua_CFunction>>();
	}
}
