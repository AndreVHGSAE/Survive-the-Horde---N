using UnityEngine;

public class PowerupBase : MonoBehaviour
{
    [SerializeField]
    private AudioSource Pickup;

    //protected solo lo pueden ver el padre u sus hijos
    protected void Update()
	{

    }

	protected void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			//crear un gameObject, su valor es el gameobject de "other" el cual es el collider con el tag de Player;
			GameObject playerGameObject = other.gameObject;
			PowerupEffect(playerGameObject);
			Pickup.Play();
			Destroy(this.gameObject);
		}
	}

	// Cuidar si las colisiones son 2D o 3D
	//Virtual permite poder sobreescribir la lógica función
	protected virtual void PowerupEffect(GameObject playerGameObject)
	{
		Debug.Log("This is not overwritten");
	}
}
