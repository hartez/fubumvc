using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Runtime;
using HtmlTags;

namespace FubuMVC.Core.Assets.Tags
{
    public class AssetTagBuilder : IAssetTagBuilder
    {
        private readonly Cache<MimeType, Func<IAssetTagSubject, string, HtmlTag>>
            _builders = new Cache<MimeType, Func<IAssetTagSubject, string, HtmlTag>>();

        private readonly IMissingAssetHandler _missingHandler;

        public AssetTagBuilder(IMissingAssetHandler missingHandler)
        {
            _missingHandler = missingHandler;

            _builders[MimeType.Javascript] = (subject, url) =>
            {
                return new HtmlTag("script")
                    .Attr("type", MimeType.Javascript.Value)
                    .Attr("src", url);
            };

            _builders[MimeType.Css] = (subject, url) =>
            {
                return new HtmlTag("link").Attr("href", url).Attr("rel", "stylesheet").Attr("type", MimeType.Css.Value);
            };
        }

        public IEnumerable<HtmlTag> Build(AssetTagPlan plan)
        {
            var missingSubjects = plan.RemoveMissingAssets();
            var func = _builders[plan.MimeType];
            Func<IAssetTagSubject, HtmlTag> builder = s =>
            {
                var url = AssetContentHandler.DetermineAssetUrl(s);
                return func(s, url);
            };

            var missingTags = _missingHandler.BuildTagsAndRecord(missingSubjects);
            var assetTags = plan.Subjects.Select(builder);
            return missingTags.Union(assetTags); 
        }
    }
}