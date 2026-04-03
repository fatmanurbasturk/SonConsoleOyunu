using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace KonsolSkorOyunu
{
    class Program
    {
        // Oyun Ayarları
        static int playerX = 15;
        static int score = 0;
        static int screenWidth = 30;
        static int screenHeight = 20;
        static bool isRunning = true;
        static List<int[]> objects = new List<int[]>(); // [x, y] formatında
        static string logFile = "game_log.txt";

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            File.WriteAllText(logFile, "--- OYUN BAŞLADI ---\n"); // Log dosyasını sıfırla

            // Nesne düşürme hızını ayarlamak için zamanlayıcı
            int frameCounter = 0;

            while (isRunning)
            {
                // 1. INPUT (Giriş Kontrolü)
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    WriteLog($"INPUT -> key={key} playerX={playerX}");

                    if (key == ConsoleKey.LeftArrow && playerX > 0) playerX--;
                    if (key == ConsoleKey.RightArrow && playerX < screenWidth - 1) playerX++;
                    if (key == ConsoleKey.Escape) isRunning = false;
                    
                    WriteLog($"UPDATE -> PlayerMoved newX={playerX}");
                }

                // 2. UPDATE (Güncelleme Mantığı)
                if (frameCounter % 5 == 0) // Her 5 döngüde bir nesneleri hareket ettir
                {
                    UpdateObjects();
                }

                if (frameCounter % 15 == 0) // Belirli aralıklarla yeni nesne ekle
                {
                    Random rnd = new Random();
                    int spawnX = rnd.Next(0, screenWidth);
                    objects.Add(new int[] { spawnX, 0 });
                    WriteLog($"UPDATE -> itemSpawned x={spawnX} y=0");
                }

                // 3. COLLISION (Çarpışma Kontrolü)
                CheckCollision();

                // 4. DRAW (Çizim)
                Draw();

                // Oyun Bitirme Şartı
                if (score >= 10)
                {
                    isRunning = false;
                    WriteLog($"GAME_OVER -> FinalScore={score}");
                }

                Thread.Sleep(50); // Oyun hızı
                frameCounter++;
            }

            Console.Clear();
            Console.WriteLine($"OYUN BİTTİ! Toplam Skor: {score}");
            Console.WriteLine("Loglar 'game_log.txt' dosyasına kaydedildi.");
        }

        static void UpdateObjects()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i][1]++; // Y ekseninde aşağı düşür
                WriteLog($"MOVE -> item[{i}] x={objects[i][0]} y={objects[i][1]}");
            }

            // Ekrandan çıkan nesneleri temizle
            objects.RemoveAll(o => o[1] >= screenHeight);
        }

        static void CheckCollision()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                // Eğer nesne oyuncunun koordinatlarına çarptıysa (Y = alt satır)
                if (objects[i][1] == screenHeight - 1 && objects[i][0] == playerX)
                {
                    score++;
                    WriteLog($"COLLISION -> score={score} atX={playerX}");
                    objects.RemoveAt(i);
                    break;
                }
            }
        }

        static void Draw()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Skor: {score} / 10 (Çıkış: ESC)");
            
            // Boş bir sahne oluştur
            char[,] buffer = new char[screenHeight, screenWidth];
            for (int y = 0; y < screenHeight; y++)
            // Boş bir sahne oluştur
           
                for (int x = 0; x < screenWidth; x++)
                    buffer[y, x] = ' ';

            // Oyuncuyu yerleştir
            buffer[screenHeight - 1, playerX] = '@';

            // Nesneleri yerleştir
            foreach (var obj in objects)
            {
                if (obj[1] < screenHeight)
                    buffer[obj[1], obj[0]] = '*';
            }

            // Sahneyi tek seferde ekrana bas (titremeyi azaltır)
            string output = "";
            for (int y = 0; y < screenHeight; y++)
            {
                for (int x = 0; x < screenWidth; x++)
                    output += buffer[y, x];
                output += "\n";
            }
            Console.Write(output);
        }

        static void WriteLog(string message)
        {
            try
            {
                File.AppendAllText(logFile, message + Environment.NewLine);
            }
            catch {  }
        }
    }
}