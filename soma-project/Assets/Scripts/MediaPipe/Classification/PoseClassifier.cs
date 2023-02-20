using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using EmbeddingPoints = SOMA.MediaPipe.Classification.PoseLandmarks.EmbeddingPoints;

namespace SOMA.MediaPipe.Classification
{
	public class PoseClassifier
	{
		public const int MAX_DISTANCE_TOP_K = 30;
		public const int MEAN_DISTANCE_TOP_K = 10;
		// Z has a lower weight as it is generally less accurate than X & Y.
		private static readonly Point AXES_WEIGHTS = new Point(1.0f, 1.0f, 0.2f);

		private readonly List<PoseSample> _poseSamples;
		private readonly int _enabledEmbeddingPointsFlag;
		private readonly int _maxDistanceTopK;
		private readonly int _meanDistanceTopK;
		private readonly Point _axesWeights;

		public int ConfidenceRange => Math.Min(_maxDistanceTopK, _meanDistanceTopK);

		public PoseClassifier(IEnumerable<PoseSample> poseSamples, EmbeddingPoints enabledEmbeddingPoints, int maxDistanceTopK = MAX_DISTANCE_TOP_K, int meanDistanceTopK = MEAN_DISTANCE_TOP_K)
			: this(poseSamples, enabledEmbeddingPoints, maxDistanceTopK, meanDistanceTopK, AXES_WEIGHTS) {}

		public PoseClassifier(IEnumerable<PoseSample> poseSamples, EmbeddingPoints enabledEmbeddingPoints, int maxDistanceTopK, int meanDistanceTopK, Point axesWeights)
		{
			this._poseSamples = poseSamples.ToList();
			int size = _poseSamples.Count();
			this._enabledEmbeddingPointsFlag = (int)enabledEmbeddingPoints;
			this._maxDistanceTopK = Math.Min(maxDistanceTopK, size);
			this._meanDistanceTopK = Math.Min(meanDistanceTopK, maxDistanceTopK);
			this._axesWeights = axesWeights;
		}

		public PoseClassificationResult Classify(PoseLandmarks landmarks)
		{
			PoseClassificationResult classificationResult = new PoseClassificationResult();
			// Return early if no landmarks detected.
			if (landmarks is null)
			{
				return classificationResult;
			}

			List<Point> originalEmbedding = landmarks.GetPoseEmbedding();
			// We do flipping on X-axis so we are horizontal (mirror) invariant.
			List<Point> flippedEmbedding = (
				from embedding in originalEmbedding
				select embedding * new Point(-1, 1, 1)
			).ToList();

			// Classification is done in two stages:
			//  * First we pick top-K samples by MAX distance. It allows to remove samples that are almost
			//    the same as given pose, but maybe has few joints bent in the other direction.
			//  * Then we pick top-K samples by MEAN distance. After outliers are removed, we pick samples
			//    that are closest by average.

			((PoseSample Sample, float MeanDistance) Sample, float MaxDistance)[] sampleDistances = new ((PoseSample, float), float)[Math.Max(_maxDistanceTopK, _meanDistanceTopK) + 1];

			// Retrieve top K poseSamples by least distance to remove outliers.
			for (int i = 0; i < _poseSamples.Count(); ++i)
			{
				var sampleEmbedding = _poseSamples[i].Embedding;

				float originalMax = float.MinValue;
				float originalSum = 0.0f;
				float flippedMax = float.MinValue;
				float flippedSum = 0.0f;

				int flag = 1;
				foreach (var originalPoint in Enumerable.Zip(sampleEmbedding, originalEmbedding, (sample, original) => (sample - original) * _axesWeights))
				{
					if ((flag & _enabledEmbeddingPointsFlag) != 0)
					{
						originalMax = Mathf.Max(originalMax, Point.MaxAbs(originalPoint));
						originalSum += Point.SumAbs(originalPoint);
					}
					flag <<= 1;
				}

				flag = 1;
				foreach (var flippedPoint in Enumerable.Zip(sampleEmbedding, flippedEmbedding, (sample, flipped) => (sample - flipped) * _axesWeights))
				{
					if ((flag & _enabledEmbeddingPointsFlag) != 0)
					{
						flippedMax = Mathf.Max(flippedMax, Point.MaxAbs(flippedPoint));
						flippedSum += Point.SumAbs(flippedPoint);
					}
					flag <<= 1;
				}

				// Set the max distance as min of original and flipped max distance.
				float maxDistance = Mathf.Min(originalMax, flippedMax);
				// Set the mean distance as min of original and flipped mean distances.
				float meanDistance = Mathf.Min(originalSum, flippedSum);

				int j = Math.Min(_maxDistanceTopK, i);
				while (j > 0 && maxDistance < sampleDistances[j - 1].MaxDistance)
				{
					sampleDistances[j] = sampleDistances[j - 1];
					--j;
				}
				sampleDistances[j] = ((_poseSamples[i], meanDistance), maxDistance);
			}

			// Retrive top K poseSamples by least mean distance to remove outliers.
			for (int i = 0; i < _maxDistanceTopK; ++i)
			{
				var sample = sampleDistances[i];
				float meanDistance = sample.Sample.MeanDistance;

				int j = Math.Min(_meanDistanceTopK, i);
				while (j > 0 && meanDistance < sampleDistances[j - 1].Sample.MeanDistance)
				{
					sampleDistances[j] = sampleDistances[j - 1];
					--j;
				}
				sampleDistances[j] = sample;
			}

			for (int i = 0; i < _meanDistanceTopK; ++i)
			{
				string className = sampleDistances[i].Sample.Sample.ClassName;
				classificationResult.IncrementClassConfidence(className);
			}

			return classificationResult;
		}
	}
}
