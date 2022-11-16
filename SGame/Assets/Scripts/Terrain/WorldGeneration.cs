using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public IslandGenerator islandGen;
    public FoliagePlacer foliageGen;

    public int currentGenerationState = 0;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (currentGenerationState) {
            case 0:
            foliageGen.startedGeneration = true;
            if (foliageGen.batched == true)
            {
                currentGenerationState = 1;
            }
                return;

            case 1:
                islandGen.startedGenertation = true;
                if (islandGen.finishedBatching == true)
                {
                    currentGenerationState = 2;
                }

                return;
        }
      


    }
}


