using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour {
	private void OnEnable()
	{
		LevelManager.OnEnterGame += Open;
	}

	private void OnDisable()
	{
		LevelManager.OnEnterGame -= Open;
	}
	
	void Open()
	{
		StartCoroutine(StartAnim());
	}

	IEnumerator StartAnim()
	{
		yield return  new WaitForSeconds(2f);
		
		iTween.MoveTo(gameObject, iTween.Hash("position", transform.position + Vector3.right*3, "time",1f));
		
	}
}
