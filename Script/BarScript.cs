using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarScript : MonoBehaviour
{

    private float fillAmount;

    [SerializeField] private float lerpSpeed;

    [SerializeField] private Text valueText;

    [SerializeField] private Image content;

    [SerializeField] private Color fullColors;

    [SerializeField] private Color lowColors;

    [SerializeField] private bool lerpColors;

    public float MaxValue { get; set; }

    public float Value
    {
        set
        {
            string[] tmp = valueText.text.Split(':');
            valueText.text = tmp[0] + ": " + value;
            fillAmount = Map(value, MaxValue);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(lerpColors)
        {
            content.color = fullColors;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleBar();
    }

    private void HandleBar()
    {
        if(fillAmount != content.fillAmount)
        {
            content.fillAmount = Mathf.Lerp(content.fillAmount, fillAmount, Time.deltaTime * lerpSpeed);
        }

        if(lerpColors)
        {
            content.color = Color.Lerp(lowColors, fullColors, fillAmount);
        }
    }

    private float Map(float value, float inMax)
    {
        return value / inMax;
       // return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
