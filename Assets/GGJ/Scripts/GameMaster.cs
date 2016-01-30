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
	
	

}
