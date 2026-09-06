using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000D08 RID: 3336
[Serializable]
public class PhotonSignal<T1, T2, T3, T4, T5> : PhotonSignal
{
	// Token: 0x170007CF RID: 1999
	// (get) Token: 0x060052BB RID: 21179 RVA: 0x001B524B File Offset: 0x001B344B
	public override int argCount
	{
		get
		{
			return 5;
		}
	}

	// Token: 0x14000096 RID: 150
	// (add) Token: 0x060052BC RID: 21180 RVA: 0x001B524E File Offset: 0x001B344E
	// (remove) Token: 0x060052BD RID: 21181 RVA: 0x001B5282 File Offset: 0x001B3482
	public new event OnSignalReceived<T1, T2, T3, T4, T5> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4, T5>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4, T5>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4, T5>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x060052BE RID: 21182 RVA: 0x001B4C57 File Offset: 0x001B2E57
	public PhotonSignal(string signalID)
		: base(signalID)
	{
	}

	// Token: 0x060052BF RID: 21183 RVA: 0x001B4C60 File Offset: 0x001B2E60
	public PhotonSignal(int signalID)
		: base(signalID)
	{
	}

	// Token: 0x060052C0 RID: 21184 RVA: 0x001B529F File Offset: 0x001B349F
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x060052C1 RID: 21185 RVA: 0x001B52AE File Offset: 0x001B34AE
	public void Raise(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		this.Raise(this._receivers, arg1, arg2, arg3, arg4, arg5);
	}

	// Token: 0x060052C2 RID: 21186 RVA: 0x001B52C4 File Offset: 0x001B34C4
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
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
		array[6] = arg5;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo photonSignalInfo = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, photonSignalInfo);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x060052C3 RID: 21187 RVA: 0x001B5388 File Offset: 0x001B3588
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 t;
		T2 t2;
		T3 t3;
		T4 t4;
		T5 t5;
		if (!args.TryParseArgs(2, out t, out t2, out t3, out t4, out t5))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2, T3, T4, T5>(this._callbacks, t, t2, t3, t4, t5, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2, T3, T4, T5>(this._callbacks, t, t2, t3, t4, t5, info);
	}

	// Token: 0x060052C4 RID: 21188 RVA: 0x001B53D6 File Offset: 0x001B35D6
	public new static implicit operator PhotonSignal<T1, T2, T3, T4, T5>(string s)
	{
		return new PhotonSignal<T1, T2, T3, T4, T5>(s);
	}

	// Token: 0x060052C5 RID: 21189 RVA: 0x001B53DE File Offset: 0x001B35DE
	public new static explicit operator PhotonSignal<T1, T2, T3, T4, T5>(int i)
	{
		return new PhotonSignal<T1, T2, T3, T4, T5>(i);
	}

	// Token: 0x04006485 RID: 25733
	private OnSignalReceived<T1, T2, T3, T4, T5> _callbacks;

	// Token: 0x04006486 RID: 25734
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2, T3, T4, T5>).FullName.GetStaticHash();
}
