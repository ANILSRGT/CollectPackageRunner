using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Vector3 deltaPos;

    [SerializeField] private Transform bag;

    private Animator animator;
    private Vector3 startTouchPosition = Vector3.zero;
    private bool isBarrier = false;
    private int collectedObjects = 0;

    private void Start()
    {
        GameManager.instance.isEndGame = false;
        animator = GetComponent<Animator>();
        animator.applyRootMotion = true;
        animator.SetFloat("WalkBlend", 1.0f);
    }

    private void Update()
    {
        if (GameManager.instance.isEndGame || isBarrier)
        {
            animator.SetFloat("WalkBlend", 0.0f);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 endTouchPosition = Input.mousePosition;
            float deltaX = endTouchPosition.x - startTouchPosition.x;

            if (deltaX > 0.4f)
            {
                transform.position += Vector3.right * 3f * Time.deltaTime;
            }
            else if (deltaX < -0.4f)
            {
                transform.position += Vector3.left * 3f * Time.deltaTime;
            }

            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1.75f, 1.75f), transform.position.y, transform.position.z);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            startTouchPosition = Vector3.zero;
        }
    }

    private void OnAnimatorMove()
    {
        if (GameManager.instance.isEndGame) return;
        transform.position += animator.deltaPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Collectable")
        {
            collectedObjects++;
            GameManager.instance.AddScore(1);
            other.tag = "Untagged";
            other.transform.SetParent(bag, false);
            other.transform.localScale = other.transform.localScale * 2f;
            other.transform.localPosition = new Vector3(0, 0.3f + 0.4f * collectedObjects, 0);
        }

        if (other.tag == "Finish")
        {
            GameManager.instance.FinishLevel();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Barrier")
        {
            collectedObjects--;
            Destroy(other.gameObject);
            animator.SetFloat("WalkBlend", 0.0f);
            GameObject lastBarrier = bag.transform.GetChild(bag.transform.childCount - 1).gameObject;
            lastBarrier.transform.SetParent(null, true);
            lastBarrier.GetComponent<Collider>().isTrigger = false;
            lastBarrier.GetComponent<Rigidbody>().useGravity = true;
            lastBarrier.GetComponent<Rigidbody>().AddForce(Vector3.left * 10f, ForceMode.Impulse);
            StartCoroutine(WalkWait());
        }
    }

    IEnumerator WalkWait()
    {
        isBarrier = true;
        yield return new WaitForSeconds(0.5f);
        animator.SetFloat("WalkBlend", 1.0f);
        isBarrier = false;
    }
}
