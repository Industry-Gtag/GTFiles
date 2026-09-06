using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Modio;
using Modio.Errors;
using Modio.Images;
using Modio.Mods;
using Modio.Unity;
using TMPro;
using UnityEngine;

// Token: 0x02000AA8 RID: 2728
public class NewMapsDisplay : MonoBehaviour
{
	// Token: 0x060045CD RID: 17869 RVA: 0x00176DC4 File Offset: 0x00174FC4
	public void OnEnable()
	{
		this.mapImage.gameObject.SetActive(false);
		this.mapInfoTMP.text = "";
		this.mapInfoTMP.gameObject.SetActive(false);
		UGCPermissionManager.SubscribeToUGCEnabled(new Action(this.OnUGCEnabled));
		UGCPermissionManager.SubscribeToUGCDisabled(new Action(this.OnUGCDisabled));
		if (!UGCPermissionManager.IsUGCDisabled)
		{
			if (!ModIOManager.IsInitialized() || !ModIOManager.TryGetNewMapsModId(out this.newMapsModId))
			{
				this.initCoroutine = base.StartCoroutine(this.DelayedInitialize());
			}
			else
			{
				if (this.newMapsModId == ModId.Null)
				{
					return;
				}
				this.Initialize();
			}
		}
		this.loadingText.gameObject.SetActive(true);
	}

	// Token: 0x060045CE RID: 17870 RVA: 0x00176E80 File Offset: 0x00175080
	public void OnDisable()
	{
		if (this.initCoroutine != null)
		{
			base.StopCoroutine(this.initCoroutine);
			this.initCoroutine = null;
		}
		this.newMapsModProfile = null;
		this.newMapDatas.Clear();
		this.slideshowActive = false;
		this.slideshowIndex = 0;
		this.lastSlideshowUpdate = 0f;
		this.mapImage.gameObject.SetActive(false);
		this.mapInfoTMP.text = "";
		this.mapInfoTMP.gameObject.SetActive(false);
		this.loadingText.text = this.loadingString;
		this.loadingText.gameObject.SetActive(false);
		UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
		UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
	}

	// Token: 0x060045CF RID: 17871 RVA: 0x00176F4C File Offset: 0x0017514C
	private void OnUGCEnabled()
	{
		if (this.newMapDatas.IsNullOrEmpty<NewMapsDisplay.NewMapData>())
		{
			if (!ModIOManager.IsInitialized() || !ModIOManager.TryGetNewMapsModId(out this.newMapsModId))
			{
				this.initCoroutine = base.StartCoroutine(this.DelayedInitialize());
				return;
			}
			if (this.newMapsModId == ModId.Null)
			{
				return;
			}
			this.Initialize();
		}
	}

	// Token: 0x060045D0 RID: 17872 RVA: 0x00176FA8 File Offset: 0x001751A8
	private void OnUGCDisabled()
	{
		this.mapImage.gameObject.SetActive(false);
		this.mapInfoTMP.text = "";
		this.mapInfoTMP.gameObject.SetActive(false);
		this.loadingText.text = this.ugcDisabledString;
		this.loadingText.gameObject.SetActive(true);
	}

	// Token: 0x060045D1 RID: 17873 RVA: 0x00177009 File Offset: 0x00175209
	private IEnumerator DelayedInitialize()
	{
		while (!ModIOManager.TryGetNewMapsModId(out this.newMapsModId))
		{
			yield return new WaitForSecondsRealtime(1f);
		}
		this.initCoroutine = null;
		if (this.newMapsModId == ModId.Null)
		{
			yield break;
		}
		this.Initialize();
		yield break;
	}

	// Token: 0x060045D2 RID: 17874 RVA: 0x00177018 File Offset: 0x00175218
	private async Task<Error> Initialize()
	{
		Error error2;
		if (!this.requestingNewMapsModProfile && !this.downloadingImages)
		{
			this.requestingNewMapsModProfile = true;
			this.loadingText.text = this.loadingString;
			Error error = await ModIOManager.Initialize();
			if (error)
			{
				error2 = error;
			}
			else if (!base.isActiveAndEnabled)
			{
				error2 = Error.None;
			}
			else
			{
				ValueTuple<Error, Mod> valueTuple = await ModIOManager.GetMod(this.newMapsModId, false, null);
				error = valueTuple.Item1;
				this.newMapsModProfile = valueTuple.Item2;
				if (error)
				{
					GTDev.LogWarning<string>("[NewMapsDisplay::OnGetNewMapsModProfile] Failed to get NewMaps ModProfile " + string.Format("from mod.io: {0}", error), null);
					error2 = error;
				}
				else
				{
					this.newMapDatas.Clear();
					string[] array = this.newMapsModProfile.MetadataBlob.Split(';', StringSplitOptions.None);
					string text = "";
					foreach (string text2 in array)
					{
						if (text2.StartsWith("mapInfo:"))
						{
							text = text2.Substring(8);
							break;
						}
					}
					string[] mapInfoList = (text.IsNullOrEmpty() ? null : text.Split(',', StringSplitOptions.None));
					this.lazyImage = new LazyImage<Texture2D>(ImageCacheTexture2D.Instance, delegate(Texture2D loadedImage)
					{
						this.downloadingImage = false;
						this.lastDownloadedImage = loadedImage;
					}, null);
					this.downloadingImages = true;
					for (int i = 0; i < this.newMapsModProfile.Gallery.Length; i++)
					{
						this.downloadingImage = true;
						this.lazyImage.SetImage<Mod.GalleryResolution>(this.newMapsModProfile.Gallery[i], Mod.GalleryResolution.X320_Y180);
						while (this.downloadingImage)
						{
							await Task.Yield();
						}
						string text3 = ((mapInfoList != null && mapInfoList.Length > i) ? mapInfoList[i] : "");
						NewMapsDisplay.NewMapData newMapData = new NewMapsDisplay.NewMapData
						{
							image = this.lastDownloadedImage,
							info = text3
						};
						this.newMapDatas.Add(newMapData);
						this.lastDownloadedImage = null;
					}
					this.downloadingImages = false;
					if (!base.isActiveAndEnabled)
					{
						error2 = Error.None;
					}
					else
					{
						this.StartSlideshow();
						this.requestingNewMapsModProfile = false;
						error2 = Error.None;
					}
				}
			}
		}
		else
		{
			error2 = new Error(ErrorCode.UNKNOWN, "Initialization already in progress.");
		}
		return error2;
	}

