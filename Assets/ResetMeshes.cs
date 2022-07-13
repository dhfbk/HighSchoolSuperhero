using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ResetMeshes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!transform.name.Contains("(Clone)"))
        {
            GameObject newObj = Instantiate(Resources.Load<GameObject>("Aiuola"));
            newObj.transform.parent = this.gameObject.transform.parent;
            newObj.transform.localRotation = this.gameObject.transform.localRotation;
            newObj.transform.position = this.gameObject.transform.position;
            newObj.transform.localScale = this.transform.localScale;
            Destroy(this.gameObject);
        }
    }
}
