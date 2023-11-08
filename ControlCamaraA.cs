using OpenCvSharp;
using prueba;
using SpinnakerNET.GenApi;
using SpinnakerNET;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using prueba;
using System.Reflection;
using TRACKING.Negocio;
namespace TRACKING
{
    public class ControlCamaraA
    {

        // public static int condicion;

        public static int contador = 0;
        public static int contador2 = 0;

        public static List<OpenCvSharp.Mat> buffer = new List<OpenCvSharp.Mat>();
        public static int RecorrerBuffer = 0;
        public static int contreBuffer = 0;
        //  static ConcurrentQueue<Bitmap> processingQueue = new ConcurrentQueue<Bitmap>();
        //static ConcurrentQueue<Bitmap> acquisitionBuffer = new ConcurrentQueue<Bitmap>();
        public static int connectionCameraA=0;
        public static Bitmap ImageProcess;
        static object lockObj = new object();
        public static bool StateCamera;
        public static bool Statecont;
        public static bool stopmed;//variable para parar la medicion
        public static int DisableHeartbeatA(IManagedCamera cam, INodeMap nodeMap, INodeMap nodeMapTLDevice)
        {
            Console.WriteLine("Checking device type to see if we need to disable the camera's heartbeat...\n\n");


            IEnum iDeviceType = nodeMapTLDevice.GetNode<IEnum>("DeviceType");
            IEnumEntry iDeviceTypeGEV = iDeviceType.GetEntryByName("GigEVision");
            // We first need to confirm that we're working with a GEV camera
            if (iDeviceType != null && iDeviceType.IsReadable)
            {
                if (iDeviceType.Value == iDeviceTypeGEV.Value)
                {

                    IBool iGEVHeartbeatDisable = nodeMap.GetNode<IBool>("GevGVCPHeartbeatDisable");
                    if (iGEVHeartbeatDisable == null || !iGEVHeartbeatDisable.IsWritable)
                    {

                    }
                    else
                    {
                        iGEVHeartbeatDisable.Value = true;

                    }
                }
                else
                {
                    // Console.WriteLine("Camera does not use GigE interface. Resuming normal execution...\n\n");
                }
            }
            else
            {
                //Console.WriteLine("Unable to access TL device nodemap. Aborting...");
                return -1;
            }

            return 0;
        }

        public static void CapturandoA(IManagedCamera cam, CancellationToken PeticionPlc)

        {
            IManagedImageProcessor processor = new ManagedImageProcessor();
            //*************** determinar el color de la imagenes en el procesador 
            processor.SetColorProcessing(ColorProcessingAlgorithm.HQ_LINEAR);
            NParametros.ActualizarParametro("StateCameraA", "Int", Convert.ToString(0));
            StateCamera = false;

            while (!PeticionPlc.IsCancellationRequested)
            //for (int imageCnt = 0; imageCnt < NumImages; imageCnt++) guarda un determinado numero de imagenes 
            {


                //double timeout = 18446744073709551615;




                try
                {
                    while (!stopmed) //para cerrar la aplocacion desde el plc 
                    {
                        Statecont = true;
                        using (IManagedImage rawImage = cam.GetNextImage())

                        // using (IManagedImage rawImage = cam.GetNextImage(10))//// habia un 10 
                        {
                            //
                            // Ensure image completion



                            if (rawImage.IsIncomplete)
                            {
                                Console.WriteLine("Image cam A incomplete with image status {0}...", rawImage.ImageStatus);
                            }
                            else
                            {

                                uint width = rawImage.Width;

                                uint height = rawImage.Height;




                                //
                                // Convert image to mono 8 pero se puede cambiar a otro para este ejemplo tenemos RGB8

                                using (
                                IManagedImage convertedImage = processor.Convert(rawImage, PixelFormatEnums.Mono8))
                                {
                                    // Create a unique filename
                                    // String filename = "C:\\Users\\Usuario\\Pictures\\Saved Pictures\\INTECOL.jpg";

                                    //  string filename = CamaraTerniumDA.ObtenerParametro("rutaImagen") + "\\INTECOLB.jpg";

                                    //contador++;
                                    //String filename = "C:\\Users\\intecol\\Pictures\\prueba\\img" + Convert.ToString(contador) + ".jpg";
                                    //// Save image

                                    //convertedImage.Save(filename); //linea de codigo para guardar las imagenes 


                                    Bitmap bmp = convertedImage.bitmap;
                                    // acquisitionBuffer.Enqueue(bmp);



                                    // Convertir el bitmap a un arreglo de bytes
                                    byte[] imageData;
                                    using (MemoryStream stream = new MemoryStream())
                                    {
                                        bmp.Save(stream, ImageFormat.Bmp);
                                        imageData = stream.ToArray();
                                    }

                                    // Crear una imagen Mat a partir de los datos de imagen en formato BGR
                                    Mat imagen = Cv2.ImDecode(imageData, ImreadModes.Color);

                                    mainA.CountVarillasCameraA(imagen);



                                }




                                rawImage.Release();






                            }
                        }
                    }
                    Statecont = false;
                    //Cv2.DestroyWindow("Imagen final A");
                    //Cv2.DestroyWindow("imagen segmentada Cam A");
                }

                catch (SpinnakerException ex)
                {

                    Console.WriteLine("Error: {0}", ex.Message);
                    StateCamera = true;
                }

            }
        }

