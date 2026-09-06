using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B0E RID: 2830
public class ObjectToggle : MonoBehaviour
{
	// Token: 0x0600486E RID: 18542 RVA: 0x00185BCA File Offset: 0x00183DCA
	public void Toggle(bool initialState = true)
	{
		if (this._toggled == null)
		{
			if (initialState)
			{
				this.Enable();
				return;
			}
			this.Disable();
			return;
		}
		else
		{
			if (this._toggled.Value)
			{
				this.Disable();
				return;
			}
			this.Enable();
			return;
		}
	}

	// Token: 0x0600486F RID: 18543 RVA: 0x00185C04 File Offset: 0x00183E04
	public void Enable()
	{
		if (this.objectsToToggle == null)
		{
			return;
		}
		for (int i = 0; i < this.objectsToToggle.Count; i++)
		{
			GameObject gameObject = this.objectsToToggle[i];
			if (!(gameObject == null))
			{
				if (this._ignoreHierarchyState)
				{
					gameObject.SetActive(true);
				}
				else if (!gameObject.activeInHierarchy)
				{
					gameObject.SetActive(true);
				}
			}
		}
		this._toggled = new bool?(true);
	}

	// Token: 0x06004870 RID: 18544 RVA: 0x00185C74 File Offset: 0x00183E74
	public void Disable()
	{
		if (this.objectsToToggle == null)
		{
			return;
		}
		for (int i = 0; i < this.objectsToToggle.Count; i++)
		{
			GameObject gameObject = this.objectsToToggle[i];
			if (!(gameObject == null))
			{
				if (this._ignoreHierarchyState)
				{
					gameObject.SetActive(false);
				}
				else if (gameObject.activeInHierarchy)
				{
					gameObject.SetActive(false);
				}
			}
		}
		this._toggled = new bool?(false);
	}

	// Token: 0x04005B03 RID: 23299
	public List<GameObject> objectsToToggle = new List<GameObject>();

	// Token: 0x04005B04 RID: 23300
	[SerializeField]
	private bool _ignoreHierarchyState;

	// Token: 0x04005B05 RID: 23301
	[NonSerialized]
	private bool? _toggled;
}
