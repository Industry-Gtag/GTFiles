using System;
using System.Collections.Generic;
using CjLib;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x02000D64 RID: 3428
public class AutoCatchThrowBall : MonoBehaviour
{
	// Token: 0x060054CE RID: 21710 RVA: 0x001BD291 File Offset: 0x001BB491
	private void Start()
	{
		this.vrRig = base.GetComponent<VRRig>();
	}

	// Token: 0x060054CF RID: 21711 RVA: 0x001BD2A0 File Offset: 0x001BB4A0
	private void Update()
	{
		float time = Time.time;
		Vector3 vector = this.vrRig.transform.position + this.vrRig.transform.forward * 0.5f;
		Quaternion quaternion = this.vrRig.transform.rotation * Quaternion.AngleAxis(-this.throwPitch, Vector3.right);
		Vector3 vector2 = vector - quaternion * Vector3.forward * 0.5f;
		int num = Physics.OverlapBoxNonAlloc(vector2, Vector3.one * 0.5f, this.overlapResults, quaternion);
		DebugUtil.DrawBox(vector2, quaternion, Vector3.one, Color.green, true, DebugUtil.Style.Wireframe);
		for (int i = 0; i < num; i++)
		{
			Collider collider = this.overlapResults[i];
			TransferrableObject componentInParent = collider.gameObject.GetComponentInParent<TransferrableObject>();
			if (componentInParent != null)
			{
				bool flag = false;
				for (int j = 0; j < this.heldBalls.Count; j++)
				{
					if (componentInParent == this.heldBalls[j].transferrable)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Debug.Log(string.Format("Catching {0} in from collider {1} at position {2}", componentInParent.gameObject.name, collider.gameObject.name, componentInParent.transform.position));
					for (int k = 0; k < this.heldBalls.Count; k++)
					{
					}
					this.heldBalls.Add(new AutoCatchThrowBall.HeldBall
					{
						held = true,
						catchTime = time,
						transferrable = componentInParent
					});
					componentInParent.OnGrab(null, null);
					componentInParent.currentState = TransferrableObject.PositionState.InRightHand;
				}
			}
		}
		for (int l = this.heldBalls.Count - 1; l >= 0; l--)
		{
			AutoCatchThrowBall.HeldBall heldBall = this.heldBalls[l];
			if (heldBall.held)
			{
				heldBall.transferrable.transform.position = vector;
				if (time > heldBall.catchTime + this.throwWaitTime)
				{
					this.Throw(heldBall.transferrable, quaternion * Vector3.forward);
					heldBall.held = false;
					heldBall.throwTime = time;
					this.heldBalls[l] = heldBall;
				}
			}
			else if (time > heldBall.throwTime + this.catchWaitTime)
			{
				Debug.Log("Removing " + heldBall.transferrable.gameObject.name);
				this.heldBalls.RemoveAt(l);
				for (int m = 0; m < this.heldBalls.Count; m++)
				{
				}
			}
		}
		if (!TestScript.IsUIOpen && Keyboard.current.tKey.wasPressedThisFrame && this.ballPrefab != null)
		{
			TransferrableObject componentInChildren = Object.Instantiate<GameObject>(this.ballPrefab, vector, Quaternion.identity, null).GetComponentInChildren<TransferrableObject>();
			componentInChildren.OnGrab(null, null);
			componentInChildren.currentState = TransferrableObject.PositionState.InRightHand;
			this.Throw(componentInChildren, quaternion * Vector3.forward);
		}
		DebugUtil.DrawRect(vector, quaternion * Quaternion.AngleAxis(-90f, Vector3.right), Vector2.one, Color.green, true, DebugUtil.Style.Wireframe);
	}

	// Token: 0x060054D0 RID: 21712 RVA: 0x001BD5E0 File Offset: 0x001BB7E0
	private void Throw(TransferrableObject transferrable, Vector3 throwDir)
	{
		Rigidbody componentInChildren = transferrable.GetComponentInChildren<Rigidbody>();
		transferrable.OnRelease(null, null);
		transferrable.currentState = TransferrableObject.PositionState.Dropped;
		componentInChildren.isKinematic = false;
		componentInChildren.linearVelocity = throwDir * this.throwSpeed;
		Debug.Log(string.Format("Throwing {0} in direction {1} at position {2}", transferrable.gameObject.name, throwDir, transferrable.transform.position));
	}

	// Token: 0x04006622 RID: 26146
	public GameObject ballPrefab;

	// Token: 0x04006623 RID: 26147
	public float throwPitch = 20f;

	// Token: 0x04006624 RID: 26148
	public float throwSpeed = 5f;

	// Token: 0x04006625 RID: 26149
	public float throwWaitTime = 1f;

	// Token: 0x04006626 RID: 26150
	public float catchWaitTime = 0.2f;

	// Token: 0x04006627 RID: 26151
	public LayerMask ballLayer;

	// Token: 0x04006628 RID: 26152
	private VRRig vrRig;

	// Token: 0x04006629 RID: 26153
	private Collider[] overlapResults = new Collider[32];

	// Token: 0x0400662A RID: 26154
	private List<AutoCatchThrowBall.HeldBall> heldBalls = new List<AutoCatchThrowBall.HeldBall>();

	// Token: 0x02000D65 RID: 3429
	private struct HeldBall
	{
		// Token: 0x0400662B RID: 26155
		public bool held;

		// Token: 0x0400662C RID: 26156
		public float catchTime;

		// Token: 0x0400662D RID: 26157
		public float throwTime;

		// Token: 0x0400662E RID: 26158
		public TransferrableObject transferrable;
	}
}
