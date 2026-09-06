using System;
using UnityEngine;

// Token: 0x020001AB RID: 427
public class Monkeye_LazerFX : MonoBehaviour
{
	// Token: 0x06000B99 RID: 2969 RVA: 0x0003E72C File Offset: 0x0003C92C
	private void Awake()
	{
		base.enabled = false;
		foreach (LineRenderer lineRenderer in this.lines)
		{
			lineRenderer.positionCount = 2;
			lineRenderer.enabled = false;
		}
		if (this.targetFx != null)
		{
			this.targetFx.SetActive(false);
		}
	}

	// Token: 0x06000B9A RID: 2970 RVA: 0x0003E780 File Offset: 0x0003C980
	public void EnableLazer(Transform[] eyes_, VRRig rig_, float maxDist = 10000f)
	{
		if (rig_ == this.targetRig)
		{
			return;
		}
		this.eyeBones = eyes_;
		this.targetRig = rig_;
		this.targetPos = this.targetRig.transform.position;
		base.enabled = true;
		LineRenderer[] array = this.lines;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		if (this.targetFx != null)
		{
			this.targetFx.transform.position = this.targetPos;
			this.targetFx.SetActive(true);
		}
	}

	// Token: 0x06000B9B RID: 2971 RVA: 0x0003E818 File Offset: 0x0003CA18
	public void EnableLazer(Transform[] eyes_, Vector3 targetPos_)
	{
		this.eyeBones = eyes_;
		this.targetRig = null;
		this.targetPos = targetPos_;
		base.enabled = true;
		LineRenderer[] array = this.lines;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		if (this.targetFx != null)
		{
			this.targetFx.transform.position = this.targetPos;
			this.targetFx.SetActive(true);
		}
	}

	// Token: 0x06000B9C RID: 2972 RVA: 0x0003E890 File Offset: 0x0003CA90
	public void DisableLazer()
	{
		this.targetRig = null;
		if (base.enabled)
		{
			base.enabled = false;
			LineRenderer[] array = this.lines;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			if (this.targetFx != null)
			{
				this.targetFx.SetActive(false);
			}
		}
	}

	// Token: 0x06000B9D RID: 2973 RVA: 0x0003E8EC File Offset: 0x0003CAEC
	private void Update()
	{
		if (this.targetRig != null)
		{
			this.targetPos = this.targetRig.transform.position;
		}
		for (int i = 0; i < this.lines.Length; i++)
		{
			this.lines[i].SetPosition(0, this.eyeBones[i].transform.position);
			this.lines[i].SetPosition(1, this.targetPos);
		}
		if (this.targetFx != null)
		{
			this.targetFx.transform.position = this.targetPos;
		}
	}

	// Token: 0x04000E02 RID: 3586
	private Transform[] eyeBones;

	// Token: 0x04000E03 RID: 3587
	private VRRig targetRig;

	// Token: 0x04000E04 RID: 3588
	private Vector3 targetPos;

	// Token: 0x04000E05 RID: 3589
	public LineRenderer[] lines;

	// Token: 0x04000E06 RID: 3590
	public GameObject targetFx;
}
