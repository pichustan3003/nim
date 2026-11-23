using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class BoardCreate : MonoBehaviour
{   
    public GameObject lnObj;
    public GameObject squareObj;
    public int toPlay = 1;
    public TMP_Text playerTitle;
    public TMP_Text winner;
    public List<Slider> sliders;
    int turnCounter = 0;
    public bool diceMode;
    public Toggle dice;
    public TMP_Text diceText;
    public bool diceTime;
    public int roll;
    private List<line> canDo;
    public bool botOn;
    public Toggle lone;
    private List<Color> colors = new List<Color>
    {
        new Color(0.8f,0,0),
        new Color(0,0.8f,0),
        new Color(0,0,0.8f),
        new Color(0.8f,0.8f,0),
        new Color(0.8f,0,0.8f),
        new Color(0,0.8f,0.8f),
        new Color(0.8f,0.4f,0),
        new Color(0.4f, 0, 0.8f),
        new Color(0, 0.8f, 0.87f)
    };
    void Start()
    {
        int i = 0;
        foreach (Slider s in sliders)
        {
            s.transform.GetChild(0).GetComponent<Image>().color = colors[i];
            i++;
        }
    }

    int fMax(List<int> counts)
    {
        int running = 0;
        counts.ForEach(x => running = x > running ? x : running);
        return running;
    }

    void createBoard(int lines, List<int> lengths)
    {
        int len = fMax(lengths);
        float w = -(len-(len%2))/2;
        float count = lines > 7 ? 1f : 1.5f;
        for (int i = 0; i < lines; i++)
        {
            float xP = (i % 2 == 1 ? 1 : -1) * count * ((i + 1) / 2);
            createLine(lengths[i], xP, w);
        }
    }

    void createLine(int len, float height = 0, float x = 0)
    {
        GameObject line = Instantiate(lnObj);
        line.transform.parent = gameObject.transform;
        line.GetComponent<line>().flatCol = colors[0];
        colors.RemoveAt(0);
        for (int i = 0; i < len; i++)
        {
            Debug.Log("Created " + i + "th object");
            GameObject newCube = Instantiate(squareObj);
            newCube.transform.position = new Vector2(i, 0);
            newCube.transform.parent = line.transform;
        }
        Debug.Log(-1*((len-(len&2))/2));
        line.transform.position = new Vector2(x == 0 ? -1*(
            (len-(len%2))/2) : x
            , height);
        line.GetComponent<line>().parent = gameObject.GetComponent<BoardCreate>();
    }

    public void playTurn()
    {
        Debug.Log("playTurn called. Player: " + toPlay);
        turnCounter = (turnCounter + 1) % 3;
        bool over = true;
        foreach (Transform line in transform)
        {
            if (line.GetComponent<line>().leng != 0)
            {
                over = false;
            }
        }
        toPlay = toPlay == 1 ? 2 : 1;
        if (over)
        {
            winner.text = !botOn ? "Player " + toPlay + " wins!" : toPlay == 1 ? "Human wins!" : "AI wins";
            winner.gameObject.SetActive(true);
            playerTitle.gameObject.SetActive(false);
            Debug.Log("GAME OVER");
            return;
        }
        playerTitle.text = "Player " + toPlay + " to play";
        if (turnCounter == 0 && diceMode)
        {
            roll = UnityEngine.Random.Range(1,7);
            diceText.text = "It's Dice time! \nThe dice rolled a "+roll;
            diceText.gameObject.SetActive(true);
            bool can = false;
            canDo = new();
            foreach (Transform child in transform)
            {  
                line ln = child.gameObject.GetComponent<line>();
                if (ln.startingLen - ln.leng  >= roll)
                {
                    can = true;
                    canDo.Add(ln);
                }
            }
            if (!can)
            {
                diceTime = false;
                diceText.text = "It's Dice time! \nThe dice rolled a "+roll +"\n However as no single section has lost that much yet nothing can be done";
            }
            else
            {
                diceTime=true;
                foreach (line ln in canDo)
                {
                    ln.reactivate(ln.leng, roll);
                }
            }
        }
        else
        {
            diceText.gameObject.SetActive(false);
            diceTime = false;
        }
        if (botOn && !diceMode && toPlay == 2)
        {
            List<line> lengths = new();

            foreach (Transform ln in transform)
            {
                line lnObj = ln.GetComponent<line>();
                if (lnObj.leng > 0)
                    lengths.Add(lnObj);
            }

            if (lengths.Count == 0)
                return;

            int xor = 0;

            foreach (line ln in lengths)
            {
                xor ^= ln.leng;
            }

            line toRem = null;
            int countRem = 0;

            int bigPiles = lengths.FindAll(x => x.leng > 1).Count;

            if (bigPiles == 1)
            {
                line bigPile = lengths.Find(x => x.leng > 1);

                int ones = lengths.Count - 1;
                int target = ones % 2 == 0 ? 1 : 0;

                toRem = bigPile;
                countRem = bigPile.leng - target;
            }
            else if (bigPiles == 0)
            {
                toRem = lengths[0];
                countRem = 1;
            }
            else
            {
                foreach (line ln in lengths)
                {
                    int target = ln.leng ^ xor;

                    if (target < ln.leng)
                    {
                        toRem = ln;
                        countRem = ln.leng - target;
                        break;
                    }
                }
            }

            // No winning move available: choose any legal move.
            if (toRem == null)
            {
                toRem = lengths[0];
                countRem = 1;
            }

            toRem.remove(toRem.leng - countRem);
            Debug.Log($"AI removing {countRem} from pile of {toRem.leng}");
            playTurn();
        }
    }

    public bool calcWinning()
    {
        List<line> lengths = new();
        foreach (Transform ln in transform)
        {
            line lnObj = ln.GetComponent<line>();
            if (lnObj.leng != 0)
            {
                lengths.Add(lnObj);
            }
        }
        if (lengths.Count == 0)
        {
            throw new System.Exception("Cannot find any block sets");
        }
        int curX = 0;
        bool allOnes = true;

        foreach (line ln in lengths)
        {
            curX ^= ln.leng;

            if (ln.leng != 1)
            {
                allOnes = false;
            }
        }

        bool losing = allOnes
            ? lengths.Count % 2 == 1
            : curX == 0;
        return losing;
    }

    public void resetDiceTime()
    {
        diceText.gameObject.SetActive(false);
        diceTime = false;
        foreach (Transform ln in transform)
        {
            foreach (Transform square in ln)
            {
                foreach (Transform r in square)
                {
                    if (r.GetComponent<SpriteRenderer>().color.a != 1f)
                    {
                        square.gameObject.SetActive(false);
                    }
                } 
            }
        }
    }

    public bool canDie(square check)
    {
        if (canDo.Contains(check.parent))
        {
            return true;
        }
        return false;
    }
    public void beginGame()
    {
        List<int> slideVals = new();
        foreach (Slider s in sliders)
        {
            slideVals.Add((int)s.value);
        }
        createBoard(9, slideVals);
        sliders[0].transform.parent.gameObject.SetActive(false);
        playerTitle.gameObject.SetActive(true);
    }

    public void ResetGame()
    {
        colors = new List<Color>
        {
            new Color(0.8f,0,0),
            new Color(0,0.8f,0),
            new Color(0,0,0.8f),
            new Color(0.8f,0.8f,0),
            new Color(0.8f,0,0.8f),
            new Color(0,0.8f,0.8f),
            new Color(0.8f,0.4f,0),
            new Color(0.4f, 0, 0.8f),
            new Color(0, 0.8f, 0.87f)
        };
        sliders[0].transform.parent.gameObject.SetActive(true);
        playerTitle.gameObject.SetActive(false);
        winner.gameObject.SetActive(false);
        toPlay = 1;
        turnCounter = 0;
        playerTitle.text = "Player " + toPlay + " to play";
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
        foreach (Slider s in sliders)
        {
            s.value = 0;
        }
    }

    public void readDice()
    {
        diceMode = dice.isOn;
        if (diceMode)
        {
            lone.enabled = false;
            ColorBlock cb = lone.colors;
            cb.normalColor = new Color(0.3f, 0.3f, 0.3f);
            lone.colors = cb;
        }
        else
        {
            lone.enabled = true;
            ColorBlock cb = lone.colors;
            cb.normalColor = new Color(1,1,1);
            lone.colors = cb;
        }
    }

    public void readLone()
    {
        botOn = lone.isOn;
        if (botOn)
        {
            dice.enabled = false;
            ColorBlock cb = dice.colors;
            cb.normalColor = new Color(0.3f, 0.3f, 0.3f);
            dice.colors = cb;
        }
        else
        {
            dice.enabled = true;
            ColorBlock cb = dice.colors;
            cb.normalColor = new Color(1,1,1);
            dice.colors = cb;
        }
    }

}
