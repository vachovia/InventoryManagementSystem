using IMS.Plugins.InMemory;
using IMS.UseCases.PluginInterfaces;
using IMS.WebApp.Components;

using IMS.UseCases.Inventories;
using IMS.UseCases.Inventories.Interfaces;
using IMS.UseCases.Products;
using IMS.UseCases.Products.Interfaces;
using IMS.UseCases.Activities;
using IMS.UseCases.Activities.Interfaces;
using IMS.UseCases.Reports.Interfaces;
using IMS.UseCases.Reports;
using IMS.Plugins.EFCoreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using IMS.WebApp.Components.Account;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using IMS.WebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddRazorComponents(); **********
// This was done for Delete Product from Product List
// with @rendermode="InteractiveServer" where Component used
// No Http Request simply used SignalR channel
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Services for Identity /////////////////////////////////////////////////////////////////////////
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

var imsConnectionString = builder.Configuration.GetConnectionString("IMSAccounts");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(imsConnectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(
    options => options.SignIn.RequireConfirmedAccount = true
)
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

///////////////////////////////////////////////////////////////////////////////////////////////////
///
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextFactory<IMSContext>(options => options.UseSqlServer(connectionString));

// This we keep for testing
if(builder.Environment.IsEnvironment("Testing"))
{
    // because "Testing" is not built-in Environment so why wwwroot files are not loaded
    // StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration); // not needed
    builder.Services.AddSingleton<IInventoryRepository, InventoryRepository>();
    builder.Services.AddSingleton<IProductRepository, ProductRepository>();
    builder.Services.AddSingleton<IInventoryTransactionRepository, InventoryTransactionRepository>();
    builder.Services.AddSingleton<IProductTransactionRepository, ProductTransactionRepository>();
}
else
{
    builder.Services.AddTransient<IInventoryRepository, InventoryRepositoryEF>();
    builder.Services.AddTransient<IProductRepository, ProductRepositoryEF>();
    builder.Services.AddTransient<IInventoryTransactionRepository, InventoryTransactionRepositoryEF>();
    builder.Services.AddTransient<IProductTransactionRepository, ProductTransactionRepositoryEF>();
}

builder.Services.AddTransient<IViewInventoriesByNameUseCase, ViewInventoriesByNameUseCase>();
builder.Services.AddTransient<IAddInventoryUseCase, AddInventoryUseCase>();
builder.Services.AddTransient<IEditInventoryUseCase, EditInventoryUseCase>();
builder.Services.AddTransient<IViewInventoryByIdUseCase, ViewInventoryByIdUseCase>();
builder.Services.AddTransient<IDeleteInventoryUseCase, DeleteInventoryUseCase>();

builder.Services.AddTransient<IViewProductsByNameUseCase, ViewProductsByNameUseCase>();
builder.Services.AddTransient<IDeleteProductUseCase, DeleteProductUseCase>();
builder.Services.AddTransient<IAddProductUseCase, AddProductUseCase>();
builder.Services.AddTransient<IViewProductByIdUseCase, ViewProductByIdUseCase>();
builder.Services.AddTransient<IEditProductUseCase, EditProductUseCase>();

builder.Services.AddTransient<IPurchaseInventoryUseCase, PurchaseInventoryUseCase>();
builder.Services.AddTransient<IProduceProductUseCase, ProduceProductUseCase>();
builder.Services.AddTransient<ISellProductUseCase, SellProductUseCase>();

builder.Services.AddTransient<ISearchInventoryTransactionsUseCase, SearchInventoryTransactionsUseCase>();
builder.Services.AddTransient<ISearchProductTransactionsUseCase, SearchProductTransactionsUseCase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint(); // from added diagnostic package
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// app.MapRazorComponents<App>(); ************
// This was done for Delete Product from Product List
// with @rendermode="InteractiveServer" where Component used
// No Http Request simply used SignalR channel
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// also added which is required by the Identity /Account Razor Components
app.MapAdditionalIdentityEndpoints();

app.Run();


// To run migration for EFCore we need to make WebApp as a start-up project and as a default project specify IMSPlugins.EFCoreSql
// InMemory Plugin was still attached while runing migrations
// Then in EFCore plugin we have to re-create Repository files

// To run migration after adding second context we need to specify DbContext
// Update-Database -Context ApplicationDbContext