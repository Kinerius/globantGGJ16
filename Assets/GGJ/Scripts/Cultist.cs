using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cultist : MonoBehaviour {

	public bool isAlive = true;
	// Use this for initialization
	void Start () {
		
	}
	
	public void Kill()
	{
		SoundManager.Instance.Play("muerte");
		isAlive = false;
		Destroy(this.gameObject);
	}


}
