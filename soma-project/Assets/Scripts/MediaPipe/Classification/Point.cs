using UnityEngine;
using Mediapipe;

namespace SOMA.MediaPipe.Classification
{
	// Immutable Point
	public readonly struct Point
	{
		public readonly float X;
		public readonly float Y;
		public readonly float Z;

		public Point(float X, float Y, float Z)
		{
			this.X = X;
			this.Y = Y;
			this.Z = Z;
		}

		public Point(NormalizedLandmark landmark)
		{
			this.X = landmark.X;
			this.Y = landmark.Y;
			this.Z = landmark.Z;
		}

		public Vector3 ToVector3() => new Vector3(this.X, this.Y, this.Z);
		public Vector2 ToVector2() => new Vector2(this.X, this.Y);

		public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		public static Point operator -(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		public static Point operator *(Point a, float k) => new Point(a.X * k, a.Y * k, a.Z * k);
		public static Point operator *(Point a, Point b) => new Point(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

		public static Point Average(Point a, Point b) => new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2, (a.Z + b.Z) / 2);
		public static float L2Norm2D(Point point) => Mathf.Sqrt(point.X * point.X + point.Y * point.Y);
		public static float MaxAbs(Point point) => Mathf.Max(Mathf.Abs(point.X), Mathf.Abs(point.Y), Mathf.Abs(point.Z));
		public static float SumAbs(Point point) => Mathf.Abs(point.X) + Mathf.Abs(point.Y) + Mathf.Abs(point.Z);

		public static float Angle2D(Point s, Point m, Point e)
		{
			Vector2 x = (s - m).ToVector2();
			Vector2 y = (e - m).ToVector2();
			return Vector2.Angle(x, y);
		}

		public static float Angle3D(Point s, Point m, Point e)
		{
			Vector3 x = (s - m).ToVector3();
			Vector3 y = (e - m).ToVector3();
			return Vector3.Angle(x, y);
		}
	}
}
