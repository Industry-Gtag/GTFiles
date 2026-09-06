using System;
using System.Runtime.CompilerServices;
using GorillaTagScripts.AI.States;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.AI.Entities
{
	// Token: 0x02001080 RID: 4224
	public class TestShark : AIEntity
	{
		// Token: 0x0600696A RID: 26986 RVA: 0x0021EDEC File Offset: 0x0021CFEC
		private new void Awake()
		{
			base.Awake();
			this.chasingTimer = 0f;
			this._stateMachine = new StateMachine();
			this.circularPatrol = new CircularPatrol_State(this);
			this.patrol = new Patrol_State(this);
			this.chase = new Chase_State(this);
			this._stateMachine.AddTransition(this.patrol, this.chase, this.<Awake>g__ShouldChase|7_0());
			this._stateMachine.AddTransition(this.chase, this.patrol, this.<Awake>g__ShouldPatrol|7_1());
			this._stateMachine.SetState(this.patrol);
		}

		// Token: 0x0600696B RID: 26987 RVA: 0x0021EE84 File Offset: 0x0021D084
		private void Update()
		{
			this._stateMachine.Tick();
			this.shouldChase = false;
			this.chasingTimer += Time.deltaTime;
			if (this.chasingTimer >= this.nextTimeToChasePlayer)
			{
				base.ChooseClosestTarget();
				if (this.followTarget != null)
				{
					this.chase.FollowTarget = this.followTarget;
					this.shouldChase = true;
				}
				this.chasingTimer = 0f;
			}
		}

		// Token: 0x0600696D RID: 26989 RVA: 0x0021EF0D File Offset: 0x0021D10D
		[CompilerGenerated]
		private Func<bool> <Awake>g__ShouldChase|7_0()
		{
			return () => this.shouldChase && PhotonNetwork.InRoom;
		}

		// Token: 0x0600696F RID: 26991 RVA: 0x0021EF2C File Offset: 0x0021D12C
		[CompilerGenerated]
		private Func<bool> <Awake>g__ShouldPatrol|7_1()
		{
			return () => this.chase.chaseOver;
		}

		// Token: 0x04007901 RID: 30977
		public float nextTimeToChasePlayer = 30f;

		// Token: 0x04007902 RID: 30978
		private float chasingTimer;

		// Token: 0x04007903 RID: 30979
		private bool shouldChase;

		// Token: 0x04007904 RID: 30980
		private StateMachine _stateMachine;

		// Token: 0x04007905 RID: 30981
		private CircularPatrol_State circularPatrol;

		// Token: 0x04007906 RID: 30982
		private Patrol_State patrol;

		// Token: 0x04007907 RID: 30983
		private Chase_State chase;
	}
}
