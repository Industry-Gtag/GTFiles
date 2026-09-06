using System;
using UnityEngine;

namespace GorillaTag.Rendering.Shaders
{
	// Token: 0x020012C5 RID: 4805
	public class ShaderConfigData
	{
		// Token: 0x06007897 RID: 30871 RVA: 0x00271C30 File Offset: 0x0026FE30
		public static ShaderConfigData.MatPropInt[] convertInts(string[] names, int[] vals)
		{
			ShaderConfigData.MatPropInt[] array = new ShaderConfigData.MatPropInt[names.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new ShaderConfigData.MatPropInt
				{
					intName = names[i],
					intVal = vals[i]
				};
			}
			return array;
		}

		// Token: 0x06007898 RID: 30872 RVA: 0x00271C7C File Offset: 0x0026FE7C
		public static ShaderConfigData.MatPropFloat[] convertFloats(string[] names, float[] vals)
		{
			ShaderConfigData.MatPropFloat[] array = new ShaderConfigData.MatPropFloat[names.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new ShaderConfigData.MatPropFloat
				{
					floatName = names[i],
					floatVal = vals[i]
				};
			}
			return array;
		}

		// Token: 0x06007899 RID: 30873 RVA: 0x00271CC8 File Offset: 0x0026FEC8
		public static ShaderConfigData.MatPropMatrix[] convertMatrices(string[] names, Matrix4x4[] vals)
		{
			ShaderConfigData.MatPropMatrix[] array = new ShaderConfigData.MatPropMatrix[names.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new ShaderConfigData.MatPropMatrix
				{
					matrixName = names[i],
					matrixVal = vals[i]
				};
			}
			return array;
		}

		// Token: 0x0600789A RID: 30874 RVA: 0x00271D18 File Offset: 0x0026FF18
		public static ShaderConfigData.MatPropVector[] convertVectors(string[] names, Vector4[] vals)
		{
			ShaderConfigData.MatPropVector[] array = new ShaderConfigData.MatPropVector[names.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new ShaderConfigData.MatPropVector
				{
					vectorName = names[i],
					vectorVal = vals[i]
				};
			}
			return array;
		}

		// Token: 0x0600789B RID: 30875 RVA: 0x00271D68 File Offset: 0x0026FF68
		public static ShaderConfigData.MatPropTexture[] convertTextures(string[] names, Texture[] vals)
		{
			ShaderConfigData.MatPropTexture[] array = new ShaderConfigData.MatPropTexture[names.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new ShaderConfigData.MatPropTexture
				{
					textureName = names[i],
					textureVal = vals[i]
				};
			}
			return array;
		}

		// Token: 0x0600789C RID: 30876 RVA: 0x00271DB4 File Offset: 0x0026FFB4
		public static string GetShaderPropertiesStringFromMaterial(Material mat, bool excludeMainTexData)
		{
			string text = "";
			string[] array = mat.GetPropertyNames(MaterialPropertyType.Int);
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = mat.GetInteger(array[i]);
				text += array2[i].ToString();
			}
			array = mat.GetPropertyNames(MaterialPropertyType.Float);
			float[] array3 = new float[array.Length];
			for (int j = 0; j < array.Length; j++)
			{
				if (excludeMainTexData || !array[j].Contains("_BaseMap"))
				{
					array3[j] = mat.GetFloat(array[j]);
					text += array3[j].ToString();
				}
			}
			array = mat.GetPropertyNames(MaterialPropertyType.Matrix);
			Matrix4x4[] array4 = new Matrix4x4[array.Length];
			for (int k = 0; k < array.Length; k++)
			{
				array4[k] = mat.GetMatrix(array[k]);
				text += array4[k].ToString();
			}
			array = mat.GetPropertyNames(MaterialPropertyType.Vector);
			Vector4[] array5 = new Vector4[array.Length];
			for (int l = 0; l < array.Length; l++)
			{
				if (excludeMainTexData || !array[l].Contains("_BaseMap"))
				{
					array5[l] = mat.GetVector(array[l]);
					text += array5[l].ToString();
				}
			}
			array = mat.GetPropertyNames(MaterialPropertyType.Texture);
			Texture[] array6 = new Texture[array.Length];
			for (int m = 0; m < array.Length; m++)
			{
				if (!array[m].Contains("_BaseMap"))
				{
					array6[m] = mat.GetTexture(array[m]);
					if (array6[m] != null)
					{
						text += array6[m].ToString();
					}
				}
			}
			return text;
		}

