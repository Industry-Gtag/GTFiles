using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NexusSDK;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200055D RID: 1373
public class NexusManager : MonoBehaviour
{
	// Token: 0x170003B1 RID: 945
	// (get) Token: 0x06002301 RID: 8961 RVA: 0x000BC5F3 File Offset: 0x000BA7F3
	public NexusManager.Environment CurrentEnvironment
	{
		get
		{
			return this.environment;
		}
	}

	// Token: 0x06002302 RID: 8962 RVA: 0x000BC5FB File Offset: 0x000BA7FB
	private void Awake()
	{
		if (NexusManager.instance == null)
		{
			this.environment = NexusManager.Environment.PRODUCTION;
			NexusManager.instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06002303 RID: 8963 RVA: 0x000BC61E File Offset: 0x000BA81E
	private void Start()
	{
		SDKInitializer.Init((this.environment == NexusManager.Environment.SANDBOX) ? "nexus_pk_ba155a8c229740489d214f024e25f25c" : "nexus_pk_4c18dcb1531846c7abad4cb00c5242bb", (this.environment == NexusManager.Environment.SANDBOX) ? "sandbox" : "production");
	}

	// Token: 0x06002304 RID: 8964 RVA: 0x000BC650 File Offset: 0x000BA850
	public async Task<Member> VerifyCreatorCode(string terminalId, string code, NexusGroupId id)
	{
		string text = SDKInitializer.ApiBaseUrl + "/manage/members/{memberCode}";
		text = text.Replace("{memberCode}", code);
		List<string> list = new List<string>();
		list.Add("groupId=" + id.Code);
		text += "?";
		text += string.Join("&", list);
		Debug.Log("CreatorCodeTerminal " + terminalId + " :: GetMemberByCode :: " + text);
		Member member;
		using (UnityWebRequest webRequest = UnityWebRequest.Get(text))
		{
			webRequest.SetRequestHeader("x-shared-secret", SDKInitializer.ApiKey);
			await webRequest.SendWebRequest();
			if (webRequest.responseCode == 200L)
			{
				Debug.Log("CreatorCodeTerminal " + terminalId + " :: GetMemberByCode :: valid");
				member = JsonConvert.DeserializeObject<Member>(webRequest.downloadHandler.text, new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				});
			}
			else
			{
				Debug.Log("CreatorCodeTerminal " + terminalId + " :: GetMemberByCode :: invalid");
				member = default(Member);
			}
		}
		return member;
	}

	// Token: 0x06002305 RID: 8965 RVA: 0x000BC6A4 File Offset: 0x000BA8A4
	public async Task<bool> VerifyCreatorCodeJIT(string memberCode, string groupCode)
	{
		string text = SDKInitializer.ApiBaseUrl + "/manage/members/{memberCode}";
		text = text.Replace("{memberCode}", memberCode);
		List<string> list = new List<string>();
		list.Add("groupId=" + groupCode);
		text += "?";
		text += string.Join("&", list);
		bool flag;
		using (UnityWebRequest webRequest = UnityWebRequest.Get(text))
		{
			webRequest.SetRequestHeader("x-shared-secret", SDKInitializer.ApiKey);
			await webRequest.SendWebRequest();
			if (webRequest.responseCode == 200L)
			{
				flag = true;
			}
			else
			{
				flag = false;
			}
		}
		return flag;
	}

	// Token: 0x04002E1D RID: 11805
	private const string ENV_PRODUCTION = "production";

	// Token: 0x04002E1E RID: 11806
	private const string ENV_SANDBOX = "sandbox";

	// Token: 0x04002E1F RID: 11807
	private const string ENV_PRODUCTION_PUBLIC_API_KEY = "nexus_pk_4c18dcb1531846c7abad4cb00c5242bb";

	// Token: 0x04002E20 RID: 11808
	private const string ENV_SANDBOX_PUBLIC_API_KEY = "nexus_pk_ba155a8c229740489d214f024e25f25c";

	// Token: 0x04002E21 RID: 11809
	private NexusManager.Environment environment = NexusManager.Environment.SANDBOX;

	// Token: 0x04002E22 RID: 11810
	public static NexusManager instance;

	// Token: 0x04002E23 RID: 11811
	private Member[] validatedMembers;

	// Token: 0x0200055E RID: 1374
	public enum Environment
	{
		// Token: 0x04002E25 RID: 11813
		PRODUCTION,
		// Token: 0x04002E26 RID: 11814
		SANDBOX
	}

	// Token: 0x0200055F RID: 1375
	[Serializable]
	public class MemberCode
	{
		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x06002307 RID: 8967 RVA: 0x000BC6FE File Offset: 0x000BA8FE
		// (set) Token: 0x06002308 RID: 8968 RVA: 0x000BC706 File Offset: 0x000BA906
		public string memberCode { get; set; }

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x06002309 RID: 8969 RVA: 0x000BC70F File Offset: 0x000BA90F
		// (set) Token: 0x0600230A RID: 8970 RVA: 0x000BC717 File Offset: 0x000BA917
		public NexusGroupId groupId { get; set; }
	}

	// Token: 0x02000560 RID: 1376
	[Serializable]
	public struct GetMembersRequest
	{
		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x0600230C RID: 8972 RVA: 0x000BC720 File Offset: 0x000BA920
		// (set) Token: 0x0600230D RID: 8973 RVA: 0x000BC728 File Offset: 0x000BA928
		public int page { readonly get; set; }

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x0600230E RID: 8974 RVA: 0x000BC731 File Offset: 0x000BA931
		// (set) Token: 0x0600230F RID: 8975 RVA: 0x000BC739 File Offset: 0x000BA939
		public int pageSize { readonly get; set; }
	}
}
