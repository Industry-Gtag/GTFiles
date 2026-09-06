using System;
using Photon.Pun;

// Token: 0x02000A47 RID: 2631
public class GorillaThrowingRock : GorillaThrowable, IPunInstantiateMagicCallback
{
	// Token: 0x06004394 RID: 17300 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
	}

	// Token: 0x0400558E RID: 21902
	public float bonkSpeedMin = 1f;

	// Token: 0x0400558F RID: 21903
	public float bonkSpeedMax = 5f;

	// Token: 0x04005590 RID: 21904
	public VRRig hitRig;
}
