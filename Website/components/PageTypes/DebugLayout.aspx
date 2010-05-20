<%@ page language="C#" autoeventwireup="true" codebehind="DebugLayout.aspx.cs" inherits="SandboxSitecore.Cms.Components.PageTypes.DebugLayout" %>
<%@ register tagprefix="sc" namespace="Sitecore.Web.UI.WebControls" assembly="Sitecore.Kernel" %>
<%@ outputcache location="None" varybyparam="none" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta http-equiv="content-type" content="text/html; charset=utf-8" />
	<title>DebugLayout - MindComet</title>
	<!-- BEGIN: Styles -->
	<link href="~/_assets/css/template_barebone.css" media="screen" rel="stylesheet" type="text/css" runat="server"/>
	<!-- END: Styles -->
	<!-- BEGIN: Dependencies -->
	<script src="<%=ResolveClientUrl("~/_assets/js/jquery.min.js")%>" type="text/javascript"></script>
	<!-- END: Dependencies -->
	<!-- BEGIN: Page Specific -->
	<script type="text/javascript">
//<![CDATA[
jQuery(document).ready(function($) {
	// Do something.
});
//]]>
	</script>
	<!-- END: Page Specific -->
</head>
<body>
<form id="form1" runat="server">
<div id="frame<%=HtmlFramePostfix%>">
	<div id="region_container"><div id="region_container_buffer">
		<div id="region_middle" class="region_middle_style">
<!-- BEGIN: Template Region Middle -->
			<sc:placeholder id="RegionMiddle" key="RegionMiddle" runat="server"/>
			
			<asp:literal id="litDebug" runat="server"/>
			
			<asp:repeater id="rptrItems" enableviewstate="false" onitemdatabound="rptrItems_ItemDataBound" runat="server">
			<headertemplate>
			</headertemplate>
			<itemtemplate>
				<asp:literal id="litItem" runat="server"/>
				<asp:literal id="litItemDebug" runat="server"/>
			</itemtemplate>
			<footertemplate>
			</footertemplate>
			</asp:repeater>
			
<!-- END: Template Region Middle -->
		</div>
		<div id="region_footer">
<!-- BEGIN: Region Footer -->
			<sc:placeholder id="RegionFooter" key="RegionFooter" runat="server"/>
<!-- END: Region Footer -->
		</div>
	</div></div>
</div>
</form>
</body>
</html>