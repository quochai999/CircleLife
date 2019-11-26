using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    private float defaultSpeed;
    public float speed;
    public float speedIncreaseByTime;

    // Start is called before the first frame update
    void Start()
    {
        defaultSpeed = speed;
        Rotating();
    }

    public float GetDefaultSpeed()
    {
        return defaultSpeed;
    }

    private async void Rotating()
    {
        Quaternion quaternion = transform.localRotation;
        while (true)
        {
            if (transform == null) break;
            transform.Rotate(0, 0, speed * Time.deltaTime);
            await Task.Delay(10);
        }
    }
}
