using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000388 RID: 904
[StructLayout(LayoutKind.Auto)]
public struct MaterialFingerprint
{
	// Token: 0x060015FE RID: 5630 RVA: 0x00074E20 File Offset: 0x00073020
	public MaterialFingerprint(UberShaderMatUsedProps used)
	{
		Material material = used.material;
		this._TransparencyMode = MaterialFingerprint.GetMatTransparencyMode(material);
		this._Cutoff = MaterialFingerprint._Round(material.GetFloat(ShaderProps._Cutoff), 100, used._Cutoff);
		this._ColorSource = ((used._ColorSource > 0) ? material.GetInt(ShaderProps._ColorSource) : 0);
		this._BaseColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._BaseColor), 100, used._BaseColor);
		this._GChannelColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._GChannelColor), 100, used._GChannelColor);
		this._BChannelColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._BChannelColor), 100, used._BChannelColor);
		this._AChannelColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._AChannelColor), 100, used._AChannelColor);
		this._BaseMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._BaseMap, used._BaseMap);
		this._BaseMap_ST = MaterialFingerprint._Round(material.GetVector(ShaderProps._BaseMap_ST), 100, used._BaseMap_ST);
		this._SettingsPreset = ((used._SettingsPreset > 0) ? material.GetInt(ShaderProps._SettingsPreset) : 0);
		this._AdvancedOptions = MaterialFingerprint._Round(material.GetFloat(ShaderProps._AdvancedOptions), 100, used._AdvancedOptions);
		this._TexMipBias = MaterialFingerprint._Round(material.GetFloat(ShaderProps._TexMipBias), 100, used._TexMipBias);
		this._BaseMap_WH = MaterialFingerprint._Round(material.GetVector(ShaderProps._BaseMap_WH), 100, used._BaseMap_WH);
		this._TexelSnapToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._TexelSnapToggle), 100, used._TexelSnapToggle);
		this._TexelSnap_Factor = MaterialFingerprint._Round(material.GetFloat(ShaderProps._TexelSnap_Factor), 100, used._TexelSnap_Factor);
		this._UVSource = ((used._UVSource > 0) ? material.GetInt(ShaderProps._UVSource) : 0);
		this._AlphaDetailToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._AlphaDetailToggle), 100, used._AlphaDetailToggle);
		this._AlphaDetail_ST = MaterialFingerprint._Round(material.GetVector(ShaderProps._AlphaDetail_ST), 100, used._AlphaDetail_ST);
		this._AlphaDetail_Opacity = MaterialFingerprint._Round(material.GetFloat(ShaderProps._AlphaDetail_Opacity), 100, used._AlphaDetail_Opacity);
		this._AlphaDetail_WorldSpace = MaterialFingerprint._Round(material.GetFloat(ShaderProps._AlphaDetail_WorldSpace), 100, used._AlphaDetail_WorldSpace);
		this._MaskMapToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._MaskMapToggle), 100, used._MaskMapToggle);
		this._MaskMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._MaskMap, used._MaskMap);
		this._MaskMap_ST = MaterialFingerprint._Round(material.GetVector(ShaderProps._MaskMap_ST), 100, used._MaskMap_ST);
		this._MaskMap_WH = MaterialFingerprint._Round(material.GetVector(ShaderProps._MaskMap_WH), 100, used._MaskMap_WH);
		this._LavaLampToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LavaLampToggle), 100, used._LavaLampToggle);
		this._GradientMapToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._GradientMapToggle), 100, used._GradientMapToggle);
		this._GradientMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._GradientMap, used._GradientMap);
		this._DoTextureRotation = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DoTextureRotation), 100, used._DoTextureRotation);
		this._RotateAngle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._RotateAngle), 100, used._RotateAngle);
		this._RotateAnim = MaterialFingerprint._Round(material.GetFloat(ShaderProps._RotateAnim), 100, used._RotateAnim);
		this._UseWaveWarp = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseWaveWarp), 100, used._UseWaveWarp);
		this._WaveAmplitude = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaveAmplitude), 100, used._WaveAmplitude);
		this._WaveFrequency = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaveFrequency), 100, used._WaveFrequency);
		this._WaveScale = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaveScale), 100, used._WaveScale);
		this._WaveTimeScale = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaveTimeScale), 100, used._WaveTimeScale);
		this._ReflectToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectToggle), 100, used._ReflectToggle);
		this._ReflectBoxProjectToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectBoxProjectToggle), 100, used._ReflectBoxProjectToggle);
		this._ReflectBoxCubePos = MaterialFingerprint._Round(material.GetVector(ShaderProps._ReflectBoxCubePos), 100, used._ReflectBoxCubePos);
		this._ReflectBoxSize = MaterialFingerprint._Round(material.GetVector(ShaderProps._ReflectBoxSize), 100, used._ReflectBoxSize);
		this._ReflectBoxRotation = MaterialFingerprint._Round(material.GetVector(ShaderProps._ReflectBoxRotation), 100, used._ReflectBoxRotation);
		this._ReflectMatcapToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectMatcapToggle), 100, used._ReflectMatcapToggle);
		this._ReflectMatcapPerspToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectMatcapPerspToggle), 100, used._ReflectMatcapPerspToggle);
		this._ReflectNormalToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectNormalToggle), 100, used._ReflectNormalToggle);
		this._ReflectTex = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._ReflectTex, used._ReflectTex);
		this._ReflectNormalTex = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._ReflectNormalTex, used._ReflectNormalTex);
		this._ReflectAlbedoTint = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectAlbedoTint), 100, used._ReflectAlbedoTint);
		this._ReflectTint = MaterialFingerprint._Round(material.GetColor(ShaderProps._ReflectTint), 100, used._ReflectTint);
		this._ReflectOpacity = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectOpacity), 100, used._ReflectOpacity);
		this._ReflectExposure = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectExposure), 100, used._ReflectExposure);
		this._ReflectOffset = MaterialFingerprint._Round(material.GetVector(ShaderProps._ReflectOffset), 100, used._ReflectOffset);
		this._ReflectScale = MaterialFingerprint._Round(material.GetVector(ShaderProps._ReflectScale), 100, used._ReflectScale);
		this._ReflectRotate = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectRotate), 100, used._ReflectRotate);
		this._HalfLambertToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._HalfLambertToggle), 100, used._HalfLambertToggle);
		this._ParallaxPlanarToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ParallaxPlanarToggle), 100, used._ParallaxPlanarToggle);
		this._ParallaxToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ParallaxToggle), 100, used._ParallaxToggle);
		this._ParallaxAAToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ParallaxAAToggle), 100, used._ParallaxAAToggle);
		this._ParallaxAABias = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ParallaxAABias), 100, used._ParallaxAABias);
		this._DepthMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._DepthMap, used._DepthMap);
		this._ParallaxAmplitude = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ParallaxAmplitude), 100, used._ParallaxAmplitude);
		this._ParallaxSamplesMinMax = MaterialFingerprint._Round(material.GetVector(ShaderProps._ParallaxSamplesMinMax), 100, used._ParallaxSamplesMinMax);
		this._UvShiftToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UvShiftToggle), 100, used._UvShiftToggle);
		this._UvShiftSteps = MaterialFingerprint._Round(material.GetVector(ShaderProps._UvShiftSteps), 100, used._UvShiftSteps);
		this._UvShiftRate = MaterialFingerprint._Round(material.GetVector(ShaderProps._UvShiftRate), 100, used._UvShiftRate);
		this._UvShiftOffset = MaterialFingerprint._Round(material.GetVector(ShaderProps._UvShiftOffset), 100, used._UvShiftOffset);
		this._UseGridEffect = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseGridEffect), 100, used._UseGridEffect);
		this._UseCrystalEffect = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseCrystalEffect), 100, used._UseCrystalEffect);
		this._CrystalPower = MaterialFingerprint._Round(material.GetFloat(ShaderProps._CrystalPower), 100, used._CrystalPower);
		this._CrystalRimColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._CrystalRimColor), 100, used._CrystalRimColor);
		this._LiquidVolume = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LiquidVolume), 100, used._LiquidVolume);
		this._LiquidFill = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LiquidFill), 100, used._LiquidFill);
		this._LiquidFillNormal = MaterialFingerprint._Round(material.GetVector(ShaderProps._LiquidFillNormal), 100, used._LiquidFillNormal);
		this._LiquidSurfaceColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._LiquidSurfaceColor), 100, used._LiquidSurfaceColor);
		this._LiquidSwayX = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LiquidSwayX), 100, used._LiquidSwayX);
		this._LiquidSwayY = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LiquidSwayY), 100, used._LiquidSwayY);
		this._LiquidContainer = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LiquidContainer), 100, used._LiquidContainer);
		this._LiquidPlanePosition = MaterialFingerprint._Round(material.GetVector(ShaderProps._LiquidPlanePosition), 100, used._LiquidPlanePosition);
		this._LiquidPlaneNormal = MaterialFingerprint._Round(material.GetVector(ShaderProps._LiquidPlaneNormal), 100, used._LiquidPlaneNormal);
		this._VertexFlapToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexFlapToggle), 100, used._VertexFlapToggle);
		this._VertexFlapAxis = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexFlapAxis), 100, used._VertexFlapAxis);
		this._VertexFlapDegreesMinMax = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexFlapDegreesMinMax), 100, used._VertexFlapDegreesMinMax);
		this._VertexFlapSpeed = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexFlapSpeed), 100, used._VertexFlapSpeed);
		this._VertexFlapPhaseOffset = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexFlapPhaseOffset), 100, used._VertexFlapPhaseOffset);
		this._VertexWaveToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexWaveToggle), 100, used._VertexWaveToggle);
		this._VertexWaveDebug = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexWaveDebug), 100, used._VertexWaveDebug);
		this._VertexWaveEnd = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexWaveEnd), 100, used._VertexWaveEnd);
		this._VertexWaveParams = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexWaveParams), 100, used._VertexWaveParams);
		this._VertexWaveFalloff = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexWaveFalloff), 100, used._VertexWaveFalloff);
		this._VertexWaveSphereMask = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexWaveSphereMask), 100, used._VertexWaveSphereMask);
		this._VertexWavePhaseOffset = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexWavePhaseOffset), 100, used._VertexWavePhaseOffset);
		this._VertexWaveAxes = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexWaveAxes), 100, used._VertexWaveAxes);
		this._VertexRotateToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexRotateToggle), 100, used._VertexRotateToggle);
		this._VertexRotateAngles = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexRotateAngles), 100, used._VertexRotateAngles);
		this._VertexRotateAnim = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexRotateAnim), 100, used._VertexRotateAnim);
		this._VertexLightToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexLightToggle), 100, used._VertexLightToggle);
		this._InnerGlowOn = MaterialFingerprint._Round(material.GetFloat(ShaderProps._InnerGlowOn), 100, used._InnerGlowOn);
		this._InnerGlowColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._InnerGlowColor), 100, used._InnerGlowColor);
		this._InnerGlowParams = MaterialFingerprint._Round(material.GetVector(ShaderProps._InnerGlowParams), 100, used._InnerGlowParams);
		this._InnerGlowTap = MaterialFingerprint._Round(material.GetFloat(ShaderProps._InnerGlowTap), 100, used._InnerGlowTap);
		this._InnerGlowSine = MaterialFingerprint._Round(material.GetFloat(ShaderProps._InnerGlowSine), 100, used._InnerGlowSine);
		this._InnerGlowSinePeriod = MaterialFingerprint._Round(material.GetFloat(ShaderProps._InnerGlowSinePeriod), 100, used._InnerGlowSinePeriod);
		this._InnerGlowSinePhaseShift = MaterialFingerprint._Round(material.GetFloat(ShaderProps._InnerGlowSinePhaseShift), 100, used._InnerGlowSinePhaseShift);
		this._StealthEffectOn = MaterialFingerprint._Round(material.GetFloat(ShaderProps._StealthEffectOn), 100, used._StealthEffectOn);
		this._UseEyeTracking = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseEyeTracking), 100, used._UseEyeTracking);
		this._EyeTileOffsetUV = MaterialFingerprint._Round(material.GetVector(ShaderProps._EyeTileOffsetUV), 100, used._EyeTileOffsetUV);
		this._EyeOverrideUV = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EyeOverrideUV), 100, used._EyeOverrideUV);
		this._EyeOverrideUVTransform = MaterialFingerprint._Round(material.GetVector(ShaderProps._EyeOverrideUVTransform), 100, used._EyeOverrideUVTransform);
		this._UseMouthFlap = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseMouthFlap), 100, used._UseMouthFlap);
		this._MouthMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._MouthMap, used._MouthMap);
		this._MouthMap_ST = MaterialFingerprint._Round(material.GetVector(ShaderProps._MouthMap_ST), 100, used._MouthMap_ST);
		this._UseVertexColor = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseVertexColor), 100, used._UseVertexColor);
		this._WaterEffect = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaterEffect), 100, used._WaterEffect);
		this._HeightBasedWaterEffect = MaterialFingerprint._Round(material.GetFloat(ShaderProps._HeightBasedWaterEffect), 100, used._HeightBasedWaterEffect);
		this._WaterCaustics = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaterCaustics), 100, used._WaterCaustics);
		this._UseDayNightLightmap = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseDayNightLightmap), 100, used._UseDayNightLightmap);
		this._DAY_CYCLE_BRIGHTNESS_ = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DAY_CYCLE_BRIGHTNESS_), 100, used._DAY_CYCLE_BRIGHTNESS_);
		this._UseWeatherMap = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseWeatherMap), 100, used._UseWeatherMap);
		this._WeatherMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._WeatherMap, used._WeatherMap);
		this._WeatherMapDissolveEdgeSize = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WeatherMapDissolveEdgeSize), 100, used._WeatherMapDissolveEdgeSize);
		this._UseSpecular = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseSpecular), 100, used._UseSpecular);
		this._UseSpecularAlphaChannel = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseSpecularAlphaChannel), 100, used._UseSpecularAlphaChannel);
		this._Smoothness = MaterialFingerprint._Round(material.GetFloat(ShaderProps._Smoothness), 100, used._Smoothness);
		this._UseSpecHighlight = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseSpecHighlight), 100, used._UseSpecHighlight);
		this._SpecularDir = MaterialFingerprint._Round(material.GetVector(ShaderProps._SpecularDir), 100, used._SpecularDir);
		this._SpecularPowerIntensity = MaterialFingerprint._Round(material.GetVector(ShaderProps._SpecularPowerIntensity), 100, used._SpecularPowerIntensity);
		this._SpecularColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._SpecularColor), 100, used._SpecularColor);
		this._SpecularUseDiffuseColor = MaterialFingerprint._Round(material.GetFloat(ShaderProps._SpecularUseDiffuseColor), 100, used._SpecularUseDiffuseColor);
		this._EmissionToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionToggle), 100, used._EmissionToggle);
		this._EmissionColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._EmissionColor), 100, used._EmissionColor);
		this._EmissionMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._EmissionMap, used._EmissionMap);
		this._EmissionMaskByBaseMapAlpha = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionMaskByBaseMapAlpha), 100, used._EmissionMaskByBaseMapAlpha);
		this._EmissionUVScrollSpeed = MaterialFingerprint._Round(material.GetVector(ShaderProps._EmissionUVScrollSpeed), 100, used._EmissionUVScrollSpeed);
		this._EmissionDissolveProgress = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionDissolveProgress), 100, used._EmissionDissolveProgress);
		this._EmissionDissolveAnimation = MaterialFingerprint._Round(material.GetVector(ShaderProps._EmissionDissolveAnimation), 100, used._EmissionDissolveAnimation);
		this._EmissionDissolveEdgeSize = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionDissolveEdgeSize), 100, used._EmissionDissolveEdgeSize);
		this._EmissionIntensityInDynamic = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionIntensityInDynamic), 100, used._EmissionIntensityInDynamic);
		this._EmissionUseUVWaveWarp = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionUseUVWaveWarp), 100, used._EmissionUseUVWaveWarp);
		this._GreyZoneException = MaterialFingerprint._Round(material.GetFloat(ShaderProps._GreyZoneException), 100, used._GreyZoneException);
		this._Cull = MaterialFingerprint._Round(material.GetFloat(ShaderProps._Cull), 100, used._Cull);
		this._StencilReference = MaterialFingerprint._Round(material.GetFloat(ShaderProps._StencilReference), 100, used._StencilReference);
		this._StencilComparison = MaterialFingerprint._Round(material.GetFloat(ShaderProps._StencilComparison), 100, used._StencilComparison);
		this._StencilPassFront = MaterialFingerprint._Round(material.GetFloat(ShaderProps._StencilPassFront), 100, used._StencilPassFront);
		this._USE_DEFORM_MAP = MaterialFingerprint._Round(material.GetFloat(ShaderProps._USE_DEFORM_MAP), 100, used._USE_DEFORM_MAP);
		this._DeformMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._DeformMap, used._DeformMap);
		this._DeformMapIntensity = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DeformMapIntensity), 100, used._DeformMapIntensity);
		this._DeformMapMaskByVertColorRAmount = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DeformMapMaskByVertColorRAmount), 100, used._DeformMapMaskByVertColorRAmount);
		this._DeformMapScrollSpeed = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapScrollSpeed), 100, used._DeformMapScrollSpeed);
		this._DeformMapUV0Influence = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapUV0Influence), 100, used._DeformMapUV0Influence);
		this._DeformMapObjectSpaceOffsetsU = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapObjectSpaceOffsetsU), 100, used._DeformMapObjectSpaceOffsetsU);
		this._DeformMapObjectSpaceOffsetsV = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapObjectSpaceOffsetsV), 100, used._DeformMapObjectSpaceOffsetsV);
		this._DeformMapWorldSpaceOffsetsU = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapWorldSpaceOffsetsU), 100, used._DeformMapWorldSpaceOffsetsU);
		this._DeformMapWorldSpaceOffsetsV = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapWorldSpaceOffsetsV), 100, used._DeformMapWorldSpaceOffsetsV);
		this._RotateOnYAxisBySinTime = MaterialFingerprint._Round(material.GetVector(ShaderProps._RotateOnYAxisBySinTime), 100, used._RotateOnYAxisBySinTime);
		this._USE_TEX_ARRAY_ATLAS = MaterialFingerprint._Round(material.GetFloat(ShaderProps._USE_TEX_ARRAY_ATLAS), 100, used._USE_TEX_ARRAY_ATLAS);
		this._BaseMap_Atlas = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._BaseMap_Atlas, used._BaseMap_Atlas);
		this._BaseMap_AtlasSlice = MaterialFingerprint._Round(material.GetFloat(ShaderProps._BaseMap_AtlasSlice), 100, used._BaseMap_AtlasSlice);
		this._BaseMap_AtlasSliceSource = MaterialFingerprint._Round(material.GetFloat(ShaderProps._BaseMap_AtlasSliceSource), 100, used._BaseMap_AtlasSliceSource);
		this._EmissionMap_Atlas = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._EmissionMap_Atlas, used._EmissionMap_Atlas);
		this._EmissionMap_AtlasSlice = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionMap_AtlasSlice), 100, used._EmissionMap_AtlasSlice);
		this._DeformMap_Atlas = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._DeformMap_Atlas, used._DeformMap_Atlas);
		this._DeformMap_AtlasSlice = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DeformMap_AtlasSlice), 100, used._DeformMap_AtlasSlice);
		this._WeatherMap_Atlas = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._WeatherMap_Atlas, used._WeatherMap_Atlas);
		this._WeatherMap_AtlasSlice = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WeatherMap_AtlasSlice), 100, used._WeatherMap_AtlasSlice);
		this._DEBUG_PAWN_DATA = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DEBUG_PAWN_DATA), 100, used._DEBUG_PAWN_DATA);
		this._SrcBlend = MaterialFingerprint._Round(material.GetFloat(ShaderProps._SrcBlend), 100, used._SrcBlend);
		this._DstBlend = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DstBlend), 100, used._DstBlend);
		this._SrcBlendAlpha = MaterialFingerprint._Round(material.GetFloat(ShaderProps._SrcBlendAlpha), 100, used._SrcBlendAlpha);
		this._DstBlendAlpha = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DstBlendAlpha), 100, used._DstBlendAlpha);
		this._ZWrite = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ZWrite), 100, used._ZWrite);
		this._AlphaToMask = MaterialFingerprint._Round(material.GetFloat(ShaderProps._AlphaToMask), 100, used._AlphaToMask);
		this._Color = MaterialFingerprint._Round(material.GetColor(ShaderProps._Color), 100, used._Color);
		this._Surface = MaterialFingerprint._Round(material.GetFloat(ShaderProps._Surface), 100, used._Surface);
		this._Metallic = MaterialFingerprint._Round(material.GetFloat(ShaderProps._Metallic), 100, used._Metallic);
		this._SpecColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._SpecColor), 100, used._SpecColor);
		this._DayNightLightmapArray = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._DayNightLightmapArray, used._DayNightLightmapArray);
		this._DayNightLightmapArray_ST = MaterialFingerprint._Round(material.GetVector(ShaderProps._DayNightLightmapArray_ST), 100, used._DayNightLightmapArray_ST);
		this._DayNightLightmapArray_AtlasSlice = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DayNightLightmapArray_AtlasSlice), 100, used._DayNightLightmapArray_AtlasSlice);
		this.isValid = true;
	}

	// Token: 0x060015FF RID: 5631 RVA: 0x00076260 File Offset: 0x00074460
	private static int4 _Round(Color c, int mul, int usedCount)
	{
		if (usedCount <= 0)
		{
			return int4.zero;
		}
		return new int4(Mathf.RoundToInt(c.r * (float)mul), Mathf.RoundToInt(c.g * (float)mul), Mathf.RoundToInt(c.b * (float)mul), Mathf.RoundToInt(c.a * (float)mul));
	}

	// Token: 0x06001600 RID: 5632 RVA: 0x000762B4 File Offset: 0x000744B4
	private static int4 _Round(Vector4 v, int mul, int usedCount)
	{
		if (usedCount <= 0)
		{
			return int4.zero;
		}
		return new int4(Mathf.RoundToInt(v.x * (float)mul), Mathf.RoundToInt(v.y * (float)mul), Mathf.RoundToInt(v.z * (float)mul), Mathf.RoundToInt(v.w * (float)mul));
	}

	// Token: 0x06001601 RID: 5633 RVA: 0x00076308 File Offset: 0x00074508
	private static int _Round(float f, int mul, int usedCount)
	{
		return Mathf.RoundToInt(f * (float)mul);
	}

	// Token: 0x06001602 RID: 5634 RVA: 0x00076314 File Offset: 0x00074514
	private static TexFormatInfo _GetTexFormatInfo(Material mat, string texPropName, int usedCount)
	{
		if (usedCount > 0)
		{
			Texture2D texture2D = mat.GetTexture(texPropName) as Texture2D;
			if (texture2D != null)
			{
				return new TexFormatInfo(texture2D);
			}
		}
		return default(TexFormatInfo);
	}

	// Token: 0x06001603 RID: 5635 RVA: 0x0007634B File Offset: 0x0007454B
	private static string _GetTexPropGuid(Material mat, int texPropId, int usedCount)
	{
		return string.Empty;
	}

	// Token: 0x06001604 RID: 5636 RVA: 0x00076358 File Offset: 0x00074558
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static GTShaderTransparencyMode GetMatTransparencyMode(Material mat)
	{
		return (GTShaderTransparencyMode)mat.GetInteger(ShaderProps._TransparencyMode);
	}

	// Token: 0x06001605 RID: 5637 RVA: 0x00076374 File Offset: 0x00074574
	public override string ToString()
	{
		string text = "";
		BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		foreach (FieldInfo fieldInfo in typeof(MaterialFingerprint).GetFields(bindingFlags))
		{
			text = string.Concat(new string[]
			{
				text,
				"|",
				fieldInfo.ToString(),
				":",
				fieldInfo.GetValue(this).ToString()
			});
		}
		return text;
	}

	// Token: 0x04001ADC RID: 6876
	public GTShaderTransparencyMode _TransparencyMode;

	// Token: 0x04001ADD RID: 6877
	public int _Cutoff;

	// Token: 0x04001ADE RID: 6878
	public int _ColorSource;

	// Token: 0x04001ADF RID: 6879
	public int4 _BaseColor;

	// Token: 0x04001AE0 RID: 6880
	public int4 _GChannelColor;

	// Token: 0x04001AE1 RID: 6881
	public int4 _BChannelColor;

	// Token: 0x04001AE2 RID: 6882
	public int4 _AChannelColor;

	// Token: 0x04001AE3 RID: 6883
	public string _BaseMap;

	// Token: 0x04001AE4 RID: 6884
	public int4 _BaseMap_ST;

	// Token: 0x04001AE5 RID: 6885
	public int _SettingsPreset;

	// Token: 0x04001AE6 RID: 6886
	public int _AdvancedOptions;

	// Token: 0x04001AE7 RID: 6887
	public int _TexMipBias;

	// Token: 0x04001AE8 RID: 6888
	public int4 _BaseMap_WH;

	// Token: 0x04001AE9 RID: 6889
	public int _TexelSnapToggle;

	// Token: 0x04001AEA RID: 6890
	public int _TexelSnap_Factor;

	// Token: 0x04001AEB RID: 6891
	public int _UVSource;

	// Token: 0x04001AEC RID: 6892
	public int _AlphaDetailToggle;

	// Token: 0x04001AED RID: 6893
	public int4 _AlphaDetail_ST;

	// Token: 0x04001AEE RID: 6894
	public int _AlphaDetail_Opacity;

	// Token: 0x04001AEF RID: 6895
	public int _AlphaDetail_WorldSpace;

	// Token: 0x04001AF0 RID: 6896
	public int _MaskMapToggle;

	// Token: 0x04001AF1 RID: 6897
	public string _MaskMap;

	// Token: 0x04001AF2 RID: 6898
	public int4 _MaskMap_ST;

	// Token: 0x04001AF3 RID: 6899
	public int4 _MaskMap_WH;

	// Token: 0x04001AF4 RID: 6900
	public int _LavaLampToggle;

	// Token: 0x04001AF5 RID: 6901
	public int _GradientMapToggle;

	// Token: 0x04001AF6 RID: 6902
	public string _GradientMap;

	// Token: 0x04001AF7 RID: 6903
	public int _DoTextureRotation;

	// Token: 0x04001AF8 RID: 6904
	public int _RotateAngle;

	// Token: 0x04001AF9 RID: 6905
	public int _RotateAnim;

	// Token: 0x04001AFA RID: 6906
	public int _UseWaveWarp;

	// Token: 0x04001AFB RID: 6907
	public int _WaveAmplitude;

	// Token: 0x04001AFC RID: 6908
	public int _WaveFrequency;

	// Token: 0x04001AFD RID: 6909
	public int _WaveScale;

	// Token: 0x04001AFE RID: 6910
	public int _WaveTimeScale;

	// Token: 0x04001AFF RID: 6911
	public int _ReflectToggle;

	// Token: 0x04001B00 RID: 6912
	public int _ReflectBoxProjectToggle;

	// Token: 0x04001B01 RID: 6913
	public int4 _ReflectBoxCubePos;

	// Token: 0x04001B02 RID: 6914
	public int4 _ReflectBoxSize;

	// Token: 0x04001B03 RID: 6915
	public int4 _ReflectBoxRotation;

	// Token: 0x04001B04 RID: 6916
	public int _ReflectMatcapToggle;

	// Token: 0x04001B05 RID: 6917
	public int _ReflectMatcapPerspToggle;

	// Token: 0x04001B06 RID: 6918
	public int _ReflectNormalToggle;

	// Token: 0x04001B07 RID: 6919
	public string _ReflectTex;

	// Token: 0x04001B08 RID: 6920
	public string _ReflectNormalTex;

	// Token: 0x04001B09 RID: 6921
	public int _ReflectAlbedoTint;

	// Token: 0x04001B0A RID: 6922
	public int4 _ReflectTint;

	// Token: 0x04001B0B RID: 6923
	public int _ReflectOpacity;

	// Token: 0x04001B0C RID: 6924
	public int _ReflectExposure;

	// Token: 0x04001B0D RID: 6925
	public int4 _ReflectOffset;

	// Token: 0x04001B0E RID: 6926
	public int4 _ReflectScale;

	// Token: 0x04001B0F RID: 6927
	public int _ReflectRotate;

	// Token: 0x04001B10 RID: 6928
	public int _HalfLambertToggle;

	// Token: 0x04001B11 RID: 6929
	public int _ParallaxPlanarToggle;

	// Token: 0x04001B12 RID: 6930
	public int _ParallaxToggle;

	// Token: 0x04001B13 RID: 6931
	public int _ParallaxAAToggle;

	// Token: 0x04001B14 RID: 6932
	public int _ParallaxAABias;

	// Token: 0x04001B15 RID: 6933
	public string _DepthMap;

	// Token: 0x04001B16 RID: 6934
	public int _ParallaxAmplitude;

	// Token: 0x04001B17 RID: 6935
	public int4 _ParallaxSamplesMinMax;

	// Token: 0x04001B18 RID: 6936
	public int _UvShiftToggle;

	// Token: 0x04001B19 RID: 6937
	public int4 _UvShiftSteps;

	// Token: 0x04001B1A RID: 6938
	public int4 _UvShiftRate;

	// Token: 0x04001B1B RID: 6939
	public int4 _UvShiftOffset;

	// Token: 0x04001B1C RID: 6940
	public int _UseGridEffect;

	// Token: 0x04001B1D RID: 6941
	public int _UseCrystalEffect;

	// Token: 0x04001B1E RID: 6942
	public int _CrystalPower;

	// Token: 0x04001B1F RID: 6943
	public int4 _CrystalRimColor;

	// Token: 0x04001B20 RID: 6944
	public int _LiquidVolume;

	// Token: 0x04001B21 RID: 6945
	public int _LiquidFill;

	// Token: 0x04001B22 RID: 6946
	public int4 _LiquidFillNormal;

	// Token: 0x04001B23 RID: 6947
	public int4 _LiquidSurfaceColor;

	// Token: 0x04001B24 RID: 6948
	public int _LiquidSwayX;

	// Token: 0x04001B25 RID: 6949
	public int _LiquidSwayY;

	// Token: 0x04001B26 RID: 6950
	public int _LiquidContainer;

	// Token: 0x04001B27 RID: 6951
	public int4 _LiquidPlanePosition;

	// Token: 0x04001B28 RID: 6952
	public int4 _LiquidPlaneNormal;

	// Token: 0x04001B29 RID: 6953
	public int _VertexFlapToggle;

	// Token: 0x04001B2A RID: 6954
	public int4 _VertexFlapAxis;

	// Token: 0x04001B2B RID: 6955
	public int4 _VertexFlapDegreesMinMax;

	// Token: 0x04001B2C RID: 6956
	public int _VertexFlapSpeed;

	// Token: 0x04001B2D RID: 6957
	public int _VertexFlapPhaseOffset;

	// Token: 0x04001B2E RID: 6958
	public int _VertexWaveToggle;

	// Token: 0x04001B2F RID: 6959
	public int _VertexWaveDebug;

	// Token: 0x04001B30 RID: 6960
	public int4 _VertexWaveEnd;

	// Token: 0x04001B31 RID: 6961
	public int4 _VertexWaveParams;

	// Token: 0x04001B32 RID: 6962
	public int4 _VertexWaveFalloff;

	// Token: 0x04001B33 RID: 6963
	public int4 _VertexWaveSphereMask;

	// Token: 0x04001B34 RID: 6964
	public int _VertexWavePhaseOffset;

	// Token: 0x04001B35 RID: 6965
	public int4 _VertexWaveAxes;

	// Token: 0x04001B36 RID: 6966
	public int _VertexRotateToggle;

	// Token: 0x04001B37 RID: 6967
	public int4 _VertexRotateAngles;

	// Token: 0x04001B38 RID: 6968
	public int _VertexRotateAnim;

	// Token: 0x04001B39 RID: 6969
	public int _VertexLightToggle;

	// Token: 0x04001B3A RID: 6970
	public int _InnerGlowOn;

	// Token: 0x04001B3B RID: 6971
	public int4 _InnerGlowColor;

	// Token: 0x04001B3C RID: 6972
	public int4 _InnerGlowParams;

	// Token: 0x04001B3D RID: 6973
	public int _InnerGlowTap;

	// Token: 0x04001B3E RID: 6974
	public int _InnerGlowSine;

	// Token: 0x04001B3F RID: 6975
	public int _InnerGlowSinePeriod;

	// Token: 0x04001B40 RID: 6976
	public int _InnerGlowSinePhaseShift;

	// Token: 0x04001B41 RID: 6977
	public int _StealthEffectOn;

	// Token: 0x04001B42 RID: 6978
	public int _UseEyeTracking;

	// Token: 0x04001B43 RID: 6979
	public int4 _EyeTileOffsetUV;

	// Token: 0x04001B44 RID: 6980
	public int _EyeOverrideUV;

	// Token: 0x04001B45 RID: 6981
	public int4 _EyeOverrideUVTransform;

	// Token: 0x04001B46 RID: 6982
	public int _UseMouthFlap;

	// Token: 0x04001B47 RID: 6983
	public string _MouthMap;

	// Token: 0x04001B48 RID: 6984
	public int4 _MouthMap_ST;

	// Token: 0x04001B49 RID: 6985
	public int _UseVertexColor;

	// Token: 0x04001B4A RID: 6986
	public int _WaterEffect;

	// Token: 0x04001B4B RID: 6987
	public int _HeightBasedWaterEffect;

	// Token: 0x04001B4C RID: 6988
	public int _WaterCaustics;

	// Token: 0x04001B4D RID: 6989
	public int _UseDayNightLightmap;

	// Token: 0x04001B4E RID: 6990
	public int _DAY_CYCLE_BRIGHTNESS_;

	// Token: 0x04001B4F RID: 6991
	public int _UseWeatherMap;

	// Token: 0x04001B50 RID: 6992
	public string _WeatherMap;

	// Token: 0x04001B51 RID: 6993
	public int _WeatherMapDissolveEdgeSize;

	// Token: 0x04001B52 RID: 6994
	public int _UseSpecular;

	// Token: 0x04001B53 RID: 6995
	public int _UseSpecularAlphaChannel;

	// Token: 0x04001B54 RID: 6996
	public int _Smoothness;

	// Token: 0x04001B55 RID: 6997
	public int _UseSpecHighlight;

	// Token: 0x04001B56 RID: 6998
	public int4 _SpecularDir;

	// Token: 0x04001B57 RID: 6999
	public int4 _SpecularPowerIntensity;

	// Token: 0x04001B58 RID: 7000
	public int4 _SpecularColor;

	// Token: 0x04001B59 RID: 7001
	public int _SpecularUseDiffuseColor;

	// Token: 0x04001B5A RID: 7002
	public int _EmissionToggle;

	// Token: 0x04001B5B RID: 7003
	public int4 _EmissionColor;

	// Token: 0x04001B5C RID: 7004
	public string _EmissionMap;

	// Token: 0x04001B5D RID: 7005
	public int _EmissionMaskByBaseMapAlpha;

	// Token: 0x04001B5E RID: 7006
	public int4 _EmissionUVScrollSpeed;

	// Token: 0x04001B5F RID: 7007
	public int _EmissionDissolveProgress;

	// Token: 0x04001B60 RID: 7008
	public int4 _EmissionDissolveAnimation;

	// Token: 0x04001B61 RID: 7009
	public int _EmissionDissolveEdgeSize;

	// Token: 0x04001B62 RID: 7010
	public int _EmissionIntensityInDynamic;

	// Token: 0x04001B63 RID: 7011
	public int _EmissionUseUVWaveWarp;

	// Token: 0x04001B64 RID: 7012
	public int _GreyZoneException;

	// Token: 0x04001B65 RID: 7013
	public int _Cull;

	// Token: 0x04001B66 RID: 7014
	public int _StencilReference;

	// Token: 0x04001B67 RID: 7015
	public int _StencilComparison;

	// Token: 0x04001B68 RID: 7016
	public int _StencilPassFront;

	// Token: 0x04001B69 RID: 7017
	public int _USE_DEFORM_MAP;

	// Token: 0x04001B6A RID: 7018
	public string _DeformMap;

	// Token: 0x04001B6B RID: 7019
	public int _DeformMapIntensity;

	// Token: 0x04001B6C RID: 7020
	public int _DeformMapMaskByVertColorRAmount;

	// Token: 0x04001B6D RID: 7021
	public int4 _DeformMapScrollSpeed;

	// Token: 0x04001B6E RID: 7022
	public int4 _DeformMapUV0Influence;

	// Token: 0x04001B6F RID: 7023
	public int4 _DeformMapObjectSpaceOffsetsU;

	// Token: 0x04001B70 RID: 7024
	public int4 _DeformMapObjectSpaceOffsetsV;

	// Token: 0x04001B71 RID: 7025
	public int4 _DeformMapWorldSpaceOffsetsU;

	// Token: 0x04001B72 RID: 7026
	public int4 _DeformMapWorldSpaceOffsetsV;

	// Token: 0x04001B73 RID: 7027
	public int4 _RotateOnYAxisBySinTime;

	// Token: 0x04001B74 RID: 7028
	public int _USE_TEX_ARRAY_ATLAS;

	// Token: 0x04001B75 RID: 7029
	public string _BaseMap_Atlas;

	// Token: 0x04001B76 RID: 7030
	public int _BaseMap_AtlasSlice;

	// Token: 0x04001B77 RID: 7031
	public int _BaseMap_AtlasSliceSource;

	// Token: 0x04001B78 RID: 7032
	public string _EmissionMap_Atlas;

	// Token: 0x04001B79 RID: 7033
	public int _EmissionMap_AtlasSlice;

	// Token: 0x04001B7A RID: 7034
	public string _DeformMap_Atlas;

	// Token: 0x04001B7B RID: 7035
	public int _DeformMap_AtlasSlice;

	// Token: 0x04001B7C RID: 7036
	public string _WeatherMap_Atlas;

	// Token: 0x04001B7D RID: 7037
	public int _WeatherMap_AtlasSlice;

	// Token: 0x04001B7E RID: 7038
	public int _DEBUG_PAWN_DATA;

	// Token: 0x04001B7F RID: 7039
	public int _SrcBlend;

	// Token: 0x04001B80 RID: 7040
	public int _DstBlend;

	// Token: 0x04001B81 RID: 7041
	public int _SrcBlendAlpha;

	// Token: 0x04001B82 RID: 7042
	public int _DstBlendAlpha;

	// Token: 0x04001B83 RID: 7043
	public int _ZWrite;

	// Token: 0x04001B84 RID: 7044
	public int _AlphaToMask;

	// Token: 0x04001B85 RID: 7045
	public int4 _Color;

	// Token: 0x04001B86 RID: 7046
	public int _Surface;

	// Token: 0x04001B87 RID: 7047
	public int _Metallic;

	// Token: 0x04001B88 RID: 7048
	public int4 _SpecColor;

	// Token: 0x04001B89 RID: 7049
	public string _DayNightLightmapArray;

	// Token: 0x04001B8A RID: 7050
	public int4 _DayNightLightmapArray_ST;

	// Token: 0x04001B8B RID: 7051
	public int _DayNightLightmapArray_AtlasSlice;

	// Token: 0x04001B8C RID: 7052
	private const bool _k_UNITY_2023_1_OR_NEWER = true;

	// Token: 0x04001B8D RID: 7053
	public bool isValid;
}
