using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public float speed = 1f;
    private bool sentEvent = false;
    public System.Action OnPassPipe;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        this.transform.position += Vector3.left * this.speed * Time.fixedDeltaTime;

        if (this.OnPassPipe != null)
        {
            if (this.transform.localPosition.x < 0 && sentEvent == false)
            {

                OnPassPipe();
                this.sentEvent = true;
            }
        }

        if (this.transform.localPosition.x <= -100f)
        {
            Destroy(this.gameObject);
        }
    }
}
