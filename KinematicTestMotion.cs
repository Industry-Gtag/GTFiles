using System;
using UnityEngine;

// Token: 0x02000D70 RID: 3440
public class KinematicTestMotion : MonoBehaviour
{
	// Token: 0x060054EE RID: 21742 RVA: 0x001BDD0A File Offset: 0x001BBF0A
	private void FixedUpdate()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.FixedUpdate)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x060054EF RID: 21743 RVA: 0x001BDD21 File Offset: 0x001BBF21
	private void Update()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.Update)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x060054F0 RID: 21744 RVA: 0x001BDD37 File Offset: 0x001BBF37
	private void LateUpdate()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.LateUpdate)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x060054F1 RID: 21745 RVA: 0x001BDD50 File Offset: 0x001BBF50
	private void UpdatePosition(float time)
	{
		float num = Mathf.Sin(time * 2f * 3.1415927f * this.period) * 0.5f + 0.5f;
		Vector3 vector = Vector3.Lerp(this.start.position, this.end.position, num);
		if (this.moveType == KinematicTestMotion.MoveType.TransformPosition)
		{
			base.transform.position = vector;
			return;
		}
		if (this.moveType == KinematicTestMotion.MoveType.RigidbodyMovePosition)
		{
			this.rigidbody.MovePosition(vector);
		}
	}

	// Token: 0x0400669D RID: 26269
	public Transform start;

	// Token: 0x0400669E RID: 26270
	public Transform end;

	// Token: 0x0400669F RID: 26271
	public Rigidbody rigidbody;

	// Token: 0x040066A0 RID: 26272
	public KinematicTestMotion.UpdateType updateType;

	// Token: 0x040066A1 RID: 26273
	public KinematicTestMotion.MoveType moveType = KinematicTestMotion.MoveType.RigidbodyMovePosition;

	// Token: 0x040066A2 RID: 26274
	public float period = 4f;

	// Token: 0x02000D71 RID: 3441
	public enum UpdateType
	{
		// Token: 0x040066A4 RID: 26276
		Update,
		// Token: 0x040066A5 RID: 26277
		LateUpdate,
		// Token: 0x040066A6 RID: 26278
		FixedUpdate
	}

	// Token: 0x02000D72 RID: 3442
	public enum MoveType
	{
		// Token: 0x040066A8 RID: 26280
		TransformPosition,
		// Token: 0x040066A9 RID: 26281
		RigidbodyMovePosition
	}
}
