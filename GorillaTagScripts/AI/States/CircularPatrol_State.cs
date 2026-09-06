using System;
using UnityEngine;

namespace GorillaTagScripts.AI.States
{
	// Token: 0x0200107E RID: 4222
	public class CircularPatrol_State : IState
	{
		// Token: 0x06006962 RID: 26978 RVA: 0x0021EC50 File Offset: 0x0021CE50
		public CircularPatrol_State(AIEntity entity)
		{
			this.entity = entity;
		}

		// Token: 0x06006963 RID: 26979 RVA: 0x0021EC60 File Offset: 0x0021CE60
		public void Tick()
		{
			Vector3 position = this.entity.circleCenter.position;
			float num = position.x + Mathf.Cos(this.angle) * this.entity.angularSpeed;
			float y = position.y;
			float num2 = position.z + Mathf.Sin(this.angle) * this.entity.angularSpeed;
			this.entity.transform.position = new Vector3(num, y, num2);
			this.angle += this.entity.angularSpeed * Time.deltaTime;
		}

		// Token: 0x06006964 RID: 26980 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnEnter()
		{
		}

		// Token: 0x06006965 RID: 26981 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnExit()
		{
		}

		// Token: 0x040078FD RID: 30973
		private AIEntity entity;

		// Token: 0x040078FE RID: 30974
		private float angle;
	}
}
