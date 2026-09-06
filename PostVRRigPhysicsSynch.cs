using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000CDD RID: 3293
[DefaultExecutionOrder(9999)]
public class PostVRRigPhysicsSynch : MonoBehaviour
{
	// Token: 0x06005181 RID: 20865 RVA: 0x001AFC18 File Offset: 0x001ADE18
	private void LateUpdate()
	{
		for (int i = 0; i < PostVRRigPhysicsSynch.k_syncList.Count; i++)
		{
			AutoSyncTransforms autoSyncTransforms = PostVRRigPhysicsSynch.k_syncList[i];
			Transform targetTransform = autoSyncTransforms.TargetTransform;
			Rigidbody targetRigidbody = autoSyncTransforms.TargetRigidbody;
			Vector3 position = targetTransform.position;
			Quaternion rotation = targetTransform.rotation;
			targetRigidbody.position = position;
			targetRigidbody.rotation = rotation;
		}
	}

	// Token: 0x06005182 RID: 20866 RVA: 0x001AFC6C File Offset: 0x001ADE6C
	public static void AddSyncTarget(AutoSyncTransforms body)
	{
		PostVRRigPhysicsSynch.k_syncList.Add(body);
	}

	// Token: 0x06005183 RID: 20867 RVA: 0x001AFC79 File Offset: 0x001ADE79
	public static void RemoveSyncTarget(AutoSyncTransforms body)
	{
		PostVRRigPhysicsSynch.k_syncList.Remove(body);
	}

	// Token: 0x04006396 RID: 25494
	private static readonly List<AutoSyncTransforms> k_syncList = new List<AutoSyncTransforms>(5);
}
