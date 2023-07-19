using  CookiesOpml;
using static CookiesOpml.Pages.OpmlModel;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<CookiesOpml.RssListService>();
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = ".AspNetCore.Antiforgery.XSRF-TOKEN";
    options.HeaderName = "RequestVerificationToken";
});
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.MapPost("/Starred/ToggleStarred", async (RssListService rssListService, HttpContext context) =>
{
    var data = await context.Request.ReadFromJsonAsync<Dictionary<string, object>>();
    if (data.TryGetValue("guid", out var guidValue) && data.TryGetValue("isStarred", out var isStarredValue))
    {
        var guid = guidValue.ToString();
        var isStarred = bool.Parse(isStarredValue.ToString());
        var item = rssListService.FindItemByGuid(guid);
        if (item != null)
        {
            item.IsStarred = isStarred;
            var existingRssModel = rssListService.RssListGlobal.FirstOrDefault(r => r.Items.Any(i => i.Guid == item.Guid));
            if (existingRssModel != null)
            {
                existingRssModel.Items.First(i => i.Guid == item.Guid).IsStarred = item.IsStarred;
                int index = rssListService.RssListGlobal.FindIndex(r => r.Items.Any(i => i.Guid == item.Guid));
                rssListService.RssListGlobal[index] = existingRssModel;
            }

            var result = new Dictionary<string, object>
                {
                    { "isStarred", isStarred }
                };
            await context.Response.WriteAsJsonAsync(result);
        }
        else
        {
            var result = new Dictionary<string, object>
            {
                { "error", "Item not found" }
            };
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(result);
        }
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});
app.Run();

