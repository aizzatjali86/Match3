using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public int horizontal;
    public int vertical;
    public int colour;
    public int countH;
    public int countV;
    public bool matched = false;

    public GameObject pieceInstance;
    GameObject garbage;
    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        garbage = GameObject.Find("Garbage");

        CreatePiece();
    }

    // Update is called once per frame
    void Update()
    {
        if (countH >= 3 || countV >= 3)
        {
            matched = true;
        }
        else
        {
            matched = false;
        }

        if (transform.childCount < 2)
        {
            if (vertical != 0)
            {
                MoveUp();
            }
            else
            {
                CreatePiece();
            }
        }
        CountColourV2();
    }

    public void CountColourV2()
    {
        countV = 1;

        int v = vertical;
        while(v > 0)
        {
            v -= 1;
            string downBlockName = "Puzzle board/Block-" + horizontal.ToString() + "-" + v.ToString();
            GameObject downBlock = GameObject.Find(downBlockName);
            if (downBlock.GetComponent<BlockManager>().colour == colour)
            {
                countV += 1;
            }
            else
            {
                break;
            }
        }

        v = vertical;
        while (v < 4)
        {
            v += 1;
            string upBlockName = "Puzzle board/Block-" + horizontal.ToString() + "-" + v.ToString();
            GameObject upBlock = GameObject.Find(upBlockName);
            if (upBlock.GetComponent<BlockManager>().colour == colour)
            {
                countV += 1;
            }
            else
            {
                break;
            }
        }

        countH = 1;

        int h = horizontal;
        while (h > 0)
        {
            h -= 1;
            string leftBlockName = "Puzzle board/Block-" + h.ToString() + "-" + vertical.ToString();
            GameObject leftBlock = GameObject.Find(leftBlockName);
            if (leftBlock.GetComponent<BlockManager>().colour == colour)
            {
                countH += 1;
            }
            else
            {
                break;
            }
        }

        h = horizontal;
        while (h < 6)
        {
            h += 1;
            string rightBlockName = "Puzzle board/Block-" + h.ToString() + "-" + vertical.ToString();
            GameObject rightBlock = GameObject.Find(rightBlockName);
            if (rightBlock.GetComponent<BlockManager>().colour == colour)
            {
                countH += 1;
            }
            else
            {
                break;
            }
        }
    }

    public bool Match()
    {
        if (countH >= 3 || countV >= 3)
        {
            matched = true;
        }
        else
        {
            matched = false;
        }

        return matched;
    }

    public void CreatePiece()
    {
        colour = Random.Range(0, 5);
        GameObject piece = (GameObject)Resources.Load(colour.ToString(), typeof(GameObject));
        pieceInstance = Instantiate(piece) as GameObject;
        pieceInstance.transform.parent = gameObject.transform;
    }

    public void SelectChoice()
    {
        if (gm.firstChoice == null)
        {
            gm.firstChoice = gameObject;
        }
        else
        {
            if ((gm.firstChoice.GetComponent<BlockManager>().vertical == vertical + 1 && gm.firstChoice.GetComponent<BlockManager>().horizontal  == horizontal) || 
                (gm.firstChoice.GetComponent<BlockManager>().vertical == vertical - 1 && gm.firstChoice.GetComponent<BlockManager>().horizontal == horizontal) ||
                (gm.firstChoice.GetComponent<BlockManager>().horizontal == horizontal + 1 && gm.firstChoice.GetComponent<BlockManager>().vertical == vertical) || 
                (gm.firstChoice.GetComponent<BlockManager>().horizontal == horizontal - 1 && gm.firstChoice.GetComponent<BlockManager>().vertical == vertical))
            {
                gm.secondChoice = gameObject;
            }
            else
            {
                gm.firstChoice = gameObject;
            }
        }
    }

    public void MoveUp()
    {
        string downBlockName = "Puzzle board/Block-" + horizontal.ToString() + "-" + (vertical - 1).ToString();
        GameObject downBlock = GameObject.Find(downBlockName);
        downBlock.transform.GetChild(1).transform.SetParent(gameObject.transform);
        colour = downBlock.GetComponent<BlockManager>().colour;
    }

}
