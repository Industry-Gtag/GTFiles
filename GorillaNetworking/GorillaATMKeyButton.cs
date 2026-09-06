using System;

namespace GorillaNetworking
{
	// Token: 0x020010DA RID: 4314
	public class GorillaATMKeyButton : GorillaKeyButton<GorillaATMKeyBindings>
	{
		// Token: 0x06006BC5 RID: 27589 RVA: 0x0022BC2D File Offset: 0x00229E2D
		protected override void OnButtonPressedEvent()
		{
			GameEvents.OnGorrillaATMKeyButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
