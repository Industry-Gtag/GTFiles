using System;
using UnityEngine;

// Token: 0x02000B61 RID: 2913
public class KIDHandReference : MonoBehaviour
{
	// Token: 0x17000717 RID: 1815
	// (get) Token: 0x060049FE RID: 18942 RVA: 0x0018A485 File Offset: 0x00188685
	public static GameObject LeftHand
	{
		get
		{
			return KIDHandReference._leftHandRef;
		}
	}

	// Token: 0x17000718 RID: 1816
	// (get) Token: 0x060049FF RID: 18943 RVA: 0x0018A48C File Offset: 0x0018868C
	public static GameObject RightHand
	{
		get
		{
			return KIDHandReference._rightHandRef;
		}
	}

	// Token: 0x06004A00 RID: 18944 RVA: 0x0018A493 File Offset: 0x00188693
	private void Awake()
	{
		KIDHandReference._leftHandRef = this._leftHand;
		KIDHandReference._rightHandRef = this._rightHand;
	}

	// Token: 0x04005C87 RID: 23687
	[SerializeField]
	private GameObject _leftHand;

	// Token: 0x04005C88 RID: 23688
	[SerializeField]
	private GameObject _rightHand;

	// Token: 0x04005C89 RID: 23689
	private static GameObject _leftHandRef;

	// Token: 0x04005C8A RID: 23690
	private static GameObject _rightHandRef;
}
