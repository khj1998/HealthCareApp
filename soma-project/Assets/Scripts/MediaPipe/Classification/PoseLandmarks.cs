using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Mediapipe;

namespace SOMA.MediaPipe.Classification
{
	public class PoseLandmarks
	{
		public enum BodyPart
		{
			Nose = 0,
			LeftEyeInner = 1,
			LeftEye = 2,
			LeftEyeOuter = 3,
			RightEyeInner = 4,
			RightEye = 5,
			RightEyeOuter = 6,
			LeftEar = 7,
			RightEar = 8,
			LeftMouth = 9,
			RightMouth = 10,
			LeftShoulder = 11,
			RightShoulder = 12,
			LeftElbow = 13,
			RightElbow = 14,
			LeftWrist = 15,
			RightWrist = 16,
			LeftPinky = 17,
			RightPinky = 18,
			LeftIndex = 19,
			RightIndex = 20,
			LeftThumb = 21,
			RightThumb = 22,
			LeftHip = 23,
			RightHip = 24,
			LeftKnee = 25,
			RightKnee = 26,
			LeftAnkle = 27,
			RightAnkle = 28,
			LeftHeel = 29,
			RightHeel = 30,
			LeftFootIndex = 31,
			RightFootIndex = 32
		}

		[Flags]
		public enum EmbeddingPoints
		{
			WristElbowL = 1 << 0,
			WristElbowR = 1 << 1,
			ElbowShoulderL = 1 << 2,
			ElbowShoulderR = 1 << 3,
			ShoulderHipM = 1 << 4, // M은 LR 중간
			HipKneeL = 1 << 5,
			HipKneeR = 1 << 6,
			KneeAnkleL = 1 << 7,
			KneeAnkleR = 1 << 8,

			WristShoulderL = 1 << 9,
			WristShoulderR = 1 << 10,
			HipAnkleL = 1 << 11,
			HipAnkleR = 1 << 12,

			WristHipL = 1 << 13,
			WristHipR = 1 << 14,
			ShoulderAnkleL = 1 << 15,
			ShoulderAnkleR = 1 << 16,

			WristAnkleL = 1 << 17,
			WristAnkleR = 1 << 18,

			WristCross = 1 << 19,
			ElbowCross = 1 << 20,
			ShoulderHipLR = 1 << 21,
			ShoulderHipRL = 1 << 22,
			HipCross = 1 << 23,
			KneeCross = 1 << 24,
			AnkleCross = 1 << 25,

			// Preset들
			All = (1 << 26) - 1,
			Upper = WristElbowL | WristElbowR | ElbowShoulderL | ElbowShoulderR | WristShoulderL | WristShoulderR | WristCross | ElbowCross,
			Core = ShoulderHipM | ShoulderHipLR | ShoulderHipRL,
			Lower = HipKneeL | HipKneeR | KneeAnkleL | KneeAnkleR | HipAnkleL | HipAnkleR | HipCross | KneeCross | AnkleCross
		}

		public const int LANDMARKS_COUNT = 33;
		public const int EMBEDDING_COUNT = 26;

		private readonly List<Point> _landmarks;
		public ReadOnlyCollection<Point> Landmarks => _landmarks.AsReadOnly();

		public Point this[int index] => _landmarks[index];
		public Point this[BodyPart index] => this[(int)index];

		private float _poseSize
		{
			get {
				Point hipsCenter = Point.Average(this[BodyPart.LeftHip], this[BodyPart.RightHip]);
				Point shouldersCenter = Point.Average(this[BodyPart.LeftShoulder], this[BodyPart.RightShoulder]);

				float torsoSize = Point.L2Norm2D(shouldersCenter - hipsCenter);
				float maxDistance = torsoSize * 2.5f; // Picked this by experimentation.

				return Mathf.Max(maxDistance, (
					from landmark in _landmarks
					select Point.L2Norm2D(landmark - hipsCenter)
				).Max());
			}
		}

