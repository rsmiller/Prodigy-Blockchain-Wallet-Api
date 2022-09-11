using ProdigyBlockchain.Wallet.BusinessLayer;
using ProdigyBlockchain.Wallet.BusinessLayer.Models;
using ProdigyBlockchain.Wallet.BusinessLayer.Services;
using ProdigyBlockchain.Wallet.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ProdigyBlockchain.Wallet.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();


            var dbSettings = new DatabaseConnectionSettings();
            var authenticationSettings = new AuthenticationSettings();
            var nodeSettings = new NodeSettings();
            var walletSettings = new WalletSettings();

            Configuration.GetSection("DBConnection").Bind(dbSettings);
            Configuration.GetSection("AuthenticationSettings").Bind(authenticationSettings);
            Configuration.GetSection("NodeSettings").Bind(nodeSettings);
            Configuration.GetSection("WalletSettings").Bind(walletSettings);

            var service = new CryptoService(walletSettings);

            services.AddSingleton<IDatabaseConnectionSettings>(dbSettings);
            services.AddSingleton<IAuthenticationSettings>(authenticationSettings);
            services.AddSingleton<INodeSettings>(nodeSettings);
            services.AddSingleton<ICryptoService>(service);
            services.AddSingleton<IWalletSettings>(walletSettings);

            services.AddScoped<IWalletContext, WalletContext>();
            services.AddScoped<IUserDataService, UserDataService>();
            services.AddScoped<INodeDataService, NodeDataService>();



            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProdigyBlockchain.Wallet.Api", Version = "v1" });
            });


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(m =>
            {
                m.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = false,
                        ValidIssuer = TokenDataService.Issuer,
                        IssuerSigningKey = TokenDataService.CreateSecurityKey(authenticationSettings.APIPrivateKey)
                    };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProdigyBlockchain.Wallet.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
