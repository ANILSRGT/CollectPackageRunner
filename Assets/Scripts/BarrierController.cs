using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public enum BarrierType { Normal, Rollable }

public class BarrierController : MonoBehaviour
{
    [SerializeField] private BarrierType barrierType = BarrierType.Normal;

    private PlayerController _playerController;

    private void Awake()
    {
        _playerController = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (GameManager.Instance.gameState != GameState.Play) return;

        if (barrierType == BarrierType.Rollable)
        {
            CheckRoll();
        }
        else if (barrierType == BarrierType.Normal)
        {
            // Normal BARRIER controller
        }
    }

    /// <summary>
    ///     Check barrie type and play the barrier
    /// </summary>
    private void CheckRoll()
    {
        foreach (Barrier item in transform.GetComponentsInChildren<Barrier>().Where(x => x.barrierType == BarrierType.Rollable && !x.isPlaying))
        {
            if (Vector3.Distance(_playerController.transform.position, item.transform.position) < 10f)
            {
                item.PlayRollableBarrier();
            }
        }
    }
}
