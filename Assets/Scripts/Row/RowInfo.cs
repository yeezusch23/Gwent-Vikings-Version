using System.Collections.Generic;
using UnityEngine;

public class RowInfo : MonoBehaviour
{
    public Sprite close;
    public Sprite range;
    public Sprite siege;
    public Sprite special;
    public Sprite closeSelected;
    public Sprite rangeSelected;
    public Sprite siegeSelected;
    public Sprite specialSelected;

    
    // public Sprite orangeCircle;
    // public Sprite blueCircle;

    public List<GameObject> listRows;
    List<GameObject> rowFields;

    public void Reset()
    {
        listRows.Clear();
    }

    public void infoField(GameObject row)
    {   
        
        rowFields.Add(row);
        rowFields = listRows;
    }
}
