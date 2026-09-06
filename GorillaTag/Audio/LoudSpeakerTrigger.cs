using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020012AE RID: 4782
	public class LoudSpeakerTrigger : MonoBehaviour
	{
		// Token: 0x0600782F RID: 30767 RVA: 0x0026E76E File Offset: 0x0026C96E
		public void SetRecorder(GTRecorder recorder)
		{
			this._recorder = recorder;
		}

		// Token: 0x06007830 RID: 30768 RVA: 0x0026E778 File Offset: 0x0026C978
		public void OnPlayerEnter(VRRig player)
		{
			if (this._recorder != null && this._network != null)
			{
				this._recorder.AllowPitchAdjustment = true;
				this._recorder.PitchAdjustment = this.PitchAdjustment;
				this._network.StartBroadcastSpeakerOutput(player);
			}
		}

		// Token: 0x06007831 RID: 30769 RVA: 0x0026E7CC File Offset: 0x0026C9CC
		public void OnPlayerExit(VRRig player)
		{
			if (this._recorder != null && this._network != null)
			{
				this._recorder.AllowPitchAdjustment = false;
				this._recorder.PitchAdjustment = 1f;
				this._network.StopBroadcastSpeakerOutput(player);
			}
		}

		// Token: 0x0400885B RID: 34907
		public float PitchAdjustment = 1f;

		// Token: 0x0400885C RID: 34908
		[SerializeField]
		private LoudSpeakerNetwork _network;

		// Token: 0x0400885D RID: 34909
		[SerializeField]
		private GTRecorder _recorder;
	}
}
