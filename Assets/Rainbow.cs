using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rainbow : MonoBehaviour
{
    public Image image;
    public bool rainbow;
    float t;
    public Color[] destColors;
    private int ind;
    public GameObject infinity;
    public Color originalColor;
    public Vector3 newSize;
    private Vector3 originalSize;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rainbow)
        {
            t += Time.deltaTime * 3;
            image.color = Color.Lerp(image.color, destColors[ind], t);
            if (t >= 1)
            {
                t = 0;
                ind += 1;
                if (ind == destColors.Length)
                    ind = 0;
            }
        }
    }

    public void StartRainbow()
    {
        transform.localScale = newSize;
        image.color = new Color(1, 1, 0, 1);
        rainbow = true;
        infinity.SetActive(true);
    }

    public void StopRainbow()
    {
        if (GetComponent<SoapBar>())
            GetComponent<SoapBar>().UpdateBar();
        image.color = originalColor;
        infinity.SetActive(false);
        rainbow = false;
    }
}
