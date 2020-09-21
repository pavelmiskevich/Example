using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Threading.Tasks;
using Enum = Vetis.Classes.Service.Enum;
using Vetis.Classes.Service;

namespace Vetis.Classes
{
    public class ApplicationResponse : baseClassApplication
    {
        private const string TableName = "ApplicationResponses";
        #region параметры объекта
        private Guid _applicationId;
        /// <summary>
        /// идентификатор запроса GUID
        /// </summary>
        public Guid ApplicationId
        {
            get { return _applicationId; }
            set { _applicationId = value; }
        }
        private Enum.ApplicationStatus _status;
        /// <summary>
        /// Статус выполнения запроса
        /// </summary>
        public Enum.ApplicationStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }
        private string _serviceId;
        /// <summary>
        /// идектификатор-имя сервиса
        /// </summary>
        public string ServiceId
        {
            get { return _serviceId; }
            set { _serviceId = value; }
        }
        private string _issuerId;
        /// <summary>
        /// идектификатор предприятия
        /// </summary>
        public string IssuerId
        {
            get { return Const.IssuerId; }
            //set { _issuerId = value; }
        }
        private DateTime _issueDate;
        /// <summary>
        /// Дата и время обращения пользователя к заявочной системе. Устанавливается разработчиком клиентской системы
        /// </summary>
        public DateTime IssueDate
        {
            get { return DateTime.Now; /*_issueDate*/}
            //set { _issueDate = value; }
        }
        private DateTime _rcvDate;
        /// <summary>
        /// Дата и время формирования результата
        /// </summary>
        public DateTime RcvDate
        {
            get { return _rcvDate; }
            set { _rcvDate = value; }
        }
        private DateTime _prdcRsltDate;
        /// <summary>
        /// Дата и время получения результата выполнения заявки
        /// </summary>
        public DateTime PrdcRsltDate
        {
            get { return _prdcRsltDate; }
            set { _prdcRsltDate = value; }
        }
        private XmlDocument _result;
        /// <summary>
        /// Результат выполнения заявки
        /// </summary>
        public XmlDocument Result
        {
            get { return _result; }
            set { _result = value; }
        }
        private XmlDocument _errors;
        /// <summary>
        /// Ошибки выполнения заявки
        /// </summary>
        public XmlDocument Errors
        {
            get { return _errors; }
            set { _errors = value; }
        }
        private List<Error> _errorList;
        /// <summary>
        /// Ошибки выполнения заявки списком
        /// </summary>
        public List<Error> ErrorList
        {
            get { return _errorList; }
            set { _errorList = value; }
        }
        private int _requestCount;
        /// <summary>
        /// Количество повторных запросов
        /// </summary>
        public int RequestCount
        {
            get { return _requestCount; }
            set { _requestCount = value; }
        }
        private XmlNodeList xmlNodeList;
        #endregion

        public ApplicationResponse()
        {
            _status = Enum.ApplicationStatus.NOTDEFINED;
        }
        public ApplicationResponse(string applicationId)
        {
            try
            {
                _applicationId = Guid.Parse(applicationId);
                _status = Enum.ApplicationStatus.NOTDEFINED;                
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                Logs.ErrorLog(ex.ToString());
            }
        }

