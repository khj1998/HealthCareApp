using UnityEngine;
using Mediapipe.Unity;

namespace SOMA.MediaPipe
{
	public struct PoseImageConfig
	{
		public int InputRotation;
		public bool InputHorizontallyFlipped;
		public bool InputVerticallyFlipped;

		public int OutputRotation;
		public bool OutputHorizontallyFlipped;
		public bool OutputVerticallyFlipped;

		public PoseImageConfig(
				int inputRotation,
				bool inputHorizontallyFlipped,
				bool inputVerticallyFlipped,
				int outputRotation,
				bool outputHorizontallyFlipped,
				bool outputVerticallyFlipped
			) : this()
		{
			this.InputRotation = inputRotation;
			this.InputHorizontallyFlipped = inputHorizontallyFlipped;
			this.InputVerticallyFlipped = inputVerticallyFlipped;

			this.OutputRotation = outputRotation;
			this.OutputHorizontallyFlipped = outputHorizontallyFlipped;
			this.OutputVerticallyFlipped = outputVerticallyFlipped;
		}

		public PoseImageConfig(WebCamTexture webCamTexture, bool isInputMirrored = false) : this()
		{
			// NOTE: The origin is left-bottom corner in Unity, and right-top corner in MediaPipe.
			int rotation = webCamTexture.videoRotationAngle;
			bool isInverted = rotation.IsInverted();

			InputRotation = rotation.Reverse();
			InputHorizontallyFlipped = isInverted ^ isInputMirrored;
			InputVerticallyFlipped = !isInverted;

			if ((InputHorizontallyFlipped && InputVerticallyFlipped) || InputRotation == 180)
			{
				InputRotation = (InputRotation + 180) % 360;
				InputHorizontallyFlipped = !InputHorizontallyFlipped;
				InputVerticallyFlipped = !InputVerticallyFlipped;
			}

			// The orientation of the output image must match that of the input image.
			OutputRotation = rotation;
			OutputHorizontallyFlipped = false;
			OutputVerticallyFlipped = webCamTexture.videoVerticallyMirrored;

			if ((OutputHorizontallyFlipped && OutputVerticallyFlipped) || OutputRotation == 180)
			{
				OutputRotation = (OutputRotation + 180) % 360;
				OutputHorizontallyFlipped = !OutputHorizontallyFlipped;
				OutputVerticallyFlipped = !OutputVerticallyFlipped;
			}
		}

		// 멍청한 유니티가 C#10 지원 안해서 이렇게 해야함
		public static PoseImageConfig DefaultConfig => new PoseImageConfig(
			0,
			false,
			false,
			0,
			false,
			false
		);
	}

	public static class RotationExtension
	{
		public static int Reverse(this int rotation) => (360 - rotation) % 360;

		public static bool IsInverted(this int rotation) => rotation == 90 || rotation == 270;
	}
}
