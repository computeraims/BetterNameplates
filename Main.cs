﻿using BetterNameplates.Utils;
using SDG.Framework.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterNameplates
{
    public class Main : MonoBehaviour, IModuleNexus
    {
        private static GameObject BetterNameplatesObject;

        public static Main Instance;

        public static Config Config;

        public void initialize()
        {
            Instance = this;
            Console.WriteLine("BetterNameplates by Corbyn loaded");

            UnityThread.initUnityThread();

            BetterNameplatesObject = new GameObject("BetterNameplates");
            DontDestroyOnLoad(BetterNameplatesObject);

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Console.WriteLine(path);

            ConfigHelper.EnsureConfig($"{path}{Path.DirectorySeparatorChar}config.json");

            Config = ConfigHelper.ReadConfig($"{path}{Path.DirectorySeparatorChar}config.json");

            Console.WriteLine(Config);

            BetterNameplatesObject.AddComponent<NameplatesManager>();
        }


        public void shutdown()
        {
            Instance = null;
        }
    }
}