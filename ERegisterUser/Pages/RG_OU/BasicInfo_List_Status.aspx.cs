using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System;
using Epoint.MisBizLogic2.Data;
using System.Text;
using EpointRegisterUser_Bizlogic;

namespace EpointRegisterUser.Pages.RG_OU
{

    public partial class BasicInfo_List_Status : Epoint.Frame.Bizlogic.BaseContentPage_UsingMaster
    {

        public int TableID = 1041;
        Epoint.MisBizLogic2.Web.ListPage oListPage;
        public string TableName = "SVG_ProManage_BasicInfo";
        SVGBLL.Pro_Fund pf = new SVGBLL.Pro_Fund();


        protected void Page_Load(object sender, System.EventArgs e)
        {

            if (!Page.IsPostBack)
            {

                
                #region ��� ��Ŀ��Դ
                DataView dv = new Epoint.MisBizLogic2.Code.DB_CodeItem().Get_Items_By_CodeName("���ݴ�Ͷ_��Ŀ��Դ");
                if (dv.Count > 0)
                {
                    for (int i = 0; i < dv.Count; i++)
                    {
                        this.s_xiangMuLY.Items.Add(new ListItem(dv[i]["ItemText"].ToString(), dv[i]["ItemValue"].ToString()));
                    }
                }
                #endregion

                #region ��� ����ƽ̨
                DataView dv_GLPT = new SVGBLL.Pro_Fund().Select_guanLiPT_byUserGuid(Session["UserGuid"].ToString());
                if (dv_GLPT.Count > 0)
                {
                    for (int i = 0; i < dv_GLPT.Count; i++)
                    {
                        this.s_GuanLiPT.Items.Add(new ListItem(dv_GLPT[i]["s_guanlir"].ToString(), dv_GLPT[i]["rowguid"].ToString()));
                    }
                }
                #endregion

                #region ��� ��Ŀ״̬
                dv = new Epoint.MisBizLogic2.Code.DB_CodeItem().Get_Items_By_CodeName("���ݴ�Ͷ_��Ŀ״̬");
                if (dv.Count > 0)
                {
                    for (int i = 1; i < dv.Count; i++)
                    {
                        this.s_xiangMuZT.Items.Add(new ListItem(dv[i]["ItemText"].ToString(), dv[i]["ItemValue"].ToString()));
                    }
                }
                #endregion

                #region ��ʱ��ת��Ϊ����ʱ��
                int thisYear = DateTime.Now.Year;
                int thisMonth = DateTime.Now.Month;
                string JD = "";
                JD += thisYear + "-";
                if (thisMonth.ToString() == "1" || thisMonth.ToString() == "2" || thisMonth.ToString() == "3")
                {
                    JD += "01-01 00:00:00";
                }
                else if (thisMonth.ToString() == "4" || thisMonth.ToString() == "5" || thisMonth.ToString() == "6")
                {
                    JD += "04-01 00:00:00";
                }
                else if (thisMonth.ToString() == "7" || thisMonth.ToString() == "8" || thisMonth.ToString() == "9")
                {
                    JD += "07-01 00:00:00";
                }
                else
                {
                    JD += "10-01 00:00:00";
                }
                ViewState["JD"] = JD;
                #endregion

                #region ת����ǰʱ��
                DateTime dt = DateTime.Parse(thisYear + "-" + thisMonth + "-01 00:00:00");
                ViewState["YC"] = dt;
                ViewState["YM"] = dt.AddMonths(1).AddSeconds(-1);
                #endregion

                //
                lblTableName.Text = oListPage.TableDetail.TableName;
                //


                this.RefreshGrid();
                //GridExcel.Visible = true;
            }

        }
        override protected void OnInit(System.EventArgs e)
        {
            oListPage = new Epoint.MisBizLogic2.Web.ListPage(TableID, Datagrid1, controlHolder, Pager, null);//���û�е���Excel��Grid��������Ϊnull
            oListPage.CustomMode = true;
            //�˷������ɾ����λ���⡣����ʹ��ѯ������ֵ�Զ���ViewState�лָ���
            controlHolder.Controls.Add(oListPage.RenderSearchCondition());

            base.OnInit(e);
        }
        protected bool GetDeleteState(object oXiangMuZT)
        {
            string XiangMuZT = oXiangMuZT.ToString();
            if (XiangMuZT == "��ĿԴ����")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (btnSearch.Text == "������")
            {
                btnSearch.Text = "�ر�����";
                td2.Style.Add("display", "");
            }
            else
            {
                btnSearch.Text = "������";
                td2.Style.Add("display", "none");
            }
            
        }
        
