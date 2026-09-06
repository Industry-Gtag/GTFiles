using System;
using UnityEngine;

// Token: 0x02000DF2 RID: 3570
public class UIMatchRotation : MonoBehaviour
{
	// Token: 0x06005785 RID: 22405 RVA: 0x001C8FC5 File Offset: 0x001C71C5
	private void Start()
	{
		this.referenceTransform = Camera.main.transform;
		base.transform.forward = this.x0z(this.referenceTransform.forward);
	}

	// Token: 0x06005786 RID: 22406 RVA: 0x001C8FF4 File Offset: 0x001C71F4
	private void Update()
	{
		Vector3 vector = this.x0z(base.transform.forward);
		Vector3 vector2 = this.x0z(this.referenceTransform.forward);
		float num = Vector3.Dot(vector, vector2);
		UIMatchRotation.State state = this.state;
		if (state != UIMatchRotation.State.Ready)
		{
			if (state != UIMatchRotation.State.Rotating)
			{
				return;
			}
			base.transform.forward = Vector3.Lerp(base.transform.forward, vector2, Time.deltaTime * this.lerpSpeed);
			if (Vector3.Dot(base.transform.forward, vector2) > 0.995f)
			{
				this.state = UIMatchRotation.State.Ready;
			}
		}
		else if (num < 1f - this.threshold)
		{
			this.state = UIMatchRotation.State.Rotating;
			return;
		}
	}

	// Token: 0x06005787 RID: 22407 RVA: 0x001C9098 File Offset: 0x001C7298
	private Vector3 x0z(Vector3 vector)
	{
		vector.y = 0f;
		return vector.normalized;
	}

	// Token: 0x04006813 RID: 26643
	[SerializeField]
	private Transform referenceTransform;

	// Token: 0x04006814 RID: 26644
	[SerializeField]
	private float threshold = 0.35f;

	// Token: 0x04006815 RID: 26645
	[SerializeField]
	private float lerpSpeed = 5f;

	// Token: 0x04006816 RID: 26646
	private UIMatchRotation.State state;

	// Token: 0x02000DF3 RID: 3571
	private enum State
	{
		// Token: 0x04006818 RID: 26648
		Ready,
		// Token: 0x04006819 RID: 26649
		Rotating
	}
}
