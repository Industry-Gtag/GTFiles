using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020004E4 RID: 1252
public class RigEventVolumeObserver : MonoBehaviour
{
	// Token: 0x06001E80 RID: 7808 RVA: 0x000A363C File Offset: 0x000A183C
	private void Awake()
	{
		for (int i = 0; i < this.tMP_Texts.Length; i++)
		{
			this.formats.Add(this.tMP_Texts[i].text);
		}
	}

	// Token: 0x06001E81 RID: 7809 RVA: 0x000A3674 File Offset: 0x000A1874
	private void OnEnable()
	{
		this.Observed_OnCountChanged();
		this.observed.OnCountChanged += this.Observed_OnCountChanged;
	}

	// Token: 0x06001E82 RID: 7810 RVA: 0x000A3693 File Offset: 0x000A1893
	private void OnDisable()
	{
		this.observed.OnCountChanged -= this.Observed_OnCountChanged;
	}

	// Token: 0x06001E83 RID: 7811 RVA: 0x000A36AC File Offset: 0x000A18AC
	private void Observed_OnCountChanged()
	{
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			this.gameObjects[i].ApplyActiveState(this.observed);
		}
		for (int j = 0; j < this.tMP_Texts.Length; j++)
		{
			this.tMP_Texts[j].text = this.Format(this.formats[j]);
		}
	}

	// Token: 0x06001E84 RID: 7812 RVA: 0x000A3714 File Offset: 0x000A1914
	private string Format(string s)
	{
		return s.Replace("\\c", this.observed.RigCount.ToString());
	}

	// Token: 0x040028B2 RID: 10418
	[SerializeField]
	private RigEventVolume observed;

	// Token: 0x040028B3 RID: 10419
	[SerializeField]
	private RigEventVolumeObserver.RigEventVolumeObserverGameObject[] gameObjects;

	// Token: 0x040028B4 RID: 10420
	[SerializeField]
	private TMP_Text[] tMP_Texts;

	// Token: 0x040028B5 RID: 10421
	private List<string> formats = new List<string>();

	// Token: 0x020004E5 RID: 1253
	[Serializable]
	private class RigEventVolumeObserverGameObject
	{
		// Token: 0x06001E86 RID: 7814 RVA: 0x000A3754 File Offset: 0x000A1954
		public bool Check(RigEventVolume rev)
		{
			switch (this.comparison)
			{
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.EQ:
				return rev.RigCount == this.value;
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.LT:
				return rev.RigCount < this.value;
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.GT:
				return rev.RigCount > this.value;
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.LT_EQ:
				return rev.RigCount <= this.value;
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.GT_EQ:
				return rev.RigCount >= this.value;
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.NEQ:
				return rev.RigCount != this.value;
			default:
				return false;
			}
		}

		// Token: 0x06001E87 RID: 7815 RVA: 0x000A37EC File Offset: 0x000A19EC
		public void ApplyActiveState(RigEventVolume rev)
		{
			this.gameObject.SetActive(this.Check(rev));
		}

		// Token: 0x040028B6 RID: 10422
		[SerializeField]
		private GameObject gameObject;

		// Token: 0x040028B7 RID: 10423
		[SerializeField]
		public RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison comparison;

		// Token: 0x040028B8 RID: 10424
		[SerializeField]
		public int value;

		// Token: 0x020004E6 RID: 1254
		public enum Comparison
		{
			// Token: 0x040028BA RID: 10426
			EQ,
			// Token: 0x040028BB RID: 10427
			LT,
			// Token: 0x040028BC RID: 10428
			GT,
			// Token: 0x040028BD RID: 10429
			LT_EQ,
			// Token: 0x040028BE RID: 10430
			GT_EQ,
			// Token: 0x040028BF RID: 10431
			NEQ
		}
	}
}
