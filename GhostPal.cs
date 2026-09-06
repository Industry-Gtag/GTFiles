using System;
using System.Collections;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000CF RID: 207
public class GhostPal : MonoBehaviour
{
	// Token: 0x060004FE RID: 1278 RVA: 0x0001BC14 File Offset: 0x00019E14
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.animator = base.GetComponentInChildren<Animator>();
		this.trailingPosition = base.transform.position;
		this.triggerAudioClipIndex = this.triggerAudioClips.GetRandomIndex<AudioClip>();
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x0001BC50 File Offset: 0x00019E50
	private IEnumerator BounceOnTrigger()
	{
		float startTime = Time.time;
		while (Time.time - startTime < this.bounceOnTrigger[this.bounceOnTrigger.length - 1].time)
		{
			this.bounceHeight = this.bounceOnTrigger.Evaluate(Time.time - startTime);
			yield return null;
		}
		this.bounceHeight = 0f;
		yield break;
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x0001BC60 File Offset: 0x00019E60
	private void LateUpdate()
	{
		Vector3 position = this.rig.bodyTransform.position;
		Vector3 vector = base.transform.parent.position - position;
		float num = vector.y * 0.5f + this.orbitHeight;
		vector.y = 0f;
		float num2 = vector.magnitude + this.minDistanceFromPlayer;
		vector = vector.normalized * num2;
		vector.y = num + this.bounceHeight;
		double num3 = (double)this.orbitSpeed * (PhotonNetwork.InRoom ? ((PhotonNetwork.Time - (double)this.rig.OwningNetPlayer.UserId.GetStaticHash()) * (double)((this.rig.OwningNetPlayer.ActorNumber % 2 == 0) ? 1 : (-1))) : Time.timeAsDouble);
		Vector3 vector2 = new Vector3(this.orbitRadius * (float)Math.Cos(num3), 0f, this.orbitRadius * (float)Math.Sin(num3));
		Vector3 vector3 = position + vector + vector2;
		Vector3 vector4 = vector3 - this.rig.head.rigTarget.position;
		if (Vector3.Dot(this.rig.head.rigTarget.forward, vector4.normalized) >= this.lookAtDotProductMin)
		{
			this.lookAtTime = Mathf.Min(this.lookAtTime + Time.deltaTime, Mathf.Max(this.rotateTowardsPlayerFromLookTime[this.rotateTowardsPlayerFromLookTime.length - 1].time, this.minLookTimeToTrigger));
			if (this.lookAtTime >= this.minLookTimeToTrigger && !this.hasTriggered && this.bounceHeight == 0f)
			{
				this.animator.SetTrigger(this.friendlyAnimID);
				this.bounceCoroutine = base.StartCoroutine(this.BounceOnTrigger());
				this.triggerAudioSource.pitch = Random.Range(this.triggerAudioPitchMinMax.x, this.triggerAudioPitchMinMax.y);
				this.triggerAudioSource.clip = this.triggerAudioClips[this.triggerAudioClipIndex];
				this.triggerAudioSource.GTPlay();
				this.triggerAudioClipIndex = (this.triggerAudioClipIndex + Random.Range(0, this.triggerAudioClips.Length - 1)) % this.triggerAudioClips.Length;
				this.hasTriggered = true;
			}
		}
		else
		{
			this.lookAtTime = Mathf.Max(this.lookAtTime - Time.deltaTime, 0f);
			if (this.lookAtTime < this.minLookTimeToTrigger && this.hasTriggered && this.bounceHeight == 0f)
			{
				this.animator.SetTrigger(this.neutralAnimID);
				this.hasTriggered = false;
			}
		}
		if ((vector3 - this.trailingPosition).sqrMagnitude > 0.1f)
		{
			float num4 = 1f - Mathf.Exp(-this.faceMovementDirectionStrength * Time.deltaTime);
			this.trailingPosition = Vector3.Lerp(this.trailingPosition, vector3, num4);
		}
		Quaternion quaternion = Quaternion.Slerp(Quaternion.LookRotation(vector3 - this.trailingPosition, Vector3.up), Quaternion.LookRotation(-vector4, Vector3.up), this.rotateTowardsPlayerFromLookTime.Evaluate(this.lookAtTime));
		base.transform.SetPositionAndRotation(vector3, quaternion);
	}

	// Token: 0x040005AA RID: 1450
	[SerializeField]
	private float minDistanceFromPlayer = 1f;

	// Token: 0x040005AB RID: 1451
	[SerializeField]
	private float orbitRadius = 1f;

	// Token: 0x040005AC RID: 1452
	[SerializeField]
	private float orbitHeight = 1f;

	// Token: 0x040005AD RID: 1453
	[SerializeField]
	private float orbitSpeed = 0.1f;

	// Token: 0x040005AE RID: 1454
	[SerializeField]
	private float faceMovementDirectionStrength = 1f;

	// Token: 0x040005AF RID: 1455
	[Space]
	[SerializeField]
	private float lookAtDotProductMin = 0.95f;

	// Token: 0x040005B0 RID: 1456
	[SerializeField]
	private AnimationCurve rotateTowardsPlayerFromLookTime;

	// Token: 0x040005B1 RID: 1457
	[SerializeField]
	private float minLookTimeToTrigger = 2f;

	// Token: 0x040005B2 RID: 1458
	[SerializeField]
	private AnimationCurve bounceOnTrigger;

	// Token: 0x040005B3 RID: 1459
	[SerializeField]
	private AudioSource triggerAudioSource;

	// Token: 0x040005B4 RID: 1460
	[SerializeField]
	private Vector2 triggerAudioPitchMinMax = new Vector2(0.9f, 1.1f);

	// Token: 0x040005B5 RID: 1461
	[SerializeField]
	private AudioClip[] triggerAudioClips;

	// Token: 0x040005B6 RID: 1462
	private VRRig rig;

	// Token: 0x040005B7 RID: 1463
	private Animator animator;

	// Token: 0x040005B8 RID: 1464
	private float lookAtTime;

	// Token: 0x040005B9 RID: 1465
	private bool hasTriggered;

	// Token: 0x040005BA RID: 1466
	private Coroutine bounceCoroutine;

	// Token: 0x040005BB RID: 1467
	private float bounceHeight;

	// Token: 0x040005BC RID: 1468
	private Vector3 trailingPosition;

	// Token: 0x040005BD RID: 1469
	private int triggerAudioClipIndex;

	// Token: 0x040005BE RID: 1470
	private int neutralAnimID = Animator.StringToHash("Neutral");

	// Token: 0x040005BF RID: 1471
	private int friendlyAnimID = Animator.StringToHash("Friendly");
}
