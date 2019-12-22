using punkOptimise.Interfaces;
using punkOptimise.Services;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Services;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;

namespace punkOptimise.Composers
{
    public class PunkOptimiseComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IImageReductionService, ImageProcessorService>(Lifetime.Request);
            composition.Register<IImageShrinkService, TinyPngShrinkService>(Lifetime.Request);
            composition.Components().Append<OptimiseMenuItemComponent>();
        }
    }

    public class OptimiseMenuItemComponent : IComponent
    {
        public void Initialize()
        {
            TreeControllerBase.MenuRendering += TreeControllerBase_MenuRendering;
        }

        void TreeControllerBase_MenuRendering(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            if (int.TryParse(e.NodeId, out int nodeId))
                switch (sender.TreeAlias)
                {
                    case Constants.Trees.Media:
                        if (nodeId != 0 &&
                            nodeId != Constants.System.RecycleBinMedia &&
                            nodeId != Constants.System.Root)
                        {
                            var textService = DependencyResolver.Current.GetService<ILocalizedTextService>();

                            var optimise = new MenuItem("optimiseNode", textService?.Localize(StaticValues.Lang.Optimise.OptimiseImage));
                            optimise.AdditionalData.Add("actionView", "/App_Plugins/punkOptimise/backoffice/optimise/optimise.html");
                            optimise.Icon = "arrow-down color-blue";
                            e.Menu.Items.Add(optimise);
                        }
                        break;
                }
        }

        public void Terminate() { }
    }
}