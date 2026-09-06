using System;
using System.Collections;
using GorillaTag.CosmeticSystem;
using TMPro;
using UnityEngine;

// Token: 0x02000276 RID: 630
public class PropHuntDebugHelper : MonoBehaviour
{
	// Token: 0x0600110F RID: 4367 RVA: 0x0005B738 File Offset: 0x00059938
	protected void Awake()
	{
		if (PropHuntDebugHelper.instance != null)
		{
			Object.Destroy(this);
			return;
		}
		PropHuntDebugHelper.instance = this;
	}

	// Token: 0x06001110 RID: 4368 RVA: 0x0005B754 File Offset: 0x00059954
	private IEnumerator Start()
	{
		yield return null;
		yield return null;
		this._propHuntManager = Object.FindAnyObjectByType<GorillaPropHuntGameManager>();
		if (this._propHuntManager != null)
		{
			Debug.Log("PropHuntDebugHelper :: Found number of props " + PropHuntPools.AllPropCosmeticIds.Length.ToString());
			this._cachedAllPropIDs = PropHuntPools.AllPropCosmeticIds;
			this._localPropHuntHandFollower = VRRig.LocalRig.GetComponent<PropHuntHandFollower>();
			this.UpdatePropsText();
		}
		yield break;
	}

	// Token: 0x06001111 RID: 4369 RVA: 0x0005B764 File Offset: 0x00059964
	public void UpdatePropsText()
	{
		string selectedPropID = this.GetSelectedPropID(this._selectedPropIndex);
		string text = string.Empty;
		if (this._selectedPropIndex != -1)
		{
			CosmeticSO cosmeticSO = this._allCosmetics.SearchForCosmeticSO(selectedPropID);
			if (cosmeticSO != null)
			{
				text = cosmeticSO.info.displayName;
			}
		}
		this._propsText.text = "Current Prop: " + this.GetCurrentPropInfo() + "\n" + string.Format("Selected Prop: {0} - {1} ({2}/{3})", new object[]
		{
			selectedPropID,
			text,
			this._selectedPropIndex,
			this._cachedAllPropIDs.Length
		});
	}

	// Token: 0x06001112 RID: 4370 RVA: 0x0005B805 File Offset: 0x00059A05
	private string GetCurrentPropInfo()
	{
		return string.Empty;
	}

	// Token: 0x06001113 RID: 4371 RVA: 0x0005B80C File Offset: 0x00059A0C
	private string GetSelectedPropID(int index)
	{
		if (index <= -1)
		{
			return "None";
		}
		return this._cachedAllPropIDs[index];
	}

	// Token: 0x06001114 RID: 4372 RVA: 0x0005B820 File Offset: 0x00059A20
	[ContextMenu("Prev Prop")]
	public void PrevProp()
	{
		this._selectedPropIndex--;
		if (this._selectedPropIndex < -1)
		{
			this._selectedPropIndex = this._cachedAllPropIDs.Length - 1;
		}
		string text = ((this._selectedPropIndex > -1) ? this.GetSelectedPropID(this._selectedPropIndex) : string.Empty);
		this.SendForcePropHandRPC(text);
		this.UpdatePropsText();
	}

	// Token: 0x06001115 RID: 4373 RVA: 0x0005B880 File Offset: 0x00059A80
	[ContextMenu("Next Prop")]
	public void NextProp()
	{
		this._selectedPropIndex++;
		if (this._selectedPropIndex >= this._cachedAllPropIDs.Length)
		{
			this._selectedPropIndex = -1;
		}
		string text = ((this._selectedPropIndex > -1) ? this.GetSelectedPropID(this._selectedPropIndex) : string.Empty);
		this.SendForcePropHandRPC(text);
		this.UpdatePropsText();
	}

	// Token: 0x06001116 RID: 4374 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void SendForcePropHandRPC(string newPropId)
	{
	}

	// Token: 0x06001117 RID: 4375 RVA: 0x00002C2D File Offset: 0x00000E2D
	[ContextMenu("Toggle Round")]
	public void ToggleRound()
	{
	}

	// Token: 0x0400144D RID: 5197
	[OnEnterPlay_SetNull]
	public static PropHuntDebugHelper instance;

	// Token: 0x0400144E RID: 5198
	[SerializeField]
	private GorillaPropHuntGameManager _propHuntManager;

	// Token: 0x0400144F RID: 5199
	[SerializeField]
	private PropHuntHandFollower _localPropHuntHandFollower;

	// Token: 0x04001450 RID: 5200
	[SerializeField]
	private TextMeshPro _propsText;

	// Token: 0x04001451 RID: 5201
	[SerializeField]
	private AllCosmeticsArraySO _allCosmetics;

	// Token: 0x04001452 RID: 5202
	private string[] _cachedAllPropIDs;

	// Token: 0x04001453 RID: 5203
	private int _selectedPropIndex = -1;
}
