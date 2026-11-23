using UnityEngine;

public class square : MonoBehaviour
{
    public line parent;
    public int order;
    public bool skip = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnMouseEnter()
    {
        if (!parent.parent.diceTime || skip){
            parent.highlight(order);
        }
    }
    void OnMouseExit()
    {
        if (!parent.parent.diceTime || skip){
        parent.reset();
        }
    }
    void OnMouseDown()
    {
        if (!parent.parent.diceTime){
            parent.remove(order);
            parent.parent.playTurn();
        }
        else
        {
            parent.dtCheck(gameObject.GetComponent<square>());
        }
    }
}
