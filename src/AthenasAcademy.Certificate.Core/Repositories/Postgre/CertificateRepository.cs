using System.Data;
using AthenasAcademy.Certificate.Core.Arguments;
using AthenasAcademy.Certificate.Core.Configurations.Settings;
using AthenasAcademy.Certificate.Core.Models;
using AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;
using Dapper;
using Microsoft.Extensions.Options;

namespace AthenasAcademy.Certificate.Core;

public class CertificateRepository : BaseRepository, ICertificateRepository
{
    private readonly ILogger<CertificateRepository> _logger;


    public CertificateRepository(
        IOptions<PostgreSettings> settings,
        ILogger<CertificateRepository> logger) : base(settings.Value)
    {
        _logger = logger;
    }

    public async Task<CertificateModel> GetCertificateByRegistration(string registration)
    {
        _logger.LogDebug("Start get certificate by registration.");
        try
        {
            string query = @"SELECT CER.CODE_CERTIFICATE        CodeCertificate,
                                    CER.Student_NAME            StudentName,
                                    CER.Student_DOCUMENT        StudentDocument,
                                    CER.Student_REGISTRATION    StudentRegistration,
                                    CER.COURSE                  Course,
                                    CER.WORKLOAD                Workload,
                                    CER.COMPLETATION            Completation,
                                    CER.UTILIZATION             Utilization,
                                    CER.CREATED_AT              CreatedAt,
                                    CER.UPDATED_AT              UpdatedAt,
                                    CEF.CODE_CERTIFICATE_FILE   CodeCertificateFile,
                                    CEF.FILE                    File,
                                    CEF.TYPE                    Type,
                                    CEF.PATH                    Path,
                                    CEF.SIZE                    Size,
                                    CEF.CREATED_AT              CreatedAt,
                                    CEF.UPDATED_AT              UpdatedAt
                            FROM CERTIFICATES CER
                            JOIN CERTIFICATE_FILES CEF 
                              ON CEF.CODE_CERTIFICATE = CER.CODE_CERTIFICATE
                           WHERE CER.Student_REGISTRATION = @Registration";

            Dictionary<int, CertificateModel> certificateDict = [];

            using IDbConnection connection = GetConnection();
            return (await connection.QueryAsync<CertificateModel>(
                sql: query,
                param: new { registration },
                transaction: null,
                buffered: true,
                splitOn: "CodeCertificateFile",
                types: [typeof(CertificateModel), typeof(FileDetailModel)],
                map: obj =>
                    {
                        CertificateModel temp;
                        CertificateModel certificate = obj[0] as CertificateModel;
                        FileDetailModel fileDetail = obj[1] as FileDetailModel;

                        if (!certificateDict.TryGetValue(certificate.CodeCertificate, out temp))
                        {
                            temp = certificate;
                            certificateDict.Add(certificate.CodeCertificate, temp);
                        }

                        if (fileDetail != null)
                        {
                            temp.Files = [];
                            temp.Files.Add(fileDetail);
                        }

                        return temp;
                    }
            )).FirstOrDefault();
        }
        catch (Exception exception)
        {
            _logger.LogError("Error on get certificate by registration. {0}", exception.Message);
            throw;
        }
        finally
        {
            _logger.LogDebug("Finished get certificate by registration.");
        }
    }

    public async Task<CertificateModel> SaveCertificate(CertificateArgument argumet)
    {
        _logger.LogDebug("Start save certificate.");
        try
        {
            string query = @"INSERT INTO CERTIFICATES 
                            (
                                Student_NAME, 
                                Student_DOCUMENT, 
                                Student_REGISTRATION, 
                                COURSE, 
                                COMPLETATION, 
                                UTILIZATION
                            ) VALUES
                            (
                                @StudentName,
                                @StudentDocument,
                                @StudentRegistration,
                                @Course, 
                                @Completion,
                                @Utilization
                            ) RETURNING CODE_CERTIFICATE;";

            using IDbConnection connection = GetConnection();
            using IDbTransaction transaction = connection.BeginTransaction();

            int codeCertificate = await connection.ExecuteScalarAsync<int>(query, transaction);

            argumet.FileDetails.ForEach(file => file.CodeCertificate = codeCertificate);

            await SaveCertificateFiles(argumet.FileDetails, codeCertificate, transaction);

            transaction.Commit();

            return await GetCertificateByRegistration(argumet.StudentRegistration);
        }
        catch (Exception exception)
        {
            _logger.LogError("Error on save certificate. {0}", exception.Message);
            throw;
        }
        finally
        {
            _logger.LogDebug("Finished save certificate.");
        }
    }

    private async Task SaveCertificateFiles(IEnumerable<FileDetailArgument> arguments, int codeCertificate, IDbTransaction transaction)
    {
        _logger.LogDebug("Start save certificate files.");
        try
        {
            string query = @"INSERT INTO CERTIFICATE_FILES 
                            (
                                CODE_CERTIFICATE, 
                                FILE, 
                                TYPE, 
                                PATH, 
                                SIZE
                            ) VALUES
                            (
                                @CodeCertificate,
                                @File,
                                @Type,
                                @Path, 
                                @Size
                            );";

            using IDbConnection connection = GetConnection();
            await connection.ExecuteAsync(query, arguments);
        }
        catch (Exception exception)
        {
            _logger.LogError("Error on save certificate files. {0}", exception.Message);
            throw;
        }
        finally
        {
            _logger.LogDebug("Finished save certificate files.");
        }
    }
}
