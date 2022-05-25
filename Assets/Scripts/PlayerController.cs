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
    private bool isDeath = false;

    private void Awake()
    {
        isDeath = false;
        _animController = GetComponent<AnimController>();
        _startTouchPositionX = 0;
        _collectedObjects = 0;

        Events.OnStartGame.AddListener(OnStartGame);
    }

    /// <summary>
    /// On destroy event
    /// </summary>
    private void OnDestroy()
    {
        Events.OnStartGame.RemoveListener(OnStartGame);
    }

    /// <summary>
    /// On start game event
    /// </summary>
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

    /// <summary>
    /// Check input and player control
    /// </summary>
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

            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1.75f, 1.75f), transform.position.y, transform.position.z);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _startTouchPositionX = 0f;
        }
    }

    /// <summary>
    /// Moving player
    /// </summary>
    private void Moving()
    {
        transform.position += _animController.deltaPos;
    }

    /// <summary>
    /// At the end of Dash animation, the OnStandUpFromDash event is triggered.
    /// </summary>
    public void OnStandUpFromDash()
    {
        if (isDeath) return;
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
            bool isWin = GameManager.Instance.FinishLevel();
            if (isWin) _animController.SetState(PlayerState.Victory);
            else _animController.SetState(PlayerState.Lose);
        }
    }

    /// <summary>
    /// Waiting dash animation for lose animation.
    /// </summary>
    private IEnumerator LoseGameWait()
    {
        isDeath = true;
        yield return new WaitForSeconds(_animController.currentAnimTime);
        _animController.SetState(PlayerState.Lose);
        GameManager.Instance.FinishLevel();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Barrier" && _animController.playerState == PlayerState.Run)
        {
            _collectedObjects--;
            GameManager.Instance.AddScore(-1);
            Destroy(other.gameObject);
            _animController.SetState(PlayerState.Dash);
            if (_collectedObjects < 0)
            {
                StartCoroutine(LoseGameWait());
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
