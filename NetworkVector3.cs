using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000DBB RID: 3515
internal class NetworkVector3
{
	// Token: 0x1700083F RID: 2111
	// (get) Token: 0x06005653 RID: 22099 RVA: 0x001C384A File Offset: 0x001C1A4A
	public Vector3 CurrentSyncTarget
	{
		get
		{
			return this._currentSyncTarget;
		}
	}

	// Token: 0x06005654 RID: 22100 RVA: 0x001C3854 File Offset: 0x001C1A54
	public void SetNewSyncTarget(Vector3 newTarget)
	{
		Vector3 currentSyncTarget = this.CurrentSyncTarget;
		(ref currentSyncTarget).SetValueSafe(in newTarget);
		this.distanceTraveled = currentSyncTarget - this._currentSyncTarget;
		this._currentSyncTarget = currentSyncTarget;
		this.lastSetNetTime = PhotonNetwork.Time;
	}

	// Token: 0x06005655 RID: 22101 RVA: 0x001C3898 File Offset: 0x001C1A98
	public Vector3 GetPredictedFuture()
	{
		float num = (float)(PhotonNetwork.Time - this.lastSetNetTime) * (float)PhotonNetwork.SerializationRate;
		Vector3 vector = this.distanceTraveled * num;
		return this._currentSyncTarget + vector;
	}

	// Token: 0x06005656 RID: 22102 RVA: 0x001C38D3 File Offset: 0x001C1AD3
	public void Reset()
	{
		this._currentSyncTarget = Vector3.zero;
		this.distanceTraveled = Vector3.zero;
		this.lastSetNetTime = 0.0;
	}

	// Token: 0x04006774 RID: 26484
	private double lastSetNetTime;

	// Token: 0x04006775 RID: 26485
	private Vector3 _currentSyncTarget = Vector3.zero;

	// Token: 0x04006776 RID: 26486
	private Vector3 distanceTraveled = Vector3.zero;
}
