using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000A4A RID: 2634
[DisallowMultipleComponent]
public class VerletLine : MonoBehaviour
{
	// Token: 0x0600439C RID: 17308 RVA: 0x00167E40 File Offset: 0x00166040
	private void Awake()
	{
		this._nodes = new VerletLine.LineNode[this.segmentNumber];
		this._positions = new Vector3[this.segmentNumber];
		for (int i = 0; i < this.segmentNumber; i++)
		{
			float num = (float)i / (float)(this.segmentNumber - 1);
			Vector3 vector = Vector3.Lerp(this.lineStart.position, this.lineEnd.position, num);
			this._nodes[i] = new VerletLine.LineNode
			{
				position = vector,
				lastPosition = vector,
				acceleration = this.gravity
			};
		}
		this.line.positionCount = this._nodes.Length;
		this.endRigidbody = this.lineEnd.GetComponent<Rigidbody>();
		if (this.endRigidbody)
		{
			this.endRigidbody.maxLinearVelocity = this.endMaxSpeed;
			this.endRigidbodyParent = this.endRigidbody.transform.parent;
			this.rigidBodyStartingLocalPosition = this.endRigidbody.transform.localPosition;
			this.endRigidbody.transform.parent = null;
			this.endRigidbody.gameObject.SetActive(false);
		}
		this.totalLineLength = this.segmentLength * (float)this.segmentNumber;
	}

	// Token: 0x0600439D RID: 17309 RVA: 0x00167F80 File Offset: 0x00166180
	private void OnEnable()
	{
		if (this.endRigidbody)
		{
			this.endRigidbody.gameObject.SetActive(true);
			this.endRigidbody.transform.localPosition = this.endRigidbodyParent.TransformPoint(this.rigidBodyStartingLocalPosition);
		}
	}

