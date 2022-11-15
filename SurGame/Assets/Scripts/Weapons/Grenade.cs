using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public Rigidbody rigidbody;
    public GameObject player;

    public float timer = 500;
    public Transform countdown;

    public Color spriteColor;

    public GameObject explosion;
    public GameObject Cam;

    
    // Start is called before the first frame update
    void Start()
    {
        spriteColor.a = 0f;
        Cam = GameObject.Find("Player/Main Camera");
        rigidbody = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        
        transform.rotation = player.transform.rotation;
        transform.Rotate(Cam.transform.localRotation.eulerAngles.x, 0, 0);
        rigidbody.AddForce(transform.forward*1000);
       // rigidbody.AddForce(transform.up * 300);
    }

    // Update is called once per frame
    void Update()
    {
      countdown.transform.rotation = Quaternion.Euler(0.0f, player.transform.rotation.x, gameObject.transform.rotation.z * -1.0f);
        spriteColor.r = 0.7f;
        spriteColor.g = (timer/500)*0.7f;
        spriteColor.b = (timer / 500)*0.7f;
        if (spriteColor.a <= 0.5f)
        {
            spriteColor.a +=Time.deltaTime/6;
        }
        countdown.GetComponent<SpriteRenderer>().color = spriteColor;
        timer -= 100*Time.deltaTime;
        if (timer <= 0)
        {
            Instantiate(explosion, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }

    }
}
