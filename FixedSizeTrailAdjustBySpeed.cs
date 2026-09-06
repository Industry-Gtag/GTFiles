using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200069B RID: 1691
public class FixedSizeTrailAdjustBySpeed : MonoBehaviour
{
	// Token: 0x06002A2F RID: 10799 RVA: 0x000E3916 File Offset: 0x000E1B16
	private void Start()
	{
		this.Setup();
	}

	// Token: 0x06002A30 RID: 10800 RVA: 0x000E391E File Offset: 0x000E1B1E
	private void OnEnable()
	{
		this.ResetTrailState();
	}

	// Token: 0x06002A31 RID: 10801 RVA: 0x000E391E File Offset: 0x000E1B1E
	private void OnDisable()
	{
		this.ResetTrailState();
	}

	// Token: 0x06002A32 RID: 10802 RVA: 0x000E3928 File Offset: 0x000E1B28
	private void ResetTrailState()
	{
		this._rawVelocity = Vector3.zero;
		this._rawSpeed = 0f;
		this._speed = 0f;
		this._lastSpeed = 0f;
		this._lastPosition = base.transform.position;
		if (!this.trail)
		{
			return;
		}
		this.trail.length = this.minLength;
		this.trail.Setup();
		this.LerpTrailColors(0f);
	}

	// Token: 0x06002A33 RID: 10803 RVA: 0x000E39A8 File Offset: 0x000E1BA8
	private void Setup()
	{
		this._lastPosition = base.transform.position;
		this._rawVelocity = Vector3.zero;
		this._rawSpeed = 0f;
		this._speed = 0f;
		if (this.trail)
		{
			this._initGravity = this.trail.gravity;
			this.trail.applyPhysics = this.adjustPhysics;
		}
		this.LerpTrailColors(0.5f);
	}

	// Token: 0x06002A34 RID: 10804 RVA: 0x000E3A24 File Offset: 0x000E1C24
	private void LerpTrailColors(float t = 0.5f)
	{
		GradientColorKey[] colorKeys = this._mixGradient.colorKeys;
		int num = colorKeys.Length;
		for (int i = 0; i < num; i++)
		{
			float num2 = (float)i / (float)(num - 1);
			Color color = this.minColors.Evaluate(num2);
			Color color2 = this.maxColors.Evaluate(num2);
			Color color3 = Color.Lerp(color, color2, t);
			colorKeys[i].color = color3;
			colorKeys[i].time = num2;
		}
		this._mixGradient.colorKeys = colorKeys;
		if (this.trail)
		{
			this.trail.renderer.colorGradient = this._mixGradient;
		}
	}

	// Token: 0x06002A35 RID: 10805 RVA: 0x000E3AC4 File Offset: 0x000E1CC4
	private void Update()
	{
		float deltaTime = Time.deltaTime;
		Vector3 position = base.transform.position;
		this._rawVelocity = (position - this._lastPosition) / deltaTime;
		this._rawSpeed = this._rawVelocity.magnitude;
		if (this._rawSpeed > this.retractMin)
		{
			this._speed += this.expandSpeed * deltaTime;
		}
		if (this._rawSpeed <= this.retractMin)
		{
			this._speed -= this.retractSpeed * deltaTime;
		}
		if (this._speed > this.maxSpeed)
		{
			this._speed = this.maxSpeed;
		}
		this._speed = Mathf.Lerp(this._lastSpeed, this._speed, 0.5f);
		if (this._speed < 0.01f)
		{
			this._speed = 0f;
		}
		this.AdjustTrail();
		this._lastSpeed = this._speed;
		this._lastPosition = position;
	}

	// Token: 0x06002A36 RID: 10806 RVA: 0x000E3BBC File Offset: 0x000E1DBC
	private void AdjustTrail()
	{
		if (!this.trail)
		{
			return;
		}
		float num = MathUtils.Linear(this._speed, this.minSpeed, this.maxSpeed, 0f, 1f);
		float num2 = MathUtils.Linear(num, 0f, 1f, this.minLength, this.maxLength);
		this.trail.length = num2;
		this.LerpTrailColors(num);
		if (this.adjustPhysics)
		{
			Transform transform = base.transform;
			Vector3 vector = transform.forward * this.gravityOffset.z + transform.right * this.gravityOffset.x + transform.up * this.gravityOffset.y;
			Vector3 vector2 = (this._initGravity + vector) * (1f - num);
			this.trail.gravity = Vector3.Lerp(Vector3.zero, vector2, 0.5f);
		}
	}

	// Token: 0x040036EC RID: 14060
	public FixedSizeTrail trail;

	// Token: 0x040036ED RID: 14061
	public bool adjustPhysics = true;

	// Token: 0x040036EE RID: 14062
	private Vector3 _rawVelocity;

	// Token: 0x040036EF RID: 14063
	private float _rawSpeed;

	// Token: 0x040036F0 RID: 14064
	private float _speed;

	// Token: 0x040036F1 RID: 14065
	private float _lastSpeed;

	// Token: 0x040036F2 RID: 14066
	private Vector3 _lastPosition;

	// Token: 0x040036F3 RID: 14067
	private Vector3 _initGravity;

	// Token: 0x040036F4 RID: 14068
	public Vector3 gravityOffset = Vector3.zero;

	// Token: 0x040036F5 RID: 14069
	[Space]
	public float retractMin = 0.5f;

	// Token: 0x040036F6 RID: 14070
	[Space]
	[FormerlySerializedAs("sizeIncreaseSpeed")]
	public float expandSpeed = 16f;

	// Token: 0x040036F7 RID: 14071
	[FormerlySerializedAs("sizeDecreaseSpeed")]
	public float retractSpeed = 4f;

	// Token: 0x040036F8 RID: 14072
	[Space]
	public float minSpeed;

	// Token: 0x040036F9 RID: 14073
	public float minLength = 1f;

	// Token: 0x040036FA RID: 14074
	public Gradient minColors = GradientHelper.FromColor(new Color(0f, 1f, 1f, 1f));

	// Token: 0x040036FB RID: 14075
	[Space]
	public float maxSpeed = 10f;

	// Token: 0x040036FC RID: 14076
	public float maxLength = 8f;

	// Token: 0x040036FD RID: 14077
	public Gradient maxColors = GradientHelper.FromColor(new Color(1f, 1f, 0f, 1f));

	// Token: 0x040036FE RID: 14078
	[Space]
	[SerializeField]
	private Gradient _mixGradient = new Gradient
	{
		colorKeys = new GradientColorKey[8],
		alphaKeys = Array.Empty<GradientAlphaKey>()
	};

	// Token: 0x0200069C RID: 1692
	[Serializable]
	public struct GradientKey
	{
		// Token: 0x06002A38 RID: 10808 RVA: 0x000E3D91 File Offset: 0x000E1F91
		public GradientKey(Color color, float time)
		{
			this.color = color;
			this.time = time;
		}

		// Token: 0x040036FF RID: 14079
		public Color color;

		// Token: 0x04003700 RID: 14080
		public float time;
	}
}
