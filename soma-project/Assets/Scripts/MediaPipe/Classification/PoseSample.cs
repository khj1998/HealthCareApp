using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace SOMA.MediaPipe.Classification
{
	public class PoseSample
	{
		public string Name { get; private set; }
		public string ClassName { get; private set; }
		private readonly List<Point> _embedding;
		public ReadOnlyCollection<Point> Embedding => _embedding.AsReadOnly();

		public PoseSample(string name, string className, IEnumerable<Point> landmarks) : this(name, className, new PoseLandmarks(landmarks)) {}

		public PoseSample(string name, string className, PoseLandmarks landmarks)
		{
			this.Name = name;
			this.ClassName = className;
			this._embedding = landmarks.GetPoseEmbedding();
		}

		public static PoseSample Parse(string s)
		{
			List<string> tokens = (
				from token in s.Split(',')
				select token.Trim()
			).ToList();

			if (tokens.Count != PoseLandmarks.LANDMARKS_COUNT * 3 + 2)
			{
				Debug.LogError($"Token count mismatch: Expected {PoseLandmarks.LANDMARKS_COUNT * 3 + 2}, Actual {tokens.Count}");
				return null;
			}

			string name = tokens[0];
			string className = tokens[1];

			IEnumerable<Point> landmarks =
				from landmarkToken in tokens.Skip(2).Chunk(3)
				let x = float.Parse(landmarkToken[0])
				let y = float.Parse(landmarkToken[1])
				let z = float.Parse(landmarkToken[2])
				select new Point(x, y, z);

			return new PoseSample(name, className, landmarks);
		}
	}
}
