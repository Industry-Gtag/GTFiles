using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000A21 RID: 2593
public class GorillaComputerTerminal : MonoBehaviour, IBuildValidation
{
	// Token: 0x06004277 RID: 17015 RVA: 0x0016278C File Offset: 0x0016098C
	public bool BuildValidationCheck()
	{
		if (this.myScreenText == null || this.myFunctionText == null || this.monitorMesh == null)
		{
			Debug.LogErrorFormat(base.gameObject, "gorilla computer terminal {0} is missing screen text, function text, or monitor mesh. this will break lots of computer stuff", new object[] { base.gameObject.name });
			return false;
		}
		return true;
	}

	// Token: 0x06004278 RID: 17016 RVA: 0x001627EA File Offset: 0x001609EA
	private void OnEnable()
	{
		if (GorillaComputer.instance == null)
		{
			base.StartCoroutine(this.<OnEnable>g__OnEnable_Local|4_0());
			return;
		}
		this.Init();
	}

	// Token: 0x06004279 RID: 17017 RVA: 0x00162810 File Offset: 0x00160A10
	private void Init()
	{
		GameEvents.ScreenTextChangedEvent.AddListener(new UnityAction<string>(this.OnScreenTextChanged));
		GameEvents.FunctionSelectTextChangedEvent.AddListener(new UnityAction<string>(this.OnFunctionTextChanged));
		GameEvents.ScreenTextMaterialsEvent.AddListener(new UnityAction<Material[]>(this.OnMaterialsChanged));
		GameEvents.LanguageEvent.AddListener(new UnityAction(this.OnLanguageChanged));
		this.myScreenText.text = GorillaComputer.instance.screenText.currentText;
		this.myFunctionText.text = GorillaComputer.instance.functionSelectText.currentText;
		if (GorillaComputer.instance.screenText.currentMaterials != null)
		{
			this.monitorMesh.sharedMaterials = GorillaComputer.instance.screenText.currentMaterials;
		}
	}

	// Token: 0x0600427A RID: 17018 RVA: 0x001628DC File Offset: 0x00160ADC
	private void OnDisable()
	{
		GameEvents.ScreenTextChangedEvent.RemoveListener(new UnityAction<string>(this.OnScreenTextChanged));
		GameEvents.FunctionSelectTextChangedEvent.RemoveListener(new UnityAction<string>(this.OnFunctionTextChanged));
		GameEvents.ScreenTextMaterialsEvent.RemoveListener(new UnityAction<Material[]>(this.OnMaterialsChanged));
	}

	// Token: 0x0600427B RID: 17019 RVA: 0x0016292B File Offset: 0x00160B2B
	public void OnScreenTextChanged(string text)
	{
		this.myScreenText.text = text;
	}

	// Token: 0x0600427C RID: 17020 RVA: 0x00162939 File Offset: 0x00160B39
	public void OnFunctionTextChanged(string text)
	{
		this.myFunctionText.text = text;
	}

	// Token: 0x0600427D RID: 17021 RVA: 0x00162947 File Offset: 0x00160B47
	private void OnMaterialsChanged(Material[] materials)
	{
		this.monitorMesh.sharedMaterials = materials;
	}

	// Token: 0x0600427E RID: 17022 RVA: 0x00162958 File Offset: 0x00160B58
	private void OnLanguageChanged()
	{
		LocalisationFontPair localisationFontPair;
		if (LocalisationManager.GetFontAssetForCurrentLocale(out localisationFontPair))
		{
			this.myScreenText.font = localisationFontPair.fontAsset;
			this.myFunctionText.font = localisationFontPair.fontAsset;
		}
		this.myScreenText.characterSpacing = localisationFontPair.charSpacing;
	}

	// Token: 0x06004280 RID: 17024 RVA: 0x001629A1 File Offset: 0x00160BA1
	[CompilerGenerated]
	private IEnumerator <OnEnable>g__OnEnable_Local|4_0()
	{
		yield return new WaitUntil(() => GorillaComputer.instance != null);
		yield return null;
		this.Init();
		yield break;
	}

	// Token: 0x04005429 RID: 21545
	public TextMeshPro myScreenText;

	// Token: 0x0400542A RID: 21546
	public TextMeshPro myFunctionText;

	// Token: 0x0400542B RID: 21547
	public MeshRenderer monitorMesh;
}
