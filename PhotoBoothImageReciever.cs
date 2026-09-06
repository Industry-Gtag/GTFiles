using System;
using UnityEngine;

// Token: 0x02000496 RID: 1174
public class PhotoBoothImageReciever : MonoBehaviour
{
	// Token: 0x06001C87 RID: 7303 RVA: 0x0009AB6A File Offset: 0x00098D6A
	private void OnEnable()
	{
		PhotoBoothCamera photoBoothCamera = this.photoBoothCamera;
		photoBoothCamera.OnCapture = (Action<Texture, int>)Delegate.Combine(photoBoothCamera.OnCapture, new Action<Texture, int>(this.photoBoothCamera_OnCapture));
	}

	// Token: 0x06001C88 RID: 7304 RVA: 0x0009AB93 File Offset: 0x00098D93
	private void photoBoothCamera_OnCapture(Texture texture, int i)
	{
		if (this.index < 0 || this.index == i)
		{
			base.GetComponent<Renderer>().material.mainTexture = texture;
		}
	}

	// Token: 0x06001C89 RID: 7305 RVA: 0x0009ABB8 File Offset: 0x00098DB8
	private void OnDisable()
	{
		PhotoBoothCamera photoBoothCamera = this.photoBoothCamera;
		photoBoothCamera.OnCapture = (Action<Texture, int>)Delegate.Remove(photoBoothCamera.OnCapture, new Action<Texture, int>(this.photoBoothCamera_OnCapture));
	}

	// Token: 0x040026AA RID: 9898
	[SerializeField]
	private PhotoBoothCamera photoBoothCamera;

	// Token: 0x040026AB RID: 9899
	[SerializeField]
	private int index = -1;
}
