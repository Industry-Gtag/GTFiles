using System;

namespace GorillaNetworking
{
	// Token: 0x020010E9 RID: 4329
	public class GorillaKeyboardButton : GorillaKeyButton<GorillaKeyboardBindings>
	{
		// Token: 0x06006CA9 RID: 27817 RVA: 0x002322B6 File Offset: 0x002304B6
		protected override void OnButtonPressedEvent()
		{
			GameEvents.OnGorrillaKeyboardButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
