using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SOMA
{
    
    public class ModelData : MonoBehaviourSingleton<ModelData>
    {
        protected override void WhenAwake() {}
        
        public int selectedModel = 0; //내가 사용하는 모델 번호
        public int[] unlockModelList = {1,0}; //1 사용가능, 0 사용 불가능
        public int[] modelPrices = {0,3000}; //모델 가격 임시용
        //위 두 변수는 씬 곳곳에 있는 ModelGenerator가 접근해서 사용함

        protected override void WhenDestroy() {}
    }
}