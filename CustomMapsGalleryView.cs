using System;
using System.Collections.Generic;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using Modio.Mods;
using PlayFab;
using UnityEngine;

// Token: 0x02000AB3 RID: 2739
public class CustomMapsGalleryView : MonoBehaviour
{
	// Token: 0x0600463C RID: 17980 RVA: 0x0017A85C File Offset: 0x00178A5C
	public void ResetGallery()
	{
		for (int i = 0; i < this.modTiles.Count; i++)
		{
			this.modTiles[i].DeactivateTile();
		}
	}

	// Token: 0x0600463D RID: 17981 RVA: 0x0017A890 File Offset: 0x00178A90
	public bool DisplayGallery(List<Mod> mods, bool useMapName, out string error)
	{
		if (mods.Count > this.modTiles.Count)
		{
			GTDev.LogError<string>("Displayed Mod list is longer than the number of mod tiles in the gallery", null);
			error = "Displayed Mod list is longer than the number of mod tiles in the gallery";
			return false;
		}
		Dictionary<Mod, Action<string>> dictionary = new Dictionary<Mod, Action<string>>();
		for (int i = 0; i < mods.Count; i++)
		{
			this.modTiles[i].SetMod(mods[i], useMapName);
			int idx = i;
			dictionary[mods[idx]] = delegate(string count)
			{
				this.modTiles[idx].PlayerCountText = count;
			};
		}
		this._synchronizer.SendRequest(dictionary, null);
		error = string.Empty;
		return true;
	}

	// Token: 0x0600463E RID: 17982 RVA: 0x0017A93C File Offset: 0x00178B3C
	public void ShowTileText(bool show, bool useMapName)
	{
		for (int i = 0; i < this.modTiles.Count; i++)
		{
			this.modTiles[i].ShowTileText(show, useMapName);
		}
	}

	// Token: 0x0600463F RID: 17983 RVA: 0x0017A972 File Offset: 0x00178B72
	public void ShowDetailsForEntry(int entryIndex)
	{
		if (this.modTiles.Count > entryIndex)
		{
			this.modTiles[entryIndex].ShowDetails();
		}
	}

	// Token: 0x06004640 RID: 17984 RVA: 0x0017A993 File Offset: 0x00178B93
	public void HighlightTileAtIndex(int tileIndex)
	{
		if (tileIndex > this.modTiles.Count)
		{
			return;
		}
		this.modTiles[tileIndex].HighlightTile();
	}

	// Token: 0x04005899 RID: 22681
	[SerializeField]
	private List<CustomMapsModTile> modTiles = new List<CustomMapsModTile>();

	// Token: 0x0400589A RID: 22682
	private readonly CustomMapsGalleryView.RequestSynchronizer _synchronizer = new CustomMapsGalleryView.RequestSynchronizer();

	// Token: 0x02000AB4 RID: 2740
	private class RequestSynchronizer
	{
		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x06004642 RID: 17986 RVA: 0x0017A9D3 File Offset: 0x00178BD3
		// (set) Token: 0x06004643 RID: 17987 RVA: 0x0017A9DB File Offset: 0x00178BDB
		public int LatestRequest { get; private set; } = -1;

		// Token: 0x06004644 RID: 17988 RVA: 0x0017A9E4 File Offset: 0x00178BE4
		public void SendRequest(IDictionary<Mod, Action<string>> modsAndCallbacks, Action<PlayFabError> errorCallback = null)
		{
			int num = this.LatestRequest + 1;
			this.LatestRequest = num;
			new CustomMapsGalleryView.SynchronizedRequest(this, this.LatestRequest, modsAndCallbacks, errorCallback).Send();
		}
	}

	// Token: 0x02000AB5 RID: 2741
	private class SynchronizedRequest
	{
		// Token: 0x06004646 RID: 17990 RVA: 0x0017AA23 File Offset: 0x00178C23
		public SynchronizedRequest(CustomMapsGalleryView.RequestSynchronizer parent, int id, IDictionary<Mod, Action<string>> modsAndCallbacks, Action<PlayFabError> errorCallback)
		{
			this._parent = parent;
			this._id = id;
			this._modsAndCallbacks = modsAndCallbacks;
			this._errorCallback = errorCallback;
		}

		// Token: 0x06004647 RID: 17991 RVA: 0x0017AA48 File Offset: 0x00178C48
		public void Send()
		{
			Dictionary<Mod, Action<string>> dictionary = new Dictionary<Mod, Action<string>>();
			foreach (Mod mod in this._modsAndCallbacks.Keys)
			{
				dictionary[mod] = this.WrapCallback(this._modsAndCallbacks[mod]);
			}
			PlayerCountHelper.GetPlayerCountBatched(dictionary, this._errorCallback);
		}

		// Token: 0x06004648 RID: 17992 RVA: 0x0017AAC0 File Offset: 0x00178CC0
		private Action<string> WrapCallback(Action<string> source)
		{
			return delegate(string result)
			{
				if (this._id != this._parent.LatestRequest)
				{
					return;
				}
				source(result);
			};
		}

		// Token: 0x0400589C RID: 22684
		private readonly CustomMapsGalleryView.RequestSynchronizer _parent;

		// Token: 0x0400589D RID: 22685
		private readonly int _id;

		// Token: 0x0400589E RID: 22686
		private readonly IDictionary<Mod, Action<string>> _modsAndCallbacks;

		// Token: 0x0400589F RID: 22687
		private readonly Action<PlayFabError> _errorCallback;
	}
}
