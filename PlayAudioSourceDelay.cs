using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000032 RID: 50
[RequireComponent(typeof(AudioSource))]
public class PlayAudioSourceDelay : MonoBehaviour
{
	// Token: 0x060000B8 RID: 184 RVA: 0x000057C5 File Offset: 0x000039C5
	public IEnumerator Start()
	{
		yield return new WaitForSecondsRealtime(this._delay);
		base.GetComponent<AudioSource>().GTPlay();
		yield break;
	}

	// Token: 0x040000D6 RID: 214
	[SerializeField]
	private float _delay;
}
