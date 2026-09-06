using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

// Token: 0x02000BBD RID: 3005
public class KIDUI_AgeDiscrepancyScreen : MonoBehaviour
{
	// Token: 0x06004BDD RID: 19421 RVA: 0x00194703 File Offset: 0x00192903
	private void Awake()
	{
		this.CheckLocalizationReferences();
	}

	// Token: 0x06004BDE RID: 19422 RVA: 0x0019470C File Offset: 0x0019290C
	public async Task ShowAgeDiscrepancyScreenWithAwait(string description)
	{
		base.gameObject.SetActive(true);
		this.CheckLocalizationReferences();
		this._descriptionText.text = description;
		await this.WaitForCompletion();
	}

	// Token: 0x06004BDF RID: 19423 RVA: 0x00194758 File Offset: 0x00192958
	public async Task ShowAgeDiscrepancyScreenWithAwait(int userAge, int accAge, int lowestAge)
	{
		base.gameObject.SetActive(true);
		this.CheckLocalizationReferences();
		this._userAgeVar.Value = userAge;
		this._accountAgeVar.Value = accAge;
		this._lowestAgeVar.Value = lowestAge;
		await this.WaitForCompletion();
	}

	// Token: 0x06004BE0 RID: 19424 RVA: 0x001947B4 File Offset: 0x001929B4
	private async Task WaitForCompletion()
	{
		do
		{
			await Task.Yield();
		}
		while (!this._hasCompleted);
	}

	// Token: 0x06004BE1 RID: 19425 RVA: 0x001947F7 File Offset: 0x001929F7
	public void OnHoldComplete()
	{
		this._hasCompleted = true;
	}

	// Token: 0x06004BE2 RID: 19426 RVA: 0x00193E22 File Offset: 0x00192022
	public void OnQuitPressed()
	{
		Application.Quit();
	}

	// Token: 0x06004BE3 RID: 19427 RVA: 0x00194800 File Offset: 0x00192A00
	private void CheckLocalizationReferences()
	{
		if (this._bodyLocStr != null && this._userAgeVar != null && this._accountAgeVar != null && this._lowestAgeVar != null)
		{
			return;
		}
		if (this._bodyTextLoc == null)
		{
			Debug.LogError("[LOCALIZATION::KIDUI_AGE_DISCREPANCY_SCREEN] [_bodyTextLoc] is not set, unable to localize smart string");
			return;
		}
		this._bodyLocStr = this._bodyTextLoc.StringReference;
		this._userAgeVar = this._bodyLocStr["user-age"] as IntVariable;
		this._accountAgeVar = this._bodyLocStr["account-age"] as IntVariable;
		this._lowestAgeVar = this._bodyLocStr["lowest-age"] as IntVariable;
	}

	// Token: 0x04005EBA RID: 24250
	[SerializeField]
	private TMP_Text _descriptionText;

	// Token: 0x04005EBB RID: 24251
	[Header("Localization")]
	[SerializeField]
	private LocalizedText _bodyTextLoc;

	// Token: 0x04005EBC RID: 24252
	private bool _hasCompleted;

	// Token: 0x04005EBD RID: 24253
	private LocalizedString _bodyLocStr;

	// Token: 0x04005EBE RID: 24254
	private IntVariable _userAgeVar;

	// Token: 0x04005EBF RID: 24255
	private IntVariable _accountAgeVar;

	// Token: 0x04005EC0 RID: 24256
	private IntVariable _lowestAgeVar;
}
