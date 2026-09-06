using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace GorillaTagScripts.VirtualStumpCustomMaps.UI
{
	// Token: 0x02000FE4 RID: 4068
	public class CustomMapsKeyButton : GorillaKeyButton<CustomMapKeyboardBinding>
	{
		// Token: 0x0600653B RID: 25915 RVA: 0x00209454 File Offset: 0x00207654
		protected override void OnEnableEvents()
		{
			base.OnEnableEvents();
			if (!this._isLocalized)
			{
				return;
			}
			this.OnLanguageChanged();
			LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		}

		// Token: 0x0600653C RID: 25916 RVA: 0x0020947C File Offset: 0x0020767C
		protected override void OnDisableEvents()
		{
			base.OnDisableEvents();
			if (!this._isLocalized)
			{
				return;
			}
			LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		}

		// Token: 0x0600653D RID: 25917 RVA: 0x002094A0 File Offset: 0x002076A0
		public static string BindingToString(CustomMapKeyboardBinding binding)
		{
			if (binding < CustomMapKeyboardBinding.up || (binding > CustomMapKeyboardBinding.option3 && binding < CustomMapKeyboardBinding.at))
			{
				if (binding >= CustomMapKeyboardBinding.up)
				{
					return binding.ToString();
				}
				int num = (int)binding;
				return num.ToString();
			}
			else
			{
				switch (binding)
				{
				case CustomMapKeyboardBinding.at:
					return "@";
				case CustomMapKeyboardBinding.dash:
					return "-";
				case CustomMapKeyboardBinding.period:
					return ".";
				case CustomMapKeyboardBinding.underscore:
					return "_";
				case CustomMapKeyboardBinding.plus:
					return "+";
				case CustomMapKeyboardBinding.space:
					return " ";
				default:
					return "";
				}
			}
		}

		// Token: 0x0600653E RID: 25918 RVA: 0x00002C2D File Offset: 0x00000E2D
		protected override void OnButtonPressedEvent()
		{
		}

		// Token: 0x0600653F RID: 25919 RVA: 0x00209528 File Offset: 0x00207728
		private void OnLanguageChanged()
		{
			if (!this._isLocalized)
			{
				return;
			}
			if (this._buttonDisplayNameTxt == null)
			{
				Debug.LogError("[LOCALIZATION::CUSTOM_MAPS_KEY_BUTTON] [_buttonDisplayNameTxt] has not been assigned and is NULL", this);
				return;
			}
			if (this._localizedName == null || this._localizedName.IsEmpty)
			{
				Debug.LogError("[LOCALIZATION::CUSTOM_MAPS_KEY_BUTTON] [_localizedName] has not been assigned", this);
				return;
			}
			this._buttonDisplayNameTxt.text = this._localizedName.GetLocalizedString();
		}

		// Token: 0x04007438 RID: 29752
		[SerializeField]
		private bool _isLocalized;

		// Token: 0x04007439 RID: 29753
		[SerializeField]
		private LocalizedString _localizedName;

		// Token: 0x0400743A RID: 29754
		[SerializeField]
		private TMP_Text _buttonDisplayNameTxt;
	}
}
