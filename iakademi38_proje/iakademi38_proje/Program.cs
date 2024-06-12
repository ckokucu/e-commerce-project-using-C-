using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//biz ekledik..//Süre 1 dk olarak belirlendi..sepete ekle 
builder.Services.AddSession(option =>
{
	option.IdleTimeout = TimeSpan.FromMinutes(1);
});

//BÝZ EKLEDÝK ALERT TÜRKCE KARAKTER SORUNU ÝÇÝN
builder.Services.AddWebEncoders(o =>
{
    o.TextEncoderSettings = new System.Text.Encodings.Web.TextEncoderSettings(UnicodeRanges.All);
});


//biz ekledik layout da session login görünümü için (logo altýnda email görünsün)
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();// biz ekledik........ sepete ekle, login giriþte session atayacagýz,order login kontrolu

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
