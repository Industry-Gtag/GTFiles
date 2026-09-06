using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02001105 RID: 4357
	public class TitleDataFeatureFlags
	{
		// Token: 0x17000A5D RID: 2653
		// (get) Token: 0x06006D2B RID: 27947 RVA: 0x00234380 File Offset: 0x00232580
		// (set) Token: 0x06006D2C RID: 27948 RVA: 0x00234388 File Offset: 0x00232588
		public bool ready { get; private set; }

		// Token: 0x06006D2D RID: 27949 RVA: 0x00234391 File Offset: 0x00232591
		public void FetchFeatureFlags()
		{
			PlayFabTitleDataCache.Instance.GetTitleData(this.TitleDataKey, delegate(string json)
			{
				try
				{
					foreach (FeatureFlagData featureFlagData in JsonUtility.FromJson<FeatureFlagListData>(json).flags)
					{
						if (featureFlagData.valueType == "percent")
						{
							this.flagValueByName.AddOrUpdate(featureFlagData.name, featureFlagData.value);
						}
						List<string> alwaysOnForUsers = featureFlagData.alwaysOnForUsers;
						if (alwaysOnForUsers != null && alwaysOnForUsers.Count > 0)
						{
							this.flagValueByUser.AddOrUpdate(featureFlagData.name, featureFlagData.alwaysOnForUsers);
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Format("Error parsing rollout feature flags: {0}", ex));
				}
				finally
				{
					this.ready = true;
				}
			}, delegate(PlayFabError e)
			{
				Debug.LogError("Error fetching rollout feature flags: " + e.ErrorMessage);
				this.ready = true;
			}, false);
		}

		// Token: 0x06006D2E RID: 27950 RVA: 0x002343BC File Offset: 0x002325BC
		public bool IsEnabled(string flagName)
		{
			return this.IsEnabledForUser(flagName, PlayFabAuthenticator.instance.GetPlayFabPlayerId());
		}

		// Token: 0x06006D2F RID: 27951 RVA: 0x002343D4 File Offset: 0x002325D4
		public bool IsEnabledForUser(string flagName, string playFabId)
		{
			bool flag = !this.logSent.Add(new ValueTuple<string, string>(flagName, playFabId));
			List<string> list;
			if (this.flagValueByUser.TryGetValue(flagName, out list) && list.Contains(playFabId))
			{
				return true;
			}
			int num;
			if (!this.flagValueByName.TryGetValue(flagName, out num))
			{
				bool flag2;
				return this.defaults.TryGetValue(flagName, out flag2) && flag2;
			}
			return num > 0 && (num >= 100 || (ulong)(XXHash32.Compute(Encoding.UTF8.GetBytes(playFabId), 0U) % 100U) < (ulong)((long)num));
		}

		// Token: 0x06006D30 RID: 27952 RVA: 0x0023446C File Offset: 0x0023266C
		public bool IsEnabledForAnyone(string flagName)
		{
			List<string> list;
			if (this.flagValueByUser.TryGetValue(flagName, out list) && list.Count > 0)
			{
				return true;
			}
			int num;
			if (!this.flagValueByName.TryGetValue(flagName, out num))
			{
				bool flag;
				return this.defaults.TryGetValue(flagName, out flag) && flag;
			}
			return num > 0;
		}

		// Token: 0x04007D6A RID: 32106
		public string TitleDataKey = "DeployFeatureFlags";

		// Token: 0x04007D6C RID: 32108
		public Dictionary<string, bool> defaults = new Dictionary<string, bool>
		{
			{ "2026-04-VStumpGrabbablesFix", true },
			{ "2026-04-SuppressZonesInVStump", true }
		};

		// Token: 0x04007D6D RID: 32109
		private Dictionary<string, int> flagValueByName = new Dictionary<string, int>();

		// Token: 0x04007D6E RID: 32110
		private Dictionary<string, List<string>> flagValueByUser = new Dictionary<string, List<string>>();

		// Token: 0x04007D6F RID: 32111
		[TupleElementNames(new string[] { "flagName", "playFabId" })]
		private readonly HashSet<ValueTuple<string, string>> logSent = new HashSet<ValueTuple<string, string>>();
	}
}
