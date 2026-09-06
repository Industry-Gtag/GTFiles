using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000335 RID: 821
public static class GTAudioSourceExtensions
{
	// Token: 0x06001449 RID: 5193 RVA: 0x0006DA2F File Offset: 0x0006BC2F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPlayOneShot(this AudioSource audioSource, IList<AudioClip> clips, float volumeScale = 1f)
	{
		audioSource.PlayOneShot(clips[Random.Range(0, clips.Count)], volumeScale);
	}

	// Token: 0x0600144A RID: 5194 RVA: 0x0006DA4A File Offset: 0x0006BC4A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPlayOneShot(this AudioSource audioSource, AudioClip clip, float volumeScale = 1f)
	{
		audioSource.PlayOneShot(clip, volumeScale);
	}

	// Token: 0x0600144B RID: 5195 RVA: 0x0006DA54 File Offset: 0x0006BC54
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPlay(this AudioSource audioSource)
	{
		audioSource.Play();
	}

	// Token: 0x0600144C RID: 5196 RVA: 0x0006DA5C File Offset: 0x0006BC5C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPlay(this AudioSource audioSource, ulong delay)
	{
		audioSource.Play(delay);
	}

	// Token: 0x0600144D RID: 5197 RVA: 0x0006DA65 File Offset: 0x0006BC65
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPause(this AudioSource audioSource)
	{
		audioSource.Pause();
	}

	// Token: 0x0600144E RID: 5198 RVA: 0x0006DA6D File Offset: 0x0006BC6D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTUnPause(this AudioSource audioSource)
	{
		audioSource.UnPause();
	}

	// Token: 0x0600144F RID: 5199 RVA: 0x0006DA75 File Offset: 0x0006BC75
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTStop(this AudioSource audioSource)
	{
		audioSource.Stop();
	}

	// Token: 0x06001450 RID: 5200 RVA: 0x0006DA7D File Offset: 0x0006BC7D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPlayDelayed(this AudioSource audioSource, float delay)
	{
		audioSource.PlayDelayed(delay);
	}

	// Token: 0x06001451 RID: 5201 RVA: 0x0006DA86 File Offset: 0x0006BC86
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPlayScheduled(this AudioSource audioSource, double time)
	{
		audioSource.PlayScheduled(time);
	}

	// Token: 0x06001452 RID: 5202 RVA: 0x0006DA8F File Offset: 0x0006BC8F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPlayClipAtPoint(AudioClip clip, Vector3 position)
	{
		AudioSource.PlayClipAtPoint(clip, position);
	}

	// Token: 0x06001453 RID: 5203 RVA: 0x0006DA98 File Offset: 0x0006BC98
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPlayClipAtPoint(AudioClip clip, Vector3 position, float volume)
	{
		AudioSource.PlayClipAtPoint(clip, position, volume);
	}

	// Token: 0x06001454 RID: 5204 RVA: 0x00002C2D File Offset: 0x00000E2D
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	private static void _BetaLogIfAudioSourceIsNotActiveAndEnabled(AudioSource audioSource)
	{
	}
}
