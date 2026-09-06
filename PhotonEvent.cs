using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000CF1 RID: 3313
[Serializable]
public class PhotonEvent : IEquatable<PhotonEvent>
{
	// Token: 0x170007C5 RID: 1989
	// (get) Token: 0x060051FC RID: 20988 RVA: 0x001B3AF5 File Offset: 0x001B1CF5
	// (set) Token: 0x060051FD RID: 20989 RVA: 0x001B3AFD File Offset: 0x001B1CFD
	public bool reliable
	{
		get
		{
			return this._reliable;
		}
		set
		{
			this._reliable = value;
		}
	}

	// Token: 0x170007C6 RID: 1990
	// (get) Token: 0x060051FE RID: 20990 RVA: 0x001B3B06 File Offset: 0x001B1D06
	// (set) Token: 0x060051FF RID: 20991 RVA: 0x001B3B0E File Offset: 0x001B1D0E
	public bool failSilent
	{
		get
		{
			return this._failSilent;
		}
		set
		{
			this._failSilent = value;
		}
	}

	// Token: 0x06005200 RID: 20992 RVA: 0x001B3B17 File Offset: 0x001B1D17
	private PhotonEvent()
	{
	}

	// Token: 0x06005201 RID: 20993 RVA: 0x001B3B26 File Offset: 0x001B1D26
	public PhotonEvent(int eventId)
	{
		if (eventId == -1)
		{
			throw new Exception(string.Format("<{0}> cannot be {1}.", "eventId", -1));
		}
		this._eventId = eventId;
		this.Enable();
	}

	// Token: 0x06005202 RID: 20994 RVA: 0x001B3B61 File Offset: 0x001B1D61
	public PhotonEvent(string eventId)
		: this(StaticHash.Compute(eventId))
	{
	}

	// Token: 0x06005203 RID: 20995 RVA: 0x001B3B6F File Offset: 0x001B1D6F
	public PhotonEvent(int eventId, Action<int, int, object[], PhotonMessageInfoWrapped> callback)
		: this(eventId)
	{
		this.AddCallback(callback);
	}

	// Token: 0x06005204 RID: 20996 RVA: 0x001B3B7F File Offset: 0x001B1D7F
	public PhotonEvent(string eventId, Action<int, int, object[], PhotonMessageInfoWrapped> callback)
		: this(eventId)
	{
		this.AddCallback(callback);
	}

	// Token: 0x06005205 RID: 20997 RVA: 0x001B3B90 File Offset: 0x001B1D90
	~PhotonEvent()
	{
		this.Dispose();
	}

	// Token: 0x06005206 RID: 20998 RVA: 0x001B3BBC File Offset: 0x001B1DBC
	public void AddCallback(Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (this._disposed)
		{
			return;
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		if (this._delegate != null)
		{
			foreach (Delegate @delegate in this._delegate.GetInvocationList())
			{
				if (@delegate != null && @delegate.Equals(callback))
				{
					return;
				}
			}
		}
		this._delegate = (Action<int, int, object[], PhotonMessageInfoWrapped>)Delegate.Combine(this._delegate, callback);
	}

	// Token: 0x06005207 RID: 20999 RVA: 0x001B3C2A File Offset: 0x001B1E2A
	public void RemoveCallback(Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (this._disposed)
		{
			return;
		}
		if (callback != null)
		{
			this._delegate = (Action<int, int, object[], PhotonMessageInfoWrapped>)Delegate.Remove(this._delegate, callback);
		}
	}

	// Token: 0x06005208 RID: 21000 RVA: 0x001B3C4F File Offset: 0x001B1E4F
	public void Enable()
	{
		if (this._disposed)
		{
			return;
		}
		if (this._enabled)
		{
			return;
		}
		if (Application.isPlaying)
		{
			PhotonEvent.AddPhotonEvent(this);
		}
		this._enabled = true;
	}

	// Token: 0x06005209 RID: 21001 RVA: 0x001B3C77 File Offset: 0x001B1E77
	public void Disable()
	{
		if (this._disposed)
		{
			return;
		}
		if (!this._enabled)
		{
			return;
		}
		if (Application.isPlaying)
		{
			PhotonEvent.RemovePhotonEvent(this);
		}
		this._enabled = false;
	}

	// Token: 0x0600520A RID: 21002 RVA: 0x001B3C9F File Offset: 0x001B1E9F
	public void Dispose()
	{
		this._delegate = null;
		if (this._enabled)
		{
			this._enabled = false;
			if (Application.isPlaying)
			{
				PhotonEvent.RemovePhotonEvent(this);
			}
		}
		this._eventId = -1;
		this._disposed = true;
	}

	// Token: 0x14000090 RID: 144
	// (add) Token: 0x0600520B RID: 21003 RVA: 0x001B3CD4 File Offset: 0x001B1ED4
	// (remove) Token: 0x0600520C RID: 21004 RVA: 0x001B3D08 File Offset: 0x001B1F08
	public static event Action<EventData, Exception> OnError;

	// Token: 0x0600520D RID: 21005 RVA: 0x001B3D3B File Offset: 0x001B1F3B
	private void InvokeDelegate(int sender, object[] args, PhotonMessageInfoWrapped info)
	{
		Action<int, int, object[], PhotonMessageInfoWrapped> @delegate = this._delegate;
		if (@delegate == null)
		{
			return;
		}
		@delegate(sender, this._eventId, args, info);
	}

	// Token: 0x0600520E RID: 21006 RVA: 0x001B3D56 File Offset: 0x001B1F56
	public void RaiseLocal(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.Local, args);
	}

