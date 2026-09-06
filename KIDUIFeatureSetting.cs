using System;
using KID.Model;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;

// Token: 0x02000BA8 RID: 2984
public class KIDUIFeatureSetting : MonoBehaviour
{
	// Token: 0x1700073D RID: 1853
	// (get) Token: 0x06004B58 RID: 19288 RVA: 0x001926F3 File Offset: 0x001908F3
	// (set) Token: 0x06004B59 RID: 19289 RVA: 0x001926FB File Offset: 0x001908FB
	public bool AlwaysCheckFeatureSetting { get; private set; }

	// Token: 0x06004B5A RID: 19290 RVA: 0x00192704 File Offset: 0x00190904
	public void CreateNewFeatureSettingGuardianManaged(KIDUI_MainScreen.FeatureToggleSetup feature, bool isEnabled)
	{
		this.CreateNewFeatureSettingWithoutToggle(feature, false);
		this._guardianManagedEnabled.SetActive(isEnabled);
		this._guardianManagedLocked.SetActive(!isEnabled);
	}

	// Token: 0x06004B5B RID: 19291 RVA: 0x00192729 File Offset: 0x00190929
	public KIDUIToggle CreateNewFeatureSettingWithToggle(KIDUI_MainScreen.FeatureToggleSetup feature, bool initialState = false, bool alwaysCheckFeatureSetting = false)
	{
		this.SetFeatureData(feature, alwaysCheckFeatureSetting, true);
		this._featureToggle.SetValue(initialState);
		KIDUIToggle featureToggle = this._featureToggle;
		if (featureToggle != null)
		{
			featureToggle.RegisterOnChangeEvent(new Action(this.SetFeatureName));
		}
		return this._featureToggle;
	}

	// Token: 0x06004B5C RID: 19292 RVA: 0x00192763 File Offset: 0x00190963
	public void CreateNewFeatureSettingWithoutToggle(KIDUI_MainScreen.FeatureToggleSetup feature, bool alwaysCheckFeatureSetting = false)
	{
		this.SetFeatureData(feature, alwaysCheckFeatureSetting, false);
	}

