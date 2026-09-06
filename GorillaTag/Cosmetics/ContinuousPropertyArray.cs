using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200130D RID: 4877
	[Serializable]
	public class ContinuousPropertyArray
	{
		// Token: 0x17000BF7 RID: 3063
		// (get) Token: 0x06007A6A RID: 31338 RVA: 0x0027EFEA File Offset: 0x0027D1EA
		public int Count
		{
			get
			{
				return this.list.Length;
			}
		}

		// Token: 0x06007A6B RID: 31339 RVA: 0x0027EFF4 File Offset: 0x0027D1F4
		private void InitIfNeeded()
		{
			if (this.initialized)
			{
				return;
			}
			this.initialized = true;
			this.inverseMaximum = 1f / this.maxExpectedValue;
			this.value = 0f;
			this.lastApplyTime = Time.time - Time.deltaTime;
			for (int i = 0; i < this.list.Length; i++)
			{
				this.list[i].Init();
			}
			if (Application.isPlaying)
			{
				for (int j = 0; j < this.list.Length; j++)
				{
					this.list[j].InitThreshold();
				}
			}
			this.uniqueShaderPropertyIndices = new List<int>();
			this.mpb = new MaterialPropertyBlock();
			ContinuousPropertyArray.PropertyComparer propertyComparer = new ContinuousPropertyArray.PropertyComparer();
			Array.Sort<ContinuousProperty>(this.list, propertyComparer);
			if (this.list[0].IsShaderProperty_Cached)
			{
				for (int k = 0; k < this.list.Length; k++)
				{
					if (!this.list[k].IsShaderProperty_Cached)
					{
						this.uniqueShaderPropertyIndices.Add(k);
						return;
					}
					if (k == this.list.Length - 1 || (k > 0 && propertyComparer.Compare(this.list[k - 1], this.list[k]) != 0))
					{
						this.uniqueShaderPropertyIndices.Add(k);
					}
				}
			}
		}

		// Token: 0x06007A6C RID: 31340 RVA: 0x0027F125 File Offset: 0x0027D325
		public void ApplyAll(bool leftHand, float f)
		{
			this.ApplyAll(f);
		}

		// Token: 0x06007A6D RID: 31341 RVA: 0x0027F130 File Offset: 0x0027D330
		public void ApplyAll(float f)
		{
			if (this.list.Length == 0)
			{
				return;
			}
			this.InitIfNeeded();
			float num = Time.time - this.lastApplyTime;
			this.value = (this.instant ? (f * this.inverseMaximum) : Mathf.Lerp(this.value, f * this.inverseMaximum, 1f - Mathf.Exp(-this.responsiveness * num)));
			this.lastApplyTime = Time.time;
			int num2 = int.MaxValue;
			if (this.uniqueShaderPropertyIndices.Count > 0)
			{
				num2 = 0;
				((Renderer)this.list[0].Target).GetPropertyBlock(this.mpb, this.list[0].IntValue);
			}
			bool flag = this.cachedRigIsLocal;
			for (int i = 0; i < this.list.Length; i++)
			{
				this.list[i].SetRigIsLocal(flag);
				this.list[i].Apply(this.value, num, this.mpb);
				if (num2 < this.uniqueShaderPropertyIndices.Count && i >= this.uniqueShaderPropertyIndices[num2] - 1)
				{
					((Renderer)this.list[i].Target).SetPropertyBlock(this.mpb, this.list[0].IntValue);
					if (++num2 < this.uniqueShaderPropertyIndices.Count)
					{
						((Renderer)this.list[i + 1].Target).GetPropertyBlock(this.mpb, this.list[i + 1].IntValue);
					}
				}
			}
		}

		// Token: 0x04008BF6 RID: 35830
		[Tooltip("Divides the input value by this number before being fed into the property array. Unless you know what you're doing, you should probably leave this at 1. You can accomplish the same thing by changing the maximum X value for all the curves/gradients, this is just a shorthand.")]
		[SerializeField]
		private float maxExpectedValue = 1f;

		// Token: 0x04008BF7 RID: 35831
		private float inverseMaximum;

		// Token: 0x04008BF8 RID: 35832
		[Tooltip("Determines how quickly the internal value lerps towards the input value. A low number will take a long time to match but will be more resistant to fluctuations, visa versa for a high value. A good starting point is 5 to 10.")]
		[SerializeField]
		private float responsiveness = 5f;

		// Token: 0x04008BF9 RID: 35833
		[Tooltip("If true (default behavior), the input value will be used directly. Disable this if you need better control over how smoothly the properties get applied.")]
		[SerializeField]
		private bool instant = true;

		// Token: 0x04008BFA RID: 35834
		[SerializeField]
		private ContinuousProperty[] list;

		// Token: 0x04008BFB RID: 35835
		private List<int> uniqueShaderPropertyIndices;

		// Token: 0x04008BFC RID: 35836
		private MaterialPropertyBlock mpb;

		// Token: 0x04008BFD RID: 35837
		private bool initialized;

		// Token: 0x04008BFE RID: 35838
		private float value;

		// Token: 0x04008BFF RID: 35839
		private float lastApplyTime;

		// Token: 0x04008C00 RID: 35840
		[NonSerialized]
		public bool cachedRigIsLocal;

		// Token: 0x0200130E RID: 4878
		private class PropertyComparer : IComparer<ContinuousProperty>
		{
			// Token: 0x06007A6F RID: 31343 RVA: 0x0027F2DC File Offset: 0x0027D4DC
			public int Compare(ContinuousProperty x, ContinuousProperty y)
			{
				if (!x.IsShaderProperty_Cached || !y.IsShaderProperty_Cached)
				{
					return y.IsShaderProperty_Cached.CompareTo(x.IsShaderProperty_Cached);
				}
				int num = x.GetTargetInstanceID() ^ x.IntValue;
				int num2 = y.GetTargetInstanceID() ^ y.IntValue;
				return num.CompareTo(num2);
			}
		}
	}
}
