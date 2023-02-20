using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using SOMA;
using SOMA.UI;
using SOMA.Managers;

namespace SOMA.UI
{
	public class TotalExp : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _totalExp;
	
	    [SerializeField]
	    private Bar _totalGauge;
		/*[SerializeField]
		//private Image _upperGauge;

		[SerializeField]
		//private Image _coreGauge;

		[SerializeField]
		//private Image _lowerGauge;*/

		[SerializeField]
		private TextMeshProUGUI _upperExp;

		[SerializeField]
		private TextMeshProUGUI _coreExp;

		[SerializeField]
		private TextMeshProUGUI _lowerExp;

		private int upperExp;
		private int coreExp;
		private int lowerExp;

		/*[SerializeField]
		private ScrollRect _mainScrollView;*/

		private int _exp = 0;

		private void Awake() 
		{
			_totalGauge.maxGauge = 600;
		}

		void Update() 
		{
			ShowTotalExp();
			ShowDetailExp();
			_totalGauge.gauge = _exp;
		}

		void ShowTotalExp()
		{
			upperExp = DataManager.Instance.UserInfosTable.Upper ;
			coreExp = DataManager.Instance.UserInfosTable.Core ;
			lowerExp = DataManager.Instance.UserInfosTable.Lower ;
			_exp = (upperExp + coreExp + lowerExp);
			_totalExp.text =  _exp.ToString(); // 총 경험치 UI 
		}

		void ShowDetailExp()
		{
			_upperExp.text =  $"상체 {upperExp}"; // 상체 경험치량
			_coreExp.text = $"코어 {coreExp}"; // 코어 경험치량
			_lowerExp.text = $"하체 {lowerExp}"; // 하체 경험치량.
			
			//_upperGauge.fillAmount = upperExp / 100f; // 각 부위별 경험치량에 따라 게이지바가 채워져야한다.
			//_coreGauge.fillAmount = coreExp / 100f;
			//_lowerGauge.fillAmount = lowerExp / 100f;

			//각 부위별로 근손실 난 곳은 게이지 빨간색 표시 + 점수 옆에 근손실 문자열 박아주기
			if (DataManager.Instance.LowerLoss == true) // 운동해서 경험치가 피드되었다면 lower_loss값은 false로 변경 
			{
				_lowerExp.color = Color.red;
			}
			else
			{
				_lowerExp.color = Color.white;
			}

			if (DataManager.Instance.UpperLoss==true) 
			{
				_upperExp.color = Color.red;
			}
			else
			{
				_upperExp.color = Color.white;
			}
				
			if (DataManager.Instance.CoreLoss==true)
			{
				_coreExp.color = Color.red;
			}
			else
			{
				_coreExp.color = Color.white;
			}
		}
	}
}
