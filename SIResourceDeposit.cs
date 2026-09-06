using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200015F RID: 351
public class SIResourceDeposit : MonoBehaviour, ISIResourceDeposit
{
	// Token: 0x170000BA RID: 186
	// (get) Token: 0x06000943 RID: 2371 RVA: 0x00032116 File Offset: 0x00030316
	public bool IsAuthority
	{
		get
		{
			return this.SIManager.gameEntityManager.IsAuthority();
		}
	}

	// Token: 0x170000BB RID: 187
	// (get) Token: 0x06000944 RID: 2372 RVA: 0x00032128 File Offset: 0x00030328
	public SuperInfectionManager SIManager
	{
		get
		{
			return this.superInfection.siManager;
		}
	}

	// Token: 0x06000945 RID: 2373 RVA: 0x00032138 File Offset: 0x00030338
	private void OnEnable()
	{
		if (this._displayResources == null || this._displayResources.Count == 0)
		{
			List<SIResource> resourcePrefabs = this.superInfection.ResourcePrefabs;
			if (resourcePrefabs != null && resourcePrefabs.Count > 0)
			{
				this._displayResources = new List<GameObject>();
				for (int i = 0; i < Mathf.Min(resourcePrefabs.Count, this.resourceDisplays.Length); i++)
				{
					GameObject gameObject = resourcePrefabs[i].gameObject;
					bool activeSelf = gameObject.activeSelf;
					try
					{
						if (activeSelf)
						{
							gameObject.SetActive(false);
						}
						GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, this.resourceDisplays[i].transform);
						gameObject2.transform.localScale = new Vector3(0.27f, 0.27f, 0.27f);
						this._displayResources.Add(gameObject2);
						foreach (MonoBehaviour monoBehaviour in gameObject2.GetComponentsInChildren<MonoBehaviour>(true))
						{
							monoBehaviour.enabled = false;
							Object.Destroy(monoBehaviour);
						}
						Rigidbody component = gameObject2.GetComponent<Rigidbody>();
						if (component != null)
						{
							Object.Destroy(component);
						}
						gameObject2.SetLayerRecursively(UnityLayer.Default);
						gameObject2.SetActive(true);
					}
					finally
					{
						if (activeSelf)
						{
							gameObject.SetActive(true);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x0003227C File Offset: 0x0003047C
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.netPlayer != null)
		{
			stream.SendNext(this.netPlayer.ActorNr);
		}
		else
		{
			stream.SendNext(-1);
		}
		stream.SendNext((int)this.netResourceType);
		stream.SendNext((int)this.netLimitedDepositType);
		stream.SendNext(this.netShowPopup);
		this.netShowPopup = false;
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x000322F8 File Offset: 0x000304F8
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.netPlayer = SIPlayer.Get((int)stream.ReceiveNext());
		this.netResourceType = (SIResource.ResourceType)((int)stream.ReceiveNext());
		this.netLimitedDepositType = (SIResource.LimitedDepositType)((int)stream.ReceiveNext());
		if ((bool)stream.ReceiveNext())
		{
			this.LocalShowPopup(this.netPlayer, this.netResourceType, this.netLimitedDepositType);
		}
	}

	// Token: 0x06000948 RID: 2376 RVA: 0x00032364 File Offset: 0x00030564
	private void LocalShowPopup(SIPlayer player, SIResource.ResourceType resourceType, SIResource.LimitedDepositType limitedDepositType)
	{
		if (limitedDepositType == SIResource.LimitedDepositType.None)
		{
			this.depositBin.SetActive(true);
		}
		this.popupScreen.EnableAndResetTimer();
		this.depositText.text = string.Format("{0} COLLECTED {1}\n(TOTAL {2})", player.gamePlayer.rig.Creator.SanitizedNickName, resourceType.GetName<SIResource.ResourceType>(), player.GetResourceAmount(resourceType));
		this.depositImage.sprite = ((resourceType == SIResource.ResourceType.TechPoint) ? this.resourceImageSprites[0] : this.resourceImageSprites[1]);
	}

	// Token: 0x06000949 RID: 2377 RVA: 0x000323E8 File Offset: 0x000305E8
	public void ResourceDeposited(SIResource resource)
	{
		bool flag = false;
		if (resource.lastPlayerHeld.gamePlayer.IsLocal() && !resource.localDeposited)
		{
			this.AuthShowPopup(resource);
			resource.HandleDepositLocal(resource.lastPlayerHeld);
			resource.lastPlayerHeld.GatherResource(resource.type, resource.limitedDepositType, 1);
			this.superInfection.siManager.CallRPC(SuperInfectionManager.ClientToAuthorityRPC.ResourceDepositDeposited, new object[]
			{
				resource.myGameEntity.GetNetId(),
				this.index
			});
			flag = true;
		}
		if (this.superInfection.siManager.gameEntityManager.IsAuthority())
		{
			resource.HandleDepositAuth(resource.lastPlayerHeld);
			this.superInfection.siManager.gameEntityManager.RequestDestroyItem(resource.myGameEntity.id);
			this.AuthShowPopup(resource);
			flag = true;
		}
		if (flag)
		{
			this.LocalShowPopup(resource.lastPlayerHeld, resource.type, resource.limitedDepositType);
		}
	}

	// Token: 0x0600094A RID: 2378 RVA: 0x000324DD File Offset: 0x000306DD
	private void AuthShowPopup(SIResource resource)
	{
		this.netPlayer = resource.lastPlayerHeld;
		this.netResourceType = resource.type;
		this.netLimitedDepositType = resource.limitedDepositType;
		this.netShowPopup = true;
	}

	// Token: 0x04000B57 RID: 2903
	public int index;

	// Token: 0x04000B58 RID: 2904
	public Text depositText;

	// Token: 0x04000B59 RID: 2905
	public Image depositImage;

	// Token: 0x04000B5A RID: 2906
	public DisableGameObjectDelayed popupScreen;

	// Token: 0x04000B5B RID: 2907
	public SuperInfection superInfection;

	// Token: 0x04000B5C RID: 2908
	public Sprite[] resourceImageSprites;

	// Token: 0x04000B5D RID: 2909
	public GameObject depositBin;

	// Token: 0x04000B5E RID: 2910
	[SerializeField]
	private Transform[] resourceDisplays;

	// Token: 0x04000B5F RID: 2911
	public SIPlayer netPlayer;

	// Token: 0x04000B60 RID: 2912
	public SIResource.ResourceType netResourceType;

	// Token: 0x04000B61 RID: 2913
	public SIResource.LimitedDepositType netLimitedDepositType;

	// Token: 0x04000B62 RID: 2914
	private bool netShowPopup;

	// Token: 0x04000B63 RID: 2915
	public List<SIUIPlayerQuestDisplay> questDisplays;

	// Token: 0x04000B64 RID: 2916
	private List<GameObject> _displayResources;
}