        //ȡ�ù���ƽ̨
        protected string rtn_GuanLiPT()
        {

            string GuanLiPT = "'-1',";
            if (s_GuanLiPT.Items.Count > 0)
            {
                for (int i = 0; i < s_GuanLiPT.Items.Count; i++)
                {
                    if (s_GuanLiPT.Items[i].Selected)
                    {
                        GuanLiPT += "'" + s_GuanLiPT.Items[i].Value + "',";
                    }
                }
            }

            if (GuanLiPT.Length > 1)
            {
                GuanLiPT = GuanLiPT.Substring(0, GuanLiPT.Length - 1);
            }
            return GuanLiPT;

        }

        
        //ȡ�ù���ƽ̨  �޶���ǰ��Ա�Ĺ���ƽ̨Ȩ��
        protected string rtn_GuanLiPT_ForAll()
        {

            DataView dv_GLPT = new SVGBLL.Pro_Fund().Select_guanLiPT_byUserGuid(Session["UserGuid"].ToString());

            string GuanLiPT = "'-1',";
            if (dv_GLPT.Count > 0)
            {
                for (int i = 0; i < dv_GLPT.Count; i++)
                {

                    GuanLiPT += "'" + dv_GLPT[i]["rowguid"].ToString() + "',";

                }
            }

            if (GuanLiPT.Length > 1)
            {
                return GuanLiPT.Substring(0, GuanLiPT.Length - 1);
            }
            return GuanLiPT;

        }

        //ȡ����Ŀ״̬
        protected string rtn_xiangMuZT()
        {
            string xiangMuZTList = "";
            if (s_xiangMuZT.Items.Count > 0)
            {
                for (int i = 0; i < s_xiangMuZT.Items.Count; i++)
                {
                    if (s_xiangMuZT.Items[i].Selected)
                    {
                        xiangMuZTList += "'" + s_xiangMuZT.Items[i].Value + "',";
                    }
                }
            }
            return xiangMuZTList.Substring(0, xiangMuZTList.Length - 1);

        }
        protected void btnOK_Click(object sender, System.EventArgs e)
        {
            Pager.CurrentPageIndex = 0;
            this.RefreshGrid();
        }

