using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ModelScreenWidth : MonoBehaviour
{
    [SerializeField]
    private GameObject modelGenerator;
    // Start is called before the first frame update
    void Start()
    {
        modelGenerator.transform.position = new Vector3( -1.6f * Screen.width / 1000, -3, 100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
