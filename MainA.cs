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
using TRACKING.Entity;
using TRACKING.Negocio;
using TRACKING.Data;

namespace prueba
{
    internal class mainA
    {
        public static int cont = 0;
        static CancellationToken cancelationTokenCloseCameraB;
        public static int Blocksize;
        public static int Double;
        public static int imagcont;

        //Se agrega la imagen promedidada para eliminar el ruido de fondo (jairo 26/06/2023)
        static Mat fondo = Cv2.ImRead("C:\\Users\\intecol\\Documents\\Despliegue\\IVIS_TRACKING\\imagen_promediada_A.jpg", ImreadModes.Grayscale);

        public static Mat CameraObjectDetectionA(Mat image_rgb, Mat image_binarizada)
        {


            //try { 

            OpenCvSharp.Point[][] contornos_anterior;
            HierarchyIndex[] Indexes_anterior;

            OpenCvSharp.Point[][] contornos_actual;
            HierarchyIndex[] Indexes_actual;
                //esta funcion recibe imagenes enescala de grises si no da error 

            Cv2.FindContours(image_binarizada, out contornos_actual, out Indexes_actual, RetrievalModes.External, method: ContourApproximationModes.ApproxSimple); 
            Mat image_binarizada_rgb = new Mat();
            Cv2.CvtColor(image_binarizada, image_binarizada_rgb, ColorConversionCodes.GRAY2BGR);

            image_rgb = tracking_cameraA.TrackerObjecCameraA(image_rgb, image_binarizada_rgb, contornos_actual, Indexes_actual);
            //if(imagcont==1) 
            //    {
            //        Cv2.DestroyWindow("Imagen final A");
            //        imagcont = 0;
            //    }

           
            //}
            //catch (Exception e) 
            //{ 
            //    image_rgb = image_rgb;
            //    Cv2.ImShow("Imagen final A", image_binarizada);
            //    Cv2.WaitKey(1);
            //    imagcont = 1;
            //}
            // self.Release();
            return (image_rgb);
        }


        public static Mat PhiltroMorphologyCameraA(Mat imagenUmbralizada, int kernel_hor, int kernel_Ver, int kernel_circ_1, int kernel_circ_2)
        {

            //// Aplica una transformación morfológica de erosión
            //Mat imagenErocionada = new Mat();
            //Mat imagenOpening = new Mat();
            //Mat imagenErocionada_circ = new Mat();
            //imagenUmbralizada = imagenUmbralizada;


            // Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(kernel_hor, kernel_Ver));
            //Mat kernel_circ = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(kernel_circ_1, kernel_circ_2));

            ////Cv2.MorphologyEx(imagenUmbralizada, imagenOpening, MorphTypes.Open, kernel_circ);
            //Cv2.Erode(imagenUmbralizada, imagenErocionada_circ, kernel_circ);
            //Cv2.Erode(imagenErocionada_circ, imagenErocionada, kernel);

            //return imagenErocionada;
            //****************nueva modificacion*******************************
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



        public static void CountVarillasCameraA(Mat frame)
        {
            try
            {
                Mat dst1 = new Mat();
                // Procesar la imagen aquí, por ejemplo, convertirla a escala de grises
                Referencias referenciaA = NReferencia.ObtenerReferenciaA(mainB.IdReceta)[0];

                int umbral_threshold = referenciaA.Umbral_Threshold;
                int kernel_hor = referenciaA.kernel_hor;
                int kernel_ver = referenciaA.Kernel__ver;
                int kernel_circ_hor = referenciaA.Kerne_cir_chor;
                int kernel_circ_ver = referenciaA.kernel_circ_ver;

                int Save_Img = Convert.ToInt32(DParametro.ObtenerParametro("GuardarImagenA"));

                Mat dst2 = new Mat();
                Mat frameRestado = new Mat();

              



                Cv2.CvtColor(frame, dst2, ColorConversionCodes.RGB2GRAY); //frame es la imagen con los 3 canales RGB


                //Mat roiImg = dst1;

                //Se agrega para quitar el ruido de fondo (jairo 28/06/2023)
                Cv2.Subtract(dst2, fondo, dst2);

                // Procesar la imagen aquí, por ejemplo, voltearla (efecto espejo) y  convertirla a escala de grises
                Cv2.Flip(dst2, dst1, FlipMode.Y);

                Mat imageFloat = new Mat();
                //se convierte la imagen a formato de 32 para poder aplicar el pow
                dst1.ConvertTo(imageFloat, MatType.CV_32F);

                ////Se aplica el pow
                //if (mainB.IdReceta != 9)
                //{
                //    Cv2.Pow(imageFloat, 1.42, dst1);
                //}
                //if (mainB.IdReceta == 2)
                //{
                //    Cv2.Pow(imageFloat, 1.5, dst1);
                //}

                Mat roiSegmentado = new Mat();
                //dst1.ConvertTo(dst1, MatType.CV_8UC1);// despues del pow es necesario volver la imagen nuevamente a formato de 8 con un solo canal

                if (mainB.IdReceta != 9 || mainB.IdReceta != 6)
                {
                    Cv2.Pow(imageFloat, 1.3, dst1);
                }
                if (mainB.IdReceta == 9 || mainB.IdReceta == 6)
                {
                    Cv2.Pow(imageFloat, 1.45, dst1);
                }
                if (mainB.IdReceta == 2)//habia un == se le aplica un pow de 1.4 para todas las referencias desde media hacia abajo
                {
                    Cv2.Pow(imageFloat, 1.5, dst1);
                }
                dst1.ConvertTo(dst1, MatType.CV_8UC1);

                //Mat mask = new Mat();
                //Cv2.Threshold(dst1, segmentacionRGB2, 130, 255, ThresholdTypes.Binary);
                Cv2.Threshold(dst1, roiSegmentado, umbral_threshold, 255, ThresholdTypes.Binary);

                if (mainB.IdReceta >= 1 && mainB.IdReceta < 9 && mainB.IdReceta != 5)
                {
                    Blocksize = 97;
                    Double = 0;
                }
                if (mainB.IdReceta == 5 || mainB.IdReceta >= 9)
                {
                    Blocksize = 127;
                    Double = 2;
                }
                if (mainB.IdReceta == 6)
                {
                    Blocksize = 127;
                    Double = 0;
                }
                //Cambios 1 1/8 jairo (30 de junio)
                if (mainB.IdReceta == 11 || mainB.IdReceta == 12 || mainB.IdReceta == 8)
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
                result_morph = PhiltroMorphologyCameraA(roiSegmentado, kernel_hor, kernel_ver, kernel_circ_hor, kernel_circ_ver);

                //dst1 = CameraObjectDetectionB(roiSegmentado, result_morph);

                dst1 = CameraObjectDetectionA(dst1, result_morph);


                if (Save_Img == 1)
                {
                    cont++;
                    Cv2.ImWrite("C:\\Users\\intecol\\Documents\\Despliegue\\ImagenesA\\image_" + Convert.ToString(cont) + ".jpg", frame);
                    Cv2.ImWrite("C:\\Users\\intecol\\Documents\\Despliegue\\ImagenesA\\image_" + Convert.ToString(cont) + "_procesada.jpg", result_morph);

                    if (cont >= 100)
                    {
                        //string parametro, string tipo, string valor)
                        DParametro.ActualizarParametro("GuardarImagenA", "int", "0");
                        cont = 0;
                    }

                }


                dst1.Dispose();



                GC.Collect();
            }
            catch (Exception ex) 
            {
            
            }
        }

    }
}
