using System;
using UnityEngine;

// Token: 0x02000A0D RID: 2573
[DefaultExecutionOrder(100)]
public class TransformFollow : MonoBehaviour
{
	// Token: 0x06004203 RID: 16899 RVA: 0x0015F8C4 File Offset: 0x0015DAC4
	private void Awake()
	{
		this.prevPos = base.transform.position;
		if (this.rotationOnly && base.transform.parent != null && base.transform.parent.GetComponent<TransformFollow>() != null)
		{
			this.forRigRecording = true;
		}
		if (this.forRigRecording)
		{
			this.parentFollow = base.transform.parent.GetComponent<TransformFollow>();
		}
	}

	// Token: 0x06004204 RID: 16900 RVA: 0x0015F93C File Offset: 0x0015DB3C
	private void LateUpdate()
	{
		this.prevPos = base.transform.position;
		if (!this.rotationOnly)
		{
			Vector3 vector;
			Quaternion quaternion;
			this.transformToFollow.GetPositionAndRotation(out vector, out quaternion);
			base.transform.SetPositionAndRotation(vector + quaternion * this.offset, quaternion);
			return;
		}
		if (this.forRigRecording)
		{
			base.transform.localRotation = Quaternion.Inverse(this.parentFollow.transformToFollow.rotation) * this.transformToFollow.rotation;
			return;
		}
		base.transform.rotation = this.transformToFollow.rotation;
	}

	// Token: 0x040052C2 RID: 21186
	public Transform transformToFollow;

	// Token: 0x040052C3 RID: 21187
	public Vector3 offset;

	// Token: 0x040052C4 RID: 21188
	public Vector3 prevPos;

	// Token: 0x040052C5 RID: 21189
	public bool rotationOnly;

	// Token: 0x040052C6 RID: 21190
	private bool forRigRecording;

	// Token: 0x040052C7 RID: 21191
	private TransformFollow parentFollow;
}
