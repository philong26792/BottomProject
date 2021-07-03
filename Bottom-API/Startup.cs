using System.Text;
using AutoMapper;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Repositories.Interfaces.DbHpBasic;
using Bottom_API._Repositories.Interfaces.DbMES;
using Bottom_API._Repositories.Interfaces.DbUser;
using Bottom_API._Repositories.Repositories;
using Bottom_API._Repositories.Repositories.DbHpBasic;
using Bottom_API._Repositories.Repositories.DbMES;
using Bottom_API._Repositories.Repositories.DbUser;
using Bottom_API._Services.Interfaces;
using Bottom_API._Services.Services;
using Bottom_API.Data;
using Bottom_API.Helpers;
using Bottom_API.Helpers.AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Bottom_API
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
            services.AddCors();
            services.AddDbContext<DataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<HPDataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("HPConnection")));
            services.AddDbContext<UserContext>(options => options.UseSqlServer(Configuration.GetConnectionString("UserConnection")));
            services.AddDbContext<MesDataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MesConnection")));
            services.AddTransient<IDatabaseConnectionFactory>(e =>
            {
                return new SqlConnectionFactory(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddControllers();
            //Auto Mapper
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<IMapper>(sp =>
            {
                return new Mapper(AutoMapperConfig.RegisterMappings());
            });
            services.AddSingleton(AutoMapperConfig.RegisterMappings());
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                        .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            // Repository
            services.AddScoped<IPackingListRepository, PackingListRepository>();
            services.AddScoped<ICodeIDDetailRepo, CodeIDDetailRepo>();
            services.AddScoped<IRackLocationRepo, RackLocationRepo>();
            services.AddScoped<IQRCodeMainRepository, QRCodeMainRepository>();
            services.AddScoped<IPackingListDetailRepository, PackingListDetailRepository>();
            services.AddScoped<IQRCodeDetailRepository, QRCodeDetailRepository>();
            services.AddScoped<IHPVendorRepository, HPVendorRepository>();
            services.AddScoped<IHPUploadTimeLogRepository, HPUploadTimeLogRepository>();
            services.AddScoped<IMaterialPurchaseRepository, MaterialPurchaseRepository>();
            services.AddScoped<IMaterialMissingRepository, MaterialMissingRepository>();
            services.AddScoped<IMaterialViewRepository, MaterialViewRepository>();
            services.AddScoped<ITransactionMainRepo, TransactionMainRepo>();
            services.AddScoped<ITransactionDetailRepo, TransactionDetailRepo>();
            services.AddScoped<IMaterialSheetSizeRepository, MaterialSheetSizeRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IRolesRepository, RolesRepository>();
            services.AddScoped<IRoleUserRepository, RoleUserRepository>();
            services.AddScoped<IMesMoRepository, MesMoRepository>();
            services.AddScoped<IViewMesMoRepository, ViewMesMoRepository>();
            services.AddScoped<IViewPoRepository, ViewPoRepository>();
            services.AddScoped<ISettingSupplierRepository, SettingSupplierRepository>();
            services.AddScoped<ITransferFormRepository, TransferFormRepository>();
            services.AddScoped<ICacheRepository, CacheRepository>();
            services.AddScoped<IHPHolidaysi46Repository, HPHolidaysi46Repository>();
            services.AddScoped<IHPHolidaysi46Repository, HPHolidaysi46Repository>();
            services.AddScoped<ISettingReasonRepository, SettingReasonRepository>();
            services.AddScoped<IMaterialOffsetRepository, MaterialOffsetRepository>();
            services.AddScoped<IMesMoSizeRepository, MesMoSizeRepository>();
            services.AddScoped<IMaterialsRepository, MaterialsRepository>();
            services.AddScoped<IPoRootsRepository, PoRootsRepository>();
            services.AddScoped<IPoMaterialsRepository, PoMaterialsRepository>();
            services.AddScoped<IMesMoBasicRepository, MesMoBasicRepository>();
            services.AddScoped<IReasonDetailRepository, ReasonDetailRepository>();
            services.AddScoped<IMaterialPurchaseSplitRepository, MaterialPurchaseSplitRepository>();
            services.AddScoped<ISettingT2SupplierRepository, SettingT2SupplierRepository>();
            services.AddScoped<IReleaseDeliveryNoRepository, ReleaseDeliveryNoRepository>();

            // Service
            services.AddScoped<IPackingListService, PackingListService>();
            services.AddScoped<ICodeIDDetailService, CodeIDDetailService>();
            services.AddScoped<IRackLocationService, RackLocationService>();
            services.AddScoped<IQRCodeMainService, QRCodeMainService>();
            services.AddScoped<IMaterialFormService, MaterialFormService>();
            services.AddScoped<IReceivingService, ReceivingService>();
            services.AddScoped<IInputService, InputService>();
            services.AddScoped<ITransferLocationService, TransferLocationService>();
            services.AddScoped<IOutputService, OutputService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IKanbanService, KanbanService>();
            services.AddScoped<IKanbanByRackService, KanbanByRackService>();
            services.AddScoped<IKanBanByPoService, KanbanByPoService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IHistoryReportService, HistoryReportService>();
            services.AddScoped<IRecevingEnoughService, RecevingEnoughService>();
            services.AddScoped<ISettingMailService, SettingMailService>();
            services.AddScoped<ITransferFormService, TransferFormService>();
            services.AddScoped<ICompareReportService, CompareReportService>();
            services.AddScoped<IHPUploadService, HPUploadService>();
            services.AddScoped<IMailUtility, MailUtility>();
            services.AddScoped<IModifyStoreService, ModifyStoreService>();
            services.AddScoped<ISettingReasonService, SettingReasonService>();
            services.AddScoped<IMergeQrCodeService, MergeQrCodeService>();
            services.AddScoped<ISettingT2SupplierService, SettingT2SupplierService>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bottom API", Version = "v1" });
            });

            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}