namespace SOMA.MediaPipe.Classification
{
	public class RepetitionCounter : IPoseClassificationResultCounter
	{
		private readonly string[] _poseClassNames;
		private readonly float _enterThreshold;
		private readonly float _exitThreshold;

		private bool _poseEntered;
		private int _repetitions;
		private int _currentClassIndex;
		private readonly int _totalClassCount;

		private string _currentClassName => _poseClassNames[_currentClassIndex];

		public string CounterText => $"{_repetitions} ({_currentClassIndex} / {_totalClassCount})";

		public RepetitionCounter(string[] poseClassNames) : this(poseClassNames, IPoseClassificationResultCounter.DEFAULT_ENTER_THRESHOLD, IPoseClassificationResultCounter.DEFAULT_EXIT_THRESHOLD) {}
		public RepetitionCounter(string[] poseClassNames, float enterThreshold, float exitThreshold)
		{
			this._poseClassNames = poseClassNames;
			this._enterThreshold = enterThreshold;
			this._exitThreshold = exitThreshold;
			this._poseEntered = false;
			this._repetitions = 0;
			this._currentClassIndex = 0;
			this._totalClassCount = poseClassNames.Length;
		}

		public float AddClassificationResult(PoseClassificationResult classificationResult, int confidenceRange)
		{
			if (_poseEntered)
			{
				float confidence = classificationResult[_currentClassName] / confidenceRange;
				if (confidence < _exitThreshold)
				{
					if (++_currentClassIndex == _totalClassCount)
					{
						++_repetitions;
						_currentClassIndex = 0;
					}
					_poseEntered = false;
				}
			}

			// 같은 이름 가진 변수 생성용 중괄호
			{
				float confidence = classificationResult[_currentClassName] / confidenceRange;
				if (confidence > _enterThreshold)
				{
					_poseEntered = true;
				}
			}

			return _repetitions;
		}
	}
}
