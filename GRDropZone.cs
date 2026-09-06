using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000774 RID: 1908
public class GRDropZone : MonoBehaviour
{
	// Token: 0x0600305E RID: 12382 RVA: 0x001064EF File Offset: 0x001046EF
	private void Awake()
	{
		this.repelDirectionWorld = base.transform.TransformDirection(this.repelDirectionLocal.normalized);
	}

	// Token: 0x0600305F RID: 12383 RVA: 0x00106510 File Offset: 0x00104710
	private void OnTriggerEnter(Collider other)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		GameEntity component = other.attachedRigidbody.GetComponent<GameEntity>();
		if (component != null && component.manager.ghostReactorManager != null)
		{
			GhostReactorManager.Get(component).EntityEnteredDropZone(component);
		}
	}

	// Token: 0x06003060 RID: 12384 RVA: 0x00106559 File Offset: 0x00104759
	public Vector3 GetRepelDirectionWorld()
	{
		return this.repelDirectionWorld;
	}

	// Token: 0x06003061 RID: 12385 RVA: 0x00106564 File Offset: 0x00104764
	public void PlayEffect()
	{
		if (this.vfxRoot != null && !this.playingEffect)
		{
			this.vfxRoot.SetActive(true);
			this.playingEffect = true;
			if (this.sfxPrefab != null)
			{
				ObjectPools.instance.Instantiate(this.sfxPrefab, base.transform.position, base.transform.rotation, true);
			}
			base.StartCoroutine(this.DelayedStopEffect());
		}
	}

	// Token: 0x06003062 RID: 12386 RVA: 0x001065DD File Offset: 0x001047DD
	private IEnumerator DelayedStopEffect()
	{
		yield return new WaitForSeconds(this.effectDuration);
		this.vfxRoot.SetActive(false);
		this.playingEffect = false;
		yield break;
	}

	// Token: 0x04003DE0 RID: 15840
	[SerializeField]
	private GameObject vfxRoot;

	// Token: 0x04003DE1 RID: 15841
	[SerializeField]
	private GameObject sfxPrefab;

	// Token: 0x04003DE2 RID: 15842
	public float effectDuration = 1f;

	// Token: 0x04003DE3 RID: 15843
	private bool playingEffect;

	// Token: 0x04003DE4 RID: 15844
	[SerializeField]
	private Vector3 repelDirectionLocal = Vector3.up;

	// Token: 0x04003DE5 RID: 15845
	private Vector3 repelDirectionWorld = Vector3.up;
}
