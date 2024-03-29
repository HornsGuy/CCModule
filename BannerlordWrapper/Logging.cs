﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BannerlordWrapper
{
    public class Logging
    {
        static Logging _instance;
        public static Logging Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new Logging();
                }
                return _instance;
            }
        }

        private bool start = false;

        private string logFile;

        private LogLevel logLevel;

        // Probably needed for thread safety
        Mutex logLock = new Mutex();

        public enum LogLevel
        {
            Trace = 0,
            Debug = 1,
            Info = 2,
            Warn = 3,
            Error = 4,
        }

        public void StartLogging(string logPath, LogLevel level=LogLevel.Info)
        {
            Directory.CreateDirectory(logPath);

            logLevel = level;
            logFile = logPath + "\\Log_"+ DateTime.Now.ToString("MM-dd-yyyy_HH-mm-ss") + ".txt";
            start = true;
            Info("Started Logging to '" + logFile + "'");
        }

        public void StopLogging()
        {
            start = false;
            Info("Stopped Logging");
        }

        public void Trace(string message)
        {
            AddMessage(message, LogLevel.Trace);
        }

        public void Debug(string message)
        {
            AddMessage(message, LogLevel.Debug);
        }

        public void Info(string message)
        {
            AddMessage(message, LogLevel.Info);
        }

        public void Warn(string message)
        {
            AddMessage(message, LogLevel.Warn);
        }

        public void Error(string message)
        {
            AddMessage(message, LogLevel.Error);
        }

        private void AddMessage(string message, LogLevel level)
        {
            if(start && level >= logLevel)
            {
                logLock.WaitOne();
                File.AppendAllText(logFile, CreateMessage(message, level));
                logLock.ReleaseMutex();
            }
        }

        private string CreateMessage(string message, LogLevel level)
        {
            return $"[{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff")}] - "+level.ToString()+" - "+message+"\n";
        }

    }
}
