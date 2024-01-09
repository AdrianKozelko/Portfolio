using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Death death;

    // Start is called before the first frame update
    void Start()
    {
        death = GameObject.FindGameObjectWithTag("Death").GetComponent<Death>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {      
            death.RunDeath();
        }
    }
}
