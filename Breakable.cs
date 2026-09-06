using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000617 RID: 1559
public class Breakable : MonoBehaviour
{
	// Token: 0x060026EF RID: 9967 RVA: 0x000CE064 File Offset: 0x000CC264
	private void Awake()
	{
		this._breakSignal.OnSignal += this.BreakRPC;
		if (this._rigidbody.IsNotNull())
		{
			this.m_useGravity = this._rigidbody.useGravity;
		}
	}

	// Token: 0x060026F0 RID: 9968 RVA: 0x000CE09C File Offset: 0x000CC29C
	private void BreakRPC(int owner, PhotonSignalInfo info)
	{
		VRRig vrrig = base.GetComponent<OwnerRig>();
		if (vrrig == null)
		{
			return;
		}
		if (vrrig.OwningNetPlayer.ActorNumber != owner)
		{
			return;
		}
		if (!this.m_spamChecker.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.OnBreak(true, false);
	}

	// Token: 0x060026F1 RID: 9969 RVA: 0x000CE0EC File Offset: 0x000CC2EC
	private void Setup()
	{
		if (this._collider == null)
		{
			SphereCollider sphereCollider;
			this.GetOrAddComponent(out sphereCollider);
			this._collider = sphereCollider;
		}
		this._collider.enabled = true;
		if (this._rigidbody == null)
		{
			this.GetOrAddComponent(out this._rigidbody);
		}
		this._rigidbody.isKinematic = false;
		this._rigidbody.useGravity = false;
		this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		this.UpdatePhysMasks();
		if (this.rendererRoot == null)
		{
			this._renderers = base.GetComponentsInChildren<Renderer>();
			return;
		}
		this._renderers = this.rendererRoot.GetComponentsInChildren<Renderer>();
	}

	// Token: 0x060026F2 RID: 9970 RVA: 0x000CE193 File Offset: 0x000CC393
	private void OnCollisionEnter(Collision col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x060026F3 RID: 9971 RVA: 0x000CE193 File Offset: 0x000CC393
	private void OnCollisionStay(Collision col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x060026F4 RID: 9972 RVA: 0x000CE193 File Offset: 0x000CC393
	private void OnTriggerEnter(Collider col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x060026F5 RID: 9973 RVA: 0x000CE193 File Offset: 0x000CC393
	private void OnTriggerStay(Collider col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x060026F6 RID: 9974 RVA: 0x000CE19D File Offset: 0x000CC39D
	private void OnEnable()
	{
		this._breakSignal.Enable();
		this._broken = false;
		this.OnSpawn(true);
	}

	// Token: 0x060026F7 RID: 9975 RVA: 0x000CE1B8 File Offset: 0x000CC3B8
	private void OnDisable()
	{
		this._breakSignal.Disable();
		this._broken = false;
		this.OnReset(false);
		this.ShowRenderers(false);
	}

	// Token: 0x060026F8 RID: 9976 RVA: 0x000CE193 File Offset: 0x000CC393
	public void Break()
	{
		this.OnBreak(true, true);
	}

	// Token: 0x060026F9 RID: 9977 RVA: 0x000CE1DA File Offset: 0x000CC3DA
	public void Reset()
	{
		this.OnReset(true);
	}

	// Token: 0x060026FA RID: 9978 RVA: 0x000CE1E4 File Offset: 0x000CC3E4
	protected virtual void ShowRenderers(bool visible)
	{
		if (this._renderers.IsNullOrEmpty<Renderer>())
		{
			return;
		}
		for (int i = 0; i < this._renderers.Length; i++)
		{
			Renderer renderer = this._renderers[i];
			if (renderer)
			{
				renderer.forceRenderingOff = !visible;
			}
		}
	}

	// Token: 0x060026FB RID: 9979 RVA: 0x000CE230 File Offset: 0x000CC430
	protected virtual void OnReset(bool callback = true)
	{
		if (this._breakEffect && this._breakEffect.isPlaying)
		{
			this._breakEffect.Stop();
		}
		this.ShowRenderers(true);
		this._broken = false;
		if (callback)
		{
			UnityEvent<Breakable> unityEvent = this.onReset;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}
	}

	// Token: 0x060026FC RID: 9980 RVA: 0x000CE284 File Offset: 0x000CC484
	protected virtual void OnSpawn(bool callback = true)
	{
		this.startTime = Time.time;
		this.endTime = this.startTime + this.canBreakDelay;
		this.ShowRenderers(true);
		if (this._rigidbody.IsNotNull())
		{
			this._rigidbody.detectCollisions = true;
			this._rigidbody.useGravity = this.m_useGravity;
		}
		if (callback)
		{
			UnityEvent<Breakable> unityEvent = this.onSpawn;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}
	}

	// Token: 0x060026FD RID: 9981 RVA: 0x000CE2F4 File Offset: 0x000CC4F4
	protected virtual void OnBreak(bool callback = true, bool signal = true)
	{
		if (this._broken)
		{
			return;
		}
		if (Time.time < this.endTime)
		{
			return;
		}
		if (this._breakEffect)
		{
			if (this._breakEffect.isPlaying)
			{
				this._breakEffect.Stop();
			}
			this._breakEffect.Play();
		}
		if (signal && PhotonNetwork.InRoom)
		{
			VRRig vrrig = base.GetComponent<OwnerRig>();
			if (vrrig != null)
			{
				this._breakSignal.Raise(vrrig.OwningNetPlayer.ActorNumber);
			}
		}
		this.ShowRenderers(false);
		if (this._rigidbody.IsNotNull())
		{
			this._rigidbody.detectCollisions = false;
			this._rigidbody.useGravity = false;
		}
		this._broken = true;
		if (callback)
		{
			UnityEvent<Breakable> unityEvent = this.onBreak;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}
	}

	// Token: 0x060026FE RID: 9982 RVA: 0x000CE3C4 File Offset: 0x000CC5C4
	private void UpdatePhysMasks()
	{
		int physicsMask = (int)this._physicsMask;
		if (this._collider)
		{
			this._collider.includeLayers = physicsMask;
			this._collider.excludeLayers = ~physicsMask;
		}
		if (this._rigidbody)
		{
			this._rigidbody.includeLayers = physicsMask;
			this._rigidbody.excludeLayers = ~physicsMask;
		}
	}

	// Token: 0x04003263 RID: 12899
	[SerializeField]
	private Collider _collider;

	// Token: 0x04003264 RID: 12900
	[SerializeField]
	private Rigidbody _rigidbody;

	// Token: 0x04003265 RID: 12901
	[SerializeField]
	private GameObject rendererRoot;

	// Token: 0x04003266 RID: 12902
	[SerializeField]
	private Renderer[] _renderers = new Renderer[0];

	// Token: 0x04003267 RID: 12903
	[Space]
	[SerializeField]
	private ParticleSystem _breakEffect;

	// Token: 0x04003268 RID: 12904
	[SerializeField]
	private UnityLayerMask _physicsMask = UnityLayerMask.GorillaHand;

	// Token: 0x04003269 RID: 12905
	public UnityEvent<Breakable> onSpawn;

	// Token: 0x0400326A RID: 12906
	public UnityEvent<Breakable> onBreak;

	// Token: 0x0400326B RID: 12907
	public UnityEvent<Breakable> onReset;

	// Token: 0x0400326C RID: 12908
	public float canBreakDelay = 1f;

	// Token: 0x0400326D RID: 12909
	[SerializeField]
	private PhotonSignal<int> _breakSignal = "_breakSignal";

	// Token: 0x0400326E RID: 12910
	[SerializeField]
	private CallLimiter m_spamChecker = new CallLimiter(2, 1f, 0.5f);

	// Token: 0x0400326F RID: 12911
	[Space]
	[NonSerialized]
	private bool _broken;

	// Token: 0x04003270 RID: 12912
	private bool m_useGravity = true;

	// Token: 0x04003271 RID: 12913
	private float startTime;

	// Token: 0x04003272 RID: 12914
	private float endTime;
}
