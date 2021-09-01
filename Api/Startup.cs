using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Api.Core.Database;
using Api.Repositories;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySqlConnector;

namespace Api {
  public class Startup {
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {

      DbProviderFactories.RegisterFactory("Main", MySqlConnectorFactory.Instance);
      services.AddSingleton<IUserRepository, UserRepository>();
      services.AddLinqToDbContext<MainDataConnection>((provider, options) => {
        options
          .UseMySqlConnector(Configuration.GetConnectionString("Main"))
          .UseDefaultLogging(provider);
      });

      services.AddControllers();
      services.AddSwaggerGen(c => {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
        c.AddSecurityDefinition("JwtBearer", new OpenApiSecurityScheme {
          Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
          Name = "Authorization",
          In = ParameterLocation.Header,
          Type = SecuritySchemeType.ApiKey,
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
          {
            new OpenApiSecurityScheme
            {
              Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "JwtBearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,
            },
            new List<string>()
          }
        });
      });
      services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = "JwtBearer";
        options.DefaultChallengeScheme = "JwtBearer";
      })
      .AddJwtBearer("JwtBearer", options => {
        var key = Configuration.GetValue<string>("JwtSigningKey");
        options.TokenValidationParameters = new TokenValidationParameters {
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
          ValidateIssuerSigningKey = true,
          ValidateIssuer = false,
          ValidateAudience = false,
          ValidateLifetime = true,
          ClockSkew = TimeSpan.FromMinutes(5),
        };

      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
      }

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints => {
        endpoints.MapControllers();
      });
    }
  }
}
