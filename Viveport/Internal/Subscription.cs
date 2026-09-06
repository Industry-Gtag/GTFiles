using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000EDE RID: 3806
	internal class Subscription
	{
		// Token: 0x06005CA6 RID: 23718 RVA: 0x001DC457 File Offset: 0x001DA657
		static Subscription()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x06005CA7 RID: 23719
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportSubscription_IsReady")]
		internal static extern void IsReady(StatusCallback2 IsReadyCallback);

		// Token: 0x06005CA8 RID: 23720
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportSubscription_IsReady")]
		internal static extern void IsReady_64(StatusCallback2 IsReadyCallback);

		// Token: 0x06005CA9 RID: 23721
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportSubscription_IsWindowsSubscriber")]
		internal static extern bool IsWindowsSubscriber();

		// Token: 0x06005CAA RID: 23722
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportSubscription_IsWindowsSubscriber")]
		internal static extern bool IsWindowsSubscriber_64();

		// Token: 0x06005CAB RID: 23723
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportSubscription_IsAndroidSubscriber")]
		internal static extern bool IsAndroidSubscriber();

		// Token: 0x06005CAC RID: 23724
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportSubscription_IsAndroidSubscriber")]
		internal static extern bool IsAndroidSubscriber_64();

		// Token: 0x06005CAD RID: 23725
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportSubscription_GetTransactionType")]
		internal static extern ESubscriptionTransactionType GetTransactionType();

		// Token: 0x06005CAE RID: 23726
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportSubscription_GetTransactionType")]
		internal static extern ESubscriptionTransactionType GetTransactionType_64();
	}
}
