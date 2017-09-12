<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="TestNamespace.TestPage" %>

<%@ Register TagPrefix="foo" TagName="ControlAlias" Src="~/Controls/UserControl.ascx" %>

<foo:ControlAlias ID="SomeControlID" runat="server"></foo:ControlAlias>