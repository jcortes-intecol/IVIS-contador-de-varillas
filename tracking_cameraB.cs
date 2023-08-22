using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TRACKING.Data;
using TRACKING.Entity;
using TRACKING.Negocio;

using System.Diagnostics;

namespace prueba
{

    internal class tracking_cameraB
    {
        public static int id_cont = 0;
        public static int contador = 0;
        public static int posXanterior = 0;
        public static int posYanterior = 0;
        public static int posXActual = 0;
        public static int posYActual = 0;
        public static int contorAnterior = 0;
        public static bool PLCimg_morfologica_display;

        public static int x_umbral1 = 0;//se adiciono
        public static int x_umbral2 = 0;//se adiciono
        public static int lineaEliminar;

        public static int auxcontador1 = 0;//se adiciono
        public static int auxcontador2 = 0;//se adiciono
        public static int auxcontador3 = 0;//se adiciono
        public static bool pararconteo;

        ////Lista donde se van a guardar las coordenadas.
        //public static List<(int x, int y)> coordenadas = new List<(int, int)>();

        //public static Dictionary<int, (int, int, int)> objetos = new Dictionary<int, (int, int, int)>();
        public static Dictionary<int, (int, int, int, int, int)> objetos = new Dictionary<int, (int, int, int, int, int)>();//se adiciono
                                                                                                                            //public static List<int> objetosAEliminar = new List<int>(objetos.Keys);
                                                                                                                            //public static Dictionary<int, Tuple<int, int, int>> objetosNuevos = new Dictionary<int, Tuple<int, int, int>>();




