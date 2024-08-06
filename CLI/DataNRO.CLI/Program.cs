﻿using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using DataNRO.Interfaces;
using Newtonsoft.Json;
using Starksoft.Net.Proxy;

namespace DataNRO
{
    internal class Program
    {
        static string proxyData = "";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            proxyData = Environment.GetEnvironmentVariable("PROXY");
            string[] datas = Environment.GetEnvironmentVariable("DATA").Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string data in datas)
                LoginAndGetData(data);
        }

        static void LoginAndGetData(string data)
        {
            string[] arr = data.Split('|');
            string type = arr[0];
            string host = arr[1];
            ushort port = ushort.Parse(arr[2]);
            string unregisteredUser = arr[3];
            string account = arr[4];
            string password = arr[5];
            string folderName = arr[6];
            if (!Directory.Exists("Data\\" + folderName))
                Directory.CreateDirectory("Data\\" + folderName);
            ISession session;
            try
            {
                Assembly assembly = Assembly.LoadFrom($"DataNRO.{type}.dll");
                session = (ISession)Activator.CreateInstance(assembly.GetType($"DataNRO.{type}.{type}Session"), new object[] { host, port });
                Console.WriteLine($"Server type \"{type}\" has been loaded!");
            }
            catch
            {
                Console.WriteLine($"Server type \"{type}\" not found!");
                return;
            }
            Console.WriteLine($"Connecting to {session.Host}:{session.Port}...");
            if (!TryConnect(session))
                return;
            Console.WriteLine("Connected successfully!");
            IMessageWriter writer = session.MessageWriter;
            writer.SetClientType();
            Thread.Sleep(500);
            writer.ImageSource();
            Thread.Sleep(1000);
            if (!string.IsNullOrEmpty(unregisteredUser))
            {
                Console.WriteLine($"[{session.Host}:{session.Port}] Login as User{new string('*', unregisteredUser.Length - 4)}");
                writer.Login(unregisteredUser, "", 1);
            }
            else
            {
                Console.WriteLine($"[{session.Host}:{session.Port}] Login as {new string('*', account.Length)}");
                writer.Login(account, password, 0);
            }
            Thread.Sleep(1000);
            writer.UpdateMap();
            writer.UpdateSkill();
            writer.UpdateItem();
            writer.UpdateData();
            Thread.Sleep(100);
            writer.ClientOk();
            writer.FinishUpdate();
            Thread.Sleep(500);
            writer.FinishLoadMap();
            Thread.Sleep(3000);
            Location location;
            do
            {
                location = session.Player.location;
            }
            while (location == null || string.IsNullOrEmpty(location.mapName));
            Console.WriteLine($"Current map: {location.mapName} [{location.mapId}], zone {location.zoneId}");
            Thread.Sleep(1000);
            TryGoOutsideIfAtHome(session);
            writer.Chat("DataNRO by ElectroHeavenVN");
            Console.WriteLine($"[{session.Host}:{session.Port}] Disconnect from {session.Host}:{session.Port} in 10s...");
            Thread.Sleep(10000);
            session.Disconnect();
            Console.WriteLine($"[{session.Host}:{session.Port}] Writing data to {folderName}\\...");
            Formatting formatting = Formatting.Indented;
            File.WriteAllText($"Data\\{folderName}\\{nameof(GameData.Maps)}.json", JsonConvert.SerializeObject(session.Data.Maps, formatting));
            File.WriteAllText($"Data\\{folderName}\\{nameof(GameData.NpcTemplates)}.json", JsonConvert.SerializeObject(session.Data.NpcTemplates, formatting));
            File.WriteAllText($"Data\\{folderName}\\{nameof(GameData.MobTemplates)}.json", JsonConvert.SerializeObject(session.Data.MobTemplates, formatting));
            File.WriteAllText($"Data\\{folderName}\\{nameof(GameData.ItemOptionTemplates)}.json", JsonConvert.SerializeObject(session.Data.ItemOptionTemplates, formatting));
            File.WriteAllText($"Data\\{folderName}\\{nameof(GameData.NClasses)}.json", JsonConvert.SerializeObject(session.Data.NClasses, formatting));
            File.WriteAllText($"Data\\{folderName}\\{nameof(GameData.ItemTemplates)}.json", JsonConvert.SerializeObject(session.Data.ItemTemplates, formatting));
            File.WriteAllText($"Data\\{folderName}\\LastUpdated", DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));
            Thread.Sleep(3000);
            session.Dispose();
        }

        static void TryGoOutsideIfAtHome(ISession session)
        {
            IMessageWriter writer = session.MessageWriter;
            Location location = session.Player.location;
            while (location.mapId <= 23 && location.mapId >= 21)
            {
                Console.WriteLine("Go outside");
                int x = 0, y = 336;
                switch (location.mapId)
                {
                    case 21:
                        x = 495;
                        break;
                    case 22:
                        x = 205;
                        break;
                    case 23:
                        x = 475;
                        break;
                }
                writer.CharMove(x, y);
                Thread.Sleep(200);
                writer.CharMove(x, ++y);
                Thread.Sleep(200);
                writer.CharMove(x, --y);
                Thread.Sleep(200);
                writer.GetMapOffline();
                Thread.Sleep(1000);
                writer.FinishLoadMap();
                Thread.Sleep(3000);
                location = session.Player.location;
                Console.WriteLine($"Current map: {location.mapName} [{location.mapId}], zone {location.zoneId}");
            }
        }

        static bool TryConnect(ISession session)
        {
            int retryTimes = 0;
            try
            {
                session.Connect();
            }
            catch
            {
                while (!session.IsConnected)
                {
                    try
                    {
                        session.Connect();
                    }
                    catch
                    {
                        if (retryTimes >= 3)
                            break;
                        Console.WriteLine($"Retry {retryTimes + 1}...");
                        Thread.Sleep(1000);
                    }
                    retryTimes++;
                }
                if (!session.IsConnected)
                {
                    if (!string.IsNullOrEmpty(proxyData))
                    {
                        string[] arrP = proxyData.Split(':');
                        ProxyType proxyType = (ProxyType)Enum.Parse(typeof(ProxyType), arrP[0]);
                        string proxyHost = arrP[1];
                        ushort proxyPort = ushort.Parse(arrP[2]);
                        string proxyUsername = "";
                        string proxyPassword = "";
                        if (arrP.Length > 3)
                            proxyUsername = arrP[3];
                        if (arrP.Length > 4)
                            proxyPassword = arrP[4];
                        retryTimes = 0;
                        Console.WriteLine($"Failed to connect! Retry with proxy {Regex.Replace(proxyHost, "[0-9]", "*")}:{proxyPort}...");
                        try
                        {
                            session.Connect(proxyHost, proxyPort, proxyUsername, proxyPassword, proxyType);
                        }
                        catch
                        {
                            while (!session.IsConnected)
                            {
                                try
                                {
                                    session.Connect(proxyHost, proxyPort, proxyUsername, proxyPassword, proxyType);
                                }
                                catch
                                {
                                    if (retryTimes >= 3)
                                        break;
                                    Console.WriteLine($"Retry {retryTimes + 1}...");
                                    Thread.Sleep(1000);
                                }
                                retryTimes++;
                            }
                            if (!session.IsConnected)
                            {
                                Console.WriteLine($"Failed to connect!");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to connect!");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}