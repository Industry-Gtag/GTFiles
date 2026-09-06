using System;
using ExitGames.Client.Photon;
using GorillaNetworking;
using Photon.Pun;
using PlayFab;
using UnityEngine;

// Token: 0x02000D0D RID: 3341
public class PUNErrorLogging : MonoBehaviour
{
	// Token: 0x06005300 RID: 21248 RVA: 0x001B6778 File Offset: 0x001B4978
	private void Start()
	{
		PhotonNetwork.InternalEventError = (Action<EventData, Exception>)Delegate.Combine(PhotonNetwork.InternalEventError, new Action<EventData, Exception>(this.PUNError));
		PlayFabTitleDataCache.Instance.GetTitleData("PUNErrorLogging", delegate(string data)
		{
			int num;
			if (!int.TryParse(data, out num))
			{
				return;
			}
			PUNErrorLogging.LogFlags logFlags = (PUNErrorLogging.LogFlags)num;
			this.m_logSerializeView = logFlags.HasFlag(PUNErrorLogging.LogFlags.SerializeView);
			this.m_logOwnershipTransfer = logFlags.HasFlag(PUNErrorLogging.LogFlags.OwnershipTransfer);
			this.m_logOwnershipRequest = logFlags.HasFlag(PUNErrorLogging.LogFlags.OwnershipRequest);
			this.m_logOwnershipUpdate = logFlags.HasFlag(PUNErrorLogging.LogFlags.OwnershipUpdate);
			this.m_logRPC = logFlags.HasFlag(PUNErrorLogging.LogFlags.RPC);
			this.m_logInstantiate = logFlags.HasFlag(PUNErrorLogging.LogFlags.Instantiate);
			this.m_logDestroy = logFlags.HasFlag(PUNErrorLogging.LogFlags.Destroy);
			this.m_logDestroyPlayer = logFlags.HasFlag(PUNErrorLogging.LogFlags.DestroyPlayer);
		}, delegate(PlayFabError error)
		{
		}, false);
	}

	// Token: 0x06005301 RID: 21249 RVA: 0x001B67E0 File Offset: 0x001B49E0
	private void PUNError(EventData data, Exception exception)
	{
		NetworkSystem.Instance.GetPlayer(data.Sender);
		byte code = data.Code;
		switch (code)
		{
		case 200:
			this.PrintException(exception, this.m_logRPC);
			return;
		case 201:
		case 206:
			this.PrintException(exception, this.m_logSerializeView);
			return;
		case 202:
			this.PrintException(exception, this.m_logInstantiate);
			return;
		case 203:
		case 205:
		case 208:
		case 211:
			break;
		case 204:
			this.PrintException(exception, this.m_logDestroy);
			return;
		case 207:
			this.PrintException(exception, this.m_logDestroyPlayer);
			return;
		case 209:
			this.PrintException(exception, this.m_logOwnershipRequest);
			return;
		case 210:
			this.PrintException(exception, this.m_logOwnershipTransfer);
			return;
		case 212:
			this.PrintException(exception, this.m_logOwnershipUpdate);
			return;
		default:
			if (code == 254)
			{
				this.PrintException(exception, true);
				return;
			}
			break;
		}
		this.PrintException(exception, true);
	}

	// Token: 0x06005302 RID: 21250 RVA: 0x001B68CE File Offset: 0x001B4ACE
	private void PrintException(Exception e, bool print)
	{
		if (print)
		{
			Debug.LogException(e);
		}
	}

	// Token: 0x04006498 RID: 25752
	[SerializeField]
	private bool m_logSerializeView = true;

	// Token: 0x04006499 RID: 25753
	[SerializeField]
	private bool m_logOwnershipTransfer = true;

	// Token: 0x0400649A RID: 25754
	[SerializeField]
	private bool m_logOwnershipRequest = true;

	// Token: 0x0400649B RID: 25755
	[SerializeField]
	private bool m_logOwnershipUpdate = true;

	// Token: 0x0400649C RID: 25756
	[SerializeField]
	private bool m_logRPC = true;

	// Token: 0x0400649D RID: 25757
	[SerializeField]
	private bool m_logInstantiate = true;

	// Token: 0x0400649E RID: 25758
	[SerializeField]
	private bool m_logDestroy = true;

	// Token: 0x0400649F RID: 25759
	[SerializeField]
	private bool m_logDestroyPlayer = true;

	// Token: 0x02000D0E RID: 3342
	[Flags]
	private enum LogFlags
	{
		// Token: 0x040064A1 RID: 25761
		SerializeView = 1,
		// Token: 0x040064A2 RID: 25762
		OwnershipTransfer = 2,
		// Token: 0x040064A3 RID: 25763
		OwnershipRequest = 4,
		// Token: 0x040064A4 RID: 25764
		OwnershipUpdate = 8,
		// Token: 0x040064A5 RID: 25765
		RPC = 16,
		// Token: 0x040064A6 RID: 25766
		Instantiate = 32,
		// Token: 0x040064A7 RID: 25767
		Destroy = 64,
		// Token: 0x040064A8 RID: 25768
		DestroyPlayer = 128
	}
}
