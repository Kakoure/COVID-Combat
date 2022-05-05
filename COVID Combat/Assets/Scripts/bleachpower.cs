using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class bleachpower : NetworkBehaviour
{
   // [SerializeField] FlashImage _flashImage = null;
   // [SerializeField] Color _newColor = Color.red;
    public float dis = 1000f;
    public Slider slider;
    public GameObject chargedParticles;
    public Image bleachImg;
    public LayerMask killMask;
    public CameraShake camShake;
    public Image flashImage;
    [SerializeField]
    AudioClip powerSound;

    bool isReady;

    // Start is called before the first frame update
    public void unleashbleach()
    {
        if (!isServer)
        {
            return;
        }
        RpcBleachSequence();
        Collider[] hitCells = Physics.OverlapSphere(transform.position, dis, killMask, QueryTriggerInteraction.Collide);
        foreach (Collider cell in hitCells)
        {
            var cellCntrl = cell.gameObject.GetComponent<CellMoveNetwork>();
            cellCntrl.ReturnCellToPool();
            if (cell.CompareTag("virus"))
            {
                GameObject.Find("Score").GetComponent<ScoreTracker>().IncreaseScore();
            }
        }


    }

    private void Update()
    {
        if(!isReady && slider.value == slider.maxValue)
        {
            bleachImg.color = Color.white;
            chargedParticles.SetActive(true);
            isReady = true;
        } else if (isReady && slider.value != slider.maxValue)
        {
            bleachImg.color = Color.gray;
            chargedParticles.SetActive(false);
            isReady = false;
        }
    }

    [ClientRpc]
    void RpcBleachSequence()
    {
        StartCoroutine(BleachFlash());
    }

    IEnumerator BleachFlash()
    {
        GetComponent<AudioSource>().PlayOneShot(powerSound);
        camShake.SetShake(3f);
        for(float i = 1f; i > 0f; i -= .04f)
        {
            flashImage.color = new Color(1f, 1f, 1f, i);
            yield return new WaitForSeconds(.1f);
        }
    }

}
