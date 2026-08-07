using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField, Range(-500, 500)] private float rotationSpeed = 100f;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
