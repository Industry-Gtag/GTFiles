using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000B05 RID: 2821
public class LifeCycleEventTrigger : MonoBehaviour
{
	// Token: 0x06004838 RID: 18488 RVA: 0x00184DDF File Offset: 0x00182FDF
	private void Awake()
	{
		UnityEvent onAwake = this._onAwake;
		if (onAwake == null)
		{
			return;
		}
		onAwake.Invoke();
	}

	// Token: 0x06004839 RID: 18489 RVA: 0x00184DF1 File Offset: 0x00182FF1
	private void Start()
	{
		UnityEvent onStart = this._onStart;
		if (onStart == null)
		{
			return;
		}
		onStart.Invoke();
	}

	// Token: 0x0600483A RID: 18490 RVA: 0x00184E03 File Offset: 0x00183003
	private void OnEnable()
	{
		UnityEvent onEnable = this._onEnable;
		if (onEnable == null)
		{
			return;
		}
		onEnable.Invoke();
	}

	// Token: 0x0600483B RID: 18491 RVA: 0x00184E15 File Offset: 0x00183015
	private void OnDisable()
	{
		UnityEvent onDisable = this._onDisable;
		if (onDisable == null)
		{
			return;
		}
		onDisable.Invoke();
	}

	// Token: 0x0600483C RID: 18492 RVA: 0x00184E27 File Offset: 0x00183027
	private void OnDestroy()
	{
		UnityEvent onDestroy = this._onDestroy;
		if (onDestroy == null)
		{
			return;
		}
		onDestroy.Invoke();
	}

	// Token: 0x04005AA4 RID: 23204
	[SerializeField]
	private UnityEvent _onAwake;

	// Token: 0x04005AA5 RID: 23205
	[SerializeField]
	private UnityEvent _onStart;

	// Token: 0x04005AA6 RID: 23206
	[SerializeField]
	private UnityEvent _onEnable;

	// Token: 0x04005AA7 RID: 23207
	[SerializeField]
	private UnityEvent _onDisable;

	// Token: 0x04005AA8 RID: 23208
	[SerializeField]
	private UnityEvent _onDestroy;
}
