using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    
    public void SetMaxHealth(int Health)
    {
        slider.maxValue = Health;
        slider.value = Health;

        fill.color = gradient.Evaluate(1f);
    }
    public IEnumerator SetHealth(int Health) //현재체력
    {
        while (slider.value > Health) {
            if(slider.value <= 0)
            {
                yield break;
            }

            var value = Mathf.Max(slider.value - slider.maxValue/40, Health);
            slider.value = value;
            fill.color = gradient.Evaluate(slider.normalizedValue);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
