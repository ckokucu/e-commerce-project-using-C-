using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//biz ekledik..//S�re 1 dk olarak belirlendi..sepete ekle 
builder.Services.AddSession(option =>
{
	option.IdleTimeout = TimeSpan.FromMinutes(1);
});

//B�Z EKLED�K ALERT T�RKCE KARAKTER SORUNU ���N
builder.Services.AddWebEncoders(o =>
{
    o.TextEncoderSettings = new System.Text.Encodings.Web.TextEncoderSettings(UnicodeRanges.All);
});


//biz ekledik layout da session login g�r�n�m� i�in (logo alt�nda email g�r�ns�n)
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

app.UseSession();// biz ekledik........ sepete ekle, login giri�te session atayacag�z,order login kontrolu

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();