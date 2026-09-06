using System;
using UnityEngine;

// Token: 0x02000D66 RID: 3430
public class BuildTargetManager : MonoBehaviour
{
	// Token: 0x060054D2 RID: 21714 RVA: 0x001BD6A7 File Offset: 0x001BB8A7
	public string GetPath()
	{
		return this.path;
	}

	// Token: 0x0400662F RID: 26159
	public BuildTargetManager.BuildTowards newBuildTarget;

	// Token: 0x04006630 RID: 26160
	public bool isBeta;

	// Token: 0x04006631 RID: 26161
	public bool isQA;

	// Token: 0x04006632 RID: 26162
	public bool spoofIDs;

	// Token: 0x04006633 RID: 26163
	public bool spoofChild;

	// Token: 0x04006634 RID: 26164
	public bool enableAllCosmetics;

	// Token: 0x04006635 RID: 26165
	public OVRManager ovrManager;

	// Token: 0x04006636 RID: 26166
	private string path = "Assets/csc.rsp";

	// Token: 0x04006637 RID: 26167
	public BuildTargetManager.BuildTowards currentBuildTargetDONOTCHANGE;

	// Token: 0x04006638 RID: 26168
	public GorillaTagger gorillaTagger;

	// Token: 0x04006639 RID: 26169
	public GameObject[] betaDisableObjects;

	// Token: 0x0400663A RID: 26170
	public GameObject[] betaEnableObjects;

	// Token: 0x0400663B RID: 26171
	public BuildTargetManager.NetworkBackend networkBackend;

	// Token: 0x02000D67 RID: 3431
	public enum BuildTowards
	{
		// Token: 0x0400663D RID: 26173
		Steam,
		// Token: 0x0400663E RID: 26174
		OculusPC,
		// Token: 0x0400663F RID: 26175
		Quest,
		// Token: 0x04006640 RID: 26176
		Viveport
	}

	// Token: 0x02000D68 RID: 3432
	public enum NetworkBackend
	{
		// Token: 0x04006642 RID: 26178
		Pun,
		// Token: 0x04006643 RID: 26179
		Fusion
	}
}
