using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001201 RID: 4609
	[ExecuteAlways]
	public class TextureTransitioner : MonoBehaviour, IResettableItem
	{
		// Token: 0x060074E8 RID: 29928 RVA: 0x0025F803 File Offset: 0x0025DA03
		protected void Awake()
		{
			if (Application.isPlaying || this.editorPreview)
			{
				TextureTransitionerManager.EnsureInstanceIsAvailable();
			}
			this.RefreshShaderParams();
			this.iDynamicFloat = (IDynamicFloat)this.dynamicFloatComponent;
			this.ResetToDefaultState();
		}

		// Token: 0x060074E9 RID: 29929 RVA: 0x0025F838 File Offset: 0x0025DA38
		protected void OnEnable()
		{
			TextureTransitionerManager.Register(this);
			if (Application.isPlaying && !this.remapInfo.IsValid())
			{
				Debug.LogError("Bad min/max values for remapRanges: " + this.GetComponentPath(int.MaxValue), this);
				base.enabled = false;
			}
			if (Application.isPlaying && this.textures.Length == 0)
			{
				Debug.LogError("Textures array is empty: " + this.GetComponentPath(int.MaxValue), this);
				base.enabled = false;
			}
			if (Application.isPlaying && this.iDynamicFloat == null)
			{
				if (this.dynamicFloatComponent == null)
				{
					Debug.LogError("dynamicFloatComponent cannot be null: " + this.GetComponentPath(int.MaxValue), this);
				}
				this.iDynamicFloat = (IDynamicFloat)this.dynamicFloatComponent;
				if (this.iDynamicFloat == null)
				{
					Debug.LogError("Component assigned to dynamicFloatComponent does not implement IDynamicFloat: " + this.GetComponentPath(int.MaxValue), this);
					base.enabled = false;
				}
			}
		}

		// Token: 0x060074EA RID: 29930 RVA: 0x0025F926 File Offset: 0x0025DB26
		protected void OnDisable()
		{
			TextureTransitionerManager.Unregister(this);
		}

		// Token: 0x060074EB RID: 29931 RVA: 0x0025F92E File Offset: 0x0025DB2E
		private void RefreshShaderParams()
		{
			this.texTransitionShaderParam = Shader.PropertyToID(this.texTransitionShaderParamName);
			this.tex1ShaderParam = Shader.PropertyToID(this.tex1ShaderParamName);
			this.tex2ShaderParam = Shader.PropertyToID(this.tex2ShaderParamName);
		}

		// Token: 0x060074EC RID: 29932 RVA: 0x0025F963 File Offset: 0x0025DB63
		public void ResetToDefaultState()
		{
			this.normalizedValue = 0f;
			this.transitionPercent = 0;
			this.tex1Index = 0;
			this.tex2Index = 0;
		}

		// Token: 0x04008492 RID: 33938
		public bool editorPreview;

		// Token: 0x04008493 RID: 33939
		[Tooltip("The component that will drive the texture transitions.")]
		public MonoBehaviour dynamicFloatComponent;

		// Token: 0x04008494 RID: 33940
		[Tooltip("Set these values so that after remap 0 is the first texture in the textures list and 1 is the last.")]
		public GorillaMath.RemapFloatInfo remapInfo;

		// Token: 0x04008495 RID: 33941
		public TextureTransitioner.DirectionRetentionMode directionRetentionMode;

		// Token: 0x04008496 RID: 33942
		public string texTransitionShaderParamName = "_TexTransition";

		// Token: 0x04008497 RID: 33943
		public string tex1ShaderParamName = "_MainTex";

		// Token: 0x04008498 RID: 33944
		public string tex2ShaderParamName = "_Tex2";

		// Token: 0x04008499 RID: 33945
		public Texture[] textures;

		// Token: 0x0400849A RID: 33946
		public Renderer[] renderers;

		// Token: 0x0400849B RID: 33947
		[NonSerialized]
		public IDynamicFloat iDynamicFloat;

		// Token: 0x0400849C RID: 33948
		[NonSerialized]
		public int texTransitionShaderParam;

		// Token: 0x0400849D RID: 33949
		[NonSerialized]
		public int tex1ShaderParam;

		// Token: 0x0400849E RID: 33950
		[NonSerialized]
		public int tex2ShaderParam;

		// Token: 0x0400849F RID: 33951
		[DebugReadout]
		[NonSerialized]
		public float normalizedValue;

		// Token: 0x040084A0 RID: 33952
		[DebugReadout]
		[NonSerialized]
		public int transitionPercent;

		// Token: 0x040084A1 RID: 33953
		[DebugReadout]
		[NonSerialized]
		public int tex1Index;

		// Token: 0x040084A2 RID: 33954
		[DebugReadout]
		[NonSerialized]
		public int tex2Index;

		// Token: 0x02001202 RID: 4610
		public enum DirectionRetentionMode
		{
			// Token: 0x040084A4 RID: 33956
			None,
			// Token: 0x040084A5 RID: 33957
			IncreaseOnly,
			// Token: 0x040084A6 RID: 33958
			DecreaseOnly
		}
	}
}
