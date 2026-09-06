using System;
using System.Collections;
using UnityEngine;

// Token: 0x020005F4 RID: 1524
public class AtticHider : MonoBehaviour
{
	// Token: 0x060025F1 RID: 9713 RVA: 0x000C910D File Offset: 0x000C730D
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x060025F2 RID: 9714 RVA: 0x000C913B File Offset: 0x000C733B
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x060025F3 RID: 9715 RVA: 0x000C9164 File Offset: 0x000C7364
	private void OnZoneChanged()
	{
		if (this.AtticRenderer == null)
		{
			return;
		}
		if (ZoneManagement.instance.IsZoneActive(GTZone.attic))
		{
			if (this._coroutine != null)
			{
				base.StopCoroutine(this._coroutine);
				this._coroutine = null;
			}
			this._coroutine = base.StartCoroutine(this.WaitForAtticLoad());
			return;
		}
		if (this._coroutine != null)
		{
			base.StopCoroutine(this._coroutine);
			this._coroutine = null;
		}
		this.AtticRenderer.enabled = true;
	}

	// Token: 0x060025F4 RID: 9716 RVA: 0x000C91E3 File Offset: 0x000C73E3
	private IEnumerator WaitForAtticLoad()
	{
		while (!ZoneManagement.instance.IsSceneLoaded(GTZone.attic))
		{
			yield return new WaitForSeconds(0.2f);
		}
		yield return null;
		this.AtticRenderer.enabled = false;
		this._coroutine = null;
		yield break;
	}

	// Token: 0x04003186 RID: 12678
	[SerializeField]
	private MeshRenderer AtticRenderer;

	// Token: 0x04003187 RID: 12679
	private Coroutine _coroutine;
}
