using System;
using System.Threading.Tasks;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000E69 RID: 3689
public class TransformRotator : MonoBehaviour
{
	// Token: 0x06005A04 RID: 23044 RVA: 0x001D2E6C File Offset: 0x001D106C
	private async void Start()
	{
		this.baseRotation = base.transform.localRotation;
		base.gameObject.SetActive(false);
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
		Debug.Log(string.Format("TransformRotator :: anchor = {0}", this.anchor));
		if (base.gameObject)
		{
			base.gameObject.SetActive(true);
		}
	}

	// Token: 0x06005A05 RID: 23045 RVA: 0x001D2EA4 File Offset: 0x001D10A4
	private void LateUpdate()
	{
		this.UpdateRotation((GorillaComputer.instance.GetServerTime() - this.anchor).TotalSeconds);
	}

	// Token: 0x06005A06 RID: 23046 RVA: 0x001D2ED8 File Offset: 0x001D10D8
	private void UpdateRotation(double t)
	{
		float num = this.degreesPerSecond * (float)t;
		if (this.sinAmp != 0f)
		{
			num = this.sinAmp * Mathf.Sin(num / this.sinAmp);
		}
		Quaternion quaternion = Quaternion.AngleAxis(num, this.axis);
		base.transform.localRotation = this.baseRotation * quaternion;
	}

	// Token: 0x04006A54 RID: 27220
	[SerializeField]
	private Vector3 axis = Vector3.forward;

	// Token: 0x04006A55 RID: 27221
	[SerializeField]
	private float degreesPerSecond = 90f;

	// Token: 0x04006A56 RID: 27222
	[SerializeField]
	private float sinAmp;

	// Token: 0x04006A57 RID: 27223
	private Quaternion baseRotation;

	// Token: 0x04006A58 RID: 27224
	private DateTime anchor;
}
