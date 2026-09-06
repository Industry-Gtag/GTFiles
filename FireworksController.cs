using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000E1C RID: 3612
public class FireworksController : MonoBehaviour
{
	// Token: 0x06005878 RID: 22648 RVA: 0x001CB87A File Offset: 0x001C9A7A
	private void Awake()
	{
		this._launchOrder = this.fireworks.ToArray<Firework>();
		this._rnd = new SRand(this.seed);
	}

	// Token: 0x06005879 RID: 22649 RVA: 0x001CB8A0 File Offset: 0x001C9AA0
	public void LaunchVolley()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this._rnd.Shuffle<Firework>(this._launchOrder);
		for (int i = 0; i < this._launchOrder.Length; i++)
		{
			MonoBehaviour monoBehaviour = this._launchOrder[i];
			float num = this._rnd.NextFloat() * this.roundLength;
			monoBehaviour.Invoke("Launch", num);
		}
	}

	// Token: 0x0600587A RID: 22650 RVA: 0x001CB904 File Offset: 0x001C9B04
	public void LaunchVolleyRound()
	{
		int num = 0;
		while ((long)num < (long)((ulong)this.roundNumVolleys))
		{
			float num2 = this._rnd.NextFloat() * this.roundLength;
			base.Invoke("LaunchVolley", num2);
			num++;
		}
	}

	// Token: 0x0600587B RID: 22651 RVA: 0x001CB948 File Offset: 0x001C9B48
	public void Launch(Firework fw)
	{
		if (!fw)
		{
			return;
		}
		Vector3 position = fw.origin.position;
		Vector3 position2 = fw.target.position;
		AudioSource sourceOrigin = fw.sourceOrigin;
		int num = this._rnd.NextInt(this.bursts.Length);
		AudioClip audioClip = this.whistles[this._rnd.NextInt(this.whistles.Length)];
		AudioClip audioClip2 = this.bursts[num];
		while (this._lastWhistle == audioClip)
		{
			audioClip = this.whistles[this._rnd.NextInt(this.whistles.Length)];
		}
		while (this._lastBurst == audioClip2)
		{
			num = this._rnd.NextInt(this.bursts.Length);
			audioClip2 = this.bursts[num];
		}
		this._lastWhistle = audioClip;
		this._lastBurst = audioClip2;
		int num2 = this._rnd.NextInt(fw.explosions.Length);
		ParticleSystem particleSystem = fw.explosions[num2];
		if (fw.doTrail)
		{
			ParticleSystem trail = fw.trail;
			trail.startColor = fw.colorOrigin;
			trail.subEmitters.GetSubEmitterSystem(0).colorOverLifetime.color = new ParticleSystem.MinMaxGradient(fw.colorOrigin, fw.colorTarget);
			trail.Stop();
			trail.Play();
		}
		sourceOrigin.pitch = this._rnd.NextFloat(0.92f, 1f);
		fw.doTrailAudio = this._rnd.NextBool();
		FireworksController.ExplosionEvent explosionEvent = new FireworksController.ExplosionEvent
		{
			firework = fw,
			timeSince = TimeSince.Now(),
			burstIndex = num,
			explosionIndex = num2,
			delay = (double)(fw.doTrail ? audioClip.length : 0f),
			active = true
		};
		if (fw.doExplosion)
		{
			this.PostExplosionEvent(explosionEvent);
		}
		if (fw.doTrailAudio && this._timeSinceLastWhistle > this.minWhistleDelay)
		{
			this._timeSinceLastWhistle = TimeSince.Now();
			sourceOrigin.PlayOneShot(audioClip, this._rnd.NextFloat(this.whistleVolumeMin, this.whistleVolumeMax));
		}
		particleSystem.Stop();
		particleSystem.transform.position = position2;
	}

	// Token: 0x0600587C RID: 22652 RVA: 0x001CBB78 File Offset: 0x001C9D78
	private void PostExplosionEvent(FireworksController.ExplosionEvent ev)
	{
		for (int i = 0; i < this._explosionQueue.Length; i++)
		{
			if (!this._explosionQueue[i].active)
			{
				this._explosionQueue[i] = ev;
				return;
			}
		}
	}

	// Token: 0x0600587D RID: 22653 RVA: 0x001CBBB9 File Offset: 0x001C9DB9
	private void Update()
	{
		this.ProcessEvents();
	}

	// Token: 0x0600587E RID: 22654 RVA: 0x001CBBC4 File Offset: 0x001C9DC4
	private void ProcessEvents()
	{
		if (this._explosionQueue == null || this._explosionQueue.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this._explosionQueue.Length; i++)
		{
			FireworksController.ExplosionEvent explosionEvent = this._explosionQueue[i];
			if (explosionEvent.active && explosionEvent.timeSince >= explosionEvent.delay)
			{
				this.DoExplosion(explosionEvent);
				this._explosionQueue[i] = default(FireworksController.ExplosionEvent);
			}
		}
	}

	// Token: 0x0600587F RID: 22655 RVA: 0x001CBC38 File Offset: 0x001C9E38
	private void DoExplosion(FireworksController.ExplosionEvent ev)
	{
		Firework firework = ev.firework;
		ParticleSystem particleSystem = firework.explosions[ev.explosionIndex];
		ParticleSystem.MinMaxGradient minMaxGradient = new ParticleSystem.MinMaxGradient(firework.colorOrigin, firework.colorTarget);
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime2 = particleSystem.subEmitters.GetSubEmitterSystem(0).colorOverLifetime;
		colorOverLifetime.color = minMaxGradient;
		colorOverLifetime2.color = minMaxGradient;
		ParticleSystem particleSystem2 = firework.explosions[ev.explosionIndex];
		particleSystem2.Stop();
		particleSystem2.Play();
		firework.sourceTarget.PlayOneShot(this.bursts[ev.burstIndex]);
	}

	// Token: 0x06005880 RID: 22656 RVA: 0x001CBCC8 File Offset: 0x001C9EC8
	public void RenderGizmo(Firework fw, Color c)
	{
		if (!fw)
		{
			return;
		}
		if (!fw.origin || !fw.target)
		{
			return;
		}
		Gizmos.color = c;
		Vector3 position = fw.origin.position;
		Vector3 position2 = fw.target.position;
		Gizmos.DrawLine(position, position2);
		Gizmos.DrawWireCube(position, Vector3.one * 0.5f);
		Gizmos.DrawWireCube(position2, Vector3.one * 0.5f);
	}

	// Token: 0x0400689A RID: 26778
	public Firework[] fireworks;

	// Token: 0x0400689B RID: 26779
	public AudioClip[] whistles;

	// Token: 0x0400689C RID: 26780
	public AudioClip[] bursts;

	// Token: 0x0400689D RID: 26781
	[Space]
	[Range(0f, 1f)]
	public float whistleVolumeMin = 0.1f;

	// Token: 0x0400689E RID: 26782
	[Range(0f, 1f)]
	public float whistleVolumeMax = 0.15f;

	// Token: 0x0400689F RID: 26783
	public float minWhistleDelay = 1f;

	// Token: 0x040068A0 RID: 26784
	[Space]
	[NonSerialized]
	private AudioClip _lastWhistle;

	// Token: 0x040068A1 RID: 26785
	[NonSerialized]
	private AudioClip _lastBurst;

	// Token: 0x040068A2 RID: 26786
	[NonSerialized]
	private Firework[] _launchOrder;

	// Token: 0x040068A3 RID: 26787
	[NonSerialized]
	private SRand _rnd;

	// Token: 0x040068A4 RID: 26788
	[NonSerialized]
	private FireworksController.ExplosionEvent[] _explosionQueue = new FireworksController.ExplosionEvent[8];

	// Token: 0x040068A5 RID: 26789
	[NonSerialized]
	private TimeSince _timeSinceLastWhistle = 10f;

	// Token: 0x040068A6 RID: 26790
	[Space]
	public string seed = "Fireworks.Summer23";

	// Token: 0x040068A7 RID: 26791
	[Space]
	public uint roundNumVolleys = 6U;

	// Token: 0x040068A8 RID: 26792
	public uint roundLength = 6U;

	// Token: 0x040068A9 RID: 26793
	[FormerlySerializedAs("_timeOfDayEvent")]
	[FormerlySerializedAs("_timeOfDay")]
	[Space]
	[SerializeField]
	private TimeEvent _fireworksEvent;

	// Token: 0x02000E1D RID: 3613
	[Serializable]
	public struct ExplosionEvent
	{
		// Token: 0x040068AA RID: 26794
		public TimeSince timeSince;

		// Token: 0x040068AB RID: 26795
		public double delay;

		// Token: 0x040068AC RID: 26796
		public int explosionIndex;

		// Token: 0x040068AD RID: 26797
		public int burstIndex;

		// Token: 0x040068AE RID: 26798
		public bool active;

		// Token: 0x040068AF RID: 26799
		public Firework firework;
	}
}
