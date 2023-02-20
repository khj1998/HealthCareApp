using System;
using UnityEngine;
using Mediapipe;

namespace SOMA.MediaPipe
{
	public static class Texture2DExtension
	{
		[AOT.MonoPInvokeCallback(typeof(GlTextureBuffer.DeletionCallback))]
		private static void OnReleaseTexture(uint textureName, IntPtr syncTokenPtr)
		{
			if (syncTokenPtr == IntPtr.Zero)
			{
				return;
			}

			// using declaration, auto dispose
			using var glSyncToken = new GlSyncPoint(syncTokenPtr);

			// Waits until the GPU has executed all commands up to the sync point.
			// This blocks the CPU, and ensures the commands are complete from the
			// point of view of all threads and contexts.
			glSyncToken.Wait();

			// Ensures that the following commands on the current OpenGL context will
			// not be executed until the sync point has been reached.
			// This does not block the CPU, and only affects the current OpenGL context.
			glSyncToken.WaitOnGpu();
		}

		#if UNITY_ANDROID
		public static GpuBuffer ToGpuBuffer(this Texture2D texture, GlContext glContext)
		{
			var glTextureBuffer = new GlTextureBuffer((uint)texture.GetNativeTexturePtr(), texture.width, texture.height, GpuBufferFormat.kBGRA32, OnReleaseTexture, glContext);
			return new GpuBuffer(glTextureBuffer); 
		}
		#endif

		public static ImageFrame ToImageFrame(this Texture2D texture) => new ImageFrame(texture.format.ToImageFormat(), texture.width, texture.height, texture.width * 4, texture.GetRawTextureData<byte>());
	}
}
