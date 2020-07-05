using System;
using MetadataExtractor;

namespace PhotoOrganizerTest.Models
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