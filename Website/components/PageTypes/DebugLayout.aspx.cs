using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using SandboxSitecore.Cms.Core;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Links;
using Sitecore.Resources.Media;

namespace SandboxSitecore.Cms.Components.PageTypes
{

public partial class DebugLayout : System.Web.UI.Page
{

#region Properties

    public string HtmlFramePostfix {get;set;}

#endregion

    protected void Page_Init(object sender, EventArgs e)
    {
        HtmlFramePostfix = "1";
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        this.Page.Header.DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Header.Title = Sitecore.Context.Item["Identity_Title"];

        var sbDebug = new StringBuilder();
        sbDebug.Append("<div>Debug:</div>");
        sbDebug.AppendFormat("<div>{0}</div>", Sitecore.Context.Item["Identity_Title"]);
        sbDebug.AppendFormat("<div>{0}</div>", Sitecore.Context.Item["Identity_Body"]);
        sbDebug.Append("<br/>");
        litDebug.Text = sbDebug.ToString();

        var dataParent1 = Sitecore.Context.Database.Items["/sitecore/content/home/news"];
        if(dataParent1.Children != null && dataParent1.Children.Count > 0) {
            rptrItems.DataSource = dataParent1.Children;
            rptrItems.DataBind();
        }
    }

    protected void rptrItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        // This event is raised for the header, the footer, separators, and items.
        if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {
            var item = e.Item.DataItem as Sitecore.Data.Items.Item;
            var sbItem = new StringBuilder();
            var sbItemDebug = new StringBuilder();
            var litItem = e.Item.FindControl("litItem") as Literal;
            var litItemDebug = e.Item.FindControl("litItemDebug") as Literal;

            sbItem.AppendLine("<div>Debug:</div>");
            sbItem.AppendFormat("<div>{0}</div>", item["News_Title"]);
            sbItem.AppendFormat("<div>{0}</div>", item["News_Body"]);
            sbItem.AppendFormat("<div>{0}</div>", item["News_Body"]);
            sbItem.AppendFormat("<div>{0}</div>", item.GetItemUrl().ToAbsoluteUrl());
            litItem.Text = sbItem.ToString();

            sbItemDebug.AppendFormat("<div>{0}</div>", LinkManager.GetItemUrl(item));
            sbItemDebug.Append("<br/>");
            litItemDebug.Text = sbItemDebug.ToString();
        }
    }
}

} // END namespace SandboxSitecore.Cms.Components.PageTypes