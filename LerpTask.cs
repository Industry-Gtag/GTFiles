using System;
using UnityEngine;

// Token: 0x02000911 RID: 2321
public class LerpTask<T>
{
	// Token: 0x06003CCF RID: 15567 RVA: 0x0014B4BA File Offset: 0x001496BA
	public void Reset()
	{
		this.onLerp(this.lerpFrom, this.lerpTo, 0f);
		this.active = false;
		this.elapsed = 0f;
	}

	// Token: 0x06003CD0 RID: 15568 RVA: 0x0014B4EA File Offset: 0x001496EA
	public void Start(T from, T to, float duration)
	{
		this.lerpFrom = from;
		this.lerpTo = to;
		this.duration = duration;
		this.elapsed = 0f;
		this.active = true;
	}

	// Token: 0x06003CD1 RID: 15569 RVA: 0x0014B514 File Offset: 0x00149714
	public void Finish()
	{
		this.onLerp(this.lerpFrom, this.lerpTo, 1f);
		Action action = this.onLerpEnd;
		if (action != null)
		{
			action();
		}
		this.active = false;
		this.elapsed = 0f;
	}

	// Token: 0x06003CD2 RID: 15570 RVA: 0x0014B560 File Offset: 0x00149760
	public void Update()
	{
		if (!this.active)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (this.elapsed < this.duration)
		{
			float num = ((this.elapsed + deltaTime >= this.duration) ? 1f : (this.elapsed / this.duration));
			this.onLerp(this.lerpFrom, this.lerpTo, num);
			this.elapsed += deltaTime;
			return;
		}
		this.Finish();
	}

	// Token: 0x04004D77 RID: 19831
	public float elapsed;

	// Token: 0x04004D78 RID: 19832
	public float duration;

	// Token: 0x04004D79 RID: 19833
	public T lerpFrom;

	// Token: 0x04004D7A RID: 19834
	public T lerpTo;

	// Token: 0x04004D7B RID: 19835
	public Action<T, T, float> onLerp;

	// Token: 0x04004D7C RID: 19836
	public Action onLerpEnd;

	// Token: 0x04004D7D RID: 19837
	public bool active;
}
