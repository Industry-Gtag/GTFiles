using System;
using UnityEngine;

// Token: 0x0200002D RID: 45
public class BootscreenPositioner : MonoBehaviour
{
	// Token: 0x060000A4 RID: 164 RVA: 0x000054FC File Offset: 0x000036FC
	private void Awake()
	{
		base.transform.position = this._pov.position;
		base.transform.rotation = Quaternion.Euler(0f, this._pov.rotation.eulerAngles.y, 0f);
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x00005554 File Offset: 0x00003754
	private void LateUpdate()
	{
		if (Vector3.Distance(base.transform.position, this._pov.position) > this._distanceThreshold)
		{
			base.transform.position = this._pov.position;
		}
		if (Mathf.Abs(this._pov.rotation.eulerAngles.y - base.transform.rotation.eulerAngles.y) > this._rotationThreshold)
		{
			base.transform.rotation = Quaternion.Euler(0f, this._pov.rotation.eulerAngles.y, 0f);
		}
	}

	// Token: 0x040000C6 RID: 198
	[SerializeField]
	private Transform _pov;

	// Token: 0x040000C7 RID: 199
	[SerializeField]
	private float _distanceThreshold;

	// Token: 0x040000C8 RID: 200
	[SerializeField]
	private float _rotationThreshold;
}
