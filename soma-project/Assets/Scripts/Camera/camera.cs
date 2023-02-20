using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    [SerializeField]
    private GameObject model;

    [SerializeField]
    private float plusYPos;
    // Start is called before the first frame update
    void Start()
    {
        model = GameObject.FindWithTag("spine");
        //캐릭터 모델의 척추 리깅에 spine 태그를 달아놓음
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(model.transform.position.x,plusYPos, -30), 0.05f);  
    }
}
