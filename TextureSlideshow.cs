using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000034 RID: 52
public class TextureSlideshow : MonoBehaviour
{
	// Token: 0x060000C0 RID: 192 RVA: 0x00005844 File Offset: 0x00003A44
	private void Awake()
	{
		this._renderer = base.GetComponent<Renderer>();
		this._renderer.material.mainTexture = this.textures[0];
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x0000586A File Offset: 0x00003A6A
	private void OnEnable()
	{
		base.StartCoroutine(this.runSlideshow());
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x00005879 File Offset: 0x00003A79
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x00005881 File Offset: 0x00003A81
	private IEnumerator runSlideshow()
	{
		yield return new WaitForSecondsRealtime(this.prePause);
		int i = 0;
		for (;;)
		{
			yield return new WaitForSecondsRealtime(Random.Range(this.minMaxPause.x, this.minMaxPause.y));
			this._renderer.material.mainTexture = this.textures[i];
			i = (i + 1) % this.textures.Length;
		}
		yield break;
	}

	// Token: 0x040000DA RID: 218
	private Renderer _renderer;

	// Token: 0x040000DB RID: 219
	[SerializeField]
	private Texture[] textures;

	// Token: 0x040000DC RID: 220
	[SerializeField]
	private Vector2 minMaxPause;

	// Token: 0x040000DD RID: 221
	[SerializeField]
	private float prePause = 1f;
}
