using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GorillaNetworking;
using KID.Model;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

// Token: 0x020001AD RID: 429
public class VODPlayer : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x1700011D RID: 285
	// (get) Token: 0x06000BA1 RID: 2977 RVA: 0x0003E9A2 File Offset: 0x0003CBA2
	public static Material StandbyMaterial
	{
		get
		{
			return VODPlayer._standbyMaterial;
		}
	}

	// Token: 0x06000BA2 RID: 2978 RVA: 0x0003E9A9 File Offset: 0x0003CBA9
	private void Awake()
	{
		VODPlayer._standbyMaterial = this.standbyMaterial;
	}

	// Token: 0x06000BA3 RID: 2979 RVA: 0x0003E9B8 File Offset: 0x0003CBB8
	public async void OnEnable()
	{
		VODPlayer.state = VODPlayer.State.INITIALIZING;
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		VODTarget.AlertEnabled = (Action<VODTarget>)Delegate.Combine(VODTarget.AlertEnabled, new Action<VODTarget>(this.VODTarget_AlertEnabled));
		VODTarget.AlertDisabled = (Action<VODTarget>)Delegate.Combine(VODTarget.AlertDisabled, new Action<VODTarget>(this.VODTarget_AlertDisabled));
		PlayerPrefFlags.OnFlagChange = (Action<PlayerPrefFlags.Flag, bool>)Delegate.Combine(PlayerPrefFlags.OnFlagChange, new Action<PlayerPrefFlags.Flag, bool>(this.PlayerPreFlagChange));
		this.playerPrefMuted = PlayerPrefFlags.Check(PlayerPrefFlags.Flag.GTV_MUTED);
		if (this.voiceChatPermRequiredList == null)
		{
			this.voiceChatPermRequiredList = new List<VODPlayer.VODStream.VODStreamChannel>(this.voiceChatPermRequired);
		}
		if (this.player == null)
		{
			this.player = base.GetComponent<VideoPlayer>();
			this.player.loopPointReached += this.Player_loopPointReached;
			this.audioSource = base.GetComponentInChildren<AudioSource>();
			while (PlayFabTitleDataCache.Instance == null)
			{
				await Task.Yield();
			}
			this.tdGot = 0;
			for (int i = 0; i < this.titleDataKey.Length; i++)
			{
				PlayFabTitleDataCache.Instance.GetTitleData(this.titleDataKey[i], new Action<string>(this.onTD), new Action<PlayFabError>(this.onTDError), false);
			}
			this.waitOnServerTimeAndSchedule();
		}
	}

	// Token: 0x06000BA4 RID: 2980 RVA: 0x0003E9EF File Offset: 0x0003CBEF
	private void PlayerPreFlagChange(PlayerPrefFlags.Flag flag, bool v)
	{
		if (flag == PlayerPrefFlags.Flag.GTV_MUTED)
		{
			this.playerPrefMuted = v;
		}
	}

	// Token: 0x06000BA5 RID: 2981 RVA: 0x0003E9FC File Offset: 0x0003CBFC
	private async void waitOnServerTimeAndSchedule()
	{
		while (this.tdGot < this.titleDataKey.Length || GorillaComputer.instance == null || GorillaComputer.instance.GetServerTime().Year < 2000)
		{
			await Task.Yield();
		}
		if (this.schedule.hourly.Length == 0)
		{
			VODPlayer.state = VODPlayer.State.CRASHED;
			if (VODPlayer.OnCrash != null)
			{
				VODPlayer.OnCrash();
			}
			Debug.LogError("VOD :: CRASHED :: Nothing Scheduled");
			for (int i = 0; i < this.targets.Count; i++)
			{
				this.targets[i].ShowStatic(true);
			}
		}
		VODPlayer.state = VODPlayer.State.IDLE;
	}

	// Token: 0x06000BA6 RID: 2982 RVA: 0x0003EA33 File Offset: 0x0003CC33
	private Material getStandby(VODTarget o)
	{
		if (!(o.StandbyOverride == null))
		{
			return o.StandbyOverride;
		}
		return this.standbyMaterial;
	}

	// Token: 0x06000BA7 RID: 2983 RVA: 0x0003EA50 File Offset: 0x0003CC50
	private void VODTarget_AlertEnabled(VODTarget o)
	{
		if (!this.targets.Contains(o))
		{
			IEnumerable<VODPlayer.VODStream.VODStreamChannel> priorityChannelArray = this.getPriorityChannelArray();
			this.targets.Add(o);
			bool flag = !priorityChannelArray.SequenceEqual(this.getPriorityChannelArray());
			if (VODPlayer.state == VODPlayer.State.RUNNING && this.player.isPlaying && o.VerifyChannel(this.playerChannel))
			{
				o.Renderer.material = this.playBackMaterial;
				return;
			}
			o.Renderer.material = this.getStandby(o);
			o.SetNext(this.GetNextStream(o.Channel));
			if ((!this.player.isPlaying && this.targets.Count == 1) || flag)
			{
				this.PlayPreviouStream();
			}
		}
	}

	// Token: 0x06000BA8 RID: 2984 RVA: 0x0003EB10 File Offset: 0x0003CD10
	private void VODTarget_AlertDisabled(VODTarget o)
	{
		if (this.targets.Contains(o))
		{
			IEnumerable<VODPlayer.VODStream.VODStreamChannel> priorityChannelArray = this.getPriorityChannelArray();
			this.targets.Remove(o);
			bool flag = !priorityChannelArray.SequenceEqual(this.getPriorityChannelArray());
			o.Renderer.material = ((o.StandbyOverride == null) ? this.standbyMaterial : o.StandbyOverride);
			o.ClearNext();
			if (this.playerBusy)
			{
				return;
			}
			if (this.player.isPlaying && (this.targets.Count == 0 || flag))
			{
				this.player.Stop();
			}
			if (this.targets.Count > 0 && flag)
			{
				this.PlayPreviouStream();
			}
		}
	}

	// Token: 0x06000BA9 RID: 2985 RVA: 0x0003EBC8 File Offset: 0x0003CDC8
	private void Player_loopPointReached(VideoPlayer source)
	{
		if (!this.playerBusy)
		{
			this.player.Stop();
			for (int i = 0; i < this.targets.Count; i++)
			{
				this.targets[i].Renderer.material = this.getStandby(this.targets[i]);
				this.targets[i].SetNext(this.GetNextStream(this.targets[i].Channel));
			}
		}
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x0003EC50 File Offset: 0x0003CE50
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		this.player.loopPointReached -= this.Player_loopPointReached;
		VODTarget.AlertEnabled = (Action<VODTarget>)Delegate.Remove(VODTarget.AlertEnabled, new Action<VODTarget>(this.VODTarget_AlertEnabled));
		VODTarget.AlertDisabled = (Action<VODTarget>)Delegate.Remove(VODTarget.AlertDisabled, new Action<VODTarget>(this.VODTarget_AlertDisabled));
		PlayerPrefFlags.OnFlagChange = (Action<PlayerPrefFlags.Flag, bool>)Delegate.Remove(PlayerPrefFlags.OnFlagChange, new Action<PlayerPrefFlags.Flag, bool>(this.PlayerPreFlagChange));
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x0003ECDC File Offset: 0x0003CEDC
	private void OnDestroy()
	{
		VODTarget.AlertEnabled = (Action<VODTarget>)Delegate.Remove(VODTarget.AlertEnabled, new Action<VODTarget>(this.VODTarget_AlertEnabled));
		VODTarget.AlertDisabled = (Action<VODTarget>)Delegate.Remove(VODTarget.AlertDisabled, new Action<VODTarget>(this.VODTarget_AlertDisabled));
		PlayerPrefFlags.OnFlagChange = (Action<PlayerPrefFlags.Flag, bool>)Delegate.Remove(PlayerPrefFlags.OnFlagChange, new Action<PlayerPrefFlags.Flag, bool>(this.PlayerPreFlagChange));
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x0003ED4C File Offset: 0x0003CF4C
	void IGorillaSliceableSimple.SliceUpdate()
	{
		switch (VODPlayer.state)
		{
		case VODPlayer.State.INITIALIZING:
		case VODPlayer.State.CRASHED:
			return;
		case VODPlayer.State.IDLE:
			if (this.targets.Count > 0)
			{
				VODPlayer.state = VODPlayer.State.RUNNING;
			}
			return;
		case VODPlayer.State.RUNNING:
		{
			if (this.targets.Count == 0)
			{
				if (!this.playerBusy)
				{
					this.player.Stop();
				}
				VODPlayer.state = VODPlayer.State.IDLE;
				return;
			}
			if (this.player.isPlaying)
			{
				this.PositionAudio();
			}
			DateTime serverTime = GorillaComputer.instance.GetServerTime();
			DayOfWeek dayOfWeek = serverTime.DayOfWeek;
			int hour = serverTime.Hour;
			int minute = serverTime.Minute;
			if (minute == this.lastCheck)
			{
				return;
			}
			this.lastCheck = minute;
			List<VODPlayer.VODStream> list = new List<VODPlayer.VODStream>();
			for (int i = 0; i < this.schedule.hourly.Length; i++)
			{
				if (this.schedule.hourly[i].minute - minute == 0 && this.schedule.hourly[i].IsDateInRange(serverTime))
				{
					list.Add(this.schedule.hourly[i].stream);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			if (list.Count == 1)
			{
				this.StartPlayback(list[0], 1.0);
				return;
			}
			List<VODPlayer.VODStream.VODStreamChannel> priorityChannels = this.getPriorityChannels();
			for (int j = 0; j < list.Count; j++)
			{
				if (priorityChannels.Contains(list[j].ch))
				{
					this.StartPlayback(list[j], 1.0);
					return;
				}
			}
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x0003EEE4 File Offset: 0x0003D0E4
	private List<VODPlayer.VODStream.VODStreamChannel> getPriorityChannels()
	{
		return new List<VODPlayer.VODStream.VODStreamChannel>(this.getPriorityChannelArray());
	}

	// Token: 0x06000BAE RID: 2990 RVA: 0x0003EEF4 File Offset: 0x0003D0F4
	private VODPlayer.VODStream.VODStreamChannel[] getPriorityChannelArray()
	{
		float num = float.MaxValue;
		VODTarget vodtarget = null;
		for (int i = 0; i < this.targets.Count; i++)
		{
			if (this.targets[i].Distance < num)
			{
				num = this.targets[i].Distance;
				vodtarget = this.targets[i];
			}
		}
		if (vodtarget != null)
		{
			return vodtarget.Channel;
		}
		return new VODPlayer.VODStream.VODStreamChannel[0];
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x0003EF68 File Offset: 0x0003D168
	private bool CheckForCachedFile(string fileId, string extension, out string filePath)
	{
		string text = string.Format("V{0:X}.{1}", fileId.GetHashCode(), extension);
		filePath = Path.Combine(Application.persistentDataPath, text);
		if (File.Exists(filePath))
		{
			return true;
		}
		string text2 = Path.Combine(Application.persistentDataPath, "GTv_Cache");
		if (!Directory.Exists(text2))
		{
			Directory.CreateDirectory(text2);
		}
		filePath = Path.Combine(text2, text);
		return File.Exists(filePath);
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x0003EFD4 File Offset: 0x0003D1D4
	private async Task<string> GetCachedFile(string url, string fileId, string extension)
	{
		string filePath;
		string text;
		if (this.CheckForCachedFile(fileId, extension, out filePath))
		{
			text = filePath;
		}
		else
		{
			UnityWebRequest www = new UnityWebRequest(url);
			www.downloadHandler = new DownloadHandlerBuffer();
			await www.SendWebRequest();
			if (www.result != UnityWebRequest.Result.Success)
			{
				Debug.LogError("VOD :: error :: " + www.error);
				text = null;
			}
			else
			{
				File.WriteAllBytes(filePath, www.downloadHandler.data);
				this.cache.Add(filePath);
				PlayerPrefs.SetString("_VODCache_", JsonConvert.SerializeObject(this.cache));
				text = filePath;
			}
		}
		return text;
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x0003F030 File Offset: 0x0003D230
	private void Start()
	{
		this.cache = new List<string>();
		string @string = PlayerPrefs.GetString("_VODCache_");
		if (@string.IsNullOrEmpty())
		{
			return;
		}
		List<string> list = JsonConvert.DeserializeObject<List<string>>(@string);
		for (int i = 0; i < list.Count; i++)
		{
			if (File.Exists(list[i]))
			{
				if ((DateTime.Now - File.GetCreationTime(list[i])).TotalDays > 30.0)
				{
					File.Delete(list[i]);
				}
				else
				{
					this.cache.Add(list[i]);
				}
			}
		}
		PlayerPrefs.SetString("_VODCache_", JsonConvert.SerializeObject(this.cache));
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x0003F0E0 File Offset: 0x0003D2E0
	private void PositionAudio()
	{
		float num = float.MaxValue;
		VODTarget vodtarget = null;
		for (int i = 0; i < this.targets.Count; i++)
		{
			if ((!this.playerPrefMuted || this.targets[i].Unmutable) && this.targets[i].AudioSettings.volume > 0f && this.targets[i].VerifyChannel(this.playerChannel))
			{
				float sqrMagnitude = (VRRig.LocalRig.transform.position - this.targets[i].transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					vodtarget = this.targets[i];
					num = sqrMagnitude;
				}
			}
		}
		if (vodtarget == null)
		{
			this.audioSource.volume = 0f;
			return;
		}
		this.audioSource.transform.position = vodtarget.transform.position;
		if (this.voiceChatPerm == null)
		{
			this.voiceChatPerm = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
		}
		this.audioSource.volume = ((!this.voiceChatPermRequiredList.Contains(this.playerChannel) || this.voiceChatPerm.Enabled) ? vodtarget.AudioSettings.volume : 0f);
		this.audioSource.dopplerLevel = vodtarget.AudioSettings.dopplerLevel;
		this.audioSource.rolloffMode = vodtarget.AudioSettings.rolloffMode;
		this.audioSource.minDistance = vodtarget.AudioSettings.minDistance;
		this.audioSource.maxDistance = vodtarget.AudioSettings.maxDistance;
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x0003F288 File Offset: 0x0003D488
	private void PlayPreviouStream()
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		int hour = serverTime.Hour;
		int minute = serverTime.Minute;
		DateTime dateTime = new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, hour, minute, 0);
		int num = -1;
		List<VODPlayer.VODStream.VODStreamChannel> priorityChannels = this.getPriorityChannels();
		for (int i = 0; i < this.schedule.hourly.Length; i++)
		{
			if (priorityChannels.Contains(this.schedule.hourly[i].stream.ch) && this.schedule.hourly[i].minute <= minute && this.schedule.hourly[i].IsDateInRange(serverTime))
			{
				num = i;
			}
		}
		if (num >= 0)
		{
			int num2 = minute - this.schedule.hourly[num].minute;
			this.StartPlayback(this.schedule.hourly[num].stream, serverTime.Subtract(dateTime.AddMinutes((double)(-(double)num2))).TotalSeconds);
		}
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x0003F3B0 File Offset: 0x0003D5B0
	private void StartPlayback(VODPlayer.VODStream str, double time = 0.0)
	{
		VODPlayer.VODStream.VODStreamType type = str.type;
		if (type != VODPlayer.VODStream.VODStreamType.VIDEO)
		{
			if (type != VODPlayer.VODStream.VODStreamType.IMAGE)
			{
				return;
			}
			if (str.id.IsNullOrEmpty())
			{
				this.StartImagePlayback(str.url, str.url, str.duration, str.ch, time, null);
				return;
			}
			string text;
			if (this.CheckForCachedFile(str.id, "mp4", out text))
			{
				this.StartImagePlayback(text, str.id, str.duration, str.ch, time, text);
				return;
			}
			MothershipClientApiUnity.GetDLCFileDetails(str.id, delegate(SharedDownloadableFileResult result)
			{
				this.StartImagePlayback(result.url, str.id, str.duration, str.ch, time, null);
			}, delegate(MothershipError error, int status)
			{
				Debug.Log(string.Concat(new string[] { "VOD: FILE FETCH FAILED Mothership Error: ", error.MothershipErrorCode, " Trace ID: ", error.TraceId, " Message: ", error.Message }));
				if (!str.url.IsNullOrEmpty())
				{
					this.StartImagePlayback(str.url, str.url, str.duration, str.ch, time, null);
				}
			});
			return;
		}
		else
		{
			if (str.id.IsNullOrEmpty())
			{
				this.StartVideoPlayback(str.url, str.url, str.ch, time, null);
				return;
			}
			string text2;
			if (this.CheckForCachedFile(str.id, "mp4", out text2))
			{
				this.StartVideoPlayback(text2, str.id, str.ch, time, text2);
				return;
			}
			MothershipClientApiUnity.GetDLCFileDetails(str.id, delegate(SharedDownloadableFileResult result)
			{
				this.StartVideoPlayback(result.url, str.id, str.ch, time, null);
			}, delegate(MothershipError error, int status)
			{
				Debug.Log(string.Concat(new string[] { "VOD: FILE FETCH FAILED Mothership Error: ", error.MothershipErrorCode, " Trace ID: ", error.TraceId, " Message: ", error.Message }));
				if (!str.url.IsNullOrEmpty())
				{
					this.StartVideoPlayback(str.url, str.url, str.ch, time, null);
				}
			});
			return;
		}
	}

	// Token: 0x06000BB5 RID: 2997 RVA: 0x0003F558 File Offset: 0x0003D758
	private async void StartImagePlayback(string url, string fileId, int duration, VODPlayer.VODStream.VODStreamChannel ch, double time, string cachedUrl)
	{
		duration -= (int)time;
		if (duration > 0)
		{
			List<VODTarget> imageTargets = new List<VODTarget>();
			for (int i = 0; i < this.targets.Count; i++)
			{
				if (this.targets[i].VerifyChannel(ch))
				{
					imageTargets.Add(this.targets[i]);
					this.targets[i].Renderer.material = this.busyMaterial;
					this.targets[i].ClearNext();
				}
			}
			if (cachedUrl == null)
			{
				string text = await this.GetCachedFile(url, fileId, "png");
				cachedUrl = text;
			}
			if (cachedUrl == null)
			{
				Debug.LogError("VOD :: cache error :: " + url);
				for (int j = 0; j < imageTargets.Count; j++)
				{
					imageTargets[j].Renderer.material = this.getStandby(imageTargets[j]);
				}
			}
			else
			{
				UnityWebRequest www = new UnityWebRequest(cachedUrl);
				DownloadHandlerTexture downloadHandlerTexture = new DownloadHandlerTexture();
				www.downloadHandler = downloadHandlerTexture;
				await www.SendWebRequest();
				if (www.result != UnityWebRequest.Result.Success)
				{
					Debug.LogError("VOD :: error :: " + www.error + " :: " + downloadHandlerTexture.error);
					for (int k = 0; k < imageTargets.Count; k++)
					{
						imageTargets[k].Renderer.material = this.getStandby(imageTargets[k]);
					}
				}
				else
				{
					this.imageMaterial.mainTexture = downloadHandlerTexture.texture;
					for (int l = 0; l < imageTargets.Count; l++)
					{
						imageTargets[l].Renderer.material = this.imageMaterial;
					}
					await Task.Delay(duration * 1000);
					for (int m = 0; m < imageTargets.Count; m++)
					{
						if (imageTargets[m].Renderer.material == this.imageMaterial)
						{
							imageTargets[m].Renderer.material = this.getStandby(imageTargets[m]);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x0003F5C4 File Offset: 0x0003D7C4
	private async void StartVideoPlayback(string url, string fileId, VODPlayer.VODStream.VODStreamChannel ch, double time, string cachedUrl)
	{
		if (!this.playerBusy)
		{
			this.playerBusy = true;
			if (this.player.isPlaying)
			{
				if (!this.getPriorityChannels().Contains(ch))
				{
					this.playerBusy = false;
					return;
				}
				this.player.Stop();
			}
			for (int i = 0; i < this.targets.Count; i++)
			{
				if (this.targets[i].VerifyChannel(ch))
				{
					this.targets[i].Renderer.material = this.busyMaterial;
					this.targets[i].ClearNext();
				}
			}
			try
			{
				if (cachedUrl == null)
				{
					string text = await this.GetCachedFile(url, fileId, "mp4");
					cachedUrl = text;
				}
				if (cachedUrl == null)
				{
					Debug.LogError("VOD :: cache error :: " + url);
					this.playerBusy = false;
					return;
				}
				this.player.url = cachedUrl;
				this.player.Prepare();
				while (!this.player.isPrepared && Application.isPlaying)
				{
					await Task.Yield();
				}
				if (time >= this.player.length || VODPlayer.state != VODPlayer.State.RUNNING)
				{
					this.playerBusy = false;
					for (int j = 0; j < this.targets.Count; j++)
					{
						this.targets[j].Renderer.material = this.getStandby(this.targets[j]);
					}
					return;
				}
				if (time > 0.0)
				{
					this.player.time = time;
				}
				this.player.Play();
				this.playerChannel = ch;
				this.PositionAudio();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			for (int k = 0; k < this.targets.Count; k++)
			{
				if (this.targets[k].VerifyChannel(ch))
				{
					this.targets[k].Renderer.material = this.playBackMaterial;
				}
			}
			this.playerBusy = false;
		}
	}

	// Token: 0x06000BB7 RID: 2999 RVA: 0x0003F628 File Offset: 0x0003D828
	private void onTD(string s)
	{
		this.tdGot++;
		if (s.IsNullOrEmpty())
		{
			Debug.LogError("Crash(\"No schedule data\")");
			return;
		}
		VODPlayer.VODStreamSchedule vodstreamSchedule = default(VODPlayer.VODStreamSchedule);
		try
		{
			vodstreamSchedule = JsonConvert.DeserializeObject<VODPlayer.VODStreamSchedule>(s);
		}
		catch (Exception ex)
		{
			Debug.LogError("Crash(\"Malformed schedule data\") :: " + ex.Message + " :: " + s);
		}
		for (int i = 0; i < vodstreamSchedule.hourly.Length; i++)
		{
			vodstreamSchedule.hourly[i].ValidateDate();
		}
		this.schedule.Merge(vodstreamSchedule);
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x0003F6C8 File Offset: 0x0003D8C8
	private void onTDError(PlayFabError error)
	{
		this.tdGot++;
		Debug.LogError("TD Error: " + error.ErrorMessage);
	}

	// Token: 0x06000BB9 RID: 3001 RVA: 0x0003F6F0 File Offset: 0x0003D8F0
	private VODPlayer.VODNextStreamData GetNextStream(VODPlayer.VODStream.VODStreamChannel[] ch)
	{
		return this.GetNextStream(ch, (GorillaComputer.instance == null || GorillaComputer.instance.GetServerTime().Year < 2000) ? DateTime.UtcNow : GorillaComputer.instance.GetServerTime());
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x0003F744 File Offset: 0x0003D944
	private VODPlayer.VODNextStreamData GetNextStream(VODPlayer.VODStream.VODStreamChannel[] ch, DateTime now)
	{
		List<VODPlayer.VODStream.VODStreamChannel> list = new List<VODPlayer.VODStream.VODStreamChannel>(ch);
		for (int i = 0; i < this.schedule.hourly.Length; i++)
		{
			if (!this.schedule.hourly[i].stream.hideUpNext && list.Contains(this.schedule.hourly[i].stream.ch) && this.schedule.hourly[i].minute > now.Minute)
			{
				DateTime dateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, this.schedule.hourly[i].minute, 0);
				return new VODPlayer.VODNextStreamData(this.schedule.hourly[i].stream.name, dateTime);
			}
		}
		for (int j = 0; j < this.schedule.hourly.Length; j++)
		{
			if (!this.schedule.hourly[j].stream.hideUpNext && list.Contains(this.schedule.hourly[j].stream.ch) && this.schedule.hourly[j].minute < now.Minute)
			{
				DateTime dateTime2 = new DateTime(now.Year, now.Month, now.Day, now.Hour, this.schedule.hourly[j].minute, 0).AddHours(1.0);
				return new VODPlayer.VODNextStreamData(this.schedule.hourly[j].stream.name, dateTime2);
			}
		}
		return new VODPlayer.VODNextStreamData(string.Empty, DateTime.MinValue);
	}

	// Token: 0x06000BBB RID: 3003 RVA: 0x0003F938 File Offset: 0x0003DB38
	public static string[] GetSchedule(VODPlayer.VODStreamSchedule schedule, VODPlayer.VODStream.VODStreamChannel[] ch)
	{
		return VODPlayer.GetSchedule(schedule, ch, (GorillaComputer.instance == null || GorillaComputer.instance.GetServerTime().Year < 2000) ? DateTime.UtcNow : GorillaComputer.instance.GetServerTime());
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x0003F98C File Offset: 0x0003DB8C
	public static string[] GetSchedule(VODPlayer.VODStreamSchedule schedule, VODPlayer.VODStream.VODStreamChannel[] ch, DateTime now)
	{
		List<string> list = new List<string>();
		List<VODPlayer.VODStream.VODStreamChannel> list2 = new List<VODPlayer.VODStream.VODStreamChannel>(ch);
		for (int i = 0; i < schedule.hourly.Length; i++)
		{
			if (!schedule.hourly[i].stream.hideUpNext && list2.Contains(schedule.hourly[i].stream.ch) && schedule.hourly[i].minute > now.Minute)
			{
				DateTime dateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, schedule.hourly[i].minute, 0);
				list.Add(dateTime.ToShortTimeString() + " --- " + schedule.hourly[i].stream.name);
			}
		}
		for (int j = 0; j < schedule.hourly.Length; j++)
		{
			if (!schedule.hourly[j].stream.hideUpNext && list2.Contains(schedule.hourly[j].stream.ch) && schedule.hourly[j].minute < now.Minute)
			{
				list.Add(new DateTime(now.Year, now.Month, now.Day, now.Hour, schedule.hourly[j].minute, 0).AddHours(1.0).ToShortTimeString() + " --- " + schedule.hourly[j].stream.name);
			}
		}
		return list.ToArray();
	}

	// Token: 0x06000BBD RID: 3005 RVA: 0x0003FB68 File Offset: 0x0003DD68
	public static Dictionary<VODPlayer.VODStream.VODStreamChannel, List<string>> GetSchedule(VODPlayer.VODStreamSchedule schedule)
	{
		return VODPlayer.GetSchedule(schedule, (GorillaComputer.instance == null || GorillaComputer.instance.GetServerTime().Year < 2000) ? DateTime.UtcNow : GorillaComputer.instance.GetServerTime());
	}

	// Token: 0x06000BBE RID: 3006 RVA: 0x0003FBB8 File Offset: 0x0003DDB8
	public static Dictionary<VODPlayer.VODStream.VODStreamChannel, List<string>> GetSchedule(VODPlayer.VODStreamSchedule schedule, DateTime now)
	{
		Dictionary<VODPlayer.VODStream.VODStreamChannel, List<string>> dictionary = new Dictionary<VODPlayer.VODStream.VODStreamChannel, List<string>>();
		for (int i = 0; i < schedule.hourly.Length; i++)
		{
			if (!schedule.hourly[i].stream.hideUpNext && schedule.hourly[i].minute > now.Minute)
			{
				DateTime dateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, schedule.hourly[i].minute, 0);
				if (!dictionary.ContainsKey(schedule.hourly[i].stream.ch))
				{
					dictionary.Add(schedule.hourly[i].stream.ch, new List<string>());
				}
				dictionary[schedule.hourly[i].stream.ch].Add(dateTime.ToShortTimeString() + " --- " + schedule.hourly[i].stream.name);
			}
		}
		for (int j = 0; j < schedule.hourly.Length; j++)
		{
			if (!schedule.hourly[j].stream.hideUpNext && schedule.hourly[j].minute < now.Minute)
			{
				DateTime dateTime2 = new DateTime(now.Year, now.Month, now.Day, now.Hour, schedule.hourly[j].minute, 0).AddHours(1.0);
				if (!dictionary.ContainsKey(schedule.hourly[j].stream.ch))
				{
					dictionary.Add(schedule.hourly[j].stream.ch, new List<string>());
				}
				dictionary[schedule.hourly[j].stream.ch].Add(dateTime2.ToShortTimeString() + " --- " + schedule.hourly[j].stream.name);
			}
		}
		return dictionary;
	}

	// Token: 0x04000E08 RID: 3592
	private static Material _standbyMaterial;

	// Token: 0x04000E09 RID: 3593
	private const string PlayerPrefKey_Cache = "_VODCache_";

	// Token: 0x04000E0A RID: 3594
	public static Action OnCrash;

	// Token: 0x04000E0B RID: 3595
	public static VODPlayer.State state;

	// Token: 0x04000E0C RID: 3596
	private VideoPlayer player;

	// Token: 0x04000E0D RID: 3597
	private AudioSource audioSource;

	// Token: 0x04000E0E RID: 3598
	private VODPlayer.VODStreamSchedule schedule;

	// Token: 0x04000E0F RID: 3599
	[SerializeField]
	private string[] titleDataKey;

	// Token: 0x04000E10 RID: 3600
	[SerializeField]
	private Material standbyMaterial;

	// Token: 0x04000E11 RID: 3601
	[SerializeField]
	private Material playBackMaterial;

	// Token: 0x04000E12 RID: 3602
	[SerializeField]
	private Material busyMaterial;

	// Token: 0x04000E13 RID: 3603
	[SerializeField]
	private Material imageMaterial;

	// Token: 0x04000E14 RID: 3604
	[SerializeField]
	private VODPlayer.VODStream.VODStreamChannel[] voiceChatPermRequired;

	// Token: 0x04000E15 RID: 3605
	private List<VODTarget> targets = new List<VODTarget>();

	// Token: 0x04000E16 RID: 3606
	private List<VODPlayer.VODStream.VODStreamChannel> voiceChatPermRequiredList;

	// Token: 0x04000E17 RID: 3607
	private int lastCheck;

	// Token: 0x04000E18 RID: 3608
	private List<string> cache = new List<string>();

	// Token: 0x04000E19 RID: 3609
	private bool playerBusy;

	// Token: 0x04000E1A RID: 3610
	private VODPlayer.VODStream.VODStreamChannel playerChannel;

	// Token: 0x04000E1B RID: 3611
	private int tdGot;

	// Token: 0x04000E1C RID: 3612
	private Permission voiceChatPerm;

	// Token: 0x04000E1D RID: 3613
	private bool playerPrefMuted;

	// Token: 0x020001AE RID: 430
	public enum State
	{
		// Token: 0x04000E1F RID: 3615
		INITIALIZING,
		// Token: 0x04000E20 RID: 3616
		IDLE,
		// Token: 0x04000E21 RID: 3617
		RUNNING,
		// Token: 0x04000E22 RID: 3618
		CRASHED
	}

	// Token: 0x020001AF RID: 431
	public struct VODNextStreamData
	{
		// Token: 0x06000BC0 RID: 3008 RVA: 0x0003FE16 File Offset: 0x0003E016
		public VODNextStreamData(string title, DateTime startTime)
		{
			this.Title = title;
			this.StartTime = startTime;
		}

		// Token: 0x04000E23 RID: 3619
		public string Title;

		// Token: 0x04000E24 RID: 3620
		public DateTime StartTime;
	}

	// Token: 0x020001B0 RID: 432
	[Serializable]
	public struct VODStreamSchedule
	{
		// Token: 0x06000BC1 RID: 3009 RVA: 0x0003FE28 File Offset: 0x0003E028
		internal void Merge(VODPlayer.VODStreamSchedule subSchedule)
		{
			List<VODPlayer.VODHourlyStream> list = new List<VODPlayer.VODHourlyStream>();
			if (this.hourly != null)
			{
				list.AddRange(this.hourly);
			}
			for (int i = 0; i < subSchedule.hourly.Length; i++)
			{
				list.Add(subSchedule.hourly[i]);
				int num = 0;
				while (subSchedule.hourly[i].repeats != null && num < subSchedule.hourly[i].repeats.Length)
				{
					VODPlayer.VODHourlyStream vodhourlyStream = default(VODPlayer.VODHourlyStream);
					vodhourlyStream.stream = subSchedule.hourly[i].stream;
					vodhourlyStream.minute = subSchedule.hourly[i].repeats[num];
					vodhourlyStream.startDateTime = subSchedule.hourly[i].startDateTime;
					vodhourlyStream.endDateTime = subSchedule.hourly[i].endDateTime;
					vodhourlyStream.ValidateDate();
					list.Add(vodhourlyStream);
					num++;
				}
			}
			list.Sort();
			this.hourly = list.ToArray();
		}

		// Token: 0x04000E25 RID: 3621
		public VODPlayer.VODHourlyStream[] hourly;
	}

	// Token: 0x020001B1 RID: 433
	[Serializable]
	public struct VODStream
	{
		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000BC2 RID: 3010 RVA: 0x0003FF38 File Offset: 0x0003E138
		public string displayTitle
		{
			get
			{
				if (!this.hideUpNext)
				{
					return this.name;
				}
				return string.Empty;
			}
		}

		// Token: 0x04000E26 RID: 3622
		public string name;

		// Token: 0x04000E27 RID: 3623
		public bool hideUpNext;

		// Token: 0x04000E28 RID: 3624
		public string id;

		// Token: 0x04000E29 RID: 3625
		[Obsolete]
		public string url;

		// Token: 0x04000E2A RID: 3626
		public VODPlayer.VODStream.VODStreamType type;

		// Token: 0x04000E2B RID: 3627
		public int duration;

		// Token: 0x04000E2C RID: 3628
		public VODPlayer.VODStream.VODStreamChannel ch;

		// Token: 0x020001B2 RID: 434
		public enum VODStreamType
		{
			// Token: 0x04000E2E RID: 3630
			VIDEO,
			// Token: 0x04000E2F RID: 3631
			IMAGE
		}

		// Token: 0x020001B3 RID: 435
		public enum VODStreamChannel
		{
			// Token: 0x04000E31 RID: 3633
			DEFAULT,
			// Token: 0x04000E32 RID: 3634
			VIM,
			// Token: 0x04000E33 RID: 3635
			MM,
			// Token: 0x04000E34 RID: 3636
			GCORP,
			// Token: 0x04000E35 RID: 3637
			EVENT,
			// Token: 0x04000E36 RID: 3638
			FEATURED
		}
	}

	// Token: 0x020001B4 RID: 436
	[Serializable]
	public struct VODHourlyStream : IComparable<VODPlayer.VODHourlyStream>
	{
		// Token: 0x06000BC3 RID: 3011 RVA: 0x0003FF4E File Offset: 0x0003E14E
		public int CompareTo(VODPlayer.VODHourlyStream other)
		{
			return this.minute - other.minute;
		}

		// Token: 0x06000BC4 RID: 3012 RVA: 0x0003FF60 File Offset: 0x0003E160
		public void ValidateDate()
		{
			try
			{
				this.startDT = DateTime.Parse(this.startDateTime);
			}
			catch
			{
				this.startDT = DateTime.Parse("1/1/0001");
			}
			try
			{
				this.endDT = DateTime.Parse(this.endDateTime);
			}
			catch
			{
				this.endDT = DateTime.Parse("1/1/3001");
			}
			this.startDateTime = this.startDT.ToString();
			this.endDateTime = this.endDT.ToString();
		}

		// Token: 0x06000BC5 RID: 3013 RVA: 0x0003FFF8 File Offset: 0x0003E1F8
		internal bool IsDateInRange(DateTime serverTime)
		{
			return serverTime >= this.startDT && serverTime <= this.endDT;
		}

		// Token: 0x06000BC6 RID: 3014 RVA: 0x00040016 File Offset: 0x0003E216
		internal DateTime ClampedDateTime(DateTime dateTime)
		{
			if (dateTime < this.startDT)
			{
				return this.startDT;
			}
			if (dateTime > this.endDT)
			{
				return this.endDT;
			}
			return dateTime;
		}

		// Token: 0x04000E37 RID: 3639
		public VODPlayer.VODStream stream;

		// Token: 0x04000E38 RID: 3640
		[Range(0f, 59f)]
		public int minute;

		// Token: 0x04000E39 RID: 3641
		[Range(0f, 59f)]
		public int[] repeats;

		// Token: 0x04000E3A RID: 3642
		public string startDateTime;

		// Token: 0x04000E3B RID: 3643
		private DateTime startDT;

		// Token: 0x04000E3C RID: 3644
		public string endDateTime;

		// Token: 0x04000E3D RID: 3645
		private DateTime endDT;
	}
}
