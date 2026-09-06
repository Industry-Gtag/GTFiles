using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using LitJson;
using Viveport.Core;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000EB9 RID: 3769
	public class IAPurchase
	{
		// Token: 0x06005B5F RID: 23391 RVA: 0x001DA493 File Offset: 0x001D8693
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void IsReadyIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.isReadyIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005B60 RID: 23392 RVA: 0x001DA4A1 File Offset: 0x001D86A1
		public static void IsReady(IAPurchase.IAPurchaseListener listener, string pchAppKey)
		{
			IAPurchase.isReadyIl2cppCallback = new IAPurchase.IAPHandler(listener).getIsReadyHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.IsReady_64(new IAPurchaseCallback(IAPurchase.IsReadyIl2cppCallback), pchAppKey);
				return;
			}
			IAPurchase.IsReady(new IAPurchaseCallback(IAPurchase.IsReadyIl2cppCallback), pchAppKey);
		}

		// Token: 0x06005B61 RID: 23393 RVA: 0x001DA4E0 File Offset: 0x001D86E0
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Request01Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.request01Il2cppCallback(errorCode, message);
		}

		// Token: 0x06005B62 RID: 23394 RVA: 0x001DA4EE File Offset: 0x001D86EE
		public static void Request(IAPurchase.IAPurchaseListener listener, string pchPrice)
		{
			IAPurchase.request01Il2cppCallback = new IAPurchase.IAPHandler(listener).getRequestHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Request_64(new IAPurchaseCallback(IAPurchase.Request01Il2cppCallback), pchPrice);
				return;
			}
			IAPurchase.Request(new IAPurchaseCallback(IAPurchase.Request01Il2cppCallback), pchPrice);
		}

		// Token: 0x06005B63 RID: 23395 RVA: 0x001DA52D File Offset: 0x001D872D
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Request02Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.request02Il2cppCallback(errorCode, message);
		}

		// Token: 0x06005B64 RID: 23396 RVA: 0x001DA53C File Offset: 0x001D873C
		public static void Request(IAPurchase.IAPurchaseListener listener, string pchPrice, string pchUserData)
		{
			IAPurchase.request02Il2cppCallback = new IAPurchase.IAPHandler(listener).getRequestHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Request_64(new IAPurchaseCallback(IAPurchase.Request02Il2cppCallback), pchPrice, pchUserData);
				return;
			}
			IAPurchase.Request(new IAPurchaseCallback(IAPurchase.Request02Il2cppCallback), pchPrice, pchUserData);
		}

		// Token: 0x06005B65 RID: 23397 RVA: 0x001DA588 File Offset: 0x001D8788
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void PurchaseIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.purchaseIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005B66 RID: 23398 RVA: 0x001DA596 File Offset: 0x001D8796
		public static void Purchase(IAPurchase.IAPurchaseListener listener, string pchPurchaseId)
		{
			IAPurchase.purchaseIl2cppCallback = new IAPurchase.IAPHandler(listener).getPurchaseHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Purchase_64(new IAPurchaseCallback(IAPurchase.PurchaseIl2cppCallback), pchPurchaseId);
				return;
			}
			IAPurchase.Purchase(new IAPurchaseCallback(IAPurchase.PurchaseIl2cppCallback), pchPurchaseId);
		}

		// Token: 0x06005B67 RID: 23399 RVA: 0x001DA5D5 File Offset: 0x001D87D5
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Query01Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.query01Il2cppCallback(errorCode, message);
		}

		// Token: 0x06005B68 RID: 23400 RVA: 0x001DA5E3 File Offset: 0x001D87E3
		public static void Query(IAPurchase.IAPurchaseListener listener, string pchPurchaseId)
		{
			IAPurchase.query01Il2cppCallback = new IAPurchase.IAPHandler(listener).getQueryHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Query_64(new IAPurchaseCallback(IAPurchase.Query01Il2cppCallback), pchPurchaseId);
				return;
			}
			IAPurchase.Query(new IAPurchaseCallback(IAPurchase.Query01Il2cppCallback), pchPurchaseId);
		}

		// Token: 0x06005B69 RID: 23401 RVA: 0x001DA622 File Offset: 0x001D8822
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Query02Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.query02Il2cppCallback(errorCode, message);
		}

		// Token: 0x06005B6A RID: 23402 RVA: 0x001DA630 File Offset: 0x001D8830
		public static void Query(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.query02Il2cppCallback = new IAPurchase.IAPHandler(listener).getQueryListHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Query_64(new IAPurchaseCallback(IAPurchase.Query02Il2cppCallback));
				return;
			}
			IAPurchase.Query(new IAPurchaseCallback(IAPurchase.Query02Il2cppCallback));
		}

		// Token: 0x06005B6B RID: 23403 RVA: 0x001DA66D File Offset: 0x001D886D
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void GetBalanceIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.getBalanceIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005B6C RID: 23404 RVA: 0x001DA67B File Offset: 0x001D887B
		public static void GetBalance(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.getBalanceIl2cppCallback = new IAPurchase.IAPHandler(listener).getBalanceHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.GetBalance_64(new IAPurchaseCallback(IAPurchase.GetBalanceIl2cppCallback));
				return;
			}
			IAPurchase.GetBalance(new IAPurchaseCallback(IAPurchase.GetBalanceIl2cppCallback));
		}

		// Token: 0x06005B6D RID: 23405 RVA: 0x001DA6B8 File Offset: 0x001D88B8
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void RequestSubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.requestSubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005B6E RID: 23406 RVA: 0x001DA6C8 File Offset: 0x001D88C8
		public static void RequestSubscription(IAPurchase.IAPurchaseListener listener, string pchPrice, string pchFreeTrialType, int nFreeTrialValue, string pchChargePeriodType, int nChargePeriodValue, int nNumberOfChargePeriod, string pchPlanId)
		{
			IAPurchase.requestSubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getRequestSubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.RequestSubscription_64(new IAPurchaseCallback(IAPurchase.RequestSubscriptionIl2cppCallback), pchPrice, pchFreeTrialType, nFreeTrialValue, pchChargePeriodType, nChargePeriodValue, nNumberOfChargePeriod, pchPlanId);
				return;
			}
			IAPurchase.RequestSubscription(new IAPurchaseCallback(IAPurchase.RequestSubscriptionIl2cppCallback), pchPrice, pchFreeTrialType, nFreeTrialValue, pchChargePeriodType, nChargePeriodValue, nNumberOfChargePeriod, pchPlanId);
		}

		// Token: 0x06005B6F RID: 23407 RVA: 0x001DA726 File Offset: 0x001D8926
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void RequestSubscriptionWithPlanIDIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.requestSubscriptionWithPlanIDIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005B70 RID: 23408 RVA: 0x001DA734 File Offset: 0x001D8934
		public static void RequestSubscriptionWithPlanID(IAPurchase.IAPurchaseListener listener, string pchPlanId)
		{
			IAPurchase.requestSubscriptionWithPlanIDIl2cppCallback = new IAPurchase.IAPHandler(listener).getRequestSubscriptionWithPlanIDHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.RequestSubscriptionWithPlanID_64(new IAPurchaseCallback(IAPurchase.RequestSubscriptionWithPlanIDIl2cppCallback), pchPlanId);
				return;
			}
			IAPurchase.RequestSubscriptionWithPlanID(new IAPurchaseCallback(IAPurchase.RequestSubscriptionWithPlanIDIl2cppCallback), pchPlanId);
		}

		// Token: 0x06005B71 RID: 23409 RVA: 0x001DA773 File Offset: 0x001D8973
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void SubscribeIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.subscribeIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005B72 RID: 23410 RVA: 0x001DA781 File Offset: 0x001D8981
		public static void Subscribe(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.subscribeIl2cppCallback = new IAPurchase.IAPHandler(listener).getSubscribeHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Subscribe_64(new IAPurchaseCallback(IAPurchase.SubscribeIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.Subscribe(new IAPurchaseCallback(IAPurchase.SubscribeIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x06005B73 RID: 23411 RVA: 0x001DA7C0 File Offset: 0x001D89C0
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void QuerySubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.querySubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005B74 RID: 23412 RVA: 0x001DA7CE File Offset: 0x001D89CE
		public static void QuerySubscription(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.querySubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getQuerySubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.QuerySubscription_64(new IAPurchaseCallback(IAPurchase.QuerySubscriptionIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.QuerySubscription(new IAPurchaseCallback(IAPurchase.QuerySubscriptionIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x06005B75 RID: 23413 RVA: 0x001DA80D File Offset: 0x001D8A0D
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void QuerySubscriptionListIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.querySubscriptionListIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005B76 RID: 23414 RVA: 0x001DA81B File Offset: 0x001D8A1B
		public static void QuerySubscriptionList(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.querySubscriptionListIl2cppCallback = new IAPurchase.IAPHandler(listener).getQuerySubscriptionListHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.QuerySubscriptionList_64(new IAPurchaseCallback(IAPurchase.QuerySubscriptionListIl2cppCallback));
				return;
			}
			IAPurchase.QuerySubscriptionList(new IAPurchaseCallback(IAPurchase.QuerySubscriptionListIl2cppCallback));
		}

		// Token: 0x06005B77 RID: 23415 RVA: 0x001DA858 File Offset: 0x001D8A58
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void CancelSubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.cancelSubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005B78 RID: 23416 RVA: 0x001DA866 File Offset: 0x001D8A66
		public static void CancelSubscription(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.cancelSubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getCancelSubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.CancelSubscription_64(new IAPurchaseCallback(IAPurchase.CancelSubscriptionIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.CancelSubscription(new IAPurchaseCallback(IAPurchase.CancelSubscriptionIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x04006C37 RID: 27703
		private static IAPurchaseCallback isReadyIl2cppCallback;

		// Token: 0x04006C38 RID: 27704
		private static IAPurchaseCallback request01Il2cppCallback;

		// Token: 0x04006C39 RID: 27705
		private static IAPurchaseCallback request02Il2cppCallback;

		// Token: 0x04006C3A RID: 27706
		private static IAPurchaseCallback purchaseIl2cppCallback;

		// Token: 0x04006C3B RID: 27707
		private static IAPurchaseCallback query01Il2cppCallback;

		// Token: 0x04006C3C RID: 27708
		private static IAPurchaseCallback query02Il2cppCallback;

		// Token: 0x04006C3D RID: 27709
		private static IAPurchaseCallback getBalanceIl2cppCallback;

		// Token: 0x04006C3E RID: 27710
		private static IAPurchaseCallback requestSubscriptionIl2cppCallback;

		// Token: 0x04006C3F RID: 27711
		private static IAPurchaseCallback requestSubscriptionWithPlanIDIl2cppCallback;

		// Token: 0x04006C40 RID: 27712
		private static IAPurchaseCallback subscribeIl2cppCallback;

		// Token: 0x04006C41 RID: 27713
		private static IAPurchaseCallback querySubscriptionIl2cppCallback;

		// Token: 0x04006C42 RID: 27714
		private static IAPurchaseCallback querySubscriptionListIl2cppCallback;

		// Token: 0x04006C43 RID: 27715
		private static IAPurchaseCallback cancelSubscriptionIl2cppCallback;

		// Token: 0x02000EBA RID: 3770
		private class IAPHandler : IAPurchase.BaseHandler
		{
			// Token: 0x06005B7A RID: 23418 RVA: 0x001DA8A5 File Offset: 0x001D8AA5
			public IAPHandler(IAPurchase.IAPurchaseListener cb)
			{
				IAPurchase.IAPHandler.listener = cb;
			}

			// Token: 0x06005B7B RID: 23419 RVA: 0x001DA8B3 File Offset: 0x001D8AB3
			public IAPurchaseCallback getIsReadyHandler()
			{
				return new IAPurchaseCallback(this.IsReadyHandler);
			}

			// Token: 0x06005B7C RID: 23420 RVA: 0x001DA8C4 File Offset: 0x001D8AC4
			protected override void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[IsReadyHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text3 = "[IsReadyHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text3 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[IsReadyHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["currencyName"];
						}
						catch (Exception ex3)
						{
							string text4 = "[IsReadyHandler] currencyName ex=";
							Exception ex4 = ex3;
							Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[IsReadyHandler] currencyName=" + text);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06005B7D RID: 23421 RVA: 0x001DA9F4 File Offset: 0x001D8BF4
			public IAPurchaseCallback getRequestHandler()
			{
				return new IAPurchaseCallback(this.RequestHandler);
			}

			// Token: 0x06005B7E RID: 23422 RVA: 0x001DAA04 File Offset: 0x001D8C04
			protected override void RequestHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[RequestHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text3 = "[RequestHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text3 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[RequestHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
						}
						catch (Exception ex3)
						{
							string text4 = "[RequestHandler] purchase_id ex=";
							Exception ex4 = ex3;
							Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[RequestHandler] purchaseId =" + text);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06005B7F RID: 23423 RVA: 0x001DAB34 File Offset: 0x001D8D34
			public IAPurchaseCallback getPurchaseHandler()
			{
				return new IAPurchaseCallback(this.PurchaseHandler);
			}

			// Token: 0x06005B80 RID: 23424 RVA: 0x001DAB44 File Offset: 0x001D8D44
			protected override void PurchaseHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[PurchaseHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				long num2 = 0L;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text3 = "[PurchaseHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text3 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[PurchaseHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
							num2 = (long)jsonData["paid_timestamp"];
						}
						catch (Exception ex3)
						{
							string text4 = "[PurchaseHandler] purchase_id,paid_timestamp ex=";
							Exception ex4 = ex3;
							Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[PurchaseHandler] purchaseId =" + text + ",paid_timestamp=" + num2.ToString());
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnPurchaseSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06005B81 RID: 23425 RVA: 0x001DAC94 File Offset: 0x001D8E94
			public IAPurchaseCallback getQueryHandler()
			{
				return new IAPurchaseCallback(this.QueryHandler);
			}

			// Token: 0x06005B82 RID: 23426 RVA: 0x001DACA4 File Offset: 0x001D8EA4
			protected override void QueryHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QueryHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				string text4 = "";
				string text5 = "";
				string text6 = "";
				long num2 = 0L;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text7 = "[QueryHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text7 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QueryHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
							text3 = (string)jsonData["order_id"];
							text4 = (string)jsonData["status"];
							text5 = (string)jsonData["price"];
							text6 = (string)jsonData["currency"];
							num2 = (long)jsonData["paid_timestamp"];
						}
						catch (Exception ex3)
						{
							string text8 = "[QueryHandler] purchase_id, order_id ex=";
							Exception ex4 = ex3;
							Logger.Log(text8 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log(string.Concat(new string[] { "[QueryHandler] status =", text4, ",price=", text5, ",currency=", text6 }));
						Logger.Log(string.Concat(new string[]
						{
							"[QueryHandler] purchaseId =",
							text,
							",order_id=",
							text3,
							",paid_timestamp=",
							num2.ToString()
						}));
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.QueryResponse queryResponse = new IAPurchase.QueryResponse();
							queryResponse.purchase_id = text;
							queryResponse.order_id = text3;
							queryResponse.price = text5;
							queryResponse.currency = text6;
							queryResponse.paid_timestamp = num2;
							queryResponse.status = text4;
							IAPurchase.IAPHandler.listener.OnQuerySuccess(queryResponse);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06005B83 RID: 23427 RVA: 0x001DAEF0 File Offset: 0x001D90F0
			public IAPurchaseCallback getQueryListHandler()
			{
				return new IAPurchaseCallback(this.QueryListHandler);
			}

			// Token: 0x06005B84 RID: 23428 RVA: 0x001DAF00 File Offset: 0x001D9100
			protected override void QueryListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QueryListHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				List<IAPurchase.QueryResponse2> list = new List<IAPurchase.QueryResponse2>();
				string text = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text2 = "[QueryListHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text2 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QueryListHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							JsonData jsonData2 = JsonMapper.ToObject(text);
							num2 = (int)jsonData2["total"];
							num3 = (int)jsonData2["from"];
							num4 = (int)jsonData2["to"];
							JsonData jsonData3 = jsonData2["purchases"];
							bool isArray = jsonData3.IsArray;
							foreach (object obj in ((IEnumerable)jsonData3))
							{
								JsonData jsonData4 = (JsonData)obj;
								IAPurchase.QueryResponse2 queryResponse = new IAPurchase.QueryResponse2();
								IDictionary dictionary = jsonData4;
								queryResponse.app_id = (dictionary.Contains("app_id") ? ((string)jsonData4["app_id"]) : "");
								queryResponse.currency = (dictionary.Contains("currency") ? ((string)jsonData4["currency"]) : "");
								queryResponse.purchase_id = (dictionary.Contains("purchase_id") ? ((string)jsonData4["purchase_id"]) : "");
								queryResponse.order_id = (dictionary.Contains("order_id") ? ((string)jsonData4["order_id"]) : "");
								queryResponse.price = (dictionary.Contains("price") ? ((string)jsonData4["price"]) : "");
								queryResponse.user_data = (dictionary.Contains("user_data") ? ((string)jsonData4["user_data"]) : "");
								if (dictionary.Contains("paid_timestamp"))
								{
									if (jsonData4["paid_timestamp"].IsLong)
									{
										queryResponse.paid_timestamp = (long)jsonData4["paid_timestamp"];
									}
									else if (jsonData4["paid_timestamp"].IsInt)
									{
										queryResponse.paid_timestamp = (long)(int)jsonData4["paid_timestamp"];
									}
								}
								list.Add(queryResponse);
							}
						}
						catch (Exception ex3)
						{
							string text3 = "[QueryListHandler] purchase_id, order_id ex=";
							Exception ex4 = ex3;
							Logger.Log(text3 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.QueryListResponse queryListResponse = new IAPurchase.QueryListResponse();
							queryListResponse.total = num2;
							queryListResponse.from = num3;
							queryListResponse.to = num4;
							queryListResponse.purchaseList = list;
							IAPurchase.IAPHandler.listener.OnQuerySuccess(queryListResponse);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06005B85 RID: 23429 RVA: 0x001DB288 File Offset: 0x001D9488
			public IAPurchaseCallback getBalanceHandler()
			{
				return new IAPurchaseCallback(this.BalanceHandler);
			}

			// Token: 0x06005B86 RID: 23430 RVA: 0x001DB298 File Offset: 0x001D9498
			protected override void BalanceHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[BalanceHandler] code=" + code.ToString() + ",message= " + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text3 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text4 = "[BalanceHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text4 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[BalanceHandler] statusCode =" + num.ToString() + ",errMessage=" + text3);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["currencyName"];
							text2 = (string)jsonData["balance"];
						}
						catch (Exception ex3)
						{
							string text5 = "[BalanceHandler] currencyName, balance ex=";
							Exception ex4 = ex3;
							Logger.Log(text5 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[BalanceHandler] currencyName=" + text + ",balance=" + text2);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnBalanceSuccess(text2);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text3);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06005B87 RID: 23431 RVA: 0x001DB3F4 File Offset: 0x001D95F4
			public IAPurchaseCallback getRequestSubscriptionHandler()
			{
				return new IAPurchaseCallback(this.RequestSubscriptionHandler);
			}

			// Token: 0x06005B88 RID: 23432 RVA: 0x001DB404 File Offset: 0x001D9604
			protected override void RequestSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[RequestSubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string text3 = "[RequestSubscriptionHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(text3 + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[RequestSubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
					}
					catch (Exception ex3)
					{
						string text4 = "[RequestSubscriptionHandler] subscription_id ex=";
						Exception ex4 = ex3;
						Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[RequestSubscriptionHandler] subscription_id =" + text);
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSubscriptionSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06005B89 RID: 23433 RVA: 0x001DB52C File Offset: 0x001D972C
			public IAPurchaseCallback getRequestSubscriptionWithPlanIDHandler()
			{
				return new IAPurchaseCallback(this.RequestSubscriptionWithPlanIDHandler);
			}

			// Token: 0x06005B8A RID: 23434 RVA: 0x001DB53C File Offset: 0x001D973C
			protected override void RequestSubscriptionWithPlanIDHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[RequestSubscriptionWithPlanIDHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string text3 = "[RequestSubscriptionWithPlanIDHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(text3 + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[RequestSubscriptionWithPlanIDHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
					}
					catch (Exception ex3)
					{
						string text4 = "[RequestSubscriptionWithPlanIDHandler] subscription_id ex=";
						Exception ex4 = ex3;
						Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[RequestSubscriptionWithPlanIDHandler] subscription_id =" + text);
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSubscriptionWithPlanIDSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06005B8B RID: 23435 RVA: 0x001DB664 File Offset: 0x001D9864
			public IAPurchaseCallback getSubscribeHandler()
			{
				return new IAPurchaseCallback(this.SubscribeHandler);
			}

			// Token: 0x06005B8C RID: 23436 RVA: 0x001DB674 File Offset: 0x001D9874
			protected override void SubscribeHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[SubscribeHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				long num2 = 0L;
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string text4 = "[SubscribeHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(text4 + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[SubscribeHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
						text3 = (string)jsonData["plan_id"];
						num2 = (long)jsonData["subscribed_timestamp"];
					}
					catch (Exception ex3)
					{
						string text5 = "[SubscribeHandler] subscription_id, plan_id ex=";
						Exception ex4 = ex3;
						Logger.Log(text5 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log(string.Concat(new string[]
					{
						"[SubscribeHandler] subscription_id =",
						text,
						", plan_id=",
						text3,
						", timestamp=",
						num2.ToString()
					}));
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnSubscribeSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06005B8D RID: 23437 RVA: 0x001DB7FC File Offset: 0x001D99FC
			public IAPurchaseCallback getQuerySubscriptionHandler()
			{
				return new IAPurchaseCallback(this.QuerySubscriptionHandler);
			}

			// Token: 0x06005B8E RID: 23438 RVA: 0x001DB80C File Offset: 0x001D9A0C
			protected override void QuerySubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QuerySubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				List<IAPurchase.Subscription> list = null;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text2 = "[QuerySubscriptionHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text2 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QuerySubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							list = JsonMapper.ToObject<IAPurchase.QuerySubscritionResponse>(message).subscriptions;
						}
						catch (Exception ex3)
						{
							string text3 = "[QuerySubscriptionHandler] ex =";
							Exception ex4 = ex3;
							Logger.Log(text3 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0 && list != null && list.Count > 0)
						{
							IAPurchase.IAPHandler.listener.OnQuerySubscriptionSuccess(list.ToArray());
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06005B8F RID: 23439 RVA: 0x001DB934 File Offset: 0x001D9B34
			public IAPurchaseCallback getQuerySubscriptionListHandler()
			{
				return new IAPurchaseCallback(this.QuerySubscriptionListHandler);
			}

			// Token: 0x06005B90 RID: 23440 RVA: 0x001DB944 File Offset: 0x001D9B44
			protected override void QuerySubscriptionListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QuerySubscriptionListHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				List<IAPurchase.Subscription> list = null;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text2 = "[QuerySubscriptionListHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text2 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QuerySubscriptionListHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							list = JsonMapper.ToObject<IAPurchase.QuerySubscritionResponse>(message).subscriptions;
						}
						catch (Exception ex3)
						{
							string text3 = "[QuerySubscriptionListHandler] ex =";
							Exception ex4 = ex3;
							Logger.Log(text3 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0 && list != null && list.Count > 0)
						{
							IAPurchase.IAPHandler.listener.OnQuerySubscriptionListSuccess(list.ToArray());
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06005B91 RID: 23441 RVA: 0x001DBA6C File Offset: 0x001D9C6C
			public IAPurchaseCallback getCancelSubscriptionHandler()
			{
				return new IAPurchaseCallback(this.CancelSubscriptionHandler);
			}

			// Token: 0x06005B92 RID: 23442 RVA: 0x001DBA7C File Offset: 0x001D9C7C
			protected override void CancelSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[CancelSubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				bool flag = false;
				string text = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text2 = "[CancelSubscriptionHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text2 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[CancelSubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						flag = true;
						Logger.Log("[CancelSubscriptionHandler] isCanceled = " + flag.ToString());
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnCancelSubscriptionSuccess(flag);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x04006C44 RID: 27716
			private static IAPurchase.IAPurchaseListener listener;
		}

		// Token: 0x02000EBB RID: 3771
		private abstract class BaseHandler
		{
			// Token: 0x06005B93 RID: 23443
			protected abstract void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06005B94 RID: 23444
			protected abstract void RequestHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06005B95 RID: 23445
			protected abstract void PurchaseHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06005B96 RID: 23446
			protected abstract void QueryHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06005B97 RID: 23447
			protected abstract void QueryListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06005B98 RID: 23448
			protected abstract void BalanceHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06005B99 RID: 23449
			protected abstract void RequestSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06005B9A RID: 23450
			protected abstract void RequestSubscriptionWithPlanIDHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06005B9B RID: 23451
			protected abstract void SubscribeHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06005B9C RID: 23452
			protected abstract void QuerySubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06005B9D RID: 23453
			protected abstract void QuerySubscriptionListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06005B9E RID: 23454
			protected abstract void CancelSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);
		}

		// Token: 0x02000EBC RID: 3772
		public class IAPurchaseListener
		{
			// Token: 0x06005BA0 RID: 23456 RVA: 0x00002C2D File Offset: 0x00000E2D
			public virtual void OnSuccess(string pchCurrencyName)
			{
			}

			// Token: 0x06005BA1 RID: 23457 RVA: 0x00002C2D File Offset: 0x00000E2D
			public virtual void OnRequestSuccess(string pchPurchaseId)
			{
			}

			// Token: 0x06005BA2 RID: 23458 RVA: 0x00002C2D File Offset: 0x00000E2D
			public virtual void OnPurchaseSuccess(string pchPurchaseId)
			{
			}

			// Token: 0x06005BA3 RID: 23459 RVA: 0x00002C2D File Offset: 0x00000E2D
			public virtual void OnQuerySuccess(IAPurchase.QueryResponse response)
			{
			}

			// Token: 0x06005BA4 RID: 23460 RVA: 0x00002C2D File Offset: 0x00000E2D
			public virtual void OnQuerySuccess(IAPurchase.QueryListResponse response)
			{
			}

			// Token: 0x06005BA5 RID: 23461 RVA: 0x00002C2D File Offset: 0x00000E2D
			public virtual void OnBalanceSuccess(string pchBalance)
			{
			}

			// Token: 0x06005BA6 RID: 23462 RVA: 0x00002C2D File Offset: 0x00000E2D
			public virtual void OnFailure(int nCode, string pchMessage)
			{
			}

			// Token: 0x06005BA7 RID: 23463 RVA: 0x00002C2D File Offset: 0x00000E2D
			public virtual void OnRequestSubscriptionSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x06005BA8 RID: 23464 RVA: 0x00002C2D File Offset: 0x00000E2D
			public virtual void OnRequestSubscriptionWithPlanIDSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x06005BA9 RID: 23465 RVA: 0x00002C2D File Offset: 0x00000E2D
			public virtual void OnSubscribeSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x06005BAA RID: 23466 RVA: 0x00002C2D File Offset: 0x00000E2D
			public virtual void OnQuerySubscriptionSuccess(IAPurchase.Subscription[] subscriptionlist)
			{
			}

			// Token: 0x06005BAB RID: 23467 RVA: 0x00002C2D File Offset: 0x00000E2D
			public virtual void OnQuerySubscriptionListSuccess(IAPurchase.Subscription[] subscriptionlist)
			{
			}

			// Token: 0x06005BAC RID: 23468 RVA: 0x00002C2D File Offset: 0x00000E2D
			public virtual void OnCancelSubscriptionSuccess(bool bCanceled)
			{
			}
		}

		// Token: 0x02000EBD RID: 3773
		public class QueryResponse
		{
			// Token: 0x170008BD RID: 2237
			// (get) Token: 0x06005BAE RID: 23470 RVA: 0x001DBB6C File Offset: 0x001D9D6C
			// (set) Token: 0x06005BAF RID: 23471 RVA: 0x001DBB74 File Offset: 0x001D9D74
			public string order_id { get; set; }

			// Token: 0x170008BE RID: 2238
			// (get) Token: 0x06005BB0 RID: 23472 RVA: 0x001DBB7D File Offset: 0x001D9D7D
			// (set) Token: 0x06005BB1 RID: 23473 RVA: 0x001DBB85 File Offset: 0x001D9D85
			public string purchase_id { get; set; }

			// Token: 0x170008BF RID: 2239
			// (get) Token: 0x06005BB2 RID: 23474 RVA: 0x001DBB8E File Offset: 0x001D9D8E
			// (set) Token: 0x06005BB3 RID: 23475 RVA: 0x001DBB96 File Offset: 0x001D9D96
			public string status { get; set; }

			// Token: 0x170008C0 RID: 2240
			// (get) Token: 0x06005BB4 RID: 23476 RVA: 0x001DBB9F File Offset: 0x001D9D9F
			// (set) Token: 0x06005BB5 RID: 23477 RVA: 0x001DBBA7 File Offset: 0x001D9DA7
			public string price { get; set; }

			// Token: 0x170008C1 RID: 2241
			// (get) Token: 0x06005BB6 RID: 23478 RVA: 0x001DBBB0 File Offset: 0x001D9DB0
			// (set) Token: 0x06005BB7 RID: 23479 RVA: 0x001DBBB8 File Offset: 0x001D9DB8
			public string currency { get; set; }

			// Token: 0x170008C2 RID: 2242
			// (get) Token: 0x06005BB8 RID: 23480 RVA: 0x001DBBC1 File Offset: 0x001D9DC1
			// (set) Token: 0x06005BB9 RID: 23481 RVA: 0x001DBBC9 File Offset: 0x001D9DC9
			public long paid_timestamp { get; set; }
		}

		// Token: 0x02000EBE RID: 3774
		public class QueryResponse2
		{
			// Token: 0x170008C3 RID: 2243
			// (get) Token: 0x06005BBB RID: 23483 RVA: 0x001DBBD2 File Offset: 0x001D9DD2
			// (set) Token: 0x06005BBC RID: 23484 RVA: 0x001DBBDA File Offset: 0x001D9DDA
			public string order_id { get; set; }

			// Token: 0x170008C4 RID: 2244
			// (get) Token: 0x06005BBD RID: 23485 RVA: 0x001DBBE3 File Offset: 0x001D9DE3
			// (set) Token: 0x06005BBE RID: 23486 RVA: 0x001DBBEB File Offset: 0x001D9DEB
			public string app_id { get; set; }

			// Token: 0x170008C5 RID: 2245
			// (get) Token: 0x06005BBF RID: 23487 RVA: 0x001DBBF4 File Offset: 0x001D9DF4
			// (set) Token: 0x06005BC0 RID: 23488 RVA: 0x001DBBFC File Offset: 0x001D9DFC
			public string purchase_id { get; set; }

			// Token: 0x170008C6 RID: 2246
			// (get) Token: 0x06005BC1 RID: 23489 RVA: 0x001DBC05 File Offset: 0x001D9E05
			// (set) Token: 0x06005BC2 RID: 23490 RVA: 0x001DBC0D File Offset: 0x001D9E0D
			public string user_data { get; set; }

			// Token: 0x170008C7 RID: 2247
			// (get) Token: 0x06005BC3 RID: 23491 RVA: 0x001DBC16 File Offset: 0x001D9E16
			// (set) Token: 0x06005BC4 RID: 23492 RVA: 0x001DBC1E File Offset: 0x001D9E1E
			public string price { get; set; }

			// Token: 0x170008C8 RID: 2248
			// (get) Token: 0x06005BC5 RID: 23493 RVA: 0x001DBC27 File Offset: 0x001D9E27
			// (set) Token: 0x06005BC6 RID: 23494 RVA: 0x001DBC2F File Offset: 0x001D9E2F
			public string currency { get; set; }

			// Token: 0x170008C9 RID: 2249
			// (get) Token: 0x06005BC7 RID: 23495 RVA: 0x001DBC38 File Offset: 0x001D9E38
			// (set) Token: 0x06005BC8 RID: 23496 RVA: 0x001DBC40 File Offset: 0x001D9E40
			public long paid_timestamp { get; set; }
		}

		// Token: 0x02000EBF RID: 3775
		public class QueryListResponse
		{
			// Token: 0x170008CA RID: 2250
			// (get) Token: 0x06005BCA RID: 23498 RVA: 0x001DBC49 File Offset: 0x001D9E49
			// (set) Token: 0x06005BCB RID: 23499 RVA: 0x001DBC51 File Offset: 0x001D9E51
			public int total { get; set; }

			// Token: 0x170008CB RID: 2251
			// (get) Token: 0x06005BCC RID: 23500 RVA: 0x001DBC5A File Offset: 0x001D9E5A
			// (set) Token: 0x06005BCD RID: 23501 RVA: 0x001DBC62 File Offset: 0x001D9E62
			public int from { get; set; }

			// Token: 0x170008CC RID: 2252
			// (get) Token: 0x06005BCE RID: 23502 RVA: 0x001DBC6B File Offset: 0x001D9E6B
			// (set) Token: 0x06005BCF RID: 23503 RVA: 0x001DBC73 File Offset: 0x001D9E73
			public int to { get; set; }

			// Token: 0x04006C55 RID: 27733
			public List<IAPurchase.QueryResponse2> purchaseList;
		}

		// Token: 0x02000EC0 RID: 3776
		public class StatusDetailTransaction
		{
			// Token: 0x170008CD RID: 2253
			// (get) Token: 0x06005BD1 RID: 23505 RVA: 0x001DBC7C File Offset: 0x001D9E7C
			// (set) Token: 0x06005BD2 RID: 23506 RVA: 0x001DBC84 File Offset: 0x001D9E84
			public long create_time { get; set; }

			// Token: 0x170008CE RID: 2254
			// (get) Token: 0x06005BD3 RID: 23507 RVA: 0x001DBC8D File Offset: 0x001D9E8D
			// (set) Token: 0x06005BD4 RID: 23508 RVA: 0x001DBC95 File Offset: 0x001D9E95
			public string payment_method { get; set; }

			// Token: 0x170008CF RID: 2255
			// (get) Token: 0x06005BD5 RID: 23509 RVA: 0x001DBC9E File Offset: 0x001D9E9E
			// (set) Token: 0x06005BD6 RID: 23510 RVA: 0x001DBCA6 File Offset: 0x001D9EA6
			public string status { get; set; }
		}

		// Token: 0x02000EC1 RID: 3777
		public class StatusDetail
		{
			// Token: 0x170008D0 RID: 2256
			// (get) Token: 0x06005BD8 RID: 23512 RVA: 0x001DBCAF File Offset: 0x001D9EAF
			// (set) Token: 0x06005BD9 RID: 23513 RVA: 0x001DBCB7 File Offset: 0x001D9EB7
			public long date_next_charge { get; set; }

			// Token: 0x170008D1 RID: 2257
			// (get) Token: 0x06005BDA RID: 23514 RVA: 0x001DBCC0 File Offset: 0x001D9EC0
			// (set) Token: 0x06005BDB RID: 23515 RVA: 0x001DBCC8 File Offset: 0x001D9EC8
			public IAPurchase.StatusDetailTransaction[] transactions { get; set; }

			// Token: 0x170008D2 RID: 2258
			// (get) Token: 0x06005BDC RID: 23516 RVA: 0x001DBCD1 File Offset: 0x001D9ED1
			// (set) Token: 0x06005BDD RID: 23517 RVA: 0x001DBCD9 File Offset: 0x001D9ED9
			public string cancel_reason { get; set; }
		}

		// Token: 0x02000EC2 RID: 3778
		public class TimePeriod
		{
			// Token: 0x170008D3 RID: 2259
			// (get) Token: 0x06005BDF RID: 23519 RVA: 0x001DBCE2 File Offset: 0x001D9EE2
			// (set) Token: 0x06005BE0 RID: 23520 RVA: 0x001DBCEA File Offset: 0x001D9EEA
			public string time_type { get; set; }

			// Token: 0x170008D4 RID: 2260
			// (get) Token: 0x06005BE1 RID: 23521 RVA: 0x001DBCF3 File Offset: 0x001D9EF3
			// (set) Token: 0x06005BE2 RID: 23522 RVA: 0x001DBCFB File Offset: 0x001D9EFB
			public int value { get; set; }
		}

		// Token: 0x02000EC3 RID: 3779
		public class Subscription
		{
			// Token: 0x170008D5 RID: 2261
			// (get) Token: 0x06005BE4 RID: 23524 RVA: 0x001DBD04 File Offset: 0x001D9F04
			// (set) Token: 0x06005BE5 RID: 23525 RVA: 0x001DBD0C File Offset: 0x001D9F0C
			public string app_id { get; set; }

			// Token: 0x170008D6 RID: 2262
			// (get) Token: 0x06005BE6 RID: 23526 RVA: 0x001DBD15 File Offset: 0x001D9F15
			// (set) Token: 0x06005BE7 RID: 23527 RVA: 0x001DBD1D File Offset: 0x001D9F1D
			public string order_id { get; set; }

			// Token: 0x170008D7 RID: 2263
			// (get) Token: 0x06005BE8 RID: 23528 RVA: 0x001DBD26 File Offset: 0x001D9F26
			// (set) Token: 0x06005BE9 RID: 23529 RVA: 0x001DBD2E File Offset: 0x001D9F2E
			public string subscription_id { get; set; }

			// Token: 0x170008D8 RID: 2264
			// (get) Token: 0x06005BEA RID: 23530 RVA: 0x001DBD37 File Offset: 0x001D9F37
			// (set) Token: 0x06005BEB RID: 23531 RVA: 0x001DBD3F File Offset: 0x001D9F3F
			public string price { get; set; }

			// Token: 0x170008D9 RID: 2265
			// (get) Token: 0x06005BEC RID: 23532 RVA: 0x001DBD48 File Offset: 0x001D9F48
			// (set) Token: 0x06005BED RID: 23533 RVA: 0x001DBD50 File Offset: 0x001D9F50
			public string currency { get; set; }

			// Token: 0x170008DA RID: 2266
			// (get) Token: 0x06005BEE RID: 23534 RVA: 0x001DBD59 File Offset: 0x001D9F59
			// (set) Token: 0x06005BEF RID: 23535 RVA: 0x001DBD61 File Offset: 0x001D9F61
			public long subscribed_timestamp { get; set; }

			// Token: 0x170008DB RID: 2267
			// (get) Token: 0x06005BF0 RID: 23536 RVA: 0x001DBD6A File Offset: 0x001D9F6A
			// (set) Token: 0x06005BF1 RID: 23537 RVA: 0x001DBD72 File Offset: 0x001D9F72
			public IAPurchase.TimePeriod free_trial_period { get; set; }

			// Token: 0x170008DC RID: 2268
			// (get) Token: 0x06005BF2 RID: 23538 RVA: 0x001DBD7B File Offset: 0x001D9F7B
			// (set) Token: 0x06005BF3 RID: 23539 RVA: 0x001DBD83 File Offset: 0x001D9F83
			public IAPurchase.TimePeriod charge_period { get; set; }

			// Token: 0x170008DD RID: 2269
			// (get) Token: 0x06005BF4 RID: 23540 RVA: 0x001DBD8C File Offset: 0x001D9F8C
			// (set) Token: 0x06005BF5 RID: 23541 RVA: 0x001DBD94 File Offset: 0x001D9F94
			public int number_of_charge_period { get; set; }

			// Token: 0x170008DE RID: 2270
			// (get) Token: 0x06005BF6 RID: 23542 RVA: 0x001DBD9D File Offset: 0x001D9F9D
			// (set) Token: 0x06005BF7 RID: 23543 RVA: 0x001DBDA5 File Offset: 0x001D9FA5
			public string plan_id { get; set; }

			// Token: 0x170008DF RID: 2271
			// (get) Token: 0x06005BF8 RID: 23544 RVA: 0x001DBDAE File Offset: 0x001D9FAE
			// (set) Token: 0x06005BF9 RID: 23545 RVA: 0x001DBDB6 File Offset: 0x001D9FB6
			public string plan_name { get; set; }

			// Token: 0x170008E0 RID: 2272
			// (get) Token: 0x06005BFA RID: 23546 RVA: 0x001DBDBF File Offset: 0x001D9FBF
			// (set) Token: 0x06005BFB RID: 23547 RVA: 0x001DBDC7 File Offset: 0x001D9FC7
			public string status { get; set; }

			// Token: 0x170008E1 RID: 2273
			// (get) Token: 0x06005BFC RID: 23548 RVA: 0x001DBDD0 File Offset: 0x001D9FD0
			// (set) Token: 0x06005BFD RID: 23549 RVA: 0x001DBDD8 File Offset: 0x001D9FD8
			public IAPurchase.StatusDetail status_detail { get; set; }
		}

		// Token: 0x02000EC4 RID: 3780
		public class QuerySubscritionResponse
		{
			// Token: 0x170008E2 RID: 2274
			// (get) Token: 0x06005BFF RID: 23551 RVA: 0x001DBDE1 File Offset: 0x001D9FE1
			// (set) Token: 0x06005C00 RID: 23552 RVA: 0x001DBDE9 File Offset: 0x001D9FE9
			public int statusCode { get; set; }

			// Token: 0x170008E3 RID: 2275
			// (get) Token: 0x06005C01 RID: 23553 RVA: 0x001DBDF2 File Offset: 0x001D9FF2
			// (set) Token: 0x06005C02 RID: 23554 RVA: 0x001DBDFA File Offset: 0x001D9FFA
			public string message { get; set; }

			// Token: 0x170008E4 RID: 2276
			// (get) Token: 0x06005C03 RID: 23555 RVA: 0x001DBE03 File Offset: 0x001DA003
			// (set) Token: 0x06005C04 RID: 23556 RVA: 0x001DBE0B File Offset: 0x001DA00B
			public List<IAPurchase.Subscription> subscriptions { get; set; }
		}
	}
}
