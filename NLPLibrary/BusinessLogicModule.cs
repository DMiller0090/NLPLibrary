using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NLPLibrary
{
    public class BusinessLogicModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NLP>().As<IProperCaseFormatter>();
            builder.RegisterType<AcronymDict>().As<IReadable>();
            builder.RegisterType<AcronymFileDetector>().As<IAcronymDetector>();
        }
    }
}
