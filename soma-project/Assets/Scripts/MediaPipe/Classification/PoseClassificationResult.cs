using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SOMA.MediaPipe.Classification
{
	public class PoseClassificationResult
	{
		private readonly Dictionary<string, float> _classConfidences;
		public ReadOnlyDictionary<string, float> ClassConfidences => new ReadOnlyDictionary<string, float>(_classConfidences);
		public IEnumerable<string> ClassNames => _classConfidences.Keys;

		public PoseClassificationResult()
		{
			_classConfidences = new Dictionary<string, float>();
		}

		public float this[string className]
		{
			get {
				if (_classConfidences.TryGetValue(className, out float confidence))
				{
					return confidence;
				}
				else
				{
					return 0;
				}
			}
			set => _classConfidences[className] = value;
		}

		public string MaxConfidenceClass => _classConfidences.MaxBy(x => x.Value).Key;

		public void IncrementClassConfidence(string className) => this[className] += 1;
	}
}
