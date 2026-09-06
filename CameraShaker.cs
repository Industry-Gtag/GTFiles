using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000037 RID: 55
public class CameraShaker : MonoBehaviour
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x060000D1 RID: 209 RVA: 0x00005A48 File Offset: 0x00003C48
	// (remove) Token: 0x060000D2 RID: 210 RVA: 0x00005A7C File Offset: 0x00003C7C
	private static event Action<float, float, Vector2, bool, Transform, float> ShakeRequested;

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x060000D3 RID: 211 RVA: 0x00005AB0 File Offset: 0x00003CB0
	// (remove) Token: 0x060000D4 RID: 212 RVA: 0x00005AE4 File Offset: 0x00003CE4
	private static event Action HaltRequested;

	// Token: 0x060000D5 RID: 213 RVA: 0x00005B17 File Offset: 0x00003D17
	public static void Shake(float duration, float magnitude)
	{
		if (CameraShaker.ShakeRequested != null)
		{
			CameraShaker.ShakeRequested(duration, magnitude, new Vector2(0.02f, 0.1f), true, null, 0f);
		}
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x00005B42 File Offset: 0x00003D42
	public static void Shake(float duration, float magnitude, Vector2 freqRange)
	{
		if (CameraShaker.ShakeRequested != null)
		{
			CameraShaker.ShakeRequested(duration, magnitude, freqRange, true, null, 0f);
		}
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00005B5F File Offset: 0x00003D5F
	public static void Shake(float duration, float magnitude, Vector2 freqRange, bool rollOffOverDuration)
	{
		if (CameraShaker.ShakeRequested != null)
		{
			CameraShaker.ShakeRequested(duration, magnitude, freqRange, rollOffOverDuration, null, 0f);
		}
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00005B7C File Offset: 0x00003D7C
	public static void ShakeInProximity(float duration, float magnitude, Vector2 freqRange, bool rollOffOverDuration, Transform source, float distance)
	{
		if (CameraShaker.ShakeRequested != null)
		{
			CameraShaker.ShakeRequested(duration, magnitude, freqRange, rollOffOverDuration, source, distance);
		}
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x00005B97 File Offset: 0x00003D97
	public static void Halt()
	{
		if (CameraShaker.HaltRequested != null)
		{
			CameraShaker.HaltRequested();
		}
	}

	// Token: 0x060000DA RID: 218 RVA: 0x00005BAA File Offset: 0x00003DAA
	private void OnEnable()
	{
		CameraShaker.ShakeRequested += this._ShakeRequested;
		CameraShaker.HaltRequested += this._HaltRequested;
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00005BD0 File Offset: 0x00003DD0
	private void _ShakeRequested(float _duration, float _magnitude, Vector2 _freqRange, bool _rollOff, Transform source, float distance)
	{
		this.stopTime = Time.time + _duration;
		this.duration = _duration;
		this.magnitude = _magnitude;
		this.freqRange = _freqRange;
		this.rollOff = _rollOff;
		if (!this.rumbling && (source == null || (base.transform.position - source.transform.position).sqrMagnitude < distance * distance))
		{
			base.StartCoroutine(this.crRumble());
		}
	}

	// Token: 0x060000DC RID: 220 RVA: 0x00005C51 File Offset: 0x00003E51
	private void _HaltRequested()
	{
		this.stopTime = Time.time;
	}

	// Token: 0x060000DD RID: 221 RVA: 0x00005C5E File Offset: 0x00003E5E
	private void OnDisable()
	{
		CameraShaker.ShakeRequested -= this._ShakeRequested;
		CameraShaker.HaltRequested -= this._HaltRequested;
	}

	// Token: 0x060000DE RID: 222 RVA: 0x00005C5E File Offset: 0x00003E5E
	private void OnDestroy()
	{
		CameraShaker.ShakeRequested -= this._ShakeRequested;
		CameraShaker.HaltRequested -= this._HaltRequested;
	}

	// Token: 0x060000DF RID: 223 RVA: 0x00005C82 File Offset: 0x00003E82
	private IEnumerator crRumble()
	{
		this.rumbling = true;
		while (this.stopTime > Time.time)
		{
			Vector3 vector = Random.insideUnitSphere * this.magnitude;
			if (this.rollOff)
			{
				vector *= (this.stopTime - Time.time) / this.duration;
			}
			base.transform.localPosition += vector;
			yield return new WaitForSeconds(Random.Range(this.freqRange.x, this.freqRange.y));
		}
		this.rumbling = false;
		yield break;
	}

	// Token: 0x040000E9 RID: 233
	private bool rumbling;

	// Token: 0x040000EA RID: 234
	private float stopTime;

	// Token: 0x040000ED RID: 237
	private bool rollOff;

	// Token: 0x040000EE RID: 238
	private float magnitude;

	// Token: 0x040000EF RID: 239
	private float duration;

	// Token: 0x040000F0 RID: 240
	private Vector2 freqRange;
}
