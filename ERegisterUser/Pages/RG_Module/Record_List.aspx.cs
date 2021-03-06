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
using Epoint.RegisterUser.DataSyn.Bizlogic;

namespace EpointRegisterUser.Pages.RG_Module
{

    public partial class Record_List : Epoint.Frame.Bizlogic.BaseContentPage_UsingMaster
    {
        Epoint.RegisterUser.Front.Bizlogic.RegisterModule.RegisterModule rgModule = new Epoint.RegisterUser.Front.Bizlogic.RegisterModule.RegisterModule();

        public int TableID = 2015;
        Epoint.MisBizLogic2.Web.ListPage oListPage;
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                lblTableName.Text = oListPage.TableDetail.TableName;
                this.CurrentHead = oListPage.TableDetail.TableName;

                #region 是否联机表
                if (oListPage.TableDetail.Is_OnlineTable == "1")
                {
                    btnAddRecord.Visible = false;
                    btnDel.Visible = false;
                }
                else
                {
                    btnAddRecord.Visible = true;
                    btnDel.Visible = true;
                }
                #endregion

                this.RefreshGrid();
                if (Request["AppGuid"] == "all")
                {
                    addbutton.Style["display"] = "none";
                    quickaddbutton.Style["display"] = "none";
                }
                else
                {
                    addbutton.Style["display"] = "";
                    quickaddbutton.Style["display"] = "";
                }
            }
        }

        override protected void OnInit(System.EventArgs e)
        {
            oListPage = new Epoint.MisBizLogic2.Web.ListPage(TableID, Datagrid1, controlHolder, Pager, GridExcel);//如果没有导出Excel的Grid，就设置为null


            //此处添加全局通用条件 
            string sqlstr = "and ParentGuid='" + Request.QueryString["ParentGuid"] + "'";
            if (!string.IsNullOrEmpty(Request["IsBelongtoApp"]))
                sqlstr += "and IsBelongtoApp='" + Request["IsBelongtoApp"] + "'";
            if (!string.IsNullOrEmpty(Request["AppGuid"]))
            {
                if (Request["AppGuid"] == "all")
                    sqlstr += "and AppGuid!=''";
                else
                    sqlstr += "and AppGuid='" + Request["AppGuid"] + "'";
            }
            oListPage.OtherCondition = sqlstr;
            oListPage.SortExpression = " Order by OrderNum Desc ";
            oListPage.CustomMode = true;
            //此方法解决删除错位问题。可以使查询条件的值自动从ViewState中恢复。
            controlHolder.Controls.Add(oListPage.RenderSearchCondition());

            base.OnInit(e);
        }


        protected void btnOK_Click(object sender, System.EventArgs e)
        {
            Pager.CurrentPageIndex = 0;
            this.RefreshGrid();
        }


        private void RefreshGrid()
        {
            oListPage.GenerateSearchResult();
        }


        protected void btnDel_Click(object sender, System.EventArgs e)
        {
            CheckBox chk;
            for (int i = 0; i < Datagrid1.Items.Count; i++)
            {
                chk = (CheckBox)Datagrid1.Items[i].FindControl("chkAdd");
                if (chk.Checked)
                {
                    DeleteChildModule(Convert.ToString(Datagrid1.DataKeys[i]));
                }
            }
            this.RefreshGrid();
        }

        /// <summary>
        /// 递归删除子模块和本身以及模块的权限
        /// </summary>
        /// <param name="parentGuid"></param>
        private void DeleteChildModule(string parentGuid)
        {
            DataView dv = Epoint.MisBizLogic2.DB.ExecuteDataView("select RowGuid from RG_Module where ParentGuid='" + parentGuid + "'");
            foreach (DataRowView row in dv)
            {
                DeleteChildModule(row["RowGuid"].ToString());
            }

            new ComDataSyn().DeleteWithKeyValue(DataSynTarget.BackEndToFront, "RG_Module", "RowGuid", parentGuid);

            Epoint.MisBizLogic2.Data.CommonDataTable.DeleteRecord_FromSqlTable(
                       oListPage.TableID,
                       oListPage.TableDetail.SQL_TableName,
                       parentGuid
                       );
            new ComDataSyn().DeleteWithKeyValue(DataSynTarget.BackEndToFront, "RG_Module_Right", "ModuleGuid", parentGuid);

            Epoint.MisBizLogic2.DB.ExecuteNonQuery("delete from RG_Module_Right where ModuleGuid='" + parentGuid + "'");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (btnSearch.Text == "打开搜索")
            {
                btnSearch.Text = "关闭搜索";
                tdCl.Style.Add("display", "");
            }
            else
            {
                btnSearch.Text = "打开搜索";
                tdCl.Style.Add("display", "none");
            }

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
        protected void Datagrid1_ItemDataBound(object sender, DataGridItemEventArgs e) 
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                string ModuleGuid = ((Label)e.Item.FindControl("lblRGuid")).Text;
                bool isExists = Epoint.MisBizLogic2.DB.ExecuteToInt("select COUNT(*) FROM RG_Module_Right WHERE AllowGuid='All' and ModuleGuid='" + ModuleGuid + "'") > 0;
                CheckBox chkAllowToAll = (CheckBox)e.Item.FindControl("chkAdd2");
                if (isExists)
                {
                    chkAllowToAll.Checked = true;
                }
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
        protected void btnQuickNew_Click(object sender, EventArgs e)
        {
            string RowGuid = Guid.NewGuid().ToString();
            Epoint.MisBizLogic2.Data.MisGuidRow orow = new Epoint.MisBizLogic2.Data.MisGuidRow("RG_Module", RowGuid);
            orow["ParentGuid"] = Request["ParentGuid"];
            orow["ModuleName"] = "模块名称";
            orow["IsForbid"] = 0;
            orow["OrderNum"] = -1;
            orow["IsBlank"] = 0;
            orow["IsHidden"] = 0;
            orow["IsBelongtoBus"] = 0;
            orow["ModuleAddress"] = "";
            orow["SmallImgAddress"] = "";
            orow["BigImgAddress"] = "";
            orow["OperateUserName"] = Session["DisplayName"];
            orow["OperateDate"] = System.DateTime.Now;
            orow["YearFlag"] = "";
            orow["HidUrl"] = "";
            orow["IsBelongtoApp"] = 0;
            if (Request["ParentGuid"] == "" && rgModule.TopModuleCode("", 4) == "")
                orow["ModuleCode"] = "0001";
            else if (Request["ParentGuid"] == "")
                orow["ModuleCode"] = (Convert.ToInt64(rgModule.TopModuleCode("", 4)) + 1).ToString().PadLeft(4, '0');
            else
            {
                string ParentModuleCode = Epoint.MisBizLogic2.DB.ExecuteToString("select ModuleCode from RG_Module where RowGuid='" + Request["ParentGuid"] + "'");
                int CodeLength = ParentModuleCode.Length + 4;
                string LastModuleCode = rgModule.TopModuleCode(ParentModuleCode, CodeLength);//取得上一个ModuleCode
                if (string.IsNullOrEmpty(LastModuleCode))
                    LastModuleCode = ParentModuleCode + "0000";
                orow["ModuleCode"] = (Convert.ToInt64(LastModuleCode) + 1).ToString().PadLeft(CodeLength, '0');
            }
            if (string.IsNullOrEmpty(Request["AppGuid"]))
                orow["AppGuid"] = "";
            else
                orow["AppGuid"] = Request["AppGuid"];
            orow.Insert();
            new ComDataSyn().InsertWithKeyValue(DataSynTarget.BackEndToFront, "RG_Module", "RowGuid", RowGuid);
            this.RefreshGrid();
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Datagrid1.Items.Count; i++)
            {
                TextBox txtName = (TextBox)Datagrid1.Items[i].FindControl("txtName");
                TextBox txtAddress = (TextBox)Datagrid1.Items[i].FindControl("txtAddress");
                TextBox txtOrderNum = (TextBox)Datagrid1.Items[i].FindControl("txtOrderNum");
                string gd = Convert.ToString(Datagrid1.DataKeys[i]);
                Epoint.MisBizLogic2.Data.MisGuidRow orow = new Epoint.MisBizLogic2.Data.MisGuidRow("RG_Module", gd);
                orow["ModuleName"] = txtName.Text;
                orow["ModuleAddress"] = txtAddress.Text;
                orow["OrderNum"] = txtOrderNum.Text;
                orow.Update();
                new ComDataSyn().UpdateWithKeyValue(DataSynTarget.BackEndToFront, "RG_Module", "RowGuid", gd);


                string ModuleGuid = gd;
                bool isExists = Epoint.MisBizLogic2.DB.ExecuteToInt("select COUNT(*) FROM RG_Module_Right WHERE AllowGuid='All' and ModuleGuid='" + ModuleGuid + "'") > 0;
                CheckBox chkAllowToAll = (CheckBox)Datagrid1.Items[i].FindControl("chkAdd2");
                if (chkAllowToAll.Checked)
                {
                    // 添加完全公开的配置
                    // 判断是否存在,如果存在,不需要添加配置
                    if (!isExists)
                    {
                        string RowGuid = Guid.NewGuid().ToString();
                        Epoint.MisBizLogic2.Data.MisGuidRow orow2 = new Epoint.MisBizLogic2.Data.MisGuidRow("RG_Module_Right", RowGuid);
                        orow2["ModuleGuid"] = ModuleGuid;
                        orow2["AllowGuid"] = "All";
                        orow2["AllowType"] = "";
                        orow2.Insert();
                        new ComDataSyn().InsertWithKeyValue(DataSynTarget.BackEndToFront, "RG_Module_Right", "RowGuid", RowGuid);
                    }
                }
                else
                {
                    // 删除完全公开的配置 
                    new ComDataSyn().DeleteWithCondition(DataSynTarget.BackEndToFront, "RG_Module_Right", "AllowGuid='All'and ModuleGuid='" + ModuleGuid + "'", "RowGuid");
                    Epoint.MisBizLogic2.DB.ExecuteNonQuery("delete from RG_Module_Right WHERE AllowGuid='All'and ModuleGuid='" + ModuleGuid + "'");
                }
            }

        }

        /// <summary>
        /// 生成模块菜单的脚本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGenerateSQL_Click(object sender, EventArgs e)
        {
            CheckBox chkSel;
            txtSQL.Text = "";
            for (int i = 0; i < Datagrid1.Items.Count; i++)
            {
                chkSel = (CheckBox)Datagrid1.Items[i].FindControl("chkAdd");
                if (chkSel.Checked)
                {
                    string strSQL;
                    Epoint.MisBizLogic2.Data.MisGuidRow oRow = new Epoint.MisBizLogic2.Data.MisGuidRow("RG_Module", Datagrid1.DataKeys[i].ToString());
                    strSQL = "if not exists(select 1 from  RG_Module where RowGuid = '" + oRow["RowGuid"] + "')\r\n";
                    strSQL += " begin \r\n";
                    strSQL += " insert into RG_Module(IsForbid, ParentGuid, OrderNum, ModuleName, ModuleAddress, SmallImgAddress, BigImgAddress, BelongXiaQuCode, OperateUserName, OperateDate, YearFlag, RowGuid, IsBlank,IsHidden,IsBelongtoApp,ModuleCode,IsBelongtoBus)";
                    strSQL += " values('" + oRow["IsForbid"] + "',";
                    strSQL += "'" + oRow["ParentGuid"] + "',";
                    strSQL += "'" + oRow["OrderNum"] + "',";
                    strSQL += "'" + oRow["ModuleName"] + "',";
                    strSQL += "'" + oRow["ModuleAddress"] + "',";
                    strSQL += "'" + oRow["SmallImgAddress"] + "',";
                    strSQL += "'" + oRow["BigImgAddress"] + "',";
                    strSQL += "'" + oRow["BelongXiaQuCode"] + "',";
                    strSQL += "'" + oRow["OperateUserName"] + "',";
                    strSQL += "'" + oRow["OperateDate"] + "',";
                    strSQL += "'" + oRow["YearFlag"] + "',";
                    strSQL += "'" + oRow["RowGuid"] + "',";
                    strSQL += "'" + oRow["IsBlank"] + "',";
                    strSQL += "'" + oRow["IsHidden"] + "',";
                    strSQL += "'" + oRow["IsBelongtoApp"] + "',";
                    strSQL += "'" + oRow["ModuleCode"] + "',";
                    strSQL += "'" + oRow["IsBelongtoBus"] + "'";
                    strSQL += ")\r\n";
                    strSQL += " end \r\n";
                    txtSQL.Text += strSQL;
                }
            }
        }
    }
}


