using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F9B RID: 3995
	public class LayerChanger : MonoBehaviour
	{
		// Token: 0x06006364 RID: 25444 RVA: 0x001FF3F9 File Offset: 0x001FD5F9
		public void InitializeLayers(Transform parent)
		{
			if (!this.layersStored)
			{
				this.StoreOriginalLayers(parent);
				this.layersStored = true;
			}
		}

		// Token: 0x06006365 RID: 25445 RVA: 0x001FF414 File Offset: 0x001FD614
		private void StoreOriginalLayers(Transform parent)
		{
			if (!this.includeChildren)
			{
				this.StoreOriginalLayers(parent);
				return;
			}
			foreach (object obj in parent)
			{
				Transform transform = (Transform)obj;
				this.originalLayers[transform] = transform.gameObject.layer;
				this.StoreOriginalLayers(transform);
			}
		}

		// Token: 0x06006366 RID: 25446 RVA: 0x001FF490 File Offset: 0x001FD690
		public void ChangeLayer(Transform parent, string newLayer)
		{
			if (!this.layersStored)
			{
				Debug.LogWarning("Layers have not been initialized. Call InitializeLayers first.");
				return;
			}
			this.ChangeLayers(parent, LayerMask.NameToLayer(newLayer));
		}

		// Token: 0x06006367 RID: 25447 RVA: 0x001FF4B4 File Offset: 0x001FD6B4
		private void ChangeLayers(Transform parent, int newLayer)
		{
			if (!this.includeChildren)
			{
				if (!this.restrictedLayers.Contains(parent.gameObject.layer))
				{
					parent.gameObject.layer = newLayer;
				}
				return;
			}
			foreach (object obj in parent)
			{
				Transform transform = (Transform)obj;
				if (!this.restrictedLayers.Contains(transform.gameObject.layer))
				{
					transform.gameObject.layer = newLayer;
					this.ChangeLayers(transform, newLayer);
				}
			}
		}

		// Token: 0x06006368 RID: 25448 RVA: 0x001FF55C File Offset: 0x001FD75C
		public void RestoreOriginalLayers()
		{
			if (!this.layersStored)
			{
				Debug.LogWarning("Layers have not been initialized. Call InitializeLayers first.");
				return;
			}
			foreach (KeyValuePair<Transform, int> keyValuePair in this.originalLayers)
			{
				keyValuePair.Key.gameObject.layer = keyValuePair.Value;
			}
		}

		// Token: 0x04007200 RID: 29184
		public LayerMask restrictedLayers;

		// Token: 0x04007201 RID: 29185
		public bool includeChildren = true;

		// Token: 0x04007202 RID: 29186
		private Dictionary<Transform, int> originalLayers = new Dictionary<Transform, int>();

		// Token: 0x04007203 RID: 29187
		private bool layersStored;
	}
}
