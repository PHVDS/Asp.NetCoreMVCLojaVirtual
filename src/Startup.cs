using AutoMapper;
using Coravel;
using LojaVirtual.Database;
using LojaVirtual.Libraries.AutoMapper;
using LojaVirtual.Libraries.CarrinhoCompra;
using LojaVirtual.Libraries.Email;
using LojaVirtual.Libraries.Gerenciador.Frete;
using LojaVirtual.Libraries.Gerenciador.Pagamento;
using LojaVirtual.Libraries.Gerenciador.Scheduler.Invocable;
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
			services.AddScoped<IEnderecoEntregaRepository, EnderecoEntregaRepository>();
			services.AddScoped<IPedidoRepository, PedidoRepository>();
			services.AddScoped<IPedidoSituacaoRepository, PedidoSituacaoRepository>();

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
			services.AddScoped<Libraries.Cookie.Cookie>();
			services.AddScoped<CookieCarrinhoCompra>();
			services.AddScoped<CookieFrete>();
			services.AddScoped<CalcularPacote>();
			services.AddScoped<WSCorreiosCalcularFrete>();


			services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			//Session - Configuracao
			services.AddMemoryCache(); //Guarda os dados na memoria
			
			services.AddScoped<Sessao>();
			services.AddScoped<Libraries.Cookie.Cookie>();
			services.AddScoped<LoginCliente>();
			services.AddScoped<LoginColaborador>();
			services.AddScoped<GerenciarPagarMe>();
			
			services.AddMvc(opt =>
			{
				opt.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => "O campo deve ser preenchido!");
			}).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
			.AddSessionStateTempDataProvider();

			services.AddSession(options =>
			{
				options.Cookie.IsEssential = true;
			});

			services.AddDbContext<LojaVirtualContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

			services.AddTransient<PedidoPagamentoSituacao>();
			services.AddTransient<PedidoEntregueJob>();
			services.AddTransient<PedidoFinalizadoJob>();
			services.AddTransient<PedidoDevolverEntregueJob>();
			services.AddScheduler();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			//if (env.IsDevelopment())
			if(false)
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseStatusCodePagesWithReExecute("/Error/{0}");
				app.UseExceptionHandler("/Error/Error500");
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

			// Scheduler - Coravel
			app.ApplicationServices.UseScheduler(scheduler => {
				scheduler.Schedule<PedidoPagamentoSituacao>().EveryTenSeconds();
				scheduler.Schedule<PedidoEntregueJob>().EveryTenSeconds();
				scheduler.Schedule<PedidoFinalizadoJob>().EveryTenSeconds();
				scheduler.Schedule<PedidoDevolverEntregueJob>().EveryTenSeconds();
			});
		}
	}
}
