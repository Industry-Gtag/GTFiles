using System;
using GorillaTag.Reactions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000599 RID: 1433
public class PaperPlaneProjectile : MonoBehaviour
{
	// Token: 0x1400004F RID: 79
	// (add) Token: 0x06002448 RID: 9288 RVA: 0x000C2CFC File Offset: 0x000C0EFC
	// (remove) Token: 0x06002449 RID: 9289 RVA: 0x000C2D34 File Offset: 0x000C0F34
	public event PaperPlaneProjectile.PaperPlaneHit OnHit;

	// Token: 0x170003D2 RID: 978
	// (get) Token: 0x0600244A RID: 9290 RVA: 0x000C2D69 File Offset: 0x000C0F69
	public new Transform transform
	{
		get
		{
			return this._tCached;
		}
	}

	// Token: 0x170003D3 RID: 979
	// (get) Token: 0x0600244B RID: 9291 RVA: 0x000C2D71 File Offset: 0x000C0F71
	public VRRig MyRig
	{
		get
		{
			return this.myRig;
		}
	}

	// Token: 0x0600244C RID: 9292 RVA: 0x000C2D79 File Offset: 0x000C0F79
	private void Awake()
	{
		this._tCached = base.transform;
		this.spawnWorldEffects = base.GetComponent<SpawnWorldEffects>();
	}

	// Token: 0x0600244D RID: 9293 RVA: 0x000C2D93 File Offset: 0x000C0F93
	private void Start()
	{
		this.ResetProjectile();
	}

	// Token: 0x0600244E RID: 9294 RVA: 0x000C2D9B File Offset: 0x000C0F9B
	public void ResetProjectile()
	{
		this._timeElapsed = 0f;
		this.flyingObject.SetActive(true);
		this.crashingObject.SetActive(false);
	}

	// Token: 0x0600244F RID: 9295 RVA: 0x000C2DC0 File Offset: 0x000C0FC0
	internal void SetTransferrableState(TransferrableObject.SyncOptions syncType, int state)
	{
		if (!this.useTransferrableObjectState)
		{
			return;
		}
		if (syncType != TransferrableObject.SyncOptions.Bool)
		{
			if (syncType != TransferrableObject.SyncOptions.Int)
			{
				return;
			}
			UnityEvent<int> onItemStateIntChanged = this.OnItemStateIntChanged;
			if (onItemStateIntChanged == null)
			{
				return;
			}
			onItemStateIntChanged.Invoke(state);
			return;
		}
		else
		{
			bool flag = (state & 1) != 0;
			bool flag2 = (state & 2) != 0;
			bool flag3 = (state & 4) != 0;
			bool flag4 = (state & 8) != 0;
			if (flag)
			{
				UnityEvent onItemStateBoolATrue = this.OnItemStateBoolATrue;
				if (onItemStateBoolATrue != null)
				{
					onItemStateBoolATrue.Invoke();
				}
			}
			else
			{
				UnityEvent onItemStateBoolAFalse = this.OnItemStateBoolAFalse;
				if (onItemStateBoolAFalse != null)
				{
					onItemStateBoolAFalse.Invoke();
				}
			}
			if (flag2)
			{
				UnityEvent onItemStateBoolBTrue = this.OnItemStateBoolBTrue;
				if (onItemStateBoolBTrue != null)
				{
					onItemStateBoolBTrue.Invoke();
				}
			}
			else
			{
				UnityEvent onItemStateBoolBFalse = this.OnItemStateBoolBFalse;
				if (onItemStateBoolBFalse != null)
				{
					onItemStateBoolBFalse.Invoke();
				}
			}
			if (flag3)
			{
				UnityEvent onItemStateBoolCTrue = this.OnItemStateBoolCTrue;
				if (onItemStateBoolCTrue != null)
				{
					onItemStateBoolCTrue.Invoke();
				}
			}
			else
			{
				UnityEvent onItemStateBoolCFalse = this.OnItemStateBoolCFalse;
				if (onItemStateBoolCFalse != null)
				{
					onItemStateBoolCFalse.Invoke();
				}
			}
			if (flag4)
			{
				UnityEvent onItemStateBoolDTrue = this.OnItemStateBoolDTrue;
				if (onItemStateBoolDTrue == null)
				{
					return;
				}
				onItemStateBoolDTrue.Invoke();
				return;
			}
			else
			{
				UnityEvent onItemStateBoolDFalse = this.OnItemStateBoolDFalse;
				if (onItemStateBoolDFalse == null)
				{
					return;
				}
				onItemStateBoolDFalse.Invoke();
				return;
			}
		}
	}

