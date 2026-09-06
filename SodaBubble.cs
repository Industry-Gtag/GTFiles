using System;
using System.Collections;
using UnityEngine;

// Token: 0x020009E6 RID: 2534
public class SodaBubble : MonoBehaviour
{
	// Token: 0x06004108 RID: 16648 RVA: 0x0015ABBB File Offset: 0x00158DBB
	public void Pop()
	{
		base.StartCoroutine(this.PopCoroutine());
	}

	// Token: 0x06004109 RID: 16649 RVA: 0x0015ABCA File Offset: 0x00158DCA
	private IEnumerator PopCoroutine()
	{
		this.audioSource.GTPlay();
		this.bubbleMesh.gameObject.SetActive(false);
		this.bubbleCollider.gameObject.SetActive(false);
		yield return new WaitForSeconds(1f);
		this.bubbleMesh.gameObject.SetActive(true);
		this.bubbleCollider.gameObject.SetActive(true);
		ObjectPools.instance.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x040051A3 RID: 20899
	public MeshRenderer bubbleMesh;

	// Token: 0x040051A4 RID: 20900
	public Rigidbody body;

	// Token: 0x040051A5 RID: 20901
	public MeshCollider bubbleCollider;

	// Token: 0x040051A6 RID: 20902
	public AudioSource audioSource;
}
