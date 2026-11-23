using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class line : MonoBehaviour
{
    public Color flatCol;
    public int leng = 0;
    public BoardCreate parent;
    public int startingLen;
    void Start()
    {
        foreach (Transform square in transform)
        {
            foreach (Transform obj in square)
            {
                if (obj.name == "Back")
                {
                    obj.GetComponent<SpriteRenderer>().color = new Color(flatCol.r > 0.34f ? flatCol.r-0.34f : 0, flatCol.g > 0.34f ? flatCol.g-0.34f : 0, flatCol.b > 0.34f ? flatCol.b-0.34f : 0);
                }
                else
                {
                    obj.GetComponent<SpriteRenderer>().color = flatCol;
                }
            }
            leng++;
            square.GetComponent<square>().parent = gameObject.GetComponent<line>();
            square.GetComponent<square>().order = leng-1;
            startingLen = leng;
        }
    }

    public void reset()
    {
        foreach (Transform square in transform)
        {
            if (square.GetComponent<square>().skip)
            {
                continue;
            }
            foreach (Transform obj in square)
            {
                if (obj.name == "Back")
                {
                    obj.GetComponent<SpriteRenderer>().color = new Color(flatCol.r > 0.34f ? flatCol.r-0.34f : 0, flatCol.g > 0.34f ? flatCol.g-0.34f : 0, flatCol.b > 0.34f ? flatCol.b-0.34f : 0);
                }
                else
                {
                    obj.GetComponent<SpriteRenderer>().color = flatCol;
                }
            }
        }
    }

    public void dtCheck(square check)
    {
        if (parent.canDie(check))
        {
            for (int i = leng; i < leng + parent.roll; i++)
            {
                GameObject chosen = transform.GetChild(i).gameObject;
                chosen.GetComponent<square>().skip = false;
                foreach (Transform child in chosen.transform)
                {
                    Color orig = child.GetComponent<SpriteRenderer>().color;
                    child.GetComponent<SpriteRenderer>().color = new Color(orig.r, orig.g, orig.b);
                }
            }
            leng += parent.roll;
            parent.resetDiceTime();
        }
    }

    public void highlight(int beginIndx)
    {
        foreach (Transform square in transform)
        {
            if (square.GetComponent<square>().order >= beginIndx && !square.GetComponent<square>().skip)
            {

                foreach (Transform obj in square)
                {
                    if (obj.name == "Back")
                    {
                        obj.GetComponent<SpriteRenderer>().color = new Color(flatCol.r > 0.14f ? flatCol.r-0.14f : 0, flatCol.g > 0.14f ? flatCol.g-0.14f : 0, flatCol.b > 0.14f ? flatCol.b-0.14f : 0);
                    }
                    else
                    {
                        obj.GetComponent<SpriteRenderer>().color = new Color(flatCol.r != 0 ? flatCol.r+0.2f : 0, flatCol.g != 0 ? flatCol.g+0.2f : 0, flatCol.b != 0 ? flatCol.b+0.2f : 0);
                    }
                }

            }
        }
    }

    public void remove(int beginIndx)
    {
        while (beginIndx < leng)
        {
            leng--;
            GameObject chosen = transform.GetChild(leng).gameObject;
            chosen.SetActive(false);
        }
    }

    public void reactivate(int beginIndx, int count)
    {
        for (int i = beginIndx; i < beginIndx + count; i++)
        {
            GameObject chosen = transform.GetChild(i).gameObject;
            chosen.GetComponent<square>().skip = true;
            chosen.SetActive(true);
            foreach (Transform child in chosen.transform)
            {
                Color orig = child.GetComponent<SpriteRenderer>().color;
                child.GetComponent<SpriteRenderer>().color = new Color(orig.r, orig.g, orig.b, 0.3f);
            }
        }
    }
}