using System;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x020009F9 RID: 2553
public class SynchedMusicController : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06004189 RID: 16777 RVA: 0x0015C70C File Offset: 0x0015A90C
	private void Start()
	{
		if (this.usingNewSyncedSongsCode)
		{
			this.New_Start();
			return;
		}
		this.totalLoopTime = 0L;
		AudioSource[] array = this.audioSourceArray;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].mute = PlayerPrefs.GetInt(this.locationName + "Muted", 0) != 0;
		}
		this.audioSource.mute = PlayerPrefs.GetInt(this.locationName + "Muted", 0) != 0;
		this.muteButton.isOn = this.audioSource.mute;
		this.muteButton.UpdateColor();
		for (int j = 0; j < this.muteButtons.Length; j++)
		{
			this.muteButtons[j].isOn = this.audioSource.mute;
			this.muteButtons[j].UpdateColor();
		}
		this.randomNumberGenerator = new Random(this.mySeed);
		this.GenerateSongStartRandomTimes();
		if (this.twoLayer)
		{
			array = this.audioSourceArray;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].clip.LoadAudioData();
			}
		}
	}

	// Token: 0x0600418A RID: 16778 RVA: 0x0015C828 File Offset: 0x0015AA28
	public void SliceUpdate()
	{
		if (this.usingNewSyncedSongsCode)
		{
			this.New_Update();
			return;
		}
		if (GorillaComputer.instance.startupMillis == 0L || this.totalLoopTime == 0L || this.songStartTimes.Length == 0)
		{
			return;
		}
		this.isPlayingCurrently = this.audioSource.isPlaying;
		if (this.testPlay)
		{
			this.testPlay = false;
			if (this.usingMultipleSources && this.usingMultipleSongs)
			{
				this.audioSource = this.audioSourceArray[Random.Range(0, this.audioSourceArray.Length)];
				this.audioSource.clip = this.songsArray[Random.Range(0, this.songsArray.Length)];
				this.audioSource.time = 0f;
			}
			if (this.twoLayer)
			{
				this.StartPlayingSongs(0L, 0L);
			}
			else if (this.audioSource.volume != 0f)
			{
				this.audioSource.GTPlay();
			}
		}
		if (GorillaComputer.instance == null)
		{
			return;
		}
		this.currentTime = (GorillaComputer.instance.startupMillis + (long)(Time.realtimeSinceStartup * 1000f)) % this.totalLoopTime;
		if (!this.audioSource.isPlaying)
		{
			if (this.lastPlayIndex >= 0 && this.songStartTimes[this.lastPlayIndex % this.songStartTimes.Length] < this.currentTime && this.currentTime < this.songStartTimes[(this.lastPlayIndex + 1) % this.songStartTimes.Length])
			{
				if (this.twoLayer)
				{
					if (this.songStartTimes[this.lastPlayIndex] + (long)(this.audioSource.clip.length * 1000f) > this.currentTime)
					{
						this.StartPlayingSongs(this.songStartTimes[this.lastPlayIndex], this.currentTime);
						return;
					}
				}
				else if (this.usingMultipleSongs && this.usingMultipleSources)
				{
					if (this.songStartTimes[this.lastPlayIndex] + (long)(this.songsArray[this.audioClipsForPlaying[this.lastPlayIndex]].length * 1000f) > this.currentTime)
					{
						this.StartPlayingSong(this.songStartTimes[this.lastPlayIndex], this.currentTime, this.songsArray[this.audioClipsForPlaying[this.lastPlayIndex]], this.audioSourceArray[this.audioSourcesForPlaying[this.lastPlayIndex]]);
						return;
					}
				}
				else if (this.songStartTimes[this.lastPlayIndex] + (long)(this.audioSource.clip.length * 1000f) > this.currentTime)
				{
					this.StartPlayingSong(this.songStartTimes[this.lastPlayIndex], this.currentTime);
					return;
				}
			}
			else
			{
				for (int i = 0; i < this.songStartTimes.Length; i++)
				{
					if (this.songStartTimes[i] > this.currentTime)
					{
						this.lastPlayIndex = (i - 1) % this.songStartTimes.Length;
						return;
					}
				}
			}
		}
	}

	// Token: 0x0600418B RID: 16779 RVA: 0x0015CB06 File Offset: 0x0015AD06
	private void StartPlayingSong(long timeStarted, long currentTime)
	{
		if (this.audioSource.volume != 0f)
		{
			this.audioSource.GTPlay();
		}
		this.audioSource.time = (float)(currentTime - timeStarted) / 1000f;
	}

	// Token: 0x0600418C RID: 16780 RVA: 0x0015CB3C File Offset: 0x0015AD3C
	private void StartPlayingSongs(long timeStarted, long currentTime)
	{
		foreach (AudioSource audioSource in this.audioSourceArray)
		{
			if (audioSource.volume != 0f)
			{
				audioSource.GTPlay();
			}
			audioSource.time = (float)(currentTime - timeStarted) / 1000f;
		}
	}

	// Token: 0x0600418D RID: 16781 RVA: 0x0015CB88 File Offset: 0x0015AD88
	private void StartPlayingSong(long timeStarted, long currentTime, AudioClip clipToPlay, AudioSource sourceToPlay)
	{
		this.audioSource = sourceToPlay;
		sourceToPlay.clip = clipToPlay;
		if (sourceToPlay.isActiveAndEnabled && sourceToPlay.volume != 0f)
		{
			sourceToPlay.GTPlay();
		}
		sourceToPlay.time = (float)(currentTime - timeStarted) / 1000f;
	}

	// Token: 0x0600418E RID: 16782 RVA: 0x0015CBD4 File Offset: 0x0015ADD4
	private void GenerateSongStartRandomTimes()
	{
		this.songStartTimes = new long[500];
		this.audioSourcesForPlaying = new int[500];
		this.audioClipsForPlaying = new int[500];
		this.songStartTimes[0] = this.minimumWait + (long)this.randomNumberGenerator.Next(this.randomInterval);
		for (int i = 1; i < this.songStartTimes.Length; i++)
		{
			this.songStartTimes[i] = this.songStartTimes[i - 1] + this.minimumWait + (long)this.randomNumberGenerator.Next(this.randomInterval);
		}
		if (this.usingMultipleSources)
		{
			for (int j = 0; j < this.audioSourcesForPlaying.Length; j++)
			{
				this.audioSourcesForPlaying[j] = this.randomNumberGenerator.Next(this.audioSourceArray.Length);
			}
		}
		if (this.usingMultipleSongs)
		{
			for (int k = 0; k < this.audioClipsForPlaying.Length; k++)
			{
				this.audioClipsForPlaying[k] = this.randomNumberGenerator.Next(this.songsArray.Length);
			}
		}
		if (this.usingMultipleSongs)
		{
			this.totalLoopTime = this.songStartTimes[this.songStartTimes.Length - 1] + (long)(this.songsArray[this.audioClipsForPlaying[this.audioClipsForPlaying.Length - 1]].length * 1000f);
			return;
		}
		if (this.audioSource.clip != null)
		{
			this.totalLoopTime = this.songStartTimes[this.songStartTimes.Length - 1] + (long)(this.audioSource.clip.length * 1000f);
		}
	}

	// Token: 0x0600418F RID: 16783 RVA: 0x0015CD64 File Offset: 0x0015AF64
	public void MuteAudio(GorillaPressableButton pressedButton)
	{
		AudioSource[] array;
		if (this.audioSource.mute)
		{
			PlayerPrefs.SetInt(this.locationName + "Muted", 0);
			PlayerPrefs.Save();
			this.audioSource.mute = false;
			array = this.audioSourceArray;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].mute = false;
			}
			pressedButton.isOn = false;
			pressedButton.UpdateColor();
			for (int j = 0; j < this.muteButtons.Length; j++)
			{
				if (this.muteButtons[j] != null)
				{
					this.muteButtons[j].isOn = false;
					this.muteButtons[j].UpdateColor();
				}
			}
			return;
		}
		PlayerPrefs.SetInt(this.locationName + "Muted", 1);
		PlayerPrefs.Save();
		this.audioSource.mute = true;
		array = this.audioSourceArray;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].mute = true;
		}
		pressedButton.isOn = true;
		pressedButton.UpdateColor();
		for (int k = 0; k < this.muteButtons.Length; k++)
		{
			if (this.muteButtons[k] != null)
			{
				this.muteButtons[k].isOn = true;
				this.muteButtons[k].UpdateColor();
			}
		}
	}

	// Token: 0x06004190 RID: 16784 RVA: 0x0015CEA4 File Offset: 0x0015B0A4
	protected void New_Start()
	{
		string text = this.New_Validate();
		if (text.Length > 0)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Disabling SynchedMusicController on \"",
				base.name,
				"\" due to invalid setup: ",
				text,
				" Path: ",
				base.transform.GetPathQ()
			}), this);
			base.enabled = false;
		}
		if (this.usingMultipleSources && this.audioSource == null)
		{
			this.audioSource = this.audioSourceArray[0];
		}
		this.totalLoopTime = 0L;
		bool flag = PlayerPrefs.GetInt(this.locationName + "Muted", 0) != 0;
		if (this.muteButton == null && this.muteButtons.Length >= 1 && this.muteButtons[0] != null)
		{
			this.muteButton = this.muteButtons[0];
		}
		if (this.audioSource != null)
		{
			this.audioSource.mute = flag;
			this.muteButton.isOn = this.audioSource.mute;
		}
		foreach (AudioSource audioSource in this.audioSourceArray)
		{
			audioSource.mute = flag;
			this.muteButton.isOn = audioSource.mute || this.muteButton.isOn;
		}
		for (int j = 0; j < this.muteButtons.Length; j++)
		{
			if (!(this.muteButtons[j] == null))
			{
				this.muteButtons[j].isOn = this.muteButton.isOn;
				this.muteButtons[j].UpdateColor();
			}
		}
		this.muteButton.UpdateColor();
		this.randomNumberGenerator = new Random(this.mySeed);
		this.New_GeneratePlaylistArrays();
		foreach (SynchedMusicController.SyncedSongInfo syncedSongInfo in this.syncedSongs)
		{
			if (syncedSongInfo.songLayers.Length > 1)
			{
				SynchedMusicController.SyncedSongLayerInfo[] songLayers = syncedSongInfo.songLayers;
				for (int k = 0; k < songLayers.Length; k++)
				{
					songLayers[k].audioClip.LoadAudioData();
				}
			}
		}
	}

	// Token: 0x06004191 RID: 16785 RVA: 0x0015D0CB File Offset: 0x0015B2CB
	public void OnEnable()
	{
		this.lastPlayIndex = -1;
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06004192 RID: 16786 RVA: 0x0015D0DB File Offset: 0x0015B2DB
	public void OnDisable()
	{
		this.StopAllAudioSources();
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06004193 RID: 16787 RVA: 0x0015D0EC File Offset: 0x0015B2EC
	private void StopAllAudioSources()
	{
		for (int i = 0; i < this.audioSourceArray.Length; i++)
		{
			this.audioSourceArray[i].Stop();
		}
	}

	// Token: 0x06004194 RID: 16788 RVA: 0x0015D11C File Offset: 0x0015B31C
	private void New_Update()
	{
		if (!GorillaComputer.hasInstance)
		{
			return;
		}
		if (GorillaComputer.instance.startupMillis == 0L || this.totalLoopTime <= 0L || this.songStartTimes.Length == 0)
		{
			return;
		}
		long startupMillis = GorillaComputer.instance.startupMillis;
		if (startupMillis <= 0L)
		{
			return;
		}
		long num = startupMillis + (long)(Time.realtimeSinceStartup * 1000f);
		long num2 = ((this.totalLoopTime > 0L) ? (num % this.totalLoopTime) : 0L);
		bool flag = false;
		if (this.lastPlayIndex < 0)
		{
			flag = true;
			for (int i = 1; i < 256; i++)
			{
				if (this.songStartTimes[i] > num2)
				{
					this.lastPlayIndex = (i - 1) % 256;
					break;
				}
			}
			if (this.lastPlayIndex < 0)
			{
				this.lastPlayIndex = 255;
			}
		}
		int num3 = (this.lastPlayIndex + 1) % 256;
		if (this.songStartTimes[num3] < num2)
		{
			this.lastPlayIndex = num3;
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		long num4 = this.songStartTimes[this.lastPlayIndex];
		SynchedMusicController.SyncedSongInfo syncedSongInfo = this.syncedSongs[this.audioClipsForPlaying[this.lastPlayIndex]];
		float length = syncedSongInfo.songLayers[0].audioClip.length;
		float num5 = (float)(num2 - num4) / 1000f;
		if (num5 < 0f || length < num5)
		{
			return;
		}
		for (int j = 0; j < syncedSongInfo.songLayers.Length; j++)
		{
			SynchedMusicController.SyncedSongLayerInfo syncedSongLayerInfo = syncedSongInfo.songLayers[j];
			if (syncedSongLayerInfo.audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.All)
			{
				foreach (AudioSource audioSource in this.audioSourceArray)
				{
					audioSource.clip = syncedSongLayerInfo.audioClip;
					if (audioSource.volume > 0f)
					{
						audioSource.GTPlay();
					}
					audioSource.time = num5;
				}
			}
			else if (syncedSongLayerInfo.audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.Shuffle)
			{
				AudioSource audioSource2 = this.audioSourceArray[this.audioSourcesForPlaying[this.lastPlayIndex]];
				audioSource2.clip = syncedSongLayerInfo.audioClip;
				if (audioSource2.volume > 0f)
				{
					audioSource2.GTPlay();
				}
				audioSource2.time = num5;
			}
			else if (syncedSongLayerInfo.audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.Specific)
			{
				foreach (AudioSource audioSource3 in syncedSongLayerInfo.audioSources)
				{
					audioSource3.clip = syncedSongLayerInfo.audioClip;
					if (audioSource3.volume > 0f)
					{
						audioSource3.GTPlay();
					}
					audioSource3.time = num5;
				}
			}
		}
	}

	// Token: 0x06004195 RID: 16789 RVA: 0x0015D39C File Offset: 0x0015B59C
	private string New_Validate()
	{
		if (this.syncedSongs == null)
		{
			return "syncedSongs array cannot be null.";
		}
		if (this.syncedSongs.Length == 0)
		{
			return "syncedSongs array cannot be empty.";
		}
		for (int i = 0; i < this.syncedSongs.Length; i++)
		{
			SynchedMusicController.SyncedSongInfo syncedSongInfo = this.syncedSongs[i];
			if (syncedSongInfo.songLayers == null)
			{
				return string.Format("Song {0}'s songLayers array is null.", i);
			}
			if (syncedSongInfo.songLayers.Length == 0)
			{
				return string.Format("Song {0}'s songLayers array is empty.", i);
			}
			for (int j = 0; j < syncedSongInfo.songLayers.Length; j++)
			{
				SynchedMusicController.SyncedSongLayerInfo syncedSongLayerInfo = syncedSongInfo.songLayers[j];
				if (syncedSongLayerInfo.audioClip == null)
				{
					return string.Format("Song {0}'s song layer {1} does not have an audio clip.", i, j);
				}
				if (syncedSongLayerInfo.audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.Specific)
				{
					if (syncedSongLayerInfo.audioSources == null || syncedSongLayerInfo.audioSources.Length == 0)
					{
						return string.Format("Song {0}'s song layer {1} has audioSourcePickMode set to {2} ", i, j, syncedSongLayerInfo.audioSourcePickMode) + "but layer's audioSources array is empty or null.";
					}
				}
				else if (this.audioSourceArray == null || this.audioSourceArray.Length == 0)
				{
					return string.Format("{0} is null or empty, while Song {1}'s song layer {2} has ", "audioSourceArray", i, j) + string.Format("audioSourcePickMode set to {0}, which uses the ", syncedSongLayerInfo.audioSourcePickMode) + "component's audioSourceArray.";
				}
			}
		}
		return string.Empty;
	}

	// Token: 0x06004196 RID: 16790 RVA: 0x0015D504 File Offset: 0x0015B704
	private void New_GeneratePlaylistArrays()
	{
		if (this.syncedSongs == null || this.syncedSongs.Length == 0)
		{
			return;
		}
		this.songStartTimes = new long[256];
		this.songStartTimes[0] = this.minimumWait + (long)this.randomNumberGenerator.Next(this.randomInterval);
		for (int i = 1; i < this.songStartTimes.Length; i++)
		{
			this.songStartTimes[i] = this.songStartTimes[i - 1] + this.minimumWait + (long)this.randomNumberGenerator.Next(this.randomInterval);
		}
		this.audioSourcesForPlaying = new int[256];
		bool flag = false;
		SynchedMusicController.SyncedSongInfo[] array = this.syncedSongs;
		for (int j = 0; j < array.Length; j++)
		{
			SynchedMusicController.SyncedSongLayerInfo[] songLayers = array[j].songLayers;
			for (int k = 0; k < songLayers.Length; k++)
			{
				if (songLayers[k].audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.Shuffle)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			for (int l = 0; l < this.audioSourcesForPlaying.Length; l++)
			{
				this.audioSourcesForPlaying[l] = this.randomNumberGenerator.Next(this.audioSourceArray.Length);
			}
		}
		this.audioClipsForPlaying = new int[256];
		for (int m = 0; m < this.audioClipsForPlaying.Length; m++)
		{
			if (this.shufflePlaylist)
			{
				this.audioClipsForPlaying[m] = this.randomNumberGenerator.Next(this.syncedSongs.Length);
			}
			else
			{
				this.audioClipsForPlaying[m] = this.syncedSongs.Length - 1;
			}
		}
		SynchedMusicController.SyncedSongInfo[] array2 = this.syncedSongs;
		int[] array3 = this.audioClipsForPlaying;
		long num = (long)array2[array3[array3.Length - 1]].songLayers[0].audioClip.length * 1000L;
		long[] array4 = this.songStartTimes;
		long num2 = array4[array4.Length - 1];
		this.totalLoopTime = num + num2;
	}

	// Token: 0x04005225 RID: 21029
	[SerializeField]
	private bool usingNewSyncedSongsCode;

	// Token: 0x04005226 RID: 21030
	[SerializeField]
	private bool shufflePlaylist = true;

	// Token: 0x04005227 RID: 21031
	[SerializeField]
	private SynchedMusicController.SyncedSongInfo[] syncedSongs;

	// Token: 0x04005228 RID: 21032
	[Tooltip("This should be unique per sound post. Sound posts that share the same seed and the same song count will play songs a the same times.")]
	public int mySeed;

	// Token: 0x04005229 RID: 21033
	private Random randomNumberGenerator = new Random();

	// Token: 0x0400522A RID: 21034
	[Tooltip("In milliseconds.")]
	public long minimumWait = 900000L;

	// Token: 0x0400522B RID: 21035
	[Tooltip("In milliseconds. A random value between 0 and this will be picked. The max wait time is randomInterval + minimumWait.")]
	public int randomInterval = 600000;

	// Token: 0x0400522C RID: 21036
	[DebugReadout]
	public long[] songStartTimes;

	// Token: 0x0400522D RID: 21037
	[DebugReadout]
	public int[] audioSourcesForPlaying;

	// Token: 0x0400522E RID: 21038
	[DebugReadout]
	public int[] audioClipsForPlaying;

	// Token: 0x0400522F RID: 21039
	public AudioSource audioSource;

	// Token: 0x04005230 RID: 21040
	public AudioSource[] audioSourceArray;

	// Token: 0x04005231 RID: 21041
	public AudioClip[] songsArray;

	// Token: 0x04005232 RID: 21042
	[DebugReadout]
	public int lastPlayIndex;

	// Token: 0x04005233 RID: 21043
	[DebugReadout]
	public long currentTime;

	// Token: 0x04005234 RID: 21044
	[DebugReadout]
	public long totalLoopTime;

	// Token: 0x04005235 RID: 21045
	public GorillaPressableButton muteButton;

	// Token: 0x04005236 RID: 21046
	public GorillaPressableButton[] muteButtons;

	// Token: 0x04005237 RID: 21047
	public bool usingMultipleSongs;

	// Token: 0x04005238 RID: 21048
	public bool usingMultipleSources;

	// Token: 0x04005239 RID: 21049
	[DebugReadout]
	public bool isPlayingCurrently;

	// Token: 0x0400523A RID: 21050
	[DebugReadout]
	public bool testPlay;

	// Token: 0x0400523B RID: 21051
	public bool twoLayer;

	// Token: 0x0400523C RID: 21052
	[Tooltip("Used to store the muted sound posts in player prefs.")]
	public string locationName;

	// Token: 0x0400523D RID: 21053
	private const int kPlaylistLength = 256;

	// Token: 0x020009FA RID: 2554
	[Serializable]
	public struct SyncedSongInfo
	{
		// Token: 0x0400523E RID: 21054
		[Tooltip("A layer for a song. For no layers, just add a single entry.")]
		[RequiredListLength(1, null)]
		public SynchedMusicController.SyncedSongLayerInfo[] songLayers;
	}

	// Token: 0x020009FB RID: 2555
	[Serializable]
	public struct SyncedSongLayerInfo
	{
		// Token: 0x0400523F RID: 21055
		[Tooltip("The clip that will be played.")]
		public AudioClip audioClip;

		// Token: 0x04005240 RID: 21056
		public SynchedMusicController.AudioSourcePickMode audioSourcePickMode;

		// Token: 0x04005241 RID: 21057
		[Tooltip("The audio sources that should play the audio clip.")]
		public AudioSource[] audioSources;
	}

	// Token: 0x020009FC RID: 2556
	public enum AudioSourcePickMode
	{
		// Token: 0x04005243 RID: 21059
		All,
		// Token: 0x04005244 RID: 21060
		Shuffle,
		// Token: 0x04005245 RID: 21061
		Specific
	}
}
