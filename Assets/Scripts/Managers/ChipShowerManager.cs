using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ChipShowerManager : MonoBehaviour
{
    public static ChipShowerManager Instance;
    [SerializeField] List<RectTransform> chipGOs;
    [SerializeField] List<RectTransform> cardGOs;
    void Awake()
    {
        Instance = this;
    }


    public IEnumerator FlyCoinAndCard(RectTransform start, RectTransform target, bool isCoin = true)
    {
        if (isCoin)
            yield return new WaitForSeconds(2f);
        List<RectTransform> pool = isCoin ? chipGOs : cardGOs;
        if (pool == null || pool.Count == 0)
            yield break;

        RectTransform flying = pool[0];
        pool.RemoveAt(0);
        flying.position = start.position;
        flying.gameObject.SetActive(true);
        float t = 0f;
        float duration = 1f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            flying.position = Vector3.Lerp(start.position, target.position, EaseOut(t));
            yield return null;
        }

        pool.Add(flying);
        flying.gameObject.SetActive(false);
    }
    float EaseOut(float t)
    {
        return 1 - Mathf.Pow(1 - t, 3);

    }
}
