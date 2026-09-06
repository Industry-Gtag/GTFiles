using System;
using UnityEngine;

// Token: 0x02000E0D RID: 3597
public class SpawnManager : MonoBehaviour
{
	// Token: 0x06005810 RID: 22544 RVA: 0x001CAECE File Offset: 0x001C90CE
	public Transform[] ChildrenXfs()
	{
		return base.transform.GetComponentsInChildren<Transform>();
	}
}
