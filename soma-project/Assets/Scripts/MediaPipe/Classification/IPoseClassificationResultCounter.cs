namespace SOMA.MediaPipe.Classification
{
	public interface IPoseClassificationResultCounter
	{
		protected const float DEFAULT_ENTER_THRESHOLD = 0.6f;
		protected const float DEFAULT_EXIT_THRESHOLD = 0.4f;

		public string CounterText { get; }

		public float AddClassificationResult(PoseClassificationResult classificationResult, int confidenceRange);
	}
}
