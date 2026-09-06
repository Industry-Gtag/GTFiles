using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Cysharp.Text;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace GorillaExtensions
{
	// Token: 0x020011CB RID: 4555
	public static class GTExt
	{
		// Token: 0x060072BB RID: 29371 RVA: 0x00256290 File Offset: 0x00254490
		public static T GetComponentInHierarchy<T>(this Scene scene, bool includeInactive = true) where T : Component
		{
			if (!scene.IsValid())
			{
				return default(T);
			}
			foreach (GameObject gameObject in scene.GetRootGameObjects())
			{
				T t = gameObject.GetComponent<T>();
				if (t != null)
				{
					return t;
				}
				Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(includeInactive);
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					t = componentsInChildren[j].GetComponent<T>();
					if (t != null)
					{
						return t;
					}
				}
			}
			return default(T);
		}

		// Token: 0x060072BC RID: 29372 RVA: 0x00256328 File Offset: 0x00254528
		public static List<T> GetComponentsInHierarchy<T>(this Scene scene, bool includeInactive = true, int capacity = 64)
		{
			List<T> list = new List<T>(capacity);
			if (!scene.IsValid())
			{
				return list;
			}
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				T[] componentsInChildren = rootGameObjects[i].GetComponentsInChildren<T>(includeInactive);
				list.AddRange(componentsInChildren);
			}
			return list;
		}

		// Token: 0x060072BD RID: 29373 RVA: 0x00256370 File Offset: 0x00254570
		public static List<Object> GetComponentsInHierarchy(this Scene scene, Type type, bool includeInactive = true, int capacity = 64)
		{
			List<Object> list = new List<Object>(capacity);
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				Component[] componentsInChildren = rootGameObjects[i].GetComponentsInChildren(type, includeInactive);
				list.AddRange(componentsInChildren);
			}
			return list;
		}

		// Token: 0x060072BE RID: 29374 RVA: 0x002563AD File Offset: 0x002545AD
		public static List<GameObject> GetGameObjectsInHierarchy(this Scene scene, bool includeInactive = true, int capacity = 64)
		{
			return scene.GetComponentsInHierarchy(includeInactive, capacity);
		}

		// Token: 0x060072BF RID: 29375 RVA: 0x002563B8 File Offset: 0x002545B8
		public static List<T> GetComponentsInHierarchyUntil<T, TStop1>(this Scene scene, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component
		{
			List<T> list = new List<T>(capacity);
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				List<T> componentsInChildrenUntil = rootGameObjects[i].transform.GetComponentsInChildrenUntil(includeInactive, stopAtRoot, capacity);
				list.AddRange(componentsInChildrenUntil);
			}
			return list;
		}

		// Token: 0x060072C0 RID: 29376 RVA: 0x002563FC File Offset: 0x002545FC
		public static List<T> GetComponentsInHierarchyUntil<T, TStop1, TStop2>(this Scene scene, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component
		{
			List<T> list = new List<T>(capacity);
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				List<T> componentsInChildrenUntil = rootGameObjects[i].transform.GetComponentsInChildrenUntil(includeInactive, stopAtRoot, capacity);
				list.AddRange(componentsInChildrenUntil);
			}
			return list;
		}

		// Token: 0x060072C1 RID: 29377 RVA: 0x00256440 File Offset: 0x00254640
		public static List<T> GetComponentsInHierarchyUntil<T, TStop1, TStop2, TStop3>(this Scene scene, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			List<T> list = new List<T>(capacity);
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				List<T> componentsInChildrenUntil = rootGameObjects[i].transform.GetComponentsInChildrenUntil(includeInactive, stopAtRoot, capacity);
				list.AddRange(componentsInChildrenUntil);
			}
			return list;
		}

		// Token: 0x060072C2 RID: 29378 RVA: 0x00256484 File Offset: 0x00254684
		public static List<T> GetComponentsInChildrenUntil<T, TStop1>(this Component root, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component
		{
			GTExt.<>c__DisplayClass7_0<T, TStop1> CS$<>8__locals1;
			CS$<>8__locals1.includeInactive = includeInactive;
			List<T> list = new List<T>(capacity);
			if (stopAtRoot && root.GetComponent<TStop1>() != null)
			{
				return list;
			}
			T component = root.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
			GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|7_0<T, TStop1>(root.transform, ref list, ref CS$<>8__locals1);
			return list;
		}

		// Token: 0x060072C3 RID: 29379 RVA: 0x002564E4 File Offset: 0x002546E4
		public static PooledObject<List<T>> GTGetComponentsListPool<T>(this Component root, bool includeInactive, out List<T> pooledList) where T : Component
		{
			PooledObject<List<T>> pooledObject = global::UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out pooledList);
			root.GetComponentsInChildren<T>(includeInactive, pooledList);
			return pooledObject;
		}

		// Token: 0x060072C4 RID: 29380 RVA: 0x002564F5 File Offset: 0x002546F5
		public static PooledObject<List<T>> GTGetComponentsListPool<T>(this Component root, out List<T> pooledList) where T : Component
		{
			PooledObject<List<T>> pooledObject = global::UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out pooledList);
			root.GetComponentsInChildren<T>(pooledList);
			return pooledObject;
		}

		// Token: 0x060072C5 RID: 29381 RVA: 0x00256508 File Offset: 0x00254708
		public static List<T> GetComponentsInChildrenUntil<T, TStop1, TStop2>(this Component root, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component
		{
			GTExt.<>c__DisplayClass10_0<T, TStop1, TStop2> CS$<>8__locals1;
			CS$<>8__locals1.includeInactive = includeInactive;
			List<T> list = new List<T>(capacity);
			if (stopAtRoot && (root.GetComponent<TStop1>() != null || root.GetComponent<TStop2>() != null))
			{
				return list;
			}
			T component = root.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
			GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|10_0<T, TStop1, TStop2>(root.transform, ref list, ref CS$<>8__locals1);
			return list;
		}

		// Token: 0x060072C6 RID: 29382 RVA: 0x0025657C File Offset: 0x0025477C
		public static List<T> GetComponentsInChildrenUntil<T, TStop1, TStop2, TStop3>(this Component root, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			GTExt.<>c__DisplayClass11_0<T, TStop1, TStop2, TStop3> CS$<>8__locals1;
			CS$<>8__locals1.includeInactive = includeInactive;
			List<T> list = new List<T>(capacity);
			if (stopAtRoot && (root.GetComponent<TStop1>() != null || root.GetComponent<TStop2>() != null || root.GetComponent<TStop3>() != null))
			{
				return list;
			}
			T component = root.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
			GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|11_0<T, TStop1, TStop2, TStop3>(root.transform, ref list, ref CS$<>8__locals1);
			return list;
		}

		// Token: 0x060072C7 RID: 29383 RVA: 0x00256602 File Offset: 0x00254802
		public static void GetComponentsInChildrenUntil<T, TStop1, TStop2, TStop3>(this Component root, out List<T> out_included, out HashSet<T> out_excluded, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			out_included = root.GetComponentsInChildrenUntil(includeInactive, stopAtRoot, capacity);
			out_excluded = new HashSet<T>(root.GetComponentsInChildren<T>(includeInactive));
			out_excluded.ExceptWith(new HashSet<T>(out_included));
		}

		// Token: 0x060072C8 RID: 29384 RVA: 0x00256630 File Offset: 0x00254830
		private static void _GetComponentsInChildrenUntil_OutExclusions_GetRecursive<T, TStop1, TStop2, TStop3>(Transform currentTransform, List<T> included, List<Component> excluded, bool includeInactive) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			foreach (object obj in currentTransform)
			{
				Transform transform = (Transform)obj;
				if (includeInactive || transform.gameObject.activeSelf)
				{
					Component component;
					if (GTExt._HasAnyComponents<TStop1, TStop2, TStop3>(transform, out component))
					{
						excluded.Add(component);
					}
					else
					{
						T component2 = transform.GetComponent<T>();
						if (component2 != null)
						{
							included.Add(component2);
						}
						GTExt._GetComponentsInChildrenUntil_OutExclusions_GetRecursive<T, TStop1, TStop2, TStop3>(transform, included, excluded, includeInactive);
					}
				}
			}
		}

		// Token: 0x060072C9 RID: 29385 RVA: 0x002566C8 File Offset: 0x002548C8
		private static bool _HasAnyComponents<TStop1, TStop2, TStop3>(Component component, out Component stopComponent) where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			stopComponent = component.GetComponent<TStop1>();
			if (stopComponent != null)
			{
				return true;
			}
			stopComponent = component.GetComponent<TStop2>();
			if (stopComponent != null)
			{
				return true;
			}
			stopComponent = component.GetComponent<TStop3>();
			return stopComponent != null;
		}

		// Token: 0x060072CA RID: 29386 RVA: 0x00256724 File Offset: 0x00254924
		public static T GetComponentWithRegex<T>(this Component root, string regexString) where T : Component
		{
			T[] componentsInChildren = root.GetComponentsInChildren<T>();
			Regex regex = new Regex(regexString);
			foreach (T t in componentsInChildren)
			{
				if (regex.IsMatch(t.name))
				{
					return t;
				}
			}
			return default(T);
		}

		// Token: 0x060072CB RID: 29387 RVA: 0x00256774 File Offset: 0x00254974
		private static List<T> GetComponentsWithRegex_Internal<T>(IEnumerable<T> allComponents, string regexString, bool includeInactive, int capacity = 64) where T : Component
		{
			List<T> list = new List<T>(capacity);
			Regex regex = new Regex(regexString);
			GTExt.GetComponentsWithRegex_Internal<T>(allComponents, regex, ref list);
			return list;
		}

		// Token: 0x060072CC RID: 29388 RVA: 0x0025679C File Offset: 0x0025499C
		private static void GetComponentsWithRegex_Internal<T>(IEnumerable<T> allComponents, Regex regex, ref List<T> foundComponents) where T : Component
		{
			foreach (T t in allComponents)
			{
				string name = t.name;
				if (regex.IsMatch(name))
				{
					foundComponents.Add(t);
				}
			}
		}

		// Token: 0x060072CD RID: 29389 RVA: 0x002567FC File Offset: 0x002549FC
		public static List<T> GetComponentsWithRegex<T>(this Scene scene, string regexString, bool includeInactive, int capacity) where T : Component
		{
			return GTExt.GetComponentsWithRegex_Internal<T>(scene.GetComponentsInHierarchy(includeInactive, capacity), regexString, includeInactive, capacity);
		}

		// Token: 0x060072CE RID: 29390 RVA: 0x0025680E File Offset: 0x00254A0E
		public static List<T> GetComponentsWithRegex<T>(this Component root, string regexString, bool includeInactive, int capacity) where T : Component
		{
			return GTExt.GetComponentsWithRegex_Internal<T>(root.GetComponentsInChildren<T>(includeInactive), regexString, includeInactive, capacity);
		}

		// Token: 0x060072CF RID: 29391 RVA: 0x00256820 File Offset: 0x00254A20
		public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string regexString, bool includeInactive = true, int capacity = 64)
		{
			List<Transform> componentsWithRegex = scene.GetComponentsWithRegex(regexString, includeInactive, capacity);
			List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
			foreach (Transform transform in componentsWithRegex)
			{
				list.Add(transform.gameObject);
			}
			return list;
		}

		// Token: 0x060072D0 RID: 29392 RVA: 0x00256888 File Offset: 0x00254A88
		public static void GetComponentsWithRegex_Internal<T>(this List<T> allComponents, Regex[] regexes, int maxCount, ref List<T> foundComponents) where T : Component
		{
			if (maxCount == 0)
			{
				return;
			}
			int num = 0;
			foreach (T t in allComponents)
			{
				for (int i = 0; i < regexes.Length; i++)
				{
					if (regexes[i].IsMatch(t.name))
					{
						foundComponents.Add(t);
						num++;
						if (maxCount > 0 && num >= maxCount)
						{
							return;
						}
					}
				}
			}
		}

		// Token: 0x060072D1 RID: 29393 RVA: 0x00256918 File Offset: 0x00254B18
		public static List<T> GetComponentsWithRegex<T>(this Scene scene, string[] regexStrings, bool includeInactive = true, int maxCount = -1, int capacity = 64) where T : Component
		{
			List<T> componentsInHierarchy = scene.GetComponentsInHierarchy(includeInactive, capacity);
			List<T> list = new List<T>(componentsInHierarchy.Count);
			Regex[] array = new Regex[regexStrings.Length];
			for (int i = 0; i < regexStrings.Length; i++)
			{
				array[i] = new Regex(regexStrings[i]);
			}
			componentsInHierarchy.GetComponentsWithRegex_Internal(array, maxCount, ref list);
			return list;
		}

		// Token: 0x060072D2 RID: 29394 RVA: 0x00256968 File Offset: 0x00254B68
		public static List<T> GetComponentsWithRegex<T>(this Scene scene, string[] regexStrings, string[] excludeRegexStrings, bool includeInactive = true, int maxCount = -1) where T : Component
		{
			List<T> componentsInHierarchy = scene.GetComponentsInHierarchy(includeInactive, 64);
			List<T> list = new List<T>(componentsInHierarchy.Count);
			if (maxCount == 0)
			{
				return list;
			}
			int num = 0;
			foreach (T t in componentsInHierarchy)
			{
				bool flag = false;
				foreach (string text in regexStrings)
				{
					if (!flag && Regex.IsMatch(t.name, text))
					{
						foreach (string text2 in excludeRegexStrings)
						{
							if (!flag)
							{
								flag = Regex.IsMatch(t.name, text2);
							}
						}
						if (!flag)
						{
							list.Add(t);
							num++;
							if (maxCount > 0 && num >= maxCount)
							{
								return list;
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x060072D3 RID: 29395 RVA: 0x00256A6C File Offset: 0x00254C6C
		public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string[] regexStrings, bool includeInactive = true, int maxCount = -1)
		{
			List<Transform> componentsWithRegex = scene.GetComponentsWithRegex(regexStrings, includeInactive, maxCount, 64);
			List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
			foreach (Transform transform in componentsWithRegex)
			{
				list.Add(transform.gameObject);
			}
			return list;
		}

		// Token: 0x060072D4 RID: 29396 RVA: 0x00256AD8 File Offset: 0x00254CD8
		public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string[] regexStrings, string[] excludeRegexStrings, bool includeInactive = true, int maxCount = -1)
		{
			List<Transform> componentsWithRegex = scene.GetComponentsWithRegex(regexStrings, excludeRegexStrings, includeInactive, maxCount);
			List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
			foreach (Transform transform in componentsWithRegex)
			{
				list.Add(transform.gameObject);
			}
			return list;
		}

		// Token: 0x060072D5 RID: 29397 RVA: 0x00256B44 File Offset: 0x00254D44
		public static List<T> GetComponentsByName<T>(this Transform xform, string name, bool includeInactive = true) where T : Component
		{
			T[] componentsInChildren = xform.GetComponentsInChildren<T>(includeInactive);
			List<T> list = new List<T>(componentsInChildren.Length);
			foreach (T t in componentsInChildren)
			{
				if (t.name == name)
				{
					list.Add(t);
				}
			}
			return list;
		}

		// Token: 0x060072D6 RID: 29398 RVA: 0x00256B94 File Offset: 0x00254D94
		public static T GetComponentByName<T>(this Transform xform, string name, bool includeInactive = true) where T : Component
		{
			foreach (T t in xform.GetComponentsInChildren<T>(includeInactive))
			{
				if (t.name == name)
				{
					return t;
				}
			}
			return default(T);
		}

		// Token: 0x060072D7 RID: 29399 RVA: 0x00256BE0 File Offset: 0x00254DE0
		public static List<GameObject> GetGameObjectsInHierarchy(this Scene scene, string name, bool includeInactive = true)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject gameObject in scene.GetRootGameObjects())
			{
				if (gameObject.name.Contains(name))
				{
					list.Add(gameObject);
				}
				foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>(includeInactive))
				{
					if (transform.name.Contains(name))
					{
						list.Add(transform.gameObject);
					}
				}
			}
			return list;
		}

		// Token: 0x060072D8 RID: 29400 RVA: 0x00256C62 File Offset: 0x00254E62
		public static T GetOrAddComponent<T>(this GameObject gameObject, ref T component) where T : Component
		{
			if (component == null)
			{
				component = gameObject.GetOrAddComponent<T>();
			}
			return component;
		}

		// Token: 0x060072D9 RID: 29401 RVA: 0x00256C8C File Offset: 0x00254E8C
		public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
		{
			T t;
			if (!gameObject.TryGetComponent<T>(out t))
			{
				t = gameObject.AddComponent<T>();
			}
			return t;
		}

		// Token: 0x060072DA RID: 29402 RVA: 0x00256CAC File Offset: 0x00254EAC
		public static void SetLossyScale(this Transform transform, Vector3 scale)
		{
			scale = transform.InverseTransformVector(scale);
			Vector3 lossyScale = transform.lossyScale;
			transform.localScale = new Vector3(scale.x / lossyScale.x, scale.y / lossyScale.y, scale.z / lossyScale.z);
		}

		// Token: 0x060072DB RID: 29403 RVA: 0x00256CFB File Offset: 0x00254EFB
		public static Quaternion TransformRotation(this Transform transform, Quaternion localRotation)
		{
			return transform.rotation * localRotation;
		}

		// Token: 0x060072DC RID: 29404 RVA: 0x00256D09 File Offset: 0x00254F09
		public static Quaternion InverseTransformRotation(this Transform transform, Quaternion localRotation)
		{
			return Quaternion.Inverse(transform.rotation) * localRotation;
		}

		// Token: 0x060072DD RID: 29405 RVA: 0x00256D1C File Offset: 0x00254F1C
		public static Vector3 ProjectOnPlane(this Vector3 point, Vector3 planeAnchorPosition, Vector3 planeNormal)
		{
			return planeAnchorPosition + Vector3.ProjectOnPlane(point - planeAnchorPosition, planeNormal);
		}

		// Token: 0x060072DE RID: 29406 RVA: 0x00256D34 File Offset: 0x00254F34
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> match)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (match(list[i]))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x060072DF RID: 29407 RVA: 0x00256D64 File Offset: 0x00254F64
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 MultiplyBy(this Vector3 vec, in Vector3 mulitplier)
		{
			return new Vector3(vec.x * mulitplier.x, vec.y * mulitplier.y, vec.z * mulitplier.z);
		}

		// Token: 0x060072E0 RID: 29408 RVA: 0x00256D94 File Offset: 0x00254F94
		public static void ForEachBackwards<T>(this List<T> list, Action<T> action)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				T t = list[i];
				try
				{
					action(t);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
		}

		// Token: 0x060072E1 RID: 29409 RVA: 0x00256DDC File Offset: 0x00254FDC
		public static void AddSortedUnique<T>(this List<T> list, T item)
		{
			int num = list.BinarySearch(item);
			if (num < 0)
			{
				list.Insert(~num, item);
			}
		}

		// Token: 0x060072E2 RID: 29410 RVA: 0x00256E00 File Offset: 0x00255000
		public static void RemoveSorted<T>(this List<T> list, T item)
		{
			int num = list.BinarySearch(item);
			if (num >= 0)
			{
				list.RemoveAt(num);
			}
		}

		// Token: 0x060072E3 RID: 29411 RVA: 0x00256E20 File Offset: 0x00255020
		public static bool ContainsSorted<T>(this List<T> list, T item)
		{
			return list.BinarySearch(item) >= 0;
		}

		// Token: 0x060072E4 RID: 29412 RVA: 0x00256E30 File Offset: 0x00255030
		public static void SafeForEachBackwards<T>(this List<T> list, Action<T> action)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				T t = list[i];
				try
				{
					action(t);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
		}

		// Token: 0x060072E5 RID: 29413 RVA: 0x00256E78 File Offset: 0x00255078
		public static T[] Filled<T>(this T[] array, T value)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = value;
			}
			return array;
		}

		// Token: 0x060072E6 RID: 29414 RVA: 0x00256E9C File Offset: 0x0025509C
		public static bool CompareAs255Unclamped(this Color a, Color b)
		{
			int num = (int)(a.r * 255f);
			int num2 = (int)(a.g * 255f);
			int num3 = (int)(a.b * 255f);
			int num4 = (int)(a.a * 255f);
			int num5 = (int)(b.r * 255f);
			int num6 = (int)(b.g * 255f);
			int num7 = (int)(b.b * 255f);
			int num8 = (int)(b.a * 255f);
			return num == num5 && num2 == num6 && num3 == num7 && num4 == num8;
		}

		// Token: 0x060072E7 RID: 29415 RVA: 0x00256F30 File Offset: 0x00255130
		public static Quaternion QuaternionFromToVec(Vector3 toVector, Vector3 fromVector)
		{
			Vector3 vector = Vector3.Cross(fromVector, toVector);
			Debug.Log(vector);
			Debug.Log(vector.magnitude);
			Debug.Log(Vector3.Dot(fromVector, toVector) + 1f);
			Quaternion quaternion = new Quaternion(vector.x, vector.y, vector.z, 1f + Vector3.Dot(toVector, fromVector));
			Debug.Log(quaternion);
			Debug.Log(quaternion.eulerAngles);
			Debug.Log(quaternion.normalized);
			return quaternion.normalized;
		}

		// Token: 0x060072E8 RID: 29416 RVA: 0x00256FD4 File Offset: 0x002551D4
		public static Vector3 Position(this Matrix4x4 matrix)
		{
			float m = matrix.m03;
			float m2 = matrix.m13;
			float m3 = matrix.m23;
			return new Vector3(m, m2, m3);
		}

		// Token: 0x060072E9 RID: 29417 RVA: 0x00256FFC File Offset: 0x002551FC
		public static Vector3 Scale(this Matrix4x4 m)
		{
			Vector3 vector = new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
			if (Vector3.Cross(m.GetColumn(0), m.GetColumn(1)).normalized != m.GetColumn(2).normalized)
			{
				vector.x *= -1f;
			}
			return vector;
		}

		// Token: 0x060072EA RID: 29418 RVA: 0x00002C2D File Offset: 0x00000E2D
		public static void SetLocalRelativeToParentMatrixWithParityAxis(this Matrix4x4 matrix, GTExt.ParityOptions parity = GTExt.ParityOptions.XFlip)
		{
		}

		// Token: 0x060072EB RID: 29419 RVA: 0x00257094 File Offset: 0x00255294
		public static void MultiplyInPlaceWith(this Vector3 a, in Vector3 b)
		{
			a.x *= b.x;
			a.y *= b.y;
			a.z *= b.z;
		}

		// Token: 0x060072EC RID: 29420 RVA: 0x002570C8 File Offset: 0x002552C8
		public static void DecomposeWithXFlip(this Matrix4x4 matrix, out Vector3 transformation, out Quaternion rotation, out Vector3 scale)
		{
			Matrix4x4 matrix4x = matrix;
			bool flag = matrix4x.ValidTRS();
			transformation = matrix4x.Position();
			Quaternion quaternion;
			if (!flag)
			{
				quaternion = Quaternion.identity;
			}
			else
			{
				int num = 2;
				Vector3 vector = (in matrix4x).GetColumnNoCopy(in num);
				int num2 = 1;
				quaternion = Quaternion.LookRotation(vector, (in matrix4x).GetColumnNoCopy(in num2));
			}
			rotation = quaternion;
			Vector3 vector2;
			if (!flag)
			{
				vector2 = Vector3.zero;
			}
			else
			{
				Matrix4x4 matrix4x2 = matrix;
				vector2 = matrix4x2.lossyScale;
			}
			scale = vector2;
		}

		// Token: 0x060072ED RID: 29421 RVA: 0x00257144 File Offset: 0x00255344
		public static void SetLocalMatrixRelativeToParentWithXParity(this Transform transform, in Matrix4x4 matrix4X4)
		{
			Vector3 vector;
			Quaternion quaternion;
			Vector3 vector2;
			(in matrix4X4).DecomposeWithXFlip(out vector, out quaternion, out vector2);
			transform.localPosition = vector;
			transform.localRotation = quaternion;
			transform.localScale = vector2;
		}

		// Token: 0x060072EE RID: 29422 RVA: 0x00257174 File Offset: 0x00255374
		public static Matrix4x4 Matrix4x4Scale(in Vector3 vector)
		{
			Matrix4x4 matrix4x;
			matrix4x.m00 = vector.x;
			matrix4x.m01 = 0f;
			matrix4x.m02 = 0f;
			matrix4x.m03 = 0f;
			matrix4x.m10 = 0f;
			matrix4x.m11 = vector.y;
			matrix4x.m12 = 0f;
			matrix4x.m13 = 0f;
			matrix4x.m20 = 0f;
			matrix4x.m21 = 0f;
			matrix4x.m22 = vector.z;
			matrix4x.m23 = 0f;
			matrix4x.m30 = 0f;
			matrix4x.m31 = 0f;
			matrix4x.m32 = 0f;
			matrix4x.m33 = 1f;
			return matrix4x;
		}

		// Token: 0x060072EF RID: 29423 RVA: 0x00257248 File Offset: 0x00255448
		public static Vector4 GetColumnNoCopy(this Matrix4x4 matrix, in int index)
		{
			switch (index)
			{
			case 0:
				return new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30);
			case 1:
				return new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31);
			case 2:
				return new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32);
			case 3:
				return new Vector4(matrix.m03, matrix.m13, matrix.m23, matrix.m33);
			default:
				throw new IndexOutOfRangeException("Invalid column index!");
			}
		}

		// Token: 0x060072F0 RID: 29424 RVA: 0x002572F4 File Offset: 0x002554F4
		public static Quaternion RotationWithScaleContext(this Matrix4x4 m, in Vector3 scale)
		{
			Matrix4x4 matrix4x = m * GTExt.Matrix4x4Scale(in scale);
			int num = 2;
			Vector3 vector = (in matrix4x).GetColumnNoCopy(in num);
			int num2 = 1;
			return Quaternion.LookRotation(vector, (in matrix4x).GetColumnNoCopy(in num2));
		}

		// Token: 0x060072F1 RID: 29425 RVA: 0x00257338 File Offset: 0x00255538
		public static Quaternion Rotation(this Matrix4x4 m)
		{
			int num = 2;
			Vector3 vector = (in m).GetColumnNoCopy(in num);
			int num2 = 1;
			return Quaternion.LookRotation(vector, (in m).GetColumnNoCopy(in num2));
		}

		// Token: 0x060072F2 RID: 29426 RVA: 0x00257368 File Offset: 0x00255568
		public static Vector3 x0y(this Vector2 v)
		{
			return new Vector3(v.x, 0f, v.y);
		}

		// Token: 0x060072F3 RID: 29427 RVA: 0x00257380 File Offset: 0x00255580
		public static Vector3 x0y(this Vector3 v)
		{
			return new Vector3(v.x, 0f, v.y);
		}

		// Token: 0x060072F4 RID: 29428 RVA: 0x00257398 File Offset: 0x00255598
		public static Vector3 xy0(this Vector2 v)
		{
			return new Vector3(v.x, v.y, 0f);
		}

		// Token: 0x060072F5 RID: 29429 RVA: 0x002573B0 File Offset: 0x002555B0
		public static Vector3 xy0(this Vector3 v)
		{
			return new Vector3(v.x, v.y, 0f);
		}

		// Token: 0x060072F6 RID: 29430 RVA: 0x002573C8 File Offset: 0x002555C8
		public static Vector3 xz0(this Vector3 v)
		{
			return new Vector3(v.x, v.z, 0f);
		}

		// Token: 0x060072F7 RID: 29431 RVA: 0x0006DAA2 File Offset: 0x0006BCA2
		public static Vector3 x0z(this Vector3 v)
		{
			return new Vector3(v.x, 0f, v.z);
		}

		// Token: 0x060072F8 RID: 29432 RVA: 0x002573E0 File Offset: 0x002555E0
		public static Matrix4x4 LocalMatrixRelativeToParentNoScale(this Transform transform)
		{
			return Matrix4x4.TRS(transform.localPosition, transform.localRotation, Vector3.one);
		}

		// Token: 0x060072F9 RID: 29433 RVA: 0x002573F8 File Offset: 0x002555F8
		public static Matrix4x4 LocalMatrixRelativeToParentWithScale(this Transform transform)
		{
			if (transform.parent == null)
			{
				return transform.localToWorldMatrix;
			}
			return transform.parent.worldToLocalMatrix * transform.localToWorldMatrix;
		}

		// Token: 0x060072FA RID: 29434 RVA: 0x00257425 File Offset: 0x00255625
		public static void SetLocalMatrixRelativeToParent(this Transform transform, Matrix4x4 matrix)
		{
			transform.localPosition = matrix.Position();
			transform.localRotation = (in matrix).Rotation();
			transform.localScale = matrix.Scale();
		}

		// Token: 0x060072FB RID: 29435 RVA: 0x0025744C File Offset: 0x0025564C
		public static void SetLocalMatrixRelativeToParentNoScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.localPosition = matrix.Position();
			transform.localRotation = (in matrix).Rotation();
		}

		// Token: 0x060072FC RID: 29436 RVA: 0x00257467 File Offset: 0x00255667
		public static void SetLocalToWorldMatrixNoScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.position = matrix.Position();
			transform.rotation = (in matrix).Rotation();
		}

		// Token: 0x060072FD RID: 29437 RVA: 0x00257482 File Offset: 0x00255682
		public static Matrix4x4 localToWorldNoScale(this Transform transform)
		{
			return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
		}

		// Token: 0x060072FE RID: 29438 RVA: 0x0025749A File Offset: 0x0025569A
		public static void SetLocalToWorldMatrixWithScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.position = matrix.Position();
			transform.rotation = matrix.rotation;
			transform.SetLossyScale(matrix.lossyScale);
		}

		// Token: 0x060072FF RID: 29439 RVA: 0x002574C2 File Offset: 0x002556C2
		public static Matrix4x4 Matrix4X4LerpNoScale(Matrix4x4 a, Matrix4x4 b, float t)
		{
			return Matrix4x4.TRS(Vector3.Lerp(a.Position(), b.Position(), t), Quaternion.Slerp(a.rotation, b.rotation, t), b.lossyScale);
		}

		// Token: 0x06007300 RID: 29440 RVA: 0x002574F6 File Offset: 0x002556F6
		public static Matrix4x4 LerpTo(this Matrix4x4 a, Matrix4x4 b, float t)
		{
			return GTExt.Matrix4X4LerpNoScale(a, b, t);
		}

		// Token: 0x06007301 RID: 29441 RVA: 0x00257500 File Offset: 0x00255700
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNaN(this Vector3 v)
		{
			return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
		}

		// Token: 0x06007302 RID: 29442 RVA: 0x00257529 File Offset: 0x00255729
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNan(this Quaternion q)
		{
			return float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w);
		}

		// Token: 0x06007303 RID: 29443 RVA: 0x0025755F File Offset: 0x0025575F
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsInfinity(this Vector3 v)
		{
			return float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z);
		}

		// Token: 0x06007304 RID: 29444 RVA: 0x00257588 File Offset: 0x00255788
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsInfinity(this Quaternion q)
		{
			return float.IsInfinity(q.x) || float.IsInfinity(q.y) || float.IsInfinity(q.z) || float.IsInfinity(q.w);
		}

		// Token: 0x06007305 RID: 29445 RVA: 0x002575BE File Offset: 0x002557BE
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ValuesInRange(this Vector3 v, in float maxVal)
		{
			return Mathf.Abs(v.x) < maxVal && Mathf.Abs(v.y) < maxVal && Mathf.Abs(v.z) < maxVal;
		}

		// Token: 0x06007306 RID: 29446 RVA: 0x002575EF File Offset: 0x002557EF
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValid(this Vector3 v, in float maxVal = 10000f)
		{
			return !(in v).IsNaN() && !(in v).IsInfinity() && (in v).ValuesInRange(in maxVal);
		}

		// Token: 0x06007307 RID: 29447 RVA: 0x0025760C File Offset: 0x0025580C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsMagnitudeValid(this Vector3 v, float magnitude = 1000f)
		{
			if (!(in v).IsNaN() && !(in v).IsInfinity())
			{
				Vector3 vector = v;
				return vector.sqrMagnitude < magnitude * magnitude;
			}
			return false;
		}

		// Token: 0x06007308 RID: 29448 RVA: 0x00257640 File Offset: 0x00255840
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 GetValidWithFallback(this Vector3 v, in Vector3 safeVal)
		{
			float num = 10000f;
			if (!(in v).IsValid(in num))
			{
				return safeVal;
			}
			return v;
		}

		// Token: 0x06007309 RID: 29449 RVA: 0x0025766C File Offset: 0x0025586C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetValueSafe(this Vector3 v, in Vector3 newVal)
		{
			float num = 10000f;
			if ((in newVal).IsValid(in num))
			{
				v = newVal;
			}
		}

		// Token: 0x0600730A RID: 29450 RVA: 0x00257695 File Offset: 0x00255895
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNaN(this half3 h)
		{
			return math.any(math.isnan(h));
		}

		// Token: 0x0600730B RID: 29451 RVA: 0x002576AC File Offset: 0x002558AC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsInfinity(this half3 h)
		{
			return math.any(math.isinf(h));
		}

		// Token: 0x0600730C RID: 29452 RVA: 0x002576C4 File Offset: 0x002558C4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ValuesInRange(this half3 h, in float maxVal)
		{
			return math.abs(h.x) <= maxVal && math.abs(h.y) <= maxVal && math.abs(h.z) <= maxVal;
		}

		// Token: 0x0600730D RID: 29453 RVA: 0x00257712 File Offset: 0x00255912
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValid(this half3 h, in float maxVal = 10000f)
		{
			return !(in h).IsNaN() && !(in h).IsInfinity() && (in h).ValuesInRange(in maxVal);
		}

		// Token: 0x0600730E RID: 29454 RVA: 0x0025772D File Offset: 0x0025592D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValid(this Quaternion q)
		{
			return !(in q).IsNan() && !(in q).IsInfinity();
		}

		// Token: 0x0600730F RID: 29455 RVA: 0x00257742 File Offset: 0x00255942
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion GetValidWithFallback(this Quaternion q, in Quaternion safeVal)
		{
			if (!(in q).IsValid())
			{
				return safeVal;
			}
			return q;
		}

		// Token: 0x06007310 RID: 29456 RVA: 0x00257759 File Offset: 0x00255959
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetValueSafe(this Quaternion q, in Quaternion newVal)
		{
			if ((in newVal).IsValid())
			{
				q = newVal;
			}
		}

		// Token: 0x06007311 RID: 29457 RVA: 0x00257770 File Offset: 0x00255970
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 ClampMagnitudeSafe(this Vector2 v2, float magnitude)
		{
			if (!float.IsFinite(v2.x))
			{
				v2.x = 0f;
			}
			if (!float.IsFinite(v2.y))
			{
				v2.y = 0f;
			}
			if (!float.IsFinite(magnitude))
			{
				magnitude = 0f;
			}
			return Vector2.ClampMagnitude(v2, magnitude);
		}

		// Token: 0x06007312 RID: 29458 RVA: 0x002577C8 File Offset: 0x002559C8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ClampThisMagnitudeSafe(this Vector2 v2, float magnitude)
		{
			if (!float.IsFinite(v2.x))
			{
				v2.x = 0f;
			}
			if (!float.IsFinite(v2.y))
			{
				v2.y = 0f;
			}
			if (!float.IsFinite(magnitude))
			{
				magnitude = 0f;
			}
			v2 = Vector2.ClampMagnitude(v2, magnitude);
		}

		// Token: 0x06007313 RID: 29459 RVA: 0x00257828 File Offset: 0x00255A28
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 ClampMagnitudeSafe(this Vector3 v3, float magnitude)
		{
			if (!float.IsFinite(v3.x))
			{
				v3.x = 0f;
			}
			if (!float.IsFinite(v3.y))
			{
				v3.y = 0f;
			}
			if (!float.IsFinite(v3.z))
			{
				v3.z = 0f;
			}
			if (!float.IsFinite(magnitude))
			{
				magnitude = 0f;
			}
			return Vector3.ClampMagnitude(v3, magnitude);
		}

		// Token: 0x06007314 RID: 29460 RVA: 0x00257898 File Offset: 0x00255A98
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ClampThisMagnitudeSafe(this Vector3 v3, float magnitude)
		{
			if (!float.IsFinite(v3.x))
			{
				v3.x = 0f;
			}
			if (!float.IsFinite(v3.y))
			{
				v3.y = 0f;
			}
			if (!float.IsFinite(v3.z))
			{
				v3.z = 0f;
			}
			if (!float.IsFinite(magnitude))
			{
				magnitude = 0f;
			}
			v3 = Vector3.ClampMagnitude(v3, magnitude);
		}

		// Token: 0x06007315 RID: 29461 RVA: 0x0025790E File Offset: 0x00255B0E
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float MinSafe(this float value, float min)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(min))
			{
				min = 0f;
			}
			if (value >= min)
			{
				return min;
			}
			return value;
		}

		// Token: 0x06007316 RID: 29462 RVA: 0x00257935 File Offset: 0x00255B35
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThisMinSafe(this float value, float min)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(min))
			{
				min = 0f;
			}
			value = ((value < min) ? value : min);
		}

		// Token: 0x06007317 RID: 29463 RVA: 0x00257962 File Offset: 0x00255B62
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double MinSafe(this double value, float min)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite((double)min))
			{
				min = 0f;
			}
			if (value >= (double)min)
			{
				return (double)min;
			}
			return value;
		}

		// Token: 0x06007318 RID: 29464 RVA: 0x00257990 File Offset: 0x00255B90
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThisMinSafe(this double value, float min)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite((double)min))
			{
				min = 0f;
			}
			value = ((value < (double)min) ? value : ((double)min));
		}

		// Token: 0x06007319 RID: 29465 RVA: 0x002579C4 File Offset: 0x00255BC4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float MaxSafe(this float value, float max)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(max))
			{
				max = 0f;
			}
			if (value <= max)
			{
				return max;
			}
			return value;
		}

		// Token: 0x0600731A RID: 29466 RVA: 0x002579EB File Offset: 0x00255BEB
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThisMaxSafe(this float value, float max)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(max))
			{
				max = 0f;
			}
			value = ((value > max) ? value : max);
		}

		// Token: 0x0600731B RID: 29467 RVA: 0x00257A18 File Offset: 0x00255C18
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double MaxSafe(this double value, float max)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite((double)max))
			{
				max = 0f;
			}
			if (value <= (double)max)
			{
				return (double)max;
			}
			return value;
		}

		// Token: 0x0600731C RID: 29468 RVA: 0x00257A46 File Offset: 0x00255C46
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThisMaxSafe(this double value, float max)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite((double)max))
			{
				max = 0f;
			}
			value = ((value > (double)max) ? value : ((double)max));
		}

		// Token: 0x0600731D RID: 29469 RVA: 0x00257A7A File Offset: 0x00255C7A
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ClampSafe(this float value, float min, float max)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(min))
			{
				min = 0f;
			}
			if (!float.IsFinite(max))
			{
				max = 0f;
			}
			if (value > max)
			{
				return max;
			}
			if (value >= min)
			{
				return value;
			}
			return min;
		}

		// Token: 0x0600731E RID: 29470 RVA: 0x00257AB8 File Offset: 0x00255CB8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ClampSafe(this double value, double min, double max)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite(min))
			{
				min = 0.0;
			}
			if (!double.IsFinite(max))
			{
				max = 0.0;
			}
			if (value > max)
			{
				return max;
			}
			if (value >= min)
			{
				return value;
			}
			return min;
		}

		// Token: 0x0600731F RID: 29471 RVA: 0x00257B0B File Offset: 0x00255D0B
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetFinite(this float value)
		{
			if (!float.IsFinite(value))
			{
				return 0f;
			}
			return value;
		}

		// Token: 0x06007320 RID: 29472 RVA: 0x00257B1C File Offset: 0x00255D1C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double GetFinite(this double value)
		{
			if (!double.IsFinite(value))
			{
				return 0.0;
			}
			return value;
		}

		// Token: 0x06007321 RID: 29473 RVA: 0x00257B31 File Offset: 0x00255D31
		public static Matrix4x4 Matrix4X4LerpHandleNegativeScale(Matrix4x4 a, Matrix4x4 b, float t)
		{
			return Matrix4x4.TRS(Vector3.Lerp(a.Position(), b.Position(), t), Quaternion.Slerp((in a).Rotation(), (in b).Rotation(), t), b.lossyScale);
		}

		// Token: 0x06007322 RID: 29474 RVA: 0x00257B65 File Offset: 0x00255D65
		public static Matrix4x4 LerpTo_HandleNegativeScale(this Matrix4x4 a, Matrix4x4 b, float t)
		{
			return GTExt.Matrix4X4LerpHandleNegativeScale(a, b, t);
		}

		// Token: 0x06007323 RID: 29475 RVA: 0x00257B70 File Offset: 0x00255D70
		public static Vector3 LerpToUnclamped(this Vector3 a, in Vector3 b, float t)
		{
			return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
		}

		// Token: 0x06007324 RID: 29476 RVA: 0x00257BC4 File Offset: 0x00255DC4
		public static string ToLongString(this Vector3 self)
		{
			return string.Format("[{0}, {1}, {2}]", self.x, self.y, self.z);
		}

		// Token: 0x06007325 RID: 29477 RVA: 0x00257BF1 File Offset: 0x00255DF1
		public static int GetRandomIndex<T>(this IReadOnlyList<T> self)
		{
			return global::UnityEngine.Random.Range(0, self.Count);
		}

		// Token: 0x06007326 RID: 29478 RVA: 0x00257BFF File Offset: 0x00255DFF
		public static T GetRandomItem<T>(this IReadOnlyList<T> self)
		{
			return self[self.GetRandomIndex<T>()];
		}

		// Token: 0x06007327 RID: 29479 RVA: 0x00257C0D File Offset: 0x00255E0D
		public static Vector2 xx(this float v)
		{
			return new Vector2(v, v);
		}

		// Token: 0x06007328 RID: 29480 RVA: 0x00257C16 File Offset: 0x00255E16
		public static Vector2 xx(this Vector2 v)
		{
			return new Vector2(v.x, v.x);
		}

		// Token: 0x06007329 RID: 29481 RVA: 0x00257C29 File Offset: 0x00255E29
		public static Vector2 xy(this Vector2 v)
		{
			return new Vector2(v.x, v.y);
		}

		// Token: 0x0600732A RID: 29482 RVA: 0x00257C3C File Offset: 0x00255E3C
		public static Vector2 yy(this Vector2 v)
		{
			return new Vector2(v.y, v.y);
		}

		// Token: 0x0600732B RID: 29483 RVA: 0x00257C4F File Offset: 0x00255E4F
		public static Vector2 xx(this Vector3 v)
		{
			return new Vector2(v.x, v.x);
		}

		// Token: 0x0600732C RID: 29484 RVA: 0x00257C62 File Offset: 0x00255E62
		public static Vector2 xy(this Vector3 v)
		{
			return new Vector2(v.x, v.y);
		}

		// Token: 0x0600732D RID: 29485 RVA: 0x00257C75 File Offset: 0x00255E75
		public static Vector2 xz(this Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		// Token: 0x0600732E RID: 29486 RVA: 0x00257C88 File Offset: 0x00255E88
		public static Vector2 yy(this Vector3 v)
		{
			return new Vector2(v.y, v.y);
		}

		// Token: 0x0600732F RID: 29487 RVA: 0x00257C9B File Offset: 0x00255E9B
		public static Vector2 yz(this Vector3 v)
		{
			return new Vector2(v.y, v.z);
		}

		// Token: 0x06007330 RID: 29488 RVA: 0x00257CAE File Offset: 0x00255EAE
		public static Vector2 zz(this Vector3 v)
		{
			return new Vector2(v.z, v.z);
		}

		// Token: 0x06007331 RID: 29489 RVA: 0x00257CC1 File Offset: 0x00255EC1
		public static Vector2 xx(this Vector4 v)
		{
			return new Vector2(v.x, v.x);
		}

		// Token: 0x06007332 RID: 29490 RVA: 0x00257CD4 File Offset: 0x00255ED4
		public static Vector2 xy(this Vector4 v)
		{
			return new Vector2(v.x, v.y);
		}

		// Token: 0x06007333 RID: 29491 RVA: 0x00257CE7 File Offset: 0x00255EE7
		public static Vector2 xz(this Vector4 v)
		{
			return new Vector2(v.x, v.z);
		}

		// Token: 0x06007334 RID: 29492 RVA: 0x00257CFA File Offset: 0x00255EFA
		public static Vector2 xw(this Vector4 v)
		{
			return new Vector2(v.x, v.w);
		}

		// Token: 0x06007335 RID: 29493 RVA: 0x00257D0D File Offset: 0x00255F0D
		public static Vector2 yy(this Vector4 v)
		{
			return new Vector2(v.y, v.y);
		}

		// Token: 0x06007336 RID: 29494 RVA: 0x00257D20 File Offset: 0x00255F20
		public static Vector2 yz(this Vector4 v)
		{
			return new Vector2(v.y, v.z);
		}

		// Token: 0x06007337 RID: 29495 RVA: 0x00257D33 File Offset: 0x00255F33
		public static Vector2 yw(this Vector4 v)
		{
			return new Vector2(v.y, v.w);
		}

		// Token: 0x06007338 RID: 29496 RVA: 0x00257D46 File Offset: 0x00255F46
		public static Vector2 zz(this Vector4 v)
		{
			return new Vector2(v.z, v.z);
		}

		// Token: 0x06007339 RID: 29497 RVA: 0x00257D59 File Offset: 0x00255F59
		public static Vector2 zw(this Vector4 v)
		{
			return new Vector2(v.z, v.w);
		}

		// Token: 0x0600733A RID: 29498 RVA: 0x00257D6C File Offset: 0x00255F6C
		public static Vector2 ww(this Vector4 v)
		{
			return new Vector2(v.w, v.w);
		}

		// Token: 0x0600733B RID: 29499 RVA: 0x00257D7F File Offset: 0x00255F7F
		public static Vector3 xxx(this float v)
		{
			return new Vector3(v, v, v);
		}

		// Token: 0x0600733C RID: 29500 RVA: 0x00257D89 File Offset: 0x00255F89
		public static Vector3 xxx(this Vector2 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		// Token: 0x0600733D RID: 29501 RVA: 0x00257DA2 File Offset: 0x00255FA2
		public static Vector3 xxy(this Vector2 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		// Token: 0x0600733E RID: 29502 RVA: 0x00257DBB File Offset: 0x00255FBB
		public static Vector3 xyy(this Vector2 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		// Token: 0x0600733F RID: 29503 RVA: 0x00257DD4 File Offset: 0x00255FD4
		public static Vector3 yyy(this Vector2 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		// Token: 0x06007340 RID: 29504 RVA: 0x00257DED File Offset: 0x00255FED
		public static Vector3 xxx(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		// Token: 0x06007341 RID: 29505 RVA: 0x00257E06 File Offset: 0x00256006
		public static Vector3 xxy(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		// Token: 0x06007342 RID: 29506 RVA: 0x00257E1F File Offset: 0x0025601F
		public static Vector3 xxz(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.z);
		}

		// Token: 0x06007343 RID: 29507 RVA: 0x00257E38 File Offset: 0x00256038
		public static Vector3 xyy(this Vector3 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		// Token: 0x06007344 RID: 29508 RVA: 0x00257E51 File Offset: 0x00256051
		public static Vector3 xyz(this Vector3 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		// Token: 0x06007345 RID: 29509 RVA: 0x00257E6A File Offset: 0x0025606A
		public static Vector3 xzz(this Vector3 v)
		{
			return new Vector3(v.x, v.z, v.z);
		}

		// Token: 0x06007346 RID: 29510 RVA: 0x00257E83 File Offset: 0x00256083
		public static Vector3 yyy(this Vector3 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		// Token: 0x06007347 RID: 29511 RVA: 0x00257E9C File Offset: 0x0025609C
		public static Vector3 yyz(this Vector3 v)
		{
			return new Vector3(v.y, v.y, v.z);
		}

		// Token: 0x06007348 RID: 29512 RVA: 0x00257EB5 File Offset: 0x002560B5
		public static Vector3 yzz(this Vector3 v)
		{
			return new Vector3(v.y, v.z, v.z);
		}

		// Token: 0x06007349 RID: 29513 RVA: 0x00257ECE File Offset: 0x002560CE
		public static Vector3 zzz(this Vector3 v)
		{
			return new Vector3(v.z, v.z, v.z);
		}

		// Token: 0x0600734A RID: 29514 RVA: 0x00257EE7 File Offset: 0x002560E7
		public static Vector3 xxx(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		// Token: 0x0600734B RID: 29515 RVA: 0x00257F00 File Offset: 0x00256100
		public static Vector3 xxy(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		// Token: 0x0600734C RID: 29516 RVA: 0x00257F19 File Offset: 0x00256119
		public static Vector3 xxz(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.z);
		}

		// Token: 0x0600734D RID: 29517 RVA: 0x00257F32 File Offset: 0x00256132
		public static Vector3 xxw(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.w);
		}

		// Token: 0x0600734E RID: 29518 RVA: 0x00257F4B File Offset: 0x0025614B
		public static Vector3 xyy(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		// Token: 0x0600734F RID: 29519 RVA: 0x00257F64 File Offset: 0x00256164
		public static Vector3 xyz(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		// Token: 0x06007350 RID: 29520 RVA: 0x00257F7D File Offset: 0x0025617D
		public static Vector3 xyw(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.w);
		}

		// Token: 0x06007351 RID: 29521 RVA: 0x00257F96 File Offset: 0x00256196
		public static Vector3 xzz(this Vector4 v)
		{
			return new Vector3(v.x, v.z, v.z);
		}

		// Token: 0x06007352 RID: 29522 RVA: 0x00257FAF File Offset: 0x002561AF
		public static Vector3 xzw(this Vector4 v)
		{
			return new Vector3(v.x, v.z, v.w);
		}

		// Token: 0x06007353 RID: 29523 RVA: 0x00257FC8 File Offset: 0x002561C8
		public static Vector3 xww(this Vector4 v)
		{
			return new Vector3(v.x, v.w, v.w);
		}

		// Token: 0x06007354 RID: 29524 RVA: 0x00257FE1 File Offset: 0x002561E1
		public static Vector3 yyy(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		// Token: 0x06007355 RID: 29525 RVA: 0x00257FFA File Offset: 0x002561FA
		public static Vector3 yyz(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.z);
		}

		// Token: 0x06007356 RID: 29526 RVA: 0x00258013 File Offset: 0x00256213
		public static Vector3 yyw(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.w);
		}

		// Token: 0x06007357 RID: 29527 RVA: 0x0025802C File Offset: 0x0025622C
		public static Vector3 yzz(this Vector4 v)
		{
			return new Vector3(v.y, v.z, v.z);
		}

		// Token: 0x06007358 RID: 29528 RVA: 0x00258045 File Offset: 0x00256245
		public static Vector3 yzw(this Vector4 v)
		{
			return new Vector3(v.y, v.z, v.w);
		}

		// Token: 0x06007359 RID: 29529 RVA: 0x0025805E File Offset: 0x0025625E
		public static Vector3 yww(this Vector4 v)
		{
			return new Vector3(v.y, v.w, v.w);
		}

		// Token: 0x0600735A RID: 29530 RVA: 0x00258077 File Offset: 0x00256277
		public static Vector3 zzz(this Vector4 v)
		{
			return new Vector3(v.z, v.z, v.z);
		}

		// Token: 0x0600735B RID: 29531 RVA: 0x00258090 File Offset: 0x00256290
		public static Vector3 zzw(this Vector4 v)
		{
			return new Vector3(v.z, v.z, v.w);
		}

		// Token: 0x0600735C RID: 29532 RVA: 0x002580A9 File Offset: 0x002562A9
		public static Vector3 zww(this Vector4 v)
		{
			return new Vector3(v.z, v.w, v.w);
		}

		// Token: 0x0600735D RID: 29533 RVA: 0x002580C2 File Offset: 0x002562C2
		public static Vector3 www(this Vector4 v)
		{
			return new Vector3(v.w, v.w, v.w);
		}

		// Token: 0x0600735E RID: 29534 RVA: 0x002580DB File Offset: 0x002562DB
		public static Vector4 xxxx(this float v)
		{
			return new Vector4(v, v, v, v);
		}

		// Token: 0x0600735F RID: 29535 RVA: 0x002580E6 File Offset: 0x002562E6
		public static Vector4 xxxx(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		// Token: 0x06007360 RID: 29536 RVA: 0x00258105 File Offset: 0x00256305
		public static Vector4 xxxy(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		// Token: 0x06007361 RID: 29537 RVA: 0x00258124 File Offset: 0x00256324
		public static Vector4 xxyy(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		// Token: 0x06007362 RID: 29538 RVA: 0x00258143 File Offset: 0x00256343
		public static Vector4 xyyy(this Vector2 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		// Token: 0x06007363 RID: 29539 RVA: 0x00258162 File Offset: 0x00256362
		public static Vector4 yyyy(this Vector2 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		// Token: 0x06007364 RID: 29540 RVA: 0x00258181 File Offset: 0x00256381
		public static Vector4 xxxx(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		// Token: 0x06007365 RID: 29541 RVA: 0x002581A0 File Offset: 0x002563A0
		public static Vector4 xxxy(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		// Token: 0x06007366 RID: 29542 RVA: 0x002581BF File Offset: 0x002563BF
		public static Vector4 xxxz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.z);
		}

		// Token: 0x06007367 RID: 29543 RVA: 0x002581DE File Offset: 0x002563DE
		public static Vector4 xxyy(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		// Token: 0x06007368 RID: 29544 RVA: 0x002581FD File Offset: 0x002563FD
		public static Vector4 xxyz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.y, v.z);
		}

		// Token: 0x06007369 RID: 29545 RVA: 0x0025821C File Offset: 0x0025641C
		public static Vector4 xxzz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.z, v.z);
		}

		// Token: 0x0600736A RID: 29546 RVA: 0x0025823B File Offset: 0x0025643B
		public static Vector4 xyyy(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		// Token: 0x0600736B RID: 29547 RVA: 0x0025825A File Offset: 0x0025645A
		public static Vector4 xyyz(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.y, v.z);
		}

		// Token: 0x0600736C RID: 29548 RVA: 0x00258279 File Offset: 0x00256479
		public static Vector4 xyzz(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.z, v.z);
		}

		// Token: 0x0600736D RID: 29549 RVA: 0x00258298 File Offset: 0x00256498
		public static Vector4 xzzz(this Vector3 v)
		{
			return new Vector4(v.x, v.z, v.z, v.z);
		}

		// Token: 0x0600736E RID: 29550 RVA: 0x002582B7 File Offset: 0x002564B7
		public static Vector4 yyyy(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		// Token: 0x0600736F RID: 29551 RVA: 0x002582D6 File Offset: 0x002564D6
		public static Vector4 yyyz(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.y, v.z);
		}

		// Token: 0x06007370 RID: 29552 RVA: 0x002582F5 File Offset: 0x002564F5
		public static Vector4 yyzz(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.z, v.z);
		}

		// Token: 0x06007371 RID: 29553 RVA: 0x00258314 File Offset: 0x00256514
		public static Vector4 yzzz(this Vector3 v)
		{
			return new Vector4(v.y, v.z, v.z, v.z);
		}

		// Token: 0x06007372 RID: 29554 RVA: 0x00258333 File Offset: 0x00256533
		public static Vector4 zzzz(this Vector3 v)
		{
			return new Vector4(v.z, v.z, v.z, v.z);
		}

		// Token: 0x06007373 RID: 29555 RVA: 0x00258352 File Offset: 0x00256552
		public static Vector4 xxxx(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		// Token: 0x06007374 RID: 29556 RVA: 0x00258371 File Offset: 0x00256571
		public static Vector4 xxxy(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		// Token: 0x06007375 RID: 29557 RVA: 0x00258390 File Offset: 0x00256590
		public static Vector4 xxxz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.z);
		}

		// Token: 0x06007376 RID: 29558 RVA: 0x002583AF File Offset: 0x002565AF
		public static Vector4 xxxw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.w);
		}

		// Token: 0x06007377 RID: 29559 RVA: 0x002583CE File Offset: 0x002565CE
		public static Vector4 xxyy(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		// Token: 0x06007378 RID: 29560 RVA: 0x002583ED File Offset: 0x002565ED
		public static Vector4 xxyz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.z);
		}

		// Token: 0x06007379 RID: 29561 RVA: 0x0025840C File Offset: 0x0025660C
		public static Vector4 xxyw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.w);
		}

		// Token: 0x0600737A RID: 29562 RVA: 0x0025842B File Offset: 0x0025662B
		public static Vector4 xxzz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.z, v.z);
		}

		// Token: 0x0600737B RID: 29563 RVA: 0x0025844A File Offset: 0x0025664A
		public static Vector4 xxzw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.z, v.w);
		}

		// Token: 0x0600737C RID: 29564 RVA: 0x00258469 File Offset: 0x00256669
		public static Vector4 xxww(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.w, v.w);
		}

		// Token: 0x0600737D RID: 29565 RVA: 0x00258488 File Offset: 0x00256688
		public static Vector4 xyyy(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		// Token: 0x0600737E RID: 29566 RVA: 0x002584A7 File Offset: 0x002566A7
		public static Vector4 xyyz(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.z);
		}

		// Token: 0x0600737F RID: 29567 RVA: 0x002584C6 File Offset: 0x002566C6
		public static Vector4 xyyw(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.w);
		}

		// Token: 0x06007380 RID: 29568 RVA: 0x002584E5 File Offset: 0x002566E5
		public static Vector4 xyzz(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.z, v.z);
		}

		// Token: 0x06007381 RID: 29569 RVA: 0x00258504 File Offset: 0x00256704
		public static Vector4 xyzw(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.z, v.w);
		}

		// Token: 0x06007382 RID: 29570 RVA: 0x00258523 File Offset: 0x00256723
		public static Vector4 xyww(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.w, v.w);
		}

		// Token: 0x06007383 RID: 29571 RVA: 0x00258542 File Offset: 0x00256742
		public static Vector4 xzzz(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.z, v.z);
		}

		// Token: 0x06007384 RID: 29572 RVA: 0x00258561 File Offset: 0x00256761
		public static Vector4 xzzw(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.z, v.w);
		}

		// Token: 0x06007385 RID: 29573 RVA: 0x00258580 File Offset: 0x00256780
		public static Vector4 xzww(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.w, v.w);
		}

		// Token: 0x06007386 RID: 29574 RVA: 0x0025859F File Offset: 0x0025679F
		public static Vector4 xwww(this Vector4 v)
		{
			return new Vector4(v.x, v.w, v.w, v.w);
		}

		// Token: 0x06007387 RID: 29575 RVA: 0x002585BE File Offset: 0x002567BE
		public static Vector4 yyyy(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		// Token: 0x06007388 RID: 29576 RVA: 0x002585DD File Offset: 0x002567DD
		public static Vector4 yyyz(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.z);
		}

		// Token: 0x06007389 RID: 29577 RVA: 0x002585FC File Offset: 0x002567FC
		public static Vector4 yyyw(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.w);
		}

		// Token: 0x0600738A RID: 29578 RVA: 0x0025861B File Offset: 0x0025681B
		public static Vector4 yyzz(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.z, v.z);
		}

		// Token: 0x0600738B RID: 29579 RVA: 0x0025863A File Offset: 0x0025683A
		public static Vector4 yyzw(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.z, v.w);
		}

		// Token: 0x0600738C RID: 29580 RVA: 0x00258659 File Offset: 0x00256859
		public static Vector4 yyww(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.w, v.w);
		}

		// Token: 0x0600738D RID: 29581 RVA: 0x00258678 File Offset: 0x00256878
		public static Vector4 yzzz(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.z, v.z);
		}

		// Token: 0x0600738E RID: 29582 RVA: 0x00258697 File Offset: 0x00256897
		public static Vector4 yzzw(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.z, v.w);
		}

		// Token: 0x0600738F RID: 29583 RVA: 0x002586B6 File Offset: 0x002568B6
		public static Vector4 yzww(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.w, v.w);
		}

		// Token: 0x06007390 RID: 29584 RVA: 0x002586D5 File Offset: 0x002568D5
		public static Vector4 ywww(this Vector4 v)
		{
			return new Vector4(v.y, v.w, v.w, v.w);
		}

		// Token: 0x06007391 RID: 29585 RVA: 0x002586F4 File Offset: 0x002568F4
		public static Vector4 zzzz(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.z, v.z);
		}

		// Token: 0x06007392 RID: 29586 RVA: 0x00258713 File Offset: 0x00256913
		public static Vector4 zzzw(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.z, v.w);
		}

		// Token: 0x06007393 RID: 29587 RVA: 0x00258732 File Offset: 0x00256932
		public static Vector4 zzww(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.w, v.w);
		}

		// Token: 0x06007394 RID: 29588 RVA: 0x00258751 File Offset: 0x00256951
		public static Vector4 zwww(this Vector4 v)
		{
			return new Vector4(v.z, v.w, v.w, v.w);
		}

		// Token: 0x06007395 RID: 29589 RVA: 0x00258770 File Offset: 0x00256970
		public static Vector4 wwww(this Vector4 v)
		{
			return new Vector4(v.w, v.w, v.w, v.w);
		}

		// Token: 0x06007396 RID: 29590 RVA: 0x0025878F File Offset: 0x0025698F
		public static Vector4 WithX(this Vector4 v, float x)
		{
			return new Vector4(x, v.y, v.z, v.w);
		}

		// Token: 0x06007397 RID: 29591 RVA: 0x002587A9 File Offset: 0x002569A9
		public static Vector4 WithY(this Vector4 v, float y)
		{
			return new Vector4(v.x, y, v.z, v.w);
		}

		// Token: 0x06007398 RID: 29592 RVA: 0x002587C3 File Offset: 0x002569C3
		public static Vector4 WithZ(this Vector4 v, float z)
		{
			return new Vector4(v.x, v.y, z, v.w);
		}

		// Token: 0x06007399 RID: 29593 RVA: 0x002587DD File Offset: 0x002569DD
		public static Vector4 WithW(this Vector4 v, float w)
		{
			return new Vector4(v.x, v.y, v.z, w);
		}

		// Token: 0x0600739A RID: 29594 RVA: 0x002587F7 File Offset: 0x002569F7
		public static Vector3 WithX(this Vector3 v, float x)
		{
			return new Vector3(x, v.y, v.z);
		}

		// Token: 0x0600739B RID: 29595 RVA: 0x0025880B File Offset: 0x00256A0B
		public static Vector3 WithY(this Vector3 v, float y)
		{
			return new Vector3(v.x, y, v.z);
		}

		// Token: 0x0600739C RID: 29596 RVA: 0x0025881F File Offset: 0x00256A1F
		public static Vector3 WithZ(this Vector3 v, float z)
		{
			return new Vector3(v.x, v.y, z);
		}

		// Token: 0x0600739D RID: 29597 RVA: 0x00258833 File Offset: 0x00256A33
		public static Vector4 WithW(this Vector3 v, float w)
		{
			return new Vector4(v.x, v.y, v.z, w);
		}

		// Token: 0x0600739E RID: 29598 RVA: 0x0025884D File Offset: 0x00256A4D
		public static Vector2 WithX(this Vector2 v, float x)
		{
			return new Vector2(x, v.y);
		}

		// Token: 0x0600739F RID: 29599 RVA: 0x0025885B File Offset: 0x00256A5B
		public static Vector2 WithY(this Vector2 v, float y)
		{
			return new Vector2(v.x, y);
		}

		// Token: 0x060073A0 RID: 29600 RVA: 0x00258869 File Offset: 0x00256A69
		public static Vector3 WithZ(this Vector2 v, float z)
		{
			return new Vector3(v.x, v.y, z);
		}

		// Token: 0x060073A1 RID: 29601 RVA: 0x0025887D File Offset: 0x00256A7D
		public static bool IsShorterThan(this Vector2 v, float len)
		{
			return v.sqrMagnitude < len * len;
		}

		// Token: 0x060073A2 RID: 29602 RVA: 0x0025888B File Offset: 0x00256A8B
		public static bool IsShorterThan(this Vector2 v, Vector2 v2)
		{
			return v.sqrMagnitude < v2.sqrMagnitude;
		}

		// Token: 0x060073A3 RID: 29603 RVA: 0x0025889D File Offset: 0x00256A9D
		public static bool IsShorterThan(this Vector3 v, float len)
		{
			return v.sqrMagnitude < len * len;
		}

		// Token: 0x060073A4 RID: 29604 RVA: 0x002588AB File Offset: 0x00256AAB
		public static bool IsShorterThan(this Vector3 v, Vector3 v2)
		{
			return v.sqrMagnitude < v2.sqrMagnitude;
		}

		// Token: 0x060073A5 RID: 29605 RVA: 0x002588BD File Offset: 0x00256ABD
		public static bool IsLongerThan(this Vector2 v, float len)
		{
			return v.sqrMagnitude > len * len;
		}

		// Token: 0x060073A6 RID: 29606 RVA: 0x002588CB File Offset: 0x00256ACB
		public static bool IsLongerThan(this Vector2 v, Vector2 v2)
		{
			return v.sqrMagnitude > v2.sqrMagnitude;
		}

		// Token: 0x060073A7 RID: 29607 RVA: 0x002588DD File Offset: 0x00256ADD
		public static bool IsLongerThan(this Vector3 v, float len)
		{
			return v.sqrMagnitude > len * len;
		}

		// Token: 0x060073A8 RID: 29608 RVA: 0x002588EB File Offset: 0x00256AEB
		public static bool IsLongerThan(this Vector3 v, Vector3 v2)
		{
			return v.sqrMagnitude > v2.sqrMagnitude;
		}

		// Token: 0x060073A9 RID: 29609 RVA: 0x002588FD File Offset: 0x00256AFD
		public static Vector3 Normalize(this Vector3 value, out float existingMagnitude)
		{
			existingMagnitude = Vector3.Magnitude(value);
			if (existingMagnitude > 1E-05f)
			{
				return value / existingMagnitude;
			}
			return Vector3.zero;
		}

		// Token: 0x060073AA RID: 29610 RVA: 0x00258920 File Offset: 0x00256B20
		public static Vector3 GetClosestPoint(this Ray ray, Vector3 target)
		{
			float num = Vector3.Dot(target - ray.origin, ray.direction);
			return ray.origin + ray.direction * num;
		}

		// Token: 0x060073AB RID: 29611 RVA: 0x00258960 File Offset: 0x00256B60
		public static float GetClosestDistSqr(this Ray ray, Vector3 target)
		{
			return (ray.GetClosestPoint(target) - target).sqrMagnitude;
		}

		// Token: 0x060073AC RID: 29612 RVA: 0x00258984 File Offset: 0x00256B84
		public static float GetClosestDistance(this Ray ray, Vector3 target)
		{
			return (ray.GetClosestPoint(target) - target).magnitude;
		}

		// Token: 0x060073AD RID: 29613 RVA: 0x002589A8 File Offset: 0x00256BA8
		public static Vector3 ProjectToPlane(this Ray ray, Vector3 planeOrigin, Vector3 planeNormalMustBeLength1)
		{
			Vector3 vector = planeOrigin - ray.origin;
			float num = Vector3.Dot(planeNormalMustBeLength1, vector);
			float num2 = Vector3.Dot(planeNormalMustBeLength1, ray.direction);
			return ray.origin + ray.direction * num / num2;
		}

		// Token: 0x060073AE RID: 29614 RVA: 0x002589F8 File Offset: 0x00256BF8
		public static Vector3 ProjectToLine(this Ray ray, Vector3 lineStart, Vector3 lineEnd)
		{
			Vector3 normalized = (lineEnd - lineStart).normalized;
			Vector3 normalized2 = Vector3.Cross(Vector3.Cross(ray.direction, normalized), normalized).normalized;
			return ray.ProjectToPlane(lineStart, normalized2);
		}

		// Token: 0x060073AF RID: 29615 RVA: 0x00258A39 File Offset: 0x00256C39
		public static bool IsNull(this Object mono)
		{
			return mono == null || !mono;
		}

		// Token: 0x060073B0 RID: 29616 RVA: 0x00258A49 File Offset: 0x00256C49
		public static bool IsNotNull(this Object mono)
		{
			return !mono.IsNull();
		}

		// Token: 0x060073B1 RID: 29617 RVA: 0x00258A54 File Offset: 0x00256C54
		public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max)
		{
			(ref value).ClampThis(min, max);
			return value;
		}

		// Token: 0x060073B2 RID: 29618 RVA: 0x00258A60 File Offset: 0x00256C60
		public static void ClampThis(this Vector3 value, Vector3 min, Vector3 max)
		{
			value.x = Mathf.Clamp(value.x, min.x, max.x);
			value.y = Mathf.Clamp(value.y, min.y, max.y);
			value.z = Mathf.Clamp(value.z, min.z, max.z);
		}

		// Token: 0x060073B3 RID: 29619 RVA: 0x00258AC4 File Offset: 0x00256CC4
		public static string GetPath(this Transform transform)
		{
			string text = transform.name;
			while (transform.parent)
			{
				transform = transform.parent;
				text = transform.name + "/" + text;
			}
			return "/" + text;
		}

		// Token: 0x060073B4 RID: 29620 RVA: 0x00258B0C File Offset: 0x00256D0C
		public static string GetPathQ(this Transform transform)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string text;
			try
			{
				transform.GetPathQ(ref utf16ValueStringBuilder);
			}
			finally
			{
				text = utf16ValueStringBuilder.ToString();
			}
			return text;
		}

		// Token: 0x060073B5 RID: 29621 RVA: 0x00258B4C File Offset: 0x00256D4C
		public static void GetPathQ(this Transform transform, ref Utf16ValueStringBuilder sb)
		{
			sb.Append("\"");
			int length = sb.Length;
			do
			{
				if (sb.Length > length)
				{
					sb.Insert(length, "/");
				}
				sb.Insert(length, transform.name);
				transform = transform.parent;
			}
			while (transform != null);
			sb.Append("\"");
		}

		// Token: 0x060073B6 RID: 29622 RVA: 0x00258BAC File Offset: 0x00256DAC
		public static string GetPath(this Transform transform, int maxDepth)
		{
			string text = transform.name;
			int num = 0;
			while (transform.parent && num < maxDepth)
			{
				transform = transform.parent;
				text = transform.name + "/" + text;
				num++;
			}
			return "/" + text;
		}

		// Token: 0x060073B7 RID: 29623 RVA: 0x00258C00 File Offset: 0x00256E00
		public static string GetPath(this Transform transform, Transform stopper)
		{
			string text = transform.name;
			while (transform.parent && transform.parent != stopper)
			{
				transform = transform.parent;
				text = transform.name + "/" + text;
			}
			return "/" + text;
		}

		// Token: 0x060073B8 RID: 29624 RVA: 0x00258C56 File Offset: 0x00256E56
		public static string GetPath(this GameObject gameObject)
		{
			return gameObject.transform.GetPath();
		}

		// Token: 0x060073B9 RID: 29625 RVA: 0x00258C63 File Offset: 0x00256E63
		public static void GetPath(this GameObject gameObject, ref Utf16ValueStringBuilder sb)
		{
			gameObject.transform.GetPathQ(ref sb);
		}

		// Token: 0x060073BA RID: 29626 RVA: 0x00258C71 File Offset: 0x00256E71
		public static string GetPath(this GameObject gameObject, int limit)
		{
			return gameObject.transform.GetPath(limit);
		}

		// Token: 0x060073BB RID: 29627 RVA: 0x00258C80 File Offset: 0x00256E80
		public static string[] GetPaths(this GameObject[] gobj)
		{
			string[] array = new string[gobj.Length];
			for (int i = 0; i < gobj.Length; i++)
			{
				array[i] = gobj[i].GetPath();
			}
			return array;
		}

		// Token: 0x060073BC RID: 29628 RVA: 0x00258CB0 File Offset: 0x00256EB0
		public static string[] GetPaths(this Transform[] xform)
		{
			string[] array = new string[xform.Length];
			for (int i = 0; i < xform.Length; i++)
			{
				array[i] = xform[i].GetPath();
			}
			return array;
		}

		// Token: 0x060073BD RID: 29629 RVA: 0x00258CE0 File Offset: 0x00256EE0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GetRelativePath(string fromPath, string toPath, ref Utf16ValueStringBuilder ZStringBuilder)
		{
			if (string.IsNullOrEmpty(fromPath) || string.IsNullOrEmpty(toPath))
			{
				return;
			}
			int num = 0;
			while (num < fromPath.Length && fromPath[num] == '/')
			{
				num++;
			}
			int num2 = 0;
			while (num2 < toPath.Length && toPath[num2] == '/')
			{
				num2++;
			}
			int num3 = -1;
			int num4 = Mathf.Min(fromPath.Length - num, toPath.Length - num2);
			bool flag = true;
			for (int i = 0; i < num4; i++)
			{
				if (fromPath[num + i] != toPath[num2 + i])
				{
					flag = false;
					break;
				}
				if (fromPath[num + i] == '/')
				{
					num3 = i;
				}
			}
			if (flag && fromPath.Length - num > num4)
			{
				flag = fromPath[num + num4] == '/';
			}
			else if (flag && toPath.Length - num2 > num4)
			{
				flag = toPath[num2 + num4] == '/';
			}
			num3 = (flag ? num4 : num3);
			int num5 = ((num3 < fromPath.Length - num) ? (num3 + 1) : (fromPath.Length - num));
			int num6 = ((num3 < toPath.Length - num2) ? (num3 + 1) : (toPath.Length - num2));
			if (num5 < fromPath.Length - num)
			{
				ZStringBuilder.Append("../");
				for (int j = num5; j < fromPath.Length - num; j++)
				{
					if (fromPath[num + j] == '/')
					{
						ZStringBuilder.Append("../");
					}
				}
			}
			else
			{
				ZStringBuilder.Append((toPath.Length - num2 - num6 > 0) ? "./" : ".");
			}
			ZStringBuilder.Append(toPath, num2 + num6, toPath.Length - (num2 + num6));
		}

		// Token: 0x060073BE RID: 29630 RVA: 0x00258E88 File Offset: 0x00257088
		public static string GetRelativePath(string fromPath, string toPath)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string text;
			try
			{
				GTExt.GetRelativePath(fromPath, toPath, ref utf16ValueStringBuilder);
			}
			finally
			{
				text = utf16ValueStringBuilder.ToString();
				utf16ValueStringBuilder.Dispose();
			}
			return text;
		}

		// Token: 0x060073BF RID: 29631 RVA: 0x00258ED0 File Offset: 0x002570D0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GetRelativePath(this Transform fromXform, Transform toXform, ref Utf16ValueStringBuilder ZStringBuilder)
		{
			GTExt.GetRelativePath(fromXform.GetPath(), toXform.GetPath(), ref ZStringBuilder);
		}

		// Token: 0x060073C0 RID: 29632 RVA: 0x00258EE4 File Offset: 0x002570E4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetRelativePath(this Transform fromXform, Transform toXform)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string text;
			try
			{
				fromXform.GetRelativePath(toXform, ref utf16ValueStringBuilder);
			}
			finally
			{
				text = utf16ValueStringBuilder.ToString();
				utf16ValueStringBuilder.Dispose();
			}
			return text;
		}

		// Token: 0x060073C1 RID: 29633 RVA: 0x00258F2C File Offset: 0x0025712C
		public static void GetPathWithSiblingIndexes(this Transform transform, ref Utf16ValueStringBuilder strBuilder)
		{
			int length = strBuilder.Length;
			while (transform != null)
			{
				strBuilder.Insert(length, transform.name);
				strBuilder.Insert(length, "|");
				strBuilder.Insert(length, transform.GetSiblingIndex().ToString("0000"));
				strBuilder.Insert(length, "/");
				transform = transform.parent;
			}
		}

		// Token: 0x060073C2 RID: 29634 RVA: 0x00258F94 File Offset: 0x00257194
		public static string GetComponentPath(this Component component, int maxDepth = 2147483647)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string text;
			try
			{
				component.GetComponentPath(ref utf16ValueStringBuilder, maxDepth);
			}
			finally
			{
				text = utf16ValueStringBuilder.ToString();
			}
			return text;
		}

		// Token: 0x060073C3 RID: 29635 RVA: 0x00258FD4 File Offset: 0x002571D4
		public static string GetComponentPath<T>(this T component, int maxDepth = 2147483647) where T : Component
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string text;
			try
			{
				component.GetComponentPath(ref utf16ValueStringBuilder, maxDepth);
			}
			finally
			{
				text = utf16ValueStringBuilder.ToString();
			}
			return text;
		}

		// Token: 0x060073C4 RID: 29636 RVA: 0x00259014 File Offset: 0x00257214
		public static void GetComponentPath<T>(this T component, ref Utf16ValueStringBuilder strBuilder, int maxDepth = 2147483647) where T : Component
		{
			Transform transform = component.transform;
			int length = strBuilder.Length;
			if (maxDepth > 0)
			{
				strBuilder.Append("/");
			}
			strBuilder.Append("->/");
			Type typeFromHandle = typeof(T);
			strBuilder.Append(typeFromHandle.Name);
			if (maxDepth <= 0)
			{
				return;
			}
			int num = 0;
			while (transform != null)
			{
				strBuilder.Insert(length, transform.name);
				num++;
				if (maxDepth <= num)
				{
					break;
				}
				strBuilder.Insert(length, "/");
				transform = transform.parent;
			}
		}

		// Token: 0x060073C5 RID: 29637 RVA: 0x002590A0 File Offset: 0x002572A0
		public static void GetComponentPathWithSiblingIndexes<T>(this T component, ref Utf16ValueStringBuilder strBuilder) where T : Component
		{
			Transform transform = component.transform;
			int length = strBuilder.Length;
			strBuilder.Append("/->/");
			Type typeFromHandle = typeof(T);
			strBuilder.Append(typeFromHandle.Name);
			while (transform != null)
			{
				strBuilder.Insert(length, transform.name);
				strBuilder.Insert(length, "|");
				strBuilder.Insert(length, transform.GetSiblingIndex().ToString("0000"));
				strBuilder.Insert(length, "/");
				transform = transform.parent;
			}
		}

		// Token: 0x060073C6 RID: 29638 RVA: 0x00259134 File Offset: 0x00257334
		public static string GetComponentPathWithSiblingIndexes<T>(this T component) where T : Component
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string text;
			try
			{
				component.GetComponentPathWithSiblingIndexes(ref utf16ValueStringBuilder);
			}
			finally
			{
				text = utf16ValueStringBuilder.ToString();
			}
			return text;
		}

		// Token: 0x060073C7 RID: 29639 RVA: 0x00259174 File Offset: 0x00257374
		public static T GetComponentByPath<T>(this GameObject root, string path) where T : Component
		{
			string[] array = path.Split(new string[] { "/->/" }, StringSplitOptions.None);
			if (array.Length < 2)
			{
				return default(T);
			}
			string[] array2 = array[0].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
			Transform transform = root.transform;
			for (int i = 1; i < array2.Length; i++)
			{
				string text = array2[i];
				transform = transform.Find(text);
				if (transform == null)
				{
					return default(T);
				}
			}
			Type type = Type.GetType(array[1].Split('#', StringSplitOptions.None)[0]);
			if (type == null)
			{
				return default(T);
			}
			Component component = transform.GetComponent(type);
			if (component == null)
			{
				return default(T);
			}
			return component as T;
		}

		// Token: 0x060073C8 RID: 29640 RVA: 0x00259250 File Offset: 0x00257450
		public static int GetDepth(this Transform xform)
		{
			int num = 0;
			Transform transform = xform.parent;
			while (transform != null)
			{
				num++;
				transform = transform.parent;
			}
			return num;
		}

		// Token: 0x060073C9 RID: 29641 RVA: 0x00259280 File Offset: 0x00257480
		public static string GetPathWithSiblingIndexes(this Transform transform)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string text;
			try
			{
				transform.GetPathWithSiblingIndexes(ref utf16ValueStringBuilder);
			}
			finally
			{
				text = utf16ValueStringBuilder.ToString();
			}
			return text;
		}

		// Token: 0x060073CA RID: 29642 RVA: 0x002592C0 File Offset: 0x002574C0
		public static void GetPathWithSiblingIndexes(this GameObject gameObject, ref Utf16ValueStringBuilder stringBuilder)
		{
			gameObject.transform.GetPathWithSiblingIndexes(ref stringBuilder);
		}

		// Token: 0x060073CB RID: 29643 RVA: 0x002592CE File Offset: 0x002574CE
		public static string GetPathWithSiblingIndexes(this GameObject gameObject)
		{
			return gameObject.transform.GetPathWithSiblingIndexes();
		}

		// Token: 0x060073CC RID: 29644 RVA: 0x002592DC File Offset: 0x002574DC
		public static void SetFromMatrix(this Transform transform, Matrix4x4 matrix, bool useLocal = false)
		{
			if (useLocal)
			{
				transform.localPosition = matrix.GetPosition();
				transform.localRotation = matrix.rotation;
				transform.localScale = matrix.lossyScale;
				return;
			}
			transform.position = matrix.GetPosition();
			transform.rotation = matrix.rotation;
			transform.SetScaleFromMatrix(matrix);
		}

		// Token: 0x060073CD RID: 29645 RVA: 0x00259338 File Offset: 0x00257538
		public static void SetScale(this Transform transform, Vector3 scale)
		{
			if (transform.parent)
			{
				transform.localScale = (transform.parent.worldToLocalMatrix * Matrix4x4.TRS(transform.position, transform.rotation, scale)).lossyScale;
				return;
			}
			transform.localScale = scale;
		}

		// Token: 0x060073CE RID: 29646 RVA: 0x0025938C File Offset: 0x0025758C
		public static void SetScaleFromMatrix(this Transform transform, Matrix4x4 matrix)
		{
			if (transform.parent)
			{
				transform.localScale = (transform.parent.worldToLocalMatrix * matrix).lossyScale;
				return;
			}
			transform.localScale = matrix.lossyScale;
		}

		// Token: 0x060073CF RID: 29647 RVA: 0x002593D3 File Offset: 0x002575D3
		public static void AddDictValue(Transform xForm, Dictionary<string, Transform> dict)
		{
			GTExt.caseSenseInner.Add(xForm, dict);
		}

		// Token: 0x060073D0 RID: 29648 RVA: 0x002593E1 File Offset: 0x002575E1
		public static void ClearDicts()
		{
			GTExt.caseSenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();
			GTExt.caseInsenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();
		}

		// Token: 0x060073D1 RID: 29649 RVA: 0x002593F8 File Offset: 0x002575F8
		public static bool TryFindByExactPath([NotNull] string path, out Transform result, FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("TryFindByExactPath: Provided path cannot be null or empty.");
			}
			if (findObjectsInactive != FindObjectsInactive.Exclude)
			{
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded && sceneAt.TryFindByExactPath(path, out result))
					{
						return true;
					}
				}
				result = null;
				return false;
			}
			if (path[0] != '/')
			{
				path = "/" + path;
			}
			GameObject gameObject = GameObject.Find(path);
			if (gameObject)
			{
				result = gameObject.transform;
				return true;
			}
			result = null;
			return false;
		}

		// Token: 0x060073D2 RID: 29650 RVA: 0x00259484 File Offset: 0x00257684
		public static bool TryFindByExactPath(this Scene scene, string path, out Transform result)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("TryFindByExactPath: Provided path cannot be null or empty.");
			}
			string[] array = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
			return scene.TryFindByExactPath(array, out result);
		}

		// Token: 0x060073D3 RID: 29651 RVA: 0x002594B8 File Offset: 0x002576B8
		private static bool TryFindByExactPath(this Scene scene, IReadOnlyList<string> splitPath, out Transform result)
		{
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				if (GTExt.TryFindByExactPath_Internal(rootGameObjects[i].transform, splitPath, 0, out result))
				{
					return true;
				}
			}
			result = null;
			return false;
		}

		// Token: 0x060073D4 RID: 29652 RVA: 0x002594F4 File Offset: 0x002576F4
		public static bool TryFindByExactPath(this Transform rootXform, string path, out Transform result)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("TryFindByExactPath: Provided path cannot be null or empty.");
			}
			string[] array = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
			using (IEnumerator enumerator = rootXform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (GTExt.TryFindByExactPath_Internal((Transform)enumerator.Current, array, 0, out result))
					{
						return true;
					}
				}
			}
			result = null;
			return false;
		}

		// Token: 0x060073D5 RID: 29653 RVA: 0x00259574 File Offset: 0x00257774
		public static bool TryFindByExactPath(this Transform rootXform, IReadOnlyList<string> splitPath, out Transform result)
		{
			using (IEnumerator enumerator = rootXform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (GTExt.TryFindByExactPath_Internal((Transform)enumerator.Current, splitPath, 0, out result))
					{
						return true;
					}
				}
			}
			result = null;
			return false;
		}

		// Token: 0x060073D6 RID: 29654 RVA: 0x002595D4 File Offset: 0x002577D4
		private static bool TryFindByExactPath_Internal(Transform current, IReadOnlyList<string> splitPath, int index, out Transform result)
		{
			if (current.name != splitPath[index])
			{
				result = null;
				return false;
			}
			if (index == splitPath.Count - 1)
			{
				result = current;
				return true;
			}
			using (IEnumerator enumerator = current.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (GTExt.TryFindByExactPath_Internal((Transform)enumerator.Current, splitPath, index + 1, out result))
					{
						return true;
					}
				}
			}
			result = null;
			return false;
		}

		// Token: 0x060073D7 RID: 29655 RVA: 0x00259660 File Offset: 0x00257860
		public static bool TryFindByPath(string globPath, out Transform result, bool caseSensitive = false)
		{
			string[] array = GTExt._GlobPathToPathPartsRegex(globPath);
			return GTExt._TryFindByPath(null, array, -1, out result, caseSensitive, true, globPath);
		}

		// Token: 0x060073D8 RID: 29656 RVA: 0x00259680 File Offset: 0x00257880
		public static bool TryFindByPath(this Scene scene, string globPath, out Transform result, bool caseSensitive = false)
		{
			if (string.IsNullOrEmpty(globPath))
			{
				throw new Exception("TryFindByPath: Provided path cannot be null or empty.");
			}
			string[] array = GTExt._GlobPathToPathPartsRegex(globPath);
			return scene.TryFindByPath(array, out result, globPath, caseSensitive);
		}

		// Token: 0x060073D9 RID: 29657 RVA: 0x002596B4 File Offset: 0x002578B4
		private static bool TryFindByPath(this Scene scene, IReadOnlyList<string> pathPartsRegex, out Transform result, string globPath, bool caseSensitive = false)
		{
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				if (GTExt._TryFindByPath(rootGameObjects[i].transform, pathPartsRegex, 0, out result, caseSensitive, false, globPath))
				{
					return true;
				}
			}
			result = null;
			return false;
		}

		// Token: 0x060073DA RID: 29658 RVA: 0x002596F4 File Offset: 0x002578F4
		public static bool TryFindByPath(this Transform rootXform, string globPath, out Transform result, bool caseSensitive = false)
		{
			if (string.IsNullOrEmpty(globPath))
			{
				throw new Exception("TryFindByPath: Provided path cannot be null or empty.");
			}
			char c = globPath[0];
			if (c != ' ' && c != '\n' && c != '\t')
			{
				c = globPath[globPath.Length - 1];
				if (c != ' ' && c != '\n' && c != '\t')
				{
					string[] array = GTExt._GlobPathToPathPartsRegex(globPath);
					return GTExt._TryFindByPath(rootXform, array, -1, out result, caseSensitive, false, globPath);
				}
			}
			throw new Exception("TryFindByPath: Provided globPath cannot end or start with whitespace.\nProvided globPath=\"" + globPath + "\"");
		}

		// Token: 0x060073DB RID: 29659 RVA: 0x00259777 File Offset: 0x00257977
		public static List<string> ShowAllStringsUsed()
		{
			return GTExt.allStringsUsed.Keys.ToList<string>();
		}

		// Token: 0x060073DC RID: 29660 RVA: 0x00259788 File Offset: 0x00257988
		private static bool _TryFindByPath(Transform current, IReadOnlyList<string> pathPartsRegex, int index, out Transform result, bool caseSensitive, bool isAtSceneLevel, string joinedPath)
		{
			if (joinedPath != null && !GTExt.allStringsUsed.ContainsKey(joinedPath))
			{
				GTExt.allStringsUsed[joinedPath] = joinedPath;
			}
			if (caseSensitive)
			{
				if (GTExt.caseSenseInner.ContainsKey(current))
				{
					if (GTExt.caseSenseInner[current].ContainsKey(joinedPath))
					{
						result = GTExt.caseSenseInner[current][joinedPath];
						return true;
					}
				}
				else
				{
					GTExt.caseSenseInner[current] = new Dictionary<string, Transform>();
				}
			}
			else if (GTExt.caseInsenseInner.ContainsKey(current))
			{
				if (GTExt.caseInsenseInner[current].ContainsKey(joinedPath))
				{
					result = GTExt.caseInsenseInner[current][joinedPath];
					return true;
				}
			}
			else
			{
				GTExt.caseInsenseInner[current] = new Dictionary<string, Transform>();
			}
			string text;
			if (isAtSceneLevel)
			{
				index = ((index == -1) ? 0 : index);
				text = pathPartsRegex[index];
				if (text == ".." || text == "..**" || text == "**..")
				{
					result = null;
					return false;
				}
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded)
					{
						GameObject[] rootGameObjects = sceneAt.GetRootGameObjects();
						for (int j = 0; j < rootGameObjects.Length; j++)
						{
							if (GTExt._TryFindByPath(rootGameObjects[j].transform, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
							{
								if (caseSensitive)
								{
									GTExt.caseSenseInner[current][joinedPath] = result;
								}
								else
								{
									GTExt.caseInsenseInner[current][joinedPath] = result;
								}
								return true;
							}
						}
					}
				}
			}
			if (index != -1)
			{
				text = pathPartsRegex[index];
				if (!(text == "."))
				{
					if (!(text == ".."))
					{
						if (text == "**")
						{
							goto IL_050A;
						}
						if (!(text == "..**") && !(text == "**.."))
						{
							if (!Regex.IsMatch(current.name, pathPartsRegex[index], caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
							{
								goto IL_08CB;
							}
							if (index == pathPartsRegex.Count - 1)
							{
								result = current;
								if (caseSensitive)
								{
									GTExt.caseSenseInner[current][joinedPath] = result;
								}
								else
								{
									GTExt.caseInsenseInner[current][joinedPath] = result;
								}
								return true;
							}
							using (IEnumerator enumerator = current.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, index + 1, out result, caseSensitive, false, joinedPath))
									{
										if (caseSensitive)
										{
											GTExt.caseSenseInner[current][joinedPath] = result;
										}
										else
										{
											GTExt.caseInsenseInner[current][joinedPath] = result;
										}
										return true;
									}
								}
							}
							goto IL_08CB;
						}
						else
						{
							string text2;
							do
							{
								index++;
								if (index >= pathPartsRegex.Count)
								{
									break;
								}
								text2 = pathPartsRegex[index];
							}
							while (text2 == "..**" || text2 == "**..");
							if (index == pathPartsRegex.Count)
							{
								result = current.root;
								if (caseSensitive)
								{
									GTExt.caseSenseInner[current][joinedPath] = result;
								}
								else
								{
									GTExt.caseInsenseInner[current][joinedPath] = result;
								}
								return true;
							}
							Transform transform = current.parent;
							while (transform)
							{
								if (GTExt._TryFindByPath(transform, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
								{
									if (caseSensitive)
									{
										GTExt.caseSenseInner[current][joinedPath] = result;
									}
									else
									{
										GTExt.caseInsenseInner[current][joinedPath] = result;
									}
									return true;
								}
								using (IEnumerator enumerator = transform.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
										{
											if (caseSensitive)
											{
												GTExt.caseSenseInner[current][joinedPath] = result;
											}
											else
											{
												GTExt.caseInsenseInner[current][joinedPath] = result;
											}
											return true;
										}
									}
								}
								transform = transform.parent;
							}
							if (transform != null)
							{
								goto IL_08CB;
							}
							bool flag = GTExt._TryFindByPath(current.root, pathPartsRegex, index, out result, caseSensitive, true, joinedPath);
							if (caseSensitive)
							{
								GTExt.caseSenseInner[current][joinedPath] = result;
								return flag;
							}
							GTExt.caseInsenseInner[current][joinedPath] = result;
							return flag;
						}
					}
				}
				else
				{
					while (pathPartsRegex[index] == ".")
					{
						if (index == pathPartsRegex.Count - 1)
						{
							result = current;
							return true;
						}
						index++;
					}
					if (GTExt._TryFindByPath(current, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
					{
						if (caseSensitive)
						{
							GTExt.caseSenseInner[current][joinedPath] = result;
						}
						else
						{
							GTExt.caseInsenseInner[current][joinedPath] = result;
						}
						return true;
					}
					using (IEnumerator enumerator = current.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
							{
								if (caseSensitive)
								{
									GTExt.caseSenseInner[current][joinedPath] = result;
								}
								else
								{
									GTExt.caseInsenseInner[current][joinedPath] = result;
								}
								return true;
							}
						}
						goto IL_08CB;
					}
				}
				Transform transform2 = current;
				int num = index;
				while (pathPartsRegex[num] == "..")
				{
					if (num + 1 >= pathPartsRegex.Count)
					{
						result = transform2.parent;
						return result != null;
					}
					if (transform2.parent == null)
					{
						bool flag2 = GTExt._TryFindByPath(transform2, pathPartsRegex, num + 1, out result, caseSensitive, true, joinedPath);
						if (caseSensitive)
						{
							GTExt.caseSenseInner[current][joinedPath] = result;
							return flag2;
						}
						GTExt.caseInsenseInner[current][joinedPath] = result;
						return flag2;
					}
					else
					{
						transform2 = transform2.parent;
						num++;
					}
				}
				using (IEnumerator enumerator = transform2.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, num, out result, caseSensitive, false, joinedPath))
						{
							if (caseSensitive)
							{
								GTExt.caseSenseInner[current][joinedPath] = result;
							}
							else
							{
								GTExt.caseInsenseInner[current][joinedPath] = result;
							}
							return true;
						}
					}
					goto IL_08CB;
				}
				IL_050A:
				if (index == pathPartsRegex.Count - 1)
				{
					result = ((current.childCount > 0) ? current.GetChild(0) : null);
					return current.childCount > 0;
				}
				if (index <= pathPartsRegex.Count - 1 && Regex.IsMatch(current.name, pathPartsRegex[index + 1], caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
				{
					if (index + 2 == pathPartsRegex.Count)
					{
						result = current;
						return true;
					}
					using (IEnumerator enumerator = current.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, index + 2, out result, caseSensitive, false, joinedPath))
							{
								return true;
							}
						}
					}
				}
				Transform transform3;
				if (GTExt._TryBreadthFirstSearchNames(current, pathPartsRegex[index + 1], out transform3, caseSensitive))
				{
					if (index + 2 == pathPartsRegex.Count)
					{
						result = transform3;
						if (caseSensitive)
						{
							GTExt.caseSenseInner[current][joinedPath] = result;
						}
						else
						{
							GTExt.caseInsenseInner[current][joinedPath] = result;
						}
						return true;
					}
					if (GTExt._TryFindByPath(transform3, pathPartsRegex, index + 2, out result, caseSensitive, false, joinedPath))
					{
						if (caseSensitive)
						{
							GTExt.caseSenseInner[current][joinedPath] = result;
						}
						else
						{
							GTExt.caseInsenseInner[current][joinedPath] = result;
						}
						return true;
					}
				}
				IL_08CB:
				result = null;
				if (caseSensitive)
				{
					GTExt.caseSenseInner[current][joinedPath] = result;
				}
				else
				{
					GTExt.caseInsenseInner[current][joinedPath] = result;
				}
				return false;
			}
			if (pathPartsRegex.Count == 0)
			{
				result = null;
				return false;
			}
			text = pathPartsRegex[0];
			if (!(text == ".") && !(text == "..") && !(text == "..**") && !(text == "**.."))
			{
				using (IEnumerator enumerator = current.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, 0, out result, caseSensitive, false, joinedPath))
						{
							if (caseSensitive)
							{
								GTExt.caseSenseInner[current][joinedPath] = result;
							}
							else
							{
								GTExt.caseInsenseInner[current][joinedPath] = result;
							}
							return true;
						}
					}
				}
				result = null;
				if (caseSensitive)
				{
					GTExt.caseSenseInner[current][joinedPath] = result;
				}
				else
				{
					GTExt.caseInsenseInner[current][joinedPath] = result;
				}
				return false;
			}
			bool flag3 = GTExt._TryFindByPath(current, pathPartsRegex, 0, out result, caseSensitive, false, joinedPath);
			if (caseSensitive)
			{
				GTExt.caseSenseInner[current][joinedPath] = result;
				return flag3;
			}
			GTExt.caseInsenseInner[current][joinedPath] = result;
			return flag3;
		}

		// Token: 0x060073DD RID: 29661 RVA: 0x0025A0E4 File Offset: 0x002582E4
		private static bool _TryBreadthFirstSearchNames(Transform root, string regexPattern, out Transform result, bool caseSensitive)
		{
			Queue<Transform> queue = new Queue<Transform>();
			using (IEnumerator enumerator = root.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					queue.Enqueue(transform);
				}
				goto IL_009B;
			}
			IL_003D:
			Transform transform2 = queue.Dequeue();
			if (Regex.IsMatch(transform2.name, regexPattern, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
			{
				result = transform2;
				return true;
			}
			foreach (object obj2 in transform2)
			{
				Transform transform3 = (Transform)obj2;
				queue.Enqueue(transform3);
			}
			IL_009B:
			if (queue.Count <= 0)
			{
				result = null;
				return false;
			}
			goto IL_003D;
		}

		// Token: 0x060073DE RID: 29662 RVA: 0x0025A1B8 File Offset: 0x002583B8
		public static T[] FindComponentsByExactPath<T>(string path) where T : Component
		{
			List<T> list;
			T[] array;
			using (global::UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded)
					{
						list.AddRange(sceneAt.FindComponentsByExactPath(path));
					}
				}
				array = list.ToArray();
			}
			return array;
		}

		// Token: 0x060073DF RID: 29663 RVA: 0x0025A22C File Offset: 0x0025842C
		public static T[] FindComponentsByExactPath<T>(this Scene scene, string path) where T : Component
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("FindComponentsByExactPath: Provided path cannot be null or empty.");
			}
			string[] array = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
			return scene.FindComponentsByExactPath(array);
		}

		// Token: 0x060073E0 RID: 29664 RVA: 0x0025A260 File Offset: 0x00258460
		private static T[] FindComponentsByExactPath<T>(this Scene scene, string[] splitPath) where T : Component
		{
			List<T> list;
			T[] array;
			using (global::UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				GameObject[] rootGameObjects = scene.GetRootGameObjects();
				for (int i = 0; i < rootGameObjects.Length; i++)
				{
					GTExt._FindComponentsByExactPath<T>(rootGameObjects[i].transform, splitPath, 0, list);
				}
				array = list.ToArray();
			}
			return array;
		}

		// Token: 0x060073E1 RID: 29665 RVA: 0x0025A2D0 File Offset: 0x002584D0
		public static T[] FindComponentsByExactPath<T>(this Transform rootXform, string path) where T : Component
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("FindComponentsByExactPath: Provided path cannot be null or empty.");
			}
			string[] array = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
			List<T> list;
			T[] array2;
			using (global::UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				foreach (object obj in rootXform)
				{
					GTExt._FindComponentsByExactPath<T>((Transform)obj, array, 0, list);
				}
				array2 = list.ToArray();
			}
			return array2;
		}

		// Token: 0x060073E2 RID: 29666 RVA: 0x0025A37C File Offset: 0x0025857C
		public static T[] FindComponentsByExactPath<T>(this Transform rootXform, string[] splitPath) where T : Component
		{
			List<T> list;
			T[] array;
			using (global::UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				foreach (object obj in rootXform)
				{
					GTExt._FindComponentsByExactPath<T>((Transform)obj, splitPath, 0, list);
				}
				array = list.ToArray();
			}
			return array;
		}

		// Token: 0x060073E3 RID: 29667 RVA: 0x0025A408 File Offset: 0x00258608
		private static void _FindComponentsByExactPath<T>(Transform current, string[] splitPath, int index, List<T> components) where T : Component
		{
			if (current.name != splitPath[index])
			{
				return;
			}
			if (index == splitPath.Length - 1)
			{
				T component = current.GetComponent<T>();
				if (component)
				{
					components.Add(component);
				}
				return;
			}
			foreach (object obj in current)
			{
				GTExt._FindComponentsByExactPath<T>((Transform)obj, splitPath, index + 1, components);
			}
		}

		// Token: 0x060073E4 RID: 29668 RVA: 0x0025A494 File Offset: 0x00258694
		public static T[] FindComponentsByPathInLoadedScenes<T>(string wildcardPath, bool caseSensitive = false) where T : Component
		{
			List<T> list;
			T[] array2;
			using (global::UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				string[] array = GTExt._GlobPathToPathPartsRegex(wildcardPath);
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded)
					{
						GameObject[] rootGameObjects = sceneAt.GetRootGameObjects();
						for (int j = 0; j < rootGameObjects.Length; j++)
						{
							GTExt._FindComponentsByPath<T>(rootGameObjects[j].transform, array, list, caseSensitive);
						}
					}
				}
				array2 = list.ToArray();
			}
			return array2;
		}

		// Token: 0x060073E5 RID: 29669 RVA: 0x0025A534 File Offset: 0x00258734
		public static T[] FindComponentsByPath<T>(this Scene scene, string globPath, bool caseSensitive = false) where T : Component
		{
			if (string.IsNullOrEmpty(globPath))
			{
				throw new Exception("FindComponentsByPath: Provided path cannot be null or empty.");
			}
			string[] array = GTExt._GlobPathToPathPartsRegex(globPath);
			return scene.FindComponentsByPath(array, caseSensitive);
		}

		// Token: 0x060073E6 RID: 29670 RVA: 0x0025A564 File Offset: 0x00258764
		private static T[] FindComponentsByPath<T>(this Scene scene, string[] pathPartsRegex, bool caseSensitive = false) where T : Component
		{
			List<T> list;
			T[] array;
			using (global::UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				GameObject[] rootGameObjects = scene.GetRootGameObjects();
				for (int i = 0; i < rootGameObjects.Length; i++)
				{
					GTExt._FindComponentsByPath<T>(rootGameObjects[i].transform, pathPartsRegex, list, caseSensitive);
				}
				array = list.ToArray();
			}
			return array;
		}

		// Token: 0x060073E7 RID: 29671 RVA: 0x0025A5D4 File Offset: 0x002587D4
		public static T[] FindComponentsByPath<T>(this Transform rootXform, string globPath, bool caseSensitive = false) where T : Component
		{
			if (string.IsNullOrEmpty(globPath))
			{
				throw new Exception("FindComponentsByPath: Provided path cannot be null or empty.");
			}
			string[] array = GTExt._GlobPathToPathPartsRegex(globPath);
			return rootXform.FindComponentsByPath(array, caseSensitive);
		}

		// Token: 0x060073E8 RID: 29672 RVA: 0x0025A604 File Offset: 0x00258804
		public static T[] FindComponentsByPath<T>(this Transform rootXform, string[] pathPartsRegex, bool caseSensitive = false) where T : Component
		{
			List<T> list;
			T[] array;
			using (global::UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				GTExt._FindComponentsByPath<T>(rootXform, pathPartsRegex, list, caseSensitive);
				array = list.ToArray();
			}
			return array;
		}

		// Token: 0x060073E9 RID: 29673 RVA: 0x0025A654 File Offset: 0x00258854
		public static void _FindComponentsByPath<T>(Transform current, string[] pathPartsRegex, List<T> components, bool caseSensitive) where T : Component
		{
			List<Transform> list;
			using (global::UnityEngine.Pool.CollectionPool<List<Transform>, Transform>.Get(out list))
			{
				list.EnsureCapacity(64);
				if (GTExt._TryFindAllByPath(current, pathPartsRegex, 0, list, caseSensitive, false))
				{
					for (int i = 0; i < list.Count; i++)
					{
						T[] components2 = list[i].GetComponents<T>();
						components.AddRange(components2);
					}
				}
			}
		}

		// Token: 0x060073EA RID: 29674 RVA: 0x0025A6C8 File Offset: 0x002588C8
		private static bool _TryFindAllByPath(Transform current, IReadOnlyList<string> pathPartsRegex, int index, List<Transform> results, bool caseSensitive, bool isAtSceneLevel = false)
		{
			bool flag = false;
			string text;
			if (isAtSceneLevel)
			{
				text = pathPartsRegex[index];
				if (text == ".." || text == "..**" || text == "**..")
				{
					return false;
				}
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded)
					{
						foreach (GameObject gameObject in sceneAt.GetRootGameObjects())
						{
							flag |= GTExt._TryFindAllByPath(gameObject.transform, pathPartsRegex, index, results, caseSensitive, false);
						}
					}
				}
			}
			text = pathPartsRegex[index];
			if (!(text == "."))
			{
				if (!(text == ".."))
				{
					Transform transform3;
					if (!(text == "**"))
					{
						if (!(text == "..**") && !(text == "**.."))
						{
							if (Regex.IsMatch(current.name, pathPartsRegex[index], caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
							{
								if (index == pathPartsRegex.Count - 1)
								{
									results.Add(current);
									return true;
								}
								foreach (object obj in current)
								{
									Transform transform = (Transform)obj;
									flag |= GTExt._TryFindAllByPath(transform, pathPartsRegex, index + 1, results, caseSensitive, false);
								}
							}
						}
						else
						{
							int k;
							for (k = index + 1; k < pathPartsRegex.Count; k++)
							{
								string text2 = pathPartsRegex[k];
								if (!(text2 == "..**") && !(text2 == "**.."))
								{
									break;
								}
							}
							if (k == pathPartsRegex.Count)
							{
								results.Add(current.root);
								return true;
							}
							Transform transform2 = current;
							while (transform2)
							{
								flag |= GTExt._TryFindAllByPath(transform2, pathPartsRegex, index + 1, results, caseSensitive, false);
								transform2 = transform2.parent;
							}
						}
					}
					else if (index == pathPartsRegex.Count - 1)
					{
						for (int l = 0; l < current.childCount; l++)
						{
							results.Add(current.GetChild(l));
							flag = true;
						}
					}
					else if (GTExt._TryBreadthFirstSearchNames(current, pathPartsRegex[index + 1], out transform3, caseSensitive))
					{
						if (index + 2 == pathPartsRegex.Count)
						{
							results.Add(transform3);
							return true;
						}
						flag |= GTExt._TryFindAllByPath(transform3, pathPartsRegex, index + 2, results, caseSensitive, false);
					}
				}
				else if (current.parent)
				{
					if (index == pathPartsRegex.Count - 1)
					{
						results.Add(current.parent);
						return true;
					}
					flag |= GTExt._TryFindAllByPath(current.parent, pathPartsRegex, index + 1, results, caseSensitive, false);
				}
			}
			else
			{
				if (index == pathPartsRegex.Count - 1)
				{
					results.Add(current);
					return true;
				}
				flag |= GTExt._TryFindAllByPath(current, pathPartsRegex, index + 1, results, caseSensitive, false);
			}
			return flag;
		}

		// Token: 0x060073EB RID: 29675 RVA: 0x0025A9B0 File Offset: 0x00258BB0
		public static string[] _GlobPathToPathPartsRegex(string path)
		{
			string[] array = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (i > 0)
				{
					string text = array[i];
					if (text == "**" || text == "..**" || text == "**..")
					{
						text = array[i - 1];
						if (text == "**" || text == "..**" || text == "**..")
						{
							num++;
						}
					}
				}
				array[i - num] = array[i];
			}
			if (num > 0)
			{
				Array.Resize<string>(ref array, array.Length - num);
			}
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = GTExt._GlobPathPartToRegex(array[j]);
			}
			return array;
		}

		// Token: 0x060073EC RID: 29676 RVA: 0x0025AA70 File Offset: 0x00258C70
		private static string _GlobPathPartToRegex(string pattern)
		{
			if (pattern == "." || pattern == ".." || pattern == "**" || pattern == "..**" || pattern == "**.." || pattern.StartsWith("^"))
			{
				return pattern;
			}
			return "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
		}

		// Token: 0x060073EE RID: 29678 RVA: 0x0025AB14 File Offset: 0x00258D14
		[CompilerGenerated]
		internal static void <GetComponentsInChildrenUntil>g__GetRecursive|7_0<T, TStop1>(Transform currentTransform, ref List<T> components, ref GTExt.<>c__DisplayClass7_0<T, TStop1> A_2) where T : Component where TStop1 : Component
		{
			foreach (object obj in currentTransform)
			{
				Transform transform = (Transform)obj;
				if ((A_2.includeInactive || transform.gameObject.activeSelf) && !(transform.GetComponent<TStop1>() != null))
				{
					T component = transform.GetComponent<T>();
					if (component != null)
					{
						components.Add(component);
					}
					GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|7_0<T, TStop1>(transform, ref components, ref A_2);
				}
			}
		}

		// Token: 0x060073EF RID: 29679 RVA: 0x0025ABB0 File Offset: 0x00258DB0
		[CompilerGenerated]
		internal static void <GetComponentsInChildrenUntil>g__GetRecursive|10_0<T, TStop1, TStop2>(Transform currentTransform, ref List<T> components, ref GTExt.<>c__DisplayClass10_0<T, TStop1, TStop2> A_2) where T : Component where TStop1 : Component where TStop2 : Component
		{
			foreach (object obj in currentTransform)
			{
				Transform transform = (Transform)obj;
				if ((A_2.includeInactive || transform.gameObject.activeSelf) && !(transform.GetComponent<TStop1>() != null) && !(transform.GetComponent<TStop2>() != null))
				{
					T component = transform.GetComponent<T>();
					if (component != null)
					{
						components.Add(component);
					}
					GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|10_0<T, TStop1, TStop2>(transform, ref components, ref A_2);
				}
			}
		}

		// Token: 0x060073F0 RID: 29680 RVA: 0x0025AC60 File Offset: 0x00258E60
		[CompilerGenerated]
		internal static void <GetComponentsInChildrenUntil>g__GetRecursive|11_0<T, TStop1, TStop2, TStop3>(Transform currentTransform, ref List<T> components, ref GTExt.<>c__DisplayClass11_0<T, TStop1, TStop2, TStop3> A_2) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			foreach (object obj in currentTransform)
			{
				Transform transform = (Transform)obj;
				if ((A_2.includeInactive || transform.gameObject.activeSelf) && !(transform.GetComponent<TStop1>() != null) && !(transform.GetComponent<TStop2>() != null) && !(transform.GetComponent<TStop3>() != null))
				{
					T component = transform.GetComponent<T>();
					if (component != null)
					{
						components.Add(component);
					}
					GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|11_0<T, TStop1, TStop2, TStop3>(transform, ref components, ref A_2);
				}
			}
		}

		// Token: 0x04008380 RID: 33664
		private static Dictionary<Transform, Dictionary<string, Transform>> caseSenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();

		// Token: 0x04008381 RID: 33665
		private static Dictionary<Transform, Dictionary<string, Transform>> caseInsenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();

		// Token: 0x04008382 RID: 33666
		public static Dictionary<string, string> allStringsUsed = new Dictionary<string, string>();

		// Token: 0x020011CC RID: 4556
		public enum ParityOptions
		{
			// Token: 0x04008384 RID: 33668
			XFlip,
			// Token: 0x04008385 RID: 33669
			YFlip,
			// Token: 0x04008386 RID: 33670
			ZFlip,
			// Token: 0x04008387 RID: 33671
			AllFlip
		}
	}
}
