using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Node : MonoBehaviour{

	//VAR PUB
	[SerializeField]
	private List<Node> neighbors;

	//VAR PRIV
	private Node parent;
	private float distance;
	




	//------------------------------------------------------------>
	// GETs + SETs
	//------------------------------------------------------------>
	public void SetParent(Node parent) => this.parent = parent;
	public void SetDistance(float distance) => this.distance = distance;
	public void SetNeighbors(List<Node> neighbors) => this.neighbors = neighbors;
	public Node GetParent() => parent;
	public float GetDistance() => distance;
	public List<Node> GetNeighbors() => neighbors;





	//------------------------------------------------------------>
	// DEBUG
	//------------------------------------------------------------>
#if UNITY_EDITOR


	private LineRenderer line;

	/// <summary>
	/// Marcar o desmarcar un nodo como parte de la ruta
	/// </summary>
	/// <param name="status"></param>
	public void IsWay(bool status) {
		
		GetComponent<SpriteRenderer>().color = Color.gray;
		if (line == null)
			line = gameObject.AddComponent<LineRenderer>();
		line.enabled = false;

		if (status) {
			gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;

			line.enabled = true;
			line.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
			line.startColor = Color.yellow;
			line.endColor = Color.yellow;
			line.startWidth = 0.1f;
			line.endWidth = 0.1f;
			line.positionCount = 2;
			line.useWorldSpace = true;
			line.SetPosition(0, transform.position);
			line.SetPosition(1, parent.transform.position);

		} 
	}

	//------------------------------------------------------------>

	/// <summary>
	/// Dibujar los gizmos en el editor
	/// </summary>
	void OnDrawGizmos(){

			Handles.Label(transform.position, gameObject.name);

			if (neighbors == null)
				return;
			foreach(var neighbor in neighbors)
			{
				if (neighbor != null) {
					Vector2 auxOrigin = transform.position;
					Vector2 auxTarget = neighbor.transform.position;


					if (auxOrigin.y >= auxTarget.y) {
						Gizmos.color = Color.blue;
					} else {
						Gizmos.color = Color.cyan;
						auxTarget.x += 0.1f;
						auxTarget.y += 0.1f;
						auxOrigin.x += 0.1f;
						auxOrigin.y += 0.1f;
					}
					Gizmos.DrawLine(auxOrigin, auxTarget);

				}
			}
		}
	#endif

}