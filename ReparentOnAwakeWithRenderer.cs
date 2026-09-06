using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000392 RID: 914
public class ReparentOnAwakeWithRenderer : MonoBehaviour, IBuildValidation
{
	// Token: 0x06001618 RID: 5656 RVA: 0x00081155 File Offset: 0x0007F355
	public bool BuildValidationCheck()
	{
		if (base.GetComponent<MeshRenderer>() != null && this.myRenderer == null)
		{
			Debug.Log(base.name + " needs a reference to its renderer since it has one - ");
			return false;
		}
		return true;
	}

	// Token: 0x06001619 RID: 5657 RVA: 0x0008118C File Offset: 0x0007F38C
	private void OnEnable()
	{
		if (this.newParent.IsNotNull())
		{
			base.transform.SetParent(this.newParent, true);
			if (this.sortLast)
			{
				base.transform.SetAsLastSibling();
			}
			else
			{
				base.transform.SetAsFirstSibling();
			}
			if (this.myRenderer != null)
			{
				this.myRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
				this.myRenderer.lightProbeUsage = LightProbeUsage.Off;
				this.myRenderer.probeAnchor = this.newParent;
			}
		}
	}

	// Token: 0x0600161A RID: 5658 RVA: 0x0008120F File Offset: 0x0007F40F
	[ContextMenu("Set Renderer")]
	public void SetMyRenderer()
	{
		this.myRenderer = base.GetComponent<MeshRenderer>();
	}

	// Token: 0x0400206F RID: 8303
	public Transform newParent;

	// Token: 0x04002070 RID: 8304
	public MeshRenderer myRenderer;

	// Token: 0x04002071 RID: 8305
	[Tooltip("We're mostly using this for UI elements like text and images, so this will help you separate these in whatever target parent object.Keep images and texts together, otherwise you'll get extra draw calls. Put images above text or they'll overlap weird tho lol")]
	public bool sortLast;
}
