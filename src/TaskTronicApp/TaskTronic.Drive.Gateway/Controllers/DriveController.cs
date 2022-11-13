namespace TaskTronic.Drive.Gateway.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Drive.Folders;
    using Services.Drive;
    using Services.FolderViews;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TaskTronic.Controllers;

    [Authorize]
    public class DriveController : ApiController
    {
        private readonly IFolderService driveService;
        private readonly IFolderViewService folderViewService;
        private readonly IMapper mapper;

        public DriveController(
            IFolderService driveService,
            IFolderViewService folderViewService,
            IMapper mapper)
        {
            this.driveService = driveService;
            this.folderViewService = folderViewService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route(nameof(MyFolders))]
        public async Task<ActionResult<IReadOnlyCollection<OutputMineFolderFlatModel>>> MyFolders()
        {
            var mineFolders = await this.driveService.Mine();

            var mineFoldersIds = mineFolders.Select(f => f.FolderId);

            var mineFolderViews = await this.folderViewService.TotalViews(mineFoldersIds);

            var folderResults = new List<OutputMineFolderFlatModel>();

            foreach (var folderView in mineFolderViews)
            {
                var flatFolder = mineFolders.FirstOrDefault(f => f.FolderId == folderView.FolderId);

                if (flatFolder != null)
                {
                    var outputFolder = this.mapper.Map<OutputFolderFlatServiceModel, OutputMineFolderFlatModel>(flatFolder);

                    outputFolder.TotalViews = folderView.TotalViews;

                    folderResults.Add(outputFolder);
                }
            }

            return folderResults;
        }
    }
}
