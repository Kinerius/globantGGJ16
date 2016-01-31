using UnityEngine;
using System.Collections;
 
public class Effect : MonoBehaviour
{
    public ParticleSystem emitter;
	public bool manualKill = false;
   
	void Update()
	{
		if ( !emitter.isPlaying && !manualKill )
		{
			Kill();
		}
	}

	void Kill()
	{
		ObjectPool.instance.PoolObject(gameObject);
	}
   
}