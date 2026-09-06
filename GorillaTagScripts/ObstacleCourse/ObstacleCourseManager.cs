using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x0200100C RID: 4108
	[NetworkBehaviourWeaved(9)]
	public class ObstacleCourseManager : NetworkComponent, ITickSystemTick
	{
		// Token: 0x170009CC RID: 2508
		// (get) Token: 0x0600665A RID: 26202 RVA: 0x0020EA1E File Offset: 0x0020CC1E
		// (set) Token: 0x0600665B RID: 26203 RVA: 0x0020EA25 File Offset: 0x0020CC25
		public static ObstacleCourseManager Instance { get; private set; }

		// Token: 0x170009CD RID: 2509
		// (get) Token: 0x0600665C RID: 26204 RVA: 0x0020EA2D File Offset: 0x0020CC2D
		// (set) Token: 0x0600665D RID: 26205 RVA: 0x0020EA35 File Offset: 0x0020CC35
		public bool TickRunning { get; set; }

		// Token: 0x0600665E RID: 26206 RVA: 0x0020EA3E File Offset: 0x0020CC3E
		protected override void Awake()
		{
			base.Awake();
			ObstacleCourseManager.Instance = this;
		}

		// Token: 0x0600665F RID: 26207 RVA: 0x0020EA4C File Offset: 0x0020CC4C
		internal override void OnEnable()
		{
			NetworkBehaviourUtils.InternalOnEnable(this);
			base.OnEnable();
			TickSystem<object>.AddCallbackTarget(this);
		}

		// Token: 0x06006660 RID: 26208 RVA: 0x0020EA60 File Offset: 0x0020CC60
		internal override void OnDisable()
		{
			NetworkBehaviourUtils.InternalOnDisable(this);
			base.OnEnable();
			TickSystem<object>.RemoveCallbackTarget(this);
		}

		// Token: 0x06006661 RID: 26209 RVA: 0x0020EA74 File Offset: 0x0020CC74
		public void Tick()
		{
			foreach (ObstacleCourse obstacleCourse in this.allObstaclesCourses)
			{
				obstacleCourse.InvokeUpdate();
			}
		}

		// Token: 0x06006662 RID: 26210 RVA: 0x0020EAC4 File Offset: 0x0020CCC4
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			this.allObstaclesCourses.Clear();
		}

		// Token: 0x170009CE RID: 2510
		// (get) Token: 0x06006663 RID: 26211 RVA: 0x0020EAD7 File Offset: 0x0020CCD7
		// (set) Token: 0x06006664 RID: 26212 RVA: 0x0020EB01 File Offset: 0x0020CD01
		[Networked]
		[NetworkedWeaved(0, 9)]
		public unsafe ObstacleCourseData Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ObstacleCourseManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(ObstacleCourseData*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ObstacleCourseManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(ObstacleCourseData*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06006665 RID: 26213 RVA: 0x0020EB2C File Offset: 0x0020CD2C
		public override void WriteDataFusion()
		{
			this.Data = new ObstacleCourseData(this.allObstaclesCourses);
		}

		// Token: 0x06006666 RID: 26214 RVA: 0x0020EB40 File Offset: 0x0020CD40
		public override void ReadDataFusion()
		{
			for (int i = 0; i < this.Data.ObstacleCourseCount; i++)
			{
				int num = this.Data.WinnerActorNumber[i];
				ObstacleCourse.RaceState raceState = (ObstacleCourse.RaceState)this.Data.CurrentRaceState[i];
				if (this.allObstaclesCourses[i].currentState != raceState)
				{
					this.allObstaclesCourses[i].Deserialize(num, raceState);
				}
			}
		}

		// Token: 0x06006667 RID: 26215 RVA: 0x0020EBC0 File Offset: 0x0020CDC0
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			stream.SendNext(this.allObstaclesCourses.Count);
			for (int i = 0; i < this.allObstaclesCourses.Count; i++)
			{
				stream.SendNext(this.allObstaclesCourses[i].winnerActorNumber);
				stream.SendNext(this.allObstaclesCourses[i].currentState);
			}
		}

		// Token: 0x06006668 RID: 26216 RVA: 0x0020EC40 File Offset: 0x0020CE40
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			int num = (int)stream.ReceiveNext();
			for (int i = 0; i < num; i++)
			{
				int num2 = (int)stream.ReceiveNext();
				ObstacleCourse.RaceState raceState = (ObstacleCourse.RaceState)stream.ReceiveNext();
				if (this.allObstaclesCourses[i].currentState != raceState)
				{
					this.allObstaclesCourses[i].Deserialize(num2, raceState);
				}
			}
		}

		// Token: 0x0600666A RID: 26218 RVA: 0x0020ECC5 File Offset: 0x0020CEC5
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x0600666B RID: 26219 RVA: 0x0020ECDD File Offset: 0x0020CEDD
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x04007553 RID: 30035
		public List<ObstacleCourse> allObstaclesCourses = new List<ObstacleCourse>();

		// Token: 0x04007555 RID: 30037
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 9)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private ObstacleCourseData _Data;
	}
}
