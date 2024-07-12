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
                                    CER.CONCLUSION              Conclusion,
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
                            FROM CERTIFICATE CER
                            JOIN CERTIFICATE_FILE CEF
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

    public async Task<CertificateModel> SaveCertificate(CertificateArgument argument)
    {
        _logger.LogDebug("Start save certificate structure.");

        using IDbConnection connection = GetConnection();
        using IDbTransaction transaction = connection.BeginTransaction();

        try
        {
            int codeCertificate = await SaveCertificate(argument, transaction);

            argument.Disciplines.ForEach(x => x.CodeCertificate = codeCertificate);
            await SaveCertificateDisciplines(argument.Disciplines, transaction);

            argument.Files.ForEach(x => x.CodeCertificate = codeCertificate);;
            await SaveCertificateFile(argument.Files, transaction);

            transaction.Commit();

            return await GetCertificateByRegistration(argument.StudentRegistration);
        }
        catch (Exception)
        {
            transaction.Rollback();
            _logger.LogError("Error on save certificate structure.");
            throw;
        }
        finally
        {
            _logger.LogDebug("Finished save certificate structure.");
        }
    }

    private async Task<int> SaveCertificate(CertificateArgument argument, IDbTransaction transaction)
    {
        _logger.LogDebug("Start save certificate.");
        try
        {
            string query = @"INSERT INTO CERTIFICATE 
                            (
                                STUDENT_NAME, 
                                STUDENT_DOCUMENT, 
                                STUDENT_REGISTRATION, 
                                COURSE, 
                                WORKLOAD, 
                                CONCLUSION,
                                UTILIZATION,
                                SIGN
                            ) VALUES
                            (
                                @StudentName,
                                @StudentDocument,
                                @StudentRegistration,
                                @Course,
                                @Workload,
                                @Conclusion,
                                @Utilization,
                                @Sign
                            ) RETURNING CODE_CERTIFICATE;";

            using IDbConnection connection = GetConnection();
            return await connection.ExecuteScalarAsync<int>(query, argument, transaction);

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

    private async Task SaveCertificateDisciplines(IEnumerable<DisciplineArgument> arguments, IDbTransaction transaction)
    {
        _logger.LogDebug("Start save certificate disciplines.");
        try
        {
            string query = @"INSERT INTO CERTIFICATE_DISCIPLINE
                            (
                                CODE_CERTIFICATE, 
                                DISCIPLINE, 
                                WORKLOAD, 
                                UTILIZATION, 
                                CONCLUSION
                            ) VALUES
                            (
                                @CodeCertificate,
                                @Discipline,
                                @Workload,
                                @Utilization, 
                                @Conclusion
                            )";

            using IDbConnection connection = GetConnection();
            await connection.ExecuteAsync(query, arguments, transaction);
        }
        catch (Exception exception)
        {
            _logger.LogError("Error on save certificate disciplines. {0}", exception.Message);
            throw;
        }
        finally
        {
            _logger.LogDebug("Finished save certificate disciplines.");
        }
    }

    private async Task SaveCertificateFile(List<FileDetailArgument> arguments, IDbTransaction transaction)
    {
        _logger.LogDebug("Start save certificate files.");
        try
        {
            string query = @"INSERT INTO CERTIFICATE_FILE
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
            await connection.ExecuteAsync(query, arguments, transaction);
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
