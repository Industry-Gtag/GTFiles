using System;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x02001222 RID: 4642
	[Serializable]
	internal class TickSystemTimer : TickSystemTimerAbstract
	{
		// Token: 0x060075B5 RID: 30133 RVA: 0x0026488A File Offset: 0x00262A8A
		public TickSystemTimer()
		{
		}

		// Token: 0x060075B6 RID: 30134 RVA: 0x00264F27 File Offset: 0x00263127
		public TickSystemTimer(float cd)
			: base(cd)
		{
		}

		// Token: 0x060075B7 RID: 30135 RVA: 0x00264F30 File Offset: 0x00263130
		public TickSystemTimer(float cd, Action cb)
			: base(cd)
		{
			this.callback = cb;
		}

		// Token: 0x060075B8 RID: 30136 RVA: 0x00264F40 File Offset: 0x00263140
		public TickSystemTimer(Action cb)
		{
			this.callback = cb;
		}

		// Token: 0x060075B9 RID: 30137 RVA: 0x00264F4F File Offset: 0x0026314F
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void OnTimedEvent()
		{
			Action action = this.callback;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x04008590 RID: 34192
		public Action callback;
	}
}
