using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaNetworking;
using Modio.Mods;
using PlayFab;
using PlayFab.CloudScriptModels;
using PlayFab.Json;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps.UI
{
	// Token: 0x02000FE5 RID: 4069
	[NullableContext(1)]
	[Nullable(0)]
	public static class PlayerCountHelper
	{
		// Token: 0x06006541 RID: 25921 RVA: 0x00209598 File Offset: 0x00207798
		public static void GetPlayerCount(Mod mod, Action<string> successCallback, [Nullable(new byte[] { 2, 1 })] Action<PlayFabError> errorCallback = null)
		{
			PlayerCountHelper.GetPlayerCountInternal(mod.Id.ToString(), delegate(ulong count)
			{
				successCallback(PlayerCountHelper.FormatPlayerCount(count));
			}, errorCallback);
		}

		// Token: 0x06006542 RID: 25922 RVA: 0x002095D8 File Offset: 0x002077D8
		public static void GetPlayerCountBatched(IDictionary<Mod, Action<string>> modsAndCallbacks, [Nullable(new byte[] { 2, 1 })] Action<PlayFabError> errorCallback = null)
		{
			GorillaServer instance = GorillaServer.Instance;
			if (instance == null)
			{
				return;
			}
			ReturnVstumpMapStatsRequest returnVstumpMapStatsRequest = new ReturnVstumpMapStatsRequest();
			returnVstumpMapStatsRequest.mapIds = modsAndCallbacks.Keys.Select((Mod mod) => mod.Id.ToString()).ToList<string>();
			ReturnVstumpMapStatsRequest returnVstumpMapStatsRequest2 = returnVstumpMapStatsRequest;
			instance.ReturnVstumpMapStats(returnVstumpMapStatsRequest2, delegate(ExecuteFunctionResult executeFunctionResult)
			{
				PlayerCountHelper.UnpackSuccessBatched(executeFunctionResult, modsAndCallbacks);
			}, errorCallback ?? new Action<PlayFabError>(PlayerCountHelper.DefaultErrorCallback));
		}

		// Token: 0x06006543 RID: 25923 RVA: 0x00209664 File Offset: 0x00207864
		private static void GetPlayerCountInternal(string modId, Action<ulong> successCallback, [Nullable(new byte[] { 2, 1 })] Action<PlayFabError> errorCallback = null)
		{
			GorillaServer instance = GorillaServer.Instance;
			if (instance == null)
			{
				return;
			}
			ReturnVstumpMapStatsRequest returnVstumpMapStatsRequest = new ReturnVstumpMapStatsRequest
			{
				mapIds = new List<string> { modId }
			};
			instance.ReturnVstumpMapStats(returnVstumpMapStatsRequest, delegate(ExecuteFunctionResult executeFunctionResult)
			{
				PlayerCountHelper.UnpackSuccess(executeFunctionResult, modId, successCallback);
			}, errorCallback ?? new Action<PlayFabError>(PlayerCountHelper.DefaultErrorCallback));
		}

		// Token: 0x06006544 RID: 25924 RVA: 0x002096D4 File Offset: 0x002078D4
		private static void UnpackSuccess(ExecuteFunctionResult result, string modId, Action<ulong> callback)
		{
			JsonObject jsonObject = result.FunctionResult as JsonObject;
			if (jsonObject == null)
			{
				return;
			}
			JsonObject jsonObject2;
			if (!jsonObject.TryGetValue("Maps", out jsonObject2))
			{
				return;
			}
			JsonObject jsonObject3;
			if (jsonObject2 == null || !jsonObject2.TryGetValue(modId, out jsonObject3))
			{
				return;
			}
			ulong num;
			if (jsonObject3 == null || !jsonObject3.TryGetValue("PlayerCount", out num))
			{
				return;
			}
			callback(num);
		}

		// Token: 0x06006545 RID: 25925 RVA: 0x0020972C File Offset: 0x0020792C
		private static void UnpackSuccessBatched(ExecuteFunctionResult result, IDictionary<Mod, Action<string>> modsAndCallbacks)
		{
			JsonObject jsonObject = result.FunctionResult as JsonObject;
			if (jsonObject == null)
			{
				return;
			}
			JsonObject jsonObject2;
			if (!jsonObject.TryGetValue("Maps", out jsonObject2))
			{
				return;
			}
			if (jsonObject2 == null)
			{
				return;
			}
			Dictionary<string, ulong> dictionary = new Dictionary<string, ulong>();
			foreach (string text in jsonObject2.Keys)
			{
				JsonObject jsonObject3;
				ulong num;
				if (jsonObject2.TryGetValue(text, out jsonObject3) && jsonObject3 != null && jsonObject3.TryGetValue("PlayerCount", out num))
				{
					dictionary[text] = num;
				}
			}
			foreach (KeyValuePair<Mod, Action<string>> keyValuePair in modsAndCallbacks)
			{
				ulong num2;
				if (dictionary.TryGetValue(keyValuePair.Key.Id.ToString(), out num2))
				{
					string text2 = PlayerCountHelper.FormatPlayerCount(num2);
					keyValuePair.Value(text2);
				}
			}
		}

		// Token: 0x06006546 RID: 25926 RVA: 0x0020983C File Offset: 0x00207A3C
		private static void DefaultErrorCallback(PlayFabError error)
		{
			Debug.Log("Error fetching player count: " + error.ErrorMessage);
		}

		// Token: 0x06006547 RID: 25927 RVA: 0x00209854 File Offset: 0x00207A54
		private static string FormatPlayerCount(ulong count)
		{
			if (count < 1000UL)
			{
				return count.ToString();
			}
			float num = count;
			foreach (char c in new char[] { 'K', 'M', 'B' })
			{
				num /= 1000f;
				if (num < 1000f)
				{
					return num.ToString("###.###") + c.ToString();
				}
			}
			throw new Exception("Tried to format too-large player count.");
		}

		// Token: 0x0400743B RID: 29755
		private const string MapsJsonKey = "Maps";

		// Token: 0x0400743C RID: 29756
		private const string PlayerCountJsonKey = "PlayerCount";
	}
}