		// Token: 0x0600789D RID: 30877 RVA: 0x00271F80 File Offset: 0x00270180
		public static ShaderConfigData.ShaderConfig GetConfigDataFromMaterial(Material mat, bool includeMainTexData)
		{
			string[] array = mat.GetPropertyNames(MaterialPropertyType.Int);
			string[] array2 = array;
			int[] array3 = new int[array2.Length];
			bool flag = mat.IsKeywordEnabled("_WATER_EFFECT");
			bool flag2 = mat.IsKeywordEnabled("_MAINTEX_ROTATE");
			bool flag3 = mat.IsKeywordEnabled("_UV_WAVE_WARP");
			bool flag4 = mat.IsKeywordEnabled("_EMISSION_USE_UV_WAVE_WARP");
			bool flag5 = flag3 || flag4;
			bool flag6 = mat.IsKeywordEnabled("_LIQUID_CONTAINER");
			bool flag7 = mat.IsKeywordEnabled("_LIQUID_VOLUME") && !flag6;
			bool flag8 = mat.IsKeywordEnabled("_CRYSTAL_EFFECT");
			bool flag9 = mat.IsKeywordEnabled("_EMISSION") || flag8;
			bool flag10 = mat.IsKeywordEnabled("_REFLECTIONS");
			mat.IsKeywordEnabled("_REFLECTIONS_MATCAP");
			bool flag11 = mat.IsKeywordEnabled("_UV_SHIFT");
			for (int i = 0; i < array2.Length; i++)
			{
				array3[i] = mat.GetInteger(array[i]);
				if (!flag11 && (array[i] == "_UvShiftSteps" || array[i] == "_UvShiftOffset"))
				{
					array3[i] = 0;
				}
			}
			array = mat.GetPropertyNames(MaterialPropertyType.Float);
			string[] array4 = array;
			float[] array5 = new float[array4.Length];
			for (int j = 0; j < array.Length; j++)
			{
				if (includeMainTexData || !array[j].Contains("_BaseMap"))
				{
					array5[j] = mat.GetFloat(array[j]);
				}
				if ((!flag && array[j] == "_HeightBasedWaterEffect") || (!flag2 && array[j] == "_RotateSpeed") || (!flag5 && (array[j] == "_WaveAmplitude" || array[j] == "_WaveFrequency" || array[j] == "_WaveScale")) || (!flag7 && (array[j] == "_LiquidFill" || array[j] == "_LiquidSwayX" || array[j] == "_LiquidSwayY")) || (!flag8 && array[j] == "_CrystalPower") || (!flag9 && array[j].StartsWith("_Emission")) || (!flag10 && (array[j] == "_ReflectOpacity" || array[j] == "_ReflectExposure" || array[j] == "_ReflectRotate")) || (!flag11 && array[j] == "_UvShiftRate"))
				{
					array5[j] = 0f;
				}
			}
			array = mat.GetPropertyNames(MaterialPropertyType.Matrix);
			string[] array6 = array;
			Matrix4x4[] array7 = new Matrix4x4[array6.Length];
			for (int k = 0; k < array.Length; k++)
			{
				array7[k] = mat.GetMatrix(array[k]);
			}
			array = mat.GetPropertyNames(MaterialPropertyType.Vector);
			string[] array8 = array;
			Vector4[] array9 = new Vector4[array8.Length];
			for (int l = 0; l < array.Length; l++)
			{
				if (includeMainTexData || !array[l].Contains("_BaseMap"))
				{
					array9[l] = mat.GetVector(array[l]);
				}
				if ((!flag7 && (array[l] == "_LiquidFillNormal" || array[l] == "_LiquidSurfaceColor")) || (!flag6 && (array[l] == "_LiquidPlanePosition" || array[l] == "_LiquidPlaneNormal")) || (!flag8 && array[l] == "_CrystalRimColor") || (!flag9 && array[l].StartsWith("_Emission")) || (!flag10 && (array[l] == "_ReflectTint" || array[l] == "_ReflectOffset" || array[l] == "_ReflectScale")))
				{
					array9[l] = Vector4.zero;
				}
			}
			array = mat.GetPropertyNames(MaterialPropertyType.Texture);
			string[] array10 = array;
			Texture[] array11 = new Texture[array10.Length];
			for (int m = 0; m < array.Length; m++)
			{
				if (!array[m].Contains("_BaseMap"))
				{
					array11[m] = mat.GetTexture(array[m]);
				}
			}
			return new ShaderConfigData.ShaderConfig(mat.shader.name, mat, array2, array3, array4, array5, array6, array7, array8, array9, array10, array11);
		}

