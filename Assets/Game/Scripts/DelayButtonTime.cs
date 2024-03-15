using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayButtonTime : MonoBehaviour
{
    public GameObject buttons;

    void Awake()
    {
        buttons.SetActive(false);

        StartCoroutine(MakeButtonsVisible());

    }

    IEnumerator MakeButtonsVisible()
    {
        yield return new WaitForSeconds(10);

        buttons.SetActive(true);
    }


}
