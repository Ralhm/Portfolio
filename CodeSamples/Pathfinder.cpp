#include "Pathfinder.h"
#include "ofLog.h"
#include "ofApp.h"
#include <algorithm>
#include <vector>
#include "graphics/ofGraphics.h"
#include "math/ofVec2f.h"

Edge::Edge(Node* src, Node* snk, float w) {
	weight = w;
	source = src;
	sink = snk;
}

Edge::Edge() {
	weight = 1;
	source = new Node();
	sink = new Node();
}

float Edge::GetWeight() {
	return weight;
}
Node* Edge::GetSource() {
	return source;
}
Node* Edge::GetSink() {
	return sink;
}

void Edge::SetHighlighted() {
	r = 255;
	g = 0;
	b = 0;
}
void Edge::SetNormalColor() {
	r = 0;
	g = 255;
	b = 255;
}

void Edge::DrawEdge() {
	ofColor color = ofColor(r, g, b);
	ofSetColor(color);
	ofDrawLine(source->Vertex, sink->Vertex);

}

Node::Node() {

	NodeNum = 1;
	Vertex = ofVec2f(0,0);
	isCleared = false;
	NodeR = 255;
	NodeG = 255; 
	NodeB = 0;
}

Node::Node(Node* node) {

	NodeNum = node->NodeNum;
	Vertex = node->Vertex;
	isCleared = node->isCleared;
	NodeR = 255;
	NodeG = 255;
	NodeB = 0;
}

Node::Node(int num, ofVec2f vert, int cost) {

	NodeNum = num;
	Vertex = vert;
	isCleared = false;
	NodeR = 0;
	NodeG = 255;
	NodeB = 255;
}

void Node::HighlightNode() {
	NodeR = 255;
	NodeG = 0;
	NodeB = 0;
}

void Node::HighlightNodePurple() {
	NodeR = 255;
	NodeG = 0;
	NodeB = 255;
}

void Node::UnHighlightNode() {
	NodeR = 0;
	NodeG = 255;
	NodeB = 255;
}


void Node::ClearChild(int num) {
	for (int i = 0; i < Edges.size(); i++) {
		if (Edges.at(i).GetSink()->NodeNum == num) {
			Edges.erase(Edges.begin() + i);
			break;
		}
	}
	
}

void Node::AddChild(Edge edge) {
	Edges.push_back(edge);
}

void Node::AddChild(Node* src, Node* snk) {
	Edge* edge = new Edge(src, snk);
	src->Edges.push_back(*edge);
}

void Node::AddChild(std::vector<Edge> vec, Edge edge) {
	vec.push_back(edge);
}


const float Node::GetDistanceToNodeManhatten(Node* Start, Node* ChildNode) const {
	return abs(Start->Vertex.x - ChildNode->Vertex.x) + abs(Start->Vertex.y - ChildNode->Vertex.y);
}

const float Node::GetDistanceToNodeEuclidean(Node* Start, Node* ChildNode) const {
	return sqrt(pow(Start->Vertex.x - ChildNode->Vertex.x, 2) + pow(Start->Vertex.y - ChildNode->Vertex.y, 2));
}

void Node::GetManhattenDistance(Node* Start, Node* End, Node* ChildNode) {
	G = NumTilesFromStart * TileSize;
	H = GetDistanceToNodeManhatten(End, ChildNode);
	F = G + H;
	

	/*

	ofLog(OF_LOG_NOTICE, "Current G Distance: " + ofToString(G));
	ofLog(OF_LOG_NOTICE, "Current H Distance: " + ofToString(H));
	ofLog(OF_LOG_NOTICE, "Current Manhatten Distance: " + ofToString(F));

	*/
}

void Node::GetEuclideanDistance(Node* Start, Node* End, Node* ChildNode) {
	G = NumTilesFromStart * TileSize;
	H = GetDistanceToNodeEuclidean(End, ChildNode);
	F = G + H;

}

void Node::DrawPoint() {
	ofColor color = ofColor(NodeR, NodeG, NodeB);
	ofSetColor(color);
	ofDrawCircle(Vertex.x, Vertex.y, CircleRadius);
	
}

bool Node::CompareManhatten(Node i1, Node i2) {
	return (i1.F > i2.F);
}

