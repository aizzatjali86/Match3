using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    int horizontal = 7;
    int vertical = 5;
    float distance = 1.05f;

    public GameObject block;
    
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;

        int count = 0;
        
        // Create the blocks for the board
        for (int i = 0; i < horizontal; i++)
        {
            for (int j = 0; j < vertical; j++)
            {
                Vector2 position = new Vector2((i - Mathf.FloorToInt(horizontal/2)) * distance, (j - Mathf.FloorToInt(vertical / 2)) * distance);
                GameObject blockInstance = Instantiate(block) as GameObject;
                blockInstance.transform.parent = gameObject.transform;
                blockInstance.transform.localPosition = position;
                blockInstance.GetComponent<BlockManager>().horizontal = i;
                blockInstance.GetComponent<BlockManager>().vertical = j;
                blockInstance.name = "Block-" + i.ToString() + "-" + j.ToString();

                count++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