        public static Thread StartTheThreadA(IManagedCamera cam, CancellationToken PeticionPlc)
        {
            var t = new Thread(() => CapturandoA(cam, PeticionPlc));
            t.Priority = ThreadPriority.Highest;
            t.Start();
            return t;
        }

        public static int AcquireImagesCameraA(IManagedCamera cam, INodeMap nodeMap, INodeMap nodeMapTLDevice, CancellationToken PeticionPlc, string serialCam1)
        {
            int result = 0;


            try
            {

                IEnum iAcquisitionMode = nodeMap.GetNode<IEnum>("AcquisitionMode");


                // Retrieve entry node from enumeration node
                IEnumEntry iAcquisitionModeContinuous = iAcquisitionMode.GetEntryByName("Continuous");

                // Set symbolic from entry node as new value for enumeration node
                iAcquisitionMode.Value = iAcquisitionModeContinuous.Symbolic;

                Console.WriteLine("Acquisition mode set to continuous...");

#if DEBUG
                Console.WriteLine("\n\n*** DEBUG ***\n\n");
                // If using a GEV camera and debugging, should disable heartbeat first to prevent further issues

                // if (DisableHeartbeat(cam, nodeMap, nodeMapTLDevice) != 0)
                //  {
                //    return -1;
                // }

                Console.WriteLine("\n\n*** END OF DEBUG ***\n\n");
#endif
                //****** adquisicion de imagenes de manera continua
                cam.BeginAcquisition();

                Console.WriteLine("Acquiring images...");

                String deviceSerialNumber = "";

                IString iDeviceSerialNumber = nodeMapTLDevice.GetNode<IString>("DeviceSerialNumber");
                if (iDeviceSerialNumber != null && iDeviceSerialNumber.IsReadable)
                {
                    deviceSerialNumber = iDeviceSerialNumber.Value;

                    Console.WriteLine("Device serial number retrieved as {0}...", deviceSerialNumber);
                }
                Console.WriteLine();

                //********************** capturar imagenes y guardar en funcion de lo que envie modbus********


                StartTheThreadA(cam, PeticionPlc);
                //cam.EndAcquisition();
                //cam.DeInit();

            }
            catch (SpinnakerException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                StateCamera = true;

                result = -1;
            }

            return result;
        }






        public static int RunSingleCameraA(IManagedCamera cam1, CancellationToken PeticionPlc, string serialCam1)
        {
            int result = 0;
            
            try
            {
                // Retrieve TL device nodemap and print device information
                INodeMap nodeMapTLDevice = cam1.GetTLDeviceNodeMap();

                // result = PrintDeviceInfo(nodeMapTLDevice);

                // Initialize camera
                cam1.Init();

                // Retrieve GenICam nodemap
                INodeMap nodeMap = cam1.GetNodeMap();


              
                // exposicion
                cam1.ExposureMode.Value = NParametros.CamAExposureMode;
                cam1.ExposureAuto.Value = NParametros.CamAExposureAuto;
                cam1.ExposureTime.Value = NParametros.CamAExposureTime;

                // Ganancia
                cam1.GainAuto.Value = NParametros.CamAGainAuto;
                cam1.Gain.Value = NParametros.CamAGainValue;

                // Gamma
                cam1.GammaEnable.Value = NParametros.CamAGammaEnableValue;

                //Roi
                cam1.Width.Value = NParametros.CamAWidthValue;
                cam1.Height.Value = NParametros.CamAHeightValue;

                cam1.OffsetX.Value = NParametros.CamAOffsetXValue;
                cam1.OffsetY.Value = NParametros.CamAOffsetYValue;


                cam1.AcquisitionFrameRateEnable.Value = NParametros.CamAAcquisitionFrameRateEnableValue;
                cam1.AcquisitionFrameRate.Value = NParametros.CamAAquisitionFrameRateValue;


                // Acquire images
                result = result | AcquireImagesCameraA(cam1, nodeMap, nodeMapTLDevice, PeticionPlc, serialCam1);
                


                cam1.DeInit();


            }
            catch (SpinnakerException ex)
            {
                //Escribir log de eventos, no inicio la camara
              
                result = -1;
            }

            return result;
        }



        public static int MainCameraA(string serialCam1, CancellationToken ParametroModbus)
        {
            

            int result = 0;
            ManagedSystem system = new ManagedSystem();



            // Retrieve list of cameras from the system
            
            ManagedCameraList camList = system.GetCameras();

            IManagedCamera cam = camList.GetBySerial(serialCam1);
            if (cam != null)
            {

                try
                {
                    result = result | RunSingleCameraA(cam, ParametroModbus, serialCam1);
                    StateCamera = false;
                }

                catch (SpinnakerException ex)
                {
                    StateCamera = true;
                    NParametros.ActualizarParametro("StateCameraA", "Int", Convert.ToString(1));
                    Console.WriteLine("Error: {0}", ex.Message);
                    result = -1;
                    //EScribir en base de datos Log de eventos 
                }
             
            }
            else
            {
                StateCamera = true;
                NParametros.ActualizarParametro("StateCameraA", "Int", Convert.ToString(1));
            }
            // Clear camera list before releasing system
            camList.Clear();

            // Release system
            system.Dispose();

            //********Escribir log de eventos "termino la captura de imagenes"
            return result;
        }
    }
}
