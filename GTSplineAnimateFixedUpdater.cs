using System;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Splines;

// Token: 0x02000351 RID: 849
[NetworkBehaviourWeaved(1)]
public class GTSplineAnimateFixedUpdater : NetworkComponent
{
	// Token: 0x060014E5 RID: 5349 RVA: 0x0006FCA3 File Offset: 0x0006DEA3
	protected override void Awake()
	{
		base.Awake();
		this.splineAnimateRef.AddCallbackOnLoad(new Action(this.InitSplineAnimate));
		this.splineAnimateRef.AddCallbackOnUnload(new Action(this.ClearSplineAnimate));
	}

	// Token: 0x060014E6 RID: 5350 RVA: 0x0006FCD9 File Offset: 0x0006DED9
	private void InitSplineAnimate()
	{
		this.isSplineLoaded = this.splineAnimateRef.TryResolve<SplineAnimate>(out this.splineAnimate);
		if (this.isSplineLoaded && this.splineAnimate != null)
		{
			this.splineAnimate.enabled = false;
		}
	}

	// Token: 0x060014E7 RID: 5351 RVA: 0x0006FD14 File Offset: 0x0006DF14
	private void ClearSplineAnimate()
	{
		this.splineAnimate = null;
		this.isSplineLoaded = false;
	}

	// Token: 0x060014E8 RID: 5352 RVA: 0x0006FD24 File Offset: 0x0006DF24
	private void FixedUpdate()
	{
		if (!base.IsMine && this.progressLerpStartTime + 1f > Time.time)
		{
			if (this.isSplineLoaded)
			{
				this.progress = Mathf.Lerp(this.progressLerpStart, this.progressLerpEnd, (Time.time - this.progressLerpStartTime) / 1f) % this.Duration;
				this.splineAnimate.NormalizedTime = this.progress / this.Duration;
				return;
			}
		}
		else
		{
			this.progress = (this.progress + Time.fixedDeltaTime) % this.Duration;
			if (this.isSplineLoaded)
			{
				this.splineAnimate.NormalizedTime = this.progress / this.Duration;
			}
		}
	}

	// Token: 0x17000215 RID: 533
	// (get) Token: 0x060014E9 RID: 5353 RVA: 0x0006FDD9 File Offset: 0x0006DFD9
	// (set) Token: 0x060014EA RID: 5354 RVA: 0x0006FDFF File Offset: 0x0006DFFF
	[Networked]
	[NetworkedWeaved(0, 1)]
	public unsafe float Netdata
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GTSplineAnimateFixedUpdater.Netdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(float*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GTSplineAnimateFixedUpdater.Netdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(float*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060014EB RID: 5355 RVA: 0x0006FE26 File Offset: 0x0006E026
	public override void WriteDataFusion()
	{
		this.Netdata = this.progress + 1f;
	}

	// Token: 0x060014EC RID: 5356 RVA: 0x0006FE3A File Offset: 0x0006E03A
	public override void ReadDataFusion()
	{
		this.SharedReadData(this.Netdata);
	}

	// Token: 0x060014ED RID: 5357 RVA: 0x0006FE48 File Offset: 0x0006E048
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.progress + 1f);
	}

	// Token: 0x060014EE RID: 5358 RVA: 0x0006FE70 File Offset: 0x0006E070
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		float num = (float)stream.ReceiveNext();
		this.SharedReadData(num);
	}

	// Token: 0x060014EF RID: 5359 RVA: 0x0006FEA0 File Offset: 0x0006E0A0
	private void SharedReadData(float incomingValue)
	{
		if (float.IsNaN(incomingValue) || incomingValue > this.Duration + 1f || incomingValue < 0f)
		{
			return;
		}
		this.progressLerpEnd = incomingValue;
		if (this.progressLerpEnd < this.progress)
		{
			if (this.progress < this.Duration)
			{
				this.progressLerpEnd += this.Duration;
			}
			else
			{
				this.progress -= this.Duration;
			}
		}
		this.progressLerpStart = this.progress;
		this.progressLerpStartTime = Time.time;
	}

	// Token: 0x060014F1 RID: 5361 RVA: 0x0006FF2F File Offset: 0x0006E12F
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Netdata = this._Netdata;
	}

	// Token: 0x060014F2 RID: 5362 RVA: 0x0006FF47 File Offset: 0x0006E147
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Netdata = this.Netdata;
	}

	// Token: 0x040019A2 RID: 6562
	[SerializeField]
	private XSceneRef splineAnimateRef;

	// Token: 0x040019A3 RID: 6563
	[SerializeField]
	private float Duration;

	// Token: 0x040019A4 RID: 6564
	private const float progressLerpDuration = 1f;

	// Token: 0x040019A5 RID: 6565
	private SplineAnimate splineAnimate;

	// Token: 0x040019A6 RID: 6566
	private bool isSplineLoaded;

	// Token: 0x040019A7 RID: 6567
	private float progress;

	// Token: 0x040019A8 RID: 6568
	private float progressLerpStart;

	// Token: 0x040019A9 RID: 6569
	private float progressLerpEnd;

	// Token: 0x040019AA RID: 6570
	private float progressLerpStartTime;

	// Token: 0x040019AB RID: 6571
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Netdata", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private float _Netdata;
}
