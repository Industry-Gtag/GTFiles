using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000830 RID: 2096
public class GRUIBuyItem : MonoBehaviour
{
	// Token: 0x060035DA RID: 13786 RVA: 0x00129806 File Offset: 0x00127A06
	public void Setup(int standId)
	{
		this.standId = standId;
		this.buyItemButton.onPressButton.AddListener(new UnityAction(this.OnBuyItem));
		this.entityTypeId = this.entityPrefab.gameObject.name.GetStaticHash();
	}

	// Token: 0x060035DB RID: 13787 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnBuyItem()
	{
	}

	// Token: 0x060035DC RID: 13788 RVA: 0x00129846 File Offset: 0x00127A46
	public Transform GetSpawnMarker()
	{
		return this.spawnMarker;
	}

	// Token: 0x04004673 RID: 18035
	[SerializeField]
	private GorillaPressableButton buyItemButton;

	// Token: 0x04004674 RID: 18036
	[SerializeField]
	private Text itemInfoLabel;

	// Token: 0x04004675 RID: 18037
	[SerializeField]
	private Transform spawnMarker;

	// Token: 0x04004676 RID: 18038
	[SerializeField]
	private GameEntity entityPrefab;

	// Token: 0x04004677 RID: 18039
	private int entityTypeId;

	// Token: 0x04004678 RID: 18040
	private int standId;
}
