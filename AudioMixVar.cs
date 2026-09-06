using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020005F7 RID: 1527
[Serializable]
public class AudioMixVar
{
	// Token: 0x170003FC RID: 1020
	// (get) Token: 0x060025FF RID: 9727 RVA: 0x000C9338 File Offset: 0x000C7538
	// (set) Token: 0x06002600 RID: 9728 RVA: 0x000C9387 File Offset: 0x000C7587
	public float value
	{
		get
		{
			if (!this.group)
			{
				return 0f;
			}
			if (!this.mixer)
			{
				return 0f;
			}
			float num;
			if (!this.mixer.GetFloat(this.name, out num))
			{
				return 0f;
			}
			return num;
		}
		set
		{
			if (this.mixer)
			{
				this.mixer.SetFloat(this.name, value);
			}
		}
	}

	// Token: 0x06002601 RID: 9729 RVA: 0x000C93A9 File Offset: 0x000C75A9
	public void ReturnToPool()
	{
		if (this._pool != null)
		{
			this._pool.Return(this);
		}
	}

	// Token: 0x0400318C RID: 12684
	public AudioMixerGroup group;

	// Token: 0x0400318D RID: 12685
	public AudioMixer mixer;

	// Token: 0x0400318E RID: 12686
	public string name;

	// Token: 0x0400318F RID: 12687
	[NonSerialized]
	public bool taken;

	// Token: 0x04003190 RID: 12688
	[SerializeField]
	private AudioMixVarPool _pool;
}
