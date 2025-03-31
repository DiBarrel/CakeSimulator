using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class СonveyorScripts : MonoBehaviour
{
    public float conveyorSpeed = 1.0f;
    public Vector3 direction = Vector3.forward;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Cake"))
        {
            Rigidbody cakeRB = other.GetComponent<Rigidbody>();
            if (cakeRB != null)
            {
                Vector3 movement = direction.normalized * conveyorSpeed * Time.fixedDeltaTime;
                cakeRB.MovePosition(cakeRB.position + movement);
            }
        }
    }
}
