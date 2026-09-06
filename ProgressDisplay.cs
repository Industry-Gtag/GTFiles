using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000255 RID: 597
public class ProgressDisplay : MonoBehaviour
{
	// Token: 0x06001012 RID: 4114 RVA: 0x00056AA5 File Offset: 0x00054CA5
	private void Reset()
	{
		this.root = base.gameObject;
	}

	// Token: 0x06001013 RID: 4115 RVA: 0x00056AB3 File Offset: 0x00054CB3
	public void SetVisible(bool visible)
	{
		this.root.SetActive(visible);
	}

	// Token: 0x06001014 RID: 4116 RVA: 0x00056AC4 File Offset: 0x00054CC4
	public void SetProgress(int progress, int total)
	{
		if (this.text)
		{
			if (total < this.largestNumberToShow)
			{
				this.text.text = ((progress >= total) ? string.Format("{0}", total) : string.Format("{0}/{1}", progress, total));
				this.SetTextVisible(true);
			}
			else
			{
				this.SetTextVisible(false);
			}
		}
		this.progressImage.fillAmount = (float)progress / (float)total;
	}

	// Token: 0x06001015 RID: 4117 RVA: 0x00056B3E File Offset: 0x00054D3E
	public void SetProgress(float progress)
	{
		this.progressImage.fillAmount = progress;
	}

	// Token: 0x06001016 RID: 4118 RVA: 0x00056B4C File Offset: 0x00054D4C
	private void SetTextVisible(bool visible)
	{
		if (this.text.gameObject.activeSelf == visible)
		{
			return;
		}
		this.text.gameObject.SetActive(visible);
	}

	// Token: 0x04001340 RID: 4928
	[SerializeField]
	private GameObject root;

	// Token: 0x04001341 RID: 4929
	[SerializeField]
	private TMP_Text text;

	// Token: 0x04001342 RID: 4930
	[SerializeField]
	private Image progressImage;

	// Token: 0x04001343 RID: 4931
	[SerializeField]
	private int largestNumberToShow = 99;
}