std::vector<Node*> Node::GetChildNodes() {
	std::vector<Node*> children;
	for (int i = 0; i < Edges.size(); i++) {
		children.push_back(Edges.at(i).GetSink());
		
	}
	sort(children.begin(), children.end());
	return children;
}

Edge Node::GetEdge(Node* node) {
	for (int i = 0; i < Edges.size(); i++) {
		if (Edges.at(i).GetSink()->NodeNum == node->NodeNum) {
			return Edges.at(i);
		}
	}
}

Graph::Graph() {
	int NodeNum = 1;
	for (int i = 0; i < Length; i++) {
		for (int j = 0; j < Width; j++) {
			TileMap[i][j] = new Node(NodeNum, TilePoint);
			TilePoint.x += TileSize;
			NodeNum++;
		}
		TilePoint.x = 0;
		TilePoint.y += TileSize;
	}

	for (int i = 0; i < Length; i++) {
		for (int j = 0; j < Width; j++) {
			if (i != 0) { //Connect to tile above

				Node::AddChild(TileMap[i][j], TileMap[i - 1][j]);
			}
			if (i != Length - 1) { //Connect to tile below

				Node* src = TileMap[i][j];
				Node* snk = TileMap[i + 1][j];

				Node::AddChild(TileMap[i][j], TileMap[i + 1][j]);
			}
			if (j != 0) { // Connect to tile to left


				Node::AddChild(TileMap[i][j], TileMap[i][j - 1]);
			}
			if (j != Width - 1) { // Connect to tile to right


				Node::AddChild(TileMap[i][j], TileMap[i][j + 1]);
			}

		}

	}

	//Obstacle Creation
	//An admittedly nasty method, but suitable for the scope of this project
	{

		CutOutTile(TileMap[5][5]);
		CutOutTile(TileMap[5][6]);
		CutOutTile(TileMap[5][7]);
		CutOutTile(TileMap[5][8]);
		CutOutTile(TileMap[5][9]);
		CutOutTile(TileMap[5][10]);
		CutOutTile(TileMap[6][10]);
		CutOutTile(TileMap[7][10]);
		CutOutTile(TileMap[8][10]);
		CutOutTile(TileMap[8][11]);
		CutOutTile(TileMap[8][12]);
		CutOutTile(TileMap[8][13]);
		CutOutTile(TileMap[8][14]);
		CutOutTile(TileMap[8][15]);
		CutOutTile(TileMap[8][16]);

		CutOutTile(TileMap[6][5]);
		CutOutTile(TileMap[7][5]);
		CutOutTile(TileMap[8][5]);
		CutOutTile(TileMap[9][5]);
		CutOutTile(TileMap[10][5]);


		CutOutTile(TileMap[15][15]);
		CutOutTile(TileMap[15][16]);
		CutOutTile(TileMap[15][17]);
		CutOutTile(TileMap[16][15]);
		CutOutTile(TileMap[16][16]);
		CutOutTile(TileMap[16][17]);

		CutOutTile(TileMap[15][10]);
		CutOutTile(TileMap[15][11]);
		CutOutTile(TileMap[15][12]);
		CutOutTile(TileMap[16][10]);
		CutOutTile(TileMap[16][11]);
		CutOutTile(TileMap[16][12]);

		CutOutTile(TileMap[15][4]);
		CutOutTile(TileMap[15][5]);
		CutOutTile(TileMap[15][6]);
		CutOutTile(TileMap[16][4]);
		CutOutTile(TileMap[16][5]);
		CutOutTile(TileMap[16][6]);

	}

}


