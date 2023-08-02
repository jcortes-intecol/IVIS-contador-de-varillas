using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TRACKING.Data;
using TRACKING.Entity;
using TRACKING.Negocio;

namespace prueba
{
    internal class tracking_cameraA
    {
        public static int id_cont = 0;
        public static int contador = 0;
        public static int posXanterior = 0;
        public static int posYanterior = 0;
        public static int posXActual = 0;
        public static int posYActual = 0;
        public static int contorAnterior = 0;
        public static bool PLCimg_morfologica_display;

        public static int x_umbral1 = 0;//adicion 
        public static int x_umbral2 = 0;//adicion 
        public static int lineaEliminar;

        public static int auxcontador1 = 0;//adicion 
        public static int auxcontador2 = 0;//adicion 
        public static int auxcontador3 = 0;//adicion 

        //public static Dictionary<int, (int, int, int)> objetos = new Dictionary<int, (int, int, int)>();
        public static Dictionary<int, (int, int, int, int, int)> objetos = new Dictionary<int, (int, int, int, int, int)>();//se aciono
        //public static List<int> objetosAEliminar = new List<int>(objetos.Keys);
        //public static Dictionary<int, Tuple<int, int, int>> objetosNuevos = new Dictionary<int, Tuple<int, int, int>>();
        public static Mat TrackerObjecCameraA(Mat image_rgb, Mat image_binarizada_rgb, Point[][] Contornos, HierarchyIndex[] Indexes)
        {

            Referencias referenciaA = NReferencia.ObtenerReferenciaA(mainB.IdReceta)[0];
            int dist_umbral_below = referenciaA.distUmbralBelow;
            int dist_umbral_above = referenciaA.distUmbralAbove;
            double porcentaje_umbral_X = referenciaA.porcentaje_umbral;
            int Area_umbral = NReceta.ObtenerReceta(mainB.IdReceta)[0].Area;
            int width_umbral = referenciaA.width_umbral;
            int height_humbral = referenciaA.height_umbral;

            int img_morfologica_display = Convert.ToInt32(DParametro.ObtenerParametro("DebugLiveCameraA"));

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


            var sortedContours1 = Contornos.OrderBy(contour => Cv2.BoundingRect(contour).Y);
            var sortedContours = Contornos.OrderBy(sortedContours1 => Cv2.BoundingRect(sortedContours1).X);

            var numcontornos = Contornos.Length;
            var restacontor = contorAnterior - numcontornos;

            if (objetos.Count() != 0)
            {
                //objetos = objetos.OrderByDescending(kpv => kpv.Value.Item1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);// para A y B
                objetos = objetos.OrderBy(kpv => kpv.Value.Item1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                var primerElem = objetos.First();
                if (primerElem.Value.Item1 <= lineaEliminar)
                {

                    objetos.Remove(primerElem.Key);
                   
                }
             
                objetos = objetos.OrderByDescending(kpv => kpv.Value.Item1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            }

            //if (restacontor > 0 && objetos.Count() != 0)
            //{

            //    for (int i = 0; i < restacontor; i++)
            //    {
            //        objetos = objetos.OrderByDescending(kpv => kpv.Value.Item1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);// para A y B

            //        var ultimoelemento = objetos.Keys.Last();//para eliminar A 
            //        objetos.Remove(ultimoelemento);

            //    }


            //}
            //contorAnterior = numcontornos;


            var objetosAEliminar = new List<int>(objetos.Keys);
            //var objetosNuevos = new Dictionary<int, (int, int, int)>();
            var objetosNuevos = new Dictionary<int, (int, int, int, int, int)>();//se adiciono
            var mismoobjeto = false;
            int x_umbral = (int)(image_rgb.Width * porcentaje_umbral_X);
            x_umbral1 = (int)(image_rgb.Width * 0.4);//adicion 
            x_umbral2 = (int)(image_rgb.Width * 0.6);//adicion 
            lineaEliminar = (int)(image_rgb.Width * 0.025);

            Cv2.Line(image_binarizada_rgb, lineaEliminar, 2, lineaEliminar, image_rgb.Height - 1, Scalar.Blue);

            Cv2.Line(image_rgb, x_umbral1, 2, x_umbral1, image_rgb.Height - 1, Scalar.Blue);//se adiciono
            Cv2.Line(image_rgb, x_umbral, 2, x_umbral, image_rgb.Height - 1, Scalar.Blue);
            Cv2.Line(image_rgb, x_umbral2, 2, x_umbral2, image_rgb.Height - 1, Scalar.Blue);//se adiciono
            Cv2.PutText(image_rgb, Convert.ToString(contador), new Point(x_umbral + 30, x_umbral / 2), HersheyFonts.Italic, 1.2, Scalar.White);
            Cv2.PutText(image_rgb, Convert.ToString(auxcontador1), new Point(x_umbral1 + 10,150 / 2), HersheyFonts.Italic, 1.2, Scalar.White);
            Cv2.PutText(image_rgb, Convert.ToString(auxcontador3), new Point(x_umbral2 + 10,150 / 2), HersheyFonts.Italic, 1.2, Scalar.White);

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
                        if (img_morfologica_display == 1)
                        {
                            Cv2.Circle(image_binarizada_rgb, new Point(posXActual, posYActual), 2, new Scalar(0, 0, 255));
                            Cv2.PutText(image_binarizada_rgb, $"({posXActual})({posYActual})", new Point(posXActual, posYActual + image_rgb.Height * 0.11), HersheyFonts.HersheyScriptComplex, 0.7, new Scalar(255, 255, 255));
                            Cv2.PutText(image_binarizada_rgb, $"A: ({Area})", new Point(posXActual, posYActual + image_rgb.Height * 0.2), HersheyFonts.HersheyScriptComplex, 0.7, new Scalar(255, 255, 255));
                            Cv2.PutText(image_binarizada_rgb, $"w, h: ({w}),({h})", new Point(posXActual, posYActual + image_rgb.Height * 0.3), HersheyFonts.HersheyScriptComplex, 0.7, new Scalar(255, 255, 255));

                        }
                     
                        var done = 0;
                        var done1 = 0;
                        var done2 = 0;
                        mismoobjeto = false;

                        objetos = objetos.OrderByDescending(kpv => kpv.Value.Item1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);//para recorrer A
                        foreach (var data in objetos)
                        {

                            var value = data.Value;
                            var idObjeto = data.Key;
                            posXanterior = value.Item1;
                            posYanterior = value.Item2;
                          
                            done = value.Item3;
                            done1 = value.Item4;
                            done2 = value.Item5;

                            var dis = Math.Sqrt(Math.Pow(posXActual - posXanterior, 2) + Math.Pow(posYActual - posYanterior, 2));

                            dis = Convert.ToInt16(dis);


                            if (dis >= dist_umbral_below && dis < dist_umbral_above /*&& !lista_aux.Contains(idObjeto)*/ && !mismoobjeto)
                            {
                                if (img_morfologica_display == 1)
                                {
                                    Cv2.PutText(image_binarizada_rgb, Convert.ToString(dis), new Point((posXActual + posXanterior) / 2, (posYActual + posYanterior) / 2 - 10), HersheyFonts.Italic, 1, new Scalar(255, 0, 0));
                                    Cv2.Line(image_binarizada_rgb, posXActual, posYActual, posXanterior, posYanterior, new Scalar(255, 0, 0));
 
                                }
                                Cv2.DrawContours(image_rgb, sortedContours, contourIndex, Scalar.DarkGreen, 2);
                                //Cv2.Rectangle(image_rgb, new Point(x, y), new Point(x + w, y + h), Scalar.Red, 2);

                                //if (done == 0)
                                //{

                                //    done = Count(posXanterior, posXActual, x_umbral, w, h, width_umbral, height_humbral);


                                //}
                                //objetos[idObjeto] = (posXActual, posYActual, done);
                                //objetosNuevos[idObjeto] = (posXActual, posYActual, done);

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

                                objetos[idObjeto] = (posXActual, posYActual, done, done1, done2);//se adiciono
                                objetosNuevos[idObjeto] = (posXActual, posYActual, done, done1, done2);//se adiciono
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

                        }

                        //if (!mismoobjeto)
                        //{
                        //    objetosNuevos.Add(id_cont, (posXActual, posYActual, 0));
                        //    id_cont++;

                        //}
                        if (!mismoobjeto)
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
            Cv2.Add(image_rgb, mainB.fondoNuevoA, resp);
            //Cv2.Add(image_binarizada_rgb, mainB.fondoNuevoA, image_binarizada_rgb);

            if (PLCimg_morfologica_display)
            {
                Cv2.ImShow("Imagen final A", resp);
            }

            if (img_morfologica_display == 1)
            {
                Cv2.ImShow("Imagen final A", resp);
                Cv2.ImShow("imagen segmentada Cam A", image_binarizada_rgb);
            }
            if (PLCimg_morfologica_display || img_morfologica_display == 1)
            {
                Cv2.WaitKey(1);

            }
            return image_rgb;
        }
        //public static int Count(double CoordenadasXAnteriores, double CoordenadasXActuales, int x_umbral, int width, int height, int widht_umbral, int height_umbral)
        //{
        //    //double PosicionLinea = 250;
        //    int NewDone;


        //    if (CoordenadasXActuales < x_umbral && CoordenadasXAnteriores >= x_umbral)
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
            if (CoordenadasXActuales < x_umbral && CoordenadasXAnteriores >= x_umbral)//para A la condicion es asi 
                                                                                      //if (CoordenadasXActuales >= x_umbral && CoordenadasXAnteriores < x_umbral)
            {

                NewDone1 = 1;

                contaFuncion++;
                //auxcontador/*2++;*/
                //contador++;
                if (width >= widht_umbral || height > height_umbral)
                {
                    //auxcontador2++;
                    contaFuncion++;
                    //contador++;
                }

            }
            else
            {
                NewDone1 = 0;

            }





            return (NewDone1, contaFuncion);
        }

    }



}





