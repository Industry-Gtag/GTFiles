using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GorillaNetworking;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000BED RID: 3053
public class LegalAgreementBodyText : MonoBehaviour
{
	// Token: 0x06004CBF RID: 19647 RVA: 0x0019948E File Offset: 0x0019768E
	private void Awake()
	{
		this.textCollection.Add(this.textBox);
	}

	// Token: 0x06004CC0 RID: 19648 RVA: 0x001994A4 File Offset: 0x001976A4
	public void SetText(string text)
	{
		text = Regex.Unescape(text);
		string[] array = text.Split(new string[]
		{
			Environment.NewLine,
			"\\r\\n",
			"\n"
		}, StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			Text text2;
			if (i >= this.textCollection.Count)
			{
				text2 = Object.Instantiate<Text>(this.textBox, base.transform);
				this.textCollection.Add(text2);
			}
			else
			{
				text2 = this.textCollection[i];
			}
			text2.text = array[i];
		}
	}

	// Token: 0x06004CC1 RID: 19649 RVA: 0x00199534 File Offset: 0x00197734
	public void ClearText()
	{
		foreach (Text text in this.textCollection)
		{
			text.text = string.Empty;
		}
		this.state = LegalAgreementBodyText.State.Ready;
	}

	// Token: 0x06004CC2 RID: 19650 RVA: 0x00199590 File Offset: 0x00197790
	public async Task<bool> UpdateTextFromPlayFabTitleData(string key, string version)
	{
		string text = key + "_" + version;
		this.state = LegalAgreementBodyText.State.Loading;
		PlayFabTitleDataCache.Instance.GetTitleData(text, new Action<string>(this.OnTitleDataReceived), new Action<PlayFabError>(this.OnPlayFabError), false);
		while (this.state == LegalAgreementBodyText.State.Loading)
		{
			await Task.Yield();
		}
		bool flag;
		if (this.cachedText != null)
		{
			this.SetText(this.cachedText.Substring(1, this.cachedText.Length - 2));
			flag = true;
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	// Token: 0x06004CC3 RID: 19651 RVA: 0x001995E3 File Offset: 0x001977E3
	private void OnPlayFabError(PlayFabError obj)
	{
		Debug.LogError("ERROR: " + obj.ErrorMessage);
		this.state = LegalAgreementBodyText.State.Error;
	}

	// Token: 0x06004CC4 RID: 19652 RVA: 0x00199601 File Offset: 0x00197801
	private void OnTitleDataReceived(string text)
	{
		this.cachedText = text;
		this.state = LegalAgreementBodyText.State.Ready;
	}

	// Token: 0x17000759 RID: 1881
	// (get) Token: 0x06004CC5 RID: 19653 RVA: 0x00199614 File Offset: 0x00197814
	public float Height
	{
		get
		{
			return this.rectTransform.rect.height;
		}
	}

	// Token: 0x04005FE3 RID: 24547
	[SerializeField]
	private Text textBox;

	// Token: 0x04005FE4 RID: 24548
	[SerializeField]
	private TextAsset textAsset;

	// Token: 0x04005FE5 RID: 24549
	[SerializeField]
	private RectTransform rectTransform;

	// Token: 0x04005FE6 RID: 24550
	private List<Text> textCollection = new List<Text>();

	// Token: 0x04005FE7 RID: 24551
	private string cachedText;

	// Token: 0x04005FE8 RID: 24552
	private LegalAgreementBodyText.State state;

	// Token: 0x02000BEE RID: 3054
	private enum State
	{
		// Token: 0x04005FEA RID: 24554
		Ready,
		// Token: 0x04005FEB RID: 24555
		Loading,
		// Token: 0x04005FEC RID: 24556
		Error
	}
}
