using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x0200050F RID: 1295
public static class CreatorCodes
{
	// Token: 0x0600205A RID: 8282 RVA: 0x000ADFE8 File Offset: 0x000AC1E8
	public static string getCurrentCreatorCode(string id)
	{
		if (id.IsNullOrEmpty())
		{
			return string.Empty;
		}
		if (CreatorCodes.data.currentCreatorCode == null)
		{
			return string.Empty;
		}
		if (!CreatorCodes.data.currentCreatorCode.ContainsKey(id))
		{
			return string.Empty;
		}
		return CreatorCodes.data.currentCreatorCode[id];
	}

	// Token: 0x0600205B RID: 8283 RVA: 0x000AE03D File Offset: 0x000AC23D
	public static CreatorCodes.CreatorCodeStatus getCurrentCreatorCodeStatus(string id)
	{
		if (id == null)
		{
			return CreatorCodes.CreatorCodeStatus.Empty;
		}
		if (CreatorCodes.creatorCodeStatus == null)
		{
			return CreatorCodes.CreatorCodeStatus.Empty;
		}
		if (!CreatorCodes.creatorCodeStatus.ContainsKey(id))
		{
			return CreatorCodes.CreatorCodeStatus.Empty;
		}
		return CreatorCodes.creatorCodeStatus[id];
	}

	// Token: 0x1400004A RID: 74
	// (add) Token: 0x0600205C RID: 8284 RVA: 0x000AE068 File Offset: 0x000AC268
	// (remove) Token: 0x0600205D RID: 8285 RVA: 0x000AE09C File Offset: 0x000AC29C
	public static event Action<string> OnCreatorCodeChangedEvent;

	// Token: 0x1400004B RID: 75
	// (add) Token: 0x0600205E RID: 8286 RVA: 0x000AE0D0 File Offset: 0x000AC2D0
	// (remove) Token: 0x0600205F RID: 8287 RVA: 0x000AE104 File Offset: 0x000AC304
	public static event Action InitializedEvent;

	// Token: 0x1400004C RID: 76
	// (add) Token: 0x06002060 RID: 8288 RVA: 0x000AE138 File Offset: 0x000AC338
	// (remove) Token: 0x06002061 RID: 8289 RVA: 0x000AE16C File Offset: 0x000AC36C
	public static event Action<string, string, NexusGroupId> OnCreatorCodeValidEvent;

	// Token: 0x1400004D RID: 77
	// (add) Token: 0x06002062 RID: 8290 RVA: 0x000AE1A0 File Offset: 0x000AC3A0
	// (remove) Token: 0x06002063 RID: 8291 RVA: 0x000AE1D4 File Offset: 0x000AC3D4
	public static event Action<string> OnCreatorCodeFailureEvent;

	// Token: 0x06002064 RID: 8292 RVA: 0x000AE207 File Offset: 0x000AC407
	public static void Initialize()
	{
		CreatorCodes.ValidatedCreatorCode = new Dictionary<string, NexusManager.MemberCode>();
		CreatorCodes.creatorCodeStatus = new Dictionary<string, CreatorCodes.CreatorCodeStatus>();
		CreatorCodes.LoadData();
		CreatorCodes.Intialized = true;
		Action initializedEvent = CreatorCodes.InitializedEvent;
		if (initializedEvent == null)
		{
			return;
		}
		initializedEvent();
	}

	// Token: 0x06002065 RID: 8293 RVA: 0x000AE238 File Offset: 0x000AC438
	public static void DeleteCharacter(string id)
	{
		if (CreatorCodes.data.currentCreatorCode.ContainsKey(id) && CreatorCodes.data.currentCreatorCode[id].Length > 0)
		{
			CreatorCodes.data.currentCreatorCode[id] = CreatorCodes.data.currentCreatorCode[id].Substring(0, CreatorCodes.data.currentCreatorCode[id].Length - 1);
			CreatorCodes.ValidatedCreatorCode[id] = null;
			CreatorCodes.creatorCodeStatus[id] = ((CreatorCodes.data.currentCreatorCode[id].Length == 0) ? CreatorCodes.CreatorCodeStatus.Empty : CreatorCodes.CreatorCodeStatus.Unchecked);
			Action<string> onCreatorCodeChangedEvent = CreatorCodes.OnCreatorCodeChangedEvent;
			if (onCreatorCodeChangedEvent == null)
			{
				return;
			}
			onCreatorCodeChangedEvent(id);
		}
	}

	// Token: 0x06002066 RID: 8294 RVA: 0x000AE2F4 File Offset: 0x000AC4F4
	public static void AppendKey(string id, string input)
	{
		if (!CreatorCodes.data.currentCreatorCode.ContainsKey(id))
		{
			CreatorCodes.data.currentCreatorCode[id] = string.Empty;
		}
		if (CreatorCodes.data.currentCreatorCode[id].Length < 10)
		{
			Dictionary<string, string> currentCreatorCode = CreatorCodes.data.currentCreatorCode;
			currentCreatorCode[id] += input;
			CreatorCodes.ValidatedCreatorCode[id] = null;
			CreatorCodes.creatorCodeStatus[id] = CreatorCodes.CreatorCodeStatus.Unchecked;
			Action<string> onCreatorCodeChangedEvent = CreatorCodes.OnCreatorCodeChangedEvent;
			if (onCreatorCodeChangedEvent == null)
			{
				return;
			}
			onCreatorCodeChangedEvent(id);
		}
	}

