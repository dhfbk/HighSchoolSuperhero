using UnityEngine;

[ExecuteInEditMode]
public class HedgeTextureRemap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().sharedMaterial.mainTextureScale = new Vector2(transform.localScale.x, 1);
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Renderer>().sharedMaterial.mainTextureScale = new Vector2(transform.localScale.x, 1);
    }
}
