namespace WinCrypt
{
    /// <summary>
    /// Required namespaces
    /// </summary>
    #region Namespaces
    using Ninject;
    using System;
    #endregion

    /// <summary>
    /// Contains extensions for Ninject
    /// </summary>
    public static class KernelExtensions
    {
        public static IK Construct<IK>(this IK K) where IK: IKernel
        {
            // Check if K is null
            if (K == null) throw new ArgumentNullException();

            // Kernel constant class bindings
            K.Bind<ApplicationViewModel>().ToConstant(
                                                        new ApplicationViewModel());


            // Return the constructed Kernel
            return K;
        }

    }
}
