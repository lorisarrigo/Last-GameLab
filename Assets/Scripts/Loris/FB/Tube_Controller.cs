using UnityEngine;

public class Tube_Controller : MonoBehaviour
{
    Tube_Spawner TS;
    private void Update()
    {
        transform.position += Vector3.left * FB_Manager.instance.speed * Time.deltaTime;
        if(transform.position.x < FB_Manager.instance.hideInX)
        {
            TS.ReturnToPool(this);
        }
    }
    public void Initialize(Tube_Spawner tube_Spawner)
    {
        TS = tube_Spawner;
    }
}
