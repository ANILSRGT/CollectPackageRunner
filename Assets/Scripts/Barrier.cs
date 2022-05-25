using UnityEngine;

public class Barrier : MonoBehaviour
{
    public BarrierType barrierType = BarrierType.Rollable;
    public float rollSpeed;
    [HideInInspector] public Rigidbody rb;
    public bool isPlaying = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        isPlaying = false;
    }

    /// <summary>
    ///     Play the rollable barrier
    /// </summary>
    public void PlayRollableBarrier()
    {
        isPlaying = true;
        rb.velocity = Vector3.back * rollSpeed;
    }

    /// <summary>
    ///     Stop the rollable barrier
    /// </summary>
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