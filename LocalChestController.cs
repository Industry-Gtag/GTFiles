using System;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x0200035C RID: 860
public class LocalChestController : MonoBehaviour
{
	// Token: 0x06001512 RID: 5394 RVA: 0x00070620 File Offset: 0x0006E820
	private void OnTriggerEnter(Collider other)
	{
		if (this.isOpen)
		{
			return;
		}
		TransformFollow component = other.GetComponent<TransformFollow>();
		if (component == null)
		{
			return;
		}
		Transform transformToFollow = component.transformToFollow;
		if (transformToFollow == null)
		{
			return;
		}
		VRRig componentInParent = transformToFollow.GetComponentInParent<VRRig>();
		if (componentInParent == null)
		{
			return;
		}
		if (this.playerCollectionVolume != null && !this.playerCollectionVolume.containedRigs.Contains(componentInParent))
		{
			return;
		}
		this.isOpen = true;
		this.director.Play();
	}

	// Token: 0x040019F5 RID: 6645
	public PlayableDirector director;

	// Token: 0x040019F6 RID: 6646
	public MazePlayerCollection playerCollectionVolume;

	// Token: 0x040019F7 RID: 6647
	private bool isOpen;
}
