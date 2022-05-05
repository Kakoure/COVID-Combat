using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class GameflowManager : NetworkBehaviour
{
    public static GameflowManager Instance;

    public RoleManager roleManager;
    public PilotControls pilotControls;
    public ShooterControls shooterControls;
    public CellSpawner cellSpawn;
    public PlayerScript playerCntrl;

    public TextMeshProUGUI flowText;
    
    public enum FlowState
    {
        WAITING,
        STARTING,
        IN_GAME,
        DMG_TUTOR,
        HEAL_TUTOR,
        SHOOT_TUTOR,
        POWER_TUTOR,
        STARTED
    }

    public FlowState currentState;

    [SyncVar]
    public bool inTutorial;

    [SyncVar]
    public bool drivingEnabled;
    [SyncVar]
    public bool shootingEnabled;

    public bool tutorialCompleted;

    float origSpawnDist;
    float origSpawnTime;

    public string currentFlowText;

    public GameObject dmgTutWall;
    public GameObject healTutWall;
    public GameObject shootTutWall;
    public GameObject powerTutWall;
    public GameObject startedWall;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        if (!isServer)
        {
            return;
        }
        inTutorial = true;

        currentState = FlowState.WAITING;
        currentFlowText = "Waiting for Players...";
        flowText.text = "Waiting for Players...";
        drivingEnabled = false;
        shootingEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
        {
            return;
        }
        switch (currentState)
        {
            case FlowState.WAITING:
                if(roleManager.PilotJoined() && roleManager.ShooterJoined())
                {
                    StartCoroutine(StartingGameCoroutine());
                    currentState = FlowState.STARTING;
                }
                
                break;
            case FlowState.STARTING:
                if(playerCntrl.transform.position.z >= dmgTutWall.transform.position.z)
                {
                    SetFlowText("These Cells do damage. Try running into one!");
                    tutorialCompleted = false;
                    currentState = FlowState.DMG_TUTOR;
                }
                
                break;
            case FlowState.DMG_TUTOR:
                if (playerCntrl.transform.position.z >= healTutWall.transform.position.z)
                {
                    if (!tutorialCompleted)
                    {
                        SetShipPosition(roleManager.GetPilot().connectionToClient, dmgTutWall.transform.position + new Vector3(0f, 0f, 50f));
                    }
                    else
                    {
                        SetFlowText("These Cells heal. Try running into one!");
                        currentState = FlowState.HEAL_TUTOR;
                        tutorialCompleted = false;
                        foreach(CellMoveNetwork cellCntrl in dmgTutWall.GetComponentsInChildren<CellMoveNetwork>())
                        {
                            cellCntrl.ReturnCellToPool();
                        }
                    }
                } else
                {
                    if (tutorialCompleted)
                    {
                        SetFlowText("Great Job!");
                    }
                }
                break;
            case FlowState.HEAL_TUTOR:
                if (playerCntrl.transform.position.z >= shootTutWall.transform.position.z)
                {
                    if (!tutorialCompleted)
                    {
                        SetShipPosition(roleManager.GetPilot().connectionToClient, healTutWall.transform.position + new Vector3(0f, 0f, 50f));
                    }
                    else
                    {
                        SetFlowText("Try shooting this COVID virus!");
                        currentState = FlowState.SHOOT_TUTOR;
                        tutorialCompleted = false;
                        drivingEnabled = false;
                        shootingEnabled = true;
                        DisablePilotMovement(roleManager.GetPilot().connectionToClient);
                        EnableShooting(roleManager.GetShooter().connectionToClient);

                        foreach (CellMoveNetwork cellCntrl in healTutWall.GetComponentsInChildren<CellMoveNetwork>())
                        {
                            cellCntrl.ReturnCellToPool();
                        }
                    }
                }
                else
                {
                    if (tutorialCompleted)
                    {
                        SetFlowText("Great Job!");
                    }
                }

                break;
            case FlowState.SHOOT_TUTOR:
                if(tutorialCompleted && !drivingEnabled)
                {
                    drivingEnabled = true;
                    EnablePilotMovement(roleManager.GetPilot().connectionToClient);
                    SetFlowText("Great Job!");
                }

                if(playerCntrl.transform.position.z >= powerTutWall.transform.position.z)
                {
                    SetFlowText("These Cells charge your power. Try running into one!");
                    currentState = FlowState.POWER_TUTOR;
                    tutorialCompleted = false;
                    foreach (CellMoveNetwork cellCntrl in shootTutWall.GetComponentsInChildren<CellMoveNetwork>())
                    {
                        cellCntrl.ReturnCellToPool();
                    }
                }
                break;
            case FlowState.POWER_TUTOR:
                if (playerCntrl.transform.position.z >= startedWall.transform.position.z)
                {
                    if (!tutorialCompleted)
                    {
                        SetShipPosition(roleManager.GetPilot().connectionToClient, powerTutWall.transform.position + new Vector3(0f, 0f, 50f));
                    }
                    else
                    {
                        StartCoroutine(EndingTutorialCoroutine());
                        currentState = FlowState.STARTED;
                        tutorialCompleted = false;
                        inTutorial = false;

                        foreach (CellMoveNetwork cellCntrl in healTutWall.GetComponentsInChildren<CellMoveNetwork>())
                        {
                            cellCntrl.ReturnCellToPool();
                        }
                    }
                }
                else
                {
                    if (tutorialCompleted)
                    {
                        SetFlowText("Great Job!");
                    }
                }
                break;
            default:
                break;
        }
    }


    IEnumerator StartingGameCoroutine()
    {

        for(int i = 3; i > 0; i--)
        {
            SetFlowText(string.Format("All players joined! Starting in {0}...", i));
            yield return new WaitForSeconds(1f);
        }
        SetFlowText("Welcome to COVID Combat!");
        drivingEnabled = true;
        EnablePilotMovement(roleManager.GetPilot().connectionToClient);
    }

    IEnumerator EndingTutorialCoroutine()
    {
        SetFlowText("You've completed the tutorial!");
        yield return new WaitForSeconds(3f);
        SetFlowText("Kill 50 Covid viruses to win! Good Luck!");
        yield return new WaitForSeconds(3f);
        SetFlowText("");
        cellSpawn.distSpawnEnabled = true;
        cellSpawn.timeSpawnEnabled = true;
    }


    public void SetFlowText(string newText)
    {
        currentFlowText = newText;
        flowText.text = newText;
        RpcSetFlowText(newText);
 
    }

    [ClientRpc]
    public void RpcSetFlowText(string newText)
    {
        flowText.text = newText;
    }



    [TargetRpc]
    public void EnablePilotMovement(NetworkConnection target)
    {
        pilotControls.drivingEnabled = true;
    }


    [TargetRpc]
    public void DisablePilotMovement(NetworkConnection target)
    {
        pilotControls.drivingEnabled = false;
    }


    [TargetRpc]
    public void EnableShooting(NetworkConnection target)
    {
        shooterControls.shootingEnabled = true;
    }

    [TargetRpc]
    public void DisableShooting(NetworkConnection target)
    {
        shooterControls.shootingEnabled = false;
    }

    [TargetRpc]
    public void SetShipPosition(NetworkConnection target, Vector3 pos)
    {
        playerCntrl.transform.position = pos;
        playerCntrl.transform.localRotation = Quaternion.identity;
    }

    [Command(requiresAuthority =false)]
    public void CmdSetFlowText()
    {
        RpcSetFlowText(currentFlowText);
    }
}
