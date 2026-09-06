using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020000C8 RID: 200
public class CosmeticCritterShadeFleeing : CosmeticCritter
{
	// Token: 0x060004E0 RID: 1248 RVA: 0x0001B313 File Offset: 0x00019513
	public override void OnSpawn()
	{
		this.spawnFX.Play();
		this.spawnAudioSource.clip = this.spawnAudioClips.GetRandomItem<AudioClip>();
		this.spawnAudioSource.GTPlay();
		this.pullVector = Vector3.zero;
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x0001B34C File Offset: 0x0001954C
	public void SetFleePosition(Vector3 position, Vector3 fleeFrom)
	{
		this.origin = position;
		Vector3 vector = position - fleeFrom;
		this.fleeForward = vector.normalized;
		this.fleeRight = Vector3.Cross(this.fleeForward, Vector3.up);
		this.fleeUp = Vector3.Cross(this.fleeForward, this.fleeRight);
		this.trailingPosition = position + vector.normalized * 3f;
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x0001B3C0 File Offset: 0x000195C0
	public override void SetRandomVariables()
	{
		float num = 0f;
		for (int i = 0; i < this.modelSwaps.Length; i++)
		{
			num += this.modelSwaps[i].relativeProbability;
			this.modelSwaps[i].gameObject.SetActive(false);
		}
		float num2 = Random.value * num;
		for (int j = 0; j < this.modelSwaps.Length; j++)
		{
			if (num2 < this.modelSwaps[j].relativeProbability)
			{
				this.modelSwaps[j].gameObject.SetActive(true);
				break;
			}
			num2 -= this.modelSwaps[j].relativeProbability;
		}
		this.fleeBobFrequencyXY = new Vector2(Random.Range(-1f, 1f) * this.fleeBobFrequencyXYMax.x, Random.Range(-1f, 1f) * this.fleeBobFrequencyXYMax.y);
		this.fleeBobMagnitudeXY = new Vector2(Random.Range(-1f, 1f) * this.fleeBobMagnitudeXYMax.x, Random.Range(-1f, 1f) * this.fleeBobMagnitudeXYMax.y);
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x0001B4E0 File Offset: 0x000196E0
	public override void Tick()
	{
		float num = (float)base.GetAliveTime();
		Vector3 vector = this.origin + num * this.fleeForward + this.pullVector + Mathf.Sin(this.fleeBobFrequencyXY.x * num) * this.fleeBobMagnitudeXY.x * this.fleeRight + Mathf.Sin(this.fleeBobFrequencyXY.y * num) * this.fleeBobMagnitudeXY.y * this.fleeUp;
		Quaternion quaternion = Quaternion.LookRotation((vector - this.trailingPosition).normalized, Vector3.up);
		this.trailingPosition = Vector3.Lerp(this.trailingPosition, vector, 0.05f);
		base.transform.SetPositionAndRotation(vector, quaternion);
		this.animator.SetFloat(this.animatorProperty, Mathf.Sin(num * 3f) * 0.5f + 0.5f);
	}

	// Token: 0x04000563 RID: 1379
	[Tooltip("Randomly selects one of these models when spawned, accounting for relative probabilities. For example, if one model has a probability of 1 and another a probability of 2, the second is twice as likely to be picked (and thus will be picked 67% of the time).")]
	[SerializeField]
	private CosmeticCritterShadeFleeing.ModelSwap[] modelSwaps;

	// Token: 0x04000564 RID: 1380
	[Space]
	[Tooltip("Despawn the Shade after it has fled (fleed?) this many meters.")]
	[SerializeField]
	private float fleeDistanceToDespawn = 10f;

	// Token: 0x04000565 RID: 1381
	[Tooltip("Flee away from the spotter at this many meters per second.")]
	[SerializeField]
	private float fleeSpeed;

	// Token: 0x04000566 RID: 1382
	[Tooltip("The maximum strength the shade can move bob around in the horizontal and vertical axes, with final value chosen randomly.")]
	[SerializeField]
	private Vector2 fleeBobMagnitudeXYMax;

	// Token: 0x04000567 RID: 1383
	[Tooltip("The maximum frequency the shade can move bob around in the horizontal and vertical axes, with final value chosen randomly.")]
	[SerializeField]
	private Vector2 fleeBobFrequencyXYMax;

	// Token: 0x04000568 RID: 1384
	[SerializeField]
	private Animator animator;

	// Token: 0x04000569 RID: 1385
	[SerializeField]
	private ParticleSystem spawnFX;

	// Token: 0x0400056A RID: 1386
	[SerializeField]
	private AudioSource spawnAudioSource;

	// Token: 0x0400056B RID: 1387
	[SerializeField]
	private AudioClip[] spawnAudioClips;

	// Token: 0x0400056C RID: 1388
	[HideInInspector]
	public Vector3 pullVector;

	// Token: 0x0400056D RID: 1389
	private Vector3 origin;

	// Token: 0x0400056E RID: 1390
	private Vector3 fleeForward;

	// Token: 0x0400056F RID: 1391
	private Vector3 fleeRight;

	// Token: 0x04000570 RID: 1392
	private Vector3 fleeUp = Vector3.up;

	// Token: 0x04000571 RID: 1393
	private Vector2 fleeBobFrequencyXY;

	// Token: 0x04000572 RID: 1394
	private Vector2 fleeBobMagnitudeXY;

	// Token: 0x04000573 RID: 1395
	private Vector3 trailingPosition;

	// Token: 0x04000574 RID: 1396
	private float closestCatcherDistance;

	// Token: 0x04000575 RID: 1397
	private int animatorProperty = Animator.StringToHash("Distance");

	// Token: 0x020000C9 RID: 201
	[Serializable]
	private class ModelSwap
	{
		// Token: 0x04000576 RID: 1398
		public float relativeProbability;

		// Token: 0x04000577 RID: 1399
		public GameObject gameObject;
	}
}