        public static Mat TrackerObjecCameraB(Mat image_rgb, Mat image_binarizada_rgb, Point[][] Contornos, HierarchyIndex[] Indexes)
        {

            Stopwatch temporizador = new Stopwatch();
            temporizador.Start();
            Referencias referenciaB = NReferencia.ObtenerReferenciaB(mainB.IdReceta)[0];
            int dist_umbral_below = referenciaB.distUmbralBelow;
            int dist_umbral_above = referenciaB.distUmbralAbove;
            double porcentaje_umbral_X = referenciaB.porcentaje_umbral;
            int Area_umbral = NReceta.ObtenerReceta(mainB.IdReceta)[0].SecondArea;
            int width_umbral = referenciaB.width_umbral;
            int height_humbral = referenciaB.height_umbral;
            int img_morfologica_display = Convert.ToInt32(DParametro.ObtenerParametro("DebugLiveCameraB"));

            //int img_morfologica_display = NParametros.DebugLiveCameraB;
            //  bool img_morfologica_display = true;
            var lista_aux = new List<int>();
            if (Contornos.Length == 0)
            {
                objetos.Clear();
                id_cont = 0;
                //************************************ resultado conteo*******************

                int[] valores = { auxcontador1, auxcontador2, auxcontador3 };
                Array.Sort(valores);

                // Comparar los valores para encontrar el que más se repite
                int valorMasFrecuente;
                if (mainB.IdReceta < 10)
                {

                    if (valores[0] == valores[1])
                    {
                        valorMasFrecuente = valores[0];
                    }
                    else if (valores[1] == valores[2])
                    {
                        valorMasFrecuente = valores[1];
                    }
                    else if (valores[0] == valores[2])
                    {
                        valorMasFrecuente = valores[0];
                    }
                    else
                    {
                        valorMasFrecuente = valores[1];
                    }



                    contador = valorMasFrecuente;
                    auxcontador1 = valorMasFrecuente;
                    auxcontador2 = valorMasFrecuente;
                    auxcontador3 = valorMasFrecuente;
                }
                if (mainB.IdReceta >= 10)
                {
                    auxcontador2 = 0;
                    auxcontador3 = 0;
                }

            }

            var sortedContours1 = Contornos.OrderByDescending(contour => Cv2.BoundingRect(contour).Y);
            var sortedContours = Contornos.OrderByDescending(sortedContours1 => Cv2.BoundingRect(sortedContours1).X);
            //var sortedContours = Contornos.OrderByDescending(contour => Cv2.BoundingRect(contour).X); //para camara B


            //if (objetos.Count() != 0)
            //{
            //    objetos = objetos.OrderByDescending(kpv => kpv.Value.Item1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);// para A y B
            //    var primerElem = objetos.First();
            //    if (primerElem.Value.Item1 >= lineaEliminar)
            //    {
            //        objetos.Remove(primerElem.Key);

            //    }

            //}


            //var numcontornos = Contornos.Length;
            //var restacontor = contorAnterior - numcontornos;


            //if (restacontor > 0 && objetos.Count()!=0)
            //{

            //    for (int i = 0; i < restacontor; i++)
            //    {
            //        objetos = objetos.OrderByDescending(kpv => kpv.Value.Item1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);// para A y B
            //        var primerElem = objetos.First();//para B eliminar el primero
            //        objetos.Remove(primerElem.Key);


            //    }


            //}
            //contorAnterior = numcontornos;


            var objetosAEliminar = new List<int>(objetos.Keys);
            //var objetosNuevos = new Dictionary<int, (int, int, int)>();//se adiciono
            var objetosNuevos = new Dictionary<int, (int, int, int, int, int)>();
            var mismoobjeto = false;
            int x_umbral = (int)(image_rgb.Width * porcentaje_umbral_X);
            x_umbral1 = (int)(image_rgb.Width * 0.5);//0.4 habia
            x_umbral2 = (int)(image_rgb.Width * 0.7);//
            lineaEliminar = (int)(image_rgb.Width * 0.97);



            Cv2.Line(image_rgb, x_umbral, 2, x_umbral, image_rgb.Height - 1, Scalar.Blue);
            Cv2.Line(image_rgb, x_umbral1, 2, x_umbral1, image_rgb.Height - 1, Scalar.Blue);
            Cv2.Line(image_rgb, x_umbral2, 2, x_umbral2, image_rgb.Height - 1, Scalar.Blue);
            Cv2.Line(image_binarizada_rgb, lineaEliminar, 2, lineaEliminar, image_rgb.Height - 1, Scalar.Blue);
            Cv2.PutText(image_rgb, Convert.ToString(contador), new Point(x_umbral + 30, x_umbral / 2), HersheyFonts.Italic, 1.2, Scalar.White);
            Cv2.PutText(image_rgb, Convert.ToString(auxcontador1), new Point(x_umbral1 + 10, 150 / 2), HersheyFonts.Italic, 1.2, Scalar.White);
            Cv2.PutText(image_rgb, Convert.ToString(auxcontador3), new Point(x_umbral2 + 10, 150 / 2), HersheyFonts.Italic, 1.2, Scalar.White);
            Cv2.PutText(image_rgb, Convert.ToString(objetos.Count()), new Point(10, 500), HersheyFonts.Italic, 1.2, Scalar.White);
            //mainB.framesProcesados
            int counter = 0;
            int count_obj = 0;
            if (Indexes.Length > 0)
            {


                for (var contourIndex = 0; contourIndex < sortedContours.Count(); contourIndex++)//para recorer B y A
                {
                    var contour = sortedContours.ElementAt(contourIndex);


                    //var contour = Contornos[contourIndex];
                    var Area = Cv2.ContourArea(contour);
                    Mat Rgb_ig = image_rgb;

                    if (Area > Area_umbral)
                    {
                        //Cv2.DrawContours(src, Contornos, -1, 255, 1);
                        var boundingRect = Cv2.BoundingRect(contour);
                        var x = boundingRect.X;
                        var y = boundingRect.Y;
                        var w = boundingRect.Width;
                        var h = boundingRect.Height;
                        //posXActual = x + (w / 2);
                        //posYActual = y + (h / 2);
                        var moments = Cv2.Moments(contour);
                        posXActual = (int)(moments.M10 / moments.M00);
                        posYActual = (int)(moments.M01 / moments.M00);

                        ////se guardan las posiciones de cada varillas.
                        //coordenadas.Add((posXActual, posYActual));

                        if (img_morfologica_display == 1)
                        {
                            Cv2.Circle(image_binarizada_rgb, new Point(posXActual, posYActual), 2, new Scalar(0, 0, 255));
                            Cv2.PutText(image_binarizada_rgb, $"pos({posXActual})({posYActual})", new Point(posXActual, posYActual + image_rgb.Height * 0.11), HersheyFonts.HersheyScriptComplex, 0.5, new Scalar(255, 255, 0));
                            //Cv2.PutText(image_binarizada_rgb, $"A: ({Area})", new Point(posXActual, posYActual + image_rgb.Height * 0.2), HersheyFonts.HersheyScriptComplex, 0.5, new Scalar(255, 255, 0));


                            Cv2.PutText(image_binarizada_rgb, $"A: ({Area})", new Point(posXActual, posYActual + image_rgb.Height * 0.2), HersheyFonts.HersheyScriptComplex, 0.5, Scalar.LimeGreen);

                            Cv2.PutText(image_binarizada_rgb, $"w, h: ({w}),({h})", new Point(posXActual, posYActual + image_rgb.Height * 0.3), HersheyFonts.HersheyScriptComplex, 0.5, new Scalar(255, 255, 0));
                        }

                        var done = 0;
                        var done1 = 0;//se adiciono
                        var done2 = 0;//se adiciono
                        mismoobjeto = false;

                        //objetos = objetos.OrderBy(kpv => kpv.Value.Item1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);//para recorrer B

                        var objetos1 = objetos.OrderByDescending(kpv => kpv.Value.Item2).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);//para recorrer B se adiciono
                        objetos = objetos1.OrderByDescending(kpv => kpv.Value.Item1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);//para recorrer se adiciono
                        int objetoevaluar = 0;

                        foreach (var data in objetos)
                        {

                            var value = data.Value;
                            var idObjeto = data.Key;
                            posXanterior = value.Item1;
                            posYanterior = value.Item2;
                            done = value.Item3;//se adiciono
                            done1 = value.Item4;//se adiciono
                            done2 = value.Item5;//se adiciono
                            var desplazamientoX = posXActual - posXanterior;

                            var dis = Math.Sqrt(Math.Pow(posXActual - posXanterior, 2) + Math.Pow(posYActual - posYanterior, 2));

                            dis = Convert.ToInt16(dis);


                            if (dis >= dist_umbral_below && dis < dist_umbral_above && !lista_aux.Contains(idObjeto) && !mismoobjeto /*&& contourIndex == objetoevaluar*/ && desplazamientoX >= -10)
                            {
                                if (img_morfologica_display == 1)
                                {
                                    Cv2.PutText(image_binarizada_rgb, Convert.ToString(dis), new Point((posXActual + posXanterior) / 2, (posYActual + posYanterior) / 2 - count_obj * 5), HersheyFonts.Italic, 1.2, new Scalar(255, 0, 0));
                                    Cv2.Line(image_binarizada_rgb, posXActual, posYActual, posXanterior, posYanterior, new Scalar(255, 0, 0));
                                    //Cv2.Rectangle(image_binarizada_rgb, new Point(x, y), new Point(x + w, y + h), Scalar.White, 2);
                                }

                                //Cv2.Rectangle(image_rgb, new Point(x, y), new Point(x + w, y + h), Scalar.Red, 2);
                                Cv2.DrawContours(image_rgb, sortedContours, contourIndex, Scalar.DarkGreen, 2);
                                if (!pararconteo) //***********************************
                                {

                                    if (done == 0) //linea a 0.3
                                    {

                                        done = Count(posXanterior, posXActual, x_umbral1, w, h, width_umbral, height_humbral, auxcontador1).Item1;
                                        auxcontador1 = Count(posXanterior, posXActual, x_umbral1, w, h, width_umbral, height_humbral, auxcontador1).Item2;
                                    }

                                    if (done1 == 0)//Linea a 0.5
                                    {

                                        done1 = Count(posXanterior, posXActual, x_umbral, w, h, width_umbral, height_humbral, auxcontador2).Item1;
                                        auxcontador2 = Count(posXanterior, posXActual, x_umbral, w, h, width_umbral, height_humbral, auxcontador2).Item2;
                                        contador = auxcontador2;
                                    }

                                    if (done2 == 0)//Linea a 0.7
                                    {

                                        done2 = Count(posXanterior, posXActual, x_umbral2, w, h, width_umbral, height_humbral, auxcontador3).Item1;
                                        auxcontador3 = Count(posXanterior, posXActual, x_umbral2, w, h, width_umbral, height_humbral, auxcontador3).Item2;

                                    }

                                    if (mainB.IdReceta >= 10)
                                    {
                                        contador = auxcontador1;
                                    }
                                }
                                else //*****************************
                                {
                                    Cv2.PutText(image_rgb, "Conteo pausado", new Point(50, 300), HersheyFonts.Italic, 2, new Scalar(255, 0, 0), 5);
                                }
                                objetos[idObjeto] = (posXActual, posYActual, done, done1, done2);
                                objetosNuevos[idObjeto] = (posXActual, posYActual, done, done1, done2);
                                mismoobjeto = true;
                                lista_aux.Add(idObjeto);
                                break;


                            }
                            else
                            {
                                if (img_morfologica_display == 1)
                                {
                                    Cv2.PutText(image_binarizada_rgb, Convert.ToString(dis), new Point((posXActual + posXanterior) / 2, (posYActual + posYanterior) / 2 - 10), HersheyFonts.Italic, 0.5, new Scalar(0, 0, 255));
                                    Cv2.Line(image_binarizada_rgb, posXActual, posYActual, posXanterior, posYanterior, new Scalar(0, 0, 255));
                                }

                            }
                            objetoevaluar++;//**ultima adicion 

                        }

                        //if (!mismoobjeto)
                        //{
                        //    objetosNuevos.Add(id_cont, (posXActual, posYActual, 0));
                        //    id_cont++;

                        //}

                        if (!mismoobjeto )
                        {
                            objetosNuevos.Add(id_cont, (posXActual, posYActual, 0, 0, 0));
                            id_cont++;

                        }
                    }
                }
            }
            foreach (var idObjeto in objetosAEliminar)
            {
                objetos.Remove(idObjeto);
            }

