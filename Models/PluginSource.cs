using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruminoid.PluginManager.Models
{
    public sealed class PluginSource
    {
        #region Current

        public static PluginSource CurrentWebSource { get; set }

        public static PluginSource CurrentLocalSource { get; set }

        #endregion
    }
}
