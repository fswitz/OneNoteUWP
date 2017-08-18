using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace OneNoteUWP
{
    internal class DAL
    {
    }

    public class Patient
    {
        [SQLite.AutoIncrement, SQLite.PrimaryKey]
        public int OID { get; set; }

        public string PatCode { get; set; }
        public string ProviderCode { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public string Title { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string MedRegNum { get; set; }
        public string ProviderNum { get; set; }
        public string Comments { get; set; }
        public DateTime BirthDate { get; set; }

        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
        public string InsuredPlanName { get; set; }
        public string Copay { get; set; }

        public string OneNoteURL { get; set; }
        public string NotebookId { get; set; }
        public string ChartsId { get; set; }
        public string NotebookName { get; set; }
    }

    //public class Templates
    //{
    //    public string TemplateName { get; set; }
    //    public string TemplateFile { get; set; }
    //    public string Category { get; set; }
    //    public int SizeFactor { get; set; }

    //}

    public class Templates
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string TemplateName { get; set; }

        public string TemplateFile { get; set; }
        public string Category { get; set; }
        public int SizeFactor { get; set; }
    }

    //{
    //  "isDefault":false,"userRole":"Owner","isShared":false,"sectionsUrl":"https://www.onenote.com/api/v1.0/notebooks/0-50FCF51311CEE68D!4441/sections","sectionGroupsUrl":"https://www.onenote.com/api/v1.0/notebooks/0-50FCF51311CEE68D!4441/sectionGroups","links":{
    //    "oneNoteClientUrl":{
    //      "href":"onenote:https://d.docs.live.net/50fcf51311cee68d/Documents/0803Notebook"
    //    },"oneNoteWebUrl":{
    //      "href":"https://onedrive.live.com/redir.aspx?cid=50fcf51311cee68d&page=edit&resid=50FCF51311CEE68D!4441"
    //    }
    //  },"name":"0803Notebook","self":"https://www.onenote.com/api/v1.0/notebooks/0-50FCF51311CEE68D!4441","createdBy":"Fritz Switzer","lastModifiedBy":"Fritz Switzer","lastModifiedTime":"2014-08-03T15:09:49.66Z","id":"0-50FCF51311CEE68D!4441","createdTime":"2014-08-03T15:07:39.537Z"
    //}

    [DataContract]
    public class Notebooks
    {
        [DataMember]
        public bool isDefault { get; set; }

        [DataMember]
        public string userRole { get; set; }

        [DataMember]
        public string isShared { get; set; }

        [DataMember]
        public string sectionsUrl { get; set; }

        [DataMember]
        public string sectionGroupsUrl { get; set; }

        [DataMember]
        public string oneNoteWebUrl { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string self { get; set; }

        [DataMember]
        public string createdBy { get; set; }

        [DataMember]
        public string lastModifiedBy { get; set; }

        [DataMember]
        public string lastModifiedTime { get; set; }

        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string createdTime { get; set; }
    }

    [DataContract]
    internal class Person
    {
        [DataMember]
        public string forename { get; set; }

        [DataMember]
        public string surname { get; set; }

        [DataMember]
        public int age { get; set; }
    }

    public class OneNoteClientUrl
    {
        public string href { get; set; }
    }

    public class OneNoteWebUrl
    {
        public string href { get; set; }
    }

    //public class Links
    //{
    //    public OneNoteClientUrl oneNoteClientUrl { get; set; }
    //    public OneNoteWebUrl oneNoteWebUrl { get; set; }
    //}

    public class RootObject
    {
        public bool isDefault { get; set; }
        public string userRole { get; set; }
        public bool isShared { get; set; }
        public string sectionsUrl { get; set; }
        public string sectionGroupsUrl { get; set; }
        public Links links { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string self { get; set; }
        public string createdBy { get; set; }
        public string lastModifiedBy { get; set; }
        public string createdTime { get; set; }
        public string lastModifiedTime { get; set; }
    }

    public class OneNoteItems
    {
        [SQLite.AutoIncrement, SQLite.PrimaryKey]
        public int OID { get; set; }

        public bool isDefault { get; set; }
        public string userRole { get; set; }
        public bool isShared { get; set; }
        public string sectionsUrl { get; set; }
        public string sectionGroupsUrl { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string self { get; set; }
        public string createdBy { get; set; }
        public string lastModifiedBy { get; set; }
        public DateTime createdTime { get; set; }
        public DateTime lastModifiedTime { get; set; }
    }

    public class Sections
    {
        [SQLite.AutoIncrement, SQLite.PrimaryKey]
        public int OID { get; set; }

        public bool isDefault { get; set; }
        public string pagesUrl { get; set; }
        public string name { get; set; }
        public string self { get; set; }
        public string createdBy { get; set; }
        public string lastModifiedBy { get; set; }
        public DateTime lastModifiedTime { get; set; }
        public string id { get; set; }
        public DateTime createdTime { get; set; }
        public string notebook { get; set; }
        public string notebookid { get; set; }
    }

    public class Globals
    {
        public static string PatCode = "";
        public static string PatientLastName = "";
        public static string PatientFirstName = "";
        public static string PatientAddress1 = "";
        public static string PatientCity = "";
        public static string PatientState = "";
        public static string PatientPostCode = "";
        public static string PatientDOB = System.DateTime.Now.ToString();
        public static string PatientID = "";
        public static string PatientComments = "";
        public static string PatientSubjective = "";
        public static string PatientObjective = "";
        public static string PatientAssessment1 = "";
        public static string PatientAssessment2 = "";
        public static string PatientAssessment3 = "";
        public static string PatientAssessment4 = "";
        public static string PatientPlan = "";
        public static string PatientChiefComplaint = "";
        public static string PatientDxCode = "";
        public static string RecordKey = "";

        // public static string PatientProvider = "Dr." + Environment.UserName.ToString();
        public static string PatientDateOfVisit = System.DateTime.Now.ToString();

        public static string CurrentTemplate = "";
        public static string CurrentTemplateCategory = "";
        public static string CurrentTemplateLocation = "";
        public static int CurrentSizeFactor = 2;
        public static string ProviderName = "";
        public static string ProviderAddress = "";
        public static string ProviderCity = "";
        public static string ProviderState = "";
        public static string ProviderPostCode = "";
        public static string ProviderPhone = "";
        public static string ProviderDEA = "";
        public static string ProviderMedicalIDNumber = "";
        public static string ProviderTitle = "";
        public static int ProviderOID = 0;

        public static bool InactiveProbList;
        public static bool InactiveMedList;

        public static bool initialPatientScreen = true;

        public static string Medication = "";
        public static string Dispense = "";
        public static string Sig = "";
        public static string RefillCount = "";
        public static string Generic = "";

        public static string Medication1 = "";
        public static string Dispense1 = "";
        public static string Sig1 = "";
        public static string RefillCount1 = "";
        public static string Generic1 = "";

        public static string Medication2 = "";
        public static string Dispense2 = "";
        public static string Sig2 = "";
        public static string RefillCount2 = "";
        public static string Generic2 = "";

        public static string Medication3 = "";
        public static string Dispense3 = "";
        public static string Sig3 = "";
        public static string RefillCount3 = "";
        public static string Generic3 = "";

        public static string Medication4 = "";
        public static string Dispense4 = "";
        public static string Sig4 = "";
        public static string RefillCount4 = "";
        public static string Generic4 = "";

        public static bool NYScript;

        public static string Diagnosis1 = "";
        public static string Diagnosis2 = "";
        public static string Diagnosis3 = "";
        public static string SQLRemoteLocation = "";

        public static bool AuthorizationSuceess = false;

        public static string CurentNotebookName = "";
        public static string CurrentNotebookId = "";
        public static string CurrentChartsId = "";

        public static bool NotebookActive = false;
        public static bool ChartsActive = false;

        public static string TempResponse = "";
        public static string NotebookTempResponse = "";
        public static string ChartsTempResponse = "";
    }
}