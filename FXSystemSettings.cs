using System;
using UnityEngine;

// Token: 0x02000D2A RID: 3370
[CreateAssetMenu(menuName = "ScriptableObjects/FXSystemSettings", order = 2)]
public class FXSystemSettings : ScriptableObject
{
	// Token: 0x0600536C RID: 21356 RVA: 0x001B7964 File Offset: 0x001B5B64
	public void Awake()
	{
		int num = ((this.callLimits != null) ? this.callLimits.Length : 0);
		int num2 = ((this.CallLimitsCooldown != null) ? this.CallLimitsCooldown.Length : 0);
		for (int i = 0; i < num; i++)
		{
			FXType fxtype = this.callLimits[i].Key;
			int num3 = (int)fxtype;
			if (num3 < 0 || num3 >= 26)
			{
				string text = "NO_PATH_AT_RUNTIME";
				Debug.LogError("FXSystemSettings: (this should never happen) `callLimits.Key` is out of bounds of `callSettings`! Path=\"" + text + "\"", this);
			}
			if (this.callSettings[num3] != null)
			{
				Debug.Log("FXSystemSettings: call setting for " + fxtype.ToString() + " already exists, skipping.");
			}
			else
			{
				this.callSettings[num3] = this.callLimits[i];
			}
		}
		for (int i = 0; i < num2; i++)
		{
			FXType fxtype = this.CallLimitsCooldown[i].Key;
			int num3 = (int)fxtype;
			if (this.callSettings[num3] != null)
			{
				Debug.Log("FXSystemSettings: call setting for " + fxtype.ToString() + " already exists, skipping");
			}
			else
			{
				this.callSettings[num3] = this.CallLimitsCooldown[i];
			}
		}
		for (int i = 0; i < this.callSettings.Length; i++)
		{
			if (this.callSettings[i] == null)
			{
				this.callSettings[i] = new LimiterType
				{
					CallLimitSettings = new CallLimiter(0, 0f, 0f),
					Key = (FXType)i
				};
			}
		}
	}

	// Token: 0x04006506 RID: 25862
	private const string preLog = "FXSystemSettings: ";

	// Token: 0x04006507 RID: 25863
	private const string preErr = "ERROR!!!  FXSystemSettings: ";

	// Token: 0x04006508 RID: 25864
	[SerializeField]
	private LimiterType[] callLimits;

	// Token: 0x04006509 RID: 25865
	[SerializeField]
	private CooldownType[] CallLimitsCooldown;

	// Token: 0x0400650A RID: 25866
	[NonSerialized]
	public bool forLocalRig;

	// Token: 0x0400650B RID: 25867
	[NonSerialized]
	public CallLimitType<CallLimiter>[] callSettings = new CallLimitType<CallLimiter>[26];
}
