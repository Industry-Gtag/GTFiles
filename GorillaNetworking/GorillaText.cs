using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaNetworking
{
	// Token: 0x020010EB RID: 4331
	[Serializable]
	public class GorillaText
	{
		// Token: 0x06006CAC RID: 27820 RVA: 0x00232348 File Offset: 0x00230548
		public void Initialize(Material[] originalMaterials, Material failureMaterial, UnityEvent<string> callback = null, UnityEvent<Material[]> materialCallback = null)
		{
			this.failureMaterial = failureMaterial;
			this.originalMaterials = originalMaterials;
			this.currentMaterials = originalMaterials;
			Debug.Log("Original text = " + this.originalText);
			this.updateTextCallback = callback;
			this.updateMaterialCallback = materialCallback;
			GorillaTextManager.RegisterText(this);
		}

		// Token: 0x06006CAD RID: 27821 RVA: 0x00232394 File Offset: 0x00230594
		public void InvokeIfUpdated()
		{
			if (!this.modified)
			{
				return;
			}
			this.modified = false;
			string text = this.stringBuilder.ToString();
			if (this.currentText != text)
			{
				this.currentText = text;
				UnityEvent<string> unityEvent = this.updateTextCallback;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(this.currentText);
			}
		}

		// Token: 0x06006CAE RID: 27822 RVA: 0x002323E8 File Offset: 0x002305E8
		public void EnableFailedState(string failText)
		{
			this.failedState = true;
			this.failureText = failText;
			UnityEvent<string> unityEvent = this.updateTextCallback;
			if (unityEvent != null)
			{
				unityEvent.Invoke(failText);
			}
			this.originalText = this.currentText;
			this.currentText = failText;
			this.currentMaterials = (Material[])this.originalMaterials.Clone();
			this.currentMaterials[0] = this.failureMaterial;
			UnityEvent<Material[]> unityEvent2 = this.updateMaterialCallback;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke(this.currentMaterials);
		}

		// Token: 0x06006CAF RID: 27823 RVA: 0x00232464 File Offset: 0x00230664
		public void DisableFailedState()
		{
			this.failedState = false;
			UnityEvent<string> unityEvent = this.updateTextCallback;
			if (unityEvent != null)
			{
				unityEvent.Invoke(this.originalText);
			}
			this.failureText = "";
			this.currentText = this.originalText;
			this.currentMaterials = this.originalMaterials;
			UnityEvent<Material[]> unityEvent2 = this.updateMaterialCallback;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke(this.currentMaterials);
		}

		// Token: 0x06006CB0 RID: 27824 RVA: 0x002324C8 File Offset: 0x002306C8
		public void Append(string str)
		{
			this.modified = true;
			this.stringBuilder.Append(str);
		}

		// Token: 0x06006CB1 RID: 27825 RVA: 0x002324DE File Offset: 0x002306DE
		public void Set(string str)
		{
			this.modified = true;
			this.stringBuilder.Clear();
			this.stringBuilder.Append(str);
		}

		// Token: 0x04007D06 RID: 32006
		private string failureText;

		// Token: 0x04007D07 RID: 32007
		public string currentText;

		// Token: 0x04007D08 RID: 32008
		private string originalText = string.Empty;

		// Token: 0x04007D09 RID: 32009
		private StringBuilder stringBuilder = new StringBuilder();

		// Token: 0x04007D0A RID: 32010
		private bool modified;

		// Token: 0x04007D0B RID: 32011
		private bool failedState;

		// Token: 0x04007D0C RID: 32012
		private Material[] originalMaterials;

		// Token: 0x04007D0D RID: 32013
		private Material failureMaterial;

		// Token: 0x04007D0E RID: 32014
		internal Material[] currentMaterials;

		// Token: 0x04007D0F RID: 32015
		private UnityEvent<string> updateTextCallback;

		// Token: 0x04007D10 RID: 32016
		private UnityEvent<Material[]> updateMaterialCallback;
	}
}
