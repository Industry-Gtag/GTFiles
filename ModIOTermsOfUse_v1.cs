using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000AA3 RID: 2723
public class ModIOTermsOfUse_v1 : MonoBehaviour
{
	// Token: 0x060045B9 RID: 17849 RVA: 0x00176491 File Offset: 0x00174691
	private void OnEnable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x060045BA RID: 17850 RVA: 0x001764B5 File Offset: 0x001746B5
	private void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
	}

	// Token: 0x060045BB RID: 17851 RVA: 0x001764D9 File Offset: 0x001746D9
	private void PostUpdate()
	{
		if (ControllerBehaviour.Instance.IsLeftStick)
		{
			this.TurnPage(-1);
		}
		if (ControllerBehaviour.Instance.IsRightStick)
		{
			this.TurnPage(1);
		}
		if (this.waitingForAcknowledge)
		{
			this.acceptButtonDown = ControllerBehaviour.Instance.ButtonDown;
		}
	}

	// Token: 0x060045BC RID: 17852 RVA: 0x0017651C File Offset: 0x0017471C
	private async void Start()
	{
		while (!this.hasTermsOfUse)
		{
			await Task.Yield();
		}
		PrivateUIRoom.AddUI(this.uiParent);
		TaskAwaiter<bool> taskAwaiter = this.UpdateTextFromTerms().GetAwaiter();
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			TaskAwaiter<bool> taskAwaiter2;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<bool>);
		}
		if (taskAwaiter.GetResult())
		{
			await this.WaitForAcknowledgement();
			Action<bool> action = this.termsAcknowledgedCallback;
			if (action != null)
			{
				action(this.accepted);
			}
			PrivateUIRoom.RemoveUI(this.uiParent);
			Object.Destroy(base.gameObject);
			return;
		}
		for (;;)
		{
			await Task.Yield();
		}
	}

	// Token: 0x060045BD RID: 17853 RVA: 0x00176554 File Offset: 0x00174754
	private async Task<bool> UpdateTextFromTerms()
	{
		this.tmpTitle.text = this.title;
		this.tmpBody.text = "Loading...";
		bool flag = await this.UpdateTextWithFullTerms();
		if (!flag)
		{
			this.tmpBody.text = "Failed to retrieve full Terms of Use text from mod.io.\n\nPlease restart the game and try again.";
			this.tmpBody.pageToDisplay = 1;
			this.tmpPage.text = string.Empty;
		}
		return flag;
	}

	// Token: 0x060045BE RID: 17854 RVA: 0x00176598 File Offset: 0x00174798
	public async Task<bool> UpdateTextWithFullTerms()
	{
		return true;
	}

	// Token: 0x060045BF RID: 17855 RVA: 0x001765D4 File Offset: 0x001747D4
	private string GetStringForListItemIdx_LowerAlpha(int idx)
	{
		switch (idx)
		{
		case 0:
			return "  a. <indent=5%>";
		case 1:
			return "  b. <indent=5%>";
		case 2:
			return "  c. <indent=5%>";
		case 3:
			return "  d. <indent=5%>";
		case 4:
			return "  e. <indent=5%>";
		case 5:
			return "  f. <indent=5%>";
		case 6:
			return "  g. <indent=5%>";
		case 7:
			return "  h. <indent=5%>";
		case 8:
			return "  i. <indent=5%>";
		case 9:
			return "  j. <indent=5%>";
		case 10:
			return "  k. <indent=5%>";
		case 11:
			return "  l. <indent=5%>";
		case 12:
			return "  m. <indent=5%>";
		case 13:
			return "  n. <indent=5%>";
		case 14:
			return "  o. <indent=5%>";
		case 15:
			return "  p. <indent=5%>";
		case 16:
			return "  q. <indent=5%>";
		case 17:
			return "  r. <indent=5%>";
		case 18:
			return "  s. <indent=5%>";
		case 19:
			return "  t. <indent=5%>";
		case 20:
			return "  u. <indent=5%>";
		case 21:
			return "  v. <indent=5%>";
		case 22:
			return "  w. <indent=5%>";
		case 23:
			return "  x. <indent=5%>";
		case 24:
			return "  y. <indent=5%>";
		case 25:
			return "  z. <indent=5%>";
		default:
			return "";
		}
	}

	// Token: 0x060045C0 RID: 17856 RVA: 0x001766F8 File Offset: 0x001748F8
	private async Task WaitForAcknowledgement()
	{
		this.accepted = false;
		float progress = 0f;
		this.progressBar.transform.localScale = new Vector3(0f, 1f, 1f);
		while (progress < 1f)
		{
			if (this.acceptButtonDown)
			{
				progress += Time.deltaTime / this.holdTime;
			}
			else
			{
				progress = 0f;
			}
			this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(progress), 1f, 1f);
			this.progressBar.textureScale = new Vector2(Mathf.Clamp01(progress), -1f);
			await Task.Yield();
		}
		if (progress >= 1f)
		{
			this.Acknowledge(this.acceptButtonDown);
		}
	}

	// Token: 0x060045C1 RID: 17857 RVA: 0x0017673C File Offset: 0x0017493C
	public void TurnPage(int i)
	{
		this.tmpBody.pageToDisplay = Mathf.Clamp(this.tmpBody.pageToDisplay + i, 1, this.tmpBody.textInfo.pageCount);
		this.tmpPage.text = string.Format("page {0} of {1}", this.tmpBody.pageToDisplay, this.tmpBody.textInfo.pageCount);
		this.nextButton.SetActive(this.tmpBody.pageToDisplay < this.tmpBody.textInfo.pageCount);
		this.prevButton.SetActive(this.tmpBody.pageToDisplay > 1);
		this.ActivateAcceptButtonGroup();
	}

	// Token: 0x060045C2 RID: 17858 RVA: 0x001767F8 File Offset: 0x001749F8
	private void ActivateAcceptButtonGroup()
	{
		bool flag = this.tmpBody.pageToDisplay == this.tmpBody.textInfo.pageCount;
		this.yesNoButtons.SetActive(flag);
		this.waitingForAcknowledge = flag;
	}

	// Token: 0x060045C3 RID: 17859 RVA: 0x00176836 File Offset: 0x00174A36
	public void Acknowledge(bool didAccept)
	{
		this.accepted = didAccept;
	}

	// Token: 0x040057E2 RID: 22498
	[SerializeField]
	private Transform uiParent;

	// Token: 0x040057E3 RID: 22499
	[SerializeField]
	private string title;

	// Token: 0x040057E4 RID: 22500
	[SerializeField]
	private TMP_Text tmpBody;

	// Token: 0x040057E5 RID: 22501
	[SerializeField]
	private TMP_Text tmpTitle;

	// Token: 0x040057E6 RID: 22502
	[SerializeField]
	private TMP_Text tmpPage;

	// Token: 0x040057E7 RID: 22503
	[SerializeField]
	public GameObject yesNoButtons;

	// Token: 0x040057E8 RID: 22504
	[SerializeField]
	public GameObject nextButton;

	// Token: 0x040057E9 RID: 22505
	[SerializeField]
	public GameObject prevButton;

	// Token: 0x040057EA RID: 22506
	private bool hasTermsOfUse;

	// Token: 0x040057EB RID: 22507
	private Action<bool> termsAcknowledgedCallback;

	// Token: 0x040057EC RID: 22508
	private string cachedTermsText;

	// Token: 0x040057ED RID: 22509
	private bool waitingForAcknowledge;

	// Token: 0x040057EE RID: 22510
	private bool accepted;

	// Token: 0x040057EF RID: 22511
	private bool acceptButtonDown;

	// Token: 0x040057F0 RID: 22512
	[SerializeField]
	private float holdTime = 5f;

	// Token: 0x040057F1 RID: 22513
	[SerializeField]
	private LineRenderer progressBar;
}
