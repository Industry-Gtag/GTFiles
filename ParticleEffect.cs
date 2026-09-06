using System;
using UnityEngine;

// Token: 0x020002EC RID: 748
[RequireComponent(typeof(ParticleSystem))]
public class ParticleEffect : MonoBehaviour
{
	// Token: 0x170001E3 RID: 483
	// (get) Token: 0x0600130E RID: 4878 RVA: 0x0006576B File Offset: 0x0006396B
	public long effectID
	{
		get
		{
			return this._effectID;
		}
	}

	// Token: 0x170001E4 RID: 484
	// (get) Token: 0x0600130F RID: 4879 RVA: 0x00065773 File Offset: 0x00063973
	public bool isPlaying
	{
		get
		{
			return this.system && this.system.isPlaying;
		}
	}

	// Token: 0x06001310 RID: 4880 RVA: 0x0006578F File Offset: 0x0006398F
	public virtual void Play()
	{
		base.gameObject.SetActive(true);
		this.system.Play(true);
	}

	// Token: 0x06001311 RID: 4881 RVA: 0x000657A9 File Offset: 0x000639A9
	public virtual void Stop()
	{
		this.system.Stop(true);
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001312 RID: 4882 RVA: 0x000657C3 File Offset: 0x000639C3
	private void OnParticleSystemStopped()
	{
		base.gameObject.SetActive(false);
		if (this.pool)
		{
			this.pool.Return(this);
		}
	}

	// Token: 0x04001753 RID: 5971
	public ParticleSystem system;

	// Token: 0x04001754 RID: 5972
	[SerializeField]
	private long _effectID;

	// Token: 0x04001755 RID: 5973
	public ParticleEffectsPool pool;

	// Token: 0x04001756 RID: 5974
	[NonSerialized]
	public int poolIndex = -1;
}
