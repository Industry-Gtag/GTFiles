using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000B8F RID: 2959
public class MessageBox : MonoBehaviour
{
	// Token: 0x1700072F RID: 1839
	// (get) Token: 0x06004AC5 RID: 19141 RVA: 0x0018F7EB File Offset: 0x0018D9EB
	// (set) Token: 0x06004AC6 RID: 19142 RVA: 0x0018F7F3 File Offset: 0x0018D9F3
	public MessageBoxResult Result { get; private set; }

	// Token: 0x17000730 RID: 1840
	// (get) Token: 0x06004AC7 RID: 19143 RVA: 0x0018F7FC File Offset: 0x0018D9FC
	// (set) Token: 0x06004AC8 RID: 19144 RVA: 0x0018F809 File Offset: 0x0018DA09
	public string Header
	{
		get
		{
			return this._headerText.text;
		}
		set
		{
			this._headerText.text = value;
			this._headerText.gameObject.SetActive(!string.IsNullOrEmpty(value));
		}
	}

	// Token: 0x17000731 RID: 1841
	// (get) Token: 0x06004AC9 RID: 19145 RVA: 0x0018F830 File Offset: 0x0018DA30
	// (set) Token: 0x06004ACA RID: 19146 RVA: 0x0018F83D File Offset: 0x0018DA3D
	public string Body
	{
		get
		{
			return this._bodyText.text;
		}
		set
		{
			this._bodyText.text = value;
		}
	}

	// Token: 0x17000732 RID: 1842
	// (get) Token: 0x06004ACB RID: 19147 RVA: 0x0018F84B File Offset: 0x0018DA4B
	// (set) Token: 0x06004ACC RID: 19148 RVA: 0x0018F858 File Offset: 0x0018DA58
	public string LeftButton
	{
		get
		{
			return this._leftButtonText.text;
		}
		set
		{
			this._leftButtonText.text = value;
			this._leftButton.SetActive(!string.IsNullOrEmpty(value));
			if (string.IsNullOrEmpty(value))
			{
				RectTransform component = this._rightButton.GetComponent<RectTransform>();
				component.anchorMin = new Vector2(0.5f, 0.5f);
				component.anchorMax = new Vector2(0.5f, 0.5f);
				component.pivot = new Vector2(0.5f, 0.5f);
				component.anchoredPosition = Vector3.zero;
				return;
			}
			RectTransform component2 = this._rightButton.GetComponent<RectTransform>();
			component2.anchorMin = new Vector2(1f, 0.5f);
			component2.anchorMax = new Vector2(1f, 0.5f);
			component2.pivot = new Vector2(1f, 0.5f);
			component2.anchoredPosition = Vector3.zero;
		}
	}

	// Token: 0x17000733 RID: 1843
	// (get) Token: 0x06004ACD RID: 19149 RVA: 0x0018F940 File Offset: 0x0018DB40
	// (set) Token: 0x06004ACE RID: 19150 RVA: 0x0018F950 File Offset: 0x0018DB50
	public string RightButton
	{
		get
		{
			return this._rightButtonText.text;
		}
		set
		{
			this._rightButtonText.text = value;
			this._rightButton.SetActive(!string.IsNullOrEmpty(value));
			if (string.IsNullOrEmpty(value))
			{
				RectTransform component = this._leftButton.GetComponent<RectTransform>();
				component.anchorMin = new Vector2(0.5f, 0.5f);
				component.anchorMax = new Vector2(0.5f, 0.5f);
				component.pivot = new Vector2(0.5f, 0.5f);
				component.anchoredPosition3D = Vector3.zero;
				return;
			}
			RectTransform component2 = this._leftButton.GetComponent<RectTransform>();
			component2.anchorMin = new Vector2(0f, 0.5f);
			component2.anchorMax = new Vector2(0f, 0.5f);
			component2.pivot = new Vector2(0f, 0.5f);
			component2.anchoredPosition3D = Vector3.zero;
		}
	}

	// Token: 0x17000734 RID: 1844
	// (get) Token: 0x06004ACF RID: 19151 RVA: 0x0018FA2E File Offset: 0x0018DC2E
	public UnityEvent LeftButtonCallback
	{
		get
		{
			return this._leftButtonCallback;
		}
	}

	// Token: 0x17000735 RID: 1845
	// (get) Token: 0x06004AD0 RID: 19152 RVA: 0x0018FA36 File Offset: 0x0018DC36
	public UnityEvent RightButtonCallback
	{
		get
		{
			return this._rightButtonCallback;
		}
	}

	// Token: 0x06004AD1 RID: 19153 RVA: 0x0018FA3E File Offset: 0x0018DC3E
	private void Start()
	{
		this.Result = MessageBoxResult.None;
	}

	// Token: 0x06004AD2 RID: 19154 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Update()
	{
	}

	// Token: 0x06004AD3 RID: 19155 RVA: 0x0018FA48 File Offset: 0x0018DC48
	public void ShowQuitButtonAsPrimary()
	{
		this._leftButton.SetActive(false);
		RectTransform component = this._rightButton.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0.5f, 0.5f);
		component.anchorMax = new Vector2(0.5f, 0.5f);
		component.pivot = new Vector2(0.5f, 0.5f);
		component.anchoredPosition = Vector3.zero;
	}

	// Token: 0x06004AD4 RID: 19156 RVA: 0x0018FABA File Offset: 0x0018DCBA
	public void OnClickLeftButton()
	{
		this.Result = MessageBoxResult.Left;
		this._leftButtonCallback.Invoke();
	}

	// Token: 0x06004AD5 RID: 19157 RVA: 0x0018FACE File Offset: 0x0018DCCE
	public void OnClickRightButton()
	{
		this.Result = MessageBoxResult.Right;
		this._rightButtonCallback.Invoke();
	}

	// Token: 0x06004AD6 RID: 19158 RVA: 0x0018FAE2 File Offset: 0x0018DCE2
	public GameObject GetCanvas()
	{
		return base.GetComponentInChildren<Canvas>(true).gameObject;
	}

	// Token: 0x06004AD7 RID: 19159 RVA: 0x0018FAF0 File Offset: 0x0018DCF0
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x04005DA7 RID: 23975
	[SerializeField]
	private TMP_Text _headerText;

	// Token: 0x04005DA8 RID: 23976
	[SerializeField]
	private TMP_Text _bodyText;

	// Token: 0x04005DA9 RID: 23977
	[SerializeField]
	private TMP_Text _leftButtonText;

	// Token: 0x04005DAA RID: 23978
	[SerializeField]
	private TMP_Text _rightButtonText;

	// Token: 0x04005DAB RID: 23979
	[SerializeField]
	private GameObject _leftButton;

	// Token: 0x04005DAC RID: 23980
	[SerializeField]
	private GameObject _rightButton;

	// Token: 0x04005DAE RID: 23982
	[SerializeField]
	private UnityEvent _leftButtonCallback = new UnityEvent();

	// Token: 0x04005DAF RID: 23983
	[SerializeField]
	private UnityEvent _rightButtonCallback = new UnityEvent();
}
