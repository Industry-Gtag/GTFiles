using System;
using Modio;
using Modio.Errors;
using Modio.Mods;
using Modio.Unity;
using TMPro;
using UnityEngine;

// Token: 0x02000AC0 RID: 2752
public class CustomMapsModTile : CustomMapsScreenTouchPoint
{
	// Token: 0x1700069E RID: 1694
	// (get) Token: 0x06004688 RID: 18056 RVA: 0x0017CB4A File Offset: 0x0017AD4A
	// (set) Token: 0x06004689 RID: 18057 RVA: 0x0017CB57 File Offset: 0x0017AD57
	public string PlayerCountText
	{
		get
		{
			return this._playerCountText.text;
		}
		set
		{
			this._playerCountText.text = value;
		}
	}

	// Token: 0x1700069F RID: 1695
	// (get) Token: 0x0600468A RID: 18058 RVA: 0x0017CB65 File Offset: 0x0017AD65
	public Mod CurrentMod
	{
		get
		{
			return this.currentMod;
		}
	}

	// Token: 0x0600468B RID: 18059 RVA: 0x0017CB6D File Offset: 0x0017AD6D
	protected override void Awake()
	{
		base.Awake();
		this.defaultLogo = this.touchPointRenderer.sprite;
		this.highlight.SetActive(false);
	}

	// Token: 0x0600468C RID: 18060 RVA: 0x0017CB94 File Offset: 0x0017AD94
	public void ShowTileText(bool show, bool useMapName)
	{
		if (!show)
		{
			this.ratingsText.gameObject.SetActive(false);
			this.mapNameText.gameObject.SetActive(false);
			this.thumsbUp.SetActive(false);
			this._playerCountText.gameObject.SetActive(false);
			return;
		}
		if (useMapName)
		{
			this.mapNameText.gameObject.SetActive(true);
			this.ratingsText.gameObject.SetActive(false);
			this.thumsbUp.SetActive(false);
		}
		else
		{
			this.ratingsText.gameObject.SetActive(true);
			this.thumsbUp.SetActive(true);
			this.mapNameText.gameObject.SetActive(false);
		}
		this._playerCountText.gameObject.SetActive(true);
	}

	// Token: 0x0600468D RID: 18061 RVA: 0x0017CC56 File Offset: 0x0017AE56
	public void ActivateTile(bool useMapName)
	{
		this.isActive = true;
		base.gameObject.SetActive(true);
		this.ShowTileText(true, useMapName);
		CustomMapsScreenTouchPoint.pressTime = Time.time;
	}

	// Token: 0x0600468E RID: 18062 RVA: 0x0017CC7D File Offset: 0x0017AE7D
	public void DeactivateTile()
	{
		this.isActive = false;
		base.gameObject.SetActive(false);
		this.highlight.SetActive(false);
		this.ShowTileText(false, false);
		this.ResetLogo();
	}

	// Token: 0x0600468F RID: 18063 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void PressButtonColourUpdate()
	{
	}

	// Token: 0x06004690 RID: 18064 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void OnButtonPressedEvent()
	{
	}

	// Token: 0x06004691 RID: 18065 RVA: 0x0017CCAC File Offset: 0x0017AEAC
	public async void SetMod(Mod mod, bool useMapName)
	{
		this._playerCountText.text = "-";
		this.ActivateTile(useMapName);
		this.touchPointRenderer.sprite = this.defaultLogo;
		this.highlight.SetActive(false);
		this.currentMod = mod;
		if (this.IsCurrentModHidden())
		{
			this.mapNameText.text = "HIDDEN MAP";
			this.ratingsText.text = "0%";
		}
		else
		{
			this.mapNameText.text = this.currentMod.Name;
			long num = this.currentMod.Stats.RatingsNegative + this.currentMod.Stats.RatingsPositive;
			string text;
			if (num < 1000L)
			{
				text = string.Format("({0})", num);
			}
			else if (num < 1000000L)
			{
				num = (long)Mathf.FloorToInt((float)(num / 100L));
				text = string.Format("({0}K)", num / 10L);
			}
			else
			{
				num = (long)Mathf.FloorToInt((float)(num / 100L));
				text = string.Format("({0}mil)", num / 10000L);
			}
			this.ratingsText.text = this.currentMod.Stats.RatingsPercent.ToString() + "% " + text;
			if (this.isDownloadingThumbnail)
			{
				this.newDownloadRequest = true;
			}
			else
			{
				this.isDownloadingThumbnail = true;
				Error error = new Error(ErrorCode.NONE);
				Texture2D tex = new Texture2D(320, 180);
				try
				{
					ValueTuple<Error, Texture2D> valueTuple = await mod.Logo.DownloadAsTexture2D(Mod.LogoResolution.X320_Y180);
					error = valueTuple.Item1;
					tex = valueTuple.Item2;
				}
				catch (Exception ex)
				{
					GTDev.Log<string>(string.Format("CustomMapsModTile::DownloadThumbnail error {0}", ex), null);
				}
				this.isDownloadingThumbnail = false;
				if (this.newDownloadRequest)
				{
					this.newDownloadRequest = false;
					this.SetMod(this.currentMod, useMapName);
				}
				else if (error)
				{
					GTDev.LogError<string>(string.Format("CustomMapsListScreen::DownloadThumbnail {0}", error), null);
				}
				else
				{
					this.touchPointRenderer.sprite = Sprite.Create(tex, new Rect(0f, 0f, 320f, 180f), new Vector2(0.5f, 0.5f));
				}
			}
		}
	}

	// Token: 0x06004692 RID: 18066 RVA: 0x0017CCF3 File Offset: 0x0017AEF3
	public void ResetLogo()
	{
		this.touchPointRenderer.sprite = this.defaultLogo;
	}

	// Token: 0x06004693 RID: 18067 RVA: 0x0017CD06 File Offset: 0x0017AF06
	public void ShowDetails()
	{
		CustomMapsTerminal.ShowDetailsScreen(this.currentMod);
	}

	// Token: 0x06004694 RID: 18068 RVA: 0x0017CD13 File Offset: 0x0017AF13
	public void HighlightTile()
	{
		this.highlight.SetActive(true);
	}

	// Token: 0x06004695 RID: 18069 RVA: 0x0017CD21 File Offset: 0x0017AF21
	public bool IsCurrentModHidden()
	{
		return this.currentMod.Creator == null || (!ModIOManager.IsLoggedIn() && this.currentMod.IsHidden());
	}

	// Token: 0x04005912 RID: 22802
	[SerializeField]
	private TMP_Text ratingsText;

	// Token: 0x04005913 RID: 22803
	[SerializeField]
	private TMP_Text mapNameText;

	// Token: 0x04005914 RID: 22804
	[SerializeField]
	private GameObject thumsbUp;

	// Token: 0x04005915 RID: 22805
	[SerializeField]
	private GameObject highlight;

	// Token: 0x04005916 RID: 22806
	[SerializeField]
	private TMP_Text _playerCountText;

	// Token: 0x04005917 RID: 22807
	private const float LOGO_WIDTH = 320f;

	// Token: 0x04005918 RID: 22808
	private const float LOGO_HEIGHT = 180f;

	// Token: 0x04005919 RID: 22809
	private Mod currentMod;

	// Token: 0x0400591A RID: 22810
	private Sprite defaultLogo;

	// Token: 0x0400591B RID: 22811
	private bool isDownloadingThumbnail;

	// Token: 0x0400591C RID: 22812
	private bool newDownloadRequest;

	// Token: 0x0400591D RID: 22813
	private bool isActive;
}
