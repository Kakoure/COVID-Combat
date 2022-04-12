using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class GameflowManager : NetworkBehaviour
{
    public RoleManager roleManager;
    public TextMeshProUGUI screenText;
    
    public enum FlowState
    {
        WAITING,
        STARTING,
        IN_GAME
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
