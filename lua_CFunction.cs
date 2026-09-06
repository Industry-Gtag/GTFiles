using System;
using System.Runtime.InteropServices;

// Token: 0x02000C92 RID: 3218
// (Invoke) Token: 0x06004F4B RID: 20299
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate int lua_CFunction(lua_State* L);
