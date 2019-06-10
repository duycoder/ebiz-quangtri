using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.FwCore.Factory;

namespace Web.Custom
{
    /// <summary>
    /// NAMDV - 24/01/2014
    /// This class use for DI configuration
    /// </summary>
    public class DIBinding
    {
        public static void AddBinding(IKernel kernel)
        {
            //kernel.Bind<IValueCalculator>().To<LinqValueCalculator>();
        }
    }
}