using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Traking;
using prueba;
using OpenCvSharp.Tracking;
using System.Drawing;
using OpenCvSharp.ImgHash;
using Point = OpenCvSharp.Point;
using static System.Net.Mime.MediaTypeNames;
using Camara.Modbus.Servicio.control;
using TRACKING;
using TRACKING.Negocio;
using TRACKING.Entity;
using TRACKING.Data;

namespace prueba
{
    public class mainB
    {
        public static int cont = 0;
        public static Thread MeasureCameraA;
        public static Thread MeasureCameraB;
        public static Thread conexionplc;
        static CancellationTokenSource cancelationTokenCameraA = new CancellationTokenSource();
        static CancellationTokenSource cancelationTokenCameraB = new CancellationTokenSource();

        static CancellationToken cancelationTokenCloseCameraB = cancelationTokenCameraB.Token;
        static CancellationToken cancelationTokenCloseCameraA = cancelationTokenCameraA.Token;

        public static int IdReceta;
        public static bool mediconA;
        public static bool medicionB;
        public static int auxB = 0;
        public static int auxA = 0;
        public static int Blocksize;
        public static int Double;
        public static int framesProcesados = 0;

        //Se agrega la imagen promedidada para eliminar el ruido de fondo (jairo 26/06/2023)
        //static Mat fondo = Cv2.ImRead("C:\\Users\\intecol\\Documents\\Despliegue\\IVIS_TRACKING\\imagen_promediada.png", ImreadModes.Grayscale);
        static Mat fondo = Cv2.ImRead("C:\\Users\\intecol\\Documents\\Despliegue\\IVIS_TRACKING\\imagen_prueba_resta.png", ImreadModes.Grayscale);

        //public static Mat fondoNuevo = Cv2.ImRead("C:\\Users\\intecol\\Documents\\Despliegue\\IVIS_TRACKING\\fondonuevo.png", ImreadModes.Grayscale);

        //public static Mat fondoNuevoA = Cv2.ImRead("C:\\Users\\intecol\\Documents\\Despliegue\\IVIS_TRACKING\\fondonuevoa.jpg", ImreadModes.Grayscale);

        public static Mat CameraObjectDetectionB(Mat image_rgb, Mat image_binarizada)
        {

            OpenCvSharp.Point[][] contornos_anterior;
            HierarchyIndex[] Indexes_anterior;

            OpenCvSharp.Point[][] contornos_actual;
            HierarchyIndex[] Indexes_actual;

            Cv2.FindContours(image_binarizada, out contornos_actual, out Indexes_actual, RetrievalModes.External, method: ContourApproximationModes.ApproxSimple);
            Mat image_binarizada_rgb = new Mat();
            Cv2.CvtColor(image_binarizada, image_binarizada_rgb, ColorConversionCodes.GRAY2BGR);
            image_rgb = tracking_cameraB.TrackerObjecCameraB(image_rgb, image_binarizada_rgb, contornos_actual, Indexes_actual);

            return (image_rgb);

        }


        public static Mat PhiltroMorphologyCameraB(Mat imagenUmbralizada, int kernel_hor, int kernel_Ver, int kernel_circ_1, int kernel_circ_2)
        {

            // Aplica una transformación morfológica de erosión
            //Mat imagenErocionada = new Mat();
            //Mat imagenErocionada_circ = new Mat();




            //Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(kernel_hor, kernel_Ver));
            //Mat kernel_circ = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(kernel_circ_1, kernel_circ_2));

            ////Cv2.MorphologyEx(imagenUmbralizada, imagenOpening, MorphTypes.Open, kernel_circ);
            //Cv2.Erode(imagenUmbralizada, imagenErocionada_circ, kernel_circ);
            //Cv2.Erode(imagenErocionada_circ, imagenErocionada, kernel);

            //return imagenErocionada;

            //*********************** nueva modificacion*******************************
            Mat SSQUARE_KERNEL_VER = Mat.Ones(MatType.CV_8UC1, kernel_Ver * 1);
            Mat SQUARE_KERNEL_HOR = Mat.Ones(MatType.CV_8UC1, 1 * kernel_hor);
            //Mat SQUARE = Mat.Ones(MatType.CV_8UC1, 3 * 3);
            Mat kernel1 = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(kernel_circ_1, kernel_circ_2));
            var imagen = new Mat();
            Cv2.MorphologyEx(imagenUmbralizada, imagen, MorphTypes.Erode, kernel1, iterations: 2);
            Cv2.MorphologyEx(imagen, imagen, MorphTypes.Open, SSQUARE_KERNEL_VER);
            Cv2.MorphologyEx(imagen, imagen, MorphTypes.Open, SQUARE_KERNEL_HOR);




            return imagen;
        }




