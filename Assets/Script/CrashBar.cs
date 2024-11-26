using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrashBar : MonoBehaviour
{
    [SerializeField] private PlayerSystem playerSystem;
    [SerializeField] private Image bar;
    [SerializeField] private TextMeshProUGUI countText;
    private float animaitonTime = 0.5f;

    public Coroutine CrashBarRoutine;

    private void Update()
    {
        if (playerSystem.totalBreak > 0)
        {
            countText.text = playerSystem.totalBreak.ToString();

            if (CrashBarRoutine != null)
            {
                StopCoroutine(CrashBarRoutine);
            }

            CrashBarRoutine = StartCoroutine(widthChanger(playerSystem.totalBreak * 30));
        }
        else
        {
            if (CrashBarRoutine != null)
            {
                StopCoroutine(CrashBarRoutine);
            }
            countText.text = playerSystem.totalBreak.ToString();
            CrashBarRoutine = StartCoroutine(widthChanger(0));
        }
    }

    public IEnumerator widthChanger(float size)
    {
        float currentWidth = bar.rectTransform.rect.width;
        float time = 0f;

        while(time < animaitonTime)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time/animaitonTime);
            float nowWidth = Mathf.Lerp(currentWidth, size, t);
            bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nowWidth);
            yield return null;
        }

        bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
        CrashBarRoutine = null;
    }
}