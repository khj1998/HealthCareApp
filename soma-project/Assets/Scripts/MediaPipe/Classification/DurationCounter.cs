using UnityEngine;

namespace SOMA.MediaPipe.Classification
{
	public class DurationCounter : IPoseClassificationResultCounter
	{
		private readonly string _poseClassName;
		private readonly float _enterThreshold;
		private readonly float _exitThreshold;

		private bool _poseEntered;
		private float _duration;

		public string CounterText => $"{_duration}s";

		public DurationCounter(string poseClassName) : this(poseClassName, IPoseClassificationResultCounter.DEFAULT_ENTER_THRESHOLD, IPoseClassificationResultCounter.DEFAULT_EXIT_THRESHOLD) {}
		public DurationCounter(string poseClassName, float enterThreshold, float exitThreshold)
		{
			this._poseClassName = poseClassName;
			this._enterThreshold = enterThreshold;
			this._exitThreshold = exitThreshold;
			this._poseEntered = false;
			this._duration = 0;
		}

		public float AddClassificationResult(PoseClassificationResult classificationResult, int confidenceRange)
		{
			float confidence = classificationResult[_poseClassName] / confidenceRange;

			if (confidence < _exitThreshold)
			{
				_poseEntered = false;
				// _duration = 0.0f;
			}
			if (confidence > _enterThreshold)
			{
				_poseEntered = true;
			}

			if (_poseEntered)
			{
				_duration += Time.deltaTime;
			}

			return _duration;
		}
	}
}
