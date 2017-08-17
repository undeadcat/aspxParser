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

<%--<span>--%>
<%--$1$	text#1#--%>
<%--</span>--%>
<div>
<%--	<%= some commented expression %>--%>
	<%: 1 + 1 %>
</div>
<asp:Content ContentPlaceHolderID="PageContent" runat="server">
	<ib:AccountingWizardMessageControl ID="AccountingWizardMessage" runat="server">
		<ib:ProblemMessage Step="Payments"
						   CompleteMessage="Теперь с типами операций у поступлений и списаний все хорошо."
						   ShowNumberOfProblems="True"
						   Headings='<%#new[]
										{
											"<span class=\"t-emphasized\">поступление или списание</span>, в котором есть проблемы",
											"<span class=\"t-emphasized\">поступления или списания</span>, в которых есть проблемы",
											"<span class=\"t-emphasized\">поступлений или списаний</span>, в которых есть проблемы"
										} %>'
						   runat="server">
			<ib:StaticText Text="Возможные проблемы:" runat="server"/>
			<ib:HtmlList runat="server">
				<ib:HtmlListItem runat="server">не указан тип операции</ib:HtmlListItem>
				<ib:HtmlListItem runat="server">не указаны сотрудники</ib:HtmlListItem>
				<ib:HtmlListItem runat="server">не указан тип налога или взноса</ib:HtmlListItem>
				<ib:HtmlListItem runat="server">не указан счёт бухучета</ib:HtmlListItem>
				<ib:HtmlListItem runat="server">нет расчётного счёта у деньги по банку</ib:HtmlListItem>
			</ib:HtmlList>
		</ib:ProblemMessage>
		<ib:ProblemMessage Step="ForgottenDocuments"
						   ShowNumberOfProblems="False"
							CompleteButtonText="Я добавил все документы и деньги"
						   Headings='<%#GetForgottenDocumentsHeadings() %>'
						   runat="server">
			<ib:StaticText Text="Вы можете использовать импорт выписки для загрузки данных о движении средств по банку." runat="server"/>
		</ib:ProblemMessage>


		<ib:FinalBalanceProblemMessage Step="FinalBankAndCashBalances" Year="<%# Model.CurrentAccountingYear %>" runat="server">
			<ib:FinalBalanceProblemMessageContent
			                                      Step="FinalBankAndCashBalances"
			                                      Year="<%# Model.CurrentAccountingYear %>"
			                                      runat="server"/>
		</ib:FinalBalanceProblemMessage>
		<ib:FinalBalanceProblemMessage Step="FinalBankAndCashBalancesPreviousYear" Year="<%# Model.CurrentAccountingYear %>" runat="server">
			<ib:FinalBalanceProblemMessageContent
			                                      Step="FinalBankAndCashBalancesPreviousYear"
			                                      Year="<%# Model.CurrentAccountingYear %>"
			                                      runat="server"/>
		</ib:FinalBalanceProblemMessage>
		<ib:FinalBalanceProblemMessage Step="FinalBankAndCashBalancesTwoYearsAgo" Year="<%# Model.CurrentAccountingYear %>" runat="server">
			<ib:FinalBalanceProblemMessageContent 
			                                      Step="FinalBankAndCashBalancesTwoYearsAgo"
			                                      Year="<%# Model.CurrentAccountingYear %>"
			                                      runat="server"/>
		</ib:FinalBalanceProblemMessage>
	</ib:AccountingWizardMessageControl>

	<div class="g-24">
		<div class="g-col-1 g-span-16">
			<div class="t-primaryHeading inlineBlock">Деньги</div>
			<ib:SecondaryMenu InTitle="True" ID="SecondaryMenu" runat="server">
				<ib:SecondaryMenuItem ID="AllMenuItem" runat="server" To="<%#typeof (PaymentsListController) %>">Все</ib:SecondaryMenuItem>
				<ib:SecondaryMenuItem ID="IncomingMenuItem" runat="server" To="<%#typeof (PaymentsListController) %>" Query='<%#new NameValueCollection { { "direction", PaymentDirection.Income.ToString() } } %>'>Поступления</ib:SecondaryMenuItem>
				<ib:SecondaryMenuItem ID="OutgoingMenuItem" runat="server" To="<%#typeof (PaymentsListController) %>" Query='<%#new NameValueCollection { { "direction", PaymentDirection.Outcome.ToString() } } %>'>Списания</ib:SecondaryMenuItem>
			</ib:SecondaryMenu>
		</div>
		<div class="g-col-17 g-span-8">
			<ib:SearchInputControl class="search-pagingSearch" ID="SearchInput" ClientVisible="<%#Model.BusinessList.HasSourceItems %>" runat="server" />
		</div>
	</div>

	<ib:ImportPaymentMessage ID="ImportMessage" Data="<%#Model.Import %>" runat="server"/>
	<% Html.RenderPartial<ImportPaymentFailuresMessage>(Model.Import != null ? Model.Import.FailuresMessageViewData : new ImportPaymentFailuresMessageViewData(), new { Id = "ImportFailuresMessage" }); %>

	<ib:Panel ID="PaymentsActionsPanel" class="business-section-actionsPanel safeContext" runat="server">
		<div class="business-section-actionsPanel-left">
			<div class="c-actionPanel">
				<ib:PaymentActionsPanel runat="server"/>
				<ib:Pseudolink ID="PrintCashBookLink" class="c-actionPanel-control c-actionPanel-control_distant" ClientVisible="False" runat="server">Кассовая книга</ib:Pseudolink>
				<ib:DownloadExcelPanel ID="DownloadExcelPanel" BalloonCssClassSuffix="payments" class="inlineBlock c-actionPanel-control_distant c-actionPanel-control" 
									   HandlerType="<%#typeof (DownloadExcelWithPayments) %>" runat="server" Visible="<%#Model.BusinessList.HasSourceItems %>" />
				<ib:PrintCashBookPopup runat="server" />
			</div>
		</div>
		<div class="business-section-actionsPanel-right" Visible="<%#Model.ShowYellowSheetLink %>" runat="server">
			<ib:UsnTaxWithYellowSheet runat="server" />
		</div>
	</ib:Panel>
	<ib:Nanobox ID="ImportSuggestionPopup" Width="530px" class="paymentsList-importSuggestionNanobox" runat="server">
		<div class="paymentsList-importSuggestionNanobox-pointer-container">
			<div class="paymentsList-importSuggestionNanobox-pointer"></div>
		</div>

		<ib:StaticText class="c-popup-controls-closeLink dialogClose" ID="CloseLink" runat="server">Закрыть</ib:StaticText>
		Не обязательно создавать все поступления и списания руками.<br>
		Скачайте в своём интернет-банке выписку в формате 1С<br>
		и загрузите её этой кнопкой. Все деньги из банка окажутся в Эльбе.
	</ib:Nanobox>

	<% Html.RenderAction<PaymentSynchronizationController>(x => x.Index()); %>
	<%:SomeProp %>
	<%= SomethingElse %>
	<%# DataBind %>
	
	<ib:UsnWithPatentWarningMessage ID="UsnWithPatentWarningMessage" runat="server"/>

	<ib:PaymentsFilter ID="Filter" IgnoreSavedFilter="<%#IgnoreSavedFilter %>" ClientVisible="<%#Model.BusinessList.HasSourceItems %>" runat="server"/>

	<ib:Panel runat="server" ID="ExpandPanel" class="c-filterPanel-expandPanel" ClientVisible="False">
		<ib:StaticText class="t-quaternaryHeading c-filterPanel-expandPanel-text" runat="server" ID="ExpandLink" Text="Развернуть"/>
	</ib:Panel>

	<ib:Panel runat="server" ClientVisible="False" ID="EmptyFilterResult" class="t-topLine t-line">
		Ничего не найдено
	</ib:Panel>

	<ib:Panel ID="NotEmptyListPanel" ClientVisible="False" runat="server">
		<% Html.RenderPartial<PaymentsListControl>(Model.ListModel, new { ID = "ItemsList" }); %>
	    <ib:PaymentsMultipleActionsDialogPlugin runat="server"/>
	</ib:Panel>
	
	<ib:Panel ID="FooterPanel" ClientVisible="False" runat="server">
		<% Html.RenderPartial<PaymentsListFooter>(Model.ListModel); %>
	</ib:Panel>
	
	<ib:Panel ID="NoSourceItemsPanel" ClientVisible="<%#!Model.BusinessList.HasSourceItems %>" runat="server">
		<div class="sectionSummaryVideo">
			<div class="sectionSummaryVideo-heading">Доходы и расходы&nbsp;&mdash;&nbsp;с чего начать?</div>
			<div class="sectionSummaryVideo-line">
				Здесь вы можете вести учет доходов и расходов, эти данные будут использованы
				для расчета налогов и составления отчетных документов. Данные можно вводить вручную
				или загружать выписку из интернет-банка.
			</div>
			<ib:VideoPlayer ID="VideoPlayer" Video="<%#VideoReference.Instance.BusinessPayments %>" runat="server"/>
		</div>
	</ib:Panel>

</asp:Content>