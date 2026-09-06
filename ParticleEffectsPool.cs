using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

// Token: 0x020002ED RID: 749
public class ParticleEffectsPool : MonoBehaviour
{
	// Token: 0x06001314 RID: 4884 RVA: 0x000657F9 File Offset: 0x000639F9
	public void Awake()
	{
		this.OnPoolAwake();
		this.Setup();
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnPoolAwake()
	{
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x00065808 File Offset: 0x00063A08
	private void Setup()
	{
		this.MoveToSceneWorldRoot();
		this._pools = new RingBuffer<ParticleEffect>[this.effects.Length];
		this._effectToPool = new Dictionary<long, int>(this.effects.Length);
		for (int i = 0; i < this.effects.Length; i++)
		{
			ParticleEffect particleEffect = this.effects[i];
			this._pools[i] = this.InitPoolForPrefab(i, this.effects[i]);
			this._effectToPool.TryAdd(particleEffect.effectID, i);
		}
	}

	// Token: 0x06001317 RID: 4887 RVA: 0x00065887 File Offset: 0x00063A87
	private void MoveToSceneWorldRoot()
	{
		Transform transform = base.transform;
		transform.parent = null;
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x000658B8 File Offset: 0x00063AB8
	private RingBuffer<ParticleEffect> InitPoolForPrefab(int index, ParticleEffect prefab)
	{
		RingBuffer<ParticleEffect> ringBuffer = new RingBuffer<ParticleEffect>(this.poolSize);
		string text = prefab.name.Trim();
		for (int i = 0; i < this.poolSize; i++)
		{
			ParticleEffect particleEffect = Object.Instantiate<ParticleEffect>(prefab, base.transform);
			particleEffect.gameObject.SetActive(false);
			particleEffect.pool = this;
			particleEffect.poolIndex = index;
			particleEffect.name = ZString.Concat<string, string, int>(text, "*", i);
			ringBuffer.Push(particleEffect);
		}
		return ringBuffer;
	}

	// Token: 0x06001319 RID: 4889 RVA: 0x00065930 File Offset: 0x00063B30
	public void PlayEffect(ParticleEffect effect, Vector3 worldPos)
	{
		this.PlayEffect(effect.effectID, worldPos);
	}

	// Token: 0x0600131A RID: 4890 RVA: 0x0006593F File Offset: 0x00063B3F
	public void PlayEffect(ParticleEffect effect, Vector3 worldPos, float delay)
	{
		this.PlayEffect(effect.effectID, worldPos, delay);
	}

	// Token: 0x0600131B RID: 4891 RVA: 0x0006594F File Offset: 0x00063B4F
	public void PlayEffect(long effectID, Vector3 worldPos)
	{
		this.PlayEffect(this.GetPoolIndex(effectID), worldPos);
	}

	// Token: 0x0600131C RID: 4892 RVA: 0x0006595F File Offset: 0x00063B5F
	public void PlayEffect(long effectID, Vector3 worldPos, float delay)
	{
		this.PlayEffect(this.GetPoolIndex(effectID), worldPos, delay);
	}

	// Token: 0x0600131D RID: 4893 RVA: 0x00065970 File Offset: 0x00063B70
	public void PlayEffect(int index, Vector3 worldPos)
	{
		if (index == -1)
		{
			return;
		}
		ParticleEffect particleEffect;
		if (!this._pools[index].TryPop(out particleEffect))
		{
			return;
		}
		particleEffect.transform.localPosition = worldPos;
		particleEffect.Play();
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x000659A6 File Offset: 0x00063BA6
	public void PlayEffect(int index, Vector3 worldPos, float delay)
	{
		if (delay.Approx(0f, 1E-06f))
		{
			this.PlayEffect(index, worldPos);
			return;
		}
		base.StartCoroutine(this.PlayDelayed(index, worldPos, delay));
	}

	// Token: 0x0600131F RID: 4895 RVA: 0x000659D3 File Offset: 0x00063BD3
	private IEnumerator PlayDelayed(int index, Vector3 worldPos, float delay)
	{
		yield return new WaitForSeconds(delay);
		this.PlayEffect(index, worldPos);
		yield break;
	}

	// Token: 0x06001320 RID: 4896 RVA: 0x000659F7 File Offset: 0x00063BF7
	public void Return(ParticleEffect effect)
	{
		this._pools[effect.poolIndex].Push(effect);
	}

	// Token: 0x06001321 RID: 4897 RVA: 0x00065A10 File Offset: 0x00063C10
	public int GetPoolIndex(long effectID)
	{
		int num;
		if (this._effectToPool.TryGetValue(effectID, out num))
		{
			return num;
		}
		return -1;
	}

	// Token: 0x04001757 RID: 5975
	public ParticleEffect[] effects = new ParticleEffect[0];

	// Token: 0x04001758 RID: 5976
	[Space]
	public int poolSize = 10;

	// Token: 0x04001759 RID: 5977
	[Space]
	private RingBuffer<ParticleEffect>[] _pools = new RingBuffer<ParticleEffect>[0];

	// Token: 0x0400175A RID: 5978
	private Dictionary<long, int> _effectToPool = new Dictionary<long, int>();
}
