using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription
{
	// Token: 0x02000FF1 RID: 4081
	public class FeatureTogglesScreen : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x170009AD RID: 2477
		// (get) Token: 0x06006571 RID: 25969 RVA: 0x0020AD80 File Offset: 0x00208F80
		private int NumPages
		{
			get
			{
				if (this._features.Length % 3 != 0)
				{
					return this._features.Length / 3 + 1;
				}
				return this._features.Length / 3;
			}
		}

		// Token: 0x170009AE RID: 2478
		// (get) Token: 0x06006572 RID: 25970 RVA: 0x0020ADA5 File Offset: 0x00208FA5
		private int LastPageIndex
		{
			get
			{
				return Math.Max(0, this.NumPages - 1);
			}
		}

		// Token: 0x06006573 RID: 25971 RVA: 0x0020ADB8 File Offset: 0x00208FB8
		private void Awake()
		{
			this._nextButton.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(this.OnNextButtonPressed));
			this._backButton.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(this.OnBackButtonPressed));
			this._exitButton.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(this.OnExitButtonPressed));
			this.MarkDirty();
		}

		// Token: 0x06006574 RID: 25972 RVA: 0x0020AE2E File Offset: 0x0020902E
		private void OnNextButtonPressed(SITouchscreenButton.SITouchscreenButtonType type, int data, int actorNr)
		{
			this._currentPage++;
			if (this._currentPage > this.LastPageIndex)
			{
				this._currentPage = this.LastPageIndex;
			}
			this.MarkDirty();
		}

		// Token: 0x06006575 RID: 25973 RVA: 0x0020AE5E File Offset: 0x0020905E
		private void OnBackButtonPressed(SITouchscreenButton.SITouchscreenButtonType type, int data, int actorNr)
		{
			this._currentPage--;
			if (this._currentPage < 0)
			{
				this._currentPage = 0;
			}
			this.MarkDirty();
		}

		// Token: 0x06006576 RID: 25974 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void OnExitButtonPressed(SITouchscreenButton.SITouchscreenButtonType type, int data, int actorNr)
		{
		}

		// Token: 0x06006577 RID: 25975 RVA: 0x0020AE84 File Offset: 0x00209084
		public void SliceUpdate()
		{
			if (!this._dirty)
			{
				return;
			}
			this._backButton.gameObject.SetActive(this._currentPage != 0);
			this._nextButton.gameObject.SetActive(this._currentPage != this.LastPageIndex);
			this.UpdateFeatureToggleUI();
			this._dirty = false;
		}

		// Token: 0x06006578 RID: 25976 RVA: 0x0020AEE1 File Offset: 0x002090E1
		private void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			this.MarkDirty();
		}

		// Token: 0x06006579 RID: 25977 RVA: 0x00012134 File Offset: 0x00010334
		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x0600657A RID: 25978 RVA: 0x0020AEF0 File Offset: 0x002090F0
		private void UpdateFeatureToggleUI()
		{
			for (int i = 0; i < this._featureToggleUi.Length; i++)
			{
				FeatureToggleUI featureToggleUI = this._featureToggleUi[i];
				int num = this._currentPage * 3 + i;
				bool flag = num < this._features.Length;
				featureToggleUI.gameObject.SetActive(flag);
				if (flag)
				{
					FeatureTogglesScreen.Feature feature = this._features[num];
					featureToggleUI.AttachToFeature(feature);
				}
			}
		}

		// Token: 0x0600657B RID: 25979 RVA: 0x0020AF51 File Offset: 0x00209151
		public void MarkDirty()
		{
			this._dirty = true;
		}

		// Token: 0x04007474 RID: 29812
		private const int TogglesPerPage = 3;

		// Token: 0x04007475 RID: 29813
		[SerializeField]
		private FeatureTogglesScreen.Feature[] _features;

		// Token: 0x04007476 RID: 29814
		[SerializeField]
		private SITouchscreenButtonContainer _nextButton;

		// Token: 0x04007477 RID: 29815
		[SerializeField]
		private SITouchscreenButtonContainer _backButton;

		// Token: 0x04007478 RID: 29816
		[SerializeField]
		private SITouchscreenButtonContainer _exitButton;

		// Token: 0x04007479 RID: 29817
		[SerializeField]
		private FeatureToggleUI[] _featureToggleUi;

		// Token: 0x0400747A RID: 29818
		private int _currentPage;

		// Token: 0x0400747B RID: 29819
		private bool _dirty = true;

		// Token: 0x02000FF2 RID: 4082
		[Serializable]
		public class Feature
		{
			// Token: 0x0400747C RID: 29820
			public string DisplayName = string.Empty;

			// Token: 0x0400747D RID: 29821
			public SubscriptionManager.SubscriptionFeatures Value;

			// Token: 0x0400747E RID: 29822
			public UnityEvent OnPressed;

			// Token: 0x0400747F RID: 29823
			public UnityEvent<bool> OnToggle;

			// Token: 0x04007480 RID: 29824
			public string UnavailableMessage = "NOT AVAILABLE ON THIS DEVICE";
		}
	}
}
