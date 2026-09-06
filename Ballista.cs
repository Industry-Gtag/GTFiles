using System;
using System.Collections;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001D6 RID: 470
public class Ballista : MonoBehaviourPun
{
	// Token: 0x06000C8A RID: 3210 RVA: 0x00044B12 File Offset: 0x00042D12
	public void TriggerLoad()
	{
		this.animator.SetTrigger(this.loadTriggerHash);
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x00044B25 File Offset: 0x00042D25
	public void TriggerFire()
	{
		this.animator.SetTrigger(this.fireTriggerHash);
	}

	// Token: 0x17000131 RID: 305
	// (get) Token: 0x06000C8C RID: 3212 RVA: 0x00044B38 File Offset: 0x00042D38
	private float LaunchSpeed
	{
		get
		{
			if (!this.useSpeedOptions)
			{
				return this.launchSpeed;
			}
			return this.speedOptions[this.currentSpeedIndex];
		}
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x00044B58 File Offset: 0x00042D58
	private void Awake()
	{
		this.launchDirection = this.launchEnd.position - this.launchStart.position;
		this.launchRampDistance = this.launchDirection.magnitude;
		this.launchDirection /= this.launchRampDistance;
		this.collidingLayer = LayerMask.NameToLayer("Default");
		this.notCollidingLayer = LayerMask.NameToLayer("Prop");
		this.playerPullInRate = Mathf.Exp(this.playerMagnetismStrength);
		this.animator.SetFloat(this.pitchParamHash, this.pitch);
		this.appliedAnimatorPitch = this.pitch;
		this.RefreshButtonColors();
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x00044C08 File Offset: 0x00042E08
	private void Update()
	{
		float deltaTime = Time.deltaTime;
		AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.shortNameHash == this.idleStateHash)
		{
			if (this.prevStateHash == this.fireStateHash)
			{
				this.fireCompleteTime = Time.time;
			}
			if (Time.time - this.fireCompleteTime > this.reloadDelay)
			{
				this.animator.SetTrigger(this.loadTriggerHash);
				this.loadStartTime = Time.time;
			}
		}
		else if (currentAnimatorStateInfo.shortNameHash == this.loadStateHash)
		{
			if (Time.time - this.loadStartTime > this.loadTime)
			{
				if (this.playerInTrigger)
				{
					GTPlayer instance = GTPlayer.Instance;
					Vector3 playerBodyCenterPosition = this.GetPlayerBodyCenterPosition(instance);
					Vector3 vector = Vector3.Dot(playerBodyCenterPosition - this.launchStart.position, this.launchDirection) * this.launchDirection + this.launchStart.position;
					Vector3 vector2 = playerBodyCenterPosition - vector;
					Vector3 vector3 = Vector3.Lerp(Vector3.zero, vector2, Mathf.Exp(-this.playerPullInRate * deltaTime));
					instance.transform.position = instance.transform.position + (vector3 - vector2);
					this.playerReadyToFire = vector3.sqrMagnitude < this.playerReadyToFireDist * this.playerReadyToFireDist;
				}
				else
				{
					this.playerReadyToFire = false;
				}
				if (this.playerReadyToFire)
				{
					if (PhotonNetwork.InRoom)
					{
						base.photonView.RPC("FireBallistaRPC", RpcTarget.Others, Array.Empty<object>());
					}
					this.FireLocal();
				}
			}
		}
		else if (currentAnimatorStateInfo.shortNameHash == this.fireStateHash && !this.playerLaunched && (this.playerReadyToFire || this.playerInTrigger))
		{
			float num = Vector3.Dot(this.launchBone.position - this.launchStart.position, this.launchDirection) / this.launchRampDistance;
			GTPlayer instance2 = GTPlayer.Instance;
			Vector3 playerBodyCenterPosition2 = this.GetPlayerBodyCenterPosition(instance2);
			float num2 = Vector3.Dot(playerBodyCenterPosition2 - this.launchStart.position, this.launchDirection) / this.launchRampDistance;
			float num3 = 0.25f / this.launchRampDistance;
			float num4 = Mathf.Max(num + num3, num2);
			float num5 = num4 * this.launchRampDistance;
			Vector3 vector4 = this.launchDirection * num5 + this.launchStart.position;
			instance2.transform.position + (vector4 - playerBodyCenterPosition2);
			instance2.transform.position = instance2.transform.position + (vector4 - playerBodyCenterPosition2);
			instance2.SetPlayerVelocity(Vector3.zero);
			if (num4 >= 1f)
			{
				this.playerLaunched = true;
				instance2.SetPlayerVelocity(this.LaunchSpeed * this.launchDirection);
				instance2.SetMaximumSlipThisFrame();
			}
		}
		this.prevStateHash = currentAnimatorStateInfo.shortNameHash;
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x00044EFD File Offset: 0x000430FD
	private void FireLocal()
	{
		this.animator.SetTrigger(this.fireTriggerHash);
		this.playerLaunched = false;
		if (this.debugDrawTrajectoryOnLaunch)
		{
			this.DebugDrawTrajectory(8f);
		}
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x00044F2C File Offset: 0x0004312C
	private Vector3 GetPlayerBodyCenterPosition(GTPlayer player)
	{
		return player.headCollider.transform.position + Quaternion.Euler(0f, player.headCollider.transform.rotation.eulerAngles.y, 0f) * new Vector3(0f, 0f, -0.15f) + Vector3.down * 0.4f;
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x00044FA8 File Offset: 0x000431A8
	private void OnTriggerEnter(Collider other)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && instance.bodyCollider == other)
		{
			this.playerInTrigger = true;
		}
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x00044FDC File Offset: 0x000431DC
	private void OnTriggerExit(Collider other)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && instance.bodyCollider == other)
		{
			this.playerInTrigger = false;
		}
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x0004500D File Offset: 0x0004320D
	[PunRPC]
	public void FireBallistaRPC(PhotonMessageInfo info)
	{
		this.FireLocal();
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x00045018 File Offset: 0x00043218
	private void UpdatePredictionLine()
	{
		float num = 0.033333335f;
		Vector3 vector = this.launchEnd.position;
		Vector3 vector2 = (this.launchEnd.position - this.launchStart.position).normalized * this.LaunchSpeed;
		for (int i = 0; i < 240; i++)
		{
			this.predictionLinePoints[i] = vector;
			vector += vector2 * num;
			vector2 += Vector3.down * 9.8f * num;
		}
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x000450B2 File Offset: 0x000432B2
	private IEnumerator DebugDrawTrajectory(float duration)
	{
		this.UpdatePredictionLine();
		float startTime = Time.time;
		while (Time.time < startTime + duration)
		{
			DebugUtil.DrawLine(this.launchStart.position, this.launchEnd.position, Color.yellow, true);
			DebugUtil.DrawLines(this.predictionLinePoints, Color.yellow, true);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x000450C8 File Offset: 0x000432C8
	private void OnDrawGizmosSelected()
	{
		if (this.launchStart != null && this.launchEnd != null)
		{
			this.UpdatePredictionLine();
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(this.launchStart.position, this.launchEnd.position);
			Gizmos.DrawLineList(this.predictionLinePoints);
		}
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x0004512C File Offset: 0x0004332C
	public void RefreshButtonColors()
	{
		this.speedZeroButton.isOn = this.currentSpeedIndex == 0;
		this.speedZeroButton.UpdateColor();
		this.speedOneButton.isOn = this.currentSpeedIndex == 1;
		this.speedOneButton.UpdateColor();
		this.speedTwoButton.isOn = this.currentSpeedIndex == 2;
		this.speedTwoButton.UpdateColor();
		this.speedThreeButton.isOn = this.currentSpeedIndex == 3;
		this.speedThreeButton.UpdateColor();
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x000451B5 File Offset: 0x000433B5
	public void SetSpeedIndex(int index)
	{
		this.currentSpeedIndex = index;
		this.RefreshButtonColors();
	}

	// Token: 0x04000F2D RID: 3885
	public Animator animator;

	// Token: 0x04000F2E RID: 3886
	public Transform launchStart;

	// Token: 0x04000F2F RID: 3887
	public Transform launchEnd;

	// Token: 0x04000F30 RID: 3888
	public Transform launchBone;

	// Token: 0x04000F31 RID: 3889
	public float reloadDelay = 1f;

	// Token: 0x04000F32 RID: 3890
	public float loadTime = 1.933f;

	// Token: 0x04000F33 RID: 3891
	public float playerMagnetismStrength = 3f;

	// Token: 0x04000F34 RID: 3892
	public float launchSpeed = 20f;

	// Token: 0x04000F35 RID: 3893
	[Range(0f, 1f)]
	public float pitch;

	// Token: 0x04000F36 RID: 3894
	private bool useSpeedOptions;

	// Token: 0x04000F37 RID: 3895
	public float[] speedOptions = new float[] { 10f, 15f, 20f, 25f };

	// Token: 0x04000F38 RID: 3896
	public int currentSpeedIndex;

	// Token: 0x04000F39 RID: 3897
	public GorillaPressableButton speedZeroButton;

	// Token: 0x04000F3A RID: 3898
	public GorillaPressableButton speedOneButton;

	// Token: 0x04000F3B RID: 3899
	public GorillaPressableButton speedTwoButton;

	// Token: 0x04000F3C RID: 3900
	public GorillaPressableButton speedThreeButton;

	// Token: 0x04000F3D RID: 3901
	private bool debugDrawTrajectoryOnLaunch;

	// Token: 0x04000F3E RID: 3902
	private int loadTriggerHash = Animator.StringToHash("Load");

	// Token: 0x04000F3F RID: 3903
	private int fireTriggerHash = Animator.StringToHash("Fire");

	// Token: 0x04000F40 RID: 3904
	private int pitchParamHash = Animator.StringToHash("Pitch");

	// Token: 0x04000F41 RID: 3905
	private int idleStateHash = Animator.StringToHash("Idle");

	// Token: 0x04000F42 RID: 3906
	private int loadStateHash = Animator.StringToHash("Load");

	// Token: 0x04000F43 RID: 3907
	private int fireStateHash = Animator.StringToHash("Fire");

	// Token: 0x04000F44 RID: 3908
	private int prevStateHash = Animator.StringToHash("Idle");

	// Token: 0x04000F45 RID: 3909
	private float fireCompleteTime;

	// Token: 0x04000F46 RID: 3910
	private float loadStartTime;

	// Token: 0x04000F47 RID: 3911
	private bool playerInTrigger;

	// Token: 0x04000F48 RID: 3912
	private bool playerReadyToFire;

	// Token: 0x04000F49 RID: 3913
	private bool playerLaunched;

	// Token: 0x04000F4A RID: 3914
	private float playerReadyToFireDist = 0.1f;

	// Token: 0x04000F4B RID: 3915
	private Vector3 playerBodyOffsetFromHead = new Vector3(0f, -0.4f, -0.15f);

	// Token: 0x04000F4C RID: 3916
	private Vector3 launchDirection;

	// Token: 0x04000F4D RID: 3917
	private float launchRampDistance;

	// Token: 0x04000F4E RID: 3918
	private int collidingLayer;

	// Token: 0x04000F4F RID: 3919
	private int notCollidingLayer;

	// Token: 0x04000F50 RID: 3920
	private float playerPullInRate;

	// Token: 0x04000F51 RID: 3921
	private float appliedAnimatorPitch;

	// Token: 0x04000F52 RID: 3922
	private const int predictionLineSamples = 240;

	// Token: 0x04000F53 RID: 3923
	private Vector3[] predictionLinePoints = new Vector3[240];
}
