namespace AJT.Contracts
{
    public interface IAJTConfigurator
    {
        /// <summary>
        /// Provide custom password hashing utility implementing <see cref="IPasswordHasher"/> interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IAJTConfigurator UsePasswordHashing<T>() where T : class, IPasswordHasher;
        
        /// <summary>
        /// Finds all roles used in controllers, and adds them to DB. Ignores ATJOptions.Roles parameter
        /// </summary>
        /// <returns></returns>
        IAJTConfigurator AutomaticallyDetectRoles();
        
        /// <summary>
        /// Ensures defined roles match exactly those in DB, if not repopulates tables to match new role structure
        /// This process includes changing existing user roles to newly defined roles;
        /// </summary>
        /// <returns></returns>
        IAJTConfigurator UseRoleBootstrapper();
        
        /// <summary>
        /// Migrates database to ensure correct DB structure. 
        /// WARNING!!!! CAN REMOVE RECORDS IN CASE OF SOME CHANGES
        /// </summary>
        /// <returns></returns>
        IAJTConfigurator MigrateDatabase();

        /// <summary>
        /// Configure additional data to be stored inside tokens, can be access by context.Items["AJT"]
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        IAJTConfigurator AddDataToToken(Func<Guid, IServiceProvider, Task<object>> func);
    }
}
