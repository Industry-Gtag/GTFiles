using System;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020000B1 RID: 177
public class DevWatch : MonoBehaviour
{
	// Token: 0x0600043F RID: 1087 RVA: 0x00018E68 File Offset: 0x00017068
	private void Awake()
	{
		this.SearchButton.SearchEvent.AddListener(new UnityAction(this.SearchItems));
		this.TakeOwnershipButton.onClick.AddListener(new UnityAction(this.TakeOwneshipOfItem));
		this.DestroyObjectButton.onClick.AddListener(new UnityAction(this.TryDestroyItem));
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x00018ECC File Offset: 0x000170CC
	public void SearchItems()
	{
		this.FoundNetworkObjects.Clear();
		RaycastHit[] array = Physics.SphereCastAll(new Ray(this.RayCastStartPos.position, this.RayCastDirection.position - this.RayCastStartPos.position), 0.3f, 100f);
		if (array.Length != 0)
		{
			foreach (RaycastHit raycastHit in array)
			{
				NetworkObject networkObject;
				if (raycastHit.collider.gameObject.TryGetComponent<NetworkObject>(out networkObject))
				{
					this.FoundNetworkObjects.Add(networkObject);
				}
			}
		}
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x00018F60 File Offset: 0x00017160
	public void Cleanup()
	{
		this.FoundNetworkObjects.Clear();
		if (this.Items.Count > 0)
		{
			for (int i = this.Items.Count - 1; i >= 0; i--)
			{
				Object.Destroy(this.Items[i]);
			}
		}
		this.Items.Clear();
		this.Panel1.SetActive(true);
		this.Panel2.SetActive(false);
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x00018FD2 File Offset: 0x000171D2
	public void ItemSelected(DevWatchSelectableItem item)
	{
		this.Panel1.SetActive(false);
		this.Panel2.SetActive(true);
		this.SelectedItem = item;
		this.SelectedItemName.text = item.ItemName.text;
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void TryDestroyItem()
	{
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void TakeOwneshipOfItem()
	{
	}

	// Token: 0x040004A6 RID: 1190
	public DevWatchButton SearchButton;

	// Token: 0x040004A7 RID: 1191
	public GameObject Panel1;

	// Token: 0x040004A8 RID: 1192
	public GameObject Panel2;

	// Token: 0x040004A9 RID: 1193
	public DevWatchSelectableItem SelectableItemPrefab;

	// Token: 0x040004AA RID: 1194
	public List<DevWatchSelectableItem> Items;

	// Token: 0x040004AB RID: 1195
	public Transform RayCastStartPos;

	// Token: 0x040004AC RID: 1196
	public Transform RayCastDirection;

	// Token: 0x040004AD RID: 1197
	public Transform ItemsFoundContainer;

	// Token: 0x040004AE RID: 1198
	public Button TakeOwnershipButton;

	// Token: 0x040004AF RID: 1199
	public Button DestroyObjectButton;

	// Token: 0x040004B0 RID: 1200
	public List<NetworkObject> FoundNetworkObjects = new List<NetworkObject>();

	// Token: 0x040004B1 RID: 1201
	public TextMeshProUGUI SelectedItemName;

	// Token: 0x040004B2 RID: 1202
	public DevWatchSelectableItem SelectedItem;
}
