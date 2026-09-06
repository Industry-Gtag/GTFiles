using System;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000074 RID: 116
public class CritterSpawnTrigger : MonoBehaviour
{
	// Token: 0x060002D5 RID: 725 RVA: 0x000113D4 File Offset: 0x0000F5D4
	private ValueDropdownList<int> GetCritterTypeList()
	{
		return new ValueDropdownList<int>();
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x000113DC File Offset: 0x0000F5DC
	private void OnTriggerEnter(Collider other)
	{
		if (!CrittersManager.instance.LocalAuthority())
		{
			return;
		}
		if (Time.realtimeSinceStartup < this._nextSpawnTime)
		{
			return;
		}
		CrittersActor componentInParent = other.GetComponentInParent<CrittersActor>();
		if (!componentInParent)
		{
			return;
		}
		if (componentInParent.crittersActorType != this.triggerActorType)
		{
			return;
		}
		if (this.requiredSubObjectIndex >= 0 && componentInParent.subObjectIndex != this.requiredSubObjectIndex)
		{
			return;
		}
		if (!string.IsNullOrEmpty(this.triggerActorName) && !componentInParent.GetActorSubtype().Contains(this.triggerActorName))
		{
			return;
		}
		CrittersManager.instance.DespawnActor(componentInParent);
		CrittersManager.instance.SpawnCritter(this.critterType, this.spawnPoint.position, this.spawnPoint.rotation);
		this._nextSpawnTime = Time.realtimeSinceStartup + this.triggerCooldown;
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x000114A6 File Offset: 0x0000F6A6
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(base.transform.position, this.spawnPoint.position);
		Gizmos.DrawWireSphere(this.spawnPoint.position, 0.1f);
	}

	// Token: 0x04000345 RID: 837
	[Header("Trigger Settings")]
	[SerializeField]
	private CrittersActor.CrittersActorType triggerActorType;

	// Token: 0x04000346 RID: 838
	[SerializeField]
	private int requiredSubObjectIndex = -1;

	// Token: 0x04000347 RID: 839
	[SerializeField]
	private string triggerActorName;

	// Token: 0x04000348 RID: 840
	[SerializeField]
	private float triggerCooldown = 1f;

	// Token: 0x04000349 RID: 841
	[Header("Spawn Settings")]
	[SerializeField]
	private Transform spawnPoint;

	// Token: 0x0400034A RID: 842
	[SerializeField]
	private int critterType;

	// Token: 0x0400034B RID: 843
	private float _nextSpawnTime;
}
