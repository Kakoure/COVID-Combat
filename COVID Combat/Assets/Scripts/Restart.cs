using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class Restart : NetworkBehaviour
{

    void Update()
    {
        
    }

    public void RestartGame()
    {
        CmdRestartGame();
    }


    [Command(requiresAuthority=false)]
    public void CmdRestartGame()
    {
        //SceneManager.LoadScene(1);
        NetworkManager.singleton.ServerChangeScene("Game Scene");
        //SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
    }
}
