namespace TaskTronic.Drive.Services.DapperRepo
{
    using Dapper;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Threading.Tasks;
    using TaskTronic.Data.Models;
    using TaskTronic.Drive.Models.Files;
    using TaskTronic.Messages.Drive.Files;

    internal class FileDapper : IFileDapper
    {
        private const int DEFAULT_CHUNK_SIZE = 1_048_576; // 1mb
        private const int BUFFER_SIZE = 1024 * 1024;

        private const string BlobsTableName = "[dbo].[Blobsdata]";
        private const string FileTableName = "[dbo].[Files]";
        private const string MessagesTableName = "[dbo].[Messages]";

        private readonly IDbConnectionFactory dbConnectionFactory;

        public FileDapper(IDbConnectionFactory dbConnectionFactory) 
            => this.dbConnectionFactory = dbConnectionFactory;

        public async Task<bool> CreateBlobAsync(InputFileServiceModel file)
        {
            var sql = $@"
                DECLARE @newdata varbinary(max) = @data
                INSERT INTO {BlobsTableName}
                (
                    [EmployeeId],
                    [FileName],
                    [Timestamp],
                    [FileSize],
                    [FinishedUpload],
                    [Data]
                )
                VALUES
                (
                    @employeeId,
                    @fileName,
                    @timestamp,
                    @fileSize,
                    @finishedUpload,
                    @newdata
                )";

            int size;

            try
            {
                size = Convert.ToInt32(file.Stream.Length);
            }
            catch (OverflowException)
            {
                size = DEFAULT_CHUNK_SIZE;
            }

            var dbParams = new DynamicParameters();
            dbParams.Add("@data", file.Stream, DbType.Binary, size: size);
            dbParams.Add("@employeeId", file.EmployeeId, DbType.String);
            dbParams.Add("@fileName", file.FileName, DbType.String);
            dbParams.Add("@fileSize", file.Filesize, DbType.Int64);
            dbParams.Add("@finishedUpload", false, DbType.Boolean);
            dbParams.Add("@timestamp", DateTimeOffset.UtcNow, DbType.DateTimeOffset);

            using var conn = this.dbConnectionFactory.GetSqlConnection;

            return (await conn.ExecuteAsync(sql, dbParams)) > 0;
        }

        public async Task<bool> AppendChunkToBlobAsync(InputFileServiceModel file)
        {
            var sql = $@"
                UPDATE {BlobsTableName} 
                SET Data = Data + @data, 
                    Filesize = Filesize + @fileSize 
                WHERE 
                    EmployeeId = @employeeId 
                    AND FinishedUpload = 0
                    AND FileName = @fileName";

            int size;

            try
            {
                size = Convert.ToInt32(file.Stream.Length);
            }
            catch (OverflowException)
            {
                size = DEFAULT_CHUNK_SIZE;
            }

            var dbParams = new DynamicParameters();
            dbParams.Add("@data", file.Stream, DbType.Binary, size: size);
            dbParams.Add("@employeeId", file.EmployeeId, DbType.Int32);
            dbParams.Add("@fileSize", file.Filesize, DbType.Int64);
            dbParams.Add("@fileName", file.FileName, DbType.String);

            using var conn = this.dbConnectionFactory.GetSqlConnection;

            return (await conn.ExecuteAsync(sql, dbParams)) > 0;
        }

        #region TODO: merge in one method
        public async Task<(int FileId, int MessageId)> SaveCompletedUploadAsync(InputFileServiceModel file, string oldFileName)
        {
            using (var conn = this.dbConnectionFactory.GetSqlConnection)
            {
                conn.Open();

                var transaction = conn.BeginTransaction();

                int insertedFileId;
                int insertedMessageId;

                try
                {
                    // update blobsdata to done
                    var sqlUpdate = $@"
                        UPDATE {BlobsTableName}
                        SET FinishedUpload = 1
                        OUTPUT INSERTED.BlobId
                        WHERE EmployeeId = @employeeId
                            AND FinishedUpload = 0
                            AND FileName = @oldFileName";

                    var employeeId = file.EmployeeId;
                    var fileName = file.FileName;

                    var updatedBlobId = await conn.ExecuteScalarAsync<int>(sqlUpdate, new
                    {
                        employeeId,
                        fileName,
                        oldFileName
                    }, transaction);

                    // insert new file
                    var sqlInsert = $@"
                        INSERT INTO {FileTableName}
                        (
                            [CatalogId],
                            [FolderId],
                            [BlobId],
                            [FileName],
                            [FileType],
                            [FileSize],
                            [ContentType],
                            [EmployeeId],
                            [CreateDate]
                        )
                        VALUES
                        (
                            @CatalogId,
                            @FolderId,
                            @BlobId,
                            @Filename,
                            @Filetype,
                            (SELECT Filesize FROM {BlobsTableName} WHERE BlobId = @BlobId),
                            @ContentType,
                            @EmployeeId,
                            @CreateDate
                        ); SELECT CAST (SCOPE_IDENTITY() AS INT)";

                    file.BlobId = updatedBlobId;
                    file.EmployeeId = employeeId;

                    insertedFileId = (await conn.ExecuteScalarAsync<int>(sqlInsert, file, transaction));

                    // save and send message to all subscribers
                    var sqlMessages = $@"
                        INSERT INTO {MessagesTableName}
                        (
                            [Type],
                            [Published],
                            [serializedData]
                        )
                        VALUES
                        (
                            @Type,
                            @Published,
                            @serializedData
                        ); SELECT CAST (SCOPE_IDENTITY() AS INT)";

                    var messageData = new FileUploadedMessage
                    {
                        FileId = insertedFileId,
                        Name = file.FileName,
                        Type = file.ContentType
                    };

                    var message = new Message(messageData);

                    insertedMessageId = await conn.ExecuteScalarAsync<int>(sqlMessages, new
                    {
                        Type = message.Type.AssemblyQualifiedName,
                        message.Published,
                        message.serializedData
                    }, transaction);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }

                return (insertedFileId, insertedMessageId);
            }
        }

