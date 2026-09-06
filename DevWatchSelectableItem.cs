using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000B3 RID: 179
public class DevWatchSelectableItem : MonoBehaviour
{
	// Token: 0x06000448 RID: 1096 RVA: 0x0001903C File Offset: 0x0001723C
	public void Init(NetworkObject obj)
	{
		this.SelectedObject = obj;
		this.ItemName.text = obj.name;
		this.Button.onClick.AddListener(delegate
		{
			this.OnSelected(this.ItemName.text, this.SelectedObject);
		});
	}

	// Token: 0x040004B4 RID: 1204
	public Button Button;

	// Token: 0x040004B5 RID: 1205
	public TextMeshProUGUI ItemName;

	// Token: 0x040004B6 RID: 1206
	public NetworkObject SelectedObject;

	// Token: 0x040004B7 RID: 1207
	public Action<string, NetworkObject> OnSelected;
}
