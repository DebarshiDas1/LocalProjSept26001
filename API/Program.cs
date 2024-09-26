using LocalProjSept26001.Data;
using Microsoft.OpenApi.Models;
using System.Reflection;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LocalProjSept26001.Models;
using LocalProjSept26001.Services;
using LocalProjSept26001.Logger;
using Newtonsoft.Json;
var builder = WebApplication.CreateBuilder(args);

// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LocalProjSept26001", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.IgnoreObsoleteActions();
    c.IgnoreObsoleteProperties();
    c.CustomSchemaIds(type => type.ToString());
    c.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter into field the word 'Bearer' following by space and JWT",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                    Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string> ()
                    }
                });
});
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
// Build the configuration object from appsettings.json
var config = new ConfigurationBuilder()
  .AddJsonFile("appsettings.json", optional: false)
  .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
  .Build();
//Set value to appsetting
AppSetting.JwtIssuer = config.GetValue<string>("Jwt:Issuer");
AppSetting.JwtKey = config.GetValue<string>("Jwt:Key");
AppSetting.TokenExpirationtime = config.GetValue<int>("TokenExpirationtime");
// Add NLog as the logging service
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders(); // Remove other logging providers
    loggingBuilder.SetMinimumLevel(LogLevel.Trace);
});
// Add JWT authentication services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = AppSetting.JwtIssuer,
        ValidAudience = AppSetting.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSetting.JwtKey ?? ""))
    };
});
//Service inject
builder.Services.AddScoped<IMedicationService, MedicationService>();
builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
builder.Services.AddScoped<ITreatmentService, TreatmentService>();
builder.Services.AddScoped<IDiagnosisService, DiagnosisService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IBillingService, BillingService>();
builder.Services.AddScoped<IInsuranceService, InsuranceService>();
builder.Services.AddScoped<ISpecialtyService, SpecialtyService>();
builder.Services.AddScoped<IClinicService, ClinicService>();
builder.Services.AddScoped<INurseService, NurseService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IExaminationService, ExaminationService>();
builder.Services.AddScoped<IRadiologyService, RadiologyService>();
builder.Services.AddScoped<ILabResultService, LabResultService>();
builder.Services.AddScoped<IReferralService, ReferralService>();
builder.Services.AddScoped<IImmunizationService, ImmunizationService>();
builder.Services.AddScoped<IAllergyService, AllergyService>();
builder.Services.AddScoped<IHealthHistoryService, HealthHistoryService>();
builder.Services.AddScoped<IBillableItemsService, BillableItemsService>();
builder.Services.AddScoped<IProcedureCodesService, ProcedureCodesService>();
builder.Services.AddScoped<IDiagnosisCodesService, DiagnosisCodesService>();
builder.Services.AddScoped<IMedicalServicesService, MedicalServicesService>();
builder.Services.AddScoped<IInsuranceCompaniesService, InsuranceCompaniesService>();
builder.Services.AddScoped<IMedicalProvidersService, MedicalProvidersService>();
builder.Services.AddScoped<IVitalSignsService, VitalSignsService>();
builder.Services.AddScoped<IBillingCycleService, BillingCycleService>();
builder.Services.AddScoped<IExplanationOfBenefitsService, ExplanationOfBenefitsService>();
builder.Services.AddScoped<IDenialReasonService, DenialReasonService>();
builder.Services.AddScoped<IPreauthorizationService, PreauthorizationService>();
builder.Services.AddScoped<IAdjustmentService, AdjustmentService>();
builder.Services.AddScoped<IClaimItemService, ClaimItemService>();
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IAgingReportService, AgingReportService>();
builder.Services.AddScoped<IWriteOffsService, WriteOffsService>();
builder.Services.AddScoped<IDunningLettersService, DunningLettersService>();
builder.Services.AddScoped<ICollectionAgencyService, CollectionAgencyService>();
builder.Services.AddScoped<IStatementService, StatementService>();
builder.Services.AddScoped<IRemittanceAdviceService, RemittanceAdviceService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IEntityService, EntityService>();
builder.Services.AddScoped<IRoleEntitlementService, RoleEntitlementService>();
builder.Services.AddScoped<IUserInRoleService, UserInRoleService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
builder.Services.AddScoped<IFieldMapperService, FieldMapperService>();
builder.Services.AddScoped<IJsonMessageService, JsonMessageService>();
builder.Services.AddTransient<ILoggerService, LoggerService>();
//Json handler
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    // Configure Newtonsoft.Json settings here
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
});
//Inject context
builder.Services.AddTransient<LocalProjSept26001Context>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.SetIsOriginAllowed(_ => true)
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LocalProjSept26001 API v1");
        c.RoutePrefix = string.Empty;
    });
    app.MapSwagger().RequireAuthorization();
}
app.UseAuthorization();
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();