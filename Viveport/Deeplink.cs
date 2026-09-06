using System;
using System.Text;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000EC7 RID: 3783
	public class Deeplink
	{
		// Token: 0x06005C10 RID: 23568 RVA: 0x001DC07D File Offset: 0x001DA27D
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			Deeplink.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06005C11 RID: 23569 RVA: 0x001DC08C File Offset: 0x001DA28C
		public static void IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Deeplink.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(Deeplink.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.IsReady_64(new StatusCallback(Deeplink.IsReadyIl2cppCallback));
				return;
			}
			Deeplink.IsReady(new StatusCallback(Deeplink.IsReadyIl2cppCallback));
		}

		// Token: 0x06005C12 RID: 23570 RVA: 0x001DC0F9 File Offset: 0x001DA2F9
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GoToAppIl2cppCallback(int errorCode, string message)
		{
			Deeplink.goToAppIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005C13 RID: 23571 RVA: 0x001DC108 File Offset: 0x001DA308
		public static void GoToApp(StatusCallback2 callback, string viveportId, string launchData)
		{
			if (callback == null || string.IsNullOrEmpty(viveportId))
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(viveportId)");
			}
			Deeplink.goToAppIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Deeplink.GoToAppIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.GoToApp_64(new StatusCallback2(Deeplink.GoToAppIl2cppCallback), viveportId, launchData);
				return;
			}
			Deeplink.GoToApp(new StatusCallback2(Deeplink.GoToAppIl2cppCallback), viveportId, launchData);
		}

		// Token: 0x06005C14 RID: 23572 RVA: 0x001DC181 File Offset: 0x001DA381
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GoToAppWithBranchNameIl2cppCallback(int errorCode, string message)
		{
			Deeplink.goToAppWithBranchNameIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005C15 RID: 23573 RVA: 0x001DC190 File Offset: 0x001DA390
		public static void GoToApp(StatusCallback2 callback, string viveportId, string launchData, string branchName)
		{
			if (callback == null || string.IsNullOrEmpty(viveportId))
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(viveportId)");
			}
			Deeplink.goToAppWithBranchNameIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Deeplink.GoToAppWithBranchNameIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.GoToApp_64(new StatusCallback2(Deeplink.GoToAppWithBranchNameIl2cppCallback), viveportId, launchData, branchName);
				return;
			}
			Deeplink.GoToApp(new StatusCallback2(Deeplink.GoToAppWithBranchNameIl2cppCallback), viveportId, launchData, branchName);
		}

		// Token: 0x06005C16 RID: 23574 RVA: 0x001DC20B File Offset: 0x001DA40B
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GoToStoreIl2cppCallback(int errorCode, string message)
		{
			Deeplink.goToStoreIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005C17 RID: 23575 RVA: 0x001DC21C File Offset: 0x001DA41C
		public static void GoToStore(StatusCallback2 callback, string viveportId = "")
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(viveportId)");
			}
			Deeplink.goToStoreIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Deeplink.GoToStoreIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.GoToStore_64(new StatusCallback2(Deeplink.GoToStoreIl2cppCallback), viveportId);
				return;
			}
			Deeplink.GoToStore(new StatusCallback2(Deeplink.GoToStoreIl2cppCallback), viveportId);
		}

		// Token: 0x06005C18 RID: 23576 RVA: 0x001DC28B File Offset: 0x001DA48B
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GoToAppOrGoToStoreIl2cppCallback(int errorCode, string message)
		{
			Deeplink.goToAppOrGoToStoreIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005C19 RID: 23577 RVA: 0x001DC29C File Offset: 0x001DA49C
		public static void GoToAppOrGoToStore(StatusCallback2 callback, string viveportId, string launchData)
		{
			if (callback == null || string.IsNullOrEmpty(viveportId))
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(viveportId)");
			}
			Deeplink.goToAppOrGoToStoreIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Deeplink.GoToAppOrGoToStoreIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.GoToAppOrGoToStore_64(new StatusCallback2(Deeplink.GoToAppOrGoToStoreIl2cppCallback), viveportId, launchData);
				return;
			}
			Deeplink.GoToAppOrGoToStore(new StatusCallback2(Deeplink.GoToAppOrGoToStoreIl2cppCallback), viveportId, launchData);
		}

		// Token: 0x06005C1A RID: 23578 RVA: 0x001DC318 File Offset: 0x001DA518
		public static string GetAppLaunchData()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (IntPtr.Size == 8)
			{
				Deeplink.GetAppLaunchData_64(stringBuilder, 256);
			}
			else
			{
				Deeplink.GetAppLaunchData(stringBuilder, 256);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04006C71 RID: 27761
		private const int MaxIdLength = 256;

		// Token: 0x04006C72 RID: 27762
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x04006C73 RID: 27763
		private static StatusCallback2 goToAppIl2cppCallback;

		// Token: 0x04006C74 RID: 27764
		private static StatusCallback2 goToAppWithBranchNameIl2cppCallback;

		// Token: 0x04006C75 RID: 27765
		private static StatusCallback2 goToStoreIl2cppCallback;

		// Token: 0x04006C76 RID: 27766
		private static StatusCallback2 goToAppOrGoToStoreIl2cppCallback;
	}
}
