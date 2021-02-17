using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using CreateUser;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.BusinessObjects.Users;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.ComponentModel;
using EllieMae.Encompass.Configuration;
using Newtonsoft.Json;
using Excel = Microsoft.Office.Interop.Excel;
using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.RemotingServices;
using EllieMae.EMLite.WebServices;
using EllieMae.EMLite.Common;

namespace UserCreate
{
    [Plugin]
    public class CreateUser
    {
        private static string acctExec = "PersonaPullJSON.json";
        private static PersonaDB file;
        private static string PersName = null;
        private static string PersAddName = null;
        private static string OrgFolder = null;
        public static string Group = null;
        public static string Access = null;
        public static string PersonaName = null;
        public static PersonaDB CDO => file ?? DownloadCDO();
        public static EllieMae.Encompass.Configuration.EncompassSettings SetIt;

        public CreateUser()
        {
            EncompassApplication.Login += EncompassApplication_Login;
        }

        private void EncompassApplication_Login(object sender, EventArgs e)
        {

            EncompassApplication.LoanOpened += EncompassApplication_LoanOpened;

        }

        private static PersonaDB DownloadCDO()
        {
            file = JsonConvert.DeserializeObject<PersonaDB>(Encoding.UTF8.GetString(EncompassApplication.Session.DataExchange.GetCustomDataObject(acctExec).Data));
            return file;

        }
        private void EncompassApplication_LoanOpened(object sender, EventArgs e)
        {
            EncompassApplication.CurrentLoan.FieldChange += CurrentLoan_FieldChange;
        }


        private void CurrentLoan_FieldChange(object source, EllieMae.Encompass.BusinessObjects.Loans.FieldChangeEventArgs e)
        {
            //string oFieldVal = EncompassApplication.CurrentLoan.Fields["CX.CREATEUSER.TEST"].Value.ToString();
            //string oSet = "GSE Services";
            //var oSettings =  EllieMae.EMLite.Common.ServicesMapping.GetServiceSetting(oSet);
            //int show;
           

            
            
            if (e.FieldID == "CX.ADMIN.CREATE.USER")
            {
                DownloadCDO();
                matchThePersona();
               
                MessageBox.Show("All Users have been created Successfully");
                
            }
            //if (e.FieldID == "CX.CREATEUSER.ADDPERSONA")
            //{
                //DownloadCDO();
                //AssignPersona();

                //MessageBox.Show("Additions Complete");
            //}
            //if (e.FieldID == "CX.CREATEUSER.TEST")
            //{

                //var oDirectory = SetIt.EncompassProgramDirectory;
                //show = oDirectory[0];
                //MessageBox.Show(show.ToString());

           // }           


        }

