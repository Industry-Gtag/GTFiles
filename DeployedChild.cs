using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002BF RID: 703
public class DeployedChild : MonoBehaviour
{
	// Token: 0x0600122F RID: 4655 RVA: 0x00061AE0 File Offset: 0x0005FCE0
	public void Deploy(DeployableObject parent, Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, bool isRemote = false)
	{
		this._parent = parent;
		this._parent.DeployChild();
		Transform transform = base.transform;
		transform.position = launchPos;
		transform.rotation = launchRot;
		transform.localScale = this._parent.transform.lossyScale;
		this._rigidbody.linearVelocity = releaseVel;
		this._isRemote = isRemote;
	}

	// Token: 0x06001230 RID: 4656 RVA: 0x00061B3D File Offset: 0x0005FD3D
	public void ReturnToParent(float delay)
	{
		if (delay > 0f)
		{
			base.StartCoroutine(this.ReturnToParentDelayed(delay));
			return;
		}
		if (this._parent != null)
		{
			this._parent.ReturnChild();
		}
	}

	// Token: 0x06001231 RID: 4657 RVA: 0x00061B6F File Offset: 0x0005FD6F
	private IEnumerator ReturnToParentDelayed(float delay)
	{
		float start = Time.time;
		while (Time.time < start + delay)
		{
			yield return null;
		}
		if (this._parent != null)
		{
			this._parent.ReturnChild();
		}
		yield break;
	}

	// Token: 0x040015FA RID: 5626
	[SerializeField]
	private Rigidbody _rigidbody;

	// Token: 0x040015FB RID: 5627
	[SerializeReference]
	private DeployableObject _parent;

	// Token: 0x040015FC RID: 5628
	private bool _isRemote;
}
