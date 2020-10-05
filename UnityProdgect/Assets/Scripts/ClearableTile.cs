using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ClearableTile : MonoBehaviour
{
    public float timeClear;
    private bool isBeingCleared = false;

    public bool IsBeingCleared
    {
        get { return isBeingCleared; }
    }

    protected GameTitle title;

    void Awake()
    {
        title = GetComponent<GameTitle>();
    }

    public void Clear()
    {
        isBeingCleared = true;
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine()
    {
        transform.DOScale(1.5f, timeClear);

        yield return new WaitForSeconds(timeClear);
        Debug.Log("DEDD");
        Destroy(gameObject);
    }
}