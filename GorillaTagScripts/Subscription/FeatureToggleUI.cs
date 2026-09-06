using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.Subscription
{
	// Token: 0x02000FF3 RID: 4083
	[RequireComponent(typeof(SITouchscreenButtonContainer))]
	public class FeatureToggleUI : MonoBehaviour
	{
		// Token: 0x170009AF RID: 2479
		// (get) Token: 0x0600657E RID: 25982 RVA: 0x0020AF87 File Offset: 0x00209187
		// (set) Token: 0x0600657F RID: 25983 RVA: 0x0020AF8F File Offset: 0x0020918F
		public SITouchscreenButtonContainer ButtonContainer { get; private set; }

		// Token: 0x170009B0 RID: 2480
		// (get) Token: 0x06006580 RID: 25984 RVA: 0x0020AF98 File Offset: 0x00209198
		// (set) Token: 0x06006581 RID: 25985 RVA: 0x0020AFA5 File Offset: 0x002091A5
		public string LabelText
		{
			get
			{
				return this._label.text;
			}
			set
			{
				this._label.text = value;
			}
		}

		// Token: 0x06006582 RID: 25986 RVA: 0x0020AFB3 File Offset: 0x002091B3
		private void Awake()
		{
			this.ButtonContainer = base.gameObject.GetComponent<SITouchscreenButtonContainer>();
		}

		// Token: 0x06006583 RID: 25987 RVA: 0x0020AFC8 File Offset: 0x002091C8
		public void AttachToFeature(FeatureTogglesScreen.Feature feature)
		{
			this.ButtonContainer.button.buttonPressed.RemoveAllListeners();
			this.ButtonContainer.button.buttonToggled.RemoveAllListeners();
			this.LabelText = feature.DisplayName;
			bool flag = SubscriptionManager.GetSubscriptionSettingBool(feature.Value);
			bool flag2 = SubscriptionManager.IsSubscriptionFeatureAvailable(feature.Value);
			bool flag3 = true;
			if (flag2 && flag3)
			{
				this.ButtonContainer.button.buttonPressed.AddListener(delegate(SITouchscreenButton.SITouchscreenButtonType type, int data, int nr)
				{
					this.OnPressed(nr, feature);
				});
				this.ButtonContainer.button.buttonToggled.AddListener(delegate(SITouchscreenButton.SITouchscreenButtonType type, int data, int nr, bool state)
				{
					this.OnToggled(nr, feature, state);
				});
				this._unavailable.gameObject.SetActive(false);
			}
			else
			{
				flag = false;
				this._unavailable.gameObject.SetActive(true);
				if (!flag3)
				{
					this._unavailable.text = "ENABLE PERMISSION IN QUEST SETTINGS";
				}
				else
				{
					this._unavailable.text = "NOT AVAILABLE ON THIS DEVICE";
				}
			}
			this.ButtonContainer.button.SetToggleState(flag, false);
			this.ButtonContainer.UpdateToggleVisual();
		}

		// Token: 0x06006584 RID: 25988 RVA: 0x0020B0F5 File Offset: 0x002092F5
		private void OnPressed(int actorNr, FeatureTogglesScreen.Feature feature)
		{
			if (Time.time < this._disableUntil)
			{
				return;
			}
			if (actorNr != NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				return;
			}
			this._disableUntil = Time.time + 0.5f;
			feature.OnPressed.Invoke();
		}

		// Token: 0x06006585 RID: 25989 RVA: 0x0020B134 File Offset: 0x00209334
		private void OnToggled(int actorNr, FeatureTogglesScreen.Feature feature, bool state)
		{
			if (Time.time < this._disableUntil)
			{
				return;
			}
			if (actorNr != NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				return;
			}
			this._disableUntil = Time.time + 0.5f;
			feature.OnToggle.Invoke(state);
			this.ButtonContainer.button.SetToggleState(state, false);
			this.ButtonContainer.UpdateToggleVisual();
		}

		// Token: 0x04007482 RID: 29826
		[SerializeField]
		private TextMeshPro _label;

		// Token: 0x04007483 RID: 29827
		[SerializeField]
		private TextMeshPro _unavailable;

		// Token: 0x04007484 RID: 29828
		private const float DEBOUNCE_TIME = 0.5f;

		// Token: 0x04007485 RID: 29829
		private float _disableUntil = float.MinValue;
	}
}