		public List<Point> GetPoseEmbedding()
		{
			Point center = Point.Average(this[BodyPart.LeftHip], this[BodyPart.RightHip]);
			float scale = 100.0f / _poseSize;

			PoseLandmarks normalizedLandmarks = new PoseLandmarks(
				from landmark in _landmarks
				let centeredLandmark = landmark - center
				select centeredLandmark * scale
			);
			List<Point> embedding = new List<Point>();

			// We use several pairwise 3D distances to form pose embedding. These were selected
			// based on experimentation for best results with our default pose classes as captued in the
			// pose samples csv. Feel free to play with this and add or remove for your use-cases.

			// We group our distances by number of joints between the pairs.
			// One joint.
			embedding.Add(normalizedLandmarks[BodyPart.LeftWrist] - normalizedLandmarks[BodyPart.LeftElbow]);
			embedding.Add(normalizedLandmarks[BodyPart.RightWrist] - normalizedLandmarks[BodyPart.RightElbow]);

			embedding.Add(normalizedLandmarks[BodyPart.LeftElbow] - normalizedLandmarks[BodyPart.LeftShoulder]);
			embedding.Add(normalizedLandmarks[BodyPart.RightElbow] - normalizedLandmarks[BodyPart.RightShoulder]);

			embedding.Add(Point.Average(normalizedLandmarks[BodyPart.LeftShoulder], normalizedLandmarks[BodyPart.RightShoulder]) - Point.Average(normalizedLandmarks[BodyPart.LeftHip], normalizedLandmarks[BodyPart.RightHip]));

			embedding.Add(normalizedLandmarks[BodyPart.LeftHip] - normalizedLandmarks[BodyPart.LeftKnee]);
			embedding.Add(normalizedLandmarks[BodyPart.RightHip] - normalizedLandmarks[BodyPart.RightKnee]);

			embedding.Add(normalizedLandmarks[BodyPart.LeftKnee] - normalizedLandmarks[BodyPart.LeftAnkle]);
			embedding.Add(normalizedLandmarks[BodyPart.RightKnee] - normalizedLandmarks[BodyPart.RightAnkle]);

			// Two joints.
			embedding.Add(normalizedLandmarks[BodyPart.LeftWrist] - normalizedLandmarks[BodyPart.LeftShoulder]);
			embedding.Add(normalizedLandmarks[BodyPart.RightWrist] - normalizedLandmarks[BodyPart.RightShoulder]);

			embedding.Add(normalizedLandmarks[BodyPart.LeftHip] - normalizedLandmarks[BodyPart.LeftAnkle]);
			embedding.Add(normalizedLandmarks[BodyPart.RightHip] - normalizedLandmarks[BodyPart.RightAnkle]);

			// Three joints.
			embedding.Add(normalizedLandmarks[BodyPart.LeftWrist] - normalizedLandmarks[BodyPart.LeftHip]);
			embedding.Add(normalizedLandmarks[BodyPart.RightWrist] - normalizedLandmarks[BodyPart.RightHip]);

			embedding.Add(normalizedLandmarks[BodyPart.LeftShoulder] - normalizedLandmarks[BodyPart.LeftAnkle]);
			embedding.Add(normalizedLandmarks[BodyPart.RightShoulder] - normalizedLandmarks[BodyPart.RightAnkle]);

			// Five joints.
			embedding.Add(normalizedLandmarks[BodyPart.LeftWrist] - normalizedLandmarks[BodyPart.LeftAnkle]);
			embedding.Add(normalizedLandmarks[BodyPart.RightWrist] - normalizedLandmarks[BodyPart.RightAnkle]);

			// Cross body.
			embedding.Add(normalizedLandmarks[BodyPart.RightWrist] - normalizedLandmarks[BodyPart.LeftWrist]);
			embedding.Add(normalizedLandmarks[BodyPart.RightElbow] - normalizedLandmarks[BodyPart.LeftElbow]);
			embedding.Add(normalizedLandmarks[BodyPart.LeftShoulder] - normalizedLandmarks[BodyPart.RightHip]);
			embedding.Add(normalizedLandmarks[BodyPart.RightShoulder] - normalizedLandmarks[BodyPart.LeftHip]);
			embedding.Add(normalizedLandmarks[BodyPart.RightHip] - normalizedLandmarks[BodyPart.LeftHip]);
			embedding.Add(normalizedLandmarks[BodyPart.RightKnee] - normalizedLandmarks[BodyPart.LeftKnee]);
			embedding.Add(normalizedLandmarks[BodyPart.RightAnkle] - normalizedLandmarks[BodyPart.LeftAnkle]);

			return embedding;
		}

		public PoseLandmarks(IEnumerable<Point> landmarks)
		{
			_landmarks = landmarks.ToList();

			if (_landmarks.Count != LANDMARKS_COUNT)
			{
				throw new ArgumentException($"Landmark count mismatch: Expected {LANDMARKS_COUNT}, Actual {_landmarks.Count}");
			}
		}

		public static PoseLandmarks Build(NormalizedLandmarkList landmarks)
		{
			if (landmarks is null)
			{
				return null;
			}

			return new PoseLandmarks(
				from landmark in landmarks.Landmark
				select new Point(landmark)
			);
		}
	}
}
