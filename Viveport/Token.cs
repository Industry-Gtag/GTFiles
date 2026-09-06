using System;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000EC8 RID: 3784
	internal class Token
	{
		// Token: 0x06005C1C RID: 23580 RVA: 0x001DC358 File Offset: 0x001DA558
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			Token.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06005C1D RID: 23581 RVA: 0x001DC368 File Offset: 0x001DA568
		public static void IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Token.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(Token.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Token.IsReady_64(new StatusCallback(Token.IsReadyIl2cppCallback));
				return;
			}
			Token.IsReady(new StatusCallback(Token.IsReadyIl2cppCallback));
		}

		// Token: 0x06005C1E RID: 23582 RVA: 0x001DC3D7 File Offset: 0x001DA5D7
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GetSessionTokenIl2cppCallback(int errorCode, string message)
		{
			Token.getSessionTokenIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005C1F RID: 23583 RVA: 0x001DC3E8 File Offset: 0x001DA5E8
		public static void GetSessionToken(StatusCallback2 callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Token.getSessionTokenIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Token.GetSessionTokenIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Token.GetSessionToken_64(new StatusCallback2(Token.GetSessionTokenIl2cppCallback));
				return;
			}
			Token.GetSessionToken(new StatusCallback2(Token.GetSessionTokenIl2cppCallback));
		}

		// Token: 0x04006C77 RID: 27767
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x04006C78 RID: 27768
		private static StatusCallback2 getSessionTokenIl2cppCallback;
	}
}
