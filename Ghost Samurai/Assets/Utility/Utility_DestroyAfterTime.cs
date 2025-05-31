using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility_DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float timeUntilDestroy = 5f;
    private void Awake()
    {
        Destroy(gameObject, timeUntilDestroy);
    }
}
