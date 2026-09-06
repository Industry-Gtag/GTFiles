using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// Token: 0x02000E2D RID: 3629
public class SynthesisColumns : MonoBehaviour
{
	// Token: 0x060058CB RID: 22731 RVA: 0x001CD43D File Offset: 0x001CB63D
	internal static SynthesisColumns.SynthesisColumnLayoutPayload ParseColumnsJson(string json)
	{
		return JsonUtility.FromJson<SynthesisColumns.SynthesisColumnLayoutPayload>(json);
	}

	// Token: 0x060058CC RID: 22732 RVA: 0x001CD445 File Offset: 0x001CB645
	internal void LoadLayoutFromJson(string json)
	{
		this._currentLayout = SynthesisColumns.ParseColumnsJson(json);
		this.SpawnForActiveScene();
	}

	// Token: 0x060058CD RID: 22733 RVA: 0x001CD45C File Offset: 0x001CB65C
	internal void AutoAssignFromResources()
	{
		if (this.defaultColumnPrefab == null)
		{
			this.defaultColumnPrefab = Resources.Load<GameObject>("Prefab_Column_Default");
			if (this.defaultColumnPrefab == null)
			{
				Debug.LogWarning("[Columns] Could not auto-load defaultColumnPrefab from Resources/Prefab_Column_Default.prefab");
			}
		}
		if (this.defaultRectPrefab == null)
		{
			this.defaultRectPrefab = Resources.Load<GameObject>("Prefab_Rect_Default");
			if (this.defaultRectPrefab == null)
			{
				Debug.LogWarning("[Columns] Missing Resources/Prefab_Rect_Default.prefab");
			}
		}
		if (this.defaultMaterialURP == null)
		{
			this.defaultMaterialURP = Resources.Load<Material>("Material_URP_Cylinder");
			if (this.defaultMaterialURP == null)
			{
				Debug.LogWarning("[Columns] Could not auto-load defaultMaterialURP from Resources/Material_URP_Cylinder.mat");
			}
		}
		if (this.defaultMaterialBuiltin == null)
		{
			this.defaultMaterialBuiltin = Resources.Load<Material>("Material_Builtin_Cylinder");
			if (this.defaultMaterialBuiltin == null)
			{
				Debug.LogWarning("[Columns] Could not auto-load defaultMaterialBuiltin from Resources/Material_Builtin_Cylinder.mat");
			}
		}
		if (this.textureLibrary == null)
		{
			this.textureLibrary = new List<SynthesisColumns.TextureMapping>();
		}
		Texture2D texture2D = Resources.Load<Texture2D>("danger_pattern");
		if (texture2D == null)
		{
			Debug.LogWarning("[Columns] Could not auto-load danger_pattern.png from Resources/danger_pattern.png");
			return;
		}
		bool flag = false;
		for (int i = 0; i < this.textureLibrary.Count; i++)
		{
			if (this.textureLibrary[i] != null && this.textureLibrary[i].key == "danger_pattern")
			{
				if (this.textureLibrary[i].texture == null)
				{
					this.textureLibrary[i].texture = texture2D;
				}
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			this.textureLibrary.Add(new SynthesisColumns.TextureMapping
			{
				key = "danger_pattern",
				texture = texture2D
			});
		}
	}

	// Token: 0x060058CE RID: 22734 RVA: 0x001CD60C File Offset: 0x001CB80C
	private static SynthesisColumns.ColumnShape ParseShape(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return SynthesisColumns.ColumnShape.Cylinder;
		}
		string text = s.Trim().ToLowerInvariant();
		if (text == "rect" || text == "rectangle" || text == "box" || text == "quad")
		{
			return SynthesisColumns.ColumnShape.Rectangle;
		}
		return SynthesisColumns.ColumnShape.Cylinder;
	}

	// Token: 0x060058CF RID: 22735 RVA: 0x001CD666 File Offset: 0x001CB866
	private SynthesisColumns.RenderPipelineType GetActivePipeline()
	{
		if (GraphicsSettings.currentRenderPipeline == null)
		{
			return SynthesisColumns.RenderPipelineType.Builtin;
		}
		return SynthesisColumns.RenderPipelineType.SRP_URP;
	}

