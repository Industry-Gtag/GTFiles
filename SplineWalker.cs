using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000DE3 RID: 3555
public class SplineWalker : MonoBehaviour, IPunObservable
{
	// Token: 0x0600571E RID: 22302 RVA: 0x001C79F8 File Offset: 0x001C5BF8
	private void Awake()
	{
		this._view = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600571F RID: 22303 RVA: 0x001C7A08 File Offset: 0x001C5C08
	private void Update()
	{
		if (this.goingForward)
		{
			this.progress += Time.deltaTime / this.duration;
			if (this.progress > 1f)
			{
				if (this.mode == SplineWalkerMode.Once)
				{
					this.progress = 1f;
				}
				else if (this.mode == SplineWalkerMode.Loop)
				{
					this.progress -= 1f;
				}
				else
				{
					this.progress = 2f - this.progress;
					this.goingForward = false;
				}
			}
		}
		else
		{
			this.progress -= Time.deltaTime / this.duration;
			if (this.progress < 0f)
			{
				this.progress = -this.progress;
				this.goingForward = true;
			}
		}
		if (this.linearSpline != null && this.walkLinearPath)
		{
			Vector3 vector = this.linearSpline.Evaluate(this.progress);
			if (this.useWorldPosition)
			{
				base.transform.position = vector;
			}
			else
			{
				base.transform.localPosition = vector;
			}
			if (this.lookForward)
			{
				base.transform.LookAt(vector + this.linearSpline.GetForwardTangent(this.progress, 0.01f));
				return;
			}
		}
		else if (this.spline != null)
		{
			Vector3 point = this.spline.GetPoint(this.progress);
			if (this.useWorldPosition)
			{
				base.transform.position = point;
			}
			else
			{
				base.transform.localPosition = point;
			}
			if (this.lookForward)
			{
				base.transform.LookAt(point + this.spline.GetDirection(this.progress));
			}
		}
	}

	// Token: 0x06005720 RID: 22304 RVA: 0x001C7BB6 File Offset: 0x001C5DB6
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.Serialize(ref this.progress);
	}

	// Token: 0x040067E5 RID: 26597
	public BezierSpline spline;

	// Token: 0x040067E6 RID: 26598
	public LinearSpline linearSpline;

	// Token: 0x040067E7 RID: 26599
	public float duration;

	// Token: 0x040067E8 RID: 26600
	public bool lookForward;

	// Token: 0x040067E9 RID: 26601
	public SplineWalkerMode mode;

	// Token: 0x040067EA RID: 26602
	public bool walkLinearPath;

	// Token: 0x040067EB RID: 26603
	public bool useWorldPosition;

	// Token: 0x040067EC RID: 26604
	public float progress;

	// Token: 0x040067ED RID: 26605
	private bool goingForward = true;

	// Token: 0x040067EE RID: 26606
	public bool DoNetworkSync = true;

	// Token: 0x040067EF RID: 26607
	private PhotonView _view;
}
