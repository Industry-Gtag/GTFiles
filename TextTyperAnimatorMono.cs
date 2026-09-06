using System;
using System.Collections.Generic;
using Cysharp.Text;
using GorillaExtensions;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000BB RID: 187
public class TextTyperAnimatorMono : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06000487 RID: 1159 RVA: 0x00019D15 File Offset: 0x00017F15
	public void EdRestartAnimation()
	{
		this.m_textMesh.maxVisibleCharacters = 0;
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x00019D24 File Offset: 0x00017F24
	protected void Awake()
	{
		this._has_typingSoundBank = this.m_typingSoundBank != null;
		this._has_beginEntrySoundBank = this.m_beginEntrySoundBank != null;
		this._waitTime = this._random.NextFloat(this.m_typingSpeedMinMax.x, this.m_typingSpeedMinMax.y);
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x0001212B File Offset: 0x0001032B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x00012134 File Offset: 0x00010334
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x00019D7C File Offset: 0x00017F7C
	public void SliceUpdate()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		int num = this.m_textMesh.maxVisibleCharacters;
		if (num < 0 || num >= this._charCount || this._timeOfLastTypedChar + this._waitTime > realtimeSinceStartup)
		{
			return;
		}
		num = (this.m_textMesh.maxVisibleCharacters = num + 1);
		this._timeOfLastTypedChar = realtimeSinceStartup;
		if (this._has_beginEntrySoundBank && num == 1)
		{
			this.m_beginEntrySoundBank.Play();
		}
		else if (this._has_typingSoundBank)
		{
			this.m_typingSoundBank.Play();
		}
		this._waitTime = this._random.NextFloat(this.m_typingSpeedMinMax.x, this.m_typingSpeedMinMax.y);
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x00019E23 File Offset: 0x00018023
	public void SetText(string text, IList<int> entryIndexes, int nonRichTextTagsCharCount)
	{
		this._charCount = nonRichTextTagsCharCount;
		this.m_textMesh.SetText(text);
		this.m_textMesh.maxVisibleCharacters = 0;
		this._SetEntryIndexes(entryIndexes);
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x00019E4B File Offset: 0x0001804B
	public void SetText(string text, IList<int> entryIndexes)
	{
		this.SetText(text, entryIndexes, text.Length);
		this.m_textMesh.SetText(text);
		this.m_textMesh.maxVisibleCharacters = 0;
		this._SetEntryIndexes(entryIndexes);
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x00019E7A File Offset: 0x0001807A
	public void SetText(string text)
	{
		this.SetText(text, Array.Empty<int>());
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x00019E88 File Offset: 0x00018088
	public void SetText(Utf16ValueStringBuilder zStringBuilder, IList<int> entryIndexes, int nonRichTextTagsCharCount)
	{
		this._charCount = nonRichTextTagsCharCount;
		this.m_textMesh.SetTextToZString(zStringBuilder);
		this.m_textMesh.maxVisibleCharacters = 0;
		this._SetEntryIndexes(entryIndexes);
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x00019EB0 File Offset: 0x000180B0
	public void SetText(Utf16ValueStringBuilder zStringBuilder)
	{
		this.SetText(zStringBuilder, Array.Empty<int>(), zStringBuilder.Length);
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x00019EC5 File Offset: 0x000180C5
	private void _SetEntryIndexes(IList<int> entryIndexes)
	{
		this._entryIndexes.Clear();
		this._entryIndexes.AddRange(entryIndexes);
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x00019EE0 File Offset: 0x000180E0
	public void UpdateText(Utf16ValueStringBuilder zStringBuilder, int nonRichTextTagsCharCount)
	{
		TMP_Text textMesh = this.m_textMesh;
		this._charCount = nonRichTextTagsCharCount;
		textMesh.maxVisibleCharacters = nonRichTextTagsCharCount;
		this.m_textMesh.SetTextToZString(zStringBuilder);
	}

	// Token: 0x040004E7 RID: 1255
	[FormerlySerializedAs("_textMesh")]
	[Tooltip("Text Mesh Pro component.")]
	[SerializeField]
	private TMP_Text m_textMesh;

	// Token: 0x040004E8 RID: 1256
	[Tooltip("Delay between characters in seconds")]
	[SerializeField]
	private Vector2 m_typingSpeedMinMax = new Vector2(0.05f, 0.1f);

	// Token: 0x040004E9 RID: 1257
	[Header("Audio")]
	[Tooltip("AudioClips to play while typing.")]
	[SerializeField]
	private SoundBankPlayer m_typingSoundBank;

	// Token: 0x040004EA RID: 1258
	private bool _has_typingSoundBank;

	// Token: 0x040004EB RID: 1259
	[Tooltip("AudioClips to play when a ")]
	[SerializeField]
	private SoundBankPlayer m_beginEntrySoundBank;

	// Token: 0x040004EC RID: 1260
	private bool _has_beginEntrySoundBank;

	// Token: 0x040004ED RID: 1261
	private int _charCount;

	// Token: 0x040004EE RID: 1262
	private readonly List<int> _entryIndexes = new List<int>(16);

	// Token: 0x040004EF RID: 1263
	private float _waitTime;

	// Token: 0x040004F0 RID: 1264
	private float _timeOfLastTypedChar = -1f;

	// Token: 0x040004F1 RID: 1265
	private Unity.Mathematics.Random _random = new Unity.Mathematics.Random(6746U);
}
