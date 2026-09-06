using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020008F2 RID: 2290
public class GTSignalRelay : MonoBehaviourStatic<GTSignalRelay>, IOnEventCallback
{
	// Token: 0x17000572 RID: 1394
	// (get) Token: 0x06003C06 RID: 15366 RVA: 0x00147AAE File Offset: 0x00145CAE
	public static IReadOnlyList<GTSignalListener> ActiveListeners
	{
		get
		{
			return GTSignalRelay.gActiveListeners;
		}
	}

	// Token: 0x06003C07 RID: 15367 RVA: 0x00147AB5 File Offset: 0x00145CB5
	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			PhotonNetwork.AddCallbackTarget(this);
		}
	}

	// Token: 0x06003C08 RID: 15368 RVA: 0x00147AC4 File Offset: 0x00145CC4
	private void OnDisable()
	{
		if (Application.isPlaying)
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}
	}

	// Token: 0x06003C09 RID: 15369 RVA: 0x00147AD4 File Offset: 0x00145CD4
	public static void Register(GTSignalListener listener)
	{
		if (listener == null)
		{
			return;
		}
		int num = listener.signal;
		if (num == 0)
		{
			return;
		}
		if (!GTSignalRelay.gListenerSet.Add(listener))
		{
			return;
		}
		GTSignalRelay.gActiveListeners.Add(listener);
		List<GTSignalListener> list;
		if (!GTSignalRelay.gSignalIdToListeners.TryGetValue(num, out list))
		{
			list = new List<GTSignalListener>(64);
			GTSignalRelay.gSignalIdToListeners.Add(num, list);
		}
		list.Add(listener);
	}

	// Token: 0x06003C0A RID: 15370 RVA: 0x00147B40 File Offset: 0x00145D40
	public static void Unregister(GTSignalListener listener)
	{
		if (listener == null)
		{
			return;
		}
		GTSignalRelay.gListenerSet.Remove(listener);
		GTSignalRelay.gActiveListeners.Remove(listener);
		List<GTSignalListener> list;
		if (GTSignalRelay.gSignalIdToListeners.TryGetValue(listener.signal, out list))
		{
			list.Remove(listener);
		}
	}

	// Token: 0x06003C0B RID: 15371 RVA: 0x00147B90 File Offset: 0x00145D90
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeOnLoad()
	{
		Object.DontDestroyOnLoad(new GameObject("GTSignalRelay").AddComponent<GTSignalRelay>());
	}

	// Token: 0x06003C0C RID: 15372 RVA: 0x00147BA8 File Offset: 0x00145DA8
	void IOnEventCallback.OnEvent(EventData eventData)
	{
		if (eventData.Code == 186)
		{
			object[] array = eventData.CustomData as object[];
			if (array != null)
			{
				int num = (int)array[0];
				List<GTSignalListener> list;
				if (!GTSignalRelay.gSignalIdToListeners.TryGetValue(num, out list))
				{
					return;
				}
				int sender = eventData.Sender;
				for (int i = 0; i < list.Count; i++)
				{
					try
					{
						GTSignalListener gtsignalListener = list[i];
						if (!gtsignalListener.deafen)
						{
							if (gtsignalListener.IsReady())
							{
								if (!gtsignalListener.ignoreSelf || sender != gtsignalListener.rigActorID)
								{
									if (!gtsignalListener.listenToSelfOnly || sender == gtsignalListener.rigActorID)
									{
										gtsignalListener.HandleSignalReceived(sender, array);
										if (gtsignalListener.callUnityEvent)
										{
											UnityEvent onSignalReceived = gtsignalListener.onSignalReceived;
											if (onSignalReceived != null)
											{
												onSignalReceived.Invoke();
											}
										}
									}
								}
							}
						}
					}
					catch (Exception)
					{
					}
				}
				return;
			}
		}
	}

	// Token: 0x04004C7F RID: 19583
	private static List<GTSignalListener> gActiveListeners = new List<GTSignalListener>(128);

	// Token: 0x04004C80 RID: 19584
	private static HashSet<GTSignalListener> gListenerSet = new HashSet<GTSignalListener>(128);

	// Token: 0x04004C81 RID: 19585
	private static Dictionary<int, List<GTSignalListener>> gSignalIdToListeners = new Dictionary<int, List<GTSignalListener>>(128);
}
