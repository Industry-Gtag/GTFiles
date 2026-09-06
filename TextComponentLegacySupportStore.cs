using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000C12 RID: 3090
public struct TextComponentLegacySupportStore
{
	// Token: 0x06004D54 RID: 19796 RVA: 0x0019C4EC File Offset: 0x0019A6EC
	public TextComponentLegacySupportStore(Transform objRef)
	{
		this._objectReference = objRef;
		this._legacyTextReference = null;
		this._legacyTextMeshReference = null;
		this._tmpTextReference = objRef.GetComponent<TMP_Text>();
		if (this._tmpTextReference != null)
		{
			return;
		}
		this._legacyTextReference = objRef.GetComponent<Text>();
		if (this._legacyTextReference)
		{
			return;
		}
		this._legacyTextMeshReference = objRef.GetComponent<TextMesh>();
		if (this._legacyTextMeshReference)
		{
			return;
		}
		Debug.LogError("[LOCALIZATION::TEXT_COMPONENT_LEGACY_SUPPORT_STORE] Could not find either a [TMP_Text], Legacy-[Text], or Legacy-[TextMesh] component on object [" + objRef.name + "]", this._objectReference);
	}

	// Token: 0x17000768 RID: 1896
	// (get) Token: 0x06004D55 RID: 19797 RVA: 0x0019C57D File Offset: 0x0019A77D
	public bool IsValid
	{
		get
		{
			return this._tmpTextReference || this._legacyTextReference || this._legacyTextMeshReference;
		}
	}

	// Token: 0x17000769 RID: 1897
	// (get) Token: 0x06004D56 RID: 19798 RVA: 0x0019C5A6 File Offset: 0x0019A7A6
	// (set) Token: 0x06004D57 RID: 19799 RVA: 0x0019C5C6 File Offset: 0x0019A7C6
	public float characterSpacing
	{
		get
		{
			if (this._tmpTextReference)
			{
				return this._tmpTextReference.characterSpacing;
			}
			return 0f;
		}
		set
		{
			if (this._tmpTextReference)
			{
				this._tmpTextReference.characterSpacing = value;
				return;
			}
		}
	}

	// Token: 0x06004D58 RID: 19800 RVA: 0x0019C5E4 File Offset: 0x0019A7E4
	public void SetFont(TMP_FontAsset font, Font legacyFont)
	{
		if (font != null && this._tmpTextReference)
		{
			this.SetFont(font);
			return;
		}
		if (legacyFont != null && (this._legacyTextReference || this._legacyTextMeshReference))
		{
			this.SetFont(legacyFont);
			return;
		}
		if (!this.IsValid)
		{
			Debug.LogError("[LOCALIZATION::TEXT_COMPONENT_LEGACY_SUPPORT_STORE] Trying to change font but both text references are NULL.");
		}
	}

	// Token: 0x06004D59 RID: 19801 RVA: 0x0019C64C File Offset: 0x0019A84C
	public void SetFont(Font font)
	{
		if (this._legacyTextReference)
		{
			this._legacyTextReference.font = font;
			return;
		}
		if (this._legacyTextMeshReference)
		{
			this._legacyTextMeshReference.font = font;
			return;
		}
		Debug.LogError("[LOCALIZATION::TEXT_COMPONENT_LEGACY_SUPPORT_STORE] Trying to change font for non-legacy reference but passed in a legacy font.", font);
	}

	// Token: 0x06004D5A RID: 19802 RVA: 0x0019C698 File Offset: 0x0019A898
	public void SetFont(TMP_FontAsset font)
	{
		if (this._tmpTextReference == null)
		{
			return;
		}
		this._tmpTextReference.font = font;
	}

	// Token: 0x06004D5B RID: 19803 RVA: 0x0019C6B8 File Offset: 0x0019A8B8
	public void SetFontSize(float fontSize)
	{
		if (!this._tmpTextReference)
		{
			return;
		}
		TMP_Text tmpTextReference = this._tmpTextReference;
		this._tmpTextReference.fontSizeMax = fontSize;
		tmpTextReference.fontSize = fontSize;
	}

	// Token: 0x1700076A RID: 1898
	// (get) Token: 0x06004D5C RID: 19804 RVA: 0x0019C6F0 File Offset: 0x0019A8F0
	// (set) Token: 0x06004D5D RID: 19805 RVA: 0x0019C758 File Offset: 0x0019A958
	public string text
	{
		get
		{
			if (this._tmpTextReference)
			{
				return this._tmpTextReference.text;
			}
			if (this._legacyTextReference)
			{
				return this._legacyTextReference.text;
			}
			if (this._legacyTextMeshReference)
			{
				return this._legacyTextMeshReference.text;
			}
			Debug.LogError("[LOCALIZATION::TEXT_COMPONENT_LEGACY_SUPPORT_STORE] Both Legacy Text ref and TMP text ref are null!");
			return "";
		}
		set
		{
			if (this._tmpTextReference != null)
			{
				this._tmpTextReference.text = value;
				return;
			}
			if (this._legacyTextReference != null)
			{
				this._legacyTextReference.text = value;
				return;
			}
			if (this._legacyTextMeshReference)
			{
				this._legacyTextMeshReference.text = value;
				return;
			}
			Debug.LogError("[LOCALIZATION::TEXT_COMPONENT_LEGACY_SUPPORT_STORE] Both Legacy Text ref and TMP text ref are null and cannot be set!", this._objectReference);
		}
	}

	// Token: 0x06004D5E RID: 19806 RVA: 0x0019C7C5 File Offset: 0x0019A9C5
	public void SetText(string newText)
	{
		this.text = newText;
	}

	// Token: 0x06004D5F RID: 19807 RVA: 0x0019C7CE File Offset: 0x0019A9CE
	public void SetCharSpacing(float spacing)
	{
		this.characterSpacing = spacing;
	}

	// Token: 0x040060B2 RID: 24754
	private Transform _objectReference;

	// Token: 0x040060B3 RID: 24755
	private TMP_Text _tmpTextReference;

	// Token: 0x040060B4 RID: 24756
	private Text _legacyTextReference;

	// Token: 0x040060B5 RID: 24757
	private TextMesh _legacyTextMeshReference;
}
