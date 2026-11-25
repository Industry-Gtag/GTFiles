using System;
using GorillaNetworking;
using PlayFab;

namespace GorillaTagScripts.Builder;

public static class SMBFeaturedMaps
{
	public enum FeaturedSlot
	{
		None,
		A,
		B
	}

	private const string FEATURED_MAPS_TITLE_DATA_KEY = "SMBFeaturedMaps";

	public static void RequestFeaturedMapId(FeaturedSlot slot, Action<string> callback)
	{
		if (slot == FeaturedSlot.None)
		{
			callback?.Invoke(null);
			return;
		}
		PlayFabTitleDataCache.RegisterOnLoad(delegate(PlayFabTitleDataCache cache)
		{
			cache.GetTitleData("SMBFeaturedMaps", delegate(string data)
			{
				callback?.Invoke(ParseMapId(data, slot));
			}, delegate(PlayFabError error)
			{
				GTDev.LogError("[SMBFeaturedMaps] Failed to get SMBFeaturedMaps: " + error.ErrorMessage);
				callback?.Invoke(null);
			});
		});
	}

	private static string ParseMapId(string data, FeaturedSlot slot)
	{
		if (data.IsNullOrEmpty())
		{
			return null;
		}
		string[] array = data.Trim().Trim('"').Split(',');
		int num = (int)(slot - 1);
		if (num >= array.Length)
		{
			return null;
		}
		string text = array[num].Replace("-", "").Trim().ToUpperInvariant();
		if (text.IsNullOrEmpty())
		{
			return null;
		}
		if (!SharedBlocksManager.IsMapIDValid(text))
		{
			GTDev.LogError($"[SMBFeaturedMaps] Invalid featured map code \"{array[num]}\" for slot {slot}");
			return null;
		}
		return text;
	}
}
