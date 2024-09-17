using System;
using System.Net.Sockets;
using System.Text;

class TicTacToeClient
{
    static void Main(string[] args)
    {
        string serverAddress = "127.0.0.1"; // Endereço IP do servidor
        int port = 13000; // Porta do servidor
        TcpClient client = null;

        try
        {
            client = new TcpClient(serverAddress, port);
            NetworkStream stream = client.GetStream();
            Console.WriteLine("Conectado ao servidor!");

            // Receber o identificador do jogador
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string playerId = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
            Console.WriteLine($"Você é o Jogador {playerId}");

            while (true)
            {
                // Receber o estado do tabuleiro e resultado do jogo
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                string serverMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                if (serverMessage.Length >= 9) // Garantir que o estado do tabuleiro está completo
                {
                    Console.Clear();
                    Console.WriteLine("Estado do tabuleiro:\n");

                    // Mostrar o tabuleiro
                    for (int i = 0; i < 9; i += 3)
                    {
                        Console.WriteLine($" {serverMessage[i]} | {serverMessage[i + 1]} | {serverMessage[i + 2]} ");
                        if (i < 6) Console.WriteLine("---|---|---");
                    }

                    Console.WriteLine();

                    // Verificar se o jogo terminou
                    if (serverMessage.Length > 9)
                    {
                        char gameState = serverMessage[9];
                        if (gameState == '1')
                        {
                            Console.WriteLine("Jogo terminou. Você venceu!");
                            break;
                        }
                        else if (gameState == '2')
                        {
                            Console.WriteLine("Jogo terminou. Você perdeu.");
                            break;
                        }
                        else if (gameState == '3')
                        {
                            Console.WriteLine("Jogo terminou. Empate!");
                            break;
                        }
                    }
                }

                // Se for sua vez de jogar
                if (serverMessage == "X" || serverMessage == "O")
                {
                    Console.WriteLine("Sua vez de jogar!");
                    int move;
                    while (true)
                    {
                        Console.Write("Escolha uma posição (1-9): ");
                        if (int.TryParse(Console.ReadLine(), out move) && move >= 1 && move <= 9)
                        {
                            // Enviar jogada para o servidor
                            byte[] moveBytes = Encoding.UTF8.GetBytes(move.ToString());
                            stream.Write(moveBytes, 0, moveBytes.Length);

                            // Receber confirmação da jogada
                            bytesRead = stream.Read(buffer, 0, buffer.Length);
                            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                            if (response == "-1")
                            {
                                Console.WriteLine("Movimento inválido. Tente novamente.");
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Movimento inválido. Tente novamente.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Aguardando o outro jogador...");
                }
            }
        }
        finally
        {
            client?.Close();
        }
    }
}