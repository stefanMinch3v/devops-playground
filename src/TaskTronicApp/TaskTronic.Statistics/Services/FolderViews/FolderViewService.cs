namespace TaskTronic.Statistics.Services.FolderViews
{
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using TaskTronic.Statistics.Models.FolderViews;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TaskTronic.Services;

    public class FolderViewService : IFolderViewService
    {
        private readonly StatisticsDbContext dbContext;

        public FolderViewService(StatisticsDbContext dbContext) 
            => this.dbContext = dbContext;

        public async Task AddViewAsync(int folderId, string userId)
        {
            if (folderId < 1 || string.IsNullOrEmpty(userId))
            {
                return;
            }

            var folderView = new FolderView { UserId = userId, FolderId = folderId };
            this.dbContext.FolderViews.Add(folderView);

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<int> GetTotalViews(int folderId)
            => await this.dbContext.FolderViews
                .CountAsync(v => v.FolderId == folderId);

        public async Task<IReadOnlyCollection<OutputFolderViewServiceModel>> GetTotalViews(IEnumerable<int> ids)
            => await this.dbContext.FolderViews
                .Where(v => ids.Contains(v.FolderId))
                .GroupBy(v => v.FolderId)
                .Select(gr => new OutputFolderViewServiceModel
                {
                    FolderId = gr.Key,
                    TotalViews = gr.Count()
                })
                .ToListAsync();
    }
}
