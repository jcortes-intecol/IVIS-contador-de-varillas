using OpenCvSharp;
using SpinnakerNET;
using SpinnakerNET.GenApi;
using System.Drawing;
using System.Drawing.Imaging;
using TRACKING;
using prueba;
using System.Diagnostics;
using TRACKING.Negocio;

namespace Camara.Modbus.Servicio.control
{

    public class ControlCameraB
    {
        // public static int condicion;

        public static int contador = 0;
        public static int contador2 = 0;

        public static List<OpenCvSharp.Mat> buffer = new List<OpenCvSharp.Mat>();
        public static int RecorrerBuffer = 0;
        public static int contreBuffer = 0;
        public static bool StateCamera;
        public static bool stopmed;
        public static bool Statecont;
        //  static ConcurrentQueue<Bitmap> processingQueue = new ConcurrentQueue<Bitmap>();
        //static ConcurrentQueue<Bitmap> acquisitionBuffer = new ConcurrentQueue<Bitmap>();

        public static Bitmap ImageProcess;
        static object lockObj = new object();
        public static int DisableHeartbeat(IManagedCamera cam, INodeMap nodeMap, INodeMap nodeMapTLDevice)
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
        public static void Capturando(IManagedCamera cam, CancellationToken PeticionPlc)

        {
            IManagedImageProcessor processor = new ManagedImageProcessor();
            //*************** determinar el color de la imagenes en el procesador 
            processor.SetColorProcessing(ColorProcessingAlgorithm.HQ_LINEAR);
            NParametros.ActualizarParametro("StateCameraB", "Int", Convert.ToString(0));
            StateCamera = false;

            Queue<Bitmap> acquisitionBuffer = new Queue<Bitmap>();
            while (!PeticionPlc.IsCancellationRequested)
            //for (int imageCnt = 0; imageCnt < NumImages; imageCnt++) guarda un determinado numero de imagenes 
            {


                //double timeout = 18446744073709551615;




                try
                {
                    while (!stopmed)//para parar la medicion desde el plc 
                    {
                        Statecont = true;
                        using (IManagedImage rawImage = cam.GetNextImage())

                        // using (IManagedImage rawImage = cam.GetNextImage(10))//// habia un 10 
                        {
                            //
                            // Ensure image completion



                            if (rawImage.IsIncomplete)
                            {
                                Console.WriteLine("Image incomplete with image status {0}...", rawImage.ImageStatus);
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
                                    mainB.CountVarillasCameraB(imagen);


                                }





                            }
                        }
                    }
                    Statecont = false;
                    //Cv2.DestroyWindow("Imagen final B");
                    //Cv2.DestroyWindow("imagen segmentada Cam B");
                }

                catch (SpinnakerException ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                    StateCamera = true;
                }

            }
        }


        public static Thread StartTheThread(IManagedCamera cam, CancellationToken PeticionPlc)
        {
            var t = new Thread(() => Capturando(cam, PeticionPlc));
            t.Priority = ThreadPriority.Highest;
            t.Start();
            return t;
        }


        public static int AcquireImagesCameraB(IManagedCamera cam, INodeMap nodeMap, INodeMap nodeMapTLDevice, CancellationToken PeticionPlc, string serialCam1)
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

                //if (DisableHeartbeat(cam, nodeMap, nodeMapTLDevice) != 0)
                //{
                //    return -1;
                //}

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


                StartTheThread(cam, PeticionPlc);
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






        public static int RunSingleCameraB(IManagedCamera cam, CancellationToken PeticionPlc, string serialCam1)
        {
            int result = 0;

            try
            {
                // Retrieve TL device nodemap and print device information
                INodeMap nodeMapTLDevice = cam.GetTLDeviceNodeMap();

                // result = PrintDeviceInfo(nodeMapTLDevice);

                // Initialize camera
                cam.Init();

                // Retrieve GenICam nodemap
                INodeMap nodeMap = cam.GetNodeMap();



                //// exposicion
                //cam.ExposureMode.Value = "Timed";
                //cam.ExposureAuto.Value = "Off";
                //cam.ExposureTime.Value = 1500;

                //// Ganancia
                //cam.GainAuto.Value = "Off";
                //cam.Gain.Value = 30;

                //// Gamma
                //cam.GammaEnable.Value = false;

                ////Roi
                //cam.Width.Value = 572;
                //cam.Height.Value = 714;

                //cam.OffsetX.Value = 808;
                //cam.OffsetY.Value = 196;


                //// Frame rate
                //cam.AcquisitionFrameRateEnable.Value = true;
                //cam.AcquisitionFrameRate.Value = 110;



                // exposicion
                cam.ExposureMode.Value = NParametros.CamBExposureMode;
                cam.ExposureAuto.Value = NParametros.CamBExposureAuto;
                cam.ExposureTime.Value = NParametros.CamBExposureTime;

                // Ganancia
                cam.GainAuto.Value = NParametros.CamBGainAuto;
                cam.Gain.Value = NParametros.CamBGainValue;

                // Gamma
                cam.GammaEnable.Value = NParametros.CamBGammaEnableValue;

                //Roi
                cam.Width.Value = NParametros.CamBWidthValue;
                cam.Height.Value = NParametros.CamBHeightValue;

                cam.OffsetX.Value = NParametros.CamBOffsetXValue;
                cam.OffsetY.Value = NParametros.CamBOffsetYValue;


                // Frame rate
                cam.AcquisitionFrameRateEnable.Value = NParametros.CamBAcquisitionFrameRateEnableValue;
                cam.AcquisitionFrameRate.Value = NParametros.CamBAquisitionFrameRateValue;
                //cam.AcquisitionFrameRate.Value = 110;

                // Acquire images
                result = result | AcquireImagesCameraB(cam, nodeMap, nodeMapTLDevice, PeticionPlc, serialCam1);



                cam.DeInit();


            }
            catch (SpinnakerException ex)
            {
                //Escribir log de eventos, no inicio la camara
                Console.WriteLine("Error: {0}", ex.Message);

                result = -1;
            }

            return result;
        }



        public static int MainCameraB(string serialCam1, CancellationToken ParametroModbus)
        {
           
            int result = 0;

            ManagedSystem system = new ManagedSystem();

            // Retrieve list of cameras from the system

            ManagedCameraList camList1 = system.GetCameras();
        
            IManagedCamera cam1 = camList1.GetBySerial(serialCam1);


            if (cam1 != null)
            {
                try
                {
                    result = result | RunSingleCameraB(cam1, ParametroModbus, serialCam1);
                    StateCamera = false;
                }
                catch (SpinnakerException ex)
                {
                    StateCamera = true;
                    NParametros.ActualizarParametro("StateCameraB", "Int", Convert.ToString(1));

                    Console.WriteLine("Error: {0}", ex.Message);
                    result = -1;
                    //EScribir en base de datos Log de eventos 
                }
            }
            else
            {
                StateCamera = true;
                NParametros.ActualizarParametro("StateCameraB", "Int", Convert.ToString(1));

            }

            // Clear camera list before releasing system
            camList1.Clear();

            // Release system
            system.Dispose();

            //********Escribir log de eventos "termino la captura de imagenes"
            return result;
        }
    }
}