        /// <summary>
        /// Конструктор из первоначального ответа API 2.0
        /// </summary>
        /// <param name="receiveApplicationResultResponse"></param>
        public ApplicationResponse(XmlDocument receiveApplicationResultResponse)
        {
            try
            {
                #region Errors
                XmlNodeList xmlNL = receiveApplicationResultResponse.GetElementsByTagName("error");
                if (xmlNL.Count > 0)
                {
                    _errorList = new List<Error>();
                    foreach (XmlNode xmlNode in xmlNL)
                    {
                        Error error = new Error(xmlNode.Attributes["code"].Value, xmlNode.InnerText);
                        _errorList.Add(error);                        
                        string er = string.Format("{0} - {1}", xmlNode.Attributes["code"].Value, xmlNode.InnerText);
                        Console.WriteLine("{0} Exception caught.", er);
                        Logs.ErrorLog(er);
                        error = null;
                    }
                    _errors = receiveApplicationResultResponse;
                    //TODO: придумать как XmlNodeList сохранить в БД
                }
                else
                {
                    xmlNL = receiveApplicationResultResponse.GetElementsByTagName("apl:error");
                    if (xmlNL.Count > 0)
                    {
                        _errorList = new List<Error>();
                        foreach (XmlNode xmlNode in xmlNL)
                        {
                            Error error = new Error(xmlNode.Attributes["code"].Value, xmlNode.InnerText);
                            _errorList.Add(error);
                            string er = string.Format("{0} - {1}", xmlNode.Attributes["code"].Value, xmlNode.InnerText);
                            Console.WriteLine("{0} Exception caught.", er);
                            Logs.ErrorLog(er);
                            error = null;
                        }
                        _errors = receiveApplicationResultResponse;
                        //TODO: придумать как XmlNodeList сохранить в БД
                    }
                }
                #endregion Errors

                xmlNL = receiveApplicationResultResponse.GetElementsByTagName("applicationId");
                if (xmlNL.Count == 0)
                    xmlNL = receiveApplicationResultResponse.GetElementsByTagName("ws:applicationId");
                _applicationId = Guid.Parse(xmlNL[0].InnerText);
                xmlNL = receiveApplicationResultResponse.GetElementsByTagName("status");
                if (xmlNL.Count == 0)
                    xmlNL = receiveApplicationResultResponse.GetElementsByTagName("ws:status");
                _status = (Enum.ApplicationStatus)System.Enum.Parse(typeof(Enum.ApplicationStatus), xmlNL[0].InnerText);
                _serviceId = receiveApplicationResultResponse.GetElementsByTagName("serviceId")[0].InnerText; //Const.ServiceId;
                xmlNL = receiveApplicationResultResponse.GetElementsByTagName("issuerId");
                if (xmlNL.Count == 0)
                    xmlNL = receiveApplicationResultResponse.GetElementsByTagName("ws:issuerId");
                _issuerId = xmlNL[0].InnerText;
                _issueDate = DateTime.Parse(receiveApplicationResultResponse.GetElementsByTagName("issueDate")[0].InnerText);
                _rcvDate = DateTime.Parse(receiveApplicationResultResponse.GetElementsByTagName("rcvDate")[0].InnerText);
                xmlNL = receiveApplicationResultResponse.GetElementsByTagName("prdcRsltDate");
                if (xmlNL.Count != 0)
                {
                    xmlNodeList = receiveApplicationResultResponse.GetElementsByTagName("prdcRsltDate");
                    if (xmlNodeList.Count > 0)
                        _prdcRsltDate = DateTime.Parse(xmlNodeList[0].InnerText);
                    _result = receiveApplicationResultResponse;                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                Logs.ErrorLog(ex.ToString());
            }
        }

        /// <summary>
        /// ПРОВЕРИТЬ. Скорее всего API 1.4
        /// </summary>
        /// <param name="receiveApplicationResultResponse"></param>
        //public ApplicationResponse(XmlDocument receiveApplicationResultResponse)
        //{
        //    XmlNodeList xmlNL = receiveApplicationResultResponse.GetElementsByTagName("applicationId");
        //    if (xmlNL.Count == 0)
        //        xmlNL = receiveApplicationResultResponse.GetElementsByTagName("ws:applicationId");
        //    _applicationId = Guid.Parse(xmlNL[0].InnerText);
        //    xmlNL = receiveApplicationResultResponse.GetElementsByTagName("issuerId");
        //    if (xmlNL.Count == 0)
        //        xmlNL = receiveApplicationResultResponse.GetElementsByTagName("ws:issuerId");
        //    _issuerId = xmlNL[0].InnerText;
        //    xmlNL = receiveApplicationResultResponse.GetElementsByTagName("status");
        //    if (xmlNL.Count != 0)
        //    {
        //        _status = (Enum.ApplicationStatus)System.Enum.Parse(typeof(Enum.ApplicationStatus), xmlNL[0].InnerText);
        //        _serviceId = receiveApplicationResultResponse.GetElementsByTagName("serviceId")[0].InnerText; //Const.ServiceId;            
        //        _issueDate = DateTime.Parse(receiveApplicationResultResponse.GetElementsByTagName("issueDate")[0].InnerText);
        //        _rcvDate = DateTime.Parse(receiveApplicationResultResponse.GetElementsByTagName("rcvDate")[0].InnerText);
        //        xmlNodeList = receiveApplicationResultResponse.GetElementsByTagName("prdcRsltDate");
        //        if (xmlNodeList.Count > 0)
        //            _prdcRsltDate = DateTime.Parse(xmlNodeList[0].InnerText);
        //        _result = receiveApplicationResultResponse;
        //        //TODO: получение блока ошибок и их нормализация
        //    }
        //}

        public ApplicationResponse(SqlDataReader reader)
        {
            try
            {
                _applicationId = Guid.Parse(reader[0].ToString());
                _status = (Enum.ApplicationStatus)System.Enum.Parse(typeof(Enum.ApplicationStatus), reader[1].ToString());
                _serviceId = reader[2].ToString();
                _issuerId = reader[3].ToString();
                _issueDate = DateTime.Parse(reader[4].ToString());
                _rcvDate = DateTime.Parse(reader[5].ToString());
                if (reader[7] != null && reader[7].ToString() != "")
                {
                    _prdcRsltDate = DateTime.Parse(reader[6].ToString());
                    _result = new XmlDocument();
                    _result.LoadXml(reader[7].ToString());
                    _errors = new XmlDocument();
                    if (reader[8] != null && reader[8].ToString() != "")
                        _errors.LoadXml(reader[8].ToString());
                    //TODO: получение списка ошибок
                    _errorList = Error.SelectAllByApplicationId(_applicationId);
                }
                CreDate = DateTime.Parse(reader[9].ToString());
                IsActive = bool.Parse(reader[10].ToString());
                if(reader[11] != null)
                    _requestCount = int.Parse(reader[11].ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                Logs.ErrorLog(ex.ToString());
            }
        }

        private void ObjectFill(SqlDataReader reader)
        {
            try
            {
                _applicationId = Guid.Parse(reader[0].ToString());
                _status = (Enum.ApplicationStatus)System.Enum.Parse(typeof(Enum.ApplicationStatus), reader[1].ToString());
                _serviceId = reader[2].ToString();
                _issuerId = reader[3].ToString();
                _issueDate = DateTime.Parse(reader[4].ToString());
                _rcvDate = DateTime.Parse(reader[5].ToString());
                if (reader[7] != null && reader[7].ToString() != "")
                {
                    _prdcRsltDate = DateTime.Parse(reader[6].ToString());
                    _result = new XmlDocument();
                    _result.LoadXml(reader[7].ToString());
                    _errors = new XmlDocument();
                    if (reader[8] != null && reader[8].ToString() != "")
                        _errors.LoadXml(reader[8].ToString());
                    //TODO: получение списка ошибок
                    _errorList = Error.SelectAllByApplicationId(_applicationId);
                }
                CreDate = DateTime.Parse(reader[9].ToString());
                IsActive = bool.Parse(reader[10].ToString());
                if (reader[11] != null)
                    _requestCount = int.Parse(reader[11].ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                Logs.ErrorLog(ex.ToString());
            }
        }

        /// <summary>
        /// Сохранение в БД с параметрами внутреннего объекта
        /// </summary>
        public override Guid Save()
        {
            SqlConnection sqlConnection = new SqlConnection(Const.cs);
            SqlCommand sqlCommand = null;

            sqlCommand = new SqlCommand(TableName + "Insert", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            try
            {
                par = new SqlParameter("@ApplicationId", SqlDbType.UniqueIdentifier);
                par.Value = _applicationId;
                sqlCommand.Parameters.Add(par);
                par = new SqlParameter("@IssuerId", SqlDbType.NVarChar, 50);
                par.Value = _issuerId;
                sqlCommand.Parameters.Add(par);
                par = new SqlParameter("@IssueDate", SqlDbType.DateTime);
                par.Value = IssueDate;
                sqlCommand.Parameters.Add(par);
                par = new SqlParameter("@Status", SqlDbType.NVarChar, 50);
                par.Value = _status;
                sqlCommand.Parameters.Add(par);
                par = new SqlParameter("@ServiceId", SqlDbType.NVarChar, 50);
                par.Value = _serviceId;
                sqlCommand.Parameters.Add(par);
                par = new SqlParameter("@RcvDate", SqlDbType.DateTime);
                par.Value = _rcvDate;
                sqlCommand.Parameters.Add(par);
                if (_prdcRsltDate != DateTime.MinValue)
                {                    
                    par = new SqlParameter("@PrdcRsltDate", SqlDbType.DateTime);
                    par.Value = _prdcRsltDate;
                    sqlCommand.Parameters.Add(par);
                    par = new SqlParameter("@Result", SqlDbType.Xml);
                    ////par.Value = (_result.RemoveChild(_result.FirstChild)).InnerXml;
                    //XmlNode xN = _result.FirstChild;
                    //xN.Value = xN.Value.Replace("UTF-8", "UTF-16");
                    //_result.ReplaceChild(xN, _result.FirstChild);
                    par.Value = Service.XML.RemoveXmlDefinition(_result).InnerXml;
                    sqlCommand.Parameters.Add(par);
                    if (_errors != null)
                    {
                        par = new SqlParameter("@Errors", SqlDbType.Xml);
                        par.Value = _errors.InnerXml;
                        sqlCommand.Parameters.Add(par);                        
                    }
                }
                par = new SqlParameter("@RequestCount", SqlDbType.Int);
                par.Value = _requestCount;
                sqlCommand.Parameters.Add(par);

                //par = new SqlParameter("@Id", SqlDbType.Int);
                //par.Direction = ParameterDirection.Output;
                //sqlCommand.Parameters.Add(par);

                sqlConnection.Open();
                //sqlCommand.ExecuteNonQuery();
                //Id = int.Parse(sqlCommand.Parameters["@Id"].Value.ToString());
                // используется для @@IDENTITY в SQL запросе
                ApplicationId = Guid.Parse(sqlCommand.ExecuteScalar().ToString());
                
                if (_errorList != null)
                {
                    foreach (Error error in _errorList)
                    {
                        int errorId = new Error(error.Name, error.Description).Save();
                        ResponceError.Save(ApplicationId, errorId);
                    }
                    //TODO: подумать о подмене учетки при ошибки блокировки
                    //2020 - 05 - 07T10: 24:54: MERC31558 - Данные не могут быть получены, так как у пользователя, ответственного за выполнение, отсутствует доступ
                    //2020 - 05 - 07T10: 24:54: MERC37558 - Данные не могут быть получены, так как у пользователя, ответственного за выполнение, отсутствует доступ
                    #region обновление остатков при ошибке MERC17276 или MERC17009 или MERC02137 В запросе для записи складского журнала продукции указан идентификатор устаревшей версии записи реестра РСХН.
                    //MERC02137 - Используемый объём должен быть меньше или равен остатку
                    if (_errorList.Exists(x => x.Name == "MERC17009") || _errorList.Exists(x => x.Name == "MERC17276") || _errorList.Exists(x => x.Name == "MERC02137"))
                    {
                        Logs.Log(string.Format("!!!Ошибка MERC17009 или MERC17276 или MERC02137, пауза 15 минут и обновление остатков!!!"));
                        System.Threading.Thread.Sleep(900000);
                        Const.InitStockEntrys();
                        while (Const.GlobalStockEntryList.Count == 0)
                        {
                            Const.InitStockEntrys();
                        }
                        throw new Exception(Const.ErrorThrow);
                    }
                    #endregion обновление остатков при ошибке MERC17276 или MERC17009 или MERC02137 В запросе для записи складского журнала продукции указан идентификатор устаревшей версии записи реестра РСХН.
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                Logs.ErrorLog(ex.ToString());
                throw;
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection = null;
            }

            return ApplicationId;
        }

        /// <summary>
        /// Активация сущности
        /// </summary>
        public void SetIsActive()
        {
            SqlConnection sqlConnection = new SqlConnection(Const.cs);
            SqlCommand sqlCommand = new SqlCommand("SetIsActive", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            try
            {
                SqlParameter par = new SqlParameter("@ApplicationId", SqlDbType.UniqueIdentifier);
                par.Value = ApplicationId;
                sqlCommand.Parameters.Add(par);
                par = new SqlParameter("@Table", SqlDbType.NVarChar, 50);
                par.Value = TableName;
                sqlCommand.Parameters.Add(par);

                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                IsActive = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                Logs.ErrorLog(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection = null;
            }
        }

        /// <summary>
        /// Деактивация сущности
        /// </summary>
        public void SetNotIsActive()
        {
            SqlConnection sqlConnection = new SqlConnection(Const.cs);
            SqlCommand sqlCommand = new SqlCommand("SetNotIsActive", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            try
            {
                SqlParameter par = new SqlParameter("@ApplicationId", SqlDbType.UniqueIdentifier);
                par.Value = ApplicationId;
                sqlCommand.Parameters.Add(par);
                par = new SqlParameter("@Table", SqlDbType.NVarChar, 50);
                par.Value = TableName;
                sqlCommand.Parameters.Add(par);

                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                IsActive = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                Logs.ErrorLog(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection = null;
            }
        }

        /// <summary>
        /// Получение всех объектов из БД
        /// </summary>
        public static List<ApplicationRequest> SelectAll()
        {
            List<ApplicationRequest> list = null;
            ApplicationRequest entity = null;
            SqlConnection sqlConnection = new SqlConnection(Const.cs);
            SqlCommand sqlCommand = new SqlCommand("GetAll", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            try
            {
                SqlParameter par = new SqlParameter("@Table", SqlDbType.NVarChar, 50);
                par.Value = TableName;
                sqlCommand.Parameters.Add(par);
                //par = new SqlParameter("@OrderBy", SqlDbType.NVarChar, 250);
                //par.Value = "Mininitialinstalment, Minvalue";
                //sqlCommand.Parameters.Add(par);

                sqlConnection.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    list = new List<ApplicationRequest>();
                    while (reader.Read())
                    {
                        entity = new ApplicationRequest(reader);
                        list.Add(entity);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                Logs.ErrorLog(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection = null;
            }
            return list;
        }

        /// <summary>
        /// Получение всех объектов из БД по IsActive
        /// </summary>
        public static List<ApplicationRequest> SelectAll(bool isActive)
        {
            List<ApplicationRequest> list = null;
            ApplicationRequest entity = null;
            SqlConnection sqlConnection = new SqlConnection(Const.cs);
            //SqlCommand sqlCommand = new SqlCommand("GetAll", sqlConnection);
            SqlCommand sqlCommand = new SqlCommand("GetAll", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            try
            {
                SqlParameter par = new SqlParameter("@Table", SqlDbType.NVarChar, 50);
                par.Value = TableName;
                sqlCommand.Parameters.Add(par);
                par = new SqlParameter("@IsActive", SqlDbType.Bit);
                par.Value = isActive;
                sqlCommand.Parameters.Add(par);

                sqlConnection.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    list = new List<ApplicationRequest>();
                    while (reader.Read())
                    {
                        entity = new ApplicationRequest(reader);
                        list.Add(entity);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                Logs.ErrorLog(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection = null;
            }
            return list;
        }

        /// <summary>
        /// Поиск по Id или по ApplicationId
        /// </summary>
        public override void GetByIdOrApplicationId()
        {
            SqlConnection sqlConnection = new SqlConnection(Const.cs);
            SqlDataReader reader = null;
            //SqlCommand sqlCommand = new SqlCommand("GetCreditServiceBybaseServicesYaId", sqlConnection);
            SqlCommand sqlCommand = new SqlCommand("GetByIdOrApplicationId", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            try
            {
                //SqlParameter par = new SqlParameter("@YaId", SqlDbType.Int);
                //par.Value = yaId;
                //sqlCommand.Parameters.Add(par);

                if (Id != 0)
                {
                    par = new SqlParameter("@Id", SqlDbType.Int);
                    par.Value = Id;
                    sqlCommand.Parameters.Add(par);
                }
                else
                {
                    par = new SqlParameter("@ApplicationId", SqlDbType.UniqueIdentifier);
                    par.Value = ApplicationId;
                    sqlCommand.Parameters.Add(par);
                }
                par = new SqlParameter("@Table", SqlDbType.NVarChar, 50);
                par.Value = TableName;
                sqlCommand.Parameters.Add(par);

                sqlConnection.Open();
                reader = sqlCommand.ExecuteReader();
                reader.Read();
                if (reader.HasRows)
                    ObjectFill(reader);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                Logs.ErrorLog(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection = null;
            }
        }

        /// <summary>
        /// Поиск по Id или по ApplicationId
        /// </summary>
        public static ApplicationResponse GetByIdOrApplicationId(int id, Guid applicationId)
        {
            SqlParameter par;
            ApplicationResponse entity = null;
            SqlConnection sqlConnection = new SqlConnection(Const.cs);
            SqlDataReader reader = null;
            //SqlCommand sqlCommand = new SqlCommand("GetCreditServiceBybaseServicesYaId", sqlConnection);
            SqlCommand sqlCommand = new SqlCommand("GetByIdOrApplicationId", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            try
            {
                //SqlParameter par = new SqlParameter("@YaId", SqlDbType.Int);
                //par.Value = yaId;
                //sqlCommand.Parameters.Add(par);

                if (id != 0)
                {
                    par = new SqlParameter("@Id", SqlDbType.Int);
                    par.Value = id;
                    sqlCommand.Parameters.Add(par);
                }
                else
                {
                    par = new SqlParameter("@ApplicationId", SqlDbType.UniqueIdentifier);
                    par.Value = applicationId;
                    sqlCommand.Parameters.Add(par);
                }
                par = new SqlParameter("@Table", SqlDbType.NVarChar, 50);
                par.Value = TableName;
                sqlCommand.Parameters.Add(par);

                sqlConnection.Open();
                reader = sqlCommand.ExecuteReader();
                reader.Read();
                if (reader.HasRows)
                    entity = new ApplicationResponse(reader);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                Logs.ErrorLog(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection = null;
            }
            return entity;
        }
    }
}
