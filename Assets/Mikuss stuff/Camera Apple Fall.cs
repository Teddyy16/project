using UnityEngine;

public class CameraAppleFall : MonoBehaviour
{
    public float _delay = 1f;
    public GameObject Apple_Final;
    public Rigidbody rb;


    void Start()
    {

        InvokeRepeating("Spawn", _delay, _delay);
        rb = GetComponent<Rigidbody>();


    }

    void Spawn()
    {
        Instantiate(Apple_Final, new Vector3(Random.Range(-1, 2), 7, 0), Quaternion.identity);
    }
}
