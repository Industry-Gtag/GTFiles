using System;
using UnityEngine;

// Token: 0x02000D7A RID: 3450
public class AmbientSoundRandomizer : MonoBehaviour
{
	// Token: 0x06005512 RID: 21778 RVA: 0x001BE256 File Offset: 0x001BC456
	private void Button_Cache()
	{
		this.audioSources = base.GetComponentsInChildren<AudioSource>();
	}

	// Token: 0x06005513 RID: 21779 RVA: 0x001BE264 File Offset: 0x001BC464
	private void Awake()
	{
		this.SetTarget();
	}

	// Token: 0x06005514 RID: 21780 RVA: 0x001BE26C File Offset: 0x001BC46C
	private void Update()
	{
		if (this.timer >= this.timerTarget)
		{
			int num = Random.Range(0, this.audioSources.Length);
			int num2 = Random.Range(0, this.audioClips.Length);
			this.audioSources[num].clip = this.audioClips[num2];
			this.audioSources[num].GTPlay();
			this.SetTarget();
			return;
		}
		this.timer += Time.deltaTime;
	}

	// Token: 0x06005515 RID: 21781 RVA: 0x001BE2E0 File Offset: 0x001BC4E0
	private void SetTarget()
	{
		this.timerTarget = this.baseTime + Random.Range(0f, this.randomModifier);
		this.timer = 0f;
	}

	// Token: 0x040066BC RID: 26300
	[SerializeField]
	private AudioSource[] audioSources;

	// Token: 0x040066BD RID: 26301
	[SerializeField]
	private AudioClip[] audioClips;

	// Token: 0x040066BE RID: 26302
	[SerializeField]
	private float baseTime = 15f;

	// Token: 0x040066BF RID: 26303
	[SerializeField]
	private float randomModifier = 5f;

	// Token: 0x040066C0 RID: 26304
	private float timer;

	// Token: 0x040066C1 RID: 26305
	private float timerTarget;
}
