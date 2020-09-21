using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
using Vetis.Classes;
using Vetis.Classes.Service;
using Enum = Vetis.Classes.Service.Enum;
using Vetis.VetisAPI;
using Excel = Microsoft.Office.Interop.Excel;

namespace VetisTest
{
    /// <summary>
    /// Сводное описание для CodedUITest1
    /// </summary>
    [CodedUITest]
    public class XMLRequestsEnterpriseServiceTest
    {
        string url = Const.UrlEnterpriseService;
        XMLRequests entity;
        XMLRequests entityGet;
        XMLRequestsEnterpriseService xmlRequests = null;
        XmlDocument answer = null;
        string name = "name";
        public XMLRequestsEnterpriseServiceTest()
        {
        }

        [TestMethod]
        public void Test()
        {
            
        }

        [TestMethod]
        public void GetActivityLocationListTest()
        {
            BusinessEntity businessEntity = new BusinessEntity();
            businessEntity.guid = "c6196e02-4637-862e-86d5-654175a1743f"; //Const.IssuerId;
            xmlRequests = 
                new XMLRequestsEnterpriseService(Enum.RequestTypesEnterpriseService.getActivityLocationListRequest);
            xmlRequests.GetActivityLocationList(businessEntity);
            answer = XMLResponces.GetResponce(xmlRequests, url);
            Assert.AreNotEqual(answer, null, "GetActivityLocationList() answer not null");
            Assert.AreNotEqual(answer.GetElementsByTagName("dt:activityLocationList"), null, "GetActivityLocationList() correctly");
        }

        [TestMethod]
        public void GetBusinessEntityByGuidTest()
        {
            BusinessEntity businessEntity = new BusinessEntity();
            businessEntity.guid = Const.IssuerId;
            xmlRequests =
                new XMLRequestsEnterpriseService(Enum.RequestTypesEnterpriseService.getBusinessEntityByGuidRequest);
            xmlRequests.GetBusinessEntityByGuid("ba06e77a-4ea0-4652-852c-e63301f8192e"); //Guid.Parse(Const.IssuerId) //c566587f-39fc-4c58-8d97-ad99173b6d6b
            answer = XMLResponces.GetResponce(xmlRequests, url);
            Assert.AreNotEqual(answer, null, "GetBusinessEntityByGuid() answer not null");
            Assert.AreNotEqual(answer.GetElementsByTagName("dt:businessEntity"), null, "GetBusinessEntityByGuid() correctly");
        }

        [TestMethod]
        public void GetBusinessEntityByUuidTest()
        {
            BusinessEntity businessEntity = new BusinessEntity();
            businessEntity.guid = Const.IssuerId;
            xmlRequests =
                new XMLRequestsEnterpriseService(Enum.RequestTypesEnterpriseService.getBusinessEntityByUuidRequest);
            xmlRequests.GetBusinessEntityByUuid("facfd8ec-c52e-4f14-b843-d4da521950f1"); //Guid.Parse(Const.IssuerId)
            answer = XMLResponces.GetResponce(xmlRequests, url);
            Assert.AreNotEqual(answer, null, "GetBusinessEntityByUuid() answer not null");
            Assert.AreNotEqual(answer.GetElementsByTagName("dt:businessEntity"), null, "GetBusinessEntityByUuid() correctly");
        }

        [TestMethod]
        public void GetEnterpriseByGuidTest()
        {
            BusinessEntity businessEntity = new BusinessEntity();
            businessEntity.guid = Const.IssuerId;
            xmlRequests =
                new XMLRequestsEnterpriseService(Enum.RequestTypesEnterpriseService.getEnterpriseByGuidRequest);
            xmlRequests.GetEnterpriseByGuid("10041f9f-a376-4328-8e58-53b6725cb3b2"); //Guid.Parse(Const.IssuerId) //c566587f-39fc-4c58-8d97-ad99173b6d6b
            answer = XMLResponces.GetResponce(xmlRequests, url);
            Assert.AreNotEqual(answer, null, "GetEnterpriseByGuid() answer not null");
            Assert.AreNotEqual(answer.GetElementsByTagName("dt:enterprise"), null, "GetEnterpriseByGuid() correctly");
        }

        [TestMethod]
        public void GetEnterpriseByUuidTest()
        {
            BusinessEntity businessEntity = new BusinessEntity();
            businessEntity.guid = Const.IssuerId;
            xmlRequests =
                new XMLRequestsEnterpriseService(Enum.RequestTypesEnterpriseService.getEnterpriseByUuidRequest);
            xmlRequests.GetEnterpriseByUuid("8db48fc9-411d-40b1-9153-eccf4fbb711d"); //Guid.Parse(Const.IssuerId)
            answer = XMLResponces.GetResponce(xmlRequests, url);
            Assert.AreNotEqual(answer, null, "GetEnterpriseByUuid() answer not null");
            Assert.AreNotEqual(answer.GetElementsByTagName("dt:enterprise"), null, "GetEnterpriseByUuid() correctly");
        }
    }
}
