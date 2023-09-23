using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 off_set = new Vector3(0f, 0f, -10f); 
    private float smooth_time = 0.25f;
    private Vector3 velocty = Vector3.zero;

    [SerializeField] private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target_position = target.position + off_set;
        transform.position = Vector3.SmoothDamp(transform.position, target_position, ref velocty, smooth_time);
    }
}
