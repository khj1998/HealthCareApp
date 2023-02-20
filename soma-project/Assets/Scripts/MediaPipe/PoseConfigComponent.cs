using System;
using UnityEngine;

namespace SOMA.MediaPipe
{
	public class PoseConfigComponent : MonoBehaviour
	{
		// If set to false, the solution treats the input images as a video stream.
		// It will try to detect the most prominent person in the very first images, and upon a successful detection further localizes the pose landmarks.
		// In subsequent images, it then simply tracks those landmarks without invoking another detection until it loses track, on reducing computation and latency.
		// If set to true, person detection runs every input image, ideal for processing a batch of static, possibly unrelated, images.
		// Default to false.
		public bool StaticImageMode = PoseConfig.DefaultConfig.StaticImageMode;
		// If set to true, the solution filters pose landmarks across different input images to reduce jitter.
		// Ignored if static_image_mode is also set to true.
		public bool SmoothLandmarks = PoseConfig.DefaultConfig.SmoothLandmarks;
		// If set to true, in addition to the pose landmarks the solution also generates the segmentation mask.
		public bool EnableSegmentation = PoseConfig.DefaultConfig.EnableSegmentation;
		// If set to true, the solution filters segmentation masks across different input images to reduce jitter.
		// Ignored if enable_segmentation is false or static_image_mode is true.
		public bool SmoothSegmentation = PoseConfig.DefaultConfig.SmoothSegmentation;

		// Minimum confidence value ([0.0, 1.0]) from the person-detection model for the detection to be considered successful.
		[Range(0.0f, 1.0f)]
		public float MinDetectionConfidence = PoseConfig.DefaultConfig.MinDetectionConfidence;
		// Minimum confidence value ([0.0, 1.0]) from the landmark-tracking model for the pose landmarks to be considered tracked successfully,
		// or otherwise person detection will be invoked automatically on the next input image.
		// Setting it to a higher value can increase robustness of the solution, at the expense of a higher latency.
		// Ignored if static_image_mode is true.
		[Range(0.0f, 1.0f)]
		public float MinTrackingConfidence = PoseConfig.DefaultConfig.MinTrackingConfidence;
		[Min(0)]
		public long TimeoutMicroseconds = PoseConfig.DefaultConfig.TimeoutMicroseconds;

		public PoseConfig ToPoseConfig()
		{
			return new PoseConfig(
				StaticImageMode,
				SmoothLandmarks,
				EnableSegmentation,
				SmoothSegmentation,
				MinDetectionConfidence,
				MinTrackingConfidence,
				TimeoutMicroseconds
			);
		}
	}
}
