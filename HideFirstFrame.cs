using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200002E RID: 46
public class HideFirstFrame : MonoBehaviour
{
	// Token: 0x060000A7 RID: 167 RVA: 0x0000560A File Offset: 0x0000380A
	private void Awake()
	{
		this._cam = base.GetComponent<Camera>();
		this._farClipPlane = this._cam.farClipPlane;
		this._cam.farClipPlane = this._cam.nearClipPlane + 0.1f;
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x00005645 File Offset: 0x00003845
	public IEnumerator Start()
	{
		int num;
		for (int i = 0; i < this._frameDelay; i = num + 1)
		{
			yield return null;
			num = i;
		}
		this._cam.farClipPlane = this._farClipPlane;
		yield break;
	}

	// Token: 0x040000C9 RID: 201
	[SerializeField]
	private int _frameDelay = 1;

	// Token: 0x040000CA RID: 202
	private Camera _cam;

	// Token: 0x040000CB RID: 203
	private float _farClipPlane;
}
