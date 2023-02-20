using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SOMA.MediaPipe.Classification
{
	public class ScoreCounter : IPoseClassificationResultCounter
	{
		private const float SCORE_ENTER_THRESHOLD = 1.0f;
		private const float SCORE_EXIT_THRESHOLD = 0.7f;

		private readonly Dictionary<string, int> _poseClassScore;
		private readonly float _enterThreshold;
		private readonly float _exitThreshold;

		private bool _poseEntered;
		private float _maxScore;
		private float _totalScore;

		private string _counterText;
		public string CounterText => _counterText;

		public ScoreCounter(string[] poseClassNames) : this(poseClassNames, SCORE_ENTER_THRESHOLD, SCORE_EXIT_THRESHOLD) {}
		public ScoreCounter(string[] poseClassNames, float enterThreshold, float exitThreshold)
		{
			this._poseClassScore = new Dictionary<string, int>(
				poseClassNames.Select((poseClassName, index) => new KeyValuePair<string, int>(poseClassName, index))
			);
			this._enterThreshold = enterThreshold;
			this._exitThreshold = exitThreshold;
			this._poseEntered = false;
			this._maxScore = 0;
			this._totalScore = 0;
		}

		public float AddClassificationResult(PoseClassificationResult classificationResult, int confidenceRange)
		{
			float currentScore = ClassificationScore(classificationResult, confidenceRange);

			if (currentScore > _enterThreshold)
			{
				_poseEntered = true;
			}
			if (_poseEntered)
			{
				_maxScore = Mathf.Max(_maxScore, currentScore);
				if (currentScore < _exitThreshold)
				{
					_totalScore += ScoreFunction(_maxScore);
					_maxScore = 0;
					_poseEntered = false;
				}
			}

			_counterText = $"TOT: {_totalScore} / MAX: {_maxScore} / {currentScore}: {ScoreString(classificationResult)}";

			return _totalScore;
		}

		//private float ScoreFunction(float score) => Mathf.Floor(MoreMath.Square(score - 1.5f) * 20.0f + 5.0f);
		private float ScoreFunction(float score) => Mathf.Floor(MoreMath.Square(score) * 10);	

		private float ClassificationScore(PoseClassificationResult classificationResult, int confidenceRange)
		{
			float score = 0.0f;
			foreach (var (className, confidence) in classificationResult.ClassConfidences)
			{
				float classScore = _poseClassScore[className];
				score += classScore * confidence;
			}

			return score / confidenceRange;
		}

		private string ScoreString(PoseClassificationResult classificationResult)
		{
			return string.Concat(
				from classScore in _poseClassScore
				let name = classScore.Key
				let score = classScore.Value
				let confidence = classificationResult[name]
				orderby score
				select (score, confidence)
			);
		}
	}
}
