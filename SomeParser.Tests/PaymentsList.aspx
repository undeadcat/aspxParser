<%@ Page Title="Деньги" Language="C#" MasterPageFile="~/Business/BusinessPage.Master" AutoEventWireup="true" CodeBehind="PaymentsList.aspx.cs" Inherits="Front.Business.Payments.List.PaymentsList" %>
<%@ Import Namespace="Kontur.Elba.Web.Infrastructure.Common.Mvc.Views" %>
<%@ Import Namespace="Front.Business.Payments" %>
<%@ Import Namespace="Front.Business.Payments.List" %>
<%@ Import Namespace="Kontur.Elba.Web.Application.Business.Filters" %>
<%@ Import Namespace="Kontur.Elba.Web.Application.Business.Payments.List" %>
<%@ Import Namespace="Kontur.Elba.Web.Application.Videos" %>
<%@ Import Namespace="Front.Business.Excel" %>
<%@ Import Namespace="Kontur.Elba.Web.Models.Business.Payments" %>

<%@ Register TagPrefix="ib" Namespace="Front.Accounting" Assembly="Elba.Front" %>
<%@ Register TagPrefix="ib" Namespace="Front.Controls.System.MultipleActions.PaymentsMultipleActionsDialog" Assembly="Elba.Front" %>
<%@ Register TagPrefix="ib" Namespace="Front.Business.Payments.Balance" Assembly="Elba.Front" %>
<%@ Register TagPrefix="ib" TagName="PrintCashBookPopup" Src="~/Business/Payments/CashBook/PrintCashBookPopupControl.ascx" %>
<%@ Register TagPrefix="ib" TagName="PaymentActionsPanel" Src="~/Business/Documents/PaymentActionsPanel.ascx" %>
<%@ Register TagPrefix="ib" TagName="ImportPaymentMessage" Src="~/Business/Payments/ImportPaymentMessage.ascx" %>
<%@ Register TagPrefix="ib" TagName="PaymentsFilter" Src="~/Business/Payments/PaymentsFilter.ascx" %>
<%@ Register TagPrefix="ib" TagName="SearchInputControl" Src="~/Controls/Application/Searching/SearchInputControl.ascx" %>
<%@ Register TagPrefix="ib" TagName="AccountingWizardMessageControl" Src="~/Accounting/AccountingWizardMessageControl.ascx" %>
<%@ Register TagPrefix="ib" TagName="DownloadExcelPanel" Src="~/Business/Excel/DownloadExcelPanel.ascx" %>
<%@ Register TagPrefix="ib" TagName="VideoPlayer" Src="~/Controls/Application/Videos/VideoPlayer.ascx" %>
<%@ Register TagPrefix="ib" TagName="UsnTaxWithYellowSheet" Src="~/Reports/Usn/UsnTaxWithYellowSheet.ascx" %>
<%@ Register TagPrefix="ib" TagName="Nanobox" Src="~/Controls/System/Dialogs/Nanobox.ascx" %>
<%@ Register TagPrefix="ib" TagName="SecondaryMenu" Src="~/Controls/Application/Menus/SecondaryMenu.ascx" %>
<%@ Register TagPrefix="ib" TagName="SecondaryMenuItem" Src="~/Controls/Application/Menus/SecondaryMenuItem.ascx" %>
<%@ Register TagPrefix="ib" TagName="UsnWithPatentWarningMessage" Src="~/Requisites/UsnWithPatentWarningMessage.ascx" %>
<%@ Register TagPrefix="ib" TagName="FinalBalanceProblemMessageContent" Src="~/Business/Payments/Balance/FinalBalanceProblemMessageContent.ascx" %>

<asp:Content ContentPlaceHolderID="PageContent" runat="server">
</asp:Content>