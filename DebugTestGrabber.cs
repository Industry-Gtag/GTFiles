using System;
using UnityEngine;

// Token: 0x02000083 RID: 131
public class DebugTestGrabber : MonoBehaviour
{
	// Token: 0x06000333 RID: 819 RVA: 0x000135A9 File Offset: 0x000117A9
	private void Awake()
	{
		if (this.grabber == null)
		{
			this.grabber = base.GetComponentInChildren<CrittersGrabber>();
		}
	}

	// Token: 0x06000334 RID: 820 RVA: 0x000135C8 File Offset: 0x000117C8
	private void LateUpdate()
	{
		if (this.transformToFollow != null)
		{
			base.transform.rotation = this.transformToFollow.rotation;
			base.transform.position = this.transformToFollow.position;
		}
		if (this.grabber == null)
		{
			return;
		}
		if (!this.isGrabbing && this.setIsGrabbing)
		{
			this.setIsGrabbing = false;
			this.isGrabbing = true;
			this.remainingGrabDuration = this.grabDuration;
		}
		else if (this.isGrabbing && this.setRelease)
		{
			this.setRelease = false;
			this.isGrabbing = false;
			this.DoRelease();
		}
		if (this.isGrabbing && this.remainingGrabDuration > 0f)
		{
			this.remainingGrabDuration -= Time.deltaTime;
			this.DoGrab();
		}
	}

	// Token: 0x06000335 RID: 821 RVA: 0x0001369C File Offset: 0x0001189C
	private void DoGrab()
	{
		this.grabber.grabbing = true;
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.grabRadius, this.colliders, LayerMask.GetMask(new string[] { "GorillaInteractable" }));
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				CrittersActor componentInParent = this.colliders[i].GetComponentInParent<CrittersActor>();
				if (!(componentInParent == null) && componentInParent.usesRB && componentInParent.CanBeGrabbed(this.grabber))
				{
					this.isHandGrabbingDisabled = true;
					if (componentInParent.equipmentStorable)
					{
						componentInParent.localCanStore = true;
					}
					componentInParent.GrabbedBy(this.grabber, false, default(Quaternion), default(Vector3), false);
					this.grabber.grabbedActors.Add(componentInParent);
					this.remainingGrabDuration = 0f;
					return;
				}
			}
		}
	}

	// Token: 0x06000336 RID: 822 RVA: 0x00013780 File Offset: 0x00011980
	private void DoRelease()
	{
		this.grabber.grabbing = false;
		for (int i = this.grabber.grabbedActors.Count - 1; i >= 0; i--)
		{
			CrittersActor crittersActor = this.grabber.grabbedActors[i];
			crittersActor.Released(true, crittersActor.transform.rotation, crittersActor.transform.position, this.estimator.linearVelocity, default(Vector3));
			if (i < this.grabber.grabbedActors.Count)
			{
				this.grabber.grabbedActors.RemoveAt(i);
			}
		}
		if (this.isHandGrabbingDisabled)
		{
			this.isHandGrabbingDisabled = false;
		}
	}

	// Token: 0x040003CB RID: 971
	public bool isGrabbing;

	// Token: 0x040003CC RID: 972
	public bool setIsGrabbing;

	// Token: 0x040003CD RID: 973
	public bool setRelease;

	// Token: 0x040003CE RID: 974
	public Collider[] colliders = new Collider[50];

	// Token: 0x040003CF RID: 975
	public bool isLeft;

	// Token: 0x040003D0 RID: 976
	public float grabRadius = 0.05f;

	// Token: 0x040003D1 RID: 977
	public Transform transformToFollow;

	// Token: 0x040003D2 RID: 978
	public GorillaVelocityEstimator estimator;

	// Token: 0x040003D3 RID: 979
	public CrittersGrabber grabber;

	// Token: 0x040003D4 RID: 980
	public CrittersActorGrabber otherHand;

	// Token: 0x040003D5 RID: 981
	private bool isHandGrabbingDisabled;

	// Token: 0x040003D6 RID: 982
	private float grabDuration = 0.3f;

	// Token: 0x040003D7 RID: 983
	private float remainingGrabDuration;
}