	// Token: 0x06002450 RID: 9296 RVA: 0x000C2EA8 File Offset: 0x000C10A8
	public void Launch(Vector3 startPos, Quaternion startRot, Vector3 vel)
	{
		base.gameObject.SetActive(true);
		this.ResetProjectile();
		this.transform.position = startPos;
		if (this.enableRotation)
		{
			this.transform.rotation = startRot;
		}
		else
		{
			this.transform.LookAt(this.transform.position + vel.normalized);
		}
		this._direction = vel.normalized;
		this._speed = Mathf.Clamp(this.speedCurve.Evaluate(vel.magnitude), this.minSpeed, this.maxSpeed);
		this._stopped = false;
		this.scaleFactor = 0.7f * (this.transform.lossyScale.x - 1f + 1.4285715f);
	}

	// Token: 0x06002451 RID: 9297 RVA: 0x000C2F70 File Offset: 0x000C1170
	private void Update()
	{
		if (this._stopped)
		{
			if (!this.crashingObject.gameObject.activeSelf)
			{
				if (ObjectPools.instance)
				{
					ObjectPools.instance.Destroy(base.gameObject);
					return;
				}
				base.gameObject.SetActive(false);
			}
			return;
		}
		this._timeElapsed += Time.deltaTime;
		this.nextPos = this.transform.position + this._direction * this._speed * Time.deltaTime * this.scaleFactor;
		if (this._timeElapsed < this.maxFlightTime && (this._timeElapsed < this.minFlightTime || Physics.RaycastNonAlloc(this.transform.position, this.nextPos - this.transform.position, this.results, Vector3.Distance(this.transform.position, this.nextPos), this.layerMask.value) == 0))
		{
			this.transform.position = this.nextPos;
			this.transform.Rotate(Mathf.Sin(this._timeElapsed) * 10f * Time.deltaTime, 0f, 0f);
			return;
		}
		if (this._timeElapsed < this.maxFlightTime)
		{
			SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
			if (this.results[0].collider.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
			{
				slingshotProjectileHitNotifier.InvokeHit(this, this.results[0].collider);
			}
			if (this.spawnWorldEffects != null)
			{
				this.spawnWorldEffects.RequestSpawn(this.nextPos);
			}
		}
		this._stopped = true;
		this._timeElapsed = 0f;
		PaperPlaneProjectile.PaperPlaneHit onHit = this.OnHit;
		if (onHit != null)
		{
			onHit(this.nextPos);
		}
		this.OnHit = null;
		this.flyingObject.SetActive(false);
		this.crashingObject.SetActive(true);
	}

	// Token: 0x06002452 RID: 9298 RVA: 0x000C3162 File Offset: 0x000C1362
	internal void SetVRRig(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06002453 RID: 9299 RVA: 0x000C316B File Offset: 0x000C136B
	private void OnDisable()
	{
		if (this.useTransferrableObjectState)
		{
			UnityEvent onResetProjectileState = this.OnResetProjectileState;
			if (onResetProjectileState == null)
			{
				return;
			}
			onResetProjectileState.Invoke();
		}
	}

	// Token: 0x04002F8F RID: 12175
	private const float speedScaleRatio = 0.7f;

	// Token: 0x04002F91 RID: 12177
	[Space]
	[NonSerialized]
	private float _timeElapsed;

	// Token: 0x04002F92 RID: 12178
	[NonSerialized]
	private float _speed;

	// Token: 0x04002F93 RID: 12179
	[NonSerialized]
	private Vector3 _direction;

	// Token: 0x04002F94 RID: 12180
	[NonSerialized]
	private bool _stopped;

	// Token: 0x04002F95 RID: 12181
	private Transform _tCached;

	// Token: 0x04002F96 RID: 12182
	private SpawnWorldEffects spawnWorldEffects;

	// Token: 0x04002F97 RID: 12183
	private Vector3 nextPos;

	// Token: 0x04002F98 RID: 12184
	private RaycastHit[] results = new RaycastHit[1];

	// Token: 0x04002F99 RID: 12185
	[Tooltip("Maximum lifetime in seconds for the projectile")]
	[SerializeField]
	private float maxFlightTime = 7.5f;

	// Token: 0x04002F9A RID: 12186
	[Tooltip("Collisions are ignored for minFlightTime seconds after launch")]
	[SerializeField]
	private float minFlightTime = 0.5f;

	// Token: 0x04002F9B RID: 12187
	[Tooltip("Hand speed to projectile launch Speed")]
	[SerializeField]
	private AnimationCurve speedCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f),
		new Keyframe(6.324555f, 20f, 6.324555f, 6.324555f)
	});

	// Token: 0x04002F9C RID: 12188
	[Tooltip("maximum speed of launched projectile (clamped after applying speed curve)")]
	[SerializeField]
	private float maxSpeed = 10f;

	// Token: 0x04002F9D RID: 12189
	[Tooltip("minimum speed of launched projectile (clamped after applying speed curve)")]
	[SerializeField]
	private float minSpeed = 1f;

	// Token: 0x04002F9E RID: 12190
	[SerializeField]
	private bool enableRotation;

	// Token: 0x04002F9F RID: 12191
	[Tooltip("Objects enabled when launched and disabled on Hit")]
	[SerializeField]
	private GameObject flyingObject;

	// Token: 0x04002FA0 RID: 12192
	[Tooltip("Objects disabled when launched and enabled on Hit")]
	[SerializeField]
	private GameObject crashingObject;

	// Token: 0x04002FA1 RID: 12193
	[Tooltip("Layers the projectile collides with")]
	[SerializeField]
	private LayerMask layerMask;

	// Token: 0x04002FA2 RID: 12194
	[SerializeField]
	private bool useTransferrableObjectState;

	// Token: 0x04002FA3 RID: 12195
	[SerializeField]
	protected UnityEvent OnResetProjectileState;

	// Token: 0x04002FA4 RID: 12196
	[SerializeField]
	protected string boolADebugName;

	// Token: 0x04002FA5 RID: 12197
	[SerializeField]
	protected UnityEvent OnItemStateBoolATrue;

	// Token: 0x04002FA6 RID: 12198
	[SerializeField]
	protected UnityEvent OnItemStateBoolAFalse;

	// Token: 0x04002FA7 RID: 12199
	[SerializeField]
	protected string boolBDebugName;

	// Token: 0x04002FA8 RID: 12200
	[SerializeField]
	protected UnityEvent OnItemStateBoolBTrue;

	// Token: 0x04002FA9 RID: 12201
	[SerializeField]
	protected UnityEvent OnItemStateBoolBFalse;

	// Token: 0x04002FAA RID: 12202
	[SerializeField]
	protected string boolCDebugName;

	// Token: 0x04002FAB RID: 12203
	[SerializeField]
	protected UnityEvent OnItemStateBoolCTrue;

	// Token: 0x04002FAC RID: 12204
	[SerializeField]
	protected UnityEvent OnItemStateBoolCFalse;

	// Token: 0x04002FAD RID: 12205
	[SerializeField]
	protected string boolDDebugName;

	// Token: 0x04002FAE RID: 12206
	[SerializeField]
	protected UnityEvent OnItemStateBoolDTrue;

	// Token: 0x04002FAF RID: 12207
	[SerializeField]
	protected UnityEvent OnItemStateBoolDFalse;

	// Token: 0x04002FB0 RID: 12208
	[SerializeField]
	protected UnityEvent<int> OnItemStateIntChanged;

	// Token: 0x04002FB1 RID: 12209
	private VRRig myRig;

	// Token: 0x04002FB2 RID: 12210
	private float scaleFactor;

	// Token: 0x0200059A RID: 1434
	// (Invoke) Token: 0x06002456 RID: 9302
	public delegate void PaperPlaneHit(Vector3 endPoint);
}
