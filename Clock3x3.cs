using System;
using System.Threading.Tasks;
using GorillaNetworking;
using PlayFab;
using TMPro;
using UnityEngine;

// Token: 0x02000039 RID: 57
public class Clock3x3 : ObservableBehavior
{
	// Token: 0x060000E7 RID: 231 RVA: 0x00005D7C File Offset: 0x00003F7C
	protected override void ObservableSliceUpdate()
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		DateTime now = DateTime.Now;
		if (serverTime.Year < 2000)
		{
			return;
		}
		this.display.text = string.Format(this.formatString, new object[]
		{
			this.headings[0],
			this.headings[1],
			serverTime.ToString("hh:mm:sstt"),
			DateTime.Now.ToString("hh:mm:sstt"),
			this.HexColor(this.color.Evaluate(((float)serverTime.Hour + (float)serverTime.Minute / 60f) / 24f)),
			this.HexColor(this.color.Evaluate(((float)now.Hour + (float)now.Minute / 60f) / 24f))
		});
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x00005E64 File Offset: 0x00004064
	public string HexColor(Color color)
	{
		return "#" + Mathf.FloorToInt(Mathf.Clamp01(color.r) * 255f).ToString("X2") + Mathf.FloorToInt(Mathf.Clamp01(color.g) * 255f).ToString("X2") + Mathf.FloorToInt(Mathf.Clamp01(color.b) * 255f).ToString("X2");
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x00005EE4 File Offset: 0x000040E4
	protected override void OnBecameObservable()
	{
		this.display.gameObject.SetActive(true);
		this.Initialize();
	}

	// Token: 0x060000EA RID: 234 RVA: 0x00005F00 File Offset: 0x00004100
	private async void Initialize()
	{
		if (!this.initialized)
		{
			this.initialized = true;
			this.formatString = this.display.text;
			this.display.text = string.Empty;
			if (!this.titleDataKey.IsNullOrEmpty())
			{
				while (PlayFabTitleDataCache.Instance == null)
				{
					await Task.Yield();
				}
				PlayFabTitleDataCache.Instance.GetTitleData(this.titleDataKey, new Action<string>(this.onTD), new Action<PlayFabError>(this.onTDError), false);
			}
		}
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00005F37 File Offset: 0x00004137
	private void onTD(string s)
	{
		this.headings = s.Split(";", StringSplitOptions.None);
	}

	// Token: 0x060000EC RID: 236 RVA: 0x00005F4B File Offset: 0x0000414B
	private void onTDError(PlayFabError error)
	{
		Debug.LogError(string.Format("Clock3x3 :: onTDError :: {0} :: {1}", this.titleDataKey, error));
	}

	// Token: 0x060000ED RID: 237 RVA: 0x00005F63 File Offset: 0x00004163
	protected override void OnLostObservable()
	{
		this.display.gameObject.SetActive(false);
	}

	// Token: 0x040000F4 RID: 244
	[SerializeField]
	private string titleDataKey;

	// Token: 0x040000F5 RID: 245
	[SerializeField]
	private TMP_Text display;

	// Token: 0x040000F6 RID: 246
	[SerializeField]
	private Gradient color;

	// Token: 0x040000F7 RID: 247
	private string formatString;

	// Token: 0x040000F8 RID: 248
	private bool initialized;

	// Token: 0x040000F9 RID: 249
	[SerializeField]
	private string[] headings;
}
