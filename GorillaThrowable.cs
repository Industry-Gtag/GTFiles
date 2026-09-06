using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000A46 RID: 2630
public class GorillaThrowable : MonoBehaviourPun, IPunObservable, IPhotonViewCallback
{
	// Token: 0x06004389 RID: 17289 RVA: 0x0016742C File Offset: 0x0016562C
	public virtual void Start()
	{
		this.offset = Vector3.zero;
		this.headsetTransform = GTPlayer.Instance.headCollider.transform;
		this.velocityHistory = new Vector3[this.trackingHistorySize];
		this.positionHistory = new Vector3[this.trackingHistorySize];
		this.headsetPositionHistory = new Vector3[this.trackingHistorySize];
		this.rotationHistory = new Vector3[this.trackingHistorySize];
		this.rotationalVelocityHistory = new Vector3[this.trackingHistorySize];
		for (int i = 0; i < this.trackingHistorySize; i++)
		{
			this.velocityHistory[i] = Vector3.zero;
			this.positionHistory[i] = base.transform.position - this.headsetTransform.position;
			this.headsetPositionHistory[i] = this.headsetTransform.position;
			this.rotationHistory[i] = base.transform.eulerAngles;
			this.rotationalVelocityHistory[i] = Vector3.zero;
		}
		this.currentIndex = 0;
		this.rigidbody = base.GetComponentInChildren<Rigidbody>();
	}

