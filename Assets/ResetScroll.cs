using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    private Vector3 initialPos;

    private void Start()
    {
        initialPos = transform.position;
    }

    public void Reset()
    {
        if(transform.position.y > 1000 || transform.position.y < -400) StartCoroutine(MoveFunction());

    }

    private IEnumerator MoveFunction()
    {
        var timeSinceStarted = 0f;
        while (true)
        {
            timeSinceStarted += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, initialPos, timeSinceStarted);

            if (transform.position == initialPos)
            {
                yield break;

            }

            yield return null;
        }
    }


}
