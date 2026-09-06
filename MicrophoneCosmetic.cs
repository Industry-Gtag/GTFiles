using System;
using UnityEngine;

// Token: 0x0200022C RID: 556
public class MicrophoneCosmetic : MonoBehaviour
{
	// Token: 0x06000EB7 RID: 3767 RVA: 0x000501B8 File Offset: 0x0004E3B8
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		if (!Application.isEditor && Application.platform == RuntimePlatform.Android && Microphone.devices.Length != 0)
		{
			this.audioSource.clip = Microphone.Start(Microphone.devices[0], true, 10, 16000);
		}
		else
		{
			int sampleRate = AudioSettings.GetConfiguration().sampleRate;
			this.audioSource.clip = Microphone.Start(null, true, 10, sampleRate);
		}
		this.audioSource.loop = true;
	}

	// Token: 0x06000EB8 RID: 3768 RVA: 0x00050238 File Offset: 0x0004E438
	private void OnEnable()
	{
		int num = ((Application.platform == RuntimePlatform.Android && Microphone.devices.Length != 0) ? Microphone.GetPosition(Microphone.devices[0]) : Microphone.GetPosition(null));
		num -= 10;
		if ((float)num < 0f)
		{
			num = this.audioSource.clip.samples + num - 1;
		}
		this.audioSource.GTPlay();
		this.audioSource.timeSamples = num;
	}

	// Token: 0x06000EB9 RID: 3769 RVA: 0x000502A5 File Offset: 0x0004E4A5
	private void OnDisable()
	{
		this.audioSource.GTStop();
	}

	// Token: 0x06000EBA RID: 3770 RVA: 0x000502B4 File Offset: 0x0004E4B4
	private void Update()
	{
		Vector3 vector = this.mouthTransform.position - base.transform.position;
		float sqrMagnitude = vector.sqrMagnitude;
		float num = 0f;
		if (sqrMagnitude < this.mouthProximityRampRange.x * this.mouthProximityRampRange.x)
		{
			float magnitude = vector.magnitude;
			num = Mathf.InverseLerp(this.mouthProximityRampRange.x, this.mouthProximityRampRange.y, magnitude);
		}
		if (num != this.audioSource.volume)
		{
			this.audioSource.volume = num;
		}
		int num2 = (this.audioSource.timeSamples -= 10);
		if ((float)num2 < 0f)
		{
			num2 = this.audioSource.clip.samples + num2 - 1;
		}
		this.audioSource.clip.SetData(this.zero, num2);
	}

	// Token: 0x06000EBB RID: 3771 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnAudioFilterRead(float[] data, int channels)
	{
	}

	// Token: 0x040011A0 RID: 4512
	[SerializeField]
	private Transform mouthTransform;

	// Token: 0x040011A1 RID: 4513
	[SerializeField]
	private Vector2 mouthProximityRampRange = new Vector2(0.6f, 0.3f);

	// Token: 0x040011A2 RID: 4514
	private AudioSource audioSource;

	// Token: 0x040011A3 RID: 4515
	private float[] zero = new float[1];
}
