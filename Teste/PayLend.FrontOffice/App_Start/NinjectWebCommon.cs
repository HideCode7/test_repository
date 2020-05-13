using FluentValidation;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using PayLend.Business.NinjectModules;
using PayLend.Core.Entities;
using PayLend.Core.Interfaces;
using PayLend.Database;
using PayLend.Financiamento.Models;
using PayLend.Financiamento.Models.ModelValidators;
using PayLend.Financiamento.Models.Modules;
using PayLend.Framework;
using PayLend.FrontOffice.App_Start;
using System;
using System.Web;
using PayLend.Repository.IdentityRpository;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(NinjectWebCommon), "Stop")]

namespace PayLend.FrontOffice.App_Start
{

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            Bootstrapper.Initialize(CreateKernel);
        }

        public static void Stop()
        {
            Bootstrapper.ShutDown();
        }

        private static IKernel CreateKernel()
        {
            var modules = new INinjectModule[]
                {
                    new ModelsModule(),
                    new BusinessModule()
                };
            var kernel = new StandardKernel(modules);

            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            RegisterServices(kernel);
            return kernel;
        }
        private static void RegisterServices(IKernel kernel)
        {
           
            kernel.Bind<IValidator<InvestmentModel>>().To<InvestmentModelValidator>();
            kernel.Bind<IValidator<SuspendFundingModel>>().To<SuspendFundingModelValidator>();
            kernel.Bind<IPayLendUser>().To<Lender>();
            kernel.Bind<IIdentityRepository>()
                .To<IdentityRpository<PayLendContext>>()
                .InRequestScope();

            CompositionRoot.Init(kernel);
        }
    }
}
