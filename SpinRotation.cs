using System;
using UnityEngine;

// Token: 0x020002FB RID: 763
public class SpinRotation : MonoBehaviour, ITickSystemTick
{
	// Token: 0x170001EC RID: 492
	// (get) Token: 0x06001375 RID: 4981 RVA: 0x00066EB0 File Offset: 0x000650B0
	// (set) Token: 0x06001376 RID: 4982 RVA: 0x00066EB8 File Offset: 0x000650B8
	public bool TickRunning { get; set; }

	// Token: 0x06001377 RID: 4983 RVA: 0x00066EC1 File Offset: 0x000650C1
	public void Tick()
	{
		base.transform.localRotation = Quaternion.Euler(this.rotationPerSecondEuler * (Time.time - this.baseTime)) * this.baseRotation;
	}

	// Token: 0x06001378 RID: 4984 RVA: 0x00066EF5 File Offset: 0x000650F5
	private void Awake()
	{
		this.baseRotation = base.transform.localRotation;
	}

	// Token: 0x06001379 RID: 4985 RVA: 0x00066F08 File Offset: 0x00065108
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
		this.baseTime = Time.time;
	}

	// Token: 0x0600137A RID: 4986 RVA: 0x0001A29F File Offset: 0x0001849F
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x040017CB RID: 6091
	[SerializeField]
	private Vector3 rotationPerSecondEuler;

	// Token: 0x040017CC RID: 6092
	private Quaternion baseRotation;

	// Token: 0x040017CD RID: 6093
	private float baseTime;
}
