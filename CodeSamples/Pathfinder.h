#pragma once
#include "math/ofVec2f.h"


struct Node;
struct Edge;

//Main class 
struct Node {
public:
	Node();
	Node(Node* node);
	Node(int num, ofVec2f vert = ofVec2f(0, 0), int cost = 1);



	//Min distance for the node to be considered reached
	float CircleRadius = 1.0f;

	//Is this node Accessible
	bool isCleared;

	//ID Value
	int NodeNum;

	//Position
	ofVec2f Vertex;

	//Array of data for connecting nodes
	std::vector<Edge> Edges;

	//Pointer to previous node, for pathfinding purposes
	Node* Previous;

	void AddChild(Edge edge);
	void ClearChild(int num);

	float NodeR = 255, NodeG = 255;
	float NodeB = 0;

	float NumTilesFromStart = 0;

	//G is Base cost
	//H is Heuristic cost
	//F is final cost
	float G, H, F;

	void HighlightNode();
	void HighlightNodePurple();
	void UnHighlightNode();

	static void AddChild(std::vector<Edge> vec, Edge edge);
	static void AddChild(Node* src, Node* snk);
	void DrawPoint();
	std::vector<Node*> GetChildNodes();
	const float GetDistanceToNodeManhatten(Node* Start, Node* ChildNode) const;
	const float GetDistanceToNodeEuclidean(Node* Start, Node* ChildNode) const;
	void GetManhattenDistance(Node* Start, Node* End, Node* ChildNode);
	void GetEuclideanDistance(Node* Start, Node* End, Node* ChildNode);
	bool CompareManhatten(Node i1, Node i2);

	Edge GetEdge(Node* node);


	//Overloads
	bool operator < (const Node& str) const
	{
		return (F < str.F);
	}

	bool operator > (const Node& str) const
	{
		return (F > str.F);
	}

	bool operator == (const Node& str) const
	{
		return (NodeNum == str.NodeNum);
	}


};

//Data containing pointers connecting nodes
struct Edge {
	float weight;
	Node *source;
	Node *sink;
public:

	Edge(Node* src, Node* snk, float w = 1);
	Edge();

	float r = 0, g = 255, b = 255;
	bool IsPath = false;
	float GetWeight();
	Node* GetSource();
	Node* GetSink();

	void SetHighlighted();
	void SetNormalColor();
	void DrawEdge();


};

const int Length = 80, Width = 60;
const int TileSize = 15;

class Graph
{
public:

	ofVec2f TilePoint = ofVec2f(0, 0);
	Node* TileMap[Length][Width] = { { } };
	Node* EndNode;

	//Tell boid to seek the sink of the current edge.
	//Upon reaching said sink, increment num and seek next sink
	std::vector<Edge> CurrentPath; 
	int NumNodesToFollow;

	Graph();


	//Main Algorithm
	std::vector<Edge> GetPathAStar(Node* start, Node* end);

	//Unfinished test algorithm
	std::vector<Edge> GetPathDjikstra(Node* start, Node* end);

	//Takes the final calculated path and reverses it, allowing it to be followed
	std::vector<Edge> GetPath(Node* start, Node* end);

	//Remove tiles from pathfinding consideration
	void CutOutRow(int start, int length, int column);
	void CutOutColumn(int start, int length, int row);
	void CutOutTile(Node* node);


	void FindEndNode(ofVec2f point);
	Node* GetNearestNode(ofVec2f point);
	Node* GetNodeInList(int num, std::vector<Node*> nodes);
	void PrintGraph();
	static void ColorPath(std::vector<Edge> path);
	void UnColorPath();
	static void ClearColorPath(std::vector<Edge> path);
	static float FindLowestF(std::vector<Node*> nodes);
	static float FindLowestG(std::vector<Node*> nodes);
	static int FindLowestFIndex(std::vector<Node*> nodes);
	
};

