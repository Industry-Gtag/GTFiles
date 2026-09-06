using System;
using System.Collections;
using Photon.Voice.Unity;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020012A8 RID: 4776
	public class GTRecorder : Recorder, ITickSystemPost
	{
		// Token: 0x17000BAE RID: 2990
		// (get) Token: 0x06007800 RID: 30720 RVA: 0x0026DC06 File Offset: 0x0026BE06
		// (set) Token: 0x06007801 RID: 30721 RVA: 0x0026DC0E File Offset: 0x0026BE0E
		public bool PostTickRunning { get; set; }

		// Token: 0x06007802 RID: 30722 RVA: 0x001AF0B5 File Offset: 0x001AD2B5
		private void OnEnable()
		{
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x06007803 RID: 30723 RVA: 0x0015AB83 File Offset: 0x00158D83
		private void OnDisable()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x06007804 RID: 30724 RVA: 0x0026DC17 File Offset: 0x0026BE17
		protected override MicWrapper CreateMicWrapper(string micDev, int samplingRateInt, VoiceLogger logger)
		{
			this._micWrapper = new GTMicWrapper(micDev, samplingRateInt, this.AllowPitchAdjustment, this.PitchAdjustment, this.AllowVolumeAdjustment, this.VolumeAdjustment, logger);
			return this._micWrapper;
		}

		// Token: 0x06007805 RID: 30725 RVA: 0x0026DC45 File Offset: 0x0026BE45
		private IEnumerator DoTestEcho()
		{
			base.DebugEchoMode = true;
			yield return new WaitForSeconds(this.DebugEchoLength);
			base.DebugEchoMode = false;
			yield return null;
			this._testEchoCoroutine = null;
			yield break;
		}

		// Token: 0x06007806 RID: 30726 RVA: 0x0026DC54 File Offset: 0x0026BE54
		public void PostTick()
		{
			if (this._micWrapper != null)
			{
				this._micWrapper.UpdateWrapper(this.AllowPitchAdjustment, this.PitchAdjustment, this.AllowVolumeAdjustment, this.VolumeAdjustment);
			}
		}

		// Token: 0x04008838 RID: 34872
		public bool AllowPitchAdjustment;

		// Token: 0x04008839 RID: 34873
		public float PitchAdjustment = 1f;

		// Token: 0x0400883A RID: 34874
		public bool AllowVolumeAdjustment;

		// Token: 0x0400883B RID: 34875
		public float VolumeAdjustment = 1f;

		// Token: 0x0400883C RID: 34876
		public float DebugEchoLength = 5f;

		// Token: 0x0400883D RID: 34877
		private GTMicWrapper _micWrapper;

		// Token: 0x0400883E RID: 34878
		private Coroutine _testEchoCoroutine;
	}
}
