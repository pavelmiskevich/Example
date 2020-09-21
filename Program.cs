using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Vetis.Classes;
using Vetis.Classes.Service;
using Enum = Vetis.Classes.Service.Enum;
using System.Globalization;
using System.Threading;
using Vetis.VetisAPI;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.CompilerServices;
using CryptoClassLibrary;
using Error = Vetis.Classes.Error;
using Product = Vetis.Classes.Ware;

namespace Vetis
{
    class Program
    {
        private static int lastUpHour = 0;
        private static int lastUpHourSE = 0;
        private static int lastUpHourDN = 0;
        private static int lastUpHourRT = 0;
        private static int lastUpHourIn = 0;

        static void RegisterProductionOperationThread()
        {
            CultureInfo ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            System.Object lockThis = new System.Object();
            while (true)
            {
                try
                {
                    lock (lockThis)
                    {
                        Console.WriteLine("{0} Поток производства", DateTime.Now.ToString("F"));
                        FTP.GetFilesByRootPath(Const.FTPPathRegisterProductionOperationRequest, Enum.RequestTypes.registerProductionOperationRequest, -10);
                        System.Threading.Thread.Sleep(20000);                        
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("{0} Exception caught.", ex);
                    Logs.ErrorLog(ex.ToString());
                }
            }
        }

        static void ProductItemThread()
        {
            CultureInfo ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            
            System.Object lockThis = new System.Object();
            while (true)
            {
                try
                {
                    lock (lockThis)
                    {
                        if (DateTime.Now.Hour > 8 && DateTime.Now.Hour < 19 && DateTime.Now.Minute > 10 && DateTime.Now.Minute < 20)
                        {
                            FTP.GetFiles4ProductItems();
                            //Const.InitProductItems();
                        }
                        System.Threading.Thread.Sleep(600000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} Exception caught.", ex);
                    Logs.ErrorLog(ex.ToString());
                }
            }
        }

        static void ReturnThread()
        {
            CultureInfo ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            
            System.Object lockThis = new System.Object();
            while (true)
            {
                try
                {
                    lock (lockThis)
                    {
                        if (DateTime.Now.Hour > 8 && DateTime.Now.Hour < 19 && DateTime.Now.Minute > 20 && DateTime.Now.Minute < 30 && DateTime.Now.Hour != lastUpHourRT)
                        {
                            WorkMethods.ReturnReception();
                            FTP.GetFiles4Returns();
                            lastUpHourRT = DateTime.Now.Hour;
                        }
                        System.Threading.Thread.Sleep(80000);
                        //System.Threading.Thread.Sleep(600000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} Exception caught.", ex);
                    Logs.ErrorLog(ex.ToString());
                }
            }
        }

        static void IncomingThread()
        {
            CultureInfo ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            System.Object lockThis = new System.Object();
            while (true)
            {
                try
                {
                    lock (lockThis)
                    {
                        if (DateTime.Now.Hour != lastUpHourIn)
                        {
                            WorkMethods.IncomingReception();
                            FTP.GetFiles4Incomings(dayOffset: -1);
                            lastUpHourIn = DateTime.Now.Hour;
                        }
                        System.Threading.Thread.Sleep(90000);
                        //System.Threading.Thread.Sleep(600000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} Exception caught.", ex);
                    Logs.ErrorLog(ex.ToString());
                }
            }
        }

        static void ErrorThread()
        {
            CultureInfo ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            
            System.Object lockThis = new System.Object();
            while (true)
            {
                try
                {
                    lock (lockThis)
                    {
                        int pause = 600000;
                        if (DateTime.Now.Minute > 0 && DateTime.Now.Minute < 10)
                        {
                            FTP.GetFiles4Error(Const.FTPPathPrepareOutgoingConsignmentRequest, -1);
                            pause = 1200000;
                        }
                        System.Threading.Thread.Sleep(pause);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} Exception caught.", ex);
                    Logs.ErrorLog(ex.ToString());
                }
            }
        }

        static void ReportThread()
        {
            CultureInfo ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            //TODO: генерация отчетов
            System.Object lockThis = new System.Object();
            while (true)
            {
                try
                {
                    lock (lockThis)
                    {
                        //Console.WriteLine("{0} Поток отчетов", DateTime.Now.ToString("F"));
                        //TODO: протестировать и расскоментить
                        FTP.GetFiles4Reports();
                        System.Threading.Thread.Sleep(30000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} Exception caught.", ex);
                    Logs.ErrorLog(ex.ToString());
                }
            }
        }

        static void WhatsAppThread()
        {
            CultureInfo ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            //TODO: генерация отчетов
            System.Object lockThis = new System.Object();
            //while (true)
            //{
                try
                {
                    lock (lockThis)
                    {
                        Console.WriteLine("{0} Поток WhatsApp", DateTime.Now.ToString("F"));
                        //TODO: протестировать и расскоментить
                        //Classes.Service.WhatsApp.Send("привет /r/n так");
                        Classes.Service.sms.Send("привет /r/n так");
                        //System.Threading.Thread.Sleep(30000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} Exception caught.", ex);
                    Logs.ErrorLog(ex.ToString());
                }
            //}
        }

        static Program()
        {
            Resolver.RegisterDependencyResolver();
        }

        static void Main(string[] args)
        {
            ConfigEncryption.ToggleConfigEncryption();

            Const.CheckIsSingleInstance();
            //System.Threading.Thread.Sleep(600000);

            CultureInfo ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            //WorkMethods.ReturnReception();
            //WorkMethods.PrepareOutgoingConsignmentOperation("C:\\Projects\\OMKS\\mercury\\Работы\\avans\\241601.xml");
            //return;
            Logs.Log(string.Format("----------------------------Начало работы {0}------------------------------------", Const.FTPServer));
            Const.DoubleNotice = new List<string>();
            Const.InitCheeseAndButter();
            Const.InitProductItems();
            while (Const.GlobalProductItemList.Count == 0)
            {
                Const.InitProductItems();
            }

            //TODO: РАСКОММЕНТИРОВАТЬ!!!!!!!!!!!!!!!!!!!!!!!!!
            Const.InitStockEntrys();
            while (Const.GlobalStockEntryList.Count == 0)
            {
                Const.InitStockEntrys();
            }
            Enterprise enterprise = new Enterprise(); enterprise.guid = Const.EnterpriseId; //OMK                        
            //StockEntrys.MergeAll(RegisterModificationType.MERGE);            
            //fileNames.AddRange(FTP.GetFilesByRootPath(Const.FTPPathRegisterProductionOperationRequest, Enum.RequestTypes.registerProductionOperationRequest, -1));
            //fileNames.AddRange(FTP.GetFilesByRootPath(Const.FTPPathPrepareOutgoingConsignmentRequest, Enum.RequestTypes.prepareOutgoingConsignmentRequest, -1));
            
            Thread threadRegisterProductionOperation = null;
            #region ProductItemThread
            //FTP.GetFiles4ProductItems();
            //return;
            Thread threadProductItem = null;
            threadProductItem = new Thread(ProductItemThread);
            threadProductItem.Name = "ProductItemThread";
            //threadProductItem.Start();
            #endregion ProductItemThread
            #region ReturnThread
            //WorkMethods.ReturnReception();
            //return;
            Thread threadReturn = null;
            threadReturn = new Thread(ReturnThread);
            threadReturn.Name = "ReturnThread";
            threadReturn.Start();
            #endregion ReturnThread
            #region IncomingThread
            //для теста
            //WorkMethods.IncomingReception();
            //return;
            //для теста
            Thread threadIncoming = null;
            threadIncoming = new Thread(IncomingThread);
            threadIncoming.Name = "IncomingThread";
            threadIncoming.Start();
            #endregion IncomingThread
            #region ErrorThread
            //FTP.GetFiles4Error(Const.FTPPathPrepareOutgoingConsignmentRequest, -2);
            //return;
            Thread threadError = null;
            threadError = new Thread(ErrorThread);
            threadError.Name = "ErrorThread";
            //threadError.Start();
            #endregion ErrorThread
            #region ReportThread
            Thread threadReport = null;
            threadRegisterProductionOperation = new Thread(RegisterProductionOperationThread);
            threadRegisterProductionOperation.Name = "ReportThread";
            //threadRegisterProductionOperation.Start();
            threadReport = new Thread(ReportThread);
            threadReport.Name = "ReportThread";
            threadReport.Start(); //Const.FTPPathRegisterProductionOperationRequest, Enum.RequestTypes.prepareOutgoingConsignmentRequest
            #endregion ReportThread
            #region WhatsAppThread
            //Thread threadWhatsApp = null;
            //threadWhatsApp = new Thread(WhatsAppThread);
            //threadWhatsApp.Name = "WhatsAppThread";
            //threadWhatsApp.Start();
            //threadWhatsApp.Abort();
            #endregion WhatsAppThread
            System.Object lockThis = new System.Object();
            while (true)
            {
                try
                {
                    lock (lockThis)
                    {
                        //Logs.Log(string.Format("------------------------------------Начало работы------------------------------------"));
                        //Console.WriteLine("{0} Поток отгрузки", DateTime.Now.ToString("F"));
                        #region проверка лицензии
                        //bool isLicensed = false;
                        //isLicensed = ServiceDB.CheckLicense(Const.IssuerId);
                        //if (!isLicensed)
                        //{
                        //    Logs.Log(string.Format("!!!Проблема соединения с интернетом, ИР {0}", Const.FTPServer));
                        //    System.Threading.Thread.Sleep(60000);
                        //    isLicensed = ServiceDB.CheckLicense(Const.IssuerId);
                        //}
                        //if (!isLicensed)
                        //{
                        //    #region запись в БД для рассылки Телеграмм
                        //    ApplicationResponse ar = new ApplicationResponse(Guid.NewGuid().ToString());
                        //    ar.Status = Enum.ApplicationStatus.REJECTED;
                        //    ar.RcvDate = DateTime.Now;
                        //    ar.ErrorList = new List<Error>();
                        //    Error error = new Error("ADM00001", "!!!Проблема соединения с интернетом, работа ИР ЗАВЕРШЕНА!!!");
                        //    ar.ErrorList.Add(error);
                        //    ar.Save();
                        //    #endregion запись в БД для рассылки Телеграмм
                        //    Logs.Log(string.Format("!!!Проблема соединения с интернетом, работа ИР {0} ЗАВЕРШЕНА!!!", Const.FTPServer));
                        //    Environment.Exit(0);
                        //}
                        #endregion проверка лицензии
                        Const.CheckLicense();
                        if (DateTime.Now.Hour == 23 && DateTime.Now.Minute > 57)
                        {
                            int bufPause = (86400 - (int) ((DateTime.Now - DateTime.Today).TotalSeconds));
                            Logs.Log(string.Format("Буферная пауза перед полуночью длительнотью {0} секунд", bufPause));
                            System.Threading.Thread.Sleep(bufPause * 1000);
                        }
                        FTP.GetFilesByRootPath(Const.FTPPathRegisterProductionOperationRequest, Enum.RequestTypes.registerProductionOperationRequest, -30); //-5
                        System.Threading.Thread.Sleep(1000);
                        StockEntrys.MergeAll();
                        FTP.GetFilesByRootPath(Const.FTPPathPrepareOutgoingConsignmentRequest, Enum.RequestTypes.prepareOutgoingConsignmentRequest, -1); //-30
                        System.Threading.Thread.Sleep(1000);
                        FTP.GetFilesByRootPath(Const.FTPPathInvalidate, Enum.RequestTypes.InvalidateRequest, -6);
                        System.Threading.Thread.Sleep(1000);
                        FTP.GetFilesByRootPath(Const.FTPPathDeleteTTN, Enum.RequestTypes.DeleteTTNRequest, -1);
                        System.Threading.Thread.Sleep(1000);

                        //FTP.GetFiles4Reports();
                        //System.Threading.Thread.Sleep(1000);
                        //Logs.Log(string.Format("------------------------------------Окончание работы------------------------------------"));
                        //System.Threading.Thread.Sleep(60000);
                        if (DateTime.Now.Hour > 8 && DateTime.Now.Hour < 19 && DateTime.Now.Minute > 0 && DateTime.Now.Minute < 5
                            && DateTime.Now.Hour != lastUpHour)
                        {
                            Const.InitProductItems();
                            while (Const.GlobalProductItemList.Count == 0)
                            {
                                Const.InitProductItems();
                            }
                            lastUpHour = DateTime.Now.Hour;
                        }
                        if ((DateTime.Now.Hour == 10 || DateTime.Now.Hour == 17) && DateTime.Now.Minute > 5 && DateTime.Now.Minute < 10 && DateTime.Now.Hour != lastUpHourSE)
                        {
                            Const.InitStockEntrys();
                            while (Const.GlobalStockEntryList.Count == 0)
                            {
                                Const.InitStockEntrys();
                            }
                            lastUpHourSE = DateTime.Now.Hour;
                        }
                        if (Const.DoubleNotice != null && DateTime.Now.Minute > 0 && DateTime.Now.Minute < 5 && DateTime.Now.Hour != lastUpHourDN)
                        {
                            Const.DoubleNotice.Clear();
                            lastUpHourDN = DateTime.Now.Hour;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} Exception caught.", ex);
                    Logs.ErrorLog(ex.ToString());
                }
            }
        }
    }
}