	// Token: 0x060058D0 RID: 22736 RVA: 0x001CD678 File Offset: 0x001CB878
	private string GetSkinsRootFolder()
	{
		if (string.IsNullOrEmpty(this._skinsRoot))
		{
			this._skinsRoot = Path.Combine(Application.persistentDataPath, "PlayspaceSkins");
			if (!Directory.Exists(this._skinsRoot))
			{
				Directory.CreateDirectory(this._skinsRoot);
			}
		}
		return this._skinsRoot;
	}

	// Token: 0x060058D1 RID: 22737 RVA: 0x001CD6C6 File Offset: 0x001CB8C6
	internal void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (this._lastSceneSpawnedFor == scene.name)
		{
			return;
		}
		this._lastSceneSpawnedFor = scene.name;
		this.SpawnForActiveScene();
	}

	// Token: 0x060058D2 RID: 22738 RVA: 0x001CD6F0 File Offset: 0x001CB8F0
	private void SafeClearSpawnedColumns()
	{
		if (this._spawnedColumns.Count == 0)
		{
			return;
		}
		List<GameObject> list = new List<GameObject>(this._spawnedColumns);
		for (int i = 0; i < list.Count; i++)
		{
			GameObject gameObject = list[i];
			if (!(gameObject == null))
			{
				Collider[] componentsInChildren = gameObject.GetComponentsInChildren<Collider>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].enabled = false;
				}
				gameObject.SetActive(false);
			}
		}
		this._spawnedColumns.Clear();
		base.StartCoroutine(this.DestroyAfterPhysicsStep(list));
	}

	// Token: 0x060058D3 RID: 22739 RVA: 0x001CD77C File Offset: 0x001CB97C
	private IEnumerator DestroyAfterPhysicsStep(List<GameObject> toDestroy)
	{
		yield return new WaitForFixedUpdate();
		yield return null;
		for (int i = 0; i < toDestroy.Count; i++)
		{
			GameObject gameObject = toDestroy[i];
			if (gameObject != null)
			{
				Object.Destroy(gameObject);
			}
		}
		yield break;
	}

	// Token: 0x060058D4 RID: 22740 RVA: 0x001CD78C File Offset: 0x001CB98C
	private void SpawnForActiveScene()
	{
		this.SafeClearSpawnedColumns();
		if (this._currentLayout == null || this._currentLayout.columns == null)
		{
			Debug.LogWarning("[ColumnManager] No layout loaded, nothing to spawn.");
			return;
		}
		if (this._lastSceneSpawnedFor == null || this._lastSceneSpawnedFor.Length == 0)
		{
			Debug.LogWarning("[ColumnManager] No Scene loaded.");
			return;
		}
		foreach (SynthesisColumns.SynthesisColumnLayoutEntry synthesisColumnLayoutEntry in this._currentLayout.columns)
		{
			if ((synthesisColumnLayoutEntry.texture ?? "") == "" && this.textureLibrary.Count > 0)
			{
				synthesisColumnLayoutEntry.texture = this.textureLibrary.First<SynthesisColumns.TextureMapping>().key;
			}
			this.SpawnSingleColumn(synthesisColumnLayoutEntry);
		}
	}

	// Token: 0x060058D5 RID: 22741 RVA: 0x001CD86C File Offset: 0x001CBA6C
	private void SpawnSingleColumn(SynthesisColumns.SynthesisColumnLayoutEntry entry)
	{
		SynthesisColumns.ColumnShape columnShape = SynthesisColumns.ParseShape(entry.shape);
		GameObject gameObject = ((columnShape == SynthesisColumns.ColumnShape.Rectangle) ? this.defaultRectPrefab : this.defaultColumnPrefab);
		if (gameObject == null)
		{
			Debug.LogWarning("[ColumnManager] Missing prefab for shape " + columnShape.ToString());
			return;
		}
		Debug.LogWarning(string.Format("[ColumnManager][Scene={0}] Spawn new column {1} ; Prefab=[{2}]", this._lastSceneSpawnedFor, columnShape, gameObject));
		GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject);
		gameObject2.transform.SetParent(base.transform, true);
		Vector3 vector = new Vector3(entry.x, 0f, entry.y);
		Quaternion quaternion = Quaternion.Euler(0f, entry.rotate, 0f);
		gameObject2.transform.SetPositionAndRotation(vector, quaternion);
		Vector3 localScale = gameObject2.transform.localScale;
		if (columnShape == SynthesisColumns.ColumnShape.Cylinder)
		{
			float num = Mathf.Max(0.01f, entry.diameter);
			localScale.x = num;
			localScale.z = num;
			localScale.y = ((entry.height > 0f) ? entry.height : localScale.y);
		}
		else
		{
			float num2 = Mathf.Max(0.01f, entry.width);
			float num3 = Mathf.Max(0.01f, entry.depth);
			localScale.x = num2;
			localScale.z = num3;
			localScale.y = ((entry.height > 0f) ? entry.height : localScale.y);
		}
		gameObject2.transform.localScale = localScale;
		if (columnShape == SynthesisColumns.ColumnShape.Cylinder)
		{
			this.ApplyTexture(gameObject2, entry.texture, entry.diameter);
		}
		else
		{
			Transform transform = gameObject2.transform.Find("Visual");
			MeshFilter meshFilter = (transform ? transform.GetComponent<MeshFilter>() : null);
			if (meshFilter != null)
			{
				transform.localScale = Vector3.one;
				transform.localRotation = Quaternion.identity;
				Mesh mesh = SynthesisColumns.BuildStripUVBox(localScale.x, localScale.z, localScale.y, 0.5f);
				meshFilter.sharedMesh = mesh;
			}
			else
			{
				Debug.LogWarning("[Columns] Rectangle has no MeshFilter to swap.");
			}
			float y = meshFilter.sharedMesh.bounds.size.y;
			transform.localPosition = new Vector3(0f, y * 0.5f, 0f);
			gameObject2.transform.localScale = Vector3.one;
			BoxCollider component = gameObject2.GetComponent<BoxCollider>();
			if (component)
			{
				component.size = new Vector3(localScale.x, localScale.z, localScale.y);
				component.center = new Vector3(0f, localScale.z * 0.5f, 0f);
			}
			float num4 = 2f * (localScale.x + localScale.z) / 3.1415927f;
			this.ApplyTexture(gameObject2, entry.texture, num4);
		}
		this._spawnedColumns.Add(gameObject2);
	}

	// Token: 0x060058D6 RID: 22742 RVA: 0x001CDB60 File Offset: 0x001CBD60
	private void ApplyTexture(GameObject columnGO, string textureKey, float worldDiameterM)
	{
		MeshRenderer componentInChildren = columnGO.GetComponentInChildren<MeshRenderer>();
		if (componentInChildren == null)
		{
			return;
		}
		SynthesisColumns.ColumnVisualRuntimeData columnVisualRuntimeData = componentInChildren.GetComponent<SynthesisColumns.ColumnVisualRuntimeData>();
		if (columnVisualRuntimeData == null)
		{
			columnVisualRuntimeData = componentInChildren.gameObject.AddComponent<SynthesisColumns.ColumnVisualRuntimeData>();
		}
		columnVisualRuntimeData.worldDiameterM = worldDiameterM;
		Material material = ((this.GetActivePipeline() == SynthesisColumns.RenderPipelineType.SRP_URP) ? this.defaultMaterialURP : this.defaultMaterialBuiltin);
		if (string.IsNullOrEmpty(textureKey))
		{
			if (material != null)
			{
				componentInChildren.material = new Material(material);
			}
			return;
		}
		Texture2D texture2D = this.FindTexture(textureKey);
		if (texture2D != null)
		{
			Debug.Log(string.Concat(new string[]
			{
				"[Columns] ApplyTexture immediate for key=",
				textureKey,
				" tex=",
				texture2D.width.ToString(),
				"x",
				texture2D.height.ToString(),
				" ; Diameter=",
				worldDiameterM.ToString()
			}));
			this.ApplyTextureToRenderer(componentInChildren, texture2D, worldDiameterM);
			return;
		}
		Debug.Log("[Columns] Texture not ready yet for key=" + textureKey + " - registering pending.");
		if (material != null)
		{
			componentInChildren.material = new Material(material);
		}
		if (this._pendingTextureAssignments == null)
		{
			this._pendingTextureAssignments = new Dictionary<string, List<MeshRenderer>>();
		}
		List<MeshRenderer> list;
		if (!this._pendingTextureAssignments.TryGetValue(textureKey, out list))
		{
			list = new List<MeshRenderer>();
			this._pendingTextureAssignments[textureKey] = list;
		}
		if (!list.Contains(componentInChildren))
		{
			list.Add(componentInChildren);
		}
	}

	// Token: 0x060058D7 RID: 22743 RVA: 0x001CDCC8 File Offset: 0x001CBEC8
	private Texture2D FindTexture(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return null;
		}
		for (int i = 0; i < this.textureLibrary.Count; i++)
		{
			if (this.textureLibrary[i].key == key && this.textureLibrary[i].texture != null)
			{
				return this.textureLibrary[i].texture;
			}
		}
		if (this._dynamicTextureCache == null)
		{
			this._dynamicTextureCache = new Dictionary<string, Texture2D>();
		}
		Texture2D texture2D;
		if (this._dynamicTextureCache.TryGetValue(key, out texture2D))
		{
			Debug.Log("[Columns] FindTexture('" + key + "') -> cached in memory");
			return texture2D;
		}
		Texture2D texture2D2 = this.TryLoadTextureFromDisk(key);
		if (texture2D2 != null)
		{
			Debug.Log("[Columns] FindTexture('" + key + "') -> loaded from disk");
			this._dynamicTextureCache[key] = texture2D2;
			return texture2D2;
		}
		if (this.IsHttpUrl(key))
		{
			Debug.Log("[Columns] FindTexture('" + key + "') -> not cached, starting download");
			base.StartCoroutine(this.DownloadAndCacheTextureCoroutine(key));
		}
		else
		{
			Debug.Log("[Columns] FindTexture('" + key + "') -> MISS (no file, not URL)");
		}
		return null;
	}

	// Token: 0x060058D8 RID: 22744 RVA: 0x001CDDEC File Offset: 0x001CBFEC
	private bool IsHttpUrl(string s)
	{
		return s.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || s.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
	}

	// Token: 0x060058D9 RID: 22745 RVA: 0x001CDE0C File Offset: 0x001CC00C
	private string GetCacheFilePathForUrl(string url)
	{
		string text2;
		using (MD5 md = MD5.Create())
		{
			string text = BitConverter.ToString(md.ComputeHash(Encoding.UTF8.GetBytes(url))).Replace("-", "").ToLowerInvariant();
			text2 = Path.Combine(this.GetSkinsRootFolder(), "cache_" + text + ".png");
		}
		return text2;
	}

	// Token: 0x060058DA RID: 22746 RVA: 0x001CDE84 File Offset: 0x001CC084
	private Texture2D TryLoadTextureFromDisk(string keyOrUrl)
	{
		string text;
		if (this.IsHttpUrl(keyOrUrl))
		{
			text = this.GetCacheFilePathForUrl(keyOrUrl);
		}
		else
		{
			text = Path.Combine(this.GetSkinsRootFolder(), keyOrUrl);
		}
		if (!File.Exists(text))
		{
			return null;
		}
		Texture2D texture2D2;
		try
		{
			byte[] array = File.ReadAllBytes(text);
			Texture2D texture2D = new Texture2D(2, 2, TextureFormat.RGBA32, false);
			if (!texture2D.LoadImage(array))
			{
				Debug.LogWarning("[Columns] Failed to decode image at " + text);
				texture2D2 = null;
			}
			else
			{
				texture2D.wrapMode = TextureWrapMode.Repeat;
				texture2D.filterMode = FilterMode.Bilinear;
				texture2D.Apply();
				texture2D2 = texture2D;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[Columns] Error loading texture from " + text + " -> " + ex.Message);
			texture2D2 = null;
		}
		return texture2D2;
	}

	// Token: 0x060058DB RID: 22747 RVA: 0x001CDF38 File Offset: 0x001CC138
	private IEnumerator DownloadAndCacheTextureCoroutine(string url)
	{
		string cachePath = this.GetCacheFilePathForUrl(url);
		if (this._dynamicTextureCache == null)
		{
			this._dynamicTextureCache = new Dictionary<string, Texture2D>();
		}
		if (File.Exists(cachePath))
		{
			Texture2D texture2D = this.TryLoadTextureFromDisk(url);
			if (texture2D != null)
			{
				this._dynamicTextureCache[url] = texture2D;
				this.ApplyDownloadedTextureToPending(url, texture2D);
			}
			yield break;
		}
		using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(url))
		{
			yield return req.SendWebRequest();
			if (req.result != UnityWebRequest.Result.Success)
			{
				Debug.LogWarning("[Columns] Download failed: " + req.error + " url=" + url);
				yield break;
			}
			Texture2D content = DownloadHandlerTexture.GetContent(req);
			if (content == null)
			{
				Debug.LogWarning("[Columns] Got empty texture from " + url);
				yield break;
			}
			try
			{
				byte[] array = content.EncodeToPNG();
				File.WriteAllBytes(cachePath, array);
			}
			catch (Exception ex)
			{
				Debug.LogError("[Columns] Failed saving cache PNG for " + url + " -> " + ex.Message);
			}
			this._dynamicTextureCache[url] = content;
			this.ApplyDownloadedTextureToPending(url, content);
		}
		UnityWebRequest req = null;
		yield break;
		yield break;
	}

	// Token: 0x060058DC RID: 22748 RVA: 0x001CDF50 File Offset: 0x001CC150
	private void ApplyDownloadedTextureToPending(string textureKey, Texture2D tex)
	{
		if (tex == null)
		{
			return;
		}
		if (this._pendingTextureAssignments == null)
		{
			return;
		}
		List<MeshRenderer> list;
		if (!this._pendingTextureAssignments.TryGetValue(textureKey, out list))
		{
			return;
		}
		Debug.Log(string.Concat(new string[]
		{
			"[Columns] Fulfilling pending texture key=",
			textureKey,
			" for ",
			list.Count.ToString(),
			" renderer(s)"
		}));
		for (int i = 0; i < list.Count; i++)
		{
			MeshRenderer meshRenderer = list[i];
			if (!(meshRenderer == null))
			{
				SynthesisColumns.ColumnVisualRuntimeData columnVisualRuntimeData = ((meshRenderer != null) ? meshRenderer.GetComponent<SynthesisColumns.ColumnVisualRuntimeData>() : null);
				float num = ((columnVisualRuntimeData != null) ? columnVisualRuntimeData.worldDiameterM : 0.35f);
				this.ApplyTextureToRenderer(meshRenderer, tex, num);
			}
		}
		this._pendingTextureAssignments.Remove(textureKey);
	}

	// Token: 0x060058DD RID: 22749 RVA: 0x001CE028 File Offset: 0x001CC228
	private void ApplyTextureToRenderer(MeshRenderer r, Texture2D tex, float worldDiameterM)
	{
		if (r == null || tex == null)
		{
			return;
		}
		SynthesisColumns.RenderPipelineType activePipeline = this.GetActivePipeline();
		Material material;
		if (activePipeline == SynthesisColumns.RenderPipelineType.SRP_URP)
		{
			if (this.defaultMaterialURP == null)
			{
				Debug.LogError("[Columns] No URP base material assigned, cannot skin column.");
				return;
			}
			material = this.defaultMaterialURP;
		}
		else
		{
			if (this.defaultMaterialBuiltin == null)
			{
				Debug.LogError("[Columns] No Built-in base material assigned, cannot skin column.");
				return;
			}
			material = this.defaultMaterialBuiltin;
		}
		Material material2 = new Material(material);
		if (material2.HasProperty("_MainTex"))
		{
			material2.SetTexture("_MainTex", tex);
		}
		if (material2.HasProperty("_BaseMap"))
		{
			material2.SetTexture("_BaseMap", tex);
		}
		material2.mainTexture = tex;
		Color white = Color.white;
		white.a = 0.9f;
		if (material2.HasProperty("_BaseColor"))
		{
			material2.SetColor("_BaseColor", white);
		}
		if (material2.HasProperty("_Color"))
		{
			material2.SetColor("_Color", white);
		}
		if (activePipeline == SynthesisColumns.RenderPipelineType.SRP_URP)
		{
			if (material2.HasProperty("_Surface"))
			{
				material2.SetFloat("_Surface", 1f);
			}
			if (material2.HasProperty("_AlphaClip"))
			{
				material2.SetFloat("_AlphaClip", 0f);
			}
			material2.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
			material2.DisableKeyword("_SURFACE_TYPE_OPAQUE");
			material2.DisableKeyword("_ALPHATEST_ON");
		}
		float num = this.ComputeTileY_FromAspect(tex, 1f, 0.5f);
		num = Mathf.Clamp(num, 1f, 8f);
		float num2 = 1f;
		if (material2.HasProperty("_BaseMap_ST"))
		{
			Vector4 vector = material2.GetVector("_BaseMap_ST");
			vector.x = num2;
			vector.y = num;
			vector.z = 0f;
			vector.w = 0f;
			material2.SetVector("_BaseMap_ST", vector);
		}
		material2.mainTextureScale = new Vector2(num2, num);
		material2.mainTextureOffset = Vector2.zero;
		Material[] materials = r.materials;
		for (int i = 0; i < materials.Length; i++)
		{
			materials[i] = material2;
		}
		r.materials = materials;
		Debug.Log(string.Concat(new string[]
		{
			"[Columns] Applied runtime transparent material to ",
			r.name,
			" shader=",
			material2.shader.name,
			" tex=",
			tex.width.ToString(),
			"x",
			tex.height.ToString(),
			" tiling=",
			num2.ToString(),
			"x",
			num.ToString(),
			" slots=",
			materials.Length.ToString()
		}));
	}

	// Token: 0x060058DE RID: 22750 RVA: 0x001CE2E8 File Offset: 0x001CC4E8
	private static Mesh BuildStripUVBox(float w, float d, float h, float tileMeters)
	{
		float num = w * 0.5f;
		float num2 = h * 0.5f;
		float num3 = d * 0.5f;
		float num4 = Mathf.Max(1f, Mathf.Round(w / tileMeters));
		float num5 = Mathf.Max(1f, Mathf.Round(d / tileMeters));
		float num6 = 0f;
		float num7 = num6 + num4;
		float num8 = num7 + num5;
		float num9 = num8 + num4;
		float num10 = num9 + num5;
		SynthesisColumns.<>c__DisplayClass37_0 CS$<>8__locals1;
		CS$<>8__locals1.verts = new Vector3[24];
		CS$<>8__locals1.uvs = new Vector2[24];
		CS$<>8__locals1.tris = new int[36];
		CS$<>8__locals1.vi = 0;
		CS$<>8__locals1.ti = 0;
		SynthesisColumns.<BuildStripUVBox>g__Face|37_0(new Vector3(-num, -num2, num3), new Vector3(-num, num2, num3), new Vector3(num, num2, num3), new Vector3(num, -num2, num3), new Vector2(num7, 0f), new Vector2(num7, 1f), new Vector2(num6, 1f), new Vector2(num6, 0f), ref CS$<>8__locals1);
		SynthesisColumns.<BuildStripUVBox>g__Face|37_0(new Vector3(num, -num2, num3), new Vector3(num, num2, num3), new Vector3(num, num2, -num3), new Vector3(num, -num2, -num3), new Vector2(num8, 0f), new Vector2(num8, 1f), new Vector2(num7, 1f), new Vector2(num7, 0f), ref CS$<>8__locals1);
		SynthesisColumns.<BuildStripUVBox>g__Face|37_0(new Vector3(num, -num2, -num3), new Vector3(num, num2, -num3), new Vector3(-num, num2, -num3), new Vector3(-num, -num2, -num3), new Vector2(num9, 0f), new Vector2(num9, 1f), new Vector2(num8, 1f), new Vector2(num8, 0f), ref CS$<>8__locals1);
		SynthesisColumns.<BuildStripUVBox>g__Face|37_0(new Vector3(-num, -num2, -num3), new Vector3(-num, num2, -num3), new Vector3(-num, num2, num3), new Vector3(-num, -num2, num3), new Vector2(num10, 0f), new Vector2(num10, 1f), new Vector2(num9, 1f), new Vector2(num9, 0f), ref CS$<>8__locals1);
		SynthesisColumns.<BuildStripUVBox>g__Face|37_0(new Vector3(-num, num2, num3), new Vector3(-num, num2, -num3), new Vector3(num, num2, -num3), new Vector3(num, num2, num3), new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f), ref CS$<>8__locals1);
		SynthesisColumns.<BuildStripUVBox>g__Face|37_0(new Vector3(-num, -num2, -num3), new Vector3(-num, -num2, num3), new Vector3(num, -num2, num3), new Vector3(num, -num2, -num3), new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f), ref CS$<>8__locals1);
		Mesh mesh = new Mesh();
		mesh.name = "StripUVBox_IntRepeats";
		mesh.vertices = CS$<>8__locals1.verts;
		mesh.uv = CS$<>8__locals1.uvs;
		mesh.triangles = CS$<>8__locals1.tris;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.RecalculateTangents();
		return mesh;
	}

	// Token: 0x060058DF RID: 22751 RVA: 0x001CE620 File Offset: 0x001CC820
	private float ComputeTileY_FromAspect(Texture2D tex, float heightMeters, float tileMetersX)
	{
		if (tex == null || tileMetersX <= 0f)
		{
			return 1f;
		}
		float num = tileMetersX * ((float)tex.height / (float)Mathf.Max(1, tex.width));
		float num2 = heightMeters / Mathf.Max(0.0001f, num);
		num2 = Mathf.Max(1f, Mathf.Round(num2));
		return Mathf.Max(1f, num2);
	}

	// Token: 0x060058E0 RID: 22752 RVA: 0x001CE688 File Offset: 0x001CC888
	internal string readColumnsFromJsonFile(SynthesisArcadeObject _instance)
	{
		string text = _instance.ReadCommandLineArgument("-svrcolumns") ?? "{}";
		if (File.Exists(Path.Combine(Application.persistentDataPath, "Columns", text)))
		{
			try
			{
				text = File.ReadAllText(Path.Combine(Application.persistentDataPath, "Columns", text));
				goto IL_00DD;
			}
			catch (Exception ex)
			{
				Debug.LogError("[" + _instance.TAG + "] [COLUMNS_JSON_ERROR/1] => " + ex.Message);
				goto IL_00DD;
			}
		}
		if (File.Exists(Path.Combine(Environment.ExpandEnvironmentVariables("%localappdata%Low\\SynthesisVR\\Columns"), text)))
		{
			try
			{
				text = File.ReadAllText(Path.Combine(Environment.ExpandEnvironmentVariables("%localappdata%Low\\SynthesisVR\\Columns"), text));
				goto IL_00DD;
			}
			catch (Exception ex2)
			{
				Debug.LogError("[" + _instance.TAG + "] [COLUMNS_JSON_ERROR/2] => " + ex2.Message);
				goto IL_00DD;
			}
		}
		Debug.Log("[" + _instance.TAG + "] Column File Does not Exists = " + text);
		text = "{}";
		IL_00DD:
		return text ?? "{}";
	}

	// Token: 0x060058E2 RID: 22754 RVA: 0x001CE7B8 File Offset: 0x001CC9B8
	[CompilerGenerated]
	internal static void <BuildStripUVBox>g__Face|37_0(Vector3 bl, Vector3 tl, Vector3 tr, Vector3 br, Vector2 uvBL, Vector2 uvTL, Vector2 uvTR, Vector2 uvBR, ref SynthesisColumns.<>c__DisplayClass37_0 A_8)
	{
		A_8.verts[A_8.vi] = bl;
		A_8.uvs[A_8.vi] = uvBL;
		A_8.verts[A_8.vi + 1] = tl;
		A_8.uvs[A_8.vi + 1] = uvTL;
		A_8.verts[A_8.vi + 2] = tr;
		A_8.uvs[A_8.vi + 2] = uvTR;
		A_8.verts[A_8.vi + 3] = br;
		A_8.uvs[A_8.vi + 3] = uvBR;
		A_8.tris[A_8.ti] = A_8.vi;
		A_8.tris[A_8.ti + 1] = A_8.vi + 2;
		A_8.tris[A_8.ti + 2] = A_8.vi + 1;
		A_8.tris[A_8.ti + 3] = A_8.vi;
		A_8.tris[A_8.ti + 4] = A_8.vi + 3;
		A_8.tris[A_8.ti + 5] = A_8.vi + 2;
		A_8.vi += 4;
		A_8.ti += 6;
	}

	// Token: 0x040068F7 RID: 26871
	[Header("Default column prefab (cylinder mesh + collider)")]
	internal GameObject defaultColumnPrefab;

	// Token: 0x040068F8 RID: 26872
	[Header("Default rectangular prefab (box mesh + collider)")]
	internal GameObject defaultRectPrefab;

	// Token: 0x040068F9 RID: 26873
	[Header("Column materials per pipeline")]
	internal Material defaultMaterialURP;

	// Token: 0x040068FA RID: 26874
	internal Material defaultMaterialBuiltin;

	// Token: 0x040068FB RID: 26875
	[Header("Optional known textures (can be Resources, Addressables, etc.)")]
	public List<SynthesisColumns.TextureMapping> textureLibrary;

	// Token: 0x040068FC RID: 26876
	private SynthesisColumns.SynthesisColumnLayoutPayload _currentLayout;

	// Token: 0x040068FD RID: 26877
	private Dictionary<string, Texture2D> _dynamicTextureCache = new Dictionary<string, Texture2D>();

	// Token: 0x040068FE RID: 26878
	private Dictionary<string, List<MeshRenderer>> _pendingTextureAssignments;

	// Token: 0x040068FF RID: 26879
	private string _lastSceneSpawnedFor;

	// Token: 0x04006900 RID: 26880
	private string _skinsRoot;

	// Token: 0x04006901 RID: 26881
	private readonly List<GameObject> _spawnedColumns = new List<GameObject>();

	// Token: 0x04006902 RID: 26882
	internal MaterialPropertyBlock _mpb;

	// Token: 0x02000E2E RID: 3630
	public enum ColumnShape
	{
		// Token: 0x04006904 RID: 26884
		Cylinder,
		// Token: 0x04006905 RID: 26885
		Rectangle
	}

	// Token: 0x02000E2F RID: 3631
	[Serializable]
	public class SynthesisColumnLayoutPayload
	{
		// Token: 0x04006906 RID: 26886
		public List<SynthesisColumns.SynthesisColumnLayoutEntry> columns;
	}

	// Token: 0x02000E30 RID: 3632
	[Serializable]
	public class SynthesisColumnLayoutEntry
	{
		// Token: 0x04006907 RID: 26887
		public string shape;

		// Token: 0x04006908 RID: 26888
		public float x;

		// Token: 0x04006909 RID: 26889
		public float y;

		// Token: 0x0400690A RID: 26890
		public float diameter;

		// Token: 0x0400690B RID: 26891
		public float width;

		// Token: 0x0400690C RID: 26892
		public float depth;

		// Token: 0x0400690D RID: 26893
		public float height;

		// Token: 0x0400690E RID: 26894
		public string texture;

		// Token: 0x0400690F RID: 26895
		public float rotate;
	}

	// Token: 0x02000E31 RID: 3633
	[Serializable]
	public class TextureMapping
	{
		// Token: 0x04006910 RID: 26896
		public string key;

		// Token: 0x04006911 RID: 26897
		public Texture2D texture;
	}

	// Token: 0x02000E32 RID: 3634
	[Serializable]
	private class ColumnVisualRuntimeData : MonoBehaviour
	{
		// Token: 0x04006912 RID: 26898
		public float worldDiameterM;
	}

	// Token: 0x02000E33 RID: 3635
	private enum RenderPipelineType
	{
		// Token: 0x04006914 RID: 26900
		Builtin,
		// Token: 0x04006915 RID: 26901
		SRP_URP
	}
}
