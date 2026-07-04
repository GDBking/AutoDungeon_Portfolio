using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MapUi : MonoBehaviour
{
    public AudioClip mapBgm;
    public GameObject scorll;
    public GameObject BTN;
    public Text OnOffText;

    private void OnEnable()
    {
        AudioManager.instance.PlayBgm(mapBgm);

        StartCoroutine(ExampleCoroutine());   
    }

    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(0.1f);

        if (GameManager.instance.currentLevel >= 2)
        {
            BTN.SetActive(true);
        }
    }

    public void ONOFF()
    {
        scorll.SetActive(!scorll.activeSelf);
        if (scorll.activeSelf)
        {
            OnOffText.text = "OFF";
        }
        else
        {
            OnOffText.text = "ON";
        }
    }
}