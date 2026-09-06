using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x020013F7 RID: 5111
	[CreateAssetMenu(fileName = "New Game Object Schedule", menuName = "Game Object Scheduling/Game Object Schedule", order = 0)]
	public class GameObjectSchedule : ScriptableObject
	{
		// Token: 0x17000C64 RID: 3172
		// (get) Token: 0x060080E7 RID: 32999 RVA: 0x0029E3D6 File Offset: 0x0029C5D6
		public GameObjectSchedule.GameObjectScheduleNode[] Nodes
		{
			get
			{
				return this.nodes;
			}
		}

		// Token: 0x17000C65 RID: 3173
		// (get) Token: 0x060080E8 RID: 33000 RVA: 0x0029E3DE File Offset: 0x0029C5DE
		public bool InitialState
		{
			get
			{
				return this.initialState;
			}
		}

		// Token: 0x060080E9 RID: 33001 RVA: 0x0029E3E8 File Offset: 0x0029C5E8
		public int GetCurrentNodeIndex(DateTime currentDate, out DateTime startDate)
		{
			int i = -1;
			startDate = default(DateTime);
			while (i < this.nodes.Length - 1)
			{
				if (currentDate < this.nodes[i + 1].DateTime)
				{
					if (i >= 0)
					{
						startDate = this.nodes[i].DateTime;
					}
					return i;
				}
				i++;
			}
			return int.MaxValue;
		}

		// Token: 0x060080EA RID: 33002 RVA: 0x0029E446 File Offset: 0x0029C646
		public void Validate()
		{
			if (this.validated)
			{
				return;
			}
			this._validate();
			this.validated = true;
		}

		// Token: 0x060080EB RID: 33003 RVA: 0x0029E460 File Offset: 0x0029C660
		private void _validate()
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				this.nodes[i].Validate();
			}
			List<GameObjectSchedule.GameObjectScheduleNode> list = new List<GameObjectSchedule.GameObjectScheduleNode>(this.nodes);
			list.Sort((GameObjectSchedule.GameObjectScheduleNode e1, GameObjectSchedule.GameObjectScheduleNode e2) => e1.DateTime.CompareTo(e2.DateTime));
			this.nodes = list.ToArray();
		}

		// Token: 0x060080EC RID: 33004 RVA: 0x0029E4CC File Offset: 0x0029C6CC
		public static void GenerateDailyShuffle(DateTime startDate, DateTime endDate, GameObjectSchedule[] schedules)
		{
			TimeSpan timeSpan = TimeSpan.FromDays(1.0);
			int num = schedules.Length - 1;
			int num2 = schedules.Length - 2;
			DateTime dateTime = startDate;
			List<GameObjectSchedule.GameObjectScheduleNode>[] array = new List<GameObjectSchedule.GameObjectScheduleNode>[schedules.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new List<GameObjectSchedule.GameObjectScheduleNode>();
			}
			while (dateTime < endDate)
			{
				int num3 = Random.Range(0, schedules.Length - 2);
				if (num <= num3)
				{
					num3++;
					if (num2 <= num3)
					{
						num3++;
					}
				}
				else if (num2 <= num3)
				{
					num3++;
					if (num <= num3)
					{
						num3++;
					}
				}
				array[num].Add(new GameObjectSchedule.GameObjectScheduleNode
				{
					activeDateTime = dateTime.ToString(),
					activeState = false
				});
				array[num3].Add(new GameObjectSchedule.GameObjectScheduleNode
				{
					activeDateTime = dateTime.ToString(),
					activeState = true
				});
				dateTime += timeSpan;
				num2 = num;
				num = num3;
			}
			array[num].Add(new GameObjectSchedule.GameObjectScheduleNode
			{
				activeDateTime = dateTime.ToString(),
				activeState = false
			});
			for (int j = 0; j < array.Length; j++)
			{
				schedules[j].nodes = array[j].ToArray();
			}
		}

		// Token: 0x040091E7 RID: 37351
		[SerializeField]
		private bool initialState;

		// Token: 0x040091E8 RID: 37352
		[SerializeField]
		private GameObjectSchedule.GameObjectScheduleNode[] nodes;

		// Token: 0x040091E9 RID: 37353
		[SerializeField]
		private SchedulingOptions options;

		// Token: 0x040091EA RID: 37354
		private bool validated;

		// Token: 0x020013F8 RID: 5112
		[Serializable]
		public class GameObjectScheduleNode
		{
			// Token: 0x17000C66 RID: 3174
			// (get) Token: 0x060080EE RID: 33006 RVA: 0x0029E603 File Offset: 0x0029C803
			public bool ActiveState
			{
				get
				{
					return this.activeState;
				}
			}

			// Token: 0x17000C67 RID: 3175
			// (get) Token: 0x060080EF RID: 33007 RVA: 0x0029E60B File Offset: 0x0029C80B
			public DateTime DateTime
			{
				get
				{
					return this.dateTime;
				}
			}

			// Token: 0x060080F0 RID: 33008 RVA: 0x0029E614 File Offset: 0x0029C814
			public void Validate()
			{
				try
				{
					this.dateTime = DateTime.Parse(this.activeDateTime, CultureInfo.InvariantCulture);
				}
				catch
				{
					this.dateTime = DateTime.MinValue;
				}
			}

			// Token: 0x040091EB RID: 37355
			[SerializeField]
			public string activeDateTime = "1/1/0001 00:00:00";

			// Token: 0x040091EC RID: 37356
			[SerializeField]
			[Tooltip("Check to turn on. Uncheck to turn off.")]
			public bool activeState = true;

			// Token: 0x040091ED RID: 37357
			private DateTime dateTime;
		}
	}
}