		// Token: 0x020012C6 RID: 4806
		[Serializable]
		public struct ShaderConfig
		{
			// Token: 0x0600789F RID: 30879 RVA: 0x002723A8 File Offset: 0x002705A8
			public ShaderConfig(string shadName, Material fMat, string[] intNames, int[] intVals, string[] floatNames, float[] floatVals, string[] matrixNames, Matrix4x4[] matrixVals, string[] vectorNames, Vector4[] vectorVals, string[] textureNames, Texture[] textureVals)
			{
				this.shaderName = shadName;
				this.firstMat = fMat;
				this.ints = ShaderConfigData.convertInts(intNames, intVals);
				this.floats = ShaderConfigData.convertFloats(floatNames, floatVals);
				this.matrices = ShaderConfigData.convertMatrices(matrixNames, matrixVals);
				this.vectors = ShaderConfigData.convertVectors(vectorNames, vectorVals);
				this.textures = ShaderConfigData.convertTextures(textureNames, textureVals);
			}

			// Token: 0x040088F1 RID: 35057
			public string shaderName;

			// Token: 0x040088F2 RID: 35058
			public Material firstMat;

			// Token: 0x040088F3 RID: 35059
			public ShaderConfigData.MatPropInt[] ints;

			// Token: 0x040088F4 RID: 35060
			public ShaderConfigData.MatPropFloat[] floats;

			// Token: 0x040088F5 RID: 35061
			public ShaderConfigData.MatPropMatrix[] matrices;

			// Token: 0x040088F6 RID: 35062
			public ShaderConfigData.MatPropVector[] vectors;

			// Token: 0x040088F7 RID: 35063
			public ShaderConfigData.MatPropTexture[] textures;
		}

		// Token: 0x020012C7 RID: 4807
		[Serializable]
		public struct MatPropInt
		{
			// Token: 0x040088F8 RID: 35064
			public string intName;

			// Token: 0x040088F9 RID: 35065
			public int intVal;
		}

		// Token: 0x020012C8 RID: 4808
		[Serializable]
		public struct MatPropFloat
		{
			// Token: 0x040088FA RID: 35066
			public string floatName;

			// Token: 0x040088FB RID: 35067
			public float floatVal;
		}

		// Token: 0x020012C9 RID: 4809
		[Serializable]
		public struct MatPropMatrix
		{
			// Token: 0x040088FC RID: 35068
			public string matrixName;

			// Token: 0x040088FD RID: 35069
			public Matrix4x4 matrixVal;
		}

		// Token: 0x020012CA RID: 4810
		[Serializable]
		public struct MatPropVector
		{
			// Token: 0x040088FE RID: 35070
			public string vectorName;

			// Token: 0x040088FF RID: 35071
			public Vector4 vectorVal;
		}

		// Token: 0x020012CB RID: 4811
		[Serializable]
		public struct MatPropTexture
		{
			// Token: 0x04008900 RID: 35072
			public string textureName;

			// Token: 0x04008901 RID: 35073
			public Texture textureVal;
		}

		// Token: 0x020012CC RID: 4812
		[Serializable]
		public struct RenderersForShaderWithSameProperties
		{
			// Token: 0x04008902 RID: 35074
			public MeshRenderer[] renderers;
		}
	}
}
