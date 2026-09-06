using System;
using System.Runtime.InteropServices;
using AOT;
using LitJson;
using Viveport.Core;
using Viveport.Internal.Arcade;

namespace Viveport.Arcade
{
	// Token: 0x02000EE3 RID: 3811
	internal class Session
	{
		// Token: 0x06005CCF RID: 23759 RVA: 0x001DC4B1 File Offset: 0x001DA6B1
		[MonoPInvokeCallback(typeof(SessionCallback))]
		private static void IsReadyIl2cppCallback(int errorCode, string message)
		{
			Session.isReadyIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005CD0 RID: 23760 RVA: 0x001DC4BF File Offset: 0x001DA6BF
		public static void IsReady(Session.SessionListener listener)
		{
			Session.isReadyIl2cppCallback = new Session.SessionHandler(listener).getIsReadyHandler();
			if (IntPtr.Size == 8)
			{
				Session.IsReady_64(new SessionCallback(Session.IsReadyIl2cppCallback));
				return;
			}
			Session.IsReady(new SessionCallback(Session.IsReadyIl2cppCallback));
		}

		// Token: 0x06005CD1 RID: 23761 RVA: 0x001DC4FC File Offset: 0x001DA6FC
		[MonoPInvokeCallback(typeof(SessionCallback))]
		private static void StartIl2cppCallback(int errorCode, string message)
		{
			Session.startIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005CD2 RID: 23762 RVA: 0x001DC50A File Offset: 0x001DA70A
		public static void Start(Session.SessionListener listener)
		{
			Session.startIl2cppCallback = new Session.SessionHandler(listener).getStartHandler();
			if (IntPtr.Size == 8)
			{
				Session.Start_64(new SessionCallback(Session.StartIl2cppCallback));
				return;
			}
			Session.Start(new SessionCallback(Session.StartIl2cppCallback));
		}

		// Token: 0x06005CD3 RID: 23763 RVA: 0x001DC547 File Offset: 0x001DA747
		[MonoPInvokeCallback(typeof(SessionCallback))]
		private static void StopIl2cppCallback(int errorCode, string message)
		{
			Session.stopIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005CD4 RID: 23764 RVA: 0x001DC555 File Offset: 0x001DA755
		public static void Stop(Session.SessionListener listener)
		{
			Session.stopIl2cppCallback = new Session.SessionHandler(listener).getStopHandler();
			if (IntPtr.Size == 8)
			{
				Session.Stop_64(new SessionCallback(Session.StopIl2cppCallback));
				return;
			}
			Session.Stop(new SessionCallback(Session.StopIl2cppCallback));
		}

		// Token: 0x04006CA7 RID: 27815
		private static SessionCallback isReadyIl2cppCallback;

		// Token: 0x04006CA8 RID: 27816
		private static SessionCallback startIl2cppCallback;

		// Token: 0x04006CA9 RID: 27817
		private static SessionCallback stopIl2cppCallback;

		// Token: 0x02000EE4 RID: 3812
		private class SessionHandler : Session.BaseHandler
		{
			// Token: 0x06005CD6 RID: 23766 RVA: 0x001DC592 File Offset: 0x001DA792
			public SessionHandler(Session.SessionListener cb)
			{
				Session.SessionHandler.listener = cb;
			}

			// Token: 0x06005CD7 RID: 23767 RVA: 0x001DC5A0 File Offset: 0x001DA7A0
			public SessionCallback getIsReadyHandler()
			{
				return new SessionCallback(this.IsReadyHandler);
			}

			// Token: 0x06005CD8 RID: 23768 RVA: 0x001DC5B0 File Offset: 0x001DA7B0
			protected override void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[Session IsReadyHandler] message=" + message + ",code=" + code.ToString());
				JsonData jsonData = null;
				try
				{
					jsonData = JsonMapper.ToObject(message);
				}
				catch (Exception ex)
				{
					string text = "[Session IsReadyHandler] exception=";
					Exception ex2 = ex;
					Logger.Log(text + ((ex2 != null) ? ex2.ToString() : null));
				}
				int num = -1;
				string text2 = "";
				string text3 = "";
				if (code == 0 && jsonData != null)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex3)
					{
						string text4 = "[IsReadyHandler] statusCode, message ex=";
						Exception ex4 = ex3;
						Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[IsReadyHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text3 = (string)jsonData["appID"];
						}
						catch (Exception ex5)
						{
							string text5 = "[IsReadyHandler] appID ex=";
							Exception ex6 = ex5;
							Logger.Log(text5 + ((ex6 != null) ? ex6.ToString() : null));
						}
						Logger.Log("[IsReadyHandler] appID=" + text3);
					}
				}
				if (Session.SessionHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							Session.SessionHandler.listener.OnSuccess(text3);
							return;
						}
						Session.SessionHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						Session.SessionHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06005CD9 RID: 23769 RVA: 0x001DC724 File Offset: 0x001DA924
			public SessionCallback getStartHandler()
			{
				return new SessionCallback(this.StartHandler);
			}

			// Token: 0x06005CDA RID: 23770 RVA: 0x001DC734 File Offset: 0x001DA934
			protected override void StartHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[Session StartHandler] message=" + message + ",code=" + code.ToString());
				JsonData jsonData = null;
				try
				{
					jsonData = JsonMapper.ToObject(message);
				}
				catch (Exception ex)
				{
					string text = "[Session StartHandler] exception=";
					Exception ex2 = ex;
					Logger.Log(text + ((ex2 != null) ? ex2.ToString() : null));
				}
				int num = -1;
				string text2 = "";
				string text3 = "";
				string text4 = "";
				if (code == 0 && jsonData != null)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex3)
					{
						string text5 = "[StartHandler] statusCode, message ex=";
						Exception ex4 = ex3;
						Logger.Log(text5 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[StartHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text3 = (string)jsonData["appID"];
							text4 = (string)jsonData["Guid"];
						}
						catch (Exception ex5)
						{
							string text6 = "[StartHandler] appID, Guid ex=";
							Exception ex6 = ex5;
							Logger.Log(text6 + ((ex6 != null) ? ex6.ToString() : null));
						}
						Logger.Log("[StartHandler] appID=" + text3 + ",Guid=" + text4);
					}
				}
				if (Session.SessionHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							Session.SessionHandler.listener.OnStartSuccess(text3, text4);
							return;
						}
						Session.SessionHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						Session.SessionHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06005CDB RID: 23771 RVA: 0x001DC8C8 File Offset: 0x001DAAC8
			public SessionCallback getStopHandler()
			{
				return new SessionCallback(this.StopHandler);
			}

			// Token: 0x06005CDC RID: 23772 RVA: 0x001DC8D8 File Offset: 0x001DAAD8
			protected override void StopHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[Session StopHandler] message=" + message + ",code=" + code.ToString());
				JsonData jsonData = null;
				try
				{
					jsonData = JsonMapper.ToObject(message);
				}
				catch (Exception ex)
				{
					string text = "[Session StopHandler] exception=";
					Exception ex2 = ex;
					Logger.Log(text + ((ex2 != null) ? ex2.ToString() : null));
				}
				int num = -1;
				string text2 = "";
				string text3 = "";
				string text4 = "";
				if (code == 0 && jsonData != null)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex3)
					{
						string text5 = "[StopHandler] statusCode, message ex=";
						Exception ex4 = ex3;
						Logger.Log(text5 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[StopHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text3 = (string)jsonData["appID"];
							text4 = (string)jsonData["Guid"];
						}
						catch (Exception ex5)
						{
							string text6 = "[StopHandler] appID, Guid ex=";
							Exception ex6 = ex5;
							Logger.Log(text6 + ((ex6 != null) ? ex6.ToString() : null));
						}
						Logger.Log("[StopHandler] appID=" + text3 + ",Guid=" + text4);
					}
				}
				if (Session.SessionHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							Session.SessionHandler.listener.OnStopSuccess(text3, text4);
							return;
						}
						Session.SessionHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						Session.SessionHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x04006CAA RID: 27818
			private static Session.SessionListener listener;
		}

		// Token: 0x02000EE5 RID: 3813
		private abstract class BaseHandler
		{
			// Token: 0x06005CDD RID: 23773
			protected abstract void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06005CDE RID: 23774
			protected abstract void StartHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06005CDF RID: 23775
			protected abstract void StopHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);
		}

		// Token: 0x02000EE6 RID: 3814
		public class SessionListener
		{
			// Token: 0x06005CE1 RID: 23777 RVA: 0x00002C2D File Offset: 0x00000E2D
			public virtual void OnSuccess(string pchAppID)
			{
			}

			// Token: 0x06005CE2 RID: 23778 RVA: 0x00002C2D File Offset: 0x00000E2D
			public virtual void OnStartSuccess(string pchAppID, string pchGuid)
			{
			}

			// Token: 0x06005CE3 RID: 23779 RVA: 0x00002C2D File Offset: 0x00000E2D
			public virtual void OnStopSuccess(string pchAppID, string pchGuid)
			{
			}

			// Token: 0x06005CE4 RID: 23780 RVA: 0x00002C2D File Offset: 0x00000E2D
			public virtual void OnFailure(int nCode, string pchMessage)
			{
			}
		}
	}
}
