using System;
using GorillaTagScripts.GhostReactor;
using TMPro;
using UnityEngine;

// Token: 0x020007DF RID: 2015
public class GRRecyclerScanner : MonoBehaviour
{
	// Token: 0x06003365 RID: 13157 RVA: 0x00119284 File Offset: 0x00117484
	private void Awake()
	{
		this.titleText.text = "";
		this.descriptionText.text = "";
		this.annotationText.text = "";
		this.recycleValueText.text = "";
	}

	// Token: 0x06003366 RID: 13158 RVA: 0x001192D4 File Offset: 0x001174D4
	public void ScanItem(GameEntityId id)
	{
		if (this.recycler != null && this.recycler.reactor != null && this.recycler.reactor.grManager != null && this.recycler.reactor.grManager.gameEntityManager != null)
		{
			GameEntity gameEntity = this.recycler.reactor.grManager.gameEntityManager.GetGameEntity(id);
			if (gameEntity == null)
			{
				return;
			}
			GRScannable component = gameEntity.GetComponent<GRScannable>();
			if (component == null)
			{
				return;
			}
			this.titleText.text = component.GetTitleText(this.recycler.reactor);
			this.descriptionText.text = component.GetBodyText(this.recycler.reactor);
			this.annotationText.text = component.GetAnnotationText(this.recycler.reactor);
			this.recycleValueText.text = string.Format("Recycle value: {0}", this.recycler.GetRecycleValue(gameEntity.gameObject.GetToolType()));
			this.audioSource.volume = this.recyclerBarcodeAudioVolume;
			this.audioSource.PlayOneShot(this.recyclerBarcodeAudio);
		}
	}

	// Token: 0x06003367 RID: 13159 RVA: 0x00119420 File Offset: 0x00117620
	private void OnTriggerEnter(Collider other)
	{
		if (this.recycler.reactor == null)
		{
			return;
		}
		if (!this.recycler.reactor.grManager.IsAuthority())
		{
			return;
		}
		GRScannable componentInParent = other.gameObject.GetComponentInParent<GRScannable>();
		if (componentInParent == null)
		{
			return;
		}
		this.recycler.reactor.grManager.RequestRecycleScanItem(componentInParent.gameEntity.id);
	}

	// Token: 0x040042B1 RID: 17073
	public GRRecycler recycler;

	// Token: 0x040042B2 RID: 17074
	[SerializeField]
	private TextMeshPro titleText;

	// Token: 0x040042B3 RID: 17075
	[SerializeField]
	private TextMeshPro descriptionText;

	// Token: 0x040042B4 RID: 17076
	[SerializeField]
	private TextMeshPro annotationText;

	// Token: 0x040042B5 RID: 17077
	[SerializeField]
	private TextMeshPro recycleValueText;

	// Token: 0x040042B6 RID: 17078
	public AudioSource audioSource;

	// Token: 0x040042B7 RID: 17079
	public AudioClip recyclerBarcodeAudio;

	// Token: 0x040042B8 RID: 17080
	public float recyclerBarcodeAudioVolume = 0.5f;
}
