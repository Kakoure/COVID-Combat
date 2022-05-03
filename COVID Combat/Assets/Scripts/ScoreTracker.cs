using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.UI;

public class ScoreTracker : NetworkBehaviour
{
    [SyncVar]
    public int score;
    public GameObject scoreObj;

    [SerializeField]
    Image fadeImage;
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        scoreObj.GetComponent<TextMeshProUGUI>().text = "Score: 0";
    }

    public void IncreaseScore()
    {
        score++;
        scoreObj.GetComponent<TextMeshProUGUI>().text = "Score: " + score;
        RpcIncreaseScore(score);
    }

    [ClientRpc]
    public void RpcIncreaseScore(int rpcScore)
    {
        scoreObj.GetComponent<TextMeshProUGUI>().text = "Score: " + rpcScore;
    }




    // Update is called once per frame
    void Update()
    {

    }
}
