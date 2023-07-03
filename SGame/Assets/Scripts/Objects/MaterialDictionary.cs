using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class MaterialDictionary : MonoBehaviour
{
    public GameObject defaultParticles;
    public Dictionary<Material, GameObject> HitParticles = new Dictionary<Material, GameObject>();
    public static MaterialDictionary instance { get; private set; }
    //Mark DontDestroyOnLoad and assign instance in awake
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        instance = this;
    }

    /// <summary>
    /// Void that returns the particle system from the material at a hit point
    /// </summary>
    /// <param name="collider">The meshcollider to reference</param>
    /// <param name="triangleIndex">The index of the triangle hit</param>
    /// <returns>The particle system</returns>
    public bool RequestParticleSystem(MeshFilter collider, int triangleIndex, out GameObject value)
    {
        Mesh mesh = collider.sharedMesh;
        int limit = triangleIndex * 3;
        int submeshNumber;
        for (submeshNumber = 0; submeshNumber < mesh.subMeshCount; submeshNumber++)
        {
            int numIndices = mesh.GetTriangles(submeshNumber).Length;
            if (numIndices > limit)
            {
                break;
            }
            limit -= numIndices;
        }

        Material foundMaterial = collider.GetComponent<MeshRenderer>().sharedMaterials[submeshNumber];
        GameObject foundObject = GetParticleEffectFromMaterial(foundMaterial);
        if(foundObject != null)
        {
            value = foundObject;
            return true;
        }
        else
        {
            value = null;
            return false;
        }
        
    }
    /// <summary>
    /// GameObject request that returns the particle system associated with a material
    /// </summary>
    /// <param name="material">The material to reference</param>
    /// <returns>The particlesystem associated with the material</returns>
    private GameObject GetParticleEffectFromMaterial(Material material)
    {
        if (HitParticles.TryGetValue(material, out GameObject system))
        {
            return system;
        }
        else
        {
            return defaultParticles;
        }
    }
}
/// <summary>
/// Class holding the pairs of materials and systems to load into the dictionary on start
/// </summary>
[Serializable]
public class MaterialSystemLink
{
    public Material material;
    public GameObject system;
}