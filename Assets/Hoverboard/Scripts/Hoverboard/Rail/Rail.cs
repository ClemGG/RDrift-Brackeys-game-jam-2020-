using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RailWay { OneWay, TwoWays };
public enum RailMovement { Constant, GravityOnly, ConstantPlusGravity };

public class Rail : MonoBehaviour
{

	[Space(10)]
	[Header("Rail Settings")]
	[Space(10)]

	[SerializeField] RailWay railWay;
	[SerializeField] RailMovement railMovement;


}
