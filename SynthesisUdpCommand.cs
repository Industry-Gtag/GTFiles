using System;
using UnityEngine.Events;

// Token: 0x02000E24 RID: 3620
[Serializable]
public struct SynthesisUdpCommand
{
	// Token: 0x040068C7 RID: 26823
	public string commandName;

	// Token: 0x040068C8 RID: 26824
	public UnityEvent<string> EventReceiver;

	// Token: 0x040068C9 RID: 26825
	internal string extraArgs;
}
