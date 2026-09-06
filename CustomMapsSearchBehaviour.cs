using System;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000A70 RID: 2672
public class CustomMapsSearchBehaviour : CustomMapsBehaviourBase
{
	// Token: 0x060044B7 RID: 17591 RVA: 0x0016F95C File Offset: 0x0016DB5C
	public CustomMapsSearchBehaviour(CustomMapsAIBehaviourController AIcontroller, AIAgent agentSettings)
	{
		this.sightOffset = agentSettings.sightOffset;
		this.sightDist = agentSettings.sightDist;
		this.sightDistSq = this.sightDist * this.sightDist;
		this.sightFOV = agentSettings.sightFOV;
		this.sightMinDot = Mathf.Cos(this.sightFOV / 2f * 0.017453292f);
		this.controller = AIcontroller;
	}

	// Token: 0x060044B8 RID: 17592 RVA: 0x0016F9CA File Offset: 0x0016DBCA
	public override bool CanExecute()
	{
		return !this.controller.IsNull();
	}

	// Token: 0x060044B9 RID: 17593 RVA: 0x0016F9DC File Offset: 0x0016DBDC
	public override bool CanContinueExecuting()
	{
		return this.CanExecute() && this.controller.TargetPlayer == null;
	}

	// Token: 0x060044BA RID: 17594 RVA: 0x0016F9FC File Offset: 0x0016DBFC
	public override void Execute()
	{
		if (Time.time < this.lastSearchTime + 0.1f)
		{
			return;
		}
		this.lastSearchTime = Time.time;
		Vector3 vector = this.controller.transform.position + this.controller.transform.TransformVector(this.sightOffset);
		this.controller.SetTarget(this.controller.FindBestTarget(vector, this.sightDist, this.sightDistSq, this.sightMinDot));
	}

	// Token: 0x060044BB RID: 17595 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void NetExecute()
	{
	}

	// Token: 0x060044BC RID: 17596 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void ResetBehavior()
	{
	}

	// Token: 0x060044BD RID: 17597 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnTriggerEnter(Collider otherCollider)
	{
	}

	// Token: 0x040056CD RID: 22221
	private const float SEARCH_COOLDOWN = 0.1f;

	// Token: 0x040056CE RID: 22222
	private CustomMapsAIBehaviourController controller;

	// Token: 0x040056CF RID: 22223
	private float sightDist;

	// Token: 0x040056D0 RID: 22224
	private float sightDistSq;

	// Token: 0x040056D1 RID: 22225
	private Vector3 sightOffset;

	// Token: 0x040056D2 RID: 22226
	private float sightFOV;

	// Token: 0x040056D3 RID: 22227
	private float sightMinDot;

	// Token: 0x040056D4 RID: 22228
	private float lastSearchTime;
}