        //�󶨱���
        private void RefreshGrid()
        {
            string str = " and s_Pro_Type='��ҵ��'";

            if (s_xiangMuJL.Text != "")
            {
                str += " and s_xiangMuJL like '%" + this.s_xiangMuJL.Text.Replace("'", "''").Replace("_", "\\_").Replace("%", "\\%") + "%'";
            }

            if (s_xiangMuMC.Text != "")
            {
                str += " and s_xiangMuMC like '%" + this.s_xiangMuMC.Text.Replace("'", "''").Replace("_", "\\_").Replace("%", "\\%") + "%'";
            }

            if (s_xiangMuGSQC.Text != "")
            {
                str += " and s_xiangMuGSQC like '%" + this.s_xiangMuGSQC.Text.Replace("'", "''").Replace("_", "\\_").Replace("%", "\\%") + "%'";
            }

            if (this.s_xiangmuBH.Text.Trim() != "")
            {
                str += " and s_xiangmuBH like '%" + this.s_xiangmuBH.Text.Trim().Replace("'", "''").Replace("_", "\\_").Replace("%", "\\%") + "%'";
            }

            if (s_LiXiangSQR.Text != "")
            {
                str += " and s_LiXiangSQR like '%" + this.s_LiXiangSQR.Text.Replace("'", "''").Replace("_", "\\_").Replace("%", "\\%") + "%'";
            }

            if (s_dengJiR.Text != "")
            {
                str += " and s_dengJiR like '%" + this.s_dengJiR.Text.Replace("'", "''").Replace("_", "\\_").Replace("%", "\\%") + "%'";
            }

            if (this.d_dengJiRQ.FromText.ToString() != "")
            {
                str += " and d_dengJiRQ>'" + DateTime.Parse(this.d_dengJiRQ.FromText.ToString()).AddDays(-1) + "'";
            }
            if (this.d_dengJiRQ.ToText.ToString() != "")
            {
                str += " and d_dengJiRQ<'" + DateTime.Parse(this.d_dengJiRQ.ToText.ToString()).AddDays(1) + "'";
            }
            //����ʱ��
            if (this.d_TouZiJCSJ.FromText.ToString() != "")
            {
                str += " and d_TouZiJCSJ>'" + DateTime.Parse(this.d_TouZiJCSJ.FromText.ToString()).AddHours(-2) + "'";
            }
            if (this.d_TouZiJCSJ.ToText.ToString() != "")
            {
                str += " and d_TouZiJCSJ<'" + DateTime.Parse(this.d_TouZiJCSJ.ToText.ToString()).AddHours(23) + "'";
            }
            if (this.s_suoShuHY.Text.ToString() != "")
            {
                str += " and s_suoShuHY='" + this.s_suoShuHY.Text.ToString() + "'";
            }
            if (this.s_xiangMuLY.SelectedValue.ToString() != "")
            {
                str += " and s_xiangMuLY_Drop=" + this.s_xiangMuLY.SelectedValue.ToString();
            }
            if (this.s_GuanLiPT.SelectedValue.ToString() != "")
            {
                string GuanLiPT = rtn_GuanLiPT();

                str += " and s_guanLiPT in (" + GuanLiPT + ")";

            }
            if (this.s_xiangMuZT.SelectedValue.ToString() != "")
            {
                string xiangMuZTList = rtn_xiangMuZT();

                str += " and s_xiangMuZT in (" + xiangMuZTList + ")";
            }
            str += " and (s_del_TrueOrFalse is Null or s_del_TrueOrFalse='')";

            string GuanLiPT_All = rtn_GuanLiPT_ForAll();

            str += " and s_guanLiPT in (" + GuanLiPT_All + ")";
            str += " and DWGuid>'' ";
            oListPage.OtherCondition = str;
            oListPage.TableDetail.SQL_TableName = "VIEW_CurrentVersion";//�ҵ����°汾����ͼ
            oListPage.SortExpression = "d_dengJiRQ desc";
            //oListPage.SortExpression = "Row_ID";
            oListPage.GenerateSearchResult();

            string strSql = string.Format(" select * from  VIEW_CurrentVersion where 1=1 {0} order by {1}", str, "d_dengJiRQ desc");
            GridExcel.DataSource = Epoint.MisBizLogic2.DB.ExecuteDataView(strSql);
            GridExcel.DataBind();
        }


        //��ֹ��Ŀ
        protected void btnStop_Click(object sender, System.EventArgs e)
        {
            Epoint.MisBizLogic2.Data.MisGuidRow oRow;
            CheckBox chk;
            string RowGuid;
            for (int i = 0; i < Datagrid1.Items.Count; i++)
            {
                chk = (CheckBox)Datagrid1.Items[i].FindControl("chkAdd");
                if (chk.Checked)
                {
                    RowGuid = Convert.ToString(Datagrid1.DataKeys[i]);
                    oRow = new Epoint.MisBizLogic2.Data.MisGuidRow(TableName, RowGuid);
                    oRow["s_xiangMuZT"] = "����ֹ";
                    oRow.Update();
                }

            }
            this.RefreshGrid();
        }



