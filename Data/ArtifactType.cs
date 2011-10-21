using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kCura.Relativity.ImportAPI.Data
{
    public class ArtifactType
    {
        public int ID { get; internal set; }
        public string Name { get; internal set; }

        public override String ToString()
        {
            return Name;
        }
    }
}
