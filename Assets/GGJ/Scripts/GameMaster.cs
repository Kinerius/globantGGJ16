using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
	public float castTime = 0;
	public float cooldownTime = 0;
	public float cooldownBonusPerFollower = 0;
	public float maxFollowers = 0;
	public float followerTimer = 0;
	public float followerPenalization = 0;
}

public class GameMaster : MonoBehaviour {

	public PlayerEntity player_1;
	public PlayerEntity player_2;
	// Use this for initialization

	[SerializeField]
	public PlayerData data;
/*
	public float castTime = 0;
	public float cooldownTime = 0;
	public float cooldownBonusPerFollower = 0;
	public float maxFollowers = 0;
	public float followerTimer = 0;
	public float followerPenalization = 0;*/

	void Start () {
		player_1.data = data;
		player_2.data = data;
	}
	bool someoneWon = false;
	void Update()
	{
		if ( someoneWon )
			return;

		if ( player_1.cultistList.Count == 0)
		{
			GameObject confeti = ObjectPool.instance.GetObjectForType("confeti",true);
			confeti.transform.position = player_2.spawnPointTransform.transform.position;
			someoneWon = true;
		}	

		if ( player_2.cultistList.Count == 0)
		{
			GameObject confeti = ObjectPool.instance.GetObjectForType("confeti",true);
			confeti.transform.position = player_1.spawnPointTransform.transform.position;
			someoneWon = true;
		}	

	}

}
