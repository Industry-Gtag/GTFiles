using System;
using System.Collections;
using GorillaTagScripts.VirtualStumpCustomMaps;
using UnityEngine;

// Token: 0x02000A51 RID: 2641
public class CustomMapsLoadRoomMapButton : GorillaPressableButton
{
	// Token: 0x060043C9 RID: 17353 RVA: 0x00168EE3 File Offset: 0x001670E3
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		base.StartCoroutine(this.ButtonPressed_Local());
		if (CustomMapManager.CanLoadRoomMap())
		{
			CustomMapManager.ApproveAndLoadRoomMap();
		}
	}

	// Token: 0x060043CA RID: 17354 RVA: 0x00168F04 File Offset: 0x00167104
	private IEnumerator ButtonPressed_Local()
	{
		this.isOn = true;
		this.UpdateColor();
		yield return new WaitForSeconds(this.pressedTime);
		this.isOn = false;
		this.UpdateColor();
		yield break;
	}

	// Token: 0x040055C4 RID: 21956
	[SerializeField]
	private float pressedTime = 0.2f;
}
