using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000A0F RID: 2575
public static class UberShader
{
	// Token: 0x1700063D RID: 1597
	// (get) Token: 0x0600420A RID: 16906 RVA: 0x0015FA6D File Offset: 0x0015DC6D
	public static Material ReferenceMaterial
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceMaterial;
		}
	}

	// Token: 0x1700063E RID: 1598
	// (get) Token: 0x0600420B RID: 16907 RVA: 0x0015FA79 File Offset: 0x0015DC79
	public static Shader ReferenceShader
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceShader;
		}
	}

	// Token: 0x1700063F RID: 1599
	// (get) Token: 0x0600420C RID: 16908 RVA: 0x0015FA85 File Offset: 0x0015DC85
	public static Material ReferenceMaterialNonSRP
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceMaterialNonSRP;
		}
	}

	// Token: 0x17000640 RID: 1600
	// (get) Token: 0x0600420D RID: 16909 RVA: 0x0015FA91 File Offset: 0x0015DC91
	public static Shader ReferenceShaderNonSRP
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceShaderNonSRP;
		}
	}

	// Token: 0x17000641 RID: 1601
	// (get) Token: 0x0600420E RID: 16910 RVA: 0x0015FA9D File Offset: 0x0015DC9D
	public static UberShaderProperty[] AllProperties
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kProperties;
		}
	}

	// Token: 0x0600420F RID: 16911 RVA: 0x0015FAAC File Offset: 0x0015DCAC
	public static bool IsAnimated(Material m)
	{
		if (m == null)
		{
			return false;
		}
		if ((double)UberShader.UvShiftToggle.GetValue<float>(m) <= 0.5)
		{
			return false;
		}
		Vector2 value = UberShader.UvShiftRate.GetValue<Vector2>(m);
		return value.x > 0f || value.y > 0f;
	}

	// Token: 0x06004210 RID: 16912 RVA: 0x0015FB07 File Offset: 0x0015DD07
	private static UberShaderProperty GetProperty(int i)
	{
		UberShader.InitDependencies();
		return UberShader.kProperties[i];
	}

	// Token: 0x06004211 RID: 16913 RVA: 0x0015FB07 File Offset: 0x0015DD07
	private static UberShaderProperty GetProperty(int i, string expectedName)
	{
		UberShader.InitDependencies();
		return UberShader.kProperties[i];
	}

	// Token: 0x06004212 RID: 16914 RVA: 0x0015FB18 File Offset: 0x0015DD18
	private static void InitDependencies()
	{
		if (UberShader.gInitialized)
		{
			return;
		}
		UberShader.kReferenceShader = Shader.Find("GorillaTag/UberShader");
		UberShader.kReferenceMaterial = new Material(UberShader.kReferenceShader);
		UberShader.kReferenceShaderNonSRP = Shader.Find("GorillaTag/UberShaderNonSRP");
		UberShader.kReferenceMaterialNonSRP = new Material(UberShader.kReferenceShaderNonSRP);
		UberShader.kProperties = UberShader.EnumerateAllProperties(UberShader.kReferenceShader);
		UberShader.gInitialized = true;
	}

	// Token: 0x06004213 RID: 16915 RVA: 0x0015FA79 File Offset: 0x0015DC79
	public static Shader GetShader()
	{
		UberShader.InitDependencies();
		return UberShader.kReferenceShader;
	}

	// Token: 0x06004214 RID: 16916 RVA: 0x0015FB80 File Offset: 0x0015DD80
	private static UberShaderProperty[] EnumerateAllProperties(Shader uberShader)
	{
		int propertyCount = uberShader.GetPropertyCount();
		UberShaderProperty[] array = new UberShaderProperty[propertyCount];
		for (int i = 0; i < propertyCount; i++)
		{
			UberShaderProperty uberShaderProperty = new UberShaderProperty
			{
				index = i,
				flags = uberShader.GetPropertyFlags(i),
				type = uberShader.GetPropertyType(i),
				nameID = uberShader.GetPropertyNameId(i),
				name = uberShader.GetPropertyName(i),
				attributes = uberShader.GetPropertyAttributes(i)
			};
			if (uberShaderProperty.type == ShaderPropertyType.Range)
			{
				uberShaderProperty.rangeLimits = uberShader.GetPropertyRangeLimits(uberShaderProperty.index);
			}
			string[] attributes = uberShaderProperty.attributes;
			if (attributes != null && attributes.Length != 0)
			{
				foreach (string text in attributes)
				{
					if (!string.IsNullOrWhiteSpace(text))
					{
						bool flag = text.StartsWith("Toggle(");
						uberShaderProperty.isKeywordToggle = flag;
						if (flag)
						{
							string text2 = text.Split('(', StringSplitOptions.RemoveEmptyEntries)[1].RemoveEnd(")", StringComparison.InvariantCulture);
							uberShaderProperty.keyword = text2;
						}
					}
				}
			}
			array[i] = uberShaderProperty;
		}
		return array;
	}

	// Token: 0x040052CC RID: 21196
	private static Shader kReferenceShader;

	// Token: 0x040052CD RID: 21197
	private static Material kReferenceMaterial;

	// Token: 0x040052CE RID: 21198
	private static Shader kReferenceShaderNonSRP;

	// Token: 0x040052CF RID: 21199
	private static Material kReferenceMaterialNonSRP;

	// Token: 0x040052D0 RID: 21200
	private static UberShaderProperty[] kProperties;

	// Token: 0x040052D1 RID: 21201
	private static bool gInitialized = false;

	// Token: 0x040052D2 RID: 21202
	public static UberShaderProperty TransparencyMode = UberShader.GetProperty(0);

	// Token: 0x040052D3 RID: 21203
	public static UberShaderProperty Cutoff = UberShader.GetProperty(1);

	// Token: 0x040052D4 RID: 21204
	public static UberShaderProperty ColorSource = UberShader.GetProperty(2);

	// Token: 0x040052D5 RID: 21205
	public static UberShaderProperty BaseColor = UberShader.GetProperty(3);

	// Token: 0x040052D6 RID: 21206
	public static UberShaderProperty GChannelColor = UberShader.GetProperty(4);

	// Token: 0x040052D7 RID: 21207
	public static UberShaderProperty BChannelColor = UberShader.GetProperty(5);

	// Token: 0x040052D8 RID: 21208
	public static UberShaderProperty AChannelColor = UberShader.GetProperty(6);

	// Token: 0x040052D9 RID: 21209
	public static UberShaderProperty BaseMap = UberShader.GetProperty(7);

	// Token: 0x040052DA RID: 21210
	public static UberShaderProperty BaseMap_WH = UberShader.GetProperty(8);

	// Token: 0x040052DB RID: 21211
	public static UberShaderProperty TexelSnapToggle = UberShader.GetProperty(9);

	// Token: 0x040052DC RID: 21212
	public static UberShaderProperty TexelSnap_Factor = UberShader.GetProperty(10);

	// Token: 0x040052DD RID: 21213
	public static UberShaderProperty UVSource = UberShader.GetProperty(11);

	// Token: 0x040052DE RID: 21214
	public static UberShaderProperty AlphaDetailToggle = UberShader.GetProperty(12);

	// Token: 0x040052DF RID: 21215
	public static UberShaderProperty AlphaDetail_ST = UberShader.GetProperty(13);

	// Token: 0x040052E0 RID: 21216
	public static UberShaderProperty AlphaDetail_Opacity = UberShader.GetProperty(14);

	// Token: 0x040052E1 RID: 21217
	public static UberShaderProperty AlphaDetail_WorldSpace = UberShader.GetProperty(15);

	// Token: 0x040052E2 RID: 21218
	public static UberShaderProperty MaskMapToggle = UberShader.GetProperty(16);

	// Token: 0x040052E3 RID: 21219
	public static UberShaderProperty MaskMap = UberShader.GetProperty(17);

	// Token: 0x040052E4 RID: 21220
	public static UberShaderProperty MaskMap_WH = UberShader.GetProperty(18);

	// Token: 0x040052E5 RID: 21221
	public static UberShaderProperty LavaLampToggle = UberShader.GetProperty(19);

	// Token: 0x040052E6 RID: 21222
	public static UberShaderProperty GradientMapToggle = UberShader.GetProperty(20);

	// Token: 0x040052E7 RID: 21223
	public static UberShaderProperty GradientMap = UberShader.GetProperty(21);

	// Token: 0x040052E8 RID: 21224
	public static UberShaderProperty DoTextureRotation = UberShader.GetProperty(22);

	// Token: 0x040052E9 RID: 21225
	public static UberShaderProperty RotateAngle = UberShader.GetProperty(23);

	// Token: 0x040052EA RID: 21226
	public static UberShaderProperty RotateAnim = UberShader.GetProperty(24);

	// Token: 0x040052EB RID: 21227
	public static UberShaderProperty UseWaveWarp = UberShader.GetProperty(25);

	// Token: 0x040052EC RID: 21228
	public static UberShaderProperty WaveAmplitude = UberShader.GetProperty(26);

	// Token: 0x040052ED RID: 21229
	public static UberShaderProperty WaveFrequency = UberShader.GetProperty(27);

	// Token: 0x040052EE RID: 21230
	public static UberShaderProperty WaveScale = UberShader.GetProperty(28);

	// Token: 0x040052EF RID: 21231
	public static UberShaderProperty WaveTimeScale = UberShader.GetProperty(29);

	// Token: 0x040052F0 RID: 21232
	public static UberShaderProperty UseWeatherMap = UberShader.GetProperty(30);

	// Token: 0x040052F1 RID: 21233
	public static UberShaderProperty WeatherMap = UberShader.GetProperty(31);

	// Token: 0x040052F2 RID: 21234
	public static UberShaderProperty WeatherMapDissolveEdgeSize = UberShader.GetProperty(32);

	// Token: 0x040052F3 RID: 21235
	public static UberShaderProperty ReflectToggle = UberShader.GetProperty(33);

	// Token: 0x040052F4 RID: 21236
	public static UberShaderProperty ReflectBoxProjectToggle = UberShader.GetProperty(34);

	// Token: 0x040052F5 RID: 21237
	public static UberShaderProperty ReflectBoxCubePos = UberShader.GetProperty(35);

	// Token: 0x040052F6 RID: 21238
	public static UberShaderProperty ReflectBoxSize = UberShader.GetProperty(36);

	// Token: 0x040052F7 RID: 21239
	public static UberShaderProperty ReflectBoxRotation = UberShader.GetProperty(37);

	// Token: 0x040052F8 RID: 21240
	public static UberShaderProperty ReflectMatcapToggle = UberShader.GetProperty(38);

	// Token: 0x040052F9 RID: 21241
	public static UberShaderProperty ReflectMatcapPerspToggle = UberShader.GetProperty(39);

	// Token: 0x040052FA RID: 21242
	public static UberShaderProperty ReflectNormalToggle = UberShader.GetProperty(40);

	// Token: 0x040052FB RID: 21243
	public static UberShaderProperty ReflectTex = UberShader.GetProperty(41);

	// Token: 0x040052FC RID: 21244
	public static UberShaderProperty ReflectNormalTex = UberShader.GetProperty(42);

	// Token: 0x040052FD RID: 21245
	public static UberShaderProperty ReflectAlbedoTint = UberShader.GetProperty(43);

	// Token: 0x040052FE RID: 21246
	public static UberShaderProperty ReflectTint = UberShader.GetProperty(44);

	// Token: 0x040052FF RID: 21247
	public static UberShaderProperty ReflectOpacity = UberShader.GetProperty(45);

	// Token: 0x04005300 RID: 21248
	public static UberShaderProperty ReflectExposure = UberShader.GetProperty(46);

	// Token: 0x04005301 RID: 21249
	public static UberShaderProperty ReflectOffset = UberShader.GetProperty(47);

	// Token: 0x04005302 RID: 21250
	public static UberShaderProperty ReflectScale = UberShader.GetProperty(48);

	// Token: 0x04005303 RID: 21251
	public static UberShaderProperty ReflectRotate = UberShader.GetProperty(49);

	// Token: 0x04005304 RID: 21252
	public static UberShaderProperty HalfLambertToggle = UberShader.GetProperty(50);

	// Token: 0x04005305 RID: 21253
	public static UberShaderProperty ZFightOffset = UberShader.GetProperty(51);

	// Token: 0x04005306 RID: 21254
	public static UberShaderProperty ParallaxPlanarToggle = UberShader.GetProperty(52);

	// Token: 0x04005307 RID: 21255
	public static UberShaderProperty ParallaxToggle = UberShader.GetProperty(53);

	// Token: 0x04005308 RID: 21256
	public static UberShaderProperty ParallaxAAToggle = UberShader.GetProperty(54);

	// Token: 0x04005309 RID: 21257
	public static UberShaderProperty ParallaxAABias = UberShader.GetProperty(55);

	// Token: 0x0400530A RID: 21258
	public static UberShaderProperty DepthMap = UberShader.GetProperty(56);

	// Token: 0x0400530B RID: 21259
	public static UberShaderProperty ParallaxAmplitude = UberShader.GetProperty(57);

	// Token: 0x0400530C RID: 21260
	public static UberShaderProperty ParallaxSamplesMinMax = UberShader.GetProperty(58);

	// Token: 0x0400530D RID: 21261
	public static UberShaderProperty UvShiftToggle = UberShader.GetProperty(59);

	// Token: 0x0400530E RID: 21262
	public static UberShaderProperty UvShiftSteps = UberShader.GetProperty(60);

	// Token: 0x0400530F RID: 21263
	public static UberShaderProperty UvShiftRate = UberShader.GetProperty(61);

	// Token: 0x04005310 RID: 21264
	public static UberShaderProperty UvShiftOffset = UberShader.GetProperty(62);

	// Token: 0x04005311 RID: 21265
	public static UberShaderProperty UseGridEffect = UberShader.GetProperty(63);

	// Token: 0x04005312 RID: 21266
	public static UberShaderProperty UseCrystalEffect = UberShader.GetProperty(64);

	// Token: 0x04005313 RID: 21267
	public static UberShaderProperty CrystalPower = UberShader.GetProperty(65);

	// Token: 0x04005314 RID: 21268
	public static UberShaderProperty CrystalRimColor = UberShader.GetProperty(66);

	// Token: 0x04005315 RID: 21269
	public static UberShaderProperty LiquidVolume = UberShader.GetProperty(67);

	// Token: 0x04005316 RID: 21270
	public static UberShaderProperty LiquidFill = UberShader.GetProperty(68);

	// Token: 0x04005317 RID: 21271
	public static UberShaderProperty LiquidFillNormal = UberShader.GetProperty(69);

	// Token: 0x04005318 RID: 21272
	public static UberShaderProperty LiquidSurfaceColor = UberShader.GetProperty(70);

	// Token: 0x04005319 RID: 21273
	public static UberShaderProperty LiquidSwayX = UberShader.GetProperty(71);

	// Token: 0x0400531A RID: 21274
	public static UberShaderProperty LiquidSwayY = UberShader.GetProperty(72);

	// Token: 0x0400531B RID: 21275
	public static UberShaderProperty LiquidContainer = UberShader.GetProperty(73);

	// Token: 0x0400531C RID: 21276
	public static UberShaderProperty LiquidPlanePosition = UberShader.GetProperty(74);

	// Token: 0x0400531D RID: 21277
	public static UberShaderProperty LiquidPlaneNormal = UberShader.GetProperty(75);

	// Token: 0x0400531E RID: 21278
	public static UberShaderProperty VertexFlapToggle = UberShader.GetProperty(76);

	// Token: 0x0400531F RID: 21279
	public static UberShaderProperty VertexFlapAxis = UberShader.GetProperty(77);

	// Token: 0x04005320 RID: 21280
	public static UberShaderProperty VertexFlapDegreesMinMax = UberShader.GetProperty(78);

	// Token: 0x04005321 RID: 21281
	public static UberShaderProperty VertexFlapSpeed = UberShader.GetProperty(79);

	// Token: 0x04005322 RID: 21282
	public static UberShaderProperty VertexFlapPhaseOffset = UberShader.GetProperty(80);

	// Token: 0x04005323 RID: 21283
	public static UberShaderProperty VertexWaveToggle = UberShader.GetProperty(81);

	// Token: 0x04005324 RID: 21284
	public static UberShaderProperty VertexWaveDebug = UberShader.GetProperty(82);

	// Token: 0x04005325 RID: 21285
	public static UberShaderProperty VertexWaveEnd = UberShader.GetProperty(83);

	// Token: 0x04005326 RID: 21286
	public static UberShaderProperty VertexWaveParams = UberShader.GetProperty(84);

	// Token: 0x04005327 RID: 21287
	public static UberShaderProperty VertexWaveFalloff = UberShader.GetProperty(85);

	// Token: 0x04005328 RID: 21288
	public static UberShaderProperty VertexWaveSphereMask = UberShader.GetProperty(86);

	// Token: 0x04005329 RID: 21289
	public static UberShaderProperty VertexWavePhaseOffset = UberShader.GetProperty(87);

	// Token: 0x0400532A RID: 21290
	public static UberShaderProperty VertexWaveAxes = UberShader.GetProperty(88);

	// Token: 0x0400532B RID: 21291
	public static UberShaderProperty VertexRotateToggle = UberShader.GetProperty(89);

	// Token: 0x0400532C RID: 21292
	public static UberShaderProperty VertexRotateAngles = UberShader.GetProperty(90);

	// Token: 0x0400532D RID: 21293
	public static UberShaderProperty VertexRotateAnim = UberShader.GetProperty(91);

	// Token: 0x0400532E RID: 21294
	public static UberShaderProperty VertexLightToggle = UberShader.GetProperty(92);

	// Token: 0x0400532F RID: 21295
	public static UberShaderProperty InnerGlowOn = UberShader.GetProperty(93);

	// Token: 0x04005330 RID: 21296
	public static UberShaderProperty InnerGlowColor = UberShader.GetProperty(94);

	// Token: 0x04005331 RID: 21297
	public static UberShaderProperty InnerGlowParams = UberShader.GetProperty(95);

	// Token: 0x04005332 RID: 21298
	public static UberShaderProperty InnerGlowTap = UberShader.GetProperty(96);

	// Token: 0x04005333 RID: 21299
	public static UberShaderProperty InnerGlowSine = UberShader.GetProperty(97);

	// Token: 0x04005334 RID: 21300
	public static UberShaderProperty InnerGlowSinePeriod = UberShader.GetProperty(98);

	// Token: 0x04005335 RID: 21301
	public static UberShaderProperty InnerGlowSinePhaseShift = UberShader.GetProperty(99);

	// Token: 0x04005336 RID: 21302
	public static UberShaderProperty StealthEffectOn = UberShader.GetProperty(100);

	// Token: 0x04005337 RID: 21303
	public static UberShaderProperty UseEyeTracking = UberShader.GetProperty(101);

	// Token: 0x04005338 RID: 21304
	public static UberShaderProperty EyeTileOffsetUV = UberShader.GetProperty(102);

	// Token: 0x04005339 RID: 21305
	public static UberShaderProperty EyeOverrideUV = UberShader.GetProperty(103);

	// Token: 0x0400533A RID: 21306
	public static UberShaderProperty EyeOverrideUVTransform = UberShader.GetProperty(104);

	// Token: 0x0400533B RID: 21307
	public static UberShaderProperty UseMouthFlap = UberShader.GetProperty(105);

	// Token: 0x0400533C RID: 21308
	public static UberShaderProperty MouthMap = UberShader.GetProperty(106);

	// Token: 0x0400533D RID: 21309
	public static UberShaderProperty MouthMap_Atlas = UberShader.GetProperty(107);

	// Token: 0x0400533E RID: 21310
	public static UberShaderProperty MouthMap_AtlasSlice = UberShader.GetProperty(108);

	// Token: 0x0400533F RID: 21311
	public static UberShaderProperty UseVertexColor = UberShader.GetProperty(109);

	// Token: 0x04005340 RID: 21312
	public static UberShaderProperty WaterEffect = UberShader.GetProperty(110);

	// Token: 0x04005341 RID: 21313
	public static UberShaderProperty HeightBasedWaterEffect = UberShader.GetProperty(111);

	// Token: 0x04005342 RID: 21314
	public static UberShaderProperty UseDayNightLightmap = UberShader.GetProperty(112);

	// Token: 0x04005343 RID: 21315
	public static UberShaderProperty UseSpecular = UberShader.GetProperty(113);

	// Token: 0x04005344 RID: 21316
	public static UberShaderProperty UseSpecularAlphaChannel = UberShader.GetProperty(114);

	// Token: 0x04005345 RID: 21317
	public static UberShaderProperty Smoothness = UberShader.GetProperty(115);

	// Token: 0x04005346 RID: 21318
	public static UberShaderProperty UseSpecHighlight = UberShader.GetProperty(116);

	// Token: 0x04005347 RID: 21319
	public static UberShaderProperty SpecularDir = UberShader.GetProperty(117);

	// Token: 0x04005348 RID: 21320
	public static UberShaderProperty SpecularPowerIntensity = UberShader.GetProperty(118);

	// Token: 0x04005349 RID: 21321
	public static UberShaderProperty SpecularColor = UberShader.GetProperty(119);

	// Token: 0x0400534A RID: 21322
	public static UberShaderProperty SpecularUseDiffuseColor = UberShader.GetProperty(120);

	// Token: 0x0400534B RID: 21323
	public static UberShaderProperty EmissionToggle = UberShader.GetProperty(121);

	// Token: 0x0400534C RID: 21324
	public static UberShaderProperty EmissionColor = UberShader.GetProperty(122);

	// Token: 0x0400534D RID: 21325
	public static UberShaderProperty EmissionMap = UberShader.GetProperty(123);

	// Token: 0x0400534E RID: 21326
	public static UberShaderProperty EmissionMaskByBaseMapAlpha = UberShader.GetProperty(124);

	// Token: 0x0400534F RID: 21327
	public static UberShaderProperty EmissionUVScrollSpeed = UberShader.GetProperty(125);

	// Token: 0x04005350 RID: 21328
	public static UberShaderProperty EmissionDissolveProgress = UberShader.GetProperty(126);

	// Token: 0x04005351 RID: 21329
	public static UberShaderProperty EmissionDissolveAnimation = UberShader.GetProperty(127);

	// Token: 0x04005352 RID: 21330
	public static UberShaderProperty EmissionDissolveEdgeSize = UberShader.GetProperty(128);

	// Token: 0x04005353 RID: 21331
	public static UberShaderProperty EmissionUseUVWaveWarp = UberShader.GetProperty(129);

	// Token: 0x04005354 RID: 21332
	public static UberShaderProperty GreyZoneException = UberShader.GetProperty(130);

	// Token: 0x04005355 RID: 21333
	public static UberShaderProperty Cull = UberShader.GetProperty(131);

	// Token: 0x04005356 RID: 21334
	public static UberShaderProperty StencilReference = UberShader.GetProperty(132);

	// Token: 0x04005357 RID: 21335
	public static UberShaderProperty StencilComparison = UberShader.GetProperty(133);

	// Token: 0x04005358 RID: 21336
	public static UberShaderProperty StencilPassFront = UberShader.GetProperty(134);

	// Token: 0x04005359 RID: 21337
	public static UberShaderProperty USE_DEFORM_MAP = UberShader.GetProperty(135);

	// Token: 0x0400535A RID: 21338
	public static UberShaderProperty DeformMap = UberShader.GetProperty(136);

	// Token: 0x0400535B RID: 21339
	public static UberShaderProperty DeformMapIntensity = UberShader.GetProperty(137);

	// Token: 0x0400535C RID: 21340
	public static UberShaderProperty DeformMapMaskByVertColorRAmount = UberShader.GetProperty(138);

	// Token: 0x0400535D RID: 21341
	public static UberShaderProperty DeformMapScrollSpeed = UberShader.GetProperty(139);

	// Token: 0x0400535E RID: 21342
	public static UberShaderProperty DeformMapUV0Influence = UberShader.GetProperty(140);

	// Token: 0x0400535F RID: 21343
	public static UberShaderProperty DeformMapObjectSpaceOffsetsU = UberShader.GetProperty(141);

	// Token: 0x04005360 RID: 21344
	public static UberShaderProperty DeformMapObjectSpaceOffsetsV = UberShader.GetProperty(142);

	// Token: 0x04005361 RID: 21345
	public static UberShaderProperty DeformMapWorldSpaceOffsetsU = UberShader.GetProperty(143);

	// Token: 0x04005362 RID: 21346
	public static UberShaderProperty DeformMapWorldSpaceOffsetsV = UberShader.GetProperty(144);

	// Token: 0x04005363 RID: 21347
	public static UberShaderProperty RotateOnYAxisBySinTime = UberShader.GetProperty(145);

	// Token: 0x04005364 RID: 21348
	public static UberShaderProperty USE_TEX_ARRAY_ATLAS = UberShader.GetProperty(146);

	// Token: 0x04005365 RID: 21349
	public static UberShaderProperty BaseMap_Atlas = UberShader.GetProperty(147);

	// Token: 0x04005366 RID: 21350
	public static UberShaderProperty BaseMap_AtlasSlice = UberShader.GetProperty(148);

	// Token: 0x04005367 RID: 21351
	public static UberShaderProperty EmissionMap_Atlas = UberShader.GetProperty(149);

	// Token: 0x04005368 RID: 21352
	public static UberShaderProperty EmissionMap_AtlasSlice = UberShader.GetProperty(150);

	// Token: 0x04005369 RID: 21353
	public static UberShaderProperty DeformMap_Atlas = UberShader.GetProperty(151);

	// Token: 0x0400536A RID: 21354
	public static UberShaderProperty DeformMap_AtlasSlice = UberShader.GetProperty(152);

	// Token: 0x0400536B RID: 21355
	public static UberShaderProperty DEBUG_PAWN_DATA = UberShader.GetProperty(153);

	// Token: 0x0400536C RID: 21356
	public static UberShaderProperty SrcBlend = UberShader.GetProperty(154);

	// Token: 0x0400536D RID: 21357
	public static UberShaderProperty DstBlend = UberShader.GetProperty(155);

	// Token: 0x0400536E RID: 21358
	public static UberShaderProperty SrcBlendAlpha = UberShader.GetProperty(156);

	// Token: 0x0400536F RID: 21359
	public static UberShaderProperty DstBlendAlpha = UberShader.GetProperty(157);

	// Token: 0x04005370 RID: 21360
	public static UberShaderProperty ZWrite = UberShader.GetProperty(158);

	// Token: 0x04005371 RID: 21361
	public static UberShaderProperty AlphaToMask = UberShader.GetProperty(159);

	// Token: 0x04005372 RID: 21362
	public static UberShaderProperty Color = UberShader.GetProperty(160);

	// Token: 0x04005373 RID: 21363
	public static UberShaderProperty Surface = UberShader.GetProperty(161);

	// Token: 0x04005374 RID: 21364
	public static UberShaderProperty Metallic = UberShader.GetProperty(162);

	// Token: 0x04005375 RID: 21365
	public static UberShaderProperty SpecColor = UberShader.GetProperty(163);

	// Token: 0x04005376 RID: 21366
	public static UberShaderProperty DayNightLightmapArray = UberShader.GetProperty(164);

	// Token: 0x04005377 RID: 21367
	public static UberShaderProperty DayNightLightmapArray_AtlasSlice = UberShader.GetProperty(165);

	// Token: 0x04005378 RID: 21368
	public static UberShaderProperty SingleLightmap = UberShader.GetProperty(166);
}
