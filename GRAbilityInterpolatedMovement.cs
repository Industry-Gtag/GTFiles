using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000736 RID: 1846
[Serializable]
public class GRAbilityInterpolatedMovement
{
	// Token: 0x06002EE3 RID: 12003 RVA: 0x001000D6 File Offset: 0x000FE2D6
	public void Setup(Transform root)
	{
		this.root = root;
		this.rb = root.gameObject.GetComponent<Rigidbody>();
		this.walkableArea = NavMesh.GetAreaFromName("walkable");
	}

	// Token: 0x06002EE4 RID: 12004 RVA: 0x00100100 File Offset: 0x000FE300
	public void InitFromVelocityAndDuration(Vector3 velocity, float duration)
	{
		this.velocity = velocity;
		this.duration = duration;
		float magnitude = velocity.magnitude;
	}

	// Token: 0x06002EE5 RID: 12005 RVA: 0x00100118 File Offset: 0x000FE318
	public void Start()
	{
		this.startPos = this.root.position;
		this.endPos = this.startPos + this.velocity * this.duration;
		this.endTime = Time.timeAsDouble + (double)this.duration;
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(this.endPos, out navMeshHit, 5f, this.walkableArea))
		{
			this.endPos = navMeshHit.position;
		}
	}

	// Token: 0x06002EE6 RID: 12006 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void Stop()
	{
	}

	// Token: 0x06002EE7 RID: 12007 RVA: 0x00100192 File Offset: 0x000FE392
	public bool IsDone()
	{
		return Time.timeAsDouble >= this.endTime;
	}

	// Token: 0x06002EE8 RID: 12008 RVA: 0x001001A4 File Offset: 0x000FE3A4
	public void Update(float dt)
	{
		Vector3 position = this.root.position;
		float num = Mathf.Clamp01(1f - (float)((this.endTime - Time.timeAsDouble) / (double)this.duration));
		GRAbilityInterpolatedMovement.InterpType interpType = this.interpolationType;
		Vector3 vector;
		if (interpType != GRAbilityInterpolatedMovement.InterpType.Linear && interpType == GRAbilityInterpolatedMovement.InterpType.EaseOut)
		{
			vector = Vector3.Lerp(this.startPos, this.endPos, AbilityHelperFunctions.EaseOutPower(num, 2.5f));
		}
		else
		{
			vector = Vector3.Lerp(this.startPos, this.endPos, num);
		}
		vector.y = Mathf.Lerp(this.startPos.y, this.endPos.y, num * num);
		NavMeshHit navMeshHit;
		if (NavMesh.Raycast(position, vector, out navMeshHit, this.walkableArea))
		{
			vector = navMeshHit.position;
		}
		this.root.position = vector;
		if (this.rb != null)
		{
			this.rb.position = vector;
		}
	}

	// Token: 0x04003C0A RID: 15370
	public Vector3 velocity = Vector3.zero;

	// Token: 0x04003C0B RID: 15371
	private Vector3 startPos;

	// Token: 0x04003C0C RID: 15372
	private Vector3 endPos;

	// Token: 0x04003C0D RID: 15373
	public float duration;

	// Token: 0x04003C0E RID: 15374
	public double endTime;

	// Token: 0x04003C0F RID: 15375
	public float maxVelocityMagnitude = 2f;

	// Token: 0x04003C10 RID: 15376
	private Transform root;

	// Token: 0x04003C11 RID: 15377
	private Rigidbody rb;

	// Token: 0x04003C12 RID: 15378
	public GRAbilityInterpolatedMovement.InterpType interpolationType;

	// Token: 0x04003C13 RID: 15379
	private int walkableArea = -1;

	// Token: 0x02000737 RID: 1847
	public enum InterpType
	{
		// Token: 0x04003C15 RID: 15381
		Linear,
		// Token: 0x04003C16 RID: 15382
		EaseOut
	}
}
