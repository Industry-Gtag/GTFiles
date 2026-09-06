using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000569 RID: 1385
public class SmoothLoop : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
{
	// Token: 0x0600232E RID: 9006 RVA: 0x000BCD31 File Offset: 0x000BAF31
	public bool BuildValidationCheck()
	{
		if (this.source == null)
		{
			Debug.LogError("missing audio source, this will fail", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x0600232F RID: 9007 RVA: 0x000BCD54 File Offset: 0x000BAF54
	private void Start()
	{
		if (this.delay != 0f && !this.randomStart)
		{
			this.source.GTStop();
			base.StartCoroutine(this.DelayedStart());
			return;
		}
		if (this.randomStart)
		{
			if (this.source.isActiveAndEnabled)
			{
				this.source.GTPlay();
			}
			this.source.time = Random.Range(0f, this.source.clip.length);
		}
	}

	// Token: 0x06002330 RID: 9008 RVA: 0x000BCDD4 File Offset: 0x000BAFD4
	public void SliceUpdate()
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.source.time > this.source.clip.length * this.loopEnd)
		{
			this.source.time = this.loopStart;
		}
	}

	// Token: 0x06002331 RID: 9009 RVA: 0x000BCE14 File Offset: 0x000BB014
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (!this.sourceCheck())
		{
			return;
		}
		if (this.randomStart)
		{
			if (this.source.isActiveAndEnabled)
			{
				this.source.GTPlay();
			}
			this.source.time = Random.Range(0f, this.source.clip.length);
		}
	}

	// Token: 0x06002332 RID: 9010 RVA: 0x00019269 File Offset: 0x00017469
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002333 RID: 9011 RVA: 0x000BCE78 File Offset: 0x000BB078
	private bool sourceCheck()
	{
		if (!this.source || !this.source.clip)
		{
			Debug.LogError("SmoothLoop: Disabling because AudioSource is null or has no clip assigned. Path: " + base.transform.GetPathQ(), this);
			base.enabled = false;
			base.StopAllCoroutines();
			return false;
		}
		return true;
	}

	// Token: 0x06002334 RID: 9012 RVA: 0x000BCECF File Offset: 0x000BB0CF
	public IEnumerator DelayedStart()
	{
		if (!this.sourceCheck())
		{
			yield break;
		}
		yield return new WaitForSeconds(this.delay);
		this.source.GTPlay();
		yield break;
	}

	// Token: 0x04002E43 RID: 11843
	public AudioSource source;

	// Token: 0x04002E44 RID: 11844
	public float delay;

	// Token: 0x04002E45 RID: 11845
	public bool randomStart;

	// Token: 0x04002E46 RID: 11846
	[SerializeField]
	[Range(0f, 1f)]
	private float loopStart = 0.1f;

	// Token: 0x04002E47 RID: 11847
	[SerializeField]
	[Range(0f, 1f)]
	private float loopEnd = 0.95f;
}
