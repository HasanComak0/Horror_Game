using System.Net.NetworkInformation;
using UnityEngine;

public class TurningLight : MonoBehaviour
{
    public float speed;
    public Vector3 turn;
    void Update()
    {
        //transform.localRotation = Quaternion.Euler(new Vector3(1, 0, 0)*speed*Time.deltaTime);
        transform.Rotate(turn * speed * Time.deltaTime);
    }
}
