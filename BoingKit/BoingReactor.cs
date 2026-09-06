using System;

namespace BoingKit
{
	// Token: 0x0200143D RID: 5181
	public class BoingReactor : BoingBehavior
	{
		// Token: 0x060082A3 RID: 33443 RVA: 0x002A9F4A File Offset: 0x002A814A
		protected override void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060082A4 RID: 33444 RVA: 0x002A9F52 File Offset: 0x002A8152
		protected override void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060082A5 RID: 33445 RVA: 0x002A9F5A File Offset: 0x002A815A
		public override void PrepareExecute()
		{
			base.PrepareExecute(true);
		}
	}
}
