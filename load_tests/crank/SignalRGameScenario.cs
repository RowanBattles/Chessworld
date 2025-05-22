using System.Threading.Tasks;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;
using NanoidDotNet;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    private static readonly string[] moves =
    [
        "e2e4", "e7e5", "g1f3", "b8c6", "f1c4", "g8f6", "d2d3", "f8c5",
        "c2c3", "d7d6", "b1d2", "c8e6", "b2b4", "c5b6", "a2a4", "a7a6",
        "a4a5", "b6a7", "d1b3", "d6d5", "e4d5", "e6d5", "c4d5", "f6d5",
        "b3d5", "d8d5", "e1g1", "e8g8", "c1b2", "a8d8"
    ];

    private static int failureCount_CreatingGame = 0;
    private static int failureCount_CreatingGame_StatusCode = 0;
    private static int failureCount_Connecting = 0;
    private static int failureCount_MakingMove = 0;   
    private static int successCount = 0;
    private static readonly object lockObj = new();

    private static readonly List<long> moveLatencies = [];
    private static readonly object latencyLock = new();

    public static async Task Main(string[] args)
    {
        int numberOfGames = 2000;

        // CPU usage tracking
        var process = Process.GetCurrentProcess();
        var cpuStart = process.TotalProcessorTime;
        var wallClock = Stopwatch.StartNew();

        var stopwatch = Stopwatch.StartNew();
        var tasks = new List<Task>();

        for (int i = 0; i < numberOfGames; i++)
        {
            int gameNumber = i + 1;
            tasks.Add(SimulateGameAsync(gameNumber));
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();
        wallClock.Stop();

        // CPU usage calculation
        var cpuEnd = process.TotalProcessorTime;
        double cpuUsage = (cpuEnd - cpuStart).TotalMilliseconds / (Environment.ProcessorCount * wallClock.ElapsedMilliseconds) * 100;

        // Latency stats
        long minLatency = 0, maxLatency = 0, avgLatency = 0;
        lock (latencyLock)
        {
            if (moveLatencies.Count > 0)
            {
                minLatency = moveLatencies.Min();
                maxLatency = moveLatencies.Max();
                avgLatency = (long)moveLatencies.Average();
            }
        }

        Console.WriteLine($"Total time taken: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"CPU usage: {cpuUsage:F2}%");
        Console.WriteLine($"Move latency (ms): min={minLatency}, avg={avgLatency}, max={maxLatency}");
        Console.WriteLine($"Total successes: {successCount}");
        Console.WriteLine($"Total failures Creating game: {failureCount_CreatingGame}");
        Console.WriteLine($"Total failures Creating game status code: {failureCount_CreatingGame_StatusCode}");
        Console.WriteLine($"Total failures Connecting: {failureCount_Connecting}");
        Console.WriteLine($"Total failures Making move: {failureCount_MakingMove}");
    }

    public static async Task SimulateGameAsync(int gameNumber)
    {
        var whiteToken = Nanoid.Generate("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", size: 8);
        var blackToken = Nanoid.Generate("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", size: 8);
        var stopwatch = new Stopwatch();
        string url = $"http://localhost:8080/games/create?whiteToken={whiteToken}&blackToken={blackToken}";

        using var httpClient = new HttpClient();
        HttpResponseMessage response;
        try
        {
            response = await httpClient.PostAsync(url, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Game {gameNumber}] Exception during game creation: {ex.Message}");
            RegisterFailure("CreatingGame");
            return;
        }

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"[Game {gameNumber}] Failed to create game: {response.StatusCode}");
            RegisterFailure("CreatingGame_StatusCode");
            return;
        }

        var gameId = (await response.Content.ReadAsStringAsync()).Trim('"');
        var whiteConn = new HubConnectionBuilder()
            .WithUrl($"http://localhost:8080/play?gameId={gameId}&token={whiteToken}")
            .Build();
        var blackConn = new HubConnectionBuilder()
            .WithUrl($"http://localhost:8080/play?gameId={gameId}&token={blackToken}")
            .Build();

        whiteConn.On<string>("Error", msg =>
        {
            Console.WriteLine($"[Game {gameNumber}] White error: {msg}");
        });
        blackConn.On<string>("Error", msg =>
        {
            Console.WriteLine($"[Game {gameNumber}] Black error: {msg}");
        });

        try
        {
            await whiteConn.StartAsync();
            await blackConn.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Game {gameNumber}] Exception during connection: {ex.Message}");
            RegisterFailure("Connecting");
            return;
        }

        for (int i = 0; i < moves.Length; i++)
        {
            string move = moves[i];
            var conn = i % 2 == 0 ? whiteConn : blackConn;

            var moveStopwatch = Stopwatch.StartNew();
            try
            {
                await conn.InvokeAsync("MakeMove", move);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Game {gameNumber}] Move {i + 1} failed: {ex.Message}");
                RegisterFailure("MakingMove");
                return;
            }
            moveStopwatch.Stop();

            lock (latencyLock)
            {
                moveLatencies.Add(moveStopwatch.ElapsedMilliseconds);
            }
        }

        await whiteConn.DisposeAsync();
        await blackConn.DisposeAsync();

        Console.WriteLine($"[Game {gameNumber}] Complete.");

        RegisterSuccess();
    }

    private static void RegisterFailure(string failure)
    {
        lock (lockObj)
        {
            if (failure == "CreatingGame") failureCount_CreatingGame++;
            else if (failure == "Connecting") failureCount_Connecting++;
            else if (failure == "MakingMove") failureCount_MakingMove++;
            else if (failure == "CreatingGame_StatusCode") failureCount_CreatingGame_StatusCode++;
            else throw new ArgumentException($"Unknown failure type: {failure}");
        }
    }

    private static void RegisterSuccess()
    {
        lock (lockObj) { successCount++; }
    }
}
