using System;
using Proxier.Interfaces;

namespace Proxier.Extensions
{
    public class CopyToOptions
    {
        public bool CopyPrivates { get; set; }

        public bool IgnoreNulls { get; set; }

        public Func<ICopyContext, object> Resolver { get; set; } = context => context.Property.GetValue(context.Source);
    }
}