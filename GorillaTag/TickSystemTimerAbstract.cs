using System;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x02001221 RID: 4641
	[Serializable]
	internal abstract class TickSystemTimerAbstract : CoolDownHelper, ITickSystemPre
	{
		// Token: 0x17000B6C RID: 2924
		// (get) Token: 0x060075AB RID: 30123 RVA: 0x00264ED8 File Offset: 0x002630D8
		// (set) Token: 0x060075AC RID: 30124 RVA: 0x00264EE0 File Offset: 0x002630E0
		bool ITickSystemPre.PreTickRunning
		{
			get
			{
				return this.registered;
			}
			set
			{
				this.registered = value;
			}
		}

		// Token: 0x17000B6D RID: 2925
		// (get) Token: 0x060075AD RID: 30125 RVA: 0x00264ED8 File Offset: 0x002630D8
		public bool Running
		{
			get
			{
				return this.registered;
			}
		}

		// Token: 0x060075AE RID: 30126 RVA: 0x00264EE9 File Offset: 0x002630E9
		protected TickSystemTimerAbstract()
		{
		}

		// Token: 0x060075AF RID: 30127 RVA: 0x00264EF1 File Offset: 0x002630F1
		protected TickSystemTimerAbstract(float cd)
			: base(cd)
		{
		}

		// Token: 0x060075B0 RID: 30128 RVA: 0x00264EFA File Offset: 0x002630FA
		public override void Start()
		{
			base.Start();
			TickSystem<object>.AddPreTickCallback(this);
		}

		// Token: 0x060075B1 RID: 30129 RVA: 0x00264F08 File Offset: 0x00263108
		public override void Stop()
		{
			base.Stop();
			TickSystem<object>.RemovePreTickCallback(this);
		}

		// Token: 0x060075B2 RID: 30130 RVA: 0x00264F16 File Offset: 0x00263116
		public override void OnCheckPass()
		{
			this.OnTimedEvent();
		}

		// Token: 0x060075B3 RID: 30131
		public abstract void OnTimedEvent();

		// Token: 0x060075B4 RID: 30132 RVA: 0x00264F1E File Offset: 0x0026311E
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void ITickSystemPre.PreTick()
		{
			base.CheckCooldown();
		}

		// Token: 0x0400858F RID: 34191
		[NonSerialized]
		internal bool registered;
	}
}
