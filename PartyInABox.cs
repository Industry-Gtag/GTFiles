using System;
using UnityEngine;

// Token: 0x020002F0 RID: 752
public class PartyInABox : MonoBehaviour
{
	// Token: 0x0600132C RID: 4908 RVA: 0x00065B26 File Offset: 0x00063D26
	private void Awake()
	{
		this.Reset();
	}

	// Token: 0x0600132D RID: 4909 RVA: 0x00065B26 File Offset: 0x00063D26
	private void OnEnable()
	{
		this.Reset();
	}

	// Token: 0x0600132E RID: 4910 RVA: 0x00065B2E File Offset: 0x00063D2E
	public void Cranked_ReleaseParty()
	{
		if (!this.parentHoldable.IsLocalObject())
		{
			return;
		}
		this.ReleaseParty();
	}

	// Token: 0x0600132F RID: 4911 RVA: 0x00065B44 File Offset: 0x00063D44
	public void ReleaseParty()
	{
		if (this.isReleased)
		{
			return;
		}
		if (this.parentHoldable.IsLocalObject())
		{
			this.parentHoldable.itemState |= TransferrableObject.ItemStates.State0;
			GorillaTagger.Instance.StartVibration(true, this.partyHapticStrength, this.partyHapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.partyHapticStrength, this.partyHapticDuration);
		}
		this.isReleased = true;
		this.spring.enabled = true;
		this.anim.Play();
		this.particles.Play();
		this.partyAudio.Play();
	}

	// Token: 0x06001330 RID: 4912 RVA: 0x00065BE0 File Offset: 0x00063DE0
	private void Update()
	{
		if (this.parentHoldable.IsLocalObject())
		{
			return;
		}
		if (this.parentHoldable.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (!this.isReleased)
			{
				this.ReleaseParty();
				return;
			}
		}
		else if (this.isReleased)
		{
			this.Reset();
		}
	}

	// Token: 0x06001331 RID: 4913 RVA: 0x00065C38 File Offset: 0x00063E38
	public void Reset()
	{
		this.isReleased = false;
		this.parentHoldable.itemState &= (TransferrableObject.ItemStates)(-2);
		this.spring.enabled = false;
		this.anim.Stop();
		foreach (PartyInABox.ForceTransform forceTransform in this.forceTransforms)
		{
			forceTransform.Apply();
		}
	}

	// Token: 0x04001762 RID: 5986
	[SerializeField]
	private TransferrableObject parentHoldable;

	// Token: 0x04001763 RID: 5987
	[SerializeField]
	private ParticleSystem particles;

	// Token: 0x04001764 RID: 5988
	[SerializeField]
	private Animation anim;

	// Token: 0x04001765 RID: 5989
	[SerializeField]
	private SpringyWobbler spring;

	// Token: 0x04001766 RID: 5990
	[SerializeField]
	private AudioSource partyAudio;

	// Token: 0x04001767 RID: 5991
	[SerializeField]
	private float partyHapticStrength;

	// Token: 0x04001768 RID: 5992
	[SerializeField]
	private float partyHapticDuration;

	// Token: 0x04001769 RID: 5993
	private bool isReleased;

	// Token: 0x0400176A RID: 5994
	[SerializeField]
	private PartyInABox.ForceTransform[] forceTransforms;

	// Token: 0x020002F1 RID: 753
	[Serializable]
	private struct ForceTransform
	{
		// Token: 0x06001333 RID: 4915 RVA: 0x00065C9B File Offset: 0x00063E9B
		public void Apply()
		{
			this.transform.localPosition = this.localPosition;
			this.transform.localRotation = this.localRotation;
		}

		// Token: 0x0400176B RID: 5995
		public Transform transform;

		// Token: 0x0400176C RID: 5996
		public Vector3 localPosition;

		// Token: 0x0400176D RID: 5997
		public Quaternion localRotation;
	}
}
