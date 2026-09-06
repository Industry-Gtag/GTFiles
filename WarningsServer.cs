using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000BEA RID: 3050
internal abstract class WarningsServer : MonoBehaviour
{
	// Token: 0x06004CA4 RID: 19620
	public abstract Task<PlayerAgeGateWarningStatus?> FetchPlayerData(CancellationToken token);

	// Token: 0x06004CA5 RID: 19621
	public abstract Task<PlayerAgeGateWarningStatus?> GetOptInFollowUpMessage(CancellationToken token);

	// Token: 0x04005FD4 RID: 24532
	public static volatile WarningsServer Instance;
}
