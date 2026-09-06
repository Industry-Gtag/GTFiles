using System;
using GorillaNetworking;
using GorillaTagScripts.Builder;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005C7 RID: 1479
public class GameEvents
{
	// Token: 0x040030D2 RID: 12498
	public static UnityEvent<GorillaKeyboardBindings> OnGorrillaKeyboardButtonPressedEvent = new UnityEvent<GorillaKeyboardBindings>();

	// Token: 0x040030D3 RID: 12499
	public static UnityEvent<GorillaATMKeyBindings> OnGorrillaATMKeyButtonPressedEvent = new UnityEvent<GorillaATMKeyBindings>();

	// Token: 0x040030D4 RID: 12500
	internal static UnityEvent<string> ScreenTextChangedEvent = new UnityEvent<string>();

	// Token: 0x040030D5 RID: 12501
	internal static UnityEvent<Material[]> ScreenTextMaterialsEvent = new UnityEvent<Material[]>();

	// Token: 0x040030D6 RID: 12502
	internal static UnityEvent<string> FunctionSelectTextChangedEvent = new UnityEvent<string>();

	// Token: 0x040030D7 RID: 12503
	internal static UnityEvent<Material[]> FunctionTextMaterialsEvent = new UnityEvent<Material[]>();

	// Token: 0x040030D8 RID: 12504
	internal static UnityEvent LanguageEvent = new UnityEvent();

	// Token: 0x040030D9 RID: 12505
	internal static UnityEvent<string> ScoreboardTextChangedEvent = new UnityEvent<string>();

	// Token: 0x040030DA RID: 12506
	internal static UnityEvent<Material[]> ScoreboardMaterialsEvent = new UnityEvent<Material[]>();

	// Token: 0x040030DB RID: 12507
	public static UnityEvent<SharedBlocksKeyboardBindings> OnSharedBlocksKeyboardButtonPressedEvent = new UnityEvent<SharedBlocksKeyboardBindings>();
}
