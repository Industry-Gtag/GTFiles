using System;
using UnityEngine;

// Token: 0x02000E39 RID: 3641
[CreateAssetMenu(fileName = "New TeleportNode Definition", menuName = "Teleportation/TeleportNode Definition", order = 1)]
public class TeleportNodeDefinition : ScriptableObject
{
	// Token: 0x1700087C RID: 2172
	// (get) Token: 0x060058FF RID: 22783 RVA: 0x001CEEB1 File Offset: 0x001CD0B1
	public TeleportNode Forward
	{
		get
		{
			return this.forward;
		}
	}

	// Token: 0x1700087D RID: 2173
	// (get) Token: 0x06005900 RID: 22784 RVA: 0x001CEEB9 File Offset: 0x001CD0B9
	public TeleportNode Backward
	{
		get
		{
			return this.backward;
		}
	}

	// Token: 0x06005901 RID: 22785 RVA: 0x001CEEC1 File Offset: 0x001CD0C1
	public void SetForward(TeleportNode node)
	{
		Debug.Log("registered fwd node " + node.name);
		this.forward = node;
	}

	// Token: 0x06005902 RID: 22786 RVA: 0x001CEEDF File Offset: 0x001CD0DF
	public void SetBackward(TeleportNode node)
	{
		Debug.Log("registered bkwd node " + node.name);
		this.backward = node;
	}

	// Token: 0x04006933 RID: 26931
	[SerializeField]
	private TeleportNode forward;

	// Token: 0x04006934 RID: 26932
	[SerializeField]
	private TeleportNode backward;
}
