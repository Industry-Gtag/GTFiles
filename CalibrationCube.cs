using System;
using System.Collections.Generic;
using System.Reflection;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Animations.Rigging;

// Token: 0x020004EF RID: 1263
public class CalibrationCube : MonoBehaviour
{
	// Token: 0x06001EAE RID: 7854 RVA: 0x000A3D4B File Offset: 0x000A1F4B
	private void Awake()
	{
		this.calibratedLength = this.baseLength;
	}

	// Token: 0x06001EAF RID: 7855 RVA: 0x000A3D5C File Offset: 0x000A1F5C
	private void Start()
	{
		try
		{
			this.OnCollisionExit(null);
		}
		catch
		{
		}
	}

	// Token: 0x06001EB0 RID: 7856 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnTriggerEnter(Collider other)
	{
	}

	// Token: 0x06001EB1 RID: 7857 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnTriggerExit(Collider other)
	{
	}

	// Token: 0x06001EB2 RID: 7858 RVA: 0x000A3D88 File Offset: 0x000A1F88
	public void RecalibrateSize(bool pressed)
	{
		this.lastCalibratedLength = this.calibratedLength;
		this.calibratedLength = (this.rightController.transform.position - this.leftController.transform.position).magnitude;
		this.calibratedLength = ((this.calibratedLength > this.maxLength) ? this.maxLength : ((this.calibratedLength < this.minLength) ? this.minLength : this.calibratedLength));
		float num = this.calibratedLength / this.lastCalibratedLength;
		Vector3 localScale = this.playerBody.transform.localScale;
		this.playerBody.GetComponentInChildren<RigBuilder>().Clear();
		this.playerBody.transform.localScale = new Vector3(1f, 1f, 1f);
		this.playerBody.GetComponentInChildren<TransformReset>().ResetTransforms();
		this.playerBody.transform.localScale = num * localScale;
		this.playerBody.GetComponentInChildren<RigBuilder>().Build();
		this.playerBody.GetComponentInChildren<VRRig>().SetHeadBodyOffset();
		GorillaPlaySpace.Instance.bodyColliderOffset *= num;
		GorillaPlaySpace.Instance.bodyCollider.gameObject.transform.localScale *= num;
	}

	// Token: 0x06001EB3 RID: 7859 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnCollisionEnter(Collision collision)
	{
	}

	// Token: 0x06001EB4 RID: 7860 RVA: 0x000A3EE4 File Offset: 0x000A20E4
	private void OnCollisionExit(Collision collision)
	{
		try
		{
			bool flag = false;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				AssemblyName name = assemblies[i].GetName();
				if (!this.calibrationPresetsTest3[0].Contains(name.Name))
				{
					flag = true;
				}
			}
			if (!flag || Application.platform == RuntimePlatform.Android)
			{
				GorillaComputer.instance.includeUpdatedServerSynchTest = 0;
			}
		}
		catch
		{
		}
	}

	// Token: 0x040028F3 RID: 10483
	public PrimaryButtonWatcher watcher;

	// Token: 0x040028F4 RID: 10484
	public GameObject rightController;

	// Token: 0x040028F5 RID: 10485
	public GameObject leftController;

	// Token: 0x040028F6 RID: 10486
	public GameObject playerBody;

	// Token: 0x040028F7 RID: 10487
	private float calibratedLength;

	// Token: 0x040028F8 RID: 10488
	private float lastCalibratedLength;

	// Token: 0x040028F9 RID: 10489
	public float minLength = 1f;

	// Token: 0x040028FA RID: 10490
	public float maxLength = 2.5f;

	// Token: 0x040028FB RID: 10491
	public float baseLength = 1.61f;

	// Token: 0x040028FC RID: 10492
	public string[] calibrationPresets;

	// Token: 0x040028FD RID: 10493
	public string[] calibrationPresetsTest;

	// Token: 0x040028FE RID: 10494
	public string[] calibrationPresetsTest2;

	// Token: 0x040028FF RID: 10495
	public string[] calibrationPresetsTest3;

	// Token: 0x04002900 RID: 10496
	public string[] calibrationPresetsTest4;

	// Token: 0x04002901 RID: 10497
	public string outputstring;

	// Token: 0x04002902 RID: 10498
	private List<string> stringList = new List<string>();
}
