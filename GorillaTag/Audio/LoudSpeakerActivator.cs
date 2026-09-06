using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020012AC RID: 4780
	public class LoudSpeakerActivator : MonoBehaviour
	{
		// Token: 0x0600781C RID: 30748 RVA: 0x0026E113 File Offset: 0x0026C313
		private void Awake()
		{
			this._isLocal = this.IsParentedToLocalRig();
			if (!this._isLocal)
			{
				this._nonlocalRig = base.transform.root.GetComponent<VRRig>();
			}
		}

		// Token: 0x0600781D RID: 30749 RVA: 0x0026E140 File Offset: 0x0026C340
		private bool IsParentedToLocalRig()
		{
			if (VRRigCache.Instance.localRig == null)
			{
				return false;
			}
			Transform transform = base.transform.parent;
			while (transform != null)
			{
				if (transform == VRRigCache.Instance.localRig.transform)
				{
					return true;
				}
				transform = transform.parent;
			}
			return false;
		}

		// Token: 0x0600781E RID: 30750 RVA: 0x0026E199 File Offset: 0x0026C399
		public void SetRecorder(GTRecorder recorder)
		{
			this._recorder = recorder;
		}

		// Token: 0x0600781F RID: 30751 RVA: 0x0026E1A4 File Offset: 0x0026C3A4
		public void StartLocalBroadcast()
		{
			if (!this._isLocal)
			{
				if (this._network != null && this._nonlocalRig != null)
				{
					this._network.StartBroadcastSpeakerOutput(this._nonlocalRig);
				}
				return;
			}
			if (this.IsBroadcasting)
			{
				return;
			}
			if (this._recorder == null && NetworkSystem.Instance.LocalRecorder != null)
			{
				this.SetRecorder((GTRecorder)NetworkSystem.Instance.LocalRecorder);
			}
			if (this._recorder != null && this._network != null)
			{
				this.IsBroadcasting = true;
				this._recorder.AllowPitchAdjustment = true;
				this._recorder.PitchAdjustment = this.PitchAdjustment;
				this._recorder.AllowVolumeAdjustment = true;
				this._recorder.VolumeAdjustment = this.VolumeAdjustment;
				this._network.StartBroadcastSpeakerOutput(VRRigCache.Instance.localRig.Rig);
			}
		}

		// Token: 0x06007820 RID: 30752 RVA: 0x0026E29C File Offset: 0x0026C49C
		public void StopLocalBroadcast()
		{
			if (!this._isLocal)
			{
				if (this._network != null && this._nonlocalRig != null)
				{
					this._network.StopBroadcastSpeakerOutput(this._nonlocalRig);
				}
				return;
			}
			if (!this.IsBroadcasting)
			{
				return;
			}
			if (this._recorder == null && NetworkSystem.Instance.LocalRecorder != null)
			{
				this.SetRecorder((GTRecorder)NetworkSystem.Instance.LocalRecorder);
			}
			if (this._recorder != null && this._network != null)
			{
				this.IsBroadcasting = false;
				this._recorder.AllowPitchAdjustment = false;
				this._recorder.PitchAdjustment = 1f;
				this._recorder.AllowVolumeAdjustment = false;
				this._recorder.VolumeAdjustment = 1f;
				this._network.StopBroadcastSpeakerOutput(VRRigCache.Instance.localRig.Rig);
			}
		}

		// Token: 0x0400884E RID: 34894
		public float PitchAdjustment = 1f;

		// Token: 0x0400884F RID: 34895
		public float VolumeAdjustment = 2.5f;

		// Token: 0x04008850 RID: 34896
		public bool IsBroadcasting;

		// Token: 0x04008851 RID: 34897
		[SerializeField]
		private LoudSpeakerNetwork _network;

		// Token: 0x04008852 RID: 34898
		[SerializeField]
		private GTRecorder _recorder;

		// Token: 0x04008853 RID: 34899
		private bool _isLocal;

		// Token: 0x04008854 RID: 34900
		private VRRig _nonlocalRig;
	}
}
