using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000216 RID: 534
public class RotationSoundPlayer : MonoBehaviour
{
	// Token: 0x06000E08 RID: 3592 RVA: 0x0004CEAC File Offset: 0x0004B0AC
	private void Awake()
	{
		List<Transform> list = new List<Transform>(this.transforms);
		list.RemoveAll((Transform xform) => xform == null);
		this.transforms = list.ToArray();
		this.initialUpAxis = new Vector3[this.transforms.Length];
		this.lastUpAxis = new Vector3[this.transforms.Length];
		this.lastRotationSpeeds = new float[this.transforms.Length];
		for (int i = 0; i < this.transforms.Length; i++)
		{
			this.initialUpAxis[i] = this.transforms[i].localRotation * Vector3.up;
			this.lastUpAxis[i] = this.initialUpAxis[i];
			this.lastRotationSpeeds[i] = 0f;
		}
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x0004CF8C File Offset: 0x0004B18C
	private void Update()
	{
		this.cooldownTimer -= Time.deltaTime;
		for (int i = 0; i < this.transforms.Length; i++)
		{
			Vector3 vector = this.transforms[i].localRotation * Vector3.up;
			float num = Vector3.Angle(vector, this.initialUpAxis[i]);
			float num2 = Vector3.Angle(vector, this.lastUpAxis[i]);
			float deltaTime = Time.deltaTime;
			float num3 = num2 / deltaTime;
			if (this.cooldownTimer <= 0f && num > this.rotationAmountThreshold && num3 > this.rotationSpeedThreshold && !this.soundBankPlayer.isPlaying)
			{
				this.cooldownTimer = this.cooldown;
				this.soundBankPlayer.Play();
			}
			this.lastUpAxis[i] = vector;
			this.lastRotationSpeeds[i] = num3;
		}
	}

	// Token: 0x040010CC RID: 4300
	[Tooltip("Transforms that will make a noise when they rotate.")]
	[SerializeField]
	private Transform[] transforms;

	// Token: 0x040010CD RID: 4301
	[SerializeField]
	private SoundBankPlayer soundBankPlayer;

	// Token: 0x040010CE RID: 4302
	[Tooltip("How much the transform must rotate from it's initial rotation before a sound is played.")]
	private float rotationAmountThreshold = 30f;

	// Token: 0x040010CF RID: 4303
	[Tooltip("How fast the transform must rotate before a sound is played.")]
	private float rotationSpeedThreshold = 45f;

	// Token: 0x040010D0 RID: 4304
	private float cooldown = 0.6f;

	// Token: 0x040010D1 RID: 4305
	private float cooldownTimer;

	// Token: 0x040010D2 RID: 4306
	private Vector3[] initialUpAxis;

	// Token: 0x040010D3 RID: 4307
	private Vector3[] lastUpAxis;

	// Token: 0x040010D4 RID: 4308
	private float[] lastRotationSpeeds;
}
