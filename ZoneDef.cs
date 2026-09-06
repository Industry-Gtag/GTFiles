using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000E8B RID: 3723
public class ZoneDef : MonoBehaviour
{
	// Token: 0x1700089F RID: 2207
	// (get) Token: 0x06005A68 RID: 23144 RVA: 0x001D5AEC File Offset: 0x001D3CEC
	public GroupJoinZoneAB groupZoneAB
	{
		get
		{
			return new GroupJoinZoneAB
			{
				a = this.groupZone,
				b = this.groupZoneB
			};
		}
	}

	// Token: 0x06005A69 RID: 23145 RVA: 0x001D5B1C File Offset: 0x001D3D1C
	public bool IsSameZone(ZoneDef other)
	{
		return !(other == null) && this.zoneId == other.zoneId && this.subZoneId == other.subZoneId;
	}

	// Token: 0x04006B8E RID: 27534
	public GTZone zoneId;

	// Token: 0x04006B8F RID: 27535
	[FormerlySerializedAs("subZoneType")]
	[FormerlySerializedAs("subZone")]
	public GTSubZone subZoneId;

	// Token: 0x04006B90 RID: 27536
	public GroupJoinZoneA groupZone;

	// Token: 0x04006B91 RID: 27537
	public GroupJoinZoneB groupZoneB;

	// Token: 0x04006B92 RID: 27538
	public int trackStayIntervalSec = 30;

	// Token: 0x04006B93 RID: 27539
	[Space]
	public bool trackEnter = true;

	// Token: 0x04006B94 RID: 27540
	public bool trackExit;

	// Token: 0x04006B95 RID: 27541
	public bool trackStay = true;
}
