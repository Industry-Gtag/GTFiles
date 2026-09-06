using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B0D RID: 2829
public class ObjectGroup : MonoBehaviour
{
	// Token: 0x0600486A RID: 18538 RVA: 0x00185A54 File Offset: 0x00183C54
	private void OnEnable()
	{
		if (this.syncWithGroupState)
		{
			this.SetObjectStates(true);
		}
	}

	// Token: 0x0600486B RID: 18539 RVA: 0x00185A65 File Offset: 0x00183C65
	private void OnDisable()
	{
		if (this.syncWithGroupState)
		{
			this.SetObjectStates(false);
		}
	}

	// Token: 0x0600486C RID: 18540 RVA: 0x00185A78 File Offset: 0x00183C78
	public void SetObjectStates(bool active)
	{
		int count = this.gameObjects.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = this.gameObjects[i];
			if (!(gameObject == null))
			{
				gameObject.SetActive(active);
			}
		}
		int count2 = this.behaviours.Count;
		for (int j = 0; j < count2; j++)
		{
			Behaviour behaviour = this.behaviours[j];
			if (!(behaviour == null))
			{
				behaviour.enabled = active;
			}
		}
		int count3 = this.renderers.Count;
		for (int k = 0; k < count3; k++)
		{
			Renderer renderer = this.renderers[k];
			if (!(renderer == null))
			{
				renderer.enabled = active;
			}
		}
		int count4 = this.colliders.Count;
		for (int l = 0; l < count4; l++)
		{
			Collider collider = this.colliders[l];
			if (!(collider == null))
			{
				collider.enabled = active;
			}
		}
	}

	// Token: 0x04005AFE RID: 23294
	public List<GameObject> gameObjects = new List<GameObject>(16);

	// Token: 0x04005AFF RID: 23295
	public List<Behaviour> behaviours = new List<Behaviour>(16);

	// Token: 0x04005B00 RID: 23296
	public List<Renderer> renderers = new List<Renderer>(16);

	// Token: 0x04005B01 RID: 23297
	public List<Collider> colliders = new List<Collider>(16);

	// Token: 0x04005B02 RID: 23298
	public bool syncWithGroupState = true;
}
