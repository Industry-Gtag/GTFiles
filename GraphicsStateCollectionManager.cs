using System;
using System.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;

// Token: 0x02000E05 RID: 3589
public class GraphicsStateCollectionManager : MonoBehaviour
{
	// Token: 0x060057E2 RID: 22498 RVA: 0x001CA4F0 File Offset: 0x001C86F0
	private GraphicsStateCollection FindExistingCollection()
	{
		for (int i = 0; i < this.collections.Length; i++)
		{
			if (this.collections[i] != null && this.collections[i].runtimePlatform == Application.platform && this.collections[i].graphicsDeviceType == SystemInfo.graphicsDeviceType && this.collections[i].qualityLevelName == QualitySettings.names[QualitySettings.GetQualityLevel()])
			{
				return this.collections[i];
			}
		}
		return null;
	}

	// Token: 0x060057E3 RID: 22499 RVA: 0x001CA574 File Offset: 0x001C8774
	private void Awake()
	{
		if (GraphicsStateCollectionManager.Instance != null && GraphicsStateCollectionManager.Instance != this)
		{
			Debug.LogError("Only one instance of GraphicsStateCollectionManager is allowed!");
			Object.Destroy(base.gameObject);
			return;
		}
		GraphicsStateCollectionManager.Instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x060057E4 RID: 22500 RVA: 0x001CA5C4 File Offset: 0x001C87C4
	private void Start()
	{
		if (this.mode == GraphicsStateCollectionManager.Mode.Tracing)
		{
			this.m_GraphicsStateCollection = this.FindExistingCollection();
			if (this.m_GraphicsStateCollection != null)
			{
				this.m_OutputCollectionName = "SharedAssets/GraphicsStateCollections/" + this.m_GraphicsStateCollection.name;
			}
			else
			{
				int qualityLevel = QualitySettings.GetQualityLevel();
				string text = QualitySettings.names[qualityLevel];
				text = text.Replace(" ", "");
				this.m_OutputCollectionName = string.Concat(new object[]
				{
					"SharedAssets/GraphicsStateCollections/",
					"GfxState_",
					Application.platform,
					"_",
					SystemInfo.graphicsDeviceType.ToString(),
					"_",
					text
				});
				this.m_GraphicsStateCollection = new GraphicsStateCollection();
			}
			Debug.Log("Tracing started for GraphicsStateCollection by Scene '" + SceneManager.GetActiveScene().name + "'.");
			this.m_GraphicsStateCollection.BeginTrace();
			this._autoSaveRoutine = base.StartCoroutine(this.AutoSaveRoutine());
			return;
		}
		GraphicsStateCollection graphicsStateCollection = this.FindExistingCollection();
		if (graphicsStateCollection != null)
		{
			Debug.Log(string.Concat(new string[]
			{
				"Scene '",
				SceneManager.GetActiveScene().name,
				"' started warming up ",
				graphicsStateCollection.totalGraphicsStateCount.ToString(),
				" GraphicsState entries."
			}));
			graphicsStateCollection.WarmUp(default(JobHandle));
		}
	}

	// Token: 0x060057E5 RID: 22501 RVA: 0x001CA748 File Offset: 0x001C8948
	private void OnApplicationFocus(bool focus)
	{
		if (!focus && this.mode == GraphicsStateCollectionManager.Mode.Tracing && this.m_GraphicsStateCollection != null)
		{
			Debug.Log("Focus changed. Sending collection to Editor with " + this.m_GraphicsStateCollection.totalGraphicsStateCount.ToString() + " GraphicsState entries.");
			this.m_GraphicsStateCollection.SendToEditor(this.m_OutputCollectionName);
		}
	}

	// Token: 0x060057E6 RID: 22502 RVA: 0x001CA7A8 File Offset: 0x001C89A8
	private void OnDestroy()
	{
		if (this._autoSaveRoutine != null)
		{
			base.StopCoroutine(this._autoSaveRoutine);
		}
		if (this.mode == GraphicsStateCollectionManager.Mode.Tracing && this.m_GraphicsStateCollection != null)
		{
			this.m_GraphicsStateCollection.EndTrace();
			Debug.Log("Sending collection to Editor with " + this.m_GraphicsStateCollection.totalGraphicsStateCount.ToString() + " GraphicsState entries.");
			this.m_GraphicsStateCollection.SendToEditor(this.m_OutputCollectionName);
		}
	}

	// Token: 0x060057E7 RID: 22503 RVA: 0x001CA823 File Offset: 0x001C8A23
	private IEnumerator AutoSaveRoutine()
	{
		for (;;)
		{
			yield return new WaitForSeconds(5f);
			if (this.mode == GraphicsStateCollectionManager.Mode.Tracing && this.m_GraphicsStateCollection != null)
			{
				Debug.Log("Auto-saving collection with " + this.m_GraphicsStateCollection.totalGraphicsStateCount.ToString() + " GraphicsState entries.");
				this.m_GraphicsStateCollection.SendToEditor(this.m_OutputCollectionName);
			}
		}
		yield break;
	}

	// Token: 0x04006848 RID: 26696
	public GraphicsStateCollectionManager.Mode mode;

	// Token: 0x04006849 RID: 26697
	public static GraphicsStateCollectionManager Instance;

	// Token: 0x0400684A RID: 26698
	public GraphicsStateCollection[] collections;

	// Token: 0x0400684B RID: 26699
	private const string k_CollectionFolderPath = "SharedAssets/GraphicsStateCollections/";

	// Token: 0x0400684C RID: 26700
	private string m_OutputCollectionName;

	// Token: 0x0400684D RID: 26701
	private GraphicsStateCollection m_GraphicsStateCollection;

	// Token: 0x0400684E RID: 26702
	private Coroutine _autoSaveRoutine;

	// Token: 0x02000E06 RID: 3590
	public enum Mode
	{
		// Token: 0x04006850 RID: 26704
		Tracing,
		// Token: 0x04006851 RID: 26705
		WarmUp
	}
}
