using System;
using UnityEngine;

// Token: 0x02000E7C RID: 3708
public struct GTUberShader_MaterialKeywordStates
{
	// Token: 0x06005A3A RID: 23098 RVA: 0x001D4020 File Offset: 0x001D2220
	public GTUberShader_MaterialKeywordStates(Material mat)
	{
		this.material = mat;
		this._ALPHA_BLUE_LIVE_ON = mat.IsKeywordEnabled("_ALPHA_BLUE_LIVE_ON");
		this._ALPHA_DETAIL_MAP = mat.IsKeywordEnabled("_ALPHA_DETAIL_MAP");
		this._ALPHATEST_ON = mat.IsKeywordEnabled("_ALPHATEST_ON");
		this._COLOR_GRADE_ACHROMATOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOMALY");
		this._COLOR_GRADE_ACHROMATOPSIA = mat.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOPSIA");
		this._COLOR_GRADE_DEUTERANOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOMALY");
		this._COLOR_GRADE_DEUTERANOPIA = mat.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOPIA");
		this._COLOR_GRADE_PROTANOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_PROTANOMALY");
		this._COLOR_GRADE_PROTANOPIA = mat.IsKeywordEnabled("_COLOR_GRADE_PROTANOPIA");
		this._COLOR_GRADE_TRITANOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_TRITANOMALY");
		this._COLOR_GRADE_TRITANOPIA = mat.IsKeywordEnabled("_COLOR_GRADE_TRITANOPIA");
		this._CRYSTAL_EFFECT = mat.IsKeywordEnabled("_CRYSTAL_EFFECT");
		this._DAY_CYCLE_BRIGHTNESS__OPTION_1 = mat.IsKeywordEnabled("_DAY_CYCLE_BRIGHTNESS__OPTION_1");
		this._DAY_CYCLE_BRIGHTNESS__OPTION_2 = mat.IsKeywordEnabled("_DAY_CYCLE_BRIGHTNESS__OPTION_2");
		this._DEBUG_PAWN_DATA = mat.IsKeywordEnabled("_DEBUG_PAWN_DATA");
		this._EMISSION = mat.IsKeywordEnabled("_EMISSION");
		this._EMISSION_USE_UV_WAVE_WARP = mat.IsKeywordEnabled("_EMISSION_USE_UV_WAVE_WARP");
		this._EYECOMP = mat.IsKeywordEnabled("_EYECOMP");
		this._FX_LAVA_LAMP = mat.IsKeywordEnabled("_FX_LAVA_LAMP");
		this._GLOBAL_ZONE_LIQUID_TYPE__LAVA = mat.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
		this._GLOBAL_ZONE_LIQUID_TYPE__WATER = mat.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
		this._GRADIENT_MAP_ON = mat.IsKeywordEnabled("_GRADIENT_MAP_ON");
		this._GRID_EFFECT = mat.IsKeywordEnabled("_GRID_EFFECT");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY = mat.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z = mat.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z");
		this._GT_EDITOR_TIME = mat.IsKeywordEnabled("_GT_EDITOR_TIME");
		this._GT_RIM_LIGHT = mat.IsKeywordEnabled("_GT_RIM_LIGHT");
		this._GT_RIM_LIGHT_FLAT = mat.IsKeywordEnabled("_GT_RIM_LIGHT_FLAT");
		this._GT_RIM_LIGHT_USE_ALPHA = mat.IsKeywordEnabled("_GT_RIM_LIGHT_USE_ALPHA");
		this._HALF_LAMBERT_TERM = mat.IsKeywordEnabled("_HALF_LAMBERT_TERM");
		this._HEIGHT_BASED_WATER_EFFECT = mat.IsKeywordEnabled("_HEIGHT_BASED_WATER_EFFECT");
		this._INNER_GLOW = mat.IsKeywordEnabled("_INNER_GLOW");
		this._LIQUID_CONTAINER = mat.IsKeywordEnabled("_LIQUID_CONTAINER");
		this._LIQUID_VOLUME = mat.IsKeywordEnabled("_LIQUID_VOLUME");
		this._MAINTEX_ROTATE = mat.IsKeywordEnabled("_MAINTEX_ROTATE");
		this._MASK_MAP_ON = mat.IsKeywordEnabled("_MASK_MAP_ON");
		this._MOUTHCOMP = mat.IsKeywordEnabled("_MOUTHCOMP");
		this._PARALLAX = mat.IsKeywordEnabled("_PARALLAX");
		this._PARALLAX_AA = mat.IsKeywordEnabled("_PARALLAX_AA");
		this._PARALLAX_PLANAR = mat.IsKeywordEnabled("_PARALLAX_PLANAR");
		this._REFLECTIONS = mat.IsKeywordEnabled("_REFLECTIONS");
		this._REFLECTIONS_ALBEDO_TINT = mat.IsKeywordEnabled("_REFLECTIONS_ALBEDO_TINT");
		this._REFLECTIONS_BOX_PROJECT = mat.IsKeywordEnabled("_REFLECTIONS_BOX_PROJECT");
		this._REFLECTIONS_MATCAP = mat.IsKeywordEnabled("_REFLECTIONS_MATCAP");
		this._REFLECTIONS_MATCAP_PERSP_AWARE = mat.IsKeywordEnabled("_REFLECTIONS_MATCAP_PERSP_AWARE");
		this._REFLECTIONS_USE_NORMAL_TEX = mat.IsKeywordEnabled("_REFLECTIONS_USE_NORMAL_TEX");
		this._SPECULAR_HIGHLIGHT = mat.IsKeywordEnabled("_SPECULAR_HIGHLIGHT");
		this._STEALTH_EFFECT = mat.IsKeywordEnabled("_STEALTH_EFFECT");
		this._TEXEL_SNAP_UVS = mat.IsKeywordEnabled("_TEXEL_SNAP_UVS");
		this._UNITY_EDIT_MODE = mat.IsKeywordEnabled("_UNITY_EDIT_MODE");
		this._USE_DAY_NIGHT_LIGHTMAP = mat.IsKeywordEnabled("_USE_DAY_NIGHT_LIGHTMAP");
		this._USE_DEFORM_MAP = mat.IsKeywordEnabled("_USE_DEFORM_MAP");
		this._USE_TEX_ARRAY_ATLAS = mat.IsKeywordEnabled("_USE_TEX_ARRAY_ATLAS");
		this._USE_TEXTURE = mat.IsKeywordEnabled("_USE_TEXTURE");
		this._USE_VERTEX_COLOR = mat.IsKeywordEnabled("_USE_VERTEX_COLOR");
		this._USE_WEATHER_MAP = mat.IsKeywordEnabled("_USE_WEATHER_MAP");
		this._UV_SHIFT = mat.IsKeywordEnabled("_UV_SHIFT");
		this._UV_SOURCE__UV0 = mat.IsKeywordEnabled("_UV_SOURCE__UV0");
		this._UV_SOURCE__WORLD_PLANAR_Y = mat.IsKeywordEnabled("_UV_SOURCE__WORLD_PLANAR_Y");
		this._UV_WAVE_WARP = mat.IsKeywordEnabled("_UV_WAVE_WARP");
		this._VERTEX_ANIM_FLAP = mat.IsKeywordEnabled("_VERTEX_ANIM_FLAP");
		this._VERTEX_ANIM_WAVE = mat.IsKeywordEnabled("_VERTEX_ANIM_WAVE");
		this._VERTEX_ANIM_WAVE_DEBUG = mat.IsKeywordEnabled("_VERTEX_ANIM_WAVE_DEBUG");
		this._VERTEX_ROTATE = mat.IsKeywordEnabled("_VERTEX_ROTATE");
		this._WATER_CAUSTICS = mat.IsKeywordEnabled("_WATER_CAUSTICS");
		this._WATER_EFFECT = mat.IsKeywordEnabled("_WATER_EFFECT");
		this._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX = mat.IsKeywordEnabled("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
		this._ZONE_LIQUID_SHAPE__CYLINDER = mat.IsKeywordEnabled("_ZONE_LIQUID_SHAPE__CYLINDER");
		this.DIRLIGHTMAP_COMBINED = mat.IsKeywordEnabled("DIRLIGHTMAP_COMBINED");
		this.INSTANCING_ON = mat.IsKeywordEnabled("INSTANCING_ON");
		this.LIGHTMAP_ON = mat.IsKeywordEnabled("LIGHTMAP_ON");
		this.STEREO_CUBEMAP_RENDER_ON = mat.IsKeywordEnabled("STEREO_CUBEMAP_RENDER_ON");
		this.STEREO_INSTANCING_ON = mat.IsKeywordEnabled("STEREO_INSTANCING_ON");
		this.STEREO_MULTIVIEW_ON = mat.IsKeywordEnabled("STEREO_MULTIVIEW_ON");
		this.UNITY_SINGLE_PASS_STEREO = mat.IsKeywordEnabled("UNITY_SINGLE_PASS_STEREO");
		this.USE_TEXTURE__AS_MASK = mat.IsKeywordEnabled("USE_TEXTURE__AS_MASK");
	}

	// Token: 0x06005A3B RID: 23099 RVA: 0x001D4540 File Offset: 0x001D2740
	public void Refresh()
	{
		Material material = this.material;
		this._ALPHA_BLUE_LIVE_ON = material.IsKeywordEnabled("_ALPHA_BLUE_LIVE_ON");
		this._ALPHA_DETAIL_MAP = material.IsKeywordEnabled("_ALPHA_DETAIL_MAP");
		this._ALPHATEST_ON = material.IsKeywordEnabled("_ALPHATEST_ON");
		this._COLOR_GRADE_ACHROMATOMALY = material.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOMALY");
		this._COLOR_GRADE_ACHROMATOPSIA = material.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOPSIA");
		this._COLOR_GRADE_DEUTERANOMALY = material.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOMALY");
		this._COLOR_GRADE_DEUTERANOPIA = material.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOPIA");
		this._COLOR_GRADE_PROTANOMALY = material.IsKeywordEnabled("_COLOR_GRADE_PROTANOMALY");
		this._COLOR_GRADE_PROTANOPIA = material.IsKeywordEnabled("_COLOR_GRADE_PROTANOPIA");
		this._COLOR_GRADE_TRITANOMALY = material.IsKeywordEnabled("_COLOR_GRADE_TRITANOMALY");
		this._COLOR_GRADE_TRITANOPIA = material.IsKeywordEnabled("_COLOR_GRADE_TRITANOPIA");
		this._CRYSTAL_EFFECT = material.IsKeywordEnabled("_CRYSTAL_EFFECT");
		this._DAY_CYCLE_BRIGHTNESS__OPTION_1 = material.IsKeywordEnabled("_DAY_CYCLE_BRIGHTNESS__OPTION_1");
		this._DAY_CYCLE_BRIGHTNESS__OPTION_2 = material.IsKeywordEnabled("_DAY_CYCLE_BRIGHTNESS__OPTION_2");
		this._DEBUG_PAWN_DATA = material.IsKeywordEnabled("_DEBUG_PAWN_DATA");
		this._EMISSION = material.IsKeywordEnabled("_EMISSION");
		this._EMISSION_USE_UV_WAVE_WARP = material.IsKeywordEnabled("_EMISSION_USE_UV_WAVE_WARP");
		this._EYECOMP = material.IsKeywordEnabled("_EYECOMP");
		this._FX_LAVA_LAMP = material.IsKeywordEnabled("_FX_LAVA_LAMP");
		this._GLOBAL_ZONE_LIQUID_TYPE__LAVA = material.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
		this._GLOBAL_ZONE_LIQUID_TYPE__WATER = material.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
		this._GRADIENT_MAP_ON = material.IsKeywordEnabled("_GRADIENT_MAP_ON");
		this._GRID_EFFECT = material.IsKeywordEnabled("_GRID_EFFECT");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY = material.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z = material.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z");
		this._GT_EDITOR_TIME = material.IsKeywordEnabled("_GT_EDITOR_TIME");
		this._GT_RIM_LIGHT = material.IsKeywordEnabled("_GT_RIM_LIGHT");
		this._GT_RIM_LIGHT_FLAT = material.IsKeywordEnabled("_GT_RIM_LIGHT_FLAT");
		this._GT_RIM_LIGHT_USE_ALPHA = material.IsKeywordEnabled("_GT_RIM_LIGHT_USE_ALPHA");
		this._HALF_LAMBERT_TERM = material.IsKeywordEnabled("_HALF_LAMBERT_TERM");
		this._HEIGHT_BASED_WATER_EFFECT = material.IsKeywordEnabled("_HEIGHT_BASED_WATER_EFFECT");
		this._INNER_GLOW = material.IsKeywordEnabled("_INNER_GLOW");
		this._LIQUID_CONTAINER = material.IsKeywordEnabled("_LIQUID_CONTAINER");
		this._LIQUID_VOLUME = material.IsKeywordEnabled("_LIQUID_VOLUME");
		this._MAINTEX_ROTATE = material.IsKeywordEnabled("_MAINTEX_ROTATE");
		this._MASK_MAP_ON = material.IsKeywordEnabled("_MASK_MAP_ON");
		this._MOUTHCOMP = material.IsKeywordEnabled("_MOUTHCOMP");
		this._PARALLAX = material.IsKeywordEnabled("_PARALLAX");
		this._PARALLAX_AA = material.IsKeywordEnabled("_PARALLAX_AA");
		this._PARALLAX_PLANAR = material.IsKeywordEnabled("_PARALLAX_PLANAR");
		this._REFLECTIONS = material.IsKeywordEnabled("_REFLECTIONS");
		this._REFLECTIONS_ALBEDO_TINT = material.IsKeywordEnabled("_REFLECTIONS_ALBEDO_TINT");
		this._REFLECTIONS_BOX_PROJECT = material.IsKeywordEnabled("_REFLECTIONS_BOX_PROJECT");
		this._REFLECTIONS_MATCAP = material.IsKeywordEnabled("_REFLECTIONS_MATCAP");
		this._REFLECTIONS_MATCAP_PERSP_AWARE = material.IsKeywordEnabled("_REFLECTIONS_MATCAP_PERSP_AWARE");
		this._REFLECTIONS_USE_NORMAL_TEX = material.IsKeywordEnabled("_REFLECTIONS_USE_NORMAL_TEX");
		this._SPECULAR_HIGHLIGHT = material.IsKeywordEnabled("_SPECULAR_HIGHLIGHT");
		this._STEALTH_EFFECT = material.IsKeywordEnabled("_STEALTH_EFFECT");
		this._TEXEL_SNAP_UVS = material.IsKeywordEnabled("_TEXEL_SNAP_UVS");
		this._UNITY_EDIT_MODE = material.IsKeywordEnabled("_UNITY_EDIT_MODE");
		this._USE_DAY_NIGHT_LIGHTMAP = material.IsKeywordEnabled("_USE_DAY_NIGHT_LIGHTMAP");
		this._USE_DEFORM_MAP = material.IsKeywordEnabled("_USE_DEFORM_MAP");
		this._USE_TEX_ARRAY_ATLAS = material.IsKeywordEnabled("_USE_TEX_ARRAY_ATLAS");
		this._USE_TEXTURE = material.IsKeywordEnabled("_USE_TEXTURE");
		this._USE_VERTEX_COLOR = material.IsKeywordEnabled("_USE_VERTEX_COLOR");
		this._USE_WEATHER_MAP = material.IsKeywordEnabled("_USE_WEATHER_MAP");
		this._UV_SHIFT = material.IsKeywordEnabled("_UV_SHIFT");
		this._UV_SOURCE__UV0 = material.IsKeywordEnabled("_UV_SOURCE__UV0");
		this._UV_SOURCE__WORLD_PLANAR_Y = material.IsKeywordEnabled("_UV_SOURCE__WORLD_PLANAR_Y");
		this._UV_WAVE_WARP = material.IsKeywordEnabled("_UV_WAVE_WARP");
		this._VERTEX_ANIM_FLAP = material.IsKeywordEnabled("_VERTEX_ANIM_FLAP");
		this._VERTEX_ANIM_WAVE = material.IsKeywordEnabled("_VERTEX_ANIM_WAVE");
		this._VERTEX_ANIM_WAVE_DEBUG = material.IsKeywordEnabled("_VERTEX_ANIM_WAVE_DEBUG");
		this._VERTEX_ROTATE = material.IsKeywordEnabled("_VERTEX_ROTATE");
		this._WATER_CAUSTICS = material.IsKeywordEnabled("_WATER_CAUSTICS");
		this._WATER_EFFECT = material.IsKeywordEnabled("_WATER_EFFECT");
		this._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX = material.IsKeywordEnabled("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
		this._ZONE_LIQUID_SHAPE__CYLINDER = material.IsKeywordEnabled("_ZONE_LIQUID_SHAPE__CYLINDER");
		this.DIRLIGHTMAP_COMBINED = material.IsKeywordEnabled("DIRLIGHTMAP_COMBINED");
		this.INSTANCING_ON = material.IsKeywordEnabled("INSTANCING_ON");
		this.LIGHTMAP_ON = material.IsKeywordEnabled("LIGHTMAP_ON");
		this.STEREO_CUBEMAP_RENDER_ON = material.IsKeywordEnabled("STEREO_CUBEMAP_RENDER_ON");
		this.STEREO_INSTANCING_ON = material.IsKeywordEnabled("STEREO_INSTANCING_ON");
		this.STEREO_MULTIVIEW_ON = material.IsKeywordEnabled("STEREO_MULTIVIEW_ON");
		this.UNITY_SINGLE_PASS_STEREO = material.IsKeywordEnabled("UNITY_SINGLE_PASS_STEREO");
		this.USE_TEXTURE__AS_MASK = material.IsKeywordEnabled("USE_TEXTURE__AS_MASK");
	}

	// Token: 0x04006AE6 RID: 27366
	public Material material;

	// Token: 0x04006AE7 RID: 27367
	public bool _ALPHA_BLUE_LIVE_ON;

	// Token: 0x04006AE8 RID: 27368
	public bool _ALPHA_DETAIL_MAP;

	// Token: 0x04006AE9 RID: 27369
	public bool _ALPHATEST_ON;

	// Token: 0x04006AEA RID: 27370
	public bool _COLOR_GRADE_ACHROMATOMALY;

	// Token: 0x04006AEB RID: 27371
	public bool _COLOR_GRADE_ACHROMATOPSIA;

	// Token: 0x04006AEC RID: 27372
	public bool _COLOR_GRADE_DEUTERANOMALY;

	// Token: 0x04006AED RID: 27373
	public bool _COLOR_GRADE_DEUTERANOPIA;

	// Token: 0x04006AEE RID: 27374
	public bool _COLOR_GRADE_PROTANOMALY;

	// Token: 0x04006AEF RID: 27375
	public bool _COLOR_GRADE_PROTANOPIA;

	// Token: 0x04006AF0 RID: 27376
	public bool _COLOR_GRADE_TRITANOMALY;

	// Token: 0x04006AF1 RID: 27377
	public bool _COLOR_GRADE_TRITANOPIA;

	// Token: 0x04006AF2 RID: 27378
	public bool _CRYSTAL_EFFECT;

	// Token: 0x04006AF3 RID: 27379
	public bool _DAY_CYCLE_BRIGHTNESS__OPTION_1;

	// Token: 0x04006AF4 RID: 27380
	public bool _DAY_CYCLE_BRIGHTNESS__OPTION_2;

	// Token: 0x04006AF5 RID: 27381
	public bool _DEBUG_PAWN_DATA;

	// Token: 0x04006AF6 RID: 27382
	public bool _EMISSION;

	// Token: 0x04006AF7 RID: 27383
	public bool _EMISSION_USE_UV_WAVE_WARP;

	// Token: 0x04006AF8 RID: 27384
	public bool _EYECOMP;

	// Token: 0x04006AF9 RID: 27385
	public bool _FX_LAVA_LAMP;

	// Token: 0x04006AFA RID: 27386
	public bool _GLOBAL_ZONE_LIQUID_TYPE__LAVA;

	// Token: 0x04006AFB RID: 27387
	public bool _GLOBAL_ZONE_LIQUID_TYPE__WATER;

	// Token: 0x04006AFC RID: 27388
	public bool _GRADIENT_MAP_ON;

	// Token: 0x04006AFD RID: 27389
	public bool _GRID_EFFECT;

	// Token: 0x04006AFE RID: 27390
	public bool _GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY;

	// Token: 0x04006AFF RID: 27391
	public bool _GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z;

	// Token: 0x04006B00 RID: 27392
	public bool _GT_EDITOR_TIME;

	// Token: 0x04006B01 RID: 27393
	public bool _GT_RIM_LIGHT;

	// Token: 0x04006B02 RID: 27394
	public bool _GT_RIM_LIGHT_FLAT;

	// Token: 0x04006B03 RID: 27395
	public bool _GT_RIM_LIGHT_USE_ALPHA;

	// Token: 0x04006B04 RID: 27396
	public bool _HALF_LAMBERT_TERM;

	// Token: 0x04006B05 RID: 27397
	public bool _HEIGHT_BASED_WATER_EFFECT;

	// Token: 0x04006B06 RID: 27398
	public bool _INNER_GLOW;

	// Token: 0x04006B07 RID: 27399
	public bool _LIQUID_CONTAINER;

	// Token: 0x04006B08 RID: 27400
	public bool _LIQUID_VOLUME;

	// Token: 0x04006B09 RID: 27401
	public bool _MAINTEX_ROTATE;

	// Token: 0x04006B0A RID: 27402
	public bool _MASK_MAP_ON;

	// Token: 0x04006B0B RID: 27403
	public bool _MOUTHCOMP;

	// Token: 0x04006B0C RID: 27404
	public bool _PARALLAX;

	// Token: 0x04006B0D RID: 27405
	public bool _PARALLAX_AA;

	// Token: 0x04006B0E RID: 27406
	public bool _PARALLAX_PLANAR;

	// Token: 0x04006B0F RID: 27407
	public bool _REFLECTIONS;

	// Token: 0x04006B10 RID: 27408
	public bool _REFLECTIONS_ALBEDO_TINT;

	// Token: 0x04006B11 RID: 27409
	public bool _REFLECTIONS_BOX_PROJECT;

	// Token: 0x04006B12 RID: 27410
	public bool _REFLECTIONS_MATCAP;

	// Token: 0x04006B13 RID: 27411
	public bool _REFLECTIONS_MATCAP_PERSP_AWARE;

	// Token: 0x04006B14 RID: 27412
	public bool _REFLECTIONS_USE_NORMAL_TEX;

	// Token: 0x04006B15 RID: 27413
	public bool _SPECULAR_HIGHLIGHT;

	// Token: 0x04006B16 RID: 27414
	public bool _STEALTH_EFFECT;

	// Token: 0x04006B17 RID: 27415
	public bool _TEXEL_SNAP_UVS;

	// Token: 0x04006B18 RID: 27416
	public bool _UNITY_EDIT_MODE;

	// Token: 0x04006B19 RID: 27417
	public bool _USE_DAY_NIGHT_LIGHTMAP;

	// Token: 0x04006B1A RID: 27418
	public bool _USE_DEFORM_MAP;

	// Token: 0x04006B1B RID: 27419
	public bool _USE_TEX_ARRAY_ATLAS;

	// Token: 0x04006B1C RID: 27420
	public bool _USE_TEXTURE;

	// Token: 0x04006B1D RID: 27421
	public bool _USE_VERTEX_COLOR;

	// Token: 0x04006B1E RID: 27422
	public bool _USE_WEATHER_MAP;

	// Token: 0x04006B1F RID: 27423
	public bool _UV_SHIFT;

	// Token: 0x04006B20 RID: 27424
	public bool _UV_SOURCE__UV0;

	// Token: 0x04006B21 RID: 27425
	public bool _UV_SOURCE__WORLD_PLANAR_Y;

	// Token: 0x04006B22 RID: 27426
	public bool _UV_WAVE_WARP;

	// Token: 0x04006B23 RID: 27427
	public bool _VERTEX_ANIM_FLAP;

	// Token: 0x04006B24 RID: 27428
	public bool _VERTEX_ANIM_WAVE;

	// Token: 0x04006B25 RID: 27429
	public bool _VERTEX_ANIM_WAVE_DEBUG;

	// Token: 0x04006B26 RID: 27430
	public bool _VERTEX_ROTATE;

	// Token: 0x04006B27 RID: 27431
	public bool _WATER_CAUSTICS;

	// Token: 0x04006B28 RID: 27432
	public bool _WATER_EFFECT;

	// Token: 0x04006B29 RID: 27433
	public bool _ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX;

	// Token: 0x04006B2A RID: 27434
	public bool _ZONE_LIQUID_SHAPE__CYLINDER;

	// Token: 0x04006B2B RID: 27435
	public bool DIRLIGHTMAP_COMBINED;

	// Token: 0x04006B2C RID: 27436
	public bool INSTANCING_ON;

	// Token: 0x04006B2D RID: 27437
	public bool LIGHTMAP_ON;

	// Token: 0x04006B2E RID: 27438
	public bool STEREO_CUBEMAP_RENDER_ON;

	// Token: 0x04006B2F RID: 27439
	public bool STEREO_INSTANCING_ON;

	// Token: 0x04006B30 RID: 27440
	public bool STEREO_MULTIVIEW_ON;

	// Token: 0x04006B31 RID: 27441
	public bool UNITY_SINGLE_PASS_STEREO;

	// Token: 0x04006B32 RID: 27442
	public bool USE_TEXTURE__AS_MASK;
}
