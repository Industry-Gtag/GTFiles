using System;
using System.Collections.Generic;
using GorillaNetworking;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000832 RID: 2098
public class GRUIEmployeeTerminal : MonoBehaviour
{
	// Token: 0x060035E9 RID: 13801 RVA: 0x001299B4 File Offset: 0x00127BB4
	public void Setup()
	{
		this.signupButton.onPressButton.AddListener(new UnityAction(this.OnSignup));
		global::PlayFab.ClientModels.GetUserDataRequest getUserDataRequest = new global::PlayFab.ClientModels.GetUserDataRequest();
		getUserDataRequest.PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		getUserDataRequest.Keys = new List<string> { "GRData" };
		this.isSigningUp = true;
		PlayFabClientAPI.GetUserData(getUserDataRequest, new Action<GetUserDataResult>(this.OnGetUserDataInitialState), new Action<PlayFabError>(this.OnGetUserDataInitialStateFail), null, null);
		this.Refresh();
	}

	// Token: 0x060035EA RID: 13802 RVA: 0x00129A38 File Offset: 0x00127C38
	public void OnSignup()
	{
		if (this.isSigningUp || this.isEmployee)
		{
			return;
		}
		UpdateUserDataRequest updateUserDataRequest = new UpdateUserDataRequest
		{
			Data = new Dictionary<string, string> { { "GRData", "Now we have data" } }
		};
		if (!PlayFabClientAPI.IsClientLoggedIn())
		{
			if (PlayFabAuthenticator.instance != null)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
			}
			return;
		}
		this.isSigningUp = true;
		PlayFabClientAPI.UpdateUserData(updateUserDataRequest, new Action<UpdateUserDataResult>(this.OnSaveTableSuccess), new Action<PlayFabError>(this.OnSaveTableFailure), null, null);
	}

	// Token: 0x060035EB RID: 13803 RVA: 0x00129AC1 File Offset: 0x00127CC1
	public Transform GetSpawnMarker()
	{
		return this.spawnMarker;
	}

	// Token: 0x060035EC RID: 13804 RVA: 0x00129ACC File Offset: 0x00127CCC
	public void Refresh()
	{
		if (this.isSigningUp)
		{
			this.signupButtonText.text = "APPLYING";
			return;
		}
		if (this.isEmployee)
		{
			this.signupButtonText.text = "HIRED";
			return;
		}
		this.signupButtonText.text = "APPLY";
	}

	// Token: 0x060035ED RID: 13805 RVA: 0x00129B1C File Offset: 0x00127D1C
	private void OnGetUserDataInitialState(GetUserDataResult result)
	{
		UserDataRecord userDataRecord;
		if (result.Data.TryGetValue("GRData", out userDataRecord))
		{
			string value = userDataRecord.Value;
			this.isEmployee = true;
		}
		else
		{
			this.isEmployee = false;
		}
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x060035EE RID: 13806 RVA: 0x00129B61 File Offset: 0x00127D61
	private void OnGetUserDataInitialStateFail(PlayFabError error)
	{
		this.isEmployee = false;
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x060035EF RID: 13807 RVA: 0x00129B77 File Offset: 0x00127D77
	private void OnSaveTableSuccess(UpdateUserDataResult result)
	{
		this.isEmployee = true;
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x060035F0 RID: 13808 RVA: 0x00129B61 File Offset: 0x00127D61
	private void OnSaveTableFailure(PlayFabError error)
	{
		this.isEmployee = false;
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x04004686 RID: 18054
	[SerializeField]
	private GorillaPressableButton signupButton;

	// Token: 0x04004687 RID: 18055
	[SerializeField]
	private TMP_Text signupButtonText;

	// Token: 0x04004688 RID: 18056
	[SerializeField]
	private Transform spawnMarker;

	// Token: 0x04004689 RID: 18057
	[SerializeField]
	private GRUIStationEmployeeBadges badgeStation;

	// Token: 0x0400468A RID: 18058
	private int entityTypeId;

	// Token: 0x0400468B RID: 18059
	private bool isEmployee;

	// Token: 0x0400468C RID: 18060
	private bool isSigningUp;

	// Token: 0x0400468D RID: 18061
	private const string GR_DATA_KEY = "GRData";
}
