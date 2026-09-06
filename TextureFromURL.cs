using System;
using System.Threading.Tasks;
using GorillaNetworking;
using PlayFab;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000E43 RID: 3651
public class TextureFromURL : MonoBehaviour
{
	// Token: 0x06005934 RID: 22836 RVA: 0x001CFA72 File Offset: 0x001CDC72
	private void OnEnable()
	{
		if (this.data.Length == 0)
		{
			return;
		}
		if (this.source == TextureFromURL.Source.TitleData)
		{
			this.LoadFromTitleData();
			return;
		}
		this.applyRemoteTexture(this.data);
	}

	// Token: 0x06005935 RID: 22837 RVA: 0x001CFAA0 File Offset: 0x001CDCA0
	private async void LoadFromTitleData()
	{
		int attempt = 0;
		while (attempt < this.maxTitleDataAttempts && PlayFabTitleDataCache.Instance == null)
		{
			await Task.Delay(1000);
			attempt++;
		}
		if (PlayFabTitleDataCache.Instance != null)
		{
			PlayFabTitleDataCache.Instance.GetTitleData(this.data, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError), false);
		}
	}

	// Token: 0x06005936 RID: 22838 RVA: 0x001CFAD7 File Offset: 0x001CDCD7
	private void OnDisable()
	{
		if (this.texture != null)
		{
			Object.Destroy(this.texture);
			this.texture = null;
		}
	}

	// Token: 0x06005937 RID: 22839 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnPlayFabError(PlayFabError error)
	{
	}

	// Token: 0x06005938 RID: 22840 RVA: 0x001CFAFC File Offset: 0x001CDCFC
	private void OnTitleDataRequestComplete(string imageUrl)
	{
		imageUrl = imageUrl.Replace("\\r", "\r").Replace("\\n", "\n");
		if (imageUrl[0] == '"' && imageUrl[imageUrl.Length - 1] == '"')
		{
			imageUrl = imageUrl.Substring(1, imageUrl.Length - 2);
		}
		this.applyRemoteTexture(imageUrl);
	}

	// Token: 0x06005939 RID: 22841 RVA: 0x001CFB60 File Offset: 0x001CDD60
	private async void applyRemoteTexture(string imageUrl)
	{
		Texture2D texture2D = await this.GetRemoteTexture(imageUrl);
		this.texture = texture2D;
		if (this.texture != null)
		{
			this._renderer.material.mainTexture = this.texture;
		}
	}

	// Token: 0x0600593A RID: 22842 RVA: 0x001CFBA0 File Offset: 0x001CDDA0
	private async Task<Texture2D> GetRemoteTexture(string url)
	{
		Texture2D texture2D;
		using (UnityWebRequest wr = UnityWebRequestTexture.GetTexture(url))
		{
			UnityWebRequestAsyncOperation asyncOp = wr.SendWebRequest();
			while (!asyncOp.isDone)
			{
				await Task.Delay(1000);
			}
			if (wr.result == UnityWebRequest.Result.Success)
			{
				texture2D = DownloadHandlerTexture.GetContent(wr);
			}
			else
			{
				texture2D = null;
			}
		}
		return texture2D;
	}

	// Token: 0x04006967 RID: 26983
	[SerializeField]
	private Renderer _renderer;

	// Token: 0x04006968 RID: 26984
	[SerializeField]
	private TextureFromURL.Source source;

	// Token: 0x04006969 RID: 26985
	[Tooltip("If Source is set to 'TitleData' Data should be the id of the title data entry that defines an image URL. If Source is set to 'URL' Data should be a URL that points to an image.")]
	[SerializeField]
	private string data;

	// Token: 0x0400696A RID: 26986
	private Texture2D texture;

	// Token: 0x0400696B RID: 26987
	private int maxTitleDataAttempts = 10;

	// Token: 0x02000E44 RID: 3652
	private enum Source
	{
		// Token: 0x0400696D RID: 26989
		TitleData,
		// Token: 0x0400696E RID: 26990
		URL
	}
}