            //var valoresUnicos = new List<object>();
            //foreach (var (idObjeto, value) in objetosNuevos)
            //{
            //    var objetoAnonimo = new { Item1 = value.Item1, Item2 = value.Item2, Item3 = value.Item3 };
            //    if (!valoresUnicos.Any(o => o.Equals(objetoAnonimo)))
            //    {
            //        var tupla = (objetoAnonimo.Item1, objetoAnonimo.Item2, objetoAnonimo.Item3);
            //        objetos[idObjeto] = tupla;
            //        valoresUnicos.Add(objetoAnonimo);
            //    }
            //}
            var valoresUnicos = new List<object>();
            foreach (var (idObjeto, value) in objetosNuevos)
            {
                var objetoAnonimo = new { Item1 = value.Item1, Item2 = value.Item2, Item3 = value.Item3, Item4 = value.Item4, Item5 = value.Item5 };
                if (!valoresUnicos.Any(o => o.Equals(objetoAnonimo)))
                {
                    var tupla = (objetoAnonimo.Item1, objetoAnonimo.Item2, objetoAnonimo.Item3, objetoAnonimo.Item4, objetoAnonimo.Item5);
                    objetos[idObjeto] = tupla;
                    valoresUnicos.Add(objetoAnonimo);
                }
            }


