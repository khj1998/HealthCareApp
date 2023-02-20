using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace SOMA.MediaPipe.Classification
{
	public class EMASmoothing
	{
		private const int DEFAULT_WINDOW_SIZE = 10;
		private const float DEFAULT_ALPHA = 0.2f;
		private const long RESET_THRESHOLD_MS = 100;

		private readonly string[] _allClassNames;
		private readonly int _windowSize;
		private readonly float _alpha;
		private readonly long _resetThreshold;
		private readonly Queue<PoseClassificationResult> _window;
		private Stopwatch _stopwatch;
		private long _lastInputMilliseconds;

		private long _currentMilliseconds => _stopwatch.ElapsedMilliseconds;

		public EMASmoothing(string[] allClassNames, int windowSize = DEFAULT_WINDOW_SIZE, float alpha = DEFAULT_ALPHA, long resetThreshold = RESET_THRESHOLD_MS)
		{
			this._allClassNames = allClassNames;
			this._windowSize = windowSize;
			this._alpha = alpha;
			this._resetThreshold = resetThreshold;
			this._window = new Queue<PoseClassificationResult>(windowSize);
			this._stopwatch = new Stopwatch();
			this._lastInputMilliseconds = 0;

			_stopwatch.Start();
		}

		public PoseClassificationResult SmoothResult(PoseClassificationResult classificationResult)
		{
			if (_currentMilliseconds - _lastInputMilliseconds > _resetThreshold)
			{
				_window.Clear();
			}
			_lastInputMilliseconds = _currentMilliseconds;

			if (_window.Count == _windowSize)
			{
				_window.Dequeue();
			}
			_window.Enqueue(classificationResult);

			var smoothedClassificationResult = new PoseClassificationResult();

			foreach (string className in _allClassNames)
			{
				float factor = 1.0f;
				float topSum = 0.0f;
				float bottomSum = 0.0f;

				foreach (var result in _window.Reverse())
				{
					float value = result[className];

					topSum += factor * value;
					bottomSum += factor;

					factor *= 1.0f - _alpha;
				}

				smoothedClassificationResult[className] = topSum / bottomSum;
			}

			return smoothedClassificationResult;
		}

		~EMASmoothing()
		{
			if (_stopwatch?.IsRunning == true)
			{
				_stopwatch.Stop();
			}
		}
	}
}
