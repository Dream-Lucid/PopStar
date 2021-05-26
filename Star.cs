using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum StarColor
{
    a = 0,
    b = 1,
    c = 2,
    d = 3,
    e = 4,
}
public class Star : MonoBehaviour
{
    public int Row = 0;
    public int Column = 0;
    public StarColor starColor = StarColor.a;
    public int moveDownCount = 0;
    public int moveLeftCount = 0;
    private bool IsMoveDown = false;
    private bool IsMoveLeft = false;
    private int targetRow = 0;
    private int targetColumn = 0;
    public float speed = -2f;

    // Start is called before the first frame update
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
        if (IsMoveDown)
        {
            Row = targetRow;
            Vector3 downVector = new Vector3(0, 1, 0);
            if (this.transform.localPosition.y > targetRow * 48f)
            {
                this.transform.Translate(downVector * speed);
            }
            else
            {
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, targetRow * 48f, this.transform.localPosition.z);
                IsMoveDown = false;
                moveDownCount = 0;
            }
        }

        if (IsMoveLeft)
        {
            Column = targetColumn;
            Vector3 leftVector = new Vector3(1, 0, 0);
            if (this.transform.localPosition.x > targetColumn * 48f)
            {
                this.transform.Translate(leftVector * speed);
            }
            else
            {
                this.transform.localPosition = new Vector3(targetColumn * 48f, this.transform.localPosition.y, this.transform.localPosition.z);
                IsMoveLeft = false;
                moveLeftCount = 0;
            }
        }
    }
    public void OnClick_Star()
    {
        GameManger.gameManger_Instance.FindTheSameStar(this);
        GameManger.gameManger_Instance.ClearClickedStarList();
    }

    public void DestroyStar()
    {
        Destroy(this.gameObject);
    }

    public void OpenMoveDown()
    {
        IsMoveDown = true;
        targetRow = Row - moveDownCount;
    }
    public void OpenMoveLeft()
    {
        IsMoveLeft = true;
        targetColumn = Column - moveLeftCount;
    }
}
