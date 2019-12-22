using punkOptimise.Extensions;
using punkOptimise.Interfaces;
using punkOptimise.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace punkOptimise.Controllers
{
    [PluginController("punkOptimise")]
    public class OptimiseController : UmbracoAuthorizedJsonController
    {
        private readonly IMediaService _mediaService;
        private readonly IMediaFileSystem _mediaFileSystem;
        private readonly IImageShrinkService _shrinkService;
        private readonly IImageReductionService _imageService;
        private readonly IContentTypeBaseServiceProvider _contentTypeBaseServiceProvider;
        private readonly List<string> _validFileExtensions;

        private readonly string _reduceFileExtensions;
        private readonly string _shrinkfileExtensions;

        /*
         *     <add key="punkOptimise:ReduceFileExtensions" value="jpg,jpeg" />
         *     <add key="punkOptimise:ShrinkfileExtensions" value="png" />
         *     <add key="punkOptimise:DefaultQuality" value="70" />
         *     <add key="punkOptimise:TinyPng:ApiUrl" value="https://api.tinify.com/shrink" />
         *     <add key="punkOptimise:TinyPng:ApiKey" value="" />
         */

        public OptimiseController(
            IMediaService mediaService, 
            IImageShrinkService shrinkService,
            IImageReductionService imageService,
            IContentTypeBaseServiceProvider contentTypeBaseServiceProvider,
            IMediaFileSystem mediaFileSystem)
        {
            _mediaService = mediaService;
            _mediaFileSystem = mediaFileSystem;
            _contentTypeBaseServiceProvider = contentTypeBaseServiceProvider;
            _shrinkService = shrinkService;
            _imageService = imageService;

            _reduceFileExtensions = ConfigurationManager.AppSettings["punkOptimise:ReduceFileExtensions"]?.ToString();
            _shrinkfileExtensions = ConfigurationManager.AppSettings["punkOptimise:ShrinkfileExtensions"]?.ToString();

            _validFileExtensions = new List<string>();
            _validFileExtensions.AddRange(_reduceFileExtensions?.SplitToList());

            if (_shrinkService.IsEnabled())
                _validFileExtensions.AddRange(_shrinkfileExtensions?.SplitToList());
        }

        [HttpGet]
        public OptimiseResponse IsValid(int id)
        {
            var media = _mediaService.GetById(id);
            if (media == null) return new OptimiseResponse("Media Not Found") { ResultType = Enums.ResultType.Error };

            string path = media.GetUrl(StaticValues.Properties.UmbracoFile, Logger);
            string extension = _mediaFileSystem.GetExtension(path)?.Substring(1);

            if (_validFileExtensions.Contains(extension) && media.ContentType.Alias == StaticValues.DocumentTypes.Image && (Reduce(extension) || Shrink(extension)))            
                return new OptimiseResponse(string.Empty) { ResultType = Enums.ResultType.Success };            

            return new OptimiseResponse("Media Not Valid") { ResultType = Enums.ResultType.Error };
        }

        [HttpPost]
        public OptimiseResponse Save(SaveModel model)
        {
            var media = _mediaService.GetById(model.Id);
            if (media == null) return new OptimiseResponse("Media Not Found") { ResultType = Enums.ResultType.Error };

            string path = media.GetUrl(StaticValues.Properties.UmbracoFile, Logger);
            string fullPath = _mediaFileSystem.GetFullPath(path);
            string extension = _mediaFileSystem.GetExtension(path)?.Substring(1);
            bool updated = false;

            byte[] data = null;
            using (Stream inStream = _mediaFileSystem.OpenFile(fullPath))
            {
                if (Reduce(extension))
                {
                    data = _imageService.ReduceQuality(inStream);
                    updated = true;
                }

                if (Shrink(extension))
                {
                    data = _shrinkService.ShrinkImage(inStream);
                    updated = true;
                }
            }

            if (data == null || updated)
            {
                using (var outStream = new MemoryStream(data))
                {
                    outStream.Position = 0;
                    try
                    {
                        media.SetValue(_contentTypeBaseServiceProvider, StaticValues.Properties.UmbracoFile, Path.GetFileName(path), outStream);
                        _mediaService.Save(media);                    
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(typeof(OptimiseController), "Save", ex);
                        throw;
                    }
                }

                return new OptimiseResponse("Media optimised without any errors") { ResultType = Enums.ResultType.Success };
            }
            else
                return new OptimiseResponse("Media optimising was unsuccessful") { ResultType = Enums.ResultType.Error };
        }

        public bool Reduce(string extension) => _reduceFileExtensions.SplitToList()?.Contains(extension) ?? false;

        public bool Shrink(string extension) => _shrinkfileExtensions.SplitToList()?.Contains(extension) ?? false;
    }
}
 