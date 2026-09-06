using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;

namespace GorillaTag
{
	// Token: 0x0200120D RID: 4621
	public static class MonkeAgentCleanup
	{
		// Token: 0x06007514 RID: 29972 RVA: 0x002603C8 File Offset: 0x0025E5C8
		static MonkeAgentCleanup()
		{
			MonkeAgentCleanup.k_destroyTimer.callback = new Action(MonkeAgentCleanup.CheckDestroyQueue);
			RoomSystem.LeftRoomEvent += new Action(MonkeAgentCleanup.OnLeftRoom);
		}

		// Token: 0x06007515 RID: 29973 RVA: 0x00260450 File Offset: 0x0025E650
		public static void RegisterForDestroy(PhotonView target)
		{
			if (MonkeAgentCleanup.k_destroyTargets.Contains(target))
			{
				return;
			}
			if (target.gameObject.activeSelf)
			{
				target.gameObject.Disable();
			}
			if (MonkeAgentCleanup.k_destroyTargets.Add(target))
			{
				MonkeAgentCleanup.k_destroyQueue.Enqueue(target);
			}
			if (!MonkeAgentCleanup.k_destroyTimer.Running && MonkeAgentCleanup.k_destroyQueue.Count > 0)
			{
				MonkeAgentCleanup.k_destroyTimer.Start();
			}
		}

		// Token: 0x06007516 RID: 29974 RVA: 0x002604BE File Offset: 0x0025E6BE
		private static void OnLeftRoom()
		{
			MonkeAgentCleanup.k_destroyQueue.Clear();
			MonkeAgentCleanup.k_destroyTargets.Clear();
			MonkeAgentCleanup.k_destroyTimer.Stop();
		}

		// Token: 0x06007517 RID: 29975 RVA: 0x002604E0 File Offset: 0x0025E6E0
		private static void CheckDestroyQueue()
		{
			if (!RoomSystem.JoinedRoom)
			{
				return;
			}
			bool flag = RoomSystem.GetLowestActorNumberPlayer() == NetworkSystem.Instance.LocalPlayer;
			int num = 0;
			while (MonkeAgentCleanup.k_destroyQueue.Count > 0 && num < 10)
			{
				PhotonView photonView = MonkeAgentCleanup.k_destroyQueue.Dequeue();
				if (MonkeAgentCleanup.k_destroyTargets.Remove(photonView) && !photonView.IsNull())
				{
					if ((photonView.IsRoomView && flag) || photonView.IsMine)
					{
						MonkeAgentCleanup.k_cacheInfo[MonkeAgentCleanup.k_viewIdKey] = photonView.InstantiationId;
						PhotonNetwork.NetworkingClient.OpRaiseEvent(202, MonkeAgentCleanup.k_cacheInfo, MonkeAgentCleanup.k_raiseEventOptions, SendOptions.SendReliable);
					}
					PhotonNetwork.RemoveInstantiatedGO(photonView.gameObject, true);
					num++;
				}
			}
			if (MonkeAgentCleanup.k_destroyTargets.Count == 0)
			{
				MonkeAgentCleanup.k_destroyTimer.Stop();
			}
		}

		// Token: 0x040084C8 RID: 33992
		private static readonly Queue<PhotonView> k_destroyQueue = new Queue<PhotonView>();

		// Token: 0x040084C9 RID: 33993
		private static readonly HashSet<PhotonView> k_destroyTargets = new HashSet<PhotonView>();

		// Token: 0x040084CA RID: 33994
		private static readonly TickSystemTimer k_destroyTimer = new TickSystemTimer(1f);

		// Token: 0x040084CB RID: 33995
		private static readonly Hashtable k_cacheInfo = new Hashtable(1);

		// Token: 0x040084CC RID: 33996
		private static readonly RaiseEventOptions k_raiseEventOptions = new RaiseEventOptions
		{
			CachingOption = EventCaching.RemoveFromRoomCache
		};

		// Token: 0x040084CD RID: 33997
		private static readonly object k_viewIdKey = 7;
	}
}
