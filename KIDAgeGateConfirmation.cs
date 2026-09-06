using System;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

// Token: 0x02000B5D RID: 2909
public class KIDAgeGateConfirmation : MonoBehaviour
{
	// Token: 0x17000712 RID: 1810
	// (get) Token: 0x060049E3 RID: 18915 RVA: 0x00189FDC File Offset: 0x001881DC
	private IntVariable UserAgeVar
	{
		get
		{
			if (this._userAgeVar == null)
			{
				this._userAgeVar = this._localizedTextBody.StringReference["user-age"] as IntVariable;
				if (this._userAgeVar == null)
				{
					Debug.LogError("[Localization::KID_AGE_GATE_CONFIRMATION] Failed to get [user-age] smart variable as IntVariable");
				}
			}
			return this._userAgeVar;
		}
	}

	// Token: 0x17000713 RID: 1811
	// (get) Token: 0x060049E4 RID: 18916 RVA: 0x0018A029 File Offset: 0x00188229
	// (set) Token: 0x060049E5 RID: 18917 RVA: 0x0018A031 File Offset: 0x00188231
	public KidAgeConfirmationResult Result { get; private set; }

	// Token: 0x060049E6 RID: 18918 RVA: 0x0018A03A File Offset: 0x0018823A
	private void Start()
	{
		this.Result = KidAgeConfirmationResult.None;
	}

	// Token: 0x060049E7 RID: 18919 RVA: 0x0018A043 File Offset: 0x00188243
	public void OnConfirm()
	{
		this.Result = KidAgeConfirmationResult.Confirm;
	}

	// Token: 0x060049E8 RID: 18920 RVA: 0x0018A04C File Offset: 0x0018824C
	public void OnBack()
	{
		this.Result = KidAgeConfirmationResult.Back;
	}

	// Token: 0x060049E9 RID: 18921 RVA: 0x0018A055 File Offset: 0x00188255
	public void Reset(int userAge)
	{
		this.Result = KidAgeConfirmationResult.None;
		if (this.UserAgeVar == null)
		{
			Debug.LogError("[LOCALIZATION::KID_AGE_GATE_CONFIRMATION] Unable to update [UserAgeVar] value, as it is null");
			return;
		}
		this.UserAgeVar.Value = userAge;
	}

	// Token: 0x04005C5F RID: 23647
	[Header("Localization")]
	[SerializeField]
	private LocalizedText _localizedTextBody;

	// Token: 0x04005C60 RID: 23648
	private IntVariable _userAgeVar;
}
