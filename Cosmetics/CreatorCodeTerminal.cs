using System;
using System.Threading.Tasks;
using GorillaNetworking;
using TMPro;
using UnityEngine;

namespace Cosmetics
{
	// Token: 0x020011DA RID: 4570
	public class CreatorCodeTerminal : MonoBehaviour, ICreatorCodeProvider, IBuildValidation
	{
		// Token: 0x17000B3D RID: 2877
		// (get) Token: 0x06007428 RID: 29736 RVA: 0x0025CBAA File Offset: 0x0025ADAA
		public NexusGroupId[] NexusGroups
		{
			get
			{
				return this.nexusGroups;
			}
		}

		// Token: 0x17000B3E RID: 2878
		// (get) Token: 0x06007429 RID: 29737 RVA: 0x0025CBB2 File Offset: 0x0025ADB2
		public string TerminalId
		{
			get
			{
				return this.termId;
			}
		}

		// Token: 0x17000B3F RID: 2879
		// (get) Token: 0x0600742A RID: 29738 RVA: 0x000066D3 File Offset: 0x000048D3
		GameObject ICreatorCodeProvider.GameObject
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x0600742B RID: 29739 RVA: 0x0025CBBC File Offset: 0x0025ADBC
		public void Awake()
		{
			this.termId = string.Empty;
			for (int i = 0; i < this.nexusGroups.Length; i++)
			{
				this.termId += this.nexusGroups[i].Code;
			}
			this.HookupToCreatorCodes();
		}

		// Token: 0x0600742C RID: 29740 RVA: 0x0025CC0B File Offset: 0x0025AE0B
		private void OnDestroy()
		{
			this.UnhookFromCreatorCodes();
		}

		// Token: 0x0600742D RID: 29741 RVA: 0x0025CC14 File Offset: 0x0025AE14
		public void HookupToCreatorCodes()
		{
			CreatorCodes.InitializedEvent += this.OnCreatorCodesInitialized;
			CreatorCodes.OnCreatorCodeChangedEvent += this.OnCreatorCodeChanged;
			CreatorCodes.OnCreatorCodeFailureEvent += this.OnCreatorCodeFailure;
			if (CreatorCodes.Intialized)
			{
				this.OnCreatorCodesInitialized();
			}
			CosmeticsController.PushTerminalMessage = (Action<string, string>)Delegate.Combine(CosmeticsController.PushTerminalMessage, new Action<string, string>(this.OnTerminalMessage));
		}

		// Token: 0x0600742E RID: 29742 RVA: 0x0025CC84 File Offset: 0x0025AE84
		private async void OnTerminalMessage(string termId, string msg)
		{
			if (!(termId != this.termId))
			{
				this.creatorCodeTitle.text = msg;
				while (Application.isPlaying && (VRRig.LocalRig.transform.position - base.transform.position).sqrMagnitude < 4f)
				{
					await Task.Yield();
				}
				this.creatorCodeTitle.text = "CREATOR CODE: VALID";
			}
		}

		// Token: 0x0600742F RID: 29743 RVA: 0x0025CCCC File Offset: 0x0025AECC
		public void UnhookFromCreatorCodes()
		{
			CreatorCodes.InitializedEvent -= this.OnCreatorCodesInitialized;
			CreatorCodes.OnCreatorCodeChangedEvent -= this.OnCreatorCodeChanged;
			CreatorCodes.OnCreatorCodeFailureEvent -= this.OnCreatorCodeFailure;
			CosmeticsController.PushTerminalMessage = (Action<string, string>)Delegate.Remove(CosmeticsController.PushTerminalMessage, new Action<string, string>(this.OnTerminalMessage));
		}

		// Token: 0x06007430 RID: 29744 RVA: 0x0025CD2C File Offset: 0x0025AF2C
		private void OnCreatorCodesInitialized()
		{
			this.OnCreatorCodeChanged(this.termId);
		}

		// Token: 0x06007431 RID: 29745 RVA: 0x0025CD3C File Offset: 0x0025AF3C
		public void OnCreatorCodeChanged(string id)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeField.text = CreatorCodes.getCurrentCreatorCode(this.termId);
			string text = "CREATOR CODE:";
			CreatorCodes.CreatorCodeStatus currentCreatorCodeStatus = CreatorCodes.getCurrentCreatorCodeStatus(this.termId);
			if (currentCreatorCodeStatus != CreatorCodes.CreatorCodeStatus.Validating)
			{
				if (currentCreatorCodeStatus == CreatorCodes.CreatorCodeStatus.Valid)
				{
					text += " VALID";
				}
			}
			else
			{
				text += " VALIDATING";
			}
			this.creatorCodeTitle.text = text;
		}

		// Token: 0x06007432 RID: 29746 RVA: 0x0025CDAE File Offset: 0x0025AFAE
		public void CreatorCodeInput(string character)
		{
			CreatorCodes.AppendKey(this.termId, character);
		}

		// Token: 0x06007433 RID: 29747 RVA: 0x0025CDBC File Offset: 0x0025AFBC
		public void CreatorCodeDelete()
		{
			CreatorCodes.DeleteCharacter(this.termId);
		}

		// Token: 0x06007434 RID: 29748 RVA: 0x0025CDC9 File Offset: 0x0025AFC9
		public void OnCreatorCodeValid(string id, string s, NexusGroupId ngid)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeTitle.text = "CREATOR CODE: VALID";
		}

		// Token: 0x06007435 RID: 29749 RVA: 0x0025CDEA File Offset: 0x0025AFEA
		public void OnCreatorCodeValidating(string id)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeTitle.text = "CREATOR CODE: VALIDATING";
		}

		// Token: 0x06007436 RID: 29750 RVA: 0x0025CE0B File Offset: 0x0025B00B
		public void CreatorCodeInvalid(string id)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeTitle.text = "CREATOR CODE: INVALID";
		}

		// Token: 0x06007437 RID: 29751 RVA: 0x0025CE0B File Offset: 0x0025B00B
		public void OnCreatorCodeFailure(string id)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeTitle.text = "CREATOR CODE: INVALID";
		}

		// Token: 0x06007438 RID: 29752 RVA: 0x0025CE2C File Offset: 0x0025B02C
		bool IBuildValidation.BuildValidationCheck()
		{
			if (this.nexusGroups.Length == 0)
			{
				Debug.LogError("You have to set at least one nexus group in " + base.name + " or things will not work!");
				return false;
			}
			return true;
		}

		// Token: 0x06007439 RID: 29753 RVA: 0x0025CE54 File Offset: 0x0025B054
		public void GetCreatorCode(out string code, out NexusGroupId[] groups)
		{
			code = CreatorCodes.getCurrentCreatorCode(this.termId);
			groups = this.nexusGroups;
		}

		// Token: 0x040083FC RID: 33788
		private string termId;

		// Token: 0x040083FD RID: 33789
		[SerializeField]
		private TMP_Text creatorCodeField;

		// Token: 0x040083FE RID: 33790
		[SerializeField]
		private TMP_Text creatorCodeTitle;

		// Token: 0x040083FF RID: 33791
		[SerializeField]
		private NexusGroupId[] nexusGroups;
	}
}
