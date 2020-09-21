using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Vetis.Classes.Service;
using System.Threading;
using System.Net;
using Enum = Vetis.Classes.Service.Enum;
using XTO = Vetis.Classes.XmlToObject;

namespace Vetis.Classes
{
    public class XMLResponces
    {
        public  static ApplicationResponse GetResponce(XMLRequests xmlRequests, string url, string fileName = "")
        {            
            #region var
            HttpStatusCode code;
            XmlDocument responseXml = null;
            ApplicationRequest applicationRequest = null;
            ApplicationResponse applicationResponce = null;
            int requestCount = 1;
            #endregion var
            if (fileName == "") fileName = Guid.NewGuid().ToString();
            try
            {
                responseXml = SoapCaller.CallWebService(xmlRequests.RequestXML, url, out code);
                if (code == HttpStatusCode.OK)
                {
                    applicationRequest = new ApplicationRequest(fileName, xmlRequests, responseXml);
                    applicationRequest.Save();
                    applicationResponce = new ApplicationResponse(responseXml);
                    //applicationResponce.Save();
                    //if (applicationResponce.Status == Enum.ApplicationStatus.ACCEPTED)
                    if (applicationResponce.Status != Enum.ApplicationStatus.REJECTED)
                    {
                        xmlRequests = new XMLRequests(applicationResponce.ApplicationId);
                        while (applicationResponce.Status != Enum.ApplicationStatus.COMPLETED)
                        {
                            TimeSpan i = applicationResponce.RcvDate - applicationResponce.IssueDate;
                            Thread.Sleep(1000); //Thread.Sleep(3000);
                            if (i.Seconds > 0)
                                Thread.Sleep((i.Seconds + 1) * 1000); //сколько секунд ждать, для минут 60000
                            responseXml = SoapCaller.CallWebService(xmlRequests.RequestXML, url, out code);
                            requestCount++;
                            applicationResponce = new ApplicationResponse(responseXml);
                            applicationResponce.RequestCount = requestCount;
                            //TODO: определить количество повторов до выхода и подумать о вынесении в Const
                            //if (requestCount > 100)
                            //{
                            //    applicationResponce.ErrorList.Add(new Error("Превышено допустимое число повторных запросов", "Подозрение на зависание запроса в статусе IN_PROCESS"));
                            //    applicationResponce.Status = Enum.ApplicationStatus.REJECTED;
                            //}
                            if (applicationResponce.Status == Enum.ApplicationStatus.COMPLETED || applicationResponce.Status == Enum.ApplicationStatus.REJECTED)
                                applicationResponce.Save();
                            if (applicationResponce.Status == Enum.ApplicationStatus.REJECTED)
                                break;
                        }
                        XmlNodeList xmlNodeList = responseXml.GetElementsByTagName("merc:stockEntry");
                        if (xmlNodeList == null || xmlNodeList.Count == 0)
                            xmlNodeList = responseXml.GetElementsByTagName("vd:stockEntry");
                        if (xmlNodeList != null)
                        {
                            foreach (XmlNode xmlNode in xmlNodeList)
                            {
                                //StockEntrys.Save(XTO.StockEntry(xmlNode));
                            }
                        }                            
                    }
                    else
                    {
                        //TODO: сделать обработку ошибок           
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: сделать сохранение остатков если получена ошибка о недостаточности количества
                if (ex.Message != Const.ErrorThrow)
                {
                    Console.WriteLine("{0} Exception caught.", ex);
                    Logs.ErrorLog(ex.ToString());
                }
                else
                    throw;
            }
            finally
            {
                #region var = null                
                responseXml = null;
                applicationRequest = null;
                //applicationResponce = null;
                #endregion var = null
            }
            //return responseXml;
            return applicationResponce;
        }

        public static ApplicationResponse GetResponce(string applicationId, string url, string fileName = "")
        {
            #region var
            HttpStatusCode code;
            XmlDocument responseXml = null;
            //ApplicationRequest applicationRequest = null;
            ApplicationResponse applicationResponce = null;
            XMLRequests xmlRequests = null;
            int requestCount = 1;
            #endregion var
            if (fileName == "") fileName = Guid.NewGuid().ToString();
            try
            {
                applicationResponce = new ApplicationResponse(applicationId);
                xmlRequests = new XMLRequests(applicationResponce.ApplicationId);
                while (applicationResponce.Status != Enum.ApplicationStatus.COMPLETED)
                {
                    TimeSpan i = applicationResponce.RcvDate - applicationResponce.IssueDate;
                    Thread.Sleep(1000);
                    if (i.Seconds > 0)
                        Thread.Sleep((i.Seconds + 1) * 1000); //сколько секунд ждать, для минут 60000
                    responseXml = SoapCaller.CallWebService(xmlRequests.RequestXML, url, out code);
                    requestCount++;
                    applicationResponce = new ApplicationResponse(responseXml);
                    applicationResponce.RequestCount = requestCount;
                    //TODO: определить количество повторов до выхода и подумать о вынесении в Const
                    //if (requestCount > 100)
                    //{
                    //    applicationResponce.ErrorList.Add(new Error("Превышено допустимое число повторных запросов", "Подозрение на зависание запроса в статусе IN_PROCESS"));
                    //    applicationResponce.Status = Enum.ApplicationStatus.REJECTED;
                    //}
                    if (applicationResponce.Status == Enum.ApplicationStatus.COMPLETED || applicationResponce.Status == Enum.ApplicationStatus.REJECTED)
                        applicationResponce.Save();
                    if (applicationResponce.Status == Enum.ApplicationStatus.REJECTED)
                        break;
                }
                XmlNodeList xmlNodeList = responseXml.GetElementsByTagName("merc:stockEntry");
                if (xmlNodeList == null || xmlNodeList.Count == 0)
                    xmlNodeList = responseXml.GetElementsByTagName("vd:stockEntry");
                if (xmlNodeList != null)
                {
                    foreach (XmlNode xmlNode in xmlNodeList)
                    {
                        //StockEntrys.Save(XTO.StockEntry(xmlNode));
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: сделать обработку ошибок
                //TODO: сделать сохранение остатков если получена ошибка о недостаточности количества
                Console.WriteLine("{0} Exception caught.", ex);
                Logs.ErrorLog(ex.ToString());
            }
            finally
            {
                #region var = null                
                responseXml = null;
                //applicationRequest = null;
                //applicationResponce = null;
                #endregion var = null
            }
            //return responseXml;
            return applicationResponce;
        }

        private static XmlNodeList GetXmlNodeListByNodeName(XmlDocument doc)
        {
            XmlNodeList entitys = null;

            return entitys;
        }


        //TODO: сделать единый метод для все справочных сервисов
        public static XmlDocument GetResponce(XMLRequestsDictionaryService xmlRequests, string url)
        {
            HttpStatusCode code;
            XmlDocument responseXml = null;
            ApplicationResponse applicationResponce = null;

            try
            {
                responseXml = SoapCaller.CallWebService(xmlRequests.RequestXML, url, out code);
                if (code == HttpStatusCode.OK)
                {
                    
                }
            }
            catch (Exception ex)
            {
                //TODO: сделать обработку ошибок
                //TODO: сделать сохранение остатков если получена ошибка о недостаточности количества
                Console.WriteLine("{0} Exception caught.", ex);
                Logs.ErrorLog(ex.ToString());
            }
            //return responseXml;
            return responseXml;
        }

        public static XmlDocument GetResponce(XMLRequestsEnterpriseService xmlRequests, string url)
        {
            HttpStatusCode code;
            XmlDocument responseXml = null;
            ApplicationResponse applicationResponce = null;

            try
            {
                responseXml = SoapCaller.CallWebService(xmlRequests.RequestXML, url, out code);
                if (code == HttpStatusCode.OK)
                {

                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("{0} Exception caught.", ex);
                //Logs.ErrorLog(ex.ToString());
            }
            //return responseXml;
            return responseXml;
        }

        public static XmlDocument GetResponce(XMLRequestsProductService xmlRequests, string url)
        {
            HttpStatusCode code;
            XmlDocument responseXml = null;
            ApplicationResponse applicationResponce = null;

            try
            {
                responseXml = SoapCaller.CallWebService(xmlRequests.RequestXML, url, out code);
                if (code == HttpStatusCode.OK)
                {

                }
            }
            catch (Exception ex)
            {
                //TODO: сделать обработку ошибок
                //TODO: сделать сохранение остатков если получена ошибка о недостаточности количества
                Console.WriteLine("{0} Exception caught.", ex);
                Logs.ErrorLog(ex.ToString());
            }
            //return responseXml;
            return responseXml;
        }

        public static XmlDocument ReceiveApplicationResultRequest()
        {
            XmlDocument _xml = new XmlDocument();
            _xml.LoadXml(@"<env:Envelope xmlns:env=""http://schemas.xmlsoap.org/soap/envelope/"">
               <env:Header/>
               <env:Body>
                  <submitApplicationResponse xmlns = ""http://api.vetrf.ru/schema/cdm/application/ws-definitions"">
                     <application xmlns = ""http://api.vetrf.ru/schema/cdm/application"">
                        <applicationId>21911466-1977-40b0-b8b6-2c08d5e12ba5</applicationId>
                        <status>ACCEPTED</status>
                        <serviceId>mercury-g2b.service</serviceId>
                        <issuerId>85d5d1c9-19cb-45dc-b53d-05378a51eb13</issuerId>
                        <issueDate>2017-11-08T10:32:08</issueDate>
                        <rcvDate>2018-01-04T10:15:16</rcvDate>
                     </application>
                  </submitApplicationResponse>
               </env:Body>
            </env:Envelope> ");
            return _xml;
        }

        public static XmlDocument ReceiveApplicationResultResponse()
        {
            XmlDocument _xml = new XmlDocument();
            _xml.LoadXml(@"<env:Envelope xmlns:env=""http://schemas.xmlsoap.org/soap/envelope/"">
               <env:Header/>
               <env:Body>
                  <receiveApplicationResultResponse xmlns = ""http://api.vetrf.ru/schema/cdm/application/ws-definitions"">
                     <application xmlns = ""http://api.vetrf.ru/schema/cdm/application"">
                        <applicationId>21911466-1977-40b0-b8b6-2c08d5e12ba5</applicationId>
                        <status>COMPLETED</status>
                        <serviceId>mercury-g2b.service</serviceId>
                        <issuerId>85d5d1c9-19cb-45dc-b53d-05378a51eb13</issuerId>
                        <issueDate>2017-11-08T10:32:08.000+03:00</issueDate>
                        <rcvDate>2018-01-04T10:15:16.000+03:00</rcvDate>
                        <prdcRsltDate>2018-01-04T10:15:16.000+03:00</prdcRsltDate>
                        <result>
                           <merc:getBusinessEntityUserResponse xmlns:vd=""http://api.vetrf.ru/schema/cdm/mercury/vet-document/v2"" xmlns:merc=""http://api.vetrf.ru/schema/cdm/mercury/g2b/applications/v2"" xmlns:base=""http://api.vetrf.ru/schema/cdm/base"">
                              <vd:user>
                                 <base:uuid>34378c73-bcf1-4e20-aa36-5ef4dbe897eb</base:uuid>
                                 <vd:login>ljubochko_ds_171122</vd:login>
                                 <vd:firstName>Дмитрий</vd:firstName>
                                 <vd:middleName>Сергеевич</vd:middleName>
                                 <vd:lastName>Любочко</vd:lastName>
                                 <vd:authorityList>
                                    <vd:authority>
                                       <vd:ID>MERCURY_ACCESS_WEB_READ</vd:ID>
                                       <vd:name>Доступ к веб - интерфейсу ИС Меркурий только на чтение</vd:name>
                                    </vd:authority>
                                    <vd:authority>
                                       <vd:ID>MERCURY_ACCESS_WEB_WRITE</vd:ID>
                                       <vd:name>Доступ к веб - интерфейсу ИС Меркурий на выполнение операций</vd:name>
                                    </vd:authority>
                                    <vd:authority>
                                       <vd:ID>MERCURY_ACCESS_API_READ</vd:ID>
                                       <vd:name>Доступ к ИС Меркурий через Ветис.API только на чтение</vd:name>
                                    </vd:authority>
                                    <vd:authority>
                                       <vd:ID>MERCURY_ACCESS_API_WRITE</vd:ID>
                                       <vd:name>Доступ к ИС Меркурий через Ветис.API на выполнение операций</vd:name>
                                    </vd:authority>
                                    <vd:authority>
                                       <vd:ID>MERCURY_HS_AUTHORIZED_APPLICANT</vd:ID>
                                       <vd:name>Авторизованный заявитель в системе Меркурий</vd:name>
                                    </vd:authority>
                                    <vd:authority>
                                       <vd:ID>MERCURY_CONTROL_WORKING_AREA</vd:ID>
                                       <vd:name>Управление зонами ответственности пользователей</vd:name>
                                    </vd:authority>
                                    <vd:authority>
                                       <vd:ID>MERCURY_CERTIFICATE_ACCEPTANCE</vd:ID>
                                       <vd:name>Гашение ВСД</vd:name>
                                    </vd:authority>
                                    <vd:authority>
                                       <vd:ID>MERCURY_HS_AUTHORIZED_ASSIGNMENT</vd:ID>
                                       <vd:name>Назначение уполномоченных ХС</vd:name>
                                    </vd:authority>
                                    <vd:authority>
                                       <vd:ID>MERCURY_AUTHORIZED_CERTIFICATE_ACCEPTANCE</vd:ID>
                                       <vd:name>Уполномоченное гашение ВСД</vd:name>
                                    </vd:authority>
                                    <vd:authority>
                                       <vd:ID>MERCURY_AUTHORIZED_RETURNED_CERTIFICATE_CREATION</vd:ID>
                                       <vd:name>Уполномоченное оформление возвратных ВСД</vd:name>
                                    </vd:authority>
                                    <vd:authority>
                                       <vd:ID>MERCURY_HS_FISH_CERTIFICATE</vd:ID>
                                       <vd:name>Сертификация уловов ВБР</vd:name>
                                    </vd:authority>
                                    <vd:authority>
                                       <vd:ID>MERCURY_PRODUCTION_BATCH_CERTIFICATE</vd:ID>
                                       <vd:name>Оформление ВСД на производственную партию</vd:name>
                                    </vd:authority>
                                    <vd:authority>
                                       <vd:ID>MERCURY_HS_AUTHORIZED_646</vd:ID>
                                       <vd:name>Уполномоченное лицо(Оформление ВСД на продукцию из Приказа МСХ РФ №676)</vd:name>
                                    </vd:authority>
                                    <vd:authority>
                                       <vd:ID>MERCURY_HS_FRESH_MILK_CERTIFICATE</vd:ID>
                                       <vd:name>Оформление ВСД на сырое молоко(при наличии справки о безопасности сырого молока)</vd:name>
                                    </vd:authority>
                                    <vd:authority>
                                       <vd:ID>MERCURY_RETURNED_CERTIFICATE_CREATION</vd:ID>
                                       <vd:name>Оформление возвратных ВСД</vd:name>
                                    </vd:authority>
                                 </vd:authorityList>
                                 <vd:authorityList>
                                    <vd:authority>
                                       <vd:ID>ARGUS_HS_AUTHORIZED_APPLICANT</vd:ID>
                                       <vd:name>Авторизованный заявитель в системе Аргус</vd:name>
                                    </vd:authority>
                                 </vd:authorityList>
                              </vd:user>
                           </merc:getBusinessEntityUserResponse>
                        </result>
                     </application>
                  </receiveApplicationResultResponse>
               </env:Body>
            </env:Envelope>");
            return _xml;
        }
    }
}
