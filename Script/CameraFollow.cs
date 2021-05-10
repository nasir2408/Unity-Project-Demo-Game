using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float Xmin;
    [SerializeField] private float Xmax;
    [SerializeField] private float Ymin;
    [SerializeField] private float Ymax;

    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player").transform;
    }

    void LateUpdate()
    {
        if(target.position.y > -14f)
        {
            transform.position = new Vector3(Mathf.Clamp(target.position.x, Xmin, Xmax), Mathf.Clamp(target.position.y, Ymin, Ymax), transform.position.z);
        }
    }
    
}
