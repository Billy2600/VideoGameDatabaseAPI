using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using VideoGameAPI.Contexts;
using VideoGameAPI.Repositories;

namespace VideoGameAPI
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
            services.AddDbContext<ConsoleContext>(opt => opt.UseSqlServer("Server = localhost\\SQLEXPRESS; Database = VideoGames; Trusted_Connection = True;"));
            services.AddDbContext<GenreContext>(opt => opt.UseSqlServer("Server = localhost\\SQLEXPRESS; Database = VideoGames; Trusted_Connection = True;"));
            services.AddDbContext<PublisherContext>(opt => opt.UseSqlServer("Server = localhost\\SQLEXPRESS; Database = VideoGames; Trusted_Connection = True;"));
            services.AddDbContext<GameContext>(opt => opt.UseSqlServer("Server = localhost\\SQLEXPRESS; Database = VideoGames; Trusted_Connection = True;"));

            services.AddScoped<IGameRepository, GameRepository>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
