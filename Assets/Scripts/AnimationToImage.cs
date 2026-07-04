using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnimationToImage : MonoBehaviour
{
    public Image img;
    public Sprite[] frames;
    public float fps = 10f;
    public bool isLoop;
    private int index;
    private float timer;

    private void Awake()
    {
        StartCoroutine(AnimationStart());
    }

    IEnumerator AnimationStart()
    {
        while (isLoop || index != frames.Length - 1) {
            timer += Time.deltaTime;
            if (timer >= 1f / fps) {
                index = (index + 1) % frames.Length;
                img.sprite = frames[index];
                timer = 0f;
            }

            yield return null;
        }
    }

    public float GetAnimationLength()
    {
        return 1f / fps * frames.Length;
    }
}
