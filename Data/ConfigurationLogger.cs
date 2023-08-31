using IntecolWCS.logger.Entity;
using IntecolWCS.logger.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRACKING.Data
{
    public static class ConfigurationLogger
    {
        public static void ConfigurationLooger()
        {
            ParametrosLogger _ParametrosLogger;
            _ParametrosLogger = new ParametrosLogger();
            _ParametrosLogger.ConnString = "Data Source=.\\SQLEXPRESS;Initial Catalog=IvisTernium;Integrated Security=True";
            _ParametrosLogger.WindowsEventLogActivo = false;
            _ParametrosLogger.BaseDeDatosLogActivo = true;
            _ParametrosLogger.ArchivoLocalLogActivo = true;

            LoggerFacade.configurarParametrosLogger(ref _ParametrosLogger);
            LoggerFacade.doLog(LoggerFacade.NivelLog.INFO, "Inicio aplicacion IVIS");
        }
    }
}
