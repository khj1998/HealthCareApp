using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SOMA.Model
{
	public class ModelRotater : MonoBehaviour, IDragHandler
	{
		[SerializeField]
		private float _rotateSpeed;
		[SerializeField]
		private GameObject _model; //위아래 회전
		[SerializeField]
		private GameObject _modelChild; //좌우 회전

		void Start()
		{
			_modelChild = _model.transform.GetChild(0).gameObject;
		}

		public void OnDrag(PointerEventData eventData)
		{
			float x = eventData.delta.x * Time.deltaTime * _rotateSpeed;
			float y = eventData.delta.y * Time.deltaTime * _rotateSpeed;
	
			_modelChild.transform.Rotate(0, -x, 0,Space.Self);
			_model.transform.Rotate(y, 0, 0,Space.Self);
			if (_model.transform.eulerAngles.x > 0 && _model.transform.eulerAngles.x < 50)
			{
				_model.transform.rotation = Quaternion.Euler(0, _model.transform.eulerAngles.y, _model.transform.eulerAngles.z);
			}
			if (_model.transform.eulerAngles.x < 300 && _model.transform.eulerAngles.x > 250)
			{
				_model.transform.rotation = Quaternion.Euler(300, _model.transform.eulerAngles.y, _model.transform.eulerAngles.z);
			}
			Debug.Log(_model.transform.eulerAngles.x);
		}
	}
}

