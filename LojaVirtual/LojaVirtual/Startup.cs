using AutoMapper;
using LojaVirtual.Database;
using LojaVirtual.Libraries.AutoMapper;
using LojaVirtual.Libraries.CarrinhoCompra;
using LojaVirtual.Libraries.Email;
using LojaVirtual.Libraries.Gerenciador.Frete;
using LojaVirtual.Libraries.Login;
using LojaVirtual.Libraries.Middleware;
using LojaVirtual.Libraries.Sessao;
using LojaVirtual.Repositories;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Mail;
using WSCorreios;

namespace LojaVirtual
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			//AutoMapper
			services.AddAutoMapper(config => config.AddProfile<MappingProfile>());

			//Repository
			services.AddHttpContextAccessor();
			services.AddScoped<IClienteRepository, ClienteRepository>();
			services.AddScoped<INewsletterRepository, NewsletterRepository>();
			services.AddScoped<IColaboradorRepository, ColaboradorRepository>();
			services.AddScoped<ICategoriaRepository, CategoriaRepository>();
			services.AddScoped<IProdutoRepository, ProdutoRepository>();
			services.AddScoped<IImagemRepository, ImagemRepository>();

			//SMTP
			services.AddScoped<SmtpClient>(opt =>
			{
				SmtpClient smtp = new SmtpClient()
				{
					Host = Configuration.GetValue<string>("Email:ServerSMTP"),
					Port = Configuration.GetValue<int>("Email:ServerPort"),
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(Configuration.GetValue<string>("Email:Username"), Configuration.GetValue<string>("Email:Password")),
					EnableSsl = true
				};

				return smtp;
			});
			services.AddScoped<CalcPrecoPrazoWSSoap>(options => {
				var servico = new CalcPrecoPrazoWSSoapClient(CalcPrecoPrazoWSSoapClient.EndpointConfiguration.CalcPrecoPrazoWSSoap);
				return servico;	
			});
			services.AddScoped<GerenciarEmail>();
			services.AddScoped<Cookie>();
			services.AddScoped<CookieCarrinhoCompra>();
			services.AddScoped<CookieValorPrazoFrete>();
			services.AddScoped<CalcularPacote>();
			services.AddScoped<WSCorreiosCalcularFrete>();


			services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			//Session - Configuracao
			services.AddMemoryCache(); //Guarda os dados na memoria
			services.AddSession(options =>
			{
				options.Cookie.IsEssential = true;
			});

			services.AddScoped<Sessao>();
			services.AddScoped<Cookie>();
			services.AddScoped<LoginCliente>();
			services.AddScoped<LoginColaborador>();

			services.AddMvc(opt =>
			{
				opt.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => "O campo deve ser preenchido!");
			}).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services.AddDbContext<LojaVirtualContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseDefaultFiles();
			app.UseStaticFiles();
			app.UseCookiePolicy();
			app.UseSession();
			app.UseMiddleware<ValidateAntiForgeryTokenMiddleware>();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "areas",
					template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
				);
				routes.MapRoute(
					name: "default",
					template: "/{controller=Home}/{action=Index}/{id?}");
			});

		}
	}
}
