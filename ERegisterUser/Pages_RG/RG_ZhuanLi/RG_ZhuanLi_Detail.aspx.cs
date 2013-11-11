using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace EpointRegisterUser.Pages_RG.RG_ZhuanLi
{
	
	public partial class RG_ZhuanLi_Detail : Epoint.Frame.Bizlogic.BaseContentPage_UsingMaster
	{

	public int TableID=2026;

	Epoint.MisBizLogic2.Web.DetailPage oDetailPage;
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack )
			{

				ViewState ["TableName"]=oDetailPage.TableDetail.TableName;
                Epoint.MisBizLogic2.Data.MisGuidRow oRow = new Epoint.MisBizLogic2.Data.MisGuidRow(oDetailPage.TableDetail.SQL_TableName, Request["RowGuid"]);
				
                if (!oRow.R_HasFilled)
                {			
					//lblMessage.Visible=true;
					this.AlertAjaxMessage ("没有对应的数据记录！");
                    this.WriteAjaxMessage("window.close();");
					return;
				}
				Epoint.MisBizLogic2.Web.CodeGenerator.InitiateControl_DetailPage(oDetailPage, tdContainer, oRow);

                CL_ZLJS.MisRowGuid = Request["RowGuid"];
                CL_ZLJS.MisTableID = TableID;
                CL_ZLJS.ProjectGuid = "";
                CL_ZLJS.Comment = Request["DWGuid"];
                CL_ZLJS.d_TiJiaoSJ = DateTime.Now.ToString();
			
			}
		
		}

        override protected void OnInit(EventArgs e)
        {
            oDetailPage = new Epoint.MisBizLogic2.Web.DetailPage (TableID);            
            base.OnInit(e);
        }
   }
}

