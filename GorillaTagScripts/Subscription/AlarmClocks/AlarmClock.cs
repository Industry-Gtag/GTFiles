using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription.AlarmClocks
{
	// Token: 0x02000FFB RID: 4091
	public sealed class AlarmClock : MonoBehaviour
	{
		// Token: 0x170009B3 RID: 2483
		// (get) Token: 0x060065CB RID: 26059 RVA: 0x0020C7E6 File Offset: 0x0020A9E6
		public string Key
		{
			get
			{
				return this._key;
			}
		}

		// Token: 0x170009B4 RID: 2484
		// (get) Token: 0x060065CC RID: 26060 RVA: 0x0020C7EE File Offset: 0x0020A9EE
		// (set) Token: 0x060065CD RID: 26061 RVA: 0x0020C7F6 File Offset: 0x0020A9F6
		public bool Initialized { get; private set; }

		// Token: 0x170009B5 RID: 2485
		// (get) Token: 0x060065CE RID: 26062 RVA: 0x0020C7FF File Offset: 0x0020A9FF
		public bool IsVIMOnly
		{
			get
			{
				if (!this._vim_only_fetched)
				{
					this._isVIMOnly = AlarmClockManager.IsVIMOnly(this._key);
					this._vim_only_fetched = true;
				}
				return this._isVIMOnly;
			}
		}

		// Token: 0x170009B6 RID: 2486
		// (get) Token: 0x060065CF RID: 26063 RVA: 0x0020C827 File Offset: 0x0020AA27
		public bool ShouldBePressable
		{
			get
			{
				return !this.IsVIMOnly || SubscriptionManager.IsLocalSubscribed();
			}
		}

		// Token: 0x060065D0 RID: 26064 RVA: 0x0020C838 File Offset: 0x0020AA38
		private void OnEnable()
		{
			this._button.onPressButton.AddListener(new UnityAction(this.OnButtonPressed));
			this.OnActivate.AddListener(new UnityAction(this.OnActivateCallback));
			this.OnDeactivate.AddListener(new UnityAction(this.OnDeactivateCallback));
			base.StartCoroutine(this.ActivateCoroutine());
		}

		// Token: 0x060065D1 RID: 26065 RVA: 0x0020C89C File Offset: 0x0020AA9C
		private IEnumerator ActivateCoroutine()
		{
			while (!AlarmClockManager.Instance || !AlarmClockManager.Instance.Initialized)
			{
				yield return null;
			}
			this._VIMLabel.SetActive(this.IsVIMOnly);
			this._button.SetIsSubscriberButton(this.IsVIMOnly);
			if (AlarmClockManager.Instance.ActiveKey == this._key)
			{
				this.OnActivateCallback();
			}
			else
			{
				this.OnDeactivateCallback();
			}
			this.Initialized = true;
			yield break;
		}

		// Token: 0x060065D2 RID: 26066 RVA: 0x0020C8AC File Offset: 0x0020AAAC
		private void OnDisable()
		{
			this._button.onPressButton.RemoveListener(new UnityAction(this.OnButtonPressed));
			this.OnActivate.RemoveListener(new UnityAction(this.OnActivateCallback));
			this.OnDeactivate.RemoveListener(new UnityAction(this.OnDeactivateCallback));
			base.StopAllCoroutines();
		}

		// Token: 0x060065D3 RID: 26067 RVA: 0x0020C909 File Offset: 0x0020AB09
		private void OnButtonPressed()
		{
			if (!this.Initialized)
			{
				return;
			}
			if (Time.time < this._lastTouchTime + 0.25f)
			{
				return;
			}
			if (!this.ShouldBePressable)
			{
				return;
			}
			this._lastTouchTime = Time.time;
			this.ToggleAlarmClock();
		}

		// Token: 0x060065D4 RID: 26068 RVA: 0x0020C942 File Offset: 0x0020AB42
		[ContextMenu("Set Alarm Clock")]
		private void ToggleAlarmClock()
		{
			AlarmClockManager.ToggleAlarmClock(this);
		}

		// Token: 0x060065D5 RID: 26069 RVA: 0x0020C94A File Offset: 0x0020AB4A
		private void OnActivateCallback()
		{
			this._alarmClockOff.SetActive(false);
			this._button.buttonRenderer.material.color = Color.red;
		}

		// Token: 0x060065D6 RID: 26070 RVA: 0x0020C974 File Offset: 0x0020AB74
		private void OnDeactivateCallback()
		{
			this._alarmClockOff.SetActive(true);
			this._button.buttonRenderer.material.color = (this.ShouldBePressable ? Color.white : new Color(0.33f, 0.33f, 0.33f));
		}

		// Token: 0x040074E5 RID: 29925
		private const float TouchDebouncePeriod = 0.25f;

		// Token: 0x040074E6 RID: 29926
		[SerializeField]
		private string _key;

		// Token: 0x040074E7 RID: 29927
		[SerializeField]
		private GorillaPressableButton _button;

		// Token: 0x040074E8 RID: 29928
		[SerializeField]
		private GameObject _VIMLabel;

		// Token: 0x040074E9 RID: 29929
		[SerializeField]
		private GameObject _alarmClockOff;

		// Token: 0x040074EA RID: 29930
		[SerializeField]
		private float _onTime = 1f;

		// Token: 0x040074EB RID: 29931
		[SerializeField]
		private float _offTime = 0.2f;

		// Token: 0x040074EC RID: 29932
		public UnityEvent OnActivate;

		// Token: 0x040074ED RID: 29933
		public UnityEvent OnDeactivate;

		// Token: 0x040074EE RID: 29934
		private float _lastTouchTime = float.MinValue;

		// Token: 0x040074EF RID: 29935
		private bool _isVIMOnly;

		// Token: 0x040074F0 RID: 29936
		private bool _vim_only_fetched;
	}
}
