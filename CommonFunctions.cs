using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using AjaxControlToolkit;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using iTextSharpeText = iTextSharp.text;
using System.Text;

namespace OM
{

    public class CommonFunctions : Transaction
    {
        DataSet dset = new DataSet();
        GetDataSet objTrans = new GetDataSet();
        Transaction.QueryOutPut ObjQry = new Transaction.QueryOutPut();

        public static string ServerName, UserName = "", Password = "", ToolTip;

        public static string ErrorMsg = "", Session_Error_Message = "", Application_Error_Message = "", ConStrService = "";
        public static string PDFErrorMsg = "";
        string strDropdownErrorMessage = "</br></br> Procedure returned 0 rows. ";

        public static string CRNClientName = "Community Resource Network";
        public static string CRNSubClientName = "Community Resource Network";
        public static string CRNShortClientName = "CRN";
        public static string CRNEmailUS = "", CRNPhone = "", CRNFax = "";
        public static string CRNSMTPEmail = "";
        public static string CRNEnvironment = "(Development)";
        public static bool IsDisplayCRNEnvironment = true;
        public static string PageTitle = "Community Resource Network";
        public static string ReportTitleName = "Community Resource Network";
        public static string ReportGraphTitleName = "Community Resource Network";
        public static string NWPDFPath = @"~/Newsletter/";
        public static long DateFormatType = 1;
        public static long PhoneFormatType = 1;
        public static bool IsValidatePhoneNo = true;
        public long EmailSendType = 1;
        public static string EventUploadPDFPath = @"~/EventUploadFiles/";
        public static string ResourceUploadPDFPath = @"~/ResourceUploadFiles/";

        public static string DeleteHeaderPopuup = "Delete Request - Email Requester";
        public static string DeclineHeaderPopuup = "Decline Request - Email Requester";

        public static string EventDeleteHeaderPopuup = "Delete Event - Email Organizer";
        public static string EventDeclineHeaderPopuup = "Decline Event - Email Organizer";
        public CommonFunctions()
            : base()
        {

        }

        public static void SetDefaultValue()
        {
            try
            {
                string strClientName = "";
                #region Backbone Agency Details
                CommonFunctions ObjFunction = new CommonFunctions();
                DataTable dt = new DBProcedures().Proc_Backbone_Organization_Details_Select_By_Backbone_Organization_ID(1);
                strClientName = dt.Rows[0].ItemArray[0].ToString();
                CRNSubClientName = dt.Rows[0].ItemArray[1].ToString();
                CRNShortClientName = dt.Rows[0].ItemArray[2].ToString();
                CRNEmailUS = dt.Rows[0].ItemArray[3].ToString();
                CRNPhone = dt.Rows[0].ItemArray[4].ToString();
                CRNFax = dt.Rows[0].ItemArray[5].ToString();
                #endregion
                //strClientName = DBCRN_System_Value.GetCRN_System_Value(158).Char_Value;
                //CRNSubClientName = DBCRN_System_Value.GetCRN_System_Value(159).Char_Value;
                //CRNShortClientName = DBCRN_System_Value.GetCRN_System_Value(166).Char_Value;
                CRNEnvironment = DBCRN_System_Value.GetCRN_System_Value(160).Char_Value;
                //CRNEmailUS = DBCRN_System_Value.GetCRN_System_Value(105).Char_Value;
                CRNSMTPEmail = DBCRN_System_Value.GetCRN_System_Value(153).Char_Value;
                IsDisplayCRNEnvironment = DBCRN_System_Value.GetCRN_System_Value(161).Bit_Value;
                if (IsDisplayCRNEnvironment == true)
                {
                    PageTitle = strClientName + " " + CRNSubClientName + " " + CRNEnvironment;
                    ReportTitleName = strClientName + " " + CRNSubClientName + " " + CRNEnvironment;
                    ReportGraphTitleName = strClientName + " " + CRNSubClientName + " " + CRNEnvironment;
                }
                else
                {
                    PageTitle = strClientName + " " + CRNSubClientName;
                    ReportTitleName = strClientName + " " + CRNSubClientName;
                    ReportGraphTitleName = strClientName + " " + CRNSubClientName;
                }
                CRNClientName = strClientName;

            }
            catch (Exception exc)
            { }
        }
        public static string DateFormat
        {
            get
            {
                if (DateFormatType == 1)
                    return "MM/dd/yyyy";
                else
                    return "dd-MMM-yyyy";
            }
        }
        public static string NullDate
        {
            get
            {
                if (DateFormatType == 1)
                    return "01/01/1900";
                else
                    return "01-Jan-1900";
            }
        }
        public static string DateExpression
        {
            get
            {
                if (DateFormatType == 1)
                    return @"^(((0?[1-9]|1[012])/(0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])/(29|30)|(0?[13578]|1[02])/31)/(19|[2-9]\d)\d{2}|0?2/29/((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00)))$";
                else
                    return @"^(?:((31-(Jan|Mar|May|Jul|Aug|Oct|Dec))|((([0-2]\d)|30)-(Jan|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec))|(([01]\d|2[0-8])-Feb))|(29-Feb(?=-((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)))))-((1[6-9]|[2-9]\d)\d{2})$";
                //return @"^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[13-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$";
            }
        }

        #region Fill DropDownList
        public void FillCom(DropDownList Cb)
        {
            //Only Select value is add to Dropdowlist Control
            Cb.Items.Clear();
            Cb.Items.Add(" ------ Select ------ ");
            Cb.Items[0].Value = System.Convert.ToString(0);
        }
        public void FillCombo(DropDownList Cb, string Str)
        {
            //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            // Dim text As String
            //long cnt = 0;
            try
            {
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;
                Cb.Items.Clear();
                Cb.Items.Add(" ------ Select ------ ");
                Cb.Items[0].Value = System.Convert.ToString(0);
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if (dset.Tables[0].Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            Cb.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            Cb.Items[Cb.Items.Count - 1].Attributes.Add("style", "red");
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }
                    Cb.SelectedIndex = 0;
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }

        }
        public void FillCombo(DropDownList Cb, long Item_Type_ID)
        {
            //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            // Dim text As String
            //long cnt = 0;
            try
            {
                string Str = "Exec Proc_Code_Value_Item_Code_Select_Status " + Item_Type_ID + "";
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;
                Cb.Items.Clear();
                Cb.Items.Add(" ------ Select ------ ");
                Cb.Items[0].Value = System.Convert.ToString(0);
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if (dset.Tables[0].Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            Cb.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            Cb.Items[Cb.Items.Count - 1].Attributes.Add("style", "red");
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }
                    Cb.SelectedIndex = 0;
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }

        }

