using System;
using System.Collections;
using GorillaExtensions;
using GorillaGameModes;
using GT_CustomMapSupportRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000FDC RID: 4060
	public class VirtualStumpReturnWatch : MonoBehaviour
	{
		// Token: 0x06006507 RID: 25863 RVA: 0x00208628 File Offset: 0x00206828
		private void Start()
		{
			if (this.returnButton != null)
			{
				this.returnButton.onStartPressingButton.AddListener(new UnityAction(this.OnStartedPressingButton));
				this.returnButton.onStopPressingButton.AddListener(new UnityAction(this.OnStoppedPressingButton));
				this.returnButton.onPressButton.AddListener(new UnityAction(this.OnButtonPressed));
			}
		}

		// Token: 0x06006508 RID: 25864 RVA: 0x00208698 File Offset: 0x00206898
		private void OnDestroy()
		{
			if (this.returnButton != null)
			{
				this.returnButton.onStartPressingButton.RemoveListener(new UnityAction(this.OnStartedPressingButton));
				this.returnButton.onStopPressingButton.RemoveListener(new UnityAction(this.OnStoppedPressingButton));
				this.returnButton.onPressButton.RemoveListener(new UnityAction(this.OnButtonPressed));
			}
		}

		// Token: 0x06006509 RID: 25865 RVA: 0x00208708 File Offset: 0x00206908
		public static void SetWatchProperties(VirtualStumpReturnWatchProps props)
		{
			VirtualStumpReturnWatch.currentCustomMapProps = props;
			VirtualStumpReturnWatch.currentCustomMapProps.holdDuration = Mathf.Clamp(VirtualStumpReturnWatch.currentCustomMapProps.holdDuration, 0.5f, 5f);
			VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Infection = Mathf.Clamp(VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Infection, 0.5f, 5f);
			VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Custom = Mathf.Clamp(VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Custom, 0.5f, 5f);
		}

		// Token: 0x0600650A RID: 25866 RVA: 0x00208784 File Offset: 0x00206984
		private float GetCurrentHoldDuration()
		{
			if (GorillaGameManager.instance.IsNull())
			{
				return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
			}
			switch (GorillaGameManager.instance.GameType())
			{
			case GameModeType.Infection:
				if (VirtualStumpReturnWatch.currentCustomMapProps.infectionOverride)
				{
					return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Infection;
				}
				return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
			case GameModeType.Custom:
				if (VirtualStumpReturnWatch.currentCustomMapProps.customModeOverride)
				{
					return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Custom;
				}
				return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
			}
			return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
		}

		// Token: 0x0600650B RID: 25867 RVA: 0x0020882D File Offset: 0x00206A2D
		private void OnStartedPressingButton()
		{
			this.startPressingButtonTime = Time.time;
			this.currentlyBeingPressed = true;
			this.returnButton.pressDuration = this.GetCurrentHoldDuration();
			this.ShowCountdownText();
			this.updateCountdownCoroutine = base.StartCoroutine(this.UpdateCountdownText());
		}

		// Token: 0x0600650C RID: 25868 RVA: 0x0020886A File Offset: 0x00206A6A
		private void OnStoppedPressingButton()
		{
			this.currentlyBeingPressed = false;
			this.HideCountdownText();
			if (this.updateCountdownCoroutine != null)
			{
				base.StopCoroutine(this.updateCountdownCoroutine);
				this.updateCountdownCoroutine = null;
			}
		}

		// Token: 0x0600650D RID: 25869 RVA: 0x00208894 File Offset: 0x00206A94
		private void OnButtonPressed()
		{
			this.currentlyBeingPressed = false;
			if (ZoneManagement.IsInZone(GTZone.customMaps) && !CustomMapManager.IsLocalPlayerInVirtualStump())
			{
				bool flag = VirtualStumpReturnWatch.currentCustomMapProps.shouldTagPlayer;
				bool flag2 = VirtualStumpReturnWatch.currentCustomMapProps.shouldKickPlayer;
				if (GorillaGameManager.instance.IsNotNull())
				{
					switch (GorillaGameManager.instance.GameType())
					{
					case GameModeType.Infection:
						if (VirtualStumpReturnWatch.currentCustomMapProps.infectionOverride)
						{
							flag = VirtualStumpReturnWatch.currentCustomMapProps.shouldTagPlayer_Infection;
							flag2 = VirtualStumpReturnWatch.currentCustomMapProps.shouldKickPlayer_Infection;
						}
						break;
					case GameModeType.Custom:
						if (VirtualStumpReturnWatch.currentCustomMapProps.customModeOverride)
						{
							flag = VirtualStumpReturnWatch.currentCustomMapProps.shouldTagPlayer_CustomMode;
							flag2 = VirtualStumpReturnWatch.currentCustomMapProps.shouldKickPlayer_CustomMode;
						}
						break;
					}
				}
				if (flag2 && NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.SessionIsPrivate)
				{
					NetworkSystem.Instance.ReturnToSinglePlayer();
				}
				else if (flag)
				{
					GameMode.ReportHit();
				}
				CustomMapManager.ReturnToVirtualStump();
			}
		}

		// Token: 0x0600650E RID: 25870 RVA: 0x00208994 File Offset: 0x00206B94
		private void ShowCountdownText()
		{
			if (this.countdownText.IsNull())
			{
				return;
			}
			int num = 1 + Mathf.FloorToInt(this.GetCurrentHoldDuration());
			this.countdownText.text = num.ToString();
			this.countdownText.gameObject.SetActive(true);
			if (this.buttonText.IsNotNull())
			{
				this.buttonText.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600650F RID: 25871 RVA: 0x00208A00 File Offset: 0x00206C00
		private void HideCountdownText()
		{
			if (this.countdownText.IsNull())
			{
				return;
			}
			this.countdownText.text = "";
			this.countdownText.gameObject.SetActive(false);
			if (this.buttonText.IsNotNull())
			{
				this.buttonText.gameObject.SetActive(true);
			}
		}

		// Token: 0x06006510 RID: 25872 RVA: 0x00208A5A File Offset: 0x00206C5A
		private IEnumerator UpdateCountdownText()
		{
			while (this.currentlyBeingPressed)
			{
				if (this.countdownText.IsNull())
				{
					yield break;
				}
				float num = this.GetCurrentHoldDuration() - (Time.time - this.startPressingButtonTime);
				int num2 = 1 + Mathf.FloorToInt(num);
				this.countdownText.text = num2.ToString();
				yield return null;
			}
			yield break;
		}

		// Token: 0x040073B7 RID: 29623
		[SerializeField]
		private HeldButton returnButton;

		// Token: 0x040073B8 RID: 29624
		[SerializeField]
		private TMP_Text buttonText;

		// Token: 0x040073B9 RID: 29625
		[SerializeField]
		private TMP_Text countdownText;

		// Token: 0x040073BA RID: 29626
		private static VirtualStumpReturnWatchProps currentCustomMapProps;

		// Token: 0x040073BB RID: 29627
		private float startPressingButtonTime = -1f;

		// Token: 0x040073BC RID: 29628
		private bool currentlyBeingPressed;

		// Token: 0x040073BD RID: 29629
		private Coroutine updateCountdownCoroutine;
	}
}
