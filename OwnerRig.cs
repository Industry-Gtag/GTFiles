using System;
using UnityEngine;

// Token: 0x0200095B RID: 2395
public class OwnerRig : MonoBehaviour, IVariable<VRRig>, IVariable, IRigAware
{
	// Token: 0x06003EF0 RID: 16112 RVA: 0x00152D4B File Offset: 0x00150F4B
	public void TryFindRig()
	{
		this._rig = base.GetComponentInParent<VRRig>();
		if (this._rig != null)
		{
			return;
		}
		this._rig = base.GetComponentInChildren<VRRig>();
	}

	// Token: 0x06003EF1 RID: 16113 RVA: 0x00152D74 File Offset: 0x00150F74
	public VRRig Get()
	{
		return this._rig;
	}

	// Token: 0x06003EF2 RID: 16114 RVA: 0x00152D7C File Offset: 0x00150F7C
	public void Set(VRRig value)
	{
		this._rig = value;
	}

	// Token: 0x06003EF3 RID: 16115 RVA: 0x00152D85 File Offset: 0x00150F85
	public void Set(GameObject obj)
	{
		this._rig = ((obj != null) ? obj.GetComponentInParent<VRRig>() : null);
	}

	// Token: 0x06003EF4 RID: 16116 RVA: 0x00152D7C File Offset: 0x00150F7C
	void IRigAware.SetRig(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x06003EF5 RID: 16117 RVA: 0x00152D9F File Offset: 0x00150F9F
	public static implicit operator bool(OwnerRig or)
	{
		return or != null && !(or == null) && or._rig != null && !(or._rig == null);
	}

	// Token: 0x06003EF6 RID: 16118 RVA: 0x00152DCC File Offset: 0x00150FCC
	public static implicit operator VRRig(OwnerRig or)
	{
		if (!or)
		{
			return null;
		}
		return or._rig;
	}

	// Token: 0x04004F43 RID: 20291
	[SerializeField]
	private VRRig _rig;
}
