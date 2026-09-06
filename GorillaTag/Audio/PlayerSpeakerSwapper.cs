using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020012B0 RID: 4784
	public class PlayerSpeakerSwapper : MonoBehaviour
	{
		// Token: 0x06007836 RID: 30774 RVA: 0x0026E904 File Offset: 0x0026CB04
		private void OnEnable()
		{
			NetworkSystem.Instance.OnPlayerJoined += this.OnPlayerCountChanged;
			NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerCountChanged;
			this.OnPlayerCountChanged(null);
		}

		// Token: 0x06007837 RID: 30775 RVA: 0x0026E95C File Offset: 0x0026CB5C
		private void OnDisable()
		{
			NetworkSystem.Instance.OnPlayerJoined -= this.OnPlayerCountChanged;
			NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerCountChanged;
		}

		// Token: 0x06007838 RID: 30776 RVA: 0x0026E9AC File Offset: 0x0026CBAC
		private void OnPlayerCountChanged(NetPlayer _)
		{
			int num = NetworkSystem.Instance.AllNetPlayers.Length;
			this._lowPassFilter.enabled = num >= 10;
		}

		// Token: 0x0400885F RID: 34911
		[SerializeField]
		private Behaviour _lowPassFilter;
	}
}