        public async Task<(int FileId, int MessageId)> SaveCompletedUploadAsync(InputFileServiceModel file)
        {
            using (var conn = this.dbConnectionFactory.GetSqlConnection)
            {
                conn.Open();

                var transaction = conn.BeginTransaction();

                int insertedFileId;
                int insertedMessageId;

                try
                {
                    // update blob to done
                    var sqlUpdate = $@"
                        UPDATE {BlobsTableName}
                        SET FinishedUpload = 1
                        OUTPUT INSERTED.BlobId
                        WHERE EmployeeId = @employeeId
                            AND FinishedUpload = 0
                            AND FileName = @fileName";

                    var employeeId = file.EmployeeId;
                    var fileName = file.FileName;

                    var updatedBlobId = await conn.ExecuteScalarAsync<int>(sqlUpdate, new
                    {
                        employeeId,
                        fileName
                    }, transaction);

                    // insert new file
                    var sqlInsert = $@"
                        INSERT INTO {FileTableName}
                        (
                            [CatalogId],
                            [FolderId],
                            [BlobId],
                            [FileName],
                            [FileType],
                            [FileSize],
                            [ContentType],
                            [EmployeeId],
                            [CreateDate]
                        )
                        VALUES
                        (
                            @CatalogId,
                            @FolderId,
                            @BlobId,
                            @Filename,
                            @Filetype,
                            (SELECT Filesize FROM {BlobsTableName} WHERE BlobId = @BlobId),
                            @ContentType,
                            @EmployeeId,
                            @CreateDate
                        ); SELECT CAST (SCOPE_IDENTITY() AS INT)";

                    file.BlobId = updatedBlobId;
                    file.EmployeeId = employeeId;

                    insertedFileId = (await conn.ExecuteScalarAsync<int>(sqlInsert, file, transaction));

                    // save and send message to all subscribers
                    var sqlMessages = $@"
                        INSERT INTO {MessagesTableName}
                        (
                            [Type],
                            [Published],
                            [serializedData]
                        )
                        VALUES
                        (
                            @Type,
                            @Published,
                            @serializedData
                        ); SELECT CAST (SCOPE_IDENTITY() AS INT)";

                    var messageData = new FileUploadedMessage
                    {
                        FileId = insertedFileId,
                        Name = file.FileName,
                        Type = file.ContentType
                    };

                    var message = new Message(messageData);

                    insertedMessageId = await conn.ExecuteScalarAsync<int>(sqlMessages, new
                    {
                        Type = message.Type.AssemblyQualifiedName,
                        message.Published,
                        message.serializedData
                    }, transaction);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }

                return (insertedFileId, insertedMessageId);
            }
        }
        #endregion

        public async Task<IEnumerable<OutputFileServiceModel>> GetFilesByFolderIdAsync(int folderId)
        {
            var sql = $@"
                SELECT * FROM {FileTableName}
                WHERE FolderId = @folderId";

            using var conn = this.dbConnectionFactory.GetSqlConnection;

            return await conn.QueryAsync<OutputFileServiceModel>(sql, new { folderId });
        }