	// Token: 0x0600438A RID: 17290 RVA: 0x0016754C File Offset: 0x0016574C
	public virtual void LateUpdate()
	{
		if (this.isHeld && base.photonView.IsMine)
		{
			base.transform.rotation = this.transformToFollow.rotation * this.offsetRotation;
			if (!this.initialLerp && (base.transform.position - this.transformToFollow.position).magnitude > this.lerpDistanceLimit)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, this.transformToFollow.position + this.transformToFollow.rotation * this.offset, this.pickupLerp);
			}
			else
			{
				this.initialLerp = true;
				base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
			}
		}
		if (!base.photonView.IsMine)
		{
			this.rigidbody.isKinematic = true;
			base.transform.position = Vector3.Lerp(base.transform.position, this.targetPosition, this.lerpValue);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.targetRotation, this.lerpValue);
		}
		this.StoreHistories();
	}

	// Token: 0x0600438B RID: 17291 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void IsHandPushing(XRNode node)
	{
	}

	// Token: 0x0600438C RID: 17292 RVA: 0x001676B8 File Offset: 0x001658B8
	private void StoreHistories()
	{
		this.previousPosition = this.positionHistory[this.currentIndex];
		this.previousRotation = this.rotationHistory[this.currentIndex];
		this.previousHeadsetPosition = this.headsetPositionHistory[this.currentIndex];
		this.currentIndex = (this.currentIndex + 1) % this.trackingHistorySize;
		this.currentVelocity = (base.transform.position - this.headsetTransform.position - this.previousPosition) / Time.deltaTime;
		this.currentHeadsetVelocity = (this.headsetTransform.position - this.previousHeadsetPosition) / Time.deltaTime;
		this.currentRotationalVelocity = (base.transform.eulerAngles - this.previousRotation) / Time.deltaTime;
		this.denormalizedVelocityAverage = Vector3.zero;
		this.denormalizedRotationalVelocityAverage = Vector3.zero;
		this.loopIndex = 0;
		while (this.loopIndex < this.trackingHistorySize)
		{
			this.denormalizedVelocityAverage += this.velocityHistory[this.loopIndex];
			this.denormalizedRotationalVelocityAverage += this.rotationalVelocityHistory[this.loopIndex];
			this.loopIndex++;
		}
		this.denormalizedVelocityAverage /= (float)this.trackingHistorySize;
		this.denormalizedRotationalVelocityAverage /= (float)this.trackingHistorySize;
		this.velocityHistory[this.currentIndex] = this.currentVelocity;
		this.positionHistory[this.currentIndex] = base.transform.position - this.headsetTransform.position;
		this.headsetPositionHistory[this.currentIndex] = this.headsetTransform.position;
		this.rotationHistory[this.currentIndex] = base.transform.eulerAngles;
		this.rotationalVelocityHistory[this.currentIndex] = this.currentRotationalVelocity;
	}

	// Token: 0x0600438D RID: 17293 RVA: 0x001678E4 File Offset: 0x00165AE4
	public virtual void Grabbed(Transform grabTransform)
	{
		this.grabbingTransform = grabTransform;
		this.isHeld = true;
		this.transformToFollow = this.grabbingTransform;
		this.offsetRotation = base.transform.rotation * Quaternion.Inverse(this.transformToFollow.rotation);
		this.initialLerp = false;
		this.rigidbody.isKinematic = true;
		this.rigidbody.useGravity = false;
		base.photonView.RequestOwnership();
	}

	// Token: 0x0600438E RID: 17294 RVA: 0x0016795C File Offset: 0x00165B5C
	public virtual void ThrowThisThingo()
	{
		this.transformToFollow = null;
		this.isHeld = false;
		this.synchThrow = true;
		this.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		this.rigidbody.isKinematic = false;
		this.rigidbody.useGravity = true;
		if (this.isLinear || this.denormalizedVelocityAverage.magnitude < this.linearMax)
		{
			if (this.denormalizedVelocityAverage.magnitude * this.throwMultiplier < this.throwMagnitudeLimit)
			{
				this.rigidbody.linearVelocity = this.denormalizedVelocityAverage * this.throwMultiplier + this.currentHeadsetVelocity;
			}
			else
			{
				this.rigidbody.linearVelocity = this.denormalizedVelocityAverage.normalized * this.throwMagnitudeLimit + this.currentHeadsetVelocity;
			}
		}
		else
		{
			this.rigidbody.linearVelocity = this.denormalizedVelocityAverage.normalized * Mathf.Max(Mathf.Min(Mathf.Pow(this.throwMultiplier * this.denormalizedVelocityAverage.magnitude / this.linearMax, this.exponThrowMultMax), 0.1f) * this.denormalizedHeadsetVelocityAverage.magnitude, this.throwMagnitudeLimit) + this.currentHeadsetVelocity;
		}
		this.rigidbody.angularVelocity = this.denormalizedRotationalVelocityAverage * 3.1415927f / 180f;
		this.rigidbody.MovePosition(this.rigidbody.transform.position + this.rigidbody.linearVelocity * Time.deltaTime);
	}

	// Token: 0x0600438F RID: 17295 RVA: 0x00167AF8 File Offset: 0x00165CF8
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
			stream.SendNext(this.rigidbody.linearVelocity);
			return;
		}
		Vector3 vector = (Vector3)stream.ReceiveNext();
		(ref this.targetPosition).SetValueSafe(in vector);
		Quaternion quaternion = (Quaternion)stream.ReceiveNext();
		(ref this.targetRotation).SetValueSafe(in quaternion);
		Vector3 linearVelocity = this.rigidbody.linearVelocity;
		vector = (Vector3)stream.ReceiveNext();
		(ref linearVelocity).SetValueSafe(in vector);
		this.rigidbody.linearVelocity = linearVelocity;
	}

	// Token: 0x06004390 RID: 17296 RVA: 0x00167BB0 File Offset: 0x00165DB0
	public virtual void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.GetComponent<GorillaSurfaceOverride>() != null)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				base.photonView.RPC("PlaySurfaceHit", RpcTarget.Others, new object[]
				{
					this.bounceAudioClip,
					this.InterpolateVolume()
				});
			}
			this.PlaySurfaceHit(collision.collider.GetComponent<GorillaSurfaceOverride>().overrideIndex, this.InterpolateVolume());
		}
	}

	// Token: 0x06004391 RID: 17297 RVA: 0x00167C2C File Offset: 0x00165E2C
	public void PlaySurfaceHit(int soundIndex, float tapVolume)
	{
		if (soundIndex > -1 && soundIndex < GTPlayer.Instance.materialData.Count)
		{
			this.audioSource.volume = tapVolume;
			this.audioSource.clip = (GTPlayer.Instance.materialData[soundIndex].overrideAudio ? GTPlayer.Instance.materialData[soundIndex].audio : GTPlayer.Instance.materialData[0].audio);
			this.audioSource.GTPlayOneShot(this.audioSource.clip, 1f);
		}
	}

	// Token: 0x06004392 RID: 17298 RVA: 0x00167CC8 File Offset: 0x00165EC8
	public float InterpolateVolume()
	{
		return (Mathf.Clamp(this.rigidbody.linearVelocity.magnitude, this.minVelocity, this.maxVelocity) - this.minVelocity) / (this.maxVelocity - this.minVelocity) * (this.maxVolume - this.minVolume) + this.minVolume;
	}

	// Token: 0x04005561 RID: 21857
	public int trackingHistorySize;

	// Token: 0x04005562 RID: 21858
	public float throwMultiplier;

	// Token: 0x04005563 RID: 21859
	public float throwMagnitudeLimit;

	// Token: 0x04005564 RID: 21860
	private Vector3[] velocityHistory;

	// Token: 0x04005565 RID: 21861
	private Vector3[] headsetVelocityHistory;

	// Token: 0x04005566 RID: 21862
	private Vector3[] positionHistory;

	// Token: 0x04005567 RID: 21863
	private Vector3[] headsetPositionHistory;

	// Token: 0x04005568 RID: 21864
	private Vector3[] rotationHistory;

	// Token: 0x04005569 RID: 21865
	private Vector3[] rotationalVelocityHistory;

	// Token: 0x0400556A RID: 21866
	private Vector3 previousPosition;

	// Token: 0x0400556B RID: 21867
	private Vector3 previousRotation;

	// Token: 0x0400556C RID: 21868
	private Vector3 previousHeadsetPosition;

	// Token: 0x0400556D RID: 21869
	private int currentIndex;

	// Token: 0x0400556E RID: 21870
	private Vector3 currentVelocity;

	// Token: 0x0400556F RID: 21871
	private Vector3 currentHeadsetVelocity;

	// Token: 0x04005570 RID: 21872
	private Vector3 currentRotationalVelocity;

	// Token: 0x04005571 RID: 21873
	public Vector3 denormalizedVelocityAverage;

	// Token: 0x04005572 RID: 21874
	private Vector3 denormalizedHeadsetVelocityAverage;

	// Token: 0x04005573 RID: 21875
	private Vector3 denormalizedRotationalVelocityAverage;

	// Token: 0x04005574 RID: 21876
	private Transform headsetTransform;

	// Token: 0x04005575 RID: 21877
	private Vector3 targetPosition;

	// Token: 0x04005576 RID: 21878
	private Quaternion targetRotation;

	// Token: 0x04005577 RID: 21879
	public bool initialLerp;

	// Token: 0x04005578 RID: 21880
	public float lerpValue = 0.4f;

	// Token: 0x04005579 RID: 21881
	public float lerpDistanceLimit = 0.01f;

	// Token: 0x0400557A RID: 21882
	public bool isHeld;

	// Token: 0x0400557B RID: 21883
	public Rigidbody rigidbody;

	// Token: 0x0400557C RID: 21884
	private int loopIndex;

	// Token: 0x0400557D RID: 21885
	private Transform transformToFollow;

	// Token: 0x0400557E RID: 21886
	private Vector3 offset;

	// Token: 0x0400557F RID: 21887
	private Quaternion offsetRotation;

	// Token: 0x04005580 RID: 21888
	public AudioSource audioSource;

	// Token: 0x04005581 RID: 21889
	public int timeLastReceived;

	// Token: 0x04005582 RID: 21890
	public bool synchThrow;

	// Token: 0x04005583 RID: 21891
	public float tempFloat;

	// Token: 0x04005584 RID: 21892
	public Transform grabbingTransform;

	// Token: 0x04005585 RID: 21893
	public float pickupLerp;

	// Token: 0x04005586 RID: 21894
	public float minVelocity;

	// Token: 0x04005587 RID: 21895
	public float maxVelocity;

	// Token: 0x04005588 RID: 21896
	public float minVolume;

	// Token: 0x04005589 RID: 21897
	public float maxVolume;

	// Token: 0x0400558A RID: 21898
	public bool isLinear;

	// Token: 0x0400558B RID: 21899
	public float linearMax;

	// Token: 0x0400558C RID: 21900
	public float exponThrowMultMax;

	// Token: 0x0400558D RID: 21901
	public int bounceAudioClip;
}
