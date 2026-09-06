using System;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI.States
{
	// Token: 0x0200107F RID: 4223
	public class Patrol_State : IState
	{
		// Token: 0x06006966 RID: 26982 RVA: 0x0021ECF7 File Offset: 0x0021CEF7
		public Patrol_State(AIEntity entity)
		{
			this.entity = entity;
			this.agent = this.entity.navMeshAgent;
		}

		// Token: 0x06006967 RID: 26983 RVA: 0x0021ED18 File Offset: 0x0021CF18
		public void Tick()
		{
			if (this.agent.remainingDistance <= this.agent.stoppingDistance)
			{
				Vector3 position = this.entity.waypoints[Random.Range(0, this.entity.waypoints.Count - 1)].transform.position;
				this.agent.SetDestination(position);
			}
		}

		// Token: 0x06006968 RID: 26984 RVA: 0x0021ED80 File Offset: 0x0021CF80
		public void OnEnter()
		{
			string text = "Current State: ";
			Type typeFromHandle = typeof(Patrol_State);
			Debug.Log(text + ((typeFromHandle != null) ? typeFromHandle.ToString() : null));
			if (this.entity.waypoints.Count > 0)
			{
				this.agent.SetDestination(this.entity.waypoints[0].transform.position);
			}
		}

		// Token: 0x06006969 RID: 26985 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnExit()
		{
		}

		// Token: 0x040078FF RID: 30975
		private AIEntity entity;

		// Token: 0x04007900 RID: 30976
		private NavMeshAgent agent;
	}
}