	// Token: 0x060045D3 RID: 17875 RVA: 0x0017705B File Offset: 0x0017525B
	private void StartSlideshow()
	{
		if (this.newMapDatas.IsNullOrEmpty<NewMapsDisplay.NewMapData>())
		{
			return;
		}
		this.slideshowIndex = 0;
		this.slideshowActive = true;
		this.UpdateSlideshow();
	}

	// Token: 0x060045D4 RID: 17876 RVA: 0x0017707F File Offset: 0x0017527F
	public void Update()
	{
		if (!this.slideshowActive || Time.time - this.lastSlideshowUpdate < this.slideshowUpdateInterval)
		{
			return;
		}
		this.UpdateSlideshow();
	}

	// Token: 0x060045D5 RID: 17877 RVA: 0x001770A4 File Offset: 0x001752A4
	private void UpdateSlideshow()
	{
		this.loadingText.gameObject.SetActive(false);
		this.lastSlideshowUpdate = Time.time;
		Texture2D image = this.newMapDatas[this.slideshowIndex].image;
		if (image != null)
		{
			Sprite sprite;
			if (!this.cachedTextures.TryGetValue(image, out sprite))
			{
				sprite = Sprite.Create(image, new Rect(0f, 0f, (float)image.width, (float)image.height), new Vector2(0.5f, 0.5f));
				this.cachedTextures.Add(image, sprite);
			}
			this.mapImage.sprite = sprite;
			this.mapImage.gameObject.SetActive(true);
		}
		else
		{
			this.mapImage.gameObject.SetActive(false);
		}
		this.mapInfoTMP.text = this.newMapDatas[this.slideshowIndex].info;
		this.mapInfoTMP.gameObject.SetActive(true);
		this.slideshowIndex++;
		if (this.slideshowIndex >= this.newMapDatas.Count)
		{
			this.slideshowIndex = 0;
		}
	}

	// Token: 0x04005803 RID: 22531
	[SerializeField]
	private SpriteRenderer mapImage;

	// Token: 0x04005804 RID: 22532
	[SerializeField]
	private TMP_Text loadingText;

	// Token: 0x04005805 RID: 22533
	[Tooltip("DEPRECATED")]
	[SerializeField]
	private TMP_Text modNameText;

	// Token: 0x04005806 RID: 22534
	[Tooltip("DEPRECATED")]
	[SerializeField]
	private TMP_Text modCreatorLabelText;

	// Token: 0x04005807 RID: 22535
	[Tooltip("DEPRECATED")]
	[SerializeField]
	private TMP_Text modCreatorText;

	// Token: 0x04005808 RID: 22536
	[SerializeField]
	private TMP_Text mapInfoTMP;

	// Token: 0x04005809 RID: 22537
	[SerializeField]
	private float slideshowUpdateInterval = 1f;

	// Token: 0x0400580A RID: 22538
	[SerializeField]
	private string loadingString = "LOADING...";

	// Token: 0x0400580B RID: 22539
	[SerializeField]
	private string ugcDisabledString = "UGC DISABLED BY K-ID SETTINGS";

	// Token: 0x0400580C RID: 22540
	private ModId newMapsModId = ModId.Null;

	// Token: 0x0400580D RID: 22541
	private Mod newMapsModProfile;

	// Token: 0x0400580E RID: 22542
	private List<NewMapsDisplay.NewMapData> newMapDatas = new List<NewMapsDisplay.NewMapData>();

	// Token: 0x0400580F RID: 22543
	private bool slideshowActive;

	// Token: 0x04005810 RID: 22544
	private int slideshowIndex;

	// Token: 0x04005811 RID: 22545
	private float lastSlideshowUpdate;

	// Token: 0x04005812 RID: 22546
	private bool requestingNewMapsModProfile;

	// Token: 0x04005813 RID: 22547
	private LazyImage<Texture2D> lazyImage;

	// Token: 0x04005814 RID: 22548
	private bool downloadingImages;

	// Token: 0x04005815 RID: 22549
	private bool downloadingImage;

	// Token: 0x04005816 RID: 22550
	private Texture2D lastDownloadedImage;

	// Token: 0x04005817 RID: 22551
	private Coroutine initCoroutine;

	// Token: 0x04005818 RID: 22552
	private Dictionary<Texture2D, Sprite> cachedTextures = new Dictionary<Texture2D, Sprite>();

	// Token: 0x02000AA9 RID: 2729
	private struct NewMapData
	{
		// Token: 0x04005819 RID: 22553
		public Texture2D image;

		// Token: 0x0400581A RID: 22554
		public string info;
	}
}
