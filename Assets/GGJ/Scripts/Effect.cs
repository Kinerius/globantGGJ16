using UnityEngine;
using System.Collections;
 
public class Effect : MonoBehaviour
{
    public ParticleSystem emitter;
	public Transform parent;
	public bool onParentDisable = false;
   
	void Update()
	{
		if ( !emitter.isPlaying )
		{
			Kill();
		}

		if (parent != null)
		{
			this.transform.position = parent.position;
			if (!parent.gameObject.activeSelf && onParentDisable )
			{
				Kill();
			}

		} else {
			if ( onParentDisable )
			{
				Kill();
			}
		}

		

		
	}

	void Kill()
	{
		gameObject.SetActive(false);
		ObjectPool.instance.PoolObject(gameObject);
	}
   
}