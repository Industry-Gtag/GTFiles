using System;
using UnityEngine;

// Token: 0x020004E7 RID: 1255
public class RigEventVolumeTrigger : MonoBehaviour
{
	// Token: 0x17000333 RID: 819
	// (get) Token: 0x06001E89 RID: 7817 RVA: 0x000A3800 File Offset: 0x000A1A00
	public VRRig Rig
	{
		get
		{
			return this._rig;
		}
	}

	// Token: 0x040028C0 RID: 10432
	[SerializeField]
	private VRRig _rig;
}
