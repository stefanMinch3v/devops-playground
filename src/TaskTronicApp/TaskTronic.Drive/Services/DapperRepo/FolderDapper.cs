namespace TaskTronic.Drive.Services.DapperRepo
{
    using Dapper;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TaskTronic.Data.Models;
    using TaskTronic.Drive.Models.Folders;
    using TaskTronic.Messages.Drive.Folders;

    internal class FolderDapper : IFolderDapper
    {
        private const string FolderTableName = "[dbo].[Folders]";
        private const string PermissionsTableName = "[dbo].[Permissions]";
        private const string FileTableName = "[dbo].[Files]";
        private const string MessagesTableName = "[dbo].[Messages]";

        private readonly IDbConnectionFactory dbConnectionFactory;

        public FolderDapper(IDbConnectionFactory dbConnectionFactory)
            => this.dbConnectionFactory = dbConnectionFactory;

        public async Task<(int FolderId, int MessageId)> CreateFolderAsync(InputFolderServiceModel inputModel)
        {
            using var db = this.dbConnectionFactory.GetSqlConnection;
            db.Open();

            var transaction = db.BeginTransaction();

            int insertedFolderId;
            int insertedMessageId;

            try
            {
                // insert folder
                var sql = $@"
                    INSERT INTO {FolderTableName}
                    (
                        CatalogId,
                        ParentId,
                        RootId,
                        Name,
                        IsPrivate,
                        EmployeeId,
                        CreateDate
                    )
                    VALUES
                    (
                        @CatalogId,
                        @ParentId,
                        @RootId,
                        @Name,
                        @IsPrivate,
                        @EmployeeId,
                        @CreateDate
                    ); SELECT CAST (SCOPE_IDENTITY() AS INT)";

                insertedFolderId = await db.ExecuteScalarAsync<int>(sql, inputModel, transaction);

                // save message
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

                var messageData = new FolderCreatedMessage
                {
                    FolderId = insertedFolderId
                };

                var message = new Message(messageData);

                insertedMessageId = await db.ExecuteScalarAsync<int>(sqlMessages, new
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

            return (insertedFolderId, insertedMessageId);
        }

        public async Task<int> GetFolderNumbersWithExistingNameAsync(string name, int parentFolderId)
        {
            var sql = $@"
                SELECT COUNT(*) FROM {FolderTableName}
                WHERE ParentId = @parentFolderId
                    AND [Name] LIKE CONCAT('%', @name, '%')";

            using var db = this.dbConnectionFactory.GetSqlConnection;

            return await db.ExecuteScalarAsync<int>(sql, new { name, parentFolderId });
        }

        public async Task<OutputFolderServiceModel> GetFolderByIdAsync(int folderId)
        {
            var sql = $@"
                SELECT * FROM {FolderTableName}
                WHERE FolderId = @folderId";

            using var conn = this.dbConnectionFactory.GetSqlConnection;

            return await conn.QuerySingleOrDefaultAsync<OutputFolderServiceModel>(sql, new { folderId });
        }

        public async Task<OutputFolderServiceModel> GetRootFolderByCatalogIdAsync(int catalogId)
        {
            var sql = $@"
                SELECT * FROM {FolderTableName}
                WHERE CatalogId = @CatalogId
                    AND ParentId IS NULL
                    AND RootId IS NULL";

            using var conn = this.dbConnectionFactory.GetSqlConnection;

            return await conn.QuerySingleOrDefaultAsync<OutputFolderServiceModel>(sql, new { catalogId });
        }

        public async Task<IEnumerable<OutputFolderWithAccessServiceModel>> GetFolderTreeAsync(int folderId, int employeeId)
        {
            var sql = $@"
                SELECT 
	                f.*,
	                FileCount = (SELECT COUNT(*) FROM {FileTableName} containedFile WHERE containedFile.FolderId = f.FolderId),
	                FolderCount = (SELECT COUNT(*) FROM {FolderTableName} containedFolder WHERE containedFolder.ParentId = f.FolderId),
	                HasAccess = 
		                CASE WHEN (
			                f.IsPrivate = 0 
			                OR EXISTS (
				                SELECT * 
				                FROM {PermissionsTableName} p 
				                WHERE 
					                p.FolderId = f.FolderId
					                AND p.EmployeeId = @employeeId
			                )
		                )
		                THEN 1
		                ELSE 0
		                END
                FROM {FolderTableName} f
                WHERE 
	                ( 
                        f.RootId = @folderId
                        OR f.FolderId = @folderId
                    )";

            using var conn = this.dbConnectionFactory.GetSqlConnection;

            return await conn.QueryAsync<OutputFolderWithAccessServiceModel>(sql, new { folderId, employeeId });
        }

        public async Task<IEnumerable<OutputFolderServiceModel>> GetSubFoldersAsync(int folderId)
        {
            var sql = $@"
                SELECT * FROM {FolderTableName} 
                WHERE ParentId = @folderId";

            using var conn = this.dbConnectionFactory.GetSqlConnection;

            return (await conn.QueryAsync<OutputFolderServiceModel>(sql, new { folderId })).AsList();
        }

        public async Task<bool> IsFolderPrivateAsync(int folderId)
        {
            var sql = $@"
                SELECT IsPrivate FROM {FolderTableName}
                WHERE FolderId = @folderId";

            using var conn = this.dbConnectionFactory.GetSqlConnection;

            return (await conn.QuerySingleOrDefaultAsync<int>(sql, new { folderId })) > 0;
        }

        public async Task<(bool Success, int MessageId)> DeleteAsync(int catalogId, int folderId)
        {
            using var db = this.dbConnectionFactory.GetSqlConnection;
            db.Open();
            var transaction = db.BeginTransaction();

            int insertedMessageId;

            try
            {
                // delete folder
                var sql = $@"
                    DELETE FROM {FolderTableName}
                    WHERE FolderId = @folderId
                        AND CatalogId = @CatalogId";

                await db.ExecuteAsync(sql, new { folderId, catalogId }, transaction);

                // save message
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

                var messageData = new FolderDeletedMessage
                {
                    FolderId = folderId
                };

                var message = new Message(messageData);

                insertedMessageId = await db.ExecuteScalarAsync<int>(sqlMessages, new
                {
                    Type = message.Type.AssemblyQualifiedName,
                    message.Published,
                    message.serializedData
                }, transaction);

                transaction.Commit();

                return (true, insertedMessageId);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<int> CountFoldersForEmployeeAsync(int employeeId)
        {
            var sql = $@"
                SELECT COUNT(*) FROM {FolderTableName}
                WHERE EmployeeId = @employeeId"; ;

            using var db = this.dbConnectionFactory.GetSqlConnection;
            
            return await db.ExecuteScalarAsync<int>(sql, new { employeeId });
        }

        public async Task<IReadOnlyCollection<OutputFolderFlatServiceModel>> GetAllFlatForEmployeeAsync(int employeeId)
        {
            var sql = $@"
                SELECT FolderId, [Name], IsPrivate, CreateDate FROM {FolderTableName}
                WHERE EmployeeId = @employeeId";

            using var db = this.dbConnectionFactory.GetSqlConnection;

            return (await db.QueryAsync<OutputFolderFlatServiceModel>(sql, new { employeeId })).AsList();
        }

        public async Task<int?> GetRootFolderIdAsync(int folderId)
        {
            var sql = $@"
                SELECT RootId FROM {FolderTableName}
                WHERE FolderId = @folderId";

            using var conn = this.dbConnectionFactory.GetSqlConnection;

            return await conn.QuerySingleOrDefaultAsync<int?>(sql, new { folderId });
        }

        public async Task<IEnumerable<OutputFolderSearchServiceModel>> GetAllForSearchAsync(int catalogId, int? rootFolderId)
        {
            var sql = $@"
                SELECT * FROM {FolderTableName}
                WHERE CatalogId = @catalogId
                    AND (RootId = @rootFolderId OR RootId IS NULL)
                ORDER BY FolderId ASC";

            using var conn = this.dbConnectionFactory.GetSqlConnection;

            return await conn.QueryAsync<OutputFolderSearchServiceModel>(sql, new
            {
                catalogId,
                rootFolderId
            });
        }

        public async Task<OutputFolderFlatServiceModel> GetFolderFlatByIdAsync(int folderId)
        {
            var sql = $@"
                SELECT FolderId, [Name], IsPrivate, CreateDate 
                FROM {FolderTableName}
                WHERE FolderId = @folderId";

            using var db = this.dbConnectionFactory.GetSqlConnection;

            return await db.QueryFirstOrDefaultAsync<OutputFolderFlatServiceModel>(sql, new { folderId });
        }

        public async Task<OutputFolderCatalogServiceModel> GetFolderCatalogAsync(int folderId)
        {
            var sql = $@"
                SELECT FolderId, CatalogId 
                FROM {FolderTableName}
                WHERE FolderId = @folderId";

            using var conn = this.dbConnectionFactory.GetSqlConnection;

            return await conn.QuerySingleOrDefaultAsync<OutputFolderCatalogServiceModel>(sql, new { folderId });
        }
    }
}
