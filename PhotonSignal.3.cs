using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000D05 RID: 3333
[Serializable]
public class PhotonSignal<T1, T2> : PhotonSignal
{
	// Token: 0x170007CC RID: 1996
	// (get) Token: 0x06005297 RID: 21143 RVA: 0x000133F3 File Offset: 0x000115F3
	public override int argCount
	{
		get
		{
			return 2;
		}
	}

	// Token: 0x14000093 RID: 147
	// (add) Token: 0x06005298 RID: 21144 RVA: 0x001B4D8F File Offset: 0x001B2F8F
	// (remove) Token: 0x06005299 RID: 21145 RVA: 0x001B4DC3 File Offset: 0x001B2FC3
	public new event OnSignalReceived<T1, T2> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x0600529A RID: 21146 RVA: 0x001B4C57 File Offset: 0x001B2E57
	public PhotonSignal(string signalID)
		: base(signalID)
	{
	}

	// Token: 0x0600529B RID: 21147 RVA: 0x001B4C60 File Offset: 0x001B2E60
	public PhotonSignal(int signalID)
		: base(signalID)
	{
	}

	// Token: 0x0600529C RID: 21148 RVA: 0x001B4DE0 File Offset: 0x001B2FE0
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x0600529D RID: 21149 RVA: 0x001B4DEF File Offset: 0x001B2FEF
	public void Raise(T1 arg1, T2 arg2)
	{
		this.Raise(this._receivers, arg1, arg2);
	}

	// Token: 0x0600529E RID: 21150 RVA: 0x001B4E00 File Offset: 0x001B3000
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2)
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
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo photonSignalInfo = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, photonSignalInfo);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x0600529F RID: 21151 RVA: 0x001B4EA8 File Offset: 0x001B30A8
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 t;
		T2 t2;
		if (!args.TryParseArgs(2, out t, out t2))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2>(this._callbacks, t, t2, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2>(this._callbacks, t, t2, info);
	}

	// Token: 0x060052A0 RID: 21152 RVA: 0x001B4EE8 File Offset: 0x001B30E8
	public new static implicit operator PhotonSignal<T1, T2>(string s)
	{
		return new PhotonSignal<T1, T2>(s);
	}

	// Token: 0x060052A1 RID: 21153 RVA: 0x001B4EF0 File Offset: 0x001B30F0
	public new static explicit operator PhotonSignal<T1, T2>(int i)
	{
		return new PhotonSignal<T1, T2>(i);
	}

	// Token: 0x0400647F RID: 25727
	private OnSignalReceived<T1, T2> _callbacks;

	// Token: 0x04006480 RID: 25728
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2>).FullName.GetStaticHash();
}
