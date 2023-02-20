using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace SOMA.UI
{
	public class DeckCategoryToggle : MonoBehaviour
	{
		[SerializeField]
		private Image image;

		[SerializeField]
		private Toggle toggle;

		[SerializeField]
		private DOTweenAnimation anim;

		[SerializeField]
		private Color selectedColor = Color.black;

		[SerializeField]
		private Color deselectedColor = Color.white;

		private float toggleOnTime = 0.0f;

		private void Update()
		{
			if (toggle.isOn)
			{
				toggleOnTime = Mathf.Clamp(toggleOnTime + Time.deltaTime, 0.0f, 0.5f);
				image.color = Color.Lerp(image.color, selectedColor, toggleOnTime * 2);
			}
			else
			{
				toggleOnTime = Mathf.Clamp(toggleOnTime - Time.deltaTime, 0.0f, 0.5f);
				image.color = Color.Lerp(deselectedColor, image.color, toggleOnTime * 2);
			}
		}

		public void RestartAnimation(bool doRestart)
		{
			if (doRestart)
			{
				anim.DORestart();
			}
		}
	}
}
