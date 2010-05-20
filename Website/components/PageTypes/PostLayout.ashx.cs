using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using SandboxSitecore.Cms.Core;
using Sitecore.Data.Fields;
using Sitecore.Links;
using Sitecore.Publishing;
using Sitecore.Resources.Media;
using Sitecore.SecurityModel;

namespace SandboxSitecore.Cms.Components.PageTypes
{

/// <summary>
/// Summary description for $codebehindclassname$
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class PostLayout : IHttpHandler
{

#region Properties

    Sitecore.Data.Database CmsDatabaseContext
    {
        get {return Sitecore.Context.Database;}
    }

    Sitecore.Data.Database CmsDatabaseMaster
    {
        get {return Sitecore.Configuration.Factory.GetDatabase("master");}
    }

    Sitecore.Data.Database CmsDatabaseWeb
    {
        get {return Sitecore.Configuration.Factory.GetDatabase("web");}
    }

#endregion

    /// <summary>Process HTTP web requests.</summary>
    public void ProcessRequest(HttpContext context)
    {
        var req = context.Request;
        var res = context.Response;
        var sbContent = new StringBuilder();

        // Set HTTP web response.
        res.Clear();
        res.Cache.SetCacheability(HttpCacheability.Public);
        res.Cache.SetExpires(DateTime.MinValue);
        res.ContentType = "text/plain";
        res.ContentEncoding = System.Text.Encoding.UTF8;

        // Validate query string.
        // Example: http://localhost/postdata?id={D0568599-E9C7-44A8-8143-A8F68CA2BEAE}&field=Photo_Description&value=williamchang
        if(!String.IsNullOrEmpty(req.QueryString["id"]) && !String.IsNullOrEmpty(req.QueryString["field"])) {
            var dataFieldName = req.QueryString["field"];
            var dataParent = CmsDatabaseMaster.GetItem(req.QueryString["id"].ToId());
            sbContent.AppendFormat("CMS Item Id: {0}\n", dataParent.ID);
            sbContent.AppendFormat("CMS Item Field: {0}: {1}\n\n", dataFieldName, dataParent.Fields[dataFieldName].Value);

            // Validate query string.
            if(!String.IsNullOrEmpty(req.QueryString["value"])) {
                // Use CMS security disabler to allow changes.
                using(new SecurityDisabler()) {
                    // Begin editing state.
                    dataParent.Editing.BeginEdit();
                    try {
                        dataParent.Fields[dataFieldName].Value = req.QueryString["value"];
                        sbContent.AppendFormat("Edited: CMS Item Field: {0}: {1}\n", dataFieldName, dataParent.Fields["Photo_Description"].Value);
                    } finally {
                        // End editing state
                        dataParent.Editing.EndEdit();
                    }
                }
                // Publish data to CMS.
                PublishData(dataParent, dataParent.Language, CmsDatabaseMaster, CmsDatabaseWeb);
                sbContent.AppendFormat("\nCMS data published successfully: {0}\n\n", DateTime.Now);
            }
        }

        sbContent.AppendLine("Response: Hello World");
        
        // Set HTTP web response.
        res.Write(sbContent.ToString());
    }

    /// <summary>Publish data to CMS.</summary>
    /// <param name="item">Data item.</param>
    /// <param name="language">Date item language.</param>
    /// <param name="dbMaster">Database source.</param>
    /// <param name="dbWeb">Database target.</param>
    public void PublishData(Sitecore.Data.Items.Item item, Sitecore.Globalization.Language language, Sitecore.Data.Database dbMaster, Sitecore.Data.Database dbWeb)
    {
        // Use CMS security disabler to allow changes.
        using(new SecurityDisabler()) {
            PublishOptions opt = new PublishOptions(dbMaster, dbWeb, PublishMode.SingleItem, language, DateTime.Now);
            opt.RootItem = item;
            opt.Deep = false;
            Publisher pub = new Publisher(opt);
            pub.Publish();
        }
    }

    /// <summary>Gets a value indicating whether another request can use this instance.</summary>
    public bool IsReusable
    {
        get {return false;}
    }
}

} // END namespace SandboxSitecore.Cms.Components.PageTypes