        public static void CountVarillasCameraB(Mat frame)
        {
            try
            {
                framesProcesados++;
                Referencias referenciaB = NReferencia.ObtenerReferenciaB(IdReceta)[0];
                //
                int umbral_threshold = referenciaB.Umbral_Threshold;
                int kernel_hor = referenciaB.kernel_hor;
                int kernel_ver = referenciaB.Kernel__ver;
                int kernel_circ_hor = referenciaB.Kerne_cir_chor;
                int kernel_circ_ver = referenciaB.kernel_circ_ver;
                int contraste_mult = referenciaB.contraste_mult;
                int contraste_sum = referenciaB.contraste_sun;

                int Save_Img = Convert.ToInt32(DParametro.ObtenerParametro("GuardarImagenB"));

                Mat dst1 = new Mat();

                // Procesar la imagen aquí, por ejemplo, convertirla a escala de grises
                Cv2.CvtColor(frame, dst1, ColorConversionCodes.RGB2GRAY);

                //Mat resta = new Mat();
                //Se agrega para quitar el ruido de fondo (jairo 26/06/2023)
                //deja lo antrior  hagamos la prueba aunque la verdad necesitamos restar ese fondo para todas porque a todas hayq ue realzar grises 

                Cv2.Subtract(dst1, fondo, dst1);

                //dst1.ConvertTo(dst1, -1, contraste_mult, contraste_sum);
                Mat roiSegmentado = new Mat();

                Mat imageFloat = new Mat();
                dst1.ConvertTo(imageFloat, MatType.CV_32F);



                //if (IdReceta <= 9 )
                //{

                //    Mat outputImage = new Mat();
                //    Cv2.Threshold(dst1, outputImage, 100, 255, ThresholdTypes.Binary);
                //    Cv2.Threshold(outputImage, outputImage, 120, 255, ThresholdTypes.BinaryInv);

                //    //Se invierte la imgen porque queda blanca
                //    Cv2.BitwiseNot(outputImage, outputImage);

                //}
                ////Cambios 1 1/8 jairo (30 de junio)
                //if (IdReceta > 11 ) // para las referencias grandes se le aplica un pow suave listo ?? 
                //{
                //    Cv2.Pow(imageFloat, 1.2, dst1);
                //}
                //if (IdReceta >6 && IdReceta<=11) // para estas ref ese valor esta bien  
                //{
                //    Cv2.Pow(imageFloat, 1.3, dst1);
                //}
                ////Cambios 1 1/8 jairo (30 de junio)
                //if (IdReceta <= 6 )//habia un == se le aplica un pow de 1.4 para todas las referencias desde media hacia abajo
                //{
                //    Cv2.Pow(imageFloat, 1.4, dst1);
                //}
                //if(IdReceta == 12)//habia un == se le aplica un pow de 1.4 para todas las referencias desde media hacia abajo
                //{
                //    Cv2.Pow(imageFloat, 1.45, dst1);
                //}
                //if (IdReceta == 2)//habia un == se le aplica un pow de 1.4 para todas las referencias desde media hacia abajo
                //{
                //    Cv2.Pow(imageFloat, 1.28, dst1);
                //}
                //if (IdReceta == 8)//habia un == se le aplica un pow de 1.4 para todas las referencias desde media hacia abajo
                //{
                //    Cv2.Pow(imageFloat, 1.3, dst1);
                //}
                if (IdReceta != 9 || IdReceta != 6)
                {
                    Cv2.Pow(imageFloat, 1.3, dst1);
                }
                if (IdReceta == 9 || IdReceta == 6)
                {
                    Cv2.Pow(imageFloat, 1.4, dst1);
                }
                dst1.ConvertTo(dst1, MatType.CV_8UC1);

                //Mat mask = new Mat();
                //Cv2.Threshold(dst1, segmentacionRGB2, 130, 255, ThresholdTypes.Binary);
                Cv2.Threshold(dst1, roiSegmentado, umbral_threshold, 255, ThresholdTypes.Binary);

                if (IdReceta >= 1 && IdReceta < 9 && IdReceta != 5)
                {
                    Blocksize = 97;
                    Double = 0;
                }
                if (IdReceta == 5 || IdReceta >= 9)
                {
                    Blocksize = 127;
                    Double = 2;
                }
                if (IdReceta == 6)
                {
                    Blocksize = 127;
                    Double = 0;
                }
                //Cambios 1 1/8 jairo (30 de junio)
                if (IdReceta == 11 || IdReceta == 12 || IdReceta == 8)
                {
                    Blocksize = 127;
                    Double = 2;
                }
                //cambios para habilitar el adaptative (jairo 27 de junio)
                Mat imagen_adaptative = new Mat();
                Cv2.AdaptiveThreshold(roiSegmentado, imagen_adaptative, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, Blocksize, Double);
                Cv2.BitwiseAnd(imagen_adaptative, roiSegmentado, roiSegmentado);

                // Sustraer el fondo de la imagen original utilizando la máscara
                Mat result = new Mat();

                Mat ImgSegmentado = new Mat();
                Mat result_morph;
                result_morph = PhiltroMorphologyCameraB(roiSegmentado, kernel_hor, kernel_ver, kernel_circ_hor, kernel_circ_ver);

                //dst1 = CameraObjectDetectionB(roiSegmentado, result_morph);

                dst1 = CameraObjectDetectionB(dst1, result_morph);







                if (Save_Img == 1)
                {
                    Cv2.ImWrite("C:\\Users\\intecol\\Documents\\Despliegue\\ImagenesB\\image_" + Convert.ToString(cont) + ".jpg", frame);
                    Cv2.ImWrite("C:\\Users\\intecol\\Documents\\Despliegue\\ImagenesB\\image_" + Convert.ToString(cont) + "_procesada.jpg", result_morph);
                    cont++;
                    if (cont >= 100)
                    {
                        //string parametro, string tipo, string valor)
                        DParametro.ActualizarParametro("GuardarImagenB", "int", "0");
                        cont = 0;
                    }
                }
                dst1.Dispose();


                GC.Collect();

            }
            catch { Console.WriteLine("falle"); }
        }


        static void Main()
        {

            Console.WriteLine("*******************IVIS-COUNTER**************************************");
            Console.WriteLine("DESARROLLADO POR INTECOL SAS");

            MeasureCameraB = new Thread(() => ControlCameraB.MainCameraB(NParametros.SerialCamaraB, cancelationTokenCloseCameraB));
            MeasureCameraB.Start();
            MeasureCameraA = new Thread(() => ControlCamaraA.MainCameraA(NParametros.SerialCamaraA, cancelationTokenCloseCameraA));
            MeasureCameraA.Start();
            Class2.ConectionPLC();


        }

    }
}
