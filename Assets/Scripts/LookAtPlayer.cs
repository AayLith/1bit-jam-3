using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    // Update is called once per frame
    void Update ()
    {
        float x = transform.rotation.eulerAngles.x;
        float z = transform.rotation.eulerAngles.z;
        transform.LookAt ( Player.instance.transform.position,Vector3.up );
        //transform.rotation = Quaternion.Euler ( x , transform.rotation.eulerAngles.z , z );
    }
}
