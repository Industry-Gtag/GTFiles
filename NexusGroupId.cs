using System;
using UnityEngine;

// Token: 0x0200048A RID: 1162
[CreateAssetMenu(fileName = "NexusGroupId", menuName = "Nexus/NexusGroupId")]
public class NexusGroupId : ScriptableObject
{
	// Token: 0x1700030C RID: 780
	// (get) Token: 0x06001C58 RID: 7256 RVA: 0x00099A7B File Offset: 0x00097C7B
	public string Code
	{
		get
		{
			return this.code;
		}
	}

	// Token: 0x04002672 RID: 9842
	[SerializeField]
	private string code;

	// Token: 0x04002673 RID: 9843
	[SerializeField]
	private string sandboxCode;
}
