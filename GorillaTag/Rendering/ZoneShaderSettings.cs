using System;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x020012C0 RID: 4800
	public class ZoneShaderSettings : MonoBehaviour, ITickSystemPost
	{
		// Token: 0x17000BB4 RID: 2996
		// (get) Token: 0x0600786D RID: 30829 RVA: 0x002708C3 File Offset: 0x0026EAC3
		// (set) Token: 0x0600786E RID: 30830 RVA: 0x002708CA File Offset: 0x0026EACA
		[DebugReadout]
		public static ZoneShaderSettings defaultsInstance { get; private set; }

		// Token: 0x17000BB5 RID: 2997
		// (get) Token: 0x0600786F RID: 30831 RVA: 0x002708D2 File Offset: 0x0026EAD2
		// (set) Token: 0x06007870 RID: 30832 RVA: 0x002708D9 File Offset: 0x0026EAD9
		public static bool hasDefaultsInstance { get; private set; }

		// Token: 0x17000BB6 RID: 2998
		// (get) Token: 0x06007871 RID: 30833 RVA: 0x002708E1 File Offset: 0x0026EAE1
		// (set) Token: 0x06007872 RID: 30834 RVA: 0x002708E8 File Offset: 0x0026EAE8
		[DebugReadout]
		public static ZoneShaderSettings activeInstance { get; private set; }

		// Token: 0x17000BB7 RID: 2999
		// (get) Token: 0x06007873 RID: 30835 RVA: 0x002708F0 File Offset: 0x0026EAF0
		// (set) Token: 0x06007874 RID: 30836 RVA: 0x002708F7 File Offset: 0x0026EAF7
		public static bool hasActiveInstance { get; private set; }

		// Token: 0x17000BB8 RID: 3000
		// (get) Token: 0x06007875 RID: 30837 RVA: 0x002708FF File Offset: 0x0026EAFF
		public bool isActiveInstance
		{
			get
			{
				return ZoneShaderSettings.activeInstance == this;
			}
		}

		// Token: 0x17000BB9 RID: 3001
		// (get) Token: 0x06007876 RID: 30838 RVA: 0x0027090C File Offset: 0x0026EB0C
		[DebugReadout]
		private float GroundFogDepthFadeSq
		{
			get
			{
				return 1f / Mathf.Max(1E-05f, this._groundFogDepthFadeSize * this._groundFogDepthFadeSize);
			}
		}

		// Token: 0x17000BBA RID: 3002
		// (get) Token: 0x06007877 RID: 30839 RVA: 0x0027092B File Offset: 0x0026EB2B
		[DebugReadout]
		private float GroundFogHeightFade
		{
			get
			{
				return 1f / Mathf.Max(1E-05f, this._groundFogHeightFadeSize);
			}
		}

		// Token: 0x06007878 RID: 30840 RVA: 0x00270944 File Offset: 0x0026EB44
		public void SetZoneLiquidTypeKeywordEnum(ZoneShaderSettings.EZoneLiquidType liquidType)
		{
			if (liquidType == ZoneShaderSettings.EZoneLiquidType.None)
			{
				Shader.EnableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__NONE");
			}
			else
			{
				Shader.DisableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__NONE");
			}
			if (liquidType == ZoneShaderSettings.EZoneLiquidType.Water)
			{
				Shader.EnableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
			}
			else
			{
				Shader.DisableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
			}
			if (liquidType == ZoneShaderSettings.EZoneLiquidType.Lava)
			{
				Shader.EnableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
				return;
			}
			Shader.DisableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
		}

		// Token: 0x06007879 RID: 30841 RVA: 0x0027099D File Offset: 0x0026EB9D
		public void SetZoneLiquidShapeKeywordEnum(ZoneShaderSettings.ELiquidShape shape)
		{
			if (shape == ZoneShaderSettings.ELiquidShape.Plane)
			{
				Shader.EnableKeyword("_ZONE_LIQUID_SHAPE__PLANE");
			}
			else
			{
				Shader.DisableKeyword("_ZONE_LIQUID_SHAPE__PLANE");
			}
			if (shape == ZoneShaderSettings.ELiquidShape.Cylinder)
			{
				Shader.EnableKeyword("_ZONE_LIQUID_SHAPE__CYLINDER");
				return;
			}
			Shader.DisableKeyword("_ZONE_LIQUID_SHAPE__CYLINDER");
		}

		// Token: 0x17000BBB RID: 3003
		// (get) Token: 0x0600787A RID: 30842 RVA: 0x002709D1 File Offset: 0x0026EBD1
		// (set) Token: 0x0600787B RID: 30843 RVA: 0x002709D8 File Offset: 0x0026EBD8
		public static int shaderParam_ZoneLiquidPosRadiusSq { get; private set; } = Shader.PropertyToID("_ZoneLiquidPosRadiusSq");

		// Token: 0x0600787C RID: 30844 RVA: 0x002709E0 File Offset: 0x0026EBE0
		public static float GetWaterY()
		{
			return ZoneShaderSettings.activeInstance.mainWaterSurfacePlane.position.y;
		}

		// Token: 0x0600787D RID: 30845 RVA: 0x002709F8 File Offset: 0x0026EBF8
		protected void Awake()
		{
			this.hasMainWaterSurfacePlane = this.mainWaterSurfacePlane != null && (this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues);
			this.hasDynamicWaterSurfacePlane = this.hasMainWaterSurfacePlane && !this.mainWaterSurfacePlane.gameObject.isStatic;
			this.hasLiquidBottomTransform = this.liquidBottomTransform != null && (this.liquidBottomTransform_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues);
			if (!this.CheckDefaultsInstance())
			{
				return;
			}
			if (this._activateOnAwake)
			{
				this.BecomeActiveInstance(false);
			}
		}

		// Token: 0x0600787E RID: 30846 RVA: 0x00270A93 File Offset: 0x0026EC93
		protected void OnEnable()
		{
			if (this.hasDynamicWaterSurfacePlane)
			{
				TickSystem<object>.AddPostTickCallback(this);
			}
		}

		// Token: 0x0600787F RID: 30847 RVA: 0x0015AB83 File Offset: 0x00158D83
		protected void OnDisable()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x06007880 RID: 30848 RVA: 0x00270AA3 File Offset: 0x0026ECA3
		protected void OnDestroy()
		{
			if (ZoneShaderSettings.defaultsInstance == this)
			{
				ZoneShaderSettings.hasDefaultsInstance = false;
			}
			if (ZoneShaderSettings.activeInstance == this)
			{
				ZoneShaderSettings.hasActiveInstance = false;
			}
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x17000BBC RID: 3004
		// (get) Token: 0x06007881 RID: 30849 RVA: 0x00270AD1 File Offset: 0x0026ECD1
		// (set) Token: 0x06007882 RID: 30850 RVA: 0x00270AD9 File Offset: 0x0026ECD9
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x06007883 RID: 30851 RVA: 0x00270AE2 File Offset: 0x0026ECE2
		void ITickSystemPost.PostTick()
		{
			if (ZoneShaderSettings.activeInstance == this && Application.isPlaying && !ApplicationQuittingState.IsQuitting)
			{
				this.UpdateMainPlaneShaderProperty();
			}
		}

		// Token: 0x06007884 RID: 30852 RVA: 0x00270B08 File Offset: 0x0026ED08
		private void UpdateMainPlaneShaderProperty()
		{
			Transform transform = null;
			bool flag = false;
			if (this.hasMainWaterSurfacePlane && (this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues))
			{
				flag = true;
				transform = this.mainWaterSurfacePlane;
			}
			else if (this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue && ZoneShaderSettings.hasDefaultsInstance && ZoneShaderSettings.defaultsInstance.hasMainWaterSurfacePlane)
			{
				flag = true;
				transform = ZoneShaderSettings.defaultsInstance.mainWaterSurfacePlane;
			}
			if (!flag)
			{
				return;
			}
			Vector3 position = transform.position;
			Vector3 up = transform.up;
			float num = -Vector3.Dot(up, position);
			Shader.SetGlobalVector(this.shaderParam_GlobalMainWaterSurfacePlane, new Vector4(up.x, up.y, up.z, num));
			ZoneShaderSettings.ELiquidShape eliquidShape;
			if (this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				eliquidShape = this.liquidShape;
			}
			else if (this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue && ZoneShaderSettings.hasDefaultsInstance)
			{
				eliquidShape = ZoneShaderSettings.defaultsInstance.liquidShape;
			}
			else
			{
				eliquidShape = ZoneShaderSettings.liquidShape_previousValue;
			}
			ZoneShaderSettings.liquidShape_previousValue = eliquidShape;
			float num2;
			if ((this.liquidBottomTransform_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues) && this.hasLiquidBottomTransform)
			{
				num2 = this.liquidBottomTransform.position.y;
			}
			else if (this.liquidBottomTransform_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue && ZoneShaderSettings.hasDefaultsInstance && ZoneShaderSettings.defaultsInstance.hasLiquidBottomTransform)
			{
				num2 = ZoneShaderSettings.defaultsInstance.liquidBottomTransform.position.y;
			}
			else
			{
				num2 = this.liquidBottomPosY_previousValue;
			}
			float num3;
			if (this.liquidShapeRadius_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				num3 = this.liquidShapeRadius;
			}
			else if (this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue && ZoneShaderSettings.hasDefaultsInstance)
			{
				num3 = ZoneShaderSettings.defaultsInstance.liquidShapeRadius;
			}
			else
			{
				num3 = ZoneShaderSettings.liquidShapeRadius_previousValue;
			}
			if (eliquidShape == ZoneShaderSettings.ELiquidShape.Cylinder)
			{
				Shader.SetGlobalVector(ZoneShaderSettings.shaderParam_ZoneLiquidPosRadiusSq, new Vector4(position.x, num2, position.z, num3 * num3));
				ZoneShaderSettings.liquidShapeRadius_previousValue = num3;
			}
		}

		// Token: 0x06007885 RID: 30853 RVA: 0x00270CC4 File Offset: 0x0026EEC4
		private bool CheckDefaultsInstance()
		{
			if (!this.isDefaultValues)
			{
				return true;
			}
			if (ZoneShaderSettings.hasDefaultsInstance && ZoneShaderSettings.defaultsInstance != null && ZoneShaderSettings.defaultsInstance != this)
			{
				string path = ZoneShaderSettings.defaultsInstance.transform.GetPath();
				Debug.LogError(string.Concat(new string[]
				{
					"ZoneShaderSettings: Destroying conflicting defaults instance.\n- keeping: \"",
					path,
					"\"\n- destroying (this): \"",
					base.transform.GetPath(),
					"\""
				}), this);
				Object.Destroy(base.gameObject);
				return false;
			}
			ZoneShaderSettings.defaultsInstance = this;
			ZoneShaderSettings.hasDefaultsInstance = true;
			this.BecomeActiveInstance(false);
			return true;
		}

		// Token: 0x06007886 RID: 30854 RVA: 0x00270D68 File Offset: 0x0026EF68
		public void BecomeActiveInstance(bool force = false)
		{
			if (ZoneShaderSettings.activeInstance == this && !force)
			{
				return;
			}
			if (ZoneShaderSettings.activeInstance.IsNotNull())
			{
				TickSystem<object>.RemovePostTickCallback(ZoneShaderSettings.activeInstance);
			}
			if (this.hasDynamicWaterSurfacePlane)
			{
				TickSystem<object>.AddPostTickCallback(this);
			}
			this.ApplyValues();
			ZoneShaderSettings.activeInstance = this;
			ZoneShaderSettings.hasActiveInstance = true;
		}

		// Token: 0x06007887 RID: 30855 RVA: 0x00270DBC File Offset: 0x0026EFBC
		public static void ActivateDefaultSettings()
		{
			if (ZoneShaderSettings.hasDefaultsInstance)
			{
				ZoneShaderSettings.defaultsInstance.BecomeActiveInstance(false);
			}
		}

		// Token: 0x06007888 RID: 30856 RVA: 0x00270DD0 File Offset: 0x0026EFD0
		public void SetGroundFogValue(Color fogColor, float fogDepthFade, float fogHeight, float fogHeightFade)
		{
			this.groundFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
			this.groundFogColor = fogColor;
			this.groundFogDepthFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
			this._groundFogDepthFadeSize = fogDepthFade;
			this.groundFogHeight_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
			this.groundFogHeight = fogHeight;
			this.groundFogHeightFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
			this._groundFogHeightFadeSize = fogHeightFade;
			this.BecomeActiveInstance(true);
		}

		// Token: 0x06007889 RID: 30857 RVA: 0x00270E20 File Offset: 0x0026F020
		private void ApplyValues()
		{
			if (!ZoneShaderSettings.hasDefaultsInstance || ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			this.ApplyColor(ZoneShaderSettings.groundFogColor_shaderProp, this.groundFogColor_overrideMode, this.groundFogColor, ZoneShaderSettings.defaultsInstance.groundFogColor);
			this.ApplyFloat(ZoneShaderSettings.groundFogDepthFadeSq_shaderProp, this.groundFogDepthFade_overrideMode, this.GroundFogDepthFadeSq, ZoneShaderSettings.defaultsInstance.GroundFogDepthFadeSq);
			this.ApplyFloat(ZoneShaderSettings.groundFogHeight_shaderProp, this.groundFogHeight_overrideMode, this.groundFogHeight, ZoneShaderSettings.defaultsInstance.groundFogHeight);
			this.ApplyFloat(ZoneShaderSettings.groundFogHeightFade_shaderProp, this.groundFogHeightFade_overrideMode, this.GroundFogHeightFade, ZoneShaderSettings.defaultsInstance.GroundFogHeightFade);
			if (this.zoneLiquidType_overrideMode != ZoneShaderSettings.EOverrideMode.LeaveUnchanged)
			{
				ZoneShaderSettings.EZoneLiquidType ezoneLiquidType = ((this.zoneLiquidType_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue) ? this.zoneLiquidType : ZoneShaderSettings.defaultsInstance.zoneLiquidType);
				if (ezoneLiquidType != ZoneShaderSettings.liquidType_previousValue || !ZoneShaderSettings.isInitialized)
				{
					this.SetZoneLiquidTypeKeywordEnum(ezoneLiquidType);
					ZoneShaderSettings.liquidType_previousValue = ezoneLiquidType;
				}
			}
			if (this.liquidShape_overrideMode != ZoneShaderSettings.EOverrideMode.LeaveUnchanged)
			{
				ZoneShaderSettings.ELiquidShape eliquidShape = ((this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue) ? this.liquidShape : ZoneShaderSettings.defaultsInstance.liquidShape);
				if (eliquidShape != ZoneShaderSettings.liquidShape_previousValue || !ZoneShaderSettings.isInitialized)
				{
					this.SetZoneLiquidShapeKeywordEnum(eliquidShape);
					ZoneShaderSettings.liquidShape_previousValue = eliquidShape;
				}
			}
			this.ApplyFloat(ZoneShaderSettings.shaderParam_GlobalZoneLiquidUVScale, this.zoneLiquidUVScale_overrideMode, this.zoneLiquidUVScale, ZoneShaderSettings.defaultsInstance.zoneLiquidUVScale);
			this.ApplyColor(ZoneShaderSettings.shaderParam_GlobalWaterTintColor, this.underwaterTintColor_overrideMode, this.underwaterTintColor, ZoneShaderSettings.defaultsInstance.underwaterTintColor);
			this.ApplyColor(ZoneShaderSettings.shaderParam_GlobalUnderwaterFogColor, this.underwaterFogColor_overrideMode, this.underwaterFogColor, ZoneShaderSettings.defaultsInstance.underwaterFogColor);
			this.ApplyVector(ZoneShaderSettings.shaderParam_GlobalUnderwaterFogParams, this.underwaterFogParams_overrideMode, this.underwaterFogParams, ZoneShaderSettings.defaultsInstance.underwaterFogParams);
			this.ApplyVector(ZoneShaderSettings.shaderParam_GlobalUnderwaterCausticsParams, this.underwaterCausticsParams_overrideMode, this.underwaterCausticsParams, ZoneShaderSettings.defaultsInstance.underwaterCausticsParams);
			this.ApplyTexture(ZoneShaderSettings.shaderParam_GlobalUnderwaterCausticsTex, this.underwaterCausticsTexture_overrideMode, this.underwaterCausticsTexture, ZoneShaderSettings.defaultsInstance.underwaterCausticsTexture);
			this.ApplyVector(ZoneShaderSettings.shaderParam_GlobalUnderwaterEffectsDistanceToSurfaceFade, this.underwaterEffectsDistanceToSurfaceFade_overrideMode, this.underwaterEffectsDistanceToSurfaceFade, ZoneShaderSettings.defaultsInstance.underwaterEffectsDistanceToSurfaceFade);
			this.ApplyTexture(ZoneShaderSettings.shaderParam_GlobalLiquidResidueTex, this.liquidResidueTex_overrideMode, this.liquidResidueTex, ZoneShaderSettings.defaultsInstance.liquidResidueTex);
			this.ApplyFloat(ZoneShaderSettings.shaderParam_ZoneWeatherMapDissolveProgress, this.zoneWeatherMapDissolveProgress_overrideMode, this.zoneWeatherMapDissolveProgress, ZoneShaderSettings.defaultsInstance.zoneWeatherMapDissolveProgress);
			this.UpdateMainPlaneShaderProperty();
			ZoneShaderSettings.isInitialized = true;
		}

		// Token: 0x0600788A RID: 30858 RVA: 0x00271075 File Offset: 0x0026F275
		private void ApplyColor(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, Color value, Color defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalColor(shaderProp, value.linear);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalColor(shaderProp, defaultValue.linear);
			}
		}

		// Token: 0x0600788B RID: 30859 RVA: 0x002710A2 File Offset: 0x0026F2A2
		private void ApplyFloat(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, float value, float defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalFloat(shaderProp, value);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalFloat(shaderProp, defaultValue);
			}
		}

		// Token: 0x0600788C RID: 30860 RVA: 0x002710C4 File Offset: 0x0026F2C4
		private void ApplyVector(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, Vector2 value, Vector2 defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalVector(shaderProp, value);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(shaderProp, defaultValue);
			}
		}

		// Token: 0x0600788D RID: 30861 RVA: 0x002710F0 File Offset: 0x0026F2F0
		private void ApplyVector(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, Vector3 value, Vector3 defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalVector(shaderProp, value);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(shaderProp, defaultValue);
			}
		}

		// Token: 0x0600788E RID: 30862 RVA: 0x0027111C File Offset: 0x0026F31C
		private void ApplyVector(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, Vector4 value, Vector4 defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalVector(shaderProp, value);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(shaderProp, defaultValue);
			}
		}

		// Token: 0x0600788F RID: 30863 RVA: 0x0027113E File Offset: 0x0026F33E
		private void ApplyTexture(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, Texture2D value, Texture2D defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalTexture(shaderProp, value);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalTexture(shaderProp, defaultValue);
			}
		}

		// Token: 0x06007890 RID: 30864 RVA: 0x00271160 File Offset: 0x0026F360
		public void CopySettings(CMSZoneShaderSettings cmsZoneShaderSettings, bool rerunAwake = false)
		{
			this._activateOnAwake = cmsZoneShaderSettings.activateOnLoad;
			if (cmsZoneShaderSettings.applyGroundFog)
			{
				this.groundFogColor_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetGroundFogColorOverrideMode();
				this.groundFogColor = cmsZoneShaderSettings.groundFogColor;
				this.groundFogHeight_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetGroundFogHeightOverrideMode();
				if (cmsZoneShaderSettings.groundFogHeightPlane.IsNotNull())
				{
					this.groundFogHeight = cmsZoneShaderSettings.groundFogHeightPlane.position.y;
				}
				else
				{
					this.groundFogHeight = cmsZoneShaderSettings.groundFogHeight;
				}
				this.groundFogHeightFade_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetGroundFogHeightFadeOverrideMode();
				this._groundFogHeightFadeSize = cmsZoneShaderSettings.groundFogHeightFadeSize;
				this.groundFogDepthFade_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetGroundFogDepthFadeOverrideMode();
				this._groundFogDepthFadeSize = cmsZoneShaderSettings.groundFogDepthFadeSize;
			}
			else
			{
				this.groundFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.groundFogColor = new Color(0f, 0f, 0f, 0f);
				this.groundFogHeight = -9999f;
			}
			if (cmsZoneShaderSettings.applyLiquidEffects)
			{
				this.zoneLiquidType_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetZoneLiquidTypeOverrideMode();
				this.zoneLiquidType = (ZoneShaderSettings.EZoneLiquidType)cmsZoneShaderSettings.GetZoneLiquidType();
				this.liquidShape_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetLiquidShapeOverrideMode();
				this.liquidShape = (ZoneShaderSettings.ELiquidShape)cmsZoneShaderSettings.GetZoneLiquidShape();
				this.liquidShapeRadius_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetLiquidShapeRadiusOverrideMode();
				this.liquidShapeRadius = cmsZoneShaderSettings.liquidShapeRadius;
				this.liquidBottomTransform_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetLiquidBottomTransformOverrideMode();
				this.liquidBottomTransform = cmsZoneShaderSettings.liquidBottomTransform;
				this.zoneLiquidUVScale_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetZoneLiquidUVScaleOverrideMode();
				this.zoneLiquidUVScale = cmsZoneShaderSettings.zoneLiquidUVScale;
				this.underwaterTintColor_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterTintColorOverrideMode();
				this.underwaterTintColor = cmsZoneShaderSettings.underwaterTintColor;
				this.underwaterFogColor_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterFogColorOverrideMode();
				this.underwaterFogColor = cmsZoneShaderSettings.underwaterFogColor;
				this.underwaterFogParams_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterFogParamsOverrideMode();
				this.underwaterFogParams = cmsZoneShaderSettings.underwaterFogParams;
				this.underwaterCausticsParams_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterCausticsParamsOverrideMode();
				this.underwaterCausticsParams = cmsZoneShaderSettings.underwaterCausticsParams;
				this.underwaterCausticsTexture_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterCausticsTextureOverrideMode();
				this.underwaterCausticsTexture = cmsZoneShaderSettings.underwaterCausticsTexture;
				this.underwaterEffectsDistanceToSurfaceFade_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterEffectsDistanceToSurfaceFadeOverrideMode();
				this.underwaterEffectsDistanceToSurfaceFade = cmsZoneShaderSettings.underwaterEffectsDistanceToSurfaceFade;
				this.liquidResidueTex_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetLiquidResidueTextureOverrideMode();
				this.liquidResidueTex = cmsZoneShaderSettings.liquidResidueTex;
				this.mainWaterSurfacePlane_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetMainWaterSurfacePlaneOverrideMode();
				this.mainWaterSurfacePlane = cmsZoneShaderSettings.mainWaterSurfacePlane;
			}
			else
			{
				this.underwaterTintColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterTintColor = new Color(0f, 0f, 0f, 0f);
				this.underwaterFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterFogColor = new Color(0f, 0f, 0f, 0f);
				this.mainWaterSurfacePlane_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				Transform transform = base.gameObject.transform.Find("DummyWaterPlane");
				GameObject gameObject;
				if (transform != null)
				{
					gameObject = transform.gameObject;
				}
				else
				{
					gameObject = new GameObject("DummyWaterPlane");
					gameObject.transform.SetParent(base.gameObject.transform);
					gameObject.transform.rotation = Quaternion.identity;
					gameObject.transform.position = new Vector3(0f, -9999f, 0f);
				}
				this.mainWaterSurfacePlane = gameObject.transform;
			}
			this.zoneWeatherMapDissolveProgress_overrideMode = ZoneShaderSettings.EOverrideMode.LeaveUnchanged;
			if (rerunAwake)
			{
				this.Awake();
			}
		}

		// Token: 0x06007891 RID: 30865 RVA: 0x00271470 File Offset: 0x0026F670
		public void CopySettings(ZoneShaderSettings zoneShaderSettings, bool rerunAwake = false)
		{
			this._activateOnAwake = zoneShaderSettings._activateOnAwake;
			this.groundFogColor_overrideMode = zoneShaderSettings.groundFogColor_overrideMode;
			this.groundFogColor = zoneShaderSettings.groundFogColor;
			this.groundFogHeight_overrideMode = zoneShaderSettings.groundFogHeight_overrideMode;
			this.groundFogHeight = zoneShaderSettings.groundFogHeight;
			this.groundFogHeightFade_overrideMode = zoneShaderSettings.groundFogHeightFade_overrideMode;
			this._groundFogHeightFadeSize = zoneShaderSettings._groundFogHeightFadeSize;
			this.groundFogDepthFade_overrideMode = zoneShaderSettings.groundFogDepthFade_overrideMode;
			this._groundFogDepthFadeSize = zoneShaderSettings._groundFogDepthFadeSize;
			this.zoneLiquidType_overrideMode = zoneShaderSettings.zoneLiquidType_overrideMode;
			this.zoneLiquidType = zoneShaderSettings.zoneLiquidType;
			this.liquidShape_overrideMode = zoneShaderSettings.liquidShape_overrideMode;
			this.liquidShape = zoneShaderSettings.liquidShape;
			this.liquidShapeRadius_overrideMode = zoneShaderSettings.liquidShapeRadius_overrideMode;
			this.liquidShapeRadius = zoneShaderSettings.liquidShapeRadius;
			this.liquidBottomTransform_overrideMode = zoneShaderSettings.liquidBottomTransform_overrideMode;
			this.liquidBottomTransform = zoneShaderSettings.liquidBottomTransform;
			this.zoneLiquidUVScale_overrideMode = zoneShaderSettings.zoneLiquidUVScale_overrideMode;
			this.zoneLiquidUVScale = zoneShaderSettings.zoneLiquidUVScale;
			this.underwaterTintColor_overrideMode = zoneShaderSettings.underwaterTintColor_overrideMode;
			this.underwaterTintColor = zoneShaderSettings.underwaterTintColor;
			this.underwaterFogColor_overrideMode = zoneShaderSettings.underwaterFogColor_overrideMode;
			this.underwaterFogColor = zoneShaderSettings.underwaterFogColor;
			this.underwaterFogParams_overrideMode = zoneShaderSettings.underwaterFogParams_overrideMode;
			this.underwaterFogParams = zoneShaderSettings.underwaterFogParams;
			this.underwaterCausticsParams_overrideMode = zoneShaderSettings.underwaterCausticsParams_overrideMode;
			this.underwaterCausticsParams = zoneShaderSettings.underwaterCausticsParams;
			this.underwaterCausticsTexture_overrideMode = zoneShaderSettings.underwaterCausticsTexture_overrideMode;
			this.underwaterCausticsTexture = zoneShaderSettings.underwaterCausticsTexture;
			this.underwaterEffectsDistanceToSurfaceFade_overrideMode = zoneShaderSettings.underwaterEffectsDistanceToSurfaceFade_overrideMode;
			this.underwaterEffectsDistanceToSurfaceFade = zoneShaderSettings.underwaterEffectsDistanceToSurfaceFade;
			this.liquidResidueTex_overrideMode = zoneShaderSettings.liquidResidueTex_overrideMode;
			this.liquidResidueTex = zoneShaderSettings.liquidResidueTex;
			this.mainWaterSurfacePlane_overrideMode = zoneShaderSettings.mainWaterSurfacePlane_overrideMode;
			this.mainWaterSurfacePlane = zoneShaderSettings.mainWaterSurfacePlane;
			this.zoneWeatherMapDissolveProgress_overrideMode = zoneShaderSettings.zoneWeatherMapDissolveProgress_overrideMode;
			this.zoneWeatherMapDissolveProgress = zoneShaderSettings.zoneWeatherMapDissolveProgress;
			if (rerunAwake)
			{
				this.Awake();
			}
		}

		// Token: 0x06007892 RID: 30866 RVA: 0x00271644 File Offset: 0x0026F844
		public void ReplaceDefaultValues(ZoneShaderSettings defaultZoneShaderSettings, bool rerunAwake = false)
		{
			if (this.groundFogColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.groundFogColor = defaultZoneShaderSettings.groundFogColor;
			}
			if (this.groundFogHeight_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogHeight_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.groundFogHeight = defaultZoneShaderSettings.groundFogHeight;
			}
			if (this.groundFogHeightFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogHeightFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this._groundFogHeightFadeSize = defaultZoneShaderSettings._groundFogHeightFadeSize;
			}
			if (this.groundFogDepthFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogDepthFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this._groundFogDepthFadeSize = defaultZoneShaderSettings._groundFogDepthFadeSize;
			}
			if (this.zoneLiquidType_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.zoneLiquidType_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.zoneLiquidType = defaultZoneShaderSettings.zoneLiquidType;
			}
			if (this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidShape_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidShape = defaultZoneShaderSettings.liquidShape;
			}
			if (this.liquidShapeRadius_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidShapeRadius_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidShapeRadius = defaultZoneShaderSettings.liquidShapeRadius;
			}
			if (this.liquidBottomTransform_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidBottomTransform_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidBottomTransform = defaultZoneShaderSettings.liquidBottomTransform;
			}
			if (this.zoneLiquidUVScale_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.zoneLiquidUVScale_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.zoneLiquidUVScale = defaultZoneShaderSettings.zoneLiquidUVScale;
			}
			if (this.underwaterTintColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterTintColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterTintColor = defaultZoneShaderSettings.underwaterTintColor;
			}
			if (this.underwaterFogColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterFogColor = defaultZoneShaderSettings.underwaterFogColor;
			}
			if (this.underwaterFogParams_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterFogParams_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterFogParams = defaultZoneShaderSettings.underwaterFogParams;
			}
			if (this.underwaterCausticsParams_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterCausticsParams_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterCausticsParams = defaultZoneShaderSettings.underwaterCausticsParams;
			}
			if (this.underwaterCausticsTexture_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterCausticsTexture_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterCausticsTexture = defaultZoneShaderSettings.underwaterCausticsTexture;
			}
			if (this.underwaterEffectsDistanceToSurfaceFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterEffectsDistanceToSurfaceFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterEffectsDistanceToSurfaceFade = defaultZoneShaderSettings.underwaterEffectsDistanceToSurfaceFade;
			}
			if (this.liquidResidueTex_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidResidueTex_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidResidueTex = defaultZoneShaderSettings.liquidResidueTex;
			}
			if (this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.mainWaterSurfacePlane_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.mainWaterSurfacePlane = defaultZoneShaderSettings.mainWaterSurfacePlane;
			}
			if (rerunAwake)
			{
				this.Awake();
			}
		}

		// Token: 0x06007893 RID: 30867 RVA: 0x00271838 File Offset: 0x0026FA38
		public void ReplaceDefaultValues(CMSZoneShaderSettings.CMSZoneShaderProperties defaultZoneShaderProperties, bool rerunAwake = false)
		{
			if (this.groundFogColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.groundFogColor = defaultZoneShaderProperties.groundFogColor;
			}
			if (this.groundFogHeight_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogHeight_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.groundFogHeight = defaultZoneShaderProperties.groundFogHeight;
			}
			if (this.groundFogHeightFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogHeightFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this._groundFogHeightFadeSize = defaultZoneShaderProperties.groundFogHeightFadeSize;
			}
			if (this.groundFogDepthFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogDepthFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this._groundFogDepthFadeSize = defaultZoneShaderProperties.groundFogDepthFadeSize;
			}
			if (this.zoneLiquidType_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.zoneLiquidType_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.zoneLiquidType = (ZoneShaderSettings.EZoneLiquidType)defaultZoneShaderProperties.zoneLiquidType;
			}
			if (this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidShape_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidShape = (ZoneShaderSettings.ELiquidShape)defaultZoneShaderProperties.liquidShape;
			}
			if (this.liquidShapeRadius_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidShapeRadius_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidShapeRadius = defaultZoneShaderProperties.liquidShapeRadius;
			}
			if (this.liquidBottomTransform_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidBottomTransform_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidBottomTransform = defaultZoneShaderProperties.liquidBottomTransform;
			}
			if (this.zoneLiquidUVScale_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.zoneLiquidUVScale_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.zoneLiquidUVScale = defaultZoneShaderProperties.zoneLiquidUVScale;
			}
			if (this.underwaterTintColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterTintColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterTintColor = defaultZoneShaderProperties.underwaterTintColor;
			}
			if (this.underwaterFogColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterFogColor = defaultZoneShaderProperties.underwaterFogColor;
			}
			if (this.underwaterFogParams_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterFogParams_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterFogParams = defaultZoneShaderProperties.underwaterFogParams;
			}
			if (this.underwaterCausticsParams_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterCausticsParams_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterCausticsParams = defaultZoneShaderProperties.underwaterCausticsParams;
			}
			if (this.underwaterCausticsTexture_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterCausticsTexture_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterCausticsTexture = defaultZoneShaderProperties.underwaterCausticsTexture;
			}
			if (this.underwaterEffectsDistanceToSurfaceFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterEffectsDistanceToSurfaceFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterEffectsDistanceToSurfaceFade = defaultZoneShaderProperties.underwaterEffectsDistanceToSurfaceFade;
			}
			if (this.liquidResidueTex_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidResidueTex_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidResidueTex = defaultZoneShaderProperties.liquidResidueTex;
			}
			if (this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.mainWaterSurfacePlane_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.mainWaterSurfacePlane = defaultZoneShaderProperties.mainWaterSurfacePlane;
			}
			if (rerunAwake)
			{
				this.Awake();
			}
		}

		// Token: 0x040088A2 RID: 34978
		[OnEnterPlay_Set(false)]
		private static bool isInitialized;

		// Token: 0x040088A7 RID: 34983
		[Tooltip("Set this to true for cases like it is the first ZoneShaderSettings that should be activated when entering a scene.")]
		[SerializeField]
		private bool _activateOnAwake;

		// Token: 0x040088A8 RID: 34984
		[Tooltip("These values will be used as the default global values that will be fallen back to when not in a zone and that the other scripts will reference.")]
		public bool isDefaultValues;

		// Token: 0x040088A9 RID: 34985
		private static readonly int groundFogColor_shaderProp = Shader.PropertyToID("_ZoneGroundFogColor");

		// Token: 0x040088AA RID: 34986
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogColor_overrideMode;

		// Token: 0x040088AB RID: 34987
		[SerializeField]
		private Color groundFogColor = new Color(0.7f, 0.9f, 1f, 1f);

		// Token: 0x040088AC RID: 34988
		private static readonly int groundFogDepthFadeSq_shaderProp = Shader.PropertyToID("_ZoneGroundFogDepthFadeSq");

		// Token: 0x040088AD RID: 34989
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogDepthFade_overrideMode;

		// Token: 0x040088AE RID: 34990
		[SerializeField]
		private float _groundFogDepthFadeSize = 20f;

		// Token: 0x040088AF RID: 34991
		private static readonly int groundFogHeight_shaderProp = Shader.PropertyToID("_ZoneGroundFogHeight");

		// Token: 0x040088B0 RID: 34992
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogHeight_overrideMode;

		// Token: 0x040088B1 RID: 34993
		[SerializeField]
		private float groundFogHeight = 7.45f;

		// Token: 0x040088B2 RID: 34994
		private static readonly int groundFogHeightFade_shaderProp = Shader.PropertyToID("_ZoneGroundFogHeightFade");

		// Token: 0x040088B3 RID: 34995
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogHeightFade_overrideMode;

		// Token: 0x040088B4 RID: 34996
		[SerializeField]
		private float _groundFogHeightFadeSize = 20f;

		// Token: 0x040088B5 RID: 34997
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode zoneLiquidType_overrideMode;

		// Token: 0x040088B6 RID: 34998
		[SerializeField]
		private ZoneShaderSettings.EZoneLiquidType zoneLiquidType = ZoneShaderSettings.EZoneLiquidType.Water;

		// Token: 0x040088B7 RID: 34999
		[OnEnterPlay_Set(ZoneShaderSettings.EZoneLiquidType.None)]
		private static ZoneShaderSettings.EZoneLiquidType liquidType_previousValue = ZoneShaderSettings.EZoneLiquidType.None;

		// Token: 0x040088B8 RID: 35000
		[OnEnterPlay_Set(false)]
		private static bool didEverSetLiquidShape;

		// Token: 0x040088B9 RID: 35001
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode liquidShape_overrideMode;

		// Token: 0x040088BA RID: 35002
		[SerializeField]
		private ZoneShaderSettings.ELiquidShape liquidShape;

		// Token: 0x040088BB RID: 35003
		[OnEnterPlay_Set(ZoneShaderSettings.ELiquidShape.Plane)]
		private static ZoneShaderSettings.ELiquidShape liquidShape_previousValue = ZoneShaderSettings.ELiquidShape.Plane;

		// Token: 0x040088BD RID: 35005
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode liquidShapeRadius_overrideMode;

		// Token: 0x040088BE RID: 35006
		[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
		[SerializeField]
		private float liquidShapeRadius = 1f;

		// Token: 0x040088BF RID: 35007
		[OnEnterPlay_Set(1f)]
		private static float liquidShapeRadius_previousValue;

		// Token: 0x040088C0 RID: 35008
		private bool hasLiquidBottomTransform;

		// Token: 0x040088C1 RID: 35009
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode liquidBottomTransform_overrideMode;

		// Token: 0x040088C2 RID: 35010
		[Tooltip("TODO: remove this when there is a way to precalculate the nearest triangle plane per vertex so it will work better for rivers.")]
		[SerializeField]
		private Transform liquidBottomTransform;

		// Token: 0x040088C3 RID: 35011
		private float liquidBottomPosY_previousValue;

		// Token: 0x040088C4 RID: 35012
		private static readonly int shaderParam_GlobalZoneLiquidUVScale = Shader.PropertyToID("_GlobalZoneLiquidUVScale");

		// Token: 0x040088C5 RID: 35013
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode zoneLiquidUVScale_overrideMode;

		// Token: 0x040088C6 RID: 35014
		[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
		[SerializeField]
		private float zoneLiquidUVScale = 1f;

		// Token: 0x040088C7 RID: 35015
		private static readonly int shaderParam_GlobalWaterTintColor = Shader.PropertyToID("_GlobalWaterTintColor");

		// Token: 0x040088C8 RID: 35016
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterTintColor_overrideMode;

		// Token: 0x040088C9 RID: 35017
		[SerializeField]
		private Color underwaterTintColor = new Color(0.3f, 0.65f, 1f, 0.2f);

		// Token: 0x040088CA RID: 35018
		private static readonly int shaderParam_GlobalUnderwaterFogColor = Shader.PropertyToID("_GlobalUnderwaterFogColor");

		// Token: 0x040088CB RID: 35019
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterFogColor_overrideMode;

		// Token: 0x040088CC RID: 35020
		[SerializeField]
		private Color underwaterFogColor = new Color(0.12f, 0.41f, 0.77f);

		// Token: 0x040088CD RID: 35021
		private static readonly int shaderParam_GlobalUnderwaterFogParams = Shader.PropertyToID("_GlobalUnderwaterFogParams");

		// Token: 0x040088CE RID: 35022
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterFogParams_overrideMode;

		// Token: 0x040088CF RID: 35023
		[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
		[SerializeField]
		private Vector4 underwaterFogParams = new Vector4(-5f, 40f, 0f, 0f);

		// Token: 0x040088D0 RID: 35024
		private static readonly int shaderParam_GlobalUnderwaterCausticsParams = Shader.PropertyToID("_GlobalUnderwaterCausticsParams");

		// Token: 0x040088D1 RID: 35025
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterCausticsParams_overrideMode;

		// Token: 0x040088D2 RID: 35026
		[Tooltip("Caustics params are: speed1, scale, alpha, unused")]
		[SerializeField]
		private Vector4 underwaterCausticsParams = new Vector4(0.075f, 0.075f, 1f, 0f);

		// Token: 0x040088D3 RID: 35027
		private static readonly int shaderParam_GlobalUnderwaterCausticsTex = Shader.PropertyToID("_GlobalUnderwaterCausticsTex");

		// Token: 0x040088D4 RID: 35028
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterCausticsTexture_overrideMode;

		// Token: 0x040088D5 RID: 35029
		[SerializeField]
		private Texture2D underwaterCausticsTexture;

		// Token: 0x040088D6 RID: 35030
		private static readonly int shaderParam_GlobalUnderwaterEffectsDistanceToSurfaceFade = Shader.PropertyToID("_GlobalUnderwaterEffectsDistanceToSurfaceFade");

		// Token: 0x040088D7 RID: 35031
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterEffectsDistanceToSurfaceFade_overrideMode;

		// Token: 0x040088D8 RID: 35032
		[SerializeField]
		private Vector2 underwaterEffectsDistanceToSurfaceFade = new Vector2(0.0001f, 50f);

		// Token: 0x040088D9 RID: 35033
		private const string kEdTooltip_liquidResidueTex = "This is used for things like the charred surface effect when lava burns static geo.";

		// Token: 0x040088DA RID: 35034
		private static readonly int shaderParam_GlobalLiquidResidueTex = Shader.PropertyToID("_GlobalLiquidResidueTex");

		// Token: 0x040088DB RID: 35035
		[SerializeField]
		[Tooltip("This is used for things like the charred surface effect when lava burns static geo.")]
		private ZoneShaderSettings.EOverrideMode liquidResidueTex_overrideMode;

		// Token: 0x040088DC RID: 35036
		[SerializeField]
		[Tooltip("This is used for things like the charred surface effect when lava burns static geo.")]
		private Texture2D liquidResidueTex;

		// Token: 0x040088DD RID: 35037
		private readonly int shaderParam_GlobalMainWaterSurfacePlane = Shader.PropertyToID("_GlobalMainWaterSurfacePlane");

		// Token: 0x040088DE RID: 35038
		private bool hasMainWaterSurfacePlane;

		// Token: 0x040088DF RID: 35039
		private bool hasDynamicWaterSurfacePlane;

		// Token: 0x040088E0 RID: 35040
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode mainWaterSurfacePlane_overrideMode;

		// Token: 0x040088E1 RID: 35041
		[Tooltip("TODO: remove this when there is a way to precalculate the nearest triangle plane per vertex so it will work better for rivers.")]
		[SerializeField]
		private Transform mainWaterSurfacePlane;

		// Token: 0x040088E2 RID: 35042
		private static readonly int shaderParam_ZoneWeatherMapDissolveProgress = Shader.PropertyToID("_ZoneWeatherMapDissolveProgress");

		// Token: 0x040088E3 RID: 35043
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode zoneWeatherMapDissolveProgress_overrideMode;

		// Token: 0x040088E4 RID: 35044
		[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
		[Range(0f, 1f)]
		[SerializeField]
		private float zoneWeatherMapDissolveProgress = 1f;

		// Token: 0x020012C1 RID: 4801
		public enum EOverrideMode
		{
			// Token: 0x040088E7 RID: 35047
			LeaveUnchanged,
			// Token: 0x040088E8 RID: 35048
			ApplyNewValue,
			// Token: 0x040088E9 RID: 35049
			ApplyDefaultValue
		}

		// Token: 0x020012C2 RID: 4802
		public enum EZoneLiquidType
		{
			// Token: 0x040088EB RID: 35051
			None,
			// Token: 0x040088EC RID: 35052
			Water,
			// Token: 0x040088ED RID: 35053
			Lava
		}

		// Token: 0x020012C3 RID: 4803
		public enum ELiquidShape
		{
			// Token: 0x040088EF RID: 35055
			Plane,
			// Token: 0x040088F0 RID: 35056
			Cylinder
		}
	}
}
