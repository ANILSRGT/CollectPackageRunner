using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LevelProperties
{
    public int levelID;
    public int requiredCollectableObjCount;
    public Transform cameraTransform;
    public Transform playerTransform;
}

public class LevelManager : MonoBehaviour
{
    public LevelProperties levelProperty;
}
