using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform spawnCollectableTransform;
    [SerializeField] private Transform bag;
    [SerializeField] private float swipeSpeed;

    private AnimController _animController;
    private float _startTouchPositionX = 0;
    private int _collectedObjects = 0;

    private void Awake()
    {
        _animController = GetComponent<AnimController>();

        Events.OnStartGame.AddListener(OnStartGame);
        Events.OnEndGame.AddListener(OnEndGame);
    }

    private void OnDestroy()
    {
        Events.OnStartGame.RemoveListener(OnStartGame);
        Events.OnEndGame.RemoveListener(OnEndGame);
    }

    private void OnStartGame()
    {
        _animController.SetState(PlayerState.Run);
    }

    private void Update()
    {
        if (GameManager.Instance.gameState != GameState.Play || _animController.playerState != PlayerState.Run) return;

        CheckInput();
        Moving();
    }

    private void OnEndGame()
    {
        _animController.SetState(PlayerState.Idle);
    }

    private void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _startTouchPositionX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 endTouchPosition = Input.mousePosition;
            float deltaX = endTouchPosition.x - _startTouchPositionX;
            _startTouchPositionX = Input.mousePosition.x;

            transform.position += Vector3.right * deltaX * swipeSpeed * Time.deltaTime;

            // if (deltaX > 0.4f)
            // {
            //     transform.position += Vector3.right * swipeSpeed * Time.deltaTime;
            // }
            // else if (deltaX < -0.4f)
            // {
            //     transform.position += Vector3.left * swipeSpeed * Time.deltaTime;
            // }

            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1.75f, 1.75f), transform.position.y, transform.position.z);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _startTouchPositionX = 0f;
        }
    }

    private void Moving()
    {
        transform.position += _animController.deltaPos;
    }

    public void OnStandUpFromDash()
    {
        _animController.SetState(PlayerState.Run);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Collectable")
        {
            _collectedObjects++;
            GameManager.Instance.AddScore(1);
            other.tag = "Untagged";
            var tempScale = other.transform.lossyScale;
            tempScale.x /= bag.localScale.x;
            tempScale.y /= bag.localScale.y;
            tempScale.z /= bag.localScale.z;
            other.transform.SetParent(bag, true);
            other.transform.localScale = tempScale;
            other.transform.localRotation = Quaternion.Euler(Vector3.zero);
            other.transform.localPosition = new Vector3(0, (spawnCollectableTransform.localScale.y / 2 + other.transform.localScale.y / 2) + other.transform.localScale.y * (_collectedObjects - 1), 0);
        }

        if (other.tag == "Finish")
        {
            GameManager.Instance.FinishLevel();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Barrier" && _animController.playerState == PlayerState.Run)
        {
            _collectedObjects--;
            Destroy(other.gameObject);
            _animController.SetState(PlayerState.Dash);
            if (_collectedObjects < 0)
            {
                GameManager.Instance.FinishLevel();
            }
            else
            {
                GameObject lastBarrier = bag.transform.GetChild(bag.transform.childCount - 1).gameObject;
                lastBarrier.transform.SetParent(null, true);
                lastBarrier.GetComponent<Collider>().isTrigger = false;
                lastBarrier.GetComponent<Rigidbody>().isKinematic = false;
                lastBarrier.GetComponent<Rigidbody>().useGravity = true;
                lastBarrier.GetComponent<Rigidbody>().AddForce(Vector3.left * 10f, ForceMode.Impulse);
            }
        }
    }
}
