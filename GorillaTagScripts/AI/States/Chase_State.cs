using System;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI.States
{
	// Token: 0x0200107D RID: 4221
	public class Chase_State : IState
	{
		// Token: 0x17000A03 RID: 2563
		// (get) Token: 0x0600695C RID: 26972 RVA: 0x0021EBB0 File Offset: 0x0021CDB0
		// (set) Token: 0x0600695D RID: 26973 RVA: 0x0021EBB8 File Offset: 0x0021CDB8
		public Transform FollowTarget { get; set; }

		// Token: 0x0600695E RID: 26974 RVA: 0x0021EBC1 File Offset: 0x0021CDC1
		public Chase_State(AIEntity entity)
		{
			this.entity = entity;
			this.agent = this.entity.navMeshAgent;
		}

		// Token: 0x0600695F RID: 26975 RVA: 0x0021EBE1 File Offset: 0x0021CDE1
		public void Tick()
		{
			this.agent.SetDestination(this.FollowTarget.position);
			if (this.agent.remainingDistance < this.entity.attackDistance)
			{
				this.chaseOver = true;
			}
		}

		// Token: 0x06006960 RID: 26976 RVA: 0x0021EC19 File Offset: 0x0021CE19
		public void OnEnter()
		{
			this.chaseOver = false;
			string text = "Current State: ";
			Type typeFromHandle = typeof(Chase_State);
			Debug.Log(text + ((typeFromHandle != null) ? typeFromHandle.ToString() : null));
		}

		// Token: 0x06006961 RID: 26977 RVA: 0x0021EC47 File Offset: 0x0021CE47
		public void OnExit()
		{
			this.chaseOver = true;
		}

		// Token: 0x040078F9 RID: 30969
		private AIEntity entity;

		// Token: 0x040078FA RID: 30970
		private NavMeshAgent agent;

		// Token: 0x040078FC RID: 30972
		public bool chaseOver;
	}
}
