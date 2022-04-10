using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class highlight : MonoBehaviour
{
    Outline outline;
    // Start is called before the first frame update
    void Start()
    {
        outline = gameObject.GetComponent<Outline>();
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "rbc":
                {
                    outline.enabled = true;
                    outline.OutlineColor = Color.red;
                    outline.OutlineWidth = 5f;
                    
                    break;
                }
            case "mbc":
                {
                    outline.enabled = true;
                    outline.OutlineColor = Color.yellow;
                    outline.OutlineWidth = 5f;
                    
                    break;
                }
            case "bbc":
                {
                    outline.enabled = true;
                    outline.OutlineColor = Color.blue;
                    outline.OutlineWidth = 5f;
                    Destroy(other.gameObject);
                    
                    break;
                }
            case "virus":
                {
                    outline.enabled = true;
                    outline.OutlineColor = Color.green;
                    outline.OutlineWidth = 5f;
                   
                    break;
                }
            default:
                {
                    outline.enabled = false;
                    break;
                }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        outline.enabled = false;
    }
}
