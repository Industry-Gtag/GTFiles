using System;
using System.Collections.Generic;
using KID.Model;
using TMPro;
using UnityEngine;

// Token: 0x02000B51 RID: 2897
public class KIDAgeAppeal : MonoBehaviour
{
	// Token: 0x060049B6 RID: 18870 RVA: 0x00188F00 File Offset: 0x00187100
	public void ShowAgeAppealScreen()
	{
		this._ageSlider = base.GetComponentInChildren<AgeSliderWithProgressBar>(true);
		this._ageSlider.ControllerActive = true;
		base.gameObject.SetActive(true);
		this._inputsContainer.SetActive(true);
		this._monkeLoader.SetActive(false);
	}

	// Token: 0x060049B7 RID: 18871 RVA: 0x00188F40 File Offset: 0x00187140
	public async void OnNewAgeConfirmed()
	{
		this._inputsContainer.SetActive(false);
		this._monkeLoader.SetActive(true);
		AgeStatusType ageStatusType;
		if (KIDManager.TryGetAgeStatusTypeFromAge(this._ageSlider.CurrentAge, out ageStatusType))
		{
			TelemetryData telemetryData = new TelemetryData
			{
				EventName = "kid_age_appeal_age_gate",
				CustomTags = new string[]
				{
					"kid_age_appeal",
					KIDTelemetry.GameVersionCustomTag,
					KIDTelemetry.GameEnvironment
				},
				BodyData = new Dictionary<string, string> { 
				{
					"correct_age",
					ageStatusType.ToString()
				} }
			};
			GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		}
		AttemptAgeUpdateData attemptAgeUpdateData = await KIDManager.TryAttemptAgeUpdate(this._ageSlider.CurrentAge);
		if (attemptAgeUpdateData.status == SessionStatus.PROHIBITED)
		{
			Debug.LogError("[KID::AGE-APPEAL] Age Appeal Status: PROHIBITED");
			base.gameObject.SetActive(false);
			KIDUI_AgeAppealController.Instance.StartTooYoungToPlayScreen();
		}
		else
		{
			this._ageAppealEmailScreen.ShowAgeAppealEmailScreen(attemptAgeUpdateData.status == SessionStatus.CHALLENGE, this._ageSlider.CurrentAge);
			this._ageSlider.ControllerActive = false;
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x04005C1B RID: 23579
	[SerializeField]
	private TMP_Text _ageText;

	// Token: 0x04005C1C RID: 23580
	[SerializeField]
	private KIDUI_AgeAppealEmailScreen _ageAppealEmailScreen;

	// Token: 0x04005C1D RID: 23581
	[SerializeField]
	private GameObject _inputsContainer;

	// Token: 0x04005C1E RID: 23582
	[SerializeField]
	private GameObject _monkeLoader;

	// Token: 0x04005C1F RID: 23583
	private AgeSliderWithProgressBar _ageSlider;
}
