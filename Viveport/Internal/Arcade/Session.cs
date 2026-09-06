using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal.Arcade
{
	// Token: 0x02000EE2 RID: 3810
	internal class Session
	{
		// Token: 0x06005CC8 RID: 23752
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IViveportArcadeSession_IsReady")]
		internal static extern void IsReady(SessionCallback callback);

		// Token: 0x06005CC9 RID: 23753
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IViveportArcadeSession_IsReady")]
		internal static extern void IsReady_64(SessionCallback callback);

		// Token: 0x06005CCA RID: 23754
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IViveportArcadeSession_Start")]
		internal static extern void Start(SessionCallback callback);

		// Token: 0x06005CCB RID: 23755
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IViveportArcadeSession_Start")]
		internal static extern void Start_64(SessionCallback callback);

		// Token: 0x06005CCC RID: 23756
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IViveportArcadeSession_Stop")]
		internal static extern void Stop(SessionCallback callback);

		// Token: 0x06005CCD RID: 23757
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IViveportArcadeSession_Stop")]
		internal static extern void Stop_64(SessionCallback callback);
	}
}