	// Token: 0x06004B5D RID: 19293 RVA: 0x00192770 File Offset: 0x00190970
	private void SetFeatureData(KIDUI_MainScreen.FeatureToggleSetup feature, bool alwaysCheckFeatureSetting, bool featureToggleEnabled)
	{
		string text;
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(feature.enabledText, out text, "ON", null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FEATURE_SETTING] Failed to get key for  k-ID Feature [{0}]\n[{1}]", feature.featureName, feature.enabledText), this);
		}
		this._enabledTextStr = text;
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(feature.disabledText, out text, "OFF", null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FEATURE_SETTING] Failed to get key for  k-ID Feature [{0}]\n[{1}]", feature.featureName, feature.disabledText), this);
		}
		this._disabledTextStr = text;
		this._hasToggle = featureToggleEnabled;
		this._featureType = feature.linkedFeature;
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(feature.featureName, out text, feature.permissionName, null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FeatureSetting] Failed to get key for k-ID Feature [{0}]\n[{1}]", feature.featureName, feature.disabledText), this);
		}
		this._featureName = text;
		this.SetFeatureName();
		GameObject gameObject = base.gameObject;
		string name = gameObject.name;
		string text2 = "_";
		LocalizedString featureName = feature.featureName;
		gameObject.name = name + text2 + ((featureName != null) ? featureName.ToString() : null);
		this._permissionName = feature.permissionName;
		this._featureToggle.gameObject.SetActive(featureToggleEnabled);
		this.AlwaysCheckFeatureSetting = alwaysCheckFeatureSetting;
		this._feature = feature;
	}

	// Token: 0x06004B5E RID: 19294 RVA: 0x00192898 File Offset: 0x00190A98
	public void RefreshTextOnLanguageChanged()
	{
		string text;
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(this._feature.enabledText, out text, "ON", null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FeatureSetting] Failed to get key for Game Mode [{0}]", this._feature.enabledText));
		}
		this._enabledTextStr = text;
		Debug.Log("[KIDUIFeatureSetting::Language] Refreshed enabled text: " + this._enabledTextStr);
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(this._feature.disabledText, out text, "OFF", null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FeatureSetting] Failed to get key for Game Mode [{0}]", this._feature.disabledText));
		}
		this._disabledTextStr = text;
		Debug.Log("[KIDUIFeatureSetting::Language] Refreshed disabled text: " + this._disabledTextStr);
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(this._feature.featureName, out text, this._feature.permissionName, null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FeatureSetting] Failed to get key for Game Mode [{0}]", this._feature.disabledText));
		}
		this._featureName = text;
		Debug.Log("[KIDUIFeatureSetting::Language] Refreshed feature name text: " + this._featureName);
		this.SetFeatureName();
	}

	// Token: 0x06004B5F RID: 19295 RVA: 0x001929A1 File Offset: 0x00190BA1
	public void UnregisterOnToggleChangeEvent(Action action)
	{
		this._featureToggle.UnregisterOnChangeEvent(action);
	}

	// Token: 0x06004B60 RID: 19296 RVA: 0x001929AF File Offset: 0x00190BAF
	public void RegisterToggleOnEvent(Action action)
	{
		this._featureToggle.RegisterToggleOnEvent(action);
	}

	// Token: 0x06004B61 RID: 19297 RVA: 0x001929BD File Offset: 0x00190BBD
	public void UnregisterToggleOnEvent(Action action)
	{
		this._featureToggle.UnregisterToggleOnEvent(action);
	}

	// Token: 0x06004B62 RID: 19298 RVA: 0x001929CB File Offset: 0x00190BCB
	public void RegisterToggleOffEvent(Action action)
	{
		this._featureToggle.RegisterToggleOffEvent(action);
	}

	// Token: 0x06004B63 RID: 19299 RVA: 0x001929D9 File Offset: 0x00190BD9
	public void UnregisterToggleOffEvent(Action action)
	{
		this._featureToggle.UnregisterToggleOffEvent(action);
	}

	// Token: 0x06004B64 RID: 19300 RVA: 0x001929E7 File Offset: 0x00190BE7
	public bool GetFeatureToggleState()
	{
		if (this._hasToggle)
		{
			return this._featureToggle.IsOn;
		}
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(this._featureType);
		if (permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.GUARDIAN)
		{
			Debug.LogError("[KID::FeatureSetting] GetToggleState: feature has no toggle AND is not managed by Guardian");
		}
		return permissionDataByFeature.Enabled;
	}

	// Token: 0x06004B65 RID: 19301 RVA: 0x00192A20 File Offset: 0x00190C20
	public bool GetHasToggle()
	{
		return this._hasToggle;
	}

	// Token: 0x06004B66 RID: 19302 RVA: 0x00192A28 File Offset: 0x00190C28
	public void SetFeatureSettingVisible(bool visible)
	{
		base.gameObject.SetActive(visible);
	}

	// Token: 0x06004B67 RID: 19303 RVA: 0x00192A36 File Offset: 0x00190C36
	public void SetFeatureToggle(bool enableToggle)
	{
		this._featureToggle.interactable = enableToggle;
	}

	// Token: 0x06004B68 RID: 19304 RVA: 0x00192A44 File Offset: 0x00190C44
	public void SetGuardianManagedState(bool isEnabled)
	{
		this._featureToggle.gameObject.SetActive(false);
		this._guardianManagedEnabled.SetActive(isEnabled);
		this._guardianManagedLocked.SetActive(!isEnabled);
		this.SetupGuardianManagedClickHandlers();
		this.SetFeatureName();
	}

	// Token: 0x06004B69 RID: 19305 RVA: 0x00192A80 File Offset: 0x00190C80
	public void SetPlayerManagedState(bool isInteractable, bool isOptedIn)
	{
		this._featureToggle.gameObject.SetActive(true);
		this._guardianManagedEnabled.SetActive(false);
		this._guardianManagedLocked.SetActive(false);
		this._featureToggle.interactable = isInteractable;
		this._featureToggle.SetValue(isOptedIn);
	}

	// Token: 0x06004B6A RID: 19306 RVA: 0x00192AD0 File Offset: 0x00190CD0
	private void SetFeatureName()
	{
		string text = (this.GetFeatureToggleState() ? ("<b>(" + this._enabledTextStr + ")</b>") : ("<b>(" + this._disabledTextStr + ")</b>"));
		this._featureNameTxt.text = "<b>" + this._featureName + "</b>";
		this._featureStatusTxt.text = text ?? "";
	}

	// Token: 0x06004B6B RID: 19307 RVA: 0x00192B47 File Offset: 0x00190D47
	private void SetupGuardianManagedClickHandlers()
	{
		this.AddDeniedSoundHandler(this._guardianManagedEnabled);
		this.AddDeniedSoundHandler(this._guardianManagedLocked);
	}

	// Token: 0x06004B6C RID: 19308 RVA: 0x00192B64 File Offset: 0x00190D64
	private void AddDeniedSoundHandler(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		EventTrigger component = obj.GetComponent<EventTrigger>();
		if (component != null)
		{
			Object.DestroyImmediate(component);
		}
		EventTrigger eventTrigger = obj.AddComponent<EventTrigger>();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback.AddListener(delegate(BaseEventData data)
		{
			Debug.Log("[KIDUIFeatureSetting] Guardian-managed feature clicked - playing denied sound");
			KIDAudioManager instance = KIDAudioManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.PlaySound(KIDAudioManager.KIDSoundType.Denied);
		});
		eventTrigger.triggers.Add(entry);
		this.EnsureRaycastTarget(obj);
	}

	// Token: 0x06004B6D RID: 19309 RVA: 0x00192BE0 File Offset: 0x00190DE0
	private void EnsureRaycastTarget(GameObject obj)
	{
		Graphic component = obj.GetComponent<Graphic>();
		if (component != null)
		{
			component.raycastTarget = true;
			return;
		}
		Image image = obj.GetComponent<Image>();
		if (image == null)
		{
			image = obj.AddComponent<Image>();
		}
		image.color = new Color(0f, 0f, 0f, 0f);
		image.raycastTarget = true;
	}

	// Token: 0x04005E47 RID: 24135
	[SerializeField]
	private TMP_Text _featureNameTxt;

	// Token: 0x04005E48 RID: 24136
	[SerializeField]
	private TMP_Text _featureStatusTxt;

	// Token: 0x04005E49 RID: 24137
	[SerializeField]
	private KIDUIToggle _featureToggle;

	// Token: 0x04005E4A RID: 24138
	[SerializeField]
	private GameObject _tickIcon;

	// Token: 0x04005E4B RID: 24139
	[SerializeField]
	private GameObject _crossIcon;

	// Token: 0x04005E4C RID: 24140
	[SerializeField]
	private GameObject _guardianManagedLocked;

	// Token: 0x04005E4D RID: 24141
	[SerializeField]
	private GameObject _guardianManagedEnabled;

	// Token: 0x04005E4E RID: 24142
	private bool _hasToggle;

	// Token: 0x04005E4F RID: 24143
	private string _featureName;

	// Token: 0x04005E50 RID: 24144
	private string _permissionName;

	// Token: 0x04005E51 RID: 24145
	private string _enabledTextStr;

	// Token: 0x04005E52 RID: 24146
	private string _disabledTextStr;

	// Token: 0x04005E53 RID: 24147
	private EKIDFeatures _featureType;

	// Token: 0x04005E54 RID: 24148
	private Action<EKIDFeatures> _onChangeCallback;

	// Token: 0x04005E55 RID: 24149
	private KIDUI_MainScreen.FeatureToggleSetup _feature;
}
