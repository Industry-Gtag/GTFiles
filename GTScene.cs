using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000E11 RID: 3601
[Serializable]
public class GTScene : IEquatable<GTScene>
{
	// Token: 0x1700085B RID: 2139
	// (get) Token: 0x06005818 RID: 22552 RVA: 0x001CAF8C File Offset: 0x001C918C
	public string alias
	{
		get
		{
			return this._alias;
		}
	}

	// Token: 0x1700085C RID: 2140
	// (get) Token: 0x06005819 RID: 22553 RVA: 0x001CAF94 File Offset: 0x001C9194
	public string name
	{
		get
		{
			return this._name;
		}
	}

	// Token: 0x1700085D RID: 2141
	// (get) Token: 0x0600581A RID: 22554 RVA: 0x001CAF9C File Offset: 0x001C919C
	public string path
	{
		get
		{
			return this._path;
		}
	}

	// Token: 0x1700085E RID: 2142
	// (get) Token: 0x0600581B RID: 22555 RVA: 0x001CAFA4 File Offset: 0x001C91A4
	public string guid
	{
		get
		{
			return this._guid;
		}
	}

	// Token: 0x1700085F RID: 2143
	// (get) Token: 0x0600581C RID: 22556 RVA: 0x001CAFAC File Offset: 0x001C91AC
	public int buildIndex
	{
		get
		{
			return this._buildIndex;
		}
	}

	// Token: 0x17000860 RID: 2144
	// (get) Token: 0x0600581D RID: 22557 RVA: 0x001CAFB4 File Offset: 0x001C91B4
	public bool includeInBuild
	{
		get
		{
			return this._includeInBuild;
		}
	}

	// Token: 0x17000861 RID: 2145
	// (get) Token: 0x0600581E RID: 22558 RVA: 0x001CAFBC File Offset: 0x001C91BC
	public bool isLoaded
	{
		get
		{
			return SceneManager.GetSceneByBuildIndex(this._buildIndex).isLoaded;
		}
	}

	// Token: 0x17000862 RID: 2146
	// (get) Token: 0x0600581F RID: 22559 RVA: 0x001CAFDC File Offset: 0x001C91DC
	public bool hasAlias
	{
		get
		{
			return !string.IsNullOrWhiteSpace(this._alias);
		}
	}

	// Token: 0x06005820 RID: 22560 RVA: 0x001CAFEC File Offset: 0x001C91EC
	public GTScene(string name, string path, string guid, int buildIndex, bool includeInBuild)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentNullException("name");
		}
		if (string.IsNullOrWhiteSpace(path))
		{
			throw new ArgumentNullException("path");
		}
		if (string.IsNullOrWhiteSpace(guid))
		{
			throw new ArgumentNullException("guid");
		}
		this._name = name;
		this._path = path;
		this._guid = guid;
		this._buildIndex = buildIndex;
		this._includeInBuild = includeInBuild;
	}

	// Token: 0x06005821 RID: 22561 RVA: 0x001CB05D File Offset: 0x001C925D
	public override int GetHashCode()
	{
		return this._guid.GetHashCode();
	}

	// Token: 0x06005822 RID: 22562 RVA: 0x001CB06A File Offset: 0x001C926A
	public override string ToString()
	{
		return this.ToJson(false);
	}

	// Token: 0x06005823 RID: 22563 RVA: 0x001CB073 File Offset: 0x001C9273
	public bool Equals(GTScene other)
	{
		return this._guid.Equals(other._guid) && this._name == other._name && this._path == other._path;
	}

	// Token: 0x06005824 RID: 22564 RVA: 0x001CB0B0 File Offset: 0x001C92B0
	public override bool Equals(object obj)
	{
		GTScene gtscene = obj as GTScene;
		return gtscene != null && this.Equals(gtscene);
	}

	// Token: 0x06005825 RID: 22565 RVA: 0x001CB0D0 File Offset: 0x001C92D0
	public static bool operator ==(GTScene x, GTScene y)
	{
		return x.Equals(y);
	}

	// Token: 0x06005826 RID: 22566 RVA: 0x001CB0D9 File Offset: 0x001C92D9
	public static bool operator !=(GTScene x, GTScene y)
	{
		return !x.Equals(y);
	}

	// Token: 0x06005827 RID: 22567 RVA: 0x001CB0E5 File Offset: 0x001C92E5
	public void LoadAsync()
	{
		if (this.isLoaded)
		{
			return;
		}
		SceneManager.LoadSceneAsync(this._buildIndex, LoadSceneMode.Additive);
	}

	// Token: 0x06005828 RID: 22568 RVA: 0x001CB0FD File Offset: 0x001C92FD
	public void UnloadAsync()
	{
		if (!this.isLoaded)
		{
			return;
		}
		SceneManager.UnloadSceneAsync(this._buildIndex, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
	}

	// Token: 0x06005829 RID: 22569 RVA: 0x00036275 File Offset: 0x00034475
	public static GTScene FromAsset(object sceneAsset)
	{
		return null;
	}

	// Token: 0x0600582A RID: 22570 RVA: 0x00036275 File Offset: 0x00034475
	public static GTScene From(object editorBuildSettingsScene)
	{
		return null;
	}

	// Token: 0x04006877 RID: 26743
	[SerializeField]
	private string _alias;

	// Token: 0x04006878 RID: 26744
	[SerializeField]
	private string _name;

	// Token: 0x04006879 RID: 26745
	[SerializeField]
	private string _path;

	// Token: 0x0400687A RID: 26746
	[SerializeField]
	private string _guid;

	// Token: 0x0400687B RID: 26747
	[SerializeField]
	private int _buildIndex;

	// Token: 0x0400687C RID: 26748
	[SerializeField]
	private bool _includeInBuild;
}
