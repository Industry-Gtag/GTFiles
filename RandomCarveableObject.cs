using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Voxels;

// Token: 0x020001EB RID: 491
[NetworkBehaviourWeaved(0)]
public class RandomCarveableObject : NetworkComponent
{
	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06000CD9 RID: 3289 RVA: 0x00046D6F File Offset: 0x00044F6F
	private bool HasAuthority
	{
		get
		{
			return VoxelManager.HasAuthority;
		}
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x00046D78 File Offset: 0x00044F78
	private new void Start()
	{
		if (this.world == null)
		{
			this.world = base.GetComponentInParent<VoxelWorld>();
		}
		this.spawnFX.SetActive(false);
		for (int i = this.spawnPoint.childCount - 1; i >= 0; i--)
		{
			JamUtil.Destroy(this.spawnPoint.GetChild(i).gameObject);
		}
		this.proximityTrigger.OnCountChanged += this.OnPlayerCountChanged;
		this.OnPlayerCountChanged();
		this.Init();
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x00046DF6 File Offset: 0x00044FF6
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		this.proximityTrigger.OnCountChanged -= this.OnPlayerCountChanged;
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x00046E15 File Offset: 0x00045015
	private void Init()
	{
		if (this.HasAuthority)
		{
			this.SpawnRandomCarveable();
			return;
		}
		if (this._version != 0 && this.IsValidPrefab(this._carveableIndex))
		{
			this.SpawnCarveable(this._carveableIndex);
		}
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x00046E48 File Offset: 0x00045048
	private void ClearWorld()
	{
		this.world.SetVoxels(this.world.WorldBounds, 0, this.materialId, false);
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x00046E68 File Offset: 0x00045068
	private void FillBounds()
	{
		this.SetBoundsDensity(byte.MaxValue);
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x00046E75 File Offset: 0x00045075
	private void SetBoundsDensity(byte density)
	{
		this.world.SetVoxels(this._voxels, density, this.materialId, false);
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x00046E90 File Offset: 0x00045090
	private void SetCarveable(int index)
	{
		this._carveableIndex = index;
		for (int i = this.spawnPoint.childCount - 1; i >= 0; i--)
		{
			JamUtil.Destroy(this.spawnPoint.GetChild(i).gameObject);
		}
		this._carveable = global::UnityEngine.Object.Instantiate<GameObject>(this.prefabs[this._carveableIndex], this.spawnPoint.position, this.spawnPoint.rotation, this.spawnPoint);
		this._carveable.transform.localScale = Vector3.one;
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x00046F1B File Offset: 0x0004511B
	private void OnPlayerCountChanged()
	{
		this.SetCanRequestNewCarveable(this.proximityTrigger.RigCount == 0);
	}

	// Token: 0x06000CE2 RID: 3298 RVA: 0x00046F31 File Offset: 0x00045131
	private void SetCanRequestNewCarveable(bool active)
	{
		if (this._buttonConfigured && this._canRequestNewCarveable == active)
		{
			return;
		}
		this.button.isOn = active;
		this.button.UpdateColor();
		this._buttonConfigured = true;
		this._canRequestNewCarveable = active;
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x00046F6A File Offset: 0x0004516A
	public void RequestSpawnRandomCarveable()
	{
		if (!this._canRequestNewCarveable)
		{
			return;
		}
		Debug.Log("RequestSpawnRandomCarveable()");
		if (this.HasAuthority)
		{
			this.SpawnRandomCarveable();
			return;
		}
		base.GetView.RPC("RPC_SpawnRandomCarveable", RpcTarget.MasterClient, Array.Empty<object>());
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x00046FA4 File Offset: 0x000451A4
	private bool IsValidAuthorityRPC(PhotonMessageInfo info)
	{
		return VoxelManager.HasAuthority && this.spamCheck.CheckCallTime(Time.unscaledTime);
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x00046FBF File Offset: 0x000451BF
	[PunRPC]
	public void RPC_SpawnRandomCarveable(PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RPC_SpawnRandomCarveable");
		if (!this.IsValidAuthorityRPC(info))
		{
			return;
		}
		this.SpawnRandomCarveable();
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x00046FDC File Offset: 0x000451DC
	private void SpawnRandomCarveable()
	{
		this.SpawnCarveable(global::UnityEngine.Random.Range(0, this.prefabs.Length));
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x00046FF4 File Offset: 0x000451F4
	private async void SpawnCarveable(int index)
	{
		try
		{
			if (!this.IsValidPrefab(index))
			{
				Debug.LogError(string.Format("Invalid index: {0}", index));
			}
			else
			{
				this.spawnFX.SetActive(false);
				await UniTask.WaitUntil(() => this.world.WorldGenerationComplete, PlayerLoopTiming.Update, default(CancellationToken));
				this.SetCarveable(index);
				this._carveable.gameObject.SetActive(false);
				this.world.ResetChunk(int3.zero);
				await UniTask.WaitUntil(() => this.world.BoundsChunksLoaded(this.world.WorldBounds, false), PlayerLoopTiming.PostLateUpdate, default(CancellationToken));
				this._carveable.gameObject.SetActive(true);
				this.spawnFX.SetActive(true);
				if (this.HasAuthority)
				{
					this.IncrementVersion();
				}
				else
				{
					VoxelManager.RequestWorldState(this.world);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x00047033 File Offset: 0x00045233
	private void IncrementVersion()
	{
		this._version++;
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x00047043 File Offset: 0x00045243
	private bool IsValidPrefab(int index)
	{
		return index >= 0 && index < this.prefabs.Length;
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06000CEC RID: 3308 RVA: 0x00047056 File Offset: 0x00045256
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this._carveableIndex);
		stream.SendNext(this._version);
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x0004707C File Offset: 0x0004527C
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != info.photonView.Owner)
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		int num2 = (int)stream.ReceiveNext();
		if (this._carveableIndex == num && this._version == num2)
		{
			return;
		}
		this.OnStateChange(num, num2);
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x000470D0 File Offset: 0x000452D0
	public void OnStateChange(int newIndex, int newVersion)
	{
		this._carveableIndex = newIndex;
		this._version = newVersion;
		this.IsValidPrefab(this._carveableIndex);
		if (this.IsValidPrefab(this._carveableIndex))
		{
			this.SpawnCarveable(this._carveableIndex);
		}
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x00002E6F File Offset: 0x0000106F
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06000CF3 RID: 3315 RVA: 0x00002E7B File Offset: 0x0000107B
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04000F8B RID: 3979
	[SerializeField]
	private Transform spawnPoint;

	// Token: 0x04000F8C RID: 3980
	[SerializeField]
	private GameObject[] prefabs;

	// Token: 0x04000F8D RID: 3981
	[SerializeField]
	private byte materialId;

	// Token: 0x04000F8E RID: 3982
	[SerializeField]
	private VoxelWorld world;

	// Token: 0x04000F8F RID: 3983
	[SerializeField]
	private GameObject spawnFX;

	// Token: 0x04000F90 RID: 3984
	[SerializeField]
	private CallLimiter spamCheck = new CallLimiter(1, 1f, 0.5f);

	// Token: 0x04000F91 RID: 3985
	[SerializeField]
	private RigEventVolume proximityTrigger;

	// Token: 0x04000F92 RID: 3986
	[SerializeField]
	private GorillaPressableButton button;

	// Token: 0x04000F93 RID: 3987
	private int _carveableIndex = -1;

	// Token: 0x04000F94 RID: 3988
	private int _version;

	// Token: 0x04000F95 RID: 3989
	private GameObject _carveable;

	// Token: 0x04000F96 RID: 3990
	private int3[] _voxels;

	// Token: 0x04000F97 RID: 3991
	private global::UnityEngine.BoundsInt _voxelBounds;

	// Token: 0x04000F98 RID: 3992
	private bool _buttonConfigured;

	// Token: 0x04000F99 RID: 3993
	private bool _canRequestNewCarveable;
}
