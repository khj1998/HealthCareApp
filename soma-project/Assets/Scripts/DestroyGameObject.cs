using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObject : MonoBehaviour
{
    [SerializeField]
    private float waitTime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,waitTime);
    }
}