            Mat resp = new Mat();
            //Cv2.Add(image_rgb, mainB.fondoNuevo, resp);
            //Cv2.Add(image_rgb, mainB.fondoNuevo, image_rgb);
            //Cv2.Add(image_binarizada_rgb, mainB.fondoNuevo, image_binarizada_rgb);

            /*if ( PLCimg_morfologica_display)
            {
                Cv2.ImShow("Imagen final B", image_rgb);
            }*/

            if (objetos.Count() != 0)
            {
                objetos = objetos.OrderByDescending(kpv => kpv.Value.Item1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);// para A y B
                var primerElem = objetos.First();
                if (primerElem.Value.Item1 >= lineaEliminar)
                {
                    objetos.Remove(primerElem.Key);

                }

            }

            if (img_morfologica_display == 1 || PLCimg_morfologica_display)
            {
                Cv2.ImShow("Imagen final B", image_rgb);
                Cv2.ImShow("imagen segmentada Cam B", image_binarizada_rgb);
            }
            if (PLCimg_morfologica_display || img_morfologica_display == 1)
            {
                Cv2.WaitKey(1);

            }
            //temporizador.Stop();
            //TimeSpan tiempoTranscurrido = temporizador.Elapsed;
            //double milisegundos = tiempoTranscurrido.TotalMilliseconds;
            //Console.WriteLine(milisegundos.ToString());
            return image_rgb;
        }
        //public static int Count(double CoordenadasXAnteriores, double CoordenadasXActuales, int x_umbral, int width, int height, int widht_umbral, int height_umbral)
        //{
        //    int NewDone;


        //    if (CoordenadasXActuales >= x_umbral && CoordenadasXAnteriores < x_umbral)
        //    {

        //        NewDone = 1;

        //        contador++;
        //        if (width >= widht_umbral || height > height_umbral)
        //        {
        //            contador++;
        //        }
        //    }

        //    else
        //    {
        //        NewDone = 0;

        //    }


        //    return (NewDone);
        //}

        public static (int, int) Count(double CoordenadasXAnteriores, double CoordenadasXActuales, int x_umbral, int width, int height, int widht_umbral, int height_umbral, int contaFuncion)
        {

            int NewDone1;

            ////*******************************segunda linea ***********************************************
            if (CoordenadasXActuales >= x_umbral && CoordenadasXAnteriores < x_umbral)
            {
                NewDone1 = 1;

                contaFuncion++;

                if (width >= widht_umbral || height > height_umbral)
                {
                    contaFuncion++;
                }
            }
            else
            {
                NewDone1 = 0;

            }
            return (NewDone1, contaFuncion);
        }
        //===============================================================================================================================================
        public static (int, int) CountArea(double CoordenadasXAnteriores, double CoordenadasXActuales, int x_umbral, int width, int height, int widht_umbral, int height_umbral, int contaFuncion, double area)
        {

            int NewDone1;

            ////*******************************segunda linea ***********************************************
            if (CoordenadasXActuales >= x_umbral && CoordenadasXAnteriores < x_umbral)
            {

                NewDone1 = 1;

                contaFuncion++;
                //if (width * height >= widht_umbral * height_umbral)
                //{
                //    contaFuncion++;
                //}
                if (area >= 2300)
                {
                    contaFuncion++;
                }

            }
            else
            {
                NewDone1 = 0;

            }

            return (NewDone1, contaFuncion);
        }
        //===============================================================================================================================================
    }



}






