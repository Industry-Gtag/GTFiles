using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000570 RID: 1392
public class CreatorCodeSmallDisplay : MonoBehaviour
{
	// Token: 0x0600235F RID: 9055 RVA: 0x000BE81A File Offset: 0x000BCA1A
	private void Awake()
	{
		this.codeText.text = "CREATOR CODE: <NONE>";
		ATM_Manager.instance.smallDisplays.Add(this);
	}

	// Token: 0x06002360 RID: 9056 RVA: 0x000BE83E File Offset: 0x000BCA3E
	public void SetCode(string code)
	{
		if (code == "")
		{
			this.codeText.text = "CREATOR CODE: <NONE>";
			return;
		}
		this.codeText.text = "CREATOR CODE: " + code;
	}

	// Token: 0x06002361 RID: 9057 RVA: 0x000BE874 File Offset: 0x000BCA74
	public void SuccessfulPurchase(string memberName)
	{
		if (!string.IsNullOrWhiteSpace(memberName))
		{
			this.codeText.text = "SUPPORTED: " + memberName + "!";
		}
	}

	// Token: 0x04002E8B RID: 11915
	public Text codeText;

	// Token: 0x04002E8C RID: 11916
	private const string CreatorCode = "CREATOR CODE: ";

	// Token: 0x04002E8D RID: 11917
	private const string CreatorSupported = "SUPPORTED: ";

	// Token: 0x04002E8E RID: 11918
	private const string NoCreator = "<NONE>";
}
