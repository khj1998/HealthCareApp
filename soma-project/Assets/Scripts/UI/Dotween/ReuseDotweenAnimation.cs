using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class ReuseDotweenAnimation : MonoBehaviour
{
    [SerializeField]
    private UnityEvent myEvent;
    // Start is called before the first frame update
    void OnEnable()
    {
        myEvent.Invoke();
    }
}
