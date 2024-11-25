﻿using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public class ConfigurationOptions
{
    internal SecurityOptions Security { get; set; }

    internal LoggingOptions Logging { get; set; } = new();

    /// <summary>
    /// Adds SecurityServices to the host builder services. Main entry point for customizing Endatix's Security startup settings
    /// </summary>
    /// <param name="options">Security options delegate for adding config actions</param>
    /// <returns>ConfigurationOptions for the SecurityServices</returns>
    public ConfigurationOptions AddSecurityServices(Action<SecurityOptions> options)
    {
        Security = new SecurityOptions();
        options.Invoke(Security);
        return this;
    }

    public class SecurityOptions
    {
        internal bool EnableApiAuthentication { get; set; } = false;

        internal bool EnableDevUsersFromConfig { get; set; } = false;

        /// <summary>
        /// Call this method to add Authentication for the Endatix API using default SecuritySettings
        /// </summary>
        /// <exception cref="ArgumentNullException">If configurationManager is null</exception>
        /// <param name="configurationManager">Pass ConfigurationManager <see cref="Builder.Configuration"/></param>
        /// <returns>modified SecurityOptions</returns>
        public SecurityOptions AddApiAuthentication(ConfigurationManager configurationManager)
        {
            Guard.Against.Null(configurationManager);

            EnableApiAuthentication = true;
            return this;
        }

        /// <summary>
        /// Call this method to add Authentication for the Endatix API
        /// </summary>
        /// <exception cref="ArgumentNullException">If securitySection is null</exception>
        /// <param name="securitySection">Custom config section storing the security options. For detailed structure check <see cref="SecuritySettings"/></param>
        /// <returns>modified SecurityOptions</returns>
        public SecurityOptions AddApiAuthentication(IConfigurationSection securitySection)
        {
            Guard.Against.Null(securitySection);

            EnableApiAuthentication = true;
            return this;
        }
    }

    public class LoggingOptions
    {

    }
}
