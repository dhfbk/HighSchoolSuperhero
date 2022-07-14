using UnityEngine;

public class UpdateAvatar : MonoBehaviour
{
    public GameObject eyes;
    public GameObject glasses;
    public GameObject hair;
    public GameObject mouth;
    public GameObject pants;
    public GameObject shirt;
    public GameObject shoes;
    public GameObject body;
    // Start is called before the first frame update
    void Start()
    {
        Up();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Up()
    {
        print("Avatar update temporarily disabled");
        //eyes.GetComponent<SkinnedMeshRenderer>().sharedMesh = GameObject.FindGameObjectWithTag("ModelEyeL").GetComponent<SkinnedMeshRenderer>().sharedMesh;
        //glasses.GetComponent<SkinnedMeshRenderer>().sharedMesh = GameObject.FindGameObjectWithTag("ModelGlasses").GetComponent<SkinnedMeshRenderer>().sharedMesh;
        //hair.GetComponent<SkinnedMeshRenderer>().sharedMesh = GameObject.FindGameObjectWithTag("ModelHair").GetComponent<SkinnedMeshRenderer>().sharedMesh;
        //mouth.GetComponent<SkinnedMeshRenderer>().sharedMesh = GameObject.FindGameObjectWithTag("ModelMouth").GetComponent<SkinnedMeshRenderer>().sharedMesh;
        //pants.GetComponent<SkinnedMeshRenderer>().sharedMesh = GameObject.FindGameObjectWithTag("ModelPants").GetComponent<SkinnedMeshRenderer>().sharedMesh;
        //shirt.GetComponent<SkinnedMeshRenderer>().sharedMesh = GameObject.FindGameObjectWithTag("ModelShirt").GetComponent<SkinnedMeshRenderer>().sharedMesh;
        //shoes.GetComponent<SkinnedMeshRenderer>().sharedMesh = GameObject.FindGameObjectWithTag("ModelShoes").GetComponent<SkinnedMeshRenderer>().sharedMesh;
        //body.GetComponent<SkinnedMeshRenderer>().sharedMesh = GameObject.FindGameObjectWithTag("ModelBody").GetComponent<SkinnedMeshRenderer>().sharedMesh;
        ////color
        //eyes.GetComponent<Renderer>().materials[3].color = GameObject.FindGameObjectWithTag("ModelEyeL").GetComponent<Renderer>().materials[3].color;
        //glasses.GetComponent<Renderer>().materials[0].color = GameObject.FindGameObjectWithTag("ModelGlasses").GetComponent<Renderer>().materials[0].color;
        //hair.GetComponent<Renderer>().material.color = GameObject.FindGameObjectWithTag("ModelHair").GetComponent<Renderer>().material.color;
        //mouth.GetComponent<Renderer>().material.color = GameObject.FindGameObjectWithTag("ModelMouth").GetComponent<Renderer>().material.color;
        //pants.GetComponent<Renderer>().material.color = GameObject.FindGameObjectWithTag("ModelPants").GetComponent<Renderer>().material.color;
        //shirt.GetComponent<Renderer>().material.color = GameObject.FindGameObjectWithTag("ModelShirt").GetComponent<Renderer>().material.color;
        //shoes.GetComponent<Renderer>().material.color = GameObject.FindGameObjectWithTag("ModelShoes").GetComponent<Renderer>().material.color;
        //body.GetComponent<Renderer>().material.color = GameObject.FindGameObjectWithTag("ModelBody").GetComponent<Renderer>().material.color;
    }
}
