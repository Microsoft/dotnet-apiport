﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Versioning;

namespace Microsoft.Fx.Portability.ObjectModel
{
    [DataContract]
    public sealed class AnalyzeResponseV1
    {
        public AnalyzeResponseV1() 
        { }

        public AnalyzeResponseV1(AnalyzeResponse response)
        {
            MissingDependencies = response.MissingDependencies.Select(m => new MemberInfoV1(m)).ToList();
            UnresolvedUserAssemblies = response.UnresolvedUserAssemblies;
            Targets = response.Targets;
            SubmissionId = response.SubmissionId;
        }

        [DataMember]
        public IList<MemberInfoV1> MissingDependencies { get; set; }

        [DataMember]
        public IList<string> UnresolvedUserAssemblies { get; set; }

        [DataMember]
        public IList<FrameworkName> Targets { get; set; }

        [DataMember]
        public string SubmissionId { get; set; }
    }
}
