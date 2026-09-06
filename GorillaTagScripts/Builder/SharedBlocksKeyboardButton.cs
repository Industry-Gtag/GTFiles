using System;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001053 RID: 4179
	public class SharedBlocksKeyboardButton : GorillaKeyButton<SharedBlocksKeyboardBindings>
	{
		// Token: 0x06006841 RID: 26689 RVA: 0x00219351 File Offset: 0x00217551
		protected override void OnButtonPressedEvent()
		{
			GameEvents.OnSharedBlocksKeyboardButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