        protected void btnDel_Click(object sender, System.EventArgs e)
        {
            Epoint.MisBizLogic2.Data.MisGuidRow oRow;
            CheckBox chk;
            string RowGuid;
            for (int i = 0; i < Datagrid1.Items.Count; i++)
            {
                chk = (CheckBox)Datagrid1.Items[i].FindControl("chkAdd");
                if (chk.Checked)
                {
                    RowGuid = Convert.ToString(Datagrid1.DataKeys[i]);
                    oRow = new Epoint.MisBizLogic2.Data.MisGuidRow(TableName, RowGuid);
                    oRow["s_del_TrueOrFalse"] = "Del"; //��1������ɾ�� 
                    oRow.Update();
                }

            }
            this.RefreshGrid();
        }


        protected void Pager_PageChanged(object src, Wuqi.Webdiyer.PageChangedEventArgs e)
        {
            Pager.CurrentPageIndex = e.NewPageIndex;
            this.RefreshGrid();
        }

        protected void Datagrid1_SortCommand(object source, DataGridSortCommandEventArgs e)
        {
            oListPage.PrepareForSortCommand(e.SortExpression);
            Pager.CurrentPageIndex = 0;
            this.RefreshGrid();
        }

        protected void Datagrid1_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            oListPage.GenerateSerialNumColumn(e.Item);
        }
        protected void btnTest_ServerClick(object sender, System.EventArgs e)
        {
            if (hidSelectedFields.Value != "")
            {
                string str = " and s_Pro_Type='��ҵ��'";

                if (s_xiangMuJL.Text != "")
                {
                    str += " and s_xiangMuJL like '%" + this.s_xiangMuJL.Text.Replace("'", "''").Replace("_", "\\_").Replace("%", "\\%") + "%'";
                }

                if (s_xiangMuMC.Text != "")
                {
                    str += " and s_xiangMuMC like '%" + this.s_xiangMuMC.Text.Replace("'", "''").Replace("_", "\\_").Replace("%", "\\%") + "%'";
                }

                if (s_xiangMuGSQC.Text != "")
                {
                    str += " and s_xiangMuGSQC like '%" + this.s_xiangMuGSQC.Text.Replace("'", "''").Replace("_", "\\_").Replace("%", "\\%") + "%'";
                }

                if (this.s_xiangmuBH.Text.Trim() != "")
                {
                    str += " and s_xiangmuBH like '%" + this.s_xiangmuBH.Text.Trim().Replace("'", "''").Replace("_", "\\_").Replace("%", "\\%") + "%'";
                }

                if (s_LiXiangSQR.Text != "")
                {
                    str += " and s_LiXiangSQR like '%" + this.s_LiXiangSQR.Text.Replace("'", "''").Replace("_", "\\_").Replace("%", "\\%") + "%'";
                }

                if (s_dengJiR.Text != "")
                {
                    str += " and s_dengJiR like '%" + this.s_dengJiR.Text.Replace("'", "''").Replace("_", "\\_").Replace("%", "\\%") + "%'";
                }

                if (this.d_dengJiRQ.FromText.ToString() != "")
                {
                    str += " and d_dengJiRQ>'" + DateTime.Parse(this.d_dengJiRQ.FromText.ToString()).AddDays(-1) + "'";
                }
                if (this.d_dengJiRQ.ToText.ToString() != "")
                {
                    str += " and d_dengJiRQ<'" + DateTime.Parse(this.d_dengJiRQ.ToText.ToString()).AddDays(1) + "'";
                }
                if (this.s_suoShuHY.Text.ToString() != "")
                {
                    str += " and s_suoShuHY='" + this.s_suoShuHY.Text.ToString() + "'";
                }
                if (this.s_xiangMuLY.SelectedValue.ToString() != "")
                {
                    str += " and s_xiangMuLY_Drop=" + this.s_xiangMuLY.SelectedValue.ToString();
                }
                if (this.s_GuanLiPT.SelectedValue.ToString() != "")
                {
                    string GuanLiPT = rtn_GuanLiPT();

                    str += " and s_guanLiPT in (" + GuanLiPT + ")";

                }
                if (this.s_xiangMuZT.SelectedValue.ToString() != "")
                {
                    string xiangMuZTList = rtn_xiangMuZT();

                    str += " and s_xiangMuZT in (" + xiangMuZTList + ")";
                }
                str += " and (s_del_TrueOrFalse is Null or s_del_TrueOrFalse='')";

                string GuanLiPT_All = rtn_GuanLiPT_ForAll();

                str += " and s_guanLiPT in (" + GuanLiPT_All + ")";
                //�汾����
                str += " and VersionGuid in ( select distinct VersionGuid from SVG_Pro_Version where IsCurrentVersion = 1  ) ";
                //��ȡ�ж�����
                oListPage.OtherCondition = str;
                oListPage.GenerateExcelGrid_SelectColumns(this.lblTableName.Text,
                   "",
                   "",
                   0,
                   hidSelectedFields.Value
                   );
            }
            else
            {
                this.AlertAjaxMessage("��ѡ�񵼳����ֶΣ�");
            }
        }
        protected void GridExcel_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                for (int i = 0; i < e.Item.Cells.Count; i++)
                    e.Item.Cells[i].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            }
        }


        //����
        protected void btnExcel_ServerClick(object sender, System.EventArgs e)
        {
            //�����Ǵ�ӡ����Ϣ
            int nTemplateID = 7; //��ӡģ��ID
            bool blnPreview = true; //�Ƿ���ҪԤ��
            //�Ȼ�ȡ������������Ŀ
            #region �Ȼ�ȡ������������Ŀ
            string str = " and s_Pro_Type='��ҵ��'";

            if (s_xiangMuJL.Text != "")
            {
                str += " and s_xiangMuJL like '%" + this.s_xiangMuJL.Text.Replace("'", "''").Replace("_", "\\_").Replace("%", "\\%") + "%'";
            }

            if (s_xiangMuMC.Text != "")
            {
                str += " and s_xiangMuMC like '%" + this.s_xiangMuMC.Text.Replace("'", "''").Replace("_", "\\_").Replace("%", "\\%") + "%'";
            }

            if (s_xiangMuGSQC.Text != "")
            {
                str += " and s_xiangMuGSQC like '%" + this.s_xiangMuGSQC.Text.Replace("'", "''").Replace("_", "\\_").Replace("%", "\\%") + "%'";
            }

            if (s_LiXiangSQR.Text != "")
            {
                str += " and s_LiXiangSQR like '%" + this.s_LiXiangSQR.Text.Replace("'", "''").Replace("_", "\\_").Replace("%", "\\%") + "%'";
            }

            if (s_dengJiR.Text != "")
            {
                str += " and s_dengJiR like '%" + this.s_dengJiR.Text.Replace("'", "''").Replace("_", "\\_").Replace("%", "\\%") + "%'";
            }

            if (this.d_dengJiRQ.FromText.ToString() != "")
            {
                str += " and d_dengJiRQ>'" + DateTime.Parse(this.d_dengJiRQ.FromText.ToString()).AddDays(-1) + "'";
            }
            if (this.d_dengJiRQ.ToText.ToString() != "")
            {
                str += " and d_dengJiRQ<'" + DateTime.Parse(this.d_dengJiRQ.ToText.ToString()).AddDays(1) + "'";
            }
            if (this.s_suoShuHY.Text.ToString() != "")
            {
                str += " and s_suoShuHY='" + this.s_suoShuHY.Text.ToString() + "'";
            }
            if (this.s_xiangMuLY.SelectedValue.ToString() != "")
            {
                str += " and s_xiangMuLY_Drop=" + this.s_xiangMuLY.SelectedValue.ToString();
            }
            if (this.s_GuanLiPT.SelectedValue.ToString() != "")
            {
                string GuanLiPT = rtn_GuanLiPT();

                str += " and s_guanLiPT in (" + GuanLiPT + ")";

            }
            if (this.s_xiangMuZT.SelectedValue.ToString() != "")
            {
                string xiangMuZTList = rtn_xiangMuZT();

                str += " and s_xiangMuZT in (" + xiangMuZTList + ")";
            }
            str += " and (s_del_TrueOrFalse is Null or s_del_TrueOrFalse='')";

            string GuanLiPT_All = rtn_GuanLiPT_ForAll();

            str += " and s_guanLiPT in (" + GuanLiPT_All + ")";

            DataView dvProjects = new SVGBLL.Pro_Fund().GetProjectsForPrint(str);

            #endregion
            StringBuilder sb = new StringBuilder();
            sb.Append("<��ӡ����>");
            sb.Append("<ҳ>");
            sb.Append("<����>");
            sb.Append("<��չ��>");
            sb.Append("<��Ŀ��>");//��Ŀ��ϸΪ��չ�������


            for (int i = 0; i < dvProjects.Count; i++)
            {

                sb.Append("<Row>");
                //����
                sb.Append("<Num>" + (i+1).ToString() + "</Num>");
                sb.Append("<guanLiPT>" + new SVGBLL.Pro_Fund().Get_GuanLiPTByGuanLiPTGuid( dvProjects[i]["s_guanLiPT"].ToString()) + "</guanLiPT>");
                sb.Append("<chuZiZhuTi>" + dvProjects[i]["s_chuZiZhuTi"].ToString() + "</chuZiZhuTi>");
                sb.Append("<xiangMuBH>" + dvProjects[i]["s_xiangMuBH"].ToString() + "</xiangMuBH>");
                sb.Append("<xiangMuMC>" + dvProjects[i]["s_xiangMuMC"].ToString() + "</xiangMuMC>");
                sb.Append("<xiangMuJL>" + dvProjects[i]["s_xiangMuJL"].ToString() + "</xiangMuJL>");
                sb.Append("<xiangMuZT>" + dvProjects[i]["s_xiangMuZT"].ToString() + "</xiangMuZT>");
                sb.Append("<xiangMuGSQC>" + dvProjects[i]["s_xiangMuGSQC"].ToString() + "</xiangMuGSQC>");
                
                sb.Append("<LiXiangPSSJ>" + dvProjects[i]["d_JZDCPSSJ"].ToString() + "</LiXiangPSSJ>");
                sb.Append("<JueCePSSJ>" + dvProjects[i]["d_JueCePSSJ"].ToString() + "</JueCePSSJ>");
                
                //���ݽ���
                sb.Append("</Row>");
            }


            sb.Append("</��Ŀ��>");
            sb.Append("</��չ��>");
            sb.Append("</����>");
            sb.Append("</ҳ>");
            sb.Append("</��ӡ����>");

            //hidXmlData.Value = sb.ToString();

            //Epoint.MisBizLogic2.Print.PrintExecuter.PrintPage(nTemplateID, blnPreview, this, hidXmlData.ClientID);//ǰ����������Ϊ����ӡģ��ID���Ƿ���ʾԤ��ҳ�档
        }

        #region ��ȡ״̬
        /// <summary>
        /// ��ȡ��ҵ��Ϣ״̬
        /// </summary>
        /// <param name="DWGuid"></param>
        /// <returns></returns>
        protected string GetDWStatus(object DWGuid)
        {
            string strSql = string.Format(" SELECT TOP(1)STATUS FROM RG_OUInfo WHERE DWGUID='{0}' ORDER BY ROW_ID DESC  ", DWGuid);
            return OUStatus.GetTextByValue(Epoint.MisBizLogic2.DB.ExecuteToString(strSql));
            //return "";
        }

        /// <summary>
        /// ��ȡ��̬��Ϣ
        /// </summary>
        /// <param name="DWGuid"></param>
        /// <returns></returns>
        protected string GetDTStatus(object DWGuid)
        {
            string strSql = string.Format(" SELECT COUNT(*) FROM RG_DongTaiInfo WHERE DWGUID='{0}' AND UpdateTime>='{1}' AND UpdateTime<='{1}'   ",
                DWGuid, ViewState["YC"], ViewState["YM"]);
            if (Epoint.MisBizLogic2.DB.ExecuteToInt(strSql) == 0)
            {
                return "<span style='color:red'>����δ�</span>";
            }
            strSql = string.Format(" SELECT TOP(1)STATUS FROM RG_DongTaiInfo WHERE DWGUID='{0}' AND UpdateTime>='{1}' AND UpdateTime<='{1}' ORDER BY ROW_ID DESC  ",
                DWGuid, ViewState["YC"], ViewState["YM"]);
            return OUStatus.GetTextByValue(Epoint.MisBizLogic2.DB.ExecuteToString(strSql));
            //return "";
        }

        /// <summary>
        /// ��ȡ�¶Ȳ�����Ϣ
        /// </summary>
        /// <param name="DWGuid"></param>
        /// <returns></returns>
        protected string GetCWStatus(object DWGuid)
        {
            //ע�⻹���µ׵�����
            string strSql = string.Format(" SELECT COUNT(*) FROM RG_YueDuCaiWu WHERE DWGUID='{0}' AND YEAR='{1}' AND MONTH='{2}'   ", 
                DWGuid,DateTime.Now.Year,DateTime.Now.Month);
            if (Epoint.MisBizLogic2.DB.ExecuteToInt(strSql) == 0)
            {
                return "<span style='color:red'>����δ�</span>";
            }
            strSql = string.Format(" SELECT TOP(1)STATUS FROM RG_YueDuCaiWu WHERE DWGUID='{0}' AND YEAR='{1}' AND MONTH='{2}' ORDER BY ROW_ID DESC  ", 
                DWGuid,DateTime.Now.Year,DateTime.Now.Month);
            return OUStatus.GetTextByValue(Epoint.MisBizLogic2.DB.ExecuteToString(strSql));
            //return "";
        }

        protected string GetRSStatus(object DWGuid)
        {
            //ע�⻹�м��ȵ�����
            string strSql = string.Format(" SELECT COUNT(*) FROM RG_RenShiInfo WHERE DWGUID='{0}' AND s_qijian='{1}'   ",
                DWGuid, ViewState["JD"]);
            if (Epoint.MisBizLogic2.DB.ExecuteToInt(strSql) == 0)
            {
                return "<span style='color:red'>������δ�</span>";
            }
            strSql = string.Format(" SELECT TOP(1)STATUS FROM RG_RenShiInfo WHERE DWGUID='{0}' AND s_qijian='{1}' ORDER BY ROW_ID DESC  ",
                DWGuid, ViewState["JD"]);
            return OUStatus.GetTextByValue(Epoint.MisBizLogic2.DB.ExecuteToString(strSql));
            //return "";
        }

        protected string GetOtherStatus(object DWGuid)
        {
            //ע�⻹���¶ȵ�����
            string strSql = string.Format(" SELECT COUNT(*) FROM RG_OtherInfo WHERE DWGUID='{0}' AND UpdateTime>='{1}' AND UpdateTime<='{1}'  ",
                DWGuid, ViewState["YC"], ViewState["YM"]);
            if (Epoint.MisBizLogic2.DB.ExecuteToInt(strSql) == 0)
            {
                return "<span style='color:red'>����δ�</span>";
            }
            strSql = string.Format(" SELECT TOP(1)STATUS FROM RG_OtherInfo WHERE DWGUID='{0}' AND UpdateTime>='{1}' AND UpdateTime<='{1}' ORDER BY ROW_ID DESC  ",
                DWGuid, ViewState["YC"], ViewState["YM"]);
            return OUStatus.GetTextByValue(Epoint.MisBizLogic2.DB.ExecuteToString(strSql));
            //return "";
        }

        protected string GetDQStatus(object DWGuid)
        {
            //ע�⻹���¶ȵ�����
            string strSql = string.Format(" SELECT COUNT(*) FROM RG_DingQi WHERE DWGUID='{0}' AND d_qijian='{1}'  ",
                DWGuid, ViewState["YC"]);
            if (Epoint.MisBizLogic2.DB.ExecuteToInt(strSql) == 0)
            {
                return "<span style='color:red'>����δ�</span>";
            }
            strSql = string.Format(" SELECT TOP(1)s_STATUS FROM RG_DingQi WHERE DWGUID='{0}' AND d_qijian='{1}' ORDER BY ROW_ID DESC  ",
                DWGuid, ViewState["YC"]);
            return DQStatus.GetTextByValue(Epoint.MisBizLogic2.DB.ExecuteToString(strSql));
            //return "";
        }
        #endregion
    }
}