        public async Task<(bool Success, int MessageId)> DeleteFileAsync(int catalogId, int folderId, int fileId, int blobId)
        {
            using (var conn = this.dbConnectionFactory.GetSqlConnection)
            {
                conn.Open();
                var transaction = conn.BeginTransaction();

                int insertedMessageId;

                try
                {
                    var sqlDeleteBlob = $@"
                        DELETE FROM {BlobsTableName}
                        WHERE BlobId = @blobId";

                    var sqlDeleteFileObj = $@"
                        DELETE FROM {FileTableName}
                        WHERE FileId = @fileId
                           AND CatalogId = @CatalogId
                           AND FolderId = @folderId";

                    var deletedFileBlob = (await conn.ExecuteAsync(sqlDeleteBlob, new
                    {
                        blobId
                    }, transaction)) == 1;

                    var deletedFile = (await conn.ExecuteAsync(sqlDeleteFileObj, new
                    {
                        fileId,
                        catalogId,
                        folderId
                    }, transaction)) == 1;

                    if (deletedFileBlob && deletedFile)
                    {
                        // save and send message to all subscribers
                        var sqlMessages = $@"
                            INSERT INTO {MessagesTableName}
                            (
                                [Type],
                                [Published],
                                [serializedData]
                            )
                            VALUES
                            (
                                @Type,
                                @Published,
                                @serializedData
                            ); SELECT CAST (SCOPE_IDENTITY() AS INT)";

                        var messageData = new FileDeletedMessage
                        {
                            FileId = fileId
                        };

                        var message = new Message(messageData);

                        insertedMessageId = await conn.ExecuteScalarAsync<int>(sqlMessages, new
                        {
                            Type = message.Type.AssemblyQualifiedName,
                            message.Published,
                            message.serializedData
                        }, transaction);

                        transaction.Commit();
                        return (true, insertedMessageId);
                    }
                    else
                    {
                        transaction.Rollback();
                        return (false, 0);
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        //https://stackoverflow.com/a/2101447/7588131
        public async Task ReadStreamFromFileAsync(int blobId, Stream stream)
        {
            var sql = $@"
                SELECT [Data]
                FROM {BlobsTableName}
                WHERE BlobId = @blobId";

            using var conn = this.dbConnectionFactory.GetSqlConnection;
            using var reader = await conn.ExecuteReaderAsync(sql, new { blobId });

            while (reader.Read())
            {
                var buffer = new byte[BUFFER_SIZE];
                var dataIndex = 0L;

                long bytesRead;
                while ((bytesRead = reader.GetBytes(0, dataIndex, buffer, 0, buffer.Length)) > 0)
                {
                    await stream.WriteAsync(buffer, 0, (int)bytesRead);
                    dataIndex += bytesRead;
                }
            }
        }

        public async Task<OutputFileDownloadServiceModel> GetFileInfoForDownloadAsync(int fileId)
        {
            var sql = $@"
                SELECT BlobId, Filename, Filesize, Filetype, ContentType
                FROM {FileTableName} 
                WHERE FileId = @fileId";

            using var conn = this.dbConnectionFactory.GetSqlConnection;

            return await conn.QuerySingleOrDefaultAsync<OutputFileDownloadServiceModel>(sql, new { fileId });
        }

        public async Task<IEnumerable<OutputFileSearchServiceModel>> SearchFilesAsync(
            int catalogId, 
            string value, 
            IEnumerable<int> accessibleFolders)
        {
            var sql = $@"
                SELECT * FROM {FileTableName}
                WHERE FolderId IN @accessibleFolders
                    AND CHARINDEX(@value, Filename) > 0 
                    OR CHARINDEX(@value, FileType) > 0
                    AND CatalogId = @catalogId";

            using var db = this.dbConnectionFactory.GetSqlConnection;

            return await db.QueryAsync<OutputFileSearchServiceModel>(sql, new 
            { 
                catalogId, 
                accessibleFolders,
                value
            });
        }

        public async Task<int> DoesFileWithSameNameExistInFolder(int catalogId, int folderId, string fileName, string fileType)
        {
            var sql = $@"
                SELECT FileId FROM {FileTableName} 
                WHERE Filename LIKE @fileName
                    AND Filetype LIKE @fileType
                    AND CatalogId = @CatalogId
                    AND FolderId = @folderId";

            using var db = this.dbConnectionFactory.GetSqlConnection;

            return (await db.QuerySingleOrDefaultAsync<int>(sql, new { catalogId, folderId, fileName, fileType }));
        }

        public async Task<int> CountFilesForEmployeeAsync(int employeeId)
        {
            var sql = $@"
                SELECT COUNT(*) FROM {FileTableName}
                WHERE EmployeeId = @employeeId";

            using var db = this.dbConnectionFactory.GetSqlConnection;

            return await db.ExecuteScalarAsync<int>(sql, new { employeeId });
        }

        public async Task<IEnumerable<FileContract>> GetFileNamesAndTypesAsync(int folderId)
        {
            var sql = $@"
                SELECT FileName, FileType 
                FROM {FileTableName}
                WHERE FolderId = @folderId";

            using var conn = this.dbConnectionFactory.GetSqlConnection;

            return await conn.QueryAsync<FileContract>(sql, new { folderId });
        }
    }
}
