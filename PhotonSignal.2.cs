using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000D04 RID: 3332
[Serializable]
public class PhotonSignal<T1> : PhotonSignal
{
	// Token: 0x170007CB RID: 1995
	// (get) Token: 0x0600528B RID: 21131 RVA: 0x00023F0C File Offset: 0x0002210C
	public override int argCount
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x14000092 RID: 146
	// (add) Token: 0x0600528C RID: 21132 RVA: 0x001B4C06 File Offset: 0x001B2E06
	// (remove) Token: 0x0600528D RID: 21133 RVA: 0x001B4C3A File Offset: 0x001B2E3A
	public new event OnSignalReceived<T1> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x0600528E RID: 21134 RVA: 0x001B4C57 File Offset: 0x001B2E57
	public PhotonSignal(string signalID)
		: base(signalID)
	{
	}

	// Token: 0x0600528F RID: 21135 RVA: 0x001B4C60 File Offset: 0x001B2E60
	public PhotonSignal(int signalID)
		: base(signalID)
	{
	}

	// Token: 0x06005290 RID: 21136 RVA: 0x001B4C69 File Offset: 0x001B2E69
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x06005291 RID: 21137 RVA: 0x001B4C78 File Offset: 0x001B2E78
	public void Raise(T1 arg1)
	{
		this.Raise(this._receivers, arg1);
	}

	// Token: 0x06005292 RID: 21138 RVA: 0x001B4C88 File Offset: 0x001B2E88
	public void Raise(ReceiverGroup receivers, T1 arg1)
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
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo photonSignalInfo = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, photonSignalInfo);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x06005293 RID: 21139 RVA: 0x001B4D28 File Offset: 0x001B2F28
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 t;
		if (!args.TryParseArgs(2, out t))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1>(this._callbacks, t, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1>(this._callbacks, t, info);
	}

	// Token: 0x06005294 RID: 21140 RVA: 0x001B4D64 File Offset: 0x001B2F64
	public new static implicit operator PhotonSignal<T1>(string s)
	{
		return new PhotonSignal<T1>(s);
	}

	// Token: 0x06005295 RID: 21141 RVA: 0x001B4D6C File Offset: 0x001B2F6C
	public new static explicit operator PhotonSignal<T1>(int i)
	{
		return new PhotonSignal<T1>(i);
	}

	// Token: 0x0400647D RID: 25725
	private OnSignalReceived<T1> _callbacks;

	// Token: 0x0400647E RID: 25726
	private static readonly int kSignature = typeof(PhotonSignal<T1>).FullName.GetStaticHash();
}
