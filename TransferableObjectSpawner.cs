using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000934 RID: 2356
public class TransferableObjectSpawner : MonoBehaviour
{
	// Token: 0x06003DC2 RID: 15810 RVA: 0x0014EDB4 File Offset: 0x0014CFB4
	public void Awake()
	{
		GameObject[] transferrableObjectsToSpawn = this.TransferrableObjectsToSpawn;
		for (int i = 0; i < transferrableObjectsToSpawn.Length; i++)
		{
			TransferrableObject componentInChildren = transferrableObjectsToSpawn[i].GetComponentInChildren<TransferrableObject>();
			if (componentInChildren.IsNotNull())
			{
				this.objectsToSpawn.Add(componentInChildren);
			}
			else
			{
				Debug.LogError("Failed to add object " + componentInChildren.gameObject.name + " - missing a Transferrable object");
			}
		}
	}

	// Token: 0x06003DC3 RID: 15811 RVA: 0x0014EE14 File Offset: 0x0014D014
	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			return;
		}
		foreach (GameObject gameObject in this.TransferrableObjectsToSpawn)
		{
			TransferrableObject componentInChildren = gameObject.GetComponentInChildren<TransferrableObject>();
			if (componentInChildren.IsNull())
			{
				Debug.LogError(string.Concat(new string[]
				{
					base.name,
					" at path ",
					this.GetComponentPath(int.MaxValue),
					" has ",
					gameObject.name,
					" assigned to TransferrableObjectsToSpawn collection, but it does not have a TransferrableObject component.  It will not spawn."
				}));
			}
			else if (componentInChildren.worldShareableInstance == null)
			{
				Debug.LogError(string.Concat(new string[]
				{
					base.name,
					" at path ",
					this.GetComponentPath(int.MaxValue),
					" has ",
					gameObject.name,
					" assigned to TransferrableObjectsToSpawn collection, but it's worldShareableInstance is null."
				}));
			}
		}
	}

	// Token: 0x06003DC4 RID: 15812 RVA: 0x0014EEF7 File Offset: 0x0014D0F7
	public void Update()
	{
		if (this.spawnTrigger == TransferableObjectSpawner.SpawnTrigger.Timer && PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient && PhotonNetwork.Time > this.lastSpawnTime + this.SpawnDelay)
		{
			this.SpawnTransferrableObject();
			this.lastSpawnTime = PhotonNetwork.Time;
		}
	}

	// Token: 0x06003DC5 RID: 15813 RVA: 0x0014EF34 File Offset: 0x0014D134
	private bool SpawnOnGround()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(base.transform.position + Random.insideUnitCircle.x0y() * this.spawnRadius, Vector3.down), out raycastHit, 3f, this.groundRaycastMask))
		{
			this.spawnPosition = raycastHit.point;
			this.spawnRotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
			return true;
		}
		return false;
	}

	// Token: 0x06003DC6 RID: 15814 RVA: 0x0014EFB0 File Offset: 0x0014D1B0
	private void SpawnAtCurrentLocation()
	{
		this.spawnPosition = base.transform.position;
		this.spawnRotation = base.transform.rotation;
	}

	// Token: 0x06003DC7 RID: 15815 RVA: 0x0014EFD4 File Offset: 0x0014D1D4
	public void SpawnTransferrableObject()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		TransferableObjectSpawner.SpawnMode spawnMode = this.spawnMode;
		if (spawnMode != TransferableObjectSpawner.SpawnMode.OnGround)
		{
			if (spawnMode != TransferableObjectSpawner.SpawnMode.AtCurrentTransform)
			{
				return;
			}
			this.SpawnAtCurrentLocation();
		}
		else if (!this.SpawnOnGround())
		{
			return;
		}
		TransferrableObject transferrableObject = null;
		int num = 0;
		foreach (TransferrableObject transferrableObject2 in this.objectsToSpawn)
		{
			if (!transferrableObject2.InHand())
			{
				num++;
				if (Random.Range(0, num) == 0)
				{
					transferrableObject = transferrableObject2;
				}
			}
		}
		if (transferrableObject != null)
		{
			if (!transferrableObject.IsLocalOwnedWorldShareable)
			{
				transferrableObject.WorldShareableRequestOwnership();
			}
			if (transferrableObject.worldShareableInstance != null)
			{
				transferrableObject.transform.SetPositionAndRotation(this.spawnPosition, this.spawnRotation);
				transferrableObject.worldShareableInstance.SetWillTeleport();
				return;
			}
			Debug.LogError("WorldShareableInstance for " + transferrableObject.name + " is null");
		}
	}

	// Token: 0x04004E6C RID: 20076
	private Vector3 spawnPosition = Vector3.zero;

	// Token: 0x04004E6D RID: 20077
	private Quaternion spawnRotation = Quaternion.identity;

	// Token: 0x04004E6E RID: 20078
	[SerializeField]
	private GameObject[] TransferrableObjectsToSpawn;

	// Token: 0x04004E6F RID: 20079
	private List<TransferrableObject> objectsToSpawn = new List<TransferrableObject>();

	// Token: 0x04004E70 RID: 20080
	[SerializeField]
	private TransferableObjectSpawner.SpawnMode spawnMode;

	// Token: 0x04004E71 RID: 20081
	[SerializeField]
	private TransferableObjectSpawner.SpawnTrigger spawnTrigger;

	// Token: 0x04004E72 RID: 20082
	[SerializeField]
	private double SpawnDelay = 5.0;

	// Token: 0x04004E73 RID: 20083
	private double lastSpawnTime;

	// Token: 0x04004E74 RID: 20084
	[SerializeField]
	private LayerMask groundRaycastMask = LayerMask.NameToLayer("Gorilla Object");

	// Token: 0x04004E75 RID: 20085
	[SerializeField]
	private float spawnRadius = 0.5f;

	// Token: 0x02000935 RID: 2357
	private enum SpawnMode
	{
		// Token: 0x04004E77 RID: 20087
		OnGround,
		// Token: 0x04004E78 RID: 20088
		AtCurrentTransform
	}

	// Token: 0x02000936 RID: 2358
	private enum SpawnTrigger
	{
		// Token: 0x04004E7A RID: 20090
		Timer
	}
}
