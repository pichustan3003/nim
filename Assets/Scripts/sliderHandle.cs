using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class sliderHandle : MonoBehaviour
{

    public Slider[] sliders;

    void Start()
    {
        foreach (Slider slider in sliders)
        {
            slider.onValueChanged.AddListener(value => OnSliderChanged(slider, value));
        }
    }

    void OnSliderChanged(Slider slider, float value)
    {
        slider.gameObject.GetComponentInChildren<TMP_Text>().text = value.ToString();
    }
}
