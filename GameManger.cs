 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManger : MonoBehaviour
{
    public GameObject[] starObjs;
    public int maxRow = 13;                         
    public int maxColumn = 10;                     
    public GameObject starGroup;                   
    public List<Star> StarList;                     
    public List<Star> ClickedStarList;              
    public static GameManger gameManger_Instance;
    public int currentScore = 0;
    public Text currentScoreText;                   
    public int currentTotalScore = 0;
    public Text currentTotalScoreText;             
    public int HurdleIndex = 1;
    public Text hurdleText;                         
    public int targetScore = 0;
    public Text targetScoreText;                    
    public int judgeSwitch = 0;
    public Button play;
    public Button rank;
    public Button esc;
    public Button repaly;                           
    public AudioSource clearSource;
    public AudioSource bgm;                         
    public GameObject[] particles;
    //public Button musicOn;
    //public Button musicOff;
    public Text Name;
    public InputField youName;
    public Button hhh;
   
    // Start is called before the first frame update
    public void Start()
    {
        gameManger_Instance = this;
        
        //musicOn.gameObject.SetActive(true);
        //musicOff.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    void GameStart(int hurdle)
    {
        
        HurdleIndex = hurdle;
        currentScore = 0;
        SetHurdleTargetScore(HurdleIndex);
        CreateStarList();
        judgeSwitch = 0;
    }
    void CreateStarList()
    {
        for (int r = 0; r < maxRow; r++)
        {
            for (int c = 0; c < maxColumn; c++)
            {
                int index = Random.Range(0, 5);
                var obj = Instantiate(starObjs[index],starGroup.transform);

                Vector3 pos = new Vector3(48 * c, 48 * r, 0);

                obj.transform.localPosition = pos;

                var star = obj.GetComponent<Star>();
                star.Row = r;
                star.Column = c;
                StarList.Add(star);

            }
        }
    }
    public void FindTheSameStar(Star currentStar)
    {
        int row = currentStar.Row;
        int column = currentStar.Column;
        if(row < maxRow)
        {
            foreach (var item in StarList)
            {
                if (item.Row == row + 1 && item.Column == column)
                {
                    if (item.starColor == currentStar.starColor)
                    {
                        if (!ClickedStarList.Contains(item))
                        {
                            ClickedStarList.Add(item);
                            FindTheSameStar(item);
                        }
                    }
                }
            }
        }
        if (row > 0)
        {
            foreach (var item in StarList)
            {
                if (item.Row == row - 1 && item.Column == column)
                {
                    if (item.starColor == currentStar.starColor)
                    {

                        if (!ClickedStarList.Contains(item))
                        {
                            ClickedStarList.Add(item);
                            FindTheSameStar(item);
                        }

                    }
                }
            }
        }
        if (column < maxColumn)
        {
            foreach (var item in StarList)
            {
                if (item.Row == row && item.Column == column + 1)
                {
                    if (item.starColor == currentStar.starColor)
                    {

                        if (!ClickedStarList.Contains(item))
                        {
                            ClickedStarList.Add(item);
                            FindTheSameStar(item);
                        }

                    }
                }
            }
        }
        if (column > 0)
        {
            foreach (var item in StarList)
            {
                if (item.Row == row  && item.Column == column -1)
                {
                    if (item.starColor == currentStar.starColor)
                    {
                        if (!ClickedStarList.Contains(item))
                        {
                            ClickedStarList.Add(item);
                            FindTheSameStar(item);
                        }
                    }
                }
            }
        }
    }

    public void ClearClickedStarList()
    {
        if (ClickedStarList.Count >= 2)
        {
            foreach (var item in ClickedStarList)
            {
                int colorIndex = (int)item.starColor;
                if(particles.Length>=colorIndex)
                {
                    GameObject parObj = particles[colorIndex];
                    var obj = Instantiate(parObj,starGroup.transform);
                    obj.transform.localPosition = item.transform.localPosition;
                }
              
                item.DestroyStar();
                StarList.Remove(item);
                
            }
            foreach (var restStar in StarList)
            {
                int moveCount = 0;
                foreach (var clickedStar in ClickedStarList)
                {
                    if (restStar.Column == clickedStar.Column && restStar.Row > clickedStar.Row)
                    {
                        moveCount++;
                    }
                }
                if (moveCount > 0)
                {
                    restStar.moveDownCount = moveCount;
                    restStar.OpenMoveDown();
                }

            }
            for (var col = maxColumn; col >= 0; col--)
            {
                bool IsEmpty = true;
                foreach (var restStar in StarList)
                {
                    if (restStar.Column == col)
                    {
                        IsEmpty = false;
                    }
                }
                if (IsEmpty)
                {
                    foreach (var restStar in StarList)
                    {
                        if (restStar.Column > col)
                        {
                            restStar.moveLeftCount++;
                        }
                    }
                }
            }
            foreach (var restStar in StarList)
            {
                if (restStar.moveLeftCount >= 1)
                {
                    restStar.OpenMoveLeft();
                }
            }
            if (clearSource != null)
            {
                clearSource.Play();
            }

        }
        CalculateScore(ClickedStarList.Count);
        ClickedStarList.Clear();
        JudgeHurdleOver();
    }

    public void JudgeHurdleOver()
    {
        bool isOver = true;
        foreach (var restStar in StarList)
        {

            FindTheSameStar(restStar);
            if (ClickedStarList.Count > 0)
            {
                isOver = false;
            }
        }
        ClickedStarList.Clear();
        if (isOver)
        {
            if (judgeSwitch == 0)
            {
                judgeSwitch = 1;

                Debug.Log("over");
                RestStarRewardScore(StarList.Count);
                foreach (var restStar in StarList)
                {
                    restStar.DestroyStar();
                }
                StarList.Clear();
                if (currentTotalScore >= targetScore)
                {
                    GameStart(++HurdleIndex);
                }
                else
                {
                    youName.gameObject.SetActive(true);
                }
            }
        }
        
    }

    void CalculateScore(int destoryStarCount)
    {
        if(destoryStarCount >=2)
        {
            int tempScore = 0;
            for(int i=0; i < destoryStarCount; i++)
            {
                tempScore += i * 10 + 10;
            }
            currentScore = tempScore;
            currentScoreText.text ="得分:"+ currentScore;
            currentTotalScore += currentScore;
            currentTotalScoreText.text ="总分:"+ currentTotalScore;
        }
    }

    void RestStarRewardScore(int restStarCount)
    {
        int rewardScore = 0;
        if (restStarCount<15)
        {
            rewardScore = 2000 - restStarCount * 100;
            currentTotalScore += rewardScore;
            currentTotalScoreText.text = "总分:" + currentTotalScore;
        }
    }

    void SetHurdleTargetScore(int hurdleIndex)
    {
        hurdleText.text = "关卡:" + hurdleIndex;
        if (hurdleIndex == 1)
        {
            targetScore = 1000;
        }
        else if(hurdleIndex>1)
        {
            targetScore = 1000;
            for(int i = 1; i < hurdleIndex; i++)
            {
                targetScore += 2000 + (i - 1) * 200;
            }
        }
        targetScoreText.text = "目标:" + targetScore;
    }

    public void ReplayGame()
    {
        
        SceneManager.LoadScene("SampleScene");
   
    }

    public void Playing()
    {
        GameStart(1);
        play.gameObject.SetActive(false);
        esc.gameObject.SetActive(false);
        //musicOn.gameObject.SetActive(false);
        //musicOff.gameObject.SetActive(false);
        rank.gameObject.SetActive(false);
    }
    public void Esc()
    {
        Application.Quit();
    }
    //int m = 1;
    //public void Music()
    //{

    //    if (m % 2 != 0)
    //    {
    //        musicOn.gameObject.SetActive(false);
    //        musicOff.gameObject.SetActive(true);
    //        m++;
    //    }
    //    else
    //    {
    //        musicOn.gameObject.SetActive(true);
    //        musicOff.gameObject.SetActive(false);
    //        m++;
    //    }
    //}
    public void Rank()
    {
        play.gameObject.SetActive(false);
        esc.gameObject.SetActive(false);
        //musicOn.gameObject.SetActive(false);
        //musicOff.gameObject.SetActive(false);
        rank.gameObject.SetActive(false);
    }
    public void YouName()
    {
        StartCoroutine(Fenshu.CreateNewHighScore(Name.text, currentTotalScore.ToString()));
        youName.gameObject.SetActive(false);
        hhh.gameObject.SetActive(true);
    }

}
