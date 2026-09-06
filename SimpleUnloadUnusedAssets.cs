using System;
using System.Collections;
using UnityEngine;

// Token: 0x020009EB RID: 2539
public class SimpleUnloadUnusedAssets : MonoBehaviour
{
	// Token: 0x06004135 RID: 16693 RVA: 0x0015BA64 File Offset: 0x00159C64
	private void OnEnable()
	{
		base.StartCoroutine(this.UnloadUnusedAssets());
	}

	// Token: 0x06004136 RID: 16694 RVA: 0x0015BA73 File Offset: 0x00159C73
	private IEnumerator UnloadUnusedAssets()
	{
		yield return new WaitForSeconds(this.WaitForUnload);
		Debug.Log(string.Format("SimpleUnloadUnusedAssets: Forcing unload unused assets after waiting {0} seconds!", this.WaitForUnload));
		Resources.UnloadUnusedAssets();
		yield break;
	}

	// Token: 0x040051E3 RID: 20963
	public float WaitForUnload = 5f;
}
