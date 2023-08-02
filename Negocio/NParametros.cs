using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRACKING.Data;

namespace TRACKING.Negocio
{
    public class NParametros
    {
        public static string rutaImagen = DParametro.ObtenerParametro("rutaImagen");
        public static string SerialCamaraB = DParametro.ObtenerParametro("SerialCamaraB");
        public static string SerialCamaraA = DParametro.ObtenerParametro("SerialCamaraA");
        public static int RackPLC = Convert.ToInt32(DParametro.ObtenerParametro("RackPLC"));
        public static int slotPLC = Convert.ToInt32(DParametro.ObtenerParametro("slotPLC"));
        public static string IPPlc = Convert.ToString(DParametro.ObtenerParametro("IPPlc"));
        public static int DebugLiveCameraA = Convert.ToInt32(DParametro.ObtenerParametro("DebugLiveCameraA"));
        public static int DebugLiveCameraB = Convert.ToInt32(DParametro.ObtenerParametro("DebugLiveCameraB"));
        public static int SaveImageA = Convert.ToInt32(DParametro.ObtenerParametro("GuardarImagenA"));
        public static int SaveImageB = Convert.ToInt32(DParametro.ObtenerParametro("GuardarImagenB"));

        #region Parametros Camara A

        public static string CamAExposureMode = DParametro.ObtenerParametro("CamAExposureMode");
        public static string CamAExposureAuto = DParametro.ObtenerParametro("CamAExposureAuto");
        public static int CamAExposureTime = Convert.ToInt32(DParametro.ObtenerParametro("CamAExposureTime"));
        public static string CamAGainAuto = DParametro.ObtenerParametro("CamAGainAuto");
        public static int CamAGainValue = Convert.ToInt32(DParametro.ObtenerParametro("CamAGainValue"));
        public static bool CamAGammaEnableValue =Convert.ToBoolean(DParametro.ObtenerParametro("CamAGammaEnableValue"));
        public static int CamAWidthValue = Convert.ToInt32(DParametro.ObtenerParametro("CamAWidthValue"));
        public static int CamAHeightValue = Convert.ToInt32(DParametro.ObtenerParametro("CamAHeightValue"));
        public static int CamAOffsetXValue = Convert.ToInt32(DParametro.ObtenerParametro("CamAOffsetXValue"));
        public static int CamAOffsetYValue = Convert.ToInt32(DParametro.ObtenerParametro("CamAOffsetYValue"));
        public static bool CamAAcquisitionFrameRateEnableValue = Convert.ToBoolean(DParametro.ObtenerParametro("CamAAcquisitionFrameRateEnable"));
        public static int CamAAquisitionFrameRateValue = Convert.ToInt32(DParametro.ObtenerParametro("CamAAcquisitionFrameRateValue"));
        #endregion


        #region Parametros Camara B

        public static string CamBExposureMode = DParametro.ObtenerParametro("CamBExposureMode");
        public static string CamBExposureAuto = DParametro.ObtenerParametro("CamBExposureAuto");
        public static int CamBExposureTime = Convert.ToInt32(DParametro.ObtenerParametro("CamBExposureTime"));
        public static string CamBGainAuto = DParametro.ObtenerParametro("CamBGainAuto");
        public static int CamBGainValue = Convert.ToInt32(DParametro.ObtenerParametro("CamBGainValue"));
        public static bool CamBGammaEnableValue = Convert.ToBoolean(DParametro.ObtenerParametro("CamBGammaEnableValue"));
        public static int CamBWidthValue = Convert.ToInt32(DParametro.ObtenerParametro("CamBWidthValue"));
        public static int CamBHeightValue = Convert.ToInt32(DParametro.ObtenerParametro("CamBHeightValue"));
        public static int CamBOffsetXValue = Convert.ToInt32(DParametro.ObtenerParametro("CamBOffsetXValue"));
        public static int CamBOffsetYValue = Convert.ToInt32(DParametro.ObtenerParametro("CamBOffsetYValue"));
        public static bool CamBAcquisitionFrameRateEnableValue = Convert.ToBoolean(DParametro.ObtenerParametro("CamBAcquisitionFrameRateEnable"));
        public static int CamBAquisitionFrameRateValue = Convert.ToInt32(DParametro.ObtenerParametro("CamBAcquisitionFrameRateValue"));



        #endregion

        public static void ActualizarParametro(string Parametro, string tipo, string valor)
            {
                try
                {
                    DParametro.ActualizarParametro(Parametro, tipo, valor);
                }
                catch (Exception ex)
                {
                    throw (ex);
                }

            }

        
    }
}
