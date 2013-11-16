using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AStarPathfinding
{
    /// <summary>

    /// Reresents one node in the search space

    /// </summary>

    class SearchNode
    {

        /// <summary>

        /// Location on the map

        /// </summary>

        public Microsoft.Xna.Framework.Point Position;

        /// <summary>

        /// If true, this tile can be walked on.

        /// </summary>

        public bool Walkable;


        /// <summary>

        /// This contains references to the for nodes surrounding 

        /// this tile (Up, Down, Left, Right).

        /// </summary>

        public SearchNode[] Neighbors;

        /// <summary>
        /// A reference to the node that transfered this node to
        /// the open list. This will be used to trace our path back
        /// from the goal node to the start node.
        /// </summary>
        public SearchNode Parent;

        /// <summary>
        /// Provides an easy way to check if this node
        /// is in the open list.
        /// </summary>
        public bool InOpenList;
        /// <summary>
        /// Provides an easy way to check if this node
        /// is in the closed list.
        /// </summary>
        public bool InClosedList;

        /// <summary>
        /// The approximate distance from the start node to the
        /// goal node if the path goes through this node. (F)
        /// </summary>
        public float DistanceToGoal;
        /// <summary>
        /// Distance traveled from the spawn point. (G)
        /// </summary>
        public float DistanceTraveled;

    }


    /// <summary>
    /// A physical representation of the map.
    /// </summary>
   /* public class Map
    {
        private float[,] layout;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="map"></param>
        public Map(float[,] map)
        {
            layout = map;
        }

        private List<Texture2D> textures;

        /// <summary>
        /// The width of the map.
        /// </summary>
        public int Width
        {
            get { return layout.GetLength(1); }
        }
        /// <summary>
        /// The height of the map.
        /// </summary>
        public int Height
        {
            get { return layout.GetLength(0); }
        }

        /// <summary>
        /// Sets the textures for the map to draw.
        /// </summary>
        public void SetTextures(List<Texture2D> textures)
        {
            this.textures = textures;
        }

        /// <summary>
        /// Returns the tile index for the given cell.
        /// </summary>
        public float GetIndex(int cellX, int cellY)
        {
            if (cellX < 0 || cellX > Width - 1 || cellY < 0 || cellY > Height - 1)
                return 0;

            return layout[cellY, cellX];
        }

        public float GetLandType(int x, int y)
        {
            return layout[y, x];
        }

        /// <summary>
        /// In the below Draw() function, implement draw to iterate
        /// through each index and draw the texture that coorelates with the value
        /// returned by that index in the map.
        /// </summary>
         //public void Draw(SpriteBatch spriteBatch)
         //{
                //Do nothing if texures list is empty
         //    if (textures == null)
         //    {
         //        return;
         //    }

         //    //iterate through each of the "tiles" in the map and draw the texture at that index
                //coorelates to
         //    for (int x = 0; x < Width; x++)
         //    {
         //        for (int y = 0; y < Height; y++)
         //        {
         //            //since each value retuned by layout[y,x] is a float
         //            //setup a switch statement to 
         //            int index = layout[y, x];
         //            switch x
         //             {
         //                
         //             }
         //
         //            spriteBatch.Draw(textures[index], new Vector2(x, y) 
         //                * Global.TileSize, Color.White);
         //        }
         //    }
         //}
    }*/

    public class Pathfinder
    {
        // Stores an array of the walkable search nodes.

        private SearchNode[,] searchNodes;

        private Map mapGrid;

        // The width of the map.

        private int levelWidth;

        // The height of the map.

        private int levelHeight;

        private float heuristic;

        public float Heuristic
        {
            get
            {
                return heuristic;
            }
        }


        /// <summary>

        /// Constructor.

        /// </summary>

        public Pathfinder(Map map)
        {
            mapGrid = map;
            levelWidth = map.Width;

            levelHeight = map.Height;

            InitializeSearchNodes(map);

        }

        public Pathfinder(int[,] map)
        {
            mapGrid = new Map(map);
        }

        // Holds search nodes that are avaliable to search.
        private List<SearchNode> openList = new List<SearchNode>();
        // Holds the nodes that have already been searched.
        private List<SearchNode> closedList = new List<SearchNode>();

        /// <summary>
        /// Returns an estimate of the distance between two points. (H)
        /// </summary>
        public float Heuristic_Orthogonal_Distance(Microsoft.Xna.Framework.Point point1, Microsoft.Xna.Framework.Point point2)
        {
            //h(n) = D * (abs(n.x-goal.x) + abs(n.y-goal.y))
            heuristic = Math.Abs(point1.X - point2.X) +
                   Math.Abs(point1.Y - point2.Y);
            return heuristic;
        }

        public float Heuristic_Diagonal_Distance(Microsoft.Xna.Framework.Point point1, Microsoft.Xna.Framework.Point point2)
        {
            //h(n) = D * max(abs(n.x-goal.x), abs(n.y-goal.y))
            heuristic = Math.Max(Math.Abs(point1.X - point2.X), Math.Abs(point1.Y - point2.Y));
            return heuristic;
        }

        /// <summary>
        /// Resets the state of the search nodes.
        /// </summary>
        private void ResetSearchNodes()
        {
            openList.Clear();
            closedList.Clear();

            for (int x = 0; x < levelWidth; x++)
            {
                for (int y = 0; y < levelHeight; y++)
                {
                    SearchNode node = searchNodes[x, y];

                    if (node == null)
                    {
                        continue;
                    }

                    node.InOpenList = false;
                    node.InClosedList = false;

                    node.DistanceTraveled = float.MaxValue;
                    node.DistanceToGoal = float.MaxValue;
                }
            }
        }

        /// <summary>
        /// Returns the node with the smallest distance to goal.
        /// </summary>
        private SearchNode FindBestNode()
        {
            SearchNode currentTile = openList[0];

            float smallestDistanceToGoal = float.MaxValue;

            // Find the closest node to the goal.
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].DistanceToGoal < smallestDistanceToGoal)
                {
                    currentTile = openList[i];
                    smallestDistanceToGoal = currentTile.DistanceToGoal;
                }
            }
            return currentTile;
        }

        /// <summary>
        /// Use the parent field of the search nodes to trace
        /// a path from the end node to the start node.
        /// </summary>
        private List<Vector2> FindFinalPath(SearchNode startNode, SearchNode endNode)
        {
            closedList.Add(endNode);

            SearchNode parentTile = endNode.Parent;

            // Trace back through the nodes using the parent fields
            // to find the best path.
            while (parentTile != startNode)
            {
                closedList.Add(parentTile);
                parentTile = parentTile.Parent;
            }

            List<Vector2> finalPath = new List<Vector2>();

            // Reverse the path and transform into world space.
            for (int i = closedList.Count - 1; i >= 0; i--)
            {
                finalPath.Add(new Vector2(closedList[i].Position.X * mapGrid.GetTextureWidth(),
                                          closedList[i].Position.Y * mapGrid.GetTextureHeight()));
            }

            return finalPath;
        }

        /// <summary>
        /// Finds the optimal path from one point to another.
        /// </summary>
        public List<Vector2> FindPath(Microsoft.Xna.Framework.Point startPoint, Microsoft.Xna.Framework.Point endPoint)
        {
            // Only try to find a path if the start and end points are different.
            if (startPoint == endPoint)
            {
                return new List<Vector2>();
            }

            /////////////////////////////////////////////////////////////////////
            // Step 1 : Clear the Open and Closed Lists and reset each node’s F 
            //          and G values in case they are still set from the last 
            //          time we tried to find a path. 
            /////////////////////////////////////////////////////////////////////
            ResetSearchNodes();

            // Store references to the start and end nodes for convenience.
            SearchNode startNode = searchNodes[startPoint.X, startPoint.Y];
            SearchNode endNode = searchNodes[endPoint.X, endPoint.Y];

            /////////////////////////////////////////////////////////////////////
            // Step 2 : Set the start node’s G value to 0 and its F value to the 
            //          estimated distance between the start node and goal node 
            //          (this is where our H function comes in) and add it to the 
            //          Open List. 
            /////////////////////////////////////////////////////////////////////
            startNode.InOpenList = true;

            //When using distance to goal, replace the right side with whatever function
            //that you want to implement.  I.e., diagonal or orthogonal distance.
            startNode.DistanceToGoal = Heuristic_Diagonal_Distance(startPoint, endPoint);
            startNode.DistanceTraveled = 0;

            openList.Add(startNode);

            /////////////////////////////////////////////////////////////////////
            // Setp 3 : While there are still nodes to look at in the Open list : 
            /////////////////////////////////////////////////////////////////////
            while (openList.Count > 0)
            {
                /////////////////////////////////////////////////////////////////
                // a) : Loop through the Open List and find the node that 
                //      has the smallest F value.
                /////////////////////////////////////////////////////////////////
                SearchNode currentNode = FindBestNode();

                /////////////////////////////////////////////////////////////////
                // b) : If the Open List empty or no node can be found, 
                //      no path can be found so the algorithm terminates.
                /////////////////////////////////////////////////////////////////
                if (currentNode == null)
                {
                    break;
                }

                /////////////////////////////////////////////////////////////////
                // c) : If the Active Node is the goal node, we will 
                //      find and return the final path.
                /////////////////////////////////////////////////////////////////
                if (currentNode == endNode)
                {
                    // Trace our path back to the start.
                    return FindFinalPath(startNode, endNode);
                }

                /////////////////////////////////////////////////////////////////
                // d) : Else, for each of the Active Node’s neighbours :
                /////////////////////////////////////////////////////////////////
                for (int i = 0; i < currentNode.Neighbors.Length; i++)
                {
                    SearchNode neighbor = currentNode.Neighbors[i];

                    //////////////////////////////////////////////////
                    // i) : Make sure that the neighbouring node can 
                    //      be walked across. 
                    //////////////////////////////////////////////////
                    if (neighbor == null || neighbor.Walkable == false)
                    {
                        continue;
                    }

                    //////////////////////////////////////////////////
                    // ii) Calculate a new G value for the neighbouring node.
                    //////////////////////////////////////////////////
                    float distanceTraveled = currentNode.DistanceTraveled + 1;

                    // An estimate of the distance from this node to the end node.
                    float heuristic = Heuristic_Diagonal_Distance(neighbor.Position, endPoint);

                    //////////////////////////////////////////////////
                    // iii) If the neighbouring node is not in either the Open 
                    //      List or the Closed List : 
                    //////////////////////////////////////////////////
                    if (neighbor.InOpenList == false && neighbor.InClosedList == false)
                    {
                        // (1) Set the neighbouring node’s G value to the G value 
                        //     we just calculated.
                        neighbor.DistanceTraveled = distanceTraveled;
                        // (2) Set the neighbouring node’s F value to the new G value + 
                        //     the estimated distance between the neighbouring node and
                        //     goal node.
                        neighbor.DistanceToGoal = distanceTraveled + heuristic + mapGrid.GetLandType(neighbor.Position.X, neighbor.Position.Y);
                        // (3) Set the neighbouring node’s Parent property to point at the Active 
                        //     Node.
                        neighbor.Parent = currentNode;
                        // (4) Add the neighbouring node to the Open List.
                        neighbor.InOpenList = true;
                        openList.Add(neighbor);
                    }
                    //////////////////////////////////////////////////
                    // iv) Else if the neighbouring node is in either the Open 
                    //     List or the Closed List :
                    //////////////////////////////////////////////////
                    else if (neighbor.InOpenList || neighbor.InClosedList)
                    {
                        // (1) If our new G value is less than the neighbouring 
                        //     node’s G value, we basically do exactly the same 
                        //     steps as if the nodes are not in the Open and 
                        //     Closed Lists except we do not need to add this node 
                        //     the Open List again.
                        if (neighbor.DistanceTraveled > distanceTraveled)
                        {
                            neighbor.DistanceTraveled = distanceTraveled;
                            neighbor.DistanceToGoal = distanceTraveled + heuristic;

                            neighbor.Parent = currentNode;
                        }
                    }
                }

                /////////////////////////////////////////////////////////////////
                // e) Remove the Active Node from the Open List and add it to the 
                //    Closed List
                /////////////////////////////////////////////////////////////////
                openList.Remove(currentNode);
                currentNode.InClosedList = true;
            }

            // No path could be found.
            return new List<Vector2>();
        }


        /// <summary>

        /// Splits our level up into a grid of nodes.

        /// </summary>

        private void InitializeSearchNodes(Map map)
        {
            searchNodes = new SearchNode[levelWidth, levelHeight];



            //For each of the tiles in our map, we

            // will create a search node for it.

            for (int x = 0; x < levelWidth; x++)
            {

                for (int y = 0; y < levelHeight; y++)
                {

                    //Create a search node to represent this tile.

                    SearchNode node = new SearchNode();



                    node.Position = new Microsoft.Xna.Framework.Point(x, y);



                    // Our NPC can only walk on grass or marsh tiles.

                    node.Walkable = map.GetIndex(x, y) != 0;



                    // We only want to store nodes

                    // that can be walked on.

                    if (node.Walkable == true)
                    {

                        node.Neighbors = new SearchNode[8];

                        searchNodes[x, y] = node;

                    }

                }

            }

            // Now for each of the search nodes, we will

            // connect it to each of its neighbours.

            for (int x = 0; x < levelWidth; x++)
            {

                for (int y = 0; y < levelHeight; y++)
                {

                    SearchNode node = searchNodes[x, y];



                    // We only want to look at the nodes that 

                    // our npc can walk on.

                    if (node == null || node.Walkable == false)
                    {

                        continue;

                    }



                    // An array of all of the possible neighbors this 

                    // node could have. (We will ignore diagonals for now.)

                    Microsoft.Xna.Framework.Point[] neighbors = new Microsoft.Xna.Framework.Point[]

                    {

                     new Microsoft.Xna.Framework.Point (x, y - 1), // The node above the current node

                     new Microsoft.Xna.Framework.Point (x, y + 1), // The node below the current node.

                     new Microsoft.Xna.Framework.Point (x - 1, y), // The node left of the current node.

                     new Microsoft.Xna.Framework.Point (x + 1, y), // The node right of the current node

                     new Microsoft.Xna.Framework.Point (x + 1, y - 1), //The node to the top right of the current node
                     
                     new Microsoft.Xna.Framework.Point (x - 1, y - 1), // the node to the top left of the current node

                     new Microsoft.Xna.Framework.Point (x - 1, y + 1), // The node to the bottom left of the current node

                     new Microsoft.Xna.Framework.Point (x + 1, y + 1), //the node to the bottom right of the current node

                    };



                    // We loop through each of the possible neighbors

                    for (int i = 0; i < neighbors.Length; i++)
                    {

                        Microsoft.Xna.Framework.Point position = neighbors[i];



                        // We need to make sure this neighbour is part of the level.

                        if (position.X < 0 || position.X > levelWidth - 1 ||

                            position.Y < 0 || position.Y > levelHeight - 1)
                        {

                            continue;

                        }



                        SearchNode neighbor = searchNodes[position.X, position.Y];



                        // We will only bother keeping a reference 

                        // to the nodes that can be walked on.

                        if (neighbor == null || neighbor.Walkable == false)
                        {

                            continue;

                        }

                        // If neighbor node diagonal of the parent node and is "blocked" by non walkable terrain to the 
                        //immidiate orthongonal positions, skip this node    
                        if (
                            (neighbors[4].X == neighbors[i].X && neighbors[4].Y == neighbors[i].Y && ((!BoundsCheck("top", neighbors[i].X, neighbors[i].Y) || !BoundsCheck("left", neighbors[i].X, neighbors[i].Y)) || (searchNodes[neighbors[i].X - 1, neighbors[i].Y] == null || searchNodes[neighbors[i].X, neighbors[i].Y + 1] == null))) ||
                            (neighbors[7].X == neighbors[i].X && neighbors[7].Y == neighbors[i].Y && ((!BoundsCheck("bottom", neighbors[i].X, neighbors[i].Y) || !BoundsCheck("right", neighbors[i].X, neighbors[i].Y)) || (searchNodes[neighbors[i].X - 1, neighbors[i].Y] == null || searchNodes[neighbors[i].X, neighbors[i].Y - 1] == null))) ||
                            (neighbors[6].X == neighbors[i].X && neighbors[6].Y == neighbors[i].Y && ((!BoundsCheck("bottom", neighbors[i].X, neighbors[i].Y) || !BoundsCheck("left", neighbors[i].X, neighbors[i].Y)) || (searchNodes[neighbors[i].X + 1, neighbors[i].Y] == null || searchNodes[neighbors[i].X, neighbors[i].Y - 1] == null))) ||
                            (neighbors[5].X == neighbors[i].X && neighbors[5].Y == neighbors[i].Y && ((!BoundsCheck("top", neighbors[i].X, neighbors[i].Y) || !BoundsCheck("right", neighbors[i].X, neighbors[i].Y)) || (searchNodes[neighbors[i].X + 1, neighbors[i].Y] == null || searchNodes[neighbors[i].X, neighbors[i].Y + 1] == null)))
                            )
                         {
                                continue;
                         }


                        // Store a reference to the neighbor.

                        node.Neighbors[i] = neighbor;

                    }

                }

            }

            //add diagonal check here

        }

        private bool BoundsCheck(string side, int x, int y)
        {
            if (side == "top" && y > 0)
                return true;
            if (side == "bottom" && y < (mapGrid.Height - 1))
                return true;
            if (side == "right" && x < ( mapGrid.Width - 1))
                return true;
            if (side == "left" && x > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Calculates interpolated point between two points using Catmull-Rom Spline
        /// </summary>
        /// <remarks>
        /// Points calculated exist on the spline between points two and three.
        /// </remarks>
        /// <param name="p0">First Microsoft.Xna.Framework.Point</param>
        /// <param name="p1">Second Microsoft.Xna.Framework.Point</param>
        /// <param name="p2">Third Microsoft.Xna.Framework.Point</param>
        /// <param name="p3">Fourth Microsoft.Xna.Framework.Point</param>
        /// <param name="t">
        /// Normalised distance between second and third point 
        /// where the spline point will be calculated
        /// </param>
        /// <returns>
        /// Calculated Spline Microsoft.Xna.Framework.Point
        /// </returns>
        private PointF PointOnCurve(PointF p0, PointF p1, PointF p2, PointF p3, float t)
        {
            PointF ret = new PointF();

            float t2 = t * t;
            float t3 = t2 * t;

            ret.X = 0.5f * ((2.0f * p1.X) +
            (-p0.X + p2.X) * t +
            (2.0f * p0.X - 5.0f * p1.X + 4 * p2.X - p3.X) * t2 +
            (-p0.X + 3.0f * p1.X - 3.0f * p2.X + p3.X) * t3);

            ret.Y = 0.5f * ((2.0f * p1.Y) +
            (-p0.Y + p2.Y) * t +
            (2.0f * p0.Y - 5.0f * p1.Y + 4 * p2.Y - p3.Y) * t2 +
            (-p0.Y + 3.0f * p1.Y - 3.0f * p2.Y + p3.Y) * t3);

            return ret;
        }

        /// <summary>
        /// Takes a list of vectors and outputs a list of the same vectors with
        /// Catmull-Rom splines applied.  Returns the Catmull-Rom list.
        /// Requires a list of 4 or more points else it returns input vector.
        /// <param name="input">Vector that contains the points to apply spline to</param>
        /// <param name="output">Vector that will store the returned vector with splines applied to it</param>
        /// </summary>
        public List<Vector2> SplineTransform(ref List<Vector2> input)
        {
            if (input.Count >= 4)
            {
                List<Vector2> output = new List<Vector2>();
                for (int i = 0; i < (input.Count - 3); i++)
                {
                    //create array of points that represent the 4 points along the path
                    PointF[] points = 
                        {
                            new PointF(input[i].X,input[i].Y), new PointF(input[i+1].X,input[i+1].Y), 
                            new PointF(input[i+2].X,input[i+2].Y), new PointF(input[i+3].X,input[i+3].Y)
                        };

                    
                    for (float t = 0.0f; t < 1.0f; t += 0.03f)
                    {
                        //create the new spline point
                        PointF newPoint = PointOnCurve(points[0], points[1], points[2], points[3], t);
                        //store the spline point
                        output.Add(new Vector2(newPoint.X, newPoint.Y));
                    }
                    
                }
                //store points in vector
                //output.AddRange(input.GetRange(input.Count - 3, 3));
                output.Add(input[input.Count - 1]);
                return output;
            }
            else
            {
                return input;
            }
        }
    }
}
