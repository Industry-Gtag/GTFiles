using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	// Token: 0x0200116E RID: 4462
	public class SocialCoconutCamera : MonoBehaviour
	{
		// Token: 0x06006FE6 RID: 28646 RVA: 0x002411A9 File Offset: 0x0023F3A9
		private void Awake()
		{
			if (this._propertyBlock == null)
			{
				this._propertyBlock = new MaterialPropertyBlock();
			}
			this._propertyBlock.SetInt(this.IS_RECORDING, 0);
			this._bodyRenderer.SetPropertyBlock(this._propertyBlock);
		}

		// Token: 0x06006FE7 RID: 28647 RVA: 0x002411E1 File Offset: 0x0023F3E1
		public void SetVisualsActive(bool active)
		{
			this._isActive = active;
			this._visuals.SetActive(active);
		}

		// Token: 0x06006FE8 RID: 28648 RVA: 0x002411F6 File Offset: 0x0023F3F6
		public void SetRecordingState(bool isRecording)
		{
			if (!this._isActive)
			{
				return;
			}
			this._propertyBlock.SetInt(this.IS_RECORDING, isRecording ? 1 : 0);
			this._bodyRenderer.SetPropertyBlock(this._propertyBlock);
		}

		// Token: 0x04007FCA RID: 32714
		[SerializeField]
		private GameObject _visuals;

		// Token: 0x04007FCB RID: 32715
		[SerializeField]
		private MeshRenderer _bodyRenderer;

		// Token: 0x04007FCC RID: 32716
		private bool _isActive;

		// Token: 0x04007FCD RID: 32717
		private MaterialPropertyBlock _propertyBlock;

		// Token: 0x04007FCE RID: 32718
		private string IS_RECORDING = "_Is_Recording";
	}
}
