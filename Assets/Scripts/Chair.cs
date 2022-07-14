using UnityEngine;

public class Interact : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Chair()
    {
        Control.Instance.player.transform.position = transform.position;
        Control.Instance.player.transform.LookAt(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z));
    }
}
