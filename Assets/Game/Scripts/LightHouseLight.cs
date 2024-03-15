using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHouseLight : MonoBehaviour
{
    [SerializeField] float speed = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 10 * speed * Time.deltaTime, 0);
        
    }
}
