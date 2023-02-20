using UnityEngine;
using SOMA.Managers;

namespace SOMA.Model
{
	// 해당 스크립트에서 처음에 서버로부터 받아온 블렌더 계수를 캐릭터에 적용한다. 운동 세트후 외형이 바뀌면 바뀐 계수로 바로 업데이트.
	public class BlendShape : MonoBehaviour
	{
		[SerializeField]
		private SkinnedMeshRenderer _skinnedMeshRenderer;
		private const int _overlapErrorPrevention = 25;

		private void Start() 
		{
			Debug.Log(DataManager.Instance.UserBlenderTable.UpperBlender+" "+DataManager.Instance.UserBlenderTable.CoreBlender+" "+DataManager.Instance.UserBlenderTable.LowerBlender);
			_skinnedMeshRenderer.SetBlendShapeWeight(0, 100 - Mathf.Clamp(DataManager.Instance.UserBlenderTable.UpperBlender+_overlapErrorPrevention,0,100)); // upper
			_skinnedMeshRenderer.SetBlendShapeWeight(1, 100 - Mathf.Clamp(DataManager.Instance.UserBlenderTable.CoreBlender+_overlapErrorPrevention,0,100)); // core
			_skinnedMeshRenderer.SetBlendShapeWeight(2, 100 - Mathf.Clamp(DataManager.Instance.UserBlenderTable.LowerBlender+_overlapErrorPrevention,0,100)); // lower
		}
	}
}

