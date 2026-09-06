using System;
using System.Collections.Generic;
using System.IO;
using GorillaNetworking;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

// Token: 0x02000494 RID: 1172
public class PhotoBoothCamera : MonoBehaviour
{
	// Token: 0x06001C7E RID: 7294 RVA: 0x0009A80A File Offset: 0x00098A0A
	public void SetSaveImageToDevice(bool b)
	{
		this.saveImageToDevice = b;
	}

	// Token: 0x06001C7F RID: 7295 RVA: 0x0009A813 File Offset: 0x00098A13
	public void Clear()
	{
		this.rt.Clear();
	}

	// Token: 0x06001C80 RID: 7296 RVA: 0x0009A820 File Offset: 0x00098A20
	public void Capture(float FOV = 60f)
	{
		this.cam.fieldOfView = FOV;
		this.cam.Render();
		this.rt.Add(new RenderTexture(this.renderTexture.width, this.renderTexture.height, 1));
		Graphics.Blit(this.renderTexture, this.rt[this.rt.Count - 1]);
		Action<Texture, int> onCapture = this.OnCapture;
		if (onCapture != null)
		{
			onCapture(this.rt[this.rt.Count - 1], this.rt.Count - 1);
		}
		UnityEvent onCaptureImage = this.OnCaptureImage;
		if (onCaptureImage == null)
		{
			return;
		}
		onCaptureImage.Invoke();
	}

	// Token: 0x06001C81 RID: 7297 RVA: 0x0009A8D4 File Offset: 0x00098AD4
	private void _print()
	{
		string fileName = this.saveName;
		if (this.appendDateToFile)
		{
			DateTime dateTime = DateTime.UtcNow;
			if (GorillaComputer.instance != null)
			{
				dateTime = GorillaComputer.instance.GetServerTime();
			}
			fileName += dateTime.ToString("yyyyMMddHHmmss");
		}
		Texture2D print = new Texture2D(this.renderTexture.width, this.renderTexture.height * this.rt.Count);
		for (int i = 0; i < this.rt.Count; i++)
		{
			Graphics.CopyTexture(this.rt[i], 0, 0, 0, 0, this.rt[i].width, this.rt[i].height, print, 0, 0, 0, this.rt[i].height * i);
		}
		NativeArray<Color32> narray = new NativeArray<Color32>(print.width * print.height, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		AsyncGPUReadback.RequestIntoNativeArray<Color32>(ref narray, print, 0, delegate(AsyncGPUReadbackRequest request)
		{
			if (!request.hasError)
			{
				if (this.overlay)
				{
					Color32[] pixels = this.overlay.GetPixels32();
					for (int j = 0; j < narray.Length; j++)
					{
						if (j < pixels.Length && pixels[j].a > 0)
						{
							narray[j] = pixels[j];
						}
					}
				}
				this.SaveImage(print, narray, fileName, this.imageDescription);
			}
			narray.Dispose();
			UnityEvent onSaveImage = this.OnSaveImage;
			if (onSaveImage == null)
			{
				return;
			}
			onSaveImage.Invoke();
		});
	}

	// Token: 0x06001C82 RID: 7298 RVA: 0x0009AA1A File Offset: 0x00098C1A
	public void Print()
	{
		if (this.saveImageToDevice)
		{
			this._print();
		}
	}

	// Token: 0x06001C83 RID: 7299 RVA: 0x0009AA2C File Offset: 0x00098C2C
	private void SaveImage(Texture rt, NativeArray<Color32> narray, string fileName, string desc)
	{
		NativeArray<byte> nativeArray = ImageConversion.EncodeNativeArrayToJPG<Color32>(narray, rt.graphicsFormat, (uint)rt.width, (uint)rt.height, 0U, 75);
		File.WriteAllBytes(Path.Combine(Application.persistentDataPath, fileName + ".jpg"), nativeArray.ToArray());
		nativeArray.Dispose();
	}

	// Token: 0x0400269B RID: 9883
	[SerializeField]
	private Camera cam;

	// Token: 0x0400269C RID: 9884
	[SerializeField]
	private RenderTexture renderTexture;

	// Token: 0x0400269D RID: 9885
	[SerializeField]
	private string saveName = "img";

	// Token: 0x0400269E RID: 9886
	[SerializeField]
	private bool appendDateToFile;

	// Token: 0x0400269F RID: 9887
	[SerializeField]
	private string imageDescription = "";

	// Token: 0x040026A0 RID: 9888
	[SerializeField]
	private Texture2D overlay;

	// Token: 0x040026A1 RID: 9889
	[SerializeField]
	private UnityEvent OnCaptureImage;

	// Token: 0x040026A2 RID: 9890
	[SerializeField]
	private UnityEvent OnSaveImage;

	// Token: 0x040026A3 RID: 9891
	private List<RenderTexture> rt = new List<RenderTexture>();

	// Token: 0x040026A4 RID: 9892
	public Action<Texture, int> OnCapture;

	// Token: 0x040026A5 RID: 9893
	private bool saveImageToDevice;
}
