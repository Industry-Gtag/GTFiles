using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020009F1 RID: 2545
public class SpawnPooledObject : MonoBehaviour
{
	// Token: 0x06004159 RID: 16729 RVA: 0x0015BF3E File Offset: 0x0015A13E
	private void Awake()
	{
		if (this._pooledObject == null)
		{
			return;
		}
		this._pooledObjectHash = PoolUtils.GameObjHashCode(this._pooledObject);
	}

	// Token: 0x0600415A RID: 16730 RVA: 0x0015BF60 File Offset: 0x0015A160
	public void SpawnObject()
	{
		if (!this.ShouldSpawn())
		{
			return;
		}
		if (this._pooledObject == null || this._spawnLocation == null)
		{
			return;
		}
		GameObject gameObject = ObjectPools.instance.Instantiate(this._pooledObjectHash, true);
		gameObject.transform.position = this.SpawnLocation();
		gameObject.transform.rotation = this.SpawnRotation();
		gameObject.transform.localScale = base.transform.lossyScale;
	}

	// Token: 0x0600415B RID: 16731 RVA: 0x0015BFDB File Offset: 0x0015A1DB
	private Vector3 SpawnLocation()
	{
		return this._spawnLocation.transform.position + this.offset;
	}

	// Token: 0x0600415C RID: 16732 RVA: 0x0015BFF8 File Offset: 0x0015A1F8
	private Quaternion SpawnRotation()
	{
		Quaternion quaternion = this._spawnLocation.transform.rotation;
		if (this.facePlayer)
		{
			quaternion = Quaternion.LookRotation(GTPlayer.Instance.headCollider.transform.position - this._spawnLocation.transform.position);
		}
		if (this.upright)
		{
			quaternion.eulerAngles = new Vector3(0f, quaternion.eulerAngles.y, 0f);
		}
		return quaternion;
	}

	// Token: 0x0600415D RID: 16733 RVA: 0x0015C078 File Offset: 0x0015A278
	private bool ShouldSpawn()
	{
		return Random.Range(0, 100) < this.chanceToSpawn;
	}

	// Token: 0x040051F8 RID: 20984
	[SerializeField]
	private Transform _spawnLocation;

	// Token: 0x040051F9 RID: 20985
	[SerializeField]
	private GameObject _pooledObject;

	// Token: 0x040051FA RID: 20986
	[FormerlySerializedAs("_offset")]
	public Vector3 offset;

	// Token: 0x040051FB RID: 20987
	[FormerlySerializedAs("_upright")]
	public bool upright;

	// Token: 0x040051FC RID: 20988
	[FormerlySerializedAs("_facePlayer")]
	public bool facePlayer;

	// Token: 0x040051FD RID: 20989
	[FormerlySerializedAs("_chanceToSpawn")]
	[Range(0f, 100f)]
	public int chanceToSpawn = 100;

	// Token: 0x040051FE RID: 20990
	private int _pooledObjectHash;
}
