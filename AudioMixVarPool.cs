using System;
using UnityEngine;

// Token: 0x020005F6 RID: 1526
[CreateAssetMenu(fileName = "New AudioMixVarPool", menuName = "ScriptableObjects/AudioMixVarPool", order = 0)]
public class AudioMixVarPool : ScriptableObject
{
	// Token: 0x060025FC RID: 9724 RVA: 0x000C92A0 File Offset: 0x000C74A0
	public bool Rent(out AudioMixVar mixVar)
	{
		for (int i = 0; i < this._vars.Length; i++)
		{
			if (!this._vars[i].taken)
			{
				this._vars[i].taken = true;
				mixVar = this._vars[i];
				return true;
			}
		}
		mixVar = null;
		return false;
	}

	// Token: 0x060025FD RID: 9725 RVA: 0x000C92F0 File Offset: 0x000C74F0
	public void Return(AudioMixVar mixVar)
	{
		if (mixVar == null)
		{
			return;
		}
		int num = this._vars.IndexOfRef(mixVar);
		if (num == -1)
		{
			return;
		}
		this._vars[num].taken = false;
	}

	// Token: 0x0400318B RID: 12683
	[SerializeField]
	private AudioMixVar[] _vars = new AudioMixVar[0];
}
