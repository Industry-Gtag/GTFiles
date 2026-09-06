using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000329 RID: 809
public class DevInspector : MonoBehaviour
{
	// Token: 0x06001415 RID: 5141 RVA: 0x0006CBFF File Offset: 0x0006ADFF
	private void OnEnable()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x040018F5 RID: 6389
	public GameObject pivot;

	// Token: 0x040018F6 RID: 6390
	public Text outputInfo;

	// Token: 0x040018F7 RID: 6391
	public Component[] componentToInspect;

	// Token: 0x040018F8 RID: 6392
	public bool isEnabled;

	// Token: 0x040018F9 RID: 6393
	public bool autoFind = true;

	// Token: 0x040018FA RID: 6394
	public GameObject canvas;

	// Token: 0x040018FB RID: 6395
	public int sidewaysOffset;
}