	// Token: 0x0600439E RID: 17310 RVA: 0x00167FCC File Offset: 0x001661CC
	private void OnDisable()
	{
		if (this.endRigidbody)
		{
			this.endRigidbody.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600439F RID: 17311 RVA: 0x00167FEC File Offset: 0x001661EC
	public void SetLength(float total, float delay = 0f)
	{
		this.segmentTargetLength = total / (float)this.segmentNumber;
		if (this.segmentTargetLength < this.segmentMinLength)
		{
			this.segmentTargetLength = this.segmentMinLength;
		}
		if (this.segmentTargetLength > this.segmentMaxLength)
		{
			this.segmentTargetLength = this.segmentMaxLength;
		}
		if (delay >= 0.01f)
		{
			base.StartCoroutine(this.ResizeAfterDelay(delay));
		}
	}

	// Token: 0x060043A0 RID: 17312 RVA: 0x00168054 File Offset: 0x00166254
	public void AddSegmentLength(float amount, float delay = 0f)
	{
		this.segmentTargetLength = this.segmentLength + amount;
		if (this.segmentTargetLength <= 0f)
		{
			return;
		}
		if (this.segmentTargetLength > this.segmentMaxLength)
		{
			this.segmentTargetLength = this.segmentMaxLength;
		}
		if (delay >= 0.01f)
		{
			base.StartCoroutine(this.ResizeAfterDelay(delay));
		}
	}

	// Token: 0x060043A1 RID: 17313 RVA: 0x001680B0 File Offset: 0x001662B0
	public void RemoveSegmentLength(float amount, float delay = 0f)
	{
		this.segmentTargetLength = this.segmentLength - amount;
		if (this.segmentTargetLength <= this.segmentMinLength)
		{
			this.segmentTargetLength = (this.segmentLength = this.segmentMinLength);
			return;
		}
		if (delay >= 0.01f)
		{
			base.StartCoroutine(this.ResizeAfterDelay(delay));
		}
	}

	// Token: 0x060043A2 RID: 17314 RVA: 0x00168105 File Offset: 0x00166305
	private IEnumerator ResizeAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		yield break;
	}

	// Token: 0x060043A3 RID: 17315 RVA: 0x00168114 File Offset: 0x00166314
	private void Update()
	{
		if (this.segmentLength.Approx(this.segmentTargetLength, 0.1f))
		{
			this.segmentLength = this.segmentTargetLength;
			return;
		}
		this.segmentLength = Mathf.Lerp(this.segmentLength, this.segmentTargetLength, this.resizeSpeed * this.resizeScale * Time.deltaTime);
		if (this.scaleLineWidth)
		{
			this.line.widthMultiplier = base.transform.lossyScale.x;
		}
	}

	// Token: 0x060043A4 RID: 17316 RVA: 0x00168194 File Offset: 0x00166394
	public void ForceTotalLength(float totalLength)
	{
		float num = totalLength / (float)((this.segmentNumber < 1) ? 1 : this.segmentNumber);
		this.segmentLength = (this.segmentTargetLength = num);
		this.totalLineLength = this.segmentLength * (float)this.segmentNumber;
	}

	// Token: 0x060043A5 RID: 17317 RVA: 0x001681DC File Offset: 0x001663DC
	private void FixedUpdate()
	{
		for (int i = 0; i < this._nodes.Length; i++)
		{
			VerletLine.Simulate(ref this._nodes[i], Time.fixedDeltaTime);
		}
		for (int j = 0; j < this.simIterations; j++)
		{
			for (int k = 0; k < this._nodes.Length - 1; k++)
			{
				VerletLine.LimitDistance(ref this._nodes[k], ref this._nodes[k + 1], this.segmentLength);
			}
		}
		this._nodes[0].position = this.lineStart.position;
		if (this.endRigidbody)
		{
			if (this.onlyPullAtEdges)
			{
				if ((this.endRigidbody.transform.position - this.lineStart.position).IsLongerThan(this.totalLineLength))
				{
					Vector3 vector = this.lineStart.position + (this.endRigidbody.transform.position - this.lineStart.position).normalized * this.totalLineLength;
					this.endRigidbody.linearVelocity += (vector - this.endRigidbody.transform.position) / Time.fixedDeltaTime;
					if (this.endRigidbody.linearVelocity.IsLongerThan(this.endMaxSpeed))
					{
						this.endRigidbody.linearVelocity = this.endRigidbody.linearVelocity.normalized * this.endMaxSpeed;
					}
				}
			}
			else
			{
				VerletLine.LineNode[] nodes = this._nodes;
				Vector3 vector2 = (nodes[nodes.Length - 1].position - this.lineEnd.position) * (this.tension * this.tensionScale);
				Quaternion rotation = this.endRigidbody.rotation;
				VerletLine.LineNode[] nodes2 = this._nodes;
				Vector3 position = nodes2[nodes2.Length - 1].position;
				VerletLine.LineNode[] nodes3 = this._nodes;
				Quaternion.LookRotation(position - nodes3[nodes3.Length - 2].position);
				if (!this.endRigidbody.isKinematic)
				{
					this.endRigidbody.AddForceAtPosition(vector2, this.endRigidbody.transform.TransformPoint(this.endLineAnchorLocalPosition));
				}
			}
		}
		VerletLine.LineNode[] nodes4 = this._nodes;
		nodes4[nodes4.Length - 1].position = this.lineEnd.position;
		for (int l = 0; l < this._nodes.Length; l++)
		{
			this._positions[l] = this._nodes[l].position;
		}
		this.line.SetPositions(this._positions);
	}

	// Token: 0x060043A6 RID: 17318 RVA: 0x0016849C File Offset: 0x0016669C
	private static void Simulate(ref VerletLine.LineNode p, float dt)
	{
		Vector3 position = p.position;
		p.position += p.position - p.lastPosition + p.acceleration * (dt * dt);
		p.lastPosition = position;
	}

	// Token: 0x060043A7 RID: 17319 RVA: 0x001684F4 File Offset: 0x001666F4
	private static void LimitDistance(ref VerletLine.LineNode p1, ref VerletLine.LineNode p2, float restLength)
	{
		Vector3 vector = p2.position - p1.position;
		float num = vector.magnitude + 1E-05f;
		float num2 = (num - restLength) / num;
		p1.position += vector * (num2 * 0.5f);
		p2.position -= vector * (num2 * 0.5f);
	}

	// Token: 0x04005596 RID: 21910
	public Transform lineStart;

	// Token: 0x04005597 RID: 21911
	public Transform lineEnd;

	// Token: 0x04005598 RID: 21912
	[Space]
	public LineRenderer line;

	// Token: 0x04005599 RID: 21913
	public Rigidbody endRigidbody;

	// Token: 0x0400559A RID: 21914
	public Transform endRigidbodyParent;

	// Token: 0x0400559B RID: 21915
	public Vector3 endLineAnchorLocalPosition;

	// Token: 0x0400559C RID: 21916
	private Vector3 rigidBodyStartingLocalPosition;

	// Token: 0x0400559D RID: 21917
	[Space]
	public int segmentNumber = 10;

	// Token: 0x0400559E RID: 21918
	public float segmentLength = 0.03f;

	// Token: 0x0400559F RID: 21919
	public float segmentTargetLength = 0.03f;

	// Token: 0x040055A0 RID: 21920
	public float segmentMaxLength = 0.03f;

	// Token: 0x040055A1 RID: 21921
	public float segmentMinLength = 0.03f;

	// Token: 0x040055A2 RID: 21922
	[Space]
	public Vector3 gravity = new Vector3(0f, -9.81f, 0f);

	// Token: 0x040055A3 RID: 21923
	public int simIterations = 6;

	// Token: 0x040055A4 RID: 21924
	public float tension = 10f;

	// Token: 0x040055A5 RID: 21925
	public float tensionScale = 1f;

	// Token: 0x040055A6 RID: 21926
	public float endMaxSpeed = 48f;

	// Token: 0x040055A7 RID: 21927
	[FormerlySerializedAs("lerpSpeed")]
	[Space]
	public float resizeSpeed = 1f;

	// Token: 0x040055A8 RID: 21928
	public float resizeScale = 1f;

	// Token: 0x040055A9 RID: 21929
	[NonSerialized]
	private VerletLine.LineNode[] _nodes = new VerletLine.LineNode[0];

	// Token: 0x040055AA RID: 21930
	[NonSerialized]
	private Vector3[] _positions = new Vector3[0];

	// Token: 0x040055AB RID: 21931
	private float totalLineLength;

	// Token: 0x040055AC RID: 21932
	[SerializeField]
	private bool onlyPullAtEdges;

	// Token: 0x040055AD RID: 21933
	[SerializeField]
	private bool scaleLineWidth = true;

	// Token: 0x02000A4B RID: 2635
	[Serializable]
	public struct LineNode
	{
		// Token: 0x040055AE RID: 21934
		public Vector3 position;

		// Token: 0x040055AF RID: 21935
		public Vector3 lastPosition;

		// Token: 0x040055B0 RID: 21936
		public Vector3 acceleration;
	}
}
