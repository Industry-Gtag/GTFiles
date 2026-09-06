using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.VirtualStumpCustomMaps.UI
{
	// Token: 0x02000FDF RID: 4063
	public class CustomMapLoadProgressBar : MonoBehaviour
	{
		// Token: 0x170009A6 RID: 2470
		// (get) Token: 0x0600651B RID: 25883 RVA: 0x00208B6B File Offset: 0x00206D6B
		// (set) Token: 0x0600651C RID: 25884 RVA: 0x00208B73 File Offset: 0x00206D73
		public CustomMapLoadProgressBar.BarState State { get; private set; }

		// Token: 0x170009A7 RID: 2471
		// (get) Token: 0x0600651D RID: 25885 RVA: 0x00208B7C File Offset: 0x00206D7C
		// (set) Token: 0x0600651E RID: 25886 RVA: 0x00208B84 File Offset: 0x00206D84
		public MapLoadStatus Phase { get; private set; }

		// Token: 0x170009A8 RID: 2472
		// (get) Token: 0x0600651F RID: 25887 RVA: 0x00208B8D File Offset: 0x00206D8D
		// (set) Token: 0x06006520 RID: 25888 RVA: 0x00208B95 File Offset: 0x00206D95
		public int PercentComplete { get; private set; }

		// Token: 0x170009A9 RID: 2473
		// (get) Token: 0x06006521 RID: 25889 RVA: 0x00208B9E File Offset: 0x00206D9E
		// (set) Token: 0x06006522 RID: 25890 RVA: 0x00208BA6 File Offset: 0x00206DA6
		public bool HasMeasurablePercent { get; private set; }

		// Token: 0x170009AA RID: 2474
		// (get) Token: 0x06006523 RID: 25891 RVA: 0x00208BAF File Offset: 0x00206DAF
		public float NormalizedProgress
		{
			get
			{
				return this.targetFill;
			}
		}

		// Token: 0x170009AB RID: 2475
		// (get) Token: 0x06006524 RID: 25892 RVA: 0x00208BB7 File Offset: 0x00206DB7
		public float DisplayedProgress
		{
			get
			{
				return this.displayedFill;
			}
		}

		// Token: 0x170009AC RID: 2476
		// (get) Token: 0x06006525 RID: 25893 RVA: 0x00208BBF File Offset: 0x00206DBF
		public string DetailMessage
		{
			get
			{
				return this.detailTextWithoutEllipsis;
			}
		}

		// Token: 0x06006526 RID: 25894 RVA: 0x00208BC8 File Offset: 0x00206DC8
		private void OnEnable()
		{
			CustomMapManager.OnMapLoadStatusChanged.AddListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadStatusChanged));
			CustomMapManager.OnMapLoadComplete.AddListener(new UnityAction<bool>(this.OnMapLoadComplete));
			CustomMapManager.OnMapUnloadComplete.AddListener(new UnityAction(this.OnMapUnloadComplete));
			this.SyncToCurrentState();
		}

		// Token: 0x06006527 RID: 25895 RVA: 0x00208C20 File Offset: 0x00206E20
		private void OnDisable()
		{
			CustomMapManager.OnMapLoadStatusChanged.RemoveListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadStatusChanged));
			CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnMapLoadComplete));
			CustomMapManager.OnMapUnloadComplete.RemoveListener(new UnityAction(this.OnMapUnloadComplete));
		}

		// Token: 0x06006528 RID: 25896 RVA: 0x00208C70 File Offset: 0x00206E70
		private void Update()
		{
			if (!Mathf.Approximately(this.displayedFill, this.targetFill))
			{
				this.displayedFill = ((this.fillLerpSpeed > 0f) ? Mathf.MoveTowards(this.displayedFill, this.targetFill, this.fillLerpSpeed * Time.deltaTime) : this.targetFill);
				this.ApplyFill(this.displayedFill);
			}
			if (this.showEllipsis)
			{
				this.UpdateEllipsis();
			}
			if (this.finished && this.hideDelayAfterFinished > 0f && Time.time - this.finishedAtTime >= this.hideDelayAfterFinished)
			{
				this.ShowIdle();
			}
		}

		// Token: 0x06006529 RID: 25897 RVA: 0x00208D14 File Offset: 0x00206F14
		public void SyncToCurrentState()
		{
			MapLoadStatus currentLoadStatus = CustomMapManager.CurrentLoadStatus;
			if (currentLoadStatus != MapLoadStatus.None)
			{
				this.ApplyStatus(currentLoadStatus, CustomMapManager.CurrentLoadProgress, CustomMapManager.CurrentLoadMessage);
				this.displayedFill = this.targetFill;
				this.ApplyFill(this.displayedFill);
				return;
			}
			if (CustomMapManager.IsUnloading())
			{
				this.ApplyStatus(MapLoadStatus.Unloading, 0, "");
				return;
			}
			if (CustomMapLoader.IsMapLoaded())
			{
				this.ShowFinished(this.readyString, this.readyDetailString, this.readyColor, 1f, true);
				return;
			}
			this.ShowIdle();
		}

		// Token: 0x0600652A RID: 25898 RVA: 0x00208D95 File Offset: 0x00206F95
		private void OnMapLoadStatusChanged(MapLoadStatus status, int progress, string message)
		{
			this.ApplyStatus(status, progress, message);
		}

		// Token: 0x0600652B RID: 25899 RVA: 0x00208DA0 File Offset: 0x00206FA0
		private void OnMapLoadComplete(bool success)
		{
			if (success)
			{
				this.ShowFinished(this.readyString, this.readyDetailString, this.readyColor, 1f, true);
				return;
			}
			this.ShowFinished(this.failedString, CustomMapManager.CurrentLoadMessage, this.failedColor, this.displayedFill, false);
		}

		// Token: 0x0600652C RID: 25900 RVA: 0x00208DED File Offset: 0x00206FED
		private void OnMapUnloadComplete()
		{
			this.ShowIdle();
		}

		// Token: 0x0600652D RID: 25901 RVA: 0x00208DF8 File Offset: 0x00206FF8
		private void ApplyStatus(MapLoadStatus status, int progress, string message)
		{
			this.Phase = status;
			switch (status)
			{
			case MapLoadStatus.None:
				this.ShowIdle();
				return;
			case MapLoadStatus.Downloading:
				this.SetWorking(this.downloadingString, message, progress, progress > 0);
				return;
			case MapLoadStatus.Loading:
				this.SetWorking((progress > 0) ? this.loadingString : this.preparingString, message, progress, progress > 0);
				return;
			case MapLoadStatus.Unloading:
				this.SetWorking(this.unloadingString, message, 100, false);
				return;
			case MapLoadStatus.Error:
				this.ShowFinished(this.failedString, message, this.failedColor, this.displayedFill, false);
				return;
			case MapLoadStatus.Installing:
				this.SetWorking(this.installingString, message, progress, progress > 0);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600652E RID: 25902 RVA: 0x00208EA4 File Offset: 0x002070A4
		private void SetWorking(string status, string detail, int percent, bool hasMeasurableProgress)
		{
			this.finished = false;
			this.SetContentActive(true);
			this.State = CustomMapLoadProgressBar.BarState.Working;
			this.HasMeasurablePercent = hasMeasurableProgress;
			this.PercentComplete = Mathf.Clamp(percent, 0, 100);
			CustomMapLoadProgressBar.SetText(this.statusLabel, status);
			if (this.tintStatusLabelByStatus && this.statusLabel != null)
			{
				this.statusLabel.color = this.workingColor;
			}
			if (this.percentLabel != null)
			{
				this.percentLabel.gameObject.SetActive(hasMeasurableProgress);
				if (hasMeasurableProgress)
				{
					CustomMapLoadProgressBar.SharedStringBuilder.Clear();
					CustomMapLoadProgressBar.SharedStringBuilder.Append(this.PercentComplete);
					CustomMapLoadProgressBar.SharedStringBuilder.Append('%');
					this.percentLabel.SetText(CustomMapLoadProgressBar.SharedStringBuilder);
				}
			}
			this.SetDetail(detail, !hasMeasurableProgress);
			this.targetFill = (float)this.PercentComplete / 100f;
			this.SetFillColor(this.workingColor);
		}

		// Token: 0x0600652F RID: 25903 RVA: 0x00208F98 File Offset: 0x00207198
		private void ShowFinished(string status, string detail, Color color, float fill, bool succeeded)
		{
			this.finished = true;
			this.finishedAtTime = Time.time;
			this.SetContentActive(true);
			this.State = (succeeded ? CustomMapLoadProgressBar.BarState.Ready : CustomMapLoadProgressBar.BarState.Failed);
			this.Phase = (succeeded ? MapLoadStatus.None : MapLoadStatus.Error);
			this.HasMeasurablePercent = false;
			this.PercentComplete = Mathf.RoundToInt(Mathf.Clamp01(fill) * 100f);
			CustomMapLoadProgressBar.SetText(this.statusLabel, status);
			if (this.tintStatusLabelByStatus && this.statusLabel != null)
			{
				this.statusLabel.color = color;
			}
			if (this.percentLabel != null)
			{
				this.percentLabel.gameObject.SetActive(false);
			}
			this.SetDetail(detail, false);
			this.targetFill = Mathf.Clamp01(fill);
			this.SetFillColor(color);
		}

		// Token: 0x06006530 RID: 25904 RVA: 0x00209064 File Offset: 0x00207264
		private void ShowIdle()
		{
			this.finished = false;
			this.showEllipsis = false;
			this.State = CustomMapLoadProgressBar.BarState.Idle;
			this.Phase = MapLoadStatus.None;
			this.PercentComplete = 0;
			this.HasMeasurablePercent = false;
			this.targetFill = 0f;
			this.displayedFill = 0f;
			this.ApplyFill(0f);
			if (this.contentRoot != null)
			{
				this.SetContentActive(false);
				return;
			}
			CustomMapLoadProgressBar.SetText(this.statusLabel, "");
			if (this.percentLabel != null)
			{
				this.percentLabel.gameObject.SetActive(false);
			}
			this.SetDetail("", false);
		}

		// Token: 0x06006531 RID: 25905 RVA: 0x0020910D File Offset: 0x0020730D
		private void SetContentActive(bool active)
		{
			if (this.contentRoot != null && this.contentRoot.activeSelf != active)
			{
				this.contentRoot.SetActive(active);
			}
		}

		// Token: 0x06006532 RID: 25906 RVA: 0x00209138 File Offset: 0x00207338
		private void SetDetail(string detail, bool animateEllipsis)
		{
			this.detailTextWithoutEllipsis = detail ?? "";
			this.showEllipsis = animateEllipsis && this.detailLabel != null && this.detailTextWithoutEllipsis.Length > 0;
			this.ellipsisDotCount = -1;
			if (this.detailLabel == null)
			{
				return;
			}
			this.detailLabel.gameObject.SetActive(this.detailTextWithoutEllipsis.Length > 0);
			if (this.showEllipsis)
			{
				this.UpdateEllipsis();
				return;
			}
			this.detailLabel.SetText(this.detailTextWithoutEllipsis);
		}

		// Token: 0x06006533 RID: 25907 RVA: 0x002091D4 File Offset: 0x002073D4
		private void UpdateEllipsis()
		{
			int num = (int)(Time.time / Mathf.Max(0.01f, this.ellipsisSecondsPerDot)) % 4;
			if (num == this.ellipsisDotCount)
			{
				return;
			}
			this.ellipsisDotCount = num;
			CustomMapLoadProgressBar.SharedStringBuilder.Clear();
			CustomMapLoadProgressBar.SharedStringBuilder.Append(this.detailTextWithoutEllipsis);
			for (int i = 0; i < num; i++)
			{
				CustomMapLoadProgressBar.SharedStringBuilder.Append('.');
			}
			this.detailLabel.SetText(CustomMapLoadProgressBar.SharedStringBuilder);
		}

		// Token: 0x06006534 RID: 25908 RVA: 0x00209251 File Offset: 0x00207451
		private static void SetText(TMP_Text label, string text)
		{
			if (label == null)
			{
				return;
			}
			label.gameObject.SetActive(!string.IsNullOrEmpty(text));
			label.SetText(text);
		}

		// Token: 0x06006535 RID: 25909 RVA: 0x00209278 File Offset: 0x00207478
		private void ApplyFill(float fill)
		{
			if (this.fillTransform == null)
			{
				return;
			}
			Vector3 localScale = this.fillTransform.localScale;
			switch (this.fillAxis)
			{
			case CustomMapLoadProgressBar.FillAxis.X:
				localScale.x = fill;
				break;
			case CustomMapLoadProgressBar.FillAxis.Y:
				localScale.y = fill;
				break;
			case CustomMapLoadProgressBar.FillAxis.Z:
				localScale.z = fill;
				break;
			}
			this.fillTransform.localScale = localScale;
		}

		// Token: 0x06006536 RID: 25910 RVA: 0x002092E4 File Offset: 0x002074E4
		private void SetFillColor(Color color)
		{
			if (!this.tintFillByStatus || this.fillRenderer == null || string.IsNullOrEmpty(this.fillColorPropertyName))
			{
				return;
			}
			if (this.fillPropertyBlock == null)
			{
				this.fillPropertyBlock = new MaterialPropertyBlock();
			}
			if (this.fillColorPropertyId < 0)
			{
				this.fillColorPropertyId = Shader.PropertyToID(this.fillColorPropertyName);
			}
			this.fillRenderer.GetPropertyBlock(this.fillPropertyBlock);
			this.fillPropertyBlock.SetColor(this.fillColorPropertyId, color);
			this.fillRenderer.SetPropertyBlock(this.fillPropertyBlock);
		}

		// Token: 0x040073C5 RID: 29637
		[Header("Bar")]
		[Tooltip("Scaled from 0 to 1 along Fill Axis. Its pivot must be at the empty end of the bar.")]
		[SerializeField]
		private Transform fillTransform;

		// Token: 0x040073C6 RID: 29638
		[SerializeField]
		private CustomMapLoadProgressBar.FillAxis fillAxis;

		// Token: 0x040073C7 RID: 29639
		[Tooltip("[Optional] Tinted per status when Tint Fill By Status is on")]
		[SerializeField]
		private Renderer fillRenderer;

		// Token: 0x040073C8 RID: 29640
		[SerializeField]
		private bool tintFillByStatus = true;

		// Token: 0x040073C9 RID: 29641
		[SerializeField]
		private string fillColorPropertyName = "_Color";

		// Token: 0x040073CA RID: 29642
		[SerializeField]
		private Color workingColor;

		// Token: 0x040073CB RID: 29643
		[SerializeField]
		private Color readyColor;

		// Token: 0x040073CC RID: 29644
		[SerializeField]
		private Color failedColor;

		// Token: 0x040073CD RID: 29645
		[Header("Text")]
		[Tooltip("The phase - like DOWNLOADING.")]
		[SerializeField]
		private TMP_Text statusLabel;

		// Token: 0x040073CE RID: 29646
		[Tooltip("The percentage - Hidden during phases that have no measurable progress")]
		[SerializeField]
		private TMP_Text percentLabel;

		// Token: 0x040073CF RID: 29647
		[Tooltip("The detail message from the loader - like LOADING MAP SCENE.")]
		[SerializeField]
		private TMP_Text detailLabel;

		// Token: 0x040073D0 RID: 29648
		[SerializeField]
		private bool tintStatusLabelByStatus = true;

		// Token: 0x040073D1 RID: 29649
		[SerializeField]
		private string preparingString = "PREPARING";

		// Token: 0x040073D2 RID: 29650
		[SerializeField]
		private string downloadingString = "DOWNLOADING";

		// Token: 0x040073D3 RID: 29651
		[SerializeField]
		private string installingString = "INSTALLING";

		// Token: 0x040073D4 RID: 29652
		[SerializeField]
		private string loadingString = "LOADING";

		// Token: 0x040073D5 RID: 29653
		[SerializeField]
		private string unloadingString = "UNLOADING";

		// Token: 0x040073D6 RID: 29654
		[SerializeField]
		private string readyString = "READY";

		// Token: 0x040073D7 RID: 29655
		[SerializeField]
		private string failedString = "FAILED";

		// Token: 0x040073D8 RID: 29656
		[SerializeField]
		private string readyDetailString = "";

		// Token: 0x040073D9 RID: 29657
		[Header("Visibility")]
		[Tooltip("Toggled off while there is nothing to report.Leave empty to keep the bar visible at all times")]
		[SerializeField]
		private GameObject contentRoot;

		// Token: 0x040073DA RID: 29658
		[Tooltip("How long READY / FAILED stays up before the bar hides itself. 0 keeps it up until the next load.")]
		[SerializeField]
		private float hideDelayAfterFinished = 6f;

		// Token: 0x040073DB RID: 29659
		[Tooltip("How quickly the bar catches up to a new value")]
		[SerializeField]
		private float fillLerpSpeed = 5f;

		// Token: 0x040073DC RID: 29660
		[Tooltip("Seconds per dot of the animated ellipsis shown while a phase has no measurable progress.")]
		[SerializeField]
		private float ellipsisSecondsPerDot = 0.35f;

		// Token: 0x040073DD RID: 29661
		private static readonly StringBuilder SharedStringBuilder = new StringBuilder(32);

		// Token: 0x040073DE RID: 29662
		private int fillColorPropertyId = -1;

		// Token: 0x040073DF RID: 29663
		private MaterialPropertyBlock fillPropertyBlock;

		// Token: 0x040073E0 RID: 29664
		private float targetFill;

		// Token: 0x040073E1 RID: 29665
		private float displayedFill;

		// Token: 0x040073E2 RID: 29666
		private bool showEllipsis;

		// Token: 0x040073E3 RID: 29667
		private int ellipsisDotCount = -1;

		// Token: 0x040073E4 RID: 29668
		private string detailTextWithoutEllipsis = "";

		// Token: 0x040073E5 RID: 29669
		private bool finished;

		// Token: 0x040073E6 RID: 29670
		private float finishedAtTime;

		// Token: 0x02000FE0 RID: 4064
		private enum FillAxis
		{
			// Token: 0x040073E8 RID: 29672
			X,
			// Token: 0x040073E9 RID: 29673
			Y,
			// Token: 0x040073EA RID: 29674
			Z
		}

		// Token: 0x02000FE1 RID: 4065
		public enum BarState
		{
			// Token: 0x040073EC RID: 29676
			Idle,
			// Token: 0x040073ED RID: 29677
			Working,
			// Token: 0x040073EE RID: 29678
			Ready,
			// Token: 0x040073EF RID: 29679
			Failed
		}
	}
}
