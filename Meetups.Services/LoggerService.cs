﻿using log4net;
using log4net.Config;
using Meetups.Domain.Interfaces.Services;
using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Meetups.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(LoggerService));
        public LoggerService()
        {
            try
            {
                XmlDocument log4netConfig = new XmlDocument();

                using (var fs = File.OpenRead("log4net.config"))
                {
                    log4netConfig.Load(fs);

                    var repo = LogManager.CreateRepository(
                            Assembly.GetEntryAssembly(),
                            typeof(log4net.Repository.Hierarchy.Hierarchy));
                    string ruta = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
                    GlobalContext.Properties["FilePath"] = ruta.Substring(6, ruta.Length - 6);
                    XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error", ex);
            }
        }

        public void LogInformation(string message)
        {
            _logger.Info(message);
        }
        public void LogInformation(string message, Exception ex)
        {
            _logger.Info(message, ex);
        }
        public void LogAdvertencia(string message)
        {
            _logger.Warn(message);
        }
        public void LogAdvertencia(string message, Exception ex)
        {
            _logger.Warn(message, ex);
        }
        public void LogError(string message)
        {
            _logger.Error(message);
        }
        public void LogError(string message, Exception ex)
        {
            _logger.Error(message, ex);
        }

    }
}