using System;
using System.Text;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000EC5 RID: 3781
	public class DLC
	{
		// Token: 0x06005C06 RID: 23558 RVA: 0x001DBE14 File Offset: 0x001DA014
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsDlcReadyIl2cppCallback(int errorCode)
		{
			DLC.isDlcReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06005C07 RID: 23559 RVA: 0x001DBE24 File Offset: 0x001DA024
		public static int IsDlcReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			DLC.isDlcReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(DLC.IsDlcReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return DLC.IsReady_64(new StatusCallback(DLC.IsDlcReadyIl2cppCallback));
			}
			return DLC.IsReady(new StatusCallback(DLC.IsDlcReadyIl2cppCallback));
		}

		// Token: 0x06005C08 RID: 23560 RVA: 0x001DBE91 File Offset: 0x001DA091
		public static int GetCount()
		{
			if (IntPtr.Size == 8)
			{
				return DLC.GetCount_64();
			}
			return DLC.GetCount();
		}

		// Token: 0x06005C09 RID: 23561 RVA: 0x001DBEA8 File Offset: 0x001DA0A8
		public static bool GetIsAvailable(int index, out string appId, out bool isAvailable)
		{
			StringBuilder stringBuilder = new StringBuilder(37);
			bool flag;
			if (IntPtr.Size == 8)
			{
				flag = DLC.GetIsAvailable_64(index, stringBuilder, out isAvailable);
			}
			else
			{
				flag = DLC.GetIsAvailable(index, stringBuilder, out isAvailable);
			}
			appId = stringBuilder.ToString();
			return flag;
		}

		// Token: 0x06005C0A RID: 23562 RVA: 0x001DBEE4 File Offset: 0x001DA0E4
		public static int IsSubscribed()
		{
			if (IntPtr.Size == 8)
			{
				return DLC.IsSubscribed_64();
			}
			return DLC.IsSubscribed();
		}

		// Token: 0x04006C6E RID: 27758
		private static StatusCallback isDlcReadyIl2cppCallback;

		// Token: 0x04006C6F RID: 27759
		private const int AppIdLength = 37;
	}
}
