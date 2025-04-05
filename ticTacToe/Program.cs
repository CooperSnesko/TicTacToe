using System;
using System.IO;
using System.Runtime.InteropServices;

// Tic Tac Toe game
namespace TicTacToeGame
{
    
    // Game results
    [StructLayout(LayoutKind.Explicit)]
    struct GameResult
    {
        
        [FieldOffset(0)]
        public int Winner;
        [FieldOffset(4)]
        public int Moves;
    }
    
    
    // Player base class
    abstract class Player
    {
        public string Name { get; set; }
        public char Symbol { get; set; } // 'X' or 'O'
        
        public Player(string name, char symbol)
        {
            Name = name;
            Symbol = symbol;
        }
        
        
        public abstract int GetMove(char[] board);
    }
    
    
    // Human player
    class HumanPlayer : Player
    {
        public HumanPlayer(string name, char symbol) : base(name, symbol) {}
        
        public override int GetMove(char[] board)
        {
            int move;
            while (true)
            {
                Console.Write($"{Name} ({Symbol}), enter your move (1-9): ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out move))
                {
                    move -= 1; 
                    if (move >= 0 && move < 9 && board[move] == ' ')
                        break;
                }
                Console.WriteLine("Invalid move. Try again.");
            }
            return move;
        }
    }
    
    // Game logic
    class Game
    {
        private char[] board;
        private Player player1;
        private Player player2;
        
        public Game()
        {
            board = new char[9];
            for (int i = 0; i < 9; i++)
                board[i] = ' ';
        }
        
        public void SetupPlayers()
        {
            // Prompt for players
            Console.Write("Enter name for Player 1 (X): ");
            string name1 = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name1))
                name1 = "Player 1";
            player1 = new HumanPlayer(name1, 'X');
            
            Console.Write("Enter name for Player 2 (O): ");
            string name2 = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name2))
                name2 = "Player 2";
            player2 = new HumanPlayer(name2, 'O');
        }
        
        
        private void DrawBoard()
        {
            // Draw board
            Console.WriteLine();
            Console.WriteLine(" {0} | {1} | {2} ", board[0], board[1], board[2]);
            Console.WriteLine("---+---+---");
            Console.WriteLine(" {0} | {1} | {2} ", board[3], board[4], board[5]);
            Console.WriteLine("---+---+---");
            Console.WriteLine(" {0} | {1} | {2} ", board[6], board[7], board[8]);
            Console.WriteLine();
        }
        
        
        private bool CheckWin(char symbol)
        {
            // Check win
            int[,] winPatterns = new int[,] {
                {0, 1, 2},
                {3, 4, 5},
                {6, 7, 8},
                {0, 3, 6},
                {1, 4, 7},
                {2, 5, 8},
                {0, 4, 8},
                {2, 4, 6}
            };
            for (int i = 0; i < winPatterns.GetLength(0); i++)
            {
                if (board[winPatterns[i, 0]] == symbol &&
                    board[winPatterns[i, 1]] == symbol &&
                    board[winPatterns[i, 2]] == symbol)
                {
                    return true;
                }
            }
            return false;
        }
        
        
        private bool IsBoardFull()
        {
            // Check full board
            foreach (char c in board)
            {
                if (c == ' ')
                    return false;
            }
            return true;
        }
        
       
        public GameResult Play()
        {
            // Game loop
            int moveCount = 0;
            Player currentPlayer = player1;
            while (true)
            {
                DrawBoard();
                int move = currentPlayer.GetMove(board);
                board[move] = currentPlayer.Symbol;
                moveCount++;
                
                if (CheckWin(currentPlayer.Symbol))
                {
                    DrawBoard();
                    Console.WriteLine($"{currentPlayer.Name} wins!");
                    return new GameResult { Winner = currentPlayer == player1 ? 1 : 2, Moves = moveCount };
                }
                
                if (IsBoardFull())
                {
                    DrawBoard();
                    Console.WriteLine("The game is a draw!");
                    return new GameResult { Winner = 3, Moves = moveCount };
                }
                
                // Switch player
                currentPlayer = (currentPlayer == player1) ? player2 : player1;
            }
        }
    }
    
    class Program
    {
        // Main entry point
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Tic Tac Toe!");
            Game game = new Game();
            game.SetupPlayers();
            
            GameResult result = game.Play();
            
            
            string filePath = "game_result.txt";
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("Tic Tac Toe Game Result");
                    if (result.Winner == 1)
                        writer.WriteLine("Winner: Player 1");
                    else if (result.Winner == 2)
                        writer.WriteLine("Winner: Player 2");
                    else
                        writer.WriteLine("Game ended in a Draw");
                    writer.WriteLine("Total Moves: " + result.Moves);
                }
                Console.WriteLine($"Game result saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to file: " + ex.Message);
            }
            
            Console.WriteLine("Thank you for playing!");
        }
    }
}