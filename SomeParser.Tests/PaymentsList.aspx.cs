using Front.Controls.Application.Lightboxes.Payment;
using Front.Infrastructure;
using Front.Reports.Usn;
using Kontur.Elba.Application.PersistedViews.Payments;
using Kontur.Elba.Core.Utilities.Collections;
using Kontur.Elba.Core.Web.JavaScriptSerialization;
using Kontur.Elba.Web.Application.Business.Payments.List;
using Kontur.Elba.Web.Infrastructure.Auth;
using Kontur.Elba.Web.Infrastructure.Common.ContentManagers;
using Kontur.Elba.Web.Models.Business.Payments;

namespace Front.Business.Payments.List
{
	[BanManager]
	public partial class PaymentsList: BusinessListViewPageBase<PaymentsListViewData, PaymentPersistedView>
	{
		protected bool IgnoreSavedFilter => Context.Request.QueryString.GetBoolOrFalse("ignoreSavedFilter");
		public string SomeProp { get; set; }

		protected PaymentsList()
		{
			DefaultSort = "Date,desc;Created,desc";
			ClientEditable = true;
		}

		protected override JavaScriptObject GetClientOptions()
		{
			return base.GetClientOptions()
					   .Add("paymentLightbox", this.GetComponentLazy<PaymentLightboxControl>())
					   .Add("trySuggestImport", Model.TrySuggestImport)
					   .Add("importToken", Model.Import?.Token)
					   .WithProxy(x => x.WithMethod<CancelPaymentImport>()
										.WithMethod<GetPayments>()
										.WithMethod<GetPersistedViewsByIds>(m => m.InlineArgument("viewType", typeof (PaymentPersistedView).Name))
										.WithMethod<GetUsnYellowSheetViewData>()
										.WithAction<PaymentsListController>(c => c.IsVisibleImportSuggestion())
										.WithAction<PaymentsListController>(c => c.HideImportSuggestion())
										.WithAction<PaymentsListController>(y => y.RemoveContributionsPatchMessage()));
		}

		protected string[] GetForgottenDocumentsHeadings()
		{
			return new[] { $"Добавьте поступления и списания {Model.AccountingPreparationPeriodText}" };
		}
	}
}