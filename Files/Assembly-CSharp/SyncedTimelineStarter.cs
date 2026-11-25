using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Playables;

public class SyncedTimelineStarter : MonoBehaviour, IGorillaSliceableSimple
{
	[SerializeField]
	private PlayableDirector _director;

	[Tooltip("If the Timeline drifts further than this (seconds) from where server time says it should be, snap it back. Drift mainly comes from frame hitches longer than Time.maximumDeltaTime. 0 disables correction.")]
	[SerializeField]
	[Min(0f)]
	private float _resyncThreshold = 0.5f;

	[Tooltip("Non-looping Timeline started after it already ended: evaluate the final frame so late joiners see the end state (e.g. a door that is already open). If false, nothing happens.")]
	[SerializeField]
	private bool _holdEndStateWhenLate = true;

	private DateTime _anchorServerTime = DateTime.MinValue;

	private bool HasAnchor => _anchorServerTime != DateTime.MinValue;

	private bool IsLooping => _director.extrapolationMode == DirectorWrapMode.Loop;

	public void PlayFromLateSeconds(float lateSeconds)
	{
		if (!(lateSeconds < 0f))
		{
			if (!TryGetServerNow(out var now))
			{
				SeekAndPlay(lateSeconds);
				return;
			}
			_anchorServerTime = now - TimeSpan.FromSeconds(lateSeconds);
			SeekToAnchor(now);
		}
	}

	public void PlayNow()
	{
		PlayFromLateSeconds(0f);
	}

	public void StopTimeline()
	{
		_anchorServerTime = DateTime.MinValue;
		if (_director != null)
		{
			_director.Stop();
		}
	}

	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		if (HasAnchor && TryGetServerNow(out var now))
		{
			SeekToAnchor(now);
		}
	}

	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (_resyncThreshold <= 0f || !HasAnchor || _director == null || _director.state != PlayState.Playing || !TryGetServerNow(out var now))
		{
			return;
		}
		double num = ExpectedTime(now);
		if (!(num < 0.0))
		{
			double num2 = Math.Abs(_director.time - num);
			if (IsLooping && _director.duration > 0.0)
			{
				num2 = Math.Min(num2, _director.duration - num2);
			}
			if (num2 > (double)_resyncThreshold)
			{
				_director.time = num;
				_director.Evaluate();
			}
		}
	}

	private void SeekToAnchor(DateTime now)
	{
		double totalSeconds = (now - _anchorServerTime).TotalSeconds;
		SeekAndPlay(totalSeconds);
	}

	private void SeekAndPlay(double elapsed)
	{
		if (_director == null || _director.playableAsset == null)
		{
			Debug.LogError("SyncedTimelineStarter :: " + base.name + " :: no PlayableDirector or Timeline asset assigned.", this);
			return;
		}
		double duration = _director.duration;
		if (!IsLooping && elapsed >= duration)
		{
			if (_holdEndStateWhenLate)
			{
				if (_director.state != PlayState.Playing)
				{
					_director.Play();
				}
				_director.time = duration;
				_director.Evaluate();
				_director.Pause();
			}
			_anchorServerTime = DateTime.MinValue;
		}
		else
		{
			if (_director.state != PlayState.Playing)
			{
				_director.Play();
			}
			_director.time = ((IsLooping && duration > 0.0) ? (elapsed % duration) : elapsed);
			_director.Evaluate();
		}
	}

	private double ExpectedTime(DateTime now)
	{
		double totalSeconds = (now - _anchorServerTime).TotalSeconds;
		double duration = _director.duration;
		if (IsLooping && duration > 0.0)
		{
			return totalSeconds % duration;
		}
		if (!(totalSeconds <= duration))
		{
			return -1.0;
		}
		return totalSeconds;
	}

	private static bool TryGetServerNow(out DateTime now)
	{
		now = default(DateTime);
		if (GorillaComputer.instance == null)
		{
			return false;
		}
		now = GorillaComputer.instance.GetServerTime();
		return now.Year >= 2000;
	}
}
