using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Battery : MonoBehaviour
{
    public Sprite[] sprites;
    public int charge;
    Vector3 normalScale = new Vector3(0.3f, 0.3f, 0.3f);
    // Start is called before the first frame update
    void Start()
    {
        //charge = 7;
        //normalScale = transform.localScale;
    }

    public void Change(int n)
    {
        charge += n;
        GetComponent<Image>().sprite = sprites[charge];
        GetComponent<Battery>().StopAllCoroutines();
        GetComponent<Battery>().StartCoroutine(BatteryWarning());
    }
    public void Set(int n)
    {
        charge = n;
        GetComponent<Image>().sprite = sprites[charge];
        GetComponent<Battery>().StopAllCoroutines();
        GetComponent<Battery>().StartCoroutine(BatteryWarning());
    }
    public int GetBattery()
    {
        return charge;
    }

    public IEnumerator BatteryWarning()
    {
        
        float t = 0;
        Vector3 startScale;
        Vector3 endScale;

        //Zoom in
        startScale = normalScale;
        endScale = startScale * 1.5f;
        while (t <= 1)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            t += Time.deltaTime*2;
            yield return null;
        }

        //Zoom out
        t = 0;
        startScale = transform.localScale;
        endScale = normalScale;
        while (t <= 1)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            t += Time.deltaTime*2;
            yield return null;
        }
    }
}
