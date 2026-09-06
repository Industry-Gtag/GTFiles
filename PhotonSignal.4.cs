using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000D06 RID: 3334
[Serializable]
public class PhotonSignal<T1, T2, T3> : PhotonSignal
{
	// Token: 0x170007CD RID: 1997
	// (get) Token: 0x060052A3 RID: 21155 RVA: 0x00138C59 File Offset: 0x00136E59
	public override int argCount
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x14000094 RID: 148
	// (add) Token: 0x060052A4 RID: 21156 RVA: 0x001B4F13 File Offset: 0x001B3113
	// (remove) Token: 0x060052A5 RID: 21157 RVA: 0x001B4F47 File Offset: 0x001B3147
	public new event OnSignalReceived<T1, T2, T3> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2, T3>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x060052A6 RID: 21158 RVA: 0x001B4C57 File Offset: 0x001B2E57
	public PhotonSignal(string signalID)
		: base(signalID)
	{
	}

	// Token: 0x060052A7 RID: 21159 RVA: 0x001B4C60 File Offset: 0x001B2E60
	public PhotonSignal(int signalID)
		: base(signalID)
	{
	}

	// Token: 0x060052A8 RID: 21160 RVA: 0x001B4F64 File Offset: 0x001B3164
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x060052A9 RID: 21161 RVA: 0x001B4F73 File Offset: 0x001B3173
	public void Raise(T1 arg1, T2 arg2, T3 arg3)
	{
		this.Raise(this._receivers, arg1, arg2, arg3);
	}

	// Token: 0x060052AA RID: 21162 RVA: 0x001B4F84 File Offset: 0x001B3184
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2, T3 arg3)
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
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo photonSignalInfo = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, photonSignalInfo);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x060052AB RID: 21163 RVA: 0x001B5034 File Offset: 0x001B3234
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 t;
		T2 t2;
		T3 t3;
		if (!args.TryParseArgs(2, out t, out t2, out t3))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2, T3>(this._callbacks, t, t2, t3, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2, T3>(this._callbacks, t, t2, t3, info);
	}

	// Token: 0x060052AC RID: 21164 RVA: 0x001B5078 File Offset: 0x001B3278
	public new static implicit operator PhotonSignal<T1, T2, T3>(string s)
	{
		return new PhotonSignal<T1, T2, T3>(s);
	}

	// Token: 0x060052AD RID: 21165 RVA: 0x001B5080 File Offset: 0x001B3280
	public new static explicit operator PhotonSignal<T1, T2, T3>(int i)
	{
		return new PhotonSignal<T1, T2, T3>(i);
	}

	// Token: 0x04006481 RID: 25729
	private OnSignalReceived<T1, T2, T3> _callbacks;

	// Token: 0x04006482 RID: 25730
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2, T3>).FullName.GetStaticHash();
}
