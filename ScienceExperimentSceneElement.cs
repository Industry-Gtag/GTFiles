using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020009E3 RID: 2531
public class ScienceExperimentSceneElement : MonoBehaviour, ITickSystemPost
{
	// Token: 0x17000620 RID: 1568
	// (get) Token: 0x060040FF RID: 16639 RVA: 0x0015AB00 File Offset: 0x00158D00
	// (set) Token: 0x06004100 RID: 16640 RVA: 0x0015AB08 File Offset: 0x00158D08
	bool ITickSystemPost.PostTickRunning { get; set; }

	// Token: 0x06004101 RID: 16641 RVA: 0x0015AB14 File Offset: 0x00158D14
	void ITickSystemPost.PostTick()
	{
		base.transform.position = this.followElement.position;
		base.transform.rotation = this.followElement.rotation;
		base.transform.localScale = this.followElement.localScale;
	}

	// Token: 0x06004102 RID: 16642 RVA: 0x0015AB63 File Offset: 0x00158D63
	private void Start()
	{
		this.followElement = ScienceExperimentManager.instance.GetElement(this.elementID);
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06004103 RID: 16643 RVA: 0x0015AB83 File Offset: 0x00158D83
	private void OnDestroy()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x0400519B RID: 20891
	public ScienceExperimentElementID elementID;

	// Token: 0x0400519C RID: 20892
	private Transform followElement;
}