        private void matchThePersona()
        {
            String currentUser = EncompassApplication.Session.UserID;

            Excel.Application userApp = new Excel.Application();
            Excel.Workbook userWorkbook = userApp.Workbooks.Open(@"\\ftwfs02\Groups\POS\Encompass Support\New User Plugin\UserTest16.xlsx");
            Excel._Worksheet userWorksheet = userWorkbook.Sheets[1];
            Excel.Range userRange = userWorksheet.UsedRange;
            int rCnt = 1;
            int cCnt = 1;
            int rowCount = userRange.Rows.Count;
            int colCount = userRange.Columns.Count;
            string newFirst;
            string newLast;
            string userID;
            string stopGO = "2";
            string eMail;
            string taskNo;
            string mgrID;
            string procTitle;
            string pWord = "P@ssword1";

            for (rCnt = 2; rCnt <= rowCount; rCnt++)
            {
                PersName = (string)(userWorksheet.Cells[rCnt, cCnt].Value2);
                cCnt++;
                userID = (string)(userWorksheet.Cells[rCnt, cCnt].Value2);
                cCnt++;
                newFirst = (string)(userWorksheet.Cells[rCnt, cCnt].Value2);
                cCnt++;
                newLast = (string)(userWorksheet.Cells[rCnt, cCnt].Value2);
                cCnt++;
                eMail = (string)(userWorksheet.Cells[rCnt, cCnt].Value2);
                cCnt++;
                taskNo = (string)(userWorksheet.Cells[rCnt, cCnt].Value2);
                cCnt++;
                mgrID = (string)(userWorksheet.Cells[rCnt, cCnt].Value2);
                cCnt++;
                procTitle = (string)(userWorksheet.Cells[rCnt, cCnt].Value2);
                cCnt = 1;

                if (userID.Length >= 17)
                {
                    MessageBox.Show("User " + userID + " length is longer than 16 characters.  The program will stop and all Users after this user will not be created.  Please adjust.");
                }

                List<PersonaPull> pSelect = CDO.PersonaPull.ToList();
                List<string> persColl = new List<string>();
                
                List<string> mngrGroup = new List<string>();                
                UserGroupList userGroups = new UserGroupList();
                List<string> userGroup = new List<string>();
                UserGroupList mngrGroups = new UserGroupList();
                PersonaList listPers = new PersonaList();

                foreach (PersonaPull item in pSelect)
                {
                    if (PersName != "")
                    {
                        if (item.PersName == PersName)
                        {
                            OrgFolder = item.OrgFolder;
                            Access = item.Access;
                            userGroup = item.Group.ToList();
                            persColl = item.PersonaName.ToList();

                            foreach (string perColl in persColl)
                            {
                                listPers.Add(EncompassApplication.Session.Users.Personas.GetPersonaByName(perColl));
                            }
                        }
                    }
                }

                OrganizationList orgs = EncompassApplication.Session.Organizations.GetAllOrganizations();
                foreach (Organization org in orgs)
                {
                    UserList orgUsers = org.GetUsers();
                    foreach (User useID in orgUsers)
                    {
                        
                        if (useID.ID.ToString() == userID.ToLower())
                            
                        {
                            EncompassApplication.CurrentLoan.Fields["CX.ADMIN.CREATE.USERS.DUP"].Value = EncompassApplication.CurrentLoan.Fields["CX.ADMIN.CREATE.USERS.DUP"].Value + "\n" + userID + "\n";
                            goto stop;
                        }
                    }

                    if (OrgFolder == "Processing Teams" ^ OrgFolder == "Closing Teams" ^ OrgFolder == "Underwriting Teams")
                    {
                       OrganizationList procOrgs = EncompassApplication.Session.Organizations.GetAllOrganizations();
                        foreach (Organization procOrg in procOrgs)
                        {
                            if (mgrID != "DavidJobes" ^ mgrID != "MichaelTrainor")
                            {

                                if (mgrID.ToLower() == procOrg.Description)
                                {
                                    OrgFolder = procOrg.ToString();
                                }
                            }
                            if (mgrID == "MichaelTrainor" && OrgFolder == "Processing Teams") 
                            {
                                if (procTitle == "Digital Risk")
                                {
                                    OrgFolder = "Digital Risk Processors";
                                }
                                else if (procTitle == "Maxwell") 
                                {
                                    OrgFolder = "Maxwell Processors";
                                }
                                else if (procTitle == "Accenture")
                                {
                                    OrgFolder = "Accenture Processors";
                                }
                                else
                                {
                                    OrgFolder = "Digital Risk Processors";
                                }

                            }
                            else if (mgrID == "DavidJobes" && OrgFolder == "Processing Teams")
                            {
                                OrgFolder = "CD Operations";
                            }
                            else if (mgrID == "MichaelTrainor" && OrgFolder == "Closing Teams")
                            {
                                OrgFolder = "Sourcepoint Closers";
                            }

                        }

                    }
                }

                OrganizationList userOrgs = EncompassApplication.Session.Organizations.GetOrganizationsByName(OrgFolder);
                foreach (Organization userOrg in userOrgs)
                {
                    User newUser = userOrg.CreateUser(userID, pWord, listPers);
                    newUser.FirstName = newFirst;
                    newUser.LastName = newLast;
                    newUser.Email = eMail;
                    newUser.WorkingFolder = "My Pipeline";
                    SubordinateLoanAccessRight readWrite = SubordinateLoanAccessRight.ReadWrite;
                    SubordinateLoanAccessRight readOnly = SubordinateLoanAccessRight.ReadOnly;
                    PeerLoanAccessRight readWritep = PeerLoanAccessRight.ReadWrite;
                    PeerLoanAccessRight readNo = PeerLoanAccessRight.None;

                    if (Access == "Full")
                    {
                        newUser.SubordinateLoanAccessRight = readWrite;
                        newUser.PeerLoanAccessRight = readWritep;
                    }
                    else
                    {
                        newUser.SubordinateLoanAccessRight = readOnly;
                        newUser.PeerLoanAccessRight = readNo;
                    }
                    CCSiteInfo ccSite = new CCSiteInfo();
                    ccSite.UseParentInfo = false;
                    ccSite.UseParentInfo = true;
                    //newUser.Refresh();
                    newUser.Commit();
                    listPers.Clear();
                    
                    foreach (string uGroup in userGroup)
                    {
                       userGroups.Add(EncompassApplication.Session.Users.Groups.GetGroupByName(uGroup));
                    }
                    
                    foreach (UserGroup addUser in userGroups)
                    {
                        User idGet = EncompassApplication.Session.Users.GetUser(userID);
                        addUser.AddUser(idGet);
                    }
                    
                    string glass = "TPO Wholesale Manager - Glass";
                    string cullen = "TPO Wholesale Manager - Cullen";
                    string murphy = "TPO Wholesale Manager - Murphy";
                    mngrGroup.Add(glass);
                    mngrGroup.Add(cullen);
                    mngrGroup.Add(murphy);

                    foreach (string mGroup in mngrGroup)
                    {
                        mngrGroups.Add(EncompassApplication.Session.Users.Groups.GetGroupByName(mGroup));
                    }
                    foreach (UserGroup addMngr in mngrGroups)
                    {
                        User idMngr = EncompassApplication.Session.Users.GetUser(userID);

                        if (mgrID == "michaelcullen")
                        {
                            if (addMngr == EncompassApplication.Session.Users.Groups.GetGroupByName(cullen))
                            {
                                addMngr.AddUser(idMngr);
                            }
                        }
                        if (mgrID == "ronaldglass")
                        {
                            if (addMngr == EncompassApplication.Session.Users.Groups.GetGroupByName(glass))
                            {
                                addMngr.AddUser(idMngr);
                            }
                        }
                        if (mgrID == "glenmurphy")
                        {
                            if (addMngr == EncompassApplication.Session.Users.Groups.GetGroupByName(murphy))
                            {
                                addMngr.AddUser(idMngr);
                            }
                        }
                    }
                    
                    IOrganizationManager orgMgr = EllieMae.EMLite.RemotingServices.Session.OrganizationManager;
                    UserInfo userInfo = orgMgr.GetUser(userID);
                    userInfo.RequirePasswordChange = true;
                    userInfo.PersonaAccessComments = "Created via " + taskNo + " on " + DateTime.Now;
                    orgMgr.UpdateUser(userInfo);

                }
            stop:
                stopGO = "1";
            }

            userWorkbook.Close(true, null, null);
            userApp.Quit();
            Marshal.ReleaseComObject(userWorksheet);
            Marshal.ReleaseComObject(userWorkbook);
            Marshal.ReleaseComObject(userApp);
            
        }

