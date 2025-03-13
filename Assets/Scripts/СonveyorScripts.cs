using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class СonveyorScripts : MonoBehaviour
{
    public float conveyorSpeed = 1.0f;
    public Vector3 diraction = Vector3.forward;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Cake")) // проверяем тэг объекта на конвеере
        {
            Rigidbody cakeRb = other.GetComponent<Rigidbody>();
            if (cakeRb != null) //если что-то есть, задаем движение объекту
            {
                Vector3 movement = diraction.normalized * conveyorSpeed * Time.fixedDeltaTime;
                cakeRb.MovePosition(cakeRb.position + movement);
            }
        }
    }

}