std::vector<Edge> Graph::GetPathAStar(Node* start, Node* end) {
	std::vector<Node*> ClosedList;
	std::vector<Node*> OpenList;

	start->GetManhattenDistance(start, end, start);


	OpenList.push_back(start);

	bool GoalFound = false;
	int currentIndex = 0;
	while (!OpenList.empty()) {

		if (GoalFound) {
			break;
		}

		int LowestFIndex = Graph::FindLowestFIndex(OpenList);
		Node* CurrentNode = OpenList.at(LowestFIndex);


		if (CurrentNode->NodeNum == end->NodeNum) {
			GoalFound = true;
			break;
		}
		/*
		ofLog(OF_LOG_NOTICE, "End Node Num: " + ofToString(end->NodeNum));
		ofLog(OF_LOG_NOTICE, "Current Node Num: " + ofToString(CurrentNode->NodeNum));
		ofLog(OF_LOG_NOTICE, "Current Node Manhatten Distance: " + ofToString(CurrentNode->F));
		ofLog(OF_LOG_NOTICE, "OpenList Size: " + ofToString(OpenList.size()));
		*/
		
		ClosedList.push_back(CurrentNode);


		std::vector<Node*> ChildNodes = CurrentNode->GetChildNodes();

		for (int i = 0; i < ChildNodes.size(); i++) { //Generate Children
			ChildNodes.at(i)->NumTilesFromStart = CurrentNode->NumTilesFromStart + 1;
			//ChildNodes.at(i)->GetManhattenDistance(start, end, ChildNodes.at(i));
			ChildNodes.at(i)->GetEuclideanDistance(start, end, ChildNodes.at(i));

			//Check if child is unique. don't add if it's not	
			if (std::find(ClosedList.begin(), ClosedList.end(), ChildNodes.at(i)) != ClosedList.end()) {
				Node* nearestNode = GetNodeInList(ChildNodes.at(i)->NodeNum, ClosedList);
				if (nearestNode->G <= ChildNodes.at(i)->G) {
					continue;
				}
				ClosedList.erase(std::find(ClosedList.begin(), ClosedList.end(), ChildNodes.at(i)));
			}


			if (std::find(OpenList.begin(), OpenList.end(), ChildNodes.at(i)) != OpenList.end()) {
				Node* nearestNode = GetNodeInList(ChildNodes.at(i)->NodeNum, OpenList);
				if (CurrentNode->G <= ChildNodes.at(i)->G) {
					continue;
				}

			}				
			/*
			float lowestOpenF = 0;			
			if (OpenList.size() > 0) {
				lowestOpenF = Graph::FindLowestF(OpenList);
				if (lowestOpenF < ChildNodes.at(i)->F) {
					continue;
				}
			}

			float LowestClosedF = 0;
			if (ClosedList.size() > 0) {
				LowestClosedF = Graph::FindLowestF(ClosedList);
				if (LowestClosedF < ChildNodes.at(i)->F) {
					continue;
				}
			}

			float lowestOpenG = 0;
			if (OpenList.size() > 0) {
				lowestOpenG = Graph::FindLowestG(OpenList);
				if (lowestOpenG < ChildNodes.at(i)->G) {
					continue;
				}
			}
			*/
			
			//If Unique Child, Add
			if (std::find(OpenList.begin(), OpenList.end(), ChildNodes.at(i)) == OpenList.end()) {
				ChildNodes.at(i)->Previous = CurrentNode;
				OpenList.push_back(ChildNodes.at(i)); 
			}
	
			ChildNodes.at(i)->HighlightNodePurple();

		}
		OpenList.erase(OpenList.begin() + LowestFIndex);
		ClosedList.push_back(CurrentNode);

	}


	ofLog(OF_LOG_NOTICE, "Open list size: " + ofToString(OpenList.size()));

	std::vector<Edge> path = GetPath(start, end);
	return path;

}

//Unfinished
std::vector<Edge> Graph::GetPathDjikstra(Node* start, Node* end) {
	float distance = 0;
	for (int i = 0; i < Length; i++) {
		for (int j = 0; j < Width; j++) {
			if (TileMap[i][j]->NodeNum != start->NodeNum) {

			}
		}
		
	}



	std::vector<Edge> path;
	return path;
}

//Takes the final calculated path and reverses it, allowing it to be followed
std::vector<Edge> Graph::GetPath(Node* start, Node* end) {
	std::vector<Edge> FinalPath;

	Node* CurrentNode = end;

	while (CurrentNode->NodeNum != start->NodeNum) {
		FinalPath.push_back(CurrentNode->GetEdge(CurrentNode->Previous));
		CurrentNode = CurrentNode->Previous;
	}

	std::reverse(FinalPath.begin(), FinalPath.end());
	return FinalPath;
}

float Graph::FindLowestF(std::vector<Node*> nodes) {
	float LowestF = nodes.at(0)->F;
	for (int i = 0; i < nodes.size(); i++) {
		if (nodes.at(i)->F < LowestF) {
			LowestF = nodes.at(i)->F;
		}
	}
	return LowestF;
}

