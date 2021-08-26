using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetToolBox.TwilioHelpers.Abstractions;

namespace NetToolBox.TwilioHelpers.TestWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
        }

        public IConfiguration Configuration { get; }

        private readonly IWebHostEnvironment _environment;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTwilioAspNetCoreServices(Configuration.GetSection("TwilioSettings"), _environment.EnvironmentName);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    var twilio = context.RequestServices.GetRequiredService<ITwilioServices>();
                    var stream = await twilio.GetRecordingWavAsync(new System.Uri(""));
                    await twilio.DeleteRecordingAsync("", "");
                    //  await twilio.SendSMSMessageAsync("test message", "", "");

                });
                //endpoints.MapPost("/", async context =>
                //{
                //    var validator = context.RequestServices.GetRequiredService<ITwilioSignatureValidator>();
                //    var isValid = validator.ValidateRequest(context.Request);
                //    await context.Response.WriteAsync($"Valid TwilioRequest={isValid}");
                //});
            });
        }
    }
}
