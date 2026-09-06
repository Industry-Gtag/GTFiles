using System;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000EC6 RID: 3782
	public class Subscription
	{
		// Token: 0x06005C0C RID: 23564 RVA: 0x001DBEF9 File Offset: 0x001DA0F9
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void IsReadyIl2cppCallback(int errorCode, string message)
		{
			Viveport.Subscription.isReadyIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005C0D RID: 23565 RVA: 0x001DBF08 File Offset: 0x001DA108
		public static void IsReady(StatusCallback2 callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Viveport.Subscription.isReadyIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Viveport.Subscription.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Viveport.Internal.Subscription.IsReady_64(new StatusCallback2(Viveport.Subscription.IsReadyIl2cppCallback));
				return;
			}
			Viveport.Internal.Subscription.IsReady(new StatusCallback2(Viveport.Subscription.IsReadyIl2cppCallback));
		}

		// Token: 0x06005C0E RID: 23566 RVA: 0x001DBF78 File Offset: 0x001DA178
		public static SubscriptionStatus GetUserStatus()
		{
			SubscriptionStatus subscriptionStatus = new SubscriptionStatus();
			if (IntPtr.Size == 8)
			{
				if (Viveport.Internal.Subscription.IsWindowsSubscriber_64())
				{
					subscriptionStatus.Platforms.Add(SubscriptionStatus.Platform.Windows);
				}
				if (Viveport.Internal.Subscription.IsAndroidSubscriber_64())
				{
					subscriptionStatus.Platforms.Add(SubscriptionStatus.Platform.Android);
				}
				switch (Viveport.Internal.Subscription.GetTransactionType_64())
				{
				case ESubscriptionTransactionType.UNKNOWN:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.Unknown;
					break;
				case ESubscriptionTransactionType.PAID:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.Paid;
					break;
				case ESubscriptionTransactionType.REDEEM:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.Redeem;
					break;
				case ESubscriptionTransactionType.FREEE_TRIAL:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.FreeTrial;
					break;
				default:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.Unknown;
					break;
				}
			}
			else
			{
				if (Viveport.Internal.Subscription.IsWindowsSubscriber())
				{
					subscriptionStatus.Platforms.Add(SubscriptionStatus.Platform.Windows);
				}
				if (Viveport.Internal.Subscription.IsAndroidSubscriber())
				{
					subscriptionStatus.Platforms.Add(SubscriptionStatus.Platform.Android);
				}
				switch (Viveport.Internal.Subscription.GetTransactionType())
				{
				case ESubscriptionTransactionType.UNKNOWN:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.Unknown;
					break;
				case ESubscriptionTransactionType.PAID:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.Paid;
					break;
				case ESubscriptionTransactionType.REDEEM:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.Redeem;
					break;
				case ESubscriptionTransactionType.FREEE_TRIAL:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.FreeTrial;
					break;
				default:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.Unknown;
					break;
				}
			}
			return subscriptionStatus;
		}

		// Token: 0x04006C70 RID: 27760
		private static StatusCallback2 isReadyIl2cppCallback;
	}
}