        public void FillCheckBoxList(CheckBoxList ChkList, long Item_Type_ID)
        {
            try
            {
                string Str = "Exec Proc_Code_Value_Item_Code_Select_Status " + Item_Type_ID + "";
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;
                if (dset.Tables[0].Rows.Count > 0)
                {
                    //Clear();
                    ChkList.DataBind();
                    ChkList.DataSource = dset.Tables[0].DefaultView;
                    ChkList.DataValueField = Convert.ToString(dset.Tables[0].Columns[0]);
                    ChkList.DataTextField = Convert.ToString(dset.Tables[0].Columns[1]);
                    ChkList.DataBind();


                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //the qurey record is filled in datagrid 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }

        public void FillRadioButtonList(RadioButtonList RBList, long Item_Type_ID)
        {
            try
            {
                string Str = "Exec Proc_Code_Value_Item_Code_Select_Status " + Item_Type_ID + "";
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;
                if (dset.Tables[0].Rows.Count > 0)
                {
                    //Clear();
                    RBList.DataBind();
                    RBList.DataSource = dset.Tables[0].DefaultView;
                    RBList.DataValueField = Convert.ToString(dset.Tables[0].Columns[0]);
                    RBList.DataTextField = Convert.ToString(dset.Tables[0].Columns[1]);
                    RBList.DataBind();

                    if (RBList.Items.Count > 0)
                        RBList.SelectedIndex = 0;
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //the qurey record is filled in datagrid 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }
        public void FillRadioButtonList(RadioButtonList RBList, DataTable dt)
        {
            try
            {

                int i = 0;
                if (dt.Rows.Count > 0)
                {
                    //Clear();
                    RBList.DataBind();
                    RBList.DataSource = dt.DefaultView;
                    RBList.DataValueField = Convert.ToString(dt.Columns[0]);
                    RBList.DataTextField = Convert.ToString(dt.Columns[1]);
                    RBList.DataBind();

                    if (RBList.Items.Count > 0)
                        RBList.SelectedIndex = 0;
                }

                //the qurey record is filled in datagrid 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }

        public void FillRadioButtonList(RadioButtonList RBList, long Item_Type_ID, string NewName)
        {
            try
            {
                string Str = "Exec Proc_Code_Value_Item_Code_Select_Status " + Item_Type_ID + "";
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;
                if (dset.Tables[0].Rows.Count > 0)
                {
                    //Clear();
                    RBList.DataBind();
                    RBList.DataSource = dset.Tables[0].DefaultView;
                    RBList.DataValueField = Convert.ToString(dset.Tables[0].Columns[0]);
                    RBList.DataTextField = Convert.ToString(dset.Tables[0].Columns[1]);
                    RBList.DataBind();

                    RBList.Items.Add(NewName);
                    RBList.Items[RBList.Items.Count - 1].Value = System.Convert.ToString(-1);

                    if (RBList.Items.Count > 0)
                        RBList.SelectedIndex = 0;
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //the qurey record is filled in datagrid 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }
        public void FillComboOrg(DropDownList Cb, long Status_ID)
        {
            //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            // Dim text As String
            //long cnt = 0;
            try
            {
                string Str = "Exec Proc_Organizations_Select_By_Organization_Status_ID " + Status_ID + ",null";
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;
                Cb.Items.Clear();
                Cb.Items.Add(" ------ Select ------ ");
                Cb.Items[0].Value = System.Convert.ToString(0);
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if (dset.Tables[0].Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            Cb.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            Cb.Items[Cb.Items.Count - 1].Attributes.Add("style", "red");
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }
                    Cb.SelectedIndex = 0;
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }

        }
        public void FillCombOrg(DropDownList Cb, long Status_ID)
        {
            //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            // Dim text As String
            //long cnt = 0;
            try
            {
                string Str = "Exec Proc_Organizations_Select_By_Organization_Status_ID " + Status_ID + ",null";
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;
                Cb.Items.Clear();
                Cb.Items.Add(" ------ Select ------ ");
                Cb.Items[0].Value = System.Convert.ToString(0);
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if (dset.Tables[0].Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            Cb.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            Cb.Items[Cb.Items.Count - 1].Attributes.Add("style", "red");
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }
                    Cb.SelectedIndex = 0;
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }

        }
        public void FillComboOrgWithOrgRole(DropDownList Cb, long Status_ID)
        {
            //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            // Dim text As String
            //long cnt = 0;
            try
            {
                string Str = "Exec Proc_Organizations_Select_By_Organization_With_Role " + Status_ID;
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;
                Cb.Items.Clear();
                Cb.Items.Add(" ------ Select ------ ");
                Cb.Items[0].Value = System.Convert.ToString(0);
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if (dset.Tables[0].Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            Cb.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            Cb.Items[Cb.Items.Count - 1].Attributes.Add("style", "red");
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }
                    Cb.SelectedIndex = 0;
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }

        }
        public void FillListboxOrg(ListBox lst, long Status_ID)
        {
            //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            // Dim text As String
            //long cnt = 0;
            try
            {
                string Str = "Exec Proc_Organizations_Select_By_Organization_Status_ID " + Status_ID + ",null";
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;
                lst.Items.Clear();
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if (dset.Tables[0].Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            lst.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            lst.Items[lst.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            lst.Items[lst.Items.Count - 1].Attributes.Add("style", "red");
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }

                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }

        }

        public void FillListboxOrgText(ListBox lst, long Status_ID)
        {
            //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            // Dim text As String
            //long cnt = 0;
            try
            {
                string Str = "Exec Proc_Organizations_Select_By_Organization_Status_ID " + Status_ID + ",null";
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;
                lst.Items.Clear();
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if (dset.Tables[0].Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            lst.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            lst.Items[lst.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][1]);
                            lst.Items[lst.Items.Count - 1].Attributes.Add("style", "red");
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }

                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }

        }

        public void FillListbox(ListBox lst, long Item_Type_ID)
        {
            //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            // Dim text As String
            //long cnt = 0;
            try
            {
                string Str = "Exec Proc_Code_Value_Item_Code_Select_Status " + Item_Type_ID + "";
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;
                lst.Items.Clear();
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if (dset.Tables[0].Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            lst.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            lst.Items[lst.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            lst.Items[lst.Items.Count - 1].Attributes.Add("style", "red");
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }

                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }

        }

        public void FillListboxText(ListBox lst, DataTable dt)
        {
            try
            {

                int i = 0;
                lst.Items.Clear();
                if (dt.Rows.Count > 0)
                {
                    while (i <= dt.Rows.Count - 1)
                    {
                        if (dt.Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            lst.Items.Add(System.Convert.ToString(dt.Rows[i][1]));
                            lst.Items[lst.Items.Count - 1].Value = System.Convert.ToString(dt.Rows[i][1] + "$$@@");

                            i = i + 1;
                        }
                    }
                }
                //else
                // ExceptioMessage.ShowMessage(strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }

        }
        public void FillListboxTextWithVal(ListBox lst, DataTable dt)
        {
            try
            {

                int i = 0;
                lst.Items.Clear();
                if (dt.Rows.Count > 0)
                {
                    while (i <= dt.Rows.Count - 1)
                    {
                        if (dt.Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            lst.Items.Add(System.Convert.ToString(dt.Rows[i][1]));
                            lst.Items[lst.Items.Count - 1].Value = System.Convert.ToString(dt.Rows[i][0]);

                            i = i + 1;
                        }
                    }
                }
                //else
                //    ExceptioMessage.ShowMessage(strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }

        }
        public void FillListboxText(ListBox lst, long Item_Type_ID)
        {
            //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            // Dim text As String
            //long cnt = 0;
            try
            {
                string Str = "Exec Proc_Code_Value_Item_Code_Select_Status " + Item_Type_ID + "";
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;
                lst.Items.Clear();
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if (dset.Tables[0].Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            lst.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            lst.Items[lst.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][1]);
                            lst.Items[lst.Items.Count - 1].Attributes.Add("style", "red");
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }

                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }

        }
        public void FillListbox(ListBox lst, string Str)
        {
            //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            // Dim text As String
            //long cnt = 0;
            try
            {
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;
                lst.Items.Clear();

                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if (dset.Tables[0].Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            lst.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            lst.Items[lst.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            lst.Items[lst.Items.Count - 1].Attributes.Add("style", "red");
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }

                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }

        }

        public void FillListboxText(ListBox lst, string Str)
        {
            //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            // Dim text As String
            //long cnt = 0;
            try
            {
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;
                lst.Items.Clear();

                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if (dset.Tables[0].Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            lst.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            lst.Items[lst.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][1]);
                            lst.Items[lst.Items.Count - 1].Attributes.Add("style", "red");
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }

                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }

        }
        public void FillCombo(DropDownList Cb, DataTable dt)
        {
            try
            {
                int i = 0;

                Cb.Items.Clear();
                Cb.Items.Add(" ------ Select ------ ");
                Cb.Items[0].Value = System.Convert.ToString(0);

                while (i <= dt.Rows.Count - 1)
                {
                    if (dt.Rows[i].IsNull(1) == true)
                    {
                        i = i + 1;
                    }
                    else
                    {
                        //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                        Cb.Items.Add(System.Convert.ToString(dt.Rows[i][1]));
                        Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dt.Rows[i][0]);
                        Cb.Items[Cb.Items.Count - 1].Attributes.Add("style", "red");
                        i = i + 1;
                    }

                }
                Cb.SelectedIndex = 0;
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }

        public void FillComb(DropDownList Cb, string Str)
        {
            try
            {
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 

                //long cnt = 0;
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;

                Cb.Items.Clear();
                //Cb.Items.Add(" ------ Select ------ ")
                //Cb.Items.Item(0).Value = 0
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if ((dset.Tables[0].Rows[i].IsNull(1)) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            Cb.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }

        public void FillComb(DropDownList Cb, string Str, string NewName)
        {
            try
            {
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 

                //long cnt = 0;
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;

                Cb.Items.Clear();
                //Cb.Items.Add(" ------ Select ------ ")
                //Cb.Items.Item(0).Value = 0
                Cb.Items.Add(NewName);
                Cb.Items[0].Value = System.Convert.ToString(-1);
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if ((dset.Tables[0].Rows[i].IsNull(1)) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            Cb.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }
        public void FillComb(DropDownList Cb, long Item_Type_ID)
        {
            try
            {
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
                string Str = "Exec Proc_Code_Value_Item_Code_Select_Status " + Item_Type_ID + "";
                //long cnt = 0;
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;

                Cb.Items.Clear();
                //Cb.Items.Add(" ------ Select ------ ")
                //Cb.Items.Item(0).Value = 0
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if ((dset.Tables[0].Rows[i].IsNull(1)) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            Cb.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }

        public void FillComb(DropDownList Cb, long Item_Type_ID, string NewName)
        {
            try
            {
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
                string Str = "Exec Proc_Code_Value_Item_Code_Select_Status " + Item_Type_ID + "";
                //long cnt = 0;
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;

                Cb.Items.Clear();
                //Cb.Items.Add(" ------ Select ------ ")
                //Cb.Items.Item(0).Value = 0
                Cb.Items.Add(NewName);
                Cb.Items[0].Value = System.Convert.ToString(-1);
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if ((dset.Tables[0].Rows[i].IsNull(1)) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            Cb.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }
        public void FillComb(DropDownList Cb, string Item_Type_ID, int Type)
        {
            try
            {
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
                string Str = "Exec Proc_Code_Value_Item_Code_Select_By_Item_Type_ID '" + Item_Type_ID + "'";
                //long cnt = 0;
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;

                Cb.Items.Clear();
                //Cb.Items.Add(" ------ Select ------ ")
                //Cb.Items.Item(0).Value = 0
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if ((dset.Tables[0].Rows[i].IsNull(1)) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            Cb.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }

        public void FillCombo(DropDownList Cb, string Item_Type_ID, int Type)
        {
            try
            {
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
                string Str = "Exec Proc_Code_Value_Item_Code_Select_By_Item_Type_ID '" + Item_Type_ID + "'";
                //long cnt = 0;
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;

                Cb.Items.Clear();
                Cb.Items.Add(" ------ Select ------ ");
                Cb.Items[0].Value = System.Convert.ToString(0);
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if ((dset.Tables[0].Rows[i].IsNull(1)) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            Cb.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }
        public void FillComb(DropDownList Cb, DataTable dt)
        {
            try
            {
                int i = 0;

                Cb.Items.Clear();
                //Cb.Items.Add(" ------ Select ------ ");
                //Cb.Items[0].Value = System.Convert.ToString(0);

                while (i <= dt.Rows.Count - 1)
                {
                    if (dt.Rows[i].IsNull(1) == true)
                    {
                        i = i + 1;
                    }
                    else
                    {
                        //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                        Cb.Items.Add(System.Convert.ToString(dt.Rows[i][1]));
                        Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dt.Rows[i][0]);
                        Cb.Items[Cb.Items.Count - 1].Attributes.Add("style", "red");
                        i = i + 1;
                    }

                }
                Cb.SelectedIndex = 0;
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }
        public void FillCombo(DropDownList Cb, string Str, string NewName)
        {
            try
            {
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 

                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;

                Cb.Items.Clear();
                Cb.Items.Add(" ------ Select ------ ");
                Cb.Items[0].Value = System.Convert.ToString(0);
                Cb.Items.Add(NewName);
                Cb.Items[1].Value = System.Convert.ToString(-1);
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if (dset.Tables[0].Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))
                            Cb.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;
                        }
                    }
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }
        public void FillCombo(DropDownList Cb, long Item_Type_ID, string NewName)
        {
            try
            {
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
                string Str = "Exec Proc_Code_Value_Item_Code_Select_Status " + Item_Type_ID + "";
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;

                Cb.Items.Clear();
                Cb.Items.Add(" ------ Select ------ ");
                Cb.Items[0].Value = System.Convert.ToString(0);
                Cb.Items.Add(NewName);
                Cb.Items[1].Value = System.Convert.ToString(-1);
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if (dset.Tables[0].Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))
                            Cb.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;
                        }
                    }
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }
        public void FillCombo(DropDownList Cb, string Item_Type_ID, string NewName, int type)
        {
            try
            {
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
                string Str = "Exec Proc_Code_Value_Item_Code_Select_By_Item_Type_ID '" + Item_Type_ID + "'";
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;

                Cb.Items.Clear();
                if (type == 1)
                {
                    Cb.Items.Add(" ------ Select ------ ");
                    Cb.Items[0].Value = System.Convert.ToString(0);
                    Cb.Items.Add(NewName);
                    Cb.Items[1].Value = System.Convert.ToString(-1);
                }
                else if (type == 2)
                {
                    Cb.Items.Add(NewName);
                    Cb.Items[0].Value = System.Convert.ToString(-1);
                }
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if (dset.Tables[0].Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))
                            Cb.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;
                        }
                    }
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }
        public void FillCombo(string NewName, DropDownList Cb, string Str)
        {
            try
            {
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 

                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;

                Cb.Items.Clear();
                Cb.Items.Add(NewName);
                Cb.Items[0].Value = System.Convert.ToString(0);
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if (dset.Tables[0].Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))
                            Cb.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;
                        }
                    }
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }
        public void FillCombo(string NewName, DropDownList Cb, long Item_Type_ID)
        {
            try
            {
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
                string Str = "Exec Proc_Code_Value_Item_Code_Select_Status " + Item_Type_ID + "";
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;

                Cb.Items.Clear();
                Cb.Items.Add(NewName);
                Cb.Items[0].Value = System.Convert.ToString(-1);
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if (dset.Tables[0].Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))
                            Cb.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;
                        }
                    }
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }
        public void FillMonth(DropDownList Cb)
        {
            try
            {
                string strMonth = "";
                for (int i = 1; i <= 12; i++)
                {
                    if (i == 1) strMonth = "Jan";
                    else if (i == 2) strMonth = "Feb";
                    else if (i == 3) strMonth = "Mar";
                    else if (i == 4) strMonth = "Apr";
                    else if (i == 5) strMonth = "May";
                    else if (i == 6) strMonth = "Jun";
                    else if (i == 7) strMonth = "Jul";
                    else if (i == 8) strMonth = "Aug";
                    else if (i == 9) strMonth = "Sep";
                    else if (i == 10) strMonth = "Oct";
                    else if (i == 11) strMonth = "Nov";
                    else if (i == 12) strMonth = "Dec";


                    if (i < 10)
                    {
                        Cb.Items.Add("0" + i + "-" + strMonth);
                        Cb.Items[Cb.Items.Count - 1].Value = "0" + i;
                    }
                    else
                    {
                        Cb.Items.Add(i + "-" + strMonth);
                        Cb.Items[Cb.Items.Count - 1].Value = i.ToString();
                    }

                }
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }

        public string GetMonthValue(int MonVal)
        {
            string strMonth = "";
            try
            {
                if (MonVal < 10)
                {
                    strMonth = "0" + MonVal;
                }
                else
                {
                    strMonth = MonVal.ToString();
                }

            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
            return strMonth;
        }
        public void FillYear(DropDownList Cb, int startYear, int endyear)
        {
            try
            {
                for (int i = startYear; i <= endyear; i++)
                {
                    Cb.Items.Add(i.ToString());
                    Cb.Items[Cb.Items.Count - 1].Value = i.ToString().Substring(2);
                }
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }
        public void FillDays(DropDownList Cb)
        {
            try
            {
                for (int i = 1; i <= 31; i++)
                {
                    Cb.Items.Add(i.ToString());
                    Cb.Items[Cb.Items.Count - 1].Value = i.ToString();
                }
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }
        public void FillComboClientLOC(DropDownList Cb)
        {
            //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            // Dim text As String
            //long cnt = 0;
            try
            {
                string Str = "Exec Proc_Client_Location_Select_Location_ID_For_Active_Status ";
                dset = objTrans.FillDset("NewTable", Str, DBConnection.GetConnection());
                int i = 0;
                Cb.Items.Clear();
                Cb.Items.Add(" ------ Select ------ ");
                Cb.Items[0].Value = System.Convert.ToString(0);
                if (dset.Tables[0].Rows.Count > 0)
                {
                    while (i <= dset.Tables[0].Rows.Count - 1)
                    {
                        if (dset.Tables[0].Rows[i].IsNull(1) == true)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            //Lst.Items.Add(dset.Tables(0).Rows(i).Item(0))

                            Cb.Items.Add(System.Convert.ToString(dset.Tables[0].Rows[i][1]));
                            Cb.Items[Cb.Items.Count - 1].Value = System.Convert.ToString(dset.Tables[0].Rows[i][0]);
                            Cb.Items[Cb.Items.Count - 1].Attributes.Add("style", "red");
                            //If CultureName <> "en-US" Then
                            //    Cb.Items.Item(Cb.Items.Count - 1).Value = Rm1.GetString(Cb.Items.Item(Cb.Items.Count - 1).Value, c)
                            //End If
                            i = i + 1;


                        }

                    }
                    Cb.SelectedIndex = 0;
                }
                else
                    ExceptioMessage.ShowMessage(Str + strDropdownErrorMessage);
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }

        }

        public DataTable FillComb(string Str, string NewName)
        {
            DataTable dt = new DataTable();
            try
            {
                //The Fillcombo is used for filling of dropdownlist the qurey code is filled in value propery and name in text property 

                //long cnt = 0;
                dset = objTrans.FillDset("Table", Str, DBConnection.GetConnection());

                dt = dset.Tables[0];
                DataRow dr = dt.NewRow();
                dr[0] = -1;
                dr[1] = NewName;
                dt.Rows.InsertAt(dr, 0);

            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
            return dt;
        }
        #endregion

        #region Mail Send related Methods
        public bool SendMail(string Body, string EmailTo, string strSubject)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

            Body = Body.Replace("&lt;", "<").Replace("&gt;", ">");

            string SMTPUserName = DBCRN_System_Value.GetCRN_System_Value(148).Char_Value;// GetConfigValue("EmailFrom", 2);
            string SMTPPassword = DBCRN_System_Value.GetCRN_System_Value(149).Char_Value; //GetConfigValue("EmailPassword", 2);
            string SMTPHost = DBCRN_System_Value.GetCRN_System_Value(150).Char_Value;
            int SMTPPort = Convert.ToInt32(DBCRN_System_Value.GetCRN_System_Value(151).Char_Value);
            bool SSLFlag = DBCRN_System_Value.GetCRN_System_Value(152).Bit_Value;// false;
            string SMTPFromEmail = DBCRN_System_Value.GetCRN_System_Value(153).Char_Value;

            try
            {
                SmtpClient smtp = new SmtpClient
                {
                    Host = SMTPHost,//GetConfigValue("EmailHost", 2),// "smtp8.gmail.com",
                    Port = SMTPPort,//Convert.ToInt32(GetConfigValue("EmailPort", 2)),
                    EnableSsl = SSLFlag,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = true,
                    Credentials = new System.Net.NetworkCredential(SMTPUserName, SMTPPassword),
                    Timeout = 3600000
                };
                MailMessage message = new MailMessage();
                message.From = new MailAddress(SMTPFromEmail);
                message.To.Add(EmailTo);
                string ToEmail = DBCRN_System_Value.GetCRN_System_Value(129).Char_Value;
                if (ToEmail != "")
                    message.To.Add(ToEmail);
                else message.To.Add(SMTPFromEmail);

                message.Subject = strSubject;
                message.IsBodyHtml = true;
                message.Body = Body;

                smtp.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                return false;
            }
        }

        public bool SendMail(string Body, string EmailTo, string strSubject, EmailIDCollection emailIDs)
        {
            EmailGroupCollection emailGroup = SetMailID(EmailTo, emailIDs);
            return SendMailGroup(Body, strSubject, emailGroup);
        }

        private EmailGroupCollection SetMailID(string EmailTo, EmailIDCollection emailIDsMain)
        {
            EmailGroupCollection emailGroup = new EmailGroupCollection();
            try
            {
                long cnt = 0, AllowCount = 35;
                if (EmailSendType == 2)
                    AllowCount = DBCRN_System_Value.GetCRN_System_Value_Service(128).Numeric_Value;
                else
                    AllowCount = DBCRN_System_Value.GetCRN_System_Value(128).Numeric_Value;
                if (AllowCount == 0) AllowCount = 35;

                EmailIDCollection emailIDs = new EmailIDCollection();

                if (EmailTo != "")
                {
                    emailIDs.Add(EmailTo);
                    cnt += 1;
                }


                for (int i = 0; i < emailIDsMain.Count; i++)
                {
                    if (cnt == AllowCount)
                    {
                        emailGroup.Add(emailIDs);
                        emailIDs = new EmailIDCollection();
                        cnt = 0;
                    }
                    if (emailIDsMain[i].ToString() != null)
                    {
                        emailIDs.Add(emailIDsMain[i].ToString());
                        cnt += 1;
                    }
                }

                emailGroup.Add(emailIDs);


                return emailGroup;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                return emailGroup;
            }
        }
        public bool SendMail(string Body, string EmailTo, string strSubject, string EmailCC, string[] EmailBCC)
        {
            string[] EmailBCCOther = new string[0];
            EmailGroupCollection emailGroup = SetMailID(EmailTo, EmailCC, EmailBCC, EmailBCCOther);
            return SendMailGroup(Body, strSubject, emailGroup);



        }

        public bool SendMail(string Body, string EmailTo, string strSubject, string EmailCC, string[] EmailBCC, string[] EmailBCCOther)
        {
            EmailGroupCollection emailGroup = SetMailID(EmailTo, EmailCC, EmailBCC, EmailBCCOther);
            return SendMailGroup(Body, strSubject, emailGroup);


        }
        private bool SendMailGroup(string Body, string strSubject, EmailGroupCollection emailGroup)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            Body = Body.Replace("&lt;", "<").Replace("&gt;", ">");

            string SMTPUserName, SMTPPassword, SMTPHost, SMTPFromEmail;
            int SMTPPort, SMTPDelayTime = 1, SMTPPerSecondLimit = 20, emailSendCount = 0;
            bool SSLFlag;
            if (EmailSendType == 2)
            {
                SMTPUserName = DBCRN_System_Value.GetCRN_System_Value_Service(148).Char_Value;// GetConfigValue("EmailFrom", 2);
                SMTPPassword = DBCRN_System_Value.GetCRN_System_Value_Service(149).Char_Value; //GetConfigValue("EmailPassword", 2);
                SMTPHost = DBCRN_System_Value.GetCRN_System_Value_Service(150).Char_Value;
                SMTPPort = Convert.ToInt32(DBCRN_System_Value.GetCRN_System_Value_Service(151).Char_Value);
                SSLFlag = DBCRN_System_Value.GetCRN_System_Value_Service(152).Bit_Value;// false;
                SMTPFromEmail = DBCRN_System_Value.GetCRN_System_Value_Service(153).Char_Value;
                SMTPPort = Convert.ToInt32(DBCRN_System_Value.GetCRN_System_Value_Service(151).Char_Value);
                SMTPDelayTime = Convert.ToInt32(DBCRN_System_Value.GetCRN_System_Value_Service(164).Numeric_Value);
                SMTPPerSecondLimit = Convert.ToInt32(DBCRN_System_Value.GetCRN_System_Value_Service(165).Numeric_Value);
            }
            else
            {
                SMTPUserName = DBCRN_System_Value.GetCRN_System_Value(148).Char_Value;// GetConfigValue("EmailFrom", 2);
                SMTPPassword = DBCRN_System_Value.GetCRN_System_Value(149).Char_Value; //GetConfigValue("EmailPassword", 2);
                SMTPHost = DBCRN_System_Value.GetCRN_System_Value(150).Char_Value;
                SMTPPort = Convert.ToInt32(DBCRN_System_Value.GetCRN_System_Value(151).Char_Value);
                SSLFlag = DBCRN_System_Value.GetCRN_System_Value(152).Bit_Value;// false;
                SMTPFromEmail = DBCRN_System_Value.GetCRN_System_Value(153).Char_Value;
                SMTPDelayTime = Convert.ToInt32(DBCRN_System_Value.GetCRN_System_Value(164).Numeric_Value);
                SMTPPerSecondLimit = Convert.ToInt32(DBCRN_System_Value.GetCRN_System_Value(165).Numeric_Value);
            }


            try
            {
                for (int rowindex = 0; rowindex < emailGroup.Count; rowindex++)
                {
                    SmtpClient smtp = new SmtpClient
                    {
                        Host = SMTPHost,//GetConfigValue("EmailHost", 2),// "smtp8.gmail.com",
                        Port = SMTPPort,//Convert.ToInt32(GetConfigValue("EmailPort", 2)),
                        EnableSsl = SSLFlag,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = true,
                        Credentials = new System.Net.NetworkCredential(SMTPUserName, SMTPPassword),
                        Timeout = 3600000
                    };
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(SMTPFromEmail);
                    string ToEmail;
                    if (EmailSendType == 2)
                        ToEmail = DBCRN_System_Value.GetCRN_System_Value_Service(129).Char_Value;
                    else
                        ToEmail = DBCRN_System_Value.GetCRN_System_Value(129).Char_Value;
                    if (ToEmail != "")
                        message.To.Add(ToEmail);
                    else message.To.Add(SMTPFromEmail);

                    EmailIDCollection emailIDs = emailGroup[rowindex];// new EmailIDCollection();

                    for (int i = 0; i < emailIDs.Count; i++)
                    {
                        message.Bcc.Add(emailIDs[i]);
                    }

                    message.Subject = strSubject;
                    message.IsBodyHtml = true;
                    message.Body = Body;

                    smtp.Send(message);

                    if (EmailSendType == 2)//Only use for Web service Mail functinality
                    {
                        if (emailSendCount >= SMTPPerSecondLimit)
                        {
                            System.Threading.Thread.Sleep(SMTPDelayTime * 1000);
                            emailSendCount = 0;
                        }

                        emailSendCount += emailGroup[rowindex].Count;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                //ExceptioMessage.ShowMessage(ex.Message);
                //ErrorMsg = ex.Message;
                return false;
            }
        }
        private bool SendMailGroupOld(string Body, string strSubject, EmailGroupCollection emailGroup)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

            Body = Body.Replace("&lt;", "<").Replace("&gt;", ">");

            string SMTPUserName, SMTPPassword, SMTPHost, SMTPFromEmail;
            int SMTPPort;
            bool SSLFlag;
            if (EmailSendType == 2)
            {
                SMTPUserName = DBCRN_System_Value.GetCRN_System_Value_Service(148).Char_Value;// GetConfigValue("EmailFrom", 2);
                SMTPPassword = DBCRN_System_Value.GetCRN_System_Value_Service(149).Char_Value; //GetConfigValue("EmailPassword", 2);
                SMTPHost = DBCRN_System_Value.GetCRN_System_Value_Service(150).Char_Value;
                SMTPPort = Convert.ToInt32(DBCRN_System_Value.GetCRN_System_Value_Service(151).Char_Value);
                SSLFlag = DBCRN_System_Value.GetCRN_System_Value_Service(152).Bit_Value;// false;
                SMTPFromEmail = DBCRN_System_Value.GetCRN_System_Value_Service(153).Char_Value;
            }
            else
            {
                SMTPUserName = DBCRN_System_Value.GetCRN_System_Value(148).Char_Value;// GetConfigValue("EmailFrom", 2);
                SMTPPassword = DBCRN_System_Value.GetCRN_System_Value(149).Char_Value; //GetConfigValue("EmailPassword", 2);
                SMTPHost = DBCRN_System_Value.GetCRN_System_Value(150).Char_Value;
                SMTPPort = Convert.ToInt32(DBCRN_System_Value.GetCRN_System_Value(151).Char_Value);
                SSLFlag = DBCRN_System_Value.GetCRN_System_Value(152).Bit_Value;// false;
                SMTPFromEmail = DBCRN_System_Value.GetCRN_System_Value(153).Char_Value;
            }


            try
            {
                for (int rowindex = 0; rowindex < emailGroup.Count; rowindex++)
                {
                    SmtpClient smtp = new SmtpClient
                    {
                        Host = SMTPHost,//GetConfigValue("EmailHost", 2),// "smtp8.gmail.com",
                        Port = SMTPPort,//Convert.ToInt32(GetConfigValue("EmailPort", 2)),
                        EnableSsl = SSLFlag,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = true,
                        Credentials = new System.Net.NetworkCredential(SMTPUserName, SMTPPassword),
                        Timeout = 3600000
                    };
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(SMTPFromEmail);
                    string ToEmail;
                    if (EmailSendType == 2)
                        ToEmail = DBCRN_System_Value.GetCRN_System_Value_Service(129).Char_Value;
                    else
                        ToEmail = DBCRN_System_Value.GetCRN_System_Value(129).Char_Value;
                    if (ToEmail != "")
                        message.To.Add(ToEmail);
                    else message.To.Add(SMTPFromEmail);

                    EmailIDCollection emailIDs = emailGroup[rowindex];// new EmailIDCollection();

                    for (int i = 0; i < emailIDs.Count; i++)
                    {
                        message.Bcc.Add(emailIDs[i]);
                    }

                    message.Subject = strSubject;
                    message.IsBodyHtml = true;
                    message.Body = Body;

                    smtp.Send(message);

                    if (EmailSendType == 2)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                //ExceptioMessage.ShowMessage(ex.Message);
                //ErrorMsg = ex.Message;
                return false;
            }
        }


        private EmailGroupCollection SetMailID(string EmailTo, string EmailCC, string[] EmailBCC, string[] EmailBCCOther)
        {
            EmailGroupCollection emailGroup = new EmailGroupCollection();
            try
            {
                long cnt = 0, AllowCount = 35;
                AllowCount = DBCRN_System_Value.GetCRN_System_Value(128).Numeric_Value;
                if (AllowCount == 0) AllowCount = 35;

                EmailIDCollection emailIDs = new EmailIDCollection();

                if (EmailTo != "")
                {
                    emailIDs.Add(EmailTo);
                    cnt += 1;
                }
                if (EmailCC != "")
                {
                    emailIDs.Add(EmailCC);
                    cnt += 1;
                }

                for (int i = 0; i < EmailBCC.Length; i++)
                {
                    if (cnt == AllowCount)
                    {
                        emailGroup.Add(emailIDs);
                        emailIDs = new EmailIDCollection();
                        cnt = 0;
                    }
                    if (EmailBCC[i] != null)
                    {
                        emailIDs.Add(EmailBCC[i]);
                        cnt += 1;
                    }
                }

                for (int i = 0; i < EmailBCCOther.Length; i++)
                {
                    if (cnt == AllowCount)
                    {
                        emailGroup.Add(emailIDs);
                        emailIDs = new EmailIDCollection();
                        cnt = 0;
                    }
                    if (EmailBCCOther[i] != null)
                    {
                        emailIDs.Add(EmailBCCOther[i]);
                        cnt += 1;
                    }
                }
                emailGroup.Add(emailIDs);


                return emailGroup;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                return emailGroup;
            }
        }

        public string GetJSONData(string Body, string EmailTo, string strSubject, EmailIDCollection EmailIDs)
        {
            Mail_Details mailDetails = new Mail_Details();
            mailDetails.Body = Body.Replace("\"", "$$$@@@").Replace("'", "$$$###"); ;
            mailDetails.EmailTo = EmailTo;
            mailDetails.Subject = strSubject.Replace("\"", "$$$@@@").Replace("'", "$$$###"); ;
            mailDetails.EmailIDs = EmailIDs;
            string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(mailDetails);
            DBStoreValue.SetSession("CRNMailDetails", new Security().psEncrypt(strJson));
            strJson = "0";
            return strJson;
        }
        public string GetJSONDataList(List<Mail_Details> mailDetailsList)
        {
            string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(mailDetailsList);
            DBStoreValue.SetSession("CRNMailDetailsList", new Security().psEncrypt(strJson));
            strJson = "0";
            return strJson;
        }

        public void AddMailDetailsList(string Body, string EmailTo, string strSubject, EmailIDCollection EmailIDs, List<Mail_Details> mailDetailsList)
        {
            Mail_Details mailDetails = new Mail_Details();
            mailDetails.Body = Body.Replace("\"", "$$$@@@").Replace("'", "$$$###"); ;
            mailDetails.EmailTo = EmailTo;
            mailDetails.Subject = strSubject.Replace("\"", "$$$@@@").Replace("'", "$$$###"); ;
            mailDetails.EmailIDs = EmailIDs;
            mailDetailsList.Add(mailDetails);
        }

        #endregion

        public DataView GetDataView(string sql)
        {
            System.Data.SqlClient.SqlConnection Con = new System.Data.SqlClient.SqlConnection(DBConnection.GetConnection());
            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(sql, Con);
            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
                return ds.Tables[(0)].DefaultView;
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
                return null;
                //throw;
            }
            finally
            {
                Con.Close();
            }

        }
        public DataView GetDataViewService(string sql, string ConStr)
        {
            System.Data.SqlClient.SqlConnection Con = new System.Data.SqlClient.SqlConnection(ConStr);
            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(sql, Con);
            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
                return ds.Tables[(0)].DefaultView;
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
                return null;
                //throw;
            }
            finally
            {
                Con.Close();
            }

        }
        public DataSet GetDataSet(string sql, string ConStr)
        {
            System.Data.SqlClient.SqlConnection Con = new System.Data.SqlClient.SqlConnection(ConStr);
            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(sql, Con);
            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch { throw; }
            finally
            {
                Con.Close();
            }
            return ds;
        }

        public void RequestDownloadImage(long Request_Detail_ID)
        {
            try
            {
                DBRequest dbRequest = new DBRequest();
                Request_Detail mRequestDetail = new Request_Detail();
                mRequestDetail = dbRequest.Proc_Request_Details_Select_By_Request_Detail_ID(Request_Detail_ID);

                HttpContext.Current.Response.ContentType = "image/jpeg";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + mRequestDetail.Request_File_Name);
                HttpContext.Current.Response.BinaryWrite(mRequestDetail.Request_Data);
                HttpContext.Current.Response.Flush();

            }
            catch (Exception exc)
            {
                HttpContext.Current.Response.Redirect("../ErrorPage.aspx?ErrorType=1");
            }
        }

        public void RequestDownloadImageByService(long Request_Detail_ID)
        {
            try
            {
                DBRequest dbRequest = new DBRequest();
                Request_Detail mRequestDetail = new Request_Detail();
                mRequestDetail = dbRequest.Proc_Request_Details_Select_By_Request_Detail_ID_By_Service(Request_Detail_ID);
                HttpContext.Current.Response.Headers.Clear();
                HttpContext.Current.Response.ClearHeaders();
                HttpContext.Current.Response.Charset = "";
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.ContentType = "image/jpeg";
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + mRequestDetail.Request_File_Name);
                HttpContext.Current.Response.BinaryWrite(mRequestDetail.Request_Data);

                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();

            }
            catch (Exception exc)
            {
                //HttpContext.Current.Response.Redirect("../ErrorPage.aspx?ErrorType=1");
            }
        }

        public void EventDownloadImage(long Event_Detail_ID, int typeOfFile)
        {
            try
            {
                DBEvent dbRequest = new DBEvent();
                Event_Detail mEventDetail = new Event_Detail();
                mEventDetail = dbRequest.Proc_Event_Detail_Select_By_Event_Detail_ID(Event_Detail_ID);

                if (typeOfFile == 1)
                {
                    HttpContext.Current.Response.ClearHeaders();
                    //HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + mEventDetail.Event_QRCode_File_Name);
                    HttpContext.Current.Response.ContentType = "image/jpeg";
                    //HttpContext.Current.Response.BinaryWrite(mEventDetail.Event_QRCode_Data);
                    HttpContext.Current.Response.Flush();
                }
                else if (typeOfFile == 2)
                {
                    string FilePath = HttpContext.Current.Server.MapPath("~/PDFFiles/" + mEventDetail.Event_File_Name);
                    System.IO.FileStream output = new FileStream(FilePath, FileMode.Create);
                    //output.Write(mEventDetail.Event_File_Data, 0, mEventDetail.Event_File_Data.Length);
                    output.Close();
                    DBStoreValue.SetSession("PDFFileAttachName", mEventDetail.Event_File_Name);

                    //HttpContext.Current.Response.ContentType = "application/pdf";
                    //HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + mEventDetail.Event_File_Name);
                    //HttpContext.Current.Response.OutputStream.Write(mEventDetail.Event_File_Data, 0, mEventDetail.Event_File_Data.Length);
                    ////HttpContext.Current.Response.BinaryWrite(mEventDetail.Event_File_Data);
                    //HttpContext.Current.Response.Flush();
                }

            }
            catch (Exception exc)
            {
                HttpContext.Current.Response.Redirect("../ErrorPage.aspx?ErrorType=1");
            }
        }

        public void SetHelp(ImageButton imgHelp, int AppMessageID, string divName)
        {
            CRN_Application_Message CRNAppMessage = DBCRN_Application_Message.GetMessage(AppMessageID);
            //Literal1.Text = CRNAppMessage.Message_Text;
            imgHelp.Attributes.Add("onmouseover", "ShowHelp('" + imgHelp.ClientID + "','" + divName + "','" + CRNAppMessage.Message_Header + "','" + CRNAppMessage.Message_Text + "'," + CRNAppMessage.Message_Width + "," + CRNAppMessage.Message_Height + ");");
            imgHelp.Attributes.Add("onmouseout", "CloseHelp('" + divName + "');");
            imgHelp.Attributes.Add("onclick", "return false;");
        }
        public void SetHelp(ImageButton imgHelp, CRN_Application_Message CRNAppMessage, string divName)
        {

            //Literal1.Text = CRNAppMessage.Message_Text;
            imgHelp.Attributes.Add("onmouseover", "ShowHelp('" + imgHelp.ClientID + "','" + divName + "','" + CRNAppMessage.Message_Header + "','" + CRNAppMessage.Message_Text + "'," + CRNAppMessage.Message_Width + "," + CRNAppMessage.Message_Height + ");");
            imgHelp.Attributes.Add("onmouseout", "CloseHelp('" + divName + "');");
            imgHelp.Attributes.Add("onclick", "return false;");
        }

        public string GetConfigValue(string Value, int type)
        {
            if (type == 1)
                return ConfigurationManager.ConnectionStrings[Value].ConnectionString;
            else if (type == 2)
                return ConfigurationManager.ConnectionStrings[new Security().psEncrypt(Value)].ConnectionString;
            else return "";
        }

        #region Common data Type Check related Methods
        public static bool GetAuthenticatePage(string strPage)
        {
            int cnt = 0, cnt1 = 1; string PageName = "";
            for (int i = strPage.Length - 1; i >= 0; i--)
            {
                if (Convert.ToChar(strPage[i]) == 47) break;
                cnt++;
            }
            for (int i = strPage.Length - 1; i >= 0; i--)
            {
                if (Convert.ToChar(strPage[i]) == 46) break;
                cnt1++;
            }
            PageName = strPage.Substring(strPage.Length - cnt, cnt - cnt1);

            //=======Masters
            if (PageName == "AdminHome") return true;
            else if (PageName == "MemberHome") return true;
            else if (PageName == "index") return true;
            else if (PageName == "ApplicationForm") return true;
            else return false;
        }
        #endregion

        public void SetHighLight(LinkButton lnkbtn, string strSearch)
        {
            if (strSearch != "")
            {
                lnkbtn.Text = Regex.Replace(lnkbtn.Text, "<span class='divhighlight'>", delegate (Match match)
                {
                    return string.Format("$$##$$", match.Value);
                }, RegexOptions.IgnoreCase);

                lnkbtn.Text = Regex.Replace(lnkbtn.Text, "</span>", delegate (Match match)
                {
                    return string.Format("##$$##", match.Value);
                }, RegexOptions.IgnoreCase);

                lnkbtn.Text = Regex.Replace(lnkbtn.Text, strSearch, delegate (Match match)
                {
                    return string.Format("<span class='divhighlight'>{0}</span>", match.Value);
                }, RegexOptions.IgnoreCase);

                lnkbtn.Text = lnkbtn.Text.Replace("$$##$$", "<span class='divhighlight'>");
                lnkbtn.Text = lnkbtn.Text.Replace("##$$##", "</span>");
            }
        }
        public void SetHighLight(Label lbl, string strSearch)
        {
            if (strSearch != "")
            {
                lbl.Text = Regex.Replace(lbl.Text, "<span class='divhighlight'>", delegate (Match match)
                {
                    return string.Format("$$##$$", match.Value);
                }, RegexOptions.IgnoreCase);

                lbl.Text = Regex.Replace(lbl.Text, "</span>", delegate (Match match)
                {
                    return string.Format("##$$##", match.Value);
                }, RegexOptions.IgnoreCase);

                lbl.Text = Regex.Replace(lbl.Text, strSearch, delegate (Match match)
                {
                    return string.Format("<span class='divhighlight'>{0}</span>", match.Value);
                }, RegexOptions.IgnoreCase);

                lbl.Text = lbl.Text.Replace("$$##$$", "<span class='divhighlight'>");
                lbl.Text = lbl.Text.Replace("##$$##", "</span>");
            }
        }
        public void SetHighLight(HyperLink lbl, string strSearch)
        {
            if (strSearch != "")
            {
                lbl.Text = Regex.Replace(lbl.Text, "<span class='divhighlight'>", delegate (Match match)
                {
                    return string.Format("$$##$$", match.Value);
                }, RegexOptions.IgnoreCase);

                lbl.Text = Regex.Replace(lbl.Text, "</span>", delegate (Match match)
                {
                    return string.Format("##$$##", match.Value);
                }, RegexOptions.IgnoreCase);

                lbl.Text = Regex.Replace(lbl.Text, strSearch, delegate (Match match)
                {
                    return string.Format("<span class='divhighlight'>{0}</span>", match.Value);
                }, RegexOptions.IgnoreCase);

                lbl.Text = lbl.Text.Replace("$$##$$", "<span class='divhighlight'>");
                lbl.Text = lbl.Text.Replace("##$$##", "</span>");
            }
        }
        public string RemoveHighLight(string strValue)
        {
            //string strValue = lbl.Text;
            strValue = strValue.Replace("<span class='divhighlight'>", "");
            strValue = strValue.Replace("</span>", "");
            strValue = strValue.Replace(" ", "");
            return strValue;

        }
        public string GetPhoneFormatValue(string strPhoneNo)
        {
            string strActualNo = "";
            if (strPhoneNo != "")
            {
                if (PhoneFormatType == 1)
                    strActualNo = String.Format("{0:(###)###-####}", Convert.ToInt64(strPhoneNo));
                else if (PhoneFormatType == 2)
                    strActualNo = String.Format("{0:###-###-####}", Convert.ToInt64(strPhoneNo));
            }
            //strActualNo = "(" + strPhoneNo.Substring(0, 3) + ")" + strPhoneNo.Substring(3, 3) + "-" + strPhoneNo.Substring(6, 4);
            return strActualNo;
        }
        public void SetPhoneFormat(HiddenField hfd)
        {

            if (PhoneFormatType == 1)
                hfd.Value = "(999)999-9999";
            else if (PhoneFormatType == 2)
                hfd.Value = "999-999-9999";
        }

        public void SetPhoneValidations(Label lbl, RequiredFieldValidator rqf, CustomValidator CV)
        {
            lbl.Visible = IsValidatePhoneNo;
            rqf.Visible = IsValidatePhoneNo;
            CV.Visible = false;
        }

        public string GetExcelFormat()
        {
            string strFormat = " border='1' bgColor='#ffffff' " +
                "borderColor='#000000' cellSpacing='0' cellPadding='0' " +
                "style='font-size:10.0pt; font-family:Calibri; background:white;' ";
            return strFormat;
        }

        //public string GetCRNFunctionsValue(int ID)
        //{
        //    try
        //    {
        //        DataTable dt = GetDataView("Exec Get_CRNFunctions_Value " + ID + "").Table;
        //        string strValue = "";
        //        if (dt.Rows.Count > 0)
        //        {
        //            strValue = dt.Rows[0].ItemArray[2].ToString();
        //        }

        //        return strValue;
        //    }
        //    catch (Exception exc)
        //    {
        //        ExceptioMessage.ShowMessage(exc.Message);
        //        return "";
        //    }
        //}

        public int Is_Display_Dashboard
        {
            get
            {

                if (HttpContext.Current.Session["Is_Display_Dashboard"] == null)
                {
                    HttpContext.Current.Session["Is_Display_Dashboard"] = 0;
                }
                return (int)HttpContext.Current.Session["Is_Display_Dashboard"];
            }
            set
            {
                HttpContext.Current.Session["Is_Display_Dashboard"] = value;
            }
        }

        public void CheckWebsiteFile(object sender, Page page, bool IsRoot, int FileType)
        {
            try
            {
                string WebisiteFileName = "";// "Community_Resources_Network_Instructions.pdf";
                if (FileType == 1)
                    WebisiteFileName = "Community_Resources_Network_Instructions.pdf";
                else if (FileType == 2)
                    WebisiteFileName = "How_to_Become_a_Member.pdf";
                else if (FileType == 3)
                    WebisiteFileName = "CRN_Flyer.pdf";
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/PDFFiles/" + WebisiteFileName + "")) == false)
                {
                    CommonFunctions.PDFErrorMsg = WebisiteFileName + " file not found. " +
                        " Please close the application and contact CRN help desk.";

                    HttpContext.Current.Response.Redirect("~/ErrorPage.aspx?ErrorType=5&Allow=Yes", false);
                }
                else
                {
                    if (IsRoot == true)
                        ScriptManager.RegisterClientScriptBlock((sender as Control), page.GetType(), "", "javascript:window.open('PDFFiles/" + WebisiteFileName + "','_blank');", true);
                    else
                        ScriptManager.RegisterClientScriptBlock((sender as Control), page.GetType(), "", "javascript:window.open('../PDFFiles/" + WebisiteFileName + "','_blank');", true);
                }
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }
        public int CheckIsPrimary(DataList dtl, string strCheckboxName)
        {
            int index = -1;
            bool flag = false;
            foreach (DataListItem item in dtl.Items)
            {
                if (((CheckBox)item.FindControl(strCheckboxName)).Checked == true)
                {
                    index = item.ItemIndex;
                    flag = true;
                    break;
                }
            }
            if (flag == false)
            {
                if (dtl.Items.Count > 0)
                {
                    index = 0;
                    ((CheckBox)dtl.Items[0].FindControl(strCheckboxName)).Checked = true;
                }
            }
            return index;
        }
        public static string SetCode(string ID)
        {
            Security secure = new Security();

            return HttpUtility.HtmlEncode(secure.psEncrypt(ID.ToString()).Replace("+", "%2B"));
        }

        public long GetGridCode(string ID)
        {
            Security secure = new Security();

            return Convert.ToInt64(secure.psDecrypt(HttpUtility.HtmlDecode(DBStoreValue.GetStringParamsVal(ID).ToString())));
        }

        public string GetCode(string strValue)
        {
            Security secure = new Security();

            return secure.psDecrypt(HttpUtility.HtmlDecode(strValue));
        }

        public bool GuidTryParse(string s, out Guid result)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            Regex format = new Regex(
                "^[A-Fa-f0-9]{32}$|" +
                "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
                "^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$");
            Match match = format.Match(s);
            if (match.Success)
            {
                result = new Guid(s);
                return true;
            }
            else
            {
                result = Guid.Empty;
                return false;
            }
        }

        public void SetCSRFTokenValue(HiddenField hfID)
        {
            string CSRF_Token = System.Guid.NewGuid().ToString();
            HttpContext.Current.Session["CRN_CSRF_Token"] = CSRF_Token;
            hfID.Value = CSRF_Token.ToString();
        }

        public bool CheckCSRFTokenValue(HiddenField hfID)
        {
            bool flag = false;
            try
            {
                if (hfID.Value.ToString() == HttpContext.Current.Session["CRN_CSRF_Token"].ToString())
                {
                    HttpContext.Current.Session["CRN_CSRF_Token"] = 0;
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            catch
            {
                flag = false;
            }

            if (flag == false)
            {
                //if (HttpContext.Current.Request.UrlReferrer != null) //ToString().IndexOf("ShopInfo.aspx")>0
                HttpContext.Current.Response.Redirect("~/ErrorPage.aspx?ErrorType=6", false);
                //else
                //  HttpContext.Current.Response.Redirect("~/ErrorPage.aspx?ErrorType=5");
            }
            return flag;
        }

        public string SetReportName(string ReportName)
        {
            ReportName = ReportName + "_" + new Random().Next(99, 99999);
            return ReportName;
        }

        #region Report List box Methods
        public string GetListBoxTextValues(ListBox lstRight)
        {
            string strValue = "";
            for (int i = 0; i < lstRight.Items.Count; i++)
            {
                if (strValue == "")
                    strValue = "''" + lstRight.Items[i].Text + "''";
                else strValue += ",''" + lstRight.Items[i].Text + "''";
            }


            string strMainValues = strValue;

            return strMainValues;
        }

        public string GetListBoxValues(ListBox lstRight, DataTable dt)
        {
            string strValue = "";
            for (int i = 0; i < lstRight.Items.Count; i++)
            {
                if (strValue == "")
                    strValue = "'" + lstRight.Items[i].Value.Replace("$$@@", "").Replace("'", "''") + "'";
                else strValue += ",'" + lstRight.Items[i].Value.Replace("$$@@", "").Replace("'", "''") + "'";
            }


            string strMainValues = "";
            if (strValue != "")
            {
                DataRow[] dr = dt.Select(dt.Columns[1].ColumnName + " in (" + strValue + ")");

                if (dr.Length > 0)
                {
                    for (int i = 0; i < dr.Length; i++)
                    {
                        if (strMainValues == "")
                            strMainValues = dr[i].ItemArray[0].ToString();
                        else strMainValues += "," + dr[i].ItemArray[0].ToString();
                    }
                }
            }
            return strMainValues;
        }
        //public void FillListBox(ListBox lstBox)
        //{

        //    string rightSelectedItems = HttpContext.Current.Request.Form[lstBox.UniqueID];
        //    lstBox.Items.Clear();
        //    String[] spearator = { "$$@@," };
        //    if (!string.IsNullOrEmpty(rightSelectedItems))
        //    {
        //        foreach (string item in rightSelectedItems.Split(spearator, StringSplitOptions.None))
        //        {
        //            lstBox.Items.Add(item.Replace("$$@@", ""));
        //            lstBox.Items[lstBox.Items.Count - 1].Value = System.Convert.ToString(item.Replace("$$@@", "") + "$$@@");
        //        }
        //    }

        //}
        public void FillListBox(ListBox lstBox, HiddenField hf)
        {
            string rightSelectedItems = HttpContext.Current.Request.Form[lstBox.UniqueID];
            lstBox.Items.Clear();
            String[] spearator = { "$$@@," };
            if (!string.IsNullOrEmpty(rightSelectedItems))
            {
                foreach (string item in rightSelectedItems.Split(spearator, StringSplitOptions.None))
                {
                    lstBox.Items.Add(item.Replace("$$@@", ""));
                    lstBox.Items[lstBox.Items.Count - 1].Value = System.Convert.ToString(item.Replace("$$@@", "") + "$$@@");
                }
            }

            if (lstBox.Items.Count == 0)
            {
                rightSelectedItems = HttpContext.Current.Request.Form[hf.UniqueID];
                lstBox.Items.Clear();
                spearator[0] = "$$@@";
                if (!string.IsNullOrEmpty(rightSelectedItems))
                {
                    foreach (string item in rightSelectedItems.Split(spearator, StringSplitOptions.None))
                    {
                        if (item.Replace("$$@@", "") != "")
                        {
                            lstBox.Items.Add(item.Replace("$$@@", ""));
                            lstBox.Items[lstBox.Items.Count - 1].Value = System.Convert.ToString(item.Replace("$$@@", "") + "$$@@");
                        }
                    }

                }
            }
        }

        public void ClearRightListBox(CRNRTSystem.UC.UCListBox lstBox)
        {
            lstBox.RightList.Items.Clear();
            lstBox.ListItems.Value = "";
        }
        #endregion

        public void SetMenuName(string MenuName)
        {
            DBStoreValue.SetSession("MenuName", MenuName);
        }
        public string GetMenuName()
        {
            return DBStoreValue.GetSessionString("MenuName");
        }

        public void SetRequestExtendDate(GridViewRow row)
        {
            string strTitele = "Extend Request Due Date";
            string ReqTitle = "reqTitle:\"" + ((Label)row.FindControl("lblRequestTitle")).Text + "\"";
            string ReqDate = "regDate:\"" + Convert.ToDateTime(((Label)row.FindControl("lblCreatedby_Timestamp")).Text).ToString(DateFormat) + "\"";
            string DeadlineDate = "DeadlineDate:\"" + Convert.ToDateTime(((Label)row.FindControl("lblDeadline")).Text).ToString(DateFormat) + "\"";
            string ReqID = "RequestID:\"" + ((Label)row.FindControl("lblRequestID")).Text + "\"";
            string MemberName = "Requester:\"" + ((Label)row.FindControl("lblOrganization_Name")).Text + " - " + ((Label)row.FindControl("lblCreatedBy")).Text + "\"";

            ((LinkButton)row.FindControl("lnkbtnDeadline")).Attributes.Add("OnClick", "Open_Extended_Date('" + strTitele + "',{ " + ReqID + "," + MemberName + "," + ReqTitle + ", " + ReqDate + ", " + DeadlineDate + " });return false;");
        }

        public void SetRequestExtendDate(DataListItem item)
        {
            string strTitele = "Extend Request Due Date";
            string ReqTitle = "reqTitle:\"" + ((Label)item.FindControl("lblRequestTitle")).Text + "\"";
            string ReqDate = "regDate:\"" + Convert.ToDateTime(((Label)item.FindControl("lblCreatedby_Timestamp")).Text).ToString(DateFormat) + "\"";
            string DeadlineDate = "DeadlineDate:\"" + Convert.ToDateTime(((Label)item.FindControl("lblDeadline")).Text).ToString(DateFormat) + "\"";
            string ReqID = "RequestID:\"" + ((Label)item.FindControl("lblRequestID")).Text + "\"";
            string MemberName = "Requester:\"" + ((Label)item.FindControl("lblOrganization_Name")).Text + " - " + ((Label)item.FindControl("lblCreatedBy")).Text + "\"";

            ((LinkButton)item.FindControl("lnkbtnDeadline")).Attributes.Add("OnClick", "Open_Extended_Date('" + strTitele + "',{ " + ReqID + "," + MemberName + "," + ReqTitle + ", " + ReqDate + ", " + DeadlineDate + " });return false;");
        }
        public void SetEventExtendDate(GridViewRow row)
        {
            string strTitele = "Extend Event Due Date";
            string ReqTitle = "eventTitle:\"" + ((Label)row.FindControl("lblEventTitle")).Text + "\"";
            string ReqDate = "eventStartDate:\"" + Convert.ToDateTime(((Label)row.FindControl("lblStartDate")).Text).ToString(DateFormat) + "\"";
            string DeadlineDate = "evenEndDate:\"" + Convert.ToDateTime(((Label)row.FindControl("lblEndDate")).Text).ToString(DateFormat) + "\"";
            string ReqID = "EventID:\"" + ((Label)row.FindControl("lblEventID")).Text + "\"";
            string MemberName = "Organizer:\"" + ((Label)row.FindControl("lblOrganizerOrganization")).Text + " - " + ((Label)row.FindControl("lblOrganizer")).Text + "\"";

            ((LinkButton)row.FindControl("lnkbtnEndDate")).Attributes.Add("OnClick", "Open_Extended_Date('" + strTitele + "',{ " + ReqID + "," + MemberName + "," + ReqTitle + ", " + ReqDate + ", " + DeadlineDate + " });return false;");
        }
        public void SetRequestUpdateStatus(GridViewRow row)
        {
            //string strTitele = "Extend Request Due Date";
            string strPara = "{";
            strPara += "reqTitle:\"" + ((Label)row.FindControl("lblRequestTitle")).Text + "\",";
            strPara += "regDate:\"" + Convert.ToDateTime(((Label)row.FindControl("lblCreatedby_Timestamp")).Text).ToString(DateFormat) + "\",";
            strPara += "DeadlineDate:\"" + Convert.ToDateTime(((Label)row.FindControl("lblDeadline")).Text).ToString(DateFormat) + "\",";
            strPara += "RequestID:\"" + ((Label)row.FindControl("lblRequestID")).Text + "\",";
            strPara += "Requester:\"" + ((Label)row.FindControl("lblOrganization_Name")).Text + " - " + ((Label)row.FindControl("lblCreatedBy")).Text + "\",";
            strPara += "CurrentStatus:\"" + ((Label)row.FindControl("lblStatus")).Text + "\",";
            strPara += "CurrentStatusID:\"" + ((Label)row.FindControl("lblStatusCode")).Text + "\",";
            if (((Label)row.FindControl("lblFullFilledMemberName")).Text != "")
                strPara += "FFmember:\"" + ((Label)row.FindControl("lblFullFilledOrgName")).Text + " - " + ((Label)row.FindControl("lblFullFilledMemberName")).Text + "\",";
            else
                strPara += "FFmember:\" \",";
            strPara += "FFMemberID:\"" + ((Label)row.FindControl("lblFMemberID")).Text + "\"";
            strPara += "}";

            ((LinkButton)row.FindControl("lnkbtnUpdateRequestStatus")).Attributes.Add("OnClick", "Open_Request_Status(" + strPara + ");return false;");
            //((LinkButton)row.FindControl("lnkbtnUpdateRequestStatus")).Attributes.Add("OnClick", "Open_Request_Status({ " + ReqID + "," + MemberName + "," + ReqTitle + ", " + ReqDate + ", " + DeadlineDate + " });return false;");
        }

        public string GetCommunicationPopupTitle(int commType, string MemberName, string MemberEmail, string OrgName)
        {
            string popupTitle = "";

            if (commType == DB_Code_Value_Item_Code.Comm_Type_Request_Clarification)
            {
                //popupTitle = "Email Communication – Request Clarification Email - " + OrgName + " - " + MemberName + " (" + MemberEmail + ")";
                popupTitle = "Email Requester – " + OrgName + " - " + MemberName + " (" + MemberEmail + ")";
            }
            else if (commType == DB_Code_Value_Item_Code.Comm_Type_Thank_You_Note)
            {
                if (OrgName == "" || MemberName == "" || MemberEmail == "")
                    popupTitle = "Email Communication – Thank You Email";
                else
                    popupTitle = "Email Communication – Thank You Email for Fulfilled Request.";// + OrgName + " - " + MemberName + " (" + MemberEmail + ")";
            }
            return popupTitle;
        }

        public string GetCommunicationOnBehalf(int commType, string MemberName, string MemberEmail, string OrgName)
        {
            string popupTitle = "";

            if (commType == DB_Code_Value_Item_Code.Comm_Type_Request_Clarification)
            {
                //popupTitle = "" + OrgName + " - " + MemberName + " (" + MemberEmail + ")";
                popupTitle = "" + MemberName + " from " + OrgName;
            }
            else if (commType == DB_Code_Value_Item_Code.Comm_Type_Thank_You_Note)
            {
                //popupTitle = "" + OrgName + " - " + MemberName + " (" + MemberEmail + ")";
                popupTitle = "" + MemberName + " from " + OrgName;
            }
            return popupTitle;
        }

        public string GetEventCommunicationPopupTitle(int commType, string MemberName, string MemberEmail, string OrgName)
        {
            string popupTitle = "";

            if (commType == DB_Code_Value_Item_Code.Event_Comm_Type_Event_Clarification)
            {
                popupTitle = "Email Event Organizer – " + OrgName + " - " + MemberName + " (" + MemberEmail + ")";
            }
            else if (commType == DB_Code_Value_Item_Code.Event_Comm_Type_Thank_You_Note)
            {
                if (OrgName == "" || MemberName == "" || MemberEmail == "")
                    popupTitle = "Event Email Communication – Thank You Email";
                else
                    popupTitle = "Event Email Communication – Thank You Email - " + OrgName + " - " + MemberName + " (" + MemberEmail + ")";
            }
            return popupTitle;
        }
        public string GetEventCommunicationOnBehalf(int commType, string MemberName, string MemberEmail, string OrgName)
        {
            string popupTitle = "";

            if (commType == DB_Code_Value_Item_Code.Event_Comm_Type_Event_Clarification)
            {
                popupTitle = "" + OrgName + " - " + MemberName + " (" + MemberEmail + ")";
            }
            else if (commType == DB_Code_Value_Item_Code.Event_Comm_Type_Thank_You_Note)
            {
                popupTitle = "" + OrgName + " - " + MemberName + " (" + MemberEmail + ")";
            }
            return popupTitle;
        }

        public string GetSubscriptionCommunicationPopupTitle(string SubscriberName, string SubscriberNameEmail, string OrgName)
        {
            string popupTitle = "";


            popupTitle = "Email Subscriber – " + OrgName + " - " + SubscriberName + " (" + SubscriberNameEmail + ")";

            return popupTitle;
        }
        public void SetRequestCommunication(GridViewRow row)
        {
            string strPara = "{";
            strPara += "reqTitle:\"" + ((Label)row.FindControl("lblRequestTitle")).Text + "\",";
            strPara += "regDate:\"" + Convert.ToDateTime(((Label)row.FindControl("lblRequestDate")).Text).ToString(DateFormat) + "\",";
            //strPara += "reqDesc:\"" + ((Label)row.FindControl("lblRequestDesc")).Text + "\"";
            strPara += "CommMessage:\"" + ((Label)row.FindControl("lblRequestCommMessage")).Text + "\"";

            strPara += "}";

            ((LinkButton)row.FindControl("lnkbtnViewComm")).Attributes.Add("OnClick", "Open_Communication(" + strPara + ");return false;");
        }

        public void SetBlankHTMLExtender(Page page, TextBox txtBlank, HtmlEditorExtender htmlExtender, bool disableSourceTab)
        {
            try
            {
                htmlExtender.DisplaySourceTab = disableSourceTab;
                string strFunName = "on" + txtBlank.ID + "Changed";
                htmlExtender.OnClientChange = strFunName;// "onContentChanged";
                string strVal = " function " + strFunName + "() { " +
                  " var ctrlName = '" + txtBlank.ID + "'; " +
                  //" alert($(\"#\" + ctrlName + \"_HtmlEditorExtender_ExtenderSourceView\").html()); " +
                  " if ($(\"#\" + ctrlName).val() == \"&lt;br&gt;\" || " +
                  " $(\"#\" + ctrlName + \"_HtmlEditorExtender_ExtenderSourceView\").html() == \"&lt;br&gt;\" || " +
                  " $(\"#\" + ctrlName + \"_HtmlEditorExtender_ExtenderContentEditable\").html() == \"&lt;br&gt;\" ) { " +
                  " $(\"#\" + ctrlName).val(''); " +
                  " $(\"#\" + ctrlName + \"_HtmlEditorExtender_ExtenderContentEditable\").html(\"\");" +
                  " $(\"#\" + ctrlName + \"_HtmlEditorExtender_ExtenderSourceView\").html(\"\");" +
                  " } " +
                  " } ";

                //strVal += " $(\"#" + htmlExtender.ID + "_ExtenderSourceView\").css(\"height\", $(\"#" + htmlExtender.ID + "_ExtenderContentEditable\").height()+33); " +
                //    "$(\"#" + htmlExtender.ID + "_ExtenderContentEditable\").addClass(\"TxtBox\");  ";
                ScriptManager.RegisterClientScriptBlock(page, page.GetType(), strFunName, strVal, true);


            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }

        public void SetDisableHTMLExtender(Page page, TextBox txtBlank, HtmlEditorExtender htmlExtender)
        {
            try
            {
                string strFunName = "load";
                string ctrlName = txtBlank.ID;
                string strVal = "" +
                     " $(\"#" + ctrlName + "_HtmlEditorExtender_ExtenderSourceView\").attr('contenteditable', false);" +
                     " $(\"#" + ctrlName + "_HtmlEditorExtender_ExtenderContentEditable\").attr('contenteditable', false);";
                //ScriptManager.RegisterClientScriptBlock(page, page.GetType(), strFunName, strVal, true);

                ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "", "javascript:" + strVal + "", true);
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
        }

        public DataTable GetGoogleAPI(string address)
        {
            string LinkURL = DBCRN_System_Value.GetCRN_System_Value(127).Char_Value;
            string LinkKey = DBCRN_System_Value.GetCRN_System_Value(126).Char_Value;
            //string url = "https://maps.google.com/maps/api/geocode/xml?address=" + address + "&sensor=false&key=AIzaSyASCEc_Ky1-xtpIEfZKDg7StQ21rF6Mi3k";
            address = HttpUtility.UrlEncode(address);
            string url = LinkURL + "?address=" + address + "&sensor=false&key=" + LinkKey + "";
            //url = HttpUtility.UrlEncode(url);
            WebRequest request = WebRequest.Create(url);
            DataTable dtCoordinates = new DataTable();
            if (DBCRN_System_Value.GetCRN_System_Value(130).Bit_Value == true)
            {
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        DataSet dsResult = new DataSet();
                        dsResult.ReadXml(reader);

                        dtCoordinates.Columns.AddRange(new DataColumn[4] { new DataColumn("Id", typeof(int)),
                        new DataColumn("Address", typeof(string)),
                        new DataColumn("Latitude",typeof(string)),
                        new DataColumn("Longitude",typeof(string)) });
                        foreach (DataRow row in dsResult.Tables["result"].Rows)
                        {
                            string geometry_id = dsResult.Tables["geometry"].Select("result_id = " + row["result_id"].ToString())[0]["geometry_id"].ToString();
                            DataRow location = dsResult.Tables["location"].Select("geometry_id = " + geometry_id)[0];
                            dtCoordinates.Rows.Add(row["result_id"], row["formatted_address"], location["lat"], location["lng"]);
                        }
                    }
                }
            }
            else
            {
                dtCoordinates.Columns.Add("id");
                dtCoordinates.Columns.Add("Address");
                dtCoordinates.Columns.Add("Latitude");
                dtCoordinates.Columns.Add("Longitude");
                DataRow dr = dtCoordinates.NewRow();
                dr[0] = "";
                dr[1] = "";
                dr[2] = "";
                dr[3] = "";
                dtCoordinates.Rows.Add(dr);

            }
            return dtCoordinates;
        }

        public string ConvertHtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }

        public string GetMonthStartDate(string strActualDate)
        {
            DateTime dtActualDate = Convert.ToDateTime(strActualDate);
            if (DateFormatType == 1)
                return dtActualDate.ToString("MM/01/yyyy");
            else
                return dtActualDate.ToString("01-MMM-yyyy");
        }
        public DateTime GetMonthEndDate(string strActualDate)
        {
            DateTime dtActualDate = Convert.ToDateTime(strActualDate);
            int mm = dtActualDate.Month;
            int yy = dtActualDate.Year;
            if (mm == 1 || mm == 3 || mm == 5 || mm == 7 || mm == 8 || mm == 10 || mm == 12)
            {
                dtActualDate = Convert.ToDateTime(mm + "/31/" + yy);
            }
            else if (mm == 4 || mm == 6 || mm == 9 || mm == 11)
            {
                dtActualDate = Convert.ToDateTime(mm + "/30/" + yy);
            }
            else
            {
                if (yy % 4 == 0)
                    dtActualDate = Convert.ToDateTime(mm + "/29/" + yy);
                else
                    dtActualDate = Convert.ToDateTime(mm + "/28/" + yy);
            }
            return dtActualDate;
        }

        public static void ExecuteControls(ControlCollection ctrls)
        {
            if (DateFormatType != 1)
                SetControls(ctrls);
        }
        public static void SetControls(ControlCollection ctrls)
        {
            foreach (Control c in ctrls)
            {
                if (c is AjaxControlToolkit.CalendarExtender)
                {
                    ((AjaxControlToolkit.CalendarExtender)c).Format = DateFormat;
                }
                else if (c is RegularExpressionValidator)
                {
                    ((RegularExpressionValidator)c).ValidationExpression = DateExpression;
                }
                else if (c is Panel)
                {
                    SetControls(c.Controls);
                }
                else if (c is Wizard)
                {
                    for (int i = 0; i < ((Wizard)c).WizardSteps.Count; i++)
                    {
                        SetControls(((Wizard)c).WizardSteps[i].Controls);
                    }
                }
                else if (c is AjaxControlToolkit.TabContainer)
                {
                    for (int i = 0; i < ((AjaxControlToolkit.TabContainer)c).Tabs.Count; i++)
                    {
                        SetControls(((AjaxControlToolkit.TabContainer)c).Tabs[i].Controls);
                    }
                }
                else if (c is AjaxControlToolkit.TabPanel)
                {
                    SetControls(((AjaxControlToolkit.TabPanel)c).Controls);
                }
                else if (c is GridView)
                {
                    for (int i = 0; i < ((GridView)c).Rows.Count; i++)
                    {
                        GridViewRow row = (GridViewRow)((GridView)c).Rows[i];
                        for (int j = 0; j < ((GridView)c).Columns.Count; j++)
                        {
                            SetControls(row.Cells[j].Controls);
                        }
                    }
                }
                else if (c is UpdatePanel)
                {
                    foreach (Control c1 in c.Controls)
                    {
                        if (c1 is Control)

                            SetControls(c1.Controls);
                    }
                }
                else if (c is Panel)
                {
                    SetControls(((AjaxControlToolkit.TabPanel)c).Controls);
                }
                //else if (c is UserControl)
                //{
                //    SetControls(((UserControl)c).Controls);
                //}
            }
        }

        public void SetCustomValidator(Page page, TextBox txtFrom, TextBox txtTo, CustomValidator CV, CompareValidator CMV)
        {
            CMV.Visible = false;
            string strFunName = CV.ID + txtTo.ID + "_CompareDates";
            CV.ClientValidationFunction = strFunName;
            CV.ControlToValidate = txtTo.ID;
            //CV.ErrorMessage = CVMessage;
            CV.Display = ValidatorDisplay.Dynamic;
            string strOperator = ">";
            if (CMV.Operator == ValidationCompareOperator.GreaterThanEqual || CMV.Operator == ValidationCompareOperator.GreaterThan)
                strOperator = ">";
            else if (CMV.Operator == ValidationCompareOperator.LessThan || CMV.Operator == ValidationCompareOperator.LessThanEqual)
                strOperator = "<";
            else if (CMV.Operator == ValidationCompareOperator.Equal)
                strOperator = "=";
            else if (CMV.Operator == ValidationCompareOperator.NotEqual)
                strOperator = "!=";
            string strScript = "function " + strFunName + "(source, args) " +
                " { " +
                " var strFrom = document.getElementById('" + txtFrom.ID + "').value.replace('-', '/'); " +
                " var strTo = document.getElementById('" + txtTo.ID + "').value.replace('-', '/'); " +
                " strFrom=strFrom.replace('-', '/');" +
                " strTo=strTo.replace('-', '/');" +
                " var dateFrom = new Date(strFrom); " +
                " var dateTo = new Date(strTo); " +
                " source.innerHTML= \"" + CMV.ErrorMessage + "\";" +
                "  " +
                " if (dateFrom.getTime() " + strOperator + " dateTo.getTime()) " +
                " { " +
                "    args.IsValid = false; " +
                " } " +
                " else " +
                " { " +
                "    args.IsValid = true; " +
                " } " +
                " } " +
                " ; ";
            ScriptManager.RegisterStartupScript(page, page.GetType(), strFunName, strScript, true);
        }

        public string GetHTMLTableValue(DataTable dt, string GroupByColumnName, string strWidth, string[] strOtherGroupName, string[] strOtherGroupLabelName, string[] Columnwidth)
        {
            string cssTH = "text-align:left;background-color: #E6EEEE;color: #475DA7;border: 1px solid silver;font-size: 8pt;font-family: arial; ";
            string cssTD = "color: #3d3d3d; padding-top: 4px;border: 1px solid silver;padding: 4px; ";
            string result = "";
            try
            {
                if (dt.Columns.Count < 1 || dt.Rows.Count < 1)
                    return "";


                DataTable dtDistintValues = dt.DefaultView.ToTable(true, GroupByColumnName);
                dtDistintValues.DefaultView.Sort = GroupByColumnName + " ASC";

                result += "<table style='width:" + strWidth + ";'>";
                for (int k = 0; k < dtDistintValues.Rows.Count; k++)
                {

                    DataRow[] dr = dt.Select(GroupByColumnName + "='" + dtDistintValues.Rows[k][0].ToString() + "'");
                    if (dr.Length > 0)
                    {
                        DataTable dtDataValues = dr.CopyToDataTable();

                        result += "<tr><td colspan='" + (dt.Columns.Count - 1) + "'>" + (k + 1).ToString() + "." + dtDistintValues.Rows[k][0].ToString() + "</td></tr>";
                        for (int cc = 0; cc < strOtherGroupName.Length; cc++)
                        {
                            if (dtDataValues.Rows[0][strOtherGroupName[cc]].ToString().Trim() != "" &&
                                dtDataValues.Rows[0][strOtherGroupName[cc]].ToString().Trim() != "<br/>")
                            {
                                if (cc == 0)
                                    result += "<tr><td colspan='" + (dt.Columns.Count - 1) + "'>" + strOtherGroupLabelName[cc] + ":<a target='_blank' href='" + dtDataValues.Rows[0][strOtherGroupName[cc]].ToString() + "'>" + dtDataValues.Rows[0][strOtherGroupName[cc]].ToString() + "</a> </td></tr>";
                                else
                                    result += "<tr><td colspan='" + (dt.Columns.Count - 1) + "'>" + strOtherGroupLabelName[cc] + ":" + dtDataValues.Rows[0][strOtherGroupName[cc]].ToString() + "</td></tr>";
                            }
                        }
                        result += "<tr><td>";
                        result += "<table border=1 cellpadding='0' cellspacing='0'><tr>";

                        int cnt = 0;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            if (Array.Exists(strOtherGroupName, dt.Columns[i].ColumnName.ToString().Contains) == false)
                            {
                                if (GroupByColumnName != dt.Columns[i].ColumnName.ToString())
                                {
                                    string cssWid = "width:" + Columnwidth[cnt];
                                    result += "<th style='" + cssTH + cssWid + "'>" + dt.Columns[i].ColumnName.ToString() + "</th>";
                                    cnt++;
                                }
                            }
                        }

                        result += "</tr>";

                        for (int i = 0; i < dtDataValues.Rows.Count; i++)
                        {
                            result += "<tr>";
                            for (int j = 0; j < dtDataValues.Columns.Count; j++)
                            {
                                if (Array.Exists(strOtherGroupName, dtDataValues.Columns[j].ColumnName.ToString().Contains) == false)
                                {
                                    if (GroupByColumnName != dtDataValues.Columns[j].ColumnName.ToString())
                                    {
                                        if (dtDataValues.Columns[j].DataType == typeof(short) || dtDataValues.Columns[j].DataType == typeof(long) ||
                                            dtDataValues.Columns[j].DataType == typeof(int) || dtDataValues.Columns[j].DataType == typeof(Int16) ||
                                            dtDataValues.Columns[j].DataType == typeof(Int32) || dtDataValues.Columns[j].DataType == typeof(Int64))
                                        {
                                            result += "<td style='text-align:right;" + cssTD + "'>" + dtDataValues.Rows[i][j].ToString() + "</td>";
                                        }
                                        else if (dtDataValues.Columns[j].DataType == typeof(double) || dtDataValues.Columns[j].DataType == typeof(Double) ||
                                            dtDataValues.Columns[j].DataType == typeof(decimal) || dtDataValues.Columns[j].DataType == typeof(Decimal))
                                        {
                                            result += "<td style='text-align:right;" + cssTD + "'>" + Convert.ToDouble(dtDataValues.Rows[i][j].ToString()).ToString("0.00") + "</td>";
                                        }
                                        else if (dtDataValues.Columns[j].DataType == typeof(DateTime))
                                        {
                                            result += "<td style='text-align:left;" + cssTD + "'>" + Convert.ToDateTime(dtDataValues.Rows[i][j].ToString()).ToString(DateFormat) + "</td>";
                                        }
                                        else
                                            result += "<td style='text-align:left;" + cssTD + "'>" + dtDataValues.Rows[i][j].ToString() + "</td>";
                                    }
                                }
                            }
                            result += "</tr>";
                        }

                        result += "</table>";//inner table close
                    }
                    result += "</td></tr>";

                    //if (result.Contains("<tr></tr>"))
                    //    result = result.Replace("<tr></tr>", "");
                }
                result += "</table>";//outer table close
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
            return result;
        }

        public string GetHTMLTableValue(DataTable dt, string strWidth)
        {
            string cssTH = "text-align:left;background-color: #E6EEEE;color: #475DA7;border: 1px solid silver;font-size: 8pt;font-family: arial; ";
            string cssTD = "color: #3d3d3d; padding-top: 4px;border: 1px solid silver;padding: 4px; ";
            string result = "";
            try
            {
                if (dt.Columns.Count < 1 || dt.Rows.Count < 1)
                    return "";


                result += "<table border=1 cellpadding='0' cellspacing='0' style='width:" + strWidth + ";'><tr>";

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    result += "<th style='" + cssTH + "'>" + dt.Columns[i].ColumnName.ToString() + "</th>";
                }

                result += "</tr>";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    result += "<tr>";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {

                        if (dt.Columns[j].DataType == typeof(short) || dt.Columns[j].DataType == typeof(long) ||
                            dt.Columns[j].DataType == typeof(int) || dt.Columns[j].DataType == typeof(Int16) ||
                            dt.Columns[j].DataType == typeof(Int32) || dt.Columns[j].DataType == typeof(Int64))
                        {
                            result += "<td style='text-align:right;" + cssTD + "'>" + dt.Rows[i][j].ToString() + "</td>";
                        }
                        else if (dt.Columns[j].DataType == typeof(double) || dt.Columns[j].DataType == typeof(Double) ||
                            dt.Columns[j].DataType == typeof(decimal) || dt.Columns[j].DataType == typeof(Decimal))
                        {
                            result += "<td style='text-align:right;" + cssTD + "'>" + Convert.ToDouble(dt.Rows[i][j].ToString()).ToString("0.00") + "</td>";
                        }
                        else if (dt.Columns[j].DataType == typeof(DateTime))
                        {
                            result += "<td style='text-align:left;" + cssTD + "'>" + Convert.ToDateTime(dt.Rows[i][j].ToString()).ToString(DateFormat) + "</td>";
                        }
                        else
                            result += "<td style='text-align:left;" + cssTD + "'>" + dt.Rows[i][j].ToString() + "</td>";

                    }
                    result += "</tr>";
                }
                result += "</table>";//inner table close
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
            return result;
        }

        public DataTable SetEventLocationAddress(DataTable dt, int ChangeIndex)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i].ItemArray[20].ToString() == "323")
                {
                    string strAdd = (Convert.IsDBNull(dt.Rows[i].ItemArray[11]) == false) ? dt.Rows[i].ItemArray[11].ToString() : "";
                    strAdd += (Convert.IsDBNull(dt.Rows[i].ItemArray[12]) == false) ? dt.Rows[i].ItemArray[12].ToString() : "";
                    strAdd += ", ";
                    strAdd += (Convert.IsDBNull(dt.Rows[i].ItemArray[13]) == false) ? dt.Rows[i].ItemArray[13].ToString() : "";
                    strAdd += ", ";
                    strAdd += (Convert.IsDBNull(dt.Rows[i].ItemArray[15]) == false) ? dt.Rows[i].ItemArray[15].ToString() : "";
                    strAdd += " ";
                    strAdd += (Convert.IsDBNull(dt.Rows[i].ItemArray[16]) == false) ? dt.Rows[i].ItemArray[16].ToString() : "";
                    strAdd += ", ";
                    strAdd += (Convert.IsDBNull(dt.Rows[i].ItemArray[17]) == false) ? dt.Rows[i].ItemArray[17].ToString() : "";

                    dt.Rows[i][ChangeIndex] = dt.Rows[i][ChangeIndex].ToString() + " " + strAdd;
                    dt.AcceptChanges();
                }
            }
            return dt;
        }
        public string GetHTMLTableValueText(DataTable dt, string GroupByColumnName, string strWidth)
        {
            //string cssTH = "text-align:left;background-color: #E6EEEE;color: #475DA7;border: 1px solid silver;font-size: 8pt;font-family: arial; ";
            string cssTD = "text-align:justify;";// "color: #3d3d3d; padding-top: 4px;border: 1px solid silver;padding: 4px; ";
            string result = "";
            try
            {
                if (dt.Columns.Count < 1 || dt.Rows.Count < 1)
                    return "";

                DataTable dtDistintValues = dt.DefaultView.ToTable(true, GroupByColumnName);
                dtDistintValues.DefaultView.Sort = GroupByColumnName + " ASC";

                result += "<table style='width:" + strWidth + ";'>";
                for (int k = 0; k < dtDistintValues.Rows.Count; k++)
                {

                    DataRow[] dr = dt.Select(GroupByColumnName + "='" + dtDistintValues.Rows[k][0].ToString() + "'");
                    if (dr.Length > 0)
                    {
                        DataTable dtDataValues = dr.CopyToDataTable();

                        result += "<tr><td colspan='" + (dt.Columns.Count - 1) + "'><b>" + dtDistintValues.Rows[k][0].ToString() + "</b></td></tr>";
                        result += "<tr><td>";
                        result += "<table border='0' cellpadding='0' cellspacing='0'>";

                        result += "<tr><td style='text-align:left;" + cssTD + "'>";
                        for (int i = 0; i < dtDataValues.Rows.Count; i++)
                        {
                            for (int j = 0; j < dtDataValues.Columns.Count; j++)
                            {
                                if (GroupByColumnName != dtDataValues.Columns[j].ColumnName.ToString())
                                {
                                    if (i == 0)
                                        result += dtDataValues.Rows[i][j].ToString();
                                    else
                                        result += ", " + dtDataValues.Rows[i][j].ToString();

                                }
                            }
                        }
                        result += "</td></tr>";

                        result += "</table>";//inner table close
                    }
                    result += "</td></tr>";

                    //if (result.Contains("<tr></tr>"))
                    //    result = result.Replace("<tr></tr>", "");
                }
                result += "</table>";//outer table close
            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
            return result;
        }
        public string[] GetDays()
        {
            string[] strDays = new string[7];
            strDays[0] = "MON";
            strDays[1] = "TUE";
            strDays[2] = "WED";
            strDays[3] = "THU";
            strDays[4] = "FRI";
            strDays[5] = "SAT";
            strDays[6] = "SUN";
            return strDays;
        }

        public string GetDay(long weekday)
        {
            string strDays = "";
            DayOfWeek dayofweek = (DayOfWeek)weekday;
            if (dayofweek == DayOfWeek.Sunday)
                strDays = "SUN";
            else if (dayofweek == DayOfWeek.Monday)
                strDays = "MON";
            else if (dayofweek == DayOfWeek.Tuesday)
                strDays = "TUE";
            else if (dayofweek == DayOfWeek.Wednesday)
                strDays = "WED";
            else if (dayofweek == DayOfWeek.Thursday)
                strDays = "THU";
            else if (dayofweek == DayOfWeek.Friday)
                strDays = "FRI";
            else if (dayofweek == DayOfWeek.Saturday)
                strDays = "SAT";

            return strDays;
        }

        public bool GetEmailSendQuota(long CurrentEmailCount, Page Page)
        {

            try
            {
                DateTime CurrTime = DateTime.Now;
                DateTime FromTime = Convert.ToDateTime(CurrTime.ToString("MM/dd/yyyy hh:00:00 tt"));
                DateTime ToTime = Convert.ToDateTime(DateTime.Now.AddHours(1).ToString("MM/dd/yyyy hh:00:00 tt"));
                long EmailSendCount = ObjQry.ReturnLong("Exec Proc_Request_Notification_Member_Select_Count '" + FromTime.ToString() + "','" + ToTime.ToString() + "' ");
                long EmailQuota = DBCRN_System_Value.GetCRN_System_Value(147).Numeric_Value;
                long EmailBalance = EmailQuota - EmailSendCount;
                long MinuteVal = ToTime.Subtract(CurrTime).Minutes;
                if (CurrentEmailCount > EmailBalance)
                {
                    CRN_Application_Message CRNAppMessage = DBCRN_Application_Message.GetMessage(548);
                    string strMessage = CRNAppMessage.Message_Text;
                    strMessage = strMessage.Replace("@EmailQuota", EmailQuota.ToString());
                    strMessage = strMessage.Replace("@EmailSendCount", EmailSendCount.ToString());
                    strMessage = strMessage.Replace("@EmailBalance", EmailBalance.ToString());
                    strMessage = strMessage.Replace("@EmailSendTime", MinuteVal.ToString());
                    strMessage = strMessage.Replace("@CurrentEmailCount", CurrentEmailCount.ToString());

                    CRNAppMessage.Message_Text = strMessage;
                    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "Popup", "ShowPopup('" + CRNAppMessage.Message_Header + "','" + CRNAppMessage.Message_Text + "'," + CRNAppMessage.Message_Width + "," + CRNAppMessage.Message_Height + ");", true);
                    return false;
                }

            }
            catch (Exception exc)
            {
                ExceptioMessage.ShowMessage(exc.Message);
            }
            return true;
        }

        public bool isMobileBrowser()
        {
            //GETS THE CURRENT USER CONTEXT
            HttpContext context = HttpContext.Current;

            //FIRST TRY BUILT IN ASP.NT CHECK
            if (context.Request.Browser.IsMobileDevice)
            {
                return true;
            }
            //THEN TRY CHECKING FOR THE HTTP_X_WAP_PROFILE HEADER
            if (context.Request.ServerVariables["HTTP_X_WAP_PROFILE"] != null)
            {
                return true;
            }
            //THEN TRY CHECKING THAT HTTP_ACCEPT EXISTS AND CONTAINS WAP
            if (context.Request.ServerVariables["HTTP_ACCEPT"] != null &&
                context.Request.ServerVariables["HTTP_ACCEPT"].ToLower().Contains("wap"))
            {
                return true;
            }
            //AND FINALLY CHECK THE HTTP_USER_AGENT 
            //HEADER VARIABLE FOR ANY ONE OF THE FOLLOWING
            if (context.Request.ServerVariables["HTTP_USER_AGENT"] != null)
            {
                //Create a list of all mobile types
                string[] mobiles =
                    new[]
                {
                    "midp", "j2me", "avant", "docomo",
                    "novarra", "palmos", "palmsource",
                    "240x320", "opwv", "chtml",
                    "pda", "windows ce", "mmp/",
                    "blackberry", "mib/", "symbian",
                    "wireless", "nokia", "hand", "mobi",
                    "phone", "cdm", "up.b", "audio",
                    "SIE-", "SEC-", "samsung", "HTC",
                    "mot-", "mitsu", "sagem", "sony"
                    , "alcatel", "lg", "eric", "vx",
                    "NEC", "philips", "mmm", "xx",
                    "panasonic", "sharp", "wap", "sch",
                    "rover", "pocket", "benq", "java",
                    "pt", "pg", "vox", "amoi",
                    "bird", "compal", "kg", "voda",
                    "sany", "kdd", "dbt", "sendo",
                    "sgh", "gradi", "jb", "dddi",
                    "moto", "iphone"
                };

                //Loop through each item in the list created above 
                //and check if the header contains that text
                foreach (string s in mobiles)
                {
                    if (context.Request.ServerVariables["HTTP_USER_AGENT"].
                                                        ToLower().Contains(s.ToLower()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void SetLinkNavigateURL(HyperLink hlnk)
        {
            if (((HyperLink)hlnk).NavigateUrl.IndexOf("http") < 0)
                ((HyperLink)hlnk).NavigateUrl = "http://" + ((HyperLink)hlnk).NavigateUrl;
        }
        public string SetLinkNavigateURL(string strLink)
        {
            if (strLink.IndexOf("http") < 0)
                strLink = "http://" + strLink;
            return strLink;
        }

        public DataTable JsonStringToDataTable(string jsonString)
        {
            DataTable dt = new DataTable();
            string[] jsonStringArray = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
            List<string> ColumnsName = new List<string>();
            foreach (string jSA in jsonStringArray)
            {
                string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                foreach (string ColumnsNameData in jsonStringData)
                {
                    try
                    {
                        int idx = ColumnsNameData.IndexOf(":");
                        string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                        if (!ColumnsName.Contains(ColumnsNameString))
                        {
                            ColumnsName.Add(ColumnsNameString);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    }
                }
                break;
            }
            foreach (string AddColumnName in ColumnsName)
            {
                dt.Columns.Add(AddColumnName);
            }
            foreach (string jSA in jsonStringArray)
            {
                string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                DataRow nr = dt.NewRow();
                foreach (string rowData in RowData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string RowColumns = rowData.Substring(0, idx - 1).Replace("\"", "");
                        string RowDataString = rowData.Substring(idx + 1).Replace("\"", "");
                        nr[RowColumns] = RowDataString;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                dt.Rows.Add(nr);
            }
            return dt;
        }

        public string DataTableToJsonString(DataTable table)
        {
            var JSONString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            if (table.Rows[i][j].GetType().Name == "DateTime")
                                JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + Convert.ToDateTime(table.Rows[i][j]).ToString(CommonFunctions.DateFormat) + "\",");
                            else
                                JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString().Replace("'", "''").Replace("\n", "").Replace("\"", "") + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            if (table.Rows[i][j].GetType().Name == "DateTime")
                                JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + Convert.ToDateTime(table.Rows[i][j]).ToString(CommonFunctions.DateFormat) + "\"");
                            else
                                JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString().Replace("'", "''").Replace("\n", "") + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }
            return JSONString.ToString();
        }

        public string ConvertKeyToHTMLTag(string strMailMessage)
        {
            strMailMessage = strMailMessage.Replace("\n", "<br/>");
            strMailMessage = strMailMessage.Replace("&lt;", "<").Replace("&gt;", ">");
            return strMailMessage;
        }

        public string SetCloseHTMLTag(string snippet)
        {
            if (string.IsNullOrEmpty(snippet) || (snippet.TrimStart().StartsWith("<") && snippet.TrimEnd().EndsWith(">")) || !snippet.TrimStart().StartsWith("<"))
            {
                return snippet;
            }

            var index = snippet.IndexOf('>');
            var tag = snippet.Substring(1, index - 1);
            return snippet.TrimEnd() + "</" + tag + ">";
        }

        public long GetAgeGroupID(DataTable dt, int AgeVal)
        {
            long AgeGroupID = 1;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int startrange = Convert.ToInt32(dt.Rows[i]["Age_Group_Start_Range"].ToString());
                int endreange = Convert.ToInt32(dt.Rows[i]["Age_Group_End_Range"].ToString());

                if (AgeVal >= startrange && AgeVal <= endreange)
                {
                    AgeGroupID = Convert.ToInt64(dt.Rows[i]["Age_Group_ID"].ToString());
                    break;
                }
            }
            return AgeGroupID;
        }

        public bool CheckEmailID(string strEmailID)
        {
            bool flag = true;
            if (strEmailID != "")
            {
                string pattern = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"; // A basic regex pattern
                Regex regex = new Regex(pattern);
                flag = regex.IsMatch(strEmailID);
            }
            return flag;
        }

    }

    public class DB_Code_Value_Type_Code
    {
        public static int Organization_Status
        {
            get { return 1; }
        }
        public static int Person_Status
        {
            get { return 2; }
        }
        public static int Employee_Status
        {
            get { return 3; }
        }
        public static int Application_Status
        {
            get { return 4; }
        }
        public static int Application_Notification_Status
        {
            get { return 5; }
        }
        public static int Member_Status
        {
            get { return 6; }
        }
        public static int Member_Type
        {
            get { return 7; }
        }
        public static int Member_Category_Status
        {
            get { return 8; }
        }
        public static int Member_Location_Status
        {
            get { return 9; }
        }
        public static int Request_Status
        {
            get { return 10; }
        }
        public static int Request_Purpose
        {
            get { return 11; }
        }
        public static int Request_Category_Status
        {
            get { return 12; }
        }
        public static int Request_Location_Status
        {
            get { return 13; }
        }
        public static int Organisation_Type
        {
            get { return 14; }
        }
        public static int Notification_Type
        {
            get { return 15; }
        }
        public static int CRN_Reference_Type
        {
            get { return 16; }
        }
        public static int Member_Invite_Status
        {
            get { return 17; }
        }
        public static int Resource_Status
        {
            get { return 18; }
        }
        public static int Resource_Location_Status
        {
            get { return 19; }
        }
        public static int Resource_Location_Service_Status
        {
            get { return 20; }
        }
        public static int Resource_Location_Contact_Status
        {
            get { return 21; }
        }
        public static int Resource_Location_Work_Schedule_Status
        {
            get { return 22; }
        }
        public static int Schedule_Day_Of_Week
        {
            get { return 23; }
        }
        public static int Communication_Type
        {
            get { return 24; }
        }
        public static int Event_Type
        {
            get { return 25; }
        }
        public static int Event_Status
        {
            get { return 26; }
        }
        public static int Event_Communication_Type
        {
            get { return 27; }
        }
        public static int Event_Email_Notification
        {
            get { return 28; }
        }
        public static int Event_UOM
        {
            get { return 29; }
        }
        public static int Event_Participate_Location_Type
        {
            get { return 30; }
        }
        public static int Counties_Served
        {
            get { return 31; }
        }
        public static int Event_Category
        {
            get { return 32; }
        }
        public static int Subscription_Notification_Type
        {
            get { return 33; }
        }
        public static int Event_Organizer_Type
        {
            get { return 34; }
        }
        public static int Request_Client_Gender
        {
            get { return 35; }
        }
        public static int Event_Location_Type
        {
            get { return 36; }
        }

        public static int Organization_Verified_Status
        {
            get { return 38; }
        }
        public static int Organization_Detail_File_Type
        {
            get { return 39; }
        }
        public static int Organization_Detail_File_Status
        {
            get { return 40; }
        }
        public static int Organization_Address_Status
        {
            get { return 41; }
        }
        public static int Organization_Address_Verified_Status
        {
            get { return 42; }
        }
        public static int OHP_Client_Status
        {
            get { return 43; }
        }
        public static int Client_Military_Affiliation
        {
            get { return 44; }
        }
        public static int Backbone_Organization_Status
        {
            get { return 45; }
        }
        public static int Backbone_Organization_Address_Status
        {
            get { return 46; }
        }
        public static int Backbone_Organization_Web_Link_Status
        {
            get { return 47; }
        }
        public static int Backbone_Organization_Web_Link_Type
        {
            get { return 48; }
        }
    }

    public class DB_Code_Value_Item_Code
    {
        public static int Organization_Status_Active
        {
            get { return 1; }
        }
        public static int Organization_Status_InActive
        {
            get { return 2; }
        }
        public static int Person_Status_Active
        {
            get { return 3; }
        }
        public static int Person_Status_InActive
        {
            get { return 4; }
        }
        public static int Employee_Status_Active
        {
            get { return 5; }
        }
        public static int Employee_Status_InActive
        {
            get { return 6; }
        }
        public static int Application_Status_Submitted
        {
            get { return 7; }
        }
        public static int Application_Status_Approved
        {
            get { return 8; }
        }
        public static int Application_Status_Declined
        {
            get { return 9; }
        }
        public static int Application_Status_Deleted
        {
            get { return 212; }
        }
        public static int Member_Status_Active
        {
            get { return 10; }
        }
        public static int Member_Status_InActive
        {
            get { return 11; }
        }
        public static int Member_Status_Locked
        {
            get { return 29; }
        }
        public static int Member_Type_CRN_Admin
        {
            get { return 1; }
            //return 12; 
        }
        public static int Member_Type_Organization_Admin
        {
            get { return 13; }
        }
        public static int Member_Type_CRN_Member
        {
            get { return 2; }
            //return 14;
        }
        public static int Member_Category_Status_Active
        {
            get { return 15; }
        }
        public static int Member_Category_Status_InActive
        {
            get { return 16; }
        }
        public static int Member_Location_Status_Active
        {
            get { return 17; }
        }
        public static int Member_Location_Status_InActive
        {
            get { return 18; }
        }
        public static int Request_Status_Submitted
        {
            get { return 19; }
        }
        public static int Request_Status_Decline
        {
            get { return 20; }
        }
        public static int Request_Status_Approved
        {
            get { return 21; }
        }
        public static int Request_Status_FullFilled
        {
            get { return 204; }
        }
        public static int Request_Status_UnFullfilled
        {
            get { return 205; }
        }
        public static int Request_Status_Deleted
        {
            get { return 213; }
        }
        public static int Request_Status_Withdrawn
        {
            get { return 228; }
        }
        public static int Request_Status_ReSubmitted
        {
            get { return 242; }
        }
        public static int Request_Status_InProgress
        {
            get { return 282; }
        }
        public static int Request_Status_Unknown
        {
            get { return 283; }
        }
        public static int Request_Status_Referred
        {
            get { return 284; }
        }
        public static int Request_Purpose_IHaveNeed
        {
            get { return 22; }
        }
        public static int Request_Purpose_IHaveSomethingToShare
        {
            get { return 23; }
        }
        public static int Request_Purpose_IAmSeekingHelpForaCommunityEvent
        {
            get { return 24; }
        }
        public static int Request_Category_Status_Active
        {
            get { return 25; }
        }
        public static int Request_Category_Status_InActive
        {
            get { return 26; }
        }
        public static int Request_Location_Status_Active
        {
            get { return 27; }
        }
        public static int Member_Invitation_Status_Pending
        {
            get { return 259; }
        }
        public static int Member_Invitation_Status_Accepted
        {
            get { return 260; }
        }
        public static int Member_Invitation_Status_Decline
        {
            get { return 261; }
        }
        public static int Created_By_Admin
        {
            get { return 262; }
        }

        public static int Comm_Type_Request_Clarification
        {
            get { return 280; }
        }
        public static int Comm_Type_Thank_You_Note
        {
            get { return 281; }
        }
        public static int Event_Type_Emergency
        {
            get { return 285; }
        }
        public static int Event_Type_NonEmergency
        {
            get { return 286; }
        }
        public static int Event_Status_Submitted
        {
            get { return 287; }
        }
        public static int Event_Status_Approved
        {
            get { return 288; }
        }
        public static int Event_Status_Deleted
        {
            get { return 289; }
        }
        public static int Event_Status_Declined
        {
            get { return 290; }
        }
        public static int Event_Status_Open
        {
            get { return 291; }
        }
        public static int Event_Status_Fulfilled
        {
            get { return 292; }
        }
        public static int Event_Status_Withdrawn
        {
            get { return 293; }
        }
        public static int Event_Status_UnFulfilled
        {
            get { return 294; }
        }
        public static int Event_Comm_Type_Event_Clarification
        {
            get { return 295; }
        }
        public static int Event_Comm_Type_Thank_You_Note
        {
            get { return 296; }
        }
        public static int Receive_All_Emergency_Event_Notifications
        {
            get { return 297; }
        }
        public static int Receive_Emergency_Event_Notifications
        {
            get { return 298; }
        }
        public static int Receive_Non_Emergency_Event_Notifications
        {
            get { return 299; }
        }
        public static int Receive_None_of_theEmergency_Event_Notifications
        {
            get { return 300; }
        }

        public static int Subscription_New
        {
            get { return 312; }
        }
        public static int Subscription_Custom_Notification
        {
            get { return 313; }
        }
        public static int Comm_Type_Request_Clarification_Reply
        {
            get { return 326; }
        }
        public static int Email_Status_New
        {
            get { return 327; }
        }
        public static int Email_Status_Archieve
        {
            get { return 328; }
        }
        public static int Subscription_Conversion_Subscriber
        {
            get { return 329; }
        }
        public static int Organization_Detail_File_Type_Regular
        {
            get { return 332; }
        }
        //public static int Organization_Detail_File_Type_Custom
        //{
        //    get { return 333; }
        //}
        public static int Organization_Detail_File_Status_Active
        {
            get { return 334; }
        }
        public static int Organization_Detail_File_Status_InActive
        {
            get { return 335; }
        }
        public static int Organization_Address_Status_Active
        {
            get { return 336; }
        }
        public static int Organization_Address_Status_InActive
        {
            get { return 337; }
        }
        public static int OHP_Client_Status_No
        {
            get { return 340; }
        }
        public static int Backbone_Organization_Status_Active
        {
            get { return 348; }
        }
        public static int Backbone_Organization_Status_InActive
        {
            get { return 349; }
        }
        public static int Backbone_Organization_Address_Status_Active
        {
            get { return 350; }
        }
        public static int Backbone_Organization_Address_Status_InActive
        {
            get { return 351; }
        }
        public static int Backbone_Organization_Web_Link_Status_Active
        {
            get { return 352; }
        }
        public static int Backbone_Organization_Web_Link_Status_InActive
        {
            get { return 353; }
        }
    }

    public class System_Data_Type
    {
        public static string Bit
        {
            get { return "BIT"; }
        }
        public static string Numeric
        {
            get { return "NUMERIC"; }
        }
        public static string Date
        {
            get { return "DATE"; }
        }
        public static string DateTime
        {
            get { return "DATETIME"; }
        }
        public static string Varchar
        {
            get { return "VARCHAR"; }
        }
        public static string URL_Varchar
        {
            get { return "URL_VARCHAR"; }
        }
        public static string EmailID_Varchar
        {
            get { return "EMAILID_VARCHAR"; }
        }
        public static string Phone_Varchar
        {
            get { return "PHONE_VARCHAR"; }
        }
        public static string Numeric_Select
        {
            get { return "NUMERIC_SELECT"; }
        }
        public static string Numeric_USPS
        {
            get { return "NUMERIC_USPS"; }
        }
    }

    public class AppFont
    {

        public static iTextSharpeText.Font heading1Font = new iTextSharpeText.Font(iTextSharpeText.FontFactory.GetFont("VERDANA", 14, iTextSharpeText.Font.BOLD));
        public static iTextSharpeText.Font heading2Font = new iTextSharpeText.Font(iTextSharpeText.FontFactory.GetFont("VERDANA", 10, iTextSharpeText.Font.BOLD));
        public static iTextSharpeText.Font dataHeaderFont = new iTextSharpeText.Font(iTextSharpeText.FontFactory.GetFont("VERDANA", 9, iTextSharpeText.Font.BOLD));
        public static iTextSharpeText.Font dataBoldFont = new iTextSharpeText.Font(iTextSharpeText.FontFactory.GetFont("VERDANA", 8, iTextSharpeText.Font.BOLD));
        public static iTextSharpeText.Font dataFont = new iTextSharpeText.Font(iTextSharpeText.FontFactory.GetFont("VERDANA", 8, iTextSharpeText.Font.NORMAL));
        public static iTextSharpeText.Font TableHeadFont = new iTextSharpeText.Font(iTextSharpeText.FontFactory.GetFont("VERDANA", 8, iTextSharpeText.Font.BOLD));
        public static iTextSharpeText.Font TabledataFont = new iTextSharpeText.Font(iTextSharpeText.FontFactory.GetFont("VERDANA", 8, iTextSharpeText.Font.NORMAL));

    }

    public class MAP_Location
    {
        private string mAddress_Line_1 = "";
        private string mAddress_Line_2 = "";
        private string mCity = "";
        private string mState = "";
        private string mCounty;
        private string mZip = "";
        private string mZip_Suffix;
        private string mCountry = "";
        private string mAddress_Latitude = "";
        private string mAddress_Longitude = "";


        public string Address_Line_1
        {
            get { return mAddress_Line_1; }
            set { mAddress_Line_1 = value; }
        }
        public string Address_Line_2
        {
            get { return mAddress_Line_2; }
            set { mAddress_Line_2 = value; }
        }
        public string City
        {
            get { return mCity; }
            set { mCity = value; }
        }
        public string County
        {
            get { return mCounty; }
            set { mCounty = value; }
        }
        public string State
        {
            get { return mState; }
            set { mState = value; }
        }
        public string Zip
        {
            get { return mZip; }
            set { mZip = value; }
        }
        public string Zip_Suffix
        {
            get { return mZip_Suffix; }
            set { mZip_Suffix = value; }
        }
        public string Country
        {
            get { return mCountry; }
            set { mCountry = value; }
        }
        public string Address_Latitude
        {
            get { return mAddress_Latitude; }
            set { mAddress_Latitude = value; }
        }
        public string Address_Longitude
        {
            get { return mAddress_Longitude; }
            set { mAddress_Longitude = value; }
        }

    }

    public class Mail_Details
    {
        public string Body { get; set; }
        public string EmailTo { get; set; }
        public string Subject { get; set; }
        public string EmailCC { get; set; }
        public string[] EmailBCC { get; set; }
        public string[] EmailBCCOther { get; set; }
        public EmailIDCollection EmailIDs { get; set; }
    }

    public class ViewRequestPara
    {
        public bool flageClientDetails = true;
        public bool flageEditClientDetails = true;
        public int RType = 0;
        public bool IsAnonymous = false;
        public bool RequestClone = false;
    }

    public class Member_Type
    {
        public static int Admin
        {
            get { return 1; }
        }
        public static int Requestor
        {
            get { return 2; }
        }
        public static int Donor
        {
            get { return 3; }
        }
        public static int OrgAdmin
        {
            get { return 4; }
        }
        public static int SuperAdmin
        {
            get { return 5; }
        }
        public static int DistrictAdmin
        {
            get { return 6; }
        }
    }

    public class Report_Screen
    {
        public static int Graphs_Requests
        {
            get { return 1; }
        }
        public static int Graphs_Organization
        {
            get { return 2; }
        }
        public static int Graphs_Active_Contributors
        {
            get { return 3; }
        }
    }
}