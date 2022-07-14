using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NPCUtilities : MonoBehaviour
{
    public static GameObject CreateNPC(bool random, Vector3 pos, bool free, bool silent=false, bool combine=true)
    {
        GameObject npc = Instantiate(Resources.Load<GameObject>("NPCCombine"));
        if (free)
        {
            Destroy(npc.GetComponent<NPCController>());
            Destroy(npc.GetComponent<GoBackToPlace>());
        }

        if (silent)
            Destroy(npc.GetComponent<NPCInteraction>());

        npc.transform.position = pos;

        if (random)
            NPCUtilities.RandomLook(npc);

        if (combine)
            npc.GetComponent<CombineChildren>().Combine();

        return npc;
    }
    public static void RandomLook(GameObject model)
    {
        Parts parts = model.GetComponent<Parts>();

        List<Mesh> meshes = new List<Mesh>();

        meshes = Resources.LoadAll<Mesh>("Parts/Body").ToList();

        parts.body.GetComponent<SkinnedMeshRenderer>().sharedMesh = meshes[Random.Range(0, meshes.Count)];


        //parts.GetComponent<CombineChildren>().toMerge[5].GetComponent<SkinnedMeshRenderer>().sharedMesh = parts.body.GetComponent<SkinnedMeshRenderer>().sharedMesh;

        meshes = Resources.LoadAll<Mesh>("Parts/Hair").ToList();
        parts.hair.GetComponent<SkinnedMeshRenderer>().sharedMesh = meshes[Random.Range(0, meshes.Count)];

        string name = parts.hair.GetComponent<SkinnedMeshRenderer>().sharedMesh.name;
        if (name.Contains("long"))
        {
            if (Random.Range(0, 10) < 5)
            {
                meshes = Resources.LoadAll<Mesh>("Parts/Skirt").ToList();
                parts.pants.GetComponent<SkinnedMeshRenderer>().sharedMesh = meshes[Random.Range(0, meshes.Count)];
            }
            else
            {
                meshes = Resources.LoadAll<Mesh>("Parts/Pants").ToList();
                parts.pants.GetComponent<SkinnedMeshRenderer>().sharedMesh = meshes[Random.Range(0, meshes.Count)];
            }
        }
        else
        {
            meshes = Resources.LoadAll<Mesh>("Parts/Pants").ToList();
            parts.pants.GetComponent<SkinnedMeshRenderer>().sharedMesh = meshes[Random.Range(0, meshes.Count)];
        }

        if (Random.Range(0, 5) < 2)
        {
            meshes = Resources.LoadAll<Mesh>("Parts/Glasses").ToList();
            parts.glasses.GetComponent<SkinnedMeshRenderer>().sharedMesh = meshes[Random.Range(0, meshes.Count)];
        }
        else
        {
            parts.glasses.GetComponent<SkinnedMeshRenderer>().sharedMesh = null;
        }
        parts.lenses.GetComponent<SkinnedMeshRenderer>().sharedMesh = null;

        meshes = Resources.LoadAll<Mesh>("Parts/Shirt").ToList();
        parts.shirt.GetComponent<SkinnedMeshRenderer>().sharedMesh = meshes[Random.Range(0, meshes.Count)];

        meshes = Resources.LoadAll<Mesh>("Parts/Shoes").ToList();
        parts.shoes.GetComponent<SkinnedMeshRenderer>().sharedMesh = meshes[Random.Range(0, meshes.Count)];
    }
    //public static void RandomLook(GameObject model)
    //{
    //    Material shirt = Resources.Load<Material>("Materials/Characters/Shirt");
    //    Material hair = Resources.Load<Material>("Materials/Characters/Hair");
    //    Material pants = Resources.Load<Material>("Materials/Characters/Pants");
    //    Material skin = Resources.Load<Material>("Materials/Characters/Skin");
    //    Material glasses = Resources.Load<Material>("Materials/Characters/Glasses");
    //    Parts modelParts = model.GetComponent<Parts>();

    //    SkinnedMeshRenderer pglasses = modelParts.glasses.GetComponent<SkinnedMeshRenderer>();
    //    pglasses.sharedMesh = (Mesh)Resources.Load("Glasses" + UnityEngine.Random.Range(0, 2), typeof(Mesh));
    //    pglasses.GetComponent<Renderer>().material = glasses;
    //    pglasses.GetComponent<Renderer>().materials[0].color = RandomColor(0f, 1f, 0f, 1f, 0f, 0f);

    //    SkinnedMeshRenderer peyes = modelParts.eyes.GetComponent<SkinnedMeshRenderer>();
    //    peyes.sharedMesh = Resources.Load<Mesh>("Eyes" + UnityEngine.Random.Range(0, 2));
    //    peyes.GetComponent<Renderer>().material.color = RandomColor(0f, 0f, 0f, 1f, 0f, 1f);

    //    SkinnedMeshRenderer pshirt = modelParts.shirt.GetComponent<SkinnedMeshRenderer>();
    //    pshirt.sharedMesh = Resources.Load<Mesh>("Shirt" + UnityEngine.Random.Range(0, 2));
    //    pshirt.GetComponent<Renderer>().material = shirt;
    //    pshirt.GetComponent<Renderer>().material.color = RandomColor(0f, 0.5f, 0f, 0.5f, 0f, 0.5f);

    //    SkinnedMeshRenderer ppants = modelParts.pants.GetComponent<SkinnedMeshRenderer>();
    //    ppants.sharedMesh = Resources.Load<Mesh>("Pants" + UnityEngine.Random.Range(0, 2));
    //    ppants.GetComponent<Renderer>().material = pants;
    //    ppants.GetComponent<Renderer>().material.color = RandomColor(0f, 0.5f, 0f, 0.5f, 0f, 0.5f);

    //    SkinnedMeshRenderer pshoes = modelParts.shoes.GetComponent<SkinnedMeshRenderer>();
    //    pshoes.sharedMesh = (Mesh)Resources.Load("Shoes" + UnityEngine.Random.Range(0, 2), typeof(Mesh));
    //    pshoes.GetComponent<Renderer>().material.color = RandomColor(0f, 0.5f, 0f, 0.5f, 0f, 0.5f);

    //    SkinnedMeshRenderer phair = modelParts.hair.GetComponent<SkinnedMeshRenderer>();
    //    phair.sharedMesh = (Mesh)Resources.Load("Hair" + UnityEngine.Random.Range(0, 4), typeof(Mesh));
    //    phair.GetComponent<Renderer>().material = hair;
    //    float c = UnityEngine.Random.Range(0f, 0.5f);
    //    phair.GetComponent<Renderer>().material.color = RandomColor(c, c, c / 2, c / 2, c / 4, c / 4);

    //    SkinnedMeshRenderer pskin = modelParts.body.GetComponent<SkinnedMeshRenderer>();
    //    pskin.GetComponent<Renderer>().material = skin;

    //    //Material[] materials = new Material[5];
    //    //materials[0] = shirt; materials[1] = hair; materials[2] = pants; materials[3] = skin; materials[4] = glasses;
    //    //return materials;
    //}

    public static Color RandomColor(float r, float r2, float g, float g2, float b, float b2)
    {
        return new Color(UnityEngine.Random.Range(r, r2), UnityEngine.Random.Range(g, g2), UnityEngine.Random.Range(b, b2));
    }
}
