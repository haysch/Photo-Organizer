using System;
using MetadataExtractor;

namespace PhotoOrganizerLib.Tests.Models
{
    public class TestDirectory : Directory
    {
        public override string Name => "TEST";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            throw new NotImplementedException();
        }
    }
}