namespace SOMA
{
	public static class MoreMath
	{
		public static float Square(float x) => x * x;

		public static (float width, float height) FitScreenSize((float width, float height) target, (float width, float height) screen)
		{
			float widthWhenMatchedToHeight = target.width / target.height * screen.height;
			float heightWhenMatchedToWidth = target.height / target.width * screen.width;

			if (widthWhenMatchedToHeight < screen.width)
			{
				return (widthWhenMatchedToHeight, screen.height);
			}
			else
			{
				return (screen.width, heightWhenMatchedToWidth);
			}
		}
	}
}
