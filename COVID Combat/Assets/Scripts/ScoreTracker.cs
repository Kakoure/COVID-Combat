using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class ScoreTracker : NetworkBehaviour
{
    [SyncVar]
    private int score = 0;
    public GameObject scoreObj;
    // Start is called before the first frame update
    void Start()
    {
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
