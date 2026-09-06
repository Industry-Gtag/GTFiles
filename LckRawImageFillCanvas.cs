using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000408 RID: 1032
[ExecuteInEditMode]
public class LckRawImageFillCanvas : UIBehaviour
{
	// Token: 0x06001870 RID: 6256 RVA: 0x0008B1A2 File Offset: 0x000893A2
	private new void OnEnable()
	{
		this.UpdateSizeDelta();
	}

	// Token: 0x06001871 RID: 6257 RVA: 0x0008B1A2 File Offset: 0x000893A2
	private void Update()
	{
		this.UpdateSizeDelta();
	}

	// Token: 0x06001872 RID: 6258 RVA: 0x0008B1AC File Offset: 0x000893AC
	private void UpdateSizeDelta()
	{
		if (this._rawImage == null || this._rawImage.texture == null)
		{
			return;
		}
		RectTransform rectTransform = this._rawImage.rectTransform;
		Vector2 sizeDelta = ((RectTransform)rectTransform.parent).sizeDelta;
		Vector2 vector = new Vector2((float)this._rawImage.texture.width, (float)this._rawImage.texture.height);
		float num = sizeDelta.x / sizeDelta.y;
		float num2 = vector.x / vector.y;
		float num3 = num / num2;
		Vector2 vector2 = new Vector2(sizeDelta.x, sizeDelta.x / num2);
		Vector2 vector3 = new Vector2(sizeDelta.y * num2, sizeDelta.y);
		switch (this._scaleType)
		{
		case LckRawImageFillCanvas.ScaleType.Fill:
			rectTransform.sizeDelta = ((num3 > 1f) ? vector2 : vector3);
			return;
		case LckRawImageFillCanvas.ScaleType.Inset:
			rectTransform.sizeDelta = ((num3 < 1f) ? vector2 : vector3);
			return;
		case LckRawImageFillCanvas.ScaleType.Stretch:
			rectTransform.sizeDelta = sizeDelta;
			return;
		default:
			return;
		}
	}

	// Token: 0x040023A6 RID: 9126
	[SerializeField]
	private RawImage _rawImage;

	// Token: 0x040023A7 RID: 9127
	[SerializeField]
	private LckRawImageFillCanvas.ScaleType _scaleType;

	// Token: 0x02000409 RID: 1033
	private enum ScaleType
	{
		// Token: 0x040023A9 RID: 9129
		Fill,
		// Token: 0x040023AA RID: 9130
		Inset,
		// Token: 0x040023AB RID: 9131
		Stretch
	}
}
