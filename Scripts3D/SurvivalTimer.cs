// TEACHING GUIDE: SurvivalTimer is OUR plain C# gameplay class.
// It stores and updates survival time without depending on Godot types.
// Member B can connect its announcement event and expired state to Game3D.

using System;

public sealed class SurvivalTimer
{
	public const float DefaultDurationSeconds = 60f;

	private const int AnnouncementIntervalSeconds = 10;

	private readonly float _durationSeconds;

	private int _nextAnnouncementSeconds;

	public float RemainingSeconds { get; private set; }

	public bool IsExpired =>
		RemainingSeconds <= 0f;

	// OUR event: announces whole seconds remaining at 10-second boundaries.
	public event Action<int> TimeAnnouncement;

	public SurvivalTimer(
		float durationSeconds = DefaultDurationSeconds)
	{
		if (durationSeconds <= 0f)
		{
			throw new ArgumentOutOfRangeException(
				nameof(durationSeconds),
				"Survival duration must be greater than zero.");
		}

		_durationSeconds = durationSeconds;

		Reset();
	}

	public void Reset()
	{
		RemainingSeconds = _durationSeconds;

		_nextAnnouncementSeconds =
			(int)MathF.Floor(
				_durationSeconds
				/ AnnouncementIntervalSeconds)
			* AnnouncementIntervalSeconds;

		// A 60-second round first announces 50 seconds after ten seconds pass.
		if (_nextAnnouncementSeconds >= _durationSeconds)
		{
			_nextAnnouncementSeconds -=
				AnnouncementIntervalSeconds;
		}
	}

	public void Update(float delta)
	{
		if (delta <= 0f || IsExpired)
		{
			return;
		}

		float previousSeconds = RemainingSeconds;

		RemainingSeconds = MathF.Max(
			0f,
			RemainingSeconds - delta);

		while (_nextAnnouncementSeconds > 0
			&& previousSeconds > _nextAnnouncementSeconds
			&& RemainingSeconds <= _nextAnnouncementSeconds)
		{
			TimeAnnouncement?.Invoke(
				_nextAnnouncementSeconds);

			_nextAnnouncementSeconds -=
				AnnouncementIntervalSeconds;
		}
	}
}