	// Token: 0x06002067 RID: 8295 RVA: 0x000AE38C File Offset: 0x000AC58C
	public static void ResetCreatorCode(string id)
	{
		Debug.Log("Resetting creator code");
		CreatorCodes.data.currentCreatorCode[id] = "";
		CreatorCodes.creatorCodeStatus[id] = CreatorCodes.CreatorCodeStatus.Empty;
		CreatorCodes.supportedMember = default(Member);
		CreatorCodes.ValidatedCreatorCode[id] = null;
		CreatorCodes.SaveData();
		Action<string> onCreatorCodeChangedEvent = CreatorCodes.OnCreatorCodeChangedEvent;
		if (onCreatorCodeChangedEvent == null)
		{
			return;
		}
		onCreatorCodeChangedEvent(id);
	}

	// Token: 0x06002068 RID: 8296 RVA: 0x000AE3F0 File Offset: 0x000AC5F0
	public static async Task<NexusManager.MemberCode> CheckValidationCoroutineJIT(string terminalId, string code, NexusGroupId[] group)
	{
		CreatorCodes.creatorCodeStatus[terminalId] = CreatorCodes.CreatorCodeStatus.Validating;
		Action<string> onCreatorCodeChangedEvent = CreatorCodes.OnCreatorCodeChangedEvent;
		if (onCreatorCodeChangedEvent != null)
		{
			onCreatorCodeChangedEvent(terminalId);
		}
		for (int i = 0; i < group.Length; i++)
		{
			Member member = await NexusManager.instance.VerifyCreatorCode(terminalId, code, group[i]);
			if (!member.Equals(default(Member)))
			{
				CreatorCodes.creatorCodeStatus[terminalId] = CreatorCodes.CreatorCodeStatus.Valid;
				CreatorCodes.supportedMember = member;
				CreatorCodes.ValidatedCreatorCode[terminalId] = new NexusManager.MemberCode
				{
					memberCode = code,
					groupId = group[i]
				};
				CreatorCodes.data.codeFirstUsedTime[terminalId] = DateTime.UtcNow;
				CreatorCodes.SaveData();
				Action<string, string, NexusGroupId> onCreatorCodeValidEvent = CreatorCodes.OnCreatorCodeValidEvent;
				if (onCreatorCodeValidEvent != null)
				{
					onCreatorCodeValidEvent(terminalId, code, group[i]);
				}
				return new NexusManager.MemberCode
				{
					memberCode = code,
					groupId = group[i]
				};
			}
		}
		CreatorCodes.creatorCodeStatus[terminalId] = CreatorCodes.CreatorCodeStatus.Unchecked;
		Action<string> onCreatorCodeFailureEvent = CreatorCodes.OnCreatorCodeFailureEvent;
		if (onCreatorCodeFailureEvent != null)
		{
			onCreatorCodeFailureEvent(terminalId);
		}
		return null;
	}

	// Token: 0x06002069 RID: 8297 RVA: 0x000AE443 File Offset: 0x000AC643
	private static void SaveData()
	{
		PlayerPrefs.SetString("CreatorCodes_Store", JsonConvert.SerializeObject(CreatorCodes.data));
	}

	// Token: 0x0600206A RID: 8298 RVA: 0x000AE45C File Offset: 0x000AC65C
	private static void LoadData()
	{
		string @string = PlayerPrefs.GetString("CreatorCodes_Store", string.Empty);
		if (@string.Length == 0)
		{
			return;
		}
		CreatorCodes.data = JsonConvert.DeserializeObject<CreatorCodes.CreatorCodesData>(@string);
		foreach (string text in CreatorCodes.data.currentCreatorCode.Keys)
		{
			if (CreatorCodes.data.codeFirstUsedTime.ContainsKey(text) && DateTime.UtcNow.Subtract(CreatorCodes.data.codeFirstUsedTime[text]).Days > 14)
			{
				CreatorCodes.data.currentCreatorCode[text] = string.Empty;
			}
		}
	}

	// Token: 0x04002B34 RID: 11060
	private const int MAX_CODE_LENGTH = 10;

	// Token: 0x04002B35 RID: 11061
	private const string PLAYER_PREF_KEY = "CreatorCodes_Store";

	// Token: 0x04002B36 RID: 11062
	private const int DAYS_TO_STORE_CODE = 14;

	// Token: 0x04002B37 RID: 11063
	private static CreatorCodes.CreatorCodesData data = new CreatorCodes.CreatorCodesData();

	// Token: 0x04002B38 RID: 11064
	private static Dictionary<string, NexusManager.MemberCode> ValidatedCreatorCode;

	// Token: 0x04002B39 RID: 11065
	private static Dictionary<string, CreatorCodes.CreatorCodeStatus> creatorCodeStatus;

	// Token: 0x04002B3E RID: 11070
	public static bool Intialized = false;

	// Token: 0x04002B3F RID: 11071
	public static Member supportedMember;

	// Token: 0x02000510 RID: 1296
	public enum CreatorCodeStatus
	{
		// Token: 0x04002B41 RID: 11073
		Empty,
		// Token: 0x04002B42 RID: 11074
		Unchecked,
		// Token: 0x04002B43 RID: 11075
		Validating,
		// Token: 0x04002B44 RID: 11076
		Valid
	}

	// Token: 0x02000511 RID: 1297
	[Serializable]
	private class CreatorCodesData
	{
		// Token: 0x04002B45 RID: 11077
		public Dictionary<string, string> currentCreatorCode = new Dictionary<string, string>();

		// Token: 0x04002B46 RID: 11078
		public Dictionary<string, DateTime> codeFirstUsedTime = new Dictionary<string, DateTime>();
	}
}
