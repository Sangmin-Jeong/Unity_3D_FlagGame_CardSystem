using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextGlow : MonoBehaviour
{
    [SerializeField] private float maxAlpha;
    [SerializeField] private float minAlpha;

    private TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        text.color = new Color(1, 1, 1, maxAlpha);
        StartCoroutine(FadeInText());
    }

    IEnumerator FadeInText()
    {
        while (text.color.a < maxAlpha)
        {
            yield return new WaitForSeconds(0.01f);
            text.color = new Color(text.color.r, text.color.b, text.color.g, text.color.a + Time.deltaTime * 2.0f);
        }
        StopCoroutine(FadeInText());
        StartCoroutine(FadeOutText());
        yield return null;
    }

    IEnumerator FadeOutText()
    {
        while (text.color.a > minAlpha)
        {
            yield return new WaitForSeconds(0.01f);
            text.color = new Color(text.color.r, text.color.b, text.color.g, text.color.a - Time.deltaTime * 2.0f);
        }
        StopCoroutine(FadeOutText());
        StartCoroutine(FadeInText());
        yield return null;
    }
}
