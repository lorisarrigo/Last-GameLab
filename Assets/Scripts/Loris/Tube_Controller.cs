using System.IO.Pipes;
using UnityEngine;

public class Tube_Controller : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float hideInX;
    Tube_Spawner TS;
    private void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
        if(transform.position.x < hideInX)
        {
            TS.ReturnToPool(this);
        }
    }
    public void Initialize(Tube_Spawner tube_Spawner)
    {
        TS = tube_Spawner;
    }
}
