using UnityEngine;
using System.Collections;
 
public class Effect : MonoBehaviour
{
    public ParticleSystem emitter;
	public Transform parent;
	public bool onParentDisable = false;
	public bool onTimedDisable = false;
	public float disableTime = 0;
   
	void Update()
	{
		if ( !emitter.isPlaying )
		{
			Kill();
		}

		if ( onTimedDisable )
		{
			disableTime -= Time.deltaTime;
			if ( disableTime<0)
			{
				Kill();
			}
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
		emitter.Stop();
		StartCoroutine(StopInSecondsCoroutine());
	}

	IEnumerator StopInSecondsCoroutine()
	{
		yield return new WaitForSeconds(3);
		OnStartPool();
	}

	void OnStartPool()
	{
		gameObject.SetActive(false);
		ObjectPool.instance.PoolObject(gameObject);
	}
   
}