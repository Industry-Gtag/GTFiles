using System;
using System.Text;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000EAF RID: 3759
	public class User
	{
		// Token: 0x06005B34 RID: 23348 RVA: 0x001D9CDD File Offset: 0x001D7EDD
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			User.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06005B35 RID: 23349 RVA: 0x001D9CEC File Offset: 0x001D7EEC
		public static int IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			User.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(User.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return User.IsReady_64(new StatusCallback(User.IsReadyIl2cppCallback));
			}
			return User.IsReady(new StatusCallback(User.IsReadyIl2cppCallback));
		}

		// Token: 0x06005B36 RID: 23350 RVA: 0x001D9D5C File Offset: 0x001D7F5C
		public static string GetUserId()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (IntPtr.Size == 8)
			{
				User.GetUserID_64(stringBuilder, 256);
			}
			else
			{
				User.GetUserID(stringBuilder, 256);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06005B37 RID: 23351 RVA: 0x001D9D9C File Offset: 0x001D7F9C
		public static string GetUserName()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (IntPtr.Size == 8)
			{
				User.GetUserName_64(stringBuilder, 256);
			}
			else
			{
				User.GetUserName(stringBuilder, 256);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06005B38 RID: 23352 RVA: 0x001D9DDC File Offset: 0x001D7FDC
		public static string GetUserAvatarUrl()
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			if (IntPtr.Size == 8)
			{
				User.GetUserAvatarUrl_64(stringBuilder, 512);
			}
			else
			{
				User.GetUserAvatarUrl(stringBuilder, 512);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04006C0E RID: 27662
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x04006C0F RID: 27663
		private const int MaxIdLength = 256;

		// Token: 0x04006C10 RID: 27664
		private const int MaxNameLength = 256;

		// Token: 0x04006C11 RID: 27665
		private const int MaxUrlLength = 512;
	}
}
