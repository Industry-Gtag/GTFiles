using System;
using GTMathUtil;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000923 RID: 2339
public class MovingPlatform : BasePlatform
{
	// Token: 0x06003D38 RID: 15672 RVA: 0x0014CD1C File Offset: 0x0014AF1C
	public float InitTimeOffset()
	{
		return this.startPercentage * this.cycleLength;
	}

	// Token: 0x06003D39 RID: 15673 RVA: 0x0014CD2B File Offset: 0x0014AF2B
	private long InitTimeOffsetMs()
	{
		return (long)(this.InitTimeOffset() * 1000f);
	}

	// Token: 0x06003D3A RID: 15674 RVA: 0x0014CD3A File Offset: 0x0014AF3A
	private long NetworkTimeMs()
	{
		if (PhotonNetwork.InRoom)
		{
			return (long)((ulong)(PhotonNetwork.ServerTimestamp + int.MinValue) + (ulong)this.InitTimeOffsetMs());
		}
		return (long)(Time.time * 1000f);
	}

	// Token: 0x06003D3B RID: 15675 RVA: 0x0014CD63 File Offset: 0x0014AF63
	private long CycleLengthMs()
	{
		return (long)(this.cycleLength * 1000f);
	}

	// Token: 0x06003D3C RID: 15676 RVA: 0x0014CD74 File Offset: 0x0014AF74
	public double PlatformTime()
	{
		long num = this.NetworkTimeMs();
		long num2 = this.CycleLengthMs();
		return (double)(num - num / num2 * num2) / 1000.0;
	}

	// Token: 0x06003D3D RID: 15677 RVA: 0x0014CD9F File Offset: 0x0014AF9F
	public int CycleCount()
	{
		return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
	}

	// Token: 0x06003D3E RID: 15678 RVA: 0x0014CDB0 File Offset: 0x0014AFB0
	public float CycleCompletionPercent()
	{
		float num = (float)(this.PlatformTime() / (double)this.cycleLength);
		num = Mathf.Clamp(num, 0f, 1f);
		if (this.startDelay > 0f)
		{
			float num2 = this.startDelay / this.cycleLength;
			if (num <= num2)
			{
				num = 0f;
			}
			else
			{
				num = (num - num2) / (1f - num2);
			}
		}
		return num;
	}

	// Token: 0x06003D3F RID: 15679 RVA: 0x0014CE12 File Offset: 0x0014B012
	public bool CycleForward()
	{
		return (this.CycleCount() + (this.startNextCycle ? 1 : 0)) % 2 == 0;
	}

	// Token: 0x06003D40 RID: 15680 RVA: 0x0014CE2C File Offset: 0x0014B02C
	private void Awake()
	{
		if (this.platformType == MovingPlatform.PlatformType.Child)
		{
			return;
		}
		this.rb = base.GetComponent<Rigidbody>();
		this.initLocalRotation = base.transform.localRotation;
		if (this.pivot != null)
		{
			this.initOffset = this.pivot.transform.position - this.startXf.transform.position;
		}
		this.startPos = this.startXf.position;
		this.endPos = this.endXf.position;
		this.startRot = this.startXf.rotation;
		this.endRot = this.endXf.rotation;
		this.platformInitLocalPos = base.transform.localPosition;
		this.currT = this.startPercentage;
	}

	// Token: 0x06003D41 RID: 15681 RVA: 0x0014CEFC File Offset: 0x0014B0FC
	private void OnEnable()
	{
		if (this.platformType == MovingPlatform.PlatformType.Child)
		{
			return;
		}
		base.transform.localRotation = this.initLocalRotation;
		this.startPos = this.startXf.position;
		this.endPos = this.endXf.position;
		this.startRot = this.startXf.rotation;
		this.endRot = this.endXf.rotation;
		this.platformInitLocalPos = base.transform.localPosition;
		this.currT = this.startPercentage;
	}

	// Token: 0x06003D42 RID: 15682 RVA: 0x0014CF85 File Offset: 0x0014B185
	private Vector3 UpdatePointToPoint()
	{
		return Vector3.Lerp(this.startPos, this.endPos, this.smoothedPercent);
	}

	// Token: 0x06003D43 RID: 15683 RVA: 0x0014CFA0 File Offset: 0x0014B1A0
	private Vector3 UpdateArc()
	{
		float num = Mathf.Lerp(this.rotateStartAmt, this.rotateStartAmt + this.rotateAmt, this.smoothedPercent);
		Quaternion quaternion = this.initLocalRotation;
		Vector3 vector = Quaternion.AngleAxis(num, Vector3.forward) * this.initOffset;
		return this.pivot.transform.position + vector;
	}

	// Token: 0x06003D44 RID: 15684 RVA: 0x0014CFFE File Offset: 0x0014B1FE
	private Quaternion UpdateRotation()
	{
		return Quaternion.Slerp(this.startRot, this.endRot, this.smoothedPercent);
	}

	// Token: 0x06003D45 RID: 15685 RVA: 0x0014D017 File Offset: 0x0014B217
	private Quaternion UpdateContinuousRotation()
	{
		return Quaternion.AngleAxis(this.smoothedPercent * 360f, Vector3.up) * base.transform.parent.rotation;
	}

	// Token: 0x06003D46 RID: 15686 RVA: 0x0014D044 File Offset: 0x0014B244
	private void SetupContext()
	{
		double time = PhotonNetwork.Time;
		if (this.lastServerTime == time)
		{
			this.dtSinceServerUpdate += Time.fixedDeltaTime;
		}
		else
		{
			this.dtSinceServerUpdate = 0f;
			this.lastServerTime = time;
		}
		float num = this.currT;
		this.currT = this.CycleCompletionPercent();
		this.currForward = this.CycleForward();
		this.percent = this.currT;
		if (this.reverseDirOnCycle)
		{
			this.percent = (this.currForward ? this.currT : (1f - this.currT));
		}
		if (this.reverseDir)
		{
			this.percent = 1f - this.percent;
		}
		this.smoothedPercent = this.percent;
		this.lastNT = time;
		this.lastT = Time.time;
	}

