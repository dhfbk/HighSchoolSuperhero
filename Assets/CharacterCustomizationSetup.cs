using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.EventSystems;

public class CharacterCustomizationSetup : MonoBehaviour 
{
    public static PostProcessVolume vol;
    public static DepthOfField dof;

	// Use this for initialization
	void Start ()
    {

    }

    public void Setup()
    {
        GetComponent<Movement>().Busy = true;
        //transform.gameObject.layer = 5;
        //foreach (Transform child in transform)
        //{
        //    child.gameObject.layer = 5;
        //}

        if (!vol)
            vol = FindObjectOfType<PostProcessVolume>();

        //Setto sfocatura
        //Blur();

        //Imposto posizioni
        PlaceCamAndPlayer();

        //Attivo funzionalità del canvas
        GetComponent<Player>().cameraInterface.editorCanvas.gameObject.GetComponent<CharacterCustomization>().enabled = true;
        //transform.gameObject.layer = 5;

        //Attivo customization screen
        GetComponent<Player>().cameraInterface.editorCanvas.gameObject.SetActive(true);
    }
	public static void Blur()
    {
        vol.profile.TryGetSettings(out dof);
        dof.enabled.value = true;
    }

    void PlaceCamAndPlayer() //Begin customization
    {
        Camera.main.transform.GetChild(0).GetComponent<Camera>().orthographic = false;
        transform.parent = Camera.main.transform;
        Camera.main.transform.position = new Vector3(0, 10, -20);
        Camera.main.transform.eulerAngles = new Vector3(0, 0, 0);
        transform.localPosition = new Vector3(1.3f, 0.0f, 7.0f);
        transform.eulerAngles = new Vector3(0, 180, 0);

    }
	// Update is called once per frame
	void Update ()
    {
        //rotation
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            //model.transform.eulerAngles = new Vector3(0, (-360 * Input.mousePosition.x) / Screen.width, 0);
            transform.Rotate(new Vector3(0, -Input.GetAxis("Mouse X")*6, 0));
        }
    }

    //gonna
    //cappello
    //barba
}
