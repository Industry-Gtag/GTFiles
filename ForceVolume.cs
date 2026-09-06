using System;
using GorillaExtensions;
using GorillaLocomotion;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000CA0 RID: 3232
public class ForceVolume : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06004FCB RID: 20427 RVA: 0x001A863E File Offset: 0x001A683E
	private void Awake()
	{
		this.volume = base.GetComponent<Collider>();
		this.audioState = ForceVolume.AudioState.None;
	}

	// Token: 0x06004FCC RID: 20428 RVA: 0x0001212B File Offset: 0x0001032B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06004FCD RID: 20429 RVA: 0x00012134 File Offset: 0x00010334
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06004FCE RID: 20430 RVA: 0x001A8654 File Offset: 0x001A6854
	public void SliceUpdate()
	{
		if (this.audioSource && this.audioSource != null && !this.audioSource.isPlaying && this.audioSource.enabled)
		{
			this.audioSource.enabled = false;
		}
	}

	// Token: 0x06004FCF RID: 20431 RVA: 0x001A86A4 File Offset: 0x001A68A4
	private bool TriggerFilter(Collider other, out Rigidbody rb, out Transform xf)
	{
		rb = null;
		xf = null;
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			rb = GorillaTagger.Instance.GetComponent<Rigidbody>();
			xf = GorillaTagger.Instance.headCollider.GetComponent<Transform>();
		}
		return rb != null && xf != null;
	}

	// Token: 0x06004FD0 RID: 20432 RVA: 0x001A8704 File Offset: 0x001A6904
	public void OnTriggerEnter(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.enterClip == null)
		{
			return;
		}
		if (this.audioSource)
		{
			this.audioSource.enabled = true;
			this.audioSource.GTPlayOneShot(this.enterClip, 1f);
			this.audioState = ForceVolume.AudioState.Enter;
		}
		this.enterPos = transform.position;
	}

	// Token: 0x06004FD1 RID: 20433 RVA: 0x001A8774 File Offset: 0x001A6974
	public void OnTriggerExit(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.audioSource)
		{
			this.audioSource.enabled = true;
			this.audioSource.GTPlayOneShot(this.exitClip, 1f);
			this.audioState = ForceVolume.AudioState.None;
		}
	}

	// Token: 0x06004FD2 RID: 20434 RVA: 0x001A87CC File Offset: 0x001A69CC
	public void OnTriggerStay(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.audioSource && !this.audioSource.isPlaying)
		{
			ForceVolume.AudioState audioState = this.audioState;
			if (audioState != ForceVolume.AudioState.Enter)
			{
				if (audioState == ForceVolume.AudioState.Loop)
				{
					if (this.loopClip != null)
					{
						this.audioSource.enabled = true;
						this.audioSource.GTPlayOneShot(this.loopClip, 1f);
					}
					this.audioState = ForceVolume.AudioState.Loop;
				}
			}
			else
			{
				if (this.loopCresendoClip != null)
				{
					this.audioSource.enabled = true;
					this.audioSource.GTPlayOneShot(this.loopCresendoClip, 1f);
				}
				this.audioState = ForceVolume.AudioState.Crescendo;
			}
		}
		if (this.disableGrip)
		{
			GTPlayer.Instance.SetMaximumSlipThisFrame();
		}
		VRRig.LocalRig.BreakHandLinks();
		SIPlayer localPlayer = SIPlayer.LocalPlayer;
		if (localPlayer != null)
		{
			Vector3 up = base.transform.up;
			localPlayer.PlayerKnockback(up * 0.1f, false, false);
		}
		SizeManager sizeManager = null;
		if (this.scaleWithSize)
		{
			sizeManager = rigidbody.GetComponent<SizeManager>();
		}
		Vector3 vector = rigidbody.linearVelocity;
		if (this.scaleWithSize && sizeManager)
		{
			vector /= sizeManager.currentScale;
		}
		Vector3 vector2 = Vector3.Dot(transform.position - base.transform.position, base.transform.up) * base.transform.up;
		Vector3 vector3 = base.transform.position + vector2 - transform.position;
		float num = vector3.magnitude + 0.0001f;
		Vector3 vector4 = vector3 / num;
		float num2 = Vector3.Dot(vector, vector4);
		float num3 = this.accel;
		if (this.maxDepth > -1f)
		{
			float num4 = Vector3.Dot(transform.position - this.enterPos, vector4);
			float num5 = this.maxDepth - num4;
			float num6 = 0f;
			if (num5 > 0.0001f)
			{
				num6 = num2 * num2 / num5;
			}
			num3 = Mathf.Max(this.accel, num6);
		}
		float deltaTime = Time.deltaTime;
		Vector3 vector5 = base.transform.up * num3 * deltaTime;
		vector += vector5;
		Vector3 vector6 = Mathf.Min(Vector3.Dot(vector, base.transform.up), this.maxSpeed) * base.transform.up;
		Vector3 vector7 = Vector3.Dot(vector, base.transform.right) * base.transform.right;
		Vector3 vector8 = Vector3.Dot(vector, base.transform.forward) * base.transform.forward;
		float num7 = 1f;
		float num8 = 1f;
		if (this.dampenLateralVelocity)
		{
			num7 = 1f - this.dampenXVelPerc * 0.01f * deltaTime;
			num8 = 1f - this.dampenZVelPerc * 0.01f * deltaTime;
		}
		vector = vector6 + num7 * vector7 + num8 * vector8;
		if (this.applyPullToCenterAcceleration && this.pullToCenterAccel > 0f && this.pullToCenterMaxSpeed > 0f)
		{
			vector -= num2 * vector4;
			if (num > this.pullTOCenterMinDistance)
			{
				num2 += this.pullToCenterAccel * deltaTime;
				float num9 = Mathf.Min(this.pullToCenterMaxSpeed, num / deltaTime);
				num2 = Mathf.Min(num2, num9);
			}
			else
			{
				num2 = 0f;
			}
			vector += num2 * vector4;
			if (vector.magnitude > 0.0001f)
			{
				Vector3 vector9 = Vector3.Cross(base.transform.up, vector4);
				float magnitude = vector9.magnitude;
				if (magnitude > 0.0001f)
				{
					vector9 /= magnitude;
					num2 = Vector3.Dot(vector, vector9);
					vector -= num2 * vector9;
					num2 -= this.pullToCenterAccel * deltaTime;
					num2 = Mathf.Max(0f, num2);
					vector += num2 * vector9;
				}
			}
		}
		if (this.scaleWithSize && sizeManager)
		{
			vector *= sizeManager.currentScale;
		}
		rigidbody.linearVelocity = vector;
	}

	// Token: 0x06004FD3 RID: 20435 RVA: 0x001A8C34 File Offset: 0x001A6E34
	public void OnDrawGizmosSelected()
	{
		base.GetComponents<Collider>();
		Gizmos.color = Color.magenta;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.pullTOCenterMinDistance / base.transform.lossyScale.x, 1f, this.pullTOCenterMinDistance / base.transform.lossyScale.z));
	}

	// Token: 0x06004FD4 RID: 20436 RVA: 0x001A8CA4 File Offset: 0x001A6EA4
	public void SetPropertiesFromPlaceholder(ForceVolumeProperties properties, AudioSource volumeAudioSource, Collider colliderVolume)
	{
		this.accel = properties.accel;
		this.maxDepth = properties.maxDepth;
		this.maxSpeed = properties.maxSpeed;
		this.disableGrip = properties.disableGrip;
		this.dampenLateralVelocity = properties.dampenLateralVelocity;
		this.dampenXVelPerc = properties.dampenXVel;
		this.dampenZVelPerc = properties.dampenZVel;
		this.applyPullToCenterAcceleration = properties.applyPullToCenterAcceleration;
		this.pullToCenterAccel = properties.pullToCenterAccel;
		this.pullToCenterMaxSpeed = properties.pullToCenterMaxSpeed;
		this.pullTOCenterMinDistance = properties.pullToCenterMinDistance;
		this.enterClip = properties.enterClip;
		this.exitClip = properties.exitClip;
		this.loopClip = properties.loopClip;
		this.loopCresendoClip = properties.loopCrescendoClip;
		if (volumeAudioSource.IsNotNull())
		{
			this.audioSource = volumeAudioSource;
		}
		if (colliderVolume.IsNotNull())
		{
			this.volume = colliderVolume;
		}
	}

	// Token: 0x040061FF RID: 25087
	[SerializeField]
	public bool scaleWithSize = true;

	// Token: 0x04006200 RID: 25088
	[SerializeField]
	private float accel;

	// Token: 0x04006201 RID: 25089
	[SerializeField]
	private float maxDepth = -1f;

	// Token: 0x04006202 RID: 25090
	[SerializeField]
	private float maxSpeed;

	// Token: 0x04006203 RID: 25091
	[SerializeField]
	private bool disableGrip;

	// Token: 0x04006204 RID: 25092
	[SerializeField]
	private bool dampenLateralVelocity = true;

	// Token: 0x04006205 RID: 25093
	[SerializeField]
	private float dampenXVelPerc;

	// Token: 0x04006206 RID: 25094
	[SerializeField]
	private float dampenZVelPerc;

	// Token: 0x04006207 RID: 25095
	[SerializeField]
	private bool applyPullToCenterAcceleration = true;

	// Token: 0x04006208 RID: 25096
	[SerializeField]
	private float pullToCenterAccel;

	// Token: 0x04006209 RID: 25097
	[SerializeField]
	private float pullToCenterMaxSpeed;

	// Token: 0x0400620A RID: 25098
	[SerializeField]
	private float pullTOCenterMinDistance = 0.1f;

	// Token: 0x0400620B RID: 25099
	private Collider volume;

	// Token: 0x0400620C RID: 25100
	public AudioClip enterClip;

	// Token: 0x0400620D RID: 25101
	public AudioClip exitClip;

	// Token: 0x0400620E RID: 25102
	public AudioClip loopClip;

	// Token: 0x0400620F RID: 25103
	public AudioClip loopCresendoClip;

	// Token: 0x04006210 RID: 25104
	public AudioSource audioSource;

	// Token: 0x04006211 RID: 25105
	private Vector3 enterPos;

	// Token: 0x04006212 RID: 25106
	private ForceVolume.AudioState audioState;

	// Token: 0x02000CA1 RID: 3233
	private enum AudioState
	{
		// Token: 0x04006214 RID: 25108
		None,
		// Token: 0x04006215 RID: 25109
		Enter,
		// Token: 0x04006216 RID: 25110
		Crescendo,
		// Token: 0x04006217 RID: 25111
		Loop,
		// Token: 0x04006218 RID: 25112
		Exit
	}
}
