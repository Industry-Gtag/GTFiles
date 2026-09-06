using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200032B RID: 811
public class DevInspectorScanner : MonoBehaviour
{
	// Token: 0x040018FD RID: 6397
	public Text hintTextOutput;

	// Token: 0x040018FE RID: 6398
	public float scanDistance = 10f;

	// Token: 0x040018FF RID: 6399
	public float scanAngle = 30f;

	// Token: 0x04001900 RID: 6400
	public LayerMask scanLayerMask;

	// Token: 0x04001901 RID: 6401
	public string targetComponentName;

	// Token: 0x04001902 RID: 6402
	public float rayPerDegree = 10f;
}
