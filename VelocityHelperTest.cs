using System;
using UnityEngine;

// Token: 0x02000B1D RID: 2845
public class VelocityHelperTest : MonoBehaviour
{
	// Token: 0x06004920 RID: 18720 RVA: 0x001873C5 File Offset: 0x001855C5
	private void Setup()
	{
		this.lastPosition = base.transform.position;
		this.lastVelocity = Vector3.zero;
		this.velocity = Vector3.zero;
		this.speed = 0f;
	}

	// Token: 0x06004921 RID: 18721 RVA: 0x001873F9 File Offset: 0x001855F9
	private void Start()
	{
		this.Setup();
	}

	// Token: 0x06004922 RID: 18722 RVA: 0x00187404 File Offset: 0x00185604
	private void FixedUpdate()
	{
		float deltaTime = Time.deltaTime;
		Vector3 position = base.transform.position;
		Vector3 vector = (position - this.lastPosition) / deltaTime;
		this.velocity = Vector3.Lerp(this.lastVelocity, vector, deltaTime);
		this.speed = this.velocity.magnitude;
		this.lastPosition = position;
		this.lastVelocity = vector;
	}

	// Token: 0x06004923 RID: 18723 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Update()
	{
	}

	// Token: 0x04005B3F RID: 23359
	public Vector3 velocity;

	// Token: 0x04005B40 RID: 23360
	public float speed;

	// Token: 0x04005B41 RID: 23361
	[Space]
	public Vector3 lastVelocity;

	// Token: 0x04005B42 RID: 23362
	public Vector3 lastPosition;

	// Token: 0x04005B43 RID: 23363
	[Space]
	[SerializeField]
	private float[] _deltaTimes = new float[5];
}
