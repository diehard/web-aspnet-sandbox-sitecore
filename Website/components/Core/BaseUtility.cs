using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace SandboxSitecore.Cms.Core
{

public static class BaseUtility
{
    /// <summary>Get file URL.</summary>
    /// <remarks>Extension method.</remarks>
    public static string GetFileUrl(this Sitecore.Data.Fields.Field field)
    {
        if(field != null) {
            var fieldTyped = (Sitecore.Data.Fields.FileField)field;
            if(fieldTyped != null) {
                return fieldTyped.Src;
            }
        }
        return String.Empty;
    }

    /// <summary>Get image URL.</summary>
    /// <remarks>Extension method.</remarks>
    public static string GetImageUrl(this Sitecore.Data.Fields.Field field)
    {
        if(field != null && String.Equals(field.TypeKey, "image")) {
            var fieldTyped = (Sitecore.Data.Fields.ImageField)field;
            if(fieldTyped != null) {
                return fieldTyped.Src;
            }
        }
        return String.Empty;
    }

    /// <summary>Get item URL.</summary>
    /// <remarks>Extension method.</remarks>
    public static string GetItemUrl(this Sitecore.Data.Items.Item item)
    {
        var urlOptions = (Sitecore.Links.UrlOptions)Sitecore.Links.UrlOptions.DefaultOptions.Clone();
        urlOptions.SiteResolving = Sitecore.Configuration.Settings.Rendering.SiteResolving;
        return Sitecore.Links.LinkManager.GetItemUrl(item, urlOptions);
    }

    /// <summary>Get link URL.</summary>
    /// <remarks>Extension method.</remarks>
    public static string GetLinkUrl(this Sitecore.Data.Fields.Field field)
    {
        if(field != null && String.Equals(field.TypeKey, "general link")) {
            var fieldTyped = (Sitecore.Data.Fields.LinkField)field;
            if(fieldTyped != null && !fieldTyped.IsMediaLink) {
                return fieldTyped.Url;
            } else if(fieldTyped.IsMediaLink) {
                return GetFileUrl(field);
            }
        }
        return String.Empty;
    }

    /// <summary>Get media URL.</summary>
    /// <remarks>Extension method.</remarks>
    public static string GetMediaUrl(this Sitecore.Data.Items.Item item)
    {
        return Sitecore.StringUtil.EnsurePrefix('/', Sitecore.Resources.Media.MediaManager.GetMediaUrl(item));
    }

    /// <summary>Is checked (Sitecore.Data.Fields.CheckboxField).</summary>
    /// <remarks>Extension method.</remarks>
    public static bool IsChecked(this Sitecore.Data.Fields.Field field)
    {
        if(field != null) {
            var fieldTyped = (Sitecore.Data.Fields.CheckboxField)field;
            if(fieldTyped != null) {
                return fieldTyped.Checked;
            }
        }
        return false;
    }

    /// <summary>Is checked (Sitecore.Data.Fields.CheckboxField).</summary>
    /// <remarks>Extension method.</remarks>
    public static string IsCheckedToString(this Sitecore.Data.Fields.Field field)
    {
        return IsChecked(field).ToString().ToLower();
    }

    /// <summary>To absolute from relative URL.</summary>
    /// <remarks>Extension method.</remarks>
    public static string ToAbsoluteUrl(this string relativeUrl)
    {
        if(!String.IsNullOrEmpty(relativeUrl)) {
            return Sitecore.Web.WebUtil.GetFullUrl(relativeUrl);
        } else {
            return String.Empty;
        }
    }

    /// <summary>To absolute from relative URL.</summary>
    /// <remarks>Extension method.</remarks>
    public static string ToAbsoluteUrl(this string relativeUrl, HttpRequest req)
    {
        if(req.IsSecureConnection) {
            return string.Format("https://{0}{1}", req.Url.Host, ResolveUrl(relativeUrl));
        } else {
            return string.Format("http://{0}{1}", req.Url.Host, ResolveUrl(relativeUrl));
        }
    }

    /// <summary>Convert string to Sitecore.Data.ID.</summary>
    /// <remarks>Extension method.</remarks>
    public static Sitecore.Data.ID ToId(this string value) {
        if(!String.IsNullOrEmpty(value)) {
            return Sitecore.Data.ID.Parse(value);
        } else {
            return null;
        }
    }

    /// <summary>Convert Sitecore.Data.Fields.Field (Sitecore.Data.Fields.ReferenceField) to Sitecore.Data.Items.Item.</summary>
    /// <remarks>Extension method.</remarks>
    public static Sitecore.Data.Items.Item ToItem(this Sitecore.Data.Fields.Field field) {
        if(field != null) {
            var fieldTyped = (Sitecore.Data.Fields.ReferenceField)field;
            if(fieldTyped != null) {
                return fieldTyped.TargetItem;
            }
        }
        return null;
    }

    /// <summary>Convert string (from request form) to type. If null, return default value.</summary>
    /// <remarks>Extension method.</remarks>
    public static T ToTypeOrDefault<T>(this string value, T defaultValue) where T : struct {
        if(!string.IsNullOrEmpty(value.Trim())) {
            return (T)Convert.ChangeType(value, typeof(T));
        } else {
            return defaultValue;
        }
    }

#region URL Utilties

    /// <summary>
    /// Returns a site relative HTTP path from a partial path starting out with a tilda.
    /// Same syntax that ASP.NET internally supports but this method can be used
    /// outside of the framework.
    /// 
    /// Works like Control.ResolveUrl including support for tilda syntax
    /// but returns an absolute URL.
    /// </summary>
    /// <remarks>http://west-wind.com/weblog/posts/154812.aspx</remarks>
    /// <param name="originalUrl">Any URL including those starting with ~</param>
    public static string ResolveUrl(string originalUrl)
    {
        if(String.IsNullOrEmpty(originalUrl)) {
            return originalUrl;
        }

        // If absolute path, then return.
        if(IsAbsolutePath(originalUrl)) {
            return originalUrl;
        }

        // If no tilda, then return.
        if(!originalUrl.StartsWith("~")) {
            return originalUrl;
        }

        // Fix up path for tilda (~) root application directory.
        // VirtualPathUtility blows up if there is a query string, so we have to account for this.
        int queryStringStartIndex = originalUrl.IndexOf('?');
        if(queryStringStartIndex != -1) {
            string queryString = originalUrl.Substring(queryStringStartIndex);
            string baseUrl = originalUrl.Substring(0, queryStringStartIndex);

            return string.Concat(VirtualPathUtility.ToAbsolute(baseUrl), queryString);
        } else {
            return VirtualPathUtility.ToAbsolute(originalUrl);
        }
    }

    /// <summary>
    /// This method returns a fully qualified absolute server URL which includes
    /// the protocol, server, port in addition to the server relative URL.
    /// 
    /// Works like Control.ResolveUrl including support for tilda syntax
    /// but returns an absolute URL.
    /// </summary>
    /// <remarks>http://west-wind.com/weblog/posts/154812.aspx</remarks>
    /// <param name="serverUrl">Any tilda, either application relative or fully qualified.</param>
    /// <param name="forceHttps">If true, then force the URL to use HTTPS.</param>
    public static string ResolveServerUrl(string serverUrl, bool forceHttps)
    {
        if(String.IsNullOrEmpty(serverUrl)) {
            return serverUrl;
        }

        // If absolute path, then return.
        if(IsAbsolutePath(serverUrl)) {
            return serverUrl;
        }

        string newServerUrl = ResolveUrl(serverUrl);
        Uri result = new Uri(HttpContext.Current.Request.Url, newServerUrl);

        if(!forceHttps) {
            return result.ToString();
        } else {
            return ForceUriToHttps(result).ToString();
        }
    }

    /// <summary>
    /// This method returns a fully qualified absolute server URL which includes
    /// the protocol, server, port in addition to the server relative URL.
    /// 
    /// It work like Page.ResolveUrl, but adds these to the beginning.
    /// This method is useful for generating Urls for AJAX methods
    /// </summary>
    /// <remarks>http://west-wind.com/weblog/posts/154812.aspx</remarks>
    /// <param name="serverUrl">Any tilda, either application relative or fully qualified</param>
    public static string ResolveServerUrl(string serverUrl)
    {
        return ResolveServerUrl(serverUrl, false);
    }

    /// <summary>Forces the URI to use HTTPS.</summary>
    /// <remarks>http://west-wind.com/weblog/posts/154812.aspx</remarks>
    private static Uri ForceUriToHttps(Uri uri)
    {
        // Rewrite uri using builder.
        UriBuilder builder = new UriBuilder(uri);
        builder.Scheme = Uri.UriSchemeHttps;
        builder.Port = 443;
        return builder.Uri;
    }

    /// <summary>Is absolute path.</summary>
    /// <remarks>http://west-wind.com/weblog/posts/154812.aspx</remarks>
    private static bool IsAbsolutePath(string originalUrl)
    {
        int IndexOfSlashes = originalUrl.IndexOf("://");
        int IndexOfQuestionMarks = originalUrl.IndexOf("?");

        if(IndexOfSlashes > -1 && (IndexOfQuestionMarks < 0 || (IndexOfQuestionMarks > -1 && IndexOfQuestionMarks > IndexOfSlashes))) {
            return true;
        }
        return false;
    }

#endregion

}

} // END namespace SandboxSitecore.Cms.Core