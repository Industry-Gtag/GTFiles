using System;
using UnityEngine;

// Token: 0x02000E4B RID: 3659
public class ThrowableBugBeacon : MonoBehaviour
{
	// Token: 0x140000A0 RID: 160
	// (add) Token: 0x06005956 RID: 22870 RVA: 0x001D0B78 File Offset: 0x001CED78
	// (remove) Token: 0x06005957 RID: 22871 RVA: 0x001D0BAC File Offset: 0x001CEDAC
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnCall;

	// Token: 0x140000A1 RID: 161
	// (add) Token: 0x06005958 RID: 22872 RVA: 0x001D0BE0 File Offset: 0x001CEDE0
	// (remove) Token: 0x06005959 RID: 22873 RVA: 0x001D0C14 File Offset: 0x001CEE14
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnDismiss;

	// Token: 0x140000A2 RID: 162
	// (add) Token: 0x0600595A RID: 22874 RVA: 0x001D0C48 File Offset: 0x001CEE48
	// (remove) Token: 0x0600595B RID: 22875 RVA: 0x001D0C7C File Offset: 0x001CEE7C
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnLock;

	// Token: 0x140000A3 RID: 163
	// (add) Token: 0x0600595C RID: 22876 RVA: 0x001D0CB0 File Offset: 0x001CEEB0
	// (remove) Token: 0x0600595D RID: 22877 RVA: 0x001D0CE4 File Offset: 0x001CEEE4
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnUnlock;

	// Token: 0x140000A4 RID: 164
	// (add) Token: 0x0600595E RID: 22878 RVA: 0x001D0D18 File Offset: 0x001CEF18
	// (remove) Token: 0x0600595F RID: 22879 RVA: 0x001D0D4C File Offset: 0x001CEF4C
	public static event ThrowableBugBeacon.ThrowableBugBeaconFloatEvent OnChangeSpeedMultiplier;

	// Token: 0x1700087F RID: 2175
	// (get) Token: 0x06005960 RID: 22880 RVA: 0x001D0D7F File Offset: 0x001CEF7F
	public ThrowableBug.BugName BugName
	{
		get
		{
			return this.bugName;
		}
	}

	// Token: 0x17000880 RID: 2176
	// (get) Token: 0x06005961 RID: 22881 RVA: 0x001D0D87 File Offset: 0x001CEF87
	public float Range
	{
		get
		{
			return this.range;
		}
	}

	// Token: 0x06005962 RID: 22882 RVA: 0x001D0D8F File Offset: 0x001CEF8F
	public void Call()
	{
		if (ThrowableBugBeacon.OnCall != null)
		{
			ThrowableBugBeacon.OnCall(this);
		}
	}

	// Token: 0x06005963 RID: 22883 RVA: 0x001D0DA3 File Offset: 0x001CEFA3
	public void Dismiss()
	{
		if (ThrowableBugBeacon.OnDismiss != null)
		{
			ThrowableBugBeacon.OnDismiss(this);
		}
	}

	// Token: 0x06005964 RID: 22884 RVA: 0x001D0DB7 File Offset: 0x001CEFB7
	public void Lock()
	{
		if (ThrowableBugBeacon.OnLock != null)
		{
			ThrowableBugBeacon.OnLock(this);
		}
	}

	// Token: 0x06005965 RID: 22885 RVA: 0x001D0DCB File Offset: 0x001CEFCB
	public void Unlock()
	{
		if (ThrowableBugBeacon.OnUnlock != null)
		{
			ThrowableBugBeacon.OnUnlock(this);
		}
	}

	// Token: 0x06005966 RID: 22886 RVA: 0x001D0DDF File Offset: 0x001CEFDF
	public void ChangeSpeedMultiplier(float f)
	{
		if (ThrowableBugBeacon.OnChangeSpeedMultiplier != null)
		{
			ThrowableBugBeacon.OnChangeSpeedMultiplier(this, f);
		}
	}

	// Token: 0x06005967 RID: 22887 RVA: 0x001D0DCB File Offset: 0x001CEFCB
	private void OnDisable()
	{
		if (ThrowableBugBeacon.OnUnlock != null)
		{
			ThrowableBugBeacon.OnUnlock(this);
		}
	}

	// Token: 0x040069C1 RID: 27073
	[SerializeField]
	private float range;

	// Token: 0x040069C2 RID: 27074
	[SerializeField]
	private ThrowableBug.BugName bugName;

	// Token: 0x02000E4C RID: 3660
	// (Invoke) Token: 0x0600596A RID: 22890
	public delegate void ThrowableBugBeaconEvent(ThrowableBugBeacon tbb);

	// Token: 0x02000E4D RID: 3661
	// (Invoke) Token: 0x0600596E RID: 22894
	public delegate void ThrowableBugBeaconFloatEvent(ThrowableBugBeacon tbb, float f);
}