float Graph::FindLowestG(std::vector<Node*> nodes) {
	float LowestCost = nodes.at(0)->G;
	for (int i = 0; i < nodes.size(); i++) {
		if (nodes.at(i)->G < LowestCost) {
			LowestCost = nodes.at(i)->G;
		}
	}
	return LowestCost;
}

int Graph::FindLowestFIndex(std::vector<Node*> nodes) {
	float LowestF = nodes.at(0)->F;
	int index = 0;
	for (int i = 0; i < nodes.size(); i++) {
		if (nodes.at(i)->F < LowestF) {
			LowestF = nodes.at(i)->F;
			index = i;
		}
	}
	return index;
}



void Graph::PrintGraph() {
	for (int i = 0; i < Length; i++) {
		for (int j = 0; j < Width; j++) {
			for (int k = 0; k < TileMap[i][j]->Edges.size(); k++) {
				//TileMap[i][j]->Edges.at(k).DrawEdge();
			}

			if (!TileMap[i][j]->isCleared) {
				TileMap[i][j]->DrawPoint();
			}


		}

	}
}

void Graph::UnColorPath() {
	for (int i = 0; i < Length; i++) {
		for (int j = 0; j < Width; j++) {


			TileMap[i][j]->UnHighlightNode();


		}

	}
}

//This whole method feels real nasty but I'm not familiar enough with openframeworks to make something better
//For the scope of this project, and to meet my deadline, it will suffice
void Graph::FindEndNode(ofVec2f point) {
	Node* NearestNode = TileMap[0][0];
	float NearestDist = (point - NearestNode->Vertex).length();
	for (int i = 0; i < Length; i++) {
		for (int j = 0; j < Width; j++) {
			if (!TileMap[i][j]->isCleared) {
				ofVec2f Dist = point - TileMap[i][j]->Vertex;
				if (Dist.length() < NearestDist) {
					NearestNode = TileMap[i][j];
					NearestDist = Dist.length();
				}
			}

		}
	}

	//if (EndNode != nullptr) {
	//	EndNode->UnHighlightNode();
	//}
	
	EndNode = NearestNode;
	EndNode->HighlightNode();


}

Node* Graph::GetNearestNode(ofVec2f point) {
	Node* NearestNode = TileMap[0][0];
	float NearestDist = (point - NearestNode->Vertex).length();
	for (int i = 0; i < Length; i++) {
		for (int j = 0; j < Width; j++) {
			ofVec2f Dist = point - TileMap[i][j]->Vertex;
			if (Dist.length() < NearestDist) {
				NearestNode = TileMap[i][j];
				NearestDist = Dist.length();
			}
		}
	}

	return NearestNode;


}

Node* Graph::GetNodeInList(int num, std::vector<Node*> nodes) {
	for (int i = 0; i < nodes.size(); i++) {
		if (nodes.at(i)->NodeNum == num) {
			return nodes.at(i);
		}
	}
	return nullptr;
}

void Graph::ColorPath(std::vector<Edge> path) {
	for (int i = 0; i < path.size(); i++) {
		//path.at(i).SetHighlighted();
		//ofLog(OF_LOG_NOTICE, "PathSource: " + ofToString(path.at(i).GetSource()->NodeNum));
		//ofLog(OF_LOG_NOTICE, "PathSink: " + ofToString(path.at(i).GetSink()->NodeNum));
		

		path.at(i).GetSink()->HighlightNode();


	}
}

void Graph::ClearColorPath(std::vector<Edge> path) {
	for (int i = 0; i < path.size(); i++) {
		//path.at(i).SetHighlighted();

		path.at(i).GetSink()->UnHighlightNode();


	}
}

void Graph::CutOutTile(Node* node) {
	for (int i = 0; i < node->Edges.size(); i++) {


		node->Edges.at(i).sink->ClearChild(node->NodeNum);
		node->Edges.at(i).sink = nullptr;
		node->Edges.at(i).source = nullptr;

	}
	node->isCleared = true;
	node->Edges.clear();
	
}


void Graph::CutOutRow(int start, int length, int column) {
	for (int i = start; i < start + length; i++) {
		CutOutTile(TileMap[column][i]);
	}
}

void Graph::CutOutColumn(int start, int length, int row) {
	for (int i = start; i < start + length; i++) {
		CutOutTile(TileMap[i][row]);
	}
}