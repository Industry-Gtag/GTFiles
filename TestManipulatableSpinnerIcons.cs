using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000537 RID: 1335
public class TestManipulatableSpinnerIcons : MonoBehaviour
{
	// Token: 0x060021A2 RID: 8610 RVA: 0x000B3756 File Offset: 0x000B1956
	private void Awake()
	{
		this.GenerateRollers();
	}

	// Token: 0x060021A3 RID: 8611 RVA: 0x000B375E File Offset: 0x000B195E
	private void LateUpdate()
	{
		this.currentRotation = this.spinner.angle * this.rotationScale;
		this.UpdateSelectedIndex();
		this.UpdateRollers();
	}

	// Token: 0x060021A4 RID: 8612 RVA: 0x000B3784 File Offset: 0x000B1984
	private void GenerateRollers()
	{
		for (int i = 0; i < this.rollerElementCount; i++)
		{
			float num = this.rollerElementAngle * (float)i + this.rollerElementAngle * 0.5f;
			Object.Instantiate<GameObject>(this.rollerElementTemplate, base.transform).transform.localRotation = Quaternion.Euler(num, 0f, 0f);
			GameObject gameObject = Object.Instantiate<GameObject>(this.iconElementTemplate, this.iconCanvas.transform);
			gameObject.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
			this.visibleIcons.Add(gameObject.GetComponentInChildren<Text>());
		}
		this.rollerElementTemplate.SetActive(false);
		this.iconElementTemplate.SetActive(false);
		this.UpdateRollers();
	}

	// Token: 0x060021A5 RID: 8613 RVA: 0x000B384C File Offset: 0x000B1A4C
	private void UpdateSelectedIndex()
	{
		float num = this.currentRotation / this.rollerElementAngle;
		if (this.rollerElementCount % 2 == 1)
		{
			num += 0.5f;
		}
		this.selectedIndex = Mathf.FloorToInt(num);
		this.selectedIndex %= this.scrollableCount;
		if (this.selectedIndex < 0)
		{
			this.selectedIndex = this.scrollableCount + this.selectedIndex;
		}
	}

	// Token: 0x060021A6 RID: 8614 RVA: 0x000B38B8 File Offset: 0x000B1AB8
	private void UpdateRollers()
	{
		float num = this.currentRotation;
		if (Mathf.Abs(num) > this.rollerElementAngle / 2f)
		{
			if (num > 0f)
			{
				num += this.rollerElementAngle / 2f;
				num %= this.rollerElementAngle;
				num -= this.rollerElementAngle / 2f;
			}
			else
			{
				num -= this.rollerElementAngle / 2f;
				num %= this.rollerElementAngle;
				num += this.rollerElementAngle / 2f;
			}
		}
		num -= (float)this.rollerElementCount / 2f * this.rollerElementAngle;
		base.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
		this.iconCanvas.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
		int num2 = this.rollerElementCount / 2;
		for (int i = 0; i < this.visibleIcons.Count; i++)
		{
			int num3 = this.selectedIndex - i + num2;
			if (num3 < 0)
			{
				num3 += this.scrollableCount;
			}
			else
			{
				num3 %= this.scrollableCount;
			}
			this.visibleIcons[i].text = string.Format("{0}", num3 + 1);
		}
	}

	// Token: 0x04002C62 RID: 11362
	public ManipulatableSpinner spinner;

	// Token: 0x04002C63 RID: 11363
	public float rotationScale = 1f;

	// Token: 0x04002C64 RID: 11364
	public int rollerElementCount = 5;

	// Token: 0x04002C65 RID: 11365
	public GameObject rollerElementTemplate;

	// Token: 0x04002C66 RID: 11366
	public GameObject iconCanvas;

	// Token: 0x04002C67 RID: 11367
	public GameObject iconElementTemplate;

	// Token: 0x04002C68 RID: 11368
	public float iconOffset = 1f;

	// Token: 0x04002C69 RID: 11369
	public float rollerElementAngle = 15f;

	// Token: 0x04002C6A RID: 11370
	private List<Text> visibleIcons = new List<Text>();

	// Token: 0x04002C6B RID: 11371
	private float currentRotation;

	// Token: 0x04002C6C RID: 11372
	public int scrollableCount = 50;

	// Token: 0x04002C6D RID: 11373
	public int selectedIndex;
}
