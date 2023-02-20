using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mediapipe;
using TMPro;

using EmbeddingPoints = SOMA.MediaPipe.Classification.PoseLandmarks.EmbeddingPoints;

namespace SOMA.MediaPipe.Classification
{
	public class PoseClassifierProcessor : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI tmp;
		public enum CounterType
		{
			Repetition,
			Duration,
			Score
		}

		[SerializeField]
		private TextAsset _poseSamplesText;

		[SerializeField]
		private CounterType _counterType;
		[SerializeField]
		private int _maxDistanceTopK = PoseClassifier.MAX_DISTANCE_TOP_K;
		[SerializeField]
		private int _meanDistanceTopK = PoseClassifier.MEAN_DISTANCE_TOP_K;
		[SerializeField]
		private EmbeddingPoints _enabledEmbeddingPoints = EmbeddingPoints.All;
		[SerializeField]
		private string[] _poseClassNames;

		private PoseClassifier _poseClassifier;
		private EMASmoothing _emaSmoothing;
		private IPoseClassificationResultCounter _counter;

		private float _previousResult;

		// Start is called before the first frame update
		private void Start()
		{
			_poseClassifier = new PoseClassifier(
				from poseSampleText in _poseSamplesText.text.Split('\n')
				where !string.IsNullOrWhiteSpace(poseSampleText)
				let poseSample = PoseSample.Parse(poseSampleText)
				where poseSample is not null
				select poseSample,
				_enabledEmbeddingPoints,
				_maxDistanceTopK,
				_meanDistanceTopK
			);
			_emaSmoothing = new EMASmoothing(_poseClassNames, 5, 0.3f);
			switch (_counterType)
			{
				case CounterType.Repetition: _counter = new RepetitionCounter(_poseClassNames, 0.55f, 0.45f); break;
				case CounterType.Duration: _counter = new DurationCounter(_poseClassNames[0], 0.55f, 0.45f); break;
				case CounterType.Score: _counter = new ScoreCounter(_poseClassNames); break;
			}

			_previousResult = 0.0f;
		}

		public float Classify(NormalizedLandmarkList normalizedLandmarkList)
		{
			var poseLandmarks = PoseLandmarks.Build(normalizedLandmarkList);

			if (poseLandmarks is null)
			{
				return _previousResult;
			}

			var classificationResult = _poseClassifier.Classify(poseLandmarks);
			var smoothedClassificationResult = _emaSmoothing.SmoothResult(classificationResult);
			_previousResult = _counter.AddClassificationResult(smoothedClassificationResult, _poseClassifier.ConfidenceRange);

			var maxConfidenceClass = smoothedClassificationResult.MaxConfidenceClass;
			Debug.Log($"CLASS {maxConfidenceClass} : {smoothedClassificationResult[maxConfidenceClass] / _poseClassifier.ConfidenceRange:F2} / COUNTER : {_counter.CounterText}");
			if (tmp is not null)
			{
				tmp.text = _counter.CounterText;
			}
			return _previousResult;
		}
	}
}
