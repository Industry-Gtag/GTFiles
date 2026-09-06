using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000D07 RID: 3335
[Serializable]
public class PhotonSignal<T1, T2, T3, T4> : PhotonSignal
{
	// Token: 0x170007CE RID: 1998
	// (get) Token: 0x060052AF RID: 21167 RVA: 0x001B50A3 File Offset: 0x001B32A3
	public override int argCount
	{
		get
		{
			return 4;
		}
	}

	// Token: 0x14000095 RID: 149
	// (add) Token: 0x060052B0 RID: 21168 RVA: 0x001B50A6 File Offset: 0x001B32A6
	// (remove) Token: 0x060052B1 RID: 21169 RVA: 0x001B50DA File Offset: 0x001B32DA
	public new event OnSignalReceived<T1, T2, T3, T4> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x060052B2 RID: 21170 RVA: 0x001B4C57 File Offset: 0x001B2E57
	public PhotonSignal(string signalID)
		: base(signalID)
	{
	}

	// Token: 0x060052B3 RID: 21171 RVA: 0x001B4C60 File Offset: 0x001B2E60
	public PhotonSignal(int signalID)
		: base(signalID)
	{
	}

	// Token: 0x060052B4 RID: 21172 RVA: 0x001B50F7 File Offset: 0x001B32F7
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x060052B5 RID: 21173 RVA: 0x001B5106 File Offset: 0x001B3306
	public void Raise(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		this.Raise(this._receivers, arg1, arg2, arg3, arg4);
	}

	// Token: 0x060052B6 RID: 21174 RVA: 0x001B511C File Offset: 0x001B331C
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (!this._enabled)
		{
			return;
		}
		if (this._mute)
		{
			return;
		}
		RaiseEventOptions raiseEventOptions = PhotonSignal.gGroupToOptions[receivers];
		object[] array = PhotonUtils.FetchScratchArray(2 + this.argCount);
		int serverTimestamp = PhotonNetwork.ServerTimestamp;
		array[0] = this._signalID;
		array[1] = serverTimestamp;
		array[2] = arg1;
		array[3] = arg2;
		array[4] = arg3;
		array[5] = arg4;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo photonSignalInfo = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, photonSignalInfo);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x060052B7 RID: 21175 RVA: 0x001B51D8 File Offset: 0x001B33D8
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 t;
		T2 t2;
		T3 t3;
		T4 t4;
		if (!args.TryParseArgs(2, out t, out t2, out t3, out t4))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2, T3, T4>(this._callbacks, t, t2, t3, t4, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2, T3, T4>(this._callbacks, t, t2, t3, t4, info);
	}

	// Token: 0x060052B8 RID: 21176 RVA: 0x001B5220 File Offset: 0x001B3420
	public new static implicit operator PhotonSignal<T1, T2, T3, T4>(string s)
	{
		return new PhotonSignal<T1, T2, T3, T4>(s);
	}

	// Token: 0x060052B9 RID: 21177 RVA: 0x001B5228 File Offset: 0x001B3428
	public new static explicit operator PhotonSignal<T1, T2, T3, T4>(int i)
	{
		return new PhotonSignal<T1, T2, T3, T4>(i);
	}

	// Token: 0x04006483 RID: 25731
	private OnSignalReceived<T1, T2, T3, T4> _callbacks;

	// Token: 0x04006484 RID: 25732
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2, T3, T4>).FullName.GetStaticHash();
}
