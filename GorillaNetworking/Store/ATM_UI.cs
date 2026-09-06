using System;
using System.Threading.Tasks;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GorillaNetworking.Store
{
	// Token: 0x02001136 RID: 4406
	public class ATM_UI : MonoBehaviour
	{
		// Token: 0x17000A8D RID: 2701
		// (get) Token: 0x06006E84 RID: 28292 RVA: 0x00239C3D File Offset: 0x00237E3D
		public string PurchaseLocation
		{
			get
			{
				return this.purchaseLocation;
			}
		}

		// Token: 0x06006E85 RID: 28293 RVA: 0x00239C48 File Offset: 0x00237E48
		private void Start()
		{
			if (ATM_Manager.instance == null || ATM_Manager.instance.atmUIs.Contains(this))
			{
				return;
			}
			if (!this.memberCodeTitleDataKey.IsNullOrEmpty())
			{
				this.loadMemberCodeFromTitleDate(this.memberCodeTitleDataKey);
				return;
			}
			if (!this.memberCode.IsNullOrEmpty() && this.groupId != null)
			{
				ATM_Manager.instance.AddATM(this, new Tuple<string, string>(this.memberCode, this.groupId.Code));
				return;
			}
			ATM_Manager.instance.AddATM(this, null);
		}

		// Token: 0x06006E86 RID: 28294 RVA: 0x00239CE0 File Offset: 0x00237EE0
		private async void loadMemberCodeFromTitleDate(string memberCodeTitleDataKey)
		{
			while (PlayFabTitleDataCache.Instance == null)
			{
				await Task.Yield();
			}
			PlayFabTitleDataCache.Instance.GetTitleData(memberCodeTitleDataKey, new Action<string>(this.onTD), new Action<PlayFabError>(this.onTDError), false);
		}

		// Token: 0x06006E87 RID: 28295 RVA: 0x00239D20 File Offset: 0x00237F20
		private void onTD(string result)
		{
			if (result.Contains("$"))
			{
				string[] array = result.Split('$', StringSplitOptions.None);
				ATM_Manager.instance.AddATM(this, new Tuple<string, string>(array[0], array[1]));
				return;
			}
			if (this.groupId != null)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"ATM_UI(",
					AssetUtils.GetGameObjectPath(base.gameObject),
					") :: Title Data missing group code. Using \"",
					result,
					"$",
					this.groupId.Code,
					"\". Expected format: \"<MemberCode>$<GroupCode>\" Got: \"",
					result,
					"\""
				}));
				ATM_Manager.instance.AddATM(this, new Tuple<string, string>(result, this.groupId.Code));
				return;
			}
			Debug.LogError(string.Concat(new string[]
			{
				"ATM_UI(",
				AssetUtils.GetGameObjectPath(base.gameObject),
				") :: Title Data missing group code. No code is set. Expected format: \"<MemberCode>$<GroupCode>\" Got: \"",
				result,
				"\""
			}));
			ATM_Manager.instance.AddATM(this, null);
		}

		// Token: 0x06006E88 RID: 28296 RVA: 0x00239E2F File Offset: 0x0023802F
		private void onTDError(PlayFabError error)
		{
			Debug.LogError(string.Format("ATM_UI({0}) :: PlayFabError :: {1}", AssetUtils.GetGameObjectPath(base.gameObject), error));
			ATM_Manager.instance.AddATM(this, null);
		}

		// Token: 0x06006E89 RID: 28297 RVA: 0x00239E5A File Offset: 0x0023805A
		public void SetCustomMapScene(Scene scene)
		{
			this.customMapScene = scene;
		}

		// Token: 0x06006E8A RID: 28298 RVA: 0x00239E63 File Offset: 0x00238063
		public bool IsFromCustomMapScene(Scene scene)
		{
			return this.customMapScene == scene;
		}

		// Token: 0x06006E8B RID: 28299 RVA: 0x00239E71 File Offset: 0x00238071
		internal void SetCreatorCodeTitle(string result)
		{
			if (this.creatorCodeTitle != null)
			{
				this.creatorCodeTitle.text = result;
			}
		}

		// Token: 0x06006E8C RID: 28300 RVA: 0x00239E8D File Offset: 0x0023808D
		internal void SetCreatorCodeField(string v)
		{
			if (this.creatorCodeField != null)
			{
				this.creatorCodeField.text = v;
			}
		}

		// Token: 0x06006E8D RID: 28301 RVA: 0x00239EA9 File Offset: 0x002380A9
		internal void HideCreatorCode()
		{
			if (this.creatorCodeObject != null)
			{
				this.creatorCodeObject.SetActive(false);
			}
		}

		// Token: 0x06006E8E RID: 28302 RVA: 0x00239EC5 File Offset: 0x002380C5
		internal void ShowCreatorCode()
		{
			if (this.creatorCodeObject != null)
			{
				this.creatorCodeObject.SetActive(true);
			}
		}

		// Token: 0x04007EA9 RID: 32425
		public TMP_Text atmText;

		// Token: 0x04007EAA RID: 32426
		public TMP_Text[] ATM_RightColumnButtonText;

		// Token: 0x04007EAB RID: 32427
		public TMP_Text[] ATM_RightColumnArrowText;

		// Token: 0x04007EAC RID: 32428
		[SerializeField]
		private string purchaseLocation;

		// Token: 0x04007EAD RID: 32429
		[SerializeField]
		private GameObject creatorCodeObject;

		// Token: 0x04007EAE RID: 32430
		[SerializeField]
		private TMP_Text creatorCodeTitle;

		// Token: 0x04007EAF RID: 32431
		[SerializeField]
		private TMP_Text creatorCodeField;

		// Token: 0x04007EB0 RID: 32432
		[SerializeField]
		private string memberCode;

		// Token: 0x04007EB1 RID: 32433
		[SerializeField]
		private NexusGroupId groupId;

		// Token: 0x04007EB2 RID: 32434
		[SerializeField]
		private string memberCodeTitleDataKey;

		// Token: 0x04007EB3 RID: 32435
		private Scene customMapScene;
	}
}
