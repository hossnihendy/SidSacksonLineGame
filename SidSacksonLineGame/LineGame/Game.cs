using System; 
using System.Collections.Generic;

namespace SidSacksonLineGame
{
    public class Game
    {
        private static Board board = new Board();
        private static List<Point> validStartNodes = new List<Point>();
        private static List<Line> lines = new List<Line>();
        private static bool currentPlayerIsOne;
        private static bool gameOver;

        private static Point startNode = new Point(-1, 0);
        
        public static ResponsePayload HandleRequest(RequestPayload request)
        {
            ResponsePayload response = new ResponsePayload();
            response.id = request.id;
            response.body = new ResponsePayload.Body();

            // initialize request
            if (request.msg == "INITIALIZE")
            {
                validStartNodes.Clear();
                lines.Clear();
                currentPlayerIsOne = true;
                gameOver = false;
                startNode.x = -1;

                response.msg = "INITIALIZE";
                response.body.heading = "Player 1";
                response.body.message = "Awaiting Player 1's Move";
            }

            // node click request
            if (request.msg == "NODE_CLICKED")
            {
                // after game is over, additional node clicks do not affect the display
                if (gameOver == true)
                {
                    response.msg = "INVALID_START_NODE";
                    response.body.heading = "Game Over";
                    response.body.message = currentPlayer(currentPlayerIsOne) + " Wins!";
                }
                // first move
                else if (validStartNodes.Count == 0)
                {
                    // start node 
                    if (startNode.x == -1)
                    {
                        startNode.x = request.body.x;
                        startNode.y = request.body.y;
                        response.msg = "VALID_START_NODE";
                        response.body.heading = currentPlayer(currentPlayerIsOne);
                        response.body.message = "Select a second node to complete the line";
                    }
                    // end node
                    else
                    {
                        Point endNode = new Point(request.body.x, request.body.y);
                        bool octilinear = GameRules.Isoclines(startNode, endNode);
                        // end node valid
                        if (octilinear)
                        {
                            Point firstStartNode = new Point(startNode.x, startNode.y);
                            Line newLine = new Line(firstStartNode, endNode);
                            lines.Add(newLine);
                            validStartNodes.Add(firstStartNode);
                            validStartNodes.Add(endNode);
                            currentPlayerIsOne = !currentPlayerIsOne;

                            response.body.newLine = new ResponsePayload.Body.NewLine();
                            response.body.newLine.start = new ResponsePayload.Body.NewLine.Start();
                            response.body.newLine.end = new ResponsePayload.Body.NewLine.End();
                            response.msg = "VALID_END_NODE";
                            response.body.newLine.start.x = startNode.x;
                            response.body.newLine.start.y = startNode.y;
                            response.body.newLine.end.x = endNode.x;
                            response.body.newLine.end.y = endNode.y;
                            response.body.heading = currentPlayer(currentPlayerIsOne);
                            response.body.message = "Awaiting " + currentPlayer(currentPlayerIsOne) + " 's Move";

                            startNode.x = -1;
                        }
                        // end node invalid
                        else
                        {
                            response.msg = "INVALID_END_NODE";
                            response.body.heading = currentPlayer(currentPlayerIsOne);
                            response.body.message = "Invalid move!";

                            startNode.x = -1;
                        }
                    }
                }
                // all subsequent moves
                else
                {
                    // start node
                    if (startNode.x == -1)
                    {
                        startNode.x = request.body.x;
                        startNode.y = request.body.y;
                        // start node valid                        
                        if ((startNode.x == validStartNodes[0].x && startNode.y == validStartNodes[0].y) || (startNode.x == validStartNodes[1].x && startNode.y == validStartNodes[1].y))
                        {
                            response.msg = "VALID_START_NODE";
                            response.body.heading = currentPlayer(currentPlayerIsOne);
                            response.body.message = "Select a second node to complete the line";
                        }
                        // start node invalid
                        else
                        {
                            response.msg = "INVALID_START_NODE";
                            response.body.heading = currentPlayer(currentPlayerIsOne);
                            response.body.message = "Not a valid starting position";

                            startNode.x = -1;
                        }
                    }
                    // end node
                    else
                    {
                        Point endNode = new Point(request.body.x, request.body.y);
                        Line tryLine = new Line(startNode, endNode);
                        bool octilinear = GameRules.Isoclines(startNode, endNode);
                        bool noInvalidIntersect = GameRules.NoInvalidIntersect(tryLine, lines);
                        // end node valid
                        if (octilinear && noInvalidIntersect)
                        {
                            Point currentStartNode = new Point(startNode.x, startNode.y);
                            Line newLine = new Line(currentStartNode, endNode);
                            lines.Add(newLine);
                            for (int i = 1; i > -1; i--)
                            {
                                if (validStartNodes[i].x == currentStartNode.x && validStartNodes[i].y == currentStartNode.y)
                                {
                                    validStartNodes.RemoveAt(i);
                                }
                            }
                            validStartNodes.Add(endNode);
                            currentPlayerIsOne = !currentPlayerIsOne;

                            response.body.newLine = new ResponsePayload.Body.NewLine();
                            response.body.newLine.start = new ResponsePayload.Body.NewLine.Start();
                            response.body.newLine.end = new ResponsePayload.Body.NewLine.End();

                            // check if board still has valid moves
                            bool boardComplete = GameRules.BoardComplete(board, lines, validStartNodes);
                            // game continues if board still has valid moves
                            if (!boardComplete)
                            {
                                response.msg = "VALID_END_NODE";
                                response.body.newLine.start.x = startNode.x;
                                response.body.newLine.start.y = startNode.y;
                                response.body.newLine.end.x = endNode.x;
                                response.body.newLine.end.y = endNode.y;
                                response.body.heading = currentPlayer(currentPlayerIsOne);
                                response.body.message = "Awaiting " + currentPlayer(currentPlayerIsOne) + " 's Move";

                                startNode.x = -1;
                            }
                            // game over if board no longer has valid moves
                            else
                            {
                                gameOver = true;

                                response.msg = "GAME_OVER";
                                response.body.newLine.start.x = startNode.x;
                                response.body.newLine.start.y = startNode.y;
                                response.body.newLine.end.x = endNode.x;
                                response.body.newLine.end.y = endNode.y;
                                response.body.heading = "Game Over";
                                response.body.message = currentPlayer(currentPlayerIsOne) + " Wins!";
                            }                                                       
                        }
                        // end node invalid
                        else
                        {
                            response.msg = "INVALID_END_NODE";
                            response.body.heading = currentPlayer(currentPlayerIsOne);
                            response.body.message = "Invalid move!";

                            startNode.x = -1;
                        }
                    }
                }
            }
            return response;
        }

        private static string currentPlayer(bool currentPlayerIsOne)
        {
            if (currentPlayerIsOne) return "Player # 1";
            else return "Player # 2";
        }  
    }
}