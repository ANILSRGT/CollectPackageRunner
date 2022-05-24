using UnityEngine;

public class Barrier : MonoBehaviour
{
    public BarrierType barrierType = BarrierType.Rollable;
    public float rollSpeed = 1.0f;
    [HideInInspector] public Rigidbody rb;
    public bool isPlaying = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void PlayRollableBarrier()
    {
        isPlaying = true;
        rb.velocity = Vector3.back * rollSpeed;
    }

    private void StopRollableBarrier() => rb.velocity = Vector3.zero;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StopRollableBarrier();
            isPlaying = false;
        }
    }
}