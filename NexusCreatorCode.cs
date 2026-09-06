using System;
using UnityEngine;

// Token: 0x02000489 RID: 1161
[CreateAssetMenu(fileName = "NexusCreatorCode", menuName = "Nexus/NexusCreatorCode")]
public class NexusCreatorCode : ScriptableObject
{
	// Token: 0x1700030A RID: 778
	// (get) Token: 0x06001C55 RID: 7253 RVA: 0x00099A6B File Offset: 0x00097C6B
	public string Code
	{
		get
		{
			return this.code;
		}
	}

	// Token: 0x1700030B RID: 779
	// (get) Token: 0x06001C56 RID: 7254 RVA: 0x00099A73 File Offset: 0x00097C73
	public NexusGroupId GroupId
	{
		get
		{
			return this.groupId;
		}
	}

	// Token: 0x04002670 RID: 9840
	[SerializeField]
	private string code;

	// Token: 0x04002671 RID: 9841
	[SerializeField]
	private NexusGroupId groupId;
}
