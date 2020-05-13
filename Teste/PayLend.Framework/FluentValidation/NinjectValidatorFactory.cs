using FluentValidation;
using Ninject;
using System;

namespace PayLend.Framework.FluentValidation
{
    public class NinjectValidatorFactory : ValidatorFactoryBase, IValidatorFactory
    {
        private IKernel ninjectKernel;

        public NinjectValidatorFactory(IKernel ninjectKernel)
        {
            this.ninjectKernel = ninjectKernel;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            try
            {
                return ninjectKernel.Get(validatorType) as IValidator;
            }
            catch (Exception)
            {
                return new ObjectValidator();
            }
        }
    }

    internal class ObjectValidator : AbstractValidator<object>
    {
        public ObjectValidator()
        {

        }
    }
}