        private void AssignPersona()
        {
            Excel.Application userApp = new Excel.Application();
            Excel.Workbook userWorkbook = userApp.Workbooks.Open(@"C:\Users\christopherclemency\Documents\PersonaAssign.xlsx");
            Excel._Worksheet userWorksheet = userWorkbook.Sheets[1];
            Excel.Range userRange = userWorksheet.UsedRange;
            int rCnt = 1;
            int cCnt = 1;
            int rowCount = userRange.Rows.Count;
            int colCount = userRange.Columns.Count;
            string userID;
           
            

            

            for (rCnt = 2; rCnt <= rowCount; rCnt++)
            {
                PersAddName = (string)(userWorksheet.Cells[rCnt, cCnt].Value2);
                cCnt++;
                userID = (string)(userWorksheet.Cells[rCnt, cCnt].Value2);
                cCnt = 1;
                OrganizationList orgs = EncompassApplication.Session.Organizations.GetAllOrganizations();
                foreach (Organization org in orgs)
                {
                    UserList orgUsers = org.GetUsers();
                    foreach (User useID in orgUsers)
                    {

                        if (useID.ID.ToString() == userID.ToLower())

                        {
                                                        
                            useID.Personas.Add(EncompassApplication.Session.Users.Personas.GetPersonaByName(PersAddName));
                            useID.Commit();
                                
                        }
                        else if (PersAddName == "")
                        {
                            break;
                        }
                    }
                }
            }
            
            userWorkbook.Close(true, null, null);
            userApp.Quit();
            Marshal.ReleaseComObject(userWorksheet);
            Marshal.ReleaseComObject(userWorkbook);
            Marshal.ReleaseComObject(userApp);
        }

    }
}
