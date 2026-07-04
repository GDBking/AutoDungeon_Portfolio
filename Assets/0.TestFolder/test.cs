using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public List<GameObject> list;

    private void Awake()
    {
        Destroy(list[0]);
        StartCoroutine(Routine());
    }

    IEnumerator Routine()
    {
        yield return null;
        print(list.Count);
    }
}