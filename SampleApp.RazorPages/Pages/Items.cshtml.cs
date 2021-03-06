using Dapper.CX.SqlServer.AspNetCore.Extensions;
using Dapper.CX.SqlServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SampleApp.Models;
using System.Threading.Tasks;

namespace SampleApp.RazorPages.Pages
{
    [Authorize]
    public partial class ItemsModel : BasePageModel
    {
        public ItemsModel(DapperCX<int, UserProfile> data) : base(data)
        {
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public SelectList ItemSelect { get; set; }

        public Item Item { get; set; }

        public async Task OnGetAsync()
        {
            Item = await Data.GetAsync<Item>(Id);

            ItemSelect = await Data.QuerySelectListAsync(new Queries.SelectLists.ItemSelect()
            {
                WorkspaceId = Data.User.WorkspaceId ?? 0
            }, Id);
        }

        public async Task<RedirectResult> OnPostSaveItemAsync(Item item) =>
            await Data.SaveAndRedirectAsync(item,
                (model, exc) => (model.Id != 0) ? Redirect($"/Items/{model.Id}") : Redirect("/Items"),
                beforeSave: (model) => model.WorkspaceId = Data.User.WorkspaceId ?? 0,
                onSuccess: (model) => SaveSuccessMessage($"Item {model.Name} updated successfully."),
                onException: (model, exc) => SaveErrorMessage(exc));

        public async Task<RedirectResult> OnPostDeleteAsync(int id) =>
            await Data.DeleteAndRedirectAsync<Item, int, UserProfile>(id, "/Items");
    }
}
