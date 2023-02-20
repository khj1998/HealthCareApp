using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SOMA.UI
{
	/*public class DeckPreviewSwipeBtn : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
	{
		[SerializeField]
		private GameObject _deckPreview;
		private Vector3 _initialPosition;
		
		public void OnDrag(PointerEventData eventData)
		{
			transform.localPosition = new Vector2(transform.localPosition.x + eventData.delta.x, transform.localPosition.y);
			if (transform.localPosition.x >= 1300)
			{
				ExerciseManager.card_name = DeckBtnManager.DeckName;
				ExerciseManager.Deck = ExerSet_CSV_Reader.ExerSet[ExerciseManager.card_name]; 
				ExerciseManager.stage = 1;
				transform.localPosition = _initialPosition;
				_deckPreview.SetActive(false);
			}
			else if (transform.localPosition.x <= -400)
			{
				transform.localPosition = _initialPosition;
				_deckPreview.SetActive(false);
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			_initialPosition = transform.localPosition;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			transform.localPosition = _initialPosition;
		}
	}*/
}
