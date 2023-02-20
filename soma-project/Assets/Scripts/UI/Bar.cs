using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SOMA;
namespace SOMA.UI
{
	// 시간 게이지 바
	public class Bar : MonoBehaviour
	{
		[SerializeField]
		private RectTransform _barPos;
		private RectTransform Rect;
		public float gauge;
		public float maxGauge;
		int screenWidth;
		
		private void Awake()
		{
			screenWidth = Screen.width;
			Rect = GetComponent<RectTransform>();
			//_barPos.sizeDelta = new Vector2(0, _barPos.sizeDelta.y);
			_barPos.sizeDelta = new Vector2(0, _barPos.rect.height);
		}

		private void Update()
		{
			//_barPos.sizeDelta = new Vector2( 64 + ((screenWidth + rect.sizeDelta.x - 64) * gauge) / maxGauge, _barPos.sizeDelta.y);
			_barPos.sizeDelta = new Vector2( 64 + ((Rect.rect.width - 64) * gauge) / maxGauge, _barPos.rect.height);

		}
	}
}