	// Token: 0x0600520F RID: 21007 RVA: 0x001B3D60 File Offset: 0x001B1F60
	public void RaiseOthers(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.RemoteOthers, args);
	}

	// Token: 0x06005210 RID: 21008 RVA: 0x001B3D6A File Offset: 0x001B1F6A
	public void RaiseAll(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.RemoteAll, args);
	}

	// Token: 0x06005211 RID: 21009 RVA: 0x001B3D74 File Offset: 0x001B1F74
	private void Raise(PhotonEvent.RaiseMode mode, params object[] args)
	{
		if (this._disposed)
		{
			return;
		}
		if (!Application.isPlaying)
		{
			return;
		}
		if (!this._enabled)
		{
			return;
		}
		if (args != null && args.Length > 20)
		{
			Debug.LogError(string.Format("{0}: too many event args, max is {1}, trying to send {2}. Stopping!", "PhotonEvent", 20, args.Length));
			return;
		}
		SendOptions sendOptions = (this._reliable ? PhotonEvent.gSendReliable : PhotonEvent.gSendUnreliable);
		switch (mode)
		{
		case PhotonEvent.RaiseMode.Local:
			this.InvokeDelegate(this._eventId, args, new PhotonMessageInfoWrapped(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.ServerTimestamp));
			return;
		case PhotonEvent.RaiseMode.RemoteOthers:
		{
			object[] array = args.Prepend(this._eventId).ToArray<object>();
			PhotonNetwork.RaiseEvent(176, array, PhotonEvent.gReceiversOthers, sendOptions);
			return;
		}
		case PhotonEvent.RaiseMode.RemoteAll:
		{
			object[] array2 = args.Prepend(this._eventId).ToArray<object>();
			PhotonNetwork.RaiseEvent(176, array2, PhotonEvent.gReceiversAll, sendOptions);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06005212 RID: 21010 RVA: 0x001B3E68 File Offset: 0x001B2068
	public bool Equals(PhotonEvent other)
	{
		return !(other == null) && (this._eventId == other._eventId && this._enabled == other._enabled && this._reliable == other._reliable && this._failSilent == other._failSilent) && this._disposed == other._disposed;
	}

	// Token: 0x06005213 RID: 21011 RVA: 0x001B3EC8 File Offset: 0x001B20C8
	public override bool Equals(object obj)
	{
		PhotonEvent photonEvent = obj as PhotonEvent;
		return photonEvent != null && this.Equals(photonEvent);
	}

	// Token: 0x06005214 RID: 21012 RVA: 0x001B3EE8 File Offset: 0x001B20E8
	public override int GetHashCode()
	{
		int staticHash = this._eventId.GetStaticHash();
		int num = StaticHash.Compute(this._enabled, this._reliable, this._failSilent, this._disposed);
		return StaticHash.Compute(staticHash, num);
	}

	// Token: 0x06005215 RID: 21013 RVA: 0x001B3F24 File Offset: 0x001B2124
	static PhotonEvent()
	{
		PhotonEvent.gSendUnreliable.Encrypt = true;
		PhotonEvent.gSendReliable = SendOptions.SendReliable;
		PhotonEvent.gSendReliable.Encrypt = true;
	}

	// Token: 0x06005216 RID: 21014 RVA: 0x001B3F89 File Offset: 0x001B2189
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	private static void StaticLoadAfterPhotonNetwork()
	{
		PhotonNetwork.NetworkingClient.EventReceived += PhotonEvent.StaticOnEvent;
	}

	// Token: 0x06005217 RID: 21015 RVA: 0x001B3FA1 File Offset: 0x001B21A1
	public static bool operator ==(PhotonEvent x, PhotonEvent y)
	{
		return EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}

	// Token: 0x06005218 RID: 21016 RVA: 0x001B3FAF File Offset: 0x001B21AF
	public static bool operator !=(PhotonEvent x, PhotonEvent y)
	{
		return !EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}

	// Token: 0x06005219 RID: 21017 RVA: 0x001B3FC0 File Offset: 0x001B21C0
	private static void StaticOnEvent(EventData evData)
	{
		if (evData.Code != 176)
		{
			return;
		}
		try
		{
			object[] array = evData.CustomData as object[];
			if (array != null && array.Length != 0 && array.Length <= 21)
			{
				object obj = array[0];
				if (obj is int)
				{
					int sender = (int)obj;
					if (sender != -1)
					{
						ListProcessor<PhotonEvent> listProcessor;
						if (PhotonEvent._photonEvents.TryGetValue(sender, out listProcessor))
						{
							object[] args;
							if (array.Length > 1)
							{
								args = new object[array.Length - 1];
								Array.Copy(array, 1, args, 0, args.Length);
							}
							else
							{
								args = Array.Empty<object>();
							}
							PhotonMessageInfoWrapped info = new PhotonMessageInfoWrapped(evData.Sender, PhotonNetwork.ServerTimestamp);
							listProcessor.ItemProcessor = delegate(in PhotonEvent pEv)
							{
								if (pEv._eventId == -1 || pEv._disposed || !pEv._enabled)
								{
									return;
								}
								pEv.InvokeDelegate(sender, args, info);
							};
							listProcessor.ProcessList();
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Action<EventData, Exception> onError = PhotonEvent.OnError;
			if (onError != null)
			{
				onError(evData, ex);
			}
		}
	}

	// Token: 0x0600521A RID: 21018 RVA: 0x001B40D8 File Offset: 0x001B22D8
	private static void AddPhotonEvent(PhotonEvent photonEvent)
	{
		int eventId = photonEvent._eventId;
		if (eventId == -1)
		{
			return;
		}
		ListProcessor<PhotonEvent> listProcessor;
		if (!PhotonEvent._photonEvents.TryGetValue(eventId, out listProcessor))
		{
			listProcessor = new ListProcessor<PhotonEvent>(10, null);
			PhotonEvent._photonEvents.Add(eventId, listProcessor);
		}
		if (listProcessor.Contains(in photonEvent))
		{
			return;
		}
		listProcessor.Add(in photonEvent);
	}

	// Token: 0x0600521B RID: 21019 RVA: 0x001B4128 File Offset: 0x001B2328
	private static void RemovePhotonEvent(PhotonEvent photonEvent)
	{
		ListProcessor<PhotonEvent> listProcessor;
		if (!PhotonEvent._photonEvents.TryGetValue(photonEvent._eventId, out listProcessor))
		{
			return;
		}
		listProcessor.Remove(in photonEvent);
		if (listProcessor.Count == 0)
		{
			PhotonEvent._photonEvents.Remove(photonEvent._eventId);
		}
	}

	// Token: 0x0600521C RID: 21020 RVA: 0x001B416C File Offset: 0x001B236C
	public static PhotonEvent operator +(PhotonEvent photonEvent, Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (photonEvent == null)
		{
			throw new ArgumentNullException("photonEvent");
		}
		photonEvent.AddCallback(callback);
		return photonEvent;
	}

	// Token: 0x0600521D RID: 21021 RVA: 0x001B418A File Offset: 0x001B238A
	public static PhotonEvent operator -(PhotonEvent photonEvent, Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (photonEvent == null)
		{
			throw new ArgumentNullException("photonEvent");
		}
		photonEvent.RemoveCallback(callback);
		return photonEvent;
	}

	// Token: 0x04006453 RID: 25683
	private const int MAX_EVENT_ARGS = 20;

	// Token: 0x04006454 RID: 25684
	private const int INVALID_ID = -1;

	// Token: 0x04006455 RID: 25685
	[SerializeField]
	private int _eventId = -1;

	// Token: 0x04006456 RID: 25686
	[SerializeField]
	private bool _enabled;

	// Token: 0x04006457 RID: 25687
	[SerializeField]
	private bool _reliable;

	// Token: 0x04006458 RID: 25688
	[SerializeField]
	private bool _failSilent;

	// Token: 0x04006459 RID: 25689
	[NonSerialized]
	private bool _disposed;

	// Token: 0x0400645A RID: 25690
	private Action<int, int, object[], PhotonMessageInfoWrapped> _delegate;

	// Token: 0x0400645C RID: 25692
	public const byte PHOTON_EVENT_CODE = 176;

	// Token: 0x0400645D RID: 25693
	private static readonly RaiseEventOptions gReceiversAll = new RaiseEventOptions
	{
		Receivers = ReceiverGroup.All
	};

	// Token: 0x0400645E RID: 25694
	private static readonly RaiseEventOptions gReceiversOthers = new RaiseEventOptions
	{
		Receivers = ReceiverGroup.Others
	};

	// Token: 0x0400645F RID: 25695
	private static readonly SendOptions gSendReliable;

	// Token: 0x04006460 RID: 25696
	private static readonly SendOptions gSendUnreliable = SendOptions.SendUnreliable;

	// Token: 0x04006461 RID: 25697
	private static readonly Dictionary<int, ListProcessor<PhotonEvent>> _photonEvents = new Dictionary<int, ListProcessor<PhotonEvent>>(20);

	// Token: 0x02000CF2 RID: 3314
	public enum RaiseMode
	{
		// Token: 0x04006463 RID: 25699
		Local,
		// Token: 0x04006464 RID: 25700
		RemoteOthers,
		// Token: 0x04006465 RID: 25701
		RemoteAll
	}
}