	// Token: 0x06003D47 RID: 15687 RVA: 0x0014D114 File Offset: 0x0014B314
	private void Update()
	{
		if (this.platformType == MovingPlatform.PlatformType.Child)
		{
			return;
		}
		this.SetupContext();
		Vector3 vector = base.transform.position;
		Quaternion quaternion = base.transform.rotation;
		bool flag = false;
		switch (this.platformType)
		{
		case MovingPlatform.PlatformType.PointToPoint:
			vector = this.UpdatePointToPoint();
			break;
		case MovingPlatform.PlatformType.Arc:
			vector = this.UpdateArc();
			flag = true;
			break;
		case MovingPlatform.PlatformType.Rotation:
			quaternion = this.UpdateRotation();
			flag = true;
			break;
		case MovingPlatform.PlatformType.ContinuousRotation:
			quaternion = this.UpdateContinuousRotation();
			flag = true;
			break;
		}
		if (!this.debugMovement)
		{
			this.lastPos = this.rb.position;
			this.lastRot = this.rb.rotation;
			if (this.platformType != MovingPlatform.PlatformType.Rotation)
			{
				this.rb.MovePosition(vector);
			}
			if (flag)
			{
				this.rb.MoveRotation(quaternion);
			}
		}
		else
		{
			this.lastPos = base.transform.position;
			this.lastRot = base.transform.rotation;
			base.transform.position = vector;
			if (flag)
			{
				base.transform.rotation = quaternion;
			}
		}
		this.deltaPosition = vector - this.lastPos;
	}

	// Token: 0x06003D48 RID: 15688 RVA: 0x0014D235 File Offset: 0x0014B435
	public Vector3 ThisFrameMovement()
	{
		return this.deltaPosition;
	}

	// Token: 0x04004DE7 RID: 19943
	public MovingPlatform.PlatformType platformType;

	// Token: 0x04004DE8 RID: 19944
	public float cycleLength;

	// Token: 0x04004DE9 RID: 19945
	public float smoothingHalflife = 0.1f;

	// Token: 0x04004DEA RID: 19946
	public float rotateStartAmt;

	// Token: 0x04004DEB RID: 19947
	public float rotateAmt;

	// Token: 0x04004DEC RID: 19948
	public bool reverseDirOnCycle = true;

	// Token: 0x04004DED RID: 19949
	public bool reverseDir;

	// Token: 0x04004DEE RID: 19950
	private CriticalSpringDamper springCD = new CriticalSpringDamper();

	// Token: 0x04004DEF RID: 19951
	private Rigidbody rb;

	// Token: 0x04004DF0 RID: 19952
	public Transform startXf;

	// Token: 0x04004DF1 RID: 19953
	public Transform endXf;

	// Token: 0x04004DF2 RID: 19954
	public Vector3 platformInitLocalPos;

	// Token: 0x04004DF3 RID: 19955
	private Vector3 startPos;

	// Token: 0x04004DF4 RID: 19956
	private Vector3 endPos;

	// Token: 0x04004DF5 RID: 19957
	private Quaternion startRot;

	// Token: 0x04004DF6 RID: 19958
	private Quaternion endRot;

	// Token: 0x04004DF7 RID: 19959
	public float startPercentage;

	// Token: 0x04004DF8 RID: 19960
	public float startDelay;

	// Token: 0x04004DF9 RID: 19961
	public bool startNextCycle;

	// Token: 0x04004DFA RID: 19962
	public Transform pivot;

	// Token: 0x04004DFB RID: 19963
	private Quaternion initLocalRotation;

	// Token: 0x04004DFC RID: 19964
	private Vector3 initOffset;

	// Token: 0x04004DFD RID: 19965
	private float currT;

	// Token: 0x04004DFE RID: 19966
	private float percent;

	// Token: 0x04004DFF RID: 19967
	private float smoothedPercent = -1f;

	// Token: 0x04004E00 RID: 19968
	private bool currForward;

	// Token: 0x04004E01 RID: 19969
	private float dtSinceServerUpdate;

	// Token: 0x04004E02 RID: 19970
	private double lastServerTime;

	// Token: 0x04004E03 RID: 19971
	public Vector3 currentVelocity;

	// Token: 0x04004E04 RID: 19972
	public Vector3 rotationalAxis;

	// Token: 0x04004E05 RID: 19973
	public float angularVelocity;

	// Token: 0x04004E06 RID: 19974
	public Vector3 rotationPivot;

	// Token: 0x04004E07 RID: 19975
	public Vector3 lastPos;

	// Token: 0x04004E08 RID: 19976
	public Quaternion lastRot;

	// Token: 0x04004E09 RID: 19977
	public Vector3 deltaPosition;

	// Token: 0x04004E0A RID: 19978
	public bool debugMovement;

	// Token: 0x04004E0B RID: 19979
	private double lastNT;

	// Token: 0x04004E0C RID: 19980
	private float lastT;

	// Token: 0x02000924 RID: 2340
	public enum PlatformType
	{
		// Token: 0x04004E0E RID: 19982
		PointToPoint,
		// Token: 0x04004E0F RID: 19983
		Arc,
		// Token: 0x04004E10 RID: 19984
		Rotation,
		// Token: 0x04004E11 RID: 19985
		Child,
		// Token: 0x04004E12 RID: 19986
		ContinuousRotation
	}
}
