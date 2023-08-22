using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharp7;
using prueba;
using System.Threading;
using TRACKING.Negocio;
using SpinnakerNET.GenApi;
using Camara.Modbus.Servicio.control;
using TRACKING;
using System.Collections;
using System.Diagnostics;

namespace prueba
{
    public class Class2
    {
        public static S7Client Client;
        public static bool BanderaConection;
        public static bool BanderaReConection;
        public static int Aux = 0;
        public static int AuxB = 0;
        public static int reinicarContA = 0;
        public static int reinicarContB = 0;

        public static void ConectionPLC()
        {
            Stopwatch temporizador = new Stopwatch();
            BanderaConection = true;
            while (BanderaConection)
            {
                
                try
                {
                    
                    Client = new S7Client();

                    int Rack = NParametros.RackPLC;
                    int Slot = NParametros.slotPLC;
                    //string IPPlc = "172.16.50.229";
                    string IPPlc = NParametros.IPPlc;
                    int stateConnection = Client.ConnectTo(IPPlc, Rack, Slot);

                    if (stateConnection == 0)
                    {
                        BanderaReConection = true;
                        BanderaConection = false;
                        while (BanderaReConection)
                        {


                            //temporizador.Start();
                            //        *************************declaracion de cariables para lectrura de DB principal********
                            int db = 300; //direccion db a leer 
                                          // int posicion = 2; // la posicion se refiere a la direccion en la db o el offset en caso de Tia portal 
                            int dbSize = 12; // el tamaño es el igual al numero de bytes que tiene la db en todos los campos en total
                                            // tipo: int = 2 byte, word =2 byte, Dword= 8 byte, bool = 2 byte, si en la Db se repite el numero de 
                            var buffer = new byte[dbSize]; // es el tamaño total de byte en la DB 
                            var buffer303 = new byte[8];
                            //             ******************* Lectura Db principal *****************
                            int condition = Client.DBRead(db, 0, buffer.Length, buffer); //db numero del espacio de memoria
                            int db303 = Client.DBRead(303, 0, buffer303.Length, buffer303);// leo los valores de la db303

                            //****************************estado de la conexion del plc ***********
                            bool respuesta = S7.GetBitAt(buffer, 1, 7);
                            S7.SetBitAt(buffer303, 1, 0, respuesta);
                            int watchdog = Client.DBWrite(303, 0, buffer303.Length, buffer303);
                            //******************************************************************************

                            if (condition != 0 || watchdog != 0 || db303 != 0)
                            {
                                Client.Disconnect();
                                BanderaReConection = false;

                                Console.WriteLine("error " + condition.ToString("x"));
                                break;
                            }
                            NParametros.ActualizarParametro("StatePLC", "Int", Convert.ToString(condition));
                            bool IniciarPararMedA = S7.GetBitAt(buffer, 0, 0);
                           

                            if (IniciarPararMedA)
                            {
                                ControlCamaraA.stopmed = false;
                             
                                S7.SetBitAt(buffer303, 0, 2, ControlCamaraA.Statecont);
                                int contB = Client.DBWrite(303, 0, buffer303.Length, buffer303);
                                // Console.WriteLine("se inicio medicion en A");
                                //if (Aux == 0) actualizar referencia*************
                                //{
                                mainB.mediconA = IniciarPararMedA;
                                int Reference = S7.GetIntAt(buffer, 4);

                                mainB.IdReceta = Reference;
                                NParametros.ActualizarParametro("Referencia", "Int", Convert.ToString(Reference));
                                //    Aux++;
                                //}*********************************************************
                                tracking_cameraA.PLCimg_morfologica_display = S7.GetBitAt(buffer, 0, 1);


                                bool SeparationDoneA = S7.GetBitAt(buffer, 0, 2);// separacion echa 
                                if (SeparationDoneA && reinicarContA == 0)
                                {

                                    NGuardarContador.GuardarContador(mainB.IdReceta, Convert.ToString(tracking_cameraA.contador));
                                    tracking_cameraA.contador = 0;
                                    tracking_cameraA.auxcontador1 = 0;
                                    tracking_cameraA.auxcontador2 = 0;
                                    tracking_cameraA.auxcontador3 = 0;
                                    reinicarContA++;
                                }
                                if (SeparationDoneA == false) { reinicarContA = 0; }

                            }
                            if (IniciarPararMedA == false)
                            {
                                //Console.WriteLine("Finalizó medicion A");
                                ControlCamaraA.stopmed = true;
                                
                                S7.SetBitAt(buffer303, 0, 2, ControlCamaraA.Statecont);
                                int contB = Client.DBWrite(303, 0, buffer303.Length, buffer303);
                                Aux = 0;
                            }

                            bool IniciarPararMedB = S7.GetBitAt(buffer, 0, 5);
                            if (IniciarPararMedB)
                            {
                                ControlCameraB.stopmed = false;//parar el ciclo de captura de imagenes
                                //************** señal al plc de si esta contando *****
                                
                                S7.SetBitAt(buffer303, 0, 6, ControlCameraB.Statecont);
                               // Console.WriteLine("estado conexionB "+Convert.ToString(ControlCameraB.Statecont));
                                int contB = Client.DBWrite(303, 0, buffer303.Length, buffer303);
                              //*******finaliza señal al plc*************************************************

                                mainB.medicionB = IniciarPararMedB;
                                int Reference = S7.GetIntAt(buffer, 4);

                                mainB.IdReceta = Reference;
                                NParametros.ActualizarParametro("Referencia", "Int", Convert.ToString(Reference));
                                //    AuxB++;
                                //}************************************
                                tracking_cameraB.PLCimg_morfologica_display = S7.GetBitAt(buffer, 0, 6);
                                tracking_cameraB.pararconteo = S7.GetBitAt(buffer, 1, 5);
                                bool SeparationDoneB = S7.GetBitAt(buffer, 0, 7);// separacion echa 
                                if (SeparationDoneB && reinicarContB == 0)
                                {

                                    NGuardarContador.GuardarContadorB(mainB.IdReceta, Convert.ToString(tracking_cameraB.contador));
                                    tracking_cameraB.contador = 0;
                                    tracking_cameraB.auxcontador1 = 0;
                                    tracking_cameraB.auxcontador2 = 0;
                                    tracking_cameraB.auxcontador3 = 0;
                                    mainB.framesProcesados = 0;
                                    reinicarContB++;
                                }
                                if (SeparationDoneB == false) { reinicarContB = 0; }
                                ///<summary>
                                ///in the lines 132 to 144
                                ///Forces the count of the counter and write false into the plc
                                ///</summary>
                                //Console.WriteLine("A fuera");
                                bool forzarContadorB = S7.GetBitAt(buffer, 1, 0);
                                //Console.WriteLine(forzarContadorB);
                                if (forzarContadorB)
                                {
                                    //Console.WriteLine("entro");
                                    int valorAAsignarB = S7.GetIntAt(buffer, 10);
                                    //Console.WriteLine(valorAAsignarB);
                                    tracking_cameraB.contador = valorAAsignarB;
                                    tracking_cameraB.auxcontador1 = valorAAsignarB;
                                    tracking_cameraB.auxcontador2 = valorAAsignarB;
                                    tracking_cameraB.auxcontador3 = valorAAsignarB;
                                    //var bufferForzar = new byte[1];
                                    // bool value = false;
                                    S7.SetBitAt(buffer, 1, 0, false);
                                    int writeForzar = Client.DBWrite(300, 0, buffer.Length, buffer);
                                }
                            }
                            if (IniciarPararMedB == false)
                            {

                               // Console.WriteLine("Finalizó medicion B");
                                ControlCameraB.stopmed = true;
                                
                                S7.SetBitAt(buffer303, 0, 6, ControlCameraB.Statecont);
                                int contB = Client.DBWrite(303, 0, buffer303.Length, buffer303);
                                AuxB = 0;
                            }


                            //************************* escritura medicion al plc *********************************************
                           
                            S7.SetBitAt(buffer303, 0, 3, ControlCamaraA.StateCamera);
                           int StateA = Client.DBWrite(303, 0, buffer303.Length, buffer303); //escribe en el plc el stado de las camaras 
                           //var StateCameraB = new byte[1];
                           S7.SetBitAt(buffer303, 0, 7, ControlCameraB.StateCamera);
                           int StateB = Client.DBWrite(303, 0, buffer303.Length, buffer303);
                           // Console.WriteLine(" conexionB "+StateB + Convert.ToString(ControlCameraB.Statecont));



                            // Crear los buffers de datos
                            var buffer1 = new byte[2];
                            var buffer2 = new byte[2];
                            //tracking_cameraA.contador
                            // Escribir en el buffer1
                            S7.SetIntAt(buffer1, 0, Convert.ToInt16(tracking_cameraA.contador));////condador A
                            int Write1 = Client.DBWrite(303, 2, buffer1.Length, buffer1);
                            if (Write1 != 0)
                            {
                                BanderaReConection = false;
                                Client.Disconnect();
                                break;
                            }
                            //tracking_cameraB.contador
                            // Escribir en el buffer2
                            S7.SetIntAt(buffer2, 0, Convert.ToInt16(tracking_cameraB.contador));/// contador B
                            int Write2 = Client.DBWrite(303, 4, buffer2.Length, buffer2);
                            if (Write2 != 0)
                            {
                                BanderaReConection = false;
                                Client.Disconnect();
                                break;
                            }

                            Thread.Sleep(150);

                            //temporizador.Stop();

                            //// Obtiene el tiempo transcurrido como un TimeSpan
                            //TimeSpan tiempoTranscurrido = temporizador.Elapsed;

                            //// Convierte el tiempo transcurrido a milisegundos
                            //double milisegundos = tiempoTranscurrido.TotalMilliseconds;
                            //Console.WriteLine(milisegundos.ToString());
                            //temporizador.Restart();

                        }
                    }
                    else { NParametros.ActualizarParametro("StatePLC", "Int", Convert.ToString(stateConnection)); }
                    

                }
                catch
                {

  
                    int Rack = NParametros.RackPLC;
                    int Slot = NParametros.slotPLC;
                    string IPPlc = NParametros.IPPlc;
                    int stateConnection = Client.ConnectTo(IPPlc, Rack, Slot);
                    BanderaConection = true;
                    Console.WriteLine("no se conecto el plc");
                }
                
                // Agregar un tiempo de espera de 1 segundo (1000 milisegundos)
            }
        }
    }
}
