using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour{

	//VAR PUB
	public Transform targetPosition;

	//VAR PRIV
	private Node[] allNodes;

	//------------------------------------------------------------>

	/// <summary>
	/// Ejecucion al iniciar el script
	/// </summary>	
	private void Start() {
		//Obtener todos los nodos de la escena
		GameObject[] auxAllNodes = GameObject.FindGameObjectsWithTag("Node");
		allNodes = new Node[auxAllNodes.Length];
		for (int i = 0; i < auxAllNodes.Length; i++) {
			allNodes[i] = auxAllNodes[i].GetComponent<Node>();
		}
    }

    //------------------------------------------------------------>

    /// <summary>
    /// Empleando el algoritmo A* se realiza la busqueda del path más optimo hasta llegar al nodo mas proximo a la posición de destino.
    /// 
    /// Funcionamiento:
    /// Partiendo del analisis del nodo mas proximo a la posicion de origen se establecera para cada uno de sus nodos vecinos una puntuación basada en la distancia desde estos hasta el nodo destino,
    /// de este modo se seleccionara el que tenga una puntuacion mas optima (mas cercano al objetivo) y pasara a analizarse este. Repitiendo este proceso continuamente finalmente se alcanzara el nodo objetivo o destino.
    /// Una vez llegado al final se podra reconstruir el camino a seguir ya que cada nodo almacenara el padre desde el cual se ha accedido al mismo.
    /// </summary>
    /// <param name="destination"></param>
    public Stack<Node> NavigateTo(Vector2 destination){

		//1. Inicializacion
		Stack<Node> path = new Stack<Node>();
		Node currentNode = FindClosestNode(transform.position); //Inicialmente el nodo más cercano al inicio
		Node endNode = FindClosestNode(destination);

		//Verificar existencia de inicio, final y diferencia entre ambos
		if (currentNode == null || endNode == null || currentNode == endNode)
			return path;
		
		//Lista abierta, almacena nodos para analizar (busqueda del vecino mas optimo)
		SortedList<float, Node> openList = new SortedList<float, Node>();
		//Lista cerrada, almacena nodos ya analizados
		List<Node> closedList = new List<Node> ();

		//Parametros del primer nodo
		openList.Add (0, currentNode);
		currentNode.SetParent(null);
		currentNode.SetDistance(0f);

		//2. Analisis de nodos hasta llegar al final
		while (openList.Count > 0){

			//Obtener el de la lista ordenada el valor mas cercano
			currentNode = openList.Values[0];
			openList.RemoveAt (0);			
			float dist = currentNode.GetDistance();
			closedList.Add (currentNode);

			//Finalizar al llegar al objetivo
			if (currentNode == endNode)
				break;
			
			//Recorrer nodos vecinos
			foreach (Node neighbor in currentNode.GetNeighbors()){

				//Ignorar analisis si ya han sido analizado (lista cerrada) o esta pendiente (lista abierta) 
				if (closedList.Contains (neighbor) || openList.ContainsValue (neighbor))
					continue;

				//Almacenar el nodo vecino en la lista abierta ordenado por distancia
				neighbor.SetParent(currentNode);
				neighbor.SetDistance(dist + Vector2.Distance(neighbor.transform.position, currentNode.transform.position));
				float distanceToTarget = Vector2.Distance(neighbor.transform.position, endNode.transform.position);
				openList.Add (neighbor.GetDistance() + distanceToTarget, neighbor);
			}
		}

		//3. Se ha encontrado el camino hasta el nodo final. 
		if (currentNode == endNode){
			//Recorrer hacia atrás los nodos seleccionados a traves de su parametro previus y formar la pila de recorrido a seguir
			while (currentNode.GetParent() != null){
				path.Push (currentNode);
				currentNode = currentNode.GetParent();
			}
			//path.Push(endNode);
		}

		return path;
	}

	//------------------------------------------------------------>

	/// <summary>
	/// En el update se consigue que el objeto que tiene asociado este script de desplace
	/// </summary>
	void Update(){
		foreach (Node node in allNodes) {
			node.IsWay(false);
		}

		Stack<Node> stack = NavigateTo(targetPosition.position);
		while (stack.Count > 0) {
			Node node = stack.Pop();
			node.IsWay(true);
		}
	}

	//------------------------------------------------------------>

	/// <summary>
	/// Encuentra el waypoint más cercano respecto a la posicion pasada por parametro
	/// </summary>
	/// <param name="target"></param>
	/// <returns></returns>
	private Node FindClosestNode(Vector2 targetPosition){
		Node closest = null;
		float minDist = float.MaxValue;

		for (int i = 0; i < allNodes.Length; i++) {
			float dist = Vector2.Distance(allNodes[i].transform.position, targetPosition);
			if (dist < minDist) {
				minDist = dist;
				closest = allNodes[i];
			}
		}

		return closest;
	}
}