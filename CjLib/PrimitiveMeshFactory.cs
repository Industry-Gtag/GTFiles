using System;
using System.Collections.Generic;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200140C RID: 5132
	public class PrimitiveMeshFactory
	{
		// Token: 0x06008153 RID: 33107 RVA: 0x002A0544 File Offset: 0x0029E744
		private static Mesh GetPooledLineMesh()
		{
			if (PrimitiveMeshFactory.s_lineMeshPool == null)
			{
				PrimitiveMeshFactory.s_lineMeshPool = new List<Mesh>();
				PrimitiveMeshFactory.s_lineMeshPool.Add(new Mesh());
			}
			if (PrimitiveMeshFactory.s_lastDrawLineFrame != Time.frameCount)
			{
				PrimitiveMeshFactory.s_iPooledMesh = 0;
				PrimitiveMeshFactory.s_lastDrawLineFrame = Time.frameCount;
			}
			if (PrimitiveMeshFactory.s_iPooledMesh == PrimitiveMeshFactory.s_lineMeshPool.Count)
			{
				PrimitiveMeshFactory.s_lineMeshPool.Capacity *= 2;
				for (int i = PrimitiveMeshFactory.s_iPooledMesh; i < PrimitiveMeshFactory.s_lineMeshPool.Capacity; i++)
				{
					PrimitiveMeshFactory.s_lineMeshPool.Add(new Mesh());
				}
			}
			Mesh mesh = PrimitiveMeshFactory.s_lineMeshPool[PrimitiveMeshFactory.s_iPooledMesh++];
			if (mesh == null)
			{
				mesh = (PrimitiveMeshFactory.s_lineMeshPool[PrimitiveMeshFactory.s_iPooledMesh - 1] = new Mesh());
			}
			return mesh;
		}

		// Token: 0x06008154 RID: 33108 RVA: 0x002A0614 File Offset: 0x0029E814
		public static Mesh Line(Vector3 v0, Vector3 v1)
		{
			Mesh pooledLineMesh = PrimitiveMeshFactory.GetPooledLineMesh();
			if (pooledLineMesh == null)
			{
				return null;
			}
			Vector3[] array = new Vector3[] { v0, v1 };
			int[] array2 = new int[] { 0, 1 };
			pooledLineMesh.vertices = array;
			pooledLineMesh.SetIndices(array2, MeshTopology.Lines, 0);
			pooledLineMesh.RecalculateBounds();
			return pooledLineMesh;
		}

		// Token: 0x06008155 RID: 33109 RVA: 0x002A066C File Offset: 0x0029E86C
		public static Mesh Lines(Vector3[] aVert)
		{
			if (aVert.Length <= 1)
			{
				return null;
			}
			Mesh pooledLineMesh = PrimitiveMeshFactory.GetPooledLineMesh();
			if (pooledLineMesh == null)
			{
				return null;
			}
			int[] array = new int[aVert.Length];
			for (int i = 0; i < aVert.Length; i++)
			{
				array[i] = i;
			}
			pooledLineMesh.vertices = aVert;
			pooledLineMesh.SetIndices(array, MeshTopology.Lines, 0);
			return pooledLineMesh;
		}

		// Token: 0x06008156 RID: 33110 RVA: 0x002A06C0 File Offset: 0x0029E8C0
		public static Mesh LineStrip(Vector3[] aVert)
		{
			if (aVert.Length <= 1)
			{
				return null;
			}
			Mesh pooledLineMesh = PrimitiveMeshFactory.GetPooledLineMesh();
			if (pooledLineMesh == null)
			{
				return null;
			}
			int[] array = new int[aVert.Length];
			for (int i = 0; i < aVert.Length; i++)
			{
				array[i] = i;
			}
			pooledLineMesh.vertices = aVert;
			pooledLineMesh.SetIndices(array, MeshTopology.LineStrip, 0);
			return pooledLineMesh;
		}

		// Token: 0x06008157 RID: 33111 RVA: 0x002A0714 File Offset: 0x0029E914
		public static Mesh BoxWireframe()
		{
			if (PrimitiveMeshFactory.s_boxWireframeMesh == null)
			{
				PrimitiveMeshFactory.s_boxWireframeMesh = new Mesh();
				Vector3[] array = new Vector3[]
				{
					new Vector3(-0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, -0.5f, 0.5f),
					new Vector3(-0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, -0.5f, 0.5f)
				};
				int[] array2 = new int[]
				{
					0, 1, 1, 2, 2, 3, 3, 0, 2, 6,
					6, 7, 7, 3, 7, 4, 4, 5, 5, 6,
					5, 1, 1, 0, 0, 4
				};
				PrimitiveMeshFactory.s_boxWireframeMesh.vertices = array;
				PrimitiveMeshFactory.s_boxWireframeMesh.normals = array;
				PrimitiveMeshFactory.s_boxWireframeMesh.SetIndices(array2, MeshTopology.Lines, 0);
			}
			return PrimitiveMeshFactory.s_boxWireframeMesh;
		}

		// Token: 0x06008158 RID: 33112 RVA: 0x002A0858 File Offset: 0x0029EA58
		public static Mesh BoxSolidColor()
		{
			if (PrimitiveMeshFactory.s_boxSolidColorMesh == null)
			{
				PrimitiveMeshFactory.s_boxSolidColorMesh = new Mesh();
				Vector3[] array = new Vector3[]
				{
					new Vector3(-0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, -0.5f, 0.5f),
					new Vector3(-0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, -0.5f, 0.5f)
				};
				int[] array2 = new int[]
				{
					0, 1, 2, 0, 2, 3, 3, 2, 6, 3,
					6, 7, 7, 6, 5, 7, 5, 4, 4, 5,
					1, 4, 1, 0, 1, 5, 6, 1, 6, 2,
					0, 3, 7, 0, 7, 4
				};
				PrimitiveMeshFactory.s_boxSolidColorMesh.vertices = array;
				PrimitiveMeshFactory.s_boxSolidColorMesh.SetIndices(array2, MeshTopology.Triangles, 0);
			}
			return PrimitiveMeshFactory.s_boxSolidColorMesh;
		}

		// Token: 0x06008159 RID: 33113 RVA: 0x002A0990 File Offset: 0x0029EB90
		public static Mesh BoxFlatShaded()
		{
			if (PrimitiveMeshFactory.s_boxFlatShadedMesh == null)
			{
				PrimitiveMeshFactory.s_boxFlatShadedMesh = new Mesh();
				Vector3[] array = new Vector3[]
				{
					new Vector3(-0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, -0.5f, 0.5f),
					new Vector3(-0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, -0.5f, 0.5f)
				};
				Vector3[] array2 = new Vector3[]
				{
					array[0],
					array[1],
					array[2],
					array[0],
					array[2],
					array[3],
					array[3],
					array[2],
					array[6],
					array[3],
					array[6],
					array[7],
					array[7],
					array[6],
					array[5],
					array[7],
					array[5],
					array[4],
					array[4],
					array[5],
					array[1],
					array[4],
					array[1],
					array[0],
					array[1],
					array[5],
					array[6],
					array[1],
					array[6],
					array[2],
					array[0],
					array[3],
					array[7],
					array[0],
					array[7],
					array[4]
				};
				Vector3[] array3 = new Vector3[]
				{
					new Vector3(0f, 0f, -1f),
					new Vector3(1f, 0f, 0f),
					new Vector3(0f, 0f, 1f),
					new Vector3(-1f, 0f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, -1f, 0f)
				};
				Vector3[] array4 = new Vector3[]
				{
					array3[0],
					array3[0],
					array3[0],
					array3[0],
					array3[0],
					array3[0],
					array3[1],
					array3[1],
					array3[1],
					array3[1],
					array3[1],
					array3[1],
					array3[2],
					array3[2],
					array3[2],
					array3[2],
					array3[2],
					array3[2],
					array3[3],
					array3[3],
					array3[3],
					array3[3],
					array3[3],
					array3[3],
					array3[4],
					array3[4],
					array3[4],
					array3[4],
					array3[4],
					array3[4],
					array3[5],
					array3[5],
					array3[5],
					array3[5],
					array3[5],
					array3[5]
				};
				int[] array5 = new int[array2.Length];
				for (int i = 0; i < array5.Length; i++)
				{
					array5[i] = i;
				}
				PrimitiveMeshFactory.s_boxFlatShadedMesh.vertices = array2;
				PrimitiveMeshFactory.s_boxFlatShadedMesh.normals = array4;
				PrimitiveMeshFactory.s_boxFlatShadedMesh.SetIndices(array5, MeshTopology.Triangles, 0);
			}
			return PrimitiveMeshFactory.s_boxFlatShadedMesh;
		}

		// Token: 0x0600815A RID: 33114 RVA: 0x002A0FC4 File Offset: 0x0029F1C4
		public static Mesh RectWireframe()
		{
			if (PrimitiveMeshFactory.s_rectWireframeMesh == null)
			{
				PrimitiveMeshFactory.s_rectWireframeMesh = new Mesh();
				Vector3[] array = new Vector3[]
				{
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, -0.5f)
				};
				int[] array2 = new int[] { 0, 1, 1, 2, 2, 3, 3, 0 };
				PrimitiveMeshFactory.s_rectWireframeMesh.vertices = array;
				PrimitiveMeshFactory.s_rectWireframeMesh.normals = array;
				PrimitiveMeshFactory.s_rectWireframeMesh.SetIndices(array2, MeshTopology.Lines, 0);
			}
			return PrimitiveMeshFactory.s_rectWireframeMesh;
		}

		// Token: 0x0600815B RID: 33115 RVA: 0x002A1098 File Offset: 0x0029F298
		public static Mesh RectSolidColor()
		{
			if (PrimitiveMeshFactory.s_rectSolidColorMesh == null)
			{
				PrimitiveMeshFactory.s_rectSolidColorMesh = new Mesh();
				Vector3[] array = new Vector3[]
				{
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, -0.5f)
				};
				int[] array2 = new int[]
				{
					0, 1, 2, 0, 2, 3, 0, 2, 1, 0,
					3, 2
				};
				PrimitiveMeshFactory.s_rectSolidColorMesh.vertices = array;
				PrimitiveMeshFactory.s_rectSolidColorMesh.SetIndices(array2, MeshTopology.Triangles, 0);
			}
			return PrimitiveMeshFactory.s_rectSolidColorMesh;
		}

		// Token: 0x0600815C RID: 33116 RVA: 0x002A1164 File Offset: 0x0029F364
		public static Mesh RectFlatShaded()
		{
			if (PrimitiveMeshFactory.s_rectFlatShadedMesh == null)
			{
				PrimitiveMeshFactory.s_rectFlatShadedMesh = new Mesh();
				Vector3[] array = new Vector3[]
				{
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, -0.5f)
				};
				Vector3[] array2 = new Vector3[]
				{
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, -1f, 0f),
					new Vector3(0f, -1f, 0f),
					new Vector3(0f, -1f, 0f),
					new Vector3(0f, -1f, 0f)
				};
				int[] array3 = new int[]
				{
					0, 1, 2, 0, 2, 3, 4, 6, 5, 4,
					7, 6
				};
				PrimitiveMeshFactory.s_rectFlatShadedMesh.vertices = array;
				PrimitiveMeshFactory.s_rectFlatShadedMesh.normals = array2;
				PrimitiveMeshFactory.s_rectFlatShadedMesh.SetIndices(array3, MeshTopology.Triangles, 0);
			}
			return PrimitiveMeshFactory.s_rectFlatShadedMesh;
		}

		// Token: 0x0600815D RID: 33117 RVA: 0x002A1384 File Offset: 0x0029F584
		public static Mesh CircleWireframe(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_circleWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_circleWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_circleWireframeMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments];
				int[] array2 = new int[numSegments + 1];
				float num = 6.2831855f / (float)numSegments;
				float num2 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num2) * Vector3.right + Mathf.Sin(num2) * Vector3.forward;
					array2[i] = i;
					num2 += num;
				}
				array2[numSegments] = 0;
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.LineStrip, 0);
				if (PrimitiveMeshFactory.s_circleWireframeMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_circleWireframeMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_circleWireframeMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x0600815E RID: 33118 RVA: 0x002A1478 File Offset: 0x0029F678
		public static Mesh CircleSolidColor(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_circleSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_circleSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_circleSolidColorMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments + 1];
				int[] array2 = new int[numSegments * 6];
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					num3 += num2;
					array2[num++] = numSegments;
					array2[num++] = (i + 1) % numSegments;
					array2[num++] = i;
					array2[num++] = numSegments;
					array2[num++] = i;
					array2[num++] = (i + 1) % numSegments;
				}
				array[numSegments] = Vector3.zero;
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_circleSolidColorMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_circleSolidColorMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_circleSolidColorMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x0600815F RID: 33119 RVA: 0x002A15A8 File Offset: 0x0029F7A8
		public static Mesh CircleFlatShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_circleFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_circleFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_circleFlatShadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[(numSegments + 1) * 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 6];
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					num3 += num2;
					array2[i] = new Vector3(0f, 1f, 0f);
					array3[num++] = numSegments;
					array3[num++] = (i + 1) % numSegments;
					array3[num++] = i;
				}
				array[numSegments] = Vector3.zero;
				array2[numSegments] = new Vector3(0f, 1f, 0f);
				num3 = 0f;
				for (int j = 0; j < numSegments; j++)
				{
					array[j + numSegments + 1] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					num3 -= num2;
					array2[j + numSegments + 1] = new Vector3(0f, -1f, 0f);
					array3[num++] = numSegments * 2 + 1;
					array3[num++] = (j + 1) % numSegments + numSegments + 1;
					array3[num++] = j + (numSegments + 1);
				}
				array[numSegments * 2 + 1] = Vector3.zero;
				array2[numSegments * 2 + 1] = new Vector3(0f, -1f, 0f);
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_circleFlatShadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_circleFlatShadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_circleFlatShadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06008160 RID: 33120 RVA: 0x002A17E4 File Offset: 0x0029F9E4
		public static Mesh CylinderWireframe(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_cylinderWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_cylinderWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_cylinderWireframeMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 2];
				int[] array2 = new int[numSegments * 6];
				Vector3 vector = new Vector3(0f, -0.5f, 0f);
				Vector3 vector2 = new Vector3(0f, 0.5f, 0f);
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					Vector3 vector3 = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					array[i] = vector + vector3;
					array[numSegments + i] = vector2 + vector3;
					array2[num++] = i;
					array2[num++] = (i + 1) % numSegments;
					array2[num++] = i;
					array2[num++] = numSegments + i;
					array2[num++] = numSegments + i;
					array2[num++] = numSegments + (i + 1) % numSegments;
					num3 += num2;
				}
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.Lines, 0);
				if (PrimitiveMeshFactory.s_cylinderWireframeMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_cylinderWireframeMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_cylinderWireframeMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06008161 RID: 33121 RVA: 0x002A1974 File Offset: 0x0029FB74
		public static Mesh CylinderSolidColor(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_cylinderSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_cylinderSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_cylinderSolidColorMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 2 + 2];
				int[] array2 = new int[numSegments * 12];
				Vector3 vector = new Vector3(0f, -0.5f, 0f);
				Vector3 vector2 = new Vector3(0f, 0.5f, 0f);
				int num = 0;
				int num2 = numSegments * 2;
				int num3 = numSegments * 2 + 1;
				array[num2] = vector;
				array[num3] = vector2;
				int num4 = 0;
				float num5 = 6.2831855f / (float)numSegments;
				float num6 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					Vector3 vector3 = Mathf.Cos(num6) * Vector3.right + Mathf.Sin(num6) * Vector3.forward;
					array[num + i] = vector + vector3;
					array[numSegments + i] = vector2 + vector3;
					array2[num4++] = num2;
					array2[num4++] = num + i;
					array2[num4++] = num + (i + 1) % numSegments;
					array2[num4++] = num + i;
					array2[num4++] = numSegments + (i + 1) % numSegments;
					array2[num4++] = num + (i + 1) % numSegments;
					array2[num4++] = num + i;
					array2[num4++] = numSegments + i;
					array2[num4++] = numSegments + (i + 1) % numSegments;
					array2[num4++] = num3;
					array2[num4++] = numSegments + (i + 1) % numSegments;
					array2[num4++] = numSegments + i;
					num6 += num5;
				}
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_cylinderSolidColorMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_cylinderSolidColorMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_cylinderSolidColorMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06008162 RID: 33122 RVA: 0x002A1B8C File Offset: 0x0029FD8C
		public static Mesh CylinderFlatShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 6 + 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 12];
				Vector3 vector = new Vector3(0f, -0.5f, 0f);
				Vector3 vector2 = new Vector3(0f, 0.5f, 0f);
				int num = 0;
				int num2 = numSegments * 2;
				int num3 = numSegments * 6;
				int num4 = numSegments * 6 + 1;
				array[num3] = vector;
				array[num4] = vector2;
				array2[num3] = new Vector3(0f, -1f, 0f);
				array2[num4] = new Vector3(0f, 1f, 0f);
				int num5 = 0;
				float num6 = 6.2831855f / (float)numSegments;
				float num7 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					Vector3 vector3 = Mathf.Cos(num7) * Vector3.right + Mathf.Sin(num7) * Vector3.forward;
					array[num + i] = vector + vector3;
					array[numSegments + i] = vector2 + vector3;
					array2[num + i] = new Vector3(0f, -1f, 0f);
					array2[numSegments + i] = new Vector3(0f, 1f, 0f);
					array3[num5++] = num3;
					array3[num5++] = num + i;
					array3[num5++] = num + (i + 1) % numSegments;
					array3[num5++] = num4;
					array3[num5++] = numSegments + (i + 1) % numSegments;
					array3[num5++] = numSegments + i;
					num7 += num6;
					Vector3 vector4 = Mathf.Cos(num7) * Vector3.right + Mathf.Sin(num7) * Vector3.forward;
					array[num2 + i * 4] = vector + vector3;
					array[num2 + i * 4 + 1] = vector2 + vector3;
					array[num2 + i * 4 + 2] = vector + vector4;
					array[num2 + i * 4 + 3] = vector2 + vector4;
					Vector3 normalized = Vector3.Cross(vector2 - vector, vector4 - vector3).normalized;
					array2[num2 + i * 4] = normalized;
					array2[num2 + i * 4 + 1] = normalized;
					array2[num2 + i * 4 + 2] = normalized;
					array2[num2 + i * 4 + 3] = normalized;
					array3[num5++] = num2 + i * 4;
					array3[num5++] = num2 + i * 4 + 3;
					array3[num5++] = num2 + i * 4 + 2;
					array3[num5++] = num2 + i * 4;
					array3[num5++] = num2 + i * 4 + 1;
					array3[num5++] = num2 + i * 4 + 3;
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06008163 RID: 33123 RVA: 0x002A1F24 File Offset: 0x002A0124
		public static Mesh CylinderSmoothShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 4 + 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 12];
				Vector3 vector = new Vector3(0f, -0.5f, 0f);
				Vector3 vector2 = new Vector3(0f, 0.5f, 0f);
				int num = 0;
				int num2 = numSegments * 2;
				int num3 = numSegments * 4;
				int num4 = numSegments * 4 + 1;
				array[num3] = vector;
				array[num4] = vector2;
				array2[num3] = new Vector3(0f, -1f, 0f);
				array2[num4] = new Vector3(0f, 1f, 0f);
				int num5 = 0;
				float num6 = 6.2831855f / (float)numSegments;
				float num7 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					Vector3 vector3 = Mathf.Cos(num7) * Vector3.right + Mathf.Sin(num7) * Vector3.forward;
					array[num + i] = vector + vector3;
					array[numSegments + i] = vector2 + vector3;
					array2[num + i] = new Vector3(0f, -1f, 0f);
					array2[numSegments + i] = new Vector3(0f, 1f, 0f);
					array3[num5++] = num3;
					array3[num5++] = num + i;
					array3[num5++] = num + (i + 1) % numSegments;
					array3[num5++] = num4;
					array3[num5++] = numSegments + (i + 1) % numSegments;
					array3[num5++] = numSegments + i;
					num7 += num6;
					array[num2 + i * 2] = vector + vector3;
					array[num2 + i * 2 + 1] = vector2 + vector3;
					array2[num2 + i * 2] = vector3;
					array2[num2 + i * 2 + 1] = vector3;
					array3[num5++] = num2 + i * 2;
					array3[num5++] = num2 + (i * 2 + 3) % (numSegments * 2);
					array3[num5++] = num2 + (i * 2 + 2) % (numSegments * 2);
					array3[num5++] = num2 + i * 2;
					array3[num5++] = num2 + (i * 2 + 1) % (numSegments * 2);
					array3[num5++] = num2 + (i * 2 + 3) % (numSegments * 2);
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06008164 RID: 33124 RVA: 0x002A2230 File Offset: 0x002A0430
		public static Mesh SphereWireframe(int latSegments, int longSegments)
		{
			if (latSegments <= 0 || longSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_sphereWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_sphereWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			int num = (latSegments << 16) ^ longSegments;
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_sphereWireframeMeshPool.TryGetValue(num, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegments * (latSegments - 1) + 2];
				int[] array2 = new int[longSegments * (latSegments * 2 - 1) * 2];
				Vector3 vector = new Vector3(0f, 1f, 0f);
				Vector3 vector2 = new Vector3(0f, -1f, 0f);
				int num2 = array.Length - 2;
				int num3 = array.Length - 1;
				array[num2] = vector;
				array[num3] = vector2;
				float[] array3 = new float[latSegments];
				float[] array4 = new float[latSegments];
				float num4 = 3.1415927f / (float)latSegments;
				float num5 = 0f;
				for (int i = 0; i < latSegments; i++)
				{
					num5 += num4;
					array3[i] = Mathf.Sin(num5);
					array4[i] = Mathf.Cos(num5);
				}
				float[] array5 = new float[longSegments];
				float[] array6 = new float[longSegments];
				float num6 = 6.2831855f / (float)longSegments;
				float num7 = 0f;
				for (int j = 0; j < longSegments; j++)
				{
					num7 += num6;
					array5[j] = Mathf.Sin(num7);
					array6[j] = Mathf.Cos(num7);
				}
				int num8 = 0;
				int num9 = 0;
				for (int k = 0; k < longSegments; k++)
				{
					float num10 = array5[k];
					float num11 = array6[k];
					for (int l = 0; l < latSegments - 1; l++)
					{
						float num12 = array3[l];
						float num13 = array4[l];
						array[num8] = new Vector3(num11 * num12, num13, num10 * num12);
						if (l == 0)
						{
							array2[num9++] = num2;
							array2[num9++] = num8;
						}
						else
						{
							array2[num9++] = num8 - 1;
							array2[num9++] = num8;
						}
						array2[num9++] = num8;
						array2[num9++] = (num8 + latSegments - 1) % (longSegments * (latSegments - 1));
						if (l == latSegments - 2)
						{
							array2[num9++] = num8;
							array2[num9++] = num3;
						}
						num8++;
					}
				}
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.Lines, 0);
				if (PrimitiveMeshFactory.s_sphereWireframeMeshPool.ContainsKey(num))
				{
					PrimitiveMeshFactory.s_sphereWireframeMeshPool.Remove(num);
				}
				PrimitiveMeshFactory.s_sphereWireframeMeshPool.Add(num, mesh);
			}
			return mesh;
		}

		// Token: 0x06008165 RID: 33125 RVA: 0x002A24A8 File Offset: 0x002A06A8
		public static Mesh SphereSolidColor(int latSegments, int longSegments)
		{
			if (latSegments <= 0 || longSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_sphereSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_sphereSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			int num = (latSegments << 16) ^ longSegments;
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_sphereSolidColorMeshPool.TryGetValue(num, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegments * (latSegments - 1) + 2];
				int[] array2 = new int[longSegments * (latSegments - 1) * 2 * 3];
				Vector3 vector = new Vector3(0f, 1f, 0f);
				Vector3 vector2 = new Vector3(0f, -1f, 0f);
				int num2 = array.Length - 2;
				int num3 = array.Length - 1;
				array[num2] = vector;
				array[num3] = vector2;
				float[] array3 = new float[latSegments];
				float[] array4 = new float[latSegments];
				float num4 = 3.1415927f / (float)latSegments;
				float num5 = 0f;
				for (int i = 0; i < latSegments; i++)
				{
					num5 += num4;
					array3[i] = Mathf.Sin(num5);
					array4[i] = Mathf.Cos(num5);
				}
				float[] array5 = new float[longSegments];
				float[] array6 = new float[longSegments];
				float num6 = 6.2831855f / (float)longSegments;
				float num7 = 0f;
				for (int j = 0; j < longSegments; j++)
				{
					num7 += num6;
					array5[j] = Mathf.Sin(num7);
					array6[j] = Mathf.Cos(num7);
				}
				int num8 = 0;
				int num9 = 0;
				for (int k = 0; k < longSegments; k++)
				{
					float num10 = array5[k];
					float num11 = array6[k];
					for (int l = 0; l < latSegments - 1; l++)
					{
						float num12 = array3[l];
						float num13 = array4[l];
						array[num8] = new Vector3(num11 * num12, num13, num10 * num12);
						if (l == 0)
						{
							array2[num9++] = num2;
							array2[num9++] = (num8 + latSegments - 1) % (longSegments * (latSegments - 1));
							array2[num9++] = num8;
						}
						if (l < latSegments - 2)
						{
							array2[num9++] = num8 + 1;
							array2[num9++] = num8;
							array2[num9++] = (num8 + latSegments - 1) % (longSegments * (latSegments - 1));
							array2[num9++] = num8 + 1;
							array2[num9++] = (num8 + latSegments - 1) % (longSegments * (latSegments - 1));
							array2[num9++] = (num8 + latSegments) % (longSegments * (latSegments - 1));
						}
						if (l == latSegments - 2)
						{
							array2[num9++] = num8;
							array2[num9++] = (num8 + latSegments - 1) % (longSegments * (latSegments - 1));
							array2[num9++] = num3;
						}
						num8++;
					}
				}
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_sphereSolidColorMeshPool.ContainsKey(num))
				{
					PrimitiveMeshFactory.s_sphereSolidColorMeshPool.Remove(num);
				}
				PrimitiveMeshFactory.s_sphereSolidColorMeshPool.Add(num, mesh);
			}
			return mesh;
		}

		// Token: 0x06008166 RID: 33126 RVA: 0x002A2774 File Offset: 0x002A0974
		public static Mesh SphereFlatShaded(int latSegments, int longSegments)
		{
			if (latSegments <= 1 || longSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_sphereFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_sphereFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			int num = (latSegments << 16) ^ longSegments;
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_sphereFlatShadedMeshPool.TryGetValue(num, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				int num2 = (latSegments - 2) * 4 + 6;
				int num3 = (latSegments - 2) * 2 + 2;
				Vector3[] array = new Vector3[longSegments * num2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[longSegments * num3 * 3];
				Vector3 vector = new Vector3(0f, 1f, 0f);
				Vector3 vector2 = new Vector3(0f, -1f, 0f);
				float[] array4 = new float[latSegments];
				float[] array5 = new float[latSegments];
				float num4 = 3.1415927f / (float)latSegments;
				float num5 = 0f;
				for (int i = 0; i < latSegments; i++)
				{
					num5 += num4;
					array4[i] = Mathf.Sin(num5);
					array5[i] = Mathf.Cos(num5);
				}
				float[] array6 = new float[longSegments];
				float[] array7 = new float[longSegments];
				float num6 = 6.2831855f / (float)longSegments;
				float num7 = 0f;
				for (int j = 0; j < longSegments; j++)
				{
					num7 += num6;
					array6[j] = Mathf.Sin(num7);
					array7[j] = Mathf.Cos(num7);
				}
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				for (int k = 0; k < longSegments; k++)
				{
					float num11 = array6[k];
					float num12 = array7[k];
					float num13 = array6[(k + 1) % longSegments];
					float num14 = array7[(k + 1) % longSegments];
					int num15 = num8;
					array[num8++] = vector;
					array[num8++] = new Vector3(num12 * array4[0], array5[0], num11 * array4[0]);
					array[num8++] = new Vector3(num14 * array4[0], array5[0], num13 * array4[0]);
					int num16 = num8;
					array[num8++] = vector2;
					array[num8++] = new Vector3(num12 * array4[latSegments - 2], array5[latSegments - 2], num11 * array4[latSegments - 2]);
					array[num8++] = new Vector3(num14 * array4[latSegments - 2], array5[latSegments - 2], num13 * array4[latSegments - 2]);
					Vector3 normalized = Vector3.Cross(array[num15 + 2] - array[num15], array[num15 + 1] - array[num15]).normalized;
					array2[num9++] = normalized;
					array2[num9++] = normalized;
					array2[num9++] = normalized;
					Vector3 normalized2 = Vector3.Cross(array[num16 + 1] - array[num16], array[num16 + 2] - array[num16]).normalized;
					array2[num9++] = normalized2;
					array2[num9++] = normalized2;
					array2[num9++] = normalized2;
					array3[num10++] = num15;
					array3[num10++] = num15 + 2;
					array3[num10++] = num15 + 1;
					array3[num10++] = num16;
					array3[num10++] = num16 + 1;
					array3[num10++] = num16 + 2;
					for (int l = 0; l < latSegments - 2; l++)
					{
						float num17 = array4[l];
						float num18 = array5[l];
						float num19 = array4[l + 1];
						float num20 = array5[l + 1];
						int num21 = num8;
						array[num8++] = new Vector3(num12 * num17, num18, num11 * num17);
						array[num8++] = new Vector3(num14 * num17, num18, num13 * num17);
						array[num8++] = new Vector3(num14 * num19, num20, num13 * num19);
						array[num8++] = new Vector3(num12 * num19, num20, num11 * num19);
						Vector3 normalized3 = Vector3.Cross(array[num21 + 1] - array[num21], array[num21 + 2] - array[num21]).normalized;
						array2[num9++] = normalized3;
						array2[num9++] = normalized3;
						array2[num9++] = normalized3;
						array2[num9++] = normalized3;
						array3[num10++] = num21;
						array3[num10++] = num21 + 1;
						array3[num10++] = num21 + 2;
						array3[num10++] = num21;
						array3[num10++] = num21 + 2;
						array3[num10++] = num21 + 3;
					}
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_sphereFlatShadedMeshPool.ContainsKey(num))
				{
					PrimitiveMeshFactory.s_sphereFlatShadedMeshPool.Remove(num);
				}
				PrimitiveMeshFactory.s_sphereFlatShadedMeshPool.Add(num, mesh);
			}
			return mesh;
		}

		// Token: 0x06008167 RID: 33127 RVA: 0x002A2CCC File Offset: 0x002A0ECC
		public static Mesh SphereSmoothShaded(int latSegments, int longSegments)
		{
			if (latSegments <= 1 || longSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool = new Dictionary<int, Mesh>();
			}
			int num = (latSegments << 16) ^ longSegments;
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool.TryGetValue(num, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				int num2 = latSegments - 1;
				int num3 = (latSegments - 2) * 2 + 2;
				Vector3[] array = new Vector3[longSegments * num2 + 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[longSegments * num3 * 3];
				Vector3 vector = new Vector3(0f, 1f, 0f);
				Vector3 vector2 = new Vector3(0f, -1f, 0f);
				int num4 = longSegments * num2;
				int num5 = num4 + 1;
				array[num4] = vector;
				array[num5] = vector2;
				array2[num4] = new Vector3(0f, 1f, 0f);
				array2[num5] = new Vector3(0f, -1f, 0f);
				float[] array4 = new float[latSegments];
				float[] array5 = new float[latSegments];
				float num6 = 3.1415927f / (float)latSegments;
				float num7 = 0f;
				for (int i = 0; i < latSegments; i++)
				{
					num7 += num6;
					array4[i] = Mathf.Sin(num7);
					array5[i] = Mathf.Cos(num7);
				}
				float[] array6 = new float[longSegments];
				float[] array7 = new float[longSegments];
				float num8 = 6.2831855f / (float)longSegments;
				float num9 = 0f;
				for (int j = 0; j < longSegments; j++)
				{
					num9 += num8;
					array6[j] = Mathf.Sin(num9);
					array7[j] = Mathf.Cos(num9);
				}
				int num10 = 0;
				int num11 = 0;
				int num12 = 0;
				for (int k = 0; k < longSegments; k++)
				{
					float num13 = array6[k];
					float num14 = array7[k];
					for (int l = 0; l < latSegments - 1; l++)
					{
						float num15 = array4[l];
						float num16 = array5[l];
						Vector3 vector3 = new Vector3(num14 * num15, num16, num13 * num15);
						array[num10++] = vector3;
						array2[num11++] = vector3;
						int num17 = num10 - 1;
						int num18 = num17 + 1;
						int num19 = (num17 + num2) % (longSegments * num2);
						int num20 = (num17 + num2 + 1) % (longSegments * num2);
						if (latSegments == 2)
						{
							array3[num12++] = num4;
							array3[num12++] = num19;
							array3[num12++] = num17;
							array3[num12++] = num5;
							array3[num12++] = num18;
							array3[num12++] = num20;
						}
						else if (l < latSegments - 2)
						{
							if (l == 0)
							{
								array3[num12++] = num4;
								array3[num12++] = num19;
								array3[num12++] = num17;
							}
							else if (l == latSegments - 3)
							{
								array3[num12++] = num5;
								array3[num12++] = num18;
								array3[num12++] = num20;
							}
							array3[num12++] = num17;
							array3[num12++] = num20;
							array3[num12++] = num18;
							array3[num12++] = num17;
							array3[num12++] = num19;
							array3[num12++] = num20;
						}
					}
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool.ContainsKey(num))
				{
					PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool.Remove(num);
				}
				PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool.Add(num, mesh);
			}
			return mesh;
		}

		// Token: 0x06008168 RID: 33128 RVA: 0x002A3054 File Offset: 0x002A1254
		public static Mesh CapsuleWireframe(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
		{
			if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
			{
				return null;
			}
			if (!caps && !sides)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsuleWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsuleWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			int num = (latSegmentsPerCap << 12) ^ longSegmentsPerCap ^ (caps ? 268435456 : 0) ^ (topCapOnly ? 536870912 : 0) ^ (sides ? 1073741824 : 0);
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsuleWireframeMeshPool.TryGetValue(num, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegmentsPerCap * latSegmentsPerCap * 2 + 2];
				int[] array2 = new int[longSegmentsPerCap * (latSegmentsPerCap * 4 + 1) * 2];
				Vector3 vector = new Vector3(0f, 1.5f, 0f);
				Vector3 vector2 = new Vector3(0f, -1.5f, 0f);
				int num2 = array.Length - 2;
				int num3 = array.Length - 1;
				array[num2] = vector;
				array[num3] = vector2;
				float[] array3 = new float[latSegmentsPerCap];
				float[] array4 = new float[latSegmentsPerCap];
				float num4 = 1.5707964f / (float)latSegmentsPerCap;
				float num5 = 0f;
				for (int i = 0; i < latSegmentsPerCap; i++)
				{
					num5 += num4;
					array3[i] = Mathf.Sin(num5);
					array4[i] = Mathf.Cos(num5);
				}
				float[] array5 = new float[longSegmentsPerCap];
				float[] array6 = new float[longSegmentsPerCap];
				float num6 = 6.2831855f / (float)longSegmentsPerCap;
				float num7 = 0f;
				for (int j = 0; j < longSegmentsPerCap; j++)
				{
					num7 += num6;
					array5[j] = Mathf.Sin(num7);
					array6[j] = Mathf.Cos(num7);
				}
				int num8 = 0;
				int num9 = 0;
				for (int k = 0; k < longSegmentsPerCap; k++)
				{
					float num10 = array5[k];
					float num11 = array6[k];
					for (int l = 0; l < latSegmentsPerCap; l++)
					{
						float num12 = array3[l];
						float num13 = array4[l];
						array[num8] = new Vector3(num11 * num12, num13 + 0.5f, num10 * num12);
						array[num8 + 1] = new Vector3(num11 * num12, -num13 - 0.5f, num10 * num12);
						if (caps)
						{
							if (l == 0)
							{
								array2[num9++] = num2;
								array2[num9++] = num8;
								if (!topCapOnly)
								{
									array2[num9++] = num3;
									array2[num9++] = num8 + 1;
								}
							}
							else
							{
								array2[num9++] = num8 - 2;
								array2[num9++] = num8;
								if (!topCapOnly)
								{
									array2[num9++] = num8 - 1;
									array2[num9++] = num8 + 1;
								}
							}
						}
						if (caps || l == latSegmentsPerCap - 1)
						{
							array2[num9++] = num8;
							array2[num9++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							if (!topCapOnly)
							{
								array2[num9++] = num8 + 1;
								array2[num9++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							}
						}
						if (sides && l == latSegmentsPerCap - 1)
						{
							array2[num9++] = num8;
							array2[num9++] = num8 + 1;
						}
						num8 += 2;
					}
				}
				Array.Resize<int>(ref array2, num9);
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.Lines, 0);
				if (PrimitiveMeshFactory.s_capsuleWireframeMeshPool.ContainsKey(num))
				{
					PrimitiveMeshFactory.s_capsuleWireframeMeshPool.Remove(num);
				}
				PrimitiveMeshFactory.s_capsuleWireframeMeshPool.Add(num, mesh);
			}
			return mesh;
		}

		// Token: 0x06008169 RID: 33129 RVA: 0x002A339C File Offset: 0x002A159C
		public static Mesh CapsuleSolidColor(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
		{
			if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
			{
				return null;
			}
			if (!caps && !sides)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsuleSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsuleSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			int num = (latSegmentsPerCap << 12) ^ longSegmentsPerCap ^ (caps ? 268435456 : 0) ^ (topCapOnly ? 536870912 : 0) ^ (sides ? 1073741824 : 0);
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsuleSolidColorMeshPool.TryGetValue(num, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegmentsPerCap * latSegmentsPerCap * 2 + 2];
				int[] array2 = new int[longSegmentsPerCap * (latSegmentsPerCap * 4) * 3];
				Vector3 vector = new Vector3(0f, 1.5f, 0f);
				Vector3 vector2 = new Vector3(0f, -1.5f, 0f);
				int num2 = array.Length - 2;
				int num3 = array.Length - 1;
				array[num2] = vector;
				array[num3] = vector2;
				float[] array3 = new float[latSegmentsPerCap];
				float[] array4 = new float[latSegmentsPerCap];
				float num4 = 1.5707964f / (float)latSegmentsPerCap;
				float num5 = 0f;
				for (int i = 0; i < latSegmentsPerCap; i++)
				{
					num5 += num4;
					array3[i] = Mathf.Sin(num5);
					array4[i] = Mathf.Cos(num5);
				}
				float[] array5 = new float[longSegmentsPerCap];
				float[] array6 = new float[longSegmentsPerCap];
				float num6 = 6.2831855f / (float)longSegmentsPerCap;
				float num7 = 0f;
				for (int j = 0; j < longSegmentsPerCap; j++)
				{
					num7 += num6;
					array5[j] = Mathf.Sin(num7);
					array6[j] = Mathf.Cos(num7);
				}
				int num8 = 0;
				int num9 = 0;
				for (int k = 0; k < longSegmentsPerCap; k++)
				{
					float num10 = array5[k];
					float num11 = array6[k];
					for (int l = 0; l < latSegmentsPerCap; l++)
					{
						float num12 = array3[l];
						float num13 = array4[l];
						array[num8] = new Vector3(num11 * num12, num13 + 0.5f, num10 * num12);
						array[num8 + 1] = new Vector3(num11 * num12, -num13 - 0.5f, num10 * num12);
						if (l == 0)
						{
							if (caps)
							{
								array2[num9++] = num2;
								array2[num9++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[num9++] = num8;
								if (!topCapOnly)
								{
									array2[num9++] = num3;
									array2[num9++] = num8 + 1;
									array2[num9++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								}
							}
						}
						else
						{
							if (caps)
							{
								array2[num9++] = num8 - 2;
								array2[num9++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[num9++] = num8;
								array2[num9++] = num8 - 2;
								array2[num9++] = (num8 - 2 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[num9++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								if (!topCapOnly)
								{
									array2[num9++] = num8 - 1;
									array2[num9++] = num8 + 1;
									array2[num9++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
									array2[num9++] = num8 - 1;
									array2[num9++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
									array2[num9++] = (num8 - 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								}
							}
							if (sides && l == latSegmentsPerCap - 1)
							{
								array2[num9++] = num8;
								array2[num9++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[num9++] = num8 + 1;
								array2[num9++] = num8;
								array2[num9++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[num9++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							}
						}
						num8 += 2;
					}
				}
				Array.Resize<int>(ref array2, num9);
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_capsuleSolidColorMeshPool.ContainsKey(num))
				{
					PrimitiveMeshFactory.s_capsuleSolidColorMeshPool.Remove(num);
				}
				PrimitiveMeshFactory.s_capsuleSolidColorMeshPool.Add(num, mesh);
			}
			return mesh;
		}

		// Token: 0x0600816A RID: 33130 RVA: 0x002A37B4 File Offset: 0x002A19B4
		public static Mesh CapsuleFlatShaded(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
		{
			if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
			{
				return null;
			}
			if (!caps && !sides)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			int num = (latSegmentsPerCap << 12) ^ longSegmentsPerCap ^ (caps ? 268435456 : 0) ^ (topCapOnly ? 536870912 : 0) ^ (sides ? 1073741824 : 0);
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool.TryGetValue(num, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegmentsPerCap * (latSegmentsPerCap - 1) * 8 + longSegmentsPerCap * 10];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[longSegmentsPerCap * (latSegmentsPerCap * 4) * 3];
				Vector3 vector = new Vector3(0f, 1.5f, 0f);
				Vector3 vector2 = new Vector3(0f, -1.5f, 0f);
				int num2 = array.Length - 2;
				int num3 = array.Length - 1;
				array[num2] = vector;
				array[num3] = vector2;
				array2[num2] = new Vector3(0f, 1f, 0f);
				array2[num3] = new Vector3(0f, -1f, 0f);
				float[] array4 = new float[latSegmentsPerCap];
				float[] array5 = new float[latSegmentsPerCap];
				float num4 = 1.5707964f / (float)latSegmentsPerCap;
				float num5 = 0f;
				for (int i = 0; i < latSegmentsPerCap; i++)
				{
					num5 += num4;
					array4[i] = Mathf.Sin(num5);
					array5[i] = Mathf.Cos(num5);
				}
				float[] array6 = new float[longSegmentsPerCap];
				float[] array7 = new float[longSegmentsPerCap];
				float num6 = 6.2831855f / (float)longSegmentsPerCap;
				float num7 = 0f;
				for (int j = 0; j < longSegmentsPerCap; j++)
				{
					num7 += num6;
					array6[j] = Mathf.Sin(num7);
					array7[j] = Mathf.Cos(num7);
				}
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				for (int k = 0; k < longSegmentsPerCap; k++)
				{
					float num11 = array6[k];
					float num12 = array7[k];
					float num13 = array6[(k + 1) % longSegmentsPerCap];
					float num14 = array7[(k + 1) % longSegmentsPerCap];
					for (int l = 0; l < latSegmentsPerCap; l++)
					{
						float num15 = array4[l];
						float num16 = array5[l];
						if (caps && l < latSegmentsPerCap - 1)
						{
							if (l == 0)
							{
								int num17 = num8;
								array[num8++] = vector;
								array[num8++] = new Vector3(num12 * num15, num16 + 0.5f, num11 * num15);
								array[num8++] = new Vector3(num14 * num15, num16 + 0.5f, num13 * num15);
								Vector3 vector3 = Vector3.Cross(array[num17 + 2] - array[num17], array[num17 + 1] - array[num17]);
								array2[num9++] = vector3;
								array2[num9++] = vector3;
								array2[num9++] = vector3;
								array3[num10++] = num17;
								array3[num10++] = num17 + 2;
								array3[num10++] = num17 + 1;
								if (!topCapOnly)
								{
									int num18 = num8;
									array[num8++] = vector2;
									array[num8++] = new Vector3(num12 * num15, -num16 - 0.5f, num11 * num15);
									array[num8++] = new Vector3(num14 * num15, -num16 - 0.5f, num13 * num15);
									Vector3 normalized = Vector3.Cross(array[num18 + 1] - array[num18], array[num18 + 2] - array[num18]).normalized;
									array2[num9++] = normalized;
									array2[num9++] = normalized;
									array2[num9++] = normalized;
									array3[num10++] = num18;
									array3[num10++] = num18 + 1;
									array3[num10++] = num18 + 2;
								}
							}
							float num19 = array4[l + 1];
							float num20 = array5[l + 1];
							if (caps)
							{
								int num21 = num8;
								array[num8++] = new Vector3(num12 * num15, num16 + 0.5f, num11 * num15);
								array[num8++] = new Vector3(num12 * num19, num20 + 0.5f, num11 * num19);
								array[num8++] = new Vector3(num14 * num19, num20 + 0.5f, num13 * num19);
								array[num8++] = new Vector3(num14 * num15, num16 + 0.5f, num13 * num15);
								Vector3 vector4 = Vector3.Cross(array[num21 + 3] - array[num21], array[num21 + 1] - array[num21]);
								array2[num9++] = vector4;
								array2[num9++] = vector4;
								array2[num9++] = vector4;
								array2[num9++] = vector4;
								array3[num10++] = num21;
								array3[num10++] = num21 + 2;
								array3[num10++] = num21 + 1;
								array3[num10++] = num21;
								array3[num10++] = num21 + 3;
								array3[num10++] = num21 + 2;
								if (!topCapOnly)
								{
									int num22 = num8;
									array[num8++] = new Vector3(num12 * num15, -num16 - 0.5f, num11 * num15);
									array[num8++] = new Vector3(num12 * num19, -num20 - 0.5f, num11 * num19);
									array[num8++] = new Vector3(num14 * num19, -num20 - 0.5f, num13 * num19);
									array[num8++] = new Vector3(num14 * num15, -num16 - 0.5f, num13 * num15);
									Vector3 vector5 = Vector3.Cross(array[num22 + 1] - array[num22], array[num22 + 3] - array[num22]);
									array2[num9++] = vector5;
									array2[num9++] = vector5;
									array2[num9++] = vector5;
									array2[num9++] = vector5;
									array3[num10++] = num22;
									array3[num10++] = num22 + 1;
									array3[num10++] = num22 + 2;
									array3[num10++] = num22;
									array3[num10++] = num22 + 2;
									array3[num10++] = num22 + 3;
								}
							}
						}
						else if (sides && l == latSegmentsPerCap - 1)
						{
							int num23 = num8;
							array[num8++] = new Vector3(num12 * num15, num16 + 0.5f, num11 * num15);
							array[num8++] = new Vector3(num12 * num15, -num16 - 0.5f, num11 * num15);
							array[num8++] = new Vector3(num14 * num15, -num16 - 0.5f, num13 * num15);
							array[num8++] = new Vector3(num14 * num15, num16 + 0.5f, num13 * num15);
							Vector3 normalized2 = Vector3.Cross(array[num23 + 3] - array[num23], array[num23 + 1] - array[num23]).normalized;
							array2[num9++] = normalized2;
							array2[num9++] = normalized2;
							array2[num9++] = normalized2;
							array2[num9++] = normalized2;
							array3[num10++] = num23;
							array3[num10++] = num23 + 2;
							array3[num10++] = num23 + 1;
							array3[num10++] = num23;
							array3[num10++] = num23 + 3;
							array3[num10++] = num23 + 2;
						}
					}
				}
				Array.Resize<int>(ref array3, num10);
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool.ContainsKey(num))
				{
					PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool.Remove(num);
				}
				PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool.Add(num, mesh);
			}
			return mesh;
		}

		// Token: 0x0600816B RID: 33131 RVA: 0x002A4064 File Offset: 0x002A2264
		public static Mesh CapsuleSmoothShaded(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
		{
			if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
			{
				return null;
			}
			if (!caps && !sides)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool = new Dictionary<int, Mesh>();
			}
			int num = (latSegmentsPerCap << 12) ^ longSegmentsPerCap ^ (caps ? 268435456 : 0) ^ (topCapOnly ? 536870912 : 0) ^ (sides ? 1073741824 : 0);
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool.TryGetValue(num, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegmentsPerCap * latSegmentsPerCap * 2 + 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[longSegmentsPerCap * (latSegmentsPerCap * 4) * 3];
				Vector3 vector = new Vector3(0f, 1.5f, 0f);
				Vector3 vector2 = new Vector3(0f, -1.5f, 0f);
				int num2 = array.Length - 2;
				int num3 = array.Length - 1;
				array[num2] = vector;
				array[num3] = vector2;
				array2[num2] = new Vector3(0f, 1f, 0f);
				array2[num3] = new Vector3(0f, -1f, 0f);
				float[] array4 = new float[latSegmentsPerCap];
				float[] array5 = new float[latSegmentsPerCap];
				float num4 = 1.5707964f / (float)latSegmentsPerCap;
				float num5 = 0f;
				for (int i = 0; i < latSegmentsPerCap; i++)
				{
					num5 += num4;
					array4[i] = Mathf.Sin(num5);
					array5[i] = Mathf.Cos(num5);
				}
				float[] array6 = new float[longSegmentsPerCap];
				float[] array7 = new float[longSegmentsPerCap];
				float num6 = 6.2831855f / (float)longSegmentsPerCap;
				float num7 = 0f;
				for (int j = 0; j < longSegmentsPerCap; j++)
				{
					num7 += num6;
					array6[j] = Mathf.Sin(num7);
					array7[j] = Mathf.Cos(num7);
				}
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				for (int k = 0; k < longSegmentsPerCap; k++)
				{
					float num11 = array6[k];
					float num12 = array7[k];
					for (int l = 0; l < latSegmentsPerCap; l++)
					{
						float num13 = array4[l];
						float num14 = array5[l];
						array[num8] = new Vector3(num12 * num13, num14 + 0.5f, num11 * num13);
						array[num8 + 1] = new Vector3(num12 * num13, -num14 - 0.5f, num11 * num13);
						array2[num9] = new Vector3(num12 * num13, num14, num11 * num13);
						array2[num9 + 1] = new Vector3(num12 * num13, -num14, num11 * num13);
						if (caps && l == 0)
						{
							array3[num10++] = num2;
							array3[num10++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							array3[num10++] = num8;
							if (!topCapOnly)
							{
								array3[num10++] = num3;
								array3[num10++] = num8 + 1;
								array3[num10++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							}
						}
						else
						{
							if (caps)
							{
								array3[num10++] = num8 - 2;
								array3[num10++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array3[num10++] = num8;
								array3[num10++] = num8 - 2;
								array3[num10++] = (num8 - 2 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array3[num10++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								if (!topCapOnly)
								{
									array3[num10++] = num8 - 1;
									array3[num10++] = num8 + 1;
									array3[num10++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
									array3[num10++] = num8 - 1;
									array3[num10++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
									array3[num10++] = (num8 - 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								}
							}
							if (sides && l == latSegmentsPerCap - 1)
							{
								array3[num10++] = num8;
								array3[num10++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array3[num10++] = num8 + 1;
								array3[num10++] = num8;
								array3[num10++] = (num8 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array3[num10++] = (num8 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							}
						}
						num8 += 2;
						num9 += 2;
					}
				}
				Array.Resize<int>(ref array3, num10);
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool.ContainsKey(num))
				{
					PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool.Remove(num);
				}
				PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool.Add(num, mesh);
			}
			return mesh;
		}

		// Token: 0x0600816C RID: 33132 RVA: 0x002A4518 File Offset: 0x002A2718
		public static Mesh Capsule2DWireframe(int capSegments)
		{
			if (capSegments <= 0)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsule2dWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsule2dWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsule2dWireframeMeshPool.TryGetValue(capSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[(capSegments + 1) * 2];
				int[] array2 = new int[(capSegments + 1) * 4];
				int num = 0;
				int num2 = 0;
				float num3 = 3.1415927f / (float)capSegments;
				float num4 = 0f;
				for (int i = 0; i < capSegments; i++)
				{
					array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) + 0.5f, 0f);
					num4 += num3;
				}
				array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) + 0.5f, 0f);
				for (int j = 0; j < capSegments; j++)
				{
					array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) - 0.5f, 0f);
					num4 += num3;
				}
				array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) - 0.5f, 0f);
				for (int k = 0; k < array.Length - 1; k++)
				{
					array2[num2++] = k;
					array2[num2++] = (k + 1) % array.Length;
				}
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.LineStrip, 0);
				if (PrimitiveMeshFactory.s_capsule2dWireframeMeshPool.ContainsKey(capSegments))
				{
					PrimitiveMeshFactory.s_capsule2dWireframeMeshPool.Remove(capSegments);
				}
				PrimitiveMeshFactory.s_capsule2dWireframeMeshPool.Add(capSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x0600816D RID: 33133 RVA: 0x002A46C8 File Offset: 0x002A28C8
		public static Mesh Capsule2DSolidColor(int capSegments)
		{
			if (capSegments <= 0)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool.TryGetValue(capSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[(capSegments + 1) * 2];
				int[] array2 = new int[(capSegments + 1) * 12];
				int num = 0;
				int num2 = 0;
				float num3 = 3.1415927f / (float)capSegments;
				float num4 = 0f;
				for (int i = 0; i < capSegments; i++)
				{
					array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) + 0.5f, 0f);
					num4 += num3;
				}
				array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) + 0.5f, 0f);
				for (int j = 0; j < capSegments; j++)
				{
					array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) - 0.5f, 0f);
					num4 += num3;
				}
				array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) - 0.5f, 0f);
				for (int k = 1; k < array.Length; k++)
				{
					array2[num2++] = 0;
					array2[num2++] = (k + 1) % array.Length;
					array2[num2++] = k;
					array2[num2++] = 0;
					array2[num2++] = k;
					array2[num2++] = (k + 1) % array.Length;
				}
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool.ContainsKey(capSegments))
				{
					PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool.Remove(capSegments);
				}
				PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool.Add(capSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x0600816E RID: 33134 RVA: 0x002A48A0 File Offset: 0x002A2AA0
		public static Mesh Capsule2DFlatShaded(int capSegments)
		{
			if (capSegments <= 0)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool.TryGetValue(capSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				int num = (capSegments + 1) * 2;
				Vector3[] array = new Vector3[num * 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[num * 6];
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				float num5 = 3.1415927f / (float)capSegments;
				float num6 = 0f;
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < capSegments; j++)
					{
						array[num2++] = new Vector3(Mathf.Cos(num6), Mathf.Sin(num6) + 0.5f, 0f);
						num6 += num5;
					}
					array[num2++] = new Vector3(Mathf.Cos(num6), Mathf.Sin(num6) + 0.5f, 0f);
					for (int k = 0; k < capSegments; k++)
					{
						array[num2++] = new Vector3(Mathf.Cos(num6), Mathf.Sin(num6) - 0.5f, 0f);
						num6 += num5;
					}
					array[num2++] = new Vector3(Mathf.Cos(num6), Mathf.Sin(num6) - 0.5f, 0f);
					Vector3 vector = new Vector3(0f, 0f, (i == 0) ? (-1f) : 1f);
					for (int l = 0; l < num; l++)
					{
						array2[num3++] = vector;
					}
				}
				for (int m = 1; m < num; m++)
				{
					array3[num4++] = 0;
					array3[num4++] = (m + 1) % num;
					array3[num4++] = m;
					array3[num4++] = num;
					array3[num4++] = num + m;
					array3[num4++] = num + (m + 1) % num;
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool.ContainsKey(capSegments))
				{
					PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool.Remove(capSegments);
				}
				PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool.Add(capSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x0600816F RID: 33135 RVA: 0x002A4AF4 File Offset: 0x002A2CF4
		public static Mesh ConeWireframe(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_coneWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_coneWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_coneWireframeMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments + 1];
				int[] array2 = new int[numSegments * 4];
				array[numSegments] = new Vector3(0f, 1f, 0f);
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					array2[num++] = i;
					array2[num++] = (i + 1) % numSegments;
					array2[num++] = i;
					array2[num++] = numSegments;
					num3 += num2;
				}
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.Lines, 0);
				if (PrimitiveMeshFactory.s_coneWireframeMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_coneWireframeMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_coneWireframeMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06008170 RID: 33136 RVA: 0x002A4C30 File Offset: 0x002A2E30
		public static Mesh ConeSolidColor(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_coneSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_coneSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_coneSolidColorMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments + 1];
				int[] array2 = new int[numSegments * 3 + (numSegments - 2) * 3];
				array[numSegments] = new Vector3(0f, 1f, 0f);
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					array2[num++] = numSegments;
					array2[num++] = (i + 1) % numSegments;
					array2[num++] = i;
					if (i >= 2)
					{
						array2[num++] = 0;
						array2[num++] = i - 1;
						array2[num++] = i;
					}
					num3 += num2;
				}
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_coneSolidColorMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_coneSolidColorMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_coneSolidColorMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06008171 RID: 33137 RVA: 0x002A4D8C File Offset: 0x002A2F8C
		public static Mesh ConeFlatShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_coneFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_coneFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_coneFlatShadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 3 + numSegments];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 3 + (numSegments - 2) * 3];
				Vector3 vector = new Vector3(0f, 1f, 0f);
				Vector3[] array4 = new Vector3[numSegments];
				float num = 6.2831855f / (float)numSegments;
				float num2 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array4[i] = Mathf.Cos(num2) * Vector3.right + Mathf.Sin(num2) * Vector3.forward;
					num2 += num;
				}
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				for (int j = 0; j < numSegments; j++)
				{
					int num6 = num3;
					array[num3++] = vector;
					array[num3++] = array4[j];
					array[num3++] = array4[(j + 1) % numSegments];
					Vector3 normalized = Vector3.Cross(array[num6 + 2] - array[num6], array[num6 + 1] - array[num6]).normalized;
					array2[num5++] = normalized;
					array2[num5++] = normalized;
					array2[num5++] = normalized;
					array3[num4++] = num6;
					array3[num4++] = num6 + 2;
					array3[num4++] = num6 + 1;
				}
				int num7 = num3;
				for (int k = 0; k < numSegments; k++)
				{
					array[num3++] = array4[k];
					array2[num5++] = new Vector3(0f, -1f, 0f);
					if (k >= 2)
					{
						array3[num4++] = num7;
						array3[num4++] = num7 + k - 1;
						array3[num4++] = num7 + k;
					}
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_coneFlatShadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_coneFlatShadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_coneFlatShadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06008172 RID: 33138 RVA: 0x002A5014 File Offset: 0x002A3214
		public static Mesh ConeSmoothShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_coneSmoothhadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_coneSmoothhadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_coneSmoothhadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 2 + 1];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 3 + (numSegments - 2) * 3];
				int num = array.Length - 1;
				array[num] = new Vector3(0f, 1f, 0f);
				array2[num] = new Vector3(0f, 0f, 0f);
				float num2 = Mathf.Sqrt(0.5f);
				int num3 = 0;
				float num4 = 6.2831855f / (float)numSegments;
				float num5 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					float num6 = Mathf.Cos(num5);
					float num7 = Mathf.Sin(num5);
					Vector3 vector = num6 * Vector3.right + num7 * Vector3.forward;
					array[i] = vector;
					array[numSegments + i] = vector;
					array2[i] = new Vector3(num6 * num2, num2, num7 * num2);
					array2[numSegments + i] = new Vector3(0f, -1f, 0f);
					array3[num3++] = num;
					array3[num3++] = (i + 1) % numSegments;
					array3[num3++] = i;
					if (i >= 2)
					{
						array3[num3++] = numSegments;
						array3[num3++] = numSegments + i - 1;
						array3[num3++] = numSegments + i;
					}
					num5 += num4;
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_coneSmoothhadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_coneSmoothhadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_coneSmoothhadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x04009232 RID: 37426
		private static int s_lastDrawLineFrame = -1;

		// Token: 0x04009233 RID: 37427
		private static int s_iPooledMesh = 0;

		// Token: 0x04009234 RID: 37428
		private static List<Mesh> s_lineMeshPool;

		// Token: 0x04009235 RID: 37429
		private static Mesh s_boxWireframeMesh;

		// Token: 0x04009236 RID: 37430
		private static Mesh s_boxSolidColorMesh;

		// Token: 0x04009237 RID: 37431
		private static Mesh s_boxFlatShadedMesh;

		// Token: 0x04009238 RID: 37432
		private static Mesh s_rectWireframeMesh;

		// Token: 0x04009239 RID: 37433
		private static Mesh s_rectSolidColorMesh;

		// Token: 0x0400923A RID: 37434
		private static Mesh s_rectFlatShadedMesh;

		// Token: 0x0400923B RID: 37435
		private static Dictionary<int, Mesh> s_circleWireframeMeshPool;

		// Token: 0x0400923C RID: 37436
		private static Dictionary<int, Mesh> s_circleSolidColorMeshPool;

		// Token: 0x0400923D RID: 37437
		private static Dictionary<int, Mesh> s_circleFlatShadedMeshPool;

		// Token: 0x0400923E RID: 37438
		private static Dictionary<int, Mesh> s_cylinderWireframeMeshPool;

		// Token: 0x0400923F RID: 37439
		private static Dictionary<int, Mesh> s_cylinderSolidColorMeshPool;

		// Token: 0x04009240 RID: 37440
		private static Dictionary<int, Mesh> s_cylinderFlatShadedMeshPool;

		// Token: 0x04009241 RID: 37441
		private static Dictionary<int, Mesh> s_cylinderSmoothShadedMeshPool;

		// Token: 0x04009242 RID: 37442
		private static Dictionary<int, Mesh> s_sphereWireframeMeshPool;

		// Token: 0x04009243 RID: 37443
		private static Dictionary<int, Mesh> s_sphereSolidColorMeshPool;

		// Token: 0x04009244 RID: 37444
		private static Dictionary<int, Mesh> s_sphereFlatShadedMeshPool;

		// Token: 0x04009245 RID: 37445
		private static Dictionary<int, Mesh> s_sphereSmoothShadedMeshPool;

		// Token: 0x04009246 RID: 37446
		private static Dictionary<int, Mesh> s_capsuleWireframeMeshPool;

		// Token: 0x04009247 RID: 37447
		private static Dictionary<int, Mesh> s_capsuleSolidColorMeshPool;

		// Token: 0x04009248 RID: 37448
		private static Dictionary<int, Mesh> s_capsuleFlatShadedMeshPool;

		// Token: 0x04009249 RID: 37449
		private static Dictionary<int, Mesh> s_capsuleSmoothShadedMeshPool;

		// Token: 0x0400924A RID: 37450
		private static Dictionary<int, Mesh> s_capsule2dWireframeMeshPool;

		// Token: 0x0400924B RID: 37451
		private static Dictionary<int, Mesh> s_capsule2dSolidColorMeshPool;

		// Token: 0x0400924C RID: 37452
		private static Dictionary<int, Mesh> s_capsule2dFlatShadedMeshPool;

		// Token: 0x0400924D RID: 37453
		private static Dictionary<int, Mesh> s_coneWireframeMeshPool;

		// Token: 0x0400924E RID: 37454
		private static Dictionary<int, Mesh> s_coneSolidColorMeshPool;

		// Token: 0x0400924F RID: 37455
		private static Dictionary<int, Mesh> s_coneFlatShadedMeshPool;

		// Token: 0x04009250 RID: 37456
		private static Dictionary<int, Mesh> s_coneSmoothhadedMeshPool;
	}
}
