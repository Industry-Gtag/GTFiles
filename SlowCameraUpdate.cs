using System;
using System.Collections;
using UnityEngine;

// Token: 0x020009EE RID: 2542
public class SlowCameraUpdate : MonoBehaviour
{
	// Token: 0x06004142 RID: 16706 RVA: 0x0015BB19 File Offset: 0x00159D19
	public void Awake()
	{
		this.frameRate = 30f;
		this.timeToNextFrame = 1f / this.frameRate;
		this.myCamera = base.GetComponent<Camera>();
	}

	// Token: 0x06004143 RID: 16707 RVA: 0x0015BB44 File Offset: 0x00159D44
	public void OnEnable()
	{
		base.StartCoroutine(this.UpdateMirror());
	}

	// Token: 0x06004144 RID: 16708 RVA: 0x00005879 File Offset: 0x00003A79
	public void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06004145 RID: 16709 RVA: 0x0015BB53 File Offset: 0x00159D53
	public IEnumerator UpdateMirror()
	{
		for (;;)
		{
			if (base.gameObject.activeSelf)
			{
				Debug.Log("rendering camera!");
				this.myCamera.Render();
			}
			yield return new WaitForSeconds(this.timeToNextFrame);
		}
		yield break;
	}

	// Token: 0x040051E7 RID: 20967
	private Camera myCamera;

	// Token: 0x040051E8 RID: 20968
	private float frameRate;

	// Token: 0x040051E9 RID: 20969
	private float timeToNextFrame;
}
