using System;
using System.Threading.Tasks;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000E66 RID: 3686
public class TransformOrbiter : MonoBehaviour
{
	// Token: 0x060059F3 RID: 23027 RVA: 0x001D25EC File Offset: 0x001D07EC
	private async void Start()
	{
		base.gameObject.SetActive(false);
		if (!(this.barycenter == null))
		{
			while ((base.gameObject && GorillaComputer.instance == null) || GorillaComputer.instance.GetServerTime().Year < 2000)
			{
				await Task.Yield();
			}
			this.anchor = DateTime.Parse(GorillaComputer.instance.buildDate);
			while ((GorillaComputer.instance.GetServerTime() - this.anchor).TotalDays > 14.0)
			{
				this.anchor = this.anchor.AddDays(14.0);
				await Task.Yield();
			}
			Debug.Log(string.Format("TransformOrbiter :: anchor = {0}", this.anchor));
			if (base.gameObject)
			{
				base.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x060059F4 RID: 23028 RVA: 0x001D2624 File Offset: 0x001D0824
	private void LateUpdate()
	{
		this.UpdatePosRot((GorillaComputer.instance.GetServerTime() - this.anchor).TotalSeconds);
	}

	// Token: 0x060059F5 RID: 23029 RVA: 0x001D2658 File Offset: 0x001D0858
	private void UpdatePosRot(double t)
	{
		base.transform.position = this.GetPositionAtTime(t);
		if (this.faceBarycenter)
		{
			base.transform.rotation = Quaternion.LookRotation(this.barycenter.position - base.transform.position);
		}
	}

	// Token: 0x060059F6 RID: 23030 RVA: 0x001D26AC File Offset: 0x001D08AC
	private Vector3 GetPositionAtTime(double t)
	{
		double num = Math.Sin(t * this.speed);
		double num2 = Math.Cos(t * this.speed);
		double num3 = Math.Cos(t * this.speed);
		if (this.absoluteOrbitX)
		{
			num = Math.Abs(num);
		}
		if (this.absoluteOrbitY)
		{
			num2 = Math.Abs(num2);
		}
		if (this.absoluteOrbitZ)
		{
			num3 = Math.Abs(num3);
		}
		double num4 = (double)this.orbit.x * num;
		double num5 = (double)this.orbit.y * num2;
		double num6 = (double)this.orbit.z * num3;
		return this.barycenter.position + this.translation + new Vector3((float)num4, (float)num5, (float)num6);
	}

	// Token: 0x060059F7 RID: 23031 RVA: 0x001D2767 File Offset: 0x001D0967
	private bool validateBarycenter()
	{
		return this.validateBarycenter(base.transform);
	}

	// Token: 0x060059F8 RID: 23032 RVA: 0x001D2778 File Offset: 0x001D0978
	private bool validateBarycenter(Transform t)
	{
		if (this.barycenter == null)
		{
			Debug.LogError("The Barycenter cannot be null!");
			return false;
		}
		if (this.barycenter == t)
		{
			Debug.LogError("You cannot use the TransformOrbiter's own transform, or one nested below it, as its Barycenter!");
			return false;
		}
		for (int i = 0; i < t.childCount; i++)
		{
			if (!this.validateBarycenter(t.GetChild(i)))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04006A35 RID: 27189
	[SerializeField]
	private Transform barycenter;

	// Token: 0x04006A36 RID: 27190
	[SerializeField]
	private Vector3 orbit;

	// Token: 0x04006A37 RID: 27191
	[SerializeField]
	private Vector3 translation;

	// Token: 0x04006A38 RID: 27192
	[SerializeField]
	[Range(0.01f, 10f)]
	private double speed = 1.0;

	// Token: 0x04006A39 RID: 27193
	private double orbitTime;

	// Token: 0x04006A3A RID: 27194
	[SerializeField]
	private bool faceBarycenter;

	// Token: 0x04006A3B RID: 27195
	[SerializeField]
	private bool absoluteOrbitX;

	// Token: 0x04006A3C RID: 27196
	[SerializeField]
	private bool absoluteOrbitY;

	// Token: 0x04006A3D RID: 27197
	[SerializeField]
	private bool absoluteOrbitZ;

	// Token: 0x04006A3E RID: 27198
	private DateTime anchor;
}